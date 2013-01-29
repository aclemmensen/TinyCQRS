using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.ReadModel.Generators
{
    public class ProductReadModelGenerator : ISubscribeTo<ProductCreated>
    {
        private readonly IDtoRepository<ProductDto> _repository;

        public ProductReadModelGenerator(IDtoRepository<ProductDto> repository)
        {
            _repository = repository;
        }

        public void Process(ProductCreated @event)
        {
            var dto = new ProductDto
            {
                Id = @event.AggregateId,
                Name = @event.Name
            };

            _repository.Save(dto);
        }
    }
}