$(document).ready(function () {

    formatNumberic();
    numberOnly();
    productAutocomplete();
    productCodeAutocomplete();
    unitOnchange();
    tableCheck();
    headerCheck();
    addArrowKeys();
    quantityEnter();
    codeEnter();
});




function returnSubmit() {

    $('#message').empty();
    var errorMessage = '';
    var row = 0;
    var rval = 1;
    var pval = 1;
    var ppval = 1;
    var checkFlg = 0;

    if ($("#importDate").val() == null || $("#importDate").val().trim() == "") {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += ("Chưa nhập ngày xuất kho. Vui lòng chọn ngày xuất kho.");
    }
    if ($("#storeId option:selected").val() == null || $("#storeId option:selected").val().trim() == "") {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += "Chưa chọn kho xuất, vui lòng chọn kho xuất";
    }
    $('input.chcktbl').each(function () {
        var flg = $('input.delFlg', $(this).parent().parent()).val();
        if (flg != 1) {
            
            var productId = $('input.productId', $(this).parent().parent()).val().replace(/,/gi, "");
            if (productId != null && productId != '' && productId != '0') {
                var quantity = $('input.quantity', $(this).parent().parent()).val().replace(/,/gi, "");
                if (quantity == null || quantity == '' || quantity == 0) {
                    rval = 0;
                }
                if (productId == null || productId == '' || productId == 0) {
                    ppval = 0;
                }
                row++;
            }
        }
    });

   

    if (row == 0) {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += ("Hóa đơn trả không có mặt hàng nào. Vui lòng kiểm tra lại.");
    } else {
        if (rval == 0) {
            if (errorMessage != '') {
                errorMessage += "<br>";
            }
            errorMessage += ("Có ít nhất 1 mặt hàng với số lượng xuất là 0, hoặc không được nhập số lượng xuất. Vui lòng kiểm tra lại.");
        }

        if (ppval == 0) {
            if (errorMessage != '') {
                errorMessage += "<br>";
            }
            errorMessage += ("Có ít nhất 1 dòng chưa nhập tên sản phẩm, hoặc sản phẩm không có trong danh mục sản phẩm. Vui lòng kiểm tra lại.");
        }

    }
    
    if (errorMessage != '') {
        $('#message').append(errorMessage);
        $('#message').append("<hr/>");
        return false;
    } else {
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
            url: "/BanHang/FindFactorOfProduct",
            data: "{ 'maSP': '" + productId + "' , 'unitNo' : '" + unitId + "'}",
            dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
            success: function (data) {
                convertor.val(data.heso);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
        });
    }
}

function unitOnchange() {
    $("select.unit").change(function (event) {
        var $this = $(this);
        var parent = $this.parent().parent();
        var productId = $('input.productId', parent).val();
        var unitId = $('select.unit :selected').val();
        var convertor = $('input.convertor', parent);
        if (productId != null && productId != '' && unitId != null && unitId != '') {
            GetFactorOfProduct(productId, unitId, convertor);
        }
    });
}

function productCodeAutocomplete() {
    $("input.code").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/SanPham/FindSuggestByCd", data: "{ 'prefixText': '" + request.term + "' }",
                dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
                dataFilter: function (data) { return data; }, success:
                    function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.value,
                                name: item.name,
                                value: item.value,
                                id: item.id
                            }
                        }))
                    }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
            });
            var $this = $(this);
            var parent = $this.parent().parent();
            $('input.productId', parent).val("");
            $('input.productname', parent).val("");
            $('input.convertor', parent).val("1");
        },
        select: function (event, ui) {
            var $th = $(this);
            var pa = $th.parent().parent();
            if (!checkDuplicate(ui.item.id, pa.index())) {
                $th.val(ui.item.label);
                $('input.productId', pa).val(ui.item.id);
                $('input.productname', pa).val(ui.item.name);
                createDonViTinh($th);
                $('input.convertor', pa).val("1");
                $('input.quantity', pa).focus();
            } else {
                alert("Sản phẩm này đã có trong danh sách");
                $th.val("");
                $('input.productId', pa).val("");
                $('input.productname', pa).val("");
                $('input.convertor', pa).val("1");
            }
            return false;
        },
        minLength: 1
    });
}

