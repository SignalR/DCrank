testControllerApp.directive('onRepeat', [
    '$timeout', function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attr) {
                if (scope.$last === true) {
                    $timeout(function () {
                        scope.$emit('repeatCompleted');
                    });
                }
            }
        }
    }
]);


testControllerApp.controller("AgentConsoleController", [
    '$scope', '$location', '$anchorScroll', '$stateParams', 'modelService',
    function ($scope, $location, $anchorScroll, $stateParams, modelService) {
        var vm = this;
        modelService.bindAgent(vm, 'agent', $stateParams.agentId);

        vm.consoleId = 'console-' + $scope.$id;
        var pinned = false;
        var autoScroll = true;

        var scrollToBottom = function () {
            if (autoScroll) {
                var console = $('#' + vm.consoleId);
                console.animate({ scrollTop: console[0].scrollHeight }, 100);
            }
        };

        $scope.$on('repeatCompleted', function () {
            scrollToBottom();
        });

        vm.pinnedClass = function () {
            if (pinned) {
                return 'selected-item';
            }
            return '';
        }

        vm.togglePin = function () {
            pinned = !pinned;
        };

        vm.autoScrollClass = function () {
            if (autoScroll) {
                return 'selected-item';
            }
            return '';
        }

        vm.toggleAutoScroll = function () {
            autoScroll = !autoScroll;
            scrollToBottom();
        };

        vm.disableAutoScroll = function () {
            autoScroll = false;
        }
    }
]);