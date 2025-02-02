﻿
$(document).ready(function () {

    $("#typeCustomers").val($("#kind").val());
    percentKeyPress();
    numberOnly();
    productAutocomplete();
    productCodeAutocomplete();
    quantityKeyPress();
    priceKeyPress();
    tableCheck();
    headerCheck();
    unitOnchange();
    unitChange();
    productIdChange();
    getAllTotal();
    getNumberOfRow();
    var customerDebit = $("#myCustomerDebit").val();
    if (customerDebit != null && customerDebit != '' && customerDebit > 0) {
        $("#DebitInfor").show();
        $("#CustomerDebit").val(customerDebit);
    } else {
        $("#DebitInfor").hide();
    }
    formatNumberic();
    codeEnter();
    priceEnter();
    addArrowKeys();
    quantityEnter();
    percentEnter();

    //add catch event user press Ctr+S & Ctrl+Shift+S
    $(window).bind('keydown', function (e) {
        if ((e.which == '115' || e.which == '83') && (e.ctrlKey || e.metaKey)) {
            // Ctrl + S
            returnSubmit();
            return false;
        }
        return true;
    });
});


//Stop Form Submission of Enter Key Press
$("#customerName").autocomplete({
    source: function (request, response) {
        $.ajax({
            //url: "/KhachHang/Find", data: "{ 'prefixText': '" + request.term + "' }",
            url: "/KhachHang/FindDetailCustomer", data: "{ 'prefixText': '" + request.term + "' }",
            dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
            dataFilter: function (data) { return data; }, success:
                function (data) {
                    response($.map(data, function (item) {
                        return {
                            lable: item.name,
                            value: item.name,
                            id: item.id,
                            cardNo: item.cardNo,
                            address: item.address,
                            fone: item.fone,
                            email: item.mail,
                            kind: item.kind,
                            debit: item.debit
                        }
                    }))
                }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
        });
        $('#customerId').val("");
        $("#DebitInfor").hide();
        $("#typeCustomers").val(3);
    },
    select: function (event, ui) {
        $('#customerName').val(ui.item.label);
        $('#customerId').val(ui.item.id);
        $("#typeCustomers").val(ui.item.kind);
        if (ui.item.debit != '' && ui.item.debit != null && ui.item.debit > 0) {
            $("#CustomerDebit").val(ui.item.debit);
            $("#DebitInfor").show();
            formatNumberic();
        } else {
            $("#CustomerDebit").val(0);
            $("#DebitInfor").hide();
        }
        return false;
    },
    minLength: 1
});

$(document).ready(function () {
    $('.datePicker').datepicker({
        dateFormat: 'dd/mm/yy',
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+0"
    });
});


function returnSubmit() {
    var row = 0;
    var rval = 1;
    var pval = 1;
    var ppval = 1;
    var checkFlg = 0;
    $('#message').empty();
    var errorMessage = '';

    if ($("#returnDate").val() == null || $("#returnDate").val().trim() == "") {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += "Chưa nhập ngày trả hàng. Vui lòng chọn ngày trả hàng.";

    }

    if ($("#customerName").val() == null || $("#customerName").val().trim() == "") {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += "Chưa nhập tên khách hàng. Vui lòng nhập tên khách hàng.";
    }

    $('input.chcktbl').each(function () {
        var flg = $('input.delFlg', $(this).parent().parent()).val();
        if (flg != 1) {
            var productId = $('input.productId', $(this).parent().parent()).val().replace(/,/gi, "");
            if (productId != null && productId != '' && productId != '0') {
                var quantity = $('input.quantity', $(this).parent().parent()).val().replace(/,/gi, "");
                var price = $('input.price', $(this).parent().parent()).val().replace(/,/gi, "");
                var productId = $('input.productId', $(this).parent().parent()).val().replace(/,/gi, "");
                if (quantity == null || quantity == '' || quantity == 0) {
                    rval = 0;
                }
                if (price == null || price == '' || price == 0) {
                    pval = 0;
                }
                if (productId == null || productId == '' || productId == 0) {
                    ppval = 0;
                }
                row++;
            }
        }
    });

    if (ppval == 0) {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += "Có ít nhất 1 sản phẩm không có trong danh mục sản phẩm. Vui lòng kiểm tra lại.";

    }

    if (pval == 0) {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += "Có ít nhất 1 mặt hàng với giá nhập là 0 hoặc không được nhập đơn giá. Vui lòng kiểm tra lại.";

    }

    if (row == 0) {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += "Hóa đơn trả không có mặt hàng nào. Vui lòng kiểm tra lại.";

    }
    else if (rval == 0) {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += "Có ít nhất 1 mặt hàng với số lượng nhập là 0, hoặc không được nhập số lượng nhập. Vui lòng kiểm tra lại.";

    }

    if (errorMessage != '') {
        $('#message').append(errorMessage);
        $('#message').append("<hr/>");
        return false;
    } else {
        $('#saveFlg').val(1);
        $('#index').submit();
    }

};

