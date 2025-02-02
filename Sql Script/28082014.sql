
ALTER PROCEDURE [dbo].[SP_REPORT_BY_CUSTOMER]
@START_TIME DATE, 
@END_TIME DATE, 
@MA_KHACH_HANG INT, 
@TEN_KHACH_HANG NVARCHAR(200)
AS
BEGIN
	SELECT 
	ISNULL(SALES.TEN_KHACH_HANG, RETURN_TABLE.TEN_KHACH_HANG) TEN_KHACH_HANG,
	ISNULL(SALES.MA_KHACH_HANG, RETURN_TABLE.MA_KHACH_HANG) MA_KHACH_HANG, 
	ISNULL(SALES.MA_KHU_VUC, RETURN_TABLE.MA_KHU_VUC) MA_KHU_VUC, 
	ISNULL(SALES.TEN_KHU_VUC, RETURN_TABLE.TEN_KHU_VUC) TEN_KHU_VUC,
	ISNULL(SALES.SO_TIEN_KHACH_TRA,0) SO_TIEN_KHACH_TRA,
	ISNULL(SALES.SO_TIEN_NO_GOI_DAU,0) SO_TIEN_NO_GOI_DAU, 
	ISNULL(SALES.TOTAL,0) AS BUGET_TOTAL,  
	ISNULL(RETURN_TABLE.TOTAL,0) AS RETURN_TOTAL,
	ISNULL(SALES.TOTAL,0) - ISNULL(RETURN_TABLE.TOTAL,0) AS TOTAL
	FROM
	(
		SELECT 
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) SO_TIEN_KHACH_TRA, 
		ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) SO_TIEN_NO_GOI_DAU,
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) + ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) TOTAL,
		KHACH_HANG.TEN_KHACH_HANG, 
		KHACH_HANG.MA_KHACH_HANG, 
		KHU_VUC.TEN_KHU_VUC, 
		KHU_VUC.MA_KHU_VUC
		FROM
		HOA_DON
		JOIN KHACH_HANG ON HOA_DON.MA_KHACH_HANG = KHACH_HANG.MA_KHACH_HANG
		JOIN KHU_VUC ON KHACH_HANG.MA_KHU_VUC = KHU_VUC.MA_KHU_VUC
		WHERE 
		HOA_DON.ACTIVE = 'A'
		AND (@MA_KHACH_HANG = 0 OR KHACH_HANG.MA_KHACH_HANG = @MA_KHACH_HANG)
		AND (@TEN_KHACH_HANG IS NULL OR @TEN_KHACH_HANG = '' OR UPPER(KHACH_HANG.TEN_KHACH_HANG) LIKE '%' + UPPER(@TEN_KHACH_HANG) + '%')
		AND (@START_TIME IS NULL OR CAST(HOA_DON.NGAY_BAN AS DATE) >= CAST(@START_TIME AS DATE))
		AND (@END_TIME IS NULL OR CAST(HOA_DON.NGAY_BAN AS DATE) <= CAST(@END_TIME AS DATE))
		GROUP BY 
		KHACH_HANG.TEN_KHACH_HANG, 
		KHACH_HANG.MA_KHACH_HANG, 
		KHU_VUC.TEN_KHU_VUC, 
		KHU_VUC.MA_KHU_VUC
	) SALES FULL JOIN
	(
		SELECT
		SUM(ISNULL(V_TRA_HANG.TOTAL,0)) TOTAL,
		KHACH_HANG.MA_KHACH_HANG,
		KHACH_HANG.TEN_KHACH_HANG, 
		KHU_VUC.MA_KHU_VUC, 
		KHU_VUC.TEN_KHU_VUC
		FROM 
		V_TRA_HANG
		JOIN KHACH_HANG ON V_TRA_HANG.MA_KHACH_HANG = KHACH_HANG.MA_KHACH_HANG
		JOIN KHU_VUC ON KHACH_HANG.MA_KHU_VUC = KHU_VUC.MA_KHU_VUC
		WHERE 
		(@START_TIME IS NULL OR CAST(V_TRA_HANG.NGAY_TRA AS DATE) >= CAST(@START_TIME AS DATE))
		AND (@END_TIME IS NULL OR CAST(V_TRA_HANG.NGAY_TRA AS DATE) <= CAST(@END_TIME AS DATE))
		AND (@MA_KHACH_HANG = 0 OR KHACH_HANG.MA_KHACH_HANG = @MA_KHACH_HANG)
		AND (@TEN_KHACH_HANG IS NULL OR @TEN_KHACH_HANG = '' OR UPPER(KHACH_HANG.TEN_KHACH_HANG) LIKE '%' + UPPER(@TEN_KHACH_HANG) + '%')
		GROUP BY 
		KHACH_HANG.MA_KHACH_HANG,
		KHACH_HANG.TEN_KHACH_HANG, 
		KHU_VUC.MA_KHU_VUC, 
		KHU_VUC.TEN_KHU_VUC
	)RETURN_TABLE
	ON SALES.MA_KHACH_HANG = RETURN_TABLE.MA_KHACH_HANG
	
	
	UNION ALL	
	
	
	SELECT 
	ISNULL(SALES.TEN_KHACH_HANG, RETURN_TABLE.TEN_KHACH_HANG) TEN_KHACH_HANG,
	ISNULL(SALES.MA_KHACH_HANG, RETURN_TABLE.MA_KHACH_HANG) MA_KHACH_HANG, 
	ISNULL(SALES.MA_KHU_VUC, RETURN_TABLE.MA_KHU_VUC) MA_KHU_VUC, 
	ISNULL(SALES.TEN_KHU_VUC, RETURN_TABLE.TEN_KHU_VUC) TEN_KHU_VUC,
	ISNULL(SALES.SO_TIEN_KHACH_TRA,0) SO_TIEN_KHACH_TRA,
	ISNULL(SALES.SO_TIEN_NO_GOI_DAU,0) SO_TIEN_NO_GOI_DAU, 
	ISNULL(SALES.TOTAL,0) AS BUGET_TOTAL,  
	ISNULL(RETURN_TABLE.TOTAL,0) AS RETURN_TOTAL,
	ISNULL(SALES.TOTAL,0) - ISNULL(RETURN_TABLE.TOTAL,0) AS TOTAL
	FROM
	(
		SELECT 
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) SO_TIEN_KHACH_TRA, 
		ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) SO_TIEN_NO_GOI_DAU,
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) + ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) TOTAL,
		HOA_DON.TEN_KHACH_HANG, 
		-1 MA_KHACH_HANG, 
		N'KHÁCH VÃNG LAI' TEN_KHU_VUC, 
		-1 MA_KHU_VUC
		FROM
		HOA_DON
		WHERE 
		HOA_DON.ACTIVE = 'A'
		AND (HOA_DON.MA_KHACH_HANG IS NULL)
		AND (@MA_KHACH_HANG = 0)
		AND (@TEN_KHACH_HANG IS NULL OR @TEN_KHACH_HANG = '' OR UPPER(HOA_DON.TEN_KHACH_HANG) LIKE '%' + UPPER(@TEN_KHACH_HANG) + '%')
		AND (@START_TIME IS NULL OR CAST(HOA_DON.NGAY_BAN AS DATE) >= CAST(@START_TIME AS DATE))
		AND (@END_TIME IS NULL OR CAST(HOA_DON.NGAY_BAN AS DATE) <= CAST(@END_TIME AS DATE))
		GROUP BY 
		HOA_DON.TEN_KHACH_HANG
	) SALES FULL JOIN
	(
		SELECT
		SUM(ISNULL(V_TRA_HANG.TOTAL,0)) TOTAL,
		V_TRA_HANG.TEN_KHACH_HANG, 
		-1 MA_KHACH_HANG, 
		N'KHÁCH VÃNG LAI' TEN_KHU_VUC, 
		-1 MA_KHU_VUC
		FROM 
		V_TRA_HANG
		WHERE 
		(@START_TIME IS NULL OR CAST(V_TRA_HANG.NGAY_TRA AS DATE) >= CAST(@START_TIME AS DATE))
		AND (@END_TIME IS NULL OR CAST(V_TRA_HANG.NGAY_TRA AS DATE) <= CAST(@END_TIME AS DATE))
		AND V_TRA_HANG.MA_KHACH_HANG IS NULL
		AND (@MA_KHACH_HANG = 0)
		AND (@TEN_KHACH_HANG IS NULL OR @TEN_KHACH_HANG = '' OR UPPER(V_TRA_HANG.TEN_KHACH_HANG) LIKE '%' + UPPER(@TEN_KHACH_HANG) + '%')
		GROUP BY 
		V_TRA_HANG.TEN_KHACH_HANG
	)RETURN_TABLE
	ON SALES.MA_KHACH_HANG = RETURN_TABLE.MA_KHACH_HANG
	ORDER BY 
	MA_KHU_VUC DESC,
	TOTAL DESC,
	MA_KHACH_HANG DESC	
