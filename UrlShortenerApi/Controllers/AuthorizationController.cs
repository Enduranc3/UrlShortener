using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortenerApi.Exceptions;
using UrlShortenerApi.Extensions;
using UrlShortenerApi.Models.Requests;
using UrlShortenerApi.Services;

namespace UrlShortenerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController(IAuthService authService) : Controller
{
	/// <summary>
	///     Creates a new user in the database.
	/// </summary>
	/// <param name="userRegisterRequest"></param>
	/// <returns> A message indicating the result of the operation. </returns>
	/// <response code="200">User successfully registered.</response>
	/// <response code="400">Missing parameters in body.</response>
	/// <response code="409">User already exists.</response>
	[HttpPost("register")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Register([FromBody] UserRegisterRequest userRegisterRequest)
	{
		if (userRegisterRequest.IsNull())
		{
			return BadRequest("Missing parameters in body.");
		}

		try
		{
			await authService.RegisterAsync(userRegisterRequest);
		}
		catch (UserAlreadyExistsException ex)
		{
			return Conflict(ex.Message);
		}

		return Ok("User successfully registered.");
	}

	/// <summary>
	///     Logs in a user.
	/// </summary>
	/// <param name="userLoginRequest"></param>
	/// <returns> A message indicating the result of the operation. </returns>
	/// <response code="200">User successfully logged in.</response>
	/// <response code="400">Missing parameters in body.</response>
	/// <response code="403">Wrong credentials.</response>
	/// <response code="404">User not found.</response>
	[HttpPost("login")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
	{
		if (userLoginRequest.IsNull())
		{
			return BadRequest("Missing parameters in body.");
		}

		try
		{
			var user = await authService.LoginAsync(userLoginRequest.Email, userLoginRequest.Password);
			if (user == null)
			{
				return Forbid();
			}

			return Ok(await authService.GenerateToken(userLoginRequest));
		}
		catch (UserNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
	}


	/// <summary>
	///     Gets the user information.
	/// </summary>
	/// <returns> The user information. </returns>
	/// <response code="200">User information successfully retrieved.</response>
	/// <response code="400">Invalid token.</response>
	/// <response code="403">Invalid token.</response>
	/// <response code="404">User not found.</response>
	[HttpGet("my")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[Authorize]
	public async Task<IActionResult> My()
	{
		int userId;

		try
		{
			userId = Request.TryGetUserId();
		}
		catch (Exception)
		{
			return BadRequest("Invalid token.");
		}

		var user = await authService.GetUserAsync(userId);
		if (user == null)
		{
			return NotFound("User not found.");
		}

		return Ok(new
		{
			id = user.Id,
			email = user.Email,
			username = user.Username,
			role = user.Role.Name
		});
	}
}