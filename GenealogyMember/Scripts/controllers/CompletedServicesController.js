var MetronicApp = angular.module('MetronicApp', ['datatables', 'blockUI']);
MetronicApp.controller('CompletedServicesController', function ($rootScope, $scope, $http, $timeout, completedservice) {
   
    $scope.$on('$viewContentLoaded', function () {
        // initialize core components
        //Layout.init();
        //App.initAjax();
        //App.initComponents(); // init core components
        //Layout.init();
    });

    // set sidebar closed and body solid layout mode
    $rootScope.settings.layout.pageContentWhite = true;
    $rootScope.settings.layout.pageBodySolid = false;
    $rootScope.settings.layout.pageSidebarClosed = false;



    $scope.getCompletedServices = function () {
      
        completedservice.getCompletedServices()
            .success(function (data) {
                $scope.completedServices = data;
            }).error(function (error) {
                toastr["error"]("Some error has occured.")
            });
    };


    $scope.getCompletedServices();

    $scope.completedServices = {};
    $scope.completedService = {};



    $scope.modalCompletedServices = function (service) {
        
        completedservice.getCompletedService(service.ServiceId)
                .success(function (data) {
                    $scope.completedService = data;
                }).error(function (error) {
                    toastr["error"]("Some error has occured.")
                });
        $("#modal-CompletedServices").modal("show");
    }
    $scope.saveCompletedService = function (form) {
       
        if ($('#formCompletedServices').valid()) {
            var rStart = $scope.completedService.StartDate.split('/');
            var dStart = rStart[1] + '/' + rStart[0] + '/' + rStart[2];
            var rEnd = $scope.completedService.EndDate.split('/');
            var dEnd = rEnd[1] + '/' + rEnd[0] + '/' + rEnd[2];
            if (dStart > dEnd) {
                toastr["error"]("End Date should be greater or equal to Start Date.");
                return false;
            }
            if (dStart == dEnd) {
                var ttStart = $("#ddlStartTime").val();
                var hrsStart = Number(ttStart.match(/^(\d+)/)[1]);
                var mntsStart = Number(ttStart.match(/:(\d+)/)[1]);
                var formatStart = ttStart.match(/\s(.*)$/)[1];
                if (formatStart == "PM" && hrsStart < 12) hrsStart = hrsStart + 12;
                if (formatStart == "AM" && hrsStart == 12) hrsStart = hrsStart - 12;
                var hoursStart = hrsStart.toString();
                var minutesStart = mntsStart.toString();
                if (hrsStart < 10) hoursStart = "0" + hoursStart;
                if (mntsStart < 10) minutesStart = "0" + minutesStart;
                var timeStart = hoursStart + ":" + minutesStart;

                var ttEnd = $("#ddlEndTime").val();
                var hrsEnd = Number(ttEnd.match(/^(\d+)/)[1]);
                var mntsEnd = Number(ttEnd.match(/:(\d+)/)[1]);
                var formatEnd = ttEnd.match(/\s(.*)$/)[1];
                if (formatEnd == "PM" && hrsEnd < 12) hrsEnd = hrsEnd + 12;
                if (formatEnd == "AM" && hrsEnd == 12) hrsEnd = hrsEnd - 12;
                var hoursEnd = hrsEnd.toString();
                var minutesEnd = mntsEnd.toString();
                if (hrsEnd < 10) hoursEnd = "0" + hoursEnd;
                if (mntsEnd < 10) minutesEnd = "0" + minutesEnd;
                var timeEnd = hoursEnd + ":" + minutesEnd;
                if (timeEnd <= timeStart) {
                    toastr["error"]("End time should be greater or equal to Start time.");
                    return false;
                }

            }
            completedservice.saveCompletedService($scope.completedService).then(function (response) {
                if (response.data.result) {
                    $scope.getCompletedServices();
                    $("#modal-CompletedServices").modal("hide");
                    toastr["success"](response.data.message);
                }
                else {
                    toastr["warning"](response.data.message);
                }

            }, function (error) {
                toastr["error"]("Some error has occured.");
            });
        }
    }

});



MetronicApp.factory('completedservice', function ($http) {

    var completedservice = {};


    completedservice.getCompletedServices = function () {
        return $http({
            url: redirectionUrl + 'api/CompletedServices/GetCompletedServices',
            method: "GET",

        });
    }
    completedservice.getCompletedService = function (serviceId) {
        
        return $http({
            url: redirectionUrl + 'api/CompletedServices/GetCompletedService',
            method: "GET",
            params: { ServiceId: serviceId }
        });
    }
    completedservice.saveCompletedService = function (model) {
        return $http({
            method: "POST",
            url: redirectionUrl + "api/CompletedServices/SaveCompletedService",
            data: JSON.stringify(model),
        });
    };

    return completedservice;
});