testControllerApp.config(function ($stateProvider, $urlRouterProvider) {
    // Default to the 'home' state
    $urlRouterProvider.when('', 'home');

    $stateProvider
        // Top level UI state
        .state('home', {
            url: '/home',
            views: {
                'top': {
                    templateUrl: '/Templates/run_menu.html',
                },
                'body': {
                    templateUrl: '/Templates/agent_menu.html'
                }
            }
        })
        // Agent level blade
        .state('home.agent', {
            url: '/agent/:agentId',
            templateUrl: '/Templates/agent_detail.html'
        })
        // Worker level blade
        .state('home.agent.worker', {
            url: '/worker/:workerId',
            templateUrl: '/Templates/worker_detail.html'
        });
});
