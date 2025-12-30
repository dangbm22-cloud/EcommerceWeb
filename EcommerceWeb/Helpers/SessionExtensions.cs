using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EcommerceWeb.Helpers
{
    public static class SessionExtensions
    {
        // Lưu object vào Session dưới dạng JSOM
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Lấy object từ Session và chuyển lại thành kiểu dữ liệu gốc
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            if (string.IsNullOrEmpty(value))
                return default(T);

            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (JsonException)
            {
                // Nếu dữ liệu lỗi, xóa session để tránh crash
                session.Remove(key);
                return default(T);
            }
        }

    }
}
