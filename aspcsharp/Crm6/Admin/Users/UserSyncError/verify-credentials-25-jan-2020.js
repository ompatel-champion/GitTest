var userId = parseInt($("#lblUserId").text());
var subscriberId = parseInt($("#lblSubscriberId").text());

$(function () {
    // sync
    new Sync().Init();
});

var Sync = function () {
    var self = this;
    var $syncExchange = $(".sync-exchange");
    var $divExchangeSettings = $("#divExchangeSettings");

    var $syncOffice365 = $(".sync-office365");
    var $divO365Settings = $("#divO365Settings");

    var $syncGoogle = $(".sync-google");
    var $divGoogleSettings = $("#divGoogleSettings");

    var $success = $(".success-message");
    var $error = $(".error-message");

    var $disableSync = $("[data-action='disable-sync']");

    this.Init = function () {
        $.ajax({
            type: "GET",
            url: "/api/user/GetUser?userid=" + userId + "&subscriberId=" + subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (u) {

                //$syncOffice365.addClass("hide");
                $syncGoogle.addClass("hide");
                $syncExchange.addClass("hide");

                //$divO365Settings.addClass("hide");
                $divGoogleSettings.addClass("hide");
                $divExchangeSettings.addClass("hide");

                //$("#btnSaveExchangeSyncSettings").unbind("click").click(function () {
                //    self.SaveExchangeSync();
                //});

                $("#btnSaveOffice365SyncSettings").unbind("click").click(function () {
                    self.SaveOffice365Sync();
                });

                var user = u.User;
                if (user.SyncType === "Office365") {
                    $("#txtO365Email").val(user.SyncEmail);
                    $divO365Settings.removeClass("hide");
                    $syncOffice365.removeClass("hide");
                    $error.html("Problem with your Office 365 Login - Enter your login email and password.");

                } else if (user.SyncType === "Exchange") {
                    $("#txtExchangeUserName").val(user.SyncUserName);
                    $("#txtExchangeEmail").val(user.SyncEmail);
                    $error.html("Problem with your Exchange Login - Enter your login details.");
                    $syncExchange.removeClass("hide");
                    $divExchangeSettings.removeClass("hide");

                } else if (user.SyncType === "Google") {
                    $error.html("Problem with your Google Login - Click on Verify to validate access.");
                    $syncGoogle.removeClass("hide");
                    $divGoogleSettings.removeClass("hide");
                }

            }, beforeSend: function () {
            }, error: function (request, status, error) {
            }
        });

        // disable sync
        $disableSync.unbind("click").click(function () {
            self.DisableSync();
        });

    };

    this.SaveExchangeSync = function () {
        var syncUser = new Object();
        syncUser.UserId = userId;
        syncUser.SyncEmail = $("#txtExchangeEmail").val();
        syncUser.SyncPassword = $("#txtExchangePassword").val();
        syncUser.SyncUsername = $("#txtExchangeUserName").val();
        syncUser.SyncType = "Exchange";

        // AJAX to save
        $.ajax({
            type: "POST",
            url: "/api/user/ActivateSync",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(syncUser),
            success: function (response) {
                if (response) {
                    swal.close();
                    $error.addClass("hide");
                    $success.removeClass("hide");
                    //  self.Init();
                } else {
                    swal(
                        'Sync Error...',
                        'Error activating Exchange Sync. Please check the login details and try again.',
                        'error'
                    );
                }
            }, beforeSend: function () {
                swal({ text: translatePhrase("Activating Exchnage Sync") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: true });
            }, error: function (request, status, error) {
            }
        });

    };

    this.SaveOffice365Sync = function () {
        var syncUser = new Object();
        syncUser.UserId = userId;
        syncUser.SyncEmail = $("#txtO365Email").val();
        syncUser.SyncPassword = $("#txtO365Password").val();
        syncUser.SyncType = "Office365";

        // AJAX to activate O365 sync
        $.ajax({
            type: "POST",
            url: "/api/user/ActivateSync",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(syncUser),
            success: function (response) {
                if (response) {
                    swal.close();
                    $error.addClass("hide");
                    $success.removeClass("hide");
                    //  self.Init();
                } else {
                    swal(
                        'Error...',
                        'Problem activating Office 365 Sync - Invalid Login',
                        'error'
                    );
                }
            }, beforeSend: function () {
                swal({
                    text: translatePhrase("Activating Office 365 Sync") + "...",
                    title: "<img src='/_content/_img/loading_40.gif'/>",
                    showConfirmButton: false,
                    allowOutsideClick: false,
                    html: false
                });
            }, error: function (request, status, error) {
            }
        });

    };

    this.SaveGoogleSync = function () {
        // TODO: code function
    };

    this.DisableSync = function () {
        swal({
            title: "Disable Sync!",
            text: "Are you sure you want to disable?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#ea7d7d",
            confirmButtonText: "Yes",
            closeOnConfirm: true
        }, function () {
            $.ajax({
                type: "GET",
                url: "/api/user/DisableSync/?userId=" + userId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (response) {
                    if (response) {
                        $error.addClass("hide");
                        $success.html("Calendar sync has been disabled.");
                        $success.removeClass("hide");
                    }
                },
                beforeSend: function () { },
                error: function (request) { },
            });
        });
    };
};
