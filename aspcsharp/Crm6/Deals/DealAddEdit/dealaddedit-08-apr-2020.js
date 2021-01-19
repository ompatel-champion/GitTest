var subscriberId = parseInt($("#lblSubscriberId").text());
var dealSubscriberId = parseInt($("#lblDealSubscriberId").text());
var dealId = parseInt($("#lblDealId").text());
var userId = parseInt($("#lblUserId").text());
// this will only have a value when came to this page from company detail
var companyId = parseInt($("#lblCompanyId").text());
var $divDealSetup = $("#divDealSetup");
var $loading = $("#DealAddEdit");

var $ddlCampaign = $("#ddlCampaign");
var $ddlCommodities = $("#ddlCommodities");
var $ddlCompany = $("#ddlCompany");
var $ddlCompetitors = $("#ddlCompetitors");
var $ddlContact = $("#ddlContact");
var $dealContact = $(".deal-contact");
var $ddlDealOwner = $("#ddlDealOwner");
var $ddlDealType = $("#ddlDealType");
var $ddlIncoterms = $("#ddlIncoterms");
var $ddlIndustry = $("#ddlIndustry");
var $ddlSalesStage = $("#ddlSalesStage");
var $ddlWonLostReason = $("#ddlWonLostReason");
var $divWonLostReason = $("#divWonLostReason");
var $divCompanyContainer = $("#divCompanyContainer");

var $content = $(".form-box");
var $successBox = $(".success-box");
var fromPage = "";

$(function () {
    new PageSetup().Init();
    new Deal().Init();

    // Comment by Sebworks 10-17-2019
    // Commented out the following because it was silently failing.
    // SetCompany and SetContact expect an object with { Text, Value }.
    // new Company().SetCompany();
    // new Contact().SetContact();

    if (dealId) {
        $('title').text('Edit Deal');
    } else {
        $('title').text('Add Deal');
    }
});


