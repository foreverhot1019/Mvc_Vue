﻿create table "FLBANKIF"."Loanapplbooks"
(
    "Id" number(10, 0) not null, 
    "Unn_Soc_Cr_Cd" nvarchar2(50) not null, 
    "AcctBeginDate" date not null, 
    "CustName" nvarchar2(50) not null, 
    "AR_Lmt" number(18, 2) not null, 
    "Lmt_ExDat" date not null, 
    "LoanBal" number(18, 2) not null, 
    "Rfnd_AccNo" nvarchar2(50) not null, 
    "UploadDate" date not null, 
    "AddUser" nvarchar2(20) null, 
    "AddDate" date not null, 
    "LastEditUser" nvarchar2(20) null, 
    "LastEditDate" date null,
    constraint "PK_Loanapplbooks" primary key ("Id")
) segment creation immediate
/

create sequence "FLBANKIF"."SQ_Loanapplbooks"
/

create or replace trigger "FLBANKIF"."TR_Loanapplbooks"
before insert on "FLBANKIF"."Loanapplbooks"
for each row
begin
  select "FLBANKIF"."SQ_Loanapplbooks".nextval into :new."Id" from dual;
end;
/

begin
  execute immediate
  'create index "FLBANKIF"."IX_Loanapplbooks_Unn_Soc_Cr_Cd" on "FLBANKIF"."Loanapplbooks" ("Unn_Soc_Cr_Cd")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "FLBANKIF"."LoanCompanies"
(
    "Id" raw(16) not null, 
    "KeyCode" nvarchar2(50) not null, 
    "Trans_Id" nvarchar2(50) null, 
    "Trans_Code" nvarchar2(50) null, 
    "CoPlf_ID" nvarchar2(50) not null, 
    "Unn_Soc_Cr_Cd" nvarchar2(50) not null, 
    "Splr_Nm" nvarchar2(100) not null, 
    "Pyr_Nm" nvarchar2(50) not null, 
    "TLmt_Val" number(18, 2) not null, 
    "LoanApl_Amt" number(18, 2) not null, 
    "Txn_ExDat" date not null, 
    "Rfnd_AccNo" nvarchar2(50) not null, 
    "Remark" nvarchar2(500) null, 
    "Status" number(10, 0) not null, 
    "AuditStatus" number(10, 0) not null, 
    "AddUser" nvarchar2(20) null, 
    "AddDate" date not null, 
    "LastEditUser" nvarchar2(20) null, 
    "LastEditDate" date null,
    constraint "PK_LoanCompanies" primary key ("Id")
) segment creation immediate
/

create sequence "FLBANKIF"."SQ_LoanCompanies"
/

create or replace trigger "FLBANKIF"."TR_LoanCompanies"
before insert on "FLBANKIF"."LoanCompanies"
for each row
begin
  select "FLBANKIF"."SQ_LoanCompanies".nextval into :new."Id" from dual;
end;
/

begin
  execute immediate
  'create unique index "FLBANKIF"."IX_LoanCompanies_KeyCode" on "FLBANKIF"."LoanCompanies" ("KeyCode")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "FLBANKIF"."Messages"
(
    "Id" number(10, 0) not null, 
    "IsRequest" number(1, 0) not null, 
    "Trans_Id" nvarchar2(50) null, 
    "Trans_Code" nvarchar2(50) null, 
    "Content" nclob null, 
    "AddUser" nvarchar2(20) null, 
    "AddDate" date not null,
    constraint "PK_Messages" primary key ("Id")
) segment creation immediate
/

create sequence "FLBANKIF"."SQ_Messages"
/

create or replace trigger "FLBANKIF"."TR_Messages"
before insert on "FLBANKIF"."Messages"
for each row
begin
  select "FLBANKIF"."SQ_Messages".nextval into :new."Id" from dual;
end;
/

