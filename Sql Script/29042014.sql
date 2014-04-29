CREATE VIEW V_NHAP_XUAT_DETAIL
AS
SELECT 
ISNULL(A.MA_KHO, B.MA_KHO) MA_KHO, 
ISNULL(A.MA_SAN_PHAM,B.MA_SAN_PHAM) MA_SAN_PHAM, 
ISNULL(A.NGAY_NHAP, B.NGAY_XUAT) NGAY_NHAP_XUAT,
ISNULL(A.SO_LUONG_NHAP,0) SO_LUONG_NHAP, 
ISNULL(B.SO_LUONG_XUAT,0) SO_LUONG_XUAT
FROM
(
	SELECT 
		V_NHAP_KHO.MA_KHO MA_KHO,
		V_NHAP_KHO.MA_SAN_PHAM, 
		V_NHAP_KHO.NGAY_NHAP,
		ISNULL(SUM(V_NHAP_KHO.SO_LUONG), 0) AS SO_LUONG_NHAP
	FROM 
		V_NHAP_KHO 
	GROUP BY V_NHAP_KHO.MA_SAN_PHAM, V_NHAP_KHO.MA_KHO, V_NHAP_KHO.NGAY_NHAP
)A
FULL JOIN 
(
	SELECT 
		V_XUAT_KHO.MA_KHO_XUAT MA_KHO,
		V_XUAT_KHO.MA_SAN_PHAM, 
		V_XUAT_KHO.NGAY_XUAT,
		ISNULL(SUM(V_XUAT_KHO.SO_LUONG), 0) AS SO_LUONG_XUAT
	FROM 
		V_XUAT_KHO 
	GROUP BY V_XUAT_KHO.MA_SAN_PHAM, V_XUAT_KHO.MA_KHO_XUAT,V_XUAT_KHO.NGAY_XUAT
)B
ON A.MA_KHO = B.MA_KHO AND A.MA_SAN_PHAM = B.MA_SAN_PHAM


ALTER PROCEDURE SP_GET_NHAP_XUAT
@MA_KHO INT,
@MA_SAN_PHAM INT, 
@TEN_SAN_PHAM NVARCHAR(200),
@FROM_DATE DATE, 
@TO_DATE DATE
AS
BEGIN
	SELECT 
		V_NHAP_XUAT_DETAIL.NGAY_NHAP_XUAT, 
		V_NHAP_XUAT_DETAIL.MA_SAN_PHAM, 
		SAN_PHAM.TEN_SAN_PHAM, 
		DON_VI_TINH.TEN_DON_VI,
		ISNULL(SUM(V_NHAP_XUAT_DETAIL.SO_LUONG_NHAP), 0) SO_LUONG_NHAP, 
		ISNULL(SUM(V_NHAP_XUAT_DETAIL.SO_LUONG_XUAT),0) SO_LUONG_XUAT
	FROM 
		V_NHAP_XUAT_DETAIL, 
		SAN_PHAM, 
		DON_VI_TINH
	WHERE
		(@MA_KHO IS NULL OR @MA_KHO = 0 OR V_NHAP_XUAT_DETAIL.MA_KHO = @MA_KHO)
		AND (@MA_SAN_PHAM IS NULL OR @MA_SAN_PHAM = 0 OR V_NHAP_XUAT_DETAIL.MA_SAN_PHAM = @MA_SAN_PHAM)
		AND (@FROM_DATE IS NULL OR V_NHAP_XUAT_DETAIL.NGAY_NHAP_XUAT >= @FROM_DATE)
		AND (@TO_DATE IS NULL OR V_NHAP_XUAT_DETAIL.NGAY_NHAP_XUAT <= @TO_DATE)
		AND (@TEN_SAN_PHAM IS NULL OR @TEN_SAN_PHAM = '' OR SAN_PHAM.TEN_SAN_PHAM LIKE '%' + @TEN_SAN_PHAM + '%')
		AND V_NHAP_XUAT_DETAIL.MA_SAN_PHAM = SAN_PHAM.MA_SAN_PHAM
		AND SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
	GROUP BY 
		V_NHAP_XUAT_DETAIL.NGAY_NHAP_XUAT, 
		V_NHAP_XUAT_DETAIL.MA_SAN_PHAM, 
		SAN_PHAM.TEN_SAN_PHAM, 
		DON_VI_TINH.TEN_DON_VI
	ORDER BY V_NHAP_XUAT_DETAIL.NGAY_NHAP_XUAT