var PageSetup = function () {
    var self = this;

    this.Init = function () {
        fromPage = getQueryString("from");

        $("#txtDealName").on('focus', function (e) {
            e.preventDefault();
            $(this).attr("autocomplete", "off");
        });
        // focus title
        $("#txtDealName").focus();

        $ddlCommodities.val($('#hdnCommodities').val().split(',')).trigger('change');
        $ddlCompetitors.val($('#hdnCompetitors').val().split(',')).trigger('change');
        $ddlCampaign.val($('#hdnCampaigns').val().split(',')).trigger('change');
        $ddlIncoterms.val($('#hdnIncoterms').val().split(',')).trigger('change');
        //  $ddlIndustry.val($('#hdnIndustry').val().split(',')).trigger('change');

        $('#txtDealName,#txtComments').on('keyup keypress', function () {
            if ($(this).val() === '') {
                $(this).parent('.form-group').removeClass('filled');
            } else {
                $(this).parent('.form-group').addClass('filled');
            }
        });

        $('#txtDealName,#txtComments,#ddlCommodities,#ddlIndustry,#ddlCompetitors,#ddlIncoterms').each(function () {
            if ($(this).val() !== "") {
                $(this).parent('.form-group').addClass('filled');
            }
        });

        $("#btnSave").unbind("click").click(function () {
            $divDealSetup.submit();
        });

        $("#btnCancel").unbind("click").click(function (event) {

            switch (fromPage) {
                case "dealdetail":
                    location.href = "/Deals/DealDetail/dealdetail.aspx?dealId=" + dealId + "&dealsubscriberId=" + dealSubscriberId;
                    break;
                case "companydetail":
                    location.href = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + companyId + "&subscriberId=" + dealSubscriberId + "#deals";
                    break;
                case "contactdetail":
                    event.preventDefault();
                    history.back(1);

                    break;
                case "deallist":
                    event.preventDefault();
                    history.back(1);
                    break;
                default:
                    event.preventDefault();
                    history.back(1);

            }
        });

        if (dealId > 0) {
            $("#btnDelete").removeClass("hide");
            $("#btnDelete").unbind("click").click(function () {
                new Deal().DeleteDeal();
            });
        }

        self.SetupSelect2Dropdowns();
        self.InitDatePickers();
    };

    this.SetupSelect2Dropdowns = function () {
        $ddlCampaign.select2({
            tags: true,
            "theme": "classic"
        });
        // $ddlCommodities.select2({ "theme": "classic", minimumResultsForSearch: Infinity });


        $ddlCommodities.select2({
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
            }
        });


        $ddlCompetitors.select2({
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
            }
        });

        $ddlDealOwner.select2({ "placeholder": '', minimumResultsForSearch: Infinity });
        $ddlDealType.select2({ "placeholder": '', minimumResultsForSearch: Infinity });
        // if add deal - default deal type to qualifying
        if (dealId === 0) {
            $ddlDealType.val("New Business").trigger("change");
        }
        $ddlIncoterms.select2({ "theme": "classic", minimumResultsForSearch: Infinity });
        $ddlIndustry.select2({ "placeholder": 'Select Industry' });
        $ddlSalesStage.select2({ minimumResultsForSearch: Infinity });
        $ddlWonLostReason.select2({ "theme": "classic" });
 

        $ddlCompany.select2({
            minimumInputLength: 1,
            placeholder: '',
            ajax: {
                url: function (obj) {
                    var keyword = obj.term ? obj.term : "";
                    return "/api/AutoComplete/?type=globalcompanywithpermission&UserId=" + userId + "&SusbcriberId=" + dealSubscriberId + "&prefix=" + keyword.replace("&", "%26");
                },
                dataType: "json",
                timeout: 50000,
                type: "GET",
                data: '',
                processResults: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.name,
                                id: item.id
                            };
                        })
                    };
                }
            }
        });



        // set contact dropdown for company
        //self.InitContactSelect2();

        // enable disable contact dropdowns
        self.EnableDisableContactDropdown();
        $ddlCompany.on('select2:select', function (evt) {
            self.EnableDisableContactDropdown();
        });

        // sales stage
        $ddlSalesStage.on('select2:select', function (evt) {
            // if add deal - default to qualifying
            //if (dealId === 0) {
            //    $ddlSalesStage.val("Qualifying");
            //}
            var selectedStage = $(this).val();
            if (selectedStage === "Won" || selectedStage === "Lost") {
                self.InitWonLostReasons();
            } else {
                $divWonLostReason.addClass("hide");
            }
        });
        var selectedStage = $ddlSalesStage.val();
        if (selectedStage === "Won" || selectedStage === "Lost") {
            $divWonLostReason.removeClass("hide");
        } else {
            $divWonLostReason.addClass("hide");
        }

        $ddlCompany.unbind("change").change(function () {
            if ($ddlContact.children('option').length === 1) {
                //TODO
            }
        });


    };

    this.InitWonLostReasons = function () {
        // if the selected value is WON or LOST show the won lost reasons 
        var selectedStage = $ddlSalesStage.val();
        self.LoadWonLostReasons(selectedStage);
        $divWonLostReason.removeClass("hide");
    };

    this.LoadWonLostReasons = function (salesStage) {
        var url = "";
        // won
        if (salesStage === "Won") {
            url = "/api/Dropdown/GetWonReasons";
            // lost
        } else {
            url = "/api/Dropdown/GetLostReasons";
        }

        $.ajax({
            type: "GET",
            url: url + "?subscriberId=" + dealSubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (response) {
                $ddlWonLostReason.html("");
                $ddlWonLostReason.append('<option value="0">Select Reason</option>');
                $.each(response, function (i, d) {
                    $ddlWonLostReason.append('<option value="' + d.SelectValue + '">' + d.SelectText + '</option>');
                });
                $ddlWonLostReason.val("0").trigger("change");
            }, beforeSend: function () {
            }, error: function (request, status, error) {
            }
        });
    };

    this.InitContactSelect2 = function () {
        // contact
        $ddlContact.select2({
            minimumResultsForSearch: Infinity,
            placeholder: translatePhrase("Select Contact"),

            ajax: {
                url: function (obj) {
                    var globalcompanyId = $("#ddlCompany").val();
                    var companyStr = "0";
                    if (globalcompanyId && globalcompanyId !== '0') {
                        companyStr = globalcompanyId;
                    }
                    if (!obj.term) {
                        obj.term = "";
                    }

                    var keyword = obj.term;
                    return "/api/AutoComplete/?type=companycontacts&globalcompanyId=" + companyStr + "&UserId=" + userId + "&SusbcriberId=" + dealSubscriberId + "&prefix=" + keyword.replace("&", "%26");
                },
                dataType: "json",
                timeout: 50000,
                type: "GET",
                data: '',
                processResults: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.name,
                                id: item.id
                            };
                        })
                    };
                }
            }
        });
    };

    this.EnableDisableContactDropdown = function () {

        if ($('#ddlCompany').val() && $('#ddlCompany').val() !== '' && parseInt($('#ddlCompany').val()) > 0) {
            var companyId = parseInt($("#ddlCompany").val());
            if (companyId > 0) {
                $dealContact.removeClass('hide');
                $ddlCompany.val($('#ddlCompany').val()).trigger("change");
                self.InitContactSelect2();
                $('#ddlContact').prop('disabled', false);
            } else {
                $dealContact.addClass('hide');
                $('#ddlContact').prop('disabled', true);
            }
        } else {
            $dealContact.addClass('hide');
            $('#ddlContact').prop('disabled', true);
        }
    };

    this.InitDatePickers = function () {

        // initialize dates
        $("[data-name='datepicker']").datepicker({
            format: "dd-M-y",
            dateFormat: 'dd-M-y',
            autoclose: true,
            todayHighlight: true,
            onClose: function (dateText, inst) {
                $(this).attr("disabled", false);
            },
            onSelect: function () {
                if ($(this).valid()) {
                    $(this).removeClass('error');
                }
            },
            beforeShow: function (input, inst) {
                //$(this).attr("disabled", true);
                var offset = $(this).offset();
                var posX = offset.left - $(window).scrollLeft();//- 260;  
                setTimeout(function () {
                    inst.dpDiv.css({
                        left: posX
                    });
                }, 0);
            }
        }).on("changeDate", function (e) {
            $(this).valid();
        });
    };
};


