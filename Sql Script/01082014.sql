ALTER TABLE TRA_HANG_NCC_CHI_TIET
ADD DON_GIA_TEMP FLOAT

ALTER PROCEDURE SP_GET_RE_DETAIL_BY_ID
@ID INT
AS
BEGIN
	SELECT
		TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC, 
		TRA_HANG_NCC_CHI_TIET.MA_SAN_PHAM, 
		SAN_PHAM.TEN_SAN_PHAM,
		SAN_PHAM.CODE,
		CASE ISNULL(TRA_HANG_NCC_CHI_TIET.DON_GIA_TEMP, 0) 
		WHEN 0 THEN
			ISNULL(TRA_HANG_NCC_CHI_TIET.DON_GIA,0)
		ELSE TRA_HANG_NCC_CHI_TIET.DON_GIA_TEMP
		END AS DON_GIA, 
		CASE ISNULL(TRA_HANG_NCC_CHI_TIET.MA_DON_VI, 0)
		WHEN 0 THEN
			SAN_PHAM.MA_DON_VI
		ELSE TRA_HANG_NCC_CHI_TIET.MA_DON_VI
		END AS MA_DON_VI, 
		DON_VI_TINH.TEN_DON_VI, 
		CASE ISNULL(TRA_HANG_NCC_CHI_TIET.SO_LUONG_TEMP, 0)
		WHEN 0 THEN
			TRA_HANG_NCC_CHI_TIET.SO_LUONG
		ELSE TRA_HANG_NCC_CHI_TIET.SO_LUONG_TEMP
		END AS SO_LUONG,
		TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT,
		KHO.TEN_KHO		
	FROM
	TRA_HANG_NCC_CHI_TIET, 
	SAN_PHAM, 
	DON_VI_TINH, 
	KHO
	WHERE
	TRA_HANG_NCC_CHI_TIET.ACTIVE = 'A'
	AND TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC = @ID
	AND TRA_HANG_NCC_CHI_TIET.MA_SAN_PHAM = SAN_PHAM.MA_SAN_PHAM 
	AND TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT = KHO.MA_KHO
	AND (
		TRA_HANG_NCC_CHI_TIET.MA_DON_VI IS NULL AND SAN_PHAM.MA_DON_VI = DON_VI_TINH.MA_DON_VI
		OR 
		TRA_HANG_NCC_CHI_TIET.MA_DON_VI IS NOT NULL AND TRA_HANG_NCC_CHI_TIET.MA_DON_VI = DON_VI_TINH.MA_DON_VI
	)	
END


