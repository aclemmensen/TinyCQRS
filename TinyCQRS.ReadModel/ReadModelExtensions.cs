using System;
using TinyCQRS.Contracts;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel
{
	public static class ReadModelExtensions
	{
		 public static string KeyName(this IReadModel model)
		 {
			 if (model is Entity)
			 {
				 return "GlobalId";
			 }

			 return (model is Dto) 
				 ? "LocalId" 
				 : "_id";
		 }

		public static object Id(this IReadModel model)
		{
			var name = model.KeyName();
			var property = model.GetType().GetProperty(name);
			
			return property.GetValue(model);
		}

		public static T GetOrCreate<T>(this IReadModelRepository<T> repository, object id) where T : class, IReadModel, new()
		{
			return repository.Get(id) ?? new T();
		}

		public static void Update<T>(this IReadModelRepository<T> repository, object id, Action<T> action) where T : class, IReadModel, new()
		{
			var dto = repository.Get(id);
			if (dto == null)
			{
				throw new ApplicationException(string.Format("No model of type {0} with id {1} was found", typeof(T).Name, id));
			}

			action(dto);

			repository.Update(dto);
			repository.Commit();
		}

		public static void Create<T>(this IReadModelRepository<T> repository, Action<T> action) where T : class, IReadModel, new()
		{
			var dto = repository.Create();
			action(dto);

			repository.Add(dto);
			repository.Commit();
		}

		public static void CreateOrUpdate<T>(this IReadModelRepository<T> repository, object id, Action<T> action) where T : class, IReadModel, new()
		{
			var model = repository.GetOrCreate(id);

			action(model);

			if (model.Id == null)
			{
				// TODO: This is ugly.
				var entity = model as Entity;
				if (entity != null)
				{
					entity.GlobalId = (Guid)id;
				}
				var dto = model as Dto;
				if (dto != null)
				{
					dto.LocalId = (long) id;
				}

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