namespace EcommerceWeb.Helpers
{
    public static class OrderStatusHelper
    {
        public static string GetClientStatus(string status)
        {
            return status switch
            {
                "Pending" => "Đang xử lý",
                "Shipped" => "Đang được vận chuyển đến bạn",
                "CancelledByAdmin" => "Đơn đã bị hủy bởi Admin",
                "CancelledByCustomer" => "Đã hủy",
                "Completed" => "Đã hoàn thành đơn",
                _ => status
            };
        }

        public static string GetAdminStatus(string status)
        {
            return status switch
            {
                "Pending" => "Đang xử lý",
                "Shipped" => "Đã giao cho đơn vị vận chuyển",
                "CancelledByAdmin" => "Đã hủy",
                "CancelledByCustomer" => "Đã hủy bởi người đặt",
                "Completed" => "Đã hoàn thành đơn",
                _ => status
            };
        }
    }
}
