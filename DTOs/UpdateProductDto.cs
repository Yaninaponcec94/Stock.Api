namespace Stock.Api.DTOs
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int MinStock { get; set; }
	}
}
