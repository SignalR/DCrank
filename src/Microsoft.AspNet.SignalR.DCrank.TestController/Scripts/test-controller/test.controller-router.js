testControllerApp.config(function ($stateProvider, $urlRouterProvider) {
    // Default to the 'home' state
    $urlRouterProvider.when('', 'home');

    $stateProvider
        .state('home', {
            url: '/home'
        })
        // Agent management
        .state('agents', {
            url: '/agents',
            templateUrl: '/Templates/agent_menu.html'
        })
        // Agent level blade
        .state('agents.agent', {
            url: '/agent/:agentId',
            templateUrl: '/Templates/agent_detail.html'
        })
        // Worker level blade
        .state('agents.agent.worker', {
            url: '/worker/:workerId',
            templateUrl: '/Templates/worker_detail.html'
        })
        // Run management
        .state('run', {
            url: '/runs',
            templateUrl: '/Templates/run_menu.html'
        })
        // New manual run
        .state('run.manual', {
            url: '/manual',
            templateUrl: '/Templates/manual_run.html'
        });
});