END




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
	ISNULL(SALES.NGAY_BAN, RETURN_TABLE.NGAY_TRA) DAY,
	ISNULL(SALES.MA_KHU_VUC, RETURN_TABLE.MA_KHU_VUC) MA_KHU_VUC,
	ISNULL(SALES.TEN_KHU_VUC,RETURN_TABLE.TEN_KHU_VUC) TEN_KHU_VUC
	FROM
	(
		SELECT 
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) SO_TIEN_KHACH_TRA, 
		ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) SO_TIEN_NO_GOI_DAU,
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) + ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) TOTAL,
		HOA_DON.NGAY_BAN, 
		KHU_VUC.MA_KHU_VUC, 
		KHU_VUC.TEN_KHU_VUC
		FROM
		HOA_DON
		JOIN KHACH_HANG ON HOA_DON.MA_KHACH_HANG = KHACH_HANG.MA_KHACH_HANG
		JOIN KHU_VUC ON KHACH_HANG.MA_KHU_VUC = KHU_VUC.MA_KHU_VUC
		WHERE 
		HOA_DON.ACTIVE = 'A'
		AND (@MA_KHU_VUC = 0 OR KHU_VUC.MA_KHU_VUC = @MA_KHU_VUC)
		AND (@TEN_KHU_VUC IS NULL OR @TEN_KHU_VUC = '' OR UPPER(KHU_VUC.TEN_KHU_VUC) LIKE '%' + UPPER(@TEN_KHU_VUC) + '%')
		AND (@START_TIME IS NULL OR CAST(HOA_DON.NGAY_BAN AS DATE) >= CAST(@START_TIME AS DATE))
		AND (@END_TIME IS NULL OR CAST(HOA_DON.NGAY_BAN AS DATE) <= CAST(@END_TIME AS DATE))
		GROUP BY 
		HOA_DON.NGAY_BAN, 
		KHU_VUC.MA_KHU_VUC, 
		KHU_VUC.TEN_KHU_VUC
	) SALES FULL JOIN
	(
		SELECT
		SUM(ISNULL(V_TRA_HANG.TOTAL,0)) TOTAL,
		V_TRA_HANG.NGAY_TRA, 
		KHU_VUC.MA_KHU_VUC, 
		KHU_VUC.TEN_KHU_VUC
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
		V_TRA_HANG.NGAY_TRA, 
		KHU_VUC.MA_KHU_VUC, 
		KHU_VUC.TEN_KHU_VUC
	)RETURN_TABLE
	ON SALES.NGAY_BAN = RETURN_TABLE.NGAY_TRA
	UNION ALL
	SELECT 
	ISNULL(SALES.SO_TIEN_KHACH_TRA,0) SO_TIEN_KHACH_TRA,
	ISNULL(SALES.SO_TIEN_NO_GOI_DAU,0) SO_TIEN_NO_GOI_DAU, 
	ISNULL(SALES.TOTAL,0) AS BUGET_TOTAL,  
	ISNULL(RETURN_TABLE.TOTAL,0) AS RETURN_TOTAL,
	ISNULL(SALES.TOTAL,0) - ISNULL(RETURN_TABLE.TOTAL,0) AS TOTAL,
	ISNULL(SALES.NGAY_BAN, RETURN_TABLE.NGAY_TRA) DAY,
	-1 MA_KHU_VUC,
	N'KHÁCH VÃNG LAI' TEN_KHU_VUC
	FROM
	(
		SELECT 
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) SO_TIEN_KHACH_TRA, 
		ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) SO_TIEN_NO_GOI_DAU,
		ISNULL(SUM(HOA_DON.SO_TIEN_KHACH_TRA),0) + ISNULL(SUM(HOA_DON.SO_TIEN_NO_GOI_DAU),0) TOTAL,
		HOA_DON.NGAY_BAN
		FROM
		HOA_DON
		WHERE 
		HOA_DON.ACTIVE = 'A'
		AND (HOA_DON.MA_KHACH_HANG IS NULL)
		AND (@MA_KHU_VUC = 0)
		AND (@TEN_KHU_VUC IS NULL  OR @TEN_KHU_VUC = '')
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
		WHERE 
		(@START_TIME IS NULL OR CAST(V_TRA_HANG.NGAY_TRA AS DATE) >= CAST(@START_TIME AS DATE))
		AND (@END_TIME IS NULL OR CAST(V_TRA_HANG.NGAY_TRA AS DATE) <= CAST(@END_TIME AS DATE))
		AND (@MA_KHU_VUC = 0)
		AND (@TEN_KHU_VUC IS NULL OR @TEN_KHU_VUC = '')
		AND (V_TRA_HANG.MA_KHACH_HANG IS NULL OR V_TRA_HANG.MA_KHACH_HANG  = 0)
		GROUP BY 
		V_TRA_HANG.NGAY_TRA
	)RETURN_TABLE
	ON SALES.NGAY_BAN = RETURN_TABLE.NGAY_TRA
	ORDER BY MA_KHU_VUC DESC, DAY DESC
END

