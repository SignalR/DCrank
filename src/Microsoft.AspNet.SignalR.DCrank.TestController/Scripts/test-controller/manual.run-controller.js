testControllerApp.controller("ManualRunController", [
    'runService', function (runService) {
        var vm = this;

        runService.bindRunDefinition(vm, 'run', 'manual');

        vm.startRun = function () {
            runService.startRun(vm.run);
        }
    }
]);
