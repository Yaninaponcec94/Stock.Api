using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Stock.Application.Models
{
	public class ProductStockResult
	{
		public int ProductId { get; set; }
		public int NewQuantity { get; set; }
		public int MovementId { get; set; }
	}
}
