testControllerApp.config(function ($stateProvider, $urlRouterProvider) {
    $stateProvider
        .state('root', {
            url: '',
            templateUrl: 'Templates/dashboard.html'
        })
        // Agent management
        .state('root.agents', {
            url: '/agents',
            templateUrl: 'Templates/agent_menu.html'
        })
        // Agent level blade
        .state('root.agents.agent', {
            url: '/agent/:agentId',
            templateUrl: 'Templates/agent_detail.html'
        })
        // Worker level blade
        .state('root.agents.agent.worker', {
            url: '/worker/:workerId',
            templateUrl: 'Templates/worker_detail.html'
        })
        // Run management blade
        .state('root.run', {
            url: '/runs',
            templateUrl: 'Templates/run_menu.html'
        })
        // New run blade
        .state('root.run.new', {
            url: '/new/:runType',
            templateUrl: 'Templates/new_run.html'
        })
        // Current run blade
        .state('root.run.current', {
            url: '/current',
            templateUrl: 'Templates/current_run.html'
        })
        .state('root.run.current.detail', {
            url: '/detail',
            templateUrl: 'Templates/run_detail.html'
        })
        // Current run detail blade
        .state('root.run.current.agent', {
            url: '/agent/:agentId',
            templateUrl: 'Templates/agent_detail.html'
        })
        .state('root.run.current.agent.worker', {
            url: '/worker/:workerId',
            templateUrl: 'Templates/worker_detail.html'
        });
});
