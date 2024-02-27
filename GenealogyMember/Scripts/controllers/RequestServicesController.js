angular.module('MetronicApp', ['blockUI']).controller('DashboardController', function ($rootScope, $scope, $http, $timeout, blockUI, requestService) {
    
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
   
    $scope.user = {};
    $scope.requestService = {};
    $scope.modalservice = function (user) {
      
        $('#modal-service').on('hidden.bs.modal', function () {
            $("input,textarea,select").val('');

        });
        requestService.getMasterServices()
         .success(function (data) {
             
             $scope.requestService.masterServices = data;
         }).error(function (error) {
             toastr["error"]("Some error has occured.")
         });
        $("#modal-service").modal("show");
    }
    $scope.modalserviceManual = function (user) {

        requestService.getServiceManual()
         .success(function (data) {
             $("#dvServiceManualContent").html(data.embeddedString);
             $("#modal-serviceManual").modal("show");

         }).error(function (error) {
             toastr["error"]("Some error has occured.")
         });

    }
    $scope.SaveRequestServices = function (form) {
       
        if ($('#formService').valid()) {
           
            var rStart = $scope.requestService.StartDate.split('/');
            var dStart = rStart[1] + '/' + rStart[0] + '/' + rStart[2];          
            var rEnd = $scope.requestService.EndDate.split('/');
            var dEnd = rEnd[1] + '/' + rEnd[0] + '/' + rEnd[2];          
            if (dStart > dEnd)
            {
                toastr["error"]("End Date should be greater or equal to Start Date.");
                return false;
            }
            if (dStart == dEnd)
            {
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
                if(timeEnd <= timeStart)
                {
                    toastr["error"]("End time should be greater or equal to Start time.");
                    return false;
                }

            }
            $scope.requestService.ServiceMasterId = $scope.requestService.masterServices.ServiceMasterId;
            requestService.saveService($scope.requestService).then(function (response) {
                
                if (response.data.result) {
                   // $scope.getUsers();
                    $("#modal-service").modal("hide");
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

angular.module('MetronicApp').factory('requestService', function ($http) {
    
    var requestService = {};
    requestService.saveService = function (model) {
        
        return $http({
            method: "POST",
            url: redirectionUrl + "api/RequestService/SaveRequestServices",
            data: JSON.stringify(model),
        });
    };
    requestService.getMasterServices = function () {
        return $http({
            url: redirectionUrl + 'api/RequestService/GetMasterServices',
            method: "GET",

        });
    }
    requestService.getServiceManual = function () {
        return $http({
            url: redirectionUrl + 'Dashboard/GetServiceManual',
            method: "GET",

        });
    }
    return requestService;
});
