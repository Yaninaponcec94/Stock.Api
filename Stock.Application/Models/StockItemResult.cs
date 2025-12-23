namespace Stock.Application.Models
{
	public class StockItemResult
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public DateTime UpdatedAt { get; set; }

		public int MinStock { get; set; }
		public bool IsBelowMinStock { get; set; }

	}
}