var Company = function () {
    this.SetCompany = function (companyListItem) {
        //set the option
        var option = new Option(companyListItem.Text, companyListItem.Value, true, true);
        // Append it to the select
        $ddlCompany.append(option).trigger('change');
        new PageSetup().EnableDisableContactDropdown();
    };
};


var Contact = function () {
    this.SetContact = function (contactListItem) {
        // set the option
        var option = new Option(contactListItem.Text, contactListItem.Value);
        option.selected = true;
        // append and trigger change event
        $ddlContact.append(option);
        $ddlContact.trigger("change");
    };
};


var Deal = function () {
    var self = this;
    this.Init = function () {
        // init validator
        self.InitDealSaveValidator();
    };

    this.InitDealSaveValidator = function () {

        // select2 dropdown validator
        $ddlCompany.on('select2:select', function (evt) { $(this).valid(); });
        $ddlContact.on('select2:select', function (evt) { $(this).valid(); });
        $ddlDealOwner.on('select2:select', function (evt) { $(this).valid(); });
        $ddlIndustry.on('select2:select', function (evt) { $(this).valid(); });
        $ddlSalesStage.on('select2:select', function (evt) { $(this).valid(); });
        $ddlDealType.on('select2:select', function (evt) { $(this).valid(); });

        // add the rule here
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            if (value !== null && value.length === 0)
                return false;
            return arg !== value && value !== null && value !== "" && value !== "0";
        }, "");
        $.validator.addMethod("decisionDate", function (value, element, arg) {
            var $proposalDate = $("#txtProposalDate");
            var proposalDate = moment($proposalDate.datepicker("getDate"));
            var decisionDate = moment(value);
            if (proposalDate.isValid() && decisionDate.isValid()) {
                $proposalDate.removeClass('error');
                $("#txtProposalDate-error").css({display:"none"});
                return decisionDate.isAfter(proposalDate);
            }
            return true;
        }, translatePhrase("Must be after the Proposal Date"));
         $.validator.addMethod("proposalDate", function (value, element, arg) {
            var $decisionDate = $("#txtDecisionDate");
            var proposalDate = moment(value);
            var decisionDate =  moment($decisionDate.datepicker("getDate"));
            if (proposalDate.isValid() && decisionDate.isValid()) {
                $decisionDate.removeClass('error');
                $("#txtDecisionDate-error").css({display:"none"});
                return decisionDate.isAfter(proposalDate);
            }
            return true;
        }, translatePhrase("Must be before the Decision Date"));

        // validate
        var validator = $("#divDealSetup").validate({
            rules: {
                txtDealName: { required: true },
                ddlCompany: { valueNotEquals: "null" },
                ddlContact: { valueNotEquals: "null" },
                ddlDealOwner: { valueNotEquals: "null" },
                ddlDealType: { valueNotEquals: "null" },
                ddlIndustry: { valueNotEquals: "null" },
                ddlSalesStage: { valueNotEquals: "null" },
                txtDecisionDate: { 
                    required: true,
                    decisionDate: {}
                },
                txtProposalDate: {
                    required: true,
                    proposalDate: {}
                }
            }, messages: {
                ddlCompany: translatePhrase("Required"),
                ddlContact: translatePhrase("Required"),
                txtDealName: translatePhrase("Required"),
                ddlSalesRep: translatePhrase("Required"),
                ddlDealType: translatePhrase("Required"),
                ddlIndustry: translatePhrase("Required"),
                ddlSalesStage: translatePhrase("Required"),
                txtDecisionDate:
                {
                    required: translatePhrase("Required"),
                },
                txtProposalDate:
                {
                    required: translatePhrase("Required")
                }
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
                $element.closest('.form-group').find(".select2-selection--single").addClass("error-border");
                $element.closest('.form-group').find(".select2-selection--multiple").addClass("error-border");
            },
            submitHandler: function () {
                self.SaveDeal();
            }
        });
    };

    this.SaveDeal = function () {
        var deal = new Object();
        deal.DealId = dealId;
        deal.SubscriberId = dealSubscriberId;
        deal.Comments = $("#txtComments").val();
        deal.Commodities = $ddlCommodities.val().join(",");
        deal.CompanyIdGlobal = $ddlCompany.val();
        deal.CompanyName = $ddlCompany.text();
        deal.Competitors = $("#ddlCompetitors").val().join(",");
        deal.DealName = $("#txtDealName").val();
        deal.DealOwnerId = $ddlDealOwner.val();
        deal.DealOwnerName = $ddlDealOwner.text();
        deal.DealType = $ddlDealType.val();
        deal.Incoterms = $ddlIncoterms.val().join(",");
        deal.Industry = $ddlIndustry.val();
        deal.PrimaryContactId = $ddlContact.val();
        deal.PrimaryContactName = $ddlContact.text();
        deal.SalesStageName = $ddlSalesStage.val();
        deal.Campaign = $ddlCampaign.val().join(",");

        // dates
        deal.ContractEndDate = moment($("#txtContractEndDate").datepicker("getDate")).isValid() ? moment($("#txtContractEndDate").datepicker("getDate")).format("DD-MMM-YY") : null;
        deal.DecisionDate = moment($("#txtDecisionDate").datepicker("getDate")).isValid() ? moment($("#txtDecisionDate").datepicker("getDate")).format("DD-MMM-YY") : null;
        deal.EstimatedStartDate = moment($("#txtFirstShipmentDate").datepicker("getDate")).isValid() ? moment($("#txtFirstShipmentDate").datepicker("getDate")).format("DD-MMM-YY") : null;
        deal.DateProposalDue = moment($("#txtProposalDate").datepicker("getDate")).isValid() ? moment($("#txtProposalDate").datepicker("getDate")).format("DD-MMM-YY") : null;
        deal.UpdateUserId = userId;

        // won/lost reasons
        var selectedStage = $ddlSalesStage.val();
        if (selectedStage === "Won" || selectedStage === "Lost") {
            {
                deal.ReasonWonLost = $("#ddlWonLostReason option:selected").text();
            }
        }

        var request = new Object();
        request.Deal = deal;
        request.SavingUserId = userId;
        request.SavingUserSubscriberId = subscriberId;
        // AJAX to save the deal
        $.ajax({
            type: "POST",
            url: "/api/deal/SaveDeal",
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

                    if (dealId < 1) {
                        // new deal, goto add lane
                        location.href = "/deals/laneaddedit/laneaddedit.aspx?laneId=0&dealId=" + response;
                        return;
                    } else {
                        switch (fromPage) {
                            case "dealdetail":
                                location.href = "/Deals/DealDetail/dealdetail.aspx?dealId=" + response + "&dealsubscriberId=" + dealSubscriberId;
                                break;
                            case "companydetail":
                                location.href = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + companyId + "&subscriberId=" + subscriberId + "#deals";
                                break;
                            case "contactdetail":
                                break;
                            case "deallist":
                                var url = "/Deals/DealList/DealList.aspx?";
                                var pg = getQueryString("pg");
                                if (pg && pg !== '') {
                                    url += "page=" + pg;
                                }
                                location.href = url;
                                break;
                            default:
                                location.href = "/Deals/DealDetail/dealdetail.aspx?dealId=" + response + "&dealsubscriberId=" + dealSubscriberId;
                        }
                    }


                } else {
                    alert("Error Saving Deal...");
                    removeSpinner();
                }
            }, beforeSend: function () {
                addSpinner();
                $("#btnSave").attr("disabled", true);
            }, error: function () {
                $("#btnSave").attr("disabled", false);
            }
        });
    };

    this.DeleteDeal = function () {
        swal({
            title: translatePhrase("Delete Deal!"),
            text: translatePhrase("Are you sure you want to delete this deal?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!"),
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/deal/DeleteDeal/?dealId=" + dealId + "&userId=" + userId + "&dealSubscriberId=" + dealSubscriberId + "&userSubscriberId=" + subscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        bindLoadingMsg("", $loading, false);
                        if (data) {
                            location.href = "/Deals/DealList/DealList.aspx";
                        }
                    }, beforeSend: function () {
                        addSpinner();
                    }
                });
            }
        });
    };

};


function bindLoadingMsg(msg, $parent, binsert, topMargin) {
    // delete existing spinner
    $parent.find(".loading-msg").remove();

    if (binsert) {
        // create loading spinner
        var $loading = $('<div class="loading-msg text-center"></div>');
        var $spinner = spinkit.getSpinner(spinkit.spinerTypes.fadingCircle);
        $spinner.css("margin-top", topMargin && topMargin !== "" ? topMargin : "10px");
        $loading.append($spinner);
        $loading.append($("<div class='loading-msg m-t-xs'>" + msg + "</div>"));
        $loading.appendTo($parent);
    }
}
