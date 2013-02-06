namespace TinyCQRS.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CrawlStartTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Crawls", "StartTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Crawls", "StartTime");
        }
    }
}
