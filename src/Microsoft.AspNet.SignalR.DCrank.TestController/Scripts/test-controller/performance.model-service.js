
function PerformanceCounterUpdateInformation() {
    this.timestamp = null;
};

function PerformanceCounter(name) {
    this.name = name;
    this.dataSeries = [];
};

testControllerApp.service('performanceModelService', [
    function () {
        var model = {
            updateInformation: new PerformanceCounterUpdateInformation(),
            counters: []
        };

        var tryGetPerformanceCounter = function (name) {
            for (var index = 0; index < model.counters.length; index++) {
                if (model.counters[index].name === name) {
                    return model.counters[index];
                }
            }
            return null;
        };

        var service = {
            getCreatePerformanceCounter: function (name) {
                var performanceCounter = tryGetPerformanceCounter(name);

                if (performanceCounter === null) {
                    performanceCounter = new PerformanceCounter(name);
                    model.counters.push(performanceCounter);
                }
                return performanceCounter;
            },
            getPerformanceCounters: function () {
                return model.counters;
            },
            getPerformanceCounterUpdateInformation: function () {
                return model.updateInformation;
            }
        };
        return service;
    }
]);
