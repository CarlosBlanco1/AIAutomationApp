using app_api.Models;
using AutoMapper;

public class UserProfiles : Profile
{
    public UserProfiles()
    {
        CreateMap<User, UserDTO>();
        CreateMap<UpdateUserDTO, User>();
    }
}