ALTER TABLE TRA_HANG
ADD
	IMPORT_FLG BIT, 
	RETURN_FLG BIT

ALTER VIEW V_MA_TRA_HANG
AS
SELECT DISTINCT
TRA_HANG.MA_TRA_HANG
FROM
(
	SELECT 
	TRA_HANG.MA_TRA_HANG, 
	CHI_TIET_TRA_HANG.MA_SAN_PHAM, 
	CHI_TIET_TRA_HANG.SO_LUONG_TRA
	FROM
	TRA_HANG,
	CHI_TIET_TRA_HANG
	WHERE
	TRA_HANG.ACTIVE = 'A'
	AND CHI_TIET_TRA_HANG.ACTIVE = 'A'
	AND (TRA_HANG.IMPORT_FLG IS NULL OR TRA_HANG.IMPORT_FLG = 'FALSE' OR TRA_HANG.RETURN_FLG IS NULL OR TRA_HANG.RETURN_FLG = 'FALSE')
	AND TRA_HANG.MA_TRA_HANG = CHI_TIET_TRA_HANG.MA_TRA_HANG
)TRA_HANG
LEFT JOIN
(
	SELECT 
	NHAP_KHO.MA_PHIEU_TRA, 
	CHI_TIET_NHAP_KHO.MA_SAN_PHAM, 
	CHI_TIET_NHAP_KHO.SO_LUONG
	FROM
	NHAP_KHO, 
	CHI_TIET_NHAP_KHO
	WHERE
	NHAP_KHO.ACTIVE = 'A'
	AND CHI_TIET_NHAP_KHO.ACTIVE = 'A'
	AND NHAP_KHO.MA_PHIEU_TRA IS NOT NULL
	AND NHAP_KHO.MA_NHAP_KHO = CHI_TIET_NHAP_KHO.MA_NHAP_KHO
)NHAP_KHO ON  TRA_HANG.MA_TRA_HANG = NHAP_KHO.MA_PHIEU_TRA AND TRA_HANG.MA_SAN_PHAM = NHAP_KHO.MA_SAN_PHAM
LEFT JOIN 
(
	SELECT 
	TRA_HANG_NCC.MA_PHIEU_TRA, 
	TRA_HANG_NCC_CHI_TIET.MA_SAN_PHAM, 
	TRA_HANG_NCC_CHI_TIET.SO_LUONG
	FROM 
	TRA_HANG_NCC, 
	TRA_HANG_NCC_CHI_TIET
	WHERE
	TRA_HANG_NCC.ACTIVE = 'A'
	AND TRA_HANG_NCC_CHI_TIET.ACTIVE = 'A'
	AND TRA_HANG_NCC.MA_PHIEU_TRA IS NOT NULL
	AND TRA_HANG_NCC.ID = TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC
)TRA_HANG_NCC ON TRA_HANG.MA_TRA_HANG = TRA_HANG_NCC.MA_PHIEU_TRA AND TRA_HANG.MA_SAN_PHAM = TRA_HANG_NCC.MA_SAN_PHAM
WHERE ISNULL(TRA_HANG.SO_LUONG_TRA,0) > (ISNULL(NHAP_KHO.SO_LUONG,0) + ISNULL(TRA_HANG_NCC.SO_LUONG,0))



ALTER PROCEDURE SP_GET_RETURN_LIST
@TEN_KHACH_HANG NVARCHAR(100), 
@FROM_DATE DATE, 
@TO_DATE DATE,
@MA_NHAN_VIEN_NHAN INT, 
@TEN_NHAN_VIEN NVARCHAR(100)
AS
BEGIN
	SELECT 
		TRA_HANG.MA_TRA_HANG, 
		TRA_HANG.TEN_KHACH_HANG, 
		TRA_HANG.NGAY_TRA, 
		TRA_HANG.STATUS,  
		TRA_HANG.NHAN_VIEN_NHAN,
		TRA_HANG.IMPORT_FLG, 
		TRA_HANG.RETURN_FLG,
		NGUOI_DUNG.TEN_NGUOI_DUNG
	FROM
		TRA_HANG, 
		NGUOI_DUNG
	WHERE
		TRA_HANG.ACTIVE = 'A'
		AND TRA_HANG.MA_TRA_HANG IN (SELECT MA_TRA_HANG FROM V_MA_TRA_HANG)
		AND (TRA_HANG.IMPORT_FLG IS NULL OR TRA_HANG.IMPORT_FLG = 'FALSE'
				OR TRA_HANG.RETURN_FLG IS NULL OR TRA_HANG.RETURN_FLG = 'FALSE')
		AND (@TEN_KHACH_HANG IS NULL OR @TEN_KHACH_HANG = '' OR TRA_HANG.TEN_KHACH_HANG LIKE '%' +  @TEN_KHACH_HANG + '%')
		AND (@TEN_NHAN_VIEN IS NULL OR @TEN_NHAN_VIEN = '' OR NGUOI_DUNG.TEN_NGUOI_DUNG LIKE '%' + @TEN_NHAN_VIEN + '%')
		AND (@FROM_DATE IS NULL OR CAST(NGAY_TRA AS DATE) >= CAST(@FROM_DATE AS DATE))
		AND (@TO_DATE IS NULL OR CAST(NGAY_TRA AS DATE) <= CAST(@TO_DATE AS DATE))
		AND (@MA_NHAN_VIEN_NHAN = 0 OR TRA_HANG.NHAN_VIEN_NHAN = @MA_NHAN_VIEN_NHAN)
		AND TRA_HANG.NHAN_VIEN_NHAN = NGUOI_DUNG.MA_NGUOI_DUNG