create table "FLBANKIF"."ResLoans"
(
    "Id" number(10, 0) not null, 
    "Trans_Id" nvarchar2(50) not null, 
    "Trans_Code" nvarchar2(50) not null, 
    "Unn_Soc_Cr_Cd" nvarchar2(50) not null, 
    "Sgn_Cst_Nm" nvarchar2(50) not null, 
    "CoPlf_ID" nvarchar2(50) null, 
    "Sign_Dt" date not null, 
    "AR_Lmt" number(18, 2) not null, 
    "Lmt_ExDat" date null, 
    "Rfnd_AccNo" nvarchar2(50) null, 
    "Remark" nvarchar2(500) null, 
    "Status" number(10, 0) not null, 
    "AuditStatus" number(10, 0) not null, 
    "AddUser" nvarchar2(20) null, 
    "AddDate" date not null, 
    "LastEditUser" nvarchar2(20) null, 
    "LastEditDate" date null,
    constraint "PK_ResLoans" primary key ("Id")
) segment creation immediate
/

create sequence "FLBANKIF"."SQ_ResLoans"
/

create or replace trigger "FLBANKIF"."TR_ResLoans"
before insert on "FLBANKIF"."ResLoans"
for each row
begin
  select "FLBANKIF"."SQ_ResLoans".nextval into :new."Id" from dual;
end;
/

begin
  execute immediate
  'create unique index "FLBANKIF"."IX_ResLoans_Trans_Id" on "FLBANKIF"."ResLoans" ("Trans_Id")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "FLBANKIF"."IX_ResLoans_Unn_Soc_Cr_Cd" on "FLBANKIF"."ResLoans" ("Unn_Soc_Cr_Cd")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "FLBANKIF"."AspNetRoles"
(
    "Id" nvarchar2(128) not null, 
    "Name" nvarchar2(256) not null,
    constraint "PK_AspNetRoles" primary key ("Id")
) segment creation immediate
/

begin
  execute immediate
  'create unique index "FLBANKIF"."IX_AspNetRoles_Name" on "FLBANKIF"."AspNetRoles" ("Name")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "FLBANKIF"."AspNetUserRoles"
(
    "UserId" nvarchar2(128) not null, 
    "RoleId" nvarchar2(128) not null,
    constraint "PK_AspNetUserRoles" primary key ("UserId", "RoleId")
) segment creation immediate
/

begin
  execute immediate
  'create index "FLBANKIF"."IX_AspNetUserRoles_UserId" on "FLBANKIF"."AspNetUserRoles" ("UserId")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "FLBANKIF"."IX_AspNetUserRoles_RoleId" on "FLBANKIF"."AspNetUserRoles" ("RoleId")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "FLBANKIF"."AspNetUsers"
(
    "Id" nvarchar2(128) not null, 
    "Email" nvarchar2(256) null, 
    "EmailConfirmed" number(1, 0) not null, 
    "PasswordHash" nclob null, 
    "SecurityStamp" nclob null, 
    "PhoneNumber" nclob null, 
    "PhoneNumberConfirmed" number(1, 0) not null, 
    "TwoFactorEnabled" number(1, 0) not null, 
    "LockoutEndDateUtc" date null, 
    "LockoutEnabled" number(1, 0) not null, 
    "AccessFailedCount" number(10, 0) not null, 
    "UserName" nvarchar2(256) not null,
    constraint "PK_AspNetUsers" primary key ("Id")
) segment creation immediate
/

begin
  execute immediate
  'create unique index "FLBANKIF"."IX_AspNetUsers_UserName" on "FLBANKIF"."AspNetUsers" ("UserName")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "FLBANKIF"."AspNetUserClaims"
(
    "Id" number(10, 0) not null, 
    "UserId" nvarchar2(128) not null, 
    "ClaimType" nclob null, 
    "ClaimValue" nclob null,
    constraint "PK_AspNetUserClaims" primary key ("Id")
) segment creation immediate
/

