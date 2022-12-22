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

    }
}