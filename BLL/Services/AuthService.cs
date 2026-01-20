using BLL.DTOs;
using DAL.Models;
using DAL.Models.Enum;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BLL.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO model);
        Task<AuthResponseDTO> LoginAsync(LoginDTO model);
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken);
        Task SendForgotPasswordOTPAsync(ForgotPasswordDTO model);
        Task<bool> VerifyOTPAsync(VerifyOTPDTO model);
        Task ResetPasswordAsync(ResetPasswordDTO model);
    }

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IPasswordHasher<User> passwordHasher,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new Exception("Password is required.");
            }
            var email = NormalizeEmail(model.Email);
            var existingUser = await _unitOfWork.User.GetAsync(u => u.Email == email);
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists");
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = email,
                FullName = $"{model.FullName}",
                PhoneNumber = model.PhoneNumber,
                Role = (int)Role.Customer, // Default role
                Password = string.Empty
            };
            user.Password = _passwordHasher.HashPassword(user, model.Password);

            await _unitOfWork.User.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var token = _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);
            var expiresAt = GetTokenExpiryUtc(token);

            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                UserId = user.UserId.ToString(),
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO model)
        {
            var email = NormalizeEmail(model.Email);
            var user = await _unitOfWork.User.GetAsync(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new Exception("This account uses social login");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid email or password");
            }

            var token = _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);
            var expiresAt = GetTokenExpiryUtc(token);

            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                UserId = user.UserId.ToString(),
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(refreshToken);
            if (principal == null)
            {
                throw new Exception("Invalid refresh token");
            }

            var tokenType = principal.FindFirst("typ")?.Value;
            if (!string.Equals(tokenType, "refresh", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Invalid refresh token");
            }

            var refreshExpiry = GetTokenExpiryUtc(refreshToken);
            if (refreshExpiry <= DateTime.UtcNow)
            {
                throw new Exception("Refresh token expired");
            }

            var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var user = await _unitOfWork.User.GetAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var newToken = _jwtService.GenerateJwtToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken(user);
            var expiresAt = GetTokenExpiryUtc(newToken);

            return new AuthResponseDTO
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt,
                UserId = user.UserId.ToString(),
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task SendForgotPasswordOTPAsync(ForgotPasswordDTO model)
        {
            var email = NormalizeEmail(model.Email);
            var user = await _unitOfWork.User.GetAsync(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Generate 6-digit OTP
            var otp = GenerateOtpCode();
            var expiry = DateTime.UtcNow.AddMinutes(5);

            var existingOtp = await _unitOfWork.OtpCode.GetAsync(o => o.Email == email, tracked: true);
            if (existingOtp == null)
            {
                var otpEntity = new OtpCode
                {
                    Email = email,
                    Code = otp,
                    Expiry = expiry
                };
                await _unitOfWork.OtpCode.AddAsync(otpEntity);
            }
            else
            {
                existingOtp.Code = otp;
                existingOtp.Expiry = expiry;
                await _unitOfWork.OtpCode.UpdateAsync(existingOtp);
            }
            await _unitOfWork.SaveChangesAsync();

            // Send OTP via email
            await _emailService.SendOTPEmailAsync(email, otp);
        }

        public async Task<bool> VerifyOTPAsync(VerifyOTPDTO model)
        {
            var email = NormalizeEmail(model.Email);
            var otpEntity = await _unitOfWork.OtpCode.GetAsync(o => o.Email == email, tracked: true);
            if (otpEntity == null)
            {
                throw new Exception("OTP not found or expired");
            }

            if (DateTime.UtcNow > otpEntity.Expiry)
            {
                await _unitOfWork.OtpCode.RemoveAsync(otpEntity);
                await _unitOfWork.SaveChangesAsync();
                throw new Exception("OTP has expired");
            }

            if (!string.Equals(otpEntity.Code, model.OTP, StringComparison.Ordinal))
            {
                throw new Exception("Invalid OTP");
            }

            return true;
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO model)
        {
            // Verify OTP first
            await VerifyOTPAsync(new VerifyOTPDTO { Email = model.Email, OTP = model.OTP });

            var email = NormalizeEmail(model.Email);
            var user = await _unitOfWork.User.GetAsync(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Update password
            user.Password = _passwordHasher.HashPassword(user, model.NewPassword);
            await _unitOfWork.User.UpdateAsync(user);
            var otpEntity = await _unitOfWork.OtpCode.GetAsync(o => o.Email == email, tracked: true);
            if (otpEntity != null)
            {
                await _unitOfWork.OtpCode.RemoveAsync(otpEntity);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        private static string GenerateOtpCode()
        {
            var value = RandomNumberGenerator.GetInt32(100000, 1000000);
            return value.ToString();
        }

        private static DateTime GetTokenExpiryUtc(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.ValidTo.ToUniversalTime();
        }
    }
} 