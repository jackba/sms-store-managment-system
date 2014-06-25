CREATE PROCEDURE SP_GET_PHIEU_CHUYEN_KHO 
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
		NGUOI_DUNG.TEN_NGUOI_DUNG TEN_NHAN_VIEN_XUAT
	FROM
		XUAT_KHO, 
		KHO KHO_XUAT, 
		KHO KHO_NHAN, 
		NGUOI_DUNG
	WHERE
		XUAT_KHO.LY_DO_XUAT = 3 -- XUẤT CHUYỂN KHO
		AND (@MA_KHO_CHUYEN = 0 OR XUAT_KHO.MA_KHO_XUAT = @MA_KHO_CHUYEN)
		AND (@MA_KHO_NHAN = 0 OR XUAT_KHO.MA_KHO_NHAN = @MA_KHO_NHAN)
		AND (@FROM_DATE IS NULL OR CAST(NGAY_XUAT AS DATE) <= CAST(@FROM_DATE AS DATE))
		AND (@TO_DATE IS NULL OR CAST(NGAY_XUAT AS DATE) >= CAST(@TO_DATE AS DATE))
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
					AND XUAT_KHO.MA_XUAT_KHO NOT IN (SELECT MA_PHIEU_XUAT FROM  NHAP_KHO WHERE NHAP_KHO.ACTIVE = 'A')
				)
			)
		AND XUAT_KHO.MA_KHO_XUAT = KHO_XUAT.MA_KHO
		AND XUAT_KHO.MA_KHO_NHAN = KHO_NHAN.MA_KHO
		AND XUAT_KHO.MA_NHAN_VIEN_XUAT = NGUOI_DUNG.MA_NGUOI_DUNG
END