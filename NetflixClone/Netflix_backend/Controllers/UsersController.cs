using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetflixClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, UserUpdateDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                var user = await _userService.UpdateUserAsync(id, updateDto);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(int id, ChangePasswordDto changePasswordDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                var result = await _userService.ChangePasswordAsync(id, changePasswordDto);
                if (result)
                {
                    return Ok(new { message = "Password changed successfully" });
                }
                return BadRequest(new { message = "Current password is incorrect" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/change-subscription")]
        public async Task<ActionResult> ChangeSubscriptionPlan(int id, ChangeSubscriptionDto changeSubscriptionDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                var result = await _userService.ChangeSubscriptionPlanAsync(id, changeSubscriptionDto);
                return Ok(new { message = "Subscription plan changed successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/deactivate")]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                var result = await _userService.DeactivateUserAsync(id);
                return Ok(new { message = "Account deactivated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (result)
                {
                    return Ok(new { message = "Account deleted successfully" });
                }
                return BadRequest(new { message = "Failed to delete account" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
