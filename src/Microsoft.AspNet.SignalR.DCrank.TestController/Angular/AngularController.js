var angularModule = angular.module('angularModule', []);

angularModule.service('signalRSvc', function ($rootScope) {
    var proxy = null;

    var initialize = function () {

        //Getting the connection object
        connection = $.hubConnection();

        //Creating proxy
        this.proxy = connection.createHubProxy('controllerHub');

        //Starting connection
        connection.start();

        this.proxy.on('agentConnected', function (agentId) {
            $rootScope.$emit("agentConnected", agentId);
        });

        this.proxy.on('workerConnected', function (agentId, workerId) {
            $rootScope.$emit("workerConnected", agentId, workerId);
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
    };

    var startWorker = function (agentId, numberOfWorkers) {
        for (var i = 0; i < numberOfWorkers; i++) {
            this.proxy.invoke('startWorker', agentId);
        }
    };

    var pingWorker = function (agentId, workerId) {
        this.proxy.invoke('pingWorker', agentId, workerId, 7);
    };

    return {
        initialize: initialize,
        startWorker: startWorker,
        pingWorker: pingWorker
    };
});

function SignalRAngularCtrl($scope, signalRSvc, $rootScope) {
    $scope.agents = [];

    $scope.workersToStart = '';

    $scope.currentAgentNumber = 1;

    $scope.uiGeneralDisplay = [];

    $scope.newAgentAlert = function (agentId) {
        var message = agentId + ' has connected to the hud';
        $scope.uiGeneralDisplay.push(message);
    }

    $scope.newWorkerAlert = function (agentId, workerId) {
        var message = workerId + ' has connected to the agent: ' + agentId;
        $scope.uiGeneralDisplay.push(message);
    }

    $scope.pingAgentAlert = function (agentId, value) {
        var message = agentId + ' has responded to the ping with ' + value;
        $scope.uiGeneralDisplay.push(message);
    }

    $scope.pingWorkerAlert = function (agentId, workerId, value) {
        var message = 'Worker with id: ' + workerId + ' under' +
                    ' agent: ' + agentId + ' has responded to the ping with ' + value;
        $scope.uiGeneralDisplay.push(message);
    }

    $scope.pingWorker = function () {
        var agentId = this.parent.agent.id;
        var workerId = this.worker.id;
        signalRSvc.pingWorker(agentId, workerId);
    }

    $scope.showLogging = function () {
        this.worker.display = !this.worker.display;
    }

    signalRSvc.initialize();

    $scope.startWorkers = function () {
        var agentId = this.agent.id;
        var workersToStart = Number($scope.workersToStart);
        $scope.workersToStart = '';
        signalRSvc.startWorker(agentId, workersToStart);
    };

    $scope.$parent.$on("agentConnected", function (e, agentId) {
        $scope.$apply(function () {
            var newAgent = true;
            for (var i = 0; i < $scope.agents.length; i++) {
                if ($scope.agents[i].id == agentId) {
                    newAgent = false;
                };
            };
            if (newAgent) {
                var agentNumber = $scope.currentAgentNumber;
                var agent = {
                    number: agentNumber,
                    id: agentId,
                    numberOfWorkers: 0,
                    workers: [],
                    output: [],
                    display: false
                };
                $scope.currentAgentNumber += 1;
                $scope.agents.push(agent);
                $scope.newAgentAlert(agentId);
            };
        });
    });

    $scope.$parent.$on("workerConnected", function (e, agentId, workerId) {
        $scope.$apply(function () {
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
                    display:false
                };
                $scope.agents[agentIndex].numberOfWorkers += 1;
                $scope.agents[agentIndex].workers.push(worker);
                $scope.newWorkerAlert(agentId, workerId);
            };
        });
    });

    $scope.$parent.$on("agentPongResponse", function (e, agentId, value) {
        $scope.$apply(function () {
            $scope.pingAgentAlert(agentId, value);
        });
    });

    $scope.$parent.$on("workerPongResponse", function (e, agentId, workerId, value) {
        $scope.$apply(function () {
            $scope.pingWorkerAlert(agentId, workerId, value);
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
            $scope.agents[agentIndex].output.push(message);
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
            $scope.agents[agentIndex].workers[workerIndex].output.push(message);
        });
    });


}