namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_server_migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.msgHistories",
                c => new
                    {
                        idMsgHist = c.Int(nullable: false, identity: true),
                        User = c.String(),
                        dateTime = c.DateTime(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.idMsgHist);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        IdUser = c.Int(nullable: false, identity: true),
                        user = c.String(),
                        password = c.String(),
                        email = c.String(),
                        isLoggedIn = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdUser);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.msgHistories");
        }
    }
}
