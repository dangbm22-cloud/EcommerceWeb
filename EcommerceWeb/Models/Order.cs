using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWeb.Models
{
    public enum OrderStatus //trạng thái đơn hàng
    {
        Pending, //Đang chờ xử lý
        Paid, //Đã thanh toán (nếu có thanh toán online)
        Shipped, //Đã giao hàng
        Completed, //Hoàn thành
        Cancelled //Đã hủy
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(128)]
        public string CustomerName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required, MaxLength(256)]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(32)]
        public string PaymentMethod { get; set; } = "COD";

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
