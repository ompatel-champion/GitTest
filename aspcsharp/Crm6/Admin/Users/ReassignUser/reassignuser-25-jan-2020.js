var subscriberId = parseInt($("#lblSubscriberId").text());
var userId = parseInt($("#lblUserId").text());
var $btnReassign = $("#btnReassign");

$(function () {
    // init Select2
    $("#ddlDepartingUser").select2({ placeholder: "Departing User" });
    $("#ddlNewUser").select2({ placeholder: "New User" });
});

$(function () {
    new ReassignUser().Init();
});

var ReassignUser = function () {
    var self = this;

    this.Init = function () {
        //cancel button
        $("#btnCancel").unbind("click").click(function () {
            window.history.back();
        });
        //reassign button
        $btnReassign.unbind("click").click(function () {
            self.ReassignUser();
        });
    };

    this.ReassignUser = function () {
        if ($("#ddlDepartingUser").val() === "") {
            swal(
                'Error',
                'Select the departing user.',
                'warning'
            );
        }
        else if ($("#ddlNewUser").val() === "") {
            swal(
                'Error',
                'Select the new user.',
                'warning'
            );
        }
        else {
            var request = new Object();
            request.userId = userId;
            request.DepartingUserId = $("#ddlDepartingUser").val();
            request.NewUserId = $("#ddlNewUser").val();
            request.SubscriberId = subscriberId;

            $.ajax({
                type: "POST",
                url: "/api/User/ReassignUser",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(request),
                success: function (response) {
                    if (response) {
                        swal(
                            'Success',
                            'User has been reassigned successfully!',
                            'success'
                        );
                    }
                },
                beforeSend: function () {
                    swal({
                        text: "",
                        title: "<img src='/_content/_img/loading_40.gif'/>",
                        showConfirmButton: false,
                        allowOutsideClick: false,
                        html: false
                    });
                },
                error: function (request, status, error) {
                }
            });
        }
    };
};
