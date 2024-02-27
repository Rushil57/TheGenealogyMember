var MetronicApp = angular.module('MetronicApp', ['datatables', 'blockUI']);
//var validator;
MetronicApp.controller('UserController', function ($rootScope, $scope, $http, $timeout, userService) {
    
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

    $scope.validator = $('#formUser').validate({
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

    $scope.users = {};
    $scope.user = {};

    $scope.selectFileforUpload = function (file) {
       
        $scope.SelectedFileForUpload = file[0];
    }

    $scope.GetUsers = function () {
        userService.GetUsers()
            .success(function (data) {
                $scope.users = data;
            }).error(function (error) {
                toastr["error"]("Some error has occured.")
            });
    };

    $scope.GetUsers();

    $scope.modalUser = function (user) {
        
        if (user === undefined) {
            $scope.user = {};
            $scope.user.UserId = 0;
            $("#Email").removeAttr("disabled");
            $("#MobileNumber").removeAttr("disabled");
            userService.getMasterAgencyUserCategory()
                .success(function (data) {
                    
                    $scope.user = data;
                }).error(function (error) {
                    toastr["error"]("Some error has occured.")
                });
        }
        else {
            userService.GetUser(user.UserId)
                .success(function (data) {
                    $scope.user = data;
                }).error(function (error) {
                    toastr["error"]("Some error has occured.")
                });
            $("#Email").attr("disabled", "disabled");
            $("#MobileNumber").attr("disabled", "disabled");
        }

        $("#modal-User").modal("show");       
        $("#modal-User").on("hide.bs.modal", function (e) {
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
    $scope.saveUser = function (form) {
        
        $scope.IsFormSubmitted = true;
        //$scope.Message = "";
        if ($('#formUser').valid()) {
            

            //    $scope.staffmember.file = $('input[type=file]')[0].files[0];
            userService.saveUser($scope.user, $scope.SelectedFileForUpload).then(function (response) {
                if (response.data.result) {
                    $scope.GetUsers();
                    $("#modal-User").modal("hide");
                    toastr["success"](response.data.message);
                }
                else {
                    toastr["error"](response.data.message);
                }

            }, function (error) {
                toastr["error"]("Some error has occured.");
            });
        }
    }


    $scope.deleteUser = function (userId) {

        bootbox.confirm("Are you sure to delete this User?", function (ans) {
            if (ans) {
                userService.deleteUser(userId).then(function (response) {
                    $scope.GetUsers();
                    toastr["success"]("Data saved successfully.")
                    $("#modal-User").modal("hide");
                }, function (error) {
                    toastr["error"]("Some error has occured.");
                });
            }
            else {

            }
        });



    }

});


MetronicApp.factory('userService', function ($http) {
    
    var userService = {};

    userService.GetUsers = function () {
        return $http.get('/api/User/GetUsers');
    }

    userService.GetUser = function (userId) {
        return $http({
            url: redirectionUrl + 'api/User/GetUser',
            method: "GET",
            params: { id: userId }
        });
    }
    userService.getMasterAgencyUserCategory = function () {
        return $http({
            url: redirectionUrl + 'api/User/GetMasterAgencyUserCategory',
            method: "GET",

        });
    }
    userService.saveUser = function (model, file) {
        
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
            url: redirectionUrl + "api/User/SaveUser",
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
    userService.deleteUser = function (userId) {
        return $http({
            method: "DELETE",
            url: "/api/User/DeleteUser?id=" + userId,
            data: { id: userId },
        });
    };

    return userService;
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