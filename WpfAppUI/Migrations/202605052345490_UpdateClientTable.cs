namespace WpfAppUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateClientTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Balance", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Balance");
        }
    }
}
