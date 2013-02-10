namespace TinyCQRS.Contracts
{
	public interface IConsume
	{
		
	}

    public interface IConsume<in T> : IConsume where T : Event
    {
        void Process(T @event);
    }
}