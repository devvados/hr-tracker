namespace HR.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Candidate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Patronymic = c.String(),
                        Email = c.String(maxLength: 50),
                        PhoneNumber = c.String(),
                        CompanyId = c.Int(),
                        PositionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.Position", t => t.PositionId)
                .Index(t => t.CompanyId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Position",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Candidate", "PositionId", "dbo.Position");
            DropForeignKey("dbo.Candidate", "CompanyId", "dbo.Company");
            DropIndex("dbo.Candidate", new[] { "PositionId" });
            DropIndex("dbo.Candidate", new[] { "CompanyId" });
            DropTable("dbo.Position");
            DropTable("dbo.Company");
            DropTable("dbo.Candidate");
        }
    }
}
