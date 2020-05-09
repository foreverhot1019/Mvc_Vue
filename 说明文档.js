/*
测试库：172.20.60.181:8004
正式库：172.20.70
*/

/*
建行 测试环境是194.9.253.199，生产环境是194.9.253.197
建行 网关是15.84.245.26，你们那端配15.84.245.25
*/

/*status === 200 错误返回一般都是 json格式，除了500错误会是html（一般不会出现）
  除了200 其他都是错误 如：400-badRequest,500-Server Error
  200 可能也会报错：正常的数据验证错误，按照文档规范返回*/
//获取Token
fetch("/Token", {
  method: "POST",
  headers: {
  	"Content-Type": "application/x-www-form-urlencoded",//必须是这个
  },
  body: 'client_id=Michael'+//必须是某个字段
        '&client_secret=Michael'+//必须是某个字段
        '&UserName=Michael'+//可自定义
        '&grant_type=client_credentials'+//必须是password
        '&response_type=token'//必须是token
}).then(function(res) {
  //res.status === 200 
  //除了200 其他都是错误 如：400-badRequest,500-Server Error
  //200 可能也会报错：
  console.log(res);//返回格式是JSON
  return res.json();
  /*.expires: "Mon, 15 Apr 2019 08:06:06 GMT"//UTC时间
	.issued: "Mon, 15 Apr 2019 08:01:06 GMT"
	.refresh: "True"
	access_token: "9dMneKVrjArB44fopQeFPHv6dSzEofWICrVd-u-zvQO95tvE8hiW_QBk2-DQh0n6R9gFMy2wfKkuCXCb595g8rFFFh4WXKsJYaTLVAQl8EqEeI4KyS317cVkMvX5wdXCYhtzhw_rD4WpLsfVdUPmxrp4IluX5cjizj3VaBUK4iaCNL7ph446wF7Faay_GVNAAcnCIO_ch-Jh9Dv8bFq2ApAqwrKfKKOQ6B5XHaptVu8"
	expires_in: 299//秒
	refresh_token: "cdbc4a21-2481-4530-a5c6-a0496497a71f"*/
}).then(function(resJson){console.log(resJson)});

//刷新Token
fetch("/Token", {
  method: "POST",
  headers: {
  	"Content-Type": "application/x-www-form-urlencoded"
  },
  body: 'client_id=Michael'+//必须是某个字段
        '&client_secret=Michael'+//必须是某个字段
        '&refresh_token=d9f23374-fe82-4aac-8f3f-88ed90a4e765'+//必须上次返回的acess_token中的refresh_token
        '&grant_type=refresh_token'+//必须是refresh_token
        '&response_type=token'//必须是token
}).then(function(res) {
  //res.status === 200 
  //除了200 其他都是错误 如：400-badRequest,500-Server Error
  //200 可能也会报错：
  console.log(res);
  return res.json();
  /*.expires: "Mon, 15 Apr 2019 08:06:06 GMT"//UTC时间
	.issued: "Mon, 15 Apr 2019 08:01:06 GMT"
	.refresh: "True"
	access_token: "9dMneKVrjArB44fopQeFPHv6dSzEofWICrVd-u-zvQO95tvE8hiW_QBk2-DQh0n6R9gFMy2wfKkuCXCb595g8rFFFh4WXKsJYaTLVAQl8EqEeI4KyS317cVkMvX5wdXCYhtzhw_rD4WpLsfVdUPmxrp4IluX5cjizj3VaBUK4iaCNL7ph446wF7Faay_GVNAAcnCIO_ch-Jh9Dv8bFq2ApAqwrKfKKOQ6B5XHaptVu8"
	expires_in: 299//秒
	refresh_token: "cdbc4a21-2481-4530-a5c6-a0496497a71f"*/
}).then(function(resJson){console.log(resJson)});

