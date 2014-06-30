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
    };

    var startWorker = function (agentId) {
        this.proxy.invoke('startWorker', agentId);
    };

    var pingWorker = function (workerId) {
        this.proxy.invoke('pingWorker', workerId, 7);
    };

    return {
        initialize: initialize,
        startWorker: startWorker,
        pingWorker: pingWorker
    };
});

function SignalRAngularCtrl($scope, signalRSvc, $rootScope) {
    $scope.agents = [];

    addAgent = function (agent) {
        $scope.agents.push(agent);
    };

    addWorker = function (agentIndex, worker) {
        $scope.agents[agentIndex].numberOfWorkers += 1;
        $scope.agents[agentIndex].workers.push(worker);
    };

    $scope.pingWorker = function () {
        var workerId = this.worker.id;
        signalRSvc.pingWorker(workerId);
    }

    signalRSvc.initialize();

    $scope.startWorkers = function () {
        var agentId = this.agent.id;
        signalRSvc.startWorker(agentId);
    };

    $scope.$parent.$on("agentConnected", function (e, agentId) {
        $scope.$apply(function () {
            var newAgent = true;
            for (var i = 0; i < $scope.agents.length; i++) {
                if ($scope.agent[i].id == agentId) {
                    newAgent = false;
                };
            };
            if (newAgent) {
                var agent = {
                    id: agentId,
                    numberOfWorkers: 0,
                    workers: []
                };
                addAgent(agent);
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
                };
                addWorker(agentIndex, worker);
            };
        });
    });
}