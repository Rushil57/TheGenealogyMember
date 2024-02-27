var MetronicApp = angular.module('MetronicApp', ['datatables', 'blockUI']);
//var validator;
MetronicApp.controller('CustomerController', function ($rootScope, $scope, $http, $timeout, customerService) {
  
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

    $scope.validator = $('#formCustomer').validate({
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

    $scope.customers = {};
    $scope.customer = {};

    $scope.GetCustomers = function () {
        customerService.GetCustomers()
            .success(function (data) {
                $scope.customers = data;
            }).error(function (error) {
                toastr["error"]("Some error has occured.")
            });
    };

    $scope.GetCustomers();

    $scope.modalCustomer = function (customer) {
        if (customer === undefined) {
            $scope.customer = {};
            $scope.customer.UserId = 0;
            $("#Email").removeAttr("disabled");
            $("#MobileNumber").removeAttr("disabled");
        }
        else {
            customerService.GetCustomer(customer.UserId)
                .success(function (data) {
                    $scope.customer = data;
                }).error(function (error) {
                    toastr["error"]("Some error has occured.")
                });
            $("#Email").attr("disabled", "disabled");
            $("#MobileNumber").attr("disabled", "disabled");
        }

        $("#modal-Customer").modal("show");
        $("#modal-Customer").on("hide.bs.modal", function (e) {
            $scope.validator.resetForm();
        })
    }

    $scope.saveCustomer = function (form) {
        
        if ($('#formCustomer').valid()) {
            customerService.saveCustomer($scope.customer).then(function (response) {
                if (response.data.result) {
                    $scope.GetCustomers();
                    $("#modal-Customer").modal("hide");
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


    $scope.deleteCustomer = function (userId) {

        bootbox.confirm("Are you sure to delete this customer?", function (ans) {
            if (ans) {
                customerService.deleteCustomer(userId).then(function (response) {
                    $scope.GetCustomers();
                    toastr["success"]("Data saved successfully.")
                    $("#modal-Customer").modal("hide");
                }, function (error) {
                    toastr["error"]("Some error has occured.");
                });
            }
            else {

            }
        });



    }

});


MetronicApp.factory('customerService', function ($http) {
    
    var customerService = {};

    customerService.GetCustomers = function () {
        return $http.get('/api/Customer/GetCustomers');
    }

    customerService.GetCustomer = function (userId) {
        return $http({
            url: redirectionUrl + "api/Customer/GetCustomer",
            method: "GET",
            params: { id: userId }
        });
    }
    customerService.saveCustomer = function (model) {
        return $http({
            method: "POST",
            url: redirectionUrl + "api/Customer/SaveCustomer",
            data: JSON.stringify(model),
        });
    };
    customerService.deleteCustomer = function (userId) {
        return $http({
            method: "DELETE",
            url: "/api/Customer/" + userId,
            data: { id: userId },
        });
    };

    return customerService;
});