//调用 getinfo接口
fetch("/Loan/getinfoD", {
  method: "POST",
  headers: {
      Authorization:"bearer ni9x7HW2glr5pJYIS2ns7TqGjTh5HkuDdQrvp5FvljYmulQ5wkXM5-vvGWmmnKf9Rvde15RsrHJFcErsogIM8xgI7QJW5fy_LT2K64IPFsBIAlqdYvTMabkx1gkkTy-mmG8G8ss7W78R10I6pJoXu8Ced552mUje2Ilp-8qRgeBKM1M6i4xWy8Ey8QxQR647SXJZJhEzfvQrRPP2suZ_GtK3Vuou0cSVt-9gieUyn56RtPXW5stQNfG3drb3Vu4c"
      /*Authorization:'bearer'+空格+'access_token'*/
      ,"Content-Type": "application/json",
  },
  body: JSON.stringify({
    	ENTITY_XML:`<?xml version="1.0" encoding="UTF-8"?>
		<ENTITY>
		  <Head>
		   <Trans_Id>20190412FLD1001000001</Trans_Id>
		   <Trans_Code>FLD1001</Trans_Code>
		  </Head>
		  <Data>
		    <Unn_Soc_Cr_Cd>1234567890123</Unn_Soc_Cr_Cd>
		    <CoPlf_ID>1234567</CoPlf_ID>
		    <KeyCode>KeyCode123</KeyCode>
		    <remark1></remark1>
		    <remark2></remark2>
		  </Data>
		</ENTITY>`
    })
}).then(function(res) {
  //res.status === 200 
  //除了200 其他都是错误 如：400-badRequest,500-Server Error
  //200 可能也会报错：
  console.log(res);
  return res.text();
  //返回string字符串XML-按文档规范返回
  //<?xml version="1.0" encoding="utf-16"?><ENTITY><Head><Trans_Id>20190412FLD1001000001</Trans_Id><Trans_Code>FLD1001</Trans_Code><return_code>000000</return_code><return_msg /></Head><Receivable_Accout_List><Splr_Nm>供应商名称</Splr_Nm><Unn_Soc_Cr_Cd>1234567890123</Unn_Soc_Cr_Cd><Pyr_Nm>付款方名称</Pyr_Nm><TLmt_Val>1000.00</TLmt_Val><LoanApl_Amt>100</LoanApl_Amt><Txn_ExDat>02</Txn_ExDat><Rfnd_AccNo>Rfnd_AccNo123</Rfnd_AccNo></Receivable_Accout_List></ENTITY>
  //ibI0x%QcVnrxDHT6QzGFfDaGI45%/skoCJlcNaHO6NOlGD5hiXBDzeIRFik4oZ4gbZ1%QnqL1HkHiCDjhvQSP3lkuSp5BcrIqJZjStyNdiOsAQgWHNYWdrD1donFuyKHfInYLdNWREKp6OiiULgc4eVQqUM499XaxX/601laSfBovY42406lwBUeap9x1e6sOmjWJKZrfLlYY3PxzNhLgUXCmLmPC47ffwKwr5KQw/PKtKIpeJdfdPB81b2wfCcobGtYrwyEFjeQOItRKZj11fovbIIH9uLGsa80fWZGK5UJatU/90gVW5Hxr2UyliG1wnxwDsgm%PLU1hqKpDlqcEq3SB87tFLKIWmcEVx5eb3G%d2EQ0aH19cMzx3KblIdwBJZPjs3JJV6HDclp7FwGvTjtIuhvbFXZspcJ4JiKQcGydcmSY3slEjfjAn33TrbbPQPmspTtI2q3xgibBEdpA3KWq7lnkNMnQkI3yEhw/XhJ4qZqUZP4GiwLY/uMBmq5%Dh/Iacsgj3M9II6vOZ9rMOUv3%ro0LXDEPmUtmKGaSDkf%cgZRMw1MbsbFyXC40w5MBg2QXLjPZV45AzNmgxMTZ0xkn1BdHhNK89/7OAblwDkRk4aO6g==
}).then(function(resJson){console.log(resJson)});

//调用 getinfo接口2
fetch("/Loan/getinfo", {
  method: "POST",
  headers: {
      Authorization:"bearer R8sVb0zetxZeR-0e63j31GobJM75rw484wjpJx7LOXEKlw-YfpJJLKyyUhtMDyLs7yhFMvAFIKJfrEz3vInfXm7aHxakU_G_KqsnJx2HP3vVf3HIsSZ4bRLRb6veglAIKRIk8VIfj38B2onJd6IRJQhJrRIVLkxSOz7tPTCG2iRm3qPrR9lvFnJLLidHxgYAMVCtWhX3y9dG7VM4XsF7VUSuZ6mwhWfbBbHm5qv_gzw"
      /*Authorization:'bearer'+空格+'access_token'*/
      ,"Content-Type": "application/x-www-form-urlencoded",
  },
  //开头必须是=
  body: `=<?xml version="1.0" encoding="UTF-8"?>
		<ENTITY>
		  <Head>
		   <Trans_Id>20190412FLD1001000001</Trans_Id>
		   <Trans_Code>FLD1001</Trans_Code>
		  </Head>
		  <Data>
		    <Unn_Soc_Cr_Cd>1234567890123</Unn_Soc_Cr_Cd>
		    <CoPlf_ID>1234567</CoPlf_ID>
		    <remark1></remark1>
		    <remark2></remark2>
		  </Data>
		</ENTITY>`
}).then(function(res) {
  //res.status === 200
  //除了200 其他都是错误 如：400-badRequest,500-Server Error
  //200 可能也会报错：
  console.log(res);
  return res.text();
  //返回string字符串XML-按文档规范返回
  //ibI0x%QcVnrxDHT6QzGFfDaGI45%/skoCJlcNaHO6NOlGD5hiXBDzeIRFik4oZ4gbZ1%QnqL1HkHiCDjhvQSP3lkuSp5BcrIqJZjStyNdiOsAQgWHNYWdrD1donFuyKHfInYLdNWREKp6OiiULgc4eVQqUM499XaxX/601laSfBovY42406lwBUeap9x1e6s/Y2iAA8op7cALZMGJCSuEYMGB14BSyikS0ifmXVU9JCr5YmMH30skR0PTQpy8HBiscub/UUf2Z9HYixhkJQAFu92FGu1wyhtu4HIWwtU8bZ4Hh172WpBJE817vuJHTA1ZOTpiaxXGQgmppgRNFhM/pEMkFARgBixBBGDYRUx46seB276SzhcjQ==
}).then(function(resJson){console.log(resJson)});



