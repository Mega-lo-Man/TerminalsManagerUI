namespace TerminalsManagerUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IgnoreDesignation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BaseComponents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ErpCode = c.String(),
                        Brand = c.String(),
                        WiresNumber = c.Int(),
                        IsArmoured = c.Boolean(),
                        DeviceName = c.String(),
                        DeviceDescription = c.String(),
                        TerminalString = c.String(),
                        NumbersOfCable = c.Int(),
                        BlockRef = c.String(),
                        ImagePath = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BaseComponents");
        }
    }
}
