ALTER PROCEDURE SP_GET_PHIEU_CHUYEN_KHO 
@STATUS INT, 
@MA_KHO_CHUYEN INT, 
@MA_KHO_NHAN INT, 
@FROM_DATE DATE, 
@TO_DATE DATE, 
@MA_NHAN_VIEN_CHUYEN INT, 
@TEN_NHAN_VIEN_CHUYEN NVARCHAR(100)
AS
BEGIN
	SELECT
		XUAT_KHO.MA_XUAT_KHO, 
		XUAT_KHO.MA_KHO_XUAT, 
		KHO_XUAT.TEN_KHO TEN_KHO_XUAT,
		XUAT_KHO.MA_KHO_NHAN,
		KHO_NHAN.TEN_KHO TEN_KHO_NHAN,
		XUAT_KHO.GHI_CHU,
		XUAT_KHO.NGAY_XUAT,
		XUAT_KHO.ACTIVE, 
		XUAT_KHO.MA_NHAN_VIEN_XUAT, 
		NGUOI_DUNG.TEN_NGUOI_DUNG TEN_NHAN_VIEN_XUAT,
		CASE XUAT_KHO.ACTIVE 
			WHEN 'W' THEN 1
			WHEN 'A' THEN 
				CASE (SELECT COUNT(MA_PHIEU_XUAT) FROM  NHAP_KHO WHERE NHAP_KHO.ACTIVE = 'A' AND MA_PHIEU_XUAT  = XUAT_KHO.MA_XUAT_KHO)
				WHEN 0
					THEN 2
				ELSE
					 3 
				END
		END AS STATUS
	FROM
		XUAT_KHO, 
		KHO KHO_XUAT, 
		KHO KHO_NHAN, 
		NGUOI_DUNG
	WHERE
		XUAT_KHO.LY_DO_XUAT = 3 -- XUẤT CHUYỂN KHO
		AND (@MA_KHO_CHUYEN = 0 OR XUAT_KHO.MA_KHO_XUAT = @MA_KHO_CHUYEN)
		AND (@MA_KHO_NHAN = 0 OR XUAT_KHO.MA_KHO_NHAN = @MA_KHO_NHAN)
		AND (@FROM_DATE IS NULL OR CAST(NGAY_XUAT AS DATE) >= CAST(@FROM_DATE AS DATE))
		AND (@TO_DATE IS NULL OR CAST(NGAY_XUAT AS DATE) <= CAST(@TO_DATE AS DATE))
		AND (@MA_NHAN_VIEN_CHUYEN = 0 OR XUAT_KHO.MA_NHAN_VIEN_XUAT = @MA_NHAN_VIEN_CHUYEN)
		AND (@TEN_NHAN_VIEN_CHUYEN IS NULL OR @TEN_NHAN_VIEN_CHUYEN = '' OR NGUOI_DUNG.TEN_NGUOI_DUNG LIKE '%' + @TEN_NHAN_VIEN_CHUYEN + '%')
		AND (
				@STATUS = 0 OR
				(
					@STATUS = 1 
					AND XUAT_KHO.ACTIVE = 'W'
				) OR 
				(
					@STATUS = 2 
					AND XUAT_KHO.ACTIVE = 'A' 
					AND XUAT_KHO.MA_XUAT_KHO NOT IN (SELECT ISNULL(MA_PHIEU_XUAT,0) FROM  NHAP_KHO WHERE NHAP_KHO.ACTIVE = 'A')
				)
				OR 
				(
					@STATUS = 3
					AND XUAT_KHO.ACTIVE = 'A' 
					AND XUAT_KHO.MA_XUAT_KHO  IN (SELECT ISNULL(MA_PHIEU_XUAT,0) FROM  NHAP_KHO WHERE NHAP_KHO.ACTIVE = 'A')
				)
			)
		AND XUAT_KHO.MA_KHO_XUAT = KHO_XUAT.MA_KHO
		AND XUAT_KHO.MA_KHO_NHAN = KHO_NHAN.MA_KHO
		AND XUAT_KHO.MA_NHAN_VIEN_XUAT = NGUOI_DUNG.MA_NGUOI_DUNG
	ORDER BY XUAT_KHO.NGAY_XUAT DESC
