namespace Kosmos.Extract.Tripadvisor.DbContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ExtractData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExtractResults", "ExtractData", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExtractResults", "ExtractData");
        }
    }
}
