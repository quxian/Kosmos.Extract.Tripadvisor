namespace Kosmos.Extract.Tripadvisor.DbContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_depth__ : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExtractResults", "Domain", c => c.String());
            AddColumn("dbo.ExtractResults", "Url", c => c.String());
            AddColumn("dbo.ExtractResults", "Depth", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExtractResults", "Depth");
            DropColumn("dbo.ExtractResults", "Url");
            DropColumn("dbo.ExtractResults", "Domain");
        }
    }
}
