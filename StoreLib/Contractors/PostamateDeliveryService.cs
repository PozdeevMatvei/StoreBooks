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

        public string UniqueCode => "Postamate";

        public string Title => "Доставка через постаматы в Москве и Санкт-Перербурге";

        public Form CreateForm(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return new Form(UniqueCode, order.OrderId, 1, false, new SelectionField[]
            {
                new SelectionField("Город", "city", "1", _cities),
            });
        }

        public Form MoveNextForm(int orderId, int step, IReadOnlyDictionary<string, string> values)
        {
            if (step == 1)
            {
                if (values["city"] == "1")
                {
                    return new Form(UniqueCode, orderId, 2, false, new Field[]
                    {
                        new HiddenField("Город", "city", "1"),
                        new SelectionField("Постамат", "postamate", "1", _postamates["1"]),
                    });
                }
                else if (values["city"] == "2")
                {
                    return new Form(UniqueCode, orderId, 2, false, new Field[]
                    {
                        new HiddenField("Город", "city", "2"),
                        new SelectionField("Постамат", "postamate", "4", _postamates["2"]),
                    });
                }
                else
                    throw new InvalidOperationException("Invalid postamate city.");
            }
            else if (step == 2)
            {
                return new Form(UniqueCode, orderId, 3, true, new Field[]
                {
                    new HiddenField("Город", "city", values["city"]),
                    new HiddenField("Постамат", "postamate", values["postamate"]),
                });
            }
            else
                throw new InvalidOperationException("Invalid postamate step.");
        }

        public OrderDelivery GetOrderDelivery(Form form)
        {
            if(form == null)
                throw new ArgumentNullException(nameof(form));
            if (form.UniqueCode != UniqueCode || !form.IsFinal)
                throw new InvalidOperationException("invalid form.");

            var cityId = form.Fields
                             .Single(field => field.Name == "city")
                             .Value;
            string cityName = _cities[cityId];

            var postomateId = form.Fields
                                  .Single(field => field.Name == "postamate")
                                  .Value;
            string postomateName = _postamates[cityId][postomateId];

            string description = $"Город: {cityName}, Постамат: {postomateName}";

            var parameters = new Dictionary<string, string>
            {
                {nameof(cityId), cityId },
                {nameof(cityName), cityName },
                {nameof(postomateId), postomateId },
                {nameof(postomateName), postomateName }
            };

            return new OrderDelivery(UniqueCode, description, 150m, parameters);
        }
    }
}
