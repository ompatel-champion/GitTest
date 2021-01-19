var userId = parseInt($("#lblUserId").text());
var subscriberId = parseInt($("#lblSubscriberId").text());
var $ddlCountry = $("#ddlCountry");
var $ddlCurrency = $("#ddlCurrency");
var $ddlDateFormat = $("#ddlDateFormat");
var $ddlDisplayLanguage = $("#ddlDisplayLanguage");
var $ddlDistrict = $("#ddlDistrict");
var $ddlLocation = $("#ddlLocation");
var $ddlRegions = $("#ddlRegions");
var $ddlTimezone = $("#ddlTimezone");
var errorsCurrentPage = 1;
var syncHistoryCurrentPage = 1;

$(function () {
    // setup page
    new PageSetup().Init();
    // file upload
    new FileUpload().Init();
    // init user
    new User().Init();
    // change password
    new ChangePassword().Init();
    // init sync
    new Sync().Init();
    // sync logs
    new SyncErrorLog().LoadSyncErrors();
    new SyncHistory().LoadSyncHistory();
    // if sync state is passed, go to the sync tab
    if (getQueryString("syncState")) {
        $(".tab-sync").find("a").click();
    }
    $('#divUserProfile').removeClass('hidden');
});


var PageSetup = function () {
    var self = this;

    this.Init = function () {
        self.SetupSelect2Dropdowns();

        $ddlLocation.change(function () {
            var locationId = $(this).val();
            $.ajax({
                type: "GET",
                url: "/api/Location/GetLocation?locationId=" + locationId + "&subscriberid=" + subscriberId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: '',
                success: function (location) {
                    swal.close();
                    $ddlDistrict.val(location.DistrictCode).trigger('change');
                    $ddlCountry.val(location.CountryName).trigger('change');
                    $ddlRegions.val(location.RegionName).trigger('change');
                    $("#txtAddress").val(location.Address);
                    $("#txtCity").val(location.City);
                    $("#txtStateProvince").val(location.StateProvince);
                    $("#txtPostcode").val(location.PostalCode);
                }, beforeSend: function () {
                }, error: function (request, status, error) {
                }
            });
        });
    };

    this.SetupSelect2Dropdowns = function () {
        $(".portal-select2").select2({ theme: "classic" });
    };
};


var FileUpload = function () {
    var self = this;
    var baseUploadUrl = '/_handlers/temp-file-upload-handler.ashx?';
    // buttons
    var $bProfile = $("#btnUploadProfileImage");
    // inputs
    var $iptProfile = $("#fuProfileImage");
    // image
    var $imgProfile = $("#imgProfile");

    this.Init = function () {

        $iptProfile.on('change', function () {
            readURL(this);
        });

        $bProfile.on('click', function () {
            $iptProfile.click();
        });

        var readURL = function (input) {
            if (input.files && input.files[0]) {
                // setup data
                var fileData = new window.FormData();
                fileData.append('file', input.files[0]);
                // AJAX to upload user profile picture
                $.ajax({
                    type: "POST",
                    url: baseUploadUrl + "ref=" + $("#lblGuid").text() + "&folder=userProfilePic&delete=true",
                    data: fileData,
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        removeSpinner();
                        var response = JSON.parse(data);
                        var image = response[0];
                        self.BindImage(image);
                    }, error: function (err) {
                        alert(err.statusText);
                    }, beforeSend: function () {
                        addSpinner();
                    }
                });
            }
        };
    };

    this.BindImage = function (image) {
        // assign image url
        $imgProfile.attr("src", image.Uri);
        $imgProfile.attr("data-name", image.FileName);
        $imgProfile.attr("blob-reference", image.BlobReference);
        $imgProfile.attr("container-reference", image.ContainerReference);
        self.SaveImage(image);
    };

    this.SaveImage = function (image) {
        var profilePic = new Object();
        // new file upload
        profilePic.DocumentBlobReference = image.BlobReference;
        profilePic.DocumentContainerReference = image.ContainerReference;
        profilePic.FileName = image.FileName;
        profilePic.UploadUrl = image.Uri;
        profilePic.DocumentTypeId = 1;
        profilePic.UserId = userId;
        profilePic.SubscriberId = subscriberId;
        profilePic.UploadedBy = userId;
        var documents = [];
        documents.push(profilePic);

        $.ajax({
            type: "POST",
            url: "/api/document/SaveDocuments",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(documents),
            success: function () {
            }, beforeSend: function () {
            }, error: function (request, status, error) {
            }
        });
    };

};


