using TinyCQRS.Contracts;

namespace TinyCQRS.Infrastructure
{
	public interface ICommandDispatcher
	{
		void Dispatch(Command command);
		void Dispatch<T>(T command) where T : Command;
	}
}