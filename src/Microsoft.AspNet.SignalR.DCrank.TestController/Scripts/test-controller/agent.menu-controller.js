testControllerApp.controller('AgentMenuController', [
    '$stateParams', 'modelService', function ($stateParams, modelService) {
        var vm = this;

        vm.agents = modelService.getAgents();

        vm.selectedClass = function (index) {
            if ($stateParams.agentId === vm.agents[index].id) {
                return 'selected-row';
            }
            return '';
        };
    }
]);
