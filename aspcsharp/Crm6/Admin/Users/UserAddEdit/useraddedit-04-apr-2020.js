var userId = parseInt($("#lblUserId").text());
var subscriberId = parseInt($("#lblSubscriberId").text());
var editingUserId = parseInt($("#lblEditingUserId").text());
var $ddlCurrency = $("#ddlCurrency");
var $ddlDisplayLanguage = $("#ddlDisplayLanguage");
var $ddlTimezone = $("#ddlTimezone");
var $ddlLocation = $("#ddlLocation");
var $ddlDistrict = $("#ddlDistrict");
var $ddlCountry = $("#ddlCountry"); 
var $divLoginDetails = $("#divLoginDetails");
var $ddlReportDateFormat = $("#ddlReportDateFormat");
var $divUserEdit = $("#divUserEdit");

$(function () {
    // Mobile Menu Hide/Show on Trigger
    $('a.mobile-trigger').click(function (event) {
        event.stopPropagation();

        if (!$(this).hasClass('active')) {
            $('.mobile-header .mob-menu').stop(true, true).slideDown(300);
            $(this).addClass('active');
        } else {
            $('.mobile-header .mob-menu').stop(true, true).slideUp(300);
            $(this).removeClass('active');
        }
    });

    // setup page
    new PageSetup().Init();
    // file upload
    new FileUpload().Init();
    // init user
    new User().Init();
    // setup manager
    new SetupManagers().Init();

    if (userId) {
        $('title').text('Edit User');
    } else {
        $('title').text('Add User');
    }

    var $salesManagerCheckbox = $(".user-roles").find("input[type=checkbox]").closest(".i-checks[data-id='Sales Manager']");
    if ($salesManagerCheckbox.find("input").is(":checked")) {
        $("#divManagerSalesReps").removeClass("hide");
    }
    $salesManagerCheckbox.on('ifChecked', function () {
        $("#divManagerSalesReps").removeClass("hide");
    });
    $salesManagerCheckbox.on('ifUnchecked', function () {
        $("#divManagerSalesReps").addClass("hide");
    });

    $('#multiselect').multiselect();
});


var PageSetup = function () {
    var self = this;

    this.Init = function () {
        self.SetupSelect2Dropdowns();
        self.HandleLoginControls();

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
                    $ddlCountry.val(location.CountryCode).trigger('change'); 
                    $ddlTimezone.val(location.TimeZone).trigger('change');
                    $("#txtAddress").val(location.Address);
                    $("#txtCity").val(location.City);
                    $("#txtStateProvince").val(location.StateProvince);
                    $("#txtPostcode").val(location.PostalCode);
                },
                beforeSend: function () {
                },
                error: function (request, status, error) {
                }
            });
        });
    };

    this.SetupSelect2Dropdowns = function () {
        $(".portal-select2").select2({});
    };

    this.HandleLoginControls = function () {
        if ($("#chkLoginEnabled").is(":checked")) {
            $divLoginDetails.removeClass("hide");
        } else {
            $("#txtPassword").val("");
        }
        // triggers
        $("#chkLoginEnabled").on('ifChecked',
            function () {
                $divLoginDetails.removeClass("hide");
            });
        $("#chkLoginEnabled").on('ifUnchecked',
            function () {
                $divLoginDetails.addClass("hide");
                $("#txtPassword").val("");
            });
    };
};


