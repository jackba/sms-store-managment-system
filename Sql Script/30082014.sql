ALTER PROCEDURE [dbo].[STMA_GET_GIA_TRI_HANG_BAN_TON]
@MA_KHO INT, 
@TEN_KHO NVARCHAR(200), 
@MA_SAN_PHAM INT, 
@TEN_SAN_PHAM_PR NVARCHAR(200),
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
		
	SET @GIA_VON_HANG_BAN_TOTAL = 0;
	SET @GIA_TRI_HANG_TON_TOTAL = 0;
	IF 0 < (SELECT COUNT(*) FROM KHO
					WHERE ACTIVE = 'A' AND (@MA_KHO = 0 OR @MA_KHO = '' OR MA_KHO = @MA_KHO) AND UPPER(TEN_KHO) LIKE '%' + @TEN_KHO + '%')
	BEGIN
	
		DECLARE SAN_PHAM_CURSOR CURSOR FOR
		SELECT 
			MA_SAN_PHAM, TEN_SAN_PHAM, TEN_DON_VI 
		FROM SAN_PHAM, DON_VI_TINH
		WHERE
			SAN_PHAM.ACTIVE = 'A'
			AND (@MA_SAN_PHAM  IS NULL OR @MA_SAN_PHAM  = 0 OR SAN_PHAM.MA_SAN_PHAM = @MA_SAN_PHAM)
			AND (@TEN_SAN_PHAM_PR IS NULL OR @TEN_SAN_PHAM_PR = '' OR UPPER(TEN_SAN_PHAM) LIKE '%' + UPPER(@TEN_SAN_PHAM_PR) + '%')
			AND SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
		ORDER BY SAN_PHAM.MA_SAN_PHAM
		
		CREATE TABLE #TEMP_FIFO
		(
			ID INT IDENTITY(1,1) PRIMARY KEY,  
			MA_SAN_PHAM INT, 
			TEN_SAN_PHAM NVARCHAR(100),
			TEN_DON_VI NVARCHAR(100),
			SO_LUONG_NHAP FLOAT, 
			SO_LUONG_XUAT FLOAT, 
			GIA_VON_HANG_BAN FLOAT,
			SO_LUONG_TON FLOAT,
			GIA_VON_HANG_TON FLOAT
		)
		
		SET @GIA_VON_HANG_BAN_TOTAL = 0;
		SET @GIA_TRI_HANG_TON_TOTAL = 0;
		
		OPEN SAN_PHAM_CURSOR
		FETCH NEXT FROM SAN_PHAM_CURSOR INTO @ID_SAN_PHAM ,@TEN_SAN_PHAM, @TEN_DON_VI
		WHILE @@FETCH_STATUS = 0  
		BEGIN
			SET @SO_LUONG_DA_XUAT = DBO.F_GET_EXPORT(@MA_KHO, @ID_SAN_PHAM);
			SET @SO_LUONG_DA_NHAP = DBO.F_GET_IMPORT(@MA_KHO, @ID_SAN_PHAM);
			
			DECLARE MY_IMPORT_CURSOR CURSOR FOR
			SELECT 
				SO_LUONG, GIA_VON 
			FROM V_NHAP_KHO, KHO
			WHERE
				(@MA_KHO IS NULL OR @MA_KHO = 0 OR V_NHAP_KHO.MA_KHO = @MA_KHO)
				AND (@TEN_KHO IS NULL OR @TEN_KHO = '' OR UPPER(KHO.TEN_KHO) LIKE '%' + UPPER(@TEN_KHO) + '%')
				AND (V_NHAP_KHO.MA_SAN_PHAM = @ID_SAN_PHAM)
				AND V_NHAP_KHO.MA_KHO = KHO.MA_KHO
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
							SET @GIA_TRI_HANG_TON = ISNULL(@GIA_TRI_HANG_TON,0) + (ISNULL(@PCOUNT,0) + ISNULL(@SO_LUONG_MUA,0) -  ISNULL(@SO_LUONG_DA_XUAT,0))*ISNULL(@DON_GIA_MUA,0)
						END
					ELSE
						BEGIN
							SET @GIA_TRI_HANG_TON = ISNULL(@GIA_TRI_HANG_TON,0) + ISNULL(@SO_LUONG_MUA,0)*ISNULL(@DON_GIA_MUA,0)
						END
				END
				
				SET @PCOUNT = @PCOUNT + @SO_LUONG_MUA;			
				FETCH NEXT FROM MY_IMPORT_CURSOR INTO @SO_LUONG_MUA ,@DON_GIA_MUA
			END
			CLOSE MY_IMPORT_CURSOR
			DEALLOCATE MY_IMPORT_CURSOR
			
			SET @GIA_VON_HANG_BAN_TOTAL = ISNULL(@GIA_VON_HANG_BAN_TOTAL,0) + ISNULL(@GIA_VON_HANG_BAN,0);
			SET @GIA_TRI_HANG_TON_TOTAL = ISNULL(@GIA_TRI_HANG_TON_TOTAL,0) + ISNULL(@GIA_TRI_HANG_TON,0);
			
			IF @SO_LUONG_DA_NHAP >0 
			BEGIN
				INSERT INTO #TEMP_FIFO
			(
				MA_SAN_PHAM, 
				TEN_SAN_PHAM, 
				TEN_DON_VI, 
				SO_LUONG_NHAP, 
				SO_LUONG_XUAT, 
				GIA_VON_HANG_BAN, 
				SO_LUONG_TON, 
				GIA_VON_HANG_TON
			)
			VALUES
			(
				@ID_SAN_PHAM, 
				@TEN_SAN_PHAM, 
				@TEN_DON_VI, 
				ISNULL(@SO_LUONG_DA_NHAP,0), 
				ISNULL(@SO_LUONG_DA_XUAT,0), 
				ISNULL(@GIA_VON_HANG_BAN,0), 
				ISNULL(@SO_LUONG_DA_NHAP,0) - ISNULL(@SO_LUONG_DA_XUAT,0), 
				ISNULL(@GIA_TRI_HANG_TON,0)
			)
			END;			
			FETCH NEXT FROM SAN_PHAM_CURSOR INTO @ID_SAN_PHAM ,@TEN_SAN_PHAM, @TEN_DON_VI
		END
		CLOSE SAN_PHAM_CURSOR
		DEALLOCATE SAN_PHAM_CURSOR
		
		SELECT * FROM #TEMP_FIFO
	END
END