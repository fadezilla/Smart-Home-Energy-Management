using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHome.backend.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace SmartHome.backend.Controllers
{
    /// <summary>
    /// Admin-only endpoints for managing users and other admin operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // All endpoints in this controller require Admin role
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Promotes a user to admin role.
        /// </summary>
        /// <param name="id">The user's ID.</param>
        /// <returns>No content if successful, 404 if user not found.</returns>
        /// <response code="204">User promoted successfully</response>
        /// <response code="404">User not found</response>
        [HttpPost("users/{id}/promote")]
        [SwaggerResponse(204, "User promoted successfully")]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> PromoteUserToAdmin(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userService.SetUserRoleAsync(id, "Admin");
            return NoContent();
        }

        /// <summary>
        /// Demotes a user to regular user role.
        /// </summary>
        /// <param name="id">The user's ID.</param>
        /// <returns>No content if successful, 404 if user not found.</returns>
        /// <response code="204">User demoted successfully</response>
        /// <response code="404">User not found</response>
        [HttpPost("users/{id}/demote")]
        [SwaggerResponse(204, "User demoted successfully")]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> DemoteUserToRegular(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userService.SetUserRoleAsync(id, "User");
            return NoContent();
        }
    }
}
