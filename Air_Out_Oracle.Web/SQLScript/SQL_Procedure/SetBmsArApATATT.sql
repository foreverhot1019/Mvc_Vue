create or replace procedure SetBmsArApATATT(
V_IsAr in Number :=0, 
V_Dzbh in nvarchar2 :='',
V_LineNo in number :=0
)is
  V_BillAccount2 NUMBER :=0;
  V_Bill_HasTax NUMBER :=0;
  V_Bill_TaxRate NUMBER :=0;
  V_Bill_Amount NUMBER :=0;
  V_Bill_TaxAmount NUMBER :=0;
  V_Bill_AmountTaxTotal NUMBER :=0;
begin
  dbms_output.put_line(V_IsAr);
  dbms_output.put_line(V_Dzbh);
  dbms_output.put_line(V_LineNo);
  if(V_IsAr>=0 and V_Dzbh is not null and V_LineNo >0) then
    begin
      /*设置价，税，价税合计*/
      if(V_IsAr=0) then /*应付*/
        begin
		  begin
             select sum(t.ACCOUNT2) into V_BillAccount2 from bms_bill_ap_Dtls t where t.dzbh=V_Dzbh and t.Line_No=V_LineNo;
             EXCEPTION
             WHEN NO_DATA_FOUND THEN
                dbms_output.put_line('应付明细，无数据'); 
			    V_BillAccount2 :=null;/*0;*/
          end;
          dbms_output.put_line(V_BillAccount2);
          if(V_BillAccount2 is null or V_BillAccount2<=0) then
             V_BillAccount2 :=0;
		     /*删除 没有明细的头数据, 总单应付行分摊的数据*/
			 delete from bms_bill_ap_Dtls t where  t.bms_bill_ap_id in (select n.id from bms_bill_aps n where n.ftparentid in (select t.id from bms_bill_aps t where t.dzbh=V_Dzbh and t.Line_No=V_LineNo and t.IsMBLJs=1));
			 delete from bms_bill_aps t where t.id in (select n.id from bms_bill_aps n where n.ftparentid in (select t.id from bms_bill_aps t where t.dzbh=V_Dzbh and t.Line_No=V_LineNo and t.IsMBLJs=1));
			 commit;
		     /*删除 没有明细的头数据*/
			 delete from bms_bill_aps t where t.dzbh=V_Dzbh and t.Line_No=V_LineNo;
			 commit;
          end if;
		  begin
             select Bill_HasTax,Bill_TaxRate into V_Bill_HasTax,V_Bill_TaxRate from bms_bill_aps t where t.dzbh=V_Dzbh and t.Line_No=V_LineNo;
             EXCEPTION
             when NO_DATA_FOUND then
                dbms_output.put_line('应付，无数据'); 
		        V_BillAccount2 :=null;
          end;
		  if(V_BillAccount2 is not null) then
             if(V_Bill_HasTax>0) then
                V_Bill_Amount:= (V_BillAccount2/(1+V_Bill_TaxRate));
                V_Bill_TaxAmount:= V_BillAccount2-V_Bill_Amount;
                V_Bill_AmountTaxTotal:=V_BillAccount2;
             else
                V_Bill_Amount:= V_BillAccount2;
                V_Bill_TaxAmount:= V_BillAccount2*V_Bill_TaxRate;
                V_Bill_AmountTaxTotal:=V_BillAccount2+V_Bill_TaxAmount;
             end if;
             update bms_bill_aps t set Bill_Account2 = V_BillAccount2,
             Bill_Amount= V_Bill_Amount,
             Bill_TaxAmount= V_Bill_TaxAmount,
             Bill_AmountTaxTotal= V_Bill_AmountTaxTotal
             where dzbh=V_Dzbh and Line_No=V_LineNo;
             commit;
		  end if;
        end;
      end if;
      if(V_IsAr>0) then
        begin
		   begin
             select sum(t.ACCOUNT2) into V_BillAccount2 from bms_bill_ar_Dtls t where t.dzbh=V_Dzbh and t.Line_No=V_LineNo;
             EXCEPTION
             WHEN NO_DATA_FOUND THEN
                dbms_output.put_line('应收明细，无数据'); 
			    V_BillAccount2 :=null;/*0;*/
          end;
          dbms_output.put_line(V_BillAccount2);
          if(V_BillAccount2 is null or V_BillAccount2<=0) then 
               V_BillAccount2 :=0;
		       /*删除 没有明细的头数据*/
			   delete from bms_bill_ars t where t.dzbh=V_Dzbh and t.Line_No=V_LineNo;
			   commit;
          end if;
		  begin
             select Bill_HasTax,Bill_TaxRate into V_Bill_HasTax,V_Bill_TaxRate from bms_bill_ars t where t.dzbh=V_Dzbh and t.Line_No=V_LineNo;
             EXCEPTION
             when NO_DATA_FOUND then
                dbms_output.put_line('应收，无数据'); 
		        V_BillAccount2 :=null;
          end;
		  if(V_BillAccount2 is not null) then 
             if(V_Bill_HasTax>0) then
                  V_Bill_Amount := (V_BillAccount2/(1+V_Bill_TaxRate));
                  V_Bill_TaxAmount := V_BillAccount2-V_Bill_Amount;
                  V_Bill_AmountTaxTotal :=V_BillAccount2;
             else 
                  V_Bill_Amount := V_BillAccount2;
                  V_Bill_TaxAmount := V_BillAccount2*V_Bill_TaxRate;
                  V_Bill_AmountTaxTotal :=V_BillAccount2+V_Bill_TaxAmount;
             end if;
             update bms_bill_ars t set Bill_Account2 = V_BillAccount2,
             Bill_Amount= V_Bill_Amount,
             Bill_TaxAmount= V_Bill_TaxAmount,
             Bill_AmountTaxTotal= V_Bill_AmountTaxTotal
             where t.dzbh=V_Dzbh and t.Line_No=V_LineNo;
             commit;
		  end if;
        end;
      end if;
    end;
  end if;
end SetBmsArApATATT;
