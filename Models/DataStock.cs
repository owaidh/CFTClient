using System.ComponentModel.DataAnnotations.Schema;

namespace CFTClient.Models;

[Table("Data_Stock")]
public class DataStock
{
    [Column("id_product")]
    public int IdProduct { get; set; }

    [Column("quantity")]
    public double Quantity { get; set; }
}
