namespace CCBWebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitSQLServer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Loanapplbooks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Unn_Soc_Cr_Cd = c.String(nullable: false, maxLength: 50),
                        AcctBeginDate = c.DateTime(nullable: false),
                        CustName = c.String(nullable: false, maxLength: 50),
                        AR_Lmt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Lmt_ExDat = c.DateTime(nullable: false),
                        LoanBal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rfnd_AccNo = c.String(nullable: false, maxLength: 50),
                        UploadDate = c.DateTime(nullable: false),
                        AddUser = c.String(maxLength: 20),
                        AddDate = c.DateTime(nullable: false),
                        LastEditUser = c.String(maxLength: 20),
                        LastEditDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Unn_Soc_Cr_Cd, name: "IX_Loanapp_USCC");
            
            CreateTable(
                "dbo.LoanCompanies",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        KeyCode = c.String(nullable: false, maxLength: 50),
                        Trans_Id = c.String(maxLength: 50),
                        Trans_Code = c.String(maxLength: 50),
                        CoPlf_ID = c.String(nullable: false, maxLength: 50),
                        Unn_Soc_Cr_Cd = c.String(nullable: false, maxLength: 50),
                        Splr_Nm = c.String(nullable: false, maxLength: 100),
                        Pyr_Nm = c.String(nullable: false, maxLength: 50),
                        TLmt_Val = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LoanApl_Amt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Txn_ExDat = c.DateTime(nullable: false),
                        Rfnd_AccNo = c.String(nullable: false, maxLength: 50),
                        Remark = c.String(maxLength: 500),
                        Status = c.Int(nullable: false),
                        AuditStatus = c.Int(nullable: false),
                        AddUser = c.String(maxLength: 20),
                        AddDate = c.DateTime(nullable: false),
                        LastEditUser = c.String(maxLength: 20),
                        LastEditDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.KeyCode, unique: true, name: "IX_LoanCompany_KeyCode");
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsRequest = c.Boolean(nullable: false),
                        Trans_Id = c.String(maxLength: 50),
                        Trans_Code = c.String(maxLength: 50),
                        Content = c.String(),
                        AddUser = c.String(maxLength: 20),
                        AddDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResLoans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Trans_Id = c.String(nullable: false, maxLength: 50),
                        Trans_Code = c.String(nullable: false, maxLength: 50),
                        Unn_Soc_Cr_Cd = c.String(nullable: false, maxLength: 50),
                        Sgn_Cst_Nm = c.String(nullable: false, maxLength: 50),
                        CoPlf_ID = c.String(maxLength: 50),
                        Sign_Dt = c.DateTime(nullable: false),
                        AR_Lmt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Lmt_ExDat = c.DateTime(),
                        Rfnd_AccNo = c.String(maxLength: 50),
                        Remark = c.String(maxLength: 500),
                        Status = c.Int(nullable: false),
                        AuditStatus = c.Int(nullable: false),
                        AddUser = c.String(maxLength: 20),
                        AddDate = c.DateTime(nullable: false),
                        LastEditUser = c.String(maxLength: 20),
                        LastEditDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Trans_Id, unique: true, name: "IX_ResLoan_Key")
                .Index(t => t.Unn_Soc_Cr_Cd, name: "IX_ResLoan_USCC");
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ResLoans", "IX_ResLoan_USCC");
            DropIndex("dbo.ResLoans", "IX_ResLoan_Key");
            DropIndex("dbo.LoanCompanies", "IX_LoanCompany_KeyCode");
            DropIndex("dbo.Loanapplbooks", "IX_Loanapp_USCC");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ResLoans");
            DropTable("dbo.Messages");
            DropTable("dbo.LoanCompanies");
            DropTable("dbo.Loanapplbooks");
        }
    }
}
