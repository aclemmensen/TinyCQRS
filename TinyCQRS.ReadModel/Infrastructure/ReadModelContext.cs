using System.Data.Entity;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.ReadModel.Infrastructure
{
	public class ReadModelContext : DbContext
	{
		public DbSet<Page> Pages { get; set; }
		public DbSet<Site> Sites { get; set; }
		public DbSet<CrawlJob> Crawls { get; set; }

		public bool DelayCommit { get; set; }

		public ReadModelContext() : base("TinyCQRS.ReadModel")
		{
			Configuration.ProxyCreationEnabled = true;
			Configuration.ProxyCreationEnabled = true;
		}

		public ReadModelContext(string name) : base(name) { }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<Site>()
			//			.HasKey(x => x.Id);

			//modelBuilder.Entity<Page>()
			//			.HasKey(x => x.Id);

			//modelBuilder.Entity<CrawlJob>()
			//			.HasKey(x => x.Id);

			modelBuilder.Entity<CrawlRecord>()
						.HasRequired(x => x.Page)
						.WithMany(x => x.CrawlRecords)
						.WillCascadeOnDelete(false);

			modelBuilder.Entity<CrawlJob>()
						.HasRequired(x => x.Site)
						.WithMany(x => x.Crawls)
						.HasForeignKey(x => x.SiteId);

			base.OnModelCreating(modelBuilder);
		}

		public override int SaveChanges()
		{
			return !DelayCommit ? base.SaveChanges() : 0;
		}

		public int ExecuteDelayedCommits()
		{
			return DelayCommit ? base.SaveChanges() : 0;
		}
	}
}