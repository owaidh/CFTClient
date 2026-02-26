using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CFTClient.Models;

[Table("Product_Price")]
public class ProductPrice
{
    [Key]
    [Column("price_id")]
    public int PriceId { get; set; }

    [Column("price_product_id")]
    public int PriceProductId { get; set; }

    [Column("price_product")]
    public decimal PriceProduct { get; set; }
}
