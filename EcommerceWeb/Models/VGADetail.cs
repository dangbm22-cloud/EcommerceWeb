namespace EcommerceWeb.Models
{
    public class VGADetail
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Chipset { get; set; }
        public int VRAM { get; set; }
        public string Manufacturer { get; set; }
        public string PowerRequirement { get; set; }
    }
}
