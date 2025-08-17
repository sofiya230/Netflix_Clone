using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NetflixClone.DTOs;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace NetflixClone.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(registrationDto.Email);
                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Email already in use"
                    };
                }

                var user = new User
                {
                    Email = registrationDto.Email,
                    PasswordHash = HashPassword(registrationDto.Password),
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    DateOfBirth = registrationDto.DateOfBirth,
                    IsActive = true,
                    SubscriptionPlan = "Basic",
                    Profiles = new List<Profile>
                    {
                        new Profile
                        {
                            Name = $"{registrationDto.FirstName}'s Profile",
                            AvatarUrl = "/images/defaultavatar.jpg",
                            IsKidsProfile = false,
                            Language = "en-US",
                            MaturityLevel = "Adult"
                        }
                    }
                };

                var createdUser = await _userRepository.AddAsync(user);

                var token = await GenerateJwtTokenAsync(createdUser);

                return new AuthResponseDto
                {
                    IsSuccessful = true,
                    Token = token,
                    User = await MapUserToUserResponseDto(createdUser)
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("Duplicate entry") == true)
            {
                return new AuthResponseDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Email already in use. Please use a different email address."
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "An error occurred during registration. Please try again."
                };
            }
        }

        public async Task<AuthResponseDto> LoginUserAsync(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Invalid email or password"
                };
            }

            if (!user.IsActive)
            {
                return new AuthResponseDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Account is deactivated"
                };
            }

            var userWithProfiles = await _userRepository.GetUserWithProfilesAsync(user.Id);

            var token = await GenerateJwtTokenAsync(userWithProfiles!);

            return new AuthResponseDto
            {
                IsSuccessful = true,
                Token = token,
                User = await MapUserToUserResponseDto(userWithProfiles!),
                Role = userWithProfiles!.Role
            };
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserWithProfilesAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            return await MapUserToUserResponseDto(user);
        }

        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            return await MapUserToUserResponseDto(user);
        }

        public async Task<UserResponseDto> UpdateUserAsync(int userId, UserUpdateDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            user.FirstName = updateDto.FirstName ?? string.Empty;
            user.LastName = updateDto.LastName ?? string.Empty;
            user.ProfilePictureUrl = updateDto.ProfilePictureUrl ?? string.Empty;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            var updatedUser = await _userRepository.GetUserWithProfilesAsync(userId);
            return await MapUserToUserResponseDto(updatedUser!);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangeSubscriptionPlanAsync(int userId, ChangeSubscriptionDto changeSubscriptionDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            user.SubscriptionPlan = changeSubscriptionDto.NewSubscriptionPlan ?? string.Empty;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteAsync(userId);
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }



        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(1);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserResponseDto> MapUserToUserResponseDto(User user)
        {
            var userDto = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsActive = user.IsActive,
                SubscriptionPlan = user.SubscriptionPlan,
                Profiles = new List<ProfileResponseDto>()
            };

            if (user.Profiles != null && user.Profiles.Count > 0)
            {
                foreach (var profile in user.Profiles)
                {
                    userDto.Profiles.Add(new ProfileResponseDto
                    {
                        Id = profile.Id,
                        UserId = profile.UserId,
                        Name = profile.Name,
                        AvatarUrl = profile.AvatarUrl,
                        IsKidsProfile = profile.IsKidsProfile,
                        Language = profile.Language,
                        MaturityLevel = profile.MaturityLevel
                    });
                }
            }

            return userDto;
        }


    }
}
