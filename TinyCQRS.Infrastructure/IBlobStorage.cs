using TinyCQRS.Contracts;

namespace TinyCQRS.Infrastructure
{
	public interface IBlobStorage
	{
		T Get<T>(BlobReference<T> reference);
		T Find<T>(BlobReference<T> reference);
		void Save<T>(BlobReference<T> reference);
		void Remove(BlobReference reference); 
	}
}