function tableCheck() {
    $('.chcktbl').click(function () {
        var length = $('.chcktbl:checked').length;
        var max = $('.chcktbl').length;
        if (length == max) {
            $('#chckHead').prop('checked', true);
        }
        else {
            $('#chckHead').prop('checked', false);
        }
    });
}

function headerCheck() {
    $('#chckHead').click(function () {
        if (this.checked == false) {
            $('input.chcktbl:checked').prop('checked', false);
        }
        else {
            $('input.chcktbl:not(:checked)').prop('checked', true);
        }
    });
}


function deleteCheckedRow() {
    var rows = 0;
    $('input.chcktbl:checked').each(function () {
        var flg = $('input.delFlg', $(this).parent().parent());
        if (flg.val() == null || flg.val() == '' || flg.val() == 0) {
            rows++;
        }
        flg.val(1);
        $(this).parent().parent().hide();

    });
    if (rows == 0) {
        alert("Vui lòng chọn dòng để xóa.");
    }
    getNumberOfRow();
    getAllTotal();
};

function RemoveRougeChar(convertString) {
    if (convertString.substring(0, 1) == ",") {
        return convertString.substring(1, convertString.length)
    }
    return convertString;
}

function NumbericOnly(ctrl) {
    $(ctrl).keypress(function (key) {
        //getting key code of pressed key
        var keycode = (key.which) ? key.which : key.keyCode;
        //comparing pressed keycodes

        if (keycode > 31 && (keycode < 48 || keycode > 57) && keycode != 46) {
            alert(" You can enter only characters 0 to 9 ");
            return false;
        }
        else return true;
    });

}

function formatNumberic() {
    $('input.numberic').each(function () {
        var $this = $(this);
        var AmountWithCommas = $this.val();
        var DecimalSeparator = Number("1.2").toLocaleString().substr(1, 1);
        var arParts = String(AmountWithCommas).split(DecimalSeparator);

        var intPart = arParts[0];
        var decPart = (arParts.length > 1 ? arParts[1] : '');

        var num = intPart.replace(/,/gi, "").split("").reverse().join("");
        var num2 = RemoveRougeChar(num.replace(/(.{3})/g, "$1,").split("").reverse().join(""));
        if (decPart.length > 2) {
            decPart = (decPart + '00').substr(0, 2);
        }
        if (arParts.length > 1) {
            $this.val(num2 + DecimalSeparator + decPart);
        } else {
            $this.val(num2);
        }
    });
}

function numberOnly() {
    $('input.numberic').keypress(function (key) {
        var keycode = (key.which) ? key.which : key.keyCode;
        //comparing pressed keycodes

        if (keycode > 31 && (keycode < 48 || keycode > 57) && keycode != 46) {
            return false;
        }
        else return true;

    });
    $('input.numberic').keyup(function (event) {
        // skip for arrow keys
        if (event.which >= 37 && event.which <= 40) {
            //event.preventDefault();
            return true;
        }
        var $this = $(this);
        var AmountWithCommas = $this.val();
        var DecimalSeparator = Number("1.2").toLocaleString().substr(1, 1);
        var arParts = String(AmountWithCommas).split(DecimalSeparator);

        var intPart = arParts[0];
        var decPart = (arParts.length > 1 ? arParts[1] : '');

        var num = intPart.replace(/,/gi, "").split("").reverse().join("");
        var num2 = RemoveRougeChar(num.replace(/(.{3})/g, "$1,").split("").reverse().join(""));
        if (decPart.length > 2) {
            decPart = (decPart + '00').substr(0, 2);
        }
        if (arParts.length > 1) {
            $this.val(num2 + DecimalSeparator + decPart);
        } else {
            $this.val(num2);
        }
    });
};


