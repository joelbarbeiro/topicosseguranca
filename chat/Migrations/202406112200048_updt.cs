namespace chat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updt : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ControlClients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PublicKey = c.String(),
                        State = c.Boolean(nullable: false),
                        User = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MessageChats",
                c => new
                    {
                        MsgId = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        User = c.String(),
                        Data = c.DateTime(nullable: false),
                        Messag = c.String(),
                    })
                .PrimaryKey(t => t.MsgId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MessageChats");
            DropTable("dbo.ControlClients");
        }
    }
}
