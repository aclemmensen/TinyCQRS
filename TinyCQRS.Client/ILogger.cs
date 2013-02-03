using System.Diagnostics;

namespace TinyCQRS.Client
{
	public interface ILogger
	{
		void Log(string message, params object[] parameters);
		void Fail(string message, params object[] parameters);
	}

	public class TraceLogger : ILogger
	{
		public void Log(string message, params object[] parameters)
		{
			Trace.WriteLine(string.Format(message, parameters));
		}

		public void Fail(string message, params object[] parameters)
		{
			Trace.Fail(string.Format(message, parameters));
		}
	}

	public class NullLogger : ILogger
	{
		public void Log(string message, params object[] parameters)
		{
			
		}

		public void Fail(string message, params object[] parameters)
		{
			
		}
	}
}