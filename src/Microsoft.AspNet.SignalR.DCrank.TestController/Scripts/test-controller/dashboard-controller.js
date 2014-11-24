testControllerApp.controller('DashboardController', [
    'hubService', 'ajaxService', function (hubService, ajaxService) {
        hubService.initialize();
        ajaxService.initialize();
    }
]);
