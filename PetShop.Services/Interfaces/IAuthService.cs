using PetShop.Repositories.Models;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponse> Register(RegisterRequest registerRequest);
        Task<string> Login(LoginRequest loginRequest);
        Task<string> LoginWithFacebookAsync(LoginFacebookRequest request);
        Task<string> LoginWithGoogleAsync(GoogleLoginRequest dto);
        Task LogoutAsync(string token);
        bool IsTokenBlacklisted(string token);
    }
}
