select t.Id,Operation_Id,Is_TG,Consignee_Code,Consign_Code,Entrustment_Name,MBL,
(select to_char(wm_concat(to_char(o.hbl))) HBL from OPS_H_ORDERS o where t.mblid = o.mblid) HBL,
End_Port,Airways_Code,Book_Flat_Code,Flight_No,
to_char(Flight_Date_Want,'yyyymmdd hhmiss') Flight_Date_Want,to_char(ADDTS,'yyyymmdd hhmiss') ADDTS,ADDWHO,FWD_Code,Pieces_TS,Weight_TS,Volume_TS,
Pieces_SK,Weight_SK,Volume_SK,Pieces_Fact,Weight_Fact,Volume_Fact 
from ops_entrustmentinfors t 
where (t.addts >= trunc(sysdate-1) and t.addts <trunc(sysdate)) or
( t.editts >= trunc(sysdate-1) and t.editts <trunc(sysdate))