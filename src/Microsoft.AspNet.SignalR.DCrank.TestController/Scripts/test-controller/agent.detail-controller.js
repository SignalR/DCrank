testControllerApp.controller("AgentDetailController", [
    '$stateParams', 'modelService', 'hubService', function ($stateParams, modelService, hubService) {
        var vm = this;
        modelService.bindAgent(vm, 'agent', $stateParams.agentId);

        vm.pingValue = 0;

        vm.Ping = function () {
            hubService.pingAgent(vm.agent.id, vm.pingValue);
        }
    }
]);