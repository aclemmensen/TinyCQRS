namespace TinyCQRS.Domain.BoundedContexts.Customer
{
    public class Address : ValueObject
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int Zip { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}