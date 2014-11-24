
function Model() {
    this.agents = [];
    this.performanceCounters = [];
}

function Agent(agentId) {
    this.id = agentId;
    this.workers = [];
    this.output = [];
}

function Worker(workerId) {
    this.id = workerId;
    this.output = [];
}

testControllerApp.service('modelService', [function () {
    var model = new Model();

    var service = {
        getAgents: function () {
            return model.agents;
        },
        tryGetAgent: function (agentId) {
            for (var index = 0; index < model.agents.length; index++) {
                if (model.agents[index].id === agentId) {
                    return model.agents[index];
                }
            }
            return null;
        },
        tryGetWorker: function (agent, workerId) {
            for (var index = 0; index < agent.workers.length; index++) {
                if (agent.workers[index].id === workerId) {
                    return agent.workers[index];
                }
            }
            return null;
        },
        getCreateAgent: function (agentId) {
            var agent = service.tryGetAgent(agentId);

            if (agent === null) {
                agent = new Agent(agentId);
                model.agents.push(agent);
            }
            return agent;
        },
        getCreateWorker: function (agent, workerId) {
            var worker = service.tryGetWorker(agent, workerId);

            if (worker === null) {
                worker = new Worker(workerId);
                agent.workers.push(worker);
            }
            return worker;
        },
        bindAgent: function (target, property, agentId, $scope) {
            target[property] = service.tryGetAgent(agentId);

            if (target[property] === null) {
                var unwatchAgent = $scope.$watch(
                    function () {
                        return service.tryGetAgent(agentId);
                    },
                    function (newValue, oldValue) {
                        if (newValue !== null) {
                            target[property] = newValue;
                            unwatchAgent();
                        }
                    });
            }
        },
        bindWorker: function (target, property, agentId, workerId, $scope) {
            var agent = service.tryGetAgent(agentId);

            if (agent !== null) {
                target[property] = service.tryGetWorker(agent, workerId);
            } else {
                target[property] = null;
            }

            if (target[property] === null) {
                var unwatchWorker = $scope.$watch(
                    function () {
                        agent = service.tryGetAgent(agentId);
                        if (agent !== null) {
                            return service.tryGetWorker(agent, workerId);
                        }
                        return null;
                    },
                    function (newValue, oldValue) {
                        if (newValue !== null) {
                            target[property] = newValue;
                            unwatchWorker();
                        }
                    });
            }
        }
    };

    return service;
}]);