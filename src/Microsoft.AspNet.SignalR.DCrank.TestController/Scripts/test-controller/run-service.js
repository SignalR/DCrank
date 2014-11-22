testControllerApp.service('runService', [
    '$rootScope', function ($rootScope) {
        var service = {
            getRunTypes: function () {
                // ToDo: get list from server
                return ['manual'];
            },
            bindRunDefinition: function (target, property, runType) {
                $.ajax({
                    type: 'post',
                    cache: false,
                    url: 'run/LoadDefinition',
                    dataType:'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(runType),
                    success:function(data) {
                        $rootScope.$apply(function() {
                            target[property] = data;
                        });
                    }
                });
            },
            startRun: function (run) {
                $.ajax({
                    type: 'post',
                    cache: false,
                    url: 'run/Start',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(run)
                });
            }
        };
        return service;
    }
])