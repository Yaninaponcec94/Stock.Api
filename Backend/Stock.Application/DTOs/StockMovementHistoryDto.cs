using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Application.DTOs
{
	public class StockMovementHistoryDto
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty; // "Entry" | "Exit" | "Adjustment"
		public int Quantity { get; set; }
		public string? Reason { get; set; }
		public DateTime Date { get; set; }
	}
}