create sequence "FLBANKIF"."SQ_AspNetUserClaims"
/

create or replace trigger "FLBANKIF"."TR_AspNetUserClaims"
before insert on "FLBANKIF"."AspNetUserClaims"
for each row
begin
  select "FLBANKIF"."SQ_AspNetUserClaims".nextval into :new."Id" from dual;
end;
/

begin
  execute immediate
  'create index "FLBANKIF"."IX_AspNetUserClaims_UserId" on "FLBANKIF"."AspNetUserClaims" ("UserId")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "FLBANKIF"."AspNetUserLogins"
(
    "LoginProvider" nvarchar2(128) not null, 
    "ProviderKey" nvarchar2(128) not null, 
    "UserId" nvarchar2(128) not null,
    constraint "PK_AspNetUserLogins" primary key ("LoginProvider", "ProviderKey", "UserId")
) segment creation immediate
/

begin
  execute immediate
  'create index "FLBANKIF"."IX_AspNetUserLogins_UserId" on "FLBANKIF"."AspNetUserLogins" ("UserId")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

alter table "FLBANKIF"."AspNetUserRoles" add constraint "FK_AspNetUserRoles_RoleId" foreign key ("RoleId") references "FLBANKIF"."AspNetRoles" ("Id") on delete cascade
/

alter table "FLBANKIF"."AspNetUserRoles" add constraint "FK_AspNetUserRoles_UserId" foreign key ("UserId") references "FLBANKIF"."AspNetUsers" ("Id") on delete cascade
/

alter table "FLBANKIF"."AspNetUserClaims" add constraint "FK_AspNetUserClaims_UserId" foreign key ("UserId") references "FLBANKIF"."AspNetUsers" ("Id") on delete cascade
/

alter table "FLBANKIF"."AspNetUserLogins" add constraint "FK_AspNetUserLogins_UserId" foreign key ("UserId") references "FLBANKIF"."AspNetUsers" ("Id") on delete cascade
/

create table "FLBANKIF"."__MigrationHistory"
(
    "MigrationId" nvarchar2(150) not null, 
    "ContextKey" nvarchar2(300) not null, 
    "Model" blob not null, 
    "ProductVersion" nvarchar2(32) not null,
    constraint "PK___MigrationHistory" primary key ("MigrationId", "ContextKey")
) segment creation immediate
/

