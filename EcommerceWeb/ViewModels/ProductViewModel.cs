namespace EcommerceWeb.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }

        public List<SpecificationItem> Specifications { get; set; } = new();
    }

    public class SpecificationItem
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

}
