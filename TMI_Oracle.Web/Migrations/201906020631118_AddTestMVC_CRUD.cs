namespace TMI.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTestMVC_CRUD : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TESTMVC_CRUDS",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    DZBH = c.String(maxLength: 50),
                    TESTCOLUMN1 = c.String(maxLength: 50),
                    TESTCOLUMN2 = c.String(maxLength: 50),
                    TESTCOLUMN3 = c.String(maxLength: 50),
                    TESTCOLUMN4 = c.String(maxLength: 50),
                    TESTCOLUMN5 = c.String(maxLength: 50),
                    OPERATINGPOINT = c.Int(nullable: false),
                    ADDID = c.String(maxLength: 50),
                    ADDWHO = c.String(maxLength: 20),
                    ADDTS = c.DateTime(),
                    EDITWHO = c.String(maxLength: 20),
                    EDITTS = c.DateTime(),
                    EDITID = c.String(maxLength: 50),
                })
                .PrimaryKey(t => t.ID);
        }

        public override void Down() { }
    }
}
