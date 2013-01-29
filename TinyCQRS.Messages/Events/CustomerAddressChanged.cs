using System;

namespace TinyCQRS.Messages.Events
{
    public class CustomerAddressChanged : Event
    {
        public CustomerAddress Address { get; set; }

        public CustomerAddressChanged(Guid id, CustomerAddress address) : base(id)
        {
            Address = address;
        }
    }

    public class CustomerAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int Zip { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}