var MetronicApp = angular.module('MetronicApp', ['datatables', 'blockUI']);
//var validator;
MetronicApp.controller('StaffMemberController', function ($rootScope, $scope, $http, $timeout, staffmemberService) {
    
    //$scope.Message = "";
    //$scope.FileInvalidMessage = "";
    $scope.SelectedFileForUpload = null;
    //// $scope.FileDescription = "";
    //$scope.IsFormSubmitted = false;
    //$scope.IsFileValid = false;
    $scope.IsFormValid = false;

    $scope.$on('$viewContentLoaded', function () {
        // initialize core components
        //App.init();
        //App.initAjax();
        //App.initComponents(); // init core components
        //Layout.init();
    });

    // set sidebar closed and body solid layout mode
    $rootScope.settings.layout.pageContentWhite = true;
    $rootScope.settings.layout.pageBodySolid = false;
    $rootScope.settings.layout.pageSidebarClosed = false;

    $scope.validator = $('#formStaffMember').validate({
        rules: {
            firstName: {
                required: true,
            },
            lastName: {
                required: true
            },
            email: {
                required: true,
                email: true
            },
            password: {
                required: true
            },
            confirmPassword: {
                required: true,
                equalTo: "#password"
            }
        }
    });

    $scope.staffmembers = {};
    $scope.staffmember = {};
 
    $scope.selectFileforUpload = function (file) {
        
        $scope.SelectedFileForUpload = file[0];
    }

    $scope.GetStaffMembers = function () {
        debugger;
        staffmemberService.GetStaffMembers()
            .success(function (data) {
                debugger;
                $scope.staffmembers = data;
            }).error(function (error) {
                toastr["error"]("Some error has occured.")
            });
    };

    $scope.GetStaffMembers();
  
    $scope.modalStaffMember = function (staffmember) {
       
        if (staffmember === undefined) {
            $scope.staffmember = {};
            $scope.staffmember.UserId = 0;
            $("#Email").removeAttr("disabled");
            $("#MobileNumber").removeAttr("disabled");
        }
        else {
            staffmemberService.GetStaffMember(staffmember.UserId)
                .success(function (data) {
                    $scope.staffmember = data;
                }).error(function (error) {
                    toastr["error"]("Some error has occured.")
                });
            $("#Email").attr("disabled", "disabled");
            $("#MobileNumber").attr("disabled", "disabled");
        }

        $("#modal-StaffMember").modal("show");
        $("#modal-StaffMember").on("hide.bs.modal", function (e) {
            $scope.validator.resetForm();
        })
    }
   
    //var formdata = new FormData();
    //$scope.getTheFiles = function ($files) {
    //    angular.forEach($files, function (value, key) {
    //        formdata.append(key, value);
    //    });
    //}
    //$scope.LoadFileData = function (files) {
    //  
    //    var formData = new FormData();
    //    for (var file in files) {
    //        formData.append("file", files[file]);
    ////    }
    //};
    $scope.saveStaffMember = function (form) {
       
        $scope.IsFormSubmitted = true;
        //$scope.Message = "";
        if ($('#formStaffMember').valid()) {
            
           
        //    $scope.staffmember.file = $('input[type=file]')[0].files[0];
            staffmemberService.saveStaffMember($scope.staffmember,$scope.SelectedFileForUpload).then(function (response) {
                if (response.data.result) {
                    $scope.GetStaffMembers();
                    $("#modal-StaffMember").modal("hide");
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


    $scope.deleteStaffMember = function (userId) {

        bootbox.confirm("Are you sure to delete this StaffMember?", function (ans) {
            if (ans) {
                staffmemberService.deleteStaffMember(userId).then(function (response) {
                    $scope.GetStaffMembers();
                    toastr["success"]("Data saved successfully.")
                    $("#modal-StaffMember").modal("hide");
                }, function (error) {
                    toastr["error"]("Some error has occured.");
                });
            }
            else {

            }
        });



    }

});


    MetronicApp.factory('staffmemberService', function ($http) {
        
        var staffmemberService = {};

        staffmemberService.GetStaffMembers = function () {
            return $http.get('/api/StaffMember/GetStaffMembers');
        }

        staffmemberService.GetStaffMember = function (userId) {
            return $http({
                url: redirectionUrl + 'api/StaffMember/GetStaffMember',
                method: "GET",
                params: { id: userId }
            });
        }
        staffmemberService.saveStaffMember = function (model,file) {
            
            // var $file = $('input[type=file]')[0].files[0];
           //var formData = new FormData();
           //formData.append("file", file);
           //formData.append("model", model);
           //var data1 =
           //{
           //    model:model,
           //    file:file
           //};
            
           //return $http.post("http://localhost:3094/api/StaffMember/SaveStaffMember", formData,
           //   {
           //      // withCredentials: true,
           //      headers: { 'Content-Type': undefined },
           //      transformRequest: angular.identity
            //  });
           
            return $http({
                method: 'POST',
                //data: JSON.stringify(model),
                url: redirectionUrl + "api/StaffMember/SaveStaffMember",
                headers: { 'Content-Type': undefined },
                //transformRequest: angular.identity
                transformRequest: function (data) {
                   
                    var formData = new FormData();
                    formData.append("model", angular.toJson(data.model));
                    formData.append("file", data.file);
                   
                    return formData;
                    },
                data: { model: model, file: file }
               
            });
        };
        staffmemberService.deleteStaffMember = function (userId) {
            return $http({
                method: "DELETE",
                url: "/api/StaffMember/DeleteStaffMember?id=" + userId,
                data: { id: userId },
            });
        };

        return staffmemberService;   
    });

    //app1.factory('FileUploadService', function ($http, $q) { // explained abour controller and service in part 2

    //    var fac = {};
    //    fac.SaveFile = function (file) {
    //        var formData = new FormData();
    //        formData.append("file", file);
    //        //We can send more data to server using append         
    //        //formData.append("description", description);

    //        var defer = $q.defer();
    //        $http.post("/Data/SaveFiles", formData,
    //            {
    //                // withCredentials: true,
    //                headers: { 'Content-Type': undefined },
    //                transformRequest: angular.identity
    //            })
    //        .success(function (d) {
    //            defer.resolve(d);
    //        })
    //        .error(function () {
    //            defer.reject("File Upload Failed!");
    //        });

    //        return defer.promise;

    //    }
    //    return fac;

    //});