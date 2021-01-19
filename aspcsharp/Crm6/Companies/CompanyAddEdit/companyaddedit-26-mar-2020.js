var subscriberId = parseInt($("#lblSubscriberId").text());
var companyId = parseInt($("#lblCompanyId").text());
var companySubscriberid = parseInt($("#lblCompanySubscriberId").text());
var userId = parseInt($("#lblUserId").text());
var $divCompanySetup = $("#divCompanySetup");
var $ddlSource = $("#ddlSource");
var $ddlCountry = $("#ddlCountry");
var $ddlIndustry = $("#ddlIndustry");
var $ddlCompanyType = $("#ddlCompanyType");
var $ddlOwner = $("#ddlOwner");
var $ddlCampaign = $("#ddlCampaign");
var $txtAddress = $("#txtAddress");
var $txtCity = $("#txtCity");
var $companyType = $("#ddlCompanyType");
var currentStep = 1;
var progress_line = $('.f1-progress-line');
var isAdmin = $("#lblIsAdmin").text() === "1";

var $content = $(".form-box");
var $successBox = $(".success-box");
var fromPage = "";
var fromId = "";

$(function () {
    new PageSetup().Init();
    new FileUpload().Init();
    new Company().Init();

    if (companyId) {
        $('title').text('Edit Company');
    } else {
        $('title').text('Add Company');
    }
});


