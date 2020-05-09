select Id,0 Bms_Bill_Ar_Id,Bms_Bill_Ap_Id,Dzbh,Line_No,Line_Id,Charge_Code,
Charge_Desc,Unitprice2,Qty,Account2,Money_Code,Summary
from Bms_Bill_Ap_Dtls t where (t.addts >= trunc(sysdate-1) and t.addts <trunc(sysdate)) or
( t.editts >= trunc(sysdate-1) and t.editts <trunc(sysdate))
union all
select Id,Bms_Bill_Ar_Id,0 Bms_Bill_Ap_Id,Dzbh,Line_No,Line_Id,Charge_Code,
Charge_Desc,Unitprice2,Qty,Account2,Money_Code,Summary
from Bms_Bill_Ar_Dtls t where (t.addts >= trunc(sysdate-1) and t.addts <trunc(sysdate)) or
( t.editts >= trunc(sysdate-1) and t.editts <trunc(sysdate))