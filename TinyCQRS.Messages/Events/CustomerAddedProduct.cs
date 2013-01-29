using System;

namespace TinyCQRS.Messages.Events
{
    public class CustomerAddedProduct : Event
    {
        public Guid ProductId { get; set; }

        public CustomerAddedProduct() { }
        public CustomerAddedProduct(Guid id, Guid productId) : base(id)
        {
            ProductId = productId;
        }
    }
}