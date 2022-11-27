using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Contractors
{
    public class PostamateDeliveryService : IDeliveryService
    {
        private static IReadOnlyDictionary<string, string> _cities = new Dictionary<string, string>
        {
            { "1", "Москва" },
            { "2", "Санкт-Петербург" },
        };

        private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _postamates = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            {
                "1",
                new Dictionary<string, string>
                {
                    { "1", "Казанский вокзал" },
                    { "2", "Охотный ряд" },
                    { "3", "Савёловский рынок" },
                }
            },
            {
                "2",
                new Dictionary<string, string>
                {
                    { "4", "Московский вокзал" },
                    { "5", "Гостиный двор" },
                    { "6", "Петропавловская крепость" },
                }
            }
        };


        public string Name => "Postamate";

        public string Title => "Доставка через постаматы в Москве и Санкт-Петербурге";

        public Form FirstForm(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return Form.CreateFirst(Name)
                       .AddParameter("orderId", order.OrderId.ToString())
                       .AddField(new SelectionField("Город", "city", "1", _cities));
        }

        public Form NextForm(int step, IReadOnlyDictionary<string, string> parameters)
        {
            if (step == 1)
            {
                if (parameters["city"] == "1")
                {
                    return Form.CreateNext(Name, step + 1, parameters)
                               .AddField(new SelectionField("Постамат", "postamate", "1",
                                                            _postamates["1"]));
                }
                else if (parameters["city"] == "2")
                {
                    return Form.CreateNext(Name, step + 1, parameters)
                               .AddField(new SelectionField("Постамат", "postamate", "4",
                                                            _postamates["2"]));
                }
                else
                    throw new InvalidOperationException("Invalid postamate city.");
            }
            else if (step == 2)
            {
                return Form.CreateLast(Name, step + 1, parameters);
            }
            else
                throw new InvalidOperationException("Invalid postamate step.");
        }

        public OrderDelivery GetDelivery(Form form)
        {
            if(form == null)
                throw new ArgumentNullException(nameof(form));

            if(form.ServiceName != Name || !form.IsFinal)
                throw new InvalidOperationException("invalid form.");

            var cityId = form.Parameters["city"];
            string cityName = _cities[cityId];
            var postomateId = form.Parameters["postamate"];
            string postomateName = _postamates[cityId][postomateId];

            string description = $"Город: {cityName}, Постамат: {postomateName}";

            var parameters = new Dictionary<string, string>
            {
                {nameof(cityId), cityId },
                {nameof(cityName), cityName },
                {nameof(postomateId), postomateId },
                {nameof(postomateName), postomateName }
            };

            return new OrderDelivery(Name, description, 150m, parameters);
        }
    }
}
