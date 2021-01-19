// init global variables
var subscriberId = parseInt($("#lblSubscriberId").text());
var contactId = parseInt($("#lblContactId").text());
var userId = parseInt($("#lblUserId").text());
var $ddlCompany = $("#ddlCompany");
var $ddlContactType = $("#ddlContactType");
var $txtFirstName = $("#txtFirstName");
var $txtLastName = $("#txtLastName");
var $txtJobTitle = $("#txtJobTitle");
var $txtEmail = $("#txtEmail"); 
var $ddlPreviousEmployers = $("#ddlPreviousEmployers");
var $divContactSetup = $("#divContactSetup");
var contactSubscriberId = parseInt($("#lblContactSubscriberId").text());
var globalCompanyId = 0;
var $contactProfileImage = $('#img_uploaded_profile_image');

$(function () {
    // set-up page
    new PageSetup().Init();
    // initialize contacts
    new Contact().Init();
    // initialize file upload
    new FileUpload().Init();

    if (contactId ) {
        $('title').text('Edit Contact');
    } else {
        $('title').text('Add Contact');
    }
});


var PageSetup = function () {
    var self = this;

    this.Init = function () { 
        $ddlPreviousEmployers.val($('#hdnPreviousEmployers').val().split(',')).trigger('change');
        self.SetupSelect2Dropdowns();
        self.InitPageActions();
        if (contactId==0) $('#txtFirstName').focus();
    };

    this.SetupSelect2Dropdowns = function () {
        $("#ddlContactType").select2({ placeholder: "", allowClear: false });
        $("#ddlBirthdayMonth").select2({ placeholder: "", allowClear: true, search:false });
        $("#ddlCountry").select2({ placeholder: "", allowClear: false });
        $("#ddlOwner").select2({ placeholder: "", allowClear: false });
        // company
        $ddlCompany.select2({
            minimumInputLength: 2,
            placeholder: "Company",
            ajax: {
                url: function (obj) {
                    return "/api/dropdown/GetUserCompanies?"+
                        "subscriberId="+contactSubscriberId +
                        "&userId="+userId+
                        "&keyword=" + obj.term;
                },
                dataType: "json",
                timeout: 50000,
                type: "GET",
                data: '',
                processResults: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.SelectText,
                                id: item.SelectValue
                            };
                        })
                    };
                }
            }
        });

        $ddlPreviousEmployers.select2({
            tags: true,
            "theme": "classic",
            createTag: function (params) {
                return {
                    id: params.term,
                    text: params.term,
                    newOption: true
                };
            },
            templateResult: function (data) {
                var $result = $("<span></span>");

                $result.text(data.text);

                if (data.newOption) {
                    $result.append(" <em>(new)</em>");
                }

                return $result;
            }, ajax: {
                url: function (obj) {
                    if (!obj.term) {
                        obj.term = "";
                    }
                    return "/api/dropdown/GetCompanies?subscriberId=" + contactSubscriberId + "&keyword=" + obj.term;
                },
                dataType: "json",
                timeout: 50000,
                type: "GET",
                data: '',
                processResults: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.SelectText,
                                id: item.SelectText
                            };
                        })
                    };
                }
            }
        });

        if ($ddlCompany.val() !== null && contactId === 0) {
            var companyId = parseInt($ddlCompany.val());
            new Company().GetCompany(companyId);
        }
        $ddlCompany.on('select2:select', function (evt) {
            if (contactId < 1) {
                var companyId = $ddlCompany.val();
                new Company().GetCompany(companyId);
            }
        });
    };

    this.InitPageActions = function () {
        // save button click
        $("#btnSave").unbind("click").click(function () {
            $divContactSetup.submit();
        });
        // cancel button click
        $("#btnCancel").unbind("click").click(function (event) {
            event.preventDefault();
            var fromUrl = document.referrer;
            
            if (fromUrl && fromUrl !== '') {
                if (fromUrl.indexOf("#contacts") < 0) {
                    fromUrl += "#contacts";
                }
                location.href = fromUrl;
            } else
                history.back(1);
        });
        // delete button click
        $("#btnDelete").unbind("click").click(function () {
            new Contact().DeleteContact();
        });
        if (contactId > 0) {
            $("#btnDelete").removeClass("hide");
        }
    };
};