declare
model_blob blob;
begin
dbms_lob.createtemporary(model_blob, true);
dbms_lob.append(model_blob, to_blob(cast('1F8B0800000000000400ED1DD96EE4B8F13D40FE41E8C764C6ED2333980CEC5D78DAEE8DB1BEE0B637796BD012BB2D8C44F5EA98B111EC97E5219F945F082951120F5112D5945A1E187E69F3A82A168B755064F17FFFF9EFF1CFCFBE677D8361E406E86472B0B73FB120B203C745EB934912AFDE7F9AFCFCD39FFF747CEEF8CFD66F79BB23D20EF744D1C9E4298E379FA7D3C87E823E88F67CD70E832858C57B76E04F81134C0FF7F7FF3E3D3898420C62826159D6F15D8262D787E93FF8DF59806CB88913E05D050EF4225A8E6B162954EB1AF830DA001B9E4C66B32FFF848FA71B772F6B3BB14E3D17603A16D05B4D2C805010831853F9F921828B380CD07AB1C105C0BB7FD940DC6E05BC0852EA3F97CDDB0E64FF900C645A76CC41D9491407BE26C08323CA99A9D8BD137F2705E730EFCE318FE31732EA947F2793CB0020B0D9788F41F0756289083FCFBC90349659BCC7767C6715D5EF0A81C07243FE705DE2C549084F104CE21078EFACDBE4D173ED5FE1CB7DF015A21394781E4B242613D77105B8E8360C36308C5FEEE08A927EE14CAC29DF6F2A762CBA317DB2015DA0F8E870625D63E4E0D183850C30835FC441087F8108862086CE2D886318220203A65C94B00BB81E105A2E027B390B97B3022D963EBC8C26D61578BE84681D3F9D4C3EE07533779FA19317504A1E908B171DEE138709ACA0B41EFBA96DC75FE0DA456798F81C3BF97D8F179936B419160CF26BF861DC2D2FFDB8A01FDAAE0FBC89751BE25F543F7D9A580B1B108855F3590F1EC35E9E3F63B66CCD21B21EBE10D27AA2F46E859C259ED4EB60F03978D87801708CC8D1A9E360151CD60CE1B0DD101AD11821F71244F1B9E3C643D09CE3AA269CED7B3C2DD578A3729F05FE06A0176DDD4EFBBD46D5FE4BE23ABD6A764CCA0C736AF085781F02142D2F0C18933678CC0CB1C1AE04B7DE6A797136BC4EDBA9715E6CBC7079EDD7E03DD8EF05F1ED4B03DE9E0497D8D9DF7AB48E44639D6EBCE5698FBEC2FD3332E42BECD096DFE18021FC5A8BD6C4BA5E60459B44051A1C86ED6545E728F1F5BD8604DBC50A904C7937B86FDE48132E83DEC8158C22B0865A9E08EDF31ABD90FE03CC8BE80EFE9EC0A850485F82C08300BDF915D8AF403166620D1AFCD3CCD21EAF0669BD32EF60440CA8D6CAA47DDE5666BF0BAAD34ADE4970B2638F7A8D96B328DE85736B2E866918A28BC778B6BDF7B9BB2DB5E1BCE23727F8CD091EA5139C5B903BECA9D5D9DBABE293CE69B4B986F15EDE712F03390F31B8EF41F8758F85F8CE6ADDAFB4D6876DADF5D1C1E3EAE8D3878FC039FAF83778F46178CB5DB55172F8A90FA5DEF0B1E3F0C3472358AFC137779D4EBD684DB178E3757E07BDB4367A723774BDB3F3BDA4CDE661E093FF79F9CA6AB1514E429B0C265036B907E11AC65B8A3401655E' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('AC73A8E3176D42A92CDE954DC980BAAC841CC5D0AB21A7B75FBCAD25EE74B3C193978A56660634E216A1EF6B8C5F869BF7731FB89E0135D8020B8ED8576EE84367DBCD8C5B1045580B38FF00D153EFF1FF02DA49888513FB5FFEA6776CB74F0182D789FF58EBFA18C7656C6AEEBF077360E3D0FA1C915E5BC3BB0CECAF41129FA3D4B57C886DDD90A30060841C1CBBC0289A636186CE2C48CAED27D5864343408DD5D3AEDD9099075CBFDA0F1114E9326F5AFA22D52D247F44D1ACCA27A923F53258BBA81DA9795335A9598B465269335D5209B07694D2966A42D3068D7466AD8C7979E90C9977F352B0E3F7F3C6BEF9B82B27319D3E82B477DB9462FA0D788969549D5643AA04CCAF8614ECF857434A262EFEE63AC42B6911FCE48D31F856EDABE3AAE635275036F472E0863934F26174807AB9243EB358D8CDC98B68EE817579E45A2372CBA1FEB58467207AC303C693E4BD6006B10A9AE7EE15247E301DCE590856D88AA6FAE764B22F4D05D738732D8BD607F5ADCFDC886BFEFE406678C6841A764B1BC226782E00DD09E31D1DC6A704A752DF8EF5D90093D4852FFA1CB6E8334F4364AD093B8DA2C07653DE576C0BD34D3D1E2F8E71ACE61DBEF2AB00BFCF7C8527C4252E212E219C1095EE0D3A831E8CA1756A67D7146620B28123AB193C1C4783B0DCE3AC20ACDC2DE489FB8B84135B0218924E806C124458A25C14CB66C345B6BB015E2397849E2D5D3C32F60287587306371011848D9C6883BC7A73901050E01126A58943C75346E2EA055111D5A9E6BC29C4633E53897B7683C864436CA9904B1ADFF42298F51C1B4038EB59D28600E546F72E0494C6F26D05400CECC726A0C28E82424069C8318880F21CDB8180F22C7975029A6DE1B49D7F613F676CE2C96F240D6FD66BD9B503D9E4F83132D1CC6233721212F7287DD67204678FE931C9E7B82224C074D2A820A2A1A0282204F802C695F748CBB0908A075F3D6D86555C5BAA0455D436402A8E1C4B508A9A0608C5D148094251D30421DBC295FAF31E7A03107179D5012C976003507A78400224A91A0DE2F2AF00B5D451FF4A036CBE635F0B965A45012CB33A64D8EC210AA6A1FAA885B86C5BC565C5C80A6990967FAB308A81532110A25AE707DE8229AA2F3A3263DA44093A710233303A19350C6AF0E9154CCA07639C4BB9683673A9CA55D57156B7E292E0582AB8940FC63897A88C3633A9C25DD27098B66211EFDC185A6CF91E6961878BBAE36996C982161C4F15292F8EAFB0E574D19A4981414BAC05CD7FF17EA19F1AC2CF604C6D8EDBA2D750608A8310DB4BA1961C7E76E0DC0D237246123C02B2BD38737CA959A5D7A150FF39CA0AC7429ECBDC1CE49DC86FFA91A82E634585AF4621CCF1307DE2F0A55FE11821A8EF6E91BC24C00361C587BF59E0253E52FB9FEADEC2997A1690' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('50D51EA6901882852954B58759A68760C195A51AD4D1C3E91C59B4AC3D14E60C3A0B8829D680952776E020E585EDE1B0C7DB59506CB9866430391938B160CA35B89E9F01E7D89E176AC1A9902A479B1EFE9037C778AE461FA24C1E5F23433C9E0A9A418ADC242D24C5D7BC666BADF7F230676BB5A700D452EB297BF7A3F48AEC0A2C88A2B03D9CF212160BA82CD5852413C5966B28CBE2BE10A72C8BD2DD9A872235010BAD286C0F27CF34C082C9CB34385F240EE0F85E94EAA9F1224380A8CA8B0A0DCACA6C001C6965F1AE4C437EBB898344CB34E480DE34E2C48096691804F6D2126714D88A3743F58A0D55BE93B6959152006961A0943DFB314ECC9D7B0E4859FC2318287A799EB74FB470FC8B75470B21DF10DE6A212880B45808CA9EFD2C84B10A6F2F3E1173B99CB3874CF92EFCBFE2463847545EF89A83EE378F68EC4AF6CD23521B02C516B68619E03E0EEADB82FAEEFD180479FF4FB5F7B7A3699177F18D4C51F1B9B5FB34A94128AD1C3D46C09937C5D1023594FCA421A71715A70F77366DAAEFAE1A53257ED0D69FA94608FDAC297AFF9605408B346130573825604C9DC6F60E77CB96DBE4E16A34CC2B7F9596B3B27C950695EC85598E48B6A2133C0547AB5B6838C3D21559CE25966A7536C0A4CBB2FC369854DD017605CD629DD6972AF13EADF0B54AACD608138ACBB5A20A1DB1D5521EB3E868B6B27338DBD92D058C9EBE871A317BCC45452E022B8B3561D1AB8812305A3E4A59521E46E9284BD9E1ABED64490143AD75B80B7EBCD2A9BD95A886C9DDDAE3147BDDAD45353C3D89ED552EA493286293023B2D29FE2F4EA2D053202D5E64118F85644D4896B98C8D27939B10D81EDCBB020843244607640A7D6FE6B9E94663DE14B77057308AB3BB63938383BD43E18197F13CB6328D22C7AB384FC35CCF539F2319E2FA39A23E0F9BED6FBFC8F6477E355D3E379116127D03A1FD04C2C38A93C717C881CF27937FA7FD3F5B17FF5A528E2D1F16B3D93BEB26C432F1D93A78675D440FC8FD3DC18DE68412EB0F2953A08937539CF4F796EFA53003DE92442E7963D56C1ACDDDD869F0C25328A68994F3441AE3AEFCD2492706082915ABC93BE4C8EB9042B1DBDC54A44F34455F55BA4491C82D722E288FA20CA137D789EB5821F86E5E410A8F89E8AB46CA93250554AD21EF31DFB75790625EE3360BAF5B62705390C5E4BCE65445835DDB0EBAF0404835DCF48D90AD1E003146AFF8C08771C324BFEF611A85F4BC4727FDDAA371E2B316ABC06AAF103EA570A37FB87DD662E328DEAC6D0FD6B6F25CCD0F19A148CF685451B0857CBE46A3C9BD9E816C2F78ECB07CC6B12AB55FC1F8F125BE46225BF89C944FC4DFECDFD7D415EFA1F6247226F4BE27213F6C618C0D5D7CE30E0F55740BA177BBCFD1EA15894EAE9EFEFB146F9EDEE86CCA' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('0FE5E9A90F0B0D62FCAA435B929E708B271B3494391936F995163798B134DFB2196FA3C5AB0905A17FBC8AE708F40C68D6B7309DFB02A7BB0800FF4A81A63D4FFB6E414EE7C70B5EF18AE39E0750683B7EC5747F0EC0705056F54A40A738A7F205804E902AB2FB6F0BA76F2EAA12FA1B46A3CCF3DFC56DABCEF96F9862E55300A65D22F191000DAD9777DDA1E1AB383CF44306DC23339452A6F64E7A46CEC2DE028CD10CEBDB3948AF2C73B939B37D2B672637077CA7A2AE922E630997C79263B98C1A2AE81930B5F290D9946B2E5AB40D69ACD790447904693F2B92F529E46CC054C943CB9AEA74B446A83DFE84C82313366AE915C23660DAE3A1854D757C7AE4C2A695DC7864B2B62BFBB963496B6D42779EAA58CE2D284E2B9F30562B07717620FD6432BFFC727AFDEBC51C4B42E656B2CDE4448F55385BE62AAEC598B5722B524B8A185BE4345662A22D9A91B4487BAC44425B3423E1FD6709135FAD44973D8B559D965385B15CE14AAC659306CCEAA4A0227649E549C8A5162D70EB8D9AFA6BB5C3A66D5AE05624D5AD2380DAF05A02689B160428F2D5EE22E77365C6D8AA3CDC0D06A9EE7EB864EA469CE3991B49434AF1A6E0A3F6EEA179A6F497D2D90853B825A4B844679E297D657036C212934B472363B37C1F0E3B4109EEED67DB4FD8118BDC7509E218C344D0E6DC9FA2CD055A05B9172650943711F6DAAE600C1C72712E8CDD15B0635C4D3E3BB0AF9E9DFB8FD0B9403749BC49623C64E83F7ADCE625F1E6EAF0A769A9799A8F6F36E4BFC8C41030992EF98A7383BE24AEE71474CF2B36F71420889B48F7EAC95CC664CF7EFD5240BA0E504B40947D85777B0FFD8D878145376801BEC12EB461F1BB846B60BF94BBB92A20CD13C1B3FDF8CC05EB10F8118551F6C7FF621976FCE79FFE0FCBE48310CE9F0000' as long raw)));
insert into "FLBANKIF"."__MigrationHistory"("MigrationId", "ContextKey", "Model", "ProductVersion")
values ('201905240154426_initWeb', 'CCBWebApi.Migrations.Configuration', model_blob, '6.2.0-61023');
end;
