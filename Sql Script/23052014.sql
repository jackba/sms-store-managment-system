CREATE PROCEDURE SP_GET_HOA_DON_INFO
@MA_HOA_DON INT
AS
BEGIN
	SELECT 
		HOA_DON.MA_HOA_DON, 
		HOA_DON.SO_HOA_DON, 
		HOA_DON.MA_KHACH_HANG, 
		CASE WHEN HOA_DON.MA_KHACH_HANG IS NULL
				THEN HOA_DON.TEN_KHACH_HANG
			ELSE
				KHACH_HANG.TEN_KHACH_HANG 
		END AS TEN_KHACH_HANG,
		HOA_DON.MA_NHAN_VIEN_BAN, 
		NV_BAN.TEN_NGUOI_DUNG AS TEN_NGUOI_BAN,
		HOA_DON.NGAY_BAN, 
		HOA_DON.NGAY_GIAO,
		HOA_DON.DIA_CHI_GIAO_HANG,
		HOA_DON.STATUS, 
		HOA_DON.SO_TIEN_KHACH_TRA, 
		HOA_DON.SO_TIEN_NO_GOI_DAU, 
		HOA_DON.MA_NHAN_VIEN_THU_TIEN, 
		NV_TT.TEN_NGUOI_DUNG TEN_NV_TT
	FROM 
		HOA_DON
		LEFT JOIN KHACH_HANG ON HOA_DON.MA_KHACH_HANG = KHACH_HANG.MA_KHACH_HANG
		JOIN NGUOI_DUNG NV_BAN ON HOA_DON.MA_NHAN_VIEN_BAN = NV_BAN.MA_NGUOI_DUNG
		LEFT JOIN NGUOI_DUNG NV_TT ON HOA_DON.MA_NHAN_VIEN_THU_TIEN = NV_TT.MA_NGUOI_DUNG
	WHERE
		HOA_DON.ACTIVE = 'A'	
END 

ALTER VIEW [dbo].[V_HOA_DON]
AS
SELECT 
	HOA_DON.MA_HOA_DON, 
	HOA_DON.SO_HOA_DON, 
	HOA_DON.MA_KHACH_HANG, 
	CASE 
		WHEN HOA_DON.MA_KHACH_HANG IS NULL 
			THEN HOA_DON.TEN_KHACH_HANG
		ELSE
			KHACH_HANG.TEN_KHACH_HANG
		END AS TEN_KHACH_HANG, 
	ISNULL(HOA_DON.MA_NHAN_VIEN_BAN, 0) MA_NHAN_VIEN_BAN,
	ISNULL(NGUOI_DUNG.TEN_NGUOI_DUNG, '') TEN_NHAN_VIEN_BAN,
	HOA_DON.NGAY_BAN, 
	HOA_DON.NGAY_GIAO, 
	HOA_DON.DIA_CHI_GIAO_HANG, 
	ISNULL(HOA_DON.MA_NHAN_VIEN_THU_TIEN, 0) MA_NHAN_VIEN_THU_TIEN,
	ISNULL(NHAN_VIEN_TT.TEN_NGUOI_DUNG, '') TEN_NV_THU_TIEN,
	HOA_DON.STATUS,
	ISNULL(HOA_DON.SO_TIEN_KHACH_TRA, 0) SO_TIEN_KHACH_TRA, 
	ISNULL(HOA_DON.SO_TIEN_NO_GOI_DAU, 0) SO_TIEN_NO_GOI_DAU, 
	CHI_TIET_HOA_DON.MA_SAN_PHAM, 
	SAN_PHAM.TEN_SAN_PHAM,
	ISNULL(CHI_TIET_HOA_DON.SO_LUONG, 0) SO_LUONG, 
	ISNULL(CHI_TIET_HOA_DON.DON_GIA, 0) DON_GIA, 
	ISNULL(CHI_TIET_HOA_DON.PHAN_TRAM_CHIEC_KHAU, 0) PHAN_TRAM_CHIEC_KHAU, 
	CHI_TIET_HOA_DON.MA_KHO_XUAT, 
	DON_VI_TINH.TEN_DON_VI
FROM 
	HOA_DON
	LEFT JOIN KHACH_HANG ON HOA_DON.MA_KHACH_HANG = KHACH_HANG.MA_KHACH_HANG
	LEFT JOIN NGUOI_DUNG ON  HOA_DON.MA_NHAN_VIEN_BAN = NGUOI_DUNG.MA_NGUOI_DUNG
	LEFT JOIN NGUOI_DUNG NHAN_VIEN_TT ON HOA_DON.MA_NHAN_VIEN_THU_TIEN = NHAN_VIEN_TT.MA_NGUOI_DUNG,
	CHI_TIET_HOA_DON, 
	SAN_PHAM, 
	DON_VI_TINH
WHERE
	HOA_DON.ACTIVE = 'A'
	AND CHI_TIET_HOA_DON.ACTIVE = 'A'
	AND HOA_DON.MA_HOA_DON = CHI_TIET_HOA_DON.MA_HOA_DON
	AND CHI_TIET_HOA_DON.MA_SAN_PHAM = SAN_PHAM.MA_SAN_PHAM
	AND SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
GO

