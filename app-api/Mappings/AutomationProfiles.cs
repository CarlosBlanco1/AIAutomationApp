using app_api.Models;
using AutoMapper;

public class AutomationProfiles : Profile
{
    public AutomationProfiles()
    {
        CreateMap<Automation, AutomationDTO>();
        CreateMap<CreateAutomationDTO, Automation>()
        .ForMember(dest => dest.AutomationId,
        opt => opt.MapFrom(_ => Guid.NewGuid()));
        CreateMap<UpdateAutomationDTO, Automation>();
    }
}