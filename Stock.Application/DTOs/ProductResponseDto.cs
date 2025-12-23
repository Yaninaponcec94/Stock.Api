namespace Stock.Application.DTOs

{
	public class ProductResponseDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int MinStock { get; set; }
		public bool IsActive { get; set; }
	}
}
