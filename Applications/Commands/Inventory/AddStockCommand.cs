using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Commands.Inventory
{
    public class StockCommand : IRequest<string>
    {
        public int ProductId { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
        public string PerformedBy { get; set; }
    }
}
