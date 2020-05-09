create or replace procedure InsertOPS_M_Order(
  V_ID                 NUMBER :=0,

  V_MBL   NVARCHAR2 :='',

  V_ADDID              NVARCHAR2 :='',
  V_ADDWHO             NVARCHAR2 :='',
  V_ADDTS              DATE :=sysdate,
  V_EDITID             NVARCHAR2 :='',
  V_EDITWHO            NVARCHAR2 :='',
  V_EDITTS             DATE :=sysdate,
  V_OPERATINGPOINT     NUMBER :=0,
  V_OutID              out  NUMBER
  )

  as
  begin
    insert into B2BRETS t (
      t.ID,

      t.MBL,

      t.ADDID,
      t.ADDWHO,
      t.ADDTS,
      t.EDITID,
      t.EDITWHO,
      t.EDITTS,
      t.OPERATINGPOINT
    ) VALUES (
      V_ID,
      
      V_MBL,
      
      V_ADDID,
      V_ADDWHO,
      V_ADDTS,
      V_EDITID,
      V_EDITWHO,
      V_EDITTS,
      V_OPERATINGPOINT
    )

     returning t.ID into V_OutID;
    end InsertOPS_M_Order;
