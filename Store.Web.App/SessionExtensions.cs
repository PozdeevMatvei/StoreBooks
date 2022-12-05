using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;


namespace Store.Web.App
{
    public static class SessionExtensions
    {
        private const string KeyCart = "Cart";

        public static void Set<Cart>(this ISession session, Cart value)
        {
            if (value == null)
                return;
            session.SetString(KeyCart, JsonSerializer.Serialize<Cart>(value));
        }
        public static bool TryGetCart(this ISession session, out Cart? cart)
        {
            var value = session.GetString(KeyCart);

            if (value != null)
            {
                cart = JsonSerializer.Deserialize<Cart>(value);
                return true;
            }
            else
            {
                cart = null;
                return false;
            }
        }
        public static void RemoveCart(this ISession session)
        {
            session.Remove(KeyCart);
        }

        //private static void SetString(this ISession session, string key, string value)
        //{
        //    session.Set(key, Encoding.UTF8.GetBytes(value));
        //}
        //private static string? GetString(this ISession session, string key)
        //{
        //    var data = session.Get(key);
        //    if (data == null)
        //    {
        //        return null;
        //    }
        //    return Encoding.UTF8.GetString(data);
        //}
        //private static byte[]? Get(this ISession session, string key)
        //{
        //    session.TryGetValue(key, out var value);
        //    return value;
        //}

    }
}