using TinyCQRS.Contracts;

namespace TinyCQRS.Domain.Interfaces
{
	public interface IHandler
	{
		
	}

	public interface IHandle<T> : IHandler where T : Command
	{
		void Handle(T command);
	}
}