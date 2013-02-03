namespace TinyCQRS.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Url = c.String(),
                        Content = c.String(),
                        SiteId = c.Guid(nullable: false),
                        FirstSeen = c.DateTime(),
                        LastChecked = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.CrawlRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TimeOfCheck = c.DateTime(nullable: false),
                        CrawlJobId = c.Guid(nullable: false),
                        PageId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CrawlJobs", t => t.CrawlJobId, cascadeDelete: true)
                .ForeignKey("dbo.Pages", t => t.PageId)
                .Index(t => t.CrawlJobId)
                .Index(t => t.PageId);
            
            CreateTable(
                "dbo.CrawlJobs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SiteId = c.Guid(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.Sites",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Root = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CrawlJobs", new[] { "SiteId" });
            DropIndex("dbo.CrawlRecords", new[] { "PageId" });
            DropIndex("dbo.CrawlRecords", new[] { "CrawlJobId" });
            DropIndex("dbo.Pages", new[] { "SiteId" });
            DropForeignKey("dbo.CrawlJobs", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.CrawlRecords", "PageId", "dbo.Pages");
            DropForeignKey("dbo.CrawlRecords", "CrawlJobId", "dbo.CrawlJobs");
            DropForeignKey("dbo.Pages", "SiteId", "dbo.Sites");
            DropTable("dbo.Sites");
            DropTable("dbo.CrawlJobs");
            DropTable("dbo.CrawlRecords");
            DropTable("dbo.Pages");
        }
    }
}
