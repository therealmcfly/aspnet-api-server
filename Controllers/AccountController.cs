using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnet_api_server.Dtos.Account;
using aspnet_api_server.Interfaces;
using aspnet_api_server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnet_api_server.Controllers
{
	[Route("api/account")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ITokenService _tokenService;
		private readonly SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_signInManager = signInManager;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				var appUser = new AppUser
				{
					UserName = registerDto.Username,
					Email = registerDto.Email,
					FirstName = registerDto.FirstName ?? string.Empty,
					LastName = registerDto.LastName ?? string.Empty,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				};

				var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

				if (createdUser.Succeeded)
				{
					// Add user to role as User as default if successfully created
					var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

					if (roleResult.Succeeded)
					{
						return Ok(
							new NewUserDto
							{
								Username = appUser.UserName,
								Email = appUser.Email,
								Token = _tokenService.CreateToken(appUser)
							}
						);
					}
					else
					{
						return StatusCode(500, roleResult.Errors);
					}
				}
				else
				{
					return StatusCode(500, createdUser.Errors);
				}
			}
			catch (Exception e)
			{

				return StatusCode(500, e);
			}
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

			if (user == null)
				return Unauthorized("Invalid username or password");

			var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

			if (!result.Succeeded)
				return Unauthorized("Username not found and/or password is incorrect");

			return Ok(
				new NewUserDto
				{
					Username = user.UserName,
					Email = user.Email,
					Token = _tokenService.CreateToken(user)
				}
			);
		}
	}
}