testControllerApp.controller("AgentDetailController", [
    '$scope', '$stateParams', 'modelService', 'hubService',
    function ($scope, $stateParams, modelService, hubService) {
        var vm = this;
        modelService.bindAgent(vm, 'agent', $stateParams.agentId, $scope);

        vm.pingValue = 0;

        vm.Ping = function () {
            hubService.pingAgent(vm.agent.id, vm.pingValue);
        }
    }
]);