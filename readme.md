#Simple ready to user REST Web Api
---

> **Test 1 *(Get access token by username & password)*:**
>> **Request:**
> http://localhost:58714/token
> Header: Content-Type: application/x-www-form-urlencoded
> Body: username=prem1&password=1234&*grant_type=password*
> **Response:**
> {
**"access_token"**: "AvmKEu-bCaa7Mx9xahBW7u_apavuwusEJHQDjZB3z2SWuDaggam8ue_6T2zlqw8EoQyzvlvOZ8omuxfeZVLXJgy2ibcTvmBH7xIVgZqZvAi498Kj-X6KX2L6R_9ZEqQtFL6w3zmcYxd0oRqBSagjsMHfM-spAWKe7KQ8S0Bkm5BUbxVvY1R75pB50RmaIlWkwF-yOCKEQ86H9_7-1KQgCDNDbjvaDLdTd0J3czcsBeT_bz8qu07Qqc1IO654hiAmswIlG_K3JqgsiRihsnHC_L_eRxNg8Ap7d0z7uTgT0MVnuqkP3DLYFBLZfFhuquPUIKd0dLdMEAiH8y2dbKxyYA"
"token_type": "bearer"
"expires_in": 17999
**"refresh_token"**: "d51aff279a3b492f9481fbb7a9716469"
"userName": "Prem1"
".issued": "Fri, 11 Mar 2016 14:00:17 GMT"
".expires": "Fri, 11 Mar 2016 19:00:17 GMT"
}

> **Test 2 *(Get access token by refresh token)*:**
> > **Request:**
> http://localhost:58714/token
> Header: Content-Type: application/x-www-form-urlencoded
> Body: *grant_type=refresh_token*&client_id=prem1&refresh_token=d51aff279a3b492f9481fbb7a9716469
> **Response:**
> {
"access_token": "kZpQvKSFLOlwcTyHXLjcR9JWgdlTv2LFtg39gbLRuxGfQRddoi5ont2Mb7E0m2mIEDxWtArkUqkC4MRyd7wO5mZ3nmIuqs9qC9DNQ-AdhGw_p_mgUqBKmjaeXZdr8qyVb2IrTs1ymSd5bSM2oVMP1fcbSW-y51q07HHniAl18VmYPe92-MK4jVW68Qvw-onpLwjMcKYNjnOFO-LeE8RpTH1p73l-1_wp9DHlyG-cZDxl8h7OijQ5wOVLf-Pfo9FmP7q0NC5VCGM7-OhLASPNgvPnh01kMzF45geHofiAmK6VA3YXcOaT95tb0Se1QRaxgxeq_vJw_FstCjjs4iPApg"
"token_type": "bearer"
"expires_in": 17999
"refresh_token": "d8d1a2736acb42abac5b979372f5e72a"
"userName": "Prem1"
".issued": "Fri, 11 Mar 2016 14:19:10 GMT"
".expires": "Fri, 11 Mar 2016 19:19:10 GMT"
}
