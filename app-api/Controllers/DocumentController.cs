using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : Controller
{
    private readonly IDocumentRepository documentRepository;
    private readonly IMapper mapper;

    public DocumentController(IDocumentRepository documentRepository, IMapper mapper)
    {
        this.documentRepository = documentRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    [Route("{workspaceId:guid}")]
    public async Task<IActionResult> GetDocumentsByWorkspaceId(Guid workspaceId)
    {
        var workspaceDocs = await documentRepository.GetDocumentsByWorkspaceIdAsync(workspaceId);

        if (workspaceDocs == null)
        {
            return NotFound("Workspace not found!");
        }

        return Ok(mapper.Map<List<DocumentDTO>>(workspaceDocs));
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentDTO createDocumentDTO)
    {
        var newDoc = mapper.Map<Document>(createDocumentDTO);

        newDoc = await documentRepository.CreateDocumentAsync(newDoc);

        if (newDoc == null)
        {
            return NotFound("Workspace not found!");
        }

        var returnDocDto = mapper.Map<DocumentDTO>(newDoc);

        return CreatedAtAction(nameof(GetDocumentsByWorkspaceId), new {workspaceId = newDoc.WorkspaceId}, newDoc);
    }

    [HttpPut]
    [ValidateModel]
    [Route("{documentId:guid}")]
    public async Task<IActionResult> UpdateDocument([FromRoute] Guid documentId, [FromBody] UpdateDocumentDTO updateDocumentDTO)
    {
        var documentToUpdate = mapper.Map<Document>(updateDocumentDTO);

        documentToUpdate = await documentRepository.UpdateDocumentAsync(documentId, documentToUpdate);

        if(documentToUpdate == null)
        {
            return NotFound("Document doesn't exist!");
        }

        return Ok(mapper.Map<DocumentDTO>(documentToUpdate));
    }

    [HttpDelete]
    [Route("{documentId:guid}")]
    public async Task<IActionResult> DeleteDocument([FromRoute] Guid documentId)
    {
        var documentToDelete = await documentRepository.DeleteDocumentAsync(documentId);

        if(documentToDelete == null)
        {
            return NotFound("Document doesn't exist!");
        }

        return Ok();
    }
}