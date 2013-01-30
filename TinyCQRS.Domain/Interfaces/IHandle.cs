using TinyCQRS.Messages;

namespace TinyCQRS.Domain.Interfaces
{
	public interface IHandle<T> where T : Command
	{
		void Handle(T command);
	}
}