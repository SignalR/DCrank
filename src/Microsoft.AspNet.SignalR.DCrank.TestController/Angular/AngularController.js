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

    var startWorker = function (agentId, numberOfWorkers) {
        this.proxy.invoke('startWorker', agentId, numberOfWorkers);
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

    var sendTestInfoAuto = function (targetAddress, messageSize, messageRate, numberOfConnections, numberOfAgents) {
        this.proxy.invoke('sendTestInfoAuto', targetAddress, messageSize, messageRate, numberOfConnections, numberOfAgents)
    }

    var sendTestInfoManual = function (targetAddress, messageSize, messageRate) {
        this.proxy.invoke('sendTestInfoManual', targetAddress, messageSize, messageRate)
    }

    return {
        initialize: initialize,
        startWorker: startWorker,
        pingAgent: pingAgent,
        pingWorker: pingWorker,
        killWorker: killWorker,
        stopWorker: stopWorker,
        sendTestInfoAuto: sendTestInfoAuto,
        sendTestInfoManual: sendTestInfoManual,
        killWorkers: killWorkers
    };
});

function SignalRAngularCtrl($scope, signalRSvc, $rootScope) {
    $scope.agents = [];

    $scope.currentAgentInView = undefined;

    $scope.currentWorkerInView = undefined;

    $scope.currentView = 'All Agents View';

    $scope.stopPropagation = false;

    $scope.displayView1 = true;

    $scope.displayView2 = false;

    $scope.displayView3 = false;

    $scope.autoTesting = true;

    $scope.uiGeneralDisplay = [];

    $scope.targetAddress = '';

    $scope.numberOfConnections;

    $scope.messagesPerSecond;

    $scope.messageSize;

    $scope.numberOfAgents = $scope.agents.length;

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

    $scope.backOut = function () {
        if ($scope.displayView1) {
            $scope.displayView1 = true;
        }
        else if ($scope.displayView2) {
            // Shows all agents
            for (var i = 0; i < $scope.agents.length; i++) {
                $scope.agents[i].view1 = true;
            }

            // Resets the appropriate flags
            $scope.currentAgentInView.view2 = false;
            $scope.currentAgentInView = undefined;
            $scope.displayView1 = true;
            $scope.displayView2 = false;

            // Updates the view display name
            $scope.currentView = 'All Agents View';
        }
        else if ($scope.displayView3) {
            // Updates the view display name
            $scope.currentView = 'Viewing Agent ' + $scope.currentAgentInView.number + ' (' + $scope.currentAgentInView.machineName + ')';

            // Resets the appropriate flags
            $scope.displayView2 = true;
            $scope.displayView3 = false;
            $scope.currentWorkerInView.view3 = false;
            $scope.currentWorkerInView = undefined;

            // Displays the appropriate workers
            $scope.currentAgentInView.view2 = true;
        }

    }

    $scope.toView2 = function () {
        event.stopPropagation();
        for (var i = 0; i < $scope.agents.length; i++) {
            $scope.agents[i].view1 = false;
        }

        // Sets the necessary flags
        $scope.displayView1 = false;
        this.agent.view2 = true;
        $scope.displayView2 = true;

        $scope.currentAgentInView = this.agent;
        $scope.currentView = 'Viewing Agent ' + $scope.currentAgentInView.number + ' (' + $scope.currentAgentInView.machineName + ')';
    }

    $scope.toView3 = function () {
        event.stopPropagation();
        this.worker.view3 = true;
        $scope.displayView3 = true;
        $scope.displayView2 = false;

        $scope.currentAgentInView.view2 = false;
        $scope.currentWorkerInView = this.worker;

        $scope.currentView = 'Viewing Worker ' + $scope.currentWorkerInView.id + ' on Agent ' + $scope.currentAgentInView.number + ' (' + $scope.currentAgentInView.machineName + ')';
    }

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

    $scope.sendTestInfo = function () {
        var targetAddress = $scope.targetAddress;
        var messageSize = $scope.messageSize;
        var messageRate = $scope.messagesPerSecond;
        if ($scope.autoTesting) {
            var numberOfConnections = $scope.numberOfConnections;
            var numberOfAgents = $scope.numberOfAgents;
            signalRSvc.sendTestInfoAuto(targetAddress, messageSize, messageRate, numberOfConnections, numberOfAgents);
        }
        else {
            signalRSvc.sendTestInfoManual(targetAddress, messageSize, messageRate);
        }
    }

    $scope.showWorkerLogging = function () {
        this.worker.display = !this.worker.display;
    }

    $scope.showAgentLogging = function () {
        this.agent.display = !this.agent.display;
    }

    $scope.timeOut = function (agentIndex) {
        $scope.uiGeneralDisplay.unshift('got a timeout message from: ' + $scope.agents[agentIndex].number);
        // $scope.agents.splice(agentIndex, 1);
        $scope.agents[agentIndex].state = 'deadAgent';
        $scope.$digest();
    }

    signalRSvc.initialize();

    $scope.startWorker = function () {
        event.stopPropagation();
        var worker = {
            id: 0,
            output: [],
            view3: false,
            state: 'spinningUp',
            display: false
        };
        $scope.currentAgentInView.workers.push(worker);
        $scope.currentAgentInView.numberOfWorkers += 1;
        var agentId = $scope.currentAgentInView.id;
        var workersToStart = 1;
        this.agent.workersToStart = 0;
        signalRSvc.startWorker(agentId, workersToStart);
    };

    $scope.pingAgent = function (agentId) {
        signalRSvc.pingAgent(agentId);
    };

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
                    workers: [],
                    output: [],
                    display: false,
                    workersToStart: 0,
                    target: $scope.targetAddress,
                    workersToKill: 0,
                    view1: true,
                    view2: false,
                    state: 'functionalAgent',
                    machineName: 'N/A',
                    selfdestruct: setTimeout(function () { $scope.timeOut(agentIndex) }, 5000)
                };
                $scope.agents.push(agent);
                $scope.newAgentAlert(agentNumber);
            };

            // Process the Heartbeat Information:
            $scope.agents[agentIndex].state = 'functionalAgent';
            var listOfWorkers = heartbeatInformation.Workers;
            for (var i = 0; i < listOfWorkers.length; i++) {
                var workerId = listOfWorkers[i].Id;
                $scope.workerConnected(agentId, workerId);
            }
            if (listOfWorkers.length != $scope.agents[agentIndex].workers.length) {
                $scope.deadWorkers(agentIndex, listOfWorkers);
            }

            // Handles the timeout of the agent
            clearTimeout($scope.agents[agentIndex].selfdestruct);
            $scope.agents[agentIndex].selfdestruct = setTimeout(function () { $scope.timeOut(agentIndex) }, 5000);
        });
    });

    $scope.workerConnected = function (agentId, workerId) {
        var newWorker = true;
        var agentIndex;
        for (var index = 0; index < $scope.agents.length; index++) {
            if ($scope.agents[index].id == agentId) {
                agentIndex = index;
                for (var i = 0; i < $scope.agents[index].workers.length; i++) {
                    if ($scope.agents[index].workers[i].id == workerId) {
                        newWorker = false;
                    }
                }
            };
        };
        if (newWorker) {
            var worker = {
                id: workerId,
                output: [],
                view3: false,
                state: 'functionalWorker',
                display: false
            };
            $scope.agents[agentIndex].numberOfWorkers += 1;
            $scope.agents[agentIndex].workers.push(worker);
            $scope.newWorkerAlert($scope.agents[agentIndex].number, workerId);
        };
    };

    $scope.deadWorkers = function (agentIndex, listOfWorkers) {
        var agent = $scope.agents[agentIndex];
        for (var i = 0; i < agent.workers.length; i++) {
            if (listOfWorkers.indexOf(agent.workers[i].id) < 0) {
                agent.workers.splice(i, 1);
                agent.numberOfWorkers -= 1;
                i--;
            }
        }
    }

    $scope.$parent.$on('agentPongResponse', function (e, agentId, value) {
        $scope.$apply(function () {
            var agentIndex;
            for (var i = 0; i < $scope.agents.length; i++) {
                if ($scope.agents[i].id == agentId) {
                    agentIndex = i;
                    break;
                };
            };
            if (agentIndex != undefined) {
                $scope.pingAgentAlert($scope.agents[agentIndex].number, value);
            }
        });
    });

    $scope.$parent.$on('workerPongResponse', function (e, agentId, workerId, value) {
        $scope.$apply(function () {
            var agentIndex;
            for (var i = 0; i < $scope.agents.length; i++) {
                if ($scope.agents[i].id == agentId) {
                    agentIndex = i;
                };
            };
            if (agentIndex != undefined) {
                $scope.pingWorkerAlert($scope.agents[agentIndex].number, workerId, value);
            }
        });
    })

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
        });
    });


}