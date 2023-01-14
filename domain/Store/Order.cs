using Store.DTO;
using System;

namespace Store
{
    public class Order
    {
        private readonly OrderDto _dto;
        public Order(OrderDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            _dto = dto;

            Items = new OrderItemCollection(dto);
        }
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
        public Guid? UserId => _dto.UserId;
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
                ArgumentNullException.ThrowIfNull(value);

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
                ArgumentNullException.ThrowIfNull(value);

                _dto.PaymentName = value.Name;
                _dto.PaymentDescription = value.Description;
                _dto.PaymentParameters = value.Parameters?.ToDictionary(param => param.Key,
                                                                       param => param.Value);

            }
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
