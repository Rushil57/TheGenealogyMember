var MetronicApp = angular.module('MetronicApp', ['datatables', 'blockUI']);
MetronicApp.controller('RejectedServicesController', function ($rootScope, $scope, $http, $timeout, rejectedservice) {
   
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

   
  
    $scope.getRejectedServices = function () {
       
        rejectedservice.getRejectedServices()
            .success(function (data) {
                $scope.rejectedServices = data;
            }).error(function (error) {
                toastr["error"]("Some error has occured.")
            });
    };

   
    $scope.getRejectedServices();
   
    $scope.rejectedServices = {};
    $scope.rejectedService = {};


    $scope.modalRejectedServices = function (service) {
       
        rejectedservice.getRejectedService(service.ServiceId)
                .success(function (data) {
                    $scope.rejectedService = data;
                }).error(function (error) {
                    toastr["error"]("Some error has occured.")
                });
        $("#modal-RejectedServices").modal("show");
    }
    $scope.saveRejectedService = function (form) {
       
        if ($('#formRejectedServices').valid()) {
            var rStart = $scope.rejectedService.StartDate.split('/');
            var dStart = rStart[1] + '/' + rStart[0] + '/' + rStart[2];
            var rEnd = $scope.rejectedService.EndDate.split('/');
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
            rejectedservice.saveRejectedService($scope.rejectedService).then(function (response) {
                if (response.data.result) {
                    $scope.getRejectedServices();
                    $("#modal-RejectedServices").modal("hide");
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



MetronicApp.factory('rejectedservice', function ($http) {

    var rejectedservice = {};


    rejectedservice.getRejectedServices = function () {
        return $http({
            url: redirectionUrl + 'api/RejectedServices/GetRejectedServices',
            method: "GET",

        });
    }
    rejectedservice.getRejectedService = function (serviceId) {
        
        return $http({
            url: redirectionUrl + 'api/RejectedServices/GetRejectedService',
            method: "GET",
            params: { ServiceId: serviceId }
        });
    }
    rejectedservice.saveRejectedService = function (model) {
        return $http({
            method: "POST",
            url: redirectionUrl + "api/RejectedServices/SaveRejectedService",
            data: JSON.stringify(model),
        });
    };

    return rejectedservice;
});