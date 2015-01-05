 CREATE TABLE USER_STORE
 (
	ID INT IDENTITY(1,1) PRIMARY KEY, 
	MA_KHO INT FOREIGN KEY REFERENCES KHO(MA_KHO),
	USR_ID INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),
	IS_DEFAULT BIT,
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
 )
 
 CREATE PROC SP_GET_STORES_BY_USR_ID
 @USR_ID INT
 AS
 BEGIN
	SELECT 
		USER_STORE.MA_KHO, 
		KHO.TEN_KHO
	FROM 
	USER_STORE,
	KHO
	WHERE
	USER_STORE.ACTIVE = 'A'
	AND KHO.ACTIVE = 'A'
	AND USER_STORE.USR_ID = @USR_ID
	AND KHO.MA_KHO = USER_STORE.MA_KHO
	ORDER BY IS_DEFAULT DESC
END