function GetFactorOfProduct(productId, unitId, convertor) {
    if (unitId != null && unitId != 'undefined') {
        $.ajax({
            url: "/BanHang/FindFactorOfProduct1",
            //data: "{'maSP' : '" + productNo + "'}",
            data: "{ 'maSP': '" + productId + "' , 'unitNo' : '" + unitId + "'}",
            dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
            success: function (data) {
                var parent = convertor.parent().parent();
                var kind = $("#typeCustomers").val();
                convertor.val(data.heso);
                if (kind == 1) {
                    $("input.price", parent).val(data.gia_ban_1 - data.chiec_khau_1 * data.gia_ban_1 / 100);
                    //$("input.discount", parent).val(data.chiec_khau_1);
                } else if (kind == 2) {
                    $("input.price", parent).val(data.gia_ban_2 - data.chiec_khau_2 * data.gia_ban_2 / 100);
                    //$("input.discount", parent).val(data.chiec_khau_2);
                } else {
                    $("input.price", parent).val(data.gia_ban_3 - data.chiec_khau_3 * data.gia_ban_3 / 100);
                    //$("input.discount", parent).val(data.chiec_khau_3);
                }


                var discount = $('input.percent', parent).val();
                var quality = $('input.quantity', parent).val();
                var price = $('input.price', parent).val();
                if (discount != '' && quality != '' && price != '') {
                    discount = discount.replace(/,/gi, "");
                    quality = quality.replace(/,/gi, "");
                    price = price.replace(/,/gi, "");
                    var total = quality * price;
                    var realTotal = total - total * discount / 100;
                    $('input.total', parent).val(realTotal);
                    //$('input.realTotal', parent).val(realTotal);
                    formatNumberic();
                }
                // counter();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
        });
    }
}

function unitEnter() {
    $("select.unit").focusin(function (event) {
        var $this = $(this);
        var parent = $this.parent().parent();
        var productId = $('input.productId', parent);
        createDonViTinh(productId);
    });
}


function productCodeAutocomplete() {
    $("input.code").autocomplete({
        source: function (request, response) {
            $.ajax({
                // url: "/SanPham/FindSuggestForReturn", data: "{ 'prefixText': '" + request.term + "' }",
                url: "/SanPham/FindSuggestByCode", data: "{ 'prefixText': '" + request.term + "' , 'typeCustomer' : '" + $("#typeCustomers").val() + "'}",
                dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
                dataFilter: function (data) { return data; }, success:
                    function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.code,
                                name: item.name,
                                code: item.code,
                                id: item.id,
                                price: item.price * (100 - item.discount) / 100
                            }
                        }))
                    }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
            });
            var $this = $(this);
            var parent = $this.parent().parent();
            $('input.productId', parent).val("");
            $('input.productname', parent).val("");
            $('input.price', parent).val("");
            $('input.convertor', parent).val("1");
        },
        select: function (event, ui) {
            var $th = $(this);
            var pa = $th.parent().parent();
            if (!checkDuplicate(ui.item.id, pa.index())) {
                $th.val(ui.item.label);
                $('input.productId', pa).val(ui.item.id);
                $('input.productname', pa).val(ui.item.name);
                $('input.price', pa).val(ui.item.price);
                createDonViTinh($th);
                $('input.convertor', pa).val("1");
            } else {
                alert("Sản phẩm này đã có trong danh sách");
                $th.val("");
                $('input.productId', pa).val("");
                $('input.productname', pa).val("");
                $('input.convertor', pa).val("1");
                $('input.price', pa).val("");
            }
            return false;
        },
        minLength: 1
    });
}

function priceKeyPress() {
    $('input.price').keyup(function (event) {
        var $this = $(this);
        var parent = $this.parent().parent();
        var price = $('input.quantity', parent).val().replace(/,/gi, "");
        var tt = $this.val().replace(/,/gi, "") * price;
        var percent = $('input.percent', parent).val().replace(/,/gi, "");
        tt = tt - tt * percent / 100;
        $('td input.total', parent).val(tt);
        getAllTotal();
        formatNumberic();
    });
}

