namespace TinyCQRS.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CrawlCounts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Crawls", "NewPages", c => c.Int(nullable: false));
            AddColumn("dbo.Crawls", "RemovedPages", c => c.Int(nullable: false));
            AddColumn("dbo.Crawls", "ChangedPages", c => c.Int(nullable: false));
            AddColumn("dbo.Crawls", "UnchangedPages", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Crawls", "UnchangedPages");
            DropColumn("dbo.Crawls", "ChangedPages");
            DropColumn("dbo.Crawls", "RemovedPages");
            DropColumn("dbo.Crawls", "NewPages");
        }
    }
}
