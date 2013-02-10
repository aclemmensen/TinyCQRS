using System.Data.Entity;
using TinyCQRS.Infrastructure.Persistence;

namespace TinyCQRS.Infrastructure.Database
{
	public class EventContext : DbContext
	{
		public DbSet<MessageEnvelope> Events { get; set; }

		public EventContext() : base("TinyCQRS.Events") { }

		public EventContext(string name) : base(name) { }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<MessageEnvelope>()
						.Ignore(x => x.Event);

			base.OnModelCreating(modelBuilder);
		}
	}
}