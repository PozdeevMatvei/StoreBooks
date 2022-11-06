﻿using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;

namespace Store.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IOrderRepository _orderRepository;

        public CartController(IBookRepository bookRepository, IOrderRepository orderRepository)
        {
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
        }
        public IActionResult Add(int bookId)
        {
            Cart? cart;
            Order order;
            //HttpContext.Session.GetCart(out cart);

            if (HttpContext.Session.TryGetCart(out cart))
            {
                order = _orderRepository.GetById(cart.OrderId);
            }
            else
            {
                order = _orderRepository.Create();
                cart = new Cart(order.OrderId);
            }

            var book = _bookRepository.GetById(bookId);
            order.AddItem(book, 1);
            _orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);

            return RedirectToAction("Index", "Book", new { BookId = bookId});
        }
    }
}
