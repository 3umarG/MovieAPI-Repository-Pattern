using Movies.Core.DTOs;
using Movies.Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthModel> RegisterAsync(UserRegisterDto dto);

        public Task<AuthModel> RegisterAsAdmin(UserRegisterDto dto);

        public Task<AuthModel> LoginAsync(UserLoginDto dto);

        public Task<AuthModel> RefreshTokenAsync(string oldRefreshToken);

        public Task<bool> RevokeTokenAsync(string token);
    }
}
