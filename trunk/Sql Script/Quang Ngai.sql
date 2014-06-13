CREATE DATABASE SMS;

USE SMS;


CREATE TABLE NHOM_NGUOI_DUNG -- META DATA
(
	MA_NHOM INT IDENTITY(1,1) PRIMARY KEY, 
	TEN_NHOM NVARCHAR(50) NOT NULL,
	GHI_CHU NVARCHAR(1000), 
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)


CREATE TABLE KHO -- METEDATA
(
	MA_KHO INT IDENTITY(1,1) PRIMARY KEY, 
	TEN_KHO NVARCHAR(100), 
	SO_DIEN_THOAI VARCHAR(15),
	DIA_CHI NVARCHAR(300),
	GHI_CHU NVARCHAR(1000), 
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE NGUOI_DUNG
(
	MA_NGUOI_DUNG INT IDENTITY(1,1) PRIMARY KEY, 
	TEN_NGUOI_DUNG NVARCHAR(50) NOT NULL, 
	NGAY_SINH DATE NOT NULL,
	SO_CHUNG_MINH VARCHAR(15) NOT NULL,
	DIA_CHI NVARCHAR(200) NOT NULL, 
	SO_DIEN_THOAI VARCHAR(15), 
	MA_KHO INT FOREIGN KEY REFERENCES KHO(MA_KHO), 
	USER_NAME VARCHAR(20) NOT NULL, 
	MAT_KHAU VARCHAR(40) NOT NULL,
	NGAY_VAO_LAM DATE, 
	HINH_ANH VARBINARY(MAX),
	GHI_CHU NVARCHAR(1000), 
	MA_NHOM_NGUOI_DUNG INT FOREIGN KEY REFERENCES NHOM_NGUOI_DUNG(MA_NHOM), 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

INSERT INTO NGUOI_DUNG
(
	TEN_NGUOI_DUNG,
	NGAY_SINH,
	SO_CHUNG_MINH,
	DIA_CHI,
	SO_DIEN_THOAI,
	MA_KHO,
	USER_NAME,
	MAT_KHAU,
	NGAY_VAO_LAM,
	HINH_ANH,
	GHI_CHU,
	MA_NHOM_NGUOI_DUNG,
	CREATE_BY,
	UPDATE_BY,
	CREATE_AT,
	UPDATE_AT,
	ACTIVE
)
VALUES
(
	'ADMIN', 
	GETDATE(), 
	'CHƯA CẤP',
	'CHƯA CÓ', 
	'CHƯA MUA', 
	NULL, 
	'ADMIN', 
	'ADMIN', 
	GETDATE(),
	NULL, 
	NULL,
	NULL, 
	NULL, 
	NULL, 
	GETDATE(), 
	GETDATE(), 
	'A'
)

ALTER TABLE NHOM_NGUOI_DUNG
ADD
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG)

ALTER TABLE KHO
ADD
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG)

INSERT INTO KHO(TEN_KHO, GHI_CHU, CREATE_BY, UPDATE_BY) VALUES (N'Cửa hàng 1', N'Kho cửa hàng 1', 1, 1);
INSERT INTO KHO(TEN_KHO, GHI_CHU, CREATE_BY, UPDATE_BY) VALUES (N'Cửa hàng 2', N'Kho cửa hàng 2', 1, 1);
INSERT INTO KHO(TEN_KHO, GHI_CHU, CREATE_BY, UPDATE_BY) VALUES (N'Cửa hàng 3', N'Kho cửa hàng 3', 1,1);

INSERT INTO 
NHOM_NGUOI_DUNG(TEN_NHOM,GHI_CHU, CREATE_BY, UPDATE_BY)
VALUES('Administrator', N'Nhóm quản trị',1,1);

INSERT INTO 
NHOM_NGUOI_DUNG(TEN_NHOM,GHI_CHU, CREATE_BY, UPDATE_BY)
VALUES(N'Nhập liệu', N'Nhóm nhập liệu: tạo mới danh sách sản phẩm, danh sách đơn vị, thay đổi đơn giá sản phẩm ....',1,1); 


INSERT INTO 
NHOM_NGUOI_DUNG(TEN_NHOM,GHI_CHU, CREATE_BY, UPDATE_BY)
VALUES(N'Bán hàng', N'Nhóm bán hàng',1,1);

