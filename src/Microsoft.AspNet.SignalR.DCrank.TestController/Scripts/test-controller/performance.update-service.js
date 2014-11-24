testControllerApp.service('performanceUpdateService', [
    'performanceModelService',
    function (performanceModelService) {
        var service = {
            updatePerformanceData: function (performanceData, timestamp) {
                var updateInformation = performanceModelService.getPerformanceCounterUpdateInformation();
                if (performanceData === null || updateInformation.timestamp === timestamp) {
                    return;
                }
                updateInformation.timestamp = timestamp;

                var updateTime = new Date(timestamp).getTime();

                for (var index = 0; index < performanceData.length; index++) {
                    var name = performanceData[index].CounterName;
                    var value = performanceData[index].RawValue;

                    var counter = performanceModelService.getCreatePerformanceCounter(name);

                    counter.dataSeries.push([updateTime, value]);

                    // Limit the realtime data series size
                    while (counter.dataSeries.length > 50) {
                        counter.dataSeries.shift();
                    };
                }
            }
        };
        return service;
    }
]);
