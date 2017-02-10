using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elena.Constants.OrderController
{
    public class OrderControllerRoute
    {
        public const string GetIndex = ControllerName.Order + "OrderIndex";
        public const string PostSubmit = ControllerName.Order + "PostOrder";
        public const string GetSuccess = ControllerName.Order + "OrderSuccess";
        public const string GetFail = ControllerName.Order + "OrderFail";
    }
}
