ALTER PROCEDURE SP_EXPORT_4_RETURN_2_PROVIDER
@ID INT,
@STORE_ID INT, 
@USER_ID INT,
@EXPORT_DATE DATE, 
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
				AND TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT NOT IN (SELECT MA_KHO_XUAT FROM XUAT_KHO WHERE XUAT_KHO.MA_PHIEU_TRA_NCC = TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC AND XUAT_KHO.ACTIVE ='A') 
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
					AND TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT NOT IN (SELECT MA_KHO_XUAT FROM XUAT_KHO WHERE XUAT_KHO.MA_PHIEU_TRA_NCC = TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC AND XUAT_KHO.ACTIVE ='A') 
					AND TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC = @ID
					AND TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT = @STORE_ID;
				-- Insert into XUAT_KHO table
				INSERT INTO 
				XUAT_KHO 
				(	
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
					4, 
					@STORE_ID, 
					@EXPORT_DATE, 
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
				AND TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT NOT IN (SELECT MA_KHO_XUAT FROM XUAT_KHO WHERE XUAT_KHO.MA_PHIEU_TRA_NCC = TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC AND XUAT_KHO.ACTIVE ='A') 
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
				FETCH NEXT FROM DETAIL_CURSOR INTO @MA_SAN_PHAM, @SO_LUONG, @STORE_ID
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
					AND TRA_HANG_NCC_CHI_TIET.MA_KHO_XUAT NOT IN (SELECT MA_KHO_XUAT FROM XUAT_KHO WHERE XUAT_KHO.MA_PHIEU_TRA_NCC = TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC AND XUAT_KHO.ACTIVE ='A') 
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
							4, 
							@STORE_ID, 
							@EXPORT_DATE, 
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


ALTER FUNCTION F_GET_SUM_OF_RETUN
(
	@ID INT
)
RETURNS FLOAT
AS
BEGIN
	DECLARE @RETURN_VALUE FLOAT; 
	SET @RETURN_VALUE = (SELECT SUM(SO_LUONG) FROM TRA_HANG_NCC_CHI_TIET
			WHERE ACTIVE = 'A' AND TRA_HANG_NCC_CHI_TIET.MA_PHIEU_TRA_NCC = @ID 
		);
	RETURN @RETURN_VALUE;
END

CREATE FUNCTION F_GET_SUM_OF_EXPORT
(
	@ID INT
)
RETURNS FLOAT
AS
BEGIN
	RETURN (SELECT SUM(SO_LUONG) FROM CHI_TIET_XUAT_KHO, XUAT_KHO
			WHERE XUAT_KHO.ACTIVE = 'A' AND CHI_TIET_XUAT_KHO.ACTIVE = 'A' 
			AND XUAT_KHO.MA_PHIEU_TRA_NCC = @ID 
			AND XUAT_KHO.MA_XUAT_KHO = CHI_TIET_XUAT_KHO.MA_XUAT_KHO
		)
END

CREATE FUNCTION GET_STAUS 
(
	@ID INT
)
RETURNS INT
AS
BEGIN
	DECLARE @RETURN_VALUE FLOAT; 
	DECLARE @EXPORT_VALUE FLOAT; 
	SET @RETURN_VALUE = DBO.F_GET_SUM_OF_RETUN(@ID);
	SET @EXPORT_VALUE = DBO.F_GET_SUM_OF_EXPORT(@ID);
	IF @RETURN_VALUE > @EXPORT_VALUE
		SET @RETURN_VALUE = 1;
	ELSE 
		SET @RETURN_VALUE = 2;		
	RETURN @RETURN_VALUE
END


ALTER PROCEDURE [dbo].[SP_GET_WAITING_EX_2_PROVIDER]
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
		DBO.GET_STAUS(TRA_HANG_NCC.ID) STATUS		
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
		AND 
		(@STATAUS = 0
		OR
		(@STATAUS = 1 AND DBO.F_GET_SUM_OF_RETUN(TRA_HANG_NCC.ID) > DBO.F_GET_SUM_OF_EXPORT(TRA_HANG_NCC.ID))
		OR 
		(@STATAUS = 2 AND DBO.F_GET_SUM_OF_RETUN(TRA_HANG_NCC.ID) <= DBO.F_GET_SUM_OF_EXPORT(TRA_HANG_NCC.ID))
		)	
		GROUP BY
		TRA_HANG_NCC.ID, 
		TRA_HANG_NCC.MA_NHA_CUNG_CAP,
		NHA_CUNG_CAP.TEN_NHA_CUNG_CAP,
		TRA_HANG_NCC.NGAY_LAP_PHIEU,
		TRA_HANG_NCC.NGUOI_LAP_PHIEU,
		NGUOI_DUNG.TEN_NGUOI_DUNG,
		TRA_HANG_NCC.GHI_CHU						
		ORDER BY TRA_HANG_NCC.NGUOI_LAP_PHIEU DESC
END
