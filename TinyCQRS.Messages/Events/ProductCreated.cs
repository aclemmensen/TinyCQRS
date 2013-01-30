using System;

namespace TinyCQRS.Messages.Events
{
    public class ProductCreated : Event
    {
        public string Name { get; set; }

        public ProductCreated(Guid id) : base(id) { }
    }
}