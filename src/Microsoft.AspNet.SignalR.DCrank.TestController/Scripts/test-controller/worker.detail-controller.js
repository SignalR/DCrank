testControllerApp.controller("WorkerDetailController", [
    '$scope', '$stateParams', 'modelService', 'hubService',
    function ($scope, $stateParams, modelService, hubService) {
        var vm = this;
        modelService.bindWorker(vm, 'worker', $stateParams.agentId, Number($stateParams.workerId), $scope);

        vm.agentId = $stateParams.agentId;

        vm.pingValue = 0;

        vm.Ping = function () {
            hubService.pingWorker(vm.agentId, Number($stateParams.workerId), vm.pingValue);
        };
    }
]);