using System;
using System.Linq;
using TinyCQRS.Domain.BoundedContexts.Customer;
using TinyCQRS.Domain.BoundedContexts.Product;
using TinyCQRS.Domain.EventSourced.QualityAssurance;
using TinyCQRS.Infrastructure.Persistence;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Generators;
using TinyCQRS.ReadModel.Model;
using TinyCQRS.ReadModel.Repositories;

namespace TinyCQRS.Client
{
	static class ServiceExtensions
	{
		public static void Seed(this ISiteCrawlService service, Guid siteId, string name, string url, int numberOfPagesToCreate = 5)
		{
			service.CreateNewSite(siteId, name, url);
			for (var i = 0; i < numberOfPagesToCreate; i++)
			{
				service.AddNewPage(siteId, Guid.NewGuid(), url + "/page" + (i + 1) + ".html", "This is the content");
			}
		}
	}

	class Program
    {
		static void SiteCrawlServiceTest()
		{
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();

			var service = CreateService();

			service.Seed(id1, "Testsite", "http://testsite.dk");
			service.Seed(id2, "Other Site", "http://othersite.dk");

			var crawler = new Crawler(service);

			crawler.Crawl(id1);

			var page = service.GetPagesFor(id1).First();
			
			crawler.Handle(page.Url, page.Content);
			crawler.Handle(page.Url, "a wild piece of content appears!");
			crawler.Handle("http://newurl.dk", "this is a new page");

			Console.WriteLine("rbeak");
		}

		static ISiteCrawlService CreateService()
		{
			// Infrastructure
			var messageBus = new InMemoryMessageBus();
			var eventStore = new InMemoryEventStore();
			var dispatchingEventStore = new DispatchingEventStore(eventStore, messageBus);

			// Write-side
			var siteRepository = new EventedRepository<SiteAggregate>(dispatchingEventStore);
			var pageRepository = new EventedRepository<PageAggregate>(dispatchingEventStore);
			var siteCommandHandler = new SiteCommandHandler(siteRepository);
			var pageCommandHandler = new PageCommandHandler(pageRepository);

			// Read-side
			var siteDtoRepository = new DtoRepository<SiteDto>();
			var pageDtoRepository = new DtoRepository<PageDto>();
			var siteReadModelGenerator = new SiteReadModelGenerator(siteDtoRepository, pageDtoRepository);
			var pageReadModelGenerator = new PageReadModelGenerator(pageDtoRepository, null);

			// Hook-up
			messageBus.Subscribe(pageReadModelGenerator, siteReadModelGenerator);

			// Service
			return new SiteCrawlService(siteCommandHandler, pageCommandHandler, siteDtoRepository);
		}

        static void Main(string[] args)
        {
			SiteCrawlServiceTest();

	        return;
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
