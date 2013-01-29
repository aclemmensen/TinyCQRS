using System.Reflection;

namespace TinyCQRS.Domain
{
    public abstract class ValueObject
    {
        public virtual bool Equals(ValueObject other)
        {
            if (other == null) return false;
            if (!(other.GetType() == GetType())) return false;

            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            
            foreach (var prop in properties)
            {
                var v1 = prop.GetValue(this, null);
                var v2 = prop.GetValue(other, null);

                if ((v1 == null || v2 == null) || v1 != v2) return false;
            }

            return true;
        }
    }
}