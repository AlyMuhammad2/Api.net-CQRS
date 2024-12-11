using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ArchivedTransaction
    {
        [Key]
        public int Id { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string PerformedBy { get; set; }
        public string SourceWarehouse { get; set; } = string.Empty;
        public string TargetWarehouse { get; set; } = string.Empty;
    }
}
