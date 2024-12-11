using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public string ProductName { get; set; }
        public string TransactionType { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string PerformedBy { get; set; }
    }

}
