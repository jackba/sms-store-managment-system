ALTER PROCEDURE [dbo].[SP_GET_INVENTORY]
@MA_KHO INT,
@TEN_KHO NVARCHAR(200),
@MA_SAN_PHAM INT, 
@TEN_SAN_PHAM NVARCHAR(200)
AS
SELECT TOP 999
	SAN_PHAM.MA_SAN_PHAM, 
	SAN_PHAM.TEN_SAN_PHAM, 
	DON_VI_TINH.TEN_DON_VI,
	ISNULL(SUM(SO_LUONG_TON),0) SO_LUONG_TON,
	ISNULL(SUM(SO_LUONG_TON),0)*SAN_PHAM.GIA_BAN_1 VALUE
FROM
SAN_PHAM
LEFT JOIN V_NHAP_XUAT_KHO ON V_NHAP_XUAT_KHO.MA_SAN_PHAM = SAN_PHAM.MA_SAN_PHAM AND (@MA_KHO IS NULL OR @MA_KHO = 0 OR V_NHAP_XUAT_KHO.MA_KHO = @MA_KHO)
JOIN KHO ON V_NHAP_XUAT_KHO.MA_KHO = KHO.MA_KHO AND (@TEN_KHO IS NULL OR @TEN_KHO = '' OR UPPER(KHO.TEN_KHO) LIKE '%' + UPPER(@TEN_KHO) + '%' )
JOIN DON_VI_TINH ON SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
WHERE
SAN_PHAM.ACTIVE = 'A'
AND SAN_PHAM.TEN_SAN_PHAM LIKE '%' + @TEN_SAN_PHAM + '%'
GROUP BY SAN_PHAM.MA_SAN_PHAM,SAN_PHAM.TEN_SAN_PHAM, DON_VI_TINH.TEN_DON_VI,SAN_PHAM.GIA_BAN_1
ORDER BY SAN_PHAM.MA_SAN_PHAM


ALTER PROC [dbo].[SP_GET_VALUE_OF_INVENTORY]
@MA_KHO INT,
@TEN_KHO NVARCHAR(200),
@MA_SAN_PHAM INT, 
@TEN_SAN_PHAM NVARCHAR(200)
AS
SELECT 
ISNULL(SUM(VALUE),0) VALUE
FROM
(
	SELECT 
		ISNULL(SUM(SO_LUONG_TON),0)*SAN_PHAM.GIA_BAN_1 VALUE
	FROM
	SAN_PHAM
	LEFT JOIN V_NHAP_XUAT_KHO ON V_NHAP_XUAT_KHO.MA_SAN_PHAM = SAN_PHAM.MA_SAN_PHAM AND (@MA_KHO IS NULL OR @MA_KHO = 0 OR V_NHAP_XUAT_KHO.MA_KHO = @MA_KHO)
	JOIN KHO ON V_NHAP_XUAT_KHO.MA_KHO = KHO.MA_KHO AND (@TEN_KHO IS NULL OR @TEN_KHO = '' OR UPPER(KHO.TEN_KHO) LIKE '%' + UPPER(@TEN_KHO) + '%')
	JOIN DON_VI_TINH ON SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
	WHERE
	SAN_PHAM.ACTIVE = 'A'
	AND SAN_PHAM.TEN_SAN_PHAM LIKE '%' + @TEN_SAN_PHAM + '%'
	GROUP BY SAN_PHAM.MA_SAN_PHAM,SAN_PHAM.TEN_SAN_PHAM, DON_VI_TINH.TEN_DON_VI,SAN_PHAM.GIA_BAN_1
)TEMP


