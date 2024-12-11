using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Commands.Products
{
    public class AddProductCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int LowStockThreshold { get; set; }
        public string Category { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
    }
}
