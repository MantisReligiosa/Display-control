namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FrameUseInTimeInterval : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetablockFrames", "UseInTimeInterval", c => c.Boolean(nullable: false));
            DropColumn("dbo.MetablockFrames", "UseInTimeInerval");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MetablockFrames", "UseInTimeInerval", c => c.Boolean(nullable: false));
            DropColumn("dbo.MetablockFrames", "UseInTimeInterval");
        }
    }
}