CREATE PROCEDURE SP_EXPORT_4_RETURN_2_PROVIDER
@ID INT,
@STORE_ID INT, 
@USER_ID INT, 
@RETURN_VALUE INT OUTPUT
AS
BEGIN
	DECLARE  @SO_LUONG_TON FLOAT;
	DECLARE  @MA_SAN_PHAM INT;
	DECLARE  @LASTEST BIT;
	DECLARE  @DON_GIA FLOAT;
	DECLARE  @DON_GIA_TEMP FLOAT;
	DECLARE  @SO_LUONG FLOAT;
	DECLARE  @SO_LUONG_TEMP FLOAT;
	DECLARE  @MA_DON_VI INT;
	DECLARE  @MA_KHO_XUAT INT;
	DECLARE  @MA_XUAT_KHO INT;
	DECLARE  @FLG BIT;
	DECLARE  @NEXT_STORE_ID INT;
	
	IF @STORE_ID <> 0
		BEGIN
			DECLARE DETAIL_CURSOR CURSOR FOR
			SELECT 
				TRA_HANG_NCC_CHI_TIET.MA_SAN_PHAM,
				TRA_HANG_NCC_CHI_TIET.SO_LUONG
			FROM
				TRA_HANG_NCC_CHI_TIET
			WHERE
				TRA_HANG_NCC_CHI_TIET.ACTIVE = 'A'
				AND TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC = @ID
				AND TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT = @STORE_ID;
			
			SET @FLG = 'TRUE';
			
			OPEN DETAIL_CURSOR 
			FETCH NEXT FROM DETAIL_CURSOR INTO @MA_SAN_PHAM, @SO_LUONG
			WHILE @@FETCH_STATUS = 0 
			BEGIN
				SET @SO_LUONG_TON = DBO.F_GET_TON_KHO(@STORE_ID, @MA_SAN_PHAM);
				IF @SO_LUONG_TON IS NULL OR @SO_LUONG_TON = 0 OR @SO_LUONG_TON < @SO_LUONG
				BEGIN
					SET @FLG = 'FALSE'
					BREAK;
				END
				FETCH NEXT FROM DETAIL_CURSOR INTO @MA_SAN_PHAM, @SO_LUONG
			END;
			
			CLOSE DETAIL_CURSOR;
			DEALLOCATE DETAIL_CURSOR;
			
			IF @FLG = 'FALSE'
			BEGIN
				SET @RETURN_VALUE = 0;
			END
			ELSE
			BEGIN
				DECLARE DETAIL_CURSOR_EXPORT CURSOR FOR
				SELECT 
					TRA_HANG_NCC_CHI_TIET.MA_SAN_PHAM,
					TRA_HANG_NCC_CHI_TIET.SO_LUONG,
					TRA_HANG_NCC_CHI_TIET.DON_GIA,
					TRA_HANG_NCC_CHI_TIET.DON_GIA_TEMP,
					TRA_HANG_NCC_CHI_TIET.SO_LUONG_TEMP,
					TRA_HANG_NCC_CHI_TIET.MA_DON_VI, 
					TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT
				FROM
					TRA_HANG_NCC_CHI_TIET
				WHERE
					TRA_HANG_NCC_CHI_TIET.ACTIVE = 'A'
					AND TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC = @ID
					AND TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT = @STORE_ID;
				-- Insert into XUAT_KHO table
				INSERT INTO 
				XUAT_KHO 
				(	
					MA_HOA_DON, 
					LY_DO_XUAT,  --4 export for returning to provider
					MA_KHO_XUAT,
					NGAY_XUAT, 
					MA_NHAN_VIEN_XUAT,
					MA_PHIEU_TRA_NCC,
					CREATE_BY, 
					UPDATE_BY, 
					CREATE_AT, 
					UPDATE_AT, 
					ACTIVE
				)
				VALUES
				(
					@ID, 
					4, 
					@STORE_ID, 
					GETDATE(), 
					@USER_ID,
					@ID,
					@USER_ID, 
					@USER_ID, 
					GETDATE(), 
					GETDATE(),
					'A'
				); 
				SET @MA_XUAT_KHO = (SELECT SCOPE_IDENTITY());
				
				OPEN DETAIL_CURSOR_EXPORT 
				FETCH NEXT FROM DETAIL_CURSOR_EXPORT INTO @MA_SAN_PHAM, @SO_LUONG, @DON_GIA, @DON_GIA_TEMP, @SO_LUONG_TEMP, @MA_DON_VI, @MA_KHO_XUAT
				WHILE @@FETCH_STATUS = 0 
				BEGIN
					-- Insert details into CHI_TIET_XUAT_KHO table
					INSERT INTO
					CHI_TIET_XUAT_KHO
					(
						MA_XUAT_KHO, 
						MA_SAN_PHAM, 
						SO_LUONG, 
						GIA_XUAT,
						DON_GIA_TEMP,
						SO_LUONG_TEMP, 
						MA_DON_VI,
						CREATE_BY, 
						UPDATE_BY, 
						CREATE_AT, 
						UPDATE_AT, 
						ACTIVE
					)
					VALUES
					(
						@MA_XUAT_KHO, 
						@MA_SAN_PHAM, 
						@SO_LUONG, 
						@DON_GIA, 
						@DON_GIA_TEMP,
						@SO_LUONG_TEMP, 
						@MA_DON_VI,
						@USER_ID, 
						@USER_ID,
						GETDATE(), 
						GETDATE(), 
						'A'
					);					
					FETCH NEXT FROM DETAIL_CURSOR_EXPORT INTO @MA_SAN_PHAM, @SO_LUONG, @DON_GIA, @DON_GIA_TEMP, @SO_LUONG_TEMP, @MA_DON_VI, @MA_KHO_XUAT
				END;
			END;
		END
	ELSE
		BEGIN
			DECLARE DETAIL_CURSOR CURSOR FOR
			SELECT 
				TRA_HANG_NCC_CHI_TIET.MA_SAN_PHAM,
				TRA_HANG_NCC_CHI_TIET.SO_LUONG, 
				TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT
			FROM
				TRA_HANG_NCC_CHI_TIET
			WHERE
				TRA_HANG_NCC_CHI_TIET.ACTIVE = 'A'
				AND TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC = @ID
			ORDER BY TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT;
			
			SET @FLG = 'TRUE';
			
			OPEN DETAIL_CURSOR 
			FETCH NEXT FROM DETAIL_CURSOR INTO @MA_SAN_PHAM, @SO_LUONG, @STORE_ID
			WHILE @@FETCH_STATUS = 0 
			BEGIN
				SET @SO_LUONG_TON = DBO.F_GET_TON_KHO(@STORE_ID, @MA_SAN_PHAM);
				IF @SO_LUONG_TON IS NULL OR @SO_LUONG_TON = 0 OR @SO_LUONG_TON < @SO_LUONG
				BEGIN
					SET @FLG = 'FALSE'
					BREAK;
				END
				FETCH NEXT FROM DETAIL_CURSOR INTO @MA_SAN_PHAM, @SO_LUONG
			END;
			
			CLOSE DETAIL_CURSOR;
			DEALLOCATE DETAIL_CURSOR;
			
			IF @FLG = 'FALSE'
			BEGIN
				SET @RETURN_VALUE = 0;
			END
			IF @FLG = 'TRUE'
			BEGIN
				DECLARE DETAIL_CURSOR_EXPORT CURSOR FOR
				SELECT 
					TRA_HANG_NCC_CHI_TIET.MA_SAN_PHAM,
					TRA_HANG_NCC_CHI_TIET.SO_LUONG,
					TRA_HANG_NCC_CHI_TIET.DON_GIA,
					TRA_HANG_NCC_CHI_TIET.DON_GIA_TEMP,
					TRA_HANG_NCC_CHI_TIET.SO_LUONG_TEMP,
					TRA_HANG_NCC_CHI_TIET.MA_DON_VI, 
					TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT
				FROM
					TRA_HANG_NCC_CHI_TIET
				WHERE
					TRA_HANG_NCC_CHI_TIET.ACTIVE = 'A'
					AND TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC = @ID
				ORDER BY TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT;
				
				SET @STORE_ID = 0;
				
				OPEN DETAIL_CURSOR_EXPORT
				FETCH NEXT FROM DETAIL_CURSOR_EXPORT INTO @MA_SAN_PHAM, @SO_LUONG, @DON_GIA, @DON_GIA_TEMP, @SO_LUONG_TEMP, @MA_DON_VI, @NEXT_STORE_ID
				WHILE @@FETCH_STATUS = 0 
				BEGIN
					IF @STORE_ID <> @NEXT_STORE_ID
					BEGIN
						SET @STORE_ID = @NEXT_STORE_ID;
						INSERT INTO 
						XUAT_KHO 
						(	
							MA_HOA_DON, 
							LY_DO_XUAT,  --4 export for returning to provider
							MA_KHO_XUAT,
							NGAY_XUAT, 
							MA_NHAN_VIEN_XUAT,
							MA_PHIEU_TRA_NCC,
							CREATE_BY, 
							UPDATE_BY, 
							CREATE_AT, 
							UPDATE_AT, 
							ACTIVE
						)
						VALUES
						(
							@ID, 
							4, 
							@STORE_ID, 
							GETDATE(), 
							@USER_ID,
							@ID,
							@USER_ID, 
							@USER_ID, 
							GETDATE(), 
							GETDATE(),
							'A'
						); 
						SET @MA_XUAT_KHO = (SELECT SCOPE_IDENTITY());
					END;
					-- Insert details into CHI_TIET_XUAT_KHO table
					INSERT INTO
					CHI_TIET_XUAT_KHO
					(
						MA_XUAT_KHO, 
						MA_SAN_PHAM, 
						SO_LUONG, 
						GIA_XUAT,
						DON_GIA_TEMP,
						SO_LUONG_TEMP, 
						MA_DON_VI,
						CREATE_BY, 
						UPDATE_BY, 
						CREATE_AT, 
						UPDATE_AT, 
						ACTIVE
					)
					VALUES
					(
						@MA_XUAT_KHO, 
						@MA_SAN_PHAM, 
						@SO_LUONG, 
						@DON_GIA, 
						@DON_GIA_TEMP,
						@SO_LUONG_TEMP, 
						@MA_DON_VI,
						@USER_ID, 
						@USER_ID,
						GETDATE(), 
						GETDATE(), 
						'A'
					);	
					FETCH NEXT FROM DETAIL_CURSOR_EXPORT INTO @MA_SAN_PHAM, @SO_LUONG, @DON_GIA, @DON_GIA_TEMP, @SO_LUONG_TEMP, @MA_DON_VI, @MA_KHO_XUAT
				END;
				CLOSE DETAIL_CURSOR_EXPORT;
				DEALLOCATE DETAIL_CURSOR_EXPORT;
				SET @RETURN_VALUE = 1;
			END;
		END;
