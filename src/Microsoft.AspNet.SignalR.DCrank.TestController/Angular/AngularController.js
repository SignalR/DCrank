var angularModule = angular.module('angularModule', []);

angularModule.service('signalRSvc', function ($rootScope) {
    var proxy = null;

    var initialize = function () {

        connection = $.hubConnection();

        this.proxy = connection.createHubProxy('controllerHub');

        connection.logging = true;

        this.proxy.on('agentConnected', function (agentId, heartbeatInformation) {
            $rootScope.$emit('agentConnected', agentId, heartbeatInformation);
        });

        this.proxy.on('agentPongResponse', function (agentId, value) {
            $rootScope.$emit('agentPongResponse', agentId, value);
        });

        this.proxy.on('workerPongResponse', function (agentId, workerId, value) {
            $rootScope.$emit('workerPongResponse', agentId, workerId, value);
        });

        this.proxy.on('agentsLog', function (agentId, message) {
            $rootScope.$emit('agentsLog', agentId, message);
        });

        this.proxy.on('workersLog', function (agentId, workerId, message) {
            $rootScope.$emit('workersLog', agentId, workerId, message);
        });

        this.proxy.on('updatePerfCounters', function (performanceData) {
            $rootScope.$emit('updatePerfCounters', performanceData);
        });

        connection.start();
    };


    // Agent and worker functions
    var startWorker = function (agentId, numberOfWorkers, numberOfConnections) {
        this.proxy.invoke('startWorker', agentId, numberOfWorkers, numberOfConnections);
    };

    var pingAgent = function (agentId) {
        this.proxy.invoke('pingAgent', agentId, 5); // 5 is a random int
    };

    var pingWorker = function (agentId, workerId) {
        this.proxy.invoke('pingWorker', agentId, workerId, 7); // 7 is a random int
    };

    var killWorker = function (agentId, workerId) {
        this.proxy.invoke('killWorker', agentId, workerId);
    };

    var stopWorker = function (agentId, workerId) {
        this.proxy.invoke('stopWorker', agentId, workerId);
    };

    var killWorkers = function (agentId, numberOfWorkersToKill) {
        this.proxy.invoke('killWorkers', agentId, numberOfWorkersToKill);
    };


    // Running a Test
    var setUpTest = function (targetAddress, numberOfConnections, agentIdList) {
        this.proxy.invoke('setUpTest', targetAddress, numberOfConnections, agentIdList);
    }

    var startTest = function (messageSize, messageRate) {
        this.proxy.invoke('startTest', messageSize, messageRate);
    }

    var stopWorkers = function () {
        this.proxy.invoke('stopWorkers');
    }

    var killConnections = function () {
        this.proxy.invoke('killConnections');
    }


    // Displaying and updating the database information
    var connectToDatabase = function () {
        this.proxy.invoke('connectToDatabase');
    }
    var getPerformanceData = function () {
        this.proxy.invoke('getPerformanceData');
    }

    return {
        initialize: initialize,
        startWorker: startWorker,
        pingAgent: pingAgent,
        pingWorker: pingWorker,
        killWorker: killWorker,
        stopWorker: stopWorker,
        killWorkers: killWorkers,
        setUpTest: setUpTest,
        startTest: startTest,
        stopWorkers: stopWorkers,
        killConnections: killConnections,
        connectToDatabase: connectToDatabase,
        getPerformanceData: getPerformanceData
    };
});

