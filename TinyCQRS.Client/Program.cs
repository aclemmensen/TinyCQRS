using System;
using TinyCQRS.Domain.BoundedContexts.Customer;
using TinyCQRS.Domain.BoundedContexts.Product;
using TinyCQRS.Infrastructure.Persistence;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Generators;
using TinyCQRS.ReadModel.Model;
using TinyCQRS.ReadModel.Repositories;

namespace TinyCQRS.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var messageBus = new InMemoryMessageBus();

            using (var eventStore = new DispatchingEventStore(new InMemoryEventStore(), messageBus))
            {
                var customerRepository = new EventedRepository<Customer>(eventStore);
                var productRepository = new EventedRepository<Product>(eventStore);

                var customerDtoRepository = new DtoRepository<CustomerDto>();
                var productDtoRepository = new DtoRepository<ProductDto>();
                //var addressDtoRepository = new DtoRepository<AddressDto>();

                var customerRmg = new CustomerReadModelGenerator(customerDtoRepository, productDtoRepository);
                var productRmg = new ProductReadModelGenerator(productDtoRepository);
                var addressRmg = new AddressReadModelGenerator();

                messageBus.Subscribe(customerRmg);
                messageBus.Subscribe(productRmg);
                messageBus.Subscribe(addressRmg);

                var customer = new Customer(Guid.NewGuid(), "Charles Dickens");
                customer.ChangeName("James Bond");
                customer.SetAddress(new CustomerAddress
                {
                    Address1 = "Keplersgade 2, 4.tv",
                    Zip = 2300,
                    Country = "Denmark"
                });

                customerRepository.Save(customer);

                var product1 = new Product(Guid.NewGuid(), "Philips Shaver");
                var product2 = new Product(Guid.NewGuid(), "Jameson Whiskey");
                var product3 = new Product(Guid.NewGuid(), "Apple iPod");
                productRepository.Save(product1);
                productRepository.Save(product2);
                productRepository.Save(product3);

                customer.AddProduct(product1);
                customer.AddProduct(product2);
                customer.AddProduct(product3);

                customerRepository.Save(customer);

                var dto = customerDtoRepository.GetById(customer.Id);
            }
        }
    }
}