function priceChange() {
    $('input.price').change(function (event) {
        var $this = $(this);
        var parent = $this.parent().parent();
        var price = $('input.quantity', parent).val().replace(/,/gi, "");
        var tt = $this.val().replace(/,/gi, "") * price;

        var DecimalSeparator = Number("1.2").toLocaleString().substr(1, 1);
        var arParts = String(tt).split(DecimalSeparator);
        var intPart = arParts[0];
        var decPart = (arParts.length > 1 ? arParts[1] : '');
        var num = intPart.replace(/,/gi, "").split("").reverse().join("");
        var num2 = RemoveRougeChar(num.replace(/(.{3})/g, "$1,").split("").reverse().join(""));
        if (decPart.length > 2) {
            decPart = (decPart + '00').substr(0, 2);
        }
        if (arParts.length > 1) {
            $('td input.total', parent).val(num2 + DecimalSeparator + decPart);
        } else {
            $('td input.total', parent).val(num2);
        }
        getAllTotal();
    });
}


function quantityKeyPress() {
    $('input.quantity').keyup(function (event) {
        var $this = $(this);
        var parent = $this.parent().parent();
        var price = $('input.price', parent).val().replace(/,/gi, "");
        var percent = $('input.percent', parent).val().replace(/,/gi, "");
        var tt = $this.val().replace(/,/gi, "") * price;
        tt = tt - tt * percent / 100;
        $('td input.total', parent).val(tt);
        getAllTotal();
        formatNumberic();
    });
}

function quantityChange() {
    $('input.quantity').change(function (event) {
        var $this = $(this);
        var parent = $this.parent().parent();
        var price = $('input.price', parent).val().replace(/,/gi, "");
        var tt = $this.val().replace(/,/gi, "") * price;

        var DecimalSeparator = Number("1.2").toLocaleString().substr(1, 1);
        var arParts = String(tt).split(DecimalSeparator);
        var intPart = arParts[0];
        var decPart = (arParts.length > 1 ? arParts[1] : '');
        var num = intPart.replace(/,/gi, "").split("").reverse().join("");
        var num2 = RemoveRougeChar(num.replace(/(.{3})/g, "$1,").split("").reverse().join(""));
        if (decPart.length > 2) {
            decPart = (decPart + '00').substr(0, 2);
        }
        if (arParts.length > 1) {
            $('td input.total', parent).val(num2 + DecimalSeparator + decPart);
        } else {
            $('td input.total', parent).val(num2);
        }
        getAllTotal();
    });
}

function checkDuplicate(val, p_index) {
    var flg = false;
    var index = -1;
    $('td input.productId').each(function () {
        index = $(this).parent().parent().index();
        if ($('input.delFlg', $(this).parent()).val() != 1) {
            var checkVal = $(this).val();
            if (val == checkVal && p_index != index) {
                flg = true;
            }
        }
    })
    return flg;
}

function productAutocomplete() {
    $("input.productname").autocomplete({
        source: function (request, response) {
            $.ajax({
                //url: "/SanPham/FindSuggestForReturn", data: "{ 'prefixText': '" + request.term + "' }",
                url: "/SanPham/FindSuggestByTypeCustomer", data: "{ 'prefixText': '" + request.term + "' , 'typeCustomer' : '" + $("#typeCustomers").val() + "'}",
                dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
                dataFilter: function (data) { return data; }, success:
                    function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.name,
                                value: item.name,
                                code: item.code,
                                price: item.price - item.price * item.discount / 100,
                                id: item.id
                            }
                        }))
                    }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
            });
            var $this = $(this);
            var parent = $this.parent().parent();
            $('input.productId', parent).val("");
            $('input.code', parent).val("");
            $('input.price', parent).val("");
            $('input.convertor', parent).val("1");
        },
        select: function (event, ui) {
            var $th = $(this);
            var pa = $th.parent().parent();
            if (!checkDuplicate(ui.item.id, pa.index())) {
                $th.val(ui.item.label);
                $('input.productId', pa).val(ui.item.id);
                $('input.code', pa).val(ui.item.code);
                $('input.price', pa).val(ui.item.price);
                $('input.convertor', pa).val("1");
                createDonViTinh($th);
            } else {
                alert("Sản phầm này đã được nhập.");
                $th.val("");
                $('input.productId', pa).val("");
                $('input.code', pa).val("");
                $('input.price', pa).val("");
                $('input.convertor', pa).val("1");
            }
            return false;
        },
        minLength: 1
    });
}