//function priceKeyPress() {
//    $('input.price').keyup(function (event) {
//        var $this = $(this);
//        var parent = $this.parent().parent();
//        var price = $('input.quantity', parent).val().replace(/,/gi, "");
//        var tt = $this.val().replace(/,/gi, "") * price;

//        var DecimalSeparator = Number("1.2").toLocaleString().substr(1, 1);
//        var arParts = String(tt).split(DecimalSeparator);
//        var intPart = arParts[0];
//        var decPart = (arParts.length > 1 ? arParts[1] : '');
//        var num = intPart.replace(/,/gi, "").split("").reverse().join("");
//        var num2 = RemoveRougeChar(num.replace(/(.{3})/g, "$1,").split("").reverse().join(""));
//        if (decPart.length > 2) {
//            decPart = (decPart + '00').substr(0, 2);
//        }
//        if (arParts.length > 1) {
//            $('td input.total', parent).val(num2 + DecimalSeparator + decPart);
//        } else {
//            $('td input.total', parent).val(num2);
//        }
//        getAllTotal();
//    });
//}

//function quantityKeyPress() {
//    $('input.quantity').keyup(function (event) {
//        var $this = $(this);
//        var parent = $this.parent().parent();
//        var price = $('input.price', parent).val().replace(/,/gi, "");
//        var tt = $this.val().replace(/,/gi, "") * price;

