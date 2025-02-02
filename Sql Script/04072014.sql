ALTER PROCEDURE [dbo].[SP_GET_IMPORT]
@FROM_DATE DATE, 
@TO_DATE DATE, 
@MA_NHAN_VIEN_NHAP INT, 
@TEN_NHAN_VIEN_NHAP NVARCHAR(100),
@LY_DO_NHAP INT, 
@MA_KHO INT, 
@TEN_KHO NVARCHAR(100),
@MA_NHA_CC INT, 
@TEN_NHA_CUNG_CAP NVARCHAR(100)
AS
BEGIN
	SELECT 
		NHAP_KHO.MA_NHAP_KHO, 
		NHAP_KHO.SO_HOA_DON, 
		NHAP_KHO.NGAY_NHAP, 
		NHAP_KHO.NHAN_VIEN_NHAP, 
		NHAP_KHO.MA_NHA_CUNG_CAP, 
		NHAP_KHO.MA_KHO, 
		NGUOI_DUNG.TEN_NGUOI_DUNG, 
		KHO.TEN_KHO, 
		NHA_CUNG_CAP.TEN_NHA_CUNG_CAP, 
		NHAP_KHO.LY_DO_NHAP
	FROM 
		NHAP_KHO LEFT JOIN NHA_CUNG_CAP ON NHAP_KHO.MA_NHA_CUNG_CAP = NHA_CUNG_CAP.MA_NHA_CUNG_CAP,
		KHO, 
		NGUOI_DUNG
	WHERE
		NHAP_KHO.ACTIVE = 'A'
		AND (@TEN_NHAN_VIEN_NHAP IS NULL OR @TEN_NHAN_VIEN_NHAP = '' OR NGUOI_DUNG.TEN_NGUOI_DUNG LIKE '%'  + @TEN_NHAN_VIEN_NHAP + '%')
		AND (@FROM_DATE IS NULL OR CAST(NGAY_NHAP AS DATE) >= CAST(@FROM_DATE AS DATE))
		AND (@TO_DATE IS NULL OR CAST(NGAY_NHAP AS DATE) <= CAST(@TO_DATE AS DATE))
		AND (@MA_NHAN_VIEN_NHAP = 0 OR NHAN_VIEN_NHAP = @MA_NHAN_VIEN_NHAP)
		AND (@LY_DO_NHAP = -1 OR LY_DO_NHAP = @LY_DO_NHAP)
		AND (@MA_KHO = 0 OR NHAP_KHO.MA_KHO = @MA_KHO)
		AND (@TEN_KHO IS NULL OR @TEN_KHO = '' OR KHO.TEN_KHO LIKE '%' +  @TEN_KHO + '%')
		AND (@MA_NHA_CC = 0 OR NHAP_KHO.MA_NHA_CUNG_CAP = @MA_NHA_CC) 
		AND (@TEN_NHA_CUNG_CAP IS NULL OR @TEN_NHA_CUNG_CAP = '' OR NHA_CUNG_CAP.TEN_NHA_CUNG_CAP LIKE '%' + @TEN_NHA_CUNG_CAP + '%')
		AND NHAP_KHO.MA_KHO = KHO.MA_KHO
		AND NHAP_KHO.NHAN_VIEN_NHAP = NGUOI_DUNG.MA_NGUOI_DUNG
END


CREATE PROCEDURE SP_GET_IMPORT_INFOR_BY_ID
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
		NHAP_KHO, 
		NGUOI_DUNG, 
		NHA_CUNG_CAP
	WHERE
		NHAP_KHO.MA_NHAP_KHO = @MA_NHAP_KHO
		AND NHAP_KHO.ACTIVE = 'A'
		AND NHAP_KHO.NHAN_VIEN_NHAP = NGUOI_DUNG.MA_NGUOI_DUNG
		AND NHAP_KHO.MA_NHA_CUNG_CAP = NHA_CUNG_CAP.MA_NHA_CUNG_CAP
END



