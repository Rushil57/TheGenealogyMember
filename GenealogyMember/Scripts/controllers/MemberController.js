var MetronicApp = angular.module('MetronicApp', ['datatables', 'blockUI']);
//var validator;
MetronicApp.controller('MemberController', function ($rootScope, $scope, $http, $timeout, memberService) {
    
    //$scope.Message = "";
    //$scope.FileInvalidMessage = "";
   // $scope.SelectedFileForUpload = null;
    //// $scope.FileDescription = "";
    //$scope.IsFormSubmitted = false;
    //$scope.IsFileValid = false;
    //$scope.IsFormValid = false;

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

    $scope.validator = $('#formMember').validate({
        rules: {
            theFamilyMemberId: {
                required: true,
            },
            name: {
                required: true,
            },
            dob: {
                required: true,              
            },
            bloodGroup: {
                required: true
            },
            address: {
                required: true,              
            },
            emergencyContactName_No: {
                required: true,              
            },
            medicalAllergy: {
                required: true,              
            }
        }
    });

    $scope.members = {};
    $scope.member = {};
    $scope.viewMember = {};

  

    $scope.GetMembers = function () {
        memberService.GetMembers()
            .success(function (data) {
                $scope.members = data;
            }).error(function (error) {
                toastr["error"]("Some error has occured.")
            });
    };

    $scope.GetMembers();

    $scope.modalMember = function (member) {
      
        if (member === undefined) {
            $scope.member = {};
            $scope.member.MemberId = 0;
            $("#dvTheFamilyMemberId").show();
        }
        else {
            memberService.GetMember(member.MemberId)
                .success(function (data) {
                    $scope.member = data;
                }).error(function (error) {
                    toastr["error"]("Some error has occured.")
                });
            $("#dvTheFamilyMemberId").hide();
        }

        $("#modal-Member").modal("show");
        $("#modal-Member").on("hide.bs.modal", function (e) {
            $scope.validator.resetForm();
        })
    }

    //$scope.generateBarcode = function (memberId) {
    //    memberService.GenerateBarcode(memberId)
    //            .success(function (data) {
    //                if (data.result) {                                          
    //                    toastr["success"](data.message);
    //                    $scope.GetMembers();
    //                }
    //                else {
    //                    toastr["error"](data.message);
    //                }
    //            }).error(function (error) {
    //                toastr["error"]("Some error has occured.")
    //            });
    //}
    $scope.generateQRBarcode = function (memberId) {
        
        memberService.GenerateQRBarcode(memberId)
                .success(function (data) {
                    
                    if (data.result) {
                        toastr["success"](data.message);
                       $scope.GetMembers();
                    }
                    else {
                        toastr["error"](data.message);
                    }
                }).error(function (error) {
                    toastr["error"]("Some error has occured.")
                });
    }
    $scope.modalMemberView = function (memberid) {
       
        //window.location.href = '#viewMember';
       // redirectionUrl
        //window.location.href = 'http://localhost:3094/Member/GetMember?id=' + memberid;
        window.location.href = redirectionUrl + 'Member/GetMember?id=' + memberid;
        //memberService.GetMember(member.MemberId)
        //       .success(function (data) {
        //          
        //           $scope.viewMember = data;
        //       }).error(function (error) {
        //           toastr["error"]("Some error has occured.")
        //       });
        //$("#modal-MemberView").modal("show");
    }
   
    $scope.saveMember = function (form) {
        
        if ($('#formMember').valid()) {
            var r = $scope.member.DOB.split('/');
            var d = r[1] + '/' + r[0] + '/' + r[2];
            $scope.member.DOB = d;
            memberService.saveMember($scope.member).then(function (response) {
                if (response.data.result) {
                    $scope.GetMembers();
                    $("#modal-Member").modal("hide");
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


    //$scope.deleteMember = function (memberId) {
       
    //    bootbox.confirm("Are you sure to delete this Member?", function (ans) {
    //        if (ans) {
               
    //            memberService.deleteMember(memberId).then(function (response) {
    //                $scope.GetMembers();
    //                toastr["success"]("Data saved successfully.")
    //                $("#modal-Member").modal("hide");
    //            }, function (error) {
    //                toastr["error"]("Some error has occured.");
    //            });
    //        }
    //        else {

    //        }
    //    });



    //}

});


MetronicApp.factory('memberService', function ($http) {
   
    var memberService = {};

    memberService.GetMembers = function () {
        return $http.get('/api/Member/GetMembers');
    }

    memberService.GetMember = function (memberId) {
        return $http({
            url: redirectionUrl + 'api/Member/GetMember',
            method: "GET",
            params: { id: memberId }
        });
    }
    memberService.saveMember = function (model) {
        return $http({
            method: "POST",
            url: redirectionUrl + "api/Member/SaveMember",
            data: JSON.stringify(model),
        });
    };
    //memberService.deleteMember = function (memberId) {
    //    return $http({
    //        method: "DELETE",
    //        url: "/api/Member/" + memberId,
    //        data: { id: memberId },
    //    });
    //};
    //memberService.GenerateBarcode = function (memberId) {
    //    return $http({
    //        method: "GET",
    //        url: redirectionUrl + "api/Member/GenerateBarcode",
    //        params: { id: memberId }
    //    });
    //};
    memberService.GenerateQRBarcode = function (memberId) {
        return $http({
            method: "GET",
            url: redirectionUrl + "api/Member/GenerateQRBarcode",
            params: { id: memberId }
        });
    };
    return memberService;
});
       