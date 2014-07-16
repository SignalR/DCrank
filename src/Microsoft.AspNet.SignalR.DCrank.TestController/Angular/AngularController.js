var angularModule = angular.module('angularModule', []);

angularModule.service('signalRSvc', function ($rootScope) {
    var proxy = null;

    var initialize = function () {

        connection = $.hubConnection();

        this.proxy = connection.createHubProxy('controllerHub');

        connection.logging = true;

        this.proxy.on('agentConnected', function (agentId, heartbeatInformation) {
            $rootScope.$emit("agentConnected", agentId, heartbeatInformation);
        });

        this.proxy.on('agentPongResponse', function (agentId, value) {
            $rootScope.$emit("agentPongResponse", agentId, value);
        });

        this.proxy.on('workerPongResponse', function (agentId, workerId, value) {
            $rootScope.$emit("workerPongResponse", agentId, workerId, value);
        });

        this.proxy.on('agentsLog', function (agentId, message) {
            $rootScope.$emit("agentsLog", agentId, message);
        });

        this.proxy.on('workersLog', function (agentId, workerId, message) {
            $rootScope.$emit("workersLog", agentId, workerId, message);
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

    return {
        initialize: initialize,
        startWorker: startWorker,
        pingAgent: pingAgent,
        pingWorker: pingWorker,
        killWorker: killWorker,
        stopWorker: stopWorker,
        killWorkers: killWorkers
    };
});

function SignalRAngularCtrl($scope, signalRSvc, $rootScope) {
    $scope.agents = [];

    $scope.currentAgentNumber = 1;

    $scope.uiGeneralDisplay = [];

    $scope.targetAddress = "";

    $scope.messagesPerSecond = 0;

    $scope.messageSize = 0;

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

    $scope.pingWorker = function (agentId) {
        var workerId = this.worker.id;
        signalRSvc.pingWorker(agentId, workerId);
    }

    $scope.killWorker = function (agentId) {
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

    $scope.showWorkerLogging = function () {
        this.worker.display = !this.worker.display;
    }

    $scope.showAgentLogging = function () {
        this.agent.display = !this.agent.display;
    }

    $scope.timeOut = function (agentIndex) {
        $scope.uiGeneralDisplay.unshift('got a timeout message from:' + $scope.agents[agentIndex].id);
        $scope.agents.splice(agentIndex, 1);
        $scope.$digest();
    }

    signalRSvc.initialize();

    $scope.startWorkers = function () {
        var agentId = this.agent.id;
        var workersToStart = parseInt(this.agent.workersToStart);
        this.agent.workersToStart = 0;
        signalRSvc.startWorker(agentId, workersToStart);
    };

    $scope.pingAgent = function (agentId) {
        signalRSvc.pingAgent(agentId);
    };

    $scope.$parent.$on("agentConnected", function (e, agentId, heartbeatInformation) {
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
                var agentNumber = $scope.currentAgentNumber;
                agentIndex = agentNumber - 1;
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
                    selfdestruct: setTimeout(function () { $scope.timeOut(agentIndex) }, 5000)
                };
                $scope.currentAgentNumber += 1;
                $scope.agents.push(agent);
                $scope.newAgentAlert(agentNumber);
            };
            // Process the Heartbeat Information:
            var listOfWorkers = heartbeatInformation.Workers;
            for (var i = 0; i < listOfWorkers.length; i++) {
                var workerId = listOfWorkers[i]
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

    $scope.$parent.$on("agentPongResponse", function (e, agentId, value) {
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

    $scope.$parent.$on("workerPongResponse", function (e, agentId, workerId, value) {
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

    $scope.$parent.$on("agentsLog", function (e, agentId, message) {
        $scope.$apply(function () {
            var agentIndex;
            for (var index = 0; index < $scope.agents.length; index++) {
                if ($scope.agents[index].id == agentId) {
                    agentIndex = index;
                };
            };
            if (agentIndex != undefined) {
                $scope.agents[agentIndex].output.push(message);
            }
        });
    });

    $scope.$parent.$on("workersLog", function (e, agentId, workerId, message) {
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
                $scope.agents[agentIndex].workers[workerIndex].output.push(message);
            }
        });
    });


}