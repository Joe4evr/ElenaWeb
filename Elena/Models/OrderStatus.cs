using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elena.Models
{
    public enum OrderStatus
    {
        Placed = 0,
        Confirmed = 1,
        Paid = 2,
        Shipped = 3,
        Delivered = 4
    }
}
