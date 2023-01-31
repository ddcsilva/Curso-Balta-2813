namespace DependencyStore.Models;

public class Pedido
{
    public string Codigo { get; set; }
    public DateTime Data { get; set; }
    public decimal TaxaEntrega { get; set; }
    public decimal Desconto { get; set; }
    public int[] Produtos { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
}