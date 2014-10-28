testControllerApp.controller('AgentMenuController', [
    'modelService', 'hubService', function(modelService, hubService) {
        this.agents = modelService.getAgents();

        this.pingValue = 0;

        this.Ping = function(index) {
            hubService.pingAgent(this.agents[index].id, this.pingValue);
        }
    }
]);