//调用 resultnotice接口2
fetch("/Loan/resultnotice", {
  method: "POST",
  headers: {
      Authorization:"bearer auWL4Kd8sYNbq7hpwnZDhgUfotbBHmIYL0Cy0Ixrxt55ZtJGWKnI5-s1A_WA-2FqJJZ-IfW9aWkeehhnhOOQMBh9-7Q7jJPZCAEPk08jLky2tSoxus4PSMrvudpGDaWh38whGHXpmT9OSa0GLgkbuftvY_ucpg0vPRUz4E788MuXhtBbyXrvjweWiQMbr2pNJ8ea8B2klVPABKwQkD-__9GqgBFrVYc6QYmMjeoI9GhyDxiJXW27NvjLHzpByI17"
      /*Authorization:'bearer'+空格+'access_token'*/
      ,"Content-Type": "application/x-www-form-urlencoded",
  },
  //开头必须是=
  body: `=<?xml version="1.0" encoding="UTF-8"?>
		<ENTITY>
			<Head>
				<Trans_Id>20190412FLD1002000001</Trans_Id>
				<Trans_Code>FLD1002</Trans_Code>
			</Head>
			<Data>
				<Unn_Soc_Cr_Cd>Unn_Soc_Cr_Cd345</Unn_Soc_Cr_Cd>
				<CoPlf_ID>CoPlf_ID345</CoPlf_ID>
				<Sign_Dt>20190412</Sign_Dt>
				<Sgn_Cst_Nm>甲乙丙丁</Sgn_Cst_Nm>
				<AR_Lmt>500000</AR_Lmt>
				<Lmt_ExDat>20190512</Lmt_ExDat>
				<Rfnd_AccNo>6217000000000000000</Rfnd_AccNo>
				<remark1></remark1>
				<remark2></remark2>
			</Data>
		</ENTITY>`
}).then(function(res) {
  //res.status === 200
  //除了200 其他都是错误 如：400-badRequest,500-Server Error
  //200 可能也会报错：
  console.log(res);
  return res.text();
  //返回string字符串XML-按文档规范返回
  //ibI0x%QcVnrxDHT6QzGFfDaGI45%/skoCJlcNaHO6NOlGD5hiXBDzeIRFik4oZ4gbZ1%QnqL1HkHiCDjhvQSP3lkuSp5BcrIqJZjStyNdiOsAQgWHNYWdrD1donFuyKHfInYLdNWREKp6OiiULgc4eVQqUM499XaxX/601laSfBovY42406lwBUeap9x1e6s/Y2iAA8op7cALZMGJCSuEYMGB14BSyikS0ifmXVU9JCr5YmMH30skR0PTQpy8HBiscub/UUf2Z9HYixhkJQAFu92FGu1wyhtu4HIWwtU8bZ4Hh172WpBJE817vuJHTA1ZOTpiaxXGQgmppgRNFhM/pEMkFARgBixBBGDYRUx46seB276SzhcjQ==
}).then(function(resJson){console.log(resJson)});