function createDonViTinh(obj) {
    var result = '';
    var $th = obj;
    var pr = $th.parent().parent();
    var productId = $('input.productId', pr).val();
    $.ajax({
        url: "/BanHang/FindDonViTinhByMaSP",
        data: "{'maSP' : '" + productId + "'}",
        dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
        success:
            function (data) {
                var $this = obj;
                var parent = $this.parent().parent();
                var un = $('select.unit', parent);
                $('select.unit', parent).empty();
                $.each(data, function (i, item) {
                    var opt = "<option value=" + item.MA_DON_VI + ">" + item.TEN_DON_VI + "</option>"
                    $(opt).appendTo(un);
                });
                var unitId = $('input.unitTemp', parent).val();
                $('select.unit', parent).val(unitId);
            },
        error:
            function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
    });
}

function getNumberOfRow() {
    var row = 0;
    $('td input.quantity').each(function () {
        var $this = $(this);
        var $parent = $this.parent().parent();
        if ($('input.delFlg', $parent).val() != 1) {
            row++;
        }
    });
    $('strong.frows').text(row);
};



function getAllTotal() {
    var AllTotal = 0;
    var total = 0;
    $('td input.total').each(function () {
        $this = $(this);
        $parent = $this.parent().parent();
        if ($('input.delFlg', $parent).val() != 1) {
            total = $this.val().replace(/,/gi, "");
            if (total != null && total != '') {
                AllTotal += parseFloat(total);
            }
        }
    });
    $("#totalOfBill").val(AllTotal);
    var debit = $("#myCustomerDebit").val();
    if (debit != null && debit != '') {
        debit = debit.replace(/,/gi, "");
        if (debit >= AllTotal) {
            $("#totalReturn").val("0");
        } else {
            $("#totalReturn").val(AllTotal - debit);
        }
    }
    formatNumberic();
    $('strong.fall').text($("#totalOfBill").val());
};

function productIdChange() {
    $("input.productId").each(function () {
        var $this = $(this);
        createDonViTinh($this);
    });
}

function unitOnchange() {
    $("select.unit").change(function (event) {
        var $this = $(this);
        var parent = $this.parent().parent();
        var productId = $('input.productId', parent).val();
        var unitId = $this.val();
        var convertor = $('input.convertor', parent);
        if (productId != null && productId != '' && unitId != null && unitId != '') {
            GetFactorOfProduct(productId, unitId, convertor);
        }
    });
}

function unitChange() {
    $("input.unitTemp").each(function () {
        var $this = $(this);
        var value = $this.val();
        var parent = $this.parent().parent();
        var unitId = $('select.unit', parent).val(value);
    });
}

function percentKeyPress() {
    $('input.percent').keyup(function (event) {
        var $this = $(this);
        var parent = $this.parent().parent();
        var quantity = $('input.quantity', parent).val().replace(/,/gi, "");
        var price = $('input.price', parent).val().replace(/,/gi, "");
        var percent = $this.val().replace(/,/gi, "");

        var tt = quantity * price;
        tt = tt - tt * percent / 100;
        $('td input.total', parent).val(tt);
        getAllTotal();
        formatNumberic();
    });
}





