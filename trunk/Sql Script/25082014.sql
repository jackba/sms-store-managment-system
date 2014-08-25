ALTER PROCEDURE [dbo].[SP_REPORT_BY_DAY_AND_AREA]
@START_TIME DATE, 
@END_TIME DATE, 
@MA_KHU_VUC INT, 
@TEN_KHU_VUC NVARCHAR(200)
AS
BEGIN
	SELECT 
	ISNULL(SALES.SO_TIEN_KHACH_TRA,0) SO_TIEN_KHACH_TRA,
	ISNULL(SALES.SO_TIEN_NO_GOI_DAU,0) SO_TIEN_NO_GOI_DAU, 
	ISNULL(SALES.TOTAL,0) AS BUGET_TOTAL,  
	ISNULL(RETURN_TABLE.TOTAL,0) AS RETURN_TOTAL,
	ISNULL(SALES.TOTAL,0) - ISNULL(RETURN_TABLE.TOTAL,0) AS TOTAL,
	ISNULL(SALES.NGAY_BAN, RETURN_TABLE.NGAY_TRA) DAY
	FROM
	(
		SELECT 
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) SO_TIEN_KHACH_TRA, 
		ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) SO_TIEN_NO_GOI_DAU,
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) + ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) TOTAL,
		HOA_DON.NGAY_BAN
		FROM
		HOA_DON
		JOIN KHACH_HANG ON HOA_DON.MA_KHACH_HANG = KHACH_HANG.MA_KHACH_HANG
		JOIN KHU_VUC ON KHACH_HANG.MA_KHU_VUC = KHU_VUC.MA_KHU_VUC
		WHERE 
		KHACH_HANG.ACTIVE = 'A'
		AND (@MA_KHU_VUC = 0 OR KHU_VUC.MA_KHU_VUC = @MA_KHU_VUC)
		AND (@TEN_KHU_VUC IS NULL OR @TEN_KHU_VUC = '' OR UPPER(KHU_VUC.TEN_KHU_VUC) LIKE '%' + UPPER(@TEN_KHU_VUC) + '%')
		AND (@START_TIME IS NULL OR CAST(HOA_DON.NGAY_BAN AS DATE) >= CAST(@START_TIME AS DATE))
		AND (@END_TIME IS NULL OR CAST(HOA_DON.NGAY_BAN AS DATE) <= CAST(@END_TIME AS DATE))
		GROUP BY 
		HOA_DON.NGAY_BAN
	) SALES FULL JOIN
	(
		SELECT
		SUM(ISNULL(V_TRA_HANG.TOTAL,0)) TOTAL,
		V_TRA_HANG.NGAY_TRA
		FROM 
		V_TRA_HANG
		JOIN KHACH_HANG ON V_TRA_HANG.MA_KHACH_HANG = KHACH_HANG.MA_KHACH_HANG
		JOIN KHU_VUC ON KHACH_HANG.MA_KHU_VUC = KHU_VUC.MA_KHU_VUC
		WHERE 
		(@START_TIME IS NULL OR CAST(V_TRA_HANG.NGAY_TRA AS DATE) >= CAST(@START_TIME AS DATE))
		AND (@END_TIME IS NULL OR CAST(V_TRA_HANG.NGAY_TRA AS DATE) <= CAST(@END_TIME AS DATE))
		AND (@MA_KHU_VUC = 0 OR KHU_VUC.MA_KHU_VUC = @MA_KHU_VUC)
		AND (@TEN_KHU_VUC IS NULL OR @TEN_KHU_VUC = '' OR UPPER(KHU_VUC.TEN_KHU_VUC) LIKE '%' + UPPER(@TEN_KHU_VUC) + '%')
		GROUP BY 
		V_TRA_HANG.NGAY_TRA
	)RETURN_TABLE
	ON SALES.NGAY_BAN = RETURN_TABLE.NGAY_TRA
	ORDER BY SALES.NGAY_BAN DESC
END
