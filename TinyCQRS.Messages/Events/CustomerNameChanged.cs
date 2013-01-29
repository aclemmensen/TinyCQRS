using System;

namespace TinyCQRS.Messages.Events
{
    public class CustomerNameChanged : Event
    {
        public string NewName { get; set; }

        public CustomerNameChanged() { }

        public CustomerNameChanged(Guid aggregateId, string newName) : base(aggregateId)
        {
            NewName = newName;
        }
    }
}