END

exec SP_GET_WAITING_EX_2_PROVIDER 0,'',null,null,0

ALTER PROCEDURE SP_GET_WAITING_EX_2_PROVIDER
@STORE_ID INT, 
@STORE_NAME NVARCHAR(100), 
@FROM_DATE DATE, 
@TO_DATE DATE, 
@STATAUS INT
AS
BEGIN
	SELECT 
		TRA_HANG_NCC.ID, 
		TRA_HANG_NCC.MA_NHA_CUNG_CAP,
		NHA_CUNG_CAP.TEN_NHA_CUNG_CAP,
		TRA_HANG_NCC.NGAY_LAP_PHIEU,
		TRA_HANG_NCC.NGUOI_LAP_PHIEU,
		NGUOI_DUNG.TEN_NGUOI_DUNG,
		TRA_HANG_NCC.GHI_CHU ,
		CASE (SELECT COUNT(*) FROM XUAT_KHO  
		WHERE XUAT_KHO.ACTIVE = 'A' 
		AND CAST(XUAT_KHO.NGAY_XUAT AS DATE) >= CAST(@FROM_DATE AS DATE) 
		AND XUAT_KHO.MA_PHIEU_TRA_NCC IS NOT NULL AND XUAT_KHO.MA_PHIEU_TRA_NCC = TRA_HANG_NCC.ID)
		WHEN 0 THEN 1
		ELSE 2 END AS STATUS		
	FROM
		TRA_HANG_NCC, 
		NHA_CUNG_CAP,
		NGUOI_DUNG, 
		TRA_HANG_NCC_CHI_TIET, 
		KHO
	WHERE 
		TRA_HANG_NCC.ACTIVE = 'A'
		AND TRA_HANG_NCC.MA_PHIEU_TRA IS NULL
		AND (@FROM_DATE IS NULL OR CAST(TRA_HANG_NCC.NGAY_LAP_PHIEU AS DATE) >= CAST(@FROM_DATE AS DATE))
		AND (@TO_DATE IS NULL OR CAST(TRA_HANG_NCC.NGAY_LAP_PHIEU AS DATE) <= CAST(@FROM_DATE AS DATE))
		AND (@STORE_ID = 0 OR TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT = @STORE_ID)
		AND	(@STORE_NAME IS NULL OR @STORE_NAME = '' OR KHO.TEN_KHO LIKE '%' + @STORE_NAME + '%')
		AND TRA_HANG_NCC.MA_NHA_CUNG_CAP = NHA_CUNG_CAP.MA_NHA_CUNG_CAP
		AND TRA_HANG_NCC.NGUOI_LAP_PHIEU = NGUOI_DUNG.MA_NGUOI_DUNG		
		AND TRA_HANG_NCC.ID = TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC
		AND TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT = KHO.MA_KHO	
	GROUP BY 
		TRA_HANG_NCC.ID, 
		TRA_HANG_NCC.MA_NHA_CUNG_CAP,
		NHA_CUNG_CAP.TEN_NHA_CUNG_CAP,
		TRA_HANG_NCC.NGAY_LAP_PHIEU,
		TRA_HANG_NCC.NGUOI_LAP_PHIEU,
		NGUOI_DUNG.TEN_NGUOI_DUNG,
		TRA_HANG_NCC.GHI_CHU 	
	HAVING
		@STATAUS = 0
		OR
		(@STATAUS = 1 AND SUM(TRA_HANG_NCC_CHI_TIET.SO_LUONG) > 
		(SELECT ISNULL(SUM(SO_LUONG),0) FROM CHI_TIET_XUAT_KHO 
		WHERE 
		CHI_TIET_XUAT_KHO.ACTIVE = 'A'
		AND MA_XUAT_KHO IN (SELECT MA_XUAT_KHO FROM XUAT_KHO 
								WHERE XUAT_KHO.ACTIVE = 'A' AND MA_PHIEU_TRA_NCC = TRA_HANG_NCC.ID AND CAST(XUAT_KHO.NGAY_XUAT AS DATE) >= CAST(@FROM_DATE AS DATE) AND XUAT_KHO.MA_PHIEU_TRA_NCC IS NOT NULL)))
		OR 
		(@STATAUS = 2 AND SUM(TRA_HANG_NCC_CHI_TIET.SO_LUONG) <= 
		(SELECT ISNULL(SUM(SO_LUONG),0) FROM CHI_TIET_XUAT_KHO 
		WHERE 
		CHI_TIET_XUAT_KHO.ACTIVE = 'A'
		AND MA_XUAT_KHO IN (SELECT MA_XUAT_KHO FROM XUAT_KHO 
								WHERE XUAT_KHO.ACTIVE = 'A' AND MA_PHIEU_TRA_NCC = TRA_HANG_NCC.ID AND CAST(XUAT_KHO.NGAY_XUAT AS DATE) >= CAST(@FROM_DATE AS DATE) AND XUAT_KHO.MA_PHIEU_TRA_NCC IS NOT NULL)))
								
		ORDER BY TRA_HANG_NCC.NGUOI_LAP_PHIEU DESC
END