END


EXEC SP_GET_PHIEU_CHUYEN_KHO  0, 0, 0, NULL, NULL, 0, NULL


alter PROCEDURE SP_GET_PHIEU_CHUYEN_KHO_INFO_BY_ID 
@MA_XUAT_KHO INT
AS
BEGIN
	SELECT TOP 1
		XUAT_KHO.MA_XUAT_KHO, 
		XUAT_KHO.MA_KHO_XUAT, 
		KHO_XUAT.TEN_KHO TEN_KHO_XUAT,
		XUAT_KHO.MA_KHO_NHAN,
		KHO_NHAN.TEN_KHO TEN_KHO_NHAN,
		XUAT_KHO.GHI_CHU,
		XUAT_KHO.NGAY_XUAT,
		XUAT_KHO.ACTIVE, 
		XUAT_KHO.MA_NHAN_VIEN_XUAT, 
		NGUOI_DUNG.TEN_NGUOI_DUNG TEN_NHAN_VIEN_XUAT,
		CASE XUAT_KHO.ACTIVE 
			WHEN 'W' THEN 1
			WHEN 'A' THEN 
				CASE (SELECT COUNT(MA_PHIEU_XUAT) FROM  NHAP_KHO WHERE NHAP_KHO.ACTIVE = 'A' AND MA_PHIEU_XUAT  = XUAT_KHO.MA_XUAT_KHO)
				WHEN 0
					THEN 2
				ELSE
					 3 
				END
		END AS STATUS
	FROM
		XUAT_KHO, 
		KHO KHO_XUAT, 
		KHO KHO_NHAN, 
		NGUOI_DUNG
	WHERE
		XUAT_KHO.LY_DO_XUAT = 3 -- XUẤT CHUYỂN KHO
		AND XUAT_KHO.MA_XUAT_KHO = @MA_XUAT_KHO
		AND XUAT_KHO.MA_KHO_XUAT = KHO_XUAT.MA_KHO
		AND XUAT_KHO.MA_KHO_NHAN = KHO_NHAN.MA_KHO
		AND XUAT_KHO.MA_NHAN_VIEN_XUAT = NGUOI_DUNG.MA_NGUOI_DUNG
END


EXEC SP_GET_PHIEU_CHUYEN_KHO  1, 0, 0, NULL, NULL, 0, null

alter FUNCTION F_GET_CONVERTOR
(
	@MA_SAN_PHAM INT, 
	@MA_DON_VI INT
)RETURNS FLOAT
AS
BEGIN
	DECLARE @RETURN_VALUE FLOAT;
	SELECT TOP 1 @RETURN_VALUE = ISNULL(HE_SO, 1) 
	FROM 
	CHUYEN_DOI_DON_VI_TINH
	WHERE 
	ACTIVE = 'A'
	AND MA_SAN_PHAN = @MA_SAN_PHAM
	AND MA_DON_VI_VAO = @MA_DON_VI
	IF @RETURN_VALUE IS NULL OR @RETURN_VALUE = 0
	BEGIN
		SET @RETURN_VALUE = 1
	END
	RETURN @RETURN_VALUE
END

ALTER TABLE CHI_TIET_HOA_DON
ADD
	GIA_BAN_TRUOC_CK FLOAT
	
	

SELECT DBO.F_GET_CONVERTOR(1,1)

CREATE PROCEDURE SP_GET_CHI_TIET_PHIEU_XUAT_CHUYEN
@MA_XUAT_KHO INT
AS
BEGIN
	SELECT 
	FROM
		XUAT_KHO,
		CHI_TIET_XUAT_KHO,
		SAN_PHAM
END