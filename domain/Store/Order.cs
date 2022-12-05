using Store.DTO;

namespace Store
{
    public class Order
    {
        private readonly OrderDto _dto;
        public int OrderId => _dto.Id;
        public string? CellPhone
        {
            get => _dto.CellPhone;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(CellPhone));

                _dto.CellPhone = value;
            }
        }
        public int TotalCount => Items.Sum(item => item.Count);
        public decimal TotalPrice => Items.Sum(item => item.Price * item.Count)
                                                          + (Delivery?.Price ?? 0m);
        public OrderItemCollection Items { get; }
        public OrderDelivery? Delivery
        {
            get
            {
                if (_dto.DeliveryName == null)
                    return null;

                return new OrderDelivery(_dto.DeliveryName,
                                         _dto.DeliveryDescription!,
                                         _dto.DeliveryPrice,
                                         _dto.DeliveryParameters!);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Delivery));

                _dto.DeliveryName = value.Name;
                _dto.DeliveryDescription = value.Description;
                _dto.DeliveryPrice = value.Price;
                _dto.DeliveryParameters = value.Parameters.ToDictionary(param => param.Key,
                                                                        param => param.Value);

            }
        }
        public OrderPayment? Payment
        {
            get
            {
                if (_dto.PaymentName == null)
                    return null;

                return new OrderPayment(_dto.PaymentName,
                                        _dto.PaymentDescription,
                                        _dto.PaymentParameters);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Payment));

                _dto.PaymentName = value.Name;
                _dto.PaymentDescription = value.Description;
                _dto.PaymentParameters = value.Parameters?.ToDictionary(param => param.Key,
                                                                       param => param.Value);

            }
        }

        public Order(OrderDto dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));

            Items = new OrderItemCollection(dto);
        }

        public static class DtoFactory
        {
            public static OrderDto Create() => new();
        }
        public static class Mapper
        {
            public static OrderDto Map(Order order) => order._dto;
            public static Order Map(OrderDto orderDto) => new Order(orderDto);
        }
    }
}
