using AutoMapper;
using Pgvector;

public class ChunkProfiles : Profile
{
    public ChunkProfiles()
    {
        CreateMap<ChunkResponse, Chunk>()
        .ForMember(dest => dest.Embedding,
        opt => opt.MapFrom(src => src.Vector))
        .ForMember(dest => dest.ChunkIndex,
        opt => opt.MapFrom(src => src.Index))
        .ForMember(dest => dest.ChunkText,
        opt => opt.MapFrom(src => src.Chunk))
        .ForMember(dest => dest.Embedding,
        opt => opt.MapFrom(src => new Vector(src.Vector)))
        .ForMember(dest => dest.DocumentId,
        opt => opt.MapFrom((src, dest, _, context) =>
        (Guid)context.Items["DocumentId"]));
    }
}