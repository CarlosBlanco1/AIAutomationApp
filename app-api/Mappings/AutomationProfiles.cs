using app_api.Models;
using AutoMapper;

public class AutomationProfiles : Profile
{
    public AutomationProfiles()
    {
        CreateMap<Automation, AutomationDTO>();
        CreateMap<CreateAutomationDTO, Automation>();
        CreateMap<UpdateAutomationDTO, Automation>();
    }
}