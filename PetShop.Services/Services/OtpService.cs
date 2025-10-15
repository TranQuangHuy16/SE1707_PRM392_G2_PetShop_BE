using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Services
{
    public class OtpService : IOtpService
    {
        private readonly IOtpRepository _otpRepo;
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;

        // config
        private readonly TimeSpan _otpTtl = TimeSpan.FromMinutes(5);
        private readonly int _otpLength = 5;
        public OtpService(IOtpRepository otpRepo, IUserRepository userRepo, IEmailService emailService)
        {
            _otpRepo = otpRepo;
            _userRepo = userRepo;
            _emailService = emailService;
        }

        private string GenerateNumericCode(int length)
        {
            var rng = new Random();
            var min = (int)Math.Pow(10, length - 1);
            var max = (int)Math.Pow(10, length) - 1;
            return rng.Next(min, max + 1).ToString();
        }

        public async Task<bool> RequestOtpAsync(string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            if (user == null) return false; // don't reveal to client whether email exists in detail in production

            //// delete old expired or used
            //await _otpRepo.DeleteExpiredAsync(DateTime.UtcNow);

            // generate otp
            var code = GenerateNumericCode(_otpLength);
            var otp = new Otp
            {
                UserId = user.UserId,
                Code = code,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(_otpTtl),
                IsUsed = false
            };

            await _otpRepo.CreateAsync(otp);

            var subject = "Mã OTP đặt lại mật khẩu";
            var body = $"Mã OTP của bạn là {code}. Mã có hiệu lực trong {_otpTtl.TotalMinutes} phút.";
            await _emailService.SendOtpAsync(user.Email, subject, body);

            return true;
        }

        public async Task<bool> VerifyOtpAsync(string email, string code)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            if (user == null) return false;

            var found = await _otpRepo.GetValidOtpByCodeAsync(user.UserId, code);
            if (found == null) return false;

            found.IsUsed = true;
            _otpRepo.UpdateAsync(found);

            return true;
        }

        public async Task<UserResponse> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            if (user == null) return null;

            user.Password = newPassword;

            await _userRepo.UpdateAsync(user);

            var userResponse = new UserResponse
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email,
                Phone = user.Phone
            };

            return userResponse;
        }
    }
}