INSERT INTO 
NHOM_NGUOI_DUNG(TEN_NHOM,GHI_CHU, CREATE_BY, UPDATE_BY)
VALUES(N'Thâu ngân', N'Thâu ngân',1,1);


CREATE TABLE KHU_VUC
(
	MA_KHU_VUC INT IDENTITY(1,1) PRIMARY KEY, 
	TEN_KHU_VUC NVARCHAR(50) NOT NULL, 
	GHI_CHU NVARCHAR(2000), 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE KHACH_HANG
(
	MA_KHACH_HANG INT IDENTITY(1,1) PRIMARY KEY, 
	MA_THE_KHACH_HANG VARCHAR(15), 
	TEN_KHACH_HANG NVARCHAR(50) NOT NULL, 
	DIA_CHI NVARCHAR(200), 
	SO_DIEN_THOAI VARCHAR(15), 
	EMAIL VARCHAR(50),
	MA_KHU_VUC INT FOREIGN KEY REFERENCES KHU_VUC(MA_KHU_VUC), 
	DOANH_SO MONEY, 
	NO_GOI_DAU MONEY, 
	HINH_ANH VARBINARY(MAX),
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
);

CREATE UNIQUE INDEX UNIQUE_A_THE_KH ON KHACH_HANG(MA_THE_KHACH_HANG) WHERE MA_THE_KHACH_HANG IS NOT NULL
;

CREATE TABLE NHA_CUNG_CAP
(
	MA_NHA_CUNG_CAP INT IDENTITY(1,1) PRIMARY KEY, 
	TEN_NHA_CUNG_CAP NVARCHAR(100) NOT NULL, 
	DIA_CHI NVARCHAR(200), 
	TEN_NGUOI_LIEN_HE NVARCHAR(100), 
	SO_DIEN_THOAI VARCHAR(15), 
	EMAIL VARCHAR(50),
	GHI_CHU NVARCHAR(1000), 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)


CREATE TABLE PHAN_QUYEN
(
	ID INT IDENTITY(1,1) PRIMARY KEY, 
	MA_NGUOI_DUNG INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	QUYEN_ADMIN BIT, 
	QUYEN_DANH_MUC_SAN_PHAM BIT, 
	QUYEN_BAN_HANG BIT, 
	QUYEN_THAU_NGAN BIT,
	MA_NHOM_NGUOI_DUNG INT FOREIGN KEY REFERENCES NHOM_NGUOI_DUNG(MA_NHOM), 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE NHA_SAN_XUAT
(
	MA_NHA_SAN_XUAT INT IDENTITY(1,1) PRIMARY KEY, 
	TEN_NHA_SAN_XUAT NVARCHAR(50), 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE DON_VI_TINH
(
	MA_DON_VI INT IDENTITY(1,1) PRIMARY KEY, 
	TEN_DON_VI NVARCHAR(50), 
	GHI_CHU NVARCHAR(1000), 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE SAN_PHAM
(
	MA_SAN_PHAM INT IDENTITY(1,1) PRIMARY KEY, 
	TEN_SAN_PHAM NVARCHAR(50) NOT NULL,
	KICH_THUOC VARCHAR(100), 
	CAN_NANG FLOAT, 
	DAC_TA NVARCHAR(1000), 
	MA_DON_VI INT  FOREIGN KEY REFERENCES DON_VI_TINH(MA_DON_VI),
	MA_NHA_SAN_XUAT INT FOREIGN KEY REFERENCES NHA_SAN_XUAT(MA_NHA_SAN_XUAT),
	HINH_ANH VARBINARY(MAX),
	GIA_BAN_1 FLOAT, 
	GIA_BAN_2 FLOAT, 
	GIA_BAN_3 FLOAT, 
	CHIEC_KHAU_1 FLOAT, 
	CHIEC_KHAU_2 FLOAT, 
	CHIEC_KHAU_3 FLOAT, 
	CO_SO_TOI_THIEU FLOAT, 
	CO_SO_TOI_DA FLOAT,
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE CHUYEN_DOI_DON_VI_TINH
(
	MA_CHUYEN_DOI INT IDENTITY(1,1) PRIMARY KEY, 
	MA_SAN_PHAN INT FOREIGN KEY REFERENCES SAN_PHAM(MA_SAN_PHAM),
	MA_DON_VI_VAO INT FOREIGN KEY REFERENCES DON_VI_TINH(MA_DON_VI),
	HE_SO FLOAT, 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE HOA_DON
(
	MA_HOA_DON INT IDENTITY(1,1) PRIMARY KEY, 
	SO_HOA_DON VARCHAR(30), 
	MA_KHACH_HANG INT FOREIGN KEY REFERENCES KHACH_HANG(MA_KHACH_HANG), 
	TEN_KHACH_HANG NVARCHAR(100), -- DÙNG CHO KHÁCH VÃNG LAI
	MA_NHAN_VIEN_BAN INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	NGAY_BAN DATE, 
	NGAY_GIAO DATE, 
	DIA_CHI_GIAO_HANG NVARCHAR(300), 
	STATUS SMALLINT,  -- 1: TẠO MỚI, 2 ĐÃ THU TIỀN, 3 ĐÃ XUẤT KHO
	EDIT_APPROVER INT FOREIGN  KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)


CREATE TABLE CHI_TIET_HOA_DON
(
	MA_HOA_DON INT FOREIGN KEY REFERENCES HOA_DON(MA_HOA_DON), 
	MA_SAN_PHAM INT FOREIGN KEY REFERENCES SAN_PHAM(MA_SAN_PHAM), 
	SO_LUONG FLOAT,
	DON_GIA FLOAT, 
	PHAN_TRAM_CHIEC_KHAU FLOAT, 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)


CREATE TABLE XUAT_KHO
(	
	MA_XUAT_KHO INT IDENTITY(1,1) PRIMARY KEY, 
	MA_HOA_DON INT FOREIGN KEY REFERENCES HOA_DON(MA_HOA_DON), -- XUẤT CHO BÁN HÀNG
	LY_DO_XUAT SMALLINT, -- O: XUẤT BÁN HÀNG, 1: XUẤT HỦY, 2: XUẤT DÙNG NỘI BỘ, 3: XUẤT CHUYỂN KHO
	MA_KHO_XUAT INT FOREIGN KEY REFERENCES KHO(MA_KHO), 
	MA_KHO_NHAN INT FOREIGN KEY REFERENCES KHO(MA_KHO), -- DÙNG TRONG TRƯỜNG HỢP CHUYỂN KHO
	NGAY_XUAT DATE, 
	MA_NHAN_VIEN_XUAT INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	TEN_NGUOI_NHAN_HANG NVARCHAR(50), 
	GHI_CHU NVARCHAR(1000), 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE CHI_TIET_XUAT_KHO
(
	MA_XUAT_KHO INT FOREIGN KEY REFERENCES XUAT_KHO(MA_XUAT_KHO),
	MA_SAN_PHAM INT FOREIGN KEY REFERENCES SAN_PHAM(MA_SAN_PHAM), 
	SO_LUONG FLOAT, 
	GIA_VON FLOAT, 
	GIA_XUAT FLOAT, 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)


CREATE TABLE TRA_HANG
(
	MA_TRA_HANG INT IDENTITY(1,1) PRIMARY KEY, 
	MA_XUAT_KHO INT FOREIGN KEY REFERENCES XUAT_KHO(MA_XUAT_KHO),
	TEN_KHACH_HANG NVARCHAR(100), 
	NGAY_TRA DATE, 
	NHAN_VIEN_NHAN INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	GHI_CHU NVARCHAR(1000), 
	STATUS SMALLINT, -- 1 TẠO MỚI, 2 ĐÃ NHẬP LẠI KHO, 3 ĐÃ LÀM PHIẾU TRẢ LẠI NHÀ CUNG CẤP
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE CHI_TIET_TRA_HANG
(
	MA_TRA_HANG INT FOREIGN KEY REFERENCES TRA_HANG(MA_TRA_HANG),
	MA_SAN_PHAM INT FOREIGN KEY REFERENCES SAN_PHAM(MA_SAN_PHAM), 
	SO_LUONG_TRA FLOAT, 
	GIA_BAN FLOAT, 
	GIA_VON FLOAT, 
	NHAP_LAI_KHO BIT,
	SO_LUONG_NHAP_LAI_KHO FLOAT, 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE NHAP_KHO
(
	MA_NHAP_KHO INT IDENTITY(1,1) PRIMARY KEY, 
	SO_HOA_DON VARCHAR(20), 
	NGAY_NHAP DATE, 
	NHAN_VIEN_NHAP INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	MA_NHA_CUNG_CAP INT FOREIGN KEY REFERENCES NHA_CUNG_CAP(MA_NHA_CUNG_CAP), 
	MA_KHO INT FOREIGN KEY REFERENCES KHO(MA_KHO), 
	LY_DO_NHAP SMALLINT, -- O: MUA HÀNG, 1: NHẬP LẠI HÀNG TRẢ, 2: KIỂM KHO, 3: NHẬP CHUYỂN KHO
	MA_PHIEU_TRA INT FOREIGN KEY REFERENCES TRA_HANG(MA_TRA_HANG), 
	GHI_CHU NVARCHAR(1000), 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

CREATE TABLE CHI_TIET_NHAP_KHO
(
	MA_NHAP_KHO INT FOREIGN KEY REFERENCES NHAP_KHO(MA_NHAP_KHO), 
	MA_SAN_PHAM INT FOREIGN KEY REFERENCES SAN_PHAM(MA_SAN_PHAM), 
	SO_LUONG FLOAT, 
	GIA_VON FLOAT, 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)


CREATE TABLE KIEM_KHO_HISTORY
( 
	MA_KIEM_KHO INT IDENTITY(1,1) PRIMARY KEY, 
	NGAY_KIEM_KHO DATE, 
	MA_NHAP_KHO INT FOREIGN KEY REFERENCES NHAP_KHO(MA_NHAP_KHO), 
	TONG_GIA_TRI_HANG_TON FLOAT, 
	CREATE_BY INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG), 
	UPDATE_BY  INT FOREIGN KEY REFERENCES NGUOI_DUNG(MA_NGUOI_DUNG),  
	CREATE_AT DATE, 
	UPDATE_AT DATE,
	ACTIVE CHAR(1)
)

INSERT INTO KIEM_KHO_HISTORY(NGAY_KIEM_KHO,MA_NHAP_KHO, TONG_GIA_TRI_HANG_TON, ACTIVE) VALUES(GETDATE(), NULL, 0, 'A')

CREATE FUNCTION STMA_GET_MAX_DATE_KIEM_KHO()
RETURNS DATE
AS
BEGIN
	RETURN (SELECT ISNULL(MAX(NGAY_KIEM_KHO),GETDATE()) AS NGAY_KIEM_KHO FROM KIEM_KHO_HISTORY WHERE ACTIVE = 'A')
END

-----------------------------------------------------------------------------
CREATE VIEW NHAP_XUAT_TON
AS
SELECT 
	NHAP_KHO.MA_KHO, 
	NHAP_KHO.MA_SAN_PHAM, 
	ISNULL(NHAP_KHO.SO_LUONG_NHAP,0) SO_LUONG_NHAP,
	ISNULL(XUAT_KHO.SO_LUONG_XUAT,0) SO_LUONG_XUAT,
	ISNULL(NHAP_KHO.SO_LUONG_NHAP,0) - ISNULL(XUAT_KHO.SO_LUONG_XUAT,0) AS SO_LUONG_TON
FROM
	(
		SELECT 
			NHAP_KHO.MA_KHO, 
			CHI_TIET_NHAP_KHO.MA_SAN_PHAM,
			ISNULL(SUM(CHI_TIET_NHAP_KHO.SO_LUONG),0) SO_LUONG_NHAP
		FROM 
			NHAP_KHO, 
			CHI_TIET_NHAP_KHO
		WHERE
			NHAP_KHO.ACTIVE = 'A'
			AND CAST(NHAP_KHO.NGAY_NHAP AS DATE) >= CAST(DBO.STMA_GET_MAX_DATE_KIEM_KHO() AS DATE)
			AND CHI_TIET_NHAP_KHO.ACTIVE = 'A'
			AND NHAP_KHO.MA_NHAP_KHO = CHI_TIET_NHAP_KHO.MA_NHAP_KHO
		GROUP BY NHAP_KHO.MA_KHO, CHI_TIET_NHAP_KHO.MA_SAN_PHAM
	)NHAP_KHO
FULL OUTER  JOIN 
	(	
		SELECT 
			XUAT_KHO.MA_KHO_XUAT AS MA_KHO,
			CHI_TIET_XUAT_KHO.MA_SAN_PHAM,
			ISNULL(SUM(CHI_TIET_XUAT_KHO.SO_LUONG),0) AS SO_LUONG_XUAT
		FROM 
			XUAT_KHO, 
			CHI_TIET_XUAT_KHO
		WHERE 
			XUAT_KHO.ACTIVE = 'A'
			AND CHI_TIET_XUAT_KHO.ACTIVE = 'A'
			AND CAST(XUAT_KHO.NGAY_XUAT AS DATE) >= CAST(DBO.STMA_GET_MAX_DATE_KIEM_KHO() AS DATE)
			AND XUAT_KHO.MA_XUAT_KHO = CHI_TIET_XUAT_KHO.MA_XUAT_KHO
		GROUP BY XUAT_KHO.MA_KHO_XUAT, CHI_TIET_XUAT_KHO.MA_SAN_PHAM
	)XUAT_KHO
ON   
NHAP_KHO.MA_KHO = XUAT_KHO.MA_KHO AND NHAP_KHO.MA_SAN_PHAM = XUAT_KHO.MA_SAN_PHAM

CREATE INDEX CHI_TIET_NHAP_KHO_INDEX
ON CHI_TIET_NHAP_KHO(MA_NHAP_KHO,CREATE_AT)

CREATE INDEX NHAP_KHO_INDEX
ON NHAP_KHO(MA_NHAP_KHO, NGAY_NHAP)

CREATE INDEX XUAT_KHO_INDEX
ON XUAT_KHO(MA_XUAT_KHO, NGAY_XUAT)

CREATE INDEX CHI_TIET_XUAT_KHO_INDEX
ON CHI_TIET_XUAT_KHO(MA_XUAT_KHO,CREATE_AT)

CREATE INDEX HOA_DON_INDEX
ON HOA_DON(MA_HOA_DON, NGAY_BAN)

CREATE INDEX CHI_TIET_HOA_DON_INDEX
ON CHI_TIET_HOA_DON(MA_HOA_DON, CREATE_AT)


CREATE INDEX HOA_DON_XUAT_KHO_INDEX
ON XUAT_KHO(MA_HOA_DON)


CREATE VIEW HOA_DON_CHUA_XUAT_KHO
AS
SELECT 
	MA_HOA_DON,
	SO_HOA_DON,
	MA_KHACH_HANG,
	TEN_KHACH_HANG,
	MA_NHAN_VIEN_BAN,
	NGAY_BAN,
	NGAY_GIAO,
	DIA_CHI_GIAO_HANG,
	CREATE_BY,
	UPDATE_BY,
	CREATE_AT,
	UPDATE_AT,
	ACTIVE
FROM 
	HOA_DON 
WHERE
	ACTIVE = 'A'
	AND SO_HOA_DON NOT IN( 
		SELECT 
			SO_HOA_DON 
		FROM 
			XUAT_KHO 
		WHERE 
			XUAT_KHO.ACTIVE = 'A'
			AND CAST(XUAT_KHO.NGAY_XUAT AS DATE) >= CAST(DBO.STMA_GET_MAX_DATE_KIEM_KHO() AS DATE)
	)


	
CREATE FUNCTION STMA_CHECK_TON_KHO
(
	@MA_KHO INT, 
	@MA_SAN_PHAM INT, 
	@SO_LUONG FLOAT
)
RETURNS BIT
AS
BEGIN
	DECLARE @SO_LUONG_TON FLOAT
	DECLARE @FLG BIT
	SET @SO_LUONG_TON = (SELECT TOP 1 SO_LUONG_TON FROM NHAP_XUAT_TON WHERE MA_KHO = @MA_KHO AND MA_SAN_PHAM = @MA_SAN_PHAM)
	IF @SO_LUONG_TON >= @SO_LUONG
	BEGIN
		SET @FLG = 'TRUE'
	END
	ELSE
	BEGIN
		SET @FLG = 'FALSE'
	END
	RETURN @FLG
END




