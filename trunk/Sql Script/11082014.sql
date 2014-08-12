
ALTER PROCEDURE [dbo].[SP_GET_LIST_RETURN_TO_PROVIDERS]
@PROVIDER_ID INT, 
@PROVIDER_NAME NVARCHAR(100),
@FLAG INT, 
@FROM_DATE DATE, 
@TO_DATE DATE, 
@USER_ID INT, 
@USER_FULL_NAME NVARCHAR(100)
AS
BEGIN
	SELECT 
	TRA_HANG_NCC.ID, 
	CASE ISNULL(TRA_HANG_NCC.MA_PHIEU_TRA,0)
	WHEN 0 THEN
		CASE (SELECT COUNT(*) FROM XUAT_KHO WHERE XUAT_KHO.ACTIVE = 'A' AND XUAT_KHO.MA_PHIEU_TRA_NCC = TRA_HANG_NCC.ID)
			WHEN 0  THEN 
				NULL
			ELSE -1 
		END
	ELSE TRA_HANG_NCC.MA_PHIEU_TRA		
	END AS MA_PHIEU_TRA,
	TRA_HANG_NCC.NGAY_LAP_PHIEU, 
	TRA_HANG_NCC.NGUOI_LAP_PHIEU MA_NGUOI_LAP_PHIEU, 
	TRA_HANG_NCC.GHI_CHU, 
	NHA_CUNG_CAP.TEN_NHA_CUNG_CAP, 
	NGUOI_DUNG.TEN_NGUOI_DUNG TEN_NGUOI_LAP_PHIEU
	FROM
	TRA_HANG_NCC, 
	NHA_CUNG_CAP, 
	NGUOI_DUNG
	WHERE 
	TRA_HANG_NCC.ACTIVE = 'A'
	AND (@FLAG = 0 OR (@FLAG = 1 AND MA_PHIEU_TRA IS NOT  NULL) OR (@FLAG = 2 AND MA_PHIEU_TRA IS  NULL))
	AND (@PROVIDER_ID = 0 OR TRA_HANG_NCC.MA_NHA_CUNG_CAP = @PROVIDER_ID)
	AND (@PROVIDER_NAME IS NULL OR @PROVIDER_NAME = '' OR NHA_CUNG_CAP.TEN_NHA_CUNG_CAP LIKE '%' + @PROVIDER_NAME + '%')
	AND (@FROM_DATE IS NULL OR CAST(NGAY_LAP_PHIEU AS DATE) >= CAST(@FROM_DATE AS DATE))
	AND (@TO_DATE IS NULL OR CAST(NGAY_LAP_PHIEU AS DATE) <= CAST(@TO_DATE AS DATE))
	AND (@USER_ID = 0 OR TRA_HANG_NCC.NGUOI_LAP_PHIEU = @USER_ID)
	AND (@USER_FULL_NAME IS NULL OR @USER_FULL_NAME = '' OR NGUOI_DUNG.TEN_NGUOI_DUNG LIKE '%' + @USER_FULL_NAME + '%' )
	AND TRA_HANG_NCC.MA_NHA_CUNG_CAP = NHA_CUNG_CAP.MA_NHA_CUNG_CAP
	AND TRA_HANG_NCC.NGUOI_LAP_PHIEU = NGUOI_DUNG.MA_NGUOI_DUNG
	ORDER BY TRA_HANG_NCC.NGAY_LAP_PHIEU DESC
END


