ALTER FUNCTION [dbo].[F_GET_STORE_PERMISSION]
(
	@USR_ID INT, 
	@STORE_ID INT
)
RETURNS INT
AS
BEGIN
	RETURN ISNULL((SELECT 1 + ISNULL(USR_ID,'0')/ISNULL(USR_ID,'0') + ISNULL(IS_DEFAULT,'0') FROM USER_STORE WHERE MA_KHO = @STORE_ID AND USR_ID = @USR_ID AND ACTIVE = 'A'),0);
END


ALTER PROCEDURE [dbo].[SP_GET_HOA_DON_CAN_XUAT_KHO]
@MA_KHACH_HANG INT, 
@TEN_KHACH_HANG NVARCHAR(100),
@MA_KHO INT, 
@TEN_KHO NVARCHAR(200),
@FROM_DATE DATE, 
@TO_DATE DATE
AS
BEGIN
	SELECT DISTINCT
		MA_HOA_DON, 
		SO_HOA_DON, 
		MA_KHACH_HANG, 
		TEN_KHACH_HANG, 
		MA_NHAN_VIEN_BAN,
		TEN_NHAN_VIEN_BAN, 
		NGAY_BAN, 
		NGAY_GIAO, 
		DIA_CHI_GIAO_HANG
	FROM 
		V_HOA_DON,
		KHO
	WHERE
		(
			STATUS = 2 OR 
			(STATUS = 3 AND DBO.F_SUM_QUANTITY_BY_BILL_ID(MA_HOA_DON) > DBO.F_SUM_EXPORTED_BY_BILL_ID(MA_HOA_DON))
		)
		AND V_HOA_DON.MA_KHO_XUAT NOT IN (SELECT MA_KHO_XUAT FROM XUAT_KHO WHERE ACTIVE = 'A' AND MA_HOA_DON = V_HOA_DON.MA_HOA_DON)
		AND (@MA_KHACH_HANG = 0 OR MA_KHACH_HANG = @MA_KHACH_HANG)
		AND (@TEN_KHACH_HANG IS NULL OR @TEN_KHACH_HANG = '' OR TEN_KHACH_HANG LIKE '%' + @TEN_KHACH_HANG + '%')
		AND (@MA_KHO = 0 OR MA_KHO_XUAT = @MA_KHO)		
		AND (@TEN_KHO IS NULL OR @TEN_KHO = '' OR UPPER(KHO.TEN_KHO) LIKE '%' + UPPER(@TEN_KHO) + '%')
		AND V_HOA_DON.MA_KHO_XUAT = KHO.MA_KHO
		AND (@FROM_DATE IS NULL OR CAST(NGAY_BAN AS DATE) >= CAST(@FROM_DATE AS DATE))
		AND (@TO_DATE IS NULL OR CAST(NGAY_BAN AS DATE) <= CAST(@TO_DATE AS DATE)) 
END