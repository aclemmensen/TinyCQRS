using System;
using TinyCQRS.Contracts;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel
{
	public static class ReadModelExtensions
	{

		public static T GetOrCreate<T>(this IReadModelRepository<T> repository, Guid id) where T : Dto, new()
		{
			return repository.Find(id) ?? new T();
		}

		public static void Update<T>(this IReadModelRepository<T> repository, Guid id, Action<T> action) where T : Dto, new()
		{
			var dto = repository.Get(id);

			action(dto);

			repository.Update(dto);
			repository.Commit();
		}

		public static void Create<T>(this IReadModelRepository<T> repository, Action<T> action) where T : Dto, new()
		{
			var dto = repository.Create();
			
			action(dto);

			repository.Add(dto);
			repository.Commit();
		}

		public static void CreateOrUpdate<T>(this IReadModelRepository<T> repository, Guid id, Action<T> action = null) where T : Dto, new()
		{
			var model = repository.GetOrCreate(id);

			if(action != null)
				action(model);

			if (model.Id == Guid.Empty)
			{
				model.Id = id;
				repository.Add(model);
			}
			else
			{
				repository.Update(model);
			}

			repository.Commit();
		}
	}
}