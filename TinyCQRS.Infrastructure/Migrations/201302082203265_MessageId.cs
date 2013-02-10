namespace TinyCQRS.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessageId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventEnvelopes", "MessageId", c => c.Guid(nullable: false));
            AlterColumn("dbo.EventEnvelopes", "CorrelationId", c => c.Guid());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EventEnvelopes", "CorrelationId", c => c.Guid(nullable: false));
            DropColumn("dbo.EventEnvelopes", "MessageId");
        }
    }
}