fetch("/Loan/DesDecrypt", {
  method: "POST",
  headers: {
      Authorization:"bearer qY1Sovvt_-qfCEciAwlTksviiHSrrCahuUa7s57tBIkduN8KZvaUGu-q1Ylwk6NWmC9lY5iWjt0qcD8_mSdICC1NknkL4Te8Otw6XJ5f8veOYwS93TzcbcOQzLq-A16WpUfIVQvwo86_1XQPUTrEnoeeh6-B56hc6A_utX0vpXPa90lXQQmdroMVp47vsPk7uylT1sd5T9R3OipziTh5TogNp6w_M3wUB-CcNH1Uzlj-RauCOnAtt3y59NgFUPUQ"
      ,"Content-Type": "application/json",
  },
  body: JSON.stringify({
    	Password:"fldkdjhs",IV:"00000000",EnCrypStr:`U2tniQtsPWaWji0fMkshfxFry3gU18tSSkJ83TqdWNWZ/thybk3aIc7mreWDknRDWqC3TLjt45lNe75yUGYIKQKfwIKkdgtEO9WiC+TmEag3P4SqfWNvdf8EwnIOXGEXS2bbvvWQvxR6tdUAyoRb5b0o96OFMpa37NtMrZ9ahm8syBBt/sY8V78x+OW4e/9Z5djIzs5lNJxGTrQ4rrB6/YKu8566jVBrlYbWhWO5O++wm1LnAWe9vhTTHDFoRCRO9G1njqCrjhP9Al5uDfe/CUXzaS1Jd87xaMN8L/b9qMXLZ2MI27fPWM+0wnUvY8CzDbmwDD5Vp0yQwyfJBtW0ls9DtgBvsAjnmpgvxrBwwKuCniEMqT3PIP85MCLiSD2vDnRZ/wwIawSKtbWEq5ZriLubtdfEO2kr0rusQljpoA6x8wvnLwnldQ==`
    })
}).then(function(res) {
  //res.status === 200
  //除了200 其他都是错误 如：400-badRequest,500-Server Error
  //200 可能也会报错：
  console.log(res);
  return res.text();
}).then(function(resJson){console.log(resJson)});























/*jquery-ajax方式*/

//获取Token
$.ajax({
    headers: {

    },
    type: "post",
    url:'/Token',
    data:{
    	client_id:'Michael',
        client_secret:'Michael',
        UserName:'Michael',
        grant_type:'password',
        response_type:'token'
    },
    success: function (data) {
    	console.log(data);
    }
});
//刷新Token
$.ajax({
    headers: {

    },
    type: "post",
    url:'/Token',
    data:{
    	client_id:'Michael',
        client_secret:'Michael',
        refresh_token:'5d73dcdf-8832-46d3-a3be-2a9d062864b8',
        grant_type:'refresh_token',
        response_type:'token'
    },
    success: function (data) {
    	console.log(data);
    }
});
//请求
$.ajax({
    headers: {
		Authorization:"bearer cd0L0EGjVRoisLOIxDdx_SOwUr3gFJ7LVEgC3vGb66-IUvggu7TaRLrzx-G3VH0G4QMxuZl-31Gg-q_JhJxqHCMtz_DGqzf6w5j7WMs-KNc-FA0FoiJK2J_Q7Jw1UK1u3ZbLd5pmog_pwBNJswFXjTSxwQuxy26YWxyFWuDXqiJkdO9qDHYOE1V9NzqX4WLbCt5OA0bontPjoNe_KUeEjll23UdfJpw_3xdMRNMZo4Q"
    },
    type: "get",
    url:'/api/Test/Get',
    data:{
    },
    success: function (data) {
    	console.log(data);
    }
});

$.ajax({
    headers: {
		Authorization:"bearer rNfOfmT8OiU2hkQhWdWaRZhIQJr1ASktY3P7dPFtpeRNxo4FXSol-sgPo2WsP6QbJBAvLAghA7HESIJus9q3oQURrWoYzj26ji00eeofMgyQXki7n7N29YQ8npwG6h3nYsEYEys5vIkjLbgjgElyJlkVor-HHOzDHjsv2D2dak5Mrvb5Cg_cY3EnoVz_KeMb3mSSgQamXJ9yyLKx1YmWYHnr3GUorfFYFonWpF7ulKA"
    },
    url:'/Loan/getinfoD',
    type: "post",
    contentType: 'application/json',
    data:JSON.stringify({
    	ENTITY_XML:`<?xml version="1.0" encoding="UTF-8"?>
<ENTITY>
  <Head>
   <Trans_Id>20190412FLD1001000001</Trans_Id>
   <Trans_Code>FLD1001</Trans_Code>
  </Head>
  <Data>
    <Unn_Soc_Cr_Cd>1234567890123</Unn_Soc_Cr_Cd>
    <CoPlf_ID>1234567</CoPlf_ID>
    <remark1></remark1>
    <remark2></remark2>
  </Data>
 </ENTITY>`
    }),
    success: function (data) {
    	console.log(data);
    }
});