namespace EcommerceWeb.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        // Khóa ngoại tới Order
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        // Khóa ngoại tới Product
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;  //Thêm navigation property trong cái SSMS

        // Snapshot thông tin sản phẩm tại thời điểm mua
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal LineTotal => UnitPrice * Quantity;
    }
}