END



ALTER VIEW V_TRA_NHAP_TRA_NCC
AS
SELECT 
TRA_HANG.MA_TRA_HANG, 
TRA_HANG.MA_SAN_PHAM, 
ISNULL(TRA_HANG.SO_LUONG_TRA, 0) - ISNULL(NHAP_KHO.SO_LUONG,0) - ISNULL(TRA_HANG_NCC.SO_LUONG,0) SO_LUONG
FROM
(
	SELECT 
	TRA_HANG.MA_TRA_HANG, 
	CHI_TIET_TRA_HANG.MA_SAN_PHAM, 
	CHI_TIET_TRA_HANG.SO_LUONG_TRA
	FROM
	TRA_HANG,
	CHI_TIET_TRA_HANG
	WHERE
	TRA_HANG.ACTIVE = 'A'
	AND CHI_TIET_TRA_HANG.ACTIVE = 'A'
	AND (TRA_HANG.IMPORT_FLG IS NULL OR TRA_HANG.IMPORT_FLG = 'FALSE' OR TRA_HANG.RETURN_FLG IS NULL OR TRA_HANG.RETURN_FLG = 'FALSE')
	AND TRA_HANG.MA_TRA_HANG = CHI_TIET_TRA_HANG.MA_TRA_HANG
)TRA_HANG
LEFT JOIN
(
	SELECT 
	NHAP_KHO.MA_PHIEU_TRA, 
	CHI_TIET_NHAP_KHO.MA_SAN_PHAM, 
	CHI_TIET_NHAP_KHO.SO_LUONG
	FROM
	NHAP_KHO, 
	CHI_TIET_NHAP_KHO
	WHERE
	NHAP_KHO.ACTIVE = 'A'
	AND CHI_TIET_NHAP_KHO.ACTIVE = 'A'
	AND NHAP_KHO.MA_PHIEU_TRA IS NOT NULL
	AND NHAP_KHO.MA_NHAP_KHO = CHI_TIET_NHAP_KHO.MA_NHAP_KHO
)NHAP_KHO ON  TRA_HANG.MA_TRA_HANG = NHAP_KHO.MA_PHIEU_TRA AND TRA_HANG.MA_SAN_PHAM = NHAP_KHO.MA_SAN_PHAM
LEFT JOIN 
(
	SELECT 
	TRA_HANG_NCC.MA_PHIEU_TRA, 
	TRA_HANG_NCC_CHI_TIET.MA_SAN_PHAM, 
	TRA_HANG_NCC_CHI_TIET.SO_LUONG
	FROM 
	TRA_HANG_NCC, 
	TRA_HANG_NCC_CHI_TIET
	WHERE
	TRA_HANG_NCC.ACTIVE = 'A'
	AND TRA_HANG_NCC_CHI_TIET.ACTIVE = 'A'
	AND TRA_HANG_NCC.MA_PHIEU_TRA IS NOT NULL
	AND TRA_HANG_NCC.ID = TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC
)TRA_HANG_NCC ON TRA_HANG.MA_TRA_HANG = TRA_HANG_NCC.MA_PHIEU_TRA AND TRA_HANG.MA_SAN_PHAM = TRA_HANG_NCC.MA_SAN_PHAM
WHERE ISNULL(TRA_HANG.SO_LUONG_TRA,0) > (ISNULL(NHAP_KHO.SO_LUONG,0) + ISNULL(TRA_HANG_NCC.SO_LUONG,0))



ALTER PROCEDURE SP_GET_REFUND_DETAIL
@MA_TRA_HANG INT
AS
BEGIN
	SELECT 
	V_TRA_NHAP_TRA_NCC.MA_TRA_HANG, 
	V_TRA_NHAP_TRA_NCC.MA_SAN_PHAM, 
	ISNULL(V_TRA_NHAP_TRA_NCC.SO_LUONG,0) SO_LUONG_TRA, 
	ISNULL(V_TRA_NHAP_TRA_NCC.SO_LUONG,0) SO_LUONG_TON, 
	ISNULL(CHI_TIET_TRA_HANG.GIA_VON,0) GIA_VON,
	ISNULL(V_TRA_NHAP_TRA_NCC.SO_LUONG, 0) * ISNULL(CHI_TIET_TRA_HANG.GIA_VON, 0) TONG_TIEN,
	SAN_PHAM.TEN_SAN_PHAM,
	DON_VI_TINH.TEN_DON_VI
	FROM 
	CHI_TIET_TRA_HANG,
	V_TRA_NHAP_TRA_NCC, 
	SAN_PHAM, 
	DON_VI_TINH
	WHERE
	CHI_TIET_TRA_HANG.ACTIVE = 'A'	
	AND CHI_TIET_TRA_HANG.MA_TRA_HANG = @MA_TRA_HANG
	AND CHI_TIET_TRA_HANG.MA_TRA_HANG = V_TRA_NHAP_TRA_NCC.MA_TRA_HANG
	AND CHI_TIET_TRA_HANG.MA_SAN_PHAM = V_TRA_NHAP_TRA_NCC.MA_SAN_PHAM
	AND CHI_TIET_TRA_HANG.MA_SAN_PHAM = SAN_PHAM.MA_SAN_PHAM 
	AND SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
END

