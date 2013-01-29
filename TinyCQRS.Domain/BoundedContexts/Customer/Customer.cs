using System;
using System.Collections.Generic;
using TinyCQRS.Messages.Events;

namespace TinyCQRS.Domain.BoundedContexts.Customer
{
    public class Customer : AggregateRoot
    {
        private string _name;
        private Address _address;
        private readonly List<Guid> _products = new List<Guid>();

        public Customer() { }

        public Customer(Guid id, string name)
        {
            ApplyChange(new CustomerCreated(id, name));
        }

        public void ChangeName(string newName)
        {
            ApplyChange(new CustomerNameChanged(_id, newName));
        }

        public void AddProduct(Product.Product product)
        {
            ApplyChange(new CustomerAddedProduct(_id, product.Id));
        }

        public void SetAddress(CustomerAddress address)
        {
            ApplyChange(new CustomerAddressChanged(_id, address));
        }

        #region Apply methods
        
        private void Apply(CustomerCreated message)
        {
            _id = message.AggregateId;
            _name = message.Name;
        }

        private void Apply(CustomerNameChanged message)
        {
            _name = message.NewName;
        }

        private void Apply(CustomerAddedProduct message)
        {
            _products.Add(message.ProductId);
        }

        private void Apply(CustomerAddressChanged message)
        {
            var address = new Address
            {
                Address1 = message.Address.Address1,
                Address2 = message.Address.Address2,
                Country = message.Address.Country,
                State = message.Address.State,
                Zip = message.Address.Zip
            };

            _address = address;
        }

        #endregion
    }
}
