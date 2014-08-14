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
    var setUpTest = function (targetAddress, numberOfConnections, numberOfAgents, agentIdList) {
        this.proxy.invoke('setUpTest', targetAddress, numberOfConnections, numberOfAgents, agentIdList);
    }

    var startTest = function (messageSize, messageRate) {
        this.proxy.invoke('startTest', messageSize, messageRate);
    }

    var killConnections = function () {
        this.proxy.invoke('killConnections');
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
        killConnections: killConnections
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


    // Agent and worker creation and upkeep
    $scope.$parent.$on('agentConnected', function (e, agentId, heartbeatInformation) {
        $scope.$apply(function () {
            var newAgent = true;
            var agentIndex;
            for (var i = 0; i < $scope.agents.length; i++) {
                if ($scope.agents[i].id == agentId) {
                    newAgent = false;
                    agentIndex = i;
                };
            };
            if (newAgent) {
                var agentNumber = agentId.slice(0, 8);
                agentIndex = $scope.agents.length;
                var agent = {
                    number: agentNumber,
                    id: agentId,
                    numberOfWorkers: 0,
                    targetNumberOfConnections: heartbeatInformation.TotalConnectionsRequested,
                    workers: [],
                    output: [],
                    display: false,
                    target: $scope.targetAddress,
                    applyingLoad: heartbeatInformation.ApplyingLoad,
                    workersToKill: 0,
                    allAgentsView: true,
                    singleAgentView: false,
                    state: 'functionalAgent',
                    machineName: heartbeatInformation.HostName,
                    selfdestruct: setTimeout(function () { $scope.timeOut(agentId) }, 5000)
                };
                $scope.agents.push(agent);
                $scope.newAgentAlert(agentNumber);
            };

            // Handles the timeout of the agent
            clearTimeout($scope.agents[agentIndex].selfdestruct);
            $scope.agents[agentIndex].selfdestruct = setTimeout(function () { $scope.timeOut(agentId) }, 5000);

            // Process the Heartbeat Information:
            $scope.agents[agentIndex].targetNumberOfConnections = heartbeatInformation.TotalConnectionsRequested;
            $scope.agents[agentIndex].applyingLoad = heartbeatInformation.ApplyingLoad;
            $scope.agents[agentIndex].state = 'functionalAgent';
            var listOfWorkers = heartbeatInformation.Workers;
            for (var i = 0; i < listOfWorkers.length; i++) {
                $scope.workerConnected(agentId, listOfWorkers[i].Id, listOfWorkers[i].ConnectedCount);
            }
            if (listOfWorkers.length != $scope.agents[agentIndex].workers.length) {
                var listOfWorkerIds = [];
                for (var i = 0; i < listOfWorkers.length; i++) {
                    listOfWorkerIds.push(listOfWorkers[i].Id);
                }
                $scope.deadWorkers(agentIndex, listOfWorkerIds);
            }

            // Updates appropriate worker states
            $scope.setWorkerState(agentIndex);

            // Updates the connection count
            $scope.getTargetConnectionCount();
            $scope.getConnectionCount();
            $scope.setState();
        });
    });

    $scope.workerConnected = function (agentId, workerId, connectionCount) {
        var newWorker = true;
        var agentIndex;
        var workerIndex;
        for (var index = 0; index < $scope.agents.length; index++) {
            if ($scope.agents[index].id == agentId) {
                agentIndex = index;
                for (var i = 0; i < $scope.agents[index].workers.length; i++) {
                    if ($scope.agents[index].workers[i].id == workerId) {
                        newWorker = false;
                        workerIndex = i;
                    }
                }
            };
        };
        if (newWorker) {
            var worker = {
                id: workerId,
                output: [],
                singleWorkerView: false,
                state: 'functionalWorker',
                display: false,
                targetConnectionCount: 3, // Hard coded because the worker is not passing us this parameter yet
                numberOfConnections: connectionCount
            };

            // Gives the worker any information that may have come in prior to its creation
            if ($scope.pendingLogs[agentId] != undefined && $scope.pendingLogs[agentId][workerId] != undefined) {
                while ($scope.pendingLogs[agentId][workerId].length > 0) {
                    worker.output.unshift($scope.pendingLogs[agentId][workerId].pop());
                }
            }

            // Updates the Agent
            $scope.agents[agentIndex].numberOfWorkers += 1;
            $scope.agents[agentIndex].workers.push(worker);
            $scope.newWorkerAlert($scope.agents[agentIndex].number, workerId);
        }
        else {
            $scope.agents[agentIndex].workers[workerIndex].numberOfConnections = connectionCount;
        }
    };

    $scope.deadWorkers = function (agentIndex, listOfWorkerIds) {
        var agent = $scope.agents[agentIndex];
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
            var agentIndex;
            for (var index = 0; index < $scope.agents.length; index++) {
                if ($scope.agents[index].id == agentId) {
                    agentIndex = index;
                };
            };
            if (agentIndex != undefined) {
                $scope.agents[agentIndex].output.unshift(message);
            }
        });
    });

    $scope.$parent.$on('workersLog', function (e, agentId, workerId, message) {
        $scope.$apply(function () {

            var agentIndex;
            var workerIndex;
            for (var index = 0; index < $scope.agents.length; index++) {
                if ($scope.agents[index].id == agentId) {
                    agentIndex = index;
                    for (var i = 0; i < $scope.agents[index].workers.length; i++) {
                        if ($scope.agents[index].workers[i].id == workerId) {
                            workerIndex = i;
                        }
                    }
                };
            };
            if (agentIndex != undefined && workerIndex != undefined) {
                $scope.agents[agentIndex].workers[workerIndex].output.unshift(message);
            }
            else {
                if ($scope.pendingLogs[agentId] == undefined) {
                    $scope.pendingLogs[agentId] = {};
                }
                if ($scope.pendingLogs[agentId][workerId] == undefined) {
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
        var agentIndex;
        for (var i = 0; i < $scope.agents.length; i++) {
            if ($scope.agents[i].id == agentId) {
                newAgent = false;
                agentIndex = i;
            };
        };
        $scope.uiGeneralDisplay.unshift('got a timeout message from: ' + $scope.agents[agentIndex].number);
        $scope.agents[agentIndex].state = 'deadAgent';
        $scope.setWorkerState(agentIndex);
        $scope.getConnectionCount();
        $scope.$digest();
        $scope.agents[agentIndex].selfdestruct = setTimeout(function () { $scope.removeDeadAgent(agentId) }, 15000);
    }

    $scope.setWorkerState = function (agentIndex) {
        for (var i = 0; i < $scope.agents[agentIndex].workers.length; i++) {
            if ($scope.agents[agentIndex].state == 'deadAgent') {
                $scope.agents[agentIndex].workers[i].state = 'deadWorker';
                $scope.agents[agentIndex].workers[i].numberOfConnections = 0;
            }
            else if ($scope.agents[agentIndex].workers[i].targetConnectionCount != $scope.agents[agentIndex].workers[i].numberOfConnections) {
                $scope.agents[agentIndex].workers[i].state = 'rampingUp';
            }
            else if ($scope.agents[agentIndex].state == 'functionalAgent') {
                for (var i = 0; i < $scope.agents[agentIndex].workers.length; i++) {
                    $scope.agents[agentIndex].workers[i].state = 'functionalWorker';
                }
            }
        }
    }

    $scope.removeDeadAgent = function (agentId) {
        var agentIndex;
        for (var i = 0; i < $scope.agents.length; i++) {
            if ($scope.agents[i].id == agentId) {
                agentIndex = i;
            };
        };
        $scope.uiGeneralDisplay.unshift('got a self destruct message from: ' + $scope.agents[agentIndex].id);
        $scope.agents.splice(agentIndex, 1);
        $scope.$digest();
    }


    // Functions to set up and execute a test run
    $scope.spinUpConnections = function () {
        var targetAddress = $scope.targetAddress;
        var numberOfConnections = $scope.numberOfConnections;
        var numberOfAgents = $scope.agents.length;
        $scope.targetNumber = numberOfConnections;
        var agentIdList = [];
        for (var i = 0; i < $scope.agents.length; i++) {
            agentIdList.push($scope.agents[i].id);
        }
        signalRSvc.setUpTest(targetAddress, numberOfConnections, numberOfAgents, agentIdList);
    }

    $scope.startTest = function () {
        var messageSize = $scope.messageSize;
        var messageRate = $scope.messagesPerSecond;
        signalRSvc.startTest(messageSize, messageRate);
        $scope.setState();
    }

    $scope.killConnections = function () {
        $scope.targetNumber = 0;
        signalRSvc.killConnections();
        $scope.setState();
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
        // Running the test
        if (applyingLoad) {
            $scope.setUpNeeded = false;
            $scope.spinningUpConnections = false;
            $scope.readyToStartTest = false;
            $scope.setUpNeeded = false;
            $scope.spinningUpConnections = false;
            $scope.readyToStartTest = false;
            $scope.testRunning = false;
        }
        else if ($scope.totalConnectionCount < $scope.targetNumber) {
            // Spinning up connections
            $scope.setUpNeeded = false;
            $scope.spinningUpConnections = true;
            $scope.readyToStartTest = false;
            $scope.testRunning = false;
        }
            // Need set up information
        else if ($scope.targetNumber == 0 && $scope.totalConnectionCount == 0) {
            $scope.setUpNeeded = true;
            $scope.spinningUpConnections = false;
            $scope.readyToStartTest = false;
            $scope.testRunning = false;
        }
    }
}