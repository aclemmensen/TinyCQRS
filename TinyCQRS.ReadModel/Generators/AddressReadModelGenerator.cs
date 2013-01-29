using System;
using System.Reflection;
using System.Text;
using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;

namespace TinyCQRS.ReadModel.Generators
{
    public class AddressReadModelGenerator :
        ISubscribeTo<CustomerAddressChanged>
    {
        public void Process(CustomerAddressChanged @event)
        {
            Console.WriteLine("Received address: {0}", @event.Address.PrettyPrint());
        }
    }

    public static class PrettyPrintExtensions
    {
        public static string PrettyPrint(this object obj)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{{ {0}:\n", obj.GetType().Name);
            
            var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            foreach (var prop in props)
            {
                sb.AppendFormat("  {0}: {1}\n", prop.Name, prop.GetValue(obj, null));
            }
            sb.Append("}");
            
            return sb.ToString();
        }
    }
}