using Domain.Entities;
using Infrastructue.Data;
using Infrastructue.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.Commands.Auth
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly AppDbContext _context;
        public RegisterUserCommandHandler(AppDbContext context, JwtTokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUser != null)
            {
                throw new Exception("Username is already taken");
            }

            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
                RoleId = 2
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully";
        }
    }

}
