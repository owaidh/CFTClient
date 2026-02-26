using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CFTClient.Models;

[Table("Product_Data")]
public class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_code")]
    public string? ProductCode { get; set; }

    [Column("product_name_1")]
    public string? ProductName1 { get; set; }

    [Column("product_name_2")]
    public string? ProductName2 { get; set; }

    [Column("cost_value")]
    public double CostValue { get; set; }
}
