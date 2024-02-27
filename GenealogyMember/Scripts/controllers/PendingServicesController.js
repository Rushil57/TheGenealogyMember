var MetronicApp = angular.module('MetronicApp', ['datatables', 'blockUI']);
MetronicApp.controller('PendingServicesController', function ($rootScope, $scope, $http, $timeout, pendingservice) {
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
    
   
    $scope.getPendingServices = function () {
        pendingservice.getPendingServices()
            .success(function (data) {
                $scope.pendingServices = data;
            }).error(function (error) {
                toastr["error"]("Some error has occured.")
            });
    };
    $scope.getPendingServices();
    $scope.pendingServices = {};
    $scope.pendingService = {};

    $scope.modalPendingServices = function (service) {
      
        pendingservice.getPendingService(service.ServiceId)
                .success(function (data) {
                    $scope.pendingService = data;
                }).error(function (error) {
                    toastr["error"]("Some error has occured.")
                });
        $("#modal-PendingServices").modal("show");
    }
    $scope.savePendingService = function (form) {
       
        if ($('#formPendingServices').valid()) {
            var rStart = $scope.pendingService.StartDate.split('/');
            var dStart = rStart[1] + '/' + rStart[0] + '/' + rStart[2];
            var rEnd = $scope.pendingService.EndDate.split('/');
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
            pendingservice.savePendingService($scope.pendingService).then(function (response) {
                if (response.data.result) {
                    $scope.getPendingServices();
                    $("#modal-PendingServices").modal("hide");
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
    $scope.modalAssignedservice = function (pendingService)
    {

        pendingservice.GetStaffMembers(pendingService.ServiceId)
           .success(function (data) {
              
               $scope.staffmember = data;
             
               $('#dropdownStaffMember').empty();
               $('#dropdownStaffMember').append('<option value="' + 0 + '">' + "-- Select Staff --" + '</option>');
               $.each(data, function (index, value) {
                 
                   $('#dropdownStaffMember').append('<option value="' + value.UserId + '">' + value.FirstName + " " + value.LastName + '</option>');
               });
           }).error(function (error) {
               toastr["error"]("Some error has occured.")
           });
        $("#hdnServiceId").val(pendingService.ServiceId);
        $("#modal-Assignedservice").modal("show");
    }
    $scope.assignService = function (form) {
       
        if ($('#formPendingServices').valid()) {
            pendingservice.assignService($("#hdnServiceId").val(), $("#dropdownStaffMember").val()).then(function (response) {
                
                if (response.data.result) {
                   
                    $scope.getPendingServices();
                    $("#modal-Assignedservice").modal("hide");
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

MetronicApp.factory('pendingservice', function ($http) {

    var pendingservice = {};

    pendingservice.getPendingServices = function () {
        return $http({
            url: redirectionUrl + "api/PendingServices/GetPendingServices",
            method: "GET",
        });
    }
    pendingservice.GetStaffMembers = function (serviceId) {
        
        return $http({
            url: redirectionUrl + "api/PendingServices/GetFilteredStaffMembers",
            params:{
                serviceId: serviceId
            },
            method: "GET",
        });
    }
    pendingservice.assignService = function (ServiceId, UserId) {
       
        return $http({
            url: redirectionUrl + "api/PendingServices/assignService",
            method: "GET",
            params: {
                ServiceId: ServiceId,
                UserId: UserId
            }
        });
    }
    pendingservice.getPendingService = function (serviceId) {
       
        return $http({
            url: redirectionUrl + 'api/PendingServices/GetPendingService',
            method: "GET",
            params: { ServiceId: serviceId }
        });
    }
    pendingservice.savePendingService = function (model) {
        return $http({
            method: "POST",
            url: redirectionUrl + "api/PendingServices/SavePendingService",
            data: JSON.stringify(model),
        });
    };
    return pendingservice;
});
