using Application.DTOs.Users;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces;
using Application.Wrapper;
using Domain.Settings;
using Identity.Helpers;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, JWTSettings jwtSettings, IDateTimeService dateTimeService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
        }               

        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            var usuario = await _userManager.FindByEmailAsync(request.Email);
            if (usuario == null)
                throw new ApiException($"No hay una cuenta registrada con el email {request.Email}");

            var result = await _signInManager.PasswordSignInAsync(usuario.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new ApiException($"Las credenciales del usuario no son validas {request.Email}");

            JwtSecurityToken jwtSecurityToken = await GenerateJWTToken(usuario);
            var refreshToken = GenerateRefreshToken(ipAddress);
            var rolesList = await _userManager.GetRolesAsync(usuario);

            AuthenticationResponse response = new()
            {
                Id = usuario.Id,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = usuario.Email,
                UserName = usuario.UserName,
                IsVerified = usuario.EmailConfirmed,
                Roles = rolesList.ToList(),
                RefreshToken = refreshToken.Token                
            };

            return new Response<AuthenticationResponse>(response, $"Usuario autenticado {usuario.UserName}");

        }        

        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var usuarioConElMismoUsername = await _userManager.FindByNameAsync(request.UserName);
            if (usuarioConElMismoUsername != null)
                throw new ApiException($"El nombre de usuario {request.UserName} ya fue registrado previamente");

            var usuarioConElMismoUserCorreo = await _userManager.FindByEmailAsync(request.Email);
            if (usuarioConElMismoUserCorreo != null)
                throw new ApiException($"El email {request.Email} ya fue registrado previamente");

            var usuario = new ApplicationUser
            {
                Email = request.Email,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                UserName = request.UserName
            };

            var result = await _userManager.CreateAsync(usuario, Roles.Basic.ToString());
            if (!result.Succeeded)
                throw new ApiException($"{result.Errors}");

            await _userManager.AddToRoleAsync(usuario, Roles.Basic.ToString());

            return new Response<string>(usuario.Id, message: $"Usuario registrado existosamente. {request.UserName}");

        }

        private async Task<JwtSecurityToken> GenerateJWTToken(ApplicationUser usuario)
        {
            var userClaims = await _userManager.GetClaimsAsync(usuario);
            var roles = await _userManager.GetRolesAsync(usuario);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role.ToString()));
            }

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("uid", usuario.Id),
                new Claim("ip", ipAddress),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
                
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now,
                CreatedByIp = ipAddress
            };
        }

        private string RandomTokenString()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(40);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}
