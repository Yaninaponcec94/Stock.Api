namespace Stock.Application.DTOs

{
	public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public int MinStock { get; set; }
	}
}
