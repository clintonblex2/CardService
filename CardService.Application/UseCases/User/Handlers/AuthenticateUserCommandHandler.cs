using CardService.Application.Common.Interfaces;
using CardService.Application.UseCases.User.Commands;
using CardService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CardService.Application.UseCases.User.Handlers
{
    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, string?>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AuthenticateUserCommandHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticateUserCommandHandler(
            IUnitOfWork uow,
            ILogger<AuthenticateUserCommandHandler> logger,
            IPasswordHasher passwordHasher,
            IConfiguration configuration)
        {
            _uow = uow;
            _logger = logger;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<string?> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var hashedPassword = _passwordHasher.HashPassword(request.Password);
            var user = await _uow.Repository<UserEntity>().FindAsync(x => x.Email == request.Email && x.Password == hashedPassword);

            if (user == null) return default;

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
