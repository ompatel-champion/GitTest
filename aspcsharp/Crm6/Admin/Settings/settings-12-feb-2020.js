// set global variables
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var $commodities = $("#commodities");
var $companySegments = $("#company-segments");
var $companyTypes = $("#company-types");
var $competitors = $("#competitors");
var $contactTypes = $("#contact-types");
var $dealTypes = $("#deal-types");
var $industries = $("#industries");
var $lostReasons = $("#lost-reasons");
var $salesStages = $("#sales-stages");
var $salesTeamRoles = $("#sales-team-roles");
var $sources = $("#sources");
var $tags = $("#tags");
var $wonReasons = $("#won-reasons");

//library methods
const lib = {
    //save or reset an edited input field (typically triggered on field blur)
    saveOrResetField: function (type, elm, saveFunc) {
        const elm_tr = elm.closest("tr");
        const lastVal = elm_tr.attr("data-current-" + type);
        const curVal = elm.val();
        if (curVal !== lastVal) {
            if (curVal !== '') {
                saveFunc(elm_tr);
                elm_tr.attr("data-current-" + type, curVal);
            }
            else elm.val(lastVal);
        }
    }
};


$(document).ready(function () {
    initTabs();
    setActiveTabFromQuerystring();
	
	// Mobile - Panel Dropdown Menu
	$('.panel-dropdown .ae-select-content').text($('.panel-dropdown .dropdown-nav > li.selected').text());
	var newOptions = $('.panel-dropdown .dropdown-nav > li');
	newOptions.click(function () {
		$('.panel-dropdown .ae-select-content').text($(this).text());
		$('.panel-dropdown .dropdown-nav > li').removeClass('selected');
		$(this).addClass('selected');
	});
	var aeDropdown = $('.panel-dropdown .ae-dropdown');
	aeDropdown.click(function () {
		$('.panel-dropdown .dropdown-nav').toggleClass('ae-hide');
		$('.panel-dropdown .ae-select').toggleClass('drop-open');
	});
});


var initTabs = function () {
    // init tab click function
    $('.border-tabs .btab,.nav-tabs a').click(function () {
        var tcID = $(this).attr('data-id');
        $('.btab-content').hide();
        $(tcID).fadeIn();
        $('.border-tabs .btab').removeClass('active');
        $(this).addClass('active');
    });
    // init tab row functions
    new Commodities().Init();
    new CompanySegments().Init();
    new CompanyTypes().Init();
    new Competitors().Init();
    new ContactTypes().Init();
    new DealTypes().Init();
    new Industries().Init();
    new LostReasons().Init();
    new SalesStages().Init();
    new SalesTeamRoles().Init();
    new Sources().Init();
    new Tags().Init();
    new WonReasons().Init();
};


var setActiveTabFromQuerystring = function () {
    var currentTab = getQueryString("tab");
    // check for the passed QS tab and set it
    if (currentTab && currentTab !== '') {
        $(".border-tabs").find(".btab").removeClass("active");
        switch (currentTab) {
            case "commodities":
                $("[data-id='#divCommoditiesTab']").click();
                $("[data-id='#divCommoditiesTab']").addClass("active");
                break;
            case "company-segments":
                $("[data-id='#divCompanySegmentsTab']").click();
                $("[data-id='#divCompanySegmentsTab']").addClass("active");
                break;
            case "company-types":
                $("[data-id='#divCompanyTypesTab']").click();
                $("[data-id='#divCompanyTypesTab']").addClass("active");
                break;
            case "competitors":
                $("[data-id='#divCompetitorsTab']").click();
                $("[data-id='#divCompetitorsTab']").addClass("active");
                break;
            case "contact-types":
                $("[data-id='#divContactTypesTab']").click();
                $("[data-id='#divContactTypesTab']").addClass("active");
                break;
            case "deal-types":
                $("[data-id='#divDealTypesTab']").click();
                $("[data-id='#divDealTypesTab']").addClass("active");
                break;
            case "industries":
                $("[data-id='#divIndustriesTab']").click();
                $("[data-id='#divIndustriesTab']").addClass("active");
                break;
            case "lost-reasons":
                $("[data-id='#divLostReasonsTab']").click();
                $("[data-id='#divLostReasonsTab']").addClass("active");
                break;
            case "sources":
                $("[data-id='#divSourcesTab']").click();
                $("[data-id='#divSourcesTab']").addClass("active");
                break;
            case "sales-stages":
                $("[data-id='#divSalesStagesTab']").click();
                $("[data-id='#divSalesStagesTab']").addClass("active");
                break;
            case "sales-team-roles":
                $("[data-id='#divSalesTeamRolesTab']").click();
                $("[data-id='#divSalesTeamRolesTab']").addClass("active");
                break;
            case "tags":
                $("[data-id='#divTagsTab']").click();
                $("[data-id='#divTagsTab']").addClass("active");
                break;
            case "won-reasons":
                $("[data-id='#divWonReasonsTab']").click();
                $("[data-id='#divWonReasonsTab']").addClass("active");
                break;
            default:
        }
    }
};


