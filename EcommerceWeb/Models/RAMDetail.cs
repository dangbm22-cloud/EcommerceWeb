namespace EcommerceWeb.Models
{
    public class RAMDetail
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Type { get; set; }
        public int Capacity { get; set; }
        public int BusSpeed { get; set; }
        public string Manufacturer { get; set; }
    }
}
