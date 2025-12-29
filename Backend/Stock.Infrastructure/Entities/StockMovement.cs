using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Infrastructure.Entities
{
    public class StockMovement
    {
		public int Id { get; set; }

		public int ProductId { get; set; }
		public MovementType Type { get; set; }

		public int Quantity { get; set; }

		public string? Reason { get; set; }
		public DateTime Date { get; set; } = DateTime.UtcNow;

		public Product Product { get; set; } = null!;
		public ProductStock? ProductStock { get; set; }
	}
}
