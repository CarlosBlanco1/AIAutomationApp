using app_api.Models;
using AutoMapper;

public class DocumentProfiles : Profile
{
    public DocumentProfiles()
    {
        CreateMap<Document, DocumentDTO>()
        .ForMember(dest => dest.WorkspaceName,
        opt => opt.MapFrom(src => src.Workspace.WorkspaceName));
        CreateMap<UpdateDocumentDTO, Document>();
        CreateMap<CreateDocumentDTO, Document>()
        .ForMember(dest => dest.DocumentId,
        opt => opt.MapFrom(_ => Guid.NewGuid()))
        .ForMember(dest => dest.CreatedAt,
        opt => opt.MapFrom(_ => DateTime.Now))
        .ForMember(dest => dest.FileSizeBytes,
        opt => opt.MapFrom(src => src.File.Length));
    }
}