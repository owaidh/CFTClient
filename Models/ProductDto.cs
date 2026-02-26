namespace CFTClient.Models;

public class ProductDto
{
    public int ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName1 { get; set; }
    public string? ProductName2 { get; set; }
    public double CostValue { get; set; }
    public decimal? SellingPrice { get; set; }
}
