testControllerApp.controller('DashboardController', [
    'hubService', 'ajaxService', '$state', function (hubService, ajaxService, $state) {
        hubService.initialize();
        ajaxService.initialize();

        if ($state.is('root')) {
            $state.go('root.run');
        }
    }
]);
