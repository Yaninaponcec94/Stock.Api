using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Application.Models;

namespace Stock.Application.Interfaces
{
	public interface IStockService
	{
		Task<ProductStockResult> CreateMovementAsync(int productId, string type, int quantity, string? reason);
		Task<List<StockItemResult>> GetStockAsync();



	}
}

