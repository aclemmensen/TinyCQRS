using System;
using System.Collections.Generic;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Model
{
    public class CustomerDto : IDto
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int Zip { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public List<ProductDto> Products { get; set; }

        public CustomerDto()
        {
            Products = new List<ProductDto>();
        }
    }
}