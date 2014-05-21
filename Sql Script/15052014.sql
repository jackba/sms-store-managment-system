ALTER PROCEDURE SP_GET_ALL_ROLE
@USER_ID INT, 
@USER_FULL_NAME NVARCHAR(100)
AS
BEGIN
	SELECT 
	PHAN_QUYEN.ID,
	NHOM_NGUOI_DUNG.MA_NHOM, 
	NHOM_NGUOI_DUNG.TEN_NHOM + ' (*)' TEN_NGUOI_DUNG, 
	1 STATUS,
	ISNULL(QUYEN_ADMIN,'FALSE') IS_ADMIN, 
	ISNULL(QUYEN_DANH_MUC_SAN_PHAM,'FALSE') IS_METADATA_MANAGER, 
	ISNULL(QUYEN_BAN_HANG,'FALSE') IS_SALER, 
	ISNULL(QUYEN_THAU_NGAN,'FALSE') IS_ACCOUNTING, 
	ISNULL(QUYEN_QUAN_LY_KHO,'FALSE') IS_STORE_MANAGER
FROM 
	PHAN_QUYEN,
	NHOM_NGUOI_DUNG
WHERE
	PHAN_QUYEN.ACTIVE = 'A'
	AND PHAN_QUYEN.MA_NHOM_NGUOI_DUNG = NHOM_NGUOI_DUNG.MA_NHOM
	AND (@USER_ID = 0)
	AND (@USER_FULL_NAME IS NULL)
UNION
SELECT 
		US_ROLE.ID,
		US_ROLE.MA_NGUOI_DUNG MA_NHOM,
		US_ROLE.TEN_NGUOI_DUNG,
		2 STATUS,
		CASE WHEN ISNULL(US_GROUP_ROLE.IS_ADMIN,'FALSE') = 'FALSE' 
			THEN US_ROLE.IS_ADMIN
		ELSE
			US_GROUP_ROLE.IS_ADMIN
		END AS IS_ADMIN, 
		CASE WHEN ISNULL(US_GROUP_ROLE.IS_METADATA_MANAGER,'FALSE') = 'FALSE'
			THEN US_ROLE.IS_METADATA_MANAGER
		ELSE
			US_GROUP_ROLE.IS_METADATA_MANAGER
		END AS IS_METADATA_MANAGER, 
		CASE WHEN ISNULL(US_GROUP_ROLE.IS_SALER,'FALSE') = 'FALSE'
			THEN US_ROLE.IS_SALER
		ELSE
			US_GROUP_ROLE.IS_SALER
		END AS IS_SALER, 
		CASE WHEN ISNULL(US_GROUP_ROLE.IS_ACCOUNTING, 'FALSE') = 'FALSE'
			THEN US_ROLE.IS_ACCOUNTING
		ELSE
			US_GROUP_ROLE.IS_ACCOUNTING
		END AS IS_ACCOUNTING, 
		CASE WHEN ISNULL(US_GROUP_ROLE.IS_STORE_MANAGER,'FALSE') = 'FALSE'
			THEN US_ROLE.IS_STORE_MANAGER
		ELSE
			US_GROUP_ROLE.IS_STORE_MANAGER
		END AS IS_STORE_MANAGER
	FROM
	(
		SELECT 
			NGUOI_DUNG.MA_NGUOI_DUNG, 
			NGUOI_DUNG.TEN_NGUOI_DUNG,
			ISNULL(QUYEN_ADMIN,'FALSE') IS_ADMIN, 
			ISNULL(QUYEN_DANH_MUC_SAN_PHAM,'FALSE') IS_METADATA_MANAGER, 
			ISNULL(QUYEN_BAN_HANG,'FALSE') IS_SALER, 
			ISNULL(QUYEN_THAU_NGAN,'FALSE') IS_ACCOUNTING, 
			ISNULL(QUYEN_QUAN_LY_KHO,'FALSE') IS_STORE_MANAGER
		FROM 
			NGUOI_DUNG
			LEFT JOIN PHAN_QUYEN ON NGUOI_DUNG.MA_NHOM_NGUOI_DUNG = PHAN_QUYEN.MA_NHOM_NGUOI_DUNG
		WHERE 
		PHAN_QUYEN.ACTIVE = 'A'
		AND NGUOI_DUNG.ACTIVE = 'A'
		AND (@USER_ID = 0 OR NGUOI_DUNG.MA_NGUOI_DUNG = @USER_ID)
		AND (@USER_FULL_NAME IS NULL OR NGUOI_DUNG.TEN_NGUOI_DUNG LIKE '%' + @USER_FULL_NAME + '%')
	)US_GROUP_ROLE RIGHT JOIN
	(
		SELECT 
			PHAN_QUYEN.ID,
			NGUOI_DUNG.MA_NGUOI_DUNG,
			NGUOI_DUNG.TEN_NGUOI_DUNG,
			ISNULL(QUYEN_ADMIN,'FALSE') IS_ADMIN, 
			ISNULL(QUYEN_DANH_MUC_SAN_PHAM,'FALSE') IS_METADATA_MANAGER, 
			ISNULL(QUYEN_BAN_HANG,'FALSE') IS_SALER, 
			ISNULL(QUYEN_THAU_NGAN,'FALSE') IS_ACCOUNTING, 
			ISNULL(QUYEN_QUAN_LY_KHO,'FALSE') IS_STORE_MANAGER
		FROM 
			NGUOI_DUNG LEFT JOIN PHAN_QUYEN ON NGUOI_DUNG.MA_NGUOI_DUNG = PHAN_QUYEN.MA_NGUOI_DUNG
		WHERE 
		PHAN_QUYEN.ACTIVE = 'A'
		AND NGUOI_DUNG.ACTIVE = 'A'
		AND (@USER_ID = 0 OR NGUOI_DUNG.MA_NGUOI_DUNG = @USER_ID)
		AND (@USER_FULL_NAME IS NULL OR NGUOI_DUNG.TEN_NGUOI_DUNG LIKE '%' + @USER_FULL_NAME + '%')
	)US_ROLE
	ON	US_GROUP_ROLE.MA_NGUOI_DUNG = US_ROLE.MA_NGUOI_DUNG
