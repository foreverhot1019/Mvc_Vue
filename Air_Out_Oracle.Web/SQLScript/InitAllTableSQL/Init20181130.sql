create table "AIROUTE"."BASECODES"
(
    "ID" number(10, 0) not null, 
    "CODETYPE" nvarchar2(30) not null, 
    "DESCRIPTION" nvarchar2(50) not null,
    constraint "PK_BASECODES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_BASECODES"
/

create or replace trigger "AIROUTE"."TR_BASECODES"
before insert on "AIROUTE"."BASECODES"
for each row
begin
  select "AIROUTE"."SQ_BASECODES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_BASECODES_CODETYPE" on "AIROUTE"."BASECODES" ("CODETYPE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."CODEITEMS"
(
    "ID" number(10, 0) not null, 
    "CODE" nvarchar2(30) not null, 
    "TEXT" nvarchar2(50) not null, 
    "DESCRIPTION" nvarchar2(50) null, 
    "BASECODEID" number(10, 0) not null,
    constraint "PK_CODEITEMS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_CODEITEMS"
/

create or replace trigger "AIROUTE"."TR_CODEITEMS"
before insert on "AIROUTE"."CODEITEMS"
for each row
begin
  select "AIROUTE"."SQ_CODEITEMS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_CODEITEMS_CODE_BASECODEID" on "AIROUTE"."CODEITEMS" ("CODE", "BASECODEID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."BD_DEFDOCS"
(
    "ID" number(10, 0) not null, 
    "DOCCODE" nvarchar2(20) not null, 
    "DOCNAME" nvarchar2(20) not null, 
    "REMARK" nvarchar2(50) null, 
    "STATUS" nvarchar2(10) not null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "OPERATINGPOINT" number(10, 0) not null,
    constraint "PK_BD_DEFDOCS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_BD_DEFDOCS"
/

create or replace trigger "AIROUTE"."TR_BD_DEFDOCS"
before insert on "AIROUTE"."BD_DEFDOCS"
for each row
begin
  select "AIROUTE"."SQ_BD_DEFDOCS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_BD_DEFDOCS_DOCCODE" on "AIROUTE"."BD_DEFDOCS" ("DOCCODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BD_DEFDOCS_OPERATINGPOINT" on "AIROUTE"."BD_DEFDOCS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."BD_DEFDOC_LISTS"
(
    "DOCID" number(10, 0) not null, 
    "DOCCODE" nvarchar2(50) not null, 
    "LISTCODE" nvarchar2(50) not null, 
    "ID" number(10, 0) not null, 
    "LISTNAME" nvarchar2(50) not null, 
    "LISTFULLNAME" nvarchar2(50) null, 
    "ENAME" nvarchar2(50) null, 
    "REMARK" nvarchar2(50) null, 
    "STATUS" nvarchar2(10) not null, 
    "SORTNUM" number(10, 0) not null, 
    "R_CODE" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "OPERATINGPOINT" number(10, 0) not null,
    constraint "PK_BD_DEFDOC_LISTS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_BD_DEFDOC_LISTS"
/

create or replace trigger "AIROUTE"."TR_BD_DEFDOC_LISTS"
before insert on "AIROUTE"."BD_DEFDOC_LISTS"
for each row
begin
  select "AIROUTE"."SQ_BD_DEFDOC_LISTS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_BD_DEFDOC_LISTS_D_726163164" on "AIROUTE"."BD_DEFDOC_LISTS" ("DOCID", "DOCCODE", "LISTCODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BD_DEFDOC_LISTS__1983832769" on "AIROUTE"."BD_DEFDOC_LISTS" ("DOCCODE", "LISTCODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BD_DEFDOC_LISTS__1503939803" on "AIROUTE"."BD_DEFDOC_LISTS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."BMS_BILL_APS"
(
    "ID" number(10, 0) not null, 
    "MBL" nvarchar2(50) not null, 
    "ISMBLJS" number(1, 0) not null, 
    "FTPARENTID" number(10, 0) not null, 
    "OPS_M_ORDID" number(10, 0) not null, 
    "OPS_H_ORDID" number(10, 0) not null, 
    "DZBH" nvarchar2(50) null, 
    "LINE_NO" number(10, 0) not null, 
    "BILL_TYPE" nvarchar2(20) null, 
    "BILL_ACCOUNT" number(28, 9) not null, 
    "BILL_AMOUNT" number(28, 9) not null, 
    "BILL_TAXRATETYPE" nvarchar2(50) null, 
    "BILL_TAXRATE" number(28, 9) not null, 
    "BILL_HASTAX" number(1, 0) not null, 
    "BILL_TAXAMOUNT" number(28, 9) not null, 
    "BILL_AMOUNTTAXTOTAL" number(28, 9) not null, 
    "BILL_ACCOUNT2" number(28, 9) not null, 
    "MONEY_CODE" nvarchar2(20) null, 
    "BILL_OBJECT_ID" nvarchar2(50) null, 
    "BILL_OBJECT_NAME" nvarchar2(300) null, 
    "PAYWAY" nvarchar2(20) null, 
    "REMARK" nvarchar2(200) null, 
    "BILL_DATE" date null, 
    "ORG_MONEY_CODE" nvarchar2(20) null, 
    "ORG_BILL_ACCOUNT2" number(28, 9) not null, 
    "NOWENT_ACC" number(28, 9) not null, 
    "AUDITNO" nvarchar2(50) null, 
    "AUDITID" nvarchar2(50) null, 
    "AUDITNAME" nvarchar2(20) null, 
    "AUDITDATE" date null, 
    "AUDITSTATUS" number(10, 0) not null, 
    "CANCEL_STATUS" number(1, 0) not null, 
    "CANCEL_ID" nvarchar2(50) null, 
    "CANCEL_NAME" nvarchar2(20) null, 
    "CANCEL_DATE" date null, 
    "SUMBMIT_STATUS" number(1, 0) not null, 
    "SUMBMIT_NO" nvarchar2(50) null, 
    "SUMBMIT_NO_ORG" nvarchar2(50) null, 
    "SUMBMIT_ID" nvarchar2(50) null, 
    "SUMBMIT_NAME" nvarchar2(20) null, 
    "SUMBMIT_DATE" date null, 
    "SIGNIN_STATUS" number(1, 0) not null, 
    "SIGNIN_NO" nvarchar2(50) null, 
    "SIGNIN_ID" nvarchar2(50) null, 
    "SIGNIN_NAME" nvarchar2(20) null, 
    "SIGNIN_DATE" date null, 
    "INVOICE_STATUS" number(1, 0) not null, 
    "INVOICE_NO" nvarchar2(50) null, 
    "INVOICE_DESC" nvarchar2(100) null, 
    "INVOICE_ID" nvarchar2(50) null, 
    "INVOICE_NAME" nvarchar2(20) null, 
    "INVOICE_DATE" date null, 
    "INVOICE_REMARK" nvarchar2(200) null, 
    "SELLACCOUNT_STATUS" number(1, 0) not null, 
    "SELLACCOUNT_ID" nvarchar2(50) null, 
    "SELLACCOUNT_NAME" nvarchar2(20) null, 
    "SELLACCOUNT_DATE" date null, 
    "SIGNIN_ECCNO" nvarchar2(50) null, 
    "INVOICE_MONEYCODE" nvarchar2(50) null, 
    "INVOICE_FEETYPE" nvarchar2(100) null, 
    "DSDF_STATUS" number(10, 0) not null, 
    "SUMMARY" nvarchar2(100) null, 
    "CREATE_STATUS" number(10, 0) not null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_BMS_BILL_APS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_BMS_BILL_APS"
/

create or replace trigger "AIROUTE"."TR_BMS_BILL_APS"
before insert on "AIROUTE"."BMS_BILL_APS"
for each row
begin
  select "AIROUTE"."SQ_BMS_BILL_APS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_APS_MBL" on "AIROUTE"."BMS_BILL_APS" ("MBL")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_APS_ISMBLJS" on "AIROUTE"."BMS_BILL_APS" ("ISMBLJS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_APS_OPS_M_ORDID" on "AIROUTE"."BMS_BILL_APS" ("OPS_M_ORDID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_BMS_BILL_APS_DZBH_LINE_NO" on "AIROUTE"."BMS_BILL_APS" ("DZBH", "LINE_NO")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_APS_BILL_TYPE" on "AIROUTE"."BMS_BILL_APS" ("BILL_TYPE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_APS_CANC_213679320" on "AIROUTE"."BMS_BILL_APS" ("CANCEL_STATUS", "AUDITSTATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_APS_SUMBMIT_STATUS" on "AIROUTE"."BMS_BILL_APS" ("SUMBMIT_STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_APS_STATUS" on "AIROUTE"."BMS_BILL_APS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_APS_OPERATINGPOINT" on "AIROUTE"."BMS_BILL_APS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."BMS_BILL_AP_DTLS"
(
    "ID" number(10, 0) not null, 
    "OPS_M_ORDID" number(10, 0) not null, 
    "OPS_H_ORDID" number(10, 0) not null, 
    "BMS_BILL_AP_ID" number(10, 0) null, 
    "DZBH" nvarchar2(50) null, 
    "LINE_NO" number(10, 0) not null, 
    "LINE_ID" number(10, 0) not null, 
    "CHARGE_CODE" nvarchar2(50) null, 
    "CHARGE_DESC" nvarchar2(50) null, 
    "UNITPRICE" number(28, 9) not null, 
    "UNITPRICE2" number(28, 9) not null, 
    "QTY" number(28, 9) not null, 
    "ACCOUNT" number(28, 9) not null, 
    "ACCOUNT2" number(28, 9) not null, 
    "MONEY_CODE" nvarchar2(50) null, 
    "SUMMARY" nvarchar2(50) null, 
    "COLLATE_ID" nvarchar2(50) null, 
    "COLLATE_NAME" nvarchar2(50) null, 
    "COLLATE_DATE" date null, 
    "COLLATE_STATUS" number(10, 0) not null, 
    "COLLATE_NO" nvarchar2(50) null, 
    "USER_ID" nvarchar2(50) null, 
    "USER_NAME" nvarchar2(50) null, 
    "AUDITSTATUS" number(10, 0) not null, 
    "AUDITNO" nvarchar2(50) null, 
    "INVOICE_STATUS" number(1, 0) not null, 
    "INVOICE_NO" nvarchar2(50) null, 
    "SIGNIN_STATUS" number(1, 0) not null, 
    "SIGNIN_NO" nvarchar2(50) null, 
    "SUMBMIT_STATUS" number(1, 0) not null, 
    "SUMBMIT_NO" nvarchar2(50) null, 
    "SELLACCOUNT_STATUS" number(1, 0) not null, 
    "CANCEL_STATUS" number(1, 0) not null, 
    "BILL_AMOUNT" number(28, 9) not null, 
    "BILL_TAXRATE" number(28, 9) not null, 
    "BILL_HASTAX" number(1, 0) not null, 
    "BILL_TAXAMOUNT" number(28, 9) not null, 
    "BILL_AMOUNTTAXTOTAL" number(28, 9) not null, 
    "CREATE_STATUS" number(10, 0) not null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_BMS_BILL_AP_DTLS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_BMS_BILL_AP_DTLS"
/

create or replace trigger "AIROUTE"."TR_BMS_BILL_AP_DTLS"
before insert on "AIROUTE"."BMS_BILL_AP_DTLS"
for each row
begin
  select "AIROUTE"."SQ_BMS_BILL_AP_DTLS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_AP_DTLS__920049300" on "AIROUTE"."BMS_BILL_AP_DTLS" ("BMS_BILL_AP_ID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_BMS_BILL_AP_DTLS_2054183730" on "AIROUTE"."BMS_BILL_AP_DTLS" ("DZBH", "LINE_NO", "LINE_ID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_AP_DTLS__240862673" on "AIROUTE"."BMS_BILL_AP_DTLS" ("CHARGE_CODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_AP_DTLS_STATUS" on "AIROUTE"."BMS_BILL_AP_DTLS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_AP_DTLS_1306944515" on "AIROUTE"."BMS_BILL_AP_DTLS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."BMS_BILL_ARS"
(
    "ID" number(10, 0) not null, 
    "MBL" nvarchar2(50) not null, 
    "OPS_M_ORDID" number(10, 0) not null, 
    "OPS_H_ORDID" number(10, 0) not null, 
    "DZBH" nvarchar2(50) null, 
    "LINE_NO" number(10, 0) not null, 
    "BILL_TYPE" nvarchar2(50) null, 
    "BILL_AMOUNT" number(28, 9) not null, 
    "BILL_TAXRATETYPE" nvarchar2(50) null, 
    "BILL_TAXRATE" number(28, 9) not null, 
    "BILL_HASTAX" number(1, 0) not null, 
    "BILL_TAXAMOUNT" number(28, 9) not null, 
    "BILL_AMOUNTTAXTOTAL" number(28, 9) not null, 
    "BILL_ACCOUNT" number(28, 9) not null, 
    "BILL_ACCOUNT2" number(28, 9) not null, 
    "MONEY_CODE" nvarchar2(50) null, 
    "BILL_OBJECT_ID" nvarchar2(50) null, 
    "BILL_OBJECT_NAME" nvarchar2(300) null, 
    "PAYWAY" nvarchar2(50) null, 
    "REMARK" nvarchar2(200) null, 
    "BILL_DATE" date null, 
    "ORG_MONEY_CODE" nvarchar2(50) null, 
    "ORG_BILL_ACCOUNT2" number(28, 9) not null, 
    "AUDITNO" nvarchar2(50) null, 
    "AUDITID" nvarchar2(50) null, 
    "AUDITNAME" nvarchar2(20) null, 
    "AUDITDATE" date null, 
    "AUDITSTATUS" number(10, 0) not null, 
    "CANCEL_STATUS" number(1, 0) not null, 
    "CANCEL_ID" nvarchar2(50) null, 
    "CANCEL_NAME" nvarchar2(20) null, 
    "CANCEL_DATE" date null, 
    "SUMBMIT_STATUS" number(1, 0) not null, 
    "SUMBMIT_NO" nvarchar2(50) null, 
    "SUMBMIT_NO_ORG" nvarchar2(50) null, 
    "SUMBMIT_ID" nvarchar2(50) null, 
    "SUMBMIT_NAME" nvarchar2(20) null, 
    "SUMBMIT_DATE" date null, 
    "INVOICE_STATUS" number(1, 0) not null, 
    "INVOICE_NO" nvarchar2(50) null, 
    "INVOICE_DESC" nvarchar2(100) null, 
    "INVOICE_ID" nvarchar2(50) null, 
    "INVOICE_NAME" nvarchar2(20) null, 
    "INVOICE_DATE" date null, 
    "INVOICE_REMARK" nvarchar2(200) null, 
    "SELLACCOUNT_STATUS" number(1, 0) not null, 
    "SELLACCOUNT_ID" nvarchar2(50) null, 
    "SELLACCOUNT_NAME" nvarchar2(20) null, 
    "SELLACCOUNT_DATE" date null, 
    "DSDF_STATUS" number(10, 0) not null, 
    "DK_BILL_ID" nvarchar2(50) null, 
    "DK_OPERATION_ID" nvarchar2(50) null, 
    "NOWENT_ACC" number(28, 9) not null, 
    "SUMBMIT_ECCNO" nvarchar2(50) null, 
    "INVOICE_MONEYCODE" nvarchar2(50) null, 
    "INVOICE_FEETYPE" nvarchar2(100) null, 
    "SUMMARY" nvarchar2(100) null, 
    "CREATE_STATUS" number(10, 0) not null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_BMS_BILL_ARS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_BMS_BILL_ARS"
/

create or replace trigger "AIROUTE"."TR_BMS_BILL_ARS"
before insert on "AIROUTE"."BMS_BILL_ARS"
for each row
begin
  select "AIROUTE"."SQ_BMS_BILL_ARS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_ARS_MBL" on "AIROUTE"."BMS_BILL_ARS" ("MBL")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_ARS_OPS_M_ORDID" on "AIROUTE"."BMS_BILL_ARS" ("OPS_M_ORDID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_BMS_BILL_ARS_DZBH_LINE_NO" on "AIROUTE"."BMS_BILL_ARS" ("DZBH", "LINE_NO")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_ARS_BILL_TYPE" on "AIROUTE"."BMS_BILL_ARS" ("BILL_TYPE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_ARS_CANC_612221266" on "AIROUTE"."BMS_BILL_ARS" ("CANCEL_STATUS", "AUDITSTATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_ARS_SUMBMIT_STATUS" on "AIROUTE"."BMS_BILL_ARS" ("SUMBMIT_STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_ARS_STATUS" on "AIROUTE"."BMS_BILL_ARS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_ARS_OPERATINGPOINT" on "AIROUTE"."BMS_BILL_ARS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."BMS_BILL_AR_DTLS"
(
    "ID" number(10, 0) not null, 
    "OPS_M_ORDID" number(10, 0) not null, 
    "OPS_H_ORDID" number(10, 0) not null, 
    "BMS_BILL_AR_ID" number(10, 0) null, 
    "DZBH" nvarchar2(50) null, 
    "LINE_NO" number(10, 0) not null, 
    "LINE_ID" number(10, 0) not null, 
    "CHARGE_CODE" nvarchar2(50) not null, 
    "CHARGE_DESC" nvarchar2(100) null, 
    "UNITPRICE" number(28, 9) not null, 
    "UNITPRICE2" number(28, 9) not null, 
    "QTY" number(28, 9) not null, 
    "ACCOUNT" number(28, 9) not null, 
    "ACCOUNT2" number(28, 9) not null, 
    "MONEY_CODE" nvarchar2(50) null, 
    "SUMMARY" nvarchar2(100) null, 
    "COLLATE_ID" nvarchar2(50) null, 
    "COLLATE_NAME" nvarchar2(50) null, 
    "COLLATE_DATE" date null, 
    "COLLATE_STATUS" number(10, 0) not null, 
    "COLLATE_NO" nvarchar2(50) null, 
    "USER_ID" nvarchar2(50) null, 
    "USER_NAME" nvarchar2(50) null, 
    "AUDITSTATUS" number(10, 0) not null, 
    "AUDITNO" nvarchar2(50) null, 
    "INVOICE_STATUS" number(1, 0) not null, 
    "INVOICE_NO" nvarchar2(50) null, 
    "SUMBMIT_STATUS" number(1, 0) not null, 
    "SUMBMIT_NO" nvarchar2(50) null, 
    "SELLACCOUNT_STATUS" number(1, 0) not null, 
    "CANCEL_STATUS" number(1, 0) not null, 
    "BILL_AMOUNT" number(28, 9) not null, 
    "BILL_TAXRATE" number(28, 9) not null, 
    "BILL_HASTAX" number(1, 0) not null, 
    "BILL_TAXAMOUNT" number(28, 9) not null, 
    "BILL_AMOUNTTAXTOTAL" number(28, 9) not null, 
    "CREATE_STATUS" number(10, 0) not null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_BMS_BILL_AR_DTLS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_BMS_BILL_AR_DTLS"
/

create or replace trigger "AIROUTE"."TR_BMS_BILL_AR_DTLS"
before insert on "AIROUTE"."BMS_BILL_AR_DTLS"
for each row
begin
  select "AIROUTE"."SQ_BMS_BILL_AR_DTLS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_AR_DTLS_1345282173" on "AIROUTE"."BMS_BILL_AR_DTLS" ("BMS_BILL_AR_ID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_BMS_BILL_AR_DTLS__162991438" on "AIROUTE"."BMS_BILL_AR_DTLS" ("DZBH", "LINE_NO", "LINE_ID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_AR_DTLS__497633365" on "AIROUTE"."BMS_BILL_AR_DTLS" ("CHARGE_CODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_AR_DTLS_STATUS" on "AIROUTE"."BMS_BILL_AR_DTLS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_BMS_BILL_AR_DTLS_1354244173" on "AIROUTE"."BMS_BILL_AR_DTLS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."CHANGEORDERHISTORIES"
(
    "ID" number(10, 0) not null, 
    "KEY_ID" number(10, 0) not null, 
    "TABLENAME" nvarchar2(50) null, 
    "CHANGETYPE" number(10, 0) not null, 
    "INSERTNUM" number(10, 0) not null, 
    "UPDATENUM" number(10, 0) not null, 
    "DELETENUM" number(10, 0) not null, 
    "CONTENT" nvarchar2(200) null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null, 
    "OPERATINGPOINT" number(10, 0) not null,
    constraint "PK_CHANGEORDERHISTORIES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_CHANGEORDERHISTORIES"
/

create or replace trigger "AIROUTE"."TR_CHANGEORDERHISTORIES"
before insert on "AIROUTE"."CHANGEORDERHISTORIES"
for each row
begin
  select "AIROUTE"."SQ_CHANGEORDERHISTORIES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CHANGEORDERHISTOR_119655192" on "AIROUTE"."CHANGEORDERHISTORIES" ("OPERATINGPOINT", "KEY_ID", "TABLENAME")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CHANGEORDERHISTOR_782671097" on "AIROUTE"."CHANGEORDERHISTORIES" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."COMPANIES"
(
    "ID" number(10, 0) not null, 
    "NAME" nvarchar2(30) not null, 
    "SIMPLENAME" nvarchar2(30) null, 
    "ENG_NAME" nvarchar2(100) null, 
    "ADDRESS" nvarchar2(100) not null, 
    "CITY" nvarchar2(20) null, 
    "PROVINCE" nvarchar2(20) null, 
    "REGISTERDATE" date null, 
    "LOGO" nvarchar2(500) null,
    constraint "PK_COMPANIES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_COMPANIES"
/

create or replace trigger "AIROUTE"."TR_COMPANIES"
before insert on "AIROUTE"."COMPANIES"
for each row
begin
  select "AIROUTE"."SQ_COMPANIES".nextval into :new."ID" from dual;
end;
/

create table "AIROUTE"."CONTACTSES"
(
    "ID" number(10, 0) not null, 
    "CUSBUSINFOID" nvarchar2(50) null, 
    "COMPANYNAME" nvarchar2(200) null, 
    "COMPANYCODE" nvarchar2(50) null, 
    "COADDRESS" nvarchar2(500) null, 
    "COAREA" nvarchar2(50) null, 
    "COCOUNTRY" nvarchar2(50) null, 
    "CONTACT" nvarchar2(100) null, 
    "CONTACTWHO" nvarchar2(50) null, 
    "CONTACTINFO" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "REMARK" nvarchar2(100) null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_CONTACTSES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_CONTACTSES"
/

create or replace trigger "AIROUTE"."TR_CONTACTSES"
before insert on "AIROUTE"."CONTACTSES"
for each row
begin
  select "AIROUTE"."SQ_CONTACTSES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CONTACTSES_CUSBUSINFOID" on "AIROUTE"."CONTACTSES" ("CUSBUSINFOID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CONTACTSES_OPERATINGPOINT" on "AIROUTE"."CONTACTSES" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."COPOKINDS"
(
    "ID" number(10, 0) not null, 
    "CODE" nvarchar2(50) null, 
    "NAME" nvarchar2(100) null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_COPOKINDS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_COPOKINDS"
/

create or replace trigger "AIROUTE"."TR_COPOKINDS"
before insert on "AIROUTE"."COPOKINDS"
for each row
begin
  select "AIROUTE"."SQ_COPOKINDS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_COPOKINDS_CODE" on "AIROUTE"."COPOKINDS" ("CODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_COPOKINDS_STATUS" on "AIROUTE"."COPOKINDS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."COSTMONEYS"
(
    "ID" number(10, 0) not null, 
    "SERIALNO" nvarchar2(50) not null, 
    "SETTLEACCOUNT" nvarchar2(50) not null, 
    "FEECODE" nvarchar2(50) not null, 
    "FEENAME" nvarchar2(50) not null, 
    "STARTPLACE" nvarchar2(50) null, 
    "TRANSITPLACE" nvarchar2(50) null, 
    "ENDPLACE" nvarchar2(50) null, 
    "AIRLINECO" nvarchar2(50) null, 
    "AIRLINENO" nvarchar2(50) null, 
    "WHBUYER" nvarchar2(50) null, 
    "PROXYOPERATOR" nvarchar2(50) null, 
    "DEALWITHARTICLE" nvarchar2(50) null, 
    "BSA" nvarchar2(50) null, 
    "CUSTOMSTYPE" nvarchar2(50) null, 
    "INSPECTMARK" nvarchar2(50) null, 
    "GETORDMARK" nvarchar2(50) null, 
    "MOORLEVEL" nvarchar2(50) null, 
    "BILLINGUNIT" nvarchar2(50) not null, 
    "PRICE" number(28, 9) not null, 
    "CURRENCYCODE" nvarchar2(50) not null, 
    "FEECONDITIONVAL1" nvarchar2(50) null, 
    "CALCSIGN1" nvarchar2(50) null, 
    "FEECONDITION" nvarchar2(50) null, 
    "CALCSIGN2" nvarchar2(50) null, 
    "FEECONDITIONVAL2" nvarchar2(50) null, 
    "CALCFORMULA" nvarchar2(50) not null, 
    "FEEMIN" nvarchar2(50) null, 
    "FEEMAX" nvarchar2(50) null, 
    "AUDITSTATUS" number(10, 0) not null, 
    "STARTDATE" date null, 
    "ENDDATE" date null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_COSTMONEYS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_COSTMONEYS"
/

create or replace trigger "AIROUTE"."TR_COSTMONEYS"
before insert on "AIROUTE"."COSTMONEYS"
for each row
begin
  select "AIROUTE"."SQ_COSTMONEYS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_COSTMONEYS_SERIALNO" on "AIROUTE"."COSTMONEYS" ("SERIALNO")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_COSTMONEYS_OPERAT_375055131" on "AIROUTE"."COSTMONEYS" ("OPERATINGPOINT", "STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."CUSBUSINFOS"
(
    "ID" number(10, 0) not null, 
    "ENTERPRISEID" nvarchar2(20) not null, 
    "ENTERPRISENAME" nvarchar2(500) not null, 
    "ENTERPRISESHORTNAME" nvarchar2(50) not null, 
    "ENTERPRISETYPE" nvarchar2(50) not null, 
    "CHNNAME" nvarchar2(100) null, 
    "ENGNAME" nvarchar2(100) null, 
    "ENTERPRISEGROUP" nvarchar2(50) null, 
    "ENTERPRISEGROUPCODE" nvarchar2(50) null, 
    "TOPENTERPRISECODE" nvarchar2(50) null, 
    "CIQID" nvarchar2(50) null, 
    "CHECKID" nvarchar2(50) null, 
    "CUSTOMSCODE" nvarchar2(50) null, 
    "ADDRESSCHN" nvarchar2(200) not null, 
    "ADDRESSENG" nvarchar2(200) null, 
    "WEBSITE" nvarchar2(200) null, 
    "TRADETYPECODE" nvarchar2(50) null, 
    "AREACODE" nvarchar2(50) not null, 
    "ECCAREACODE" nvarchar2(50) not null, 
    "COUNTRYCODE" nvarchar2(50) not null, 
    "COPOKINDCODE" nvarchar2(50) null, 
    "CORPARTIPERSON" nvarchar2(50) null, 
    "RESTEREDCAPITAL" nvarchar2(50) not null, 
    "ISINTERNALCOMPANY" number(1, 0) not null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "LINKERMAN" nvarchar2(50) null, 
    "TELEPHONE" nvarchar2(100) null, 
    "FAX" nvarchar2(100) null, 
    "TAXPAYERTYPE" nvarchar2(50) not null, 
    "UNIFIEDSOCIALCREDITCODE" nvarchar2(50) not null, 
    "INVOICECOUNTRYCODE" nvarchar2(50) not null, 
    "INVOICEADDRESS" nvarchar2(100) not null, 
    "BANKNAME" nvarchar2(100) not null, 
    "BANKACCOUNT" nvarchar2(100) not null, 
    "CURRENCY" nvarchar2(50) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_CUSBUSINFOS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_CUSBUSINFOS"
/

create or replace trigger "AIROUTE"."TR_CUSBUSINFOS"
before insert on "AIROUTE"."CUSBUSINFOS"
for each row
begin
  select "AIROUTE"."SQ_CUSBUSINFOS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_CUSBUSINFOS_ENTERPRISEID" on "AIROUTE"."CUSBUSINFOS" ("ENTERPRISEID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CUSBUSINFOS_OPERATINGPOINT" on "AIROUTE"."CUSBUSINFOS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."CUSQUOTEDPRICEDTLS"
(
    "ID" number(10, 0) not null, 
    "CUSQPSERIALNO" nvarchar2(50) not null, 
    "CUSQPID" number(10, 0) not null, 
    "QPSERIALNO" nvarchar2(50) not null, 
    "QPID" number(10, 0) not null, 
    "FEECODE" nvarchar2(50) not null, 
    "FEENAME" nvarchar2(50) not null, 
    "STARTPLACE" nvarchar2(50) null, 
    "TRANSITPLACE" nvarchar2(50) null, 
    "ENDPLACE" nvarchar2(50) null, 
    "AIRLINECO" nvarchar2(50) null, 
    "AIRLINENO" nvarchar2(50) null, 
    "WHBUYER" nvarchar2(50) null, 
    "PROXYOPERATOR" number(1, 0) not null, 
    "DEALWITHARTICLE" nvarchar2(50) null, 
    "BSA" number(1, 0) not null, 
    "CUSTOMSTYPE" nvarchar2(50) null, 
    "INSPECTMARK" number(1, 0) not null, 
    "GETORDMARK" number(1, 0) not null, 
    "MOORLEVEL" nvarchar2(50) null, 
    "BILLINGUNIT" nvarchar2(50) not null, 
    "PRICE" number(28, 9) not null, 
    "CURRENCYCODE" nvarchar2(50) not null, 
    "FEECONDITIONVAL1" number(28, 9) not null, 
    "CALCSIGN1" nvarchar2(50) null, 
    "FEECONDITION" nvarchar2(50) null, 
    "CALCSIGN2" nvarchar2(50) null, 
    "FEECONDITIONVAL2" number(28, 9) not null, 
    "CALCFORMULA" nvarchar2(50) not null, 
    "FEEMIN" number(28, 9) not null, 
    "FEEMAX" number(28, 9) not null, 
    "STARTDATE" date null, 
    "ENDDATE" date null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_CUSQUOTEDPRICEDTLS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_CUSQUOTEDPRICEDTLS"
/

create or replace trigger "AIROUTE"."TR_CUSQUOTEDPRICEDTLS"
before insert on "AIROUTE"."CUSQUOTEDPRICEDTLS"
for each row
begin
  select "AIROUTE"."SQ_CUSQUOTEDPRICEDTLS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CUSQUOTEDPRICEDT_1809892340" on "AIROUTE"."CUSQUOTEDPRICEDTLS" ("CUSQPSERIALNO")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CUSQUOTEDPRICEDTLS_CUSQPID" on "AIROUTE"."CUSQUOTEDPRICEDTLS" ("CUSQPID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CUSQUOTEDPRICEDT_1551065164" on "AIROUTE"."CUSQUOTEDPRICEDTLS" ("QPSERIALNO")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CUSQUOTEDPRICEDTLS_QPID" on "AIROUTE"."CUSQUOTEDPRICEDTLS" ("QPID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CUSQUOTEDPRICEDTL_570402445" on "AIROUTE"."CUSQUOTEDPRICEDTLS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."CUSTOMERQUOTEDPRICES"
(
    "ID" number(10, 0) not null, 
    "SERIALNO" nvarchar2(50) not null, 
    "BUSINESSTYPE" nvarchar2(50) null, 
    "CUSTOMERCODE" nvarchar2(50) null, 
    "LOCALWHMARK" nvarchar2(50) null, 
    "ENDPORTCODE" nvarchar2(50) null, 
    "STARTPLACE" nvarchar2(50) null, 
    "TRANSITPLACE" nvarchar2(50) null, 
    "ENDPLACE" nvarchar2(50) null, 
    "PROXYOPERATOR" nvarchar2(50) null, 
    "CUSDEFINITION" nvarchar2(50) null, 
    "RECEIVER" nvarchar2(50) null, 
    "SHIPPER" nvarchar2(50) null, 
    "CONTACT" nvarchar2(50) null, 
    "QUOTEDPRICEPOLICY" nvarchar2(50) null, 
    "SELLER" nvarchar2(50) null, 
    "STARTDATE" date null, 
    "ENDDATE" date null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "AUDITSTATUS" number(10, 0) not null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_CUSTOMERQUOTEDPRICES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_CUSTOMERQUOTEDPRICES"
/

create or replace trigger "AIROUTE"."TR_CUSTOMERQUOTEDPRICES"
before insert on "AIROUTE"."CUSTOMERQUOTEDPRICES"
for each row
begin
  select "AIROUTE"."SQ_CUSTOMERQUOTEDPRICES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_CUSTOMERQUOTEDPR_1522166728" on "AIROUTE"."CUSTOMERQUOTEDPRICES" ("SERIALNO")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CUSTOMERQUOTEDPRICES_STATUS" on "AIROUTE"."CUSTOMERQUOTEDPRICES" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CUSTOMERQUOTEDPR_1571402807" on "AIROUTE"."CUSTOMERQUOTEDPRICES" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."CUSTOMSINSPECTIONS"
(
    "ID" number(10, 0) not null, 
    "OPERATION_ID" nvarchar2(50) null, 
    "FLIGHT_NO" nvarchar2(50) null, 
    "FLIGHT_DATE_WANT" date null, 
    "MBL" nvarchar2(20) null, 
    "CONSIGN_CODE_CK" nvarchar2(50) null, 
    "BOOK_FLAT_CODE" nvarchar2(50) null, 
    "CUSTOMS_DECLARATION" nvarchar2(50) null, 
    "NUM_BG" number(28, 9) null, 
    "REMARKS_BG" nvarchar2(500) null, 
    "CUSTOMS_BROKER_BG" nvarchar2(50) null, 
    "CUSTOMS_DATE_BG" date null, 
    "PIECES_TS" number(28, 9) null, 
    "WEIGHT_TS" number(28, 9) null, 
    "VOLUME_TS" number(28, 9) null, 
    "PIECES_FACT" number(28, 9) null, 
    "WEIGHT_FACT" number(28, 9) null, 
    "VOLUME_FACT" number(28, 9) null, 
    "PIECES_BG" number(28, 9) null, 
    "WEIGHT_BG" number(28, 9) null, 
    "VOLUME_BG" number(28, 9) null, 
    "IS_CHECKED_BG" number(1, 0) not null, 
    "CHECK_QTY" date null, 
    "CHECK_DATE" number(28, 9) null, 
    "FILEUPLOAD" nvarchar2(200) null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_CUSTOMSINSPECTIONS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_CUSTOMSINSPECTIONS"
/

create or replace trigger "AIROUTE"."TR_CUSTOMSINSPECTIONS"
before insert on "AIROUTE"."CUSTOMSINSPECTIONS"
for each row
begin
  select "AIROUTE"."SQ_CUSTOMSINSPECTIONS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CUSTOMSINSPECTIONS_STATUS" on "AIROUTE"."CUSTOMSINSPECTIONS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_CUSTOMSINSPECTIONS_70552025" on "AIROUTE"."CUSTOMSINSPECTIONS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."DAILYRATES"
(
    "ID" number(10, 0) not null, 
    "LOCALCURRENCY" nvarchar2(50) null, 
    "LOCALCURRCODE" nvarchar2(50) null, 
    "FOREIGNCURRENCY" nvarchar2(50) null, 
    "FOREIGNCURRCODE" nvarchar2(50) null, 
    "PRICETYPE" nvarchar2(50) null, 
    "BANKNAME" nvarchar2(50) null, 
    "PRICE" number(28, 9) not null, 
    "SCRAPYDATE" date null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(1, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_DAILYRATES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_DAILYRATES"
/

create or replace trigger "AIROUTE"."TR_DAILYRATES"
before insert on "AIROUTE"."DAILYRATES"
for each row
begin
  select "AIROUTE"."SQ_DAILYRATES".nextval into :new."ID" from dual;
end;
/

create table "AIROUTE"."DATATABLEIMPORTMAPPINGS"
(
    "ID" number(10, 0) not null, 
    "ENTITYSETNAME" nvarchar2(50) not null, 
    "FIELDNAME" nvarchar2(50) not null, 
    "FIELDDESC" nvarchar2(50) null, 
    "TYPENAME" nvarchar2(30) null, 
    "DEFAULTVALUE" nvarchar2(50) null, 
    "ISREQUIRED" number(1, 0) not null, 
    "SOURCEFIELDNAME" nvarchar2(50) null, 
    "ISENABLED" number(1, 0) not null, 
    "REGULAREXPRESSION" nvarchar2(100) null,
    constraint "PK_DATATABLEIMPORTMAPPINGS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_DATATABLEIMPORTMAPPINGS"
/

create or replace trigger "AIROUTE"."TR_DATATABLEIMPORTMAPPINGS"
before insert on "AIROUTE"."DATATABLEIMPORTMAPPINGS"
for each row
begin
  select "AIROUTE"."SQ_DATATABLEIMPORTMAPPINGS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_DATATABLEIMPORTM_1026680092" on "AIROUTE"."DATATABLEIMPORTMAPPINGS" ("ENTITYSETNAME", "FIELDNAME")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."DEALARTICLES"
(
    "ID" number(10, 0) not null, 
    "DEALARTICLECODE" nvarchar2(50) not null, 
    "DEALARTICLENAME" nvarchar2(50) not null, 
    "TRANSARTICLE" nvarchar2(50) null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "PAY_MODECODE" nvarchar2(50) null, 
    "CARRIAGE" nvarchar2(1) null, 
    "CARRIAGEENS" nvarchar2(50) null, 
    "INCIDENTAL_EXPENSES" nvarchar2(1) null, 
    "INCIDENTAL_EXPENSESENS" nvarchar2(50) null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_DEALARTICLES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_DEALARTICLES"
/

create or replace trigger "AIROUTE"."TR_DEALARTICLES"
before insert on "AIROUTE"."DEALARTICLES"
for each row
begin
  select "AIROUTE"."SQ_DEALARTICLES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_DEALARTICLES_DEA_1874170632" on "AIROUTE"."DEALARTICLES" ("DEALARTICLECODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_DEALARTICLES_OPER_906588572" on "AIROUTE"."DEALARTICLES" ("OPERATINGPOINT", "STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."DOCUMENTMANAGEMENTS"
(
    "ID" number(10, 0) not null, 
    "OPERATION_ID" nvarchar2(50) null, 
    "DZ_TYPE" nvarchar2(50) null, 
    "DOC_NO" nvarchar2(50) null, 
    "TRADE_MODE" nvarchar2(50) null, 
    "RETURN_PRINT" nvarchar2(100) null, 
    "QTY" number(28, 9) null, 
    "BG_TT" nvarchar2(50) null, 
    "PING_NAME" nvarchar2(50) null, 
    "RETURN_DATE" date null, 
    "PRINT_DATE" date null, 
    "RETURN_CUSTOMER_DATE" date null, 
    "MBL" nvarchar2(20) null, 
    "ENTRUSTMENT_NAME" nvarchar2(100) null, 
    "ENTRUSTMENT_CODE" nvarchar2(50) null, 
    "FLIGHT_DATE_WANT" date null, 
    "IS_RETURN" number(1, 0) null, 
    "RETURNID" nvarchar2(50) null, 
    "RETURNWHO" nvarchar2(20) null, 
    "IS_RETURN_CUSTOMER" number(1, 0) null, 
    "RETURN_CUSTOMERID" nvarchar2(50) null, 
    "RETURN_CUSTOMERWHO" nvarchar2(20) null, 
    "IS_PRINT" number(1, 0) null, 
    "SIGNRECEIPT_CODE" nvarchar2(20) null, 
    "CUSTOMS_DECLARATION" nvarchar2(50) null, 
    "REMARK" nvarchar2(100) null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_DOCUMENTMANAGEMENTS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_DOCUMENTMANAGEMENTS"
/

create or replace trigger "AIROUTE"."TR_DOCUMENTMANAGEMENTS"
before insert on "AIROUTE"."DOCUMENTMANAGEMENTS"
for each row
begin
  select "AIROUTE"."SQ_DOCUMENTMANAGEMENTS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_DOCUMENTMANAGEME_1174797144" on "AIROUTE"."DOCUMENTMANAGEMENTS" ("OPERATINGPOINT", "STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."EAI_GROUPS"
(
    "ID" number(10, 0) not null, 
    "CODE" nvarchar2(50) not null, 
    "NAME" nvarchar2(100) null, 
    "REMARK" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_EAI_GROUPS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_EAI_GROUPS"
/

create or replace trigger "AIROUTE"."TR_EAI_GROUPS"
before insert on "AIROUTE"."EAI_GROUPS"
for each row
begin
  select "AIROUTE"."SQ_EAI_GROUPS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_EAI_GROUPS_CODE" on "AIROUTE"."EAI_GROUPS" ("CODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_EAI_GROUPS_STATUS" on "AIROUTE"."EAI_GROUPS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_EAI_GROUPS_OPERATINGPOINT" on "AIROUTE"."EAI_GROUPS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."EXCHANGE_RATES"
(
    "ID" number(10, 0) not null, 
    "CURR_CODE" nvarchar2(20) not null, 
    "EXCURR_CODE" nvarchar2(20) not null, 
    "YEAR" number(10, 0) not null, 
    "MONTH" number(10, 0) not null, 
    "RECRATE" number(28, 9) not null, 
    "PAYRATE" number(28, 9) not null, 
    "STATUS" number(1, 0) not null, 
    "REMARK" nvarchar2(100) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_EXCHANGE_RATES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_EXCHANGE_RATES"
/

create or replace trigger "AIROUTE"."TR_EXCHANGE_RATES"
before insert on "AIROUTE"."EXCHANGE_RATES"
for each row
begin
  select "AIROUTE"."SQ_EXCHANGE_RATES".nextval into :new."ID" from dual;
end;
/

create table "AIROUTE"."FEETYPES"
(
    "ID" number(10, 0) not null, 
    "FEECODE" nvarchar2(50) not null, 
    "FEENAME" nvarchar2(50) null, 
    "FEEENAME" nvarchar2(50) null, 
    "INSPECTIONFEE" nvarchar2(50) null, 
    "CUSTOMSFEE" nvarchar2(50) null, 
    "MATHDATE" nvarchar2(50) null, 
    "ECC_CODE" nvarchar2(50) null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_FEETYPES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_FEETYPES"
/

create or replace trigger "AIROUTE"."TR_FEETYPES"
before insert on "AIROUTE"."FEETYPES"
for each row
begin
  select "AIROUTE"."SQ_FEETYPES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_FEETYPES_FEECODE" on "AIROUTE"."FEETYPES" ("FEECODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_FEETYPES_OPERATI_1404445442" on "AIROUTE"."FEETYPES" ("OPERATINGPOINT", "STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."FEEUNITS"
(
    "ID" number(10, 0) not null, 
    "FEEUNITNAME" nvarchar2(50) not null, 
    "REMARK" nvarchar2(50) null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_FEEUNITS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_FEEUNITS"
/

create or replace trigger "AIROUTE"."TR_FEEUNITS"
before insert on "AIROUTE"."FEEUNITS"
for each row
begin
  select "AIROUTE"."SQ_FEEUNITS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_FEEUNITS_OPERATI_1651222707" on "AIROUTE"."FEEUNITS" ("OPERATINGPOINT", "STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."FILEATTACHS"
(
    "ID" number(10, 0) not null, 
    "FILEGUID" nvarchar2(100) null, 
    "FILENAME" nvarchar2(100) null, 
    "FILEPATH" nvarchar2(200) null, 
    "FILELENGTH" number(28, 9) not null, 
    "FILEDATA" blob null, 
    "PICMINDATA" blob null, 
    "ADDTS" date null, 
    "ADDWHO" nvarchar2(20) null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_FILEATTACHS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_FILEATTACHS"
/

create or replace trigger "AIROUTE"."TR_FILEATTACHS"
before insert on "AIROUTE"."FILEATTACHS"
for each row
begin
  select "AIROUTE"."SQ_FILEATTACHS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_FILEATTACHS_FILEGUID" on "AIROUTE"."FILEATTACHS" ("FILEGUID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."MAILRECEIVERS"
(
    "ID" number(10, 0) not null, 
    "ERRTYPE" nvarchar2(50) not null, 
    "ERRMETHOD" nvarchar2(50) not null, 
    "RECMAILADDRESS" nvarchar2(50) not null, 
    "CCMAILADDRESS" nvarchar2(500) null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_MAILRECEIVERS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_MAILRECEIVERS"
/

create or replace trigger "AIROUTE"."TR_MAILRECEIVERS"
before insert on "AIROUTE"."MAILRECEIVERS"
for each row
begin
  select "AIROUTE"."SQ_MAILRECEIVERS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_MAILRECEIVERS_ERR_286645421" on "AIROUTE"."MAILRECEIVERS" ("ERRTYPE", "ERRMETHOD", "RECMAILADDRESS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."MENUACTIONS"
(
    "ID" number(10, 0) not null, 
    "NAME" nvarchar2(20) not null, 
    "CODE" nvarchar2(20) not null, 
    "SORT" nvarchar2(20) not null, 
    "ISENABLED" number(1, 0) not null, 
    "DESCRIPTION" nvarchar2(50) null, 
    "CREATEDUSERID" nvarchar2(50) null, 
    "CREATEDDATETIME" date null, 
    "LASTEDITUSERID" nvarchar2(50) null, 
    "LASTEDITDATETIME" date null,
    constraint "PK_MENUACTIONS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_MENUACTIONS"
/

create or replace trigger "AIROUTE"."TR_MENUACTIONS"
before insert on "AIROUTE"."MENUACTIONS"
for each row
begin
  select "AIROUTE"."SQ_MENUACTIONS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_MENUACTIONS_NAME" on "AIROUTE"."MENUACTIONS" ("NAME")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_MENUACTIONS_CODE" on "AIROUTE"."MENUACTIONS" ("CODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."MENUITEMS"
(
    "ID" number(10, 0) not null, 
    "TITLE" nvarchar2(50) not null, 
    "DESCRIPTION" nvarchar2(100) null, 
    "CODE" nvarchar2(20) not null, 
    "URL" nvarchar2(100) not null, 
    "ISENABLED" number(1, 0) not null, 
    "CONTROLLER" nvarchar2(100) null, 
    "ACTION" nvarchar2(100) null, 
    "ICONCLS" nvarchar2(50) null, 
    "PARENTID" number(10, 0) null, 
    "CREATEDUSERID" nvarchar2(50) null, 
    "CREATEDDATETIME" date null, 
    "LASTEDITUSERID" nvarchar2(50) null, 
    "LASTEDITDATETIME" date null,
    constraint "PK_MENUITEMS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_MENUITEMS"
/

create or replace trigger "AIROUTE"."TR_MENUITEMS"
before insert on "AIROUTE"."MENUITEMS"
for each row
begin
  select "AIROUTE"."SQ_MENUITEMS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_MENUITEMS_TITLE" on "AIROUTE"."MENUITEMS" ("TITLE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_MENUITEMS_CODE" on "AIROUTE"."MENUITEMS" ("CODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_MENUITEMS_URL" on "AIROUTE"."MENUITEMS" ("URL")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_MENUITEMS_PARENTID" on "AIROUTE"."MENUITEMS" ("PARENTID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."MESSAGES"
(
    "ID" number(10, 0) not null, 
    "SUBJECT" nvarchar2(100) not null, 
    "KEY1" nvarchar2(100) null, 
    "KEY2" nvarchar2(100) null, 
    "CONTENT" nclob not null, 
    "TYPE" nvarchar2(20) not null, 
    "NEWDATE" date not null, 
    "ISSENDED" number(1, 0) not null, 
    "SENDDATE" date null, 
    "NOTIFICATIONID" number(10, 0) not null, 
    "CREATEDDATE" date null, 
    "MODIFIEDDATE" date null, 
    "CREATEDBY" nvarchar2(20) null, 
    "MODIFIEDBY" nvarchar2(20) null,
    constraint "PK_MESSAGES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_MESSAGES"
/

create or replace trigger "AIROUTE"."TR_MESSAGES"
before insert on "AIROUTE"."MESSAGES"
for each row
begin
  select "AIROUTE"."SQ_MESSAGES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_MESSAGES_NOTIFICATIONID" on "AIROUTE"."MESSAGES" ("NOTIFICATIONID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."NOTIFICATIONS"
(
    "ID" number(10, 0) not null, 
    "NAME" nvarchar2(50) not null, 
    "DESCRIPTION" nvarchar2(100) null, 
    "TYPE" nvarchar2(30) null, 
    "SENDER" nvarchar2(50) not null, 
    "RECEIVER" nvarchar2(100) not null, 
    "SCHEDULE" nvarchar2(50) null, 
    "DISABLED" number(1, 0) not null, 
    "AUTHUSER" nvarchar2(30) null, 
    "AUTHPASSWORD" nvarchar2(30) null, 
    "HOST" nvarchar2(30) null, 
    "CREATEDDATE" date null, 
    "MODIFIEDDATE" date null, 
    "CREATEDBY" nvarchar2(20) null, 
    "MODIFIEDBY" nvarchar2(20) null,
    constraint "PK_NOTIFICATIONS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_NOTIFICATIONS"
/

create or replace trigger "AIROUTE"."TR_NOTIFICATIONS"
before insert on "AIROUTE"."NOTIFICATIONS"
for each row
begin
  select "AIROUTE"."SQ_NOTIFICATIONS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_NOTIFICATIONS_NAME" on "AIROUTE"."NOTIFICATIONS" ("NAME")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."OPERATEPOINTS"
(
    "ID" number(10, 0) not null, 
    "OPERATEPOINTCODE" nvarchar2(50) not null, 
    "OPERATEPOINTNAME" nvarchar2(100) not null, 
    "DESCRIPTION" nvarchar2(100) null, 
    "ISENABLED" number(1, 0) not null, 
    "ADDTS" date null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITID" nvarchar2(50) null, 
    "EDITTS" date null,
    constraint "PK_OPERATEPOINTS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_OPERATEPOINTS"
/

create or replace trigger "AIROUTE"."TR_OPERATEPOINTS"
before insert on "AIROUTE"."OPERATEPOINTS"
for each row
begin
  select "AIROUTE"."SQ_OPERATEPOINTS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_OPERATEPOINTS_OPE_620613229" on "AIROUTE"."OPERATEPOINTS" ("OPERATEPOINTCODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."OPERATEPOINTLISTS"
(
    "ID" number(10, 0) not null, 
    "OPERATEPOINTID" number(10, 0) not null, 
    "OPERATEPOINTCODE" nvarchar2(50) not null, 
    "COMPANYCODE" nvarchar2(50) not null, 
    "COMPANYNAME" nvarchar2(100) not null, 
    "ISENABLED" number(1, 0) not null, 
    "ADDTS" date null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITID" nvarchar2(50) null, 
    "EDITTS" date null,
    constraint "PK_OPERATEPOINTLISTS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_OPERATEPOINTLISTS"
/

create or replace trigger "AIROUTE"."TR_OPERATEPOINTLISTS"
before insert on "AIROUTE"."OPERATEPOINTLISTS"
for each row
begin
  select "AIROUTE"."SQ_OPERATEPOINTLISTS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPERATEPOINTLISTS_227524665" on "AIROUTE"."OPERATEPOINTLISTS" ("OPERATEPOINTID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_OPERATEPOINTLISTS_514891354" on "AIROUTE"."OPERATEPOINTLISTS" ("COMPANYCODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."OPS_ENTRUSTMENTINFORS"
(
    "ID" number(10, 0) not null, 
    "MBLID" number(10, 0) null, 
    "MBL" nvarchar2(20) null, 
    "HBL" nvarchar2(20) null, 
    "OPERATION_ID" nvarchar2(20) null, 
    "CONSIGN_CODE" nvarchar2(50) null, 
    "CUSTOM_CODE" nvarchar2(50) null, 
    "AREA_CODE" nvarchar2(50) null, 
    "ENTRUSTMENT_NAME" nvarchar2(100) null, 
    "ENTRUSTMENT_CODE" nvarchar2(100) null, 
    "FWD_CODE" nvarchar2(50) null, 
    "CONSIGNEE_CODE" nvarchar2(50) null, 
    "CARRIAGE_ACCOUNT_CODE" nvarchar2(50) null, 
    "INCIDENTAL_ACCOUNT_CODE" nvarchar2(50) null, 
    "DEPART_PORT" nvarchar2(50) null, 
    "TRANSFER_PORT" nvarchar2(50) null, 
    "END_PORT" nvarchar2(50) null, 
    "SHIPPER_H" nvarchar2(500) null, 
    "CONSIGNEE_H" nvarchar2(500) null, 
    "NOTIFY_PART_H" nvarchar2(500) null, 
    "SHIPPER_M" nvarchar2(500) null, 
    "CONSIGNEE_M" nvarchar2(500) null, 
    "NOTIFY_PART_M" nvarchar2(500) null, 
    "IS_MOORLEVEL" number(1, 0) not null, 
    "MOORLEVEL" nvarchar2(20) null, 
    "PIECES_TS" number(28, 9) null, 
    "WEIGHT_TS" number(28, 9) null, 
    "VOLUME_TS" number(28, 9) null, 
    "CHARGE_WEIGHT_TS" number(28, 9) null, 
    "BULK_WEIGHT_TS" number(28, 9) null, 
    "PIECES_SK" number(28, 9) null, 
    "WEIGHT_SK" number(28, 9) null, 
    "VOLUME_SK" number(28, 9) null, 
    "SLAC_SK" number(28, 9) null, 
    "CHARGE_WEIGHT_SK" number(28, 9) null, 
    "BULK_WEIGHT_SK" number(28, 9) null, 
    "BULK_PERCENT_SK" number(28, 9) null, 
    "ACCOUNT_WEIGHT_SK" number(28, 9) null, 
    "PIECES_DC" number(28, 9) null, 
    "SLAC_DC" number(28, 9) null, 
    "WEIGHT_DC" number(28, 9) null, 
    "VOLUME_DC" number(28, 9) null, 
    "CHARGE_WEIGHT_DC" number(28, 9) null, 
    "BULK_WEIGHT_DC" number(28, 9) null, 
    "BULK_PERCENT_DC" number(28, 9) null, 
    "ACCOUNT_WEIGHT_DC" number(28, 9) null, 
    "PIECES_FACT" number(28, 9) null, 
    "WEIGHT_FACT" number(28, 9) null, 
    "VOLUME_FACT" number(28, 9) null, 
    "CHARGE_WEIGHT_FACT" number(28, 9) null, 
    "BULK_WEIGHT_FACT" number(28, 9) null, 
    "BULK_PERCENT_FACT" number(28, 9) null, 
    "ACCOUNT_WEIGHT_FACT" number(28, 9) null, 
    "MARKS_H" nvarchar2(1000) null, 
    "EN_NAME_H" nvarchar2(500) null, 
    "IS_BOOK_FLAT" number(1, 0) null, 
    "BOOK_FLAT_CODE" nvarchar2(20) null, 
    "AIRWAYS_CODE" nvarchar2(20) null, 
    "FLIGHT_NO" nvarchar2(20) null, 
    "FLIGHT_DATE_WANT" date null, 
    "BOOK_REMARK" nvarchar2(500) null, 
    "DELIVERY_POINT" nvarchar2(20) null, 
    "WAREHOUSE_CODE" nvarchar2(20) null, 
    "RK_DATE" date null, 
    "CK_DATE" date null, 
    "CH_NAME" nvarchar2(500) null, 
    "AMS" number(28, 9) null, 
    "LOT_NO" nvarchar2(20) null, 
    "BATCH_NUM" nvarchar2(20) null, 
    "IS_SELF" number(1, 0) not null, 
    "TY_TYPE" nvarchar2(20) null, 
    "HBL_FEIGHT" nvarchar2(20) null, 
    "IS_XC" number(1, 0) not null, 
    "IS_BAS" number(1, 0) not null, 
    "IS_DCZ" number(1, 0) not null, 
    "IS_ZB" number(1, 0) not null, 
    "IS_TG" number(1, 0) not null, 
    "ADDPOINT" number(10, 0) not null, 
    "EDITPOINT" number(10, 0) not null, 
    "IS_HDQ" number(1, 0) null, 
    "IS_BG" number(1, 0) null, 
    "IS_BQ" number(1, 0) null, 
    "IS_OUTGOING" number(1, 0) null, 
    "STATUS" number(10, 0) not null, 
    "REMARK" nvarchar2(500) null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_OPS_ENTRUSTMENTINFORS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_OPS_ENTRUSTMENTINFORS"
/

create or replace trigger "AIROUTE"."TR_OPS_ENTRUSTMENTINFORS"
before insert on "AIROUTE"."OPS_ENTRUSTMENTINFORS"
for each row
begin
  select "AIROUTE"."SQ_OPS_ENTRUSTMENTINFORS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_ENTRUSTMENTINFORS_MBLID" on "AIROUTE"."OPS_ENTRUSTMENTINFORS" ("MBLID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_ENTRUSTMENTINFORS_MBL" on "AIROUTE"."OPS_ENTRUSTMENTINFORS" ("MBL")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_ENTRUSTMENTIN_346442163" on "AIROUTE"."OPS_ENTRUSTMENTINFORS" ("FLIGHT_DATE_WANT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_ENTRUSTMENTIN_371821193" on "AIROUTE"."OPS_ENTRUSTMENTINFORS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_ENTRUSTMENTI_2059611793" on "AIROUTE"."OPS_ENTRUSTMENTINFORS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."OPS_M_ORDERS"
(
    "ID" number(10, 0) not null, 
    "MBL" nvarchar2(20) null, 
    "AIRWAYS_CODE" nvarchar2(20) null, 
    "FWD_CODE" nvarchar2(20) null, 
    "SHIPPER_M" nvarchar2(500) null, 
    "CONSIGNEE_M" nvarchar2(500) null, 
    "NOTIFY_PART_M" nvarchar2(500) null, 
    "DEPART_PORT" nvarchar2(50) null, 
    "END_PORT" nvarchar2(50) null, 
    "FLIGHT_NO" nvarchar2(20) null, 
    "FLIGHT_DATE_WANT" date null, 
    "CURRENCY_M" nvarchar2(20) null, 
    "BRAGAINON_ARTICLE_M" nvarchar2(20) null, 
    "PAY_MODE_M" nvarchar2(20) null, 
    "CARRIAGE_M" nvarchar2(1) null, 
    "INCIDENTAL_EXPENSES_M" nvarchar2(1) null, 
    "DECLARE_VALUE_TRANS_M" nvarchar2(20) null, 
    "DECLARE_VALUE_CIQ_M" nvarchar2(20) null, 
    "RISK_M" nvarchar2(20) null, 
    "MARKS_M" nvarchar2(1000) null, 
    "EN_NAME_M" nvarchar2(500) null, 
    "HAND_INFO_M" nvarchar2(1000) null, 
    "SIGNATURE_AGENT_M" nvarchar2(1000) null, 
    "RATE_CLASS_M" nvarchar2(1) null, 
    "AIR_FRAE" number(28, 9) null, 
    "AWC" number(28, 9) null, 
    "PIECES_M" number(28, 9) null, 
    "WEIGHT_M" number(28, 9) null, 
    "VOLUME_M" number(28, 9) null, 
    "CHARGE_WEIGHT_M" number(28, 9) null, 
    "PRICE_ARTICLE" nvarchar2(50) null, 
    "CCC" nvarchar2(50) null, 
    "FILE_M" nvarchar2(1000) null, 
    "STATUS" number(10, 0) not null, 
    "OPS_BMS_STATUS" number(1, 0) not null, 
    "SENDOUT_ZD" number(1, 0) null, 
    "SENDOUT_WHO" nvarchar2(20) null, 
    "SENDOUT_TS" date null, 
    "SENDOUT_ID" nvarchar2(50) null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_OPS_M_ORDERS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_OPS_M_ORDERS"
/

create or replace trigger "AIROUTE"."TR_OPS_M_ORDERS"
before insert on "AIROUTE"."OPS_M_ORDERS"
for each row
begin
  select "AIROUTE"."SQ_OPS_M_ORDERS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_M_ORDERS_MBL" on "AIROUTE"."OPS_M_ORDERS" ("MBL")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_M_ORDERS_STATUS" on "AIROUTE"."OPS_M_ORDERS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_M_ORDERS_OPERATINGPOINT" on "AIROUTE"."OPS_M_ORDERS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."OPS_H_ORDERS"
(
    "ID" number(10, 0) not null, 
    "MBLID" number(10, 0) null, 
    "OPERATION_ID" nvarchar2(20) null, 
    "SHIPPER_H" nvarchar2(500) null, 
    "CONSIGNEE_H" nvarchar2(500) null, 
    "NOTIFY_PART_H" nvarchar2(500) null, 
    "CURRENCY_H" nvarchar2(20) null, 
    "BRAGAINON_ARTICLE_H" nvarchar2(20) null, 
    "PAY_MODE_H" nvarchar2(20) null, 
    "CARRIAGE_H" nvarchar2(1) null, 
    "INCIDENTAL_EXPENSES_H" nvarchar2(1) null, 
    "DECLARE_VALUE_TRANS_H" nvarchar2(20) null, 
    "DECLARE_VALUE_CIQ_H" nvarchar2(20) null, 
    "RISK_H" nvarchar2(20) null, 
    "MARKS_H" nvarchar2(1000) null, 
    "EN_NAME_H" nvarchar2(500) null, 
    "PIECES_H" number(28, 9) null, 
    "WEIGHT_H" number(28, 9) null, 
    "VOLUME_H" number(28, 9) null, 
    "CHARGE_WEIGHT_H" number(28, 9) null, 
    "MBL" nvarchar2(20) null, 
    "HBL" nvarchar2(20) null, 
    "IS_SELF" number(1, 0) not null, 
    "TY_TYPE" nvarchar2(20) null, 
    "LOT_NO" nvarchar2(20) null, 
    "HBL_FEIGHT" nvarchar2(20) null, 
    "IS_XC" number(1, 0) not null, 
    "IS_BAS" number(1, 0) not null, 
    "IS_DCZ" number(1, 0) not null, 
    "IS_ZB" number(1, 0) not null, 
    "IS_TG" number(1, 0) not null, 
    "ADDPOINT" number(10, 0) not null, 
    "EDITPOINT" number(10, 0) not null, 
    "STATUS" number(10, 0) not null, 
    "BATCH_NUM" nvarchar2(20) null, 
    "SENDOUT_FD" number(1, 0) null, 
    "SENDOUT_WHO" nvarchar2(20) null, 
    "SENDOUT_TS" date null, 
    "SENDOUT_ID" nvarchar2(50) null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_OPS_H_ORDERS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_OPS_H_ORDERS"
/

create or replace trigger "AIROUTE"."TR_OPS_H_ORDERS"
before insert on "AIROUTE"."OPS_H_ORDERS"
for each row
begin
  select "AIROUTE"."SQ_OPS_H_ORDERS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_H_ORDERS_MBLID" on "AIROUTE"."OPS_H_ORDERS" ("MBLID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_H_ORDERS_STATUS" on "AIROUTE"."OPS_H_ORDERS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_OPS_H_ORDERS_OPERATINGPOINT" on "AIROUTE"."OPS_H_ORDERS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."PARA_AIRLINES"
(
    "ID" number(10, 0) not null, 
    "AIRCODE" nvarchar2(50) not null, 
    "AIRLINE" nvarchar2(50) null, 
    "AIRCOMPANY" nvarchar2(50) null, 
    "STARSTATION" nvarchar2(50) null, 
    "TRANSFERSTATION" nvarchar2(50) null, 
    "ENDSTATION" nvarchar2(50) null, 
    "AIRDATE" date null, 
    "AIRTIME" nvarchar2(10) null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_PARA_AIRLINES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_PARA_AIRLINES"
/

create or replace trigger "AIROUTE"."TR_PARA_AIRLINES"
before insert on "AIROUTE"."PARA_AIRLINES"
for each row
begin
  select "AIROUTE"."SQ_PARA_AIRLINES".nextval into :new."ID" from dual;
end;
/

create table "AIROUTE"."PARA_AIRPORTS"
(
    "ID" number(10, 0) not null, 
    "PORTCODE" nvarchar2(20) not null, 
    "PORTNAME" nvarchar2(20) null, 
    "PORTNAMEENG" nvarchar2(50) null, 
    "PORTTYPE" nvarchar2(50) null, 
    "COUNTRYCODE" nvarchar2(20) null, 
    "PEACEPORTNAME" nvarchar2(50) null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_PARA_AIRPORTS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_PARA_AIRPORTS"
/

create or replace trigger "AIROUTE"."TR_PARA_AIRPORTS"
before insert on "AIROUTE"."PARA_AIRPORTS"
for each row
begin
  select "AIROUTE"."SQ_PARA_AIRPORTS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_PARA_AIRPORTS_PORTCODE" on "AIROUTE"."PARA_AIRPORTS" ("PORTCODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."PARA_AREAS"
(
    "ID" number(10, 0) not null, 
    "AREACODE" nvarchar2(10) not null, 
    "AREANAME" nvarchar2(100) null, 
    "STATUS" number(10, 0) not null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_PARA_AREAS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_PARA_AREAS"
/

create or replace trigger "AIROUTE"."TR_PARA_AREAS"
before insert on "AIROUTE"."PARA_AREAS"
for each row
begin
  select "AIROUTE"."SQ_PARA_AREAS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_PARA_AREAS_AREACODE" on "AIROUTE"."PARA_AREAS" ("AREACODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."PARA_COUNTRIES"
(
    "ID" number(10, 0) not null, 
    "COUNTRY_NO" nvarchar2(3) null, 
    "COUNTRY_CO" nvarchar2(3) not null, 
    "COUNTRY_EN" nvarchar2(50) null, 
    "COUNTRY_NA" nvarchar2(50) null, 
    "EXAM_MARK" nvarchar2(1) null, 
    "HIGH_LOW" nvarchar2(1) null, 
    "STATUS" number(1, 0) not null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_PARA_COUNTRIES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_PARA_COUNTRIES"
/

create or replace trigger "AIROUTE"."TR_PARA_COUNTRIES"
before insert on "AIROUTE"."PARA_COUNTRIES"
for each row
begin
  select "AIROUTE"."SQ_PARA_COUNTRIES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_PARA_COUNTRIES_COUNTRY_NO" on "AIROUTE"."PARA_COUNTRIES" ("COUNTRY_NO")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_PARA_COUNTRIES_COUNTRY_CO" on "AIROUTE"."PARA_COUNTRIES" ("COUNTRY_CO")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."PARA_CURRS"
(
    "ID" number(10, 0) not null, 
    "CURR_CODE" nvarchar2(3) not null, 
    "CURR_NAME" nvarchar2(10) null, 
    "CURR_NAMEENG" nvarchar2(20) null, 
    "MONEY_CODE" nvarchar2(10) null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_PARA_CURRS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_PARA_CURRS"
/

create or replace trigger "AIROUTE"."TR_PARA_CURRS"
before insert on "AIROUTE"."PARA_CURRS"
for each row
begin
  select "AIROUTE"."SQ_PARA_CURRS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_PARA_CURRS_CURR_CODE" on "AIROUTE"."PARA_CURRS" ("CURR_CODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."PARA_CUSTOMSES"
(
    "CUSTOMS_CODE" nvarchar2(20) not null, 
    "CUSTOMS_NAME" nvarchar2(20) null, 
    "PINYINSIMPLENAME" nvarchar2(100) null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(1, 0) not null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_PARA_CUSTOMSES" primary key ("CUSTOMS_CODE")
) segment creation immediate
/

create table "AIROUTE"."PARA_PACKAGES"
(
    "ID" number(10, 0) not null, 
    "PACKAGECODE" nvarchar2(50) not null, 
    "PACKAGENAME" nvarchar2(50) null, 
    "ISWOOD" number(1, 0) not null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(1, 0) not null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_PARA_PACKAGES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_PARA_PACKAGES"
/

create or replace trigger "AIROUTE"."TR_PARA_PACKAGES"
before insert on "AIROUTE"."PARA_PACKAGES"
for each row
begin
  select "AIROUTE"."SQ_PARA_PACKAGES".nextval into :new."ID" from dual;
end;
/

create table "AIROUTE"."PARA_TABLESES"
(
    "ID" number(10, 0) not null, 
    "PARA_TBNAME" nvarchar2(50) null, 
    "PARA_CODE" nvarchar2(50) null, 
    "PARA_NAME" nvarchar2(50) null, 
    "PARA_CODECOLUMN" nvarchar2(200) null, 
    "PARA_NAMECOLUMN" nvarchar2(200) null,
    constraint "PK_PARA_TABLESES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_PARA_TABLESES"
/

create or replace trigger "AIROUTE"."TR_PARA_TABLESES"
before insert on "AIROUTE"."PARA_TABLESES"
for each row
begin
  select "AIROUTE"."SQ_PARA_TABLESES".nextval into :new."ID" from dual;
end;
/

create table "AIROUTE"."PICTURES"
(
    "ID" number(10, 0) not null, 
    "CODE" nvarchar2(50) null, 
    "STATUS" number(10, 0) not null, 
    "TYPE" number(10, 0) not null, 
    "ADDRESS" nvarchar2(500) null, 
    "REMARK" nvarchar2(100) null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_PICTURES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_PICTURES"
/

create or replace trigger "AIROUTE"."TR_PICTURES"
before insert on "AIROUTE"."PICTURES"
for each row
begin
  select "AIROUTE"."SQ_PICTURES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_PICTURES_STATUS_TYPE" on "AIROUTE"."PICTURES" ("STATUS", "TYPE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_PICTURES_OPERATINGPOINT" on "AIROUTE"."PICTURES" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."QUOTEDPRICES"
(
    "ID" number(10, 0) not null, 
    "SERIALNO" nvarchar2(50) not null, 
    "SETTLEACCOUNT" nvarchar2(50) not null, 
    "FEECODE" nvarchar2(50) not null, 
    "FEENAME" nvarchar2(50) not null, 
    "STARTPLACE" nvarchar2(50) null, 
    "TRANSITPLACE" nvarchar2(50) null, 
    "ENDPLACE" nvarchar2(50) null, 
    "AIRLINECO" nvarchar2(50) null, 
    "AIRLINENO" nvarchar2(50) null, 
    "WHBUYER" nvarchar2(50) null, 
    "PROXYOPERATOR" number(1, 0) not null, 
    "DEALWITHARTICLE" nvarchar2(50) null, 
    "BSA" number(1, 0) not null, 
    "CUSTOMSTYPE" nvarchar2(50) null, 
    "INSPECTMARK" number(1, 0) not null, 
    "GETORDMARK" number(1, 0) not null, 
    "MOORLEVEL" nvarchar2(50) null, 
    "BILLINGUNIT" nvarchar2(50) not null, 
    "PRICE" number(28, 9) not null, 
    "CURRENCYCODE" nvarchar2(50) not null, 
    "FEECONDITIONVAL1" number(28, 9) not null, 
    "CALCSIGN1" nvarchar2(50) null, 
    "FEECONDITION" nvarchar2(50) null, 
    "CALCSIGN2" nvarchar2(50) null, 
    "FEECONDITIONVAL2" number(28, 9) not null, 
    "CALCFORMULA" nvarchar2(50) not null, 
    "FEEMIN" number(28, 9) not null, 
    "FEEMAX" number(28, 9) not null, 
    "STARTDATE" date null, 
    "ENDDATE" date null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "AUDITSTATUS" number(10, 0) not null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_QUOTEDPRICES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_QUOTEDPRICES"
/

create or replace trigger "AIROUTE"."TR_QUOTEDPRICES"
before insert on "AIROUTE"."QUOTEDPRICES"
for each row
begin
  select "AIROUTE"."SQ_QUOTEDPRICES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_QUOTEDPRICES_SERIALNO" on "AIROUTE"."QUOTEDPRICES" ("SERIALNO")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_QUOTEDPRICES_STATUS" on "AIROUTE"."QUOTEDPRICES" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_QUOTEDPRICES_OPERATINGPOINT" on "AIROUTE"."QUOTEDPRICES" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."RATES"
(
    "ID" number(10, 0) not null, 
    "LOCALCURRENCY" nvarchar2(20) null, 
    "LOCALCURRCODE" nvarchar2(20) null, 
    "FOREIGNCURRENCY" nvarchar2(20) null, 
    "FOREIGNCURRCODE" nvarchar2(20) null, 
    "YEAR" number(10, 0) not null, 
    "MONTH" number(10, 0) not null, 
    "RECRATE" number(28, 9) not null, 
    "PAYRATE" number(28, 9) not null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_RATES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_RATES"
/

create or replace trigger "AIROUTE"."TR_RATES"
before insert on "AIROUTE"."RATES"
for each row
begin
  select "AIROUTE"."SQ_RATES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_RATES_OPERATINGPOINT" on "AIROUTE"."RATES" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."ROLEMENUS"
(
    "ID" number(10, 0) not null, 
    "ROLENAME" nvarchar2(20) not null, 
    "ROLEID" nvarchar2(50) not null, 
    "MENUID" number(10, 0) not null, 
    "ISENABLED" number(1, 0) not null, 
    "CREATEDUSERID" nvarchar2(50) null, 
    "CREATEDDATETIME" date null, 
    "LASTEDITUSERID" nvarchar2(50) null, 
    "LASTEDITDATETIME" date null,
    constraint "PK_ROLEMENUS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_ROLEMENUS"
/

create or replace trigger "AIROUTE"."TR_ROLEMENUS"
before insert on "AIROUTE"."ROLEMENUS"
for each row
begin
  select "AIROUTE"."SQ_ROLEMENUS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_ROLEMENUS_ROLENAM_595551481" on "AIROUTE"."ROLEMENUS" ("ROLENAME", "ROLEID", "MENUID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."ROLEMENUACTIONS"
(
    "ID" number(10, 0) not null, 
    "ROLEID" nvarchar2(50) not null, 
    "MENUID" number(10, 0) not null, 
    "ACTIONID" number(10, 0) not null, 
    "CREATEDUSERID" nvarchar2(50) null, 
    "CREATEDDATETIME" date null, 
    "LASTEDITUSERID" nvarchar2(50) null, 
    "LASTEDITDATETIME" date null,
    constraint "PK_ROLEMENUACTIONS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_ROLEMENUACTIONS"
/

create or replace trigger "AIROUTE"."TR_ROLEMENUACTIONS"
before insert on "AIROUTE"."ROLEMENUACTIONS"
for each row
begin
  select "AIROUTE"."SQ_ROLEMENUACTIONS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_ROLEMENUACTIONS_R_306373444" on "AIROUTE"."ROLEMENUACTIONS" ("ROLEID", "MENUID", "ACTIONID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."SEQUENCERS"
(
    "ID" number(10, 0) not null, 
    "SEED" number(10, 0) not null, 
    "PREFIX" nvarchar2(20) not null,
    constraint "PK_SEQUENCERS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_SEQUENCERS"
/

create or replace trigger "AIROUTE"."TR_SEQUENCERS"
before insert on "AIROUTE"."SEQUENCERS"
for each row
begin
  select "AIROUTE"."SQ_SEQUENCERS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_SEQUENCERS_PREFIX" on "AIROUTE"."SEQUENCERS" ("PREFIX")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."TRADETYPES"
(
    "ID" number(10, 0) not null, 
    "CODE" nvarchar2(50) null, 
    "NAME" nvarchar2(100) null, 
    "DESCRIPTION" nvarchar2(500) null, 
    "STATUS" number(10, 0) not null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null,
    constraint "PK_TRADETYPES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_TRADETYPES"
/

create or replace trigger "AIROUTE"."TR_TRADETYPES"
before insert on "AIROUTE"."TRADETYPES"
for each row
begin
  select "AIROUTE"."SQ_TRADETYPES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_TRADETYPES_CODE" on "AIROUTE"."TRADETYPES" ("CODE")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_TRADETYPES_STATUS" on "AIROUTE"."TRADETYPES" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."USEROPERATEPOINTLINKS"
(
    "ID" number(10, 0) not null, 
    "USERID" nvarchar2(50) null, 
    "OPERATEOPINTID" number(10, 0) not null,
    constraint "PK_USEROPERATEPOINTLINKS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_USEROPERATEPOINTLINKS"
/

create or replace trigger "AIROUTE"."TR_USEROPERATEPOINTLINKS"
before insert on "AIROUTE"."USEROPERATEPOINTLINKS"
for each row
begin
  select "AIROUTE"."SQ_USEROPERATEPOINTLINKS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create unique index "AIROUTE"."IX_USEROPERATEPOINTL_662258782" on "AIROUTE"."USEROPERATEPOINTLINKS" ("USERID", "OPERATEOPINTID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."WAREHOUSE_CARGO_SIZES"
(
    "ID" number(10, 0) not null, 
    "WAREHOUSE_RECEIPT_ID" number(10, 0) not null, 
    "ENTRY_ID" nvarchar2(20) not null, 
    "CM_LENGTH" number(28, 9) null, 
    "CM_WIDTH" number(28, 9) null, 
    "CM_HEIGHT" number(28, 9) null, 
    "CM_PIECE" number(28, 9) null, 
    "STATUS" number(10, 0) not null, 
    "REMARK" nvarchar2(500) null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_WAREHOUSE_CARGO_SIZES" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_WAREHOUSE_CARGO_SIZES"
/

create or replace trigger "AIROUTE"."TR_WAREHOUSE_CARGO_SIZES"
before insert on "AIROUTE"."WAREHOUSE_CARGO_SIZES"
for each row
begin
  select "AIROUTE"."SQ_WAREHOUSE_CARGO_SIZES".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_WAREHOUSE_CARGO__2114375251" on "AIROUTE"."WAREHOUSE_CARGO_SIZES" ("WAREHOUSE_RECEIPT_ID")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_WAREHOUSE_CARGO_S_761925940" on "AIROUTE"."WAREHOUSE_CARGO_SIZES" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_WAREHOUSE_CARGO__1977210970" on "AIROUTE"."WAREHOUSE_CARGO_SIZES" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

create table "AIROUTE"."WAREHOUSE_RECEIPTS"
(
    "ID" number(10, 0) not null, 
    "WAREHOUSE_ID" nvarchar2(20) not null, 
    "ENTRY_ID" nvarchar2(20) not null, 
    "WAREHOUSE_CODE" nvarchar2(20) null, 
    "PIECES_CK" number(28, 9) null, 
    "WEIGHT_CK" number(28, 9) null, 
    "VOLUME_CK" number(28, 9) null, 
    "PACKING" nvarchar2(20) null, 
    "CHARGE_WEIGHT_CK" number(28, 9) null, 
    "BULK_WEIGHT_CK" number(28, 9) null, 
    "DAMAGED_CK" number(1, 0) not null, 
    "DAMAGED_NUM" number(28, 9) null, 
    "DAMPNESS_CK" number(1, 0) not null, 
    "DAMPNESS_NUM" number(28, 9) null, 
    "DEFORMATION" number(1, 0) not null, 
    "DEFORMATION_NUM" number(28, 9) null, 
    "IS_GF" number(1, 0) not null, 
    "CLOSURE_REMARK" nvarchar2(500) null, 
    "IS_QG" number(1, 0) not null, 
    "WAREHOUSE_REMARK" nvarchar2(500) null, 
    "CONSIGN_CODE_CK" nvarchar2(50) null, 
    "MBLID" number(10, 0) null, 
    "MBL" nvarchar2(20) null, 
    "HBL" nvarchar2(20) null, 
    "FLIGHT_DATE_WANT" date null, 
    "FLIGHT_NO" nvarchar2(20) null, 
    "END_PORT" nvarchar2(50) null, 
    "IN_DATE" date null, 
    "IN_TIME" nvarchar2(10) null, 
    "OUT_DATE" date null, 
    "OUT_TIME" nvarchar2(10) null, 
    "CH_NAME_CK" nvarchar2(500) null, 
    "IS_CUSTOMERRETURN" number(1, 0) not null, 
    "IS_MYRETURN" number(1, 0) not null, 
    "TRUCK_ID" nvarchar2(20) null, 
    "DRIVER" nvarchar2(20) null, 
    "IS_DAMAGEUPLOAD" number(1, 0) not null, 
    "IS_DELIVERYUPLOAD" number(1, 0) not null, 
    "IS_ENTRYUPLOAD" number(1, 0) not null, 
    "IS_BINDING" number(1, 0) not null, 
    "STATUS" number(10, 0) not null, 
    "REMARK" nvarchar2(500) null, 
    "OPERATINGPOINT" number(10, 0) not null, 
    "ADDID" nvarchar2(50) null, 
    "ADDWHO" nvarchar2(20) null, 
    "ADDTS" date null, 
    "EDITWHO" nvarchar2(20) null, 
    "EDITTS" date null, 
    "EDITID" nvarchar2(50) null,
    constraint "PK_WAREHOUSE_RECEIPTS" primary key ("ID")
) segment creation immediate
/

create sequence "AIROUTE"."SQ_WAREHOUSE_RECEIPTS"
/

create or replace trigger "AIROUTE"."TR_WAREHOUSE_RECEIPTS"
before insert on "AIROUTE"."WAREHOUSE_RECEIPTS"
for each row
begin
  select "AIROUTE"."SQ_WAREHOUSE_RECEIPTS".nextval into :new."ID" from dual;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_WAREHOUSE_RECEIPTS_STATUS" on "AIROUTE"."WAREHOUSE_RECEIPTS" ("STATUS")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

begin
  execute immediate
  'create index "AIROUTE"."IX_WAREHOUSE_RECEIPT_548722824" on "AIROUTE"."WAREHOUSE_RECEIPTS" ("OPERATINGPOINT")';
exception
  when others then
    if sqlcode <> -1408 then
      raise;
    end if;
end;
/

alter table "AIROUTE"."CODEITEMS" add constraint "FK_CODEITEMS_BASECODEID" foreign key ("BASECODEID") references "AIROUTE"."BASECODES" ("ID")
/

alter table "AIROUTE"."BD_DEFDOC_LISTS" add constraint "FK_BD_DEFDOC_LISTS_DOCID" foreign key ("DOCID") references "AIROUTE"."BD_DEFDOCS" ("ID")
/

alter table "AIROUTE"."BMS_BILL_AP_DTLS" add constraint "FK_BMS_BILL_AP_DTLS_2017653530" foreign key ("BMS_BILL_AP_ID") references "AIROUTE"."BMS_BILL_APS" ("ID")
/

alter table "AIROUTE"."BMS_BILL_AR_DTLS" add constraint "FK_BMS_BILL_AR_DTLS_1563555861" foreign key ("BMS_BILL_AR_ID") references "AIROUTE"."BMS_BILL_ARS" ("ID")
/

alter table "AIROUTE"."MENUITEMS" add constraint "FK_MENUITEMS_PARENTID" foreign key ("PARENTID") references "AIROUTE"."MENUITEMS" ("ID")
/

alter table "AIROUTE"."MESSAGES" add constraint "FK_MESSAGES_NOTIFICATIONID" foreign key ("NOTIFICATIONID") references "AIROUTE"."NOTIFICATIONS" ("ID")
/

alter table "AIROUTE"."OPERATEPOINTLISTS" add constraint "FK_OPERATEPOINTLISTS_735099250" foreign key ("OPERATEPOINTID") references "AIROUTE"."OPERATEPOINTS" ("ID")
/

alter table "AIROUTE"."OPS_ENTRUSTMENTINFORS" add constraint "FK_OPS_ENTRUSTMENTINFORS_MBLID" foreign key ("MBLID") references "AIROUTE"."OPS_M_ORDERS" ("ID")
/

alter table "AIROUTE"."OPS_H_ORDERS" add constraint "FK_OPS_H_ORDERS_MBLID" foreign key ("MBLID") references "AIROUTE"."OPS_M_ORDERS" ("ID")
/

alter table "AIROUTE"."ROLEMENUS" add constraint "FK_ROLEMENUS_MENUID" foreign key ("MENUID") references "AIROUTE"."MENUITEMS" ("ID")
/

alter table "AIROUTE"."ROLEMENUACTIONS" add constraint "FK_ROLEMENUACTIONS_ACTIONID" foreign key ("ACTIONID") references "AIROUTE"."MENUACTIONS" ("ID")
/

alter table "AIROUTE"."ROLEMENUACTIONS" add constraint "FK_ROLEMENUACTIONS_MENUID" foreign key ("MENUID") references "AIROUTE"."MENUITEMS" ("ID")
/

alter table "AIROUTE"."WAREHOUSE_CARGO_SIZES" add constraint "FK_WAREHOUSE_CARGO__1925243485" foreign key ("WAREHOUSE_RECEIPT_ID") references "AIROUTE"."WAREHOUSE_RECEIPTS" ("ID")
/

create table "AIROUTE"."__MigrationHistory"
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
dbms_lob.append(model_blob, to_blob(cast('1F8B0800000000000400EDBD5F93A3B8B62FF83E11F31D2AEAF1DE7DBABAAAE74CDCB3A3FBDEC058B6D98981025C59D92F04652B333985C11B70EFCA33319F6C1EE623CD5718893F360601124838B34CF443571AF82DFD595A5AFFB4F4FFFD3FFFEFEFFFEBC7DE7FF7178C622F0CFE78FFF1975FDFBF83C136DC79C1D31FEF8FC9E3BFFD8FF7FFEB7FFEEFFFDBEF60B7FFF1EE4BF1DE6FF83DF46510FFF1FE39490E7FFFF021DE3EC3BD1BFFB2F7B65118878FC92FDB70FFC1DD851F3EFDFAEB7F7CF8F8F1034410EF11D6BB77BF9BC720F1F630FD03FD2987C1161E92A3EBAFC31DF4E3FC77F4C44A51DF69EE1EC607770BFF782F79917E4C7EB987DF7EC95E7EFF4EF23D1735C482FEE3FB776E1084899BA066FE7D13432B89C2E0C93AA01F5CDF7E3940F4DEA3EBC7306FFEDFCFAFD3F6E4D74FB8271FCE1F1650DB639C847B46C08FBFE543F3A1FA79AF017E7F1A3A3478000D72F2827B9D0EE01FEF676E0C653468EFDF5589FD5DF623FC22617C7F29BEFADBBBF3B3BF9D7801B10CFEEF6FEFE4A39F1C23F847008F49E4FA7F7B671CBFF9DEF60EBED8E17718FC111C7DBFDC3CD440F4ECE207F49311850718252F267CCC1BADECDEBFFB70F9DD87EA87A7CF4ADF64DD5182E4B74FEFDF6988B8FBCD87A7D92F75DD4AC2082E61002337813BC34D1218051803A6E357A35EA1858706D32A282296438BE7FDBBB5FB4385C153F28C960B5A2D0BEF07DC153FE48DD8041E5A6AE89B243A424223DB09CF61BC8DBC43C62B8DB4FF9D0F6DCDFDCB7B4AC78BD07D25817BB40E4DE8A76FC4CFDE215B8EBF144F9D33EB2DA2706F867EE9D3D343C776A32798A0DE844D6F58E131DA569AF7FB8733A7B7F27F01C7C6FFC55713FF37F2FFE8BC6FC31F8978A617B9E0DA4915FCDE3D91B4EBF4BC00872FD36211362FD36221F75AA6B3B933078BB92E33EE53C567D75AA8F31E0B753EDE42452323EB73D0C2BA9FC42C1B5DD6A4F5F8744DB096CC3BD12BD5B2257B63B550F928A473D27C7EBFD2878F692715FBD4B739E23A1B69ED5D1F81B9628FD0364C86BD71FA01AF1ED42023F482849B702D448FA32A964D14B197AF3825197796B44DEFD4F4A2C61707A947D55EF412BEE9B793046E9284DDE4860A7131BA0F9ED5AB11EED83E0411F6E264813EC17F89DE44009F1EDEF07E688551A21DF7C31697E9F061F169EB7E835B37FF5DBB6A26756EEFFD76ED7DECCC3CDF77A403E3967DFE70726D1069AD67EAE8DB8E1223AAFF382D8E598858C70D986116B6E146A897CC0E84DAA28B9DB5A3473B1E402B1E40F3FFFAF62C5A44AB5E001D2D1CD6D0746D757886B988D36C116FB7E1F12C16E770EBED5DFFFD3B2342FFCAA2369FFEC7FB77D6D6C51DF88F7EBD91F6E269D8EE0F13EF08ED03C7C7CB56A227B6532B374674862EEAA2BD634C43460311B391' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('30F505D3CA78F793302AEB30802F4E87A79ADF5AD4BFFD27DC26CE59D20965E09C5A879DF2DBAF3C081AEECBBFDC17D18368C2BD1B7D6FA5C26DF4E6E5B54FAB89464FCE782C85A98DB34CB4F05F4865C0748491908E3B2F39EFACC26C2C4C46FCFACB7AD3BEF0F8587398501F564D3FB4903A7C8C4F6DC4864DE9771060BB99711E6537D842DFB944EEBBAFE560E2E72B2734C68CE5A4FACC9975DC7FDB7B09A7C12DD0C42FBA3325A4E73F8D454D3CD79CFA3502DB14B47AF18DF71428012FB6C9C046E09A8CD008D398F7688C59CC48F5994425F82BF4B690D32C1668E2A7B1A08493245ABDBF3C8989679AD3008EC035A7211CC03623A9CE4887F073759497BC29218E200B4AD4461108257A03443B90E5F156726AE17418385C092E6097D7878FF498C7BBC70AD7F6F3F0A1CD1AADB63673984F83E508223E71086A7CE1C3AFBCC1DA8F3A709A3BDDDB3A1814AAA907B2CE41F32926F7BA6272F82B01B3D318AD93A2A814B672E6894F0EDB5DBEE3E81741B252E8AEE5BD7AD24DDBCBC3126FAA5DEA19C6C31F4FA1BC0689F4DAE258E579AB61FD2421B01464E840C9CF782176B95AF9F88732521D360C1752E8E5E410795B7181A71305710EE3CFC98B306CD101CDD71174E2E5A0EAD043F9AC8E10F53F19C3EA2E288D917B57D0EA639615DFF230264E7D166EE021DD3E1A6112533263CCA0A818C748B1AAB7EAF17BA3EEE6B71A5CE1EE71E31AC21B3B3D684AD7B94ABACEE4899A3C5193272A65AC0B9792281F542D859CC66135D00715F5F53F4593EF8948EB1A69E4AFCFDFF5669C563479DBFCB237A794EA49471397527D0B79DBDC33A96F316F9BCF09D39F256F9BCB688C97B73D25554F49D55352F54FEAF79B92AAAF9A54FD56BDF3533EEE948F3BE5E3D2D0EB33BDDC5248E7DF331D51FC10234AB9333C1CE3B4C50867090BB13E65458F93443225334F21A4298424289939A248668EAA319F881C48AABFD792CC4C7899533273342499399A92995B24D26B0BEE94E76D4A661E2B99B91FED512CC229BD794A6F1E2FBD9993663AE5374FF9CD537EF394DFFCC6230653A6F09485D235F453A6F0E4E699DC3C3C678726533812E7E069CE146EF106F572F020333278827AB483D1CA8B93106BE64CF769D5BE9FDC3C445AA82D83FD0D367E771433239DD58B4805E66D2C8ACB8F98B53DA4640F2FECBF39ECD00C0C8699431F728091C3208141DBFD657C82C2D32E747BBB1037A586E152C5FDC10D58F780ECA349F0136975A6040BB941C5DB1F3AB70A4AD21DAC1F3C75F94DF838F4A4DD2E8271CC810EAB84F79236772517B183FEFCCB0BB66DA3C8A918F713D2D260D4C77FA8864FEDEE0EF60632C8A52071B749CC2A98B2AF26C94466EC633C3BC64AF0188EE1244FF788CE74308EB446A93114768BA53E0B83482982AEF8FEC8D8A1' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('3446F99974698E10064AE9B46B8E3C7B849793706668F39729B11E69612FAF59677E2B9F39999C734D5426B388A7CA6084775EB0635519B2AFA64B4F1B849CF83D75146B82F6FA77D1D2BA5FAC77924714F2884152C4499A00C32A2AF2CF26F382CCF630F25C9F47889CF9B44B92F8B09293351AF505845749084474F98406D863B75162F86EABF382CBC660476E107BE3D002C16E143A4872E0545459FC79EF8C90F88C95FBD5ECF80223D164D09F3F5E324B22144E6C0E5DFFDE4B9EA528F1B6BE70A6985992706B35DDE8E28B109FB094B1F800B7C9BADDB4E4426A09133DDA8D41691D86910AFF82BE705EF07C1F01E304E9D145BB2134E75B3E46110CB69CFC747D76E960E76145E78BEB7F14BEDE5C7F8BCBA20A2754EED8589DFA3466A7D06C09A7873BB608A3FDD1E7E07165E7CCB5277CEA309573AAE35B4B764E75CE3E312CA4D6F53A21FE4ABC05037CBB93D3B589CA8D38394EC13E462FC7E9BBC9CDD12052D0FB87C88B616B149592231857E29978A7ED2F98BEF51C465D85BEC46C98E736F0B16658D5D895368AC71A044F23D12986731985C7837847CB05B931020C767838531D254B40F92C7EBF945740BE1B814CE6381863D8F2D40AB4C038A48CB0AA38196D10B45569E393AE82B653CB4BDA46930F1D3B7277A9881C65EE22E85EC5B007B27C35DA79EECC95681FD2C0F538594FD10119609E817440F15E0613E28C45B893DD83573AE436DAC82A48FBC52AA1EB9FB2A3B316F43D20F85AACC95E76A4EA05DF61B476854FBB0D7D78780E03F11ACFA2D511C28786EDFE30DC17185D4545455F3F7A7067855BCFC52735775E721519951FF7BEA698CC9B70B564F2991B7CE7A4C6F7A0DC1D131795439FFBFA479FEFC9EDD544E54DBBBD5E5BAEE131FE7C0CB1F70847CC980BA5D53E9F5C6D0D5224FE6C5C2DAD28A53EF408EF15DB3FBCF1536AD394DA34A5365D27B5A9BFB179ADD4A5DE058AAE9A9AD4B7D5F5D4A3BE48536AD1CF995AC4BF97534E91809C2221D3F43A9289B877ED328B88FF9D0023A7F94CCE82262A93B380ABB300E937302A99FCCCEE822AC0E430200B906BD9DAB3638C2C92781425B6608731227F6A8864F9FD6A8C0C7B6CA6865122EB73209AD464E77337570532FB1C3E7AC128BAA809B7D0FB4BBCC16F3D7B87837832DD3537B890296D4B46887619E1B544708DE41126E927CE391798A73FD5F59D34F4B7A9A1C7B977305D80ECFA79E9F3493B6F5BCCF80643E1EB70E17B4FCF89A3092F039513C27CEEDCBBA5FAEF948CBF9EA9A25724D20362EF2948AF7B71E43BE16EE630FCEE2C7C3719E57E997CF13973B8F5DD8CBD4493D48E7B67B6A4F78275299DB808575C4214B6ED1763358B909C89BA28F29C1CBC3A4A4346B9360C0F29E4B153DA4D068EF53D4C172B3FC02FA17FDC438E8079971725BD' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('9D53A77942E6DDE60999779CDFBACABBCD0F30EF343F40C572E467B8FD0E7725D0DE51548CE47CB61F581759F6E185C532B05F0BCF87C7831FBAEDE7E3A6C3B393D971D366C7DCF5FC97EC96231673E3F4D964661069A53E6B7E09C894C4C6D07517685C90223F56DF4AE4C6E85DEA411C2380429193CFAF43E282E5DBC83DBCBC764F239F6BE7A63D74DA43D39F097B68E2A6D72429FB4318256BF77048C9B36DA8248C697725CF76FA9605AF53FB61E1417F773DCA1DB76FF3091923F48E1E72B9C8650E1F5DC4C35F5CFF287C27546213FEF3E84570377427C86ECAE3C8065D0D07016EDDE0769BF0E9E8BB11F871C0E710DB77DE3E2750E9052674FD536E3893943C7F3889C68615751AA1AB242297E85F4544A6C92E239D3B18538735DC17072F8251C2196E1479EE531B9D8F1CC980A0FD261B2E672EB61E5E40AEEF20E9078318B61EC3164371847E4EB90F9301F5160DA8707BDCA3C5B27603240FF6E9D5A64C6A41EDFB493B685BCEA3643FCCFF74C670A4A1B91F21C322AD63956EC0E2F33F1167068E11795C6A6674A44D96427503036EB3A563DBC2BD995EF7CD9F3C67A18F67339DB95E5FE6448BDCFA5E1823A4F220F11DA12662313BCE2DAC6582A3843806A65329B1934D65CD60A7997FF19239A333822A721A88134FF71A91D3D7630DCD89E038637421EB2947061F574D8F0A1C3AD7049FFCBDF153DC46BA1972B299269BE90DDA4CC0F59CBC58328BA974FA6CB290C872EE1A4ED351B4A84E71FA4A8B7F4EE2B489CA244E398AD3AFF24AD296C031251B308AD4F2A7D31DBE445AF2C63439E9A9ACB5B97F5C8FF60374A361026B1D06C9F330086425984C89D5AC5595DC17A1F87CB2B746322626D9CFF502A105CC933159E471FED1A4E01269BDF97A9A9D64C0284939A783B688A2F0807CE67C1981D2DA4D9ECB6E6761754264791457EE9BBEF660B27C9AA8BCE9DDEF95593E48AA64D53819F758FCD1B4C71269E5A3739504380A37CF245727B94AA632C9558E72D5F3A164DB92BC6214ADA7EF265F1291161EA0E5D16B3BD8CDE98E26AF3389991F21340CCFADAB8817A1EC7B61EE1A4C021F663A396CBCC08D5E2EFA9296EF25F5A52BF967BBF60231D8BD64DF3862F91549586AD9B7763DFF5CC89045FA95BF9CE41F79A6A2EBDCE886E8AE61F21CB6095E513AED16F345F7056A822EE19169A97352C3268DF275C8BB57A651AE6170947AD4063C7F37D9EB445A1D4A9E98E0E05522925618B565958B21CAED002BADD78187CB3D82989536318C140E1B1E15B1B31062134AAA1B2760E725E334B6A046D75A26E1A62470CF2EDAF057936023D2B2BD84C7315821CB948FE97C1529BA89DAEEC912749D2D37298A4BB2476147F9724EE910DB519840D98681EC0B3F5D6BB8115A76B5F53B6D24636C24E87BCDFDCB7B4AE52271' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('62DEBF33A19F3E8E9FBD431E29283608A7786711857B33F44B3B4EFEC8C9CA7AA05684E4E7B61B3DC184BE55D6F11B868887B6ABA0DBD4AEA2DD3DB7DD384E8FFAB3EDBAE947D3A64BA485E6FD3F61EB951782760834086DD7F2F191B58848DB3579BC76F52081ADE75F1BFDBC03CB87B43BD6C4A8131AFC17F9A827B38660C160C7A1BE11EC777B881626DEA3B74D17D1D06B9F4B3B196B33909042CDE8F76D4E76D656D7928BFBA968A4004A8D1B52797E889B52F9052717F271796B22BE50DBA0C86F0DDAA62E9BCEB25795BF9C362CF2BABD469ECA9846628754E752CA2E15BD1C2E84620F8D745D172648DBB0B6CF70771CA1CA961773317BA563F28C0D1BD18C80E9186E1CFF2B8CDA4C282EB45661DCA62471A131EDC4FC77E2F3DECA6F17AE9AAFED7B75AF5D384B9C8379DA1CCB2E5CFE720AEB13699587E82A2734CA0DE094F6F48A75026EEED4BEA9433F4B907FC478BDF821A3CC0A6814ECE535A47A714296F0D5B79C4BC17A96F4AD2FD6ECAEF6B707D95F55E8FED21F7F3DED009D02B89BEE2BDF4DE4707F7083976BD2BECE26366D2CD3C622766311B9A754AD08BA1DA8DF9E62584EA9B49F123C868CC9C22484C9C747A4B59EA98C81EB112A49AEC493C8D835AD78DB7E13209FA8D5E98E5BE191FE74998C424A8AA03B0AA1D75C5894D3419FFBF938DC91712284E350CB0BF93BD2761B1E472AD25A2AB03F26D9393CB851E218ED79B4BC6A4E07F1238C46210682DD28742CA4B1A0BF9D95F07314E735209E56EA6B7DC10939C908D48A315C8F3886E26995C7503C35C572D66118A9F02FE80F35D46A40A2148C1BBC8B5C7EC62686C3BDA1B3A3FF9D3F6A3E41D61DE709E207984F103F40CB77B71CE12EE79B1F6E79BE39A31A1059CC014FD842A1E1DEDE9C3FE732D7D9E7079777981F60CEEEFC002FF9931F6E993F39A316FCC90FB6C29FFC8073FE5CB8E7F4594E2CC51332672A9E90976CC513B9CC58DC710BD6E2095C612E9ED06B37FA1EB72AE8C8CCE7624D69A90363045B40891DA4B67E7716BECB7A85C4E9BB512E9090BCE85FEE4B3C0AADFC02172D1C8950EF9B62D22918A930FB1CFA3851105958E5AA57A246E6DE8DE073788C3B3D505CA89977BD2E4B927B7EB6EABE838ACB925973334CD4708CE5307393EDB3A3E1926C620921A987835E430D77FBA5EB8A3A3E618E6FBEB34877B311C6E5AB3C7454F09622591C50E6F29F1C50FE9C7100B1971CA2F11CCA05E230300718D4A3D5FC33E37E8FE7B53E0CDDDFF4A0A31F9365987238D3976D951E95588FB4B057BDC79136D83CD4389595E490C6727345805A733F0CCB593B7A949E2F21A57E9C9F3BA494888BE4F1CE97EB69859D5F0C4B2D2CF78E35FB23FF704AFA20D21A21856354B3AE3B7ACE85CE145F7B5371F2B142D76FC6AB201FA30806DB97D669E663' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('F045EE93EB0561E04851E26DFD763EE61303755FD27BB8C5533AA5B5B451FAC8379505FC38C02086B1709AD93DAED0F9E2FA47E8A4A926E247F492A8ECFD533C49D38BBF8BA79239965BE78CAF6359BC005FB948AC62FD6E8C6EE1DB8C5DACA239D2138E278C4012DF59E6C8BE1B8B5F6B48477216917BF6330E8D90DCF30EB8AD79E1E5411B6E7879A88D1BDE659C8D1BAC11795B58EC81C2D32D6559B8AAE3756CE5BCD6BD18870FB60C676BCBE1737520AEEC806C4BE7CF790D89EEC3111C190529766746F1A57877D3E41F6BA232F9C706FBC7083E2FF19EB2DA61296ADF1A4BD7561962738F562592274FDA653F08AF10FD7CA4F7067BF7567DBD7BABC9BBD7EEDD633CD2954BE0510E434D89FE3C0E79E58E9C3652821C39C2499E1C39C2299D1C39AD995F821C39A269921C39C247B4EEC8114E3275E408A7F2136608E626FE891027139F1B5E6EE273C3BB34F1B9C1FE1CC7A1DF56E2D438596B537AD6949EC5373D8BFB8DB5A3255616DEA0C5E4EB9A7C5D93AFEB75FABABA72C17AFA8348DEAC36BF512F7F90219992237991EA0590CD2154FE72F208919797175DA5DEDA693E05CB28DCBDB4B29B684A68AB8EACC41DE3A2B9A2CAC748E440B01B89129AAD3E876BD067E5CB80881638170F06DD5582A2A39CBDD4C169B3E67A5B75B1B36449847DF624FCE5B4271169E1A1E193B4CBB8483061FC2FE17EEC9C0E381F7711255231A90EBF0BA7F261C72089BA6AB7F2193DE86E21C554712ADE3509FD49E897857E04DD3E121F7D36D5B026F30F1A9ACE528A426C1044B84386F0A9E1382DEC37B0B0F31DACC7DACEBF9C96379196AC6F34DB7C70B4369EF88D870A9213928713622D1E9F53069A7035AB184C49B8D5FF555A3B6BC9BC6B938E3C4269CA72E5A8FABD60327CF27A27B92B40EE6E4CB38FD0459F4D12972C24D0D020293807A3CB414CB853A3E221060B42ED1634A7CBDE02F8D2359C938371128D0244632A14E33ED231FBF24A0232A79E9FBC671595975F8FEC022C888FE206F482072FB0BCFDC187A3D8A1D7133093C2F57AA48AE16EBFBB4FB08754C9BF9CC216E4E59C8DCE55C2E939ED313CE24A7C1F86836F2A9B44D1248A24C7C6E3DF47BFC93E9CEC3FB230C023947C1B4318E4FE4FF197E8A48446ED918C0F19B409A74F7CCE5A141D13428F7E397A5BBC56189762F6D1A40F10698DB12C045543B8C818C0A0F94CE35FFA790776BB08C6B1F0ADBEB36E271F436ACAD56EA2F2A69517F257BC67875A267F3E8658EEE042316C72B9F4E1249B89B42C1879AEDF7A504C8CA166C124F1617E61C5E8D417F03AE629A2CB4777633F55152586EF6E856FC4694EB6370E2D10EC46A193E7E8CB1C560915211ECBB19DD0FD6A767CC1475B04DB1051F8E325D311C213B1FE9E12D7BFF792E7914A85CD2C69688B731FFA1869' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('A74A101FE036599794BEBEAD5EC2448F763C90682E77E433579EEF2360F4CDF85B49AE96E4EA54E749796616CACA985C6BB792C360E76145E38BEB7F14D74BD7DFE23A961F45334AB947C25394F24E7D1AB353689A3E099DA64518ED8FBE7B0D5E5C7B81B0AE6178F78730F854FFEA73A40BA9387D3E1B33AA201D11EB11FC2FA5DF7BB94A4415B89C5C160D542697053F9785992E5A165F05FE6272521069A92192BB8526249AD94EC4C638C2857653885484B1FA56223746EF1EA01B0D13B0EB30489E874198706B967750DE5BBBE1BE08C57FD3C990D366DB4465DA6C396EB6A10FD73038326EB8F957D3A64BA48587874FF227ABC046845B6BEC8A3130312B748F6F574A1808F0EB83B3C2E408E24943C237E2311454C4CE4B9D6DE9AB6E9C0064EA8DD3D8821A756B1BCB41A5139EC03DB11654211C9CF35BE72A50B587B5FA4FF53706557E2AE0A46DA606F41173D9B793B0FBD9644E36AF43512681338EC02956708BC8C95E71CA6FD7454FEDA54611547F93F522052A415922D3242E2BAFD0B478B8E8B4E03F8FC8B466BD42E1F4D9243089B42C08074A1C641F3F7A3F046B97D46C6247EE0E66815A1636397D36656013698D91FAD961A4BCF51363D391D4D7766203EB1D99630BA66E2DD50BBEB3890D12C2244188B4C651F2F2C9D00F6832D854696AAEB97723F81C1E63E8C8489F091DCBFB2FC6CD868430A927445AE7A132E11622B9ED0C3590F07D642F5CEE7E62B5CCD64E86471F61E804BCF7765CF15617574070004C2F5EE185272891A0F388051F6D600AA1345179D3DAC638219436B35E3F0BC92813924403BFF69623451179333B9BFCD41FD56E52A4FF72D0DD8A84BEF7DB8AF3CFA77D9848EB3C4E57D839AFB66597D875842C8BFC9232F98ED77E995FFFC50F30BFA68C1F20AEF2E089AF7825AF2473099C7BA02C5736C7E6CF8EFE7787FB28CFDDBDFB047725C4DE272D72A4D2A5491C1A7708601CF3695D06C5B379F0318CF617773BF43FA67282E2D9422576968BC161653F8CD136E48CA4BAA2367F5E0E6D73D9941BA5D5F99DB8A9EC2EB1AB287599FD2AE29FE33AC5859FCA3FAC1D3BF7EED9B2A15597176A2A95C5DF7108829D935D6F21961194C0E993EA8F3E1BE3D2177CF75B9FE6A5B7CD8DD03E79955D0ADBB1603989B5EC641F8C4C88D4FAC15B06025CBFF081B2A3E3F6FB085794CF23EFAFD6F3A3BC2EFDCC7491CDC10FDDC15955180EFAB8E12FDC005323831BDACC0B7625C5B62FD2E4F19A3C5E93C7ABE6F16A745C8DEFF6AAE6BEB03BCCE8DC5EC77DC9E9553B91A7C40BDF7D8A4FE3CAE202CB7E2928FCF70A360F7F189AE61D8CFC17C41565117039D96BB8FF06A3BC83BBC87D4443F3C5F58FE8AF5F6B9C71F172DAE294E3F2F73F52BC6F1DB7DBB47E53FE4D5D59AF7FB3703DFFF4C1BF7DAC4F5D36' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('842D1337DBE3ADC1F7B3C439A710EF1CA78F48E13A93281D93D08209EDB4ACDC60577EFD538FF1ADA457701CD70BE437B028B2FC75DAB19F7BF1C5EBBD981BFF293FBBC1539E7F3564F4339CF4AEDD95873E8B5EFEFB25FC75A66013642DA09D0525886144BD0250DFBDC7175A91843460989C5BF2DB90E552D625452C9A33FE9B5F3A54035BAB2EC871502BD8D719D0DCB34C2D5F72572F2D6F83ECE22A326B575F5E783E3CE6865BFEC5FF41FB8593FAE0F2AFFE9DFAAB55E9ABFF93863FA4380EB75E3AD5A58CCA342B7AE6C6590DB34BD220D8BDCB54CADA9B67A5F36C16162F21B51BCDB87740738CF4C83FDEFFB75A9FDA804F41DC33F0A97915E0FA8C9BF01146389CE8FAD8F58AD80D597DF5D0A7176CBD83EB77B5A1F22165D4148FFF8944F5C91C1E6080039E5D034B43BBF886DC8613A94A4CB76B8C7EFF50E29476069ACD9D3958CC75D95115CB764E7F36F251D3072476BA7C9785A91AA99078EBD46631CCD5D5162A1EABE55532F158D7A0D33401BD4E6EC5185C96DB138E7470E689EFE8A51F9A39ADE52322B75DBECFC46E6DA4482C576A7E85CCAFBFFCC28BEF281A35827CA399062A5157FAEC7AE2EED488A8DA99888211EB1FB53362D49F1109A45A19311A83119B1B352A23364F031B2346D763C4D37136C3C5888DBC57798FC46EE74376F47C568525B056132C3FBE6A68C408ACD430AA3494B32FAEC6375A98788FDED6CDCF44C63132A0E246EE21BE4DE2A1F28BDD6A5417012237A5CFBA79B41727B5F673047E6A1D061AFA6580ABF1D6E519A43871CA3F34F258EB57245EAB7EC022B7DA8911F8EEA20B62EC03AA368D6024504D044D3BCADF5DCD64D00DCB593BA9EFD6C1FFC6CE9C639CECB1F00D1EC3A859E8757E4964CAF347541B1E0B39125B125E14241BA987630439493D56346DC953E5AEC69DAB52470AD6696312C2FB4D9CB8227362AB646CA0D0C07C2CBCDE9BED5ADA3312B3B58CF96B67B17ACDA1A6A96F2940749EF47355347A9E6AAE5B44612770E2A3C6368CC0418D034BC53B79359FAB324FBD7A4CD75CB79492A933535191869DA59AEBD054588B4C82337335B66644266B1C789A369CCB3EBD1676A392588D358004B1DA956558434BAEC3646F4A9ED1E7DF3571438F64BC337F10CE87D2FB487A9C7E25512EE7180A311498476804BE651E3B9A3691CB418CC0D7595A26FA26415F9C3202EEE1B7DD37FC23FC413A77BC89619ED611E719AE5546C3A0164C2A21E5F7EFCE59A0B5287C8D592F31CE7900358CF3A30E8C5258B6DE90F3335A943C84DC0C95BFD085578EDCD5C1CA4FE991B2284F1B5AF6063562D48A16B12075B62DA26A5B3D958EC41B84973A396D7F70032258F1A413012DA76D1213218A479D184678E7053B2246F1A813234ED66100C97D393DEB4239C6B3638C9D212498D2C36E9C' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('D27597641E20BCD38D9A1E74BAB88293845B7F8B0A39CE2F2E4B35BD06DC8B773A50E7AEE7E7F5F86B68A5679D28899BDEACAEEC0F6194ACDDC3214D142740925FECC287AE7FBAC6AE0E5A7EDA85146E8FD899B67603F709E27F9100092F75E002D77396517824C9CCD2B32E94AFF24AD296C031251B90902E9F77A02D609E215CC3393DE946C8EE882321644FBA103C1F4AB62DC92B1248E96107CE1AF162AA8BA4A7E86A48978FBBB04AC6521DA9F49002A74107383FEAC4C8A37E0488FC4907C26560B20673F9B803EB321654C3BA7CCC809505B15AF1B257BA308941813A2EF1350AEC93CF9708797A4A81B46A455A512219922939F925A704A8CBC79458D9D1EC46ACEC31155604DD46A0F4190D8A8CEF3326EA4A978FA9B036A6D908943EA342C936D146A0E2310D162EBC425EDE978F69B0D27DB3B159C5D32EA42CAB9D84523CE94068D76C58349A06B5834AE3383BEBEBDF9F1E516234EE05D5173AF04AA5B36B50A5671D28A5CACA3594D2B30E1472CDD51A20F9B50E6CB2C7A5864D7E8D1AFBE4476A013EBD53412D7917EA26FBF924C0BBD27B15E3BDE1B8C085DBAAF9C0C0A957654741CD7D427342A084D4E89BF870D95D8AA168CC69278C085DFEFB45773A33E0CBBD6AF54ED0C19206ABD17FD263B4DA72B34923469DCB7DD93D9A6CEE7217DB9D27F4D8A4E16B71F30C1940424E71EB0076E5203774B2250B99D849A287871EBB7500EB129F7D00ABB9B084316B4D97BDE84A53C26CA9F54DE64B2B0E61141ACD20F62120A77512068222FFF3A21BED19A0A5CEB459521488C4E1219B78ECA3D39E98481825864CC68BBED1E53296FAD865E331A01346B0D522ED318C9D2975A4A164CBC3BBEC3075265EB9D3CDB62823386944696CE77E234B4A076B18CFCECCB15A47DB72C72ADD6BB0BEA9201B46ACC935C03E50848BD4EA43D491F9440CF2D773084A7D68B2993A90C48AFC960B9E9A87A4239FA72BFFA192D1431822B2314889DB30600D06269721EBE2A1D68C14A69C146E8335026F31942BAA0F5DCF5C8B81D916A581E8B48007A6571049B558F1CDE35F545A3A85F44FCF7EFF606D9FE1DECD7FF8FD037A650B0FC9D1F5B3820CC5833C48149FBFCC7F79671DDC2DB69EFFCD7AFFEEC7DE0FE23FDE3F27C9E1EF1F3EC42974FCCBDEDB46611C3E26BF6CC3FD0777177EF8F4EBAFFFF1E1E3C70FFB0CE3C3F6627BAE26209C2825618434B5CAD3CC705F78518C6FBB73BFB9B8E484BCDBD75EBB486068708014A42A390AF5692DBC21C507F8DF798E5BADA64513CC791017A85F78A34FBB0849EE86DAA7E8635C85D78D0885D2655CBA3A683EADD1FC35A695B9B6CA18B23E07F683510B76B5215DDC3D55069B034B3615C356748D84F7FB87CA98D4F25D6A835F5906D5C9A49AEA938768D85437C1504C75F3A7E2A6BA3ECD2C' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('08769A095446B0C15782F7883F9334E195EB5494E166920570EFC8237425966B73E2318997261C1AF9D2FC6D23D7CD87701D2295B1D9C56C173F32E168D2BA8E93FD488F6382B564DE5DC214BFD1A358B6646FAC4B94E2377A94A2226719A5F88D09C5B66A2036534B4E0538CB28A71FD970AA8D297EA347A996562DA3E90630255BD19686AE6844D973EDD59DB9D4392D712218CB3A6F0010B6D8AB000DE55D5A31B8080CDCEF3AD0F95736A4BAEC39FFCA80E4C5C9E2E8FBF8AF3ADA62A3AAAC88A0DE30F09645A21546497A1FC5058C6EDADA66CDD427A73EF7C56F9380BE4D01DD1A0F6492CECD4834A2B9ED6B31AA7F5A07B0FC79FA03FDF74A8C3EF847856D142BFF911E67619FCB9494A1D0EF9209349BAD57FA21CE5CC055B8DC376CCED9F1564D782B76BCF97F7D7BAE6C637FCE6A49976D0838C52CBD3BE472ABD080A331ADFA94DDEA66FE4C515587D5CECF5877BBC529620438096DD41BF2BA6F47DC3701AE7BE1D9EE0F9CD2D4D469E92B4EE6EDD5F71CB919951971E5C6089400B892D00EFAB54F0B1B8713E1F51CD10C1261DB6182AF886A982944C0D66D8949BE9479EA533353D5EAAAB661A6E71A9CBAD363AD6BE0815913485BA87FFB4FB8CD4E65D59AA8CFFE0164DB61931065D4BA4E58C665D5E90CF7E55FEECB259E213DDC4B0F4C5A547E95C630CD30EDE59CBC64E68CEB458F9E9CA699D5CDA5D36F76316A0B0F62E0DE7CA885FFC2791A08F91254D3EFD18E87219974495CAABFBA23481BA4C1B1ED08294E9591531C360ECEDA5363DDAC458C3C9B62D5F924C562E593D2450F043476BB4476832DF41D12A22C6932509DDE98D559C8F1189DBB19567D267234D6B9C8F1EAB391E3B1CE8775DC7FDB7B097100ADCD7AB656EC1E2358A056174481C8B626CE6848097C6A42447AE0B20F6A759A0B44B6793EB5B136D1A71632CE7481589FEA029179AEBDA74009C853AD2C3545EB33D319666DA2333CC679CEB06A13926131CE47DEAEFA74E42D639D8D0C8F3019191EEB5C28C15FA1B785C4C950B42FBA22831EB351A056A7A340649B8F020D0787C878383AD407B16635E7786C737CEA6D6D924FFD659CE5538F6BD37CEA71CF7926696C05660F9F1EF4FD5C25222F67A0AAB94ED4674D97D06B8BB184CCB8224BA8846559C2655E9B2564C2022D21F7949840961B041C7ED26B4DA5BA725D552E5822559759B5E502FB749E9584BC00EC1904F1EE91C866736BBEE8A71DA0C5F052DBD4D02A60B2822AF7595D6843264053DDA76DA4D5C48C32CC0FDBE2AFAE4671F29F26CFF950CF7971996015E575252A54CEB870F39693D0D83CE66404315EF3D7EE5FAEDE5C70E15C595BB9CBC260DC3F5F8FD73AC5A9762CC561B48D9F718225C16124AF24730998BD45395E5D5DCDF158B5555CD2E1909DDD2DA36D34C5364CB497F6' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('C2FAD400C6E4BBFA9C5476CFCF36D3CE490C13F4881090DD72D7F70A73D33042DFC72A46CD15A4239DD266E6F71C8DE00CCAF198BD413922C11D9423B26ABC052251A9CA317B78D48A9E870DFD66923FF898766D4A36163019E723C5A94F468AD4CB47CAD1AFC9CB9FFC16FC0CAFDE2FF5167CA4423D0222BCEC22A3CB530C985F0C78B2B2272BFB46AD6CD229DB7E16760D89C9BA267C2DC6B21E9A8FF6DA2DF3D76343F3CFFC9AF2B46E778F1695FB37657E4D995F5D586F2FF36BCAD562C09A72B5A65CAD29578B12915FAED65BF0DB4DF941537ED06DE707F1CE8C997FCF949EEAA8CEEF326D87D1DAFD9E578CC395840890B98F4C67CDADE49B395FC84E5292552E3C6F22CB6ACA8A9AFCB593BF7698BF966B5614118DCD6F3B654591B2A2EAC1FB7356146B0CFFF57874A7ACA8292BEA0267CA8A624223B85DA6ACA8292BEA76B3A2DE82B76FCA3A9A229A53D6D164C54E566CED396BDDDBFA5DB8C30CD96E400A5B9606448C397B8714DF2AC21D527AD950D27BE0EACA8C2DCD54E6727CD958108A32A79790B2BAFB9400E959F5927A8A86F42CD6A27A9BC30E09CB1AD6C6C07A2E23D61CFA908035072A60C64A6B7C5765A4AC6B0868128E6F52388AD914AF566A3CBB317CA09C25A3D008D7A62FC548D4BA18642F82B13F90C4A9A5AC0D76790A8227829D09B425BB99B9DB4530AE9A98F3B9092C3683C3AB7A906485CD8584FEFCCB0BAABE31C3D4BF281A9B6BCC844F68C38551DD896182A562D9C0647562A8E153D5CBA92F8912E76AEB3148DC6D42BA218A6D419261A85664D3A76296A47C8C67C718DF2A55F3A56DACD9C652B485CEEA4D4B650AC999B63624ED81DD9796E291EE2848F198BDD12171B1CA7A9FE51A66172B57809079C88672BA55F91228755BB07A4653FEA92B3C92CCA4F0E438352D21876254147234CC664438CC65E3DBD17C3259276BBC1BE32D2B9C57DB8A8CF0CE0B7683B722320CD556D4F469E35634E88A80E1B7D00CD72F79DF42C34750DDFA42BEDA128C9334FE39780D36E0502DC2C66FC52884168C3CD7AF056780A9482A6B6826497C480C695BC0B655D023B0BD80B02E261600B04A0A845317160887D91E4DDC28317CB76A6FA1356EDA862AB1595C76E406B147C2B34D49B314764410EC0868409B332321B6C6F9257235F4A99838C34466934419562D8C9A61B171D9FD6A767CC137BD9691D08F9B076032DACD3F5E325D2EACA021E3F9EB43A6CAE94C9873E8FAF75EF22C4589B7F561754791D47BC55E21465164952D406749955097C566701CE324DCC70447F2C6B2F5B5D5C3937C805B243DEBF9E39601649B55A55EC2448F7675B82540133067455B8761A4C2BF60253CB8D67553055F00F3314724AE71D6503DD888F47C9C36C4C675B53426E61426F9184530D8922CE48D69024D66369153291B' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('EC3CAC067D71FD8F0471AB617D59D7BE48EA47B680BABFC585512A90B2A4CAB8300A1356B995CD2DECD3BA4FE4D631254E55C6F053EB183221E3562EC2687FF4AB9E07D4CE856EAE372A9334402D5D7BF5115C2B4C638751AAF90218852D558077BA4EBA3F134E5CE0ED99D57B89F6D23A12DA4A998F6CBC4A1B6372635070C04D593F27CFF050F3A71188C6FE69F9588C018488C3E8107971EDEA58A0D9C044DBB3D570796C3762DDE63863B207AF0A54EB195F8DD7026DADF03579BDF1EB7AE2199A555594575ABDA5F847F6D01DA9C7CBFEBD5C46E1F1D0D4CDA5A96F8C01B875DDAC82CD7CE172783893A8A3DB48649F0830474894CF55A19DFFC432CF40BEABA1143F329B2A24DD36355598EF6DCC623F88E388B1DAF4776634C48C4434C48F4CA62CFC66795505E31ECC90E1CFEA46C8AE6E27F0852965F7B7338F5B04DD3A1C8E74B12201592683E1073DF0F2D819C97B9C46CFD8F10EA9DB9B0468E8778A3667478C0E480DF50C18C5559D4FD64D037B00908665B1A97D26C4C179B893DD8357CB7D45DC87563F98CB92A130E6BD2A68B745C081EB17D921979BA9A560C1A2215B238BBFBE7DD557F582EF305ABB9536217BFE0E986B89A94536F4E1E1390CAAEB0EA8C058E91A9B195EB3A7D88C29DBFD61B82F30AAEFDFB6F4D5901E80C9BA7B6F02EFD1833B2BDC7A883D10F379497D996C3465A180B9A5CB0A6212132BB63D4F38372EEDFC3C43CF159EA313D30072E41EB9003337F85E5749669276C7AA93602472292D04D6C3695E3888C8CEA1C948FC198CC4B71DEB3EC69F8F610277A93B74F809EA4E3C3AC3B30B438CFD89091BE4281CD279D1A31EA1B8149390E0857E646A5B53C3FAB5AADE24D6F64CE1C0291C388503A770E0140E9CC2815338F0E70B07BEC6F0DD6403518CF56403F5B081D0260CA392D131DC0AEA42A4B383BA51C45842FC521167C71829983141C5498FB8008B59C72986A52930014CD65D560DD1AFF7ABBA9AA3EA4892A3078C7A0ED6F5C328C99A5155F77593D927773BB68D08DD1FF1CB1C3E7A01413D410C33070B4563D64F4CB885DE5F5563C7043250BEB0593BD6B377385481AC9562186C38BC4E1F95848D11FA5ED575F979A3DB609E6AC686AE2A6C3E4C5C31A6D655A0AA8C23F60A3513DED11501C95853A2120DCAA47F5D4BFF8A73DF055E423CB4AF163C6ADDAB15438CE695AF315CB4794E5C61CC159B17BEF7F49CA4C5BD2E4C425559AE586B7EE55858663AF76E5506E49058783AF7129B14187AFB14DAFF62EF2948EB253AF25D6D1FC46E83B46862FA90417F0DC3EFCEC277134225C699AEDF390B55B2D9EB7266ECE5CCE1163D74899A09F6FF397320AB5236ED2CF8DA71EFCC969790DA669DFEC6A2E5E053BA710D293BA96B31A2157D9E45E17718D5408B1ECF4CFD0E983DB153C66C42' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('4EF9920DD7F090A2173B55416C2848D3B31C36597C0FD3B553C5BA07E9A261C3FA8201600DEB8BAE6ED680112BEFE3A2A63DE6BD5C306A90793FEB78794F59F1F2BED6F1F2DEB2E2E5FDAD7249DE5B36FEC8FB5AC5CA7BCA8695F7B38A95F7920D4BB11CF9196EBFC35D0D0F3FC2C98060CEBAC630A09396D5AD65163A8CD576332C428DD6148C55FB5E783E3C1EFCD0ADECBC0B45051B43D52536DFC7A42BD3A14CBAF2F8BAF2DCF5FC97B4DEE8301DB9118742376EF9568C4E9CFAE7C8A954A987AE4F3ED509B3AED69D3099A375689C900E4A6EE94237D1AEA0F5696B0997907371C6656D6FEAE5A93B6653EF0EF3ED9EDC72F0784462AD6DE41E5E084E22D9948C876B7B77B895A698F682DBDE0B1237AD71AAEC0F61543C1CBC31D08052ED12744062B68C5343EA320968B6623F5880F958D6C283FEAE8EB750803AEF8555BF9623C562BD94A398CE4A30070970F61A3C8FEED14FBEB8FEB19623B59036AAFD4552376C0949B109FF79F422589D4DCB049F378A09D854F3F0186D61C33C58FAC69441AFD95062106056AD35128DE04C656BA3099F8EE857F0E38053DC6B5B8609961B5532C1570367B9376C1CD71228D0F58B0CB98152A419894674B47D2D465E9448D6552C9C20982707B2AA5825DC3AC796705939360DE212B319D3206E8F5446DE4A8EE1BE38981BEAC369480FCE1A8D23B3D3D48D22CF7DAABA0C24D354A4652F2410D4AE96C8C080C678F9C7D6DB214E767D07AD7B18C4B076B64556E668E3915407AD7C840E86E2D79A4E20C1D88BC9F141893229BB57D89BC2ED11535BBB015AB8F85F43B7A84E409A9D8A02E4ADC409E77F3A755F00FE91D11380C6A4166C9CEB3263A4313D499D6E20B5ED6D0ED2ED834D294B8E51E018514D2499C0DE989A63988C02A9E60767F480CF968E6D579C25D94F2C910C62157543E951463D1F2152C5F17480587D26E95013E0D2816646CB5B57A4FE3537B3480164A63034FE8C8442845A874500B1B0BD6DA296AD418F1BACCBC8C462162764667FA5B068BE123BD994D5624FD93CB1CF7D55CA65386C122E43AA6DBC1914E3D67BEAE189291BBA7A62C901FC4EEEFC09B9CF289CB01B86E304CE3E2E04298B468359C2E2F31F699EE581C4F7389F23CDB7345E5FF6059FEAE6933A4E8932A9E3E3ABE3C0F59CAC6CD3302DBC118742F96EF9568CCEFD1ACAA44FB2851263922D6F56B67CCD6ED573D2DB5307CA97362C1A19D3FE7DA39C19761DC3C6344977C0A73FB3D6FCFAD180867AD607EF01BA154DF701484CBAED3A0C92E78AADA56BF68A4D0A6EEB97EE227590F5BE5DC37DA9E318D2032BCE6BBA15E7D665DF95A4D60266A934C3E455030A85A46AFC528C2EF4DAEABF201C400462463A9FF94098D5184B5A4503998308B887C55903CC0D4D46B4B59B3CD73D606BC95E319F7F9365' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('D2D6807F650E74BEC61CA94943A5E0814943ED21EBD3C23383653D09854ED693BF1426EB3139A278C57576AE63C34E12679238B723713C1F4AB62DC9ABA142A7118846EEB47C2CC614C61497478F70B067B96147224830853DF70A23196ED586C54886C466C6622415064F242CD4AE253B1ACEBAAD6321B590A91095E16DD75E50C7321479AD68AC683C44061FE1755306E9DAF5FC539D926142A30D8A426CB47F2E46708088507C1B98CC75B711CE1A26CF61F54212D35C037BA533465EB7782888D5AF4D20AF2545ED7313B6DC082AF7C59CF48D1BD737D630384A3C8A713403D1088E968FC5983A750D81553B181E24B4C2A87A69AB6E32A6DFF03BCFC0DBCC9223E82670B78961542B8B6D02C906F38DC59AD29263629F98EDD5AE38CA50B16BCC56D8E65275E304ECBC84D45855B26CBC6CD95B5BA0929B5BE0B6B5F78A424149E09E834820C1500A04F2A762C481ED25B5F3158A7DDD8315C305CC26AA5460DE988CF7D4F0132FB86E5D14D6EBC2E1D275A6CE5A1B2EDF2C2ED505997584956D18C87EF57C056A91ACB255567123C4C4550E3424131F41EC23E026A1F91685661CE3434B43652611854A64367C2946625AC76FFF09AB957AACCDEC1F80AD4ACF1D7CA99426BF030F4C55C911C2A71A025B7D6F249B60D5898B051360F3DE126E8162B44235F8AF7AC04F03F7ACF13E25B660B0AB0B6E24B9E78C477121A974A6D5A376A61626DEA3B74DD370ABBCA7E9B6B250E43403B7B7746B946C6C592BBBF4022E42DC559FA7B76EB122E62D9C552F87CADA37633A4D52B4AE0A56B48D8C762589589EF08162B10D8A4236B67FFE5A2D4CDE0AE570F1940A955A615F2452180F3D702BAABC7D86BB635573B7E415D2931895772F2668BA73C5625674A563F28C55A28A72BAB1575819624532DC38FE5718EDEA68866459F7BAC9D4B655185776B9956EB15D743789DBD72A6EB38834CCE2D1C3C46D1B1485B86DFF5C4C1CA04CB36E3967117990C6E359ADE832725DA89791AF2DE079DAEB9C8279AF24BAC0333240EFD37FD361C532DBAB5ECC53A490E018C50A1942BC68211FC2CF96FF6B1158F9A5D224FF617A9D744FBCBAECCBF1AE5985691253B72EA60CCB299D9C5782C7706816040D248DB8A28211637CAE676A1500FFC48A5143603236AADFAFD8BECF332C71E5931D4938B2573E29DF1D5073F69D2E0E60CFF7279EE1C209FFCC7812325E086812325ED84F84BDC54A15F7730262FA2BF3A695CE3584CDB30D003B6A5E50CCC92F8F2781E7A5C59CFC1A79661AA52260CD544A55C0FAD299C3831B258E51CB42980343326DC7604C46484BD53DC2888098D6AA5B00931913043B021CD0E6CC48F9FD5BCEAAE22BCA6EE072981230CFBCB56A622C26BCD43DF9E218783A2A88A963FCC149278409B3E8EF9ADCDF75BFFEAE9BFACB8457' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('EEEFBAB9BF4C988AE5345C3B8C9FF4B97998E72DC6B770BF88FCEC46483236344F5E4926128ABD5A393BFADF9B70671BF5AE1F6A3E27D61D714E2CA6332A79E3AA5879BBD8B0F239A962E573C28665F9EEB68664A992CC887339B755C0CBB965432ECF6D15B73CB73D500D186DB18A4284454250C61A0A1B6EB1213734B8D88A7BB539E7C7B94CE4C73953A9E874DEAB48E9BCB3E1E4FDAC22E5FD63C3CAF9BA8A95F3351BD6253F56212FF9910DB9CC8F55DC323FF6402DF891085BF0231B6E851FABC8157E64C3BEB5FBA22E79AA0E7BC955ACE865BEAA639739AB1772C15B0DD00577B16257F8AB8E5EE13056FC757AEF5C45E7CD2E9D63D27681969AB85524FC33B26DD9B094D8395D0658D3244F570132CD9090BB05252FFA97FB1293DC058A792F3D587DEB4456AF037F653748A6A3493A5A9C0E659FF3C53ECE4640F648FD24EF1CA83827011924AC2779EFDD083E87C798E47FB8974CB0D23716BBFFC1BC23954465BF494D26E1F4B8914D5E115C4BF847468F92B4AE3AF1D74CBABC1AD6D956D5597976E626DB67473BEE2B7C25D9B84B1B367B145916D07FAC09100BA80BB6A41D42B566FB81B95AF3EA9BEF2C52295E73CA3A8B547833F6EEAB5CEBDB5726FD02CB59C9AA4B5889AD643ED698FFACA1A4BF31A1FC39AB81E09F9830ECFA1D8C36D3DD8BD27C4E9042F85756F983033504A8F467562CD4B3D5FC73AD6BE96F6CF35D1F1FC6FB2E1146BD2133D676E8C76419E25B8AAA48FAC65EA2D1616AD36BAA0996C74CA6AA142D18D329D15E41D6B5A347BBC127CC5B902843AA8D5F0B8BA40E8A820AD0D2B9C5C86E2D46C03BE2C52F3AF5362CAFE2C2D3DA14E7779DB2CDC60CAD72D70BC2C0C9EFB7AAC2CE4C6929299AAE39F935576CF8C5C55455D8E25A2A46FE2E22BFD5BE17E15E36EBA07ED153159870CF132BB7E3AAF7D0496FF973D2E86C954656F71E38E9757F4E1AA51D4243F6FED94E41563EB3E19B5EFCBD0A692AD61D1B4AE665AAC0645E26269CC2CBB4AE2EF8CCCBC484B57291F0C0394155B49584E487A22D74363C7C8B02D243D15C484FD8115895E94804239514CD04E2558D516EE2D2B98EECBB716D10718EA28366D8621C49B42B3A6897AFEF88CEC294D83C07F7559FF77D1F2F775546642E6EA63EE53ED20A52EE186542CA3DDB15A4DCADCD26B52E7CDA55D175E1D06693ADF8EE6687782F617A87B3D3E3624259AE867164A699C465B1AA5DC445B118D7119FEA8048519DAD2D8784767AC68C8A8F5F217DDAF9B3622FE02358C8724D7F6747ABD942051CA33D54E0556DA2028ECD2E2AD0AAB65181D6EB16B5C92E6EC198ECE25E76F18A9B5DDC80446917377E2DCC2E1E9C619CAF4A8E19BEB796E577B2C8AA4D2C2C3226B4BA455681AD5B644CF8278BAC027BB2C8D8FA5E5864D5BE1716195BD4976091' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('5580491619130D924556A141B2C806D0C016592B056C91B1DDC8812DB20A646A9131A1BCC6B87F6E09547933B30498907275BB8294EBD94C48B9255041CA2D01B6D57261095497CC8525C03693573E25F2DAE2AC7C22D153B4B603658AD63662F1B16079E64214B6DCA2C1965B4C76EB64B74E76EB78762B3234240719A1AA170C2D58D7064561B9B67F2EC67445E488D1D81E099869B3AB38AAA2B1E2E447EC094D4A4FD8338AFF086F01B5A222680F30F13EC05A352A3FD647C42C0EF6F5C005C18E0889E4640F343486F584463480AC098D08A75E7512E1B016C77C9D97D6DCBAD0BEB2B84D7305F8885B121483B8257F2E46DC625A75798B332458052E46C2FFAA23B1E61B1748A09A03588001B61C408C57371F3118ABFD28E3F31611B178CB46B34DE6E22D0674B7B061E08024833EA33709B7768C1B146E1174B94836020EB558237EDB28D3069584C2B4C8653AFA54FCA82F4D8CC4BA2AA755F4D657512EFB792CA40628DAB5D4F8B998E594EF6DA99394B0E7313A4F8BAFE40634B9171AD0C86880ED5287A24752434F996E5C035FA5B5939D11B8E0EEF3CF0C2E6765B97254FDBEE2703EFD3A09A21B12441BD3E422850838D42288F8AD20F98368E5D9F3D5283A7B8928FC517D47CF7E66BD63AAC0AAD92927384643651D06F085D0D5B5AE81879FE432F049725C5172A405F1622EC2830C452D3F9A3E6F5C6CD9078DC5FCD84FEA14882469902132FB2DBCE0C10B2C6F7F205D726B28DA83A259CADA60BFEC765ACAED18B7B7940D77FB7DF885466D50B44BB9F173410ECB8C1CC16729C977D21230BBE0323CC27ACDF0D8EB27DF87D57B6A15EB5E67BBA4765AF0ED18B7B7E06D5C949BCBD64D46A25DEE4D5F8BD1FD5392C937D2EAC46D99316FD199FFA42E3B4CF612BEE9470D2DEBDDAE0CA9A175324E2D645AF7A73636E2E29636E35E8BE3BD2D3E2D3794DBC928349CDEF4A5983D8DC36DC55CE4FBF03BA88817A0F7B8FA7C2AB84189F24677D23694579FA0F5F9182670979EB21C28A15A9028A454EBD76224950523CFF5AB29D4F81E5949658B03A01E243ECC4B1856E16C5B0579F14216CC0524D8050BC06C13209CFABE8E707A04FDA2C4F0DD6DF50E3C5B326D43956426B434D7CB23E1A5895E0A3B2208760434A0CD9991F26C3BB9C21879BE1D5B4C27C7AA32598EC5C665F7ABD9F1A57ADD1FFA71F3C076DB1FFAF3C74BB69D841534C3D4BF3E64BB89CE843987AE7FEF25CFC483DA7320A9F78ABDEA71547B6655C257E90FCC8EB1BA3290FBC558750225880F709BAC6B1BBAA25906906DD65D7D09133DDAD5E196004DC09C158D6721F999E7FB48606F02AF5AE6555111EF2E379AC224CF32D15EE136856D65168710498ED3EC18620FE92887C1CEC3CE892FAEFF91206E35BC65EBDA174965BA2C5A76FD2D2E9551819425' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('55C6673A99B0CAAD6C6E619FD67D22B78EE952EBCA187E6A1D43B6EBB2512B1761B43FFA6EBD9D0BDD5C6F542669805ABAF6EA23B85698C60EA3B83FEA28D257E6FD9470DF35DE4E59738901E9E6EC1E1767F3F6D44947C41524734EDA2086E85196824FC98CC992EAE6A8C99262B5A470EDA28126140982C276227F26C6685243F46BB10F5F02A93A16CBF946DC0BB3BEAF9F309937763444687F23B7146D1D00ED727DDA5AC2259867675CD6F63E40B7A28B3F008949055F8741F25CCBB5B0D9CEDCC3AD59AFFA0D6493712331DC973A8E213DB0E2BCCED0D1B48174634C1B08FB0612FA700D83E3D04DA40186662369FC54CC6682E9D59D53A6CE9E548291AA6DC1388CA58650DF6BB58680B66143E179EBB01C412447779B1846D586C92640D274BEC10ECB3E98D862A89F40CC51B1E5C07A125175E30420859FD45855B26CBCE0D85B5BA0929B5BE0B6B5F7CACB59DAA6DB179F454D066358DA4D00E216F86B599659CFAB38928C75885E0B685A946F6F515AF09F47640F0CAEA2D78843B1145BBE1515E8AAEE451660DB868C083E7A1597976182854274795D696EEDC8DDC1D4D33F6C6E1B7128E6B6E5DBC6B91D76A06070BA455D019B327A7F065BE84AAB106F3499810C53F358F582EF0317240D24C5DAA48311B34C49DB2FFBB69BB75E47DD4CC8854C816E28DAEB326B4BF7D3B9D153E858DE7F0D95D03490140C410723664F3ED336E11622E159AB4C7BBEBBCF0432405294B1A217C0E76A6BA8203DFCC9B8C7AC1D15064F55EF22FE19684B360F23FAE8DEDB11A0EE953933D28A50B810FFCC5CB7107D9416E5AC41A54539C7DFBBA694414A9437BA37BF693FE559744599E8E226CD1BF098447923866839DE2CBFAF25B7C5DC0C9B572F962BA500F2F2C5329384CA8B0457B1F2F2C06C587905E32A565EC2980D0B1FA4F26A159B24F98EF1C6C6CB72C7D5A6D59F32A467956EF9AEE296EFF866439DBB7BF709EE6A8873692D2DC1BC275AADB66B01C758DD15E11D0218D7790FE1191AB058B9EF84476A6006C8DA42F818467B42ADC139C0E94ACCC5064B7884469E317BDC19BC5C54C313E94F0CBCED8731BE058AA4ABC8AA6EE12BA0D87516D4B2CFF56AC99F99565D59B7AEB7ADAC57B3B62EBFA92195A7F5E59CDDD6900A54464EE471C7C5B5EBA38BBB0DB0B89C90C79D85FC6E525402C2CDE68AC67CB339C2A9BBD0110EABA71FD782AE37085782666D515AA3BAD6A4B44235639BF25BDB093B5F766D02DB2A41B221CBE7869109936314D4E44496D50D4C13D81B9349D222ECF54B03EAFA811DCF8E8EDBEF352DCE3637F21DA316378FBCBFAA59FF7353F9C296F48FEBE1A7DBF1E6E0876E2D34ED645BF2C650758931D8EDCCA18F5BF8D2840C54DCD8875ED8A932DC009CAAC4BD50675EB0ABA977F88E01459B4F77724FF63E25' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('CE4F65EF170F918A93B85E00A3EA2B27EAF92FA7BFE3E2076C9423099399F5E7EFACED3362EFB4C7F1C1DDC22C68B5F0A218C78FDD6F6E0CB357DEBF43C3F397B78311E2D7C8DDFAF097B51B601302BF276D91D919FF22FB1EC4CC5DBC8ADEF01E619CD8E17718FCF1FEE3C75F3EBD7F27F99E1BE3C027BEAFE5C7DE0FE2BF6FD39DC30D8230AB05FEC7FBE72439FCFDC38738A51DFFB2F7B65118878FC92FDB70FFC1DD851F3EFDFAF1B70F1F3F7E80BBFD87EAE7392C15CAAFFF51A0C4F1CE2F7B264A9E92C2AE43A3915AEB973377075FAA935DB092091F4B218A0F9519AF7EF87B551AA26F30F93FDE07C7FD37BCCF1811DC7A713A421F7F7DFF0EF32D7A8AFF857D2E4B18A431889DE126098C02EC3F816927DEBFD38EBE8F139FFE78FFE8FA31AC35A6AA1C203D393B7F9537E02F37DA3EBB119ABFEA68FF5D0976F0C71FEFFFAF774ABC09BC7F1EE1DFDFA11D16BEFBBFDFBF5BBB3F32EFF41FEF7FFB95B9111721CA5A3B4AD8FFDE855D5E7DAD538CA75749E0FE46A698697AF1677F7F578C90A37CFDDBBBF4B2C1BFBFFBF837EE736F83AFF6C049E7C65009EA5117F44CB2D2E3BFF509ED33A09F4803DAC8197C587F3647EAE062AECB37C0FBA8973DD95FF9EA9C06AA83EB3FF560505DCE123CDA98B3076EA1AFF2E5F942976E43FDC8DED842F9A41F039AC6E6DA6806BA4B6D7136809322CAB75D8562DABF6155F3814DFC20864E4D12B2305FE0091B53F838AA62D9634920448F5D5E972540DADAD3C8FDCA4F6A8B1259970D26ECDB8D1F3912B99F270E19B6356312DCFB47D84669FA475E01C3FA77C5CD0EF7AE7B57E939678B8DAAB28253894511A06F6A1BB4F09D2C387444C936AC2A81D3B0DC068DC5B477DFE0DEBDC75E5BDF77A4C30D980E69E4AECF1E251DD0A762F7180B51F887D5838BA48312A7DF3270D200D9B3B00DC904592EAC18E9A61B96B3767473DE4BBF930EFA215E2B3B21EB8ADCD815B9B1DCB4C73F67AB1E6CBBF9ECA0D58D17B774A0F6F6B0EF1AB8DE561A2B669B2962EB38BA4E085E1E45551D66BF6889AF5239893FA51704ECDB52DACA5359BB4686FAF43F4E23F11FFD46425A0B27614B5F71EA7CC3980F73D995F085F661252115F16BDBD21ECC92D2D711E622238168D9BA2DA9624965DCFB491491F24D277C75C2B4F9FAEC1F40CE0E0508E0D91CBDDB42FAED57660286F4702F3DF01E141AC3EB137B63D3D19897966F2F15DC5C3AE2D801A38FC2D19A7E8F342A4C461485B43098D66144F530193779505D002CAB339D1A7828D35D1459EBA19FCA5296DC42567BB86BA8B2A4C94075783658A88191B7973F57E5C022F82A871ECA59D6663D5B2BF690B9B2F6DF923167AB68327FD9724646E6D552143A7F363BB55B009F15D883194D596A8AD6C067BC3823A32180313260013397B758C4C465D043E7' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('4DD1BEE88A0CC44E5C4184FFCC15C83897A3CBE7DE1B9C3F5F9C064400639C8684136708D2D62DA0AAB9162C586A94080958E1257421CBBC84CF4946035916B71053DB49442CA920B00014EE9F1E8B7D6ECD17148C38446F467B2D5A4A1DE6748FA66745A74437BE970679F2C83ACC2AE4301FFA9040DAB9D1BA315283F33CFD29FC7ACDF02BE0E07AE8132775E6897F03B1D29628DC1B8A9DCDD656EEC2339C7EE9621700E7342A4A7142B5970D8FEF21967CDD21BE72038546F9D2B6B2CF74535B7F13D9D6BC4E41FFD4B9B4AD8EBCC269E87DB313A85496ACA1DD661B3B34BEEE26BFAF468CE7FB444098F7FEB3FD200A5A7020F8B544EA7A39D9BA95F31E8CAE23FBCD16E143289045A44616D8432DCE0247AC69721A09EE562D2EFC2760EA525811F3D61248E266A98889FBBD713FE4DB767FB706895E7F58672C4F626BDC936702CF78995453A6D3ABC9747A0B5E3C6C1CBC45471E6EF7E4CB9B7C79C27D79D10DF8F1FA9F7988449F791898E81F4D89FE55DF55F4AABD80D1DB48F48F7A24FAF7CC069E92F027D5546812FE4D24FA0F4BC5FF4913FDC59CB07E0B89FEEC3D1F2DD17F4AC33F035F390D3F7A6B69F8F5064F69F8AF350D3F9AD2F0A734FC9F3E8C32A5734FE9DC533A3725D60819CBF3BB4C8BE53FD00839F7EDEB220EDE883FFF5A48EC299BBE61746E35D53D7A8B11B2680A8F9151A7F018D7F0D894EAFE86A247E74C757368AABBF97A53DDA3D79EEA1EBDA154F7E80DA5BA47C353DD595A2AC4A69D92DDA7647751C9EE7DD4F329DB7DCA769FB2DD7F7637ED94303E258C4F59395DD331258C672AF65B74874D09E38DA893476CB8474C7E7683279832D7CA432C19BDDC8053EC0E1933BDDC48E9AD060EEA9953CACD169B5B634B33B5E9D291BE2DFE4DC8317A6D7919C1E12CFF150D69EF226F1ED918D8EC1248600E54209480AC6B08BFE37EC21E21F5695BF849B605DE5A0AD5452F5DD2E82A97C3C8E1FEE006B7B0D9D16469B32BE3CADA68DC961AA1A9D85C5B52B8687A780491643081D5791F560FA9ABD81DFE4B76716198FA1745933B46A14FC1FBA562D9C01CEA6054F565A733849B8A8A6F17DF26F10DAC557963CD3696A22D74A2A4A710C7C5603972A7841DE8A95E1B92F6409399D6175B44B841D6A9C44027F312914D20F16F6FEA1C14510D48B325B94355EC157749713B35A8DE2DC68B83FBE489F55CD124BAF618EAC1AADB49564CFEA5C99010E55F0A8DF0CE0B76B7B079F74E00290609758E7BD68F105D1A6790988A8153A64792C50CA35878F9C5CAB149E4F0100E71B20E03780B66B8054C45528921642ADE8E13CB49E543AFFC40CA26DAB60AAAA94A14FB0315FA0200568D9E1657C82DE2480E99B6A14A5D4638BB48B64D49B31431D8409B0BC1951413E764CAFCCF7767C0FC732BEE57B3CD033079' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('C31AA6FEF52153BD75EEE07320A9F78ABD429CA7C82AF7499C59FC0DD48D65EB6B4B44091645B30C20DBDDB6133BF412A0C99B8B405EEBBAA9822F8054986A70E10D64EBE12C58EEA24E68DEAEBC314DA0C9CCEE1CFA4D45C3068DAE7D91D48FDCD95B52655CED943B70B9E1A21AFD4964A3D16873C7C70D5FE8E67AA332C9295A4E592BDC871AA396F2C4DE4E3A69AADD0C8D02206563F0D1E1D76949626DFB32B22936CB83832FAFDEE4C99777B386F5319E1D6325780C6FC0B2069A0D4CA4C258A077D0EC345C11938DFD897DD59C1B4B63B40EC1B756BA4951E1AAC7567AA6C1AAF6539E52D384382D81B614845B0CC7D2D437067F8BFE025E4454D446DBCF898A90B0ABF259C041AC1590EF04C06616AD8861C863CF88C19962E6B4DB31C6464CCE3D1E7F0F669662778C460F5CDB94E6A9081132D6269084989C40968561E7C17E41D8867EA76873316915A6817D56488BB5F8DBB388AF91700273593294F229165E23A3580A967E1AB23EB3CC1341E77EC6B775381EC7BF03E65AE23EB5365081B1D235FEBBF2A2CB2CEF81694B5F0DE9019842D49E8DA62C1430B77459418C6862DB40881CC80F638A143539096119973349BB6356E5A891A90260BD1245734F28F7F11E21897BF21BBC61BFC1B573808EF1E763886DFDC8DBC2DBA8BB84AC88CFC6C098FFE774BCB4506C4D18DCD05E87E1F2F68D764BC9D0F11C65387B8EE5C84339E5624CB918CDC06F3C1783B7D1365A4A06E7921FE36666F06D3C2141832F81294FE375E469F0EEC494AF4193AF2160D447CADBE0DCF24AFE06EFCADCAF24C9627876C1E7C2749BCE0A9151273F01173F01624418959C0537E02918E82438F956449F0E480F26034B884299E9AAC014119B5275B43DDDAF4424FB62034F3799DDF494F19CC9E21D27E91E31DF1C2C144D886E670219285FF89BBED64A310CFEB054C7C2D9613F6F741BCC53F3C5D055852D0442351C4055050CF22BD1DF44068EC7C935EE99878B36B7646778DBB754B9F1A2DD93B63C69CB42B5E55809E203DC6246BC015D59E4455D0B5559AE441479CE81F126E0DC4B6781D287C3D6B30E87658F8B45750D3B9FD2EAFE8ECC5D479EE9FA9DB350255BC8F501B993DB9903599532D6E04D42DBAC9DD9B285E59B5C4474EA21B64BAC32015E1B7B31343353BF03260585DE638F19FB0CDF87AD0D05A9C99663B7A9204386F91EA44B5018FE175DDDAC8138FC7C7C1665D55CC80809A4908F91400AF928095BACF91809C3CF474818BE623969CE3C98B7D3181475C4049CD24D3ABDAE2F4941E6FD8AFC536DC98A0A3686AA4B1D5A449FEB6707183B675DEEED193CA5B64F46CF64F408327AE6AEE7BF98B8953FBFB1933AAEFBE40C537BC531B808AD7CA19B68ABD444B5BD042FA2F5A99B524494832E6DBD678385C5B06553321E5EB30753' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('E8A546D316737B5B4CE2DA1845D91FC22859BB8783173CDDC07E03345BB11F2CD074689C420F248F9DC8A8F44201EA5C58838977DC726870F70DAC3DE2BA68C71251907F0E16D246B5BF48EA86FBCEA55826F8BC514CD0CAF543424FFAC694411B930C6C3F1AF2992AACF926586E54C9045F0D7C90AE73F3EC3CD2482F04A1EB4B51E26DFD5BD0B4718E779EDFDDBF886F69C844A7E294DA2BE4B8459A222228E15DA42668480FCE1A4DA0904883649A8AB4ECC0FDD81716689D5712B067E9CBCA1C6DEA92EA2001820880AEA3B81C2808E8476F8F5A7945BEA9625ECD0D9FFC6993B1C3D5D809B7C73DDA35D76EE03E41FCAF1BD8EE456611CCFF7444B88EE6BA2C2033212DC093EE98FCD31EED8DA93986A9301652A0CA26B4DB2AB40C890BCD968ECD3DF9D150A8AE34EB3DC8437D73E9240D46C91B5364720FC61390E982341513356F8DFE2FE68AB93201217E758E99448AE56453C66EBAD2B3037FF99AE10AD8B84FE371626191037322226A844E04C40CD5A560E73A4038192DCD9A376816518F7C37F12963822EFCEA6F08855BA4E0256FCC0822367A3280260388A701045CCF5946E1F17003764F4FDFE6698844BB3585E86434B278B4DAF2E7A17C43F95DE7464F895D931C162587BFCA2B495B02C7C476E30DC8E28D6932ABB77461FCAFE2B01F80D46A160D59F36B5DB357A2C0914161F64B27A62BCC233D8884179AE124C85899A42D17B9B88010FF750312B1B9262185B1980F93680DB5477D432ABF1E10839BD76953740D51E01E10CFFC280290D792BDBA38FCC1AB3A852C0B71CBBECA3BAC8A15F1A67C3EE4464FB6C64FB6FB5DD9D6405C86B8E91622DD4838E342944292C4E81C2B3F8B2CC50CF3E66429A1D1932C9D64295759EAF950B26D495EDD82385554B0DCF4BD65F03C54F415E8FB5C7CA3D06405F70436A4928F86D791690C8CDABC6C75FF0C2BF38A28208BE2549DF69B1F7E634FE391D78A361465B0D811230D5F8930A3163B6BD7F34DB885DE5F884D7E7EC103CCA63BA728E44E79AC443A475023D7C05EE93DC523B199020E6099405E4B8ADA7C3955DFB6FEC6BFAD7247530796FE9BB4B51BD3D6D630384AB7522CAEFFA9D06C88F09F22AF82EEEFEDCEDA27873BA1EDB374B32317BA07A8E813930CFE036687B709241BCC379688ACCD1C1CBBBD6DE5CCB77D248A2A5936962A621A5AA0B3B794494A2909DCDF808CB2159B78C492460741A3647B89DF200338DD6746BD9E7A1894FD0520EEBB68F1B731490710281BB789FCDE46FEAB90A3B810B8A97717D5EE9319200BE12605355956B99F3E35241317C6E873A95FF1E989137EA5F4B54EBBD1ABDA8DE2D87DBA850C0C6B33FB07E82AFEDF4758DD81878E0BB97AAC7704FA49C096A4D9A0146ED9565C7C749B7AE701D01EDB9106EEDB8ED4516E1A68D7988B2B' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('B6C2E1AA034DB7958522A7C7727A09DD4B0066D14BB7399EC5E390BEAEF5797AE5FB509CBC39B38E427BECDE8DA2814391A965AD1626DEA3B775270F45179797064A447D19714A7FB770642F51954AB50E45B59FA798E2EE9E3E1BA225AF90EA26A0B88E6289340AA48DBDC27A1CEFF9C3B8866459F7BAD9A121B263AF74AB43A361C79CE4BF20F9AF1F52D19AA6ACDC80FCCF727C409AE1D3DF19521E3491F1BD726B99130CAEBEEF887698F008ECBF95689CC0601AFF21109683505E77AA17DF9AC0EA659D5D0288B1CE28A4EAC0A8BCBE3624ADE9A27446898D3947A4D4CE1B2B46604F427512AA9C85AA6139E8C7082D225C8D4E091EC35B48F05ACFD45EF234FD4E487C815C7F8B4EC427D9BC2108FA20183B67AFF85708A3AF0C38EC9E3D3167F184404BC8B81402FCCAAAB1F54988BE9F8B99CD8C530010839E571C762459D637822AD5952A038B2433078664DA8ED199A9D4F39AF405308580036D2E0437BF15DCE9C8D1EF739BE48927F963A7219B07279D4AFEE8C598AC058E097FECF298F047572C67ADEBA60ABE005590EA5EC7E7B48F4E77857670E54A32917417DD8DD946BD134E249F6BEB4EEC5C0BC3CFE75A18BEA54AB238F44B4E1246A6CC496289A06D40C6DAA0302A85AE23BA37F9BA98CB22F94A187A3E3AC2F0F355270CFF725D0823535E17628914EB421895CABA104667BA279B9D7F05122A73B06832050F0BA453E162819470650DABCB16FAF86B1F9F48EA6B116066219362A6EB77CE42950415063FC10B290B2E29E6BDF46009C1CEEBF6775DE0D11BB8E142002A67EDBAF0D62E9E9239FAACC363DB63CA0455E09D03152767211B59EFBC64847D64EF2513ACF48D45E383634737EF06DF8F21738058515D4BC2BE94D6A2EC415517B18A66928D8762D3E167E975518205D485200F8BFD4071D550AFB08AB348773801C3F1B54DF31B321878FB9144954855B0A9F5A738F03F67E2B0EDA5B8D83CB13415AF340E1C62164A000DCE6AFE598CAE82D9B1C7C0D3428B6BB5BEB19768D005B5BD770536DDB040960D306EE57C41BACBE0E26EA5F1988AF20BC81B9ACA8514C9386B2765AEDBC8C1E99BF162ADD120094E77116AA0526532B0E34EB1D751731644A5155CC97FC19A03659A40931FBAA6AC877D684A4B49D174CDC9EF44E74FA2B8B99C3FF229DDA70399C7E5DFDC696477F401E78BA46E8093A6E4F01FA14B22B2F2993F0953B1EEF8A3665EE2AE311FE025E62FFC561212518AB6D045341B6F08C8BE405389F85DEB94DD7D48E083050E62178B3FAFA3FDDD59981268D17F06052FEE0507DDD682E0F3688B28F83CDC260AFE32D6268A8A612A32287627EE09A3B2CC5DA150BA37D15E1260807F01ABF0635FCB6739B3B5E508BDDE099F8DD737B6F3678F333A54439EE30BB0960BE861167381C2DF77C1C38B93' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('72DDE4C2995C38025D38AB5B72E1BCB26354224F144DD9FE2D5E80CE6B0A867B01B893387901B8239FBC005D39453CBC00BC6990BC00DC47A8EE05E04E22F50270477D83B962B9C5D8E7BA0F068B51147C6E318A82BFB418455121073B5EDB79DC379940232649694ACB99D2726E222D67880F6735BA0F4758EE60E1B6584C7E9BD7E8B7594D7E9B46D4C96F33DC6F83CC63C991BC48F5825B283D2E29A690525D08575534EE2191B4B9695D2DDEC868F733F10E28E0FE9AA2A285207824AE0521A3D11E7AD4014194AF07683292D99D0663DF56CC714799843F3F316D84D12DD47FC4C965FDCB1E9607EBEC6CE77F730F6E65F7C1A61EEED91C1794D2E33989380CDDED8BE8534A6AA3D96653A9CA61A3012419D00DF55BB8067E12ACAF50B046D0BD01A98A4BEC0D95AA68A4A845EAC71E5C8D9A28A448DFB418DFCA6294C36390442F37B01EF34D93ECC4A75991C558693AEA257571E7DF7A6EEE8E3CB49D72FF76320D28E07F276B3155127773F6ABB476BA0FFF3107B357CA72E5A8FA3D6758A139849318E5284637A6790B321475B3E94819AD528331A8959A1E5209379146A9E99383E450198A7DAEE4D1C0035539E3C9E49AC4530FF194AECDB88F84CA8A901707545965D5E5D7F44346B9D43370215E21457B50344B591B2A1062225D77614EEBF255AC4BC3DD7EBF8D5B880D49BE939640483830C716E1B154AC7B5D17750FCE24022611E0D818A6D7CEFCE62400EEED4CC42ACD741C013751A4C0425B2CE3B4E38EE5FFA9472E76D1702EF8F41CED6D9363740BFB99086EEB9D2389867DDCFCC88BA0E6A0767E129D616702CBE2BEBBD21410EB61130C4E61C4033CA52FFEBCAA051961B4F4C5CFC7100BCFC8DBDE8288B780A9486ADF70513E560E4B0CA687ED6301DB56415EC39CBB65B500622C3684CBAA55D19E32306D439564EE7B739A66A988C106DA5C086E9E264B0C24F200EE3A0EC50E7CBF9A6D1E80C95DDB36F5AF0FD9E6AA9B6DF26490312FA9F78ABD1254AE656649821A9E7B5145E4A9299A6500D9BED095F8367E09D08CCE0512A0BCF0ACC7842A2A5A43CB8DA6F017DB69E1A09601693A624A1D60C367F1456D0CB2AE619D46D7BE48EA47619D905419D742F8C87B5ACB1DE09E169137FA93C846A351FF2472D417BAB9DEA84C891DB49CB35634512DC7E8D25751E8A9DA32F43004D22286428874484B1BC45F6223C6BDFD2785AE3E729DB081C67ED1EAC9E09F0C7E4106BF891BF8F35BFAAA8E77A65CAFE13DE527702125BE7513208D4054DB4BF0225AFF00A456736C88B05AEB9ADD566665E00512B259DA6B79EB0386F42012FEEDE6840DDE33B1409B364C32EAB46172D830431FAE6170BC814DD3D41B33E36816623E50D4BEF11EF981B885C499EFD3BE4FFC7DF76BA06D7A15B6AC35EE3752E3F8DDA885E679A602' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('516950B209D06637DFE0780B7769998363CBB85C2AA08F205125CBC6C2444C430B74F696320B27698BF9EA4644D45001900D96C810DE6031506922515271536964AC36F26CAE50D935C99691648B05D1FC05DB9BA8476D81F6BD7008C31A2658285FFB892C0B7CDE004D0626B5D2D2A95451CFBF1DB93B88FFBE81F9EF7F00EE344A695608C34642B3CA7F92632A2CC35838ECC5DAF093B93D7C7FD8C430D20FE90A34422F48542FF87E03A2A2713FA7E072D290F5D443E9934F816E289ADD4BBF6B6D2F47A5949AE7EEDD083E87C7183AB21B3D858EE5FDD72D6C4FF79209563AE23CC7043240B2DBE9359D2418E6CB34E88A0FA7C51ED86ED2A0DBA8D78E0AB4656B1C605051F7B573AFCC45C2AF2E6B8373C74F6BF60B82EFBDBDDFAF1CF9C99AAE042F8DC514B2F87975A82B872CCEBB246221E81D6EA1E2E5796F13B0EB08DBCECEAD1652F931BBBE456ECBA4E5707F8B30FCFC021761F8F894B9C2BFF4CBE5C530C25A3FDBA877C289CCA5B5B404F376028392EC7302E59B19B877C1D080D5B10E06F621A320B0130067D95E56EDE67DDAE14441603F14CB598ABA264856750B5F2A2D48F1434DFF2CEA3298B26526A4F1F94581E936535E089CD441E27D8E9C6E667C1B37602DD45410E3780CB2B3CF86411F6531C7E27F3715D0E60EAE04CDFF54903334511D418828FC8F2FA619DAB4F48A1C016DC3F711496B9AE5D847566587C08069027B638ADA34109DF583500AB6B991EF04DCC93A37952F5DC7127B5DD19629341B43D52551E935980A5071FB1F44D3492D1FD144668A362FEBE19C2F561FE0B432E1F6904C6EAB8BD1981C5793E38A8FE34A8AE370EBA50C576C89E10E2A09DC3B333786F88F8AE70A04BB7738E90859BFC50B79CB2CE83FFE72FE717DF413EFE07B5B4436151F1F9A800A921740E71F2F81FE5B0DC8848F30C23E2BD797C3204E22D70B92BA77CD0BB6DEC1F5AB8DAFBC48E988C3237A82AC3E99C3030CB017ADDA3F1A5A33C94ACB4390699EA02B6EC1AE31F8FD4369A6DB196036475BDB62AECB8EAA58B673FAB3990F4E6F5C30C2F9577A4EB8A44DC6CB1F89E10A7267C5B045A5473414D1CBD7E38B7DECCC3CDF77A483334F7C472FFDD0CC1BA5772E66B3FCFBE554FEFACB2FAD2C72D98C26D4EC99182669EAB72036A9748A4A88ACB14AA7AA8E6438AF8061A22AC354131A49D31B354C6DD49B61A2168689466198685C86897A308C793D86C179CCA9EA61B818B191478AF72EA6F2FC231377149F610C2A3C4E3C71821F81212EFA4843CF904C90E52B5D850FB430F11EBD6DFA93B38671EC3ED58AB99E67B0FCF6C50C5E3EA05742728A156EC87F13C20C172D1D8521B2DED090D2745B5928B2549C50B80A475C66C0C5C8022EFDD0C819172F9567F3F2013D67549BD1089A3D14C22BCD3D17C32BB56ED1D0CC' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('D31E5347C7F598C6B09CB593FA49F035E40EB280A3639CEC11BA123C8651B348297D7939C5E5DF99B61912FD1A74FD05311C54EAC5180C44EA190DDD3CA27535DE59957887385EC23966D580B82223BE5DFE583190BB265B14A7ED1CB2FAD657476DE383D3E1F732D0F9C737AF9C920FF737CD7D7EAAF3AA939F1DB5741A0FFD5E3241FE4A950D8A9FD9198100587D248C2948FD15CB160C14CFA7685F0B7B8C2B21AEC918634B0B06B6B8AACCA8E5013B5214511CA139CF6E3D93B83CC184A7F48C436C0719BDFC821026EA489816C34DDD13D140977C1A68040ECBA278E89B047D01A352CC6EE14571327713F79B1BD7B9097F65C1A4F0F69D8274E7A8602D7E676D9FE1DE4DEB52EB1BDB06693C268B2F16312A8BC05C9794CE51BC1AA5F3A3164A6924CC06EB6E4AA57853BD53E7676DBD2ADE62209607A39A29E62FD0904D5FA5A05D0E9DD409979FB6513D47089848666EF236B2D91B74A49DB9ADB2908F5A494794644D26929D3D8E187A6CD2F5587E76832798DA442B2F4EC2E885B480082FB52DA595A42D816ECE81B9428CA69B0AD5FADD1FDC8048BD78D2BA7AD786A4D1D141126D9BC44442C5A3564A9A8DF43F8B8A9411DE79C18E48AA78D44ACAD0EF146D4E43294ED66100C9C3777AD64ACBB2D36B6C29881DE3D931C6DE1512B5D2C336721B6BB6B1146DA153D12BDD5C425E238477DAA97FDEE83698A795D6E9D6499A3405A3F21D2AC466D4DF6A6F489AB7596A0D6D53D018C707986BDB0D0DB978A7B319565EEF1F9936DD8D98BB9EFF92D697AD132F3D6B213A971435AD9349432C71D3AF94FD218C92B57B3878C1139132F9C5D666D8E8BF990A9435CE965E4B86A1684B8A3641D797A2C4DBFAC421283F6DA30E2435BF72828264B83D62C7E6DA0DDC2788FF45A24C78A9AD01BABC41760BEAB7262D01FE57773B80EB39CB283C92B482D2B316AA40529C25FA954223005FB3EDC4C94AAAD6095E3E6F235A7EB39BF002660571EA244F4F5A882D00C057725091D9041E69264F4FDAC9E02B2828C8783E946CB473AD48944A0FDB88292AC85EEBA6B74632C0C4F6D55F90A44A5D3E6EA1B9465222B57CBE000A65AAEC81A8D32C3D6CA388AC7889520A9E9D28446A9D4647EA31A0323A4E616102A1FC492B1DCB42CBBB9BCC6524BB46EBF2710BC1722CB79BEA6594B446F5F2710BD57230908D6A16466DA59CBD42499DCEC02287E2EAAD20BED6DA922C991F6DEB58A263358B62F15C4490888D383DEDA08DDEC3CA3F1DC9552BC91525C9152DC9F46A55C98B542F20ADA6CBC72D44B317B39BB4E8A91A482B69A19A3DA6A08A55155AAA11741B49A6CF3AE99940A224268747C4A82433E4F27117C9F43E3C2ABB317B7D639A8D34D3679D04D15BB4C432DDBA915EF1B89B64AA75D3F6B1B86CBE81ECE97117D9FCBA754AAAF9FD' as long raw)));
dbms_lob.append(model_blob, to_blob(cast('D60D448BA75D3453059BAAA7F90DC4047AC593365A8A6C6F4C0A32ED561CADF5C664B535D84A9D66129DB27A0ED3D6299C1EB551D155ACFC6FE82935AA59D51728A8D2AA5BE72AA575B2A5672D144F3536BB899D4B62D689959EB510B34D694E690510EBEBD5E9925F6B69022E6077A9976877DDAD2147636AAD21BFD6D29A520514C95CEA8EA5FC49313684C8534B534EEF50B5230FAAD41B518A8AD4630CE7134BEF4AEF55A20D0DC79A2EE262D5F0082255FEB116E6219D642A7D79FEB1D29B0F97DDA1E86AE3D91C428FE9CEF15C76BC1A43C97A7EFEB5A5EBE490C82540FE68F830B41D45210D05F5D195CB2ED5232D597FCABFB70D09396C52C5C89E711C14C2718BD641E93A9ED1D0A9A8A14311E5A0442D8312711A94EA9102C238B49E3A68C8D828B5F9FC634BA72F4F19507CDDA3ABE4AC79428729D2EB2F1A4FF23EA48DBF7CD0DAFD0B3749DEF3FCB7C11D6F4F0E270C004336F94537480E91B42F970F5A06A2C9B95143C91E0E1F9ACE1468D2F0B0E54D5F76B0EEADC8FA56FEBD6D805A7C2F27A0FA0B5C068A94EFDB303C9DA9C1DC0765D5F0FD8AFC7D8F21A8E7B6123ADF9100CB495C56EDA6F4CBF38FDCBA5ACFE46CE97247DA67ADEB97A6D8A9F3C5CF14DD2720541F89188AAEB96F4D70E4CC01630C017D8E1E61507A26F875A5F895FA4B78DA32706D466205AEFC02F5201685244E2968A767BF7FC86CBBFC07F467124668875F23F3C78FD35F7FFF601ED1D7FBAC04C5EF73187B4F6788DF116690C5C5CFA0C53B58D0177978951615AF548B96C1C4DDB9898B03BE8FEE36418FB748E74823CF5F5CFF885E01FB6F70A704FA31391C13D465B8FFE6BF94070367F0B5D1FFFD43ADCDBFEB07FC57CCA30BA8991EAE18A207B3A3E7EF4EED5E10EAB7374060E333AFF18AE732C11AC6D3CB09490B034AA07CF84E198D36DC1F7C0416EB81E5FE05FBB46D1343153EB9DB17F4FB5F5EBA8B3581744FC4E5B0FF3EF7DCA7C8DDC739C6F97BF427E2E1DDFEC7FFFCFF01C1D40DE197030600' as long raw)));
insert into "AIROUTE"."__MigrationHistory"("MigrationId", "ContextKey", "Model", "ProductVersion")
values ('201811300959101_Init20181130', 'AirOut.Web.Migrations.Configuration', model_blob, '6.1.3-40302');
end;
