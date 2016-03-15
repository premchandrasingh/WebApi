#Simple & ready to user REST Web Api
---
###Why named PPF
A random name while starting this sample. It stands for Past Present Future (PPF)

---
This sample does not have a database, instead have some hard coded users for testing.	
username:  prem1 ....................... Password: 1234	
username:  prem2 ....................... Password: 1234	
username:  prem3 ....................... Password: 1234	

---
**Test 1 *(Get access token by username & password)*:**	
**Request:**	
http://localhost:58714/token	
Header: Content-Type: application/x-www-form-urlencoded	
Body: username=prem1&password=1234&*grant_type=password*	

**Response:**	
{	
**"access_token"**: "AvmKEu...",	
"token_type": "bearer",	
"expires_in": 17999,	
**"refresh_token"**: "d51aff279a3b492f9481fbb7a9716469",	
"userName": "Prem1",	
".issued": "Fri, 11 Mar 2016 14:00:17 GMT",	
".expires": "Fri, 11 Mar 2016 19:00:17 GMT"	
}

---

 **Test 2 *(Get access token by refresh token)*:**	
**Request:**	
http://localhost:58714/token	
Header: Content-Type: application/x-www-form-urlencoded	
Body: *grant_type=refresh_token*&client_id=prem1&refresh_token=d51aff279a3b492f9481fbb7a9716469	

**Response:**	
{	
"access_token": "kZpQvKSFLOlwc...",	
"token_type": "bearer",	
"expires_in": 17999,	
"refresh_token": "d8d1a2736acb42abac5b979372f5e72a",	
"userName": "Prem1",	
".issued": "Fri, 11 Mar 2016 14:19:10 GMT",	
".expires": "Fri, 11 Mar 2016 19:19:10 GMT",	
}

---
# A little setting

This sample has option to enable json web token (JWT). Here is the small self explanatory setting

`[assembly: OwinStartup(typeof(PPF.API.StartupBearer))]`	
//`[assembly: OwinStartup(typeof(PPF.API.StartupJwt))]`
