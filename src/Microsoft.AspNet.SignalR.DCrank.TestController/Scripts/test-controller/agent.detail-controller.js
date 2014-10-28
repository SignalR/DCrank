testControllerApp.controller("AgentDetailController", [
    '$stateParams', 'modelService', 'hubService', function ($stateParams, modelService, hubService) {
        var vm = this;
        modelService.bindAgent(vm, 'agent', $stateParams.agentId);

        vm.url = 'http://localhost:24037/';
        vm.connections = 3;
        vm.pingValue = 0;

        vm.selectedClass = function (index) {
            var selectedWorkerId = Number($stateParams.workerId);
            if ((vm.agent != null) && (vm.agent.workers[index].id === selectedWorkerId)) {
                return 'selected-row';
            }
            return '';
        };

        vm.StartWorker = function () {
            hubService.setUpTest(vm.url, vm.connections, [vm.agent.id]);
        }

        vm.Ping = function () {
            hubService.pingAgent(vm.agent.id, vm.pingValue);
        }
    }
]);