ALTER PROCEDURE [dbo].[SP_GET_WAITING_EX_2_PROVIDER]
@STORE_ID INT, 
@STORE_NAME NVARCHAR(100), 
@FROM_DATE DATE, 
@TO_DATE DATE, 
@STATAUS INT
AS
BEGIN
	SELECT 
		TRA_HANG_NCC.ID, 
		ISNULL(XUAT_KHO.MA_XUAT_KHO,0)MA_XUAT_KHO,
		TRA_HANG_NCC.MA_NHA_CUNG_CAP,
		NHA_CUNG_CAP.TEN_NHA_CUNG_CAP,
		TRA_HANG_NCC.NGAY_LAP_PHIEU,
		TRA_HANG_NCC.NGUOI_LAP_PHIEU,
		NGUOI_DUNG.TEN_NGUOI_DUNG,
		TRA_HANG_NCC.GHI_CHU ,
		DBO.GET_STAUS(TRA_HANG_NCC.ID) STATUS		
	FROM
		TRA_HANG_NCC LEFT JOIN XUAT_KHO ON TRA_HANG_NCC.ID = XUAT_KHO.MA_PHIEU_TRA_NCC AND XUAT_KHO.ACTIVE = 'A', 
		NHA_CUNG_CAP,
		NGUOI_DUNG, 
		TRA_HANG_NCC_CHI_TIET, 
		KHO
	WHERE 
		TRA_HANG_NCC.ACTIVE = 'A'		
		AND TRA_HANG_NCC.MA_PHIEU_TRA IS NULL
		AND (@FROM_DATE IS NULL OR CAST(TRA_HANG_NCC.NGAY_LAP_PHIEU AS DATE) >= CAST(@FROM_DATE AS DATE))
		AND (@TO_DATE IS NULL OR CAST(TRA_HANG_NCC.NGAY_LAP_PHIEU AS DATE) <= CAST(@FROM_DATE AS DATE))
		AND (@STORE_ID = 0 OR TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT = @STORE_ID)
		AND	(@STORE_NAME IS NULL OR @STORE_NAME = '' OR KHO.TEN_KHO LIKE '%' + @STORE_NAME + '%')
		AND TRA_HANG_NCC.MA_NHA_CUNG_CAP = NHA_CUNG_CAP.MA_NHA_CUNG_CAP
		AND TRA_HANG_NCC.NGUOI_LAP_PHIEU = NGUOI_DUNG.MA_NGUOI_DUNG		
		AND TRA_HANG_NCC.ID = TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC
		AND TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT = KHO.MA_KHO	
		AND 
		(@STATAUS = 0
		OR
		(@STATAUS = 1 AND DBO.F_GET_SUM_OF_RETUN(TRA_HANG_NCC.ID) > DBO.F_GET_SUM_OF_EXPORT(TRA_HANG_NCC.ID))
		OR 
		(@STATAUS = 2 AND DBO.F_GET_SUM_OF_RETUN(TRA_HANG_NCC.ID) <= DBO.F_GET_SUM_OF_EXPORT(TRA_HANG_NCC.ID))
		)	
		GROUP BY
		TRA_HANG_NCC.ID, 
		TRA_HANG_NCC.MA_NHA_CUNG_CAP,
		NHA_CUNG_CAP.TEN_NHA_CUNG_CAP,
		TRA_HANG_NCC.NGAY_LAP_PHIEU,
		TRA_HANG_NCC.NGUOI_LAP_PHIEU,
		NGUOI_DUNG.TEN_NGUOI_DUNG,
		TRA_HANG_NCC.GHI_CHU,		
		XUAT_KHO.MA_XUAT_KHO				
		ORDER BY TRA_HANG_NCC.NGUOI_LAP_PHIEU DESC
END

ALTER FUNCTION [dbo].[F_GET_SUM_OF_EXPORT]
(
	@ID INT
)
RETURNS FLOAT
AS
BEGIN
	RETURN 
		(SELECT ISNULL(SUM(SO_LUONG),0) FROM CHI_TIET_XUAT_KHO, XUAT_KHO
			WHERE XUAT_KHO.ACTIVE = 'A' AND CHI_TIET_XUAT_KHO.ACTIVE = 'A' 
			AND XUAT_KHO.MA_PHIEU_TRA_NCC = @ID 
			AND XUAT_KHO.MA_XUAT_KHO = CHI_TIET_XUAT_KHO.MA_XUAT_KHO
		)
END

