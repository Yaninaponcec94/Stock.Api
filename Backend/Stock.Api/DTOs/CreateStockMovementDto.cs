namespace Stock.Api.DTOs

{
	public class CreateStockMovementDto
    {
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public string? Reason { get; set; }
	}
}
