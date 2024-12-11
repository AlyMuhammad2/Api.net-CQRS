using Domain.Entities;
using Infrastructue.Data;
using Infrastructue.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Queries.Auth
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, string>
    {
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly AppDbContext _context;

        public LoginUserQueryHandler(AppDbContext context, JwtTokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<string> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);

            if (user == null)
            {
                throw new Exception("Invalid username or password");
            }

            return _tokenGenerator.GenerateToken(user.Username, user.Role.Name );
        }
    }

}
