using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Commands.Auth
{
    public record RegisterUserCommand : IRequest<string>
    {
        [Required]
        [MinLength(6)]
        public string Username { get; set; }
        [Required]
        [StringLength(6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])",ErrorMessage ="password be contain upper and lower case ,digits, srecial characters ")]
        public string Password { get; set; }
        //public string Role { get; set; }
    }
}