var Commodities = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $commodities.find("tr").each(function () {
            var $tr = $(this);
            // delete commodity
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var commodityId = $(this).closest("tr").attr("data-id");
                self.DeleteCommodity(commodityId);
            });

            // commodity name change
            $tr.find(".commodity-name").unbind("blur").blur(function () {
                lib.saveOrResetField("commodity", $(this), self.SaveCommodity);
            });
        });

        // new commodity
        $("#txtNewCommodity").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddCommodity").unbind("click").click(function () {
            self.AddNewCommodity();
        });
    };

    this.AddNewCommodity = function () {
        var isError = false;
        if ($("#txtNewCommodity").val() === "") {
            $("#txtCommodity").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveCommodity(null, true);
    };

    this.SaveCommodity = function ($tr, newCommodity) {
        var commodity = new Object();
        commodity.SubscriberId = subscriberId;
        commodity.UpdateUserId = userId;
        if (newCommodity) {
            commodity.CommodityId = 0;
            commodity.CommodityName = $("#txtNewCommodity").val();
        } else {
            commodity.CommodityId = $tr.attr("data-id");
            commodity.CommodityName = $tr.find(".commodity-name").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/commodity/SaveCommodity",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(commodity),
            success: function (commodityId) {
                if (parseInt(commodityId) > 0) {
                    if (!newCommodity) {
                        $("#divSpinner").addClass("hide");
                        $tr.attr("data-current-commodity", commodity.CommodityName);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewCommodity").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=commodities";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteCommodity = function (commodityId) {
        swal({
            title: translatePhrase("Delete Commodity!"),
            text: translatePhrase("Are you sure you want to delete this commodity?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/commodity/DeleteCommodity/?commodityId=" + commodityId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=commodities";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var CompanySegments = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $companySegments.find("tr").each(function () {
            var $tr = $(this);

            // delete company segment
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var companySegmentId = $(this).closest("tr").attr("data-id");
                self.DeleteCompanySegment(companySegmentId);
            });

            // company segment name change
            $tr.find(".company-segment-name").unbind("blur").blur(function () {
                lib.saveOrResetField("company-segment", $(this), self.SaveCompanySegment);
            });
        });

        // new company segment
        $("#txtNewCompanySegment").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddCompanySegment").unbind("click").click(function () {
            self.AddNewCompanySegment();
        });
    };

    this.AddNewCompanySegment = function () {
        var isError = false;
        if ($("#txtNewCompanySegment").val() === "") {
            $("#txtCompanySegment").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveCompanySegment(null, true);
    };

    this.SaveCompanySegment = function ($tr, newCompanySegment) {
        var companySegment = new Object();
        companySegment.SubscriberId = subscriberId;
        companySegment.UpdateUserId = userId;
        if (newCompanySegment) {
            companySegment.CompanySegmentId = 0;
            companySegment.SegmentName = $("#txtNewCompanySegment").val();
        } else {
            companySegment.CompanySegmentId = $tr.attr("data-id");
            companySegment.SegmentName = $tr.find(".company-segment-name").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/companysegment/SaveCompanySegment",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(companySegment),
            success: function (companySegmentId) {
                if (parseInt(companySegmentId) > 0) {
                    if (!newCompanySegment) {
                        $("#divSpinner").addClass("hide");
                        $tr.attr("data-current-company-segment", companySegment.SegmentName);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewCompanySegment").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=company-segments";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteCompanySegment = function (companySegmentId) {
        swal({
            title: translatePhrase("Delete Company Segment!"),
            text: translatePhrase("Are you sure you want to delete this company segment?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/companysegment/DeleteCompanySegment/?companySegmentId=" + companySegmentId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=company-segments";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var CompanyTypes = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $companyTypes.find("tr").each(function () {
            var $tr = $(this);

            // delete company type
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var companyTypeId = $(this).closest("tr").attr("data-id");
                self.DeleteCompanyType(companyTypeId);
            });

            // company type name change
            $tr.find(".company-type-name").unbind("blur").blur(function () {
                lib.saveOrResetField("company-type-name", $(this), self.SaveCompanyType);
            });
        });

        // new company type
        $("#txtNewCompanyType").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddCompanyType").unbind("click").click(function () {
            self.AddNewCompanyType();
        });
    };

    this.AddNewCompanyType = function () {
        var isError = false;
        if ($("#txtNewCompanyType").val() === "") {
            $("#txtCompanyType").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveCompanyType(null, true);
    };

    this.SaveCompanyType = function ($tr, newCompanyType) {
        var companyType = new Object();
        companyType.SubscriberId = subscriberId;
        companyType.UpdateUserId = userId;
        if (newCompanyType) {
            companyType.CompanyTypeId = 0;
            companyType.CompanyTypeName = $("#txtNewCompanyType").val();
        } else {
            companyType.CompanyTypeId = $tr.attr("data-id");
            companyType.CompanyTypeName = $tr.find(".company-type-name").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/companytype/SaveCompanyType",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(companyType),
            success: function (companyTypeId) {
                $("#divSpinner").addClass("hide");
                if (parseInt(companyTypeId) > 0) {
                    if (!newCompanyType) {
                        $tr.attr("data-current-company-type-name", companyType.CompanyTypeName);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewCompanyType").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=company-types";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteCompanyType = function (companyTypeId) {
        swal({
            title: translatePhrase("Delete Company Type!"),
            text: translatePhrase("Are you sure you want to delete this company type?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/companytype/DeleteCompanyType/?companyTypeId=" + companyTypeId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=company-types";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var Competitors = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $competitors.find("tr").each(function () {
            var $tr = $(this);

            // delete competitor
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var competitorId = $(this).closest("tr").attr("data-id");
                self.DeleteCompetitor(competitorId);
            });

            // competitor name change
            $tr.find(".competitor-name").unbind("blur").blur(function () {
                lib.saveOrResetField("competitor-name", $(this), self.SaveCompetitor);
            });
        });

        // new competitor
        $("#txtNewCompetitor").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddCompetitor").unbind("click").click(function () {
            self.AddNewCompetitor();
        });
    };

    this.AddNewCompetitor = function () {
        var isError = false;
        if ($("#txtNewCompetitor").val() === "") {
            $("#txtCompetitor").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveCompetitor(null, true);
    };

    this.SaveCompetitor = function ($tr, newCompetitor) {
        var competitor = new Object();
        competitor.SubscriberId = subscriberId;
        competitor.UpdateUserId = userId;
        if (newCompetitor) {
            competitor.CompetitorId = 0;
            competitor.CompetitorName = $("#txtNewCompetitor").val();
        } else {
            competitor.CompetitorId = $tr.attr("data-id");
            competitor.CompetitorName = $tr.find(".competitor-name").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/competitor/SaveCompetitor",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(competitor),
            success: function (competitorId) {
                if (parseInt(competitorId) > 0) {
                    if (!newCompetitor) {
                        $("#divSpinner").addClass("hide");
                        $tr.attr("data-current-competitor", competitor.CompetitorName);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewCompetitor").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=competitors";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteCompetitor = function (competitorId) {
        swal({
            title: translatePhrase("Delete Competitor!"),
            text: translatePhrase("Are you sure you want to delete this competitor?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/competitor/DeleteCompetitor/?competitorId=" + competitorId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=competitors";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var ContactTypes = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $contactTypes.find("tr").each(function () {
            var $tr = $(this);

            // delete contact type
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var contactTypeId = $(this).closest("tr").attr("data-id");
                self.DeleteContactType(contactTypeId);
            });

            // contact type name change
            $tr.find(".contact-type-name").unbind("blur").blur(function () {
                lib.saveOrResetField("contact-type", $(this), self.SaveContactType);
            });
        });

        // new contact type
        $("#txtNewContactType").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddContactType").unbind("click").click(function () {
            self.AddNewContactType();
        });
    };

    this.AddNewContactType = function () {
        var isError = false;
        if ($("#txtNewContactType").val() === "") {
            $("#txtContactType").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveContactType(null, true);
    };

    this.SaveContactType = function ($tr, newContactType) {
        var contactType = new Object();
        contactType.SubscriberId = subscriberId;
        contactType.UpdateUserId = userId;
        if (newContactType) {
            contactType.ContactTypeId = 0;
            contactType.ContactTypeName = $("#txtNewContactType").val();
        } else {
            contactType.ContactTypeId = $tr.attr("data-id");
            contactType.ContactTypeName = $tr.find(".contact-type-name").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/contacttype/SaveContactType",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(contactType),
            success: function (contactTypeId) {
                $("#divSpinner").addClass("hide");
                if (parseInt(contactTypeId) > 0) {
                    if (!newContactType) {
                        $tr.attr("data-current-contact-type", contactType.ContactTypeName);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewContactType").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=contact-types";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteContactType = function (contactTypeId) {
        swal({
            title: translatePhrase("Delete Contact Type!"),
            text: translatePhrase("Are you sure you want to delete this contact type?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!"),
            closeOnConfirm: false
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/contacttype/DeleteContactType/?contactTypeId=" + contactTypeId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=contact-types";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) {
                    }
                });
            }
        });
    };
};


var DealTypes = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $dealTypes.find("tr").each(function () {
            var $tr = $(this);

            // delete deal type
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var dealTypeId = $(this).closest("tr").attr("data-id");
                self.DeleteDealType(dealTypeId);
            });

            // deal type name change
            $tr.find(".deal-type-name").unbind("blur").blur(function () {
                lib.saveOrResetField("deal-type", $(this), self.SaveDealType);
            });
        });

        // new deal type
        $("#txtNewDealType").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddDealType").unbind("click").click(function () {
            self.AddNewDealType();
        });
    };

    this.AddNewDealType = function () {
        var isError = false;
        if ($("#txtNewDealType").val() === "") {
            $("#txtNewDealType").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveDealType(null, true);
    };

    this.SaveDealType = function ($tr, newDealType) {
        var dealType = new Object();
        dealType.SubscriberId = subscriberId;
        dealType.UpdateUserId = userId;
        if (newDealType) {
            dealType.DealTypeId = 0;
            dealType.DealTypeName = $("#txtNewDealType").val();
        } else {
            dealType.DealTypeId = $tr.attr("data-id");
            dealType.DealTypeName = $tr.find(".deal-type-name").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/dealtype/SaveDealType",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(dealType),
            success: function (dealTypeId) {
                if (parseInt(dealTypeId) > 0) {
                    $("#divSpinner").addClass("hide");
                    if (!newDealType) {
                        $tr.attr("data-current-deal-type", dealType.DealTypeName);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewDealType").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=deal-types";
                    }
                }
            }, beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteDealType = function (dealTypeId) {
        swal({
            title: translatePhrase("Delete Deal Type!"),
            text: translatePhrase("Are you sure you want to delete this deal type?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/dealtype/DeleteDealType/?dealTypeId=" + dealTypeId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=deal-types";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var Industries = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $industries.find("tr").each(function () {
            var $tr = $(this);

            // delete industry
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var stageId = $(this).closest("tr").attr("data-id");
                self.DeleteIndustry(stageId);
            });

            // industry change
            $tr.find(".industry-name").unbind("blur").blur(function () {
                lib.saveOrResetField("industry", $(this), self.SaveIndustry);
            });
        });

        // new industry
        $("#txtNewIndustry").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddIndustry").unbind("click").click(function () {
            self.AddNewIndustry();
        });
    };

    this.AddNewIndustry = function () {
        var isError = false;
        if ($("#txtNewIndustry").val() === "") {
            $("#txtNewIndustry").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveIndustry(null, true);
    };

    this.SaveIndustry = function ($tr, newIndustry) {
        var industry = new Object();
        industry.SubscriberId = subscriberId;
        industry.UpdateUserId = userId;
        if (newIndustry) {
            industry.IndustryId = 0;
            industry.IndustryName = $("#txtNewIndustry").val();
        } else {
            industry.IndustryId = $tr.attr("data-id");
            industry.IndustryName = $tr.find(".industry-name").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/industry/SaveIndustry",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(industry),
            success: function (industryId) {
                $("#divSpinner").addClass("hide");
                if (parseInt(industryId) > 0) {
                    if (!newIndustry) {
                        $tr.attr("data-current-industry", industry.IndustryName);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewIndustry").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=industries";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteIndustry = function (industryId) {
        swal({
            title: translatePhrase("Delete Industry!"),
            text: translatePhrase("Are you sure you want to delete this industry?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/industry/DeleteIndustry/?industryId=" + industryId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=industries";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var LostReasons = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $lostReasons.find("tr").each(function () {
            var $tr = $(this);

            // delete lost reason
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var lostReasonId = $(this).closest("tr").attr("data-id");
                self.DeleteLostReason(lostReasonId);
            });

            // lost reason change
            $tr.find(".lost-reason").unbind("blur").blur(function () {
                lib.saveOrResetField("lost-reason", $(this), self.SaveLostReason);
            });
        });

        // new lost reason
        $("#txtNewLostReason").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddLostReason").unbind("click").click(function () {
            self.AddNewLostReason();
        });
    };

    this.AddNewLostReason = function () {
        var isError = false;
        if ($("#txtNewLostReason").val() === "") {
            $("#txtLostReason").addClass("error");
            isError = true;
        }

        if (!isError)
            self.SaveLostReason(null, true);
    };

    this.SaveLostReason = function ($tr, newlostReason) {
        var lostReason = new Object();
        lostReason.SubscriberId = subscriberId;
        lostReason.UpdateUserId = userId;
        if (newlostReason) {
            lostReason.lostReasonId = 0;
            lostReason.lostReasonName = $("#txtNewLostReason").val();
        } else {
            lostReason.lostReasonId = $tr.attr("data-id");
            lostReason.lostReasonName = $tr.find(".lost-reason").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/LostReason/SaveLostReason",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(lostReason),
            success: function (lostReasonId) {
                $("#divSpinner").addClass("hide");
                if (parseInt(lostReasonId) > 0) {
                    if (!newlostReason) {
                        $tr.attr("data-current-lost-reason", lostReason.lostReason);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewLostReason").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=lost-reasons";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteLostReason = function (lostReasonId) {
        swal({
            title: translatePhrase("Delete Lost Reason!"),
            text: translatePhrase("Are you sure you want to delete this lost Reason?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/LostReason/DeleteLostReason/?lostReasonId=" + lostReasonId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=lost-reasons";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var SalesStages = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $salesStages.find("tr").each(function () {
            var $tr = $(this);

            // delete sales stage
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var stageId = $(this).closest("tr").attr("data-id");
                self.DeleteSalesStage(stageId);
            });

            // sales stage name change
            $tr.find(".sales-stage-name").unbind("blur").blur(function () {
                lib.saveOrResetField("sales-stage", $(this), self.SaveSalesStage);
            });

            // sales stage percentage change
            $tr.find(".sales-percentage").unbind("blur").blur(function () {
                if ($(this).val() !== '') {
                    if (parseInt($(this).val()) > 100) {
                        $(this).addClass("error");
                    } else {
                        self.SaveSalesStage($(this).closest("tr"));
                        return;
                    }
                }
                // reset percent value
                $(this).val($(this).closest("tr").attr("data-current-sales-percentage"));
            });
            $tr.find(".sales-percentage").unbind("keyup").keyup(function () {
                if ($(this).val !== '')
                    $(this).removeClass("error");
            });
        });

        // new sales stage
        $("#txtNewSalesStage, #txtSalesStagePercentage").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        // validate percentage value => new value + percentage == 100
        $("#txtSalesStagePercentage").unbind("blur").keyup(function () {
            if ($(this).val === '')
                $(this).val("0");
            else {
                if (parseInt($(this).val()) > 100) {
                    $(this).addClass("error");
                    $(this).val("0");
                }
            }
        });

        $("#btnAddSalesStage").unbind("click").click(function () {
            self.AddNewSalesStage();
        });
    };

    this.AddNewSalesStage = function () {
        var isError = false;
        if ($("#txtNewSalesStage").val() === "") {
            $("#txtNewSalesStage").addClass("error");
            isError = true;
        }
        if ($("#txtSalesStagePercentage").val() === "" || parseInt($("#txtSalesStagePercentage").val()) < 1) {
            $("#txtSalesStagePercentage").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveSalesStage(null, true);
    };

    this.SaveSalesStage = function ($tr, newSalesStage) {
        var salesStage = new Object();
        salesStage.SubscriberId = subscriberId;
        salesStage.UpdateUserId = userId;
        if (newSalesStage) {
            salesStage.SalesStageId = 0;
            salesStage.SalesStageName = $("#txtNewSalesStage").val();
            salesStage.StagePercentage = $("#txtSalesStagePercentage").val();
        } else {
            salesStage.SalesStageId = $tr.attr("data-id");
            salesStage.SalesStageName = $tr.find(".sales-stage-name").val();
            salesStage.StagePercentage = $tr.find(".sales-percentage").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/salesstage/SaveSalesStage",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(salesStage),
            success: function (salesStageId) {
                if (parseInt(salesStageId) > 0) {
                    $("#divSpinner").addClass("hide");
                    if (!newSalesStage) {
                        $tr.attr("data-current-sales-stage", salesStage.SalesStageName);
                        $tr.attr("data-current-sales-percentage", salesStage.StagePercentage);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewSalesStage").val("");
                        $("#txtSalesStagePercentage").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=sales-stages";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) { }
        });
    };

    this.DeleteSalesStage = function (salesStageId) {
        swal({
            title: translatePhrase("Delete Sales Stage!"),
            text: translatePhrase("Are you sure you want to delete this sales stage?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!"),
            closeOnConfirm: false
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/salesstage/DeleteSalesStage/?salesStageId=" + salesStageId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=sales-stages";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var SalesTeamRoles = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $salesTeamRoles.find("tr").each(function () {
            var $tr = $(this);

            // delete sales team role
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var id = $(this).closest("tr").attr("data-id");
                self.DeleteSalesTeamRole(id);
            });

            // change sales team role
            $tr.find(".sales-team-role").unbind("blur").blur(function () {
                lib.saveOrResetField("sales-team-role", $(this), self.SaveSalesTeamRole);
            });
        });

        // new sales team role
        $("#txtNewSalesTeamRole").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddSalesTeamRole").unbind("click").click(function () {
            self.AddNew();
        });
    };

    this.AddNew = function () {
        var isError = false;
        if ($("#txtNewSalesTeamRole").val() === "") {
            $("#txtNewSalesTeamRole").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveSalesTeamRole(null, true);
    };

    this.SaveSalesTeamRole = function ($tr, newSalesTeamRole) {
        var salesTeamRole = new Object();
        salesTeamRole.SubscriberId = subscriberId;
        salesTeamRole.UpdateUserId = userId;
        if (newSalesTeamRole) {
            salesTeamRole.SalesTeamRoleId = 0;
            salesTeamRole.SalesTeamRole1 = $("#txtNewSalesTeamRole").val();
        } else {
            salesTeamRole.SalesTeamRoleId = $tr.attr("data-id");
            salesTeamRole.SalesTeamRole1 = $tr.find(".sales-team-role").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/salesteamrole/SaveSalesTeamRole",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(salesTeamRole),
            success: function (id) {
                if (parseInt(id) > 0) {
                    $("#divSpinner").addClass("hide");
                    if (!newSalesTeamRole) {
                        $tr.attr("data-current-sales-team-role", salesTeamRole.SalesTeamRole1);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewSalesTeamRole").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=sales-team-roles";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteSalesTeamRole = function (id) {
        swal({
            title: translatePhrase("Delete Sales Team Role!"),
            text: translatePhrase("Are you sure you want to delete this Sales Team Role?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/salesteamrole/DeleteSalesTeamRole/?id=" + id + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=sales-team-roles";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var Sources = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $sources.find("tr").each(function () {
            var $tr = $(this);

            // delete source
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var sourceId = $(this).closest("tr").attr("data-id");
                self.DeleteSource(sourceId);
            });

            // source name change
            $tr.find(".source-name").unbind("blur").blur(function () {
                lib.saveOrResetField("source-name", $(this), self.SaveSource);
            });
        });

        // new source
        $("#txtNewSource").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddSource").unbind("click").click(function () {
            self.AddNewSource();
        });
    };

    this.AddNewSource = function () {
        var isError = false;

        if ($("#txtNewSource").val() === "") {
            $("#txtSource").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveSource(null, true);
    };

    this.SaveSource = function ($tr, newSource) {
        var source = new Object();
        source.SubscriberId = subscriberId;
        source.UpdateUserId = userId;
        if (newSource) {
            source.SourceId = 0;
            source.SourceName = $("#txtNewSource").val();
        } else {
            source.SourceId = $tr.attr("data-id");
            source.SourceName = $tr.find(".source-name").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/source/SaveSource",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(source),
            success: function (sourceId) {
                $("#divSpinner").addClass("hide");
                if (parseInt(sourceId) > 0) {
                    if (!newSource) {
                        $tr.attr("data-current-source-name", source.SourceName);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewSource").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=sources";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteSource = function (sourceId) {
        swal({
            title: translatePhrase("Delete Source!"),
            text: translatePhrase("Are you sure you want to delete this source?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/source/DeleteSource/?sourceId=" + sourceId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=sources";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var Tags = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $tags.find("tr").each(function () {
            var $tr = $(this);

            // delete tag
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var tagId = $(this).closest("tr").attr("data-id");
                self.DeleteTag(tagId);
            });

            // tag name change
            $tr.find(".tag-name").unbind("blur").blur(function () {
                lib.saveOrResetField("tag", $(this), self.SaveTag);
            });
        });

        // new tag
        $("#txtNewTag").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddTag").unbind("click").click(function () {
            self.AddNewTag();
        });
    };

    this.AddNewTag = function () {
        var isError = false;
        if ($("#txtNewTag").val() === "") {
            $("#txtNewTag").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveTag(null, true);
    };

    this.SaveTag = function ($tr, newTag) {
        var tag = new Object();
        tag.SubscriberId = subscriberId;
        tag.UpdateUserId = userId;
        if (newTag) {
            tag.TagId = 0;
            tag.TagName = $("#txtNewTag").val();
        } else {
            tag.TagId = $tr.attr("data-id");
            tag.TagName = $tr.find(".tag-name").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/tag/SaveTag",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(tag),
            success: function (tagId) {
                $("#divSpinner").addClass("hide");
                if (parseInt(tagId) > 0) {
                    if (!newTag) {
                        $tr.attr("data-current-tag", tag.TagName);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtNewTag").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=tags";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteTag = function (tagId) {
        swal({
            title: translatePhrase("Delete Tag!"),
            text: translatePhrase("Are you sure you want to delete this tag?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/tag/DeleteTag/?tagId=" + tagId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=tags";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


var WonReasons = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        $wonReasons.find("tr").each(function () {
            var $tr = $(this);

            // delete won reason
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var wonReasonId = $(this).closest("tr").attr("data-id");
                self.DeleteWonReason(wonReasonId);
            });

            // won reason change
            $tr.find(".won-reason").unbind("blur").blur(function () {
                lib.saveOrResetField("won-reason", $(this), self.SaveWonReason);
            });
        });

        // new won reason
        $("#txtNewWonReason").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddWonReason").unbind("click").click(function () {

            self.AddWonReason();
        });
    };

    this.AddWonReason = function () {
        var isError = false;
        if ($("#txtnewWonReason").val() === "") {
            $("#txtWonReason").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveWonReason(null, true);
    };

    this.SaveWonReason = function ($tr, newWonReason) {
        var wonReason = new Object();
        wonReason.SubscriberId = subscriberId;
        wonReason.UpdateUserId = userId;
        if (newWonReason) {
            wonReason.wonReasonId = 0;
            wonReason.wonReasonName = $(".txtNewWonReason").val();
        } else {
            wonReason.wonReasonId = $tr.attr("data-id");
            wonReason.wonReasonName = $tr.find(".won-reason").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/WonReason/SaveWonReason",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(wonReason),
            success: function (wonReasonId) {
                if (parseInt(wonReasonId) > 0) {
                    $("#divSpinner").addClass("hide");
                    if (!newWonReason) {
                        $tr.attr("data-current-won-reason", wonReason.wonReason);
                        // higlight row
                        $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                    } else {
                        $("#txtnewWonReason").val("");
                        window.location.href = "/Admin/Settings/Settings.aspx?tab=won-reasons";
                    }
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteWonReason = function (wonReasonId) {
        swal({
            title: translatePhrase("Delete Won Reason!"),
            text: translatePhrase("Are you sure you want to delete this Won Reason?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (value) {
            if (value.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/WonReason/DeleteWonReason/?wonReasonId=" + wonReasonId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            window.location.href = "/Admin/Settings/Settings.aspx?tab=won-reasons";
                        }
                    },
                    beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    },
                    error: function (request) { }
                });
            }
        });
    };
};


// sort
$(function () {
    $("table tbody").sortable({
        placeholder: "highlight",
        handle: ".handle",
        helper: fixHelperModified,
        appendTo: "table tbody",
        revert: 200,
        axis: 'y',
        update: function (event, ui) {
            var url = "";
            // get table
            var $table = $(ui.item).closest("table");

            // get new ids in correct order
            var idArr = [];
            $table.find("tbody>tr").each(function () { idArr.push($(this).attr("data-id")); });
            ids = idArr.join(",");

            // get the AJAX url based on table id
            var tableId = $table.attr("id");
            switch (tableId) {
                case "commodities":
                    url = "/api/commodity/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "company-segments":
                    url = "/api/companysegment/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "company-types":
                    url = "/api/companytype/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "competitors":
                    url = "/api/competitor/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "contact-types":
                    url = "/api/contacttype/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "deal-types":
                    url = "/api/dealtype/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "industries":
                    url = "/api/industry/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "lost-reasons":
                    url = "/api/lostreason/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "sales-stages":
                    url = "/api/salesstage/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "sales-team-roles":
                    url = "/api/salesteamrole/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "sources":
                    url = "/api/source/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "tags":
                    url = "/api/tag/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                case "won-reasons":
                    url = "/api/wonreason/ChangeOrder?ids=" + ids + "&subscriberId=" + subscriberId;
                    break;
                
                default:
            }

            if (url === "") {
                return;
            }
            // change the sort order
            $.ajax({
                type: "GET",
                url: url,
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    // animate bg color
                    $(ui.item).effect("highlight", { color: "#D1EFE9" }, 1000);
                },
                beforeSend: function () { },
                error: function (request) { }
            });
        }
    });
});


var fixHelperModified = function (e, tr) {
    var $originals = tr.children();
    var $helper = tr.clone();
    $helper.children().each(function (index) {
        $(this).width($originals.eq(index).outerWidth());
    });
    return $helper;
};
