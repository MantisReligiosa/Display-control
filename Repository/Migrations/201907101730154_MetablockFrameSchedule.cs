namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MetablockFrameSchedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetablockFrames", "UseInTimeInerval", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetablockFrames", "UseFromTime", c => c.DateTime());
            AddColumn("dbo.MetablockFrames", "UseToTime", c => c.DateTime());
            AddColumn("dbo.MetablockFrames", "UseInDayOfWeek", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetablockFrames", "UseInMon", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetablockFrames", "UseInTue", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetablockFrames", "UseInWed", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetablockFrames", "UseInThu", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetablockFrames", "UseInFri", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetablockFrames", "UseInSat", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetablockFrames", "UseInSun", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetablockFrames", "UseInDate", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetablockFrames", "DateToStart", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetablockFrames", "DateToStart");
            DropColumn("dbo.MetablockFrames", "UseInDate");
            DropColumn("dbo.MetablockFrames", "UseInSun");
            DropColumn("dbo.MetablockFrames", "UseInSat");
            DropColumn("dbo.MetablockFrames", "UseInFri");
            DropColumn("dbo.MetablockFrames", "UseInThu");
            DropColumn("dbo.MetablockFrames", "UseInWed");
            DropColumn("dbo.MetablockFrames", "UseInTue");
            DropColumn("dbo.MetablockFrames", "UseInMon");
            DropColumn("dbo.MetablockFrames", "UseInDayOfWeek");
            DropColumn("dbo.MetablockFrames", "UseToTime");
            DropColumn("dbo.MetablockFrames", "UseFromTime");
            DropColumn("dbo.MetablockFrames", "UseInTimeInerval");
        }
    }
}
