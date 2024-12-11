using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class AddProductDto
    {
        [Required]
        [MinLength(5)]
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        [RegularExpression(@"^\$?\d+(\.(\d{2}))?$")]
        public decimal Price { get; set; }

        public int LowStockThreshold { get; set; }
        public string Category { get; set; }
        [Required]
        public string WarehouseName { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
