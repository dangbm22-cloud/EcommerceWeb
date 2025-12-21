using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EcommerceWeb.Helpers
{
    public static class SessionExtensions
    {
        // Lưu object vào Session dưới dạng JSON
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Lấy object từ Session và chuyển lại thành kiểu dữ liệu gốc
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
