testControllerApp.service('runUpdateService', [
    'runService',
    function (runService) {
        var runStatus = runService.getRunStatus();

        var service = {
            updateRunDefinitions: function (definitions) {
                var runDefinitions = runService.getRunDefinitions();

                // Remove existing definitions
                while (runDefinitions.length > 0) {
                    runDefinitions.pop();
                }

                // Copy new definitions into model
                for (var index = 0; index < definitions.length; index++) {
                    runDefinitions.push(definitions[index]);
                }
            },
            updateControllerStatus: function (controllerState) {
                runStatus.controllerState = controllerState;
            },
            updateActiveRun: function (activeRun) {
                runStatus.activeRun = activeRun;
            },
            beforeStartRequest: function () {
                runStatus.startRequested = true;
            },
            afterStartRequest: function () {
                runStatus.startRequested = false;
            }
        }
        return service;
    }
]);
