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

            // Reset accumulated agent data
            agent.currentConnections = 0;

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

                // Update accumulated agent data
                agent.currentConnections += workerInformation.ConnectedCount;
            }
        },
        agentLog: function (agentId, message) {
            var agent = modelService.getCreateAgent(agentId);
            var output = agent.output;

            output.push(message);
            if (output.length > 200) {
                // Periodically trim the log for performance
                output.splice(0, 50);
            }
        },
        workerLog: function (agentId, workerId, message) {
            var agent = modelService.getCreateAgent(agentId);
            var worker = modelService.getCreateWorker(agent, workerId);
            var output = worker.output;

            output.push(message);
            if (output.length > 200) {
                // Periodically trim the log for performance
                output.splice(0, 50);
            }
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