CREATE PROCEDURE SP_GET_HOA_DON_BH
@FROM_DATE DATE, 
@TO_DATE DATE, 
@MA_KHACH_HANG INT, 
@TEN_KHACH_HANG NVARCHAR(100),
@MA_NHAN_VIEN_BAN INT, 
@MA_NHAN_VIEN_TT INT, 
@STATUS INT
AS
BEGIN
	SELECT TEMP.* FROM
	(SELECT 
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
		AND (@FROM_DATE IS NULL OR CAST(NGAY_BAN AS DATE) >= CAST(@FROM_DATE AS DATE))
		AND (@TO_DATE IS NULL OR CAST(NGAY_BAN AS DATE) <= CAST(@TO_DATE AS DATE))
		AND (@STATUS = 0 OR STATUS = @STATUS)
		AND (@MA_KHACH_HANG = 0 OR HOA_DON.MA_KHACH_HANG = @MA_KHACH_HANG)
		AND (@MA_NHAN_VIEN_BAN = 0 OR HOA_DON.MA_NHAN_VIEN_BAN = @MA_NHAN_VIEN_BAN)
		AND (@MA_NHAN_VIEN_TT = 0 OR HOA_DON.MA_NHAN_VIEN_THU_TIEN = @MA_NHAN_VIEN_TT)		
	)TEMP
	WHERE 
	(@TEN_KHACH_HANG IS NULL OR TEN_KHACH_HANG = @TEN_KHACH_HANG)
END 