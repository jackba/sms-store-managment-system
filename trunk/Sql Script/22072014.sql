ALTER PROCEDURE SP_GET_EXPORT_4_CANCEL
@MA_KHO INT, 
@TEN_KHO NVARCHAR(100), 
@MA_NHAN_VIEN INT, 
@TEN_NHAN_VIEN NVARCHAR(100), 
@FROM_DATE DATE, 
@TO_DATE DATE
AS
BEGIN
	SELECT 
		XUAT_KHO.MA_XUAT_KHO, 
		XUAT_KHO.GHI_CHU,
		XUAT_KHO.NGAY_XUAT,
		XUAT_KHO.MA_NHAN_VIEN_XUAT, 
		NGUOI_DUNG.TEN_NGUOI_DUNG, 
		XUAT_KHO.MA_KHO_XUAT, 
		KHO.TEN_KHO
	FROM 
		XUAT_KHO,
		NGUOI_DUNG,
		KHO
	WHERE 
		XUAT_KHO.ACTIVE = 'A'
		AND XUAT_KHO.LY_DO_XUAT = 1
		AND (@MA_KHO = 0 OR XUAT_KHO.MA_KHO_XUAT = @MA_KHO)
		AND (@TEN_KHO IS NULL OR @TEN_KHO = '' OR KHO.TEN_KHO LIKE '%' + @TEN_KHO + '%')
		AND (@MA_NHAN_VIEN = 0 OR XUAT_KHO.MA_NHAN_VIEN_XUAT =  @MA_NHAN_VIEN)
		AND (@TEN_NHAN_VIEN IS NULL OR @TEN_NHAN_VIEN = '' OR NGUOI_DUNG.TEN_NGUOI_DUNG LIKE '%' + @TEN_NHAN_VIEN + '%')
		AND (@FROM_DATE IS NULL OR CAST(XUAT_KHO.NGAY_XUAT AS DATE) >= CAST(@FROM_DATE AS DATE))
		AND (@TO_DATE IS NULL OR CAST(XUAT_KHO.NGAY_XUAT AS DATE) <= CAST(@TO_DATE AS DATE))
		AND XUAT_KHO.MA_NHAN_VIEN_XUAT = NGUOI_DUNG.MA_NGUOI_DUNG
		AND XUAT_KHO.MA_KHO_XUAT = KHO.MA_KHO
	ORDER BY XUAT_KHO.NGAY_XUAT
END