ALTER PROCEDURE [dbo].[SP_GET_VALUE_ALL_HOA_DON]
@FROM_DATE DATE, 
@TO_DATE DATE, 
@MA_KHACH_HANG INT, 
@TEN_KHACH_HANG NVARCHAR(100),
@MA_NHAN_VIEN_BAN INT, 
@TEN_NV_BAN NVARCHAR(100),
@MA_NHAN_VIEN_TT INT, 
@TEN_NV_TT NVARCHAR(100),
@STATUS INT, 
@MA_KHU_VUC INT, 
@TEN_KHU_VUC NVARCHAR(100)
AS
BEGIN
	SELECT 
	ISNULL(SUM(SO_TIEN_KHACH_TRA),0) SO_TIEN_KHACH_TRA, 
	ISNULL(SUM(SO_TIEN_NO_GOI_DAU),0) SO_TIEN_NO_GOI_DAU
	FROM
	(SELECT  
		CASE WHEN HOA_DON.MA_KHACH_HANG IS NULL
					THEN HOA_DON.TEN_KHACH_HANG
				ELSE
					KHACH_HANG.TEN_KHACH_HANG 
			END AS TEN_KHACH_HANG,
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) SO_TIEN_KHACH_TRA, 
		ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) SO_TIEN_NO_GOI_DAU
	FROM 
		HOA_DON
		LEFT JOIN KHACH_HANG ON HOA_DON.MA_KHACH_HANG = KHACH_HANG.MA_KHACH_HANG
		LEFT JOIN KHU_VUC ON KHACH_HANG.MA_KHU_VUC = KHU_VUC.MA_KHU_VUC
		LEFT JOIN NGUOI_DUNG NV_BAN ON HOA_DON.MA_NHAN_VIEN_BAN = NV_BAN.MA_NGUOI_DUNG
		LEFT JOIN NGUOI_DUNG NV_TT ON HOA_DON.MA_NHAN_VIEN_THU_TIEN = NV_TT.MA_NGUOI_DUNG
	WHERE
		HOA_DON.ACTIVE = 'A'
		AND (@TEN_NV_BAN IS NULL OR @TEN_NV_BAN = '' OR NV_BAN.TEN_NGUOI_DUNG LIKE '%' + @TEN_NV_BAN + '%')  
		AND (@TEN_NV_TT IS NULL OR @TEN_NV_TT = '' OR NV_TT.TEN_NGUOI_DUNG LIKE '%' + @TEN_NV_TT + '%')
		AND (@FROM_DATE IS NULL OR CAST(NGAY_BAN AS DATE) >= CAST(@FROM_DATE AS DATE))
		AND (@TO_DATE IS NULL OR CAST(NGAY_BAN AS DATE) <= CAST(@TO_DATE AS DATE))
		AND (@STATUS = 0 OR STATUS = @STATUS)
		AND (@MA_KHACH_HANG = 0 OR HOA_DON.MA_KHACH_HANG = @MA_KHACH_HANG)
		AND (@MA_NHAN_VIEN_BAN = 0 OR HOA_DON.MA_NHAN_VIEN_BAN = @MA_NHAN_VIEN_BAN)
		AND (@MA_NHAN_VIEN_TT = 0 OR HOA_DON.MA_NHAN_VIEN_THU_TIEN = @MA_NHAN_VIEN_TT)	
		AND (@MA_KHU_VUC = 0 OR KHACH_HANG.MA_KHU_VUC = @MA_KHU_VUC)
		AND (@TEN_KHU_VUC IS NULL OR @TEN_KHU_VUC = '' OR KHU_VUC.TEN_KHU_VUC LIKE '%' + @TEN_KHU_VUC + '%')		
	GROUP BY 
		CASE WHEN HOA_DON.MA_KHACH_HANG IS NULL
					THEN HOA_DON.TEN_KHACH_HANG
				ELSE
					KHACH_HANG.TEN_KHACH_HANG 
		END
	)TEMP
	WHERE 
	(@TEN_KHACH_HANG IS NULL OR @TEN_KHACH_HANG = '' OR TEN_KHACH_HANG LIKE '%' + @TEN_KHACH_HANG + '%')
END 

CREATE VIEW V_BUGET_BY_MONTH
AS
SELECT 
ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) SO_TIEN_KHACH_TRA, 
ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) SO_TIEN_NO_GOI_DAU,
ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) + ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) TOTAL,
MONTH(HOA_DON.NGAY_BAN) MONTH, 
YEAR(HOA_DON.NGAY_BAN) YEAR
FROM
HOA_DON
WHERE 
ACTIVE = 'A'
GROUP BY 
MONTH(HOA_DON.NGAY_BAN),
YEAR(HOA_DON.NGAY_BAN) 


CREATE VIEW V_BUGET_BY_WEEK
AS
SELECT 
ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) SO_TIEN_KHACH_TRA, 
ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) SO_TIEN_NO_GOI_DAU,
ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0)  +  ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) TOTAL,
DATEPART(wk, HOA_DON.NGAY_BAN) WEEK, 
MONTH(HOA_DON.NGAY_BAN) MONTH, 
YEAR(HOA_DON.NGAY_BAN) YEAR,
DATEADD(wk, DATEDIFF(wk, 0, HOA_DON.NGAY_BAN), 0) START_DATE, 
DATEADD(wk, DATEDIFF(wk, 0, HOA_DON.NGAY_BAN), 6) END_DATE
FROM 
HOA_DON
WHERE
HOA_DON.ACTIVE = 'A'
GROUP BY 
DATEPART(wk, HOA_DON.NGAY_BAN),
DATEADD(wk, DATEDIFF(wk, 0, HOA_DON.NGAY_BAN), 0) ,
DATEADD(wk, DATEDIFF(wk, 0, HOA_DON.NGAY_BAN), 6), 
MONTH(HOA_DON.NGAY_BAN), 
YEAR(HOA_DON.NGAY_BAN)

SELECT 
	TRA_HANG.MA_TRA_HANG, 
	TRA_HANG.TEN_KHACH_HANG, 
	TRA_HANG.NGAY_TRA, 
	TRA_HANG.NHAN_VIEN_NHAN MA_NGUOI_NHAN,
	SUM(ISNULL(CHI_TIET_TRA_HANG.SO_LUONG_TRA,0)*ISNULL(CHI_TIET_TRA_HANG.GIA_VON,0)) TOTAL
FROM 
TRA_HANG, 
CHI_TIET_TRA_HANG
WHERE
TRA_HANG.MA_TRA_HANG = CHI_TIET_TRA_HANG.MA_TRA_HANG
GROUP BY 
TRA_HANG.MA_TRA_HANG, 
TRA_HANG.TEN_KHACH_HANG, 
TRA_HANG.NGAY_TRA, 
TRA_HANG.NHAN_VIEN_NHAN