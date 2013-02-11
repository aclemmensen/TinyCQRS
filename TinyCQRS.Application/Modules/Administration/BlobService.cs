using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Infrastructure;

namespace TinyCQRS.Application.Modules.Administration
{
	public class BlobService : IBlobService
	{
		private readonly IBlobStorage _storage;

		public BlobService(IBlobStorage storage)
		{
			_storage = storage;
		}

		public T Get<T>(BlobReference<T> reference)
		{
			return _storage.Get(reference);
		}

		public T Find<T>(BlobReference<T> reference)
		{
			return _storage.Find(reference);
		}

		public void Save<T>(BlobReference<T> reference)
		{
			_storage.Save(reference);
		}

		public void Remove(BlobReference reference)
		{
			_storage.Remove(reference);
		}
	}
}