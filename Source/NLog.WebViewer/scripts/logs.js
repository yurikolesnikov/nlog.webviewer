NL = window.NL || {};

(function (logs) {

    var requestDelay = 1000;

    function defaultOnError(message) {
        console.error("Error: " + message);
    }

    function defaultOnSuccess() { }

    function ajax(type, url, data, onSuccess, onError) {
        onSuccess = onSuccess || defaultOnSuccess;
        onError = onError || defaultOnError;

        $.support.cors = true;

        $.ajax({
            type: type,
            url: url,
            cache: false,
            data: data,
        }).done(function (result) {
            if (!result) return;
            onSuccess(result);
        }).fail(function (xhr, textStatus, errorThrown) {
            console.error("Failed: " + textStatus, errorThrown);
            onError("WebService Connection Error", errorThrown);
        });
    }

    function ViewModel() {
        var self = this,
            timeout;

        this.isAutoRefresh = ko.observable(false);
        this.isAutoRefresh.subscribe(function (value) {
            if (value) {
                self.updateLogs();
            } else {
                clearTimeout(timeout);
            }
        });

        this.updateLogs = function () {
            clearTimeout(timeout);
            getLatestLogs();
        }

        function getLatestLogs() {
            ajax("POST", "", { lastId: logs.lastId }, function (data) {
                if (data) {
                    var result = JSON.parse(data);
                    console.log(result);
                    if (result) {
                        logs.lastId = result.LastId | 0;
                        if (result.Html) {
                            var tbody = document.getElementById("logs-content");
                            tbody.innerHTML = result.Html + tbody.innerHTML;
                        }
                    }
                }
                if (self.isAutoRefresh()) {
                    clearTimeout(timeout);
                    timeout = setTimeout(getLatestLogs, requestDelay);
                }
            }, function (data) {
                console.log("Error: " + data);
            });
        }

        if (self.isAutoRefresh()) {
            clearTimeout(timeout);
            timeout = setTimeout(getLatestLogs, requestDelay);
        }
    }

    logs.viewModel = new ViewModel();
    ko.applyBindings(logs.viewModel);
})(NL);

function toggleDetails(el) {
    el = $(el);
    el.next().toggle();
    if (el.hasClass("expanded")) {
        el.removeClass("expanded");
    } else {
        el.addClass("expanded");
    }
}