END


CREATE FUNCTION F_GET_EXPORT
(
	@MA_KHO INT, 
	@MA_SAN_PHAM INT
)
RETURNS FLOAT
AS
BEGIN
	RETURN (SELECT 
	ISNULL(SUM(SO_LUONG_XUAT),0) SO_LUONG_XUAT
	FROM 
	V_NHAP_XUAT_KHO
	WHERE
		(@MA_KHO IS NULL OR @MA_KHO = 0 OR V_NHAP_XUAT_KHO.MA_KHO = @MA_KHO)
	AND V_NHAP_XUAT_KHO.MA_SAN_PHAM = @MA_SAN_PHAM
	GROUP BY V_NHAP_XUAT_KHO.MA_SAN_PHAM
	)
END

CREATE FUNCTION F_GET_IMPORT
(
	@MA_KHO INT, 
	@MA_SAN_PHAM INT
)
RETURNS FLOAT
AS
BEGIN
	RETURN (SELECT 
	ISNULL(SUM(SO_LUONG_NHAP),0) SO_LUONG_NHAP
	FROM 
	V_NHAP_XUAT_KHO
	WHERE
		(@MA_KHO IS NULL OR @MA_KHO = 0 OR V_NHAP_XUAT_KHO.MA_KHO = @MA_KHO)
	AND V_NHAP_XUAT_KHO.MA_SAN_PHAM = @MA_SAN_PHAM
	GROUP BY V_NHAP_XUAT_KHO.MA_SAN_PHAM
	)
END

