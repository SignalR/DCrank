testControllerApp.service('ajaxService', [
    '$rootScope', 'runUpdateService',
    function ($rootScope, runUpdateService) {
        var service = {
            initialize: function () {
                // Fetch run definitions
                $.ajax({
                    type: 'post',
                    cache: false,
                    url: 'run/LoadDefinitions',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({}),
                    success: function (data) {
                        $rootScope.$apply(function () {
                            runUpdateService.updateRunDefinitions(data);
                        });
                    }
                });

                // Begin polling controller status
                setInterval(function () {
                    $.ajax({
                        type: 'post',
                        cache: false,
                        url: 'run/LoadStatus',
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({}),
                        success: function (data) {
                            $rootScope.$apply(function () {
                                runUpdateService.updateControllerStatus(data.State);
                                runUpdateService.updateActiveRun(data.ActiveRun);
                            });
                        }
                    });
                }, 1000);
            },
            startRun: function (run) {
                runUpdateService.beforeStartRequest();

                $.ajax({
                    type: 'post',
                    cache: false,
                    url: 'run/Start',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(run),
                    success: function (data) {
                        $rootScope.$apply(function () {
                            runUpdateService.afterStartRequest();
                        });
                    }
                });
            },
            terminateRun: function () {
                $.ajax({
                    type: 'post',
                    cache: false,
                    url: 'run/Terminate',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({})
                });
            }
        };
        return service;
    }
]);