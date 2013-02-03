namespace TinyCQRS.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventEnvelopes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AggregateId = c.Guid(nullable: false),
                        CorrelationId = c.Guid(nullable: false),
                        Version = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Data = c.String(),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EventEnvelopes");
        }
    }
}