ALTER PROCEDURE STMA_GET_GIA_TRI_HANG_BAN_TON
@MA_KHO INT, 
@MA_SAN_PHAM INT, 
@GIA_VON_HANG_BAN_TOTAL FLOAT OUTPUT, 
@GIA_TRI_HANG_TON_TOTAL FLOAT OUTPUT
AS
BEGIN
	DECLARE @NGAY_KIEM_KHO DATE;
	DECLARE @SO_LUONG_DA_XUAT FLOAT;
	DECLARE @SO_LUONG_DA_NHAP FLOAT;
	DECLARE @PCOUNT FLOAT;
	DECLARE @ID_SAN_PHAM INT;
	DECLARE @TEN_SAN_PHAM NVARCHAR(100);
	DECLARE @TEN_DON_VI NVARCHAR(100);
	DECLARE @SO_LUONG_MUA FLOAT;
	DECLARE @DON_GIA_MUA FLOAT;
	DECLARE @GIA_VON_HANG_BAN FLOAT;
	DECLARE @GIA_TRI_HANG_TON FLOAT;
		
	
	DECLARE SAN_PHAM_CURSOR CURSOR FOR
	SELECT 
		MA_SAN_PHAM, TEN_SAN_PHAM, TEN_DON_VI 
	FROM SAN_PHAM, DON_VI_TINH
	WHERE
		SAN_PHAM.ACTIVE = 'A'
		AND SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
	ORDER BY SAN_PHAM.MA_SAN_PHAM
	
	CREATE TABLE #TEMP_FIFO
	(
		ID INT IDENTITY(1,1) PRIMARY KEY,  
		MA_SAN_PHAM INT, 
		TEN_SAN_PHAM NVARCHAR(100),
		TEN_DON_VI NVARCHAR(100),
		SO_LUONG_NHAP FLOAT, 
		SO_LUONG_BAN FLOAT, 
		GIA_VON_HANG_BAN FLOAT,
		SO_LUONG_TON FLOAT,
		GIA_VON_HANG_TON FLOAT
	)

	OPEN SAN_PHAM_CURSOR
	FETCH NEXT FROM SAN_PHAM_CURSOR INTO @ID_SAN_PHAM ,@TEN_SAN_PHAM, @TEN_DON_VI
	WHILE @@FETCH_STATUS = 0  
	BEGIN
		SET @SO_LUONG_DA_XUAT = DBO.F_GET_EXPORT(@MA_KHO, @ID_SAN_PHAM);
		SET @SO_LUONG_DA_NHAP = DBO.F_GET_IMPORT(@MA_KHO, @ID_SAN_PHAM);
		
		DECLARE MY_IMPORT_CURSOR CURSOR FOR
		SELECT 
			SO_LUONG, GIA_VON 
		FROM V_NHAP_KHO
		WHERE
			(@MA_KHO IS NULL OR @MA_KHO = 0 OR V_NHAP_KHO.MA_KHO = @MA_KHO)
			AND (V_NHAP_KHO.MA_SAN_PHAM = @ID_SAN_PHAM)
		ORDER BY V_NHAP_KHO.NGAY_NHAP;
		
		SET @PCOUNT = 0;
		SET @GIA_TRI_HANG_TON = 0;
		SET @GIA_VON_HANG_BAN = 0;
		OPEN MY_IMPORT_CURSOR
		FETCH NEXT FROM MY_IMPORT_CURSOR INTO @SO_LUONG_MUA ,@DON_GIA_MUA
		WHILE @@FETCH_STATUS = 0   
		BEGIN
			IF (@PCOUNT + @SO_LUONG_MUA) <= @SO_LUONG_DA_XUAT
				BEGIN
					SET @GIA_VON_HANG_BAN = @GIA_VON_HANG_BAN + @SO_LUONG_MUA*@DON_GIA_MUA
				END
			ELSE
				BEGIN
					IF @PCOUNT < @SO_LUONG_DA_XUAT
					BEGIN
						SET @GIA_VON_HANG_BAN = @GIA_VON_HANG_BAN + (@SO_LUONG_DA_XUAT - @PCOUNT)*@DON_GIA_MUA
					END
				END	
				
			IF (@PCOUNT + @SO_LUONG_MUA) > @SO_LUONG_DA_XUAT
			BEGIN
				IF @PCOUNT < @SO_LUONG_DA_XUAT
					BEGIN
						SET @GIA_TRI_HANG_TON = @GIA_TRI_HANG_TON + (@PCOUNT + @SO_LUONG_MUA -  @SO_LUONG_DA_XUAT)*@DON_GIA_MUA
					END
				ELSE
					BEGIN
						SET @GIA_TRI_HANG_TON = @GIA_TRI_HANG_TON + @SO_LUONG_MUA*@DON_GIA_MUA
					END
			END
			
			SET @PCOUNT = @PCOUNT + @SO_LUONG_MUA;			
			FETCH NEXT FROM MY_IMPORT_CURSOR INTO @SO_LUONG_MUA ,@DON_GIA_MUA
		END
		CLOSE MY_IMPORT_CURSOR
		DEALLOCATE MY_IMPORT_CURSOR
		
		SET @GIA_VON_HANG_BAN_TOTAL = @GIA_VON_HANG_BAN_TOTAL + @GIA_VON_HANG_BAN;
		SET @GIA_TRI_HANG_TON_TOTAL = @GIA_TRI_HANG_TON_TOTAL + @GIA_TRI_HANG_TON;
		
		IF @SO_LUONG_DA_NHAP >0 
		BEGIN
			INSERT INTO #TEMP_FIFO
		(
			MA_SAN_PHAM, 
			TEN_SAN_PHAM, 
			TEN_DON_VI, 
			SO_LUONG_NHAP, 
			SO_LUONG_BAN, 
			GIA_VON_HANG_BAN, 
			SO_LUONG_TON, 
			GIA_VON_HANG_TON
		)
		VALUES
		(
			@ID_SAN_PHAM, 
			@TEN_SAN_PHAM, 
			@TEN_DON_VI, 
			@SO_LUONG_DA_NHAP, 
			@SO_LUONG_DA_XUAT, 
			@GIA_VON_HANG_BAN, 
			@SO_LUONG_DA_NHAP - @SO_LUONG_DA_XUAT, 
			@GIA_TRI_HANG_TON
		)
		END;
		
		FETCH NEXT FROM SAN_PHAM_CURSOR INTO @ID_SAN_PHAM ,@TEN_SAN_PHAM, @TEN_DON_VI
	END
	CLOSE SAN_PHAM_CURSOR
	DEALLOCATE SAN_PHAM_CURSOR
	
	SELECT * FROM #TEMP_FIFO
END
	
	
DECLARE @GIA_VON_HANG_BAN FLOAT;
DECLARE @GIA_TRI_HANG_TON FLOAT;
exec STMA_GET_GIA_TRI_HANG_BAN_TON null, null, 0, 0

SELECT @GIA_VON_HANG_BAN, @GIA_TRI_HANG_TON

