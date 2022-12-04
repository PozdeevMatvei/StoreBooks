﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync();
        Task<Order> GetByIdAsync(int orderId);
        Task UpdateAsync(Order order);
    }
}
