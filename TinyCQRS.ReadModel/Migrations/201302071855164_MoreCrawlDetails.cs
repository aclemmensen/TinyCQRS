namespace TinyCQRS.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreCrawlDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Crawls", "OrderTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Crawls", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Crawls", "Status");
            DropColumn("dbo.Crawls", "OrderTime");
        }
    }
}
