using System;

namespace eShop.Utilities.Exceptions
{
    public class eShopException : Exception
    {
        public eShopException()
        {
            
        }
        public eShopException(string message)
            : base(message)
        {

        }
        public eShopException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}