//        var DecimalSeparator = Number("1.2").toLocaleString().substr(1, 1);
//        var arParts = String(tt).split(DecimalSeparator);
//        var intPart = arParts[0];
//        var decPart = (arParts.length > 1 ? arParts[1] : '');
//        var num = intPart.replace(/,/gi, "").split("").reverse().join("");
//        var num2 = RemoveRougeChar(num.replace(/(.{3})/g, "$1,").split("").reverse().join(""));
//        if (decPart.length > 2) {
//            decPart = (decPart + '00').substr(0, 2);
//        }
//        if (arParts.length > 1) {
//            $('td input.total', parent).val(num2 + DecimalSeparator + decPart);
//        } else {
//            $('td input.total', parent).val(num2);
//        }
//        getAllTotal();
//    });
//}

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
                url: "/SanPham/FindSuggestName", data: "{ 'prefixText': '" + request.term + "' }",
                dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
                dataFilter: function (data) { return data; }, success:
                    function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.value,
                                value: item.value,
                                code: item.code,
                                id: item.id
                            }
                        }))
                    }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
            });
            var $this = $(this);
            var parent = $this.parent().parent();
            $('input.productId', parent).val("");
            $('input.code', parent).val("");
            $('input.convertor', parent).val("1");
        },
        select: function (event, ui) {
            var $th = $(this);
            var pa = $th.parent().parent();
            if (!checkDuplicate(ui.item.id, pa.index())) {
                $th.val(ui.item.label);
                $('input.productId', pa).val(ui.item.id);
                $('input.code', pa).val(ui.item.code);
                $('input.convertor', pa).val("1");
                createDonViTinh($th);
                $('input.quantity', pa).focus();
            } else {
                alert("Sản phầm này đã được nhập.");
                $th.val("");
                $('input.productId', pa).val("");
                $('input.code', pa).val("");
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

    var DecimalSeparator = Number("1.2").toLocaleString().substr(1, 1);
    var arParts = String(AllTotal).split(DecimalSeparator);
    var intPart = arParts[0];
    var decPart = (arParts.length > 1 ? arParts[1] : '');
    var num = intPart.replace(/,/gi, "").split("").reverse().join("");
    var num2 = RemoveRougeChar(num.replace(/(.{3})/g, "$1,").split("").reverse().join(""));
    if (decPart.length > 2) {
        decPart = (decPart + '00').substr(0, 2);
    }
    if (arParts.length > 1) {
        $('strong.fall').text(num2 + DecimalSeparator + decPart);
    } else {
        $('strong.fall').text(num2);
    }
};




function addRow() {
    var row = parseInt($("#rowIndex").val()) + 1;
    $('#detailTable > tbody:last').append('<tr> ' +
        '<td class="inner alignCenter colwidth" width="5%;">' +
        '<input type="checkbox" class="chcktbl"> </td>' +
        '<td class="inner colwidth">' +
        '<input name="Detail[' + row + '].CODE" class="code textbtl ui-autocomplete-input" id="Detail_' + row + '__CODE" role="textbox" aria-haspopup="true" aria-autocomplete="list" type="text" value="" autocomplete="off">' +
        '</td>' +
        '<td class="inner colwidth">' +
        '<input name="Detail[' + row + '].DEL_FLG" class="delFlg" id="Detail_' + row + '__DEL_FLG" type="hidden" value="" data-val="true" data-val-number="The field DEL_FLG must be a number.">' +
        '<input name="Detail[' + row + '].MA_SAN_PHAM" class="productId " id="Detail_' + row + '__MA_SAN_PHAM" type="hidden" value="" data-val="true" data-val-number="The field MA_SAN_PHAM must be a number.">' +
        '<input name="Detail[' + row + '].HE_SO" class="convertor" id="Detail_' + row + '__HE_SO" type="hidden" value="" data-val="true" data-val-number="The field HE_SO must be a number.">' +
        '<input name="Detail[' + row + '].TEN_SAN_PHAM" class="productname textbtl" id="Detail_' + row + '__TEN_SAN_PHAM" type="text" value=""> </td>' +
        '<td class="inner colwidth">' +
        '<input name="Detail[' + row + '].SO_LUONG_TEMP" class="quantity textbtl numberic" id="Detail_' + row + '__SO_LUONG_TEMP" type="text" value="" data-val="true" data-val-number="The field SO_LUONG_TEMP must be a number."> </td>' +
        '<td class="innerLast colwidth">' +
        '<select name="Detail[' + row + '].MA_DON_VI" class="unit textbtl" id="Detail_' + row + '__MA_DON_VI" style="padding: 5px; font-size: 1.2em;" data-val="true" data-val-number="The field MA_DON_VI must be a number.">' +
        '<option value="">---------</option></select> </td>' +
        //'<td class="inner colwidth">' +
        //'<input name="Detail[' + row + '].GIA_VON" class="price textbtl numberic" id="Detail_' + row + '__GIA_VON" type="text" value="" data-val="true" data-val-number="The field GIA_VON must be a number."> </td>' +
        //'<td class="innerLast colwidth">' +
        //'<input name="Detail[' + row + '].THANH_TIEN" disabled="disabled" class="total textbtl numberic" id="Detail_' + row + '__THANH_TIEN" type="text" readonly="True" value="" data-val-required="The THANH_TIEN field is required." data-val="true" data-val-number="The field THANH_TIEN must be a number."> </td>' +
        '</tr>');
    $("#rowIndex").val(row);
    $('input.code', $('#detailTable > tbody:last')).focus();
    numberOnly();
    productAutocomplete();
    productCodeAutocomplete();
    getNumberOfRow();
    unitOnchange();
    tableCheck();
    headerCheck();
    addArrowKeys();
    quantityEnter();
    codeEnter();
};

function quantityEnter() {
    $('input.quantity').keypress(function (event) {
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



function codeEnter() {
    $('input.code').keypress(function (event) {
        var $this = $(this);
        if (event.which === 13 || event.which === 27) {
            if ($this.val().length >= 1) {
                $.ajax({
                    url: "/SanPham/FindSuggestFirstbyCode", data: "{ 'prefixText': '" + $this.val() + "' , 'typeCustomer' : '" + $("#kindCustomer").val() + "'}",
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
                                    counter();
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
};