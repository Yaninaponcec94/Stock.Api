namespace Stock.Api.DTOs
{
    public class CreateStockMovementDto
    {
		public int ProductId { get; set; }
		public string Type { get; set; } = string.Empty; 
		public int Quantity { get; set; }
		public string? Reason { get; set; }
	}
}
