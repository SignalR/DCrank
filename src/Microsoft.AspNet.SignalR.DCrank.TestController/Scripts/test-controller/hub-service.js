testControllerApp.service('hubService', [
    '$rootScope', 'modelUpdateService', 'performanceUpdateService',
    function ($rootScope, modelUpdateService, performanceUpdateService) {
    return {
        initialize: function () {
            var connection = $.hubConnection();
            var proxy = connection.createHubProxy('controllerHub');

            connection.logging = true;

            // Register hub callbacks to update the modelService, ensuring that
            // angular will be notified of the changes.
            proxy.on('agentConnected', function (agentId, heartbeatInformation) {
                $rootScope.$apply(modelUpdateService.agentHeartbeat(agentId, heartbeatInformation));
            });

            proxy.on('agentPongResponse', function (agentId, value) {
                $rootScope.$apply(modelUpdateService.agentPong(agentId, value));
            });

            proxy.on('workerPongResponse', function (agentId, workerId, value) {
                $rootScope.$apply(modelUpdateService.workerPong(agentId, workerId, value));
            });

            proxy.on('agentsLog', function (agentId, message) {
                $rootScope.$apply(modelUpdateService.agentLog(agentId, message));
            });

            proxy.on('workersLog', function (agentId, workerId, message) {
                $rootScope.$apply(modelUpdateService.workerLog(agentId, workerId, message));
            });

            proxy.on('updatePerfCounters', function (performanceData, timestamp) {
                $rootScope.$apply(performanceUpdateService.updatePerformanceData(performanceData, timestamp));
            });

            connection.start();

            this.proxy = proxy;
        },
        startWorker: function (agentId, numberOfWorkers, numberOfConnections) {
            this.proxy.invoke('startWorker', agentId, numberOfWorkers, numberOfConnections);
        },
        pingAgent: function (agentId, value) {
            this.proxy.invoke('pingAgent', agentId, value);
        },
        pingWorker: function (agentId, workerId, value) {
            this.proxy.invoke('pingWorker', agentId, workerId, value);
        },
        killWorker: function (agentId, workerId) {
            this.proxy.invoke('killWorker', agentId, workerId);
        },
        stopWorker: function (agentId, workerId) {
            this.proxy.invoke('stopWorker', agentId, workerId);
        },
        killWorkers: function (agentId, numberOfWorkersToKill) {
            this.proxy.invoke('killWorkers', agentId, numberOfWorkersToKill);
        },
        setUpTest: function (targetAddress, numberOfConnections, agentIdList) {
            this.proxy.invoke('setUpTest', targetAddress, numberOfConnections, agentIdList);
        },
        startTest: function (messageSize, messageRate) {
            this.proxy.invoke('startTest', messageSize, messageRate);
        },
        stopWorkers: function () {
            this.proxy.invoke('stopWorkers');
        },
        killConnections: function () {
            this.proxy.invoke('killConnections');
        },
        connectToDatabase: function () {
            this.proxy.invoke('connectToDatabase');
        },
        getPerformanceData: function () {
            this.proxy.invoke('getPerformanceData');
        }
    }
}]);
