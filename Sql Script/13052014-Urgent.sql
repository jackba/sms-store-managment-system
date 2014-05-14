ALTER TABLE CONTROLLER_PERMISSION
DROP COLUMN PERMISSION_LEVEL

ALTER TABLE CONTROLLER_PERMISSION
ADD 
IS_ADMIN BIT, 
IS_ACCOUNTING BIT, 
IS_METEDATA_MANAGER BIT, 
IS_STORE_MANAGER BIT,
IS_SALER BIT 


INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Home', 
	'Index', 
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	GETDATE(), 
	'A'
)


INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Home', 
	'Index', 
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	GETDATE(), 
	'A'
)

INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Home', 
	'DanhMuc', 
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	GETDATE(), 
	'A'
)

INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Home', 
	'QuanLyKho', 
	'TRUE',
	'TRUE',
	'FALSE',
	'TRUE',
	'TRUE',
	GETDATE(), 
	'A'
)


INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Home', 
	'QuanTri', 
	'TRUE',
	'FALSE',
	'FALSE',
	'FALSE',
	'FALSE',
	GETDATE(), 
	'A'
)



INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Home', 
	'BanHang', 
	'TRUE',
	'TRUE',
	'FALSE',
	'FALSE',
	'TRUE',
	GETDATE(), 
	'A'
)

INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'HangSanXuat', 
	'Index', 
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	GETDATE(), 
	'A'
)

INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'HangSanXuat', 
	'AddNew', 
	'TRUE',
	'FALSE',
	'TRUE',
	'FALSE',
	'FALSE',
	GETDATE(), 
	'A'
)

INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Kho', 
	'AddNew', 
	'TRUE',
	'FALSE',
	'TRUE',
	'FALSE',
	'FALSE',
	GETDATE(), 
	'A'
)

INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Kho', 
	'Index', 
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	GETDATE(), 
	'A'
)

INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Kho', 
	'Edit', 
	'TRUE',
	'FALSE',
	'TRUE',
	'TRUE',
	'FALSE',
	GETDATE(), 
	'A'
)

INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Kho', 
	'Delete', 
	'TRUE',
	'FALSE',
	'TRUE',
	'TRUE',
	'FALSE',
	GETDATE(), 
	'A'
)



INSERT INTO CONTROLLER_PERMISSION
(
	CONTROLLER_NAME, 
	ACTION_NAME, 
	IS_ADMIN, 
	IS_ACCOUNTING, 
	IS_METEDATA_MANAGER, 
	IS_STORE_MANAGER, 
	IS_SALER,
	UPDATE_AT,
	ACTIVE
)VALUES
(
	'Kho', 
	'FindSuggest', 
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	'TRUE',
	GETDATE(), 
	'A'
)

ALTER TABLE NGUOI_DUNG
ALTER COLUMN MAT_KHAU VARCHAR(255)
 
ALTER TABLE NGUOI_DUNG
ADD SALT VARCHAR(255)


UPDATE NGUOI_DUNG
SET MAT_KHAU = '0HIAaEncGuVS51Nf1Inrf1T660nkvWpde/fa7yoZ6wbeaSJh5BK6U1DdMGiw5ENQZYDphXRLZYJhtegi9oR5Mg==', 
SALT = '100000.BO869isublnNjuCkDNaB4uITX29occsE3untR2MUOVinVw=='
WHERE USER_NAME = 'ADMIN'
 




ALTER PROCEDURE SP_GET_ROLE_OF_USER
@USER_ID INT
AS
BEGIN
	SELECT 
		CASE WHEN US_GROUP_ROLE.IS_ADMIN = 'FALSE'
			THEN US_ROLE.IS_ADMIN
		ELSE
			US_GROUP_ROLE.IS_ADMIN
		END AS IS_ADMIN, 
		CASE WHEN US_GROUP_ROLE.IS_METADATA_MANAGER = 'FALSE'
			THEN US_ROLE.IS_METADATA_MANAGER
		ELSE
			US_GROUP_ROLE.IS_METADATA_MANAGER
		END AS IS_METADATA_MANAGER, 
		CASE WHEN US_GROUP_ROLE.IS_SALER = 'FALSE'
			THEN US_ROLE.IS_SALER
		ELSE
			US_GROUP_ROLE.IS_SALER
		END AS IS_SALER, 
		CASE WHEN US_GROUP_ROLE.IS_ACCOUNTING = 'FALSE'
			THEN US_ROLE.IS_ACCOUNTING
		ELSE
			US_GROUP_ROLE.IS_ACCOUNTING
		END AS IS_ACCOUNTING, 
		CASE WHEN US_GROUP_ROLE.IS_STORE_MANAGER = 'FALSE'
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

UPDATE NHOM_NGUOI_DUNG
SET ACTIVE = 'A'

INSERT INTO NHOM_NGUOI_DUNG
(
	TEN_NHOM, 
	GHI_CHU, 
	CREATE_AT, 
	UPDATE_AT, 
	ACTIVE
)VALUES
(
	N'Quản lý kho',
	N'Quản lý kho',
	GETDATE(), 
	GETDATE(), 
	'A'
)


INSERT INTO PHAN_QUYEN
(
	QUYEN_ADMIN, 
	MA_NHOM_NGUOI_DUNG, 
	CREATE_AT, 
	UPDATE_AT, 
	ACTIVE
)
VALUES
(
	'TRUE',
	1, 
	GETDATE(), 
	GETDATE(), 
	'A'
)
INSERT INTO PHAN_QUYEN
(
	QUYEN_DANH_MUC_SAN_PHAM, 
	MA_NHOM_NGUOI_DUNG, 
	CREATE_AT, 
	UPDATE_AT, 
	ACTIVE
)
VALUES
(
	'TRUE',
	2, 
	GETDATE(), 
	GETDATE(), 
	'A'
)

INSERT INTO PHAN_QUYEN
(
	QUYEN_BAN_HANG, 
	MA_NHOM_NGUOI_DUNG, 
	CREATE_AT, 
	UPDATE_AT, 
	ACTIVE
)
VALUES
(
	'TRUE',
	3, 
	GETDATE(), 
	GETDATE(), 
	'A'
)

INSERT INTO PHAN_QUYEN
(
	QUYEN_THAU_NGAN, 
	MA_NHOM_NGUOI_DUNG, 
	CREATE_AT, 
	UPDATE_AT, 
	ACTIVE
)
VALUES
(
	'TRUE',
	4, 
	GETDATE(), 
	GETDATE(), 
	'A'
)

INSERT INTO PHAN_QUYEN
(
	QUYEN_QUAN_LY_KHO, 
	MA_NHOM_NGUOI_DUNG, 
	CREATE_AT, 
	UPDATE_AT, 
	ACTIVE
)
VALUES
(
	'TRUE',
	5, 
	GETDATE(), 
	GETDATE(), 
	'A'
)
