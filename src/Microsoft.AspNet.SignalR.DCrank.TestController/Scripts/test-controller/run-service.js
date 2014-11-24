testControllerApp.service('runService', [
    function () {
        var runModel = {
            runDefinitions: [],
            runStatus: {
                controllerState: 'Unknown',
                startRequested: false,
                activeRun: null
            }
        };

        var tryGetDefinition = function (type) {
            for (var index = 0; index < runModel.runDefinitions.length; index++) {
                if (runModel.runDefinitions[index].Type === type) {
                    return runModel.runDefinitions[index];
                }
            }
            return null;
        };

        var service = {
            getRunDefinitions: function () {
                return runModel.runDefinitions;
            },
            bindRunDefinition: function (target, property, type, $scope) {
                target[property] = tryGetDefinition(type);

                if (target[property] === null) {
                    var unwatchDefinition = $scope.$watch(
                        function () {
                            return tryGetDefinition(type);
                        },
                        function (newValue, oldValue) {
                            if (newValue !== null) {
                                target[property] = newValue;
                                unwatchDefinition();
                            }
                        });
                }
            },
            getRunStatus: function () {
                return runModel.runStatus;
            }
        };
        return service;
    }
]);
