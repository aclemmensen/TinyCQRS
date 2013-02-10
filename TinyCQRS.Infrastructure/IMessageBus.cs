using TinyCQRS.Contracts;

namespace TinyCQRS.Infrastructure
{
    public interface IMessageBus
    {
		void Subscribe(params IConsume[] subscribers);

        void Notify(Event @event);
    }
}