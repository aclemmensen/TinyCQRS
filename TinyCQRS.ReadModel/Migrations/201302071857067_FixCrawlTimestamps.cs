namespace TinyCQRS.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixCrawlTimestamps : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Crawls", "OrderTime", c => c.DateTime());
            AlterColumn("dbo.Crawls", "StartTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Crawls", "StartTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Crawls", "OrderTime", c => c.DateTime(nullable: false));
        }
    }
}
