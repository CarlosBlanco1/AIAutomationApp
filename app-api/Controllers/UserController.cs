using System.Security.Claims;
using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace app_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")] // TO BE IMPLEMENTED
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userRepository.GetAllUsersAsync();

            return Ok(mapper.Map<List<UserDTO>>(users));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var userToFind = await userRepository.GetUserByIdAsync(idInToken);

            if(userToFind == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<UserDTO>(userToFind));
        }

        [HttpPut("me")]
        [ValidateModel]
        [Authorize]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDTO updateUserDTO)
        {
            var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var userToUpdate = mapper.Map<User>(updateUserDTO);

            userToUpdate = await userRepository.UpdateUserAsync(idInToken, userToUpdate);

            if(userToUpdate == null)
            {
                return NotFound("User doesn't exist!");
            }

            return Ok(mapper.Map<UserDTO>(userToUpdate));
        }

        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var userToDelete = await userRepository.DeleteUserAsync(idInToken);

            if(userToDelete == null)
            {
                return NotFound("User doesn't exist!");
            }

            return Ok();
        }
    }
}
