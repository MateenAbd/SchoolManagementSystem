using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Admin.Models;
using SMS.Application.Commands.Identity;
using SMS.Application.Queries.Identity;
using SMS.Core.Logger.Interfaces;

namespace SMS.Admin.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AuthController(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken token)
        {
            try
            {
                var userId = await _mediator.Send(new CreateUserCommand
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Password = request.Password,
                    IsActive = true
                }, token);

                await _mediator.Send(new AssignRoleCommand { UserId = userId, RoleName = request.RoleName }, token);

                return Ok(new { success = true, userId });
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "Register validation failed");
                return BadRequest(new { success = false, errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Register failed");
                return StatusCode(500, new { success = false, error = "Registration failed" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken token)
        {
            try
            {
                var user = await _mediator.Send(new ValidateUserCredentialsQuery
                {
                    UserNameOrEmail = request.UserNameOrEmail,
                    Password = request.Password
                }, token);

                if (user == null)
                    return Unauthorized(new { success = false, error = "Invalid credentials" });

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                }.ToList();

                foreach (var role in user.Roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = request.RememberMe
                    });

                return Ok(new { success = true, userId = user.UserId, roles = user.Roles });
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Login failed");
                return StatusCode(500, new { success = false, error = "Login error" });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { success = true });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> LinkStudent([FromBody] LinkStudentToUserCommand command, CancellationToken token)
        {
            try
            {
                var id = await _mediator.Send(command, token);
                return Ok(new { success = true, id });
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "LinkStudent failed");
                return StatusCode(500, new { success = false, error = "Link failed" });
            }
        }
    }
}