using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CFTClient.Models;

[Table("products")]
public class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_code")]
    public string ProductCode { get; set; } = string.Empty;

    [Column("product_name")]
    public string ProductName { get; set; } = string.Empty;

    [Column("Quantity")]
    public int Quantity { get; set; }

    [Column("price_product")]
    public decimal PriceProduct { get; set; }
}
