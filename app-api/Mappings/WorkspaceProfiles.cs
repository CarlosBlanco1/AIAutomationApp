using app_api.Models;
using AutoMapper;

public class WorkspaceProfiles : Profile
{
    public WorkspaceProfiles()
    {
        CreateMap<Workspace, WorkspaceDTO>();
        CreateMap<UpdateWorkspaceDTO, Workspace>();
        CreateMap<CreateWorkspaceDTO, Workspace>()
        .ForMember(dest => dest.WorkspaceId,
        opt => opt.MapFrom(_ => Guid.NewGuid()));
    }
}