var User = function () {
    var self = this;

    this.Init = function () {
        // init validator
        self.InitSaveValidator();
        $("#btnSave").unbind("click").click(function () {
            $("#form").submit();
        });
        $("#txtEmailAddress").unbind("blur").blur(function () {
            $("#txtEmailAddress").val($.trim($("#txtEmailAddress").val()));
        });
    };

    this.InitSaveValidator = function () {
        // select2 dropdown validator
        $ddlCountry.on('select2:select', function (evt) { $(this).valid(); });
        $ddlCurrency.on('select2:select', function (evt) { $(this).valid(); });
        $ddlDateFormat.on('select2:select', function (evt) { $(this).valid(); });
        $ddlDisplayLanguage.on('select2:select', function (evt) { $(this).valid(); });
        $ddlDistrict.on('select2:select', function (evt) { $(this).valid(); });
        $ddlLocation.on('select2:select', function (evt) { $(this).valid(); });
        $ddlRegions.on('select2:select', function (evt) { $(this).valid(); });
        $ddlTimezone.on('select2:select', function (evt) { $(this).valid(); });
        // add the rule here
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return arg !== value && value !== null && value !== null && value !== "0" && value !== "";
        }, "");

        // validate
        $("#form").validate({
            rules: {
                txtFirstName: { required: true },
                txtLastName: { required: true },
                txtEmailAddress: {
                    required: true,
                    email: true
                },
                ddlLocation: { valueNotEquals: "null" },
                ddlCurrency: { valueNotEquals: "null" },
                ddlDisplayLanguage: { valueNotEquals: "null" },
                ddlTimezone: { valueNotEquals: "null" },
                ddlCountry: { valueNotEquals: "null" },
                ddlRegions: { valueNotEquals: "null" },
                ddlDateFormat: { valueNotEquals: "null" },
            }, messages: {
                txtFirstName: translatePhrase("Enter the first name"),
                txtLastName: translatePhrase("Enter the last name"),
                txtEmailAddress: {
                    required: translatePhrase("Enter the email"),
                    email: translatePhrase("Invalid email address")
                },
                ddlLocation: translatePhrase("Select the location"),
                ddlCurrency: translatePhrase("Select a currency"),
                ddlDisplayLanguage: translatePhrase("Select the display language"),
                ddlTimezone: translatePhrase("Select the timezome"),
                ddlCountry: translatePhrase("Select the country"),
                ddlRegions: translatePhrase("Select the region"),
                ddlDateFormat: translatePhrase("Select the date format"),
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveUser();
            }
        });
    };

    this.SaveUser = function () {
        var user = new Object();
        user.UserId = userId;
        user.SubscriberId = subscriberId;
        user.FirstName = $("#txtFirstName").val();
        user.LastName = $("#txtLastName").val();
        user.EmailAddress = $.trim($("#txtEmailAddress").val());
        user.Title = $("#txtJobTitle").val();
        user.MobilePhone = $("#txtMobile").val();
        user.Phone = $("#txtPhone").val();
        user.Fax = $("#txtFax").val();
        user.LocationId = $ddlLocation.val();
        user.LocationName = $ddlLocation.find('option:selected').text();
        user.DistrictCode = $("#ddlDistrict").val();
        user.DistrictName = $("#ddlDistrict").find('option:selected').text();
        user.CountryName = $("#ddlCountry").val();
        user.CurrencyCode = $ddlCurrency.val();
        user.RegionName = $ddlRegions.val();
        user.LanguageCode = $ddlDisplayLanguage.val();
        user.DisplayLanguage = $ddlDisplayLanguage.find('option:selected').text();
        user.LanguagesSpoken = $("#txtLanguagesSpoken").val();

        // timezone
        var timeZone = null;
        $.ajax({
            type: "GET",
            url: "/api/Timezone/GetTimezone?id=" + $ddlTimezone.val(),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            timeout: 50000,
            data: JSON.stringify(request),
            success: function (response) {
                timeZone = response;
            }, beforeSend: function () {
            }, error: function (request, status, error) {
            }
        });
        user.TimeZone = timeZone.TimeZoneName;
        user.TimeZoneOffset = timeZone.UtcOffset;
        user.TimeZoneCityNames = timeZone.CityNames;
        user.DateFormat = $ddlDateFormat.val();
        //user.ReportDateFormat = $ddlReportDateFormat.val();
        user.UpdateUserId = userId;

        // AJAX to save the user
        var request = new Object();
        request.User = user;
        request.ProfilePic = null;
        $.ajax({
            type: "POST",
            url: "/api/user/SaveProfile",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(request),
            success: function (response) {
                removeSpinner();
                if (parseInt(response) > 0) {
                    // reset local storage
                    localStorage.setItem('language_data', '');
                    localStorage.setItem('language_code', '');
                    // reload languages
                    try { performLanguageTranslation(response); } catch (e) { }
                    location.href = "/Admin/Users/UserProfile/UserProfile.aspx";
                } else {
                    alert("User Save Error");
                }
            }, beforeSend: function () {
                addSpinner();
            }, error: function (request, status, error) {
            }
        });
    };
};


