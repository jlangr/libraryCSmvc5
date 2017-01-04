namespace Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Holdings", "DueDate", c => c.DateTime());
            AlterColumn("dbo.Holdings", "CheckOutTimestamp", c => c.DateTime());
            AlterColumn("dbo.Holdings", "LastCheckedIn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Holdings", "LastCheckedIn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Holdings", "CheckOutTimestamp", c => c.DateTime(nullable: false));
            DropColumn("dbo.Holdings", "DueDate");
        }
    }
}
