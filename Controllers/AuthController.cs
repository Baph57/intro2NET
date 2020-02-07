using System;
using System.Threading.Tasks;
using intro2NET.API.Data;
using intro2NET.API.DTOs;
using intro2NET.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace intro2NET.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;

        public AuthController(IAuthRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (User2Register user2RegisterDTO)
        {
            user2RegisterDTO.Username = user2RegisterDTO.Username.ToLower();

            if(await _repository.UserExistsCheck(user2RegisterDTO.Username))
            {
                return BadRequest("user2RegisterDTO Already Exists!");
            }

            User userToBeCreated = new User
            {
                Username = user2RegisterDTO.Username
            };

            User createdUser = await _repository.Register(userToBeCreated, user2RegisterDTO.Password);

            return StatusCode(201);
            //return CreatedAtRoute();
        }
    }
}
