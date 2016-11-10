$(document).ready(function() {
    Modernizr.load([{
            test: window.JSON,
            nope: 'Scripts/json2.min.js'
        },
        {
            test: !!Array.prototype.indexOf,
            nope: 'Scripts/indexOfShiv.min.js'
        }
    ]);

    //jquery.nano templating engine modified to match /:params
    (function($) {
        $.nano = function(template, data) {
            var result = { keys: [], template: "" };
            result.template = template.replace(/:([\w\.]*)/g, function(str, key) {
                result.keys.push(key);
                var keys = key.split("."),
                    value = data[keys.shift()];

                $.each(keys, function() {
                    value = value[this];
                });

                return (value === null || value === undefined) ? "" : value;
            });

            return result;
        };
    })(jQuery);

    //create details toggle event
    $('[data-details]').on('click', function(e) {
        $('form', e.target.parentElement).slideToggle("slow",
            function() { //refresh scroll spy to update with new page height.
                $('body').scrollspy('refresh');
            });
    });

    //create try it event
    $('[data-demo]').on('click', function(e) {
        e.preventDefault();
        $(e.target).addClass('disabled');

        var container = $('[data-codeContainer]', e.target.form)[0],
            form = $(e.target.form),
            url = $.attr(e.target, 'data-url'),
            valid = true,
            type = $.attr(e.target, 'data-type') || 'GET';

        //reset code container
        $(container).html('');

        //reset validation
        $('[data-required]', form).each(function() {
            $(this.parentElement.parentElement).removeClass("has-error");
        });

        //validate input
        $('[data-required]', form).each(function() {
            if (!this.value || $.trim(this.value) === "") {
                var element = $(this.parentElement.parentElement);
                element.addClass("has-error");

                $('html, body').animate({
                    scrollTop: $(this).offset().top - 200
                }, 1000);

                this.focus();
                valid = false;
            }
        });

        if (!valid) {
            $(e.target).removeClass('disabled');
            return false;
        }

        $('html, body').animate({
            scrollTop: $(e.target).offset().top - 200
        }, 1000);

        $(e.target).addClass('progress-bar');

        var d = new httpStatus(type, form);
        d.displaySampleUrl(url);

        var args = {
            type: type,
            url: d.getUrl(url),
            data: d.getData(form),
            dataType: d.getDataType(),
            timeout: 10000
        };

      $.ajax(args)
        .done(function(response) {
          $(container).html(d.displayResult(response));
          $(container).removeClass('hidden');
          $(e.target).removeClass('progress-bar disabled');
        })
        .fail(function (response) {
          if (response.responseText) {
            var json = JSON.parse(response.responseText);
            $(container).html(d.displayResult(json));
          } else {
            $(container).html('Request timed out.')
          }
          
          $(container).removeClass('hidden');
          $(e.target).removeClass('progress-bar disabled');
        });

        return false;
    });

    //create clear code window event
    $('[data-close]').on('click', function(e) {
        var container = $('[data-codeContainer]', e.target.parentElement)[0];
        $(container).html('');
        $('#sampleUrl', e.target.parentElement).html('').hide();
    });
});

var httpStatus = function(type, formNode) {
    this.type = type;
    this.template = {};
    this._form = formNode;
    this._formValues = this._form.serializeArray();
    this._postedData = {};
    this._queryStringPairs = {};

    this.getUrl = function(baseUrl) {
        console.info("getUrl");

        var templateResult = this._parseTemplate(baseUrl);
        this._cleanseFormData(templateResult, this._form);

        return templateResult.template + this._getUrl(this._queryStringPairs);
    };

    this._parseTemplate = function(templatedUrl) {
        console.info("_parseTemplate");

        var me = this;
        $.each(this._formValues, function() {
            me._queryStringPairs[this.name] = this.value;
        });

        //template replacement
        return $.nano(templatedUrl, this._queryStringPairs);
    };

    this._cleanseFormData = function(templateResult, form) {
        console.info("_cleanseFormData");

        var me = this;
        //remove templated querystring params and empty params
        $.each(this._formValues, function() {
            if (templateResult.keys.indexOf(this.name) > -1 || !this.value || this.value == "") {
                delete me._queryStringPairs[this.name];
            }
        });

        $('[data-json]', form).each(function() {
            delete me._queryStringPairs[this.name];
        });

        return this._queryStringPairs;
    };

    this._getUrl = function(values) {
        console.info("_getUrl");

        values['apiKey'] = 'AGRC-Explorer';

        return '?' + $.param(values);
    };

    this.getData = function(form) {
        console.info("getData");

        return this._isPost() ? this._postData(form) : {};
    };

    this._postData = function(form) {
        console.info("_postData");
        var me = this;

        $('[data-json]', form).each(function() {
            $.extend(me._postedData, JSON.parse(this.value));

            $.grep(me._postedData, function(value) {
                return value != this.name;
            });
        });

        return this._postedData;
    };

    this.getDataType = function() {
        var hasCallback = $.grep(this._formValues, function(key) {
            return key.name === 'callback' && key.value !== "";
        });

        return hasCallback.length > 0 ? "text" : "";
    };

    this.displaySampleUrl = function(baseUrl) {
        console.info("displaySampleUrl");

        var sampleUrl = this.getUrl(baseUrl);
        var fullUrl = window.location.protocol + "//" + window.location.host + sampleUrl;

        $('#sampleUrl', this._form).html(fullUrl).show();

        return fullUrl;
    };

    this.displayResult = function(response) {
        return this.getDataType() === "" ? JSON.stringify(response, undefined, " ") : response;
    };

    this._isPost = function() {
        console.info("_isPost" + arguments);

        return this.type === 'POST';
    };
};