function SignalRAngularCtrl($scope, signalRSvc, $rootScope) {
    signalRSvc.initialize();


    // Agents and workers - helper objects
    $scope.agents = [];
    $scope.pendingLogs = {};
    $scope.uiGeneralDisplay = [];


    // Navigation from one view to the other
    $scope.currentAgentInView = undefined;
    $scope.currentWorkerInView = undefined;
    $scope.currentView = 'All Agents View';
    $scope.displayAllAgentsView = true;
    $scope.displaySingleAgentView = false;
    $scope.displaySingleWorkerView = false;


    // Running a test - data objects
    $scope.targetAddress = '';
    $scope.numberOfConnections;
    $scope.messagesPerSecond;
    $scope.messageSize;
    $scope.totalConnectionCount = 0;


    // Running a test - navigation flags
    $scope.autoTesting = true;
    $scope.setUpNeeded = true;
    $scope.spinningUpConnections = false;
    $scope.readyToStartTest = false;
    $scope.testRunning = false;
    $scope.targetNumber = 0;


    // Running a test - performance data
    $scope.performaceCounterData;


    // Agent and worker creation and upkeep
    $scope.$parent.$on('agentConnected', function (e, agentId, heartbeatInformation) {
        $scope.$apply(function () {
            var newAgent = true;
            for (var i = 0; i < $scope.agents.length; i++) {
                var agent = $scope.agents[i];
                if (agent.id === agentId) {
                    newAgent = false;
                    break;
                };
            };
            if (newAgent) {
                var agentNumber = agentId.slice(0, 8);
                var agent = {
                    number: agentNumber,
                    id: agentId,
                    numberOfWorkers: 0,
                    targetNumberOfConnections: heartbeatInformation.TotalConnectionsRequested,
                    workers: [],
                    output: [],
                    display: false,
                    target: heartbeatInformation.TargetAddress,
                    applyingLoad: heartbeatInformation.ApplyingLoad,
                    workersToKill: 0,
                    allAgentsView: true,
                    singleAgentView: false,
                    state: 'functionalAgent',
                    machineName: heartbeatInformation.HostName,
                    selfDestruct: setTimeout(function () { $scope.timeOut(agentId) }, 5000)
                };
                $scope.agents.push(agent);
                $scope.newAgentAlert(agentNumber);
            };

            // Handles the timeout of the agent
            clearTimeout(agent.selfDestruct);
            agent.selfDestruct = setTimeout(function () { $scope.timeOut(agentId) }, 5000);

            // Process the Heartbeat Information:
            agent.targetNumberOfConnections = heartbeatInformation.TotalConnectionsRequested;
            agent.applyingLoad = heartbeatInformation.ApplyingLoad;
            agent.state = 'functionalAgent';
            var listOfWorkers = heartbeatInformation.Workers;
            for (var i = 0; i < listOfWorkers.length; i++) {
                var worker = listOfWorkers[i];
                $scope.workerConnected(agentId, worker.Id, worker.ConnectedCount, worker.TargetConnectionCount);
            }
            if (listOfWorkers.length !== agent.workers.length) {
                var listOfWorkerIds = [];
                for (var i = 0; i < listOfWorkers.length; i++) {
                    var deadWorker = listOfWorkers[i];
                    listOfWorkerIds.push(deadWorker.Id);
                }
                $scope.deadWorkers(agent, listOfWorkerIds);
            }

            // Updates appropriate worker states
            $scope.setWorkerState(agent);

            // Updates the connection count
            $scope.getTargetConnectionCount();
            $scope.getConnectionCount();
            $scope.setState(heartbeatInformation.ApplyingLoad);
        });
    });

    $scope.workerConnected = function (agentId, workerId, connectionCount, targetConnectionCount) {
        var newWorker = true;
        for (var index = 0; index < $scope.agents.length; index++) {
            var agent = $scope.agents[index];
            if (agent.id === agentId) {
                for (var i = 0; i < agent.workers.length; i++) {
                    var worker = agent.workers[i];
                    if (worker.id === workerId) {
                        newWorker = false;
                        break;
                    }
                }
                break;
            }
        }
        if (newWorker) {
            var addedWorker = {
                id: workerId,
                output: [],
                singleWorkerView: false,
                state: 'functionalWorker',
                display: false,
                targetConnectionCount: targetConnectionCount,
                numberOfConnections: connectionCount
            };

            // Gives the worker any information that may have come in prior to its creation
            if ($scope.pendingLogs[agentId] !== undefined && $scope.pendingLogs[agentId][workerId] !== undefined) {
                while ($scope.pendingLogs[agentId][workerId].length > 0) {
                    addedWorker.output.unshift($scope.pendingLogs[agentId][workerId].pop());
                }
            }

            // Updates the Agent
            agent.numberOfWorkers += 1;
            agent.workers.push(addedWorker);
            $scope.newWorkerAlert(agent.number, workerId);
        }
        else {
            worker.numberOfConnections = connectionCount;
            worker.targetConnectionCount = targetConnectionCount;
        }
    };

    $scope.deadWorkers = function (agent, listOfWorkerIds) {
        for (var i = 0; i < agent.workers.length; i++) {
            if (listOfWorkerIds.indexOf(agent.workers[i].id) < 0) {
                agent.workers.splice(i, 1);
                agent.numberOfWorkers -= 1;
                i--;
            }
        }
    }

    $scope.startWorker = function () {
        event.stopPropagation();
        var worker = {
            id: 0,
            output: [],
            singleWorkerView: false,
            state: 'spinningUp',
            display: false,
            numberOfConnections: 1,
            targetConnectionCount: 1
        };
        $scope.currentAgentInView.workers.push(worker);
        $scope.currentAgentInView.numberOfWorkers += 1;
        var agentId = $scope.currentAgentInView.id;
        var workersToStart = 1;
        var numberOfConnections = worker.numberOfConnections;
        signalRSvc.startWorker(agentId, workersToStart, numberOfConnections);
    };


    // Log messages for the Agent and Worker
    $scope.$parent.$on('agentsLog', function (e, agentId, message) {
        $scope.$apply(function () {
            for (var index = 0; index < $scope.agents.length; index++) {
                var agent = $scope.agents[index];
                if (agent.id === agentId) {
                    agent.output.unshift(message);
                    break;
                };
            };
        });
    });

    $scope.$parent.$on('workersLog', function (e, agentId, workerId, message) {
        $scope.$apply(function () {
            for (var index = 0; index < $scope.agents.length; index++) {
                var agent = $scope.agents[index];
                if (agent.id === agentId) {
                    for (var i = 0; i < agent.workers.length; i++) {
                        var worker = agent.workers[i];
                        if (worker.id === workerId) {
                            break;
                        }
                    }
                    break;
                };
            };
            if (agent !== undefined && worker !== undefined) {
                worker.output.unshift(message);
            }
            else {
                if ($scope.pendingLogs[agentId] === undefined) {
                    $scope.pendingLogs[agentId] = {};
                }
                if ($scope.pendingLogs[agentId][workerId] === undefined) {
                    $scope.pendingLogs[agentId][workerId] = [];
                }
                $scope.pendingLogs[agentId][workerId].push(message);
            }
        });
    });

    $scope.newAgentAlert = function (agentNumber) {
        var message = 'Agent ' + agentNumber + ' has connected to the hud';
        $scope.uiGeneralDisplay.unshift(message);
    }

    $scope.newWorkerAlert = function (agentNumber, workerId) {
        var message = workerId + ' has connected to agent: ' + agentNumber;
        $scope.uiGeneralDisplay.unshift(message);
    }

    $scope.pingAgentAlert = function (agentNumber, value) {
        var message = 'Agent ' + agentNumber + ' has responded to the ping with ' + value;
        $scope.uiGeneralDisplay.unshift(message);
    }

    $scope.pingWorkerAlert = function (agentNumber, workerId, value) {
        var message = 'Worker with id: ' + workerId + ' under' +
                    ' agent: ' + agentNumber + ' has responded to the ping with ' + value;
        $scope.uiGeneralDisplay.unshift(message);
    }


    // Navigation for the different views
    $scope.backOut = function () {
        if ($scope.displayAllAgentsView) {
            $scope.displayAllAgentsView = true;
        }
        else if ($scope.displaySingleAgentView) {
            // Shows all agents
            for (var i = 0; i < $scope.agents.length; i++) {
                $scope.agents[i].allAgentsView = true;
            }

            // Resets the appropriate flags
            $scope.currentAgentInView.singleAgentView = false;
            $scope.currentAgentInView = undefined;
            $scope.displayAllAgentsView = true;
            $scope.displaySingleAgentView = false;

            // Updates the view display name
            $scope.currentView = 'All Agents View';
        }
        else if ($scope.displaySingleWorkerView) {
            // Updates the view display name
            $scope.currentView = 'Viewing Agent ' + $scope.currentAgentInView.number + ' (' + $scope.currentAgentInView.machineName + ')';

            // Resets the appropriate flags
            $scope.displaySingleAgentView = true;
            $scope.displaySingleWorkerView = false;
            $scope.currentWorkerInView.singleWorkerView = false;
            $scope.currentWorkerInView = undefined;

            // Displays the appropriate workers
            $scope.currentAgentInView.singleAgentView = true;
        }

    }

    $scope.toSingleAgentView = function () {
        event.stopPropagation();
        for (var i = 0; i < $scope.agents.length; i++) {
            $scope.agents[i].allAgentsView = false;
        }

        // Sets the necessary flags
        $scope.displayAllAgentsView = false;
        this.agent.singleAgentView = true;
        $scope.displaySingleAgentView = true;

        $scope.currentAgentInView = this.agent;
        $scope.currentView = 'Viewing Agent ' + $scope.currentAgentInView.number + ' (' + $scope.currentAgentInView.machineName + ')';
    }

    $scope.toSingleWorkerView = function () {
        event.stopPropagation();
        this.worker.singleWorkerView = true;
        $scope.displaySingleWorkerView = true;
        $scope.displaySingleAgentView = false;

        $scope.currentAgentInView.singleAgentView = false;
        $scope.currentWorkerInView = this.worker;

        $scope.currentView = 'Viewing Worker ' + $scope.currentWorkerInView.id + ' on Agent ' + $scope.currentAgentInView.number + ' (' + $scope.currentAgentInView.machineName + ')';
    }


    // Helper methods for agents and workers
    $scope.pingWorker = function (agentId) {
        var workerId = this.worker.id;
        signalRSvc.pingWorker(agentId, workerId);
    }

    $scope.killWorker = function (agentId) {
        event.stopPropagation();
        var workerId = this.worker.id;
        signalRSvc.killWorker(agentId, workerId);
    }

    $scope.stopWorker = function (agentId) {
        var workerId = this.worker.id;
        signalRSvc.stopWorker(agentId, workerId);
    }

    $scope.killWorkers = function () {
        var agentId = this.agent.id;
        var numberOfWorkersToKill = parseInt(this.agent.workersToKill);
        this.agent.workersToKill = 0;
        signalRSvc.killWorkers(agentId, numberOfWorkersToKill);
    }

    $scope.timeOut = function (agentId) {
        for (var i = 0; i < $scope.agents.length; i++) {
            var agent = $scope.agents[i];
            if (agent.id === agentId) {
                newAgent = false;
                break;
            };
        };
        $scope.uiGeneralDisplay.unshift('Received a timeout message from: ' + agent.number);
        agent.state = 'deadAgent';
        $scope.setWorkerState(agent);
        $scope.getConnectionCount();
        $scope.$digest();
        agent.selfDestruct = setTimeout(function () { $scope.removeDeadAgent(agentId) }, 15000);
    }

    $scope.setWorkerState = function (agent) {
        for (var i = 0; i < agent.workers.length; i++) {
            var worker = agent.workers[i];
            if (agent.state === 'deadAgent') {
                worker.state = 'deadWorker';
                worker.numberOfConnections = 0;
            }
            else if (worker.targetConnectionCount !== worker.numberOfConnections) {
                worker.state = 'rampingUp';
            }
            else if (agent.state === 'functionalAgent') {
                worker.state = 'functionalWorker';
            }
        }
    }

    $scope.removeDeadAgent = function (agentId) {
        var agentIndex;
        for (var i = 0; i < $scope.agents.length; i++) {
            if ($scope.agents[i].id === agentId) {
                agentIndex = i;
            };
        };
        $scope.uiGeneralDisplay.unshift('Received a self destruct message from: ' + $scope.agents[agentIndex].number);
        $scope.agents.splice(agentIndex, 1);
        $scope.$digest();
    }


    // Functions to set up and execute a test run
    $scope.spinUpConnections = function () {
        var targetAddress = $scope.targetAddress;
        var numberOfConnections = $scope.numberOfConnections;
        $scope.targetNumber = numberOfConnections;
        var agentIdList = [];
        for (var i = 0; i < $scope.agents.length; i++) {
            if ($scope.agents[i].state === 'functionalAgent') {
                agentIdList.push($scope.agents[i].id);
            }
        }
        signalRSvc.setUpTest(targetAddress, numberOfConnections, agentIdList);
    }

    $scope.startTest = function () {
        var messageSize = $scope.messageSize;
        var messageRate = $scope.messagesPerSecond;
        signalRSvc.startTest(messageSize, messageRate);
    }

    $scope.killConnections = function () {
        $scope.targetNumber = 0;
        signalRSvc.killConnections();
    }

    $scope.stopWorkers = function () {
        $scope.targetNumber = 0;
        signalRSvc.stopWorkers();
    }

    $scope.getTargetConnectionCount = function () {
        var targetCount = 0;
        for (var agent = 0; agent < $scope.agents.length; agent++) {
            targetCount += $scope.agents[agent].targetNumberOfConnections;
        }
        $scope.targetNumber = targetCount;
    }

    $scope.getConnectionCount = function () {
        var currentCount = 0;
        for (var agent = 0; agent < $scope.agents.length; agent++) {
            for (var worker = 0; worker < $scope.agents[agent].workers.length; worker++) {
                currentCount += $scope.agents[agent].workers[worker].numberOfConnections;
            }
        }
        $scope.totalConnectionCount = currentCount;
    }

    $scope.setState = function (applyingLoad) {
        if (applyingLoad) {
            // Running the test
            $scope.setUpNeeded = false;
            $scope.spinningUpConnections = false;
            $scope.readyToStartTest = false;
            $scope.testRunning = true;
        }
        else if ($scope.targetNumber !== 0 && $scope.totalConnectionCount === $scope.targetNumber) {
            // Ready to start test
            $scope.setUpNeeded = false;
            $scope.spinningUpConnections = false;
            $scope.readyToStartTest = true;
            $scope.testRunning = false;
        }
        else if ($scope.totalConnectionCount < $scope.targetNumber) {
            // Spinning up connections
            $scope.setUpNeeded = false;
            $scope.spinningUpConnections = true;
            $scope.readyToStartTest = false;
            $scope.testRunning = false;
        }
        else if ($scope.targetNumber === 0 && $scope.totalConnectionCount === 0) {
            // Need set up information
            $scope.setUpNeeded = true;
            $scope.spinningUpConnections = false;
            $scope.readyToStartTest = false;
            $scope.testRunning = false;
        }
    }


    // Helps to display the signalR connection data
    $scope.$parent.$on('updatePerfCounters', function (e, performanceData) {
        $scope.$apply(function () {
            var newPerformanceData = [];

            for (var i = 0; i < performanceData.length; i++) {
                var currentRow = [];
                currentRow.push(performanceData[i].CounterName);
                currentRow.push(performanceData[i].RawValue);
                newPerformanceData.push(currentRow);
            }

            $scope.performaceCounterData = newPerformanceData;
        });
    });
}