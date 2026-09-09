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
    IHubContext<ChatHub> hubContext,
    IMapper mapper,
    ILogger<DocumentProcessingJob> logger)
{
    public async Task ProcessAsync(Guid documentId, CancellationToken cancellationToken)
    {

        // Load document
        var newDoc = await documentService.GetDocumentByIdAsync(documentId);

        if (newDoc == null)
        {
            logger.LogInformation("Document does not exist and cannot be processed.");
            return;
        }

        var documentWorkspace = await workspaceRepository.GetWorkspaceByIdAsync(newDoc.WorkspaceId);

        try
        {
            logger.LogInformation("Document fetching succesful! starting processing for doc : {documentId}", documentId);
            // Pending to Processing
            if (!await documentService.TryMarkProcessingAsync(documentId, cancellationToken))
            {
                logger.LogInformation("Skipping document processing for document {documentId}, no longer pending.", documentId);
                return;
            }

            // Notify user document is now being processed
            await hubContext.Clients
            .Group($"user:{documentWorkspace!.OwnerId}")
            .SendAsync("DocumentProcessingUpdated", documentId, cancellationToken);

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
            await documentService.MarkPendingAsync(documentId, CancellationToken.None);
            throw;
        } // Completed or Failed and notification
        catch (Exception ex)
        {
            await documentService.MarkProcessingFailedAsync(documentId, "An error has ocurred while processing your document.", CancellationToken.None);
            await hubContext.Clients
            .Group($"user:{documentWorkspace!.OwnerId}")
            .SendAsync("DocumentProcessingUpdated", documentId, cancellationToken);
            logger.LogError(ex, "An error has ocurred while processing the document : {documentId}", documentId);
            return;
        }

        try
        {
            await hubContext.Clients
                .Group($"user:{documentWorkspace!.OwnerId}")
                .SendAsync("DocumentProcessingUpdated", documentId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Document {documentId} processing was succesful but SignalR notification failed", documentId);
            return;
        }

        logger.LogInformation("Document {documentId} successfully processed.", documentId);
    }
}