testControllerApp.controller('AgentMenuController', [
    'modelService', function (modelService) {
        this.agents = modelService.getAgents();
    }
]);