ALTER PROCEDURE [dbo].[SP_IMPORT_REPORTER]
@KIND INT, 
@MA_KHO INT,
@TEN_KHO NVARCHAR(200),
@MA_SAN_PHAM INT, 
@TEN_SAN_PHAM NVARCHAR(200),
@FROM_DATE DATE, 
@TO_DATE DATE
AS
BEGIN
	SELECT 
		V_NHAP_KHO.MA_SAN_PHAM, 
		SAN_PHAM.TEN_SAN_PHAM, 
		DON_VI_TINH.TEN_DON_VI,
		SUM(V_NHAP_KHO.SO_LUONG) SO_LUONG, 
		ISNULL(V_NHAP_KHO.GIA_VON,0) GIA_VON, 
		ISNULL(SUM(V_NHAP_KHO.SO_LUONG)*GIA_VON,0) VALUE 
	FROM
	V_NHAP_KHO, 
	KHO,
	SAN_PHAM, 
	DON_VI_TINH
	WHERE 
		(@KIND IS NULL OR @KIND = -1 OR V_NHAP_KHO.LY_DO_NHAP = @KIND)
		AND (@TEN_KHO IS NULL OR @TEN_KHO = '' OR UPPER(KHO.TEN_KHO) LIKE '%' + UPPER(@TEN_KHO) + '%') 
		AND (@MA_KHO IS NULL OR @MA_KHO = 0 OR V_NHAP_KHO.MA_KHO = @MA_KHO)
		AND (@MA_SAN_PHAM IS NULL OR @MA_SAN_PHAM = 0 OR V_NHAP_KHO.MA_SAN_PHAM = @MA_SAN_PHAM)
		AND (@FROM_DATE IS NULL OR V_NHAP_KHO.NGAY_NHAP >= @FROM_DATE)
		AND (@TO_DATE IS NULL OR V_NHAP_KHO.NGAY_NHAP <= @TO_DATE)
		AND (@TEN_SAN_PHAM IS NULL OR @TEN_SAN_PHAM = '' OR SAN_PHAM.TEN_SAN_PHAM LIKE '%' + @TEN_SAN_PHAM + '%')
		AND V_NHAP_KHO.MA_SAN_PHAM = SAN_PHAM.MA_SAN_PHAM
		AND SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
		AND V_NHAP_KHO.MA_KHO = KHO.MA_KHO
	GROUP BY 
		V_NHAP_KHO.MA_SAN_PHAM, 
		SAN_PHAM.TEN_SAN_PHAM, 
		V_NHAP_KHO.GIA_VON, 
		DON_VI_TINH.TEN_DON_VI
END 

ALTER PROCEDURE [dbo].[SP_IMPORT_REPORTER_SUM]
@KIND INT, 
@MA_KHO INT,
@TEN_KHO NVARCHAR(200),
@MA_SAN_PHAM INT, 
@TEN_SAN_PHAM NVARCHAR(200),
@FROM_DATE DATE, 
@TO_DATE DATE
AS
BEGIN
		SELECT
		ISNULL(SUM(VALUE),0) VALUE
		FROM
		(
			SELECT
				SUM(V_NHAP_KHO.SO_LUONG)*GIA_VON VALUE 
			FROM
			V_NHAP_KHO, 
			KHO,
			SAN_PHAM
			WHERE 
				(@KIND IS NULL OR @KIND = -1 OR V_NHAP_KHO.LY_DO_NHAP = @KIND)
				AND (@TEN_KHO IS NULL OR @TEN_KHO = '' OR UPPER(KHO.TEN_KHO) LIKE '%' + UPPER(@TEN_KHO) + '%')
				AND (@MA_KHO IS NULL OR @MA_KHO = 0 OR V_NHAP_KHO.MA_KHO = @MA_KHO)
				AND (@MA_SAN_PHAM IS NULL OR @MA_SAN_PHAM = 0 OR V_NHAP_KHO.MA_SAN_PHAM = @MA_SAN_PHAM)
				AND (@FROM_DATE IS NULL OR V_NHAP_KHO.NGAY_NHAP >= @FROM_DATE)
				AND (@TO_DATE IS NULL OR V_NHAP_KHO.NGAY_NHAP <= @TO_DATE)
				AND (@TEN_SAN_PHAM IS NULL OR @TEN_SAN_PHAM = '' OR SAN_PHAM.TEN_SAN_PHAM LIKE '%' + @TEN_SAN_PHAM + '%')
				AND V_NHAP_KHO.MA_SAN_PHAM = SAN_PHAM.MA_SAN_PHAM
			GROUP BY 
				V_NHAP_KHO.MA_SAN_PHAM, 
				SAN_PHAM.TEN_SAN_PHAM, 
				V_NHAP_KHO.GIA_VON, 
				V_NHAP_KHO.NGAY_NHAP
		)TEMP
END 