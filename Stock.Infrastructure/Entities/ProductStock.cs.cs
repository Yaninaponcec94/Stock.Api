using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Infrastructure.Entities
{
	public class ProductStock

	{
		public int ProductId { get; set; }

		public int Quantity { get; set; }
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		public Product Product { get; set; } = null!;
		public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();
		public byte[] RowVersion { get; set; } = default!;


	}
}
