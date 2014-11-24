testControllerApp.controller('NewRunController', [
     '$stateParams', '$state', 'runService', 'ajaxService', '$scope',
     function ($stateParams, $state, runService, ajaxService, $scope) {
         var vm = this;
         vm.runStatus = runService.getRunStatus();

         vm.runType = $stateParams.runType;

         runService.bindRunDefinition(vm, 'run', vm.runType, $scope);

         vm.canStart = function () {
             return (vm.runStatus.controllerState === 'Idle' &&
                 !vm.runStatus.startRequested);
         }

         vm.startRun = function () {
             ajaxService.startRun(vm.run);
             $state.go('^.current');
         }
     }
]);
