select t.Id,0 as IsAr,IsMBLJS,Dzbh,Line_No,Bill_Type,Bill_Account2,Bill_Amount,Bill_TaxRate,
Bill_HasTax,Bill_TaxAmount,Bill_AmountTaxTotal,Money_Code,Bill_Object_Id,Payway,Remark,
to_char(Bill_Date,'yyyymmdd hhmiss') Bill_Date,to_char(Sumbmit_Date,'yyyymmdd hhmiss') Sumbmit_Date,to_char(SignIn_Date,'yyyymmdd hhmiss') SignIn_Date,to_char(Invoice_Date,'yyyymmdd hhmiss') Invoice_Date,to_char(SellAccount_Date,'yyyymmdd hhmiss') SellAccount_Date,Cancel_Status,
to_char(AuditDate,'yyyymmdd hhmiss') AuditDate,sumbmit_no,invoice_no,f.ecc_code
from Bms_Bill_Aps t 
left join FeeTypes f on t.invoice_feetype = f.feecode
where (t.addts >= trunc(sysdate-1) and t.addts <trunc(sysdate)) or
(t.editts >= trunc(sysdate-1) and t.editts <trunc(sysdate))
union all
select t.Id,1 as IsAr,0 IsMBLJS,Dzbh,Line_No,Bill_Type,Bill_Account2,Bill_Amount,Bill_TaxRate,
Bill_HasTax,Bill_TaxAmount,Bill_AmountTaxTotal,Money_Code,Bill_Object_Id,Payway,Remark,
to_char(Bill_Date,'yyyymmdd hhmiss') Bill_Date,to_char(Sumbmit_Date,'yyyymmdd hhmiss') Sumbmit_Date,null,to_char(Invoice_Date,'yyyymmdd hhmiss') Invoice_Date,to_char(SellAccount_Date,'yyyymmdd hhmiss') SellAccount_Date,Cancel_Status,
to_char(AuditDate,'yyyymmdd hhmiss') AuditDate,sumbmit_no,invoice_no,f.ecc_code
from Bms_Bill_Ars t
left join FeeTypes f on t.invoice_feetype = f.feecode 
where (t.addts >= trunc(sysdate-1) and t.addts <trunc(sysdate)) or
(t.editts >= trunc(sysdate-1) and t.editts <trunc(sysdate))