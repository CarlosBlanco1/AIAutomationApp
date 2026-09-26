using Amazon.S3;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

[AutomaticRetry(Attempts = 0)]
public class DocumentProcessingJob(IDocumentRepository documentService,
    IWorkspaceRepository workspaceRepository,
    IFileStorageService storageService,
    ITextExtractorService textExtractorService,
    IChunkRepository chunkRepository,
    IChatService chatService,
    IBackgroundJobClient backgroundJobs,
    IHubContext<ChatHub> hubContext,
    IMapper mapper,
    ILogger<DocumentProcessingJob> logger)
{
    [Queue("document-processing")]
    public async Task ProcessAsync(Guid documentId, CancellationToken cancellationToken)
    {
        // Load document
        var newDoc = await documentService.GetDocumentByIdAsync(documentId);

        if (newDoc == null)
        {
            logger.LogInformation("Document : {DocumentId} does not exist and cannot be processed.", documentId);
            return;
        }

        var documentWorkspace = await workspaceRepository.GetWorkspaceByIdAsync(newDoc.WorkspaceId);

        if (documentWorkspace == null)
        {
            logger.LogInformation("Workspace for document : {DocumentId} doesn't exist", documentId);
            return;
        }

        try
        {
            logger.LogInformation("Document fetching succesful! starting processing for doc : {documentId}", documentId);
            // Pending to Processing
            if (!await documentService.TryMarkProcessingAsync(documentId, cancellationToken))
            {
                logger.LogInformation("Skipping document processing for document {documentId}, no longer pending.", documentId);
                return;
            }

            newDoc = await documentService.GetDocumentByIdAsync(documentId);

            // Notify user document is now being processed
            try
            {
                await hubContext.Clients
                .Group($"user:{documentWorkspace!.OwnerId}")
                .SendAsync("DocumentProcessingUpdated", documentId, cancellationToken);
            }
            catch(OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                logger.LogError(e, "SignalR notification for document set to processing state failed for document : {DocumentId}", documentId);
            }

            if (newDoc!.BlobKey is null) { throw new Exception($"BlobKey for document : {documentId} is null!"); }

            // Fetch file
            using var response = await storageService.GetFileAsync(newDoc.BlobKey, cancellationToken);

            // Call existing document/text/storage services
            var fileChunks = await textExtractorService.GetTextEmbeddedChunksAsync(response.ResponseStream, newDoc.BlobKey, cancellationToken);

            fileChunks = fileChunks.OrderBy(c => c.Index).ToList();

            var summary = await chatService.GenerateSummaryAsync(fileChunks, cancellationToken);

            newDoc.Summary = summary;

            var documentChunks = mapper.Map<List<Chunk>>(fileChunks, opt =>
            {
                opt.Items["DocumentId"] = newDoc.DocumentId;
            });

            await chunkRepository.CreateChunksAsync(documentChunks, cancellationToken);

            await documentService.CompleteProcessingAsync(documentId, newDoc.Summary, cancellationToken);
        } // Hangfire stops the job
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Document processing interrupted because worker is stopping : {documentId}", documentId);

            try
            {
                await documentService.ResetProcessingDocumentAsync(documentId, ProcessingStatus.Pending, CancellationToken.None);
            }
            catch (Exception cleanupEx)
            {
                logger.LogError(cleanupEx, "Cleanup failed after cancellation for document ; {DocumentId}", documentId);
                backgroundJobs.Enqueue<DocumentProcessingJob>(job => job.CleanupAsync(documentId));
            }
            throw;
        } // Completed or Failed and notification
        catch (Exception ex)
        {
            try
            {
                await documentService.ResetProcessingDocumentAsync(documentId, ProcessingStatus.Failed, CancellationToken.None, "An error has ocurred while processing your document.");
            }
            catch (Exception cleanupEx)
            {
                logger.LogError(cleanupEx, "Cleanup failed after cancellation for document ; {DocumentId}", documentId);
                backgroundJobs.Enqueue<DocumentProcessingJob>(job => job.CleanupAsync(documentId));
            }

            try
            {
                await hubContext.Clients
                .Group($"user:{documentWorkspace!.OwnerId}")
                .SendAsync("DocumentProcessingUpdated", documentId, CancellationToken.None);
            }
            catch (Exception e)
            {
                logger.LogError(e, "SignalR notification for document set to failed state failed for document : {DocumentId}", documentId);
            }

            logger.LogError(ex, "An error has ocurred while processing the document : {documentId}", documentId);
            return;
        }

        try
        {
            await hubContext.Clients
                .Group($"user:{documentWorkspace!.OwnerId}")
                .SendAsync("DocumentProcessingUpdated", documentId, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Document {documentId} processing was succesful but SignalR notification failed", documentId);
            return;
        }

        logger.LogInformation("Document {documentId} successfully processed.", documentId);
    }

    [Queue("cleanup")]
    [AutomaticRetry(Attempts = 5)]
    public async Task CleanupAsync(Guid documentId)
    {
        try
        {
            await documentService.ResetProcessingDocumentAsync(documentId, ProcessingStatus.Failed, CancellationToken.None, "Processing was interrupted and cleanup initially failed.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception thrown while running cleanup job for document : {DocumentId}", documentId);
            throw;
        }
    }
}