using TinyCQRS.Contracts;

namespace TinyCQRS.Domain.Interfaces
{
	public interface IBlobStorage
	{
		T Get<T>(BlobReference reference);
		T Find<T>(BlobReference reference);

		void Save<T>(BlobReference reference, T payload);
		void Remove(BlobReference reference); 
	}
}