var Contact = function () {
    var self = this;

    this.Init = function () {
        // bind profile picture for updating user
        var profilePicPath = $("#txtProfileImageLink").val();
        if (profilePicPath !== '') {
            new FileUpload().BindImage(profilePicPath);
        }
        // init validator
        self.InitSaveValidator();
        $("#txtBirthdayDay").blur(function(){
            var value = parseInt(this.value);
            if (!isNaN(value) && value>0 && value<32) this.value = FFGlobal.utils.date.getDayOrdinal(value);
        });
    };

    this.InitSaveValidator = function () {
        // select2 dropdown validator
        $ddlCompany.on('select2:select', function (evt) { $(this).valid(); });
        $ddlContactType.on('select2:select', function (evt) { $(this).valid(); });

        // custom validation methods
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return (arg !== value && value !== null) && value !== null && value !== "0";
        }, "");
         $.validator.addMethod("isValidBirthdayDay", function (value, element, arg) {
            var day = parseInt(value);
            return value=="" || (!isNaN(day) && day<32 && day>0);
        }, "");

        // validate
        $divContactSetup.validate({
            rules: {
                txtFirstName: { required: true },
                txtLastName: { required: true },
                txtJobTitle: { required: true },
                txtEmail: {
                    required: true,
                    email: true
                },
                ddlCompany: { valueNotEquals: "null" },
                ddlContactType: { valueNotEquals: "null" },
                txtBirthdayDay: { isValidBirthdayDay: {} },
            }, messages: {
                txtFirstName: translatePhrase("Enter the first name"),
                txtLastName: translatePhrase("Enter the last name"),
                txtJobTitle: translatePhrase("Enter the job title"),
                txtEmail: {
                    required: translatePhrase("Enter the email address"),
                    email: translatePhrase("Invalid email address")
                },
                ddlCompany: translatePhrase("Select a Company"),
                ddlContactType: translatePhrase("Select a Contact Type"),
                txtBirthdayDay: translatePhrase("Enter a valid day.")
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveContact();
            }
        });
    };

    this.SaveContact = function () {
        var contact = new Object();
        contact.ContactId = contactId;
        contact.SubscriberId = contactSubscriberId;
        contact.CompanyId = $ddlCompany.val();
        contact.FirstName = $("#txtFirstName").val();
        contact.Title = $("#txtJobTitle").val();
        contact.LastName = $("#txtLastName").val();
        contact.ContactName = contact.FirstName + ' ' + contact.LastName;
        contact.Email = $("#txtEmail").val();
        contact.UpdateUserId = userId;
        contact.MobilePhone = $("#txtMobile").val();
        contact.BusinessPhone = $("#txtBusinessPhone").val();
        contact.ContactType = $("#ddlContactType option:selected").text();
        contact.BusinessCity = $("#txtCity").val();
        contact.Comments = $("#Notes").val();
        contact.BusinessStateProvince = $("#txtStateProvince").val();
        contact.PreviousEmployees = $ddlPreviousEmployers.val().join(",");
        contact.BusinessAddress = $("#txtAddress").val();
        contact.BusinessPostalCode = $("#txtPostalCode").val();
        contact.BusinessCountry = $("#ddlCountry").val();
        contact.Hobbies = $('#txtHobbies').val();
        contact.ReceiveEmail = $('#OktoEmail').is(':checked') ? 1 : 0;
        contact.Married = $('#Married').is(':checked') ? 1 : 0;
        contact.HasChildern = $('#HasChildren').is(':checked') ? 1 : 0;
        contact.OkToCall = $('#OktoCall').is(':checked') ? 1 : 0;
        contact.HolidayCards = $('#HolidayCard').is(':checked') ? 1 : 0;
        contact.FormerEmployee = $('#FormerEmployee').is(':checked') ? 1 : 0;
        contact.ContactOwnerUserId = userId;
        contact.BirthdayMonth = $("#ddlBirthdayMonth").val();
        contact.BirthdayDay = parseInt($("#txtBirthdayDay").val())||"";

        // profile picture
        var profilePic = null;
        if ($contactProfileImage.attr("blob-reference") && $contactProfileImage.attr("blob-reference") !== '') {
            var blobReference = $contactProfileImage.attr("blob-reference");
            var containerReference = $contactProfileImage.attr("container-reference");
            if (blobReference !== '' && containerReference !== '') {
                profilePic = new Object();
                // new file upload
                profilePic.DocumentBlobReference = blobReference;
                profilePic.DocumentContainerReference = containerReference;
                profilePic.FileName = $contactProfileImage.attr("data-name");
                profilePic.UploadUrl = $contactProfileImage.attr("src");
            }
        }

        var request = new Object();
        request.Contact = contact;
        request.ProfilePic = profilePic;

        // api to save contact
        $.ajax({
            type: "POST",
            url: "/api/contact/SaveContact",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(request),
            success: function (response) {
                $("#btnSave").attr("disabled", false);
                removeSpinner();
                if (parseInt(response) > 0) {
                    var fromUrl = document.referrer; //getQueryString("from");
                    if (fromUrl && fromUrl !== '') {
                        if (fromUrl.indexOf("#contacts") < 0) {
                            fromUrl += "#contacts";
                        }
                        location.href = fromUrl;
                    } else
                        history.back(1);
                }
            }, beforeSend: function () {
                addSpinner();
                $("#btnSave").attr("disabled", true);
            }, error: function (request) {
                alert(JSON.stringify(request));
                $("#btnSave").attr("disabled", false);
            }
        });
    };

    this.DeleteContact = function () {
        swal({
            title: translatePhrase("Delete Contact!"),
            text: translatePhrase("Are you sure you want to delete this contact?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/contact/DeleteContact/?contactId=" + contactId + "&userId=" + userId + "&subscriberid=" + contactSubscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data) {
                            if (/ContactDetail\.aspx/.test(document.referrer)) location.href = "/Contacts/ContactList/ContactList.aspx";
                            else history.back(1);
                        }
                    }, beforeSend: function () {
                        addSpinner();
                    }
                });
            }
        });
    };
};


