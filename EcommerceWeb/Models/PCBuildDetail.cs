namespace EcommerceWeb.Models
{
    public class PCBuildDetail
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string CPU { get; set; }
        public string GPU { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public string PSU { get; set; }
        public string Mainboard { get; set; }
        public string Case { get; set; }
    }
}
