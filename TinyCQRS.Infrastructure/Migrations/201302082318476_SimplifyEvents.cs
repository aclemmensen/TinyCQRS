namespace TinyCQRS.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SimplifyEvents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageEnvelopes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AggregateId = c.Guid(nullable: false),
                        MessageId = c.Guid(nullable: false),
                        CorrelationId = c.Guid(),
                        SessionId = c.Guid(),
                        Version = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Data = c.String(),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.EventEnvelopes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EventEnvelopes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AggregateId = c.Guid(nullable: false),
                        MessageId = c.Guid(nullable: false),
                        CorrelationId = c.Guid(),
                        Version = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Data = c.String(),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.MessageEnvelopes");
        }
    }
}
