testControllerApp.controller('RunDetailController', [
    '$scope', 'performanceModelService',
    function ($scope, performanceModelService) {
        var vm = this;
        vm.plotId = 'plot' + $scope.$id;
        vm.plotData = [];

        vm.updateInformation = performanceModelService.getPerformanceCounterUpdateInformation();
        vm.performanceCounters = performanceModelService.getPerformanceCounters();

        vm.selectedCounter = undefined;
        vm.plot = null;

        var replot = function () {
            if (vm.plot) {
                vm.plot.destroy();
            }

            if (vm.plotData && vm.plotData.length && vm.plotData.length > 0) {
                vm.plot = $.jqplot(vm.plotId, vm.plotData, {
                    series: [
                        {
                            markerOptions: {
                                style: 'dimaond',
                            }
                        }
                    ],
                    axes: {
                        xaxis: {
                            label: 'Time',
                            renderer: $.jqplot.DateAxisRenderer,
                            tickOptions: { formatString: '%H:%M:%S' }
                        }
                        //,
                        //yaxis: {
                        //    label: $scope.selectedPerformanceCounter,
                        //    labelRenderer: $.jqplot.CanvasAxisLabelRenderer
                        //}
                    },
                    highlighter: {
                        show: true
                    }
                });
            }
        };

        $scope.$watch(
            function () {
                return vm.selectedCounter;
            },
            function (newValue, oldValue) {
                vm.plotData = newValue;
                replot();
            });

        $scope.$watch(
            function () {
                return vm.updateInformation.timestamp;
            },
            replot);
    }
]);
