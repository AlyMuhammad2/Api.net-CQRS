using Infrastructue.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Commands.Products
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, string>
    {
        private readonly AppDbContext _context;
        public DeleteProductCommandHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<string> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = _context.Products.FirstOrDefault(p=>p.Id== request.ProductId);
            if (product == null || product.IsDeleted)
            {
                throw new Exception("Product not found ");
            }
            product.IsDeleted = true;
            await _context.SaveChangesAsync();
            return $"product : {product.Name} is deleted ";
        }
    }
}
