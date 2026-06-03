// using app_api.Models;
// using AutoMapper;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Internal;

// namespace app_api.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class UserController : ControllerBase
//     {
//         private readonly IUserRepository userRepository;
//         private readonly IMapper mapper;

//         public UserController(IUserRepository userRepository, IMapper mapper)
//         {
//             this.userRepository = userRepository;
//             this.mapper = mapper;
//         }
//         [HttpGet]
//         public async Task<IActionResult> GetAllUsers()
//         {
//             var users = await userRepository.GetAllUsersAsync();

//             return Ok(mapper.Map<List<UserDTO>>(users));
//         }

//         [HttpGet]
//         [Route("{id:guid}")]
//         public async Task<IActionResult> GetUserById(Guid id)
//         {
//             var userToFind = await userRepository.GetUserByIdAsync(id);

//             if(userToFind == null)
//             {
//                 return NotFound();
//             }

//             return Ok(mapper.Map<UserDTO>(userToFind));
//         }

//         [HttpPost]
//         [ValidateModel]
//         public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO)
//         {
//             PasswordHasher<User> hasher = new();

//             var modelUser = new User()
//             {
//                 UserId = Guid.NewGuid(),
//                 FirstName = createUserDTO.FirstName,
//                 LastName = createUserDTO.LastName,
//                 Email = createUserDTO.Email,
//                 CreatedAt = DateOnly.FromDateTime(DateTime.Now)
//             };

//             modelUser.PasswordHash = hasher.HashPassword(modelUser, createUserDTO.Password);

//             modelUser = await userRepository.CreateUserAsync(modelUser);

//             if(modelUser == null)
//             {
//                 return BadRequest("Email already exists");
//             }

//             var returnUser = mapper.Map<UserDTO>(modelUser);

//             return CreatedAtAction(nameof(GetUserById), new { id = returnUser.UserId}, returnUser);
//         }

//         [HttpPut]
//         [ValidateModel]
//         [Route("{userId:guid}")]
//         public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDTO updateUserDTO)
//         {
//             var userToUpdate = mapper.Map<User>(updateUserDTO);

//             userToUpdate = await userRepository.UpdateUserAsync(userId, userToUpdate);

//             if(userToUpdate == null)
//             {
//                 return NotFound("User doesn't exist!");
//             }

//             return Ok(mapper.Map<UserDTO>(userToUpdate));
//         }

//         [HttpDelete]
//         [Route("{userId:guid}")]
//         public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
//         {
//             var userToDelete = await userRepository.DeleteUserAsync(userId);

//             if(userToDelete == null)
//             {
//                 return NotFound("User doesn't exist!");
//             }

//             return Ok();
//         }
//     }
// }
