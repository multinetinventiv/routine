function configureValidator(rules) {
    $('form.operation-form').each(function () {
        if ($('.wrapper').children('fieldset.search-data').length <= 0
                && $(this).children().children().children("dl:first").children().children('input[type="text"]').length > 0) {
            $(this).attr('areinputsvalid', 'false');
        } else {
            $(this).attr('areinputsvalid', 'true');
        }
        $(this).bind('reset', function () {
            $(this).children().children().children("dl:first").children().children('div.error').each(function () {
                $(this).remove();
            });
        });
    });
    $('form.operation-form').submit(function (event) {
        $(this).children().children().children("dl:first").children().children('div.error').each(function () {
            $(this).remove();
        });
        if ($(this).attr('areinputsvalid').indexOf("false") > -1) {
            $(this).validate(event, rules);
        }
    });
}
;

$.fn.validate = function (event, rules) {
    var canSubmit = $(this).attr('areinputsvalid');
    event.preventDefault();
    $(this).children().children().children("dl:first").children().children('input[type="text"]').each(function () {
        $(this).chI(rules);

    });
    if ($(this).find('div.error').length <= 0) {
        canSubmit = true;
    }
    $(this).attr('areinputsvalid', canSubmit);
    if ($(this).attr('areinputsvalid').indexOf("true") > -1) {
        $(this).submit();
    }
};

$.fn.chI = function (rules) {
    if ($(this).attr('type').localeCompare("hidden") != 0) {
        for (var i = 0; i < rules.length; i++) {
            if ($(this).attr('data-type').indexOf(rules[i].datatype) > -1) {
                var retVal = $(this).matchRegex(rules[i].regex);
                if (retVal == false) {
                    $(this).displayErrorMessage($(this).closest('dd').prev('dt').html() + ' ' + rules[i].errorMessage);
                }
            }
        }

    }
};

$.fn.matchRegex = function (regex) {
    var rgx = new RegExp(regex, "g");
    var value = $(this).context.value;
    if (value.length <= 0) {
        return false;
    }
    var boo = rgx.test(value);
    if (boo) {
        return true;
    }
    return false;

};

$.fn.displayErrorMessage = function (message) {
    if ($(this).closest('dd').children('div.error').length <= 0) {
        $(this).closest('dd').append('<div class="error">' + message + '</div>');
        $(this).closest('dd').children('div.error').css('color', 'white');
        $(this).closest('dd').children('div.error').css('background-color', 'red');
        $(this).closest('dd').children('div.error').css('height', '18px');
        $(this).closest('dd').children('div.error').css('padding', '1px');
        $(this).closest('dd').children('div.error').css('width', message.length * 5 + 50 + 'px');
        $(this).closest('dd').children('div.error').css('z-index', '100');
        $(this).closest('dd').children('div.error').css('position', 'absolute');
        $(this).closest('dd').children('div.error').css('top', $(this).position().top + 'px');
        $(this).closest('dd').children('div.error').css('left', $(this).position().left + 310 + 'px');
    } else {
        $('div.error').value = message;
    }
};