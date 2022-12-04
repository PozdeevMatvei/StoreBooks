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

        public async Task<Order> GetOrderAsync()
        {
            var (isGetOrder, order) = await TryGetOrderAsync();

            if (isGetOrder)
                return order!;

            throw new InvalidOperationException("Empty session");
        }
        public async Task<(bool isGetModel, OrderModel? model)> TryGetModelAsync()
        {
            var (isGetModel, order) = await TryGetOrderAsync();
            if(isGetModel)
            {
                return (true, await MapAsync(order!));
            }
            return (false, null);
        }
        public async Task<OrderModel> AddOrderItemAsync(int bookId, int count)
        {         
            var (isGetOrder, order) = await TryGetOrderAsync();
            if (!isGetOrder)
                order = await _orderRepository.CreateAsync();

            var book = await _bookRepository.GetByIdAsync(bookId);
            order!.Items.Add(book.BookId, book.Price, count);

            await _orderRepository.UpdateAsync(order);
            UpdateSession(order!);

            return await MapAsync(order!);
        }
        public async Task<OrderModel> UpdateOrderItemAsync(int bookId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("Too few books to update");

            var order = await GetOrderAsync();
            order.Items.Get(bookId).Count = count;

            await _orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return await MapAsync(order);
        }
        public async Task<OrderModel> RemoveOrderItemAsync(int bookId)
        {
            var order = await GetOrderAsync();
            order.Items.Remove(bookId);

            await _orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return await MapAsync(order);
        }
        public async Task<OrderModel> AddBookAsync(int bookId)
        {
            var order = await GetOrderAsync();
            var item = order.Items.Get(bookId);

            if(item.Count < 100)
                item.Count += 1;

            await _orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return await MapAsync(order);
        }
        public async Task<OrderModel> PutAwayBookAsync(int bookId)
        {
            var order = await GetOrderAsync();
            var item = order.Items.Get(bookId);

            if(item.Count > 1)
                item.Count -= 1;

            await _orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return await MapAsync(order);
        }
        public async Task<bool> IsBookInCartAsync(int bookId) 
        {
            var (isGetOrder, order) = await TryGetOrderAsync();

            if(isGetOrder)
                return order!.Items.TryGet(bookId, out OrderItem? orderItem);

            return false;
        }
        public async Task<OrderModel> SendConfirmationAsync(string cellPhone)
        {
            var order = await GetOrderAsync();
            var orderModel = await MapAsync(order);

            if (TryFormatPhone(cellPhone, out string? formattedPhone))
            {
                var confirmationCode = 1234;
                orderModel.CellPhone = formattedPhone;
                Session.SetInt32(formattedPhone, confirmationCode);
                await _notificationService.SendConfirmationCodeAsync(cellPhone, confirmationCode);
            }
            else
                orderModel.Errors[cellPhone] = "Номер телефона не соответствует формату +79992457505";

            return orderModel;

        }
        public async Task<OrderModel> ConfirmCellPhoneAsync(string cellPhone, int confirmationCode)
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

            var order = await GetOrderAsync();
            order.CellPhone = cellPhone;
            await _orderRepository.UpdateAsync(order);

            Session.Remove(cellPhone);

            return await MapAsync(order);
        }
        public async Task<OrderModel> SetDeliveryAsync(OrderDelivery delivery)
        {
            var order = await GetOrderAsync();
            order.Delivery = delivery;
            await _orderRepository.UpdateAsync(order);

            return await MapAsync(order);
        }
        public async Task<OrderModel> SetPaymentAsync(OrderPayment payment)
        {
            var order = await GetOrderAsync();
            order.Payment = payment;

            await _orderRepository.UpdateAsync(order);
            Session.RemoveCart();
            await _notificationService.StartProcessAsync(order);

            return await MapAsync(order);
        }


        internal async Task<(bool isGetOrder, Order? order)> TryGetOrderAsync()
        {
            if(Session.TryGetCart(out Cart? cart))
            {
                var order = await _orderRepository.GetByIdAsync(cart!.OrderId);
                return (true, order);
            }

            return (false, null);
        }
        internal async Task<OrderModel> MapAsync(Order order)
        {
            var books = await GetAllBooksAsync(order);
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
        internal async Task<IEnumerable<Book>> GetAllBooksAsync(Order order)
        {
            var bookIds = order.Items.Select(item => item.BookId);

            return await _bookRepository.GetAllByIdsAsync(bookIds);
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