var Company = function () {
    this.GetCompany = function (companyId) {
        $.ajax({
            type: "GET",
            url: "/api/Company/GetCompany?companyId=" + companyId + "&subscriberId=" + contactSubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (company) {
                $("#txtCity").val(company.City);
                $("#txtAddress").val(company.Address);
                $("#txtPostalCode").val(company.PostalCode);
                $("#ddlCountry").val(company.CountryName);
                $("#ddlCountry").trigger('change');
                globalCompanyId = company.CompanyIdGlobal;
            }, beforeSend: function () {

            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

};


var FileUpload = function () {
    var baseUploadUrl = '/_handlers/temp-file-upload-handler.ashx?';

    this.Init = function () {
        $(".file-upload").on('change', function () {
            readURL(this);
        });
                
        $("#img_uploaded_profile_image").on('click', function () {
            $(".file-upload").click();
        });

        var readURL = function (input) {
            if (input.files && input.files[0]) {
                // setup data
                var fileData = new window.FormData();
                fileData.append('file', input.files[0]);
                // AJAX to upload
                $.ajax({
                    type: "POST",
                    url: baseUploadUrl + "ref=" + $("#lblGuid").text() + "&folder=contactProfilePic&delete=true",
                    data: fileData,
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        // remove spinner
                        removeSpinner();
                        // parse JSON result
                        var response = JSON.parse(data);
                        var image = response[0];
                        // set the values
                        $contactProfileImage.attr('src', image.Uri);
                        $contactProfileImage.attr('src', image.Uri);
                        $contactProfileImage.attr("data-name", image.FileName);
                        $contactProfileImage.attr("blob-reference", image.BlobReference);
                        $contactProfileImage.attr("container-reference", image.ContainerReference);
                    }, error: function (err) {
                        alert(err.statusText);
                    }, beforeSend: function () {
                        // add loading spinner
                        addSpinner();
                    }
                });
            }
        };
    };
};