var PageSetup = function () {
    var self = this;

    this.Init = function () {
        fromPage = getQueryString("from");
        fromId = getQueryString("fromId");

        self.SetupSelect2Dropdowns();

        if (companyId > 0) {
            $("#btnDelete").removeClass("hide");
            $("#Notes").addClass("is-edit");
        } else {
            $("#divActiveCustomerContainer").addClass("hide");
        }
        $("#btnDelete").unbind("click").click(function () {
            new Company().DeleteCompany();
        });
        $("#btnCancel").unbind("click").click(function () {
            event.preventDefault();
            history.back(1);
        });
        //strip http and https from entered URLS
        $("#txtWebsite").blur(function () {
            $(this).val($(this).val().replace(/http\:\/\/|https\:\/\//i, ""));
        });
        if (!companyId) $("#txtCompanyName").focus();
    };

    this.SetupSelect2Dropdowns = function () {
        $ddlCampaign.select2({ theme: 'classic', placeholder: '' });
        $ddlCountry.select2({ placeholder: translatePhrase("Country") });
        $ddlSource.select2({ placeholder: translatePhrase("Source") });
        $ddlIndustry.select2({ placeholder: translatePhrase("Industry") });
        $ddlCompanyType.select2({ placeholder: translatePhrase("Company Type") });
        $ddlOwner.select2({ placeholder: translatePhrase("Owner") });
    };
};


var Company = function () {

    var self = this;

    $("#btnSave").unbind("click").click(function () { 
        $divCompanySetup.submit();
    });

    this.Init = function () {
        // initialize validator
        self.InitSaveValidator();
    };

    this.InitSaveValidator = function () {
        // select2 drop-down validator
        $ddlCountry.on('select2:select', function (evt) { $(this).valid(); });
        $ddlIndustry.on('select2:select', function (evt) { $(this).valid(); });

        // add the rule here
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return arg !== value && value !== null && value !== "" && value !== "0";
        }, "");

        // validate
        $divCompanySetup.validate({
            rules: {
                txtCompanyName: { required: true },
                ddlCountry: { valueNotEquals: "null" },
                ddlIndustry: { valueNotEquals: "null" },
                txtAddress: { required: true },
                txtCity: { required: true }
            }, messages: {
                txtCompanyName: translatePhrase("Enter the company name"),
                ddlCountry: translatePhrase("Select a country"),
                ddlIndustry: translatePhrase("Select an industry"),
                txtAddress: translatePhrase("Enter the address"),
                txtCity: translatePhrase("Enter the city")
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveCompany();
            }
        });
    };

    this.SaveCompany = function () {

        var isNewCompany = companyId < 1;

        var company = new Object();
        company.SubscriberId = companySubscriberid;
        company.CompanyId = companyId;
        company.CompanyName = $("#txtCompanyName").val();
        company.Address = $("#txtAddress").val();
        company.City = $("#txtCity").val();
        company.Phone = $("#txtPhone").val();
        company.PostalCode = $("#txtPostalCode").val();
        company.CountryName = $("#ddlCountry").val();
        //   company.User = $("#ddlOwner").val();
        company.CompanyTypes = $("#ddlCompanyType").val();
        company.Industry = $("#ddlIndustry").val();
        company.Source = $ddlSource.val();
        company.Fax = $("#txtFax").val();
        company.Website = $("#txtWebsite").val();
        company.CampaignName = $("#ddlCampaign").val().join(",");
        company.UpdateUserId = userId;
        company.Division = $("#txtDivision").val();
        company.CompanyCode = $("#txtCompanyCode").val();
        company.StateProvince = $("#txtStateProvince").val();
        company.Active = $("#chkActive").length > 0 ? $("#chkActive").is(":checked") : false;
        company.CompanyOwnerUserId = $("#ddlOwner").val();
        company.IsCustomer = $("#chkCustomer").length > 0 ? $("#chkCustomer").is(":checked") : false;
        company.CompanyTypes = $ddlCompanyType.val();
        company.Comments = $("#Notes").val();

        // set company owner id to current user Id
        if (company > 0) {
            company.CompanyOwnerId = userId;
        }

        // compamy logo
        var logo = null;
        if ($("#txtCompanyLogoLink").val() !== '') {
            var $containerLogo = $("#imgCompanyLogo");
            var blobReference = $containerLogo.attr("blob-reference");
            var containerReference = $containerLogo.attr("container-reference");
            if (blobReference && blobReference !== '' && containerReference !== '') {
                logo = new Object();
                // new file upload
                logo.DocumentBlobReference = blobReference;
                logo.DocumentContainerReference = containerReference;
                logo.FileName = $containerLogo.attr("data-name");
                logo.UploadUrl = $containerLogo.attr("src");
            }
        }

        var request = new Object();
        request.Company = company;
        request.CompanyLogo = logo;

 
        if (isNewCompany) {
            $.ajax({
                type: "POST",
                url: "/api/company/HasDuplicateCompanies",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(request),
                success: function (response) {
                    $("#btnSave").attr("disabled", false);
                    if (response.length > 0) {
                        // Duplicates found
                        var htmlToDisplay = "";

                        $.each(response, function (i, val) {
                            htmlToDisplay = htmlToDisplay + "<br/><br/>" + val.Company.CompanyName + " - " + val.Company.City + " - " + val.SalesOwnerName + " - <button type='button' class='primary-btn request-access' data-id='" + val.Company.CompanyId +"'>Request Access</button>";
                        });

                        $(document).on("click", ".request-access", function () {
                            var companyId = $(this).attr("data-id");

                            $.ajax({
                                type: "GET",
                                url: "/api/company/RequestAccess/?companyid=" + companyId + "&userId=" + userId,
                                data: {},
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    if (data) {
                                        swal({
                                            title: "Request has been sent to company owner!",
                                            type: "success",
                                            showCancelButton: false
                                        });
                                    }
                                }
                            });
                        });

                        swal({
                            title: translatePhrase("Duplicate Company!"),
                            html: translatePhrase("We found " + response.length + " companies matching the same details you entered: <br/>" + htmlToDisplay),
                            type: "warning",
                            showCancelButton: true
                        }).then(function (result) {
                            if (!result.value) {
                                self.GotoPreviousPage();
                            }
                        });
                    } else {
                        // no duplicates found, add the company
                        self.SaveAjaxCall(request);
                    }
                },
                beforeSend: function () {
                    $("#btnSave").attr("disabled", true);
                },
                error: function (request, status, error) {
                    $("#btnSave").attr("disabled", false);
                }
            });
        } else {
            this.SaveAjaxCall(request);
        }

    };

    this.SaveAjaxCall = function (request) { 
        // ajax to save the company
        $.ajax({
            type: "POST",
            url: "/api/company/SaveCompany",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(request),
            success: function (response) {
                $("#btnSave").attr("disabled", false);
                if (parseInt(response) > 0) {
                    $content.addClass("hide");
                    $successBox.removeClass("hide");
                    $("html, body").animate({ scrollTop: 0 }, "slow");

                    if (request.Company.CompanyId < 1) {
                        location.href = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + response + "&subscriberid=" + request.Company.SubscriberId;
                        removeSpinner();
                    } else {
                        self.GotoPreviousPage(response, request.Company.SubscriberId);
                        removeSpinner();
                    }

                    // add contact
                    $(".add-contact").unbind("click").click(function () {
                        location.href = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + response + "&newcontact=1&subscriberid=" + company.SubscriberId;
                    });

                    return;

                }
                removeSpinner();

            },
            beforeSend: function () {
                addSpinner();
                $("#btnSave").attr("disabled", true);
            },
            error: function (request, status, error) { 
                $("#btnSave").attr("disabled", false);
            }
        });
    };


    this.GotoPreviousPage = function (response, sid) {
        switch (fromPage) {
            case "companydetail":
                location.href = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + response + "&subscriberid=" + sid;
                break;
            case "dealdetail":
                location.href = "/Deals/DealDetail/dealdetail.aspx?dealId=" + fromId + "&dealsubscriberId=" + subscriberId;
                break;
            case "contactdetail":
                break;
            case "companylist":
                var url = "/Companies/CompanyList/CompanyList.aspx?";
                var pg = getQueryString("pg");
                if (pg && pg !== '') {
                    url += "page=" + pg;
                }
                location.href = url;
                break;
            default:

        }
    };

    this.DeleteCompany = function () {
        swal({
            title: translatePhrase("Delete Company!"),
            text: translatePhrase("Are you sure you want to delete this company?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/company/DeleteCompany/?companyid=" + companyId + "&userId=" + userId + "&subscriberid=" + companySubscriberid,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data) {
                            location.href = "/Companies/CompanyList/CompanyList.aspx";
                        }
                    }, beforeSend: function () {
                        addSpinner();
                    }
                });
            }
        });
    };
};

