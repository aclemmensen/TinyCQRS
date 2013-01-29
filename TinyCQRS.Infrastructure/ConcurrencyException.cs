using System;

namespace TinyCQRS.Infrastructure
{
    public class ConcurrencyException : ApplicationException
    {
         public ConcurrencyException(string message) : base(message)
         {
             
         }
    }
}