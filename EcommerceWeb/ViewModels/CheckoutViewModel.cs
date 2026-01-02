using EcommerceWeb.Models;

namespace EcommerceWeb.ViewModels
{
    public class CheckoutViewModel
    {
        // Thông tin khách hàng
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Notes { get; set; } = "";

        // Phương thức thanh toán
        public string PaymentMethod { get; set; } = "COD";

        // Giỏ hàng
        public List<CartItem> CartItems { get; set; } = new();

        // Tính toán tổng tiền
        public decimal Subtotal => CartItems.Sum(i => i.Product.Price * i.Quantity);
        public decimal Total => Subtotal; // có thể cộng thêm phí ship/thuế sau này
    }
}