ALTER PROCEDURE SP_GET_RETURN_DETAIL_BY_ID
@MA_TRA_HANG INT
AS
BEGIN
	SELECT 
		CHI_TIET_TRA_HANG.MA_SAN_PHAM, 
		SAN_PHAM.TEN_SAN_PHAM,
		SAN_PHAM.CODE,
		ISNULL(CHI_TIET_TRA_HANG.GIA_BAN, 0) GIA_BAN,
		CASE ISNULL(CHI_TIET_TRA_HANG.DON_GIA_TEMP, 0) 
		WHEN 0 THEN CHI_TIET_TRA_HANG.GIA_VON
		ELSE CHI_TIET_TRA_HANG.DON_GIA_TEMP
		END AS DON_GIA,
		CASE ISNULL(CHI_TIET_TRA_HANG.MA_DON_VI,0)
		WHEN 0 THEN
			SAN_PHAM.MA_DON_VI
		ELSE CHI_TIET_TRA_HANG.MA_DON_VI
		END AS MA_DON_VI,
		DON_VI_TINH.TEN_DON_VI,
		CASE ISNULL(CHI_TIET_TRA_HANG.SO_LUONG_TEMP,0)
		WHEN 0 THEN
			CHI_TIET_TRA_HANG.SO_LUONG_TRA
		ELSE CHI_TIET_TRA_HANG.SO_LUONG_TEMP
		END AS SO_LUONG, 
		ISNULL(DBO.F_GET_CONVERTOR(CHI_TIET_TRA_HANG.MA_SAN_PHAM,CHI_TIET_TRA_HANG.MA_DON_VI),1) HE_SO
	FROM 
		CHI_TIET_TRA_HANG, 
		SAN_PHAM, 
		DON_VI_TINH
	WHERE
		CHI_TIET_TRA_HANG.ACTIVE = 'A'
		AND CHI_TIET_TRA_HANG.MA_TRA_HANG = @MA_TRA_HANG
		AND CHI_TIET_TRA_HANG.MA_SAN_PHAM = SAN_PHAM.MA_SAN_PHAM
		AND 
		(
			CHI_TIET_TRA_HANG.MA_DON_VI IS NULL AND SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
			OR 
			CHI_TIET_TRA_HANG.MA_DON_VI IS NOT NULL AND CHI_TIET_TRA_HANG.MA_DON_VI = DON_VI_TINH.MA_DON_VI
		)
END




CREATE PROCEDURE SP_GET_IMPORT_DETAIL_BY_ID_4_EDIT
@MA_NHAP_KHO INT
AS
BEGIN
	SELECT 
		CHI_TIET_NHAP_KHO.MA_SAN_PHAM,
		SAN_PHAM.TEN_SAN_PHAM,
		CASE ISNULL(CHI_TIET_NHAP_KHO.SO_LUONG_TEMP, 0) 
			WHEN 0 THEN CHI_TIET_NHAP_KHO.SO_LUONG
			ELSE CHI_TIET_NHAP_KHO.SO_LUONG_TEMP END AS SO_LUONG,
		CASE ISNULL(CHI_TIET_NHAP_KHO.DON_GIA_TEMP, 0) 
			WHEN 0 THEN  CHI_TIET_NHAP_KHO.GIA_VON
			ELSE CHI_TIET_NHAP_KHO.DON_GIA_TEMP END AS DON_GIA,
		CASE ISNULL(CHI_TIET_NHAP_KHO.MA_DON_VI,0)
			WHEN 0 THEN SAN_PHAM.MA_DON_VI
			ELSE CHI_TIET_NHAP_KHO.MA_DON_VI END AS MA_DON_VI, 
		DON_VI_TINH.TEN_DON_VI, 
		ISNULL(DBO.F_GET_CONVERTOR(CHI_TIET_NHAP_KHO.MA_SAN_PHAM,CHI_TIET_NHAP_KHO.MA_DON_VI),1) HE_SO
	FROM
		CHI_TIET_NHAP_KHO, 
		SAN_PHAM, 
		DON_VI_TINH
	WHERE
		CHI_TIET_NHAP_KHO.ACTIVE = 'A'
		AND CHI_TIET_NHAP_KHO.MA_NHAP_KHO = @MA_NHAP_KHO
		AND CHI_TIET_NHAP_KHO.MA_SAN_PHAM = SAN_PHAM.MA_SAN_PHAM
		AND 
		(
			CHI_TIET_NHAP_KHO.MA_DON_VI IS NULL AND SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
			OR 
			CHI_TIET_NHAP_KHO.MA_DON_VI IS NOT NULL AND CHI_TIET_NHAP_KHO.MA_DON_VI = DON_VI_TINH.MA_DON_VI
		) 
END


ALTER PROCEDURE [dbo].[STMA_GET_GIA_TRI_HANG_BAN_TON]
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
		AND (@MA_SAN_PHAM  IS NULL OR @MA_SAN_PHAM  = 0 OR SAN_PHAM.MA_SAN_PHAM = @MA_SAN_PHAM)
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

