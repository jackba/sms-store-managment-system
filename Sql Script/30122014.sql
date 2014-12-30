 CREATE TABLE USER_STORE
 (
	ID INT IDENTITY(1,1) PRIMARY KEY, 
	MA_KHO INT FOREIGN KEY REFERENCES KHO(MA_KHO),
	USR_ID INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
 )