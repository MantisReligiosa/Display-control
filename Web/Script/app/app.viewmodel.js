function AppViewModel() {
    var self = this;

    self.attach = function (options) {
        self[options.sourceName] = ko.computed(function () {
            return new options.factory(self);
        });
    }

    self.errorMsg = function (message) {
        toastr.error(message);
    }

    self.infoMsg = function (message) {
        toastr.info(message);
    }

    self.request = function (method, url, data, successHandler) {
        $.ajax({
            method: method,
            url: url,
            data: JSON.stringify(ko.toJS(data)),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: {
                'Accept': 'application/json'
            },
            success: successHandler,
            error: function (xhr, ajaxOptions, thrownError) {
                if (xhr.status === 406) {
                    location.reload(true);
                }
                toastr.error(thrownError);
                console.error(xhr, ajaxOptions, thrownError);
            }
        });
    }
}

var app = new AppViewModel();