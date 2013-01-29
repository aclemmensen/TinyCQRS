namespace TinyCQRS.Messages
{
    public interface IHandler
    {

    }

    public interface IHandler<T> : IHandler where T : Command
    {
        void Handle(T command);
    }
}