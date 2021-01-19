var subscriberId = parseInt($("#lblSubscriberId").text());
var campaignId = parseInt($("#lblCampaignId").text());
var userId = parseInt($("#lblUserId").text());
var userIdGlobal = parseInt($("#lblUserIdGlobal").text());
var $divCampaignSetup = $("#divCampaignSetup");

// fields
var $txtCampaignName = $("#txtCampaignName");
var $txtCampaignNumber = $("#txtCampaignNumber");
var $ddlCampaignOwner = $("#ddlCampaignOwner");
var $ddlCampaignType = $("#ddlCampaignType");
var $txtComments = $("#txtComments");
var $txtEndDate = $("#txtEndDate");
var $txtStartDate = $("#txtStartDate");


$(function () {
    // setup page
    new PageSetup().Init();
    // initialize campaign
    new Campaign().Init();

    if (campaignId) {
        $('title').text('Edit Campaign');
    } else {
        $('title').text('Add Campaign');
    }
});


var PageSetup = function () {
    var self = this;

    this.Init = function () {

        $("#btnSave").unbind("click").click(function () {
            $divCampaignSetup.submit();
        });

        $("#btnCancel").unbind("click").click(function (event) {
            event.preventDefault();
            history.back(1);
        });

        if (campaignId > 0) {
            $("#btnDelete").removeClass("hide");
            $("#btnDelete").unbind("click").click(function () {
                new Campaign().DeleteCampaign();
            });
        }

        $ddlCampaignType.select2();

        self.InitDatePickers();
    };

    this.InitDatePickers = function () {
        // initialize dates
        $("[data-name='datepicker']").datepicker({
            dateFormat: "dd M, yy",
            autoclose: true,
            todayHighlight: true
            //onClose: function (dateText, inst) {
            //    $(this).attr("disabled", false);
            //},
            //beforeShow: function (input, inst) {
            //    $(this).attr("disabled", true);
            //}
        }).on("changeDate", function (e) {
            $(this).valid();
        });
    };

};


var Campaign = function () {
    var self = this;

    this.Init = function () {
        // init validator
        self.InitSaveValidator();
    };

    this.InitSaveValidator = function () {
        // select2 dropdown validator
        $ddlCampaignType.on('select2:select', function (evt) { $(this).valid(); });
        // add the rule here
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return arg !== value && value !== null && value !== "0";
        }, "");

        // validate
        $divCampaignSetup.validate({
            rules: {
                txtCampaignName: { required: true },
                txtCampaignNumber: { required: true },
                txtStartDate: { required: true },
                txtEndDate: { required: true },
                ddlCampaignTypes: { valueNotEquals: "null" }
            }, messages: {
                txtCampaignName: "Enter the campaign name",
                txtCampaignNumber: "Enter the campaign number",
                txtStartDate: "Select a start date",
                txtEndDate: "Select a end date",
                ddlCampaignTypes: "Select a campaign type"
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveCampaign();
            }
        });
    };

    this.SaveCampaign = function () {
        var campaign = new Object();
        campaign.CampaignId = campaignId;
        campaign.SubscriberId = subscriberId;
        campaign.CampaignName = $txtCampaignName.val();
        campaign.CampaignNumber = $txtCampaignNumber.val(); 
        campaign.CampaignOwnerUserIdGlobal = $ddlCampaignOwner.val();
        campaign.CampaignStatus = $("#rbtActive").is(":checked") ? "Active" : "Inactive";
        campaign.CampaignType = $ddlCampaignType.val();
        campaign.Comments = $txtComments.val();
        campaign.StartDate = moment($("#txtStartDate").datepicker("getDate")).isValid() ? moment($("#txtStartDate").datepicker("getDate")).format("YYYY-MM-DD") : null;
        campaign.EndDate = moment($("#txtEndDate").datepicker("getDate")).isValid() ? moment($("#txtEndDate").datepicker("getDate")).format("YYYY-MM-DD") : null;
        campaign.UpdateUserIdGlobal = userIdGlobal;

 
        // AJAX to save the campaign
        $.ajax({
            type: "POST",
            url: "/api/campaign/SaveCampaign",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(campaign),
            success: function (response) {
                removeSpinner();
                if (response > 0) {
                    window.location.href = "/Admin/Campaigns/CampaignList/Campaigns.aspx";
                } else {
                    alert("Error saving campaign!");
                }
            }, beforeSend: function () {
                addSpinner();
            }, error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteCampaign = function () {
        swal({
            title: translatePhrase("Delete Campaign!"),
            text: translatePhrase("Are you sure you want to delete this campaign?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!"),
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/campaign/deletecampaign/?campaignId=" + campaignId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data) {
                            window.location.href = "/Admin/Campaigns/CampaignList/Campaigns.aspx";
                        }
                    }, beforeSend: function () {
                        addSpinner();
                    }
                });
            }
        });
    };

};
