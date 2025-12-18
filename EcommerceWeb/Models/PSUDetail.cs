namespace EcommerceWeb.Models
{
    public class PSUDetail
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Wattage { get; set; }
        public string Efficiency { get; set; }
        public string Manufacturer { get; set; }
        public bool Modular { get; set; }
    }
}