function addRow() {
    var row = parseInt($("#rowIndex").val()) + 1;
    $('#detailTable > tbody:last').append('<tr> ' +
        '<td class="inner alignCenter colwidth" width="5%;">' +
        '<input type="checkbox" class="arrowkey chcktbl"> </td>' +
        '<td class="inner colwidth">' +
        '<input name="Detail[' + row + '].CODE" class="arrowkey code textbt" id="Detail_' + row + '__CODE" role="textbox" aria-haspopup="true" aria-autocomplete="list" type="text" value="" autocomplete="off">' +
        '</td>' +
        '<td class="inner colwidth">' +
        '<input name="Detail[' + row + '].DEL_FLG" class="delFlg" id="Detail_' + row + '__DEL_FLG" type="hidden" value="" data-val="true" data-val-number="The field DEL_FLG must be a number.">' +
        '<input name="Detail[' + row + '].MA_SAN_PHAM" class="productId " id="Detail_' + row + '__MA_SAN_PHAM" type="hidden" value="" data-val="true" data-val-number="The field MA_SAN_PHAM must be a number.">' +
        '<input name="Detail[' + row + '].HE_SO" class="convertor" id="Detail_' + row + '__HE_SO" type="hidden" value="" data-val="true" data-val-number="The field HE_SO must be a number.">' +
        '<input name="Detail[' + row + '].TEN_SAN_PHAM" class="arrowkey productname textbtl" id="Detail_' + row + '__TEN_SAN_PHAM" type="text" value=""> </td>' +
        '<td class="inner colwidth">' +
        '<input name="Detail[' + row + '].SO_LUONG" class="arrowkey quantity numberic textbtl" id="Detail_' + row + '__SO_LUONG" type="text" value="" data-val="true" data-val-number="The field SO_LUONG_TEMP must be a number."> </td>' +
        '<td class="inner colwidth">' +
        '<select name="Detail[' + row + '].MA_DON_VI" class="arrowkey unit" id="Detail_' + row + '__MA_DON_VI" style="padding: 5px; font-size: 1.2em;width:100%;" data-val="true" data-val-number="The field MA_DON_VI must be a number.">' +
        '<option value="">---------</option></select> </td>' +
        '<td class="inner colwidth">' +
        '<input name="Detail[' + row + '].DON_GIA" class="arrowkey price numberic textbtl" id="Detail_' + row + '__DON_GIA" type="text" value="" data-val="true" data-val-number="The field GIA_VON must be a number."> </td>' +
        '<td class="innerLast colwidth">' +
        '<input name="Detail[' + row + '].PHAN_TRAM_KHAU_HAO" class="arrowkey percent numberic textbtl" id="Detail_' + row + '__PHAN_TRAM_KHAU_HAO" type="text" value="" data-val="true" data-val-number="The field PHAN_TRAM_KHAU_HAO must be a number."> </td>' +
        '<td class="innerLast colwidth">' +
        '<input name="Detail[' + row + '].THANH_TIEN" disabled="disabled" class="total textbtlReturn numberic" id="Detail_' + row + '__THANH_TIEN" type="text" readonly="True" value="" data-val-required="The THANH_TIEN field is required." data-val="true" data-val-number="The field THANH_TIEN must be a number."> </td>' +
        '</tr>');
    $("#rowIndex").val(row);
    $('input.code', $('#detailTable > tbody:last')).focus();
    numberOnly();
    productAutocomplete();
    productCodeAutocomplete();
    getNumberOfRow();
    quantityKeyPress();
    priceKeyPress();
    unitOnchange();
    tableCheck();
    headerCheck();
    percentKeyPress();
    codeEnter();
    priceEnter();
    addArrowKeys();
    quantityEnter();
    percentEnter();
};



function productNameEnter() {
    $('input.productname').keypress(function (event) {
        var $this = $(this);
        var $parent = $(this).parent().parent();
        var last = $('#detailTable >tbody >tr:visible').last().index();
        if (event.which === 13 || event.which === 27) {
            $('input.quantity', $parent).focus();
        }
    });
}


function priceEnter() {
    $('input.price').keypress(function (event) {
        var $this = $(this);
        var $parent = $(this).parent().parent();
        var last = $('#detailTable >tbody >tr:visible').last().index();
        if (event.which === 13 || event.which === 27) {
            $('input.percent', $parent).focus();
        }
    });
}

function percentEnter() {
    $('input.percent').keypress(function (event) {
        var $this = $(this);
        var $parent = $(this).parent().parent();
        var last = $('#detailTable >tbody >tr:visible').last().index();
        if (event.which === 13 || event.which === 27) {
            if (last == $parent.index()) {
                addRow();
            } else {
                $('input.code', $('#detailTable > tbody:last')).focus();
            }
        }
    });
}

