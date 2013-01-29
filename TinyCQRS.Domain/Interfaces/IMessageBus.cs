using TinyCQRS.Messages;

namespace TinyCQRS.Domain.Interfaces
{
    public interface IMessageBus
    {
        void Subscribe(ISubscriber subscriber);

        void Notify(Event @event);
    }
}