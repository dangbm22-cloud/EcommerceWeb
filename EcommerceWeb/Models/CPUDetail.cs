namespace EcommerceWeb.Models
{
    public class CPUDetail
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int CoreCount { get; set; }
        public int ThreadCount { get; set; }
        public string Socket { get; set; }
        public string Manufacturer { get; set; }
        public string BaseClock { get; set; }
    }
}
