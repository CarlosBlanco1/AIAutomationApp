using app_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace app_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MydbContext _dbContext;

        public UserController(MydbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _dbContext.Users.ToList();

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
        public IActionResult GetUserById(Guid id)
        {
            var userToFind = _dbContext.Users.Find(id);

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
        public IActionResult CreateUser([FromBody] CreateUserDTO createUserDTO)
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

            _dbContext.Users.Add(modelUser);
            _dbContext.SaveChanges();

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
        public IActionResult UpdateUser(Guid userId, [FromBody] UpdateUserDTO updateUserDTO)
        {
            var userToUpdate = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);

            if(userToUpdate == null)
            {
                return NotFound("User doesn't exist!");
            }

            userToUpdate.FirstName = updateUserDTO.FirstName;
            userToUpdate.LastName = updateUserDTO.LastName;

            _dbContext.SaveChanges();

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
        public IActionResult DeleteUser([FromRoute] Guid userId)
        {
            var userToDelete = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);

            if(userToDelete == null)
            {
                return NotFound("User doesn't exist!");
            }

            _dbContext.Users.Remove(userToDelete);
            _dbContext.SaveChanges();

            return Ok();
        }
    }
}
