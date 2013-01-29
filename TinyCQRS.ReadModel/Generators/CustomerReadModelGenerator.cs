using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.ReadModel.Generators
{
    public class CustomerReadModelGenerator : 
        ISubscribeTo<CustomerCreated>, 
        ISubscribeTo<CustomerNameChanged>,
        ISubscribeTo<CustomerAddedProduct>,
        ISubscribeTo<CustomerAddressChanged>
    {
        private readonly IDtoRepository<CustomerDto> _customerRepository;
        private readonly IDtoRepository<ProductDto> _productRepository;

        public CustomerReadModelGenerator(IDtoRepository<CustomerDto> customerRepository, IDtoRepository<ProductDto> productRepository)
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public void Process(CustomerCreated @event)
        {
            var dto = new CustomerDto
            {
                Id = @event.AggregateId,
                Name = @event.Name
            };

            _customerRepository.Save(dto);
        }

        public void Process(CustomerNameChanged @event)
        {
            var dto = _customerRepository.GetById(@event.AggregateId);
            dto.Name = @event.NewName;
        }

        public void Process(CustomerAddedProduct @event)
        {
            var dto = _customerRepository.GetById(@event.AggregateId);
            var prod = _productRepository.GetById(@event.ProductId);

            dto.Products.Add(prod);
        }

        public void Process(CustomerAddressChanged @event)
        {
            var dto = _customerRepository.GetById(@event.AggregateId);

            dto.Address1 = @event.Address.Address1;
            dto.Address2 = @event.Address.Address2;
            dto.Zip      = @event.Address.Zip;
            dto.State    = @event.Address.State;
            dto.Country  = @event.Address.Country;
        }
    }
}