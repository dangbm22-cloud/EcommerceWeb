using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWeb.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; } // nếu có đăng nhập
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Thông tin khách hàng
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Notes { get; set; } = "";
        public string PaymentMethod { get; set; } = "COD";

        // Trạng thái đơn hàng
        public string Status { get; set; } = "Pending"; // mặc định Pending

        // Tổng tiền
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }

        public List<OrderDetail> Details { get; set; } = new();

    }
}
