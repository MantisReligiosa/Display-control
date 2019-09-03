namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MetablockFrameTimespan : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MetablockFrames", "UseFromTime", c => c.Time(precision: 7));
            AlterColumn("dbo.MetablockFrames", "UseToTime", c => c.Time(precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MetablockFrames", "UseToTime", c => c.DateTime());
            AlterColumn("dbo.MetablockFrames", "UseFromTime", c => c.DateTime());
        }
    }
}
