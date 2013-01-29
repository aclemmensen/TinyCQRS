using System;
using TinyCQRS.Messages.Events;

namespace TinyCQRS.Domain.BoundedContexts.Product
{
    public class Product : AggregateRoot
    {
        private string _name;

        public string Name { get { return _name; } }

        public Product() { }

        public Product(Guid id, string name)
        {
            ApplyChange(new ProductCreated(id) { Name = name });
        }

        private void Apply(ProductCreated message)
        {
            _id = message.AggregateId;
            _name = message.Name;
        }
    }
}