function quantityEnter() {
    $('input.quantity').keypress(function (event) {
        var $this = $(this);
        var $parent = $(this).parent().parent();
        var showFlg = $("#showFlg").val();
        var last = $('#detailTable >tbody >tr:visible').last().index();
        if (event.which === 13 || event.which === 27) {
            $('input.price', $parent).focus();
        }
    });
}


function addArrowKeys() {
    $('.arrowkey').keydown(function (e) {
        var keyCode = e.keyCode || e.which,
            arrow = { left: 37, up: 38, right: 39, down: 40 };

        switch (keyCode) {
            case arrow.left:
                var preColum = $(this).closest('td').prev().find('.arrowkey');
                while (preColum != null && preColum.is(':hidden')) {
                    preColum = preColum.closest('td').prev().find('.arrowkey')
                }
                preColum.focus();
                break;
            case arrow.up:
                // row n + 1
                var prevrow = $(this).closest('tr').prev();
                while (prevrow != null && prevrow.is(':hidden')) {
                    prevrow = prevrow.closest('tr').prev();
                }

                if (prevrow != null && prevrow.length > 0) {
                    var currClass = $(this).attr('class');
                    // get next Element to focus by class 
                    var arrControl = prevrow.find('.arrowkey');
                    for (i = 0; i < arrControl.length; i++) {
                        var control = arrControl[i];
                        if (control.className.trim() == currClass.trim()) {
                            control.focus();
                            return;
                        }

                    }

                }
                break;
            case arrow.right:
                var nextColumn = $(this).closest('td').next().find('.arrowkey').not('.disabled');
                while (nextColumn != null && nextColumn.is(':hidden')) {
                    nextColumn = nextColumn.closest('td').next().find('.arrowkey').not('.disabled');
                }
                nextColumn.focus();
                break;
            case arrow.down:
                // row n + 1
                var nextrow = $(this).closest('tr').next();
                while (nextrow != null && nextrow.is(':hidden')) {
                    nextrow = nextrow.closest('tr').next();
                }
                if (nextrow != null && nextrow.length > 0) {
                    var currClass = $(this).attr('class');
                    // get next Element to focus by class 
                    var arrControl = nextrow.find('.arrowkey');
                    for (i = 0; i < arrControl.length; i++) {
                        var control = arrControl[i];
                        if (control.className.trim() == currClass.trim()) {
                            control.focus();
                            return;
                        }

                    }

                }
                break;
        }
    });
}


function codeEnter() {
    $('input.code').keypress(function (event) {
        var $this = $(this);
        if (event.which === 13 || event.which === 27) {
            if ($this.val().length >= 1) {
                $.ajax({
                    url: "/SanPham/FindSuggestFirstbyCode", data: "{ 'prefixText': '" + $this.val() + "' , 'typeCustomer' : '" + $("#typeCustomers").val() + "'}",
                    //url: "/SanPham/FindSuggestByCode", data: "{ 'prefixText': '" + request.term + "' , 'typeCustomer' : '" + $("#typeCustomers").val() + "'}",
                    dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
                    dataFilter: function (data) { return data; },
                    success:
                        function (data) {
                            $.each(data, function (i, item) {
                                //alert(item.id);
                                var $th = $this;
                                var pa = $th.parent().parent();
                                if (!checkDuplicate(item.id, pa.index())) {
                                    $th.val(item.label);
                                    $('input.productId', pa).val(item.id);
                                    $('input.productname', pa).val(item.name);
                                    $('input.quantity', pa).val("");
                                    createDonViTinh($th);
                                    formatNumberic();
                                    $('input.convertor', pa).val("1");
                                    $th.css("background-color", "white");
                                    $('input.quantity', pa).focus();
                                    $('input.price', pa).val(item.price);
                                    $('input.percent', pa).val(item.discount);
                                } else {
                                    alert("Sản phẩm này đã có trong danh sách");
                                    $th.val("");
                                    $('input.productId', pa).val("");
                                    $('input.productname', pa).val("");
                                    $('input.convertor', pa).val("1");
                                    $('input.quantity', pa).val("");
                                }
                                return false;
                            });
                        },
                    error:
                        function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
                });
            }
        };
    });
}