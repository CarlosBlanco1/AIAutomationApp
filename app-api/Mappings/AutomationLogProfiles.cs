using app_api.Models;
using AutoMapper;

public class AutomationLogProfiles : Profile
{
    public AutomationLogProfiles()
    {
        CreateMap<AutomationLog, AutomationLogDTO>();
        CreateMap<CreateAutomationLogDTO, AutomationLog>()
        .ForMember(dest => dest.AutomationLogId,
        opt => opt.MapFrom(_ => Guid.NewGuid()))
        .ForMember(dest => dest.CreatedAt,
        opt => opt.MapFrom(_ => DateTime.Now));
    }
}