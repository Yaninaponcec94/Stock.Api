using Stock.Application.Interfaces;
using Stock.Application.Models;


namespace Stock.Application.Services
{
	public class StockService : IStockService
	{
		private readonly IStockRepository _repo;

		public StockService(IStockRepository repo)
		{
			_repo = repo;
		}

		public async Task<ProductStockResult> CreateMovementAsync(
			int productId,
			StockMovementType type,
			int quantity,
			string? reason)
		{
			if (type == StockMovementType.Adjustment && string.IsNullOrWhiteSpace(reason))
				throw new InvalidOperationException("El motivo es obligatorio para un ajuste.");

			if (quantity <= 0)
				throw new InvalidOperationException("La cantidad debe ser mayor a 0");

			var exists = await _repo.ProductExistsAndActiveAsync(productId);
			if (!exists)
				throw new InvalidOperationException("Producto no existe o está inactivo");

			if (type == StockMovementType.Exit)
			{
				var current = await _repo.GetCurrentQuantityAsync(productId);
				if (quantity > current)
					throw new InvalidOperationException("Stock insuficiente para realizar la salida");
			}

			return await _repo.ApplyMovementAsync(productId, type, quantity, reason);
		}

		public Task<List<StockItemResult>> GetStockAsync()
			=> _repo.GetStockAsync();


		public Task<PagedResult<StockMovementHistory>> GetMovementsAsync(int? productId, int page, int pageSize)
			=> _repo.GetMovementsAsync(productId, page, pageSize);



	}
}
