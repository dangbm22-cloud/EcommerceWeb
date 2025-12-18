namespace EcommerceWeb.Models
{
    public class MainboardDetail
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Socket { get; set; }
        public string Chipset { get; set; }
        public string FormFactor { get; set; }
        public string Manufacturer { get; set; }
    }
}
