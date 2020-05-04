namespace HR.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPhoneNumbers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CandidatePhoneNumber",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.String(nullable: false),
                        CandidateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Candidate", t => t.CandidateId, cascadeDelete: true)
                .Index(t => t.CandidateId);
            
            DropColumn("dbo.Candidate", "PhoneNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Candidate", "PhoneNumber", c => c.String());
            DropForeignKey("dbo.CandidatePhoneNumber", "CandidateId", "dbo.Candidate");
            DropIndex("dbo.CandidatePhoneNumber", new[] { "CandidateId" });
            DropTable("dbo.CandidatePhoneNumber");
        }
    }
}
