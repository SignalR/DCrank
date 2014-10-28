testControllerApp.controller("WorkerDetailController", [
    '$stateParams', 'modelService', function ($stateParams, modelService) {
        modelService.bindWorker(this, 'worker', $stateParams.agentId, Number($stateParams.workerId));

        this.agentId = $stateParams.agentId;
    }
]);