using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Commands.Products
{
    public class DeleteProductCommand :IRequest<string>
    {
        public int ProductId { get; set; }
    }
}