var User = function () {
    var self = this;

    this.Init = function () {
        // init validator
        self.InitSaveValidator();
        $("#btnSave").unbind("click").click(function () {
            $divUserEdit.submit();
        });
        $("#btnCancel").unbind("click").click(function () {
            history.back();
        });
        $("#txtEmailAddress").unbind("blur").blur(function () {
            $("#txtEmailAddress").val($.trim($("#txtEmailAddress").val()));
        });
    };

    this.InitSaveValidator = function () {
        // select2 drop down validator
        $ddlCurrency.on('select2:select', function (evt) { $(this).valid(); });
        $ddlDisplayLanguage.on('select2:select', function (evt) { $(this).valid(); });
        $ddlTimezone.on('select2:select', function (evt) { $(this).valid(); });
        $ddlLocation.on('select2:select', function (evt) { $(this).valid(); });
        $ddlCountry.on('select2:select', function (evt) { $(this).valid(); });
        $ddlDistrict.on('select2:select', function (evt) { $(this).valid(); }); 

        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return arg !== value && value !== null && value !== "" && value !== "0";
        }, "");

        // validate
        $divUserEdit.validate({
            rules:
            {
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
                ddlReportDateFormat: { valueNotEquals: "null" }
            },
            messages: {
                ddlLocation: translatePhrase("Select the location"),
                ddlCurrency: translatePhrase("Select the currency"),
                ddlDisplayLanguage: translatePhrase("Select the display language"),
                ddlTimezone: translatePhrase("Select the timezone"),
                ddlCountry: translatePhrase("Select the country"),
                ddlReportDateFormat: translatePhrase("Select the report date format"),
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function (form) {
                if (editingUserId > 0) {
                    self.SaveUser();
                }
                else {
                    self.CheckForExistingUserAndSaveUser();
                }
            }
        });
    };



    this.CheckForExistingUserAndSaveUser = function () {
        $.ajax({
            type: "GET",
            url: "/api/user/IsUserFoundWithSameEmail?emailAddress=" + $.trim($("#txtEmailAddress").val()) + "&subscriberId=" + subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            timeout: 50000,
            data: '',
            success: function (response) {
                if (response) {
                    removeSpinner();
                    swal({
                        title: translatePhrase("There is a user with the same email address."),
                        type: "warning",
                        showCancelButton: false
                    });
                } else {
                    self.SaveUser();
                }
            },
            beforeSend: function () {
                addSpinner();
            },
            error: function (request) {
            }
        });
    };


    this.SaveUser = function () {
        var user = new Object();
        user.UserId = editingUserId;
        user.SubscriberId = subscriberId;
        user.FirstName = $("#txtFirstName").val();
        user.LastName = $("#txtLastName").val();
        user.EmailAddress = $.trim($("#txtEmailAddress").val());
        user.Title = $("#txtJobTitle").val();
        user.MobilePhone = $("#txtMobile").val();
        user.Phone = $("#txtPhone").val();
        user.Fax = $("#txtFax").val();
        user.BillingCode = $("#txtBillingCode").val();
        user.LocationId = $ddlLocation.val();
        user.LocationName = $ddlLocation.find('option:selected').text();
        user.DistrictCode = $("#ddlDistrict").val();
        user.DistrictName = $("#ddlDistrict").find('option:selected').text(); 
        user.Address = $("#txtAddress").val();
        user.City = $("#txtCity").val();
        user.StateProvince = $("#txtStateProvince").val();
        user.PostalCode = $("#txtPostcode").val();
        user.CountryName = $("#ddlCountry").val();
        user.CurrencyCode = $ddlCurrency.val();
        user.LanguageCode = $ddlDisplayLanguage.val();
        user.DisplayLanguage = $ddlDisplayLanguage.find('option:selected').text();
        user.LanguagesSpoken = $("#txtSpokenLanguage").val();
        user.UserRoles = $("#ddlUserRoles").val();
        user.DateFormat = 'dd/MM/yyyy';
        user.ReportDateFormat = $ddlReportDateFormat.val();

        var timezoneValue = $ddlTimezone.val();

        if (timezoneValue !== null && timezoneValue !== "0") {
            // timezone
            var timeZone = null;
            $.ajax({
                type: "GET",
                url: "/api/Timezone/GetTimezone?id=" + timezoneValue,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                timeout: 50000,
                data: JSON.stringify(request),
                success: function (response) {
                    timeZone = response;
                },
                beforeSend: function () {
                },
                error: function (request, status, error) {
                }
            });

            user.TimeZone = timeZone.TimeZoneName;
            user.TimeZoneOffset = timeZone.UtcOffset;
            user.TimeZoneCityNames = timeZone.CityNames;
            user.UpdateUserId = userId;

            //password
            user.LoginEnabled = $("#chkLoginEnabled").is(":checked");
            user.Password = $("#txtPassword").val();

            // AJAX to save the user
            var request = new Object();
            request.User = user;
            request.ProfilePic = null;
            if (editingUserId < 1) {
                var $imgProfile = $("#imgProfile");
                var blobReference = $imgProfile.attr("blob-reference");
                var containerReference = $imgProfile.attr("container-reference");
                if (blobReference && blobReference !== '' && containerReference && containerReference !== '') {
                    // new file upload
                    var profilePic = new Object();
                    profilePic.DocumentBlobReference = blobReference;
                    profilePic.DocumentContainerReference = containerReference;
                    profilePic.FileName = $imgProfile.attr("data-name");
                    profilePic.UploadUrl = $imgProfile.attr("src");
                    profilePic.DocumentTypeId = 1;
                    profilePic.SubscriberId = subscriberId;
                    profilePic.UploadedBy = userId;
                    request.ProfilePic = profilePic;
                }
            }
            request.ManagerUserIds = self.GetManagerUserIds();

            $.ajax({
                type: "POST",
                url: "/api/user/SaveUser",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(request),
                success: function (response) { 
                    removeSpinner();
                    if (!response.IsError) {
                       

                        // if same logged in user, reset local storage
                        if (parseInt(response.UserId) === userId) {
                            localStorage.setItem('language_data', '');
                            localStorage.setItem('language_code', '');
                            // reload languages
                            try {
                                performLanguageTranslation(response);
                            }
                            catch (e) {
                              /*ignore*/
                            }
                        }
                        location.href = "/Admin/Users/UserDetail/UserDetail.aspx?userId=" + response.UserId;
                    } else {
                        swal({
                            title: response.Error,
                            type: "warning",
                            showCancelButton: false
                        });
                    }  
                },
                beforeSend: function () {
                    addSpinner();
                },
                error: function () {
                }
            });
        }
    };



    this.GetManagerUserIds = function () {
        var ids = [];
        $("#multiselect_to option").each(function () {
            ids.push($(this).val());
        });
        return ids;
    };
};


