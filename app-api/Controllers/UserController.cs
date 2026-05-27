using app_api.Models;
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

        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userRepository.GetAllUsersAsync();

            var dtoUsers = new List<UserDTO>();

            foreach (var u in users)
            {
                dtoUsers.Add( new UserDTO ()
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PasswordHash = u.PasswordHash,
                    CreatedAt = u.CreatedAt
                });
            }

            return Ok(dtoUsers);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var userToFind = await userRepository.GetUserByIdAsync(id);

            if(userToFind == null)
            {
                return NotFound();
            }
            else
            {
                var dtoUser = new UserDTO()
                {
                    UserId = userToFind.UserId,
                    FirstName = userToFind.FirstName,
                    LastName = userToFind.LastName,
                    Email = userToFind.Email,
                    PasswordHash = userToFind.PasswordHash,
                    CreatedAt = userToFind.CreatedAt
                };

                return Ok(dtoUser);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            PasswordHasher<User> hasher = new();

            var modelUser = new User()
            {
                UserId = Guid.NewGuid(),
                FirstName = createUserDTO.FirstName,
                LastName = createUserDTO.LastName,
                Email = createUserDTO.Email,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };

            modelUser.PasswordHash = hasher.HashPassword(modelUser, createUserDTO.Password);

            modelUser = await userRepository.CreateUserAsync(modelUser);

            var returnUser = new UserDTO()
            {
                UserId = modelUser.UserId,
                FirstName = modelUser.FirstName,
                LastName = modelUser.LastName,
                Email = modelUser.Email,
                PasswordHash = modelUser.PasswordHash,
                CreatedAt = modelUser.CreatedAt
            };

            return CreatedAtAction(nameof(GetUserById), new { id = returnUser.UserId}, returnUser);
        }

        [HttpPut]
        [Route("{userId:guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDTO updateUserDTO)
        {
            var userToUpdate = new User()
            {
                FirstName = updateUserDTO.FirstName,
                LastName = updateUserDTO.LastName
            };

            userToUpdate = await userRepository.UpdateUserAsync(userId, userToUpdate);

            if(userToUpdate == null)
            {
                return NotFound("User doesn't exist!");
            }

            var userToReturn = new UserDTO()
            {
                UserId = userToUpdate.UserId,
                FirstName = userToUpdate.FirstName,
                LastName = userToUpdate.LastName,
                Email = userToUpdate.Email,
                PasswordHash = userToUpdate.PasswordHash,
                CreatedAt = userToUpdate.CreatedAt
            };

            return Ok(userToReturn);
        }

        [HttpDelete]
        [Route("{userId:guid}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var userToDelete = await userRepository.DeleteUserAsync(userId);

            if(userToDelete == null)
            {
                return NotFound("User doesn't exist!");
            }

            return Ok();
        }
    }
}
