namespace WpfAppUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bicycles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Model = c.String(),
                        Status = c.String(),
                        StationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stations", t => t.StationId)
                .Index(t => t.StationId);
            
            CreateTable(
                "dbo.Rentals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        BicycleId = c.Int(nullable: false),
                        StartStationId = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(),
                        EndStationId = c.Int(),
                        Cost = c.Decimal(precision: 18, scale: 2),
                        TariffId = c.Int(nullable: false),
                        Station_Id = c.Int(),
                        Station_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bicycles", t => t.BicycleId, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Stations", t => t.Station_Id)
                .ForeignKey("dbo.Stations", t => t.Station_Id1)
                .ForeignKey("dbo.Stations", t => t.EndStationId)
                .ForeignKey("dbo.Stations", t => t.StartStationId, cascadeDelete: true)
                .ForeignKey("dbo.Tariffs", t => t.TariffId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.BicycleId)
                .Index(t => t.StartStationId)
                .Index(t => t.EndStationId)
                .Index(t => t.TariffId)
                .Index(t => t.Station_Id)
                .Index(t => t.Station_Id1);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        FullName = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        PasswordHash = c.String(),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RentalId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaidAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rentals", t => t.RentalId, cascadeDelete: true)
                .Index(t => t.RentalId);
            
            CreateTable(
                "dbo.Tariffs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PricePerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rentals", "TariffId", "dbo.Tariffs");
            DropForeignKey("dbo.Rentals", "StartStationId", "dbo.Stations");
            DropForeignKey("dbo.Payments", "RentalId", "dbo.Rentals");
            DropForeignKey("dbo.Rentals", "EndStationId", "dbo.Stations");
            DropForeignKey("dbo.Rentals", "Station_Id1", "dbo.Stations");
            DropForeignKey("dbo.Rentals", "Station_Id", "dbo.Stations");
            DropForeignKey("dbo.Bicycles", "StationId", "dbo.Stations");
            DropForeignKey("dbo.Clients", "UserId", "dbo.Users");
            DropForeignKey("dbo.Rentals", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Rentals", "BicycleId", "dbo.Bicycles");
            DropIndex("dbo.Payments", new[] { "RentalId" });
            DropIndex("dbo.Clients", new[] { "UserId" });
            DropIndex("dbo.Rentals", new[] { "Station_Id1" });
            DropIndex("dbo.Rentals", new[] { "Station_Id" });
            DropIndex("dbo.Rentals", new[] { "TariffId" });
            DropIndex("dbo.Rentals", new[] { "EndStationId" });
            DropIndex("dbo.Rentals", new[] { "StartStationId" });
            DropIndex("dbo.Rentals", new[] { "BicycleId" });
            DropIndex("dbo.Rentals", new[] { "ClientId" });
            DropIndex("dbo.Bicycles", new[] { "StationId" });
            DropTable("dbo.Tariffs");
            DropTable("dbo.Payments");
            DropTable("dbo.Stations");
            DropTable("dbo.Users");
            DropTable("dbo.Clients");
            DropTable("dbo.Rentals");
            DropTable("dbo.Bicycles");
        }
    }
}
