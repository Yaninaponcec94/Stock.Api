using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Application.Models
{
    public class StockAlertResult
    {
        public int ProductId { get; set; } 
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int MinStock { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
	}
}
