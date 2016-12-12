namespace Kosmos.Extract.Tripadvisor.DbContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExtractResults",
                c => new
                    {
                        HashCode = c.String(nullable: false, maxLength: 32),
                        Result = c.String(),
                    })
                .PrimaryKey(t => t.HashCode);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExtractResults");
        }
    }
}
