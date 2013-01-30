using TinyCQRS.Messages;

namespace TinyCQRS.Domain.Interfaces
{
    public interface IMessageBus
    {
		void Subscribe(params IConsume[] subscribers);

        void Notify(Event @event);
    }
}