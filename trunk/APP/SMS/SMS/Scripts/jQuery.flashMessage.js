var $j = jQuery.noConflict();
$j.fn.flashMessage = function (options) {
    var target = this;
    options = $j.extend({}, options, { timeout: 3000, alert: 'info' });

    if (!options.message) {
        setFlashMessageFromCookie(options);
    }

    if (options.message) {
        $j(target).addClass(options.alert.toString().toLowerCase());

        if (typeof options.message === "string") {
            $j('p', target).html("<span>" + options.message + "</span>");
        } else {
            target.empty().append(options.message);
        }
    } else {
        return;
    }
    
    if (target.children().length === 0) return;

    target.fadeIn().one("click", function () {
        $j(this).fadeOut();
    });

    if (options.timeout > 0) {
        setTimeout(function () { target.fadeOut(); }, options.timeout);
    }

    return this;

    // Get the first alert message read from the cookie
    function setFlashMessageFromCookie() {
        $j.each(new Array('Success', 'Error', 'Warning', 'Info'), function (i, alert) {
            var cookie = $j.cookie("Flash." + alert);

            if (cookie) {
                options.message = cookie;
                options.alert = alert;

                deleteFlashMessageCookie(alert);
                return;
            }
        });
    }

    // Delete the named flash cookie
    function deleteFlashMessageCookie(alert) {
        $j.cookie("Flash." + alert, null, { path: '/' });
    }
};