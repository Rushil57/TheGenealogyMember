var LoginApp = angular.module('LoginApp', ['blockUI']);
//var validator;
LoginApp.controller('LoginController', function ($rootScope, $scope, $http, $timeout, loginService, blockUI) {
   
    $scope.$on('$viewContentLoaded', function () {
       
    });

    $scope.validator = $('#FormSignUp').validate({
        rules: {
            email: {
                required: true,
                email: true
            },
            mobileNumber: {
                required: true
            },
        }
    });

    $scope.login = {};

    $scope.verifyAccessCode = function () {
        
        if ($('#formAccessCode').valid())
            {
            loginService.verifyAccessCode($("#hdnEmail").val(), $('#hdnMobileNumber').val(), $scope.login.AccessCode, $scope.login.FirstName, $scope.login.LastName).then(function (response) {
                if (response.data.isVerified) {
                    $("#modal-AccessCode").modal("hide");
                    window.location.href = '/Dashboard/Default';
                }
                else {
                    toastr["warning"](response.data.message);
                }

            }, function (error) {
                toastr["error"]("Some error has occured.");
            });
    }
}   

    $scope.loginUser = function () {
       
        if ($("#email").val() == '' || $("#mobileNumber").val() == '')
        {
            $("#dvValidationMessage").show();
            return false;
        }

        if ($('#FormSignUp').valid()) {
            blockUI.start();
            loginService.loginUser($scope.login).then(function (response) {
                
                if (response.data.isInserted) {
                    $("#modal-AccessCode").modal("show");
                    $("#hdnEmail").val(response.data.email);
                    $("#hdnMobileNumber").val(response.data.mobileNumber);
                }
                else {
                   
                    window.location.href = '/Dashboard/Default';
                    
                }
                blockUI.stop();
            }, function (error) {
                alert(error.data);
                toastr["error"]("Some error has occured.");
            });
        }
    }



});


LoginApp.factory('loginService', function ($http) {
   
    var loginService = {};
    
    loginService.loginUser = function (model) {
    
        return $http({
            method: "POST",
            url: "/Account/Login",
            data: JSON.stringify(model),
        });
    };

    loginService.verifyAccessCode = function (email, mobileNumber, accessCode, firstName, lastName) {
       
        return $http({
            method: "POST",
            url: "/Account/VerifyAccessCode",
            params: { Email:email, MobileNumber:mobileNumber, AccessCode:accessCode, FirstName:firstName, LastName:lastName }
            
        });
    };

    loginService.loginVerifiedUser = function () {
        
        return $http({
            method: "GET",
            url: "/Account/LoginVerifiedUser"
        });
    };
   

    return loginService;
});