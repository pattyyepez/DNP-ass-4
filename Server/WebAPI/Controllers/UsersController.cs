using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto request)
        {
            await VerifyUserNameIsAvailableAsync(request.UserName); 
            var user = new User(request.UserName, request.Password);
            var createdUser = await _userRepository.AddAsync(user);
            var userDto = new UserDto { Id = createdUser.Id, UserName = createdUser.Username };
            return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userRepository.GetSingleAsync(id);
            if (user == null)
                return NotFound();

            return new UserDto { Id = user.Id, UserName = user.Username };
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetUsers([FromQuery] string? nameContains = null)
        {
            var users = _userRepository.GetManyAsync()
                .Where(u => string.IsNullOrEmpty(nameContains) || u.Username.Contains(nameContains))
                .Select(u => new UserDto { Id = u.Id, UserName = u.Username });

            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUserDto request)
        {
            var user = await _userRepository.GetSingleAsync(id);
            if (user == null)
                return NotFound();

            user.Username = request.UserName;
            user.Password = request.Password;
            await _userRepository.UpdateAsync(user);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task VerifyUserNameIsAvailableAsync(string userName)
        {
            var existingUser = _userRepository.GetManyAsync().FirstOrDefault(u => u.Username == userName);
            if (existingUser != null)
            {
                throw new Exceptions.ValidationException($"The username '{userName}' is already taken.");
            }
        }
    }
