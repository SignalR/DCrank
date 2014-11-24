testControllerApp.controller('CurrentRunController', [
    'modelService', 'runService', 'ajaxService',
    function (modelService, runService, ajaxService) {
        var vm = this;
        vm.runStatus = runService.getRunStatus();
        vm.agents = modelService.getAgents();

        vm.isRunning = function () {
            return vm.runStatus.activeRun !== null;
        };

        vm.terminateRun = function () {
            ajaxService.terminateRun();
        };
    }
]);
