using Azure.Core;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Generators;
using PetShop.Repositories.Basic;
using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using PetShop.Repositories.Models.Enums;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace PetShop.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly HashSet<string> _blacklistedTokens = new HashSet<string>();
        public AuthService(IUserRepository userRepository, IConfiguration config, HttpClient httpClient)
        {
            _userRepository = userRepository;
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<UserResponse> Register(RegisterRequest registerRequest)
        {
            // Kiểm tra trùng phone
            if (!string.IsNullOrEmpty(registerRequest.Phone))
            {
                var existedUserByPhone = await _userRepository.GetUserByPhoneAsync(registerRequest.Phone);
                if (existedUserByPhone != null)
                    throw new Exception($"Phone number {registerRequest.Phone} is already in use.");
            }

            // Kiểm tra trùng email
            if (!string.IsNullOrEmpty(registerRequest.Email))
            {
                var existedUserByEmail = await _userRepository.GetUserByEmailAsync(registerRequest.Email);
                if (existedUserByEmail != null)
                    throw new Exception($"Email {registerRequest.Email} is already in use.");
            }

            // Kiểm tra trùng username
            if (!string.IsNullOrEmpty(registerRequest.Username))
            {
                var existedUserByUsername = await _userRepository.GetUserByUsernameAsync(registerRequest.Username);
                if (existedUserByUsername != null)
                    throw new Exception($"Username {registerRequest.Username} is already in use.");
            }


            var newUser = new User
            {
                Username = registerRequest.Username,
                Password = registerRequest.Password,
                FullName = registerRequest.FullName,
                Email = registerRequest.Email,
                Phone = registerRequest.Phone,
                CreatedAt = DateTime.Now,
                Role = UserRoleEnum.Customer,
            };

            var createdUser = await _userRepository.CreateUserAsync(newUser);

            var createdUserResponse = new UserResponse
            {
                Username = createdUser.Username,
                Password = createdUser.Password,
                FullName = createdUser.FullName,
                Email = createdUser.Email,
                Phone = createdUser.Phone,
            };



            return createdUserResponse;
        }

        public async Task<string> Login(LoginRequest loginRequest)
        {
            var user = await _userRepository.GetUserByUsernameAndPasswordAsync(loginRequest.Username, loginRequest.Password);
            if (user == null)
                throw new Exception("Invalid username or password.");

            // Tạo JWT token
            var token = GenerateJwtToken(user);
            return token;
        }

        public async Task<string> LoginWithGoogleAsync(GoogleLoginRequest dto)
        {
            // 1. Xác thực ID token (chữ ký, audience, exp…)
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                dto.IdToken,
                new ValidationSettings { Audience = new[] { _config["Google:ClientId"] } });

            // 2. Lấy email
            var email = payload.Email;
            var name = payload.Name;
            var user = await _userRepository.GetUserByEmailAsync(email);


            if (user == null)
            {
                user = new User
                {
                    Username = email,
                    Password = Guid.NewGuid().ToString("N").Substring(0, 12),
                    FullName = name,
                    Email = email,
                    CreatedAt = DateTime.Now,
                    Role = UserRoleEnum.Customer,
                };

                user = await _userRepository.CreateUserAsync(user);
            }

            var token = GenerateJwtToken(user);
            return token;

        }

        public async Task<string> LoginWithFacebookAsync(LoginFacebookRequest request)
        {
            // 1. Lấy App Secret từ config (bảo mật)
            var appSecret = _config["Facebook:AppSecret"];

            // 2. Tạo AppSecret Proof (bảo mật tăng cường)
            var appSecretProof = GenerateAppSecretProof(request.AccessToken, appSecret);

            // 3. Gọi API "debug_token" của Facebook để xác thực token
            // Chúng ta cũng có thể gọi /me, nhưng debug_token an toàn hơn
            var debugUrl = $"https://graph.facebook.com/debug_token?input_token={request.AccessToken}&access_token={_config["Facebook:AppId"]}|{appSecret}";

            // HOẶC cách đơn giản hơn là gọi thẳng /me
            var fields = "id,name,email,picture.type(large)";
            var meUrl = $"https://graph.facebook.com/me?fields={fields}&access_token={request.AccessToken}";

            var response = await _httpClient.GetAsync(meUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Token không hợp lệ hoặc có lỗi
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var fbUser = JsonConvert.DeserializeObject<FacebookUserResponse>(content);

            var user = await _userRepository.GetUserByEmailAsync(fbUser.Email);

            if (user == null)
            {
                user = new User
                {
                    Username = fbUser.Email,
                    Password = Guid.NewGuid().ToString("N").Substring(0, 12),
                    FullName = fbUser.Name,
                    Email = fbUser.Email,
                    CreatedAt = DateTime.Now,
                    Role = UserRoleEnum.Customer,
                };

                user = await _userRepository.CreateUserAsync(user);
            }

            var token = GenerateJwtToken(user);
            return token;
        }


        public bool IsTokenBlacklisted(string token)
        {
            return _blacklistedTokens.Contains(token);
        }


        public Task LogoutAsync(string token)
        {
            _blacklistedTokens.Add(token);
            return Task.CompletedTask;
        }



        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            // Các claim (thông tin bạn muốn nhúng trong token)
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("role", user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // Chữ ký
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo token
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToDouble(jwtSettings["ExpireHours"] ?? "2")),
                signingCredentials: creds
            );

            // Trả token dạng string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // (Helper)
        private string GenerateAppSecretProof(string accessToken, string appSecret)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(appSecret)))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(accessToken));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
