using Microsoft.AspNetCore.Http;
using Store.Contractors;
using Store.Web.Contractors;

namespace Store.YandexKassa
{
    public class YandexKassaPaymentService : IWebContractorService, IPaymentService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public YandexKassaPaymentService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpRequest Request => _httpContextAccessor.HttpContext!.Request;
        public string Name => "YandexKassa";
        public string Title => "Оплата банковской картой";



        public Form FirstForm(Order order)
        {
            return Form.CreateFirst(Name)
                       .AddParameter("orderId", order.OrderId.ToString());
        }
        public Form NextForm(int step, IReadOnlyDictionary<string, string> parameters)
        {
            if (step != 1)
                throw new InvalidOperationException("Invalid Yandex.Kassa payment step.");

            return Form.CreateLast(Name, step + 1, parameters);
        }
        public OrderPayment GetPayment(Form form)
        {
            if (Name != form.ServiceName || !form.IsFinal)
                throw new InvalidOperationException("Invalid form payment");

            string description = "Оплата банковской картой";
            return new OrderPayment(Name, description, form.Parameters);
        }

        public Uri StartSession(IReadOnlyDictionary<string, string> parameters, Uri returnUri)
        {
            var queryString = QueryString.Create(parameters!);
            queryString += QueryString.Create("returnUri", returnUri.ToString());

            var builder = new UriBuilder(Request.Scheme, Request.Host.Host)
            {
                Path = "YandexKassa/",
                Query = queryString.ToString()
            };

            if (Request.Host.Port != null)
                builder.Port = Request.Host.Port.Value;

            return builder.Uri;
        }
    }
}
