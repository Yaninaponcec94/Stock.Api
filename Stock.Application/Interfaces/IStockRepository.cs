using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Application.Models;

namespace Stock.Application.Interfaces
{
	public interface IStockRepository
	{
		Task<bool> ProductExistsAndActiveAsync(int productId);

		Task<int> GetCurrentQuantityAsync(int productId);

		Task<ProductStockResult> ApplyMovementAsync(int productId, string type, int quantity, string? reason);

		Task<List<StockAlertResult>> GetStockAlertsAsync();



	}
}