var FileUpload = function () {
    var self = this;
    var baseUploadUrl = '/_handlers/temp-file-upload-handler.ashx?';

    // buttons
    var $bLogo = $("#btnUploadCompanyLogo");
    // inputs
    var $iptLogo = $("#fuCompanyLogo");
    // image
    var $imgCompanyLogo = $("#imgCompanyLogo");


    this.Init = function () {

        $iptLogo.on('change', function () {
            readURL(this);
        });
        $bLogo.on('click', function () {
            $iptLogo.click();
        });
        $imgCompanyLogo.on('click', function () {
            $iptLogo.click();
        });

        var readURL = function (input) {
            if (input.files && input.files[0]) {
                // setup data
                var fileData = new window.FormData();
                fileData.append('file', input.files[0]);
                // AJAX to upload
                $.ajax({
                    type: "POST",
                    url: baseUploadUrl + "ref=" + $("#lblGuid").text() + "&folder=companylogo&delete=true",
                    data: fileData,
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        removeSpinner();
                        // parse JSON result
                        var response = JSON.parse(data);
                        var image = response[0];
                        // bind image
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
        $imgCompanyLogo.attr("src", image.Uri);
        $imgCompanyLogo.attr("data-name", image.FileName);
        $imgCompanyLogo.attr("blob-reference", image.BlobReference);
        $imgCompanyLogo.attr("container-reference", image.ContainerReference);
        //self.SaveImage(image);
    };





    // initialize file uploads
    this.DeleteUploadedFile = function (docId) {
        $.ajax({
            type: "GET",
            url: "/api/document/Delete/?id=" + docId + "&userId=" + userId + "&subscriberId=" + companySubscriberid,
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
            }
        });
    };

};


