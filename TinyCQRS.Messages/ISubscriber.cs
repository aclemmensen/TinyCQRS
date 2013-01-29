namespace TinyCQRS.Messages
{
    public interface ISubscriber
    {
        
    }

    public interface ISubscribeTo<in T> : ISubscriber where T : Event
    {
        void Process(T @event);
    }
}