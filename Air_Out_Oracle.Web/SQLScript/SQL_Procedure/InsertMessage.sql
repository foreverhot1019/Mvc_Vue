create or replace procedure InsertMessage(
    V_ID               NUMBER    :=0,
    V_SUBJECT          NVARCHAR2 :='',
    V_KEY1             NVARCHAR2 :='',
    V_KEY2             NVARCHAR2 :='',
    V_CONTENT          NVARCHAR2 :='',
    V_TYPE             NVARCHAR2 :='',
    V_NEWDATE          date      :=SYSDATE,
    V_ISSENDED         NVARCHAR2 :='',
    V_SENDDATE         date      :=SYSDATE,
    V_NOTIFICATIONName NVARCHAR2 :='',
    V_CREATEDDATE      date      :=SYSDATE,
    V_MODIFIEDDATE     date      :=NULL,
    V_CREATEDBY        NVARCHAR2 :='',
    V_MODIFIEDBY       NVARCHAR2 :='',
    V_OutID            out  NUMBER
)
as
  V_NOTIFICATIONID NUMBER :=0;
begin
  select x.ID into V_NOTIFICATIONID from NOTIFICATIONS x where x.NAME = V_NOTIFICATIONName;
  
  if V_NOTIFICATIONID>0 then
    
  begin
    
  insert into MESSAGES t (
        t.ID ,
        t.SUBJECT ,
        t.KEY1 ,
        t.KEY2 ,
        t.CONTENT ,
        t.TYPE ,
        t.NEWDATE ,
        t.ISSENDED ,
        t.SENDDATE ,
        t.NOTIFICATIONID ,
        t.CREATEDDATE ,
        t.MODIFIEDDATE ,
        t.CREATEDBY,
        t.MODIFIEDBY
    ) VALUES (
        V_ID ,
        V_SUBJECT ,
        V_KEY1 ,
        V_KEY2 ,
        V_CONTENT ,
        V_TYPE ,
        V_NEWDATE ,
        V_ISSENDED ,
        V_SENDDATE ,
        V_NOTIFICATIONID,
        V_CREATEDDATE ,
        V_MODIFIEDDATE ,
        V_CREATEDBY ,
        V_MODIFIEDBY
    )

    returning t.ID into V_OutID;
     
    end;
  
  end if;

end InsertMessage;
