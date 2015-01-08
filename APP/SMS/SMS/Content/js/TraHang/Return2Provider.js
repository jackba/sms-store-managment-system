$(document).ready(function () {
    formatNumberic();
    numberOnly();
    productAutocomplete();
    productCodeAutocomplete();
    quantityKeyPress();
    priceKeyPress();
    unitOnchange();
    tableCheck();
    headerCheck();
    $("#storeInformation").hide();
    $("input.datePicker").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", new Date());
    productNameEnter();
    priceEnter();
    quantityEnter();
    codeEnter();
    addArrowKeys();

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


function placeDiv(event) {
    var width = event.width();
    var offset = event.offset();
    offset.left = offset.left + width + 20;
    $('#storeInformation').show();
    $('#storeInformation').offset({ top: offset.top, left: offset.left })

}

function checkdlc() {
    var flg = false;
    var productIdes = $('td input.productId');
    var i = 0;
    var j = 1;
    for (i = 0; i < productIdes.length - 1; i++) {
        if (productIdes[i].value != '') {
            for (j = i + 1; j < productIdes.length; j++) {
                if (productIdes[j].value != '' && productIdes[j].value == productIdes[i].value) {
                    flg = true;
                    break;
                }
            }
        }
    }
    return flg;
}

function checkEnough(storeId, ProductId, quantity, convertor) {
    var inventorNumber = 0;
    if (storeId != null && ProductId != null && quantity != null && convertor != null) {
        $.ajax({
            url: "/Import/getInventory",
            data: "{ 'storeId': '" + storeId + "' , 'productId' : '" + ProductId + "'}",
            dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
            success: function (data) {
                var inventorNumber = data;
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
        });
        if (inventorNumber < (quantity * convertor)) {
            return false;
        } else {
            return true;
        };
    }
}

function returnSubmit() {
    $('#message').empty();
    var errorMessage = '';
    if ($("#exportDate").val() == null || $("#exportDate").val().trim() == "") {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += "Chưa nhập ngày lập phiếu. Vui lòng chọn ngày lập phiếu.";
    }
    if ($("#providerId option:selected").val() == null || $("#providerId option:selected").val().trim() == "") {
        if (errorMessage != '') {
            errorMessage += "<br>";
        }
        errorMessage += "Chưa chọn nhà cung cấp. Vui lòng chọn nhà cung cấp cần trả hàng.";
    }

    var row = 0;
    var rval = 1;
    var ppval = 1;
    $('input.chcktbl').each(function () {
        var flg = $('input.delFlg', $(this).parent().parent()).val();
        if (flg != 1) {
            var productId = $('input.productId', $(this).parent().parent()).val().replace(/,/gi, "");
            if (productId != null && productId != '' && productId != '0') {
                var quantity = $('input.quantity', $(this).parent().parent()).val().replace(/,/gi, "");
                var convertor = $('input.convertor', $(this).parent().parent()).val().replace(/,/gi, "");
                if (quantity == '') {
                    rval = 0;
                }
                quantity = parseFloat(quantity);
                convertor = parseFloat(convertor);
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
        errorMessage += "Hóa đơn trả không có mặt hàng nào. Vui lòng kiểm tra lại.";

    } else {
        if (rval == 0) {
            if (errorMessage != '') {
                errorMessage += "<br>";
            }
            errorMessage += "Có ít nhất 1 mặt hàng với số lượng trả là 0, hoặc không được nhập số lượng trả. Vui lòng kiểm tra lại.";

        }

        if (ppval == 0) {
            if (errorMessage != '') {
                errorMessage += "<br>";
            }
            errorMessage += "Có ít nhất 1 dòng chưa nhập tên sản phẩm, hoặc sản phẩm không có trong danh mục sản phẩm. Vui lòng kiểm tra lại.";

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
        var unitId = $this.val();
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
                createStore($th);
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
        focus: function (event, ui) {
            var $this = $(this);
            $('#proName').text(ui.item.name);
        },
        close: function () {
            $("#storeInformation").hide();
        },
        minLength: 1
    });
}

function priceKeyPress() {
    $('input.price').keyup(function (event) {
        var $this = $(this);
        var parent = $this.parent().parent();
        var quantity = $('input.quantity', parent).val();
        if (quantity == null || quantity == '') {
            quantity = 0;
        } else {
            quantity = quantity.replace(/,/gi, "");
        }

        var price = $this.val();
        if (price == null || price == '') {
            price = 0;
        } else {
            price = price.replace(/,/gi, "");
        }
        var tt = quantity * price;
        $('td input.total', parent).val(tt);
        getAllTotal();
        formatNumberic();
    });
}

function quantityKeyPress() {
    $('input.quantity').keyup(function (event) {
        var $this = $(this);
        var parent = $this.parent().parent();
        var price = $('input.price', parent).val();
        if (price == null || price == '') {
            price = 0;
        } else {
            price = price.replace(/,/gi, "");
        }
        var quantity = $this.val();
        if (quantity == null || quantity == '') {
            quantity = 0;
        } else {
            quantity = quantity.replace(/,/gi, "");
        }
        var tt = quantity * price;
        $('td input.total', parent).val(tt);
        getAllTotal();
        formatNumberic();
    });
}

function checkDuplicate(val, p_index) {
    var flg = false;
    var index = -1;
    $('td input.productId').each(function () {        
        index = $(this).parent().parent().index();
        if ($('input.delFlg', $(this).parent()).val() != 1 && p_index != index) {
            var checkVal = $(this).val();
            if (val == checkVal) {
                flg = true;
            }
        }
    })
    return flg;
}

function codeFocusout() {
    $("input.code").focusout(function () {
        var $this = $(this);
        var code = $this.val();
        var parent = $this.parent().parent();
        if (code != null && code.trim() != '') {
            $.ajax({
                url: "/SanPham/FindProductByCode",
                data: "{ 'code': '" + code + "' }",
                dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data != null && data.ma_san_pham != null && data.ma_san_pham != '') {
                        $('input.productId', parent).val(data.ma_san_pham);
                        $('input.productname', parent).val(data.ten_san_pham);
                        createDonViTinh($this);
                        $('input.convertor', parent).val("1");
                        $this.css("background-color", "white");
                    } else {
                        alert("Sản phẩm này không tồn tại.");
                        $this.val("");
                        $this.css("background-color", "yellow");
                        $this.focus();
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
            });
        }
    });
}

function getStoreInformation(storeId, ProductId) {
    $.ajax({
        url: "/SanPham/FindInventory",
        data: "{ 'storeId': '" + storeId + "' , 'productId' : '" + ProductId + "'}",
        dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
        success: function (data) {
            var inventorNumber = data.ton_kho;
            if (inventorNumber == '') {
                $('#stInfor').text("0");
            } else {
                $('#stInfor').text(inventorNumber);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
    });
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
                createStore($th);
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
        focus: function (event, ui) {
            var $this = $(this);
            $('#proName').text(ui.item.value);
        },
        close: function () {
            $("#storeInformation").hide();
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
        '<input type="checkbox" class="arrowkey chcktbl"> </td>' +
        '<td class="inner colwidth">' +
        '<input name="Details[' + row + '].CODE" class="arrowkey code textbtl" id="Details_' + row + '__CODE" role="textbox" aria-haspopup="true" aria-autocomplete="list" type="text" value="" autocomplete="off">' +
        '</td>' +
        '<td class="inner colwidth">' +
        '<input name="Details[' + row + '].DEL_FLG" class="delFlg" id="Details_' + row + '__DEL_FLG" type="hidden" value="" data-val="true" data-val-number="The field DEL_FLG must be a number.">' +
        '<input name="Details[' + row + '].MA_SAN_PHAM" class="productId " id="Details_' + row + '__MA_SAN_PHAM" type="hidden" value="" data-val="true" data-val-number="The field MA_SAN_PHAM must be a number.">' +
        '<input name="Details[' + row + '].HE_SO" class="convertor" id="Details_' + row + '__HE_SO" type="hidden" value="" data-val="true" data-val-number="The field HE_SO must be a number.">' +
        '<input name="Details[' + row + '].TEN_SAN_PHAM" class="arrowkey productname textbtl" id="Details_' + row + '__TEN_SAN_PHAM" type="text" value=""> </td>' +
        '<td class="inner colwidth">' +
        '<input name="Details[' + row + '].SO_LUONG_TEMP" class="arrowkey quantity textbtl numberic" id="Details_' + row + '__SO_LUONG_TEMP" type="text" value="" data-val="true" data-val-number="The field SO_LUONG_TEMP must be a number."> </td>' +
        '<td class="innerLast colwidth">' +
        '<select name="Details[' + row + '].MA_DON_VI" class="arrowkey unit" id="Details_' + row + '__MA_DON_VI" style="padding: 5px; font-size: 1.2em; width:100%" data-val="true" data-val-number="The field MA_DON_VI must be a number.">' +
        '<td class="innerLast colwidth"><input name="Details[' + row + '].DON_GIA_TEMP" class="arrowkey price numberic textbtl" id="Details_' + row + '__DON_GIA_TEMP" type="text" value="" data-val="true" data-val-number="The field DON_GIA_TEMP must be a number."></td>' +
        '<td class="innerLast colwidth"><input name="Details' + row + '].THANH_TIEN" class="arrowkey numberic total textbtl" id="Details_' + row + '__THANH_TIEN" type="text" value="" data-val="true" data-val-number="The field THANH_TIEN must be a number."></td>' +
        '<option value="">---------</option></select> </td>' +
        '<td class="innerLast colwidth">' +
        '<select name="Details[' + row + '].MA_KHO_XUAT" class="arrowkey store" id="Details_ + row + __MA_KHO_XUAT" style="padding: 5px; font-size: 1.2em; width:97%" data-val="true" data-val-number="The field MA_KHO_XUAT must be a number."><option value="">---------</option>' +
        '</select></td>' +
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
    formatNumberic();
    productNameEnter();
    priceEnter();
    quantityEnter();
    codeEnter();
    addArrowKeys();
};

function createStore(obj) {
    var result = '';
    $.ajax({
        url: "/BanHang/FindStore",
        dataType: "json", type: "POST", contentType: "application/json; charset=utf-8",
        success:
            function (data) {
                var $this = obj;
                var parent = $this.parent().parent();
                var un = $('select.store', parent);
                $('select.store', parent).empty();
                $.each(data, function (i, item) {
                    var opt = '';
                    if (item.value == parseInt($('#MyStore').val())) {
                        opt = "<option value=" + item.value + " selected>" + item.name + "</option>"
                    } else {
                        opt = "<option value=" + item.value + ">" + item.name + "</option>"
                    }
                    $(opt).appendTo(un);
                });
                un.val($("#myStore").val());
            },
        error:
            function (XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
    });
}

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
            addRow();
            $('input.code', $('#detailTable > tbody:last')).focus();
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
                                    createDonViTinh($th);
                                    createStore($th);
                                    $('input.convertor', pa).val("1");
                                    $('input.quantity', pa).focus();
                                    formatNumberic();
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