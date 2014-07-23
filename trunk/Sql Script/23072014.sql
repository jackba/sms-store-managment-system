ALTER PROCEDURE [dbo].[SP_GET_IMPORT_INFOR_BY_ID]
@MA_NHAP_KHO INT
AS
BEGIN
	SELECT 
		NHAP_KHO.MA_NHAP_KHO, 
		NHAP_KHO.SO_HOA_DON, 
		NHAP_KHO.NGAY_NHAP,
		NHAP_KHO.NHAN_VIEN_NHAP MA_NHAN_VIEN_NHAP,
		NGUOI_DUNG.TEN_NGUOI_DUNG TEN_NV_NHAP,
		NHAP_KHO.MA_NHA_CUNG_CAP, 
		NHA_CUNG_CAP.TEN_NHA_CUNG_CAP, 
		NHAP_KHO.MA_KHO,
		NHAP_KHO.LY_DO_NHAP, 
		NHAP_KHO.MA_PHIEU_TRA, 
		NHAP_KHO.GHI_CHU, 
		NHAP_KHO.MA_PHIEU_XUAT
	FROM
		NHAP_KHO
		LEFT JOIN NHA_CUNG_CAP ON NHAP_KHO.MA_NHA_CUNG_CAP = NHA_CUNG_CAP.MA_NHA_CUNG_CAP, 
		NGUOI_DUNG
	WHERE
		NHAP_KHO.MA_NHAP_KHO = @MA_NHAP_KHO
		AND NHAP_KHO.ACTIVE = 'A'
		AND NHAP_KHO.NHAN_VIEN_NHAP = NGUOI_DUNG.MA_NGUOI_DUNG
END

