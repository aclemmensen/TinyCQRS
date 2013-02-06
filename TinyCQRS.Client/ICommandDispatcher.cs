using TinyCQRS.Messages;

namespace TinyCQRS.Client
{
	public interface ICommandDispatcher
	{
		void Dispatch<T>(T command) where T : Command;
	}
}