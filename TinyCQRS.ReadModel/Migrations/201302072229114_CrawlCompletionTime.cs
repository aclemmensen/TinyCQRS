namespace TinyCQRS.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CrawlCompletionTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Crawls", "CompletionTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Crawls", "CompletionTime");
        }
    }
}
