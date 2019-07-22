namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MetablockFrameScheduleEdit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetablockFrames", "DateToUse", c => c.DateTime());
            DropColumn("dbo.MetablockFrames", "DateToStart");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MetablockFrames", "DateToStart", c => c.DateTime());
            DropColumn("dbo.MetablockFrames", "DateToUse");
        }
    }
}
