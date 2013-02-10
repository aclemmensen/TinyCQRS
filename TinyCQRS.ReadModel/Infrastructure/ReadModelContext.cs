using System.Data.Entity;
using System.Diagnostics;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Models;

namespace TinyCQRS.ReadModel.Infrastructure
{
	public class ReadModelContext : DbContext
	{
		public DbSet<Page> Pages { get; set; }
		public DbSet<Site> Sites { get; set; }
		public DbSet<Crawl> Crawls { get; set; }

		public bool DelayCommit { get; set; }

		public ReadModelContext() : base("TinyCQRS.ReadModel")
		{
			Configuration.ProxyCreationEnabled = true;
			Configuration.LazyLoadingEnabled = true;

			Trace.WriteLine("Created ReadModelContext");
		}

		public ReadModelContext(string name) : base(name) { }

		public ReadModelContext(bool delayCommit) : this()
		{
			DelayCommit = delayCommit;
			
			if (delayCommit)
			{
				Configuration.AutoDetectChangesEnabled = false;
			}
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Entity>().HasKey(x => x.GlobalId);
			modelBuilder.Entity<Dto>().HasKey(x => x.LocalId);

			modelBuilder.Entity<PageCheck>()
				.HasRequired(x => x.Page)
				.WithMany(x => x.Checks)
				.WillCascadeOnDelete(true);

			modelBuilder.Entity<Crawl>()
				.HasRequired(x => x.Site)
				.WithMany(x => x.Crawls)
				.WillCascadeOnDelete(false);

			base.OnModelCreating(modelBuilder);
		}

		public override int SaveChanges()
		{
			if (!DelayCommit)
			{
				ChangeTracker.DetectChanges();
				return base.SaveChanges();
			}

			return 0;
		}

		public int ExecuteDelayedCommits()
		{
			if (DelayCommit)
			{
				ChangeTracker.DetectChanges();
				return base.SaveChanges();
			}

			return 0;
		}

		protected override void Dispose(bool disposing)
		{
			Trace.WriteLine("Disposing ReadModelContext");
			base.Dispose(disposing);
		}
	}
}