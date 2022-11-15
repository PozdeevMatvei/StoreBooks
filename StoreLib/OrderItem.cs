using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class OrderItem
    {
        public int BookId { get; }

        private int _count;
        public int Count 
        {
            get { return _count; }
            set
            {
                ThrowIfInvalidCount(value);
                _count = value;
            }

        }
        public decimal Price { get; }

        public OrderItem(int bookId, decimal price, int count)
        {
            ThrowIfInvalidCount(count);

            BookId = bookId;
            Count = count;
            Price = price;
        }

        private static void ThrowIfInvalidCount(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(count)} by zero or negative number.");
        }
    }
}
