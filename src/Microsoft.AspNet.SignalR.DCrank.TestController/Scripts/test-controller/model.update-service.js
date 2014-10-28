testControllerApp.service('modelUpdateService', ['modelService', function (modelService) {
    return {
        agentHeartbeat: function (agentId, heartbeatInformation) {
            // Get the agent, create if it doesn't exist
            var agent = modelService.getCreateAgent(agentId);

            // Update the agent data
            agent.machineName = heartbeatInformation.HostName;
            agent.targetAddress = heartbeatInformation.TargetAddress;
            agent.totalConnectionCount = heartbeatInformation.TotalConnectionsRequested;
            agent.applyingLoad = heartbeatInformation.ApplyingLoad;

            // Mark all workers out of date
            for (var index = 0; index < agent.workers.length; index++) {
                agent.workers[index].updated = false;
            }

            // Update the worker data
            for (var index = 0; index < heartbeatInformation.Workers.length; index++) {
                var workerInformation = heartbeatInformation.Workers[index];

                // Get the worker, create if it doesn't exist
                var worker = modelService.getCreateWorker(agent, workerInformation.Id);

                // Update the worker data
                worker.updated = true;
                worker.connectedCount = workerInformation.ConnectedCount;
                worker.disconnectedCount = workerInformation.DisconnectedCount;
                worker.reconnectingCount = workerInformation.ReconnectedCount;
                worker.targetConnectionCount = workerInformation.TargetConnectionCount;
            }
        },
        agentLog: function (agentId, message) {
            var agent = modelService.getCreateAgent(agentId);
            agent.output.push(message);
        },
        workerLog: function (agentId, workerId, message) {
            var agent = modelService.getCreateAgent(agentId);
            var worker = modelService.getCreateWorker(agent, workerId);
            worker.output.push(message);
        },
        agentPong: function (agentId, value) {
            var agent = modelService.tryGetAgent(agentId);

            if (agent !== null) {
                agent.pongValue = value;
            }
        },
        updatePerformanceData: function (performanceData, timestamp) { }
    };

}]);