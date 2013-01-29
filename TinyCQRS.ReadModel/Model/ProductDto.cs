using System;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Model
{
    public class ProductDto : IDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}