var ChangePassword = function () {
    var self = this;
    this.Init = function () {
        // save click
        $("#btnUpdatePassword").unbind("click").click(function () {
            self.UpdatePassword();
        });
    };

    this.UpdatePassword = function () {
        $("#tab-change-password .error-text").addClass("hide");
        var hasErrors = false;
        if ($("#txtCurrentPassword").val() === '') {
            hasErrors = true;
            $("#txtCurrentPassword").closest(".form-group").find(".error-text").html("<label class='error'>Please enter the current password.</label>");
            $("#txtCurrentPassword").closest(".form-group").find(".error-text").removeClass("hide");
        }
        if ($("#txtNewPassword").val() === '') {
            hasErrors = true;
            $("#txtNewPassword").closest(".form-group").find(".error-text").html("<label class='error'>Please enter the new password.</label>");
            $("#txtNewPassword").closest(".form-group").find(".error-text").removeClass("hide");
        }
        if ($("#txtConfirmPassword").val() === '' && $("#txtNewPassword").val() !== '') {
            hasErrors = true;
            $("#txtConfirmPassword").closest(".form-group").find(".error-text").html("<label class='error'>Please retype the new password.</label>");
            $("#txtConfirmPassword").closest(".form-group").find(".error-text").removeClass("hide");
        }
        else if ($("#txtNewPassword").val() !== $("#txtConfirmPassword").val()) {
            hasErrors = true;
            $("#txtConfirmPassword").closest(".form-group").find(".error-text").html("<label class='error'>Passwords do not match.</label>");
            $("#txtConfirmPassword").closest(".form-group").find(".error-text").removeClass("hide");
        }
        if ($("#txtNewPassword").val() === $("#txtCurrentPassword").val()) {
            hasErrors = true;
            $("#txtNewPassword").closest(".form-group").find(".error-text").html("<label class='error'>You entered the old password.</label>");
            $("#txtNewPassword").closest(".form-group").find(".error-text").removeClass("hide");
        }

        if (!hasErrors) {
            // AJAX to save the user
            var request = new Object();
            request.UserId = userId;
            request.SubscriberId = subscriberId;
            request.NewPassword = $("#txtNewPassword").val();
            request.OldPassword = $("#txtCurrentPassword").val();
            $.ajax({
                type: "POST",
                url: "/api/user/UpdatePassword",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(request),
                success: function (response) {
                    removeSpinner();
                    if (response !== "success") {
                        swal.close();
                        $("#txtCurrentPassword").closest(".form-group").find(".error-text").html("<label class='error'>" + response + "</label>");
                        $("#txtCurrentPassword").closest(".form-group").find(".error-text").removeClass("hide");
                    } else {
                        swal("Password changed!", "", "success");
                        $("#txtNewPassword").val("");
                        $("#txtCurrentPassword").val("");
                        $("#txtConfirmPassword").val("");
                    }
                }, beforeSend: function () {
                    addSpinner();
                }, error: function () {
                }
            });
        }
    };
};


var Sync = function () {
    var self = this;
    // exchange sync
    var $syncExchange = $(".sync-exchange");
    var $divExchangeSettings = $("#divExchangeSettings");
    var $btnDisableExchangeSync = $("#btnDisableExchangeSync");
    var $btnEditExchangeSync = $("#btnEditExchangeSync");
    var $btnActivateExchangeSync = $("#btnActivateExchangeSync");
    // o365 sync
    var $syncOffice365 = $(".sync-office365");
    var $divO365Settings = $("#divO365Settings");
    var $btnDisableO365Sync = $("#btnDisableO365Sync");
    var $btnEditO365Sync = $("#btnEditO365Sync");
    var $btnActivateO365Sync = $("#btnActivateO365Sync");
    // google sync
    var $syncGoogle = $(".sync-google");
    var $divGoogleSettings = $("#divGoogleSettings");
    var $btnDisableGoogleSync = $("#btnDisableGoogleSync");
    var $btnActivateGoogleSync = $("#btnActivateGoogleSync");
    // sync summary
    var $divSyncSummary = $("#divSyncSummary");
    var $btnSync = $("#btnSync");
    var $lblLastAppointmentSyncDate = $(".last-appointment-sync-date");

    this.Init = function () {

        $btnActivateO365Sync.attr("disabled", false);
        $btnActivateGoogleSync.attr("disabled", false);
        $btnActivateExchangeSync.attr("disabled", false);

        $.ajax({
            type: "GET",
            url: "/api/user/GetUser?userid=" + userId + "&subscriberId=" + subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (obj) {
                $btnEditO365Sync.addClass("hide");
                $btnEditExchangeSync.addClass("hide");
                $btnDisableO365Sync.addClass("hide");
                $btnDisableGoogleSync.addClass("hide");
                $btnDisableExchangeSync.addClass("hide");
                $syncOffice365.find(".ibox-content").removeClass("active");
                $syncGoogle.find(".ibox-content").removeClass("active");
                $syncExchange.find(".ibox-content").removeClass("active");
                $divSyncSummary.addClass("hide");
                $divO365Settings.addClass("hide");
                $divGoogleSettings.addClass("hide");
                $divExchangeSettings.addClass("hide");

                // exchange
                $btnActivateExchangeSync.removeClass("hide");
                $("#btnActivateExchangeSync, #btnEditExchangeSync").unbind("click").click(function () {
                    $divO365Settings.addClass("hide");
                    $divGoogleSettings.addClass("hide");
                    $divExchangeSettings.removeClass("hide");
                });
                $("#btnDisableExchangeSync, #btnDisableO365Sync, #btnDisableGoogleSync").unbind("click").click(function () {
                    self.DisableSync();
                });

                // office 365
                $btnActivateO365Sync.removeClass("hide");
                $("#btnActivateO365Sync, #btnEditO365Sync").unbind("click").click(function () {
                    $divO365Settings.removeClass("hide");
                    $divGoogleSettings.addClass("hide");
                    $divExchangeSettings.addClass("hide");
                });
                $btnDisableO365Sync.unbind("click").click(function () {
                    self.DisableSync();
                });

                // google
                $btnActivateGoogleSync.removeClass("hide");
                $("#btnSaveExchangeSyncSettings").unbind("click").click(function () {
                    self.SaveExchangeSync();
                });
                $("#btnSaveOffice365SyncSettings").unbind("click").click(function () {
                    self.SaveOffice365Sync();
                });

                var user = obj.User;
                if (user.SyncType === "Office365") {
                    // o365 sync
                    $("#txtO365Email").val(user.SyncEmail);
                    $btnEditO365Sync.removeClass("hide");
                    $btnDisableO365Sync.removeClass("hide");                    
                    $btnActivateO365Sync.addClass("hide");
                    $syncOffice365.find(".ibox-content").addClass("active");
                    $divSyncSummary.removeClass("hide");
                    $divO365Settings.addClass("hide");
                    // disable Exchange and Google sync options
                    $btnActivateGoogleSync.attr("disabled", true);
                    $btnActivateExchangeSync.attr("disabled", true);
                    // set appointment last sync date
                    var appointmentLastSyncDate = moment(user.SyncAppointmentsLastDateTime).isValid() ? moment(user.SyncAppointmentsLastDateTime).format("DD MMMM YYYY @ HH:mm") : "Initial sync is not done";
                    $lblLastAppointmentSyncDate.html(appointmentLastSyncDate);
                } else if (user.SyncType === "Exchange") {
                    $("#txtExchangeUserName").val(user.SyncUserName);
                    $("#txtExchangeEmail").val(user.SyncEmail);
                    // exchange sync
                    $btnEditExchangeSync.removeClass("hide");
                    $btnActivateExchangeSync.addClass("hide");
                    $btnDisableExchangeSync.removeClass("hide");
                    $syncExchange.find(".ibox-content").addClass("active");
                    $divSyncSummary.removeClass("hide");
                    $divExchangeSettings.addClass("hide");
                    // disable O365 and Google sync options
                    $btnActivateO365Sync.attr("disabled", true);
                    $btnActivateGoogleSync.attr("disabled", true);
                    // set appointment last sync date
                    appointmentLastSyncDate = moment(user.SyncAppointmentsLastDateTime).isValid() ? moment(user.SyncAppointmentsLastDateTime).format("DD MMMM YYYY @ HH:mm") : "Initial sync is not done";
                    $lblLastAppointmentSyncDate.html(appointmentLastSyncDate);
                } else if (user.SyncType === "Google") {
                    // google sync
                    $btnDisableGoogleSync.removeClass("hide");
                    $btnActivateGoogleSync.addClass("hide");
                    $syncGoogle.find(".ibox-content").addClass("active");
                    $btnActivateExchangeSync.attr("disabled", true);
                    $btnActivateO365Sync.attr("disabled", true);
                    $divSyncSummary.removeClass("hide");
                    // set appointment last sync date
                    appointmentLastSyncDate = moment(user.SyncAppointmentsLastDateTime).isValid() ? moment(user.SyncAppointmentsLastDateTime).format("DD MMMM YYYY @ HH:mm") : "Initial sync is not done";
                    $lblLastAppointmentSyncDate.html(appointmentLastSyncDate);
                }

            }, beforeSend: function () {
            }, error: function (request, status, error) {
            }
        });

        $btnSync.unbind("click").click(function () {
            self.RunSync();
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
                removeSpinner();
                if (response) {
                    self.Init();
                } else {
                    swal(
                        'Error...',
                        'Authentication error activating Office 365 Sync',
                        'error'
                    );
                }
            }, beforeSend: function () {
                addSpinner();
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
        // AJAX to save
        $.ajax({
            type: "POST",
            url: "/api/user/ActivateSync",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(syncUser),
            success: function (response) {
                removeSpinner();
                if (response) {
                    self.Init();
                } else {
                    swal(
                        'Sync Error...',
                        'Error activating Office 365 Sync',
                        'error'
                    );
                }
            }, beforeSend: function () {
                addSpinner();
            }, error: function (request, status, error) {
            }
        });
    };

    this.RunSync = function () {
        $.ajax({
            type: "GET",
            url: "/api/sync/dosync/?userId=" + userId + "&subscriberId=" + subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {},
            success: function (response) {
                if (response === false) {
                    location.href = "/Admin/Users/UserSyncError/VerifyCredentials.aspx";
                }
                removeSpinner();
                self.Init();
            },
            beforeSend: function () {
                addSpinner();
            },
            error: function (request) { }
        });
    };

    this.DisableSync = function () {
        swal({
            title: "Disable Sync!",
            text: "Are you sure you want to disable the sync?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#ea7d7d",
            confirmButtonText: "Yes",
            closeOnConfirm: true
        }).then(function(result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/user/DisableSync/?userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        location.href = "/Admin/Users/UserProfile/UserProfile.aspx";
                    },
                    beforeSend: function () { },
                    error: function (request) { }
                });
            }
        });
    };

};


var SyncErrorLog = function () {
    var self = this;
    var $tblSyncErrors = $("#tblSyncErrors");
    var $tbody = $tblSyncErrors.find("tbody");

    this.LoadSyncErrors = function () {
        // set the filters
        var filters = new Object();
        filters.UserId = userId;
        filters.CurrentPage = errorsCurrentPage;
        filters.RecordsPerPage = 200;

        // clear the rows
        $tbody.html("");

        // AJAX to retrieve sync errors
        $.ajax({
            type: "POST",
            url: "/api/Sync/GetSyncErrors",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(filters),
            success: function (response) {
                // remove loading message
                removeSpinner();
                if (response.Items.length > 0) {
                    self.BindErrors(response.Items);
                }
            }, beforeSend: function () {
                //add loading message
                addSpinner();
            }, error: function () {
            }
        });
    };

    this.BindErrors = function (items) {
        $.each(items, function (i, err) {
            var $tr = $("<tr/>", { "data-id": err.ExchangeSyncErrorLogId });
            // date
            var $tdDate = $("<td/>");
            var $pError = $("<p/>", { "class": "W100 text-muted FontSize12", "html": moment(err.ErrorDateTime).format("MMMM DD, YYYY @ HH:mm") });
            $tdDate.append($pError);
            $tr.append($tdDate);
            // error
            var $tdError = $("<td/>");
            $pError = $("<p/>", { "class": " text-muted FontSize12", "html": err.ErrorMessage });
            $tdError.append($pError);
            $tdError.append($("<span class='FontSize12 text-navy'>" + err.RoutineName + "</span>"));
            $tr.append($tdError);
            $tbody.append($tr);
        });
    };

};