var SetupManagers = function () {
    var self = this;

    this.Init = function () {

        $.ajax({
            type: "GET",
            url: '/api/dropdown/GetSalesReps?subscriberid=' + $("#lblSubscriberId").text(),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            timeout: 50000,
            data: '',
            success: function (response) {
                // manager ids
                var selectedIds = $("#lblSelectedManagerSalesReps").text().split(",");
                var selectedOptions = [];
                $.each(response, function (i, obj) {
                    if ($.inArray(obj.SelectValue, selectedIds) !== -1) {
                        selectedOptions.push(obj);
                    } else {
                        $("#multiselect").append(new Option(obj.SelectText, obj.SelectValue));
                    }
                });
                // selected managers
                $.each(selectedOptions, function (i, obj) {
                    $("#multiselect_to").append(new Option(obj.SelectText, obj.SelectValue));
                });
            },
            beforeSend: function () { },
            error: function () { }
        });
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
                // AJAX to upload
                $.ajax({
                    type: "POST",
                    url: baseUploadUrl + "ref=" + $("#lblGuid").text() + "&folder=userProfilePic&delete=true",
                    data: fileData,
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        removeSpinner();
                        // parse JSON result
                        var response = JSON.parse(data);
                        var image = response[0];
                        self.BindImage(image);
                    }, error: function (err) {
                        alert(err.statusText);
                    }, beforeSend: function () {
                        // add loading message
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
        if (editingUserId > 0)
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
        profilePic.UserId = editingUserId;
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


$("#btnDeleteUser").unbind("click").click(function () {
    swal({
        title: translatePhrase("Delete User!"),
        text: translatePhrase("Are you sure you want to delete this user?"),
        type: "error",
        showCancelButton: true,
        confirmButtonColor: "#f27474",
        confirmButtonText: translatePhrase("Yes, Delete!")
    }).then(function (result) {
        if (result.value) {
            $.ajax({
                type: "GET",
                url: "/api/user/DeleteUser/?userId=" + editingUserId + "&subscriberid=" + subscriberId + "&loggedInUserId=" + userId,
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data) {
                        location.href = "/Admin/Users/UserList/UserList.aspx";
                    }
                }, beforeSend: function () {
                    addSpinner();
                }
            });
        }
    });
});
