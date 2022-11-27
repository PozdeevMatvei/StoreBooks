using Microsoft.AspNetCore.Http;
using PhoneNumbers;
using Store.Messages;
using System.Text;

namespace Store.Web.App
{
    public class OrderService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected ISession Session => _httpContextAccessor.HttpContext.Session;
        
        public OrderService(IBookRepository bookRepository, 
                            IOrderRepository orderRepository, 
                            INotificationService notificationService, 
                            IHttpContextAccessor httpContextAccessor)
        {
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Order GetOrder()
        {
            if (TryGetOrder(out Order? order))
                return order!;

            throw new InvalidOperationException("Empty session");
        }
        public bool TryGetModel(out OrderModel? orderModel)
        {
            if(TryGetOrder(out Order? order))
            {
                orderModel = Map(order!);
                return true;
            }
            orderModel = null;
            return false;
        }
        public OrderModel AddOrderItem(int bookId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("Too few books to add.");

            if (!TryGetOrder(out Order? order))
                order = _orderRepository.Create();

            var book = _bookRepository.GetById(bookId);
            order!.Items.Add(book.BookId, book.Price, count);

            _orderRepository.Update(order);
            UpdateSession(order!);

            return Map(order!);
        }
        public OrderModel UpdateOrderItem(int bookId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("Too few books to update");

            var order = GetOrder();
            order.Items.Get(bookId).Count = count;

            _orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }
        public OrderModel RemoveOrderItem(int bookId)
        {
            var order = GetOrder();
            order.Items.Remove(bookId);

            _orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }
        public OrderModel AddBook(int bookId)
        {
            var order = GetOrder();
            var item = order.Items.Get(bookId);

            if(item.Count < 100)
                item.Count += 1;

            _orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }
        public OrderModel PutAwayBook(int bookId)
        {
            var order = GetOrder();
            var item = order.Items.Get(bookId);

            if(item.Count > 1)
                item.Count -= 1;

            _orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }
        public bool IsBookInCart(int bookId) 
        { 
            if(TryGetOrder(out Order? order))
                return order!.Items.TryGet(bookId, out OrderItem? orderItem);

            return false;
        }
        public OrderModel SendConfirmation(string cellPhone)
        {
            var order = GetOrder();
            var orderModel = Map(order);

            if (TryFormatPhone(cellPhone, out string? formattedPhone))
            {
                var confirmationCode = 1111;
                orderModel.CellPhone = formattedPhone;
                Session.SetInt32(formattedPhone, confirmationCode);
                _notificationService.SendConfirmationCode(cellPhone, confirmationCode);
            }
            else
                orderModel.Errors[cellPhone] = "Номер телефона не соответствует формату +79992457505";

            return orderModel;

        }
        public OrderModel ConfirmCellPhone(string cellPhone, int confirmationCode)
        {
            const string ErrorsKeyCode = "code";

            var storedCode = Session.GetInt32(cellPhone);
            var model = new OrderModel() {CellPhone = cellPhone };

            if(storedCode == null)
            {
                model.Errors[ErrorsKeyCode] = "Что то случилось. Попробуйте получить код еще раз.";
                return model;
            }

            if(storedCode != confirmationCode)
            {
                model.Errors[ErrorsKeyCode] = "Неверный код. Попробуйте получить код еще раз.";
                return model;
            }

            var order = GetOrder();
            order.CellPhone = cellPhone;
            _orderRepository.Update(order);

            Session.Remove(cellPhone);

            return Map(order);
        }
        public OrderModel SetDelivery(OrderDelivery delivery)
        {
            var order = GetOrder();
            order.Delivery = delivery;
            _orderRepository.Update(order);

            return Map(order);
        }
        public OrderModel SetPayment(OrderPayment payment)
        {
            var order = GetOrder();
            order.Payment = payment;
            _orderRepository.Update(order);

            Session.RemoveCart();

            return Map(order);
        }


        internal bool TryGetOrder(out Order? order)
        {
            if(Session.TryGetCart(out Cart? cart))
            {
                order = _orderRepository.GetById(cart!.OrderId);
                return true;
            }
            order = null;
            return false;
        }
        internal OrderModel Map(Order order)
        {
            var books = GetAllBooks(order);
            var orderItemsModel = from item in order.Items
                                  join book in books
                                  on item.BookId equals book.BookId
                                  select new OrderItemModel
                                  {
                                      BookId = book.BookId,
                                      Title = book.Title,
                                      Author = book.Author,
                                      Count = item.Count,
                                      Price = book.Price
                                  };
            return new OrderModel
            {
                OrderId = order.OrderId,
                OrderItems = orderItemsModel.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
                CellPhone = order.CellPhone,
                DeliveryPrice = order.Delivery?.Price,
                DeliveryDescription = order.Delivery?.Description,
                PaymentDescription = $"{order.Payment?.IsCompletePaymentOrder} {order.Payment?.Description}"
            };
        }
        internal IEnumerable<Book> GetAllBooks(Order order)
        {
            var bookIds = order.Items.Select(item => item.BookId);

            return _bookRepository.GetAllByIds(bookIds);
        }
        internal void UpdateSession(Order order)
        {
            var cart = new Cart(order.OrderId, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }      
        internal bool TryFormatPhone(string cellPhone, out string? formattedPhone)
        {
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

            try
            {
                var phoneNumber = phoneNumberUtil.Parse(cellPhone, "ru");
                formattedPhone = phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.INTERNATIONAL);
                return true;
            }
            catch (NumberParseException)
            {
                formattedPhone = null;
                return false;
            }
        }
    }
}
