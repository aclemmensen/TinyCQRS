using System;

namespace TinyCQRS.Messages.Events
{
    public class CustomerCreated : Event
    {
        public string Name { get; set; }
        
        public CustomerCreated() { }

        public CustomerCreated(Guid aggregateId, string name) : base(aggregateId)
        {
            Name = name;
        }
    }
}