testControllerApp.controller("RunMenuController", [
    'runService',
    function (runService) {
        var vm = this;
        vm.runDefinitions = runService.getRunDefinitions();

        this.isRunning = function () {
            return runService.getRunStatus().activeRun !== null;
        }
    }
]);
