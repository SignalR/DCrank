testControllerApp.filter('sumOfProperty', function() {
    return function (array, key) {
        if (array === undefined || array === null) {
            return 0;
        }

        var sum = 0;
        for (var index = 0; index < array.length; index++) {
            sum += array[index][key];
        }
        return sum;
    }
});

testControllerApp.controller("AgentDetailController", [
    '$stateParams', 'modelService', 'hubService', function ($stateParams, modelService, hubService) {
        modelService.bindAgent(this, 'agent', $stateParams.agentId);

        this.url = 'http://localhost:24037/';
        this.connections = 3;

        this.currentConnections = function() {
            var sum = 0;
            for (var index = 0; index < agent.workers.lenght; index++) {
                sum += agent.workers[index].connectionConnected;
            }
        }

        this.StartWorker = function () {
            hubService.setUpTest(this.url, this.connections, [this.agent.id]);
        }
    }
]);