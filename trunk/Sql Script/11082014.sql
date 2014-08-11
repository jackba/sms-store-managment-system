
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