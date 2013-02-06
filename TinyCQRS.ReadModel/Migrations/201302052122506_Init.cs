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
                "dbo.PageChecks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TimeOfCheck = c.DateTime(nullable: false),
                        PageId = c.Guid(nullable: false),
                        CrawlId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pages", t => t.PageId, cascadeDelete: true)
                .ForeignKey("dbo.Crawls", t => t.CrawlId, cascadeDelete: true)
                .Index(t => t.PageId)
                .Index(t => t.CrawlId);
            
            CreateTable(
                "dbo.Crawls",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SiteId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.SiteId)
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
            DropIndex("dbo.Crawls", new[] { "SiteId" });
            DropIndex("dbo.PageChecks", new[] { "CrawlId" });
            DropIndex("dbo.PageChecks", new[] { "PageId" });
            DropIndex("dbo.Pages", new[] { "SiteId" });
            DropForeignKey("dbo.Crawls", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.PageChecks", "CrawlId", "dbo.Crawls");
            DropForeignKey("dbo.PageChecks", "PageId", "dbo.Pages");
            DropForeignKey("dbo.Pages", "SiteId", "dbo.Sites");
            DropTable("dbo.Sites");
            DropTable("dbo.Crawls");
            DropTable("dbo.PageChecks");
            DropTable("dbo.Pages");
        }
    }
}