var SyncHistory = function () {
    var self = this;
    var $tblSyncHistory = $("#tblSyncHistory");
    var $tbody = $tblSyncHistory.find("tbody");

    this.LoadSyncHistory = function () {
        // set the filters
        var filters = new Object();
        filters.UserId = userId;
        filters.CurrentPage = syncHistoryCurrentPage;
        filters.RecordsPerPage = 200;

        // clear the rows
        $tbody.html("");

        // AJAX to retrieve sync errors
        $.ajax({
            type: "POST",
            url: "/api/Sync/GetSyncHistory",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(filters),
            success: function (response) {
                $("#tab-sync-history").find(".loading").remove();
                swal.close();
                if (response.Items.length > 0) {
                    self.BindHistory(response.Items);
                }
            }, beforeSend: function () {
            }, error: function () {
            }
        });
    };

    this.BindHistory = function (items) {
        $.each(items, function (i, his) {
            var $tr = $("<tr/>", { "data-id": his.ExchangeSyncLogId });
            // date
            var $tdDate = $("<td/>", { "class": "W150" });
            var $pError = $("<p/>", { "class": "W150 text-muted FontSize12", "html": moment(his.SyncDateTime).format("MMMM DD, YYYY @ HH:mm") });
            $tdDate.append($pError);
            $tr.append($tdDate);
            // message
            var $tdMessage = $("<td/>");
            var $pMessage = $("<p/>", { "class": "MB5 text-muted FontSize12", "html": his.SyncMessage });
            $tdMessage.append($pMessage);

            var hisTypeStr = "";
            switch (his.SyncType) {
                case "AddCalendarEvent": hisTypeStr = "CRM to Exchange - Add event"; break;
                case "UpdateCalendarEvent": hisTypeStr = "CRM to Exchange - Update event"; break;
                case "AddExchangeAppointment": hisTypeStr = "Exchange to CRM - Add event"; break;
                case "UpdateExchangeAppointment": hisTypeStr = "Exchange to CRM - Update event"; break;
                default:
            }
            $tdMessage.append($("<span class='FontSize12 text-navy'>" + hisTypeStr + "</span>"));
            $tr.append($tdMessage);
            $tbody.append($tr);
        });
    };
};