END



ALTER PROCEDURE SP_GET_ROLE_OF_USER
@USER_ID INT
AS
BEGIN
	SELECT 
		CASE WHEN ISNULL(US_GROUP_ROLE.IS_ADMIN,'FALSE') = 'FALSE' 
			THEN US_ROLE.IS_ADMIN
		ELSE
			US_GROUP_ROLE.IS_ADMIN
		END AS IS_ADMIN, 
		CASE WHEN ISNULL(US_GROUP_ROLE.IS_METADATA_MANAGER,'FALSE') = 'FALSE'
			THEN US_ROLE.IS_METADATA_MANAGER
		ELSE
			US_GROUP_ROLE.IS_METADATA_MANAGER
		END AS IS_METADATA_MANAGER, 
		CASE WHEN ISNULL(US_GROUP_ROLE.IS_SALER,'FALSE') = 'FALSE'
			THEN US_ROLE.IS_SALER
		ELSE
			US_GROUP_ROLE.IS_SALER
		END AS IS_SALER, 
		CASE WHEN ISNULL(US_GROUP_ROLE.IS_ACCOUNTING, 'FALSE') = 'FALSE'
			THEN US_ROLE.IS_ACCOUNTING
		ELSE
			US_GROUP_ROLE.IS_ACCOUNTING
		END AS IS_ACCOUNTING, 
		CASE WHEN ISNULL(US_GROUP_ROLE.IS_STORE_MANAGER,'FALSE') = 'FALSE'
			THEN US_ROLE.IS_STORE_MANAGER
		ELSE
			US_GROUP_ROLE.IS_STORE_MANAGER
		END AS IS_STORE_MANAGER
	FROM
	(
		SELECT 
			NGUOI_DUNG.MA_NGUOI_DUNG,
			ISNULL(QUYEN_ADMIN,'FALSE') IS_ADMIN, 
			ISNULL(QUYEN_DANH_MUC_SAN_PHAM,'FALSE') IS_METADATA_MANAGER, 
			ISNULL(QUYEN_BAN_HANG,'FALSE') IS_SALER, 
			ISNULL(QUYEN_THAU_NGAN,'FALSE') IS_ACCOUNTING, 
			ISNULL(QUYEN_QUAN_LY_KHO,'FALSE') IS_STORE_MANAGER
		FROM 
			NGUOI_DUNG
			LEFT JOIN PHAN_QUYEN ON NGUOI_DUNG.MA_NHOM_NGUOI_DUNG = PHAN_QUYEN.MA_NHOM_NGUOI_DUNG
		WHERE 
		PHAN_QUYEN.ACTIVE = 'A'
		AND NGUOI_DUNG.ACTIVE = 'A'
		AND NGUOI_DUNG.MA_NGUOI_DUNG = @USER_ID
	)US_GROUP_ROLE FULL JOIN
	(
		SELECT 
			NGUOI_DUNG.MA_NGUOI_DUNG,
			ISNULL(QUYEN_ADMIN,'FALSE') IS_ADMIN, 
			ISNULL(QUYEN_DANH_MUC_SAN_PHAM,'FALSE') IS_METADATA_MANAGER, 
			ISNULL(QUYEN_BAN_HANG,'FALSE') IS_SALER, 
			ISNULL(QUYEN_THAU_NGAN,'FALSE') IS_ACCOUNTING, 
			ISNULL(QUYEN_QUAN_LY_KHO,'FALSE') IS_STORE_MANAGER
		FROM 
			NGUOI_DUNG LEFT JOIN PHAN_QUYEN ON NGUOI_DUNG.MA_NGUOI_DUNG = PHAN_QUYEN.MA_NGUOI_DUNG
		WHERE 
		PHAN_QUYEN.ACTIVE = 'A'
		AND NGUOI_DUNG.ACTIVE = 'A'
		AND NGUOI_DUNG.MA_NGUOI_DUNG = @USER_ID
	)US_ROLE
	ON	US_GROUP_ROLE.MA_NGUOI_DUNG = US_ROLE.MA_NGUOI_DUNG
END
