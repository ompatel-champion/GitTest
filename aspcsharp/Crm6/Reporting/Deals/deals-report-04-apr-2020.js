// set global values
var recordsPerPage = 20;
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var currencyCode = $("#lblCurrencyCode").text();
var currencySymbol = $("#lblCurrencySymbol").text();
//LanguageCode
//var dateFormat = $("#lblDateFormat").text();

var currentPage = 1;
var $divReportContent = $("#divReportContent");
var $divNoItems = $("#divNoItems");
var $deals = $("#tblDealsReport>tbody");
var $datepickers = $("[data-name='datepicker']");
var $tblDealsReport = $("#tblDealsReport");
var datatableIntiated = false;
var selectedStatus = "";
var isFromDashboard = false;
var $ddlCompetitors = $("#ddlCompetitors");
var isSpotDealReport = $("#lblIsSpotDealReports").text() === "1";

$(function () {
    // init report
    new Report().Init();
});


var Report = function () {
    var self = this;

    this.Init = function () {

        // init select2 dropdowns
        self.InitSelect2DropDowns();

        // toggle advanced filter
        // self.ToggleAdvancedFilter();

        // fix for form controls inside dropdown menu
        $(".dropdown-menu").click(function (e) {
            e.stopPropagation();
        });

        // run report action
        $("#btnRunReport").unbind("click").click(function (e) {
            e.preventDefault();
            self.RunReport();
        });

        // print
        $("#btnPrint").unbind("click").click(function () {
            // TODO: new print function
            self.RunReport();
        });

        // advanced filter
        $("#advanced-filter-tab").unbind("click").click(function () {
            self.ToggleAdvancedFilter();
        });

        // currency change
        $("#ddlCurrency").change(function () {
            var currency = $("#ddlCurrency").val();
            if (currency === "") {
                return;
            }
            var aCurrency = currency.split("|");
            currencyCode = aCurrency[0];
            currencySymbol = aCurrency[1];
            $("#lblCurrencyCode").text(currencyCode);
            $("#lblCurrencySymbol").text(currencySymbol);
            $(".currency-text").html(currencyCode + currencySymbol);
        });

        // date type change
        $("#ddlDateType").change(function () {
            var dateType = $("#ddlDateType").val();
            var dateHeader = "";
            switch (dateType) {
                case "DecisionDate":
                    dateHeader = translatePhrase("Decision Date");
                    break;
                case "FirstShipment":
                    dateHeader = translatePhrase("First Shipment");
                    break;
                case "ContractEnd":
                    dateHeader = translatePhrase("Contract End");
                    break;
                case "CreatedDate":
                    dateHeader = translatePhrase("Created Date");
                    break;
                case "DateLost":
                    dateHeader = translatePhrase("Date Lost");
                    break;
                case "DateWon":
                    dateHeader = translatePhrase("Date Won");
                    break;
                case "UpdatedDate":
                    dateHeader = translatePhrase("Updated Date");
                    break;
            }
            $('.date-type').html(dateHeader);
        });

        self.PopulateLocationsDropDown("Origin");
        self.PopulateLocationsDropDown("Destination");

        // services
        $("#ddlService").change(function () {
            self.PopulateLocationsDropDown("Origin");
            self.PopulateLocationsDropDown("Destination");
        });

        // origin countries
        $("#ddlOriginCountry").change(function () {
            self.PopulateLocationsDropDown("Origin");
        });

        // destination countries
        $("#ddlDestinationCountry").change(function () {
            self.PopulateLocationsDropDown("Destination");
        });

        // user country/location/district dropdowns
        self.PopulateDistrictDropdrown();
        self.PopulateUserLocationDropdrown();
        $("#ddlUserCountry").change(function () {
            self.PopulateUserLocationDropdrown();
            self.PopulateDistrictDropdrown();
        });
        $("#ddlDistricts").change(function () {
            self.PopulateUserLocationDropdrown();
        });

        // set language
        self.SetLanguage();

        // initialize date pickers
        $datepickers.datepicker({ 
            dateFormat: "dd MM, yy",
            autoclose: true
        }).on('changeDate', function () {
        });

        $datepickers.on('focus', function (e) {
            e.preventDefault();
            $(this).attr("autocomplete", "off");
        });

        if ($("#divSalesStageButtons").find(".active").html() === ("Active")) {
            $("#ddlSalesStage").closest(".col-md-3").removeClass("hide");
        } else {
            $("#ddlSalesStage").closest(".col-md-3").addClass("hide");
        }

        // status
        $("#divSalesStageButtons").find("button").unbind("click").click(function () {
            $("#divSalesStageButtons").find("button").removeClass("active");
            $(this).addClass("active");

            if ($(this).html() === "Active") {
                $("#ddlSalesStage").closest(".col-md-3").removeClass("hide");
            } else {
                $("#ddlSalesStage").closest(".col-md-3").addClass("hide");
            }
        });

        // years/monthly
        $("#divYearsVsMonthly").find("button").unbind("click").click(function () {
            $("#divYearsVsMonthly").find("button").removeClass("active");
            $(this).addClass("active");
        });

        //$("#divYearsVsMonthly").find("button").addClass("active");

        var defaulrShipperFrequency = $("#lblShipmentFrequency").text();
        if (defaulrShipperFrequency === "Per Month") {
            $("#btnMonthly").addClass("active");
        } else {
            $("#btnYearly").addClass("active");
        }

        self.InitQuerystringFilters();

    };

    this.InitQuerystringFilters = function () {
        var runSearch = false;
        // $("#btnMonthly").removeClass("btn-white").addClass("btn-primary");
        // $("#btnYearly").removeClass("btn-primary").addClass("btn-white");

        var showAdvancedFilters = false;

        // sales stage
        var salesstage = getQueryString("salesstage");
        if (salesstage && salesstage !== '') {
            runSearch = true;
            if (salesstage === "Won" || salesstage === "Lost" || salesstage === "Stalled") {
                var $btnSalesstage = $("#divSalesStageButtons").find("button[data-type='" + salesstage + "']");
                if ($btnSalesstage.length > 0) {
                    $("#divSalesStageButtons").find("button").removeClass("active");
                    $btnSalesstage.addClass("active");
                }
            } else if (salesstage === "Inactive") {
                $btnSalesstage = $("#divSalesStageButtons").find("button[data-type='" + salesstage + "']");
                if ($btnSalesstage.length > 0) {
                    $("#divSalesStageButtons").find("button").removeClass("active");
                    $btnSalesstage.addClass("active");
                }
            } else {
                $("#ddlSalesStage").val(salesstage);
                $("#ddlSalesStage").trigger("change");
            }
        }

        // user ids
        var qsUserIds = getQueryString("userIds");
        if (qsUserIds && qsUserIds !== '') {
            runSearch = true;
            $("#ddlUser").val(qsUserIds);
            $("#ddlUser").trigger("change");
            showAdvancedFilters = true;
        }

        // location code
        var locationcode = getQueryString("locationcode");
        if (locationcode && locationcode !== '') {
            runSearch = true;
            $("#ddlLocations").val(locationcode);
            $("#ddlLocations").trigger("change");
            showAdvancedFilters = true;
        }

        // industry
        var industry = decodeURIComponent(getQueryString("industry"));
        if (industry && industry !== '') {
            runSearch = true;
            $("#ddlIndustry").val(industry);
            $("#ddlIndustry").trigger("change");
            showAdvancedFilters = true;
        }

        // district code
        var districtCode = getQueryString("districtCode");
        if (districtCode && districtCode !== '') {
            runSearch = true;
            $("#ddlDistricts").val(districtCode);
            $("#ddlDistricts").trigger("change");
            showAdvancedFilters = true;
        }

        // country code
        var countryName = getQueryString("countryName");
        if (countryName && countryName !== '') {
            runSearch = true;
            $("#ddlUserCountry").val(countryName);
            $("#ddlUserCountry").trigger("change");
            showAdvancedFilters = true;
        }

        // date type
        var dttype = getQueryString("dtType");
        if (dttype && dttype !== '') {
            runSearch = true;
            $("#ddlDateType").val(dttype);
            $("#ddlDateType").trigger("change");
            showAdvancedFilters = true;
        }

        // date from
        var dtFrom = getQueryString("dtFrom");
        if (dtFrom && dtFrom !== '') {
            runSearch = true;
            $("#txtDateFrom").datepicker("update", moment(dtFrom).toDate());
            showAdvancedFilters = true;
        }

        // date to
        var dtTo = getQueryString("dtTo");
        if (dtTo && dtTo !== '') {
            runSearch = true;
            $("#txtDateTo").datepicker("update", moment(dtTo).toDate());
            showAdvancedFilters = true;
        }

        // service
        var service = getQueryString("service");
        if (service && service !== '') {
            runSearch = true;
            if (service === "Ocean") {
                var vals = [];
                vals[0] = "Ocean LCL";
                vals[1] = "Ocean FCL";
                // vals[2] = "Logistics";
                //vals[3] = "Warehouse";
                $("#ddlService").select2('val', vals);
                $("#ddlService").trigger("change");
            } else {
                $("#ddlService").val(service);
                $("#ddlService").trigger("change");
            }
            showAdvancedFilters = true;
        }

        if (showAdvancedFilters) {
            self.ToggleAdvancedFilter();
        }

        if (runSearch) {
            isFromDashboard = true;
            self.RunReport();
        }
    };


    this.RunReport = function () {
        // retrieve deals
        currentPage = 1;
        new Deals().RetrieveDeals();
    };

    this.PrintReport = function () {
        // print report function goes here
    };

    this.SetLanguage = function () {

    };

    this.InitSelect2DropDowns = function () {
        $("#ddlCurrency").select2({ });
        $("#ddlSalesStage").select2({  minimumResultsForSearch: Infinity, placeholder: { id: "0", text: translatePhrase("") }, allowClear: true });
        $("#ddlDateType").select2({ placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlService").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlDealType").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlIndustry").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlUser").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlCampaigns").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlLocations").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlDistricts").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlUserCountry").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlOriginCountry").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlOriginLocation").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlDestinationCountry").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlDestinationLocation").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlShipper").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") }, allowClear: true });
        $("#ddlConsignee").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") }, allowClear: true });
        $("#ddlCustomer").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") }, allowClear: true });

        $ddlCompetitors.select2({
            tags: true,
            "theme": "classic",
            placeholder: { id: "0", text: translatePhrase("") },
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

    };

    this.PopulateLocationsDropDown = function (originOrDestination) {
        // services
        var services = [];
        $("#ddlService :selected").each(function (i, selected) {
            var service = $(selected).val();
            if (service !== "0") {
                services.push(service);
            }
        });
        var countryCodes = [];
        if (originOrDestination === "Origin") {
            $("#ddlOriginCountry :selected").each(function (i, selected) {
                var country = $(selected).val();
                if (country !== "0") {
                    countryCodes.push(country);
                }
            });
        } else if (originOrDestination === "Destination") {
            $("#ddlDestinationCountry :selected").each(function (i, selected) {
                var country = $(selected).val();
                if (country !== "0") {
                    countryCodes.push(country);
                }
            });
        }

        var $dropdown;
        if (originOrDestination === "Origin") {
            // placeHolderText = translatePhrase("Origin Locations");
            $dropdown = $("#ddlOriginLocation");
        } else {
            //  placeHolderText = translatePhrase("Destination Locations");
            $dropdown = $("#ddlDestinationLocation");
        }

        $dropdown.select2({
            minimumInputLength: 3,
            placeholder: "", //translatePhrase("" + originOrDestination + " Location"),
            theme: "classic",
            ajax: {
                url: function (obj) {
                    if (!obj.term) {
                        obj.term = "";
                    }
                    var servicesStr = services.join(",");
                    var countryCodesStr = countryCodes.join(",");
                    return "/api/Report/GetLocations?keyword=" + obj.term + "&services=" + servicesStr + "&countryCodes=" + countryCodesStr + "&subscriberId=" + subscriberId;
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
    };

    this.PopulateDistrictDropdrown = function () {
        var countryNames = [];
        $("#ddlUserCountry :selected").each(function (i, selected) {
            var country = $(selected).val();
            if (country !== "0") {
                countryNames.push(country);
            }
        });

        $("#ddlDistricts").select2({
            minimumInputLength: 0,
            placeholder: translatePhrase(""),
            theme: "classic",
            ajax: {
                url: function (obj) {
                    var countryNameStr = countryNames.join(",");
                    return "/api/Dropdown/GetDistricts?subscriberId=" + subscriberId + "&userId=" + userId + "&countryNames=" + countryNameStr;
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
    };

    this.PopulateUserLocationDropdrown = function () {
        var countryNames = [];
        $("#ddlUserCountry :selected").each(function (i, selected) {
            var country = $(selected).val();
            if (country !== "0") {
                countryNames.push(country);
            }
        });

        var districts = [];
        $("#ddlDistricts :selected").each(function (i, selected) {
            var district = $(selected).val();
            if (district !== "0") {
                districts.push(district);
            }
        });

        $("#ddlLocations").select2({
            minimumInputLength: 0,
            placeholder: translatePhrase(""),
            theme: "classic",
            ajax: {
                url: function (obj) {
                    if (!obj.term) {
                        obj.term = "";
                    }
                    var countryNameStr = countryNames.join(",");
                    return "/api/Dropdown/GetLocations?subscriberId=" + subscriberId + "&userId=" + userId
                        + "&countryNames=" + countryNameStr + "&districtCodes=" + districts + "&keyword=" + obj.term;
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
    };

    this.ToggleAdvancedFilter = function () {
        // toggle icon
        $('#tab-advance-filters').toggleClass("hide");
        // toggle drop icon
        var $ele1 = $('#toggleHintExpandText');
        var $ele2 = $('#toggleHintHideText');
        var $ele3 = $('#advanced-filter-tab');
        if ($ele1.hasClass("hide")) {
            $ele1.removeClass("hide");
            $ele2.addClass("hide");
            $ele3.removeClass("active");
            $('#toggletext').html(translatePhrase("Advanced Filters"));
        } else {
            $ele2.removeClass("hide");
            $ele1.addClass("hide");
            $ele3.addClass("active");
            $('#toggletext').html("Advanced Filters");
        }
        return;
    };
};


var Deals = function () {
    var self = this;

    // get deals filter
    this.GetFilter = function () {
        // set the filters
        var filters = new Object();
        filters.RecordsPerPage = recordsPerPage;
        filters.Keyword = "";
        filters.CurrentPage = currentPage;
        filters.SortBy = "companyname asc";

        // subscriberId
        filters.SubscriberId = subscriberId;
        // logged in userId
        filters.UserId = userId;
        filters.CurrencyCode = currencyCode;
        // Sales Stages
        filters.SalesStages = [];
        filters.IsFromDashboard = isFromDashboard;

        // competitors
        filters.Competitors = [];
        if ($("#ddlCompetitors").val() !== "") {
            filters.Competitors = $("#ddlCompetitors").val();
        }

        selectedStatus = $("#divSalesStageButtons").find(".active").html();
        if (selectedStatus === 'Active') {
            var salesStage = $("#ddlSalesStage").val();
            if (salesStage !== null && salesStage !== "0") {
                filters.SalesStages.push(salesStage);
            }
            if (filters.SalesStages.length === 0) {
                $("#ddlSalesStage option").each(function (i, selected) {
                    var salesStage = $(selected).val();
                    if (salesStage !== "0") {
                        // get all the active deal stages
                        filters.SalesStages.push(salesStage);
                    }
                });
            }
            $("#th-wonlostreason").addClass("hide");
        }

        if (selectedStatus === 'Inactive') {
            filters.SalesStages.push("Won");
            filters.SalesStages.push("Lost");
            filters.SalesStages.push("Stalled");
        } else {
            if (selectedStatus.toLowerCase() === "won" || selectedStatus.toLowerCase() === "lost") {
                $("#th-wonlostreason").removeClass("hide");
            } else {
                $("#th-wonlostreason").addClass("hide");
            }
            filters.SalesStages.push(selectedStatus);
        }

        // spot vs normal deals - show hide table columns
        if (isSpotDealReport) {
            $("#th-spot-volume").removeClass("hide");
            $("#th-spot-revenue").removeClass("hide");
            $("#th-spot-profit").removeClass("hide");
            $("#th-volume").addClass("hide");
            $("#th-revenue").addClass("hide");
            $("#th-profit").addClass("hide");
        } else {
            $("#th-spot-volume").addClass("hide");
            $("#th-spot-revenue").addClass("hide");
            $("#th-spot-profit").addClass("hide");
            $("#th-volume").removeClass("hide");
            $("#th-revenue").removeClass("hide");
            $("#th-profit").removeClass("hide");
        }

        // date type, date from, date to
        filters.DateType = $('#ddlDateType').val();
        filters.DateFrom = moment($("#txtDateFrom").datepicker("getDate")).isValid() ? moment($("#txtDateFrom").datepicker("getDate")).format("DD-MM-YY") : null;
        filters.DateTo = moment($("#txtDateTo").datepicker("getDate")).isValid() ? moment($("#txtDateTo").datepicker("getDate")).format("DD-MM-YY") : null;

        // services
        filters.ServiceTypes = [];
        $("#ddlService :selected").each(function (i, selected) {
            var service = $(selected).val().toLowerCase();
            if (service !== "0") {
                filters.ServiceTypes.push(service);
            }
        });

        // location code
        filters.LocationCodes = [];
        $("#ddlLocations :selected").each(function (i, selected) {
            var location = $(selected).val();
            if (location !== "0" && location !== "") {
                filters.LocationCodes.push(location);
            }
        });

        // district code
        filters.DistrictCodes = [];
        $("#ddlDistricts :selected").each(function (i, selected) {
            var district = $(selected).val();
            if (district !== "0" && district !== "") {
                filters.DistrictCodes.push(district);
            }
        });

        // user country name
        filters.CountryNames = [];
        $("#ddlUserCountry :selected").each(function (i, selected) {
            var country = $(selected).val();
            if (country !== "0" && country !== "") {
                filters.CountryNames.push(country);
            }
        });

        // deal types
        filters.DealTypes = [];
        $("#ddlDealType :selected").each(function (i, selected) {
            var dealType = $(selected).val();
            if (dealType !== "0") {
                filters.DealTypes.push(dealType);
            }
        });

        // industries
        filters.Industries = [];
        $("#ddlIndustry :selected").each(function (i, selected) {
            var industry = $(selected).val();
            if (industry !== "0") {
                filters.Industries.push(industry);
            }
        });

        filters.Campaigns = [];
        $("#ddlCampaigns :selected").each(function (i, selected) {
            var campaign = $(selected).val();
            if (campaign !== "0") {
                filters.Campaigns.push(campaign.toLowerCase());
            }
        });

        // sales reps userIds
        filters.UserIds = [];
        $("#ddlUser :selected").each(function (i, selected) {
            var user = $(selected).val();
            if (user !== "0") {
                filters.UserIds.push(user);
            }
        });

        // origin countries
        filters.OriginCountries = [];
        $("#ddlOriginCountry :selected").each(function (i, selected) {
            var country = $(selected).text();
            if (country !== "0") {
                filters.OriginCountries.push(country);
            }
        });

        // origin locations
        filters.OriginLocations = [];
        $("#ddlOriginLocation :selected").each(function (i, selected) {
            var location = $(selected).text();
            if (location !== "0") {
                if (location.indexOf("-" > -1) && location.split("-").length > 1 && location.split("-")[1] !== "") {
                    filters.OriginLocations.push(location.split("-")[1]);
                } else
                    filters.OriginLocations.push(location);
            }
        });

        // destination countries
        filters.DestinationCountries = [];
        $("#ddlDestinationCountry :selected").each(function (i, selected) {
            var country = $(selected).text();
            if (country !== "0" || country !== "") {
                filters.DestinationCountries.push(country);
            }
        });

        // destination locations
        filters.DestinationLocations = [];
        $("#ddlDestinationLocation :selected").each(function (i, selected) {
            var location = $(selected).text();
            if (location !== "0") {
                if (location !== "0") {
                    if (location.indexOf("-" > -1) && location.split("-").length > 1 && location.split("-")[1] !== "") {
                        filters.DestinationLocations.push(location.split("-")[1]);
                    } else
                        filters.DestinationLocations.push(location);
                }
                //   filters.DestinationLocationCodes.push(location);
            }
        });

        filters.IsSpotDeals = isSpotDealReport;

        // shipper name
        //filters.ShipperName = $('#ddlShipper').val();

        // consignee name
        //filters.ConsigneeName = $('#ddlConsignee').val();

        // company name
        //filters.CompanyName = $('#ddlCustomer').val();

        // shipping frequency
        if ($("#btnMonthly").hasClass("active"))
            filters.ShippingFrquency = "Per Month";
        else
            filters.ShippingFrquency = "Per Year";

        return filters;
    };

    var reportItemHtml = "";

    // retrieve deals
    this.RetrieveDeals = function () {
        // set the filters
        var filters = self.GetFilter();
        $divReportContent.addClass('hide');
        $deals.html("");
        $("#btnExcel").addClass("hide");
        $(".total-record-count").html("");

        // ajax to retrieve deals
        $.ajax({
            type: "POST",
            url: "/api/Report/GetDealsReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 600000,
            data: JSON.stringify(filters),
            success: function (dealsResponse) {
                isFromDashboard = false;
                swal.close();
                if (dealsResponse.Deals.length > 0) {

                    self.BindDeals(dealsResponse.Deals);

                    $(".total-record-count").html('<span>'+dealsResponse.RecordCount + " deals found"+'</span>');

                    //  var tblHeight = dealsResponse.Deals.length > 10 ? 650 : 400;
                    $divReportContent.removeClass("hide");

                    // excel
                    $("#btnExcel").removeClass("hide");
                    $("#btnExcel").attr("href", dealsResponse.ExcelUri).attr("download", "deal_report.xlsx");
                    $divNoItems.addClass('hide');
                } else {
                    $divReportContent.addClass('hide');
                    $divNoItems.removeClass('hide');
                }

            },
            beforeSend: function () {
                swal({
                    text: translatePhrase("Generating Report") + "...",
                    title: "<img src='/_content/_img/loading_40.gif'/>",
                    showConfirmButton: false,
                    allowOutsideClick: false
                });
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    // bind deals
    this.BindDeals = function (deals) {
        var totalRevenue = 0.0;
        var totalProfit = 0.0;
        var totalSpotRevenue = 0.0;
        var totalSpotProfit = 0.0;
        var totalLBs = 0.0;
        var totalCBMs = 0.0;
        var totalTEUs = 0.0;
        var totalKGs = 0.0;
        var totalTonnes = 0.0;
        var totalSpotLBs = 0.0;
        var totalSpotCBMs = 0.0;
        var totalSpotTEUs = 0.0;
        var totalSpotKGs = 0.0;
        var totalSpotTonnes = 0.0;

        // get template for deal row
        reportItemHtml = self.GetReportRowHtml();
        // iterate and bind deals
        $.each(deals, function (i, deal) {
            var $tr = self.GetDealItemHtml(deal);
            $deals.append($tr);
            // set revenue and profits
            totalRevenue += parseInt(deal.Revenue);
            totalProfit += parseInt(deal.Profit);
            totalSpotRevenue += parseInt(deal.SpotRevenue);
            totalSpotProfit += parseInt(deal.SpotProfit);
            // total volumes
            totalLBs += deal.LBs;
            totalCBMs += deal.CBMs;
            totalTEUs += deal.TEUs;
            totalKGs += deal.KGs;
            totalTonnes += deal.Tonnes;
            // total spot volumes
            totalSpotLBs += deal.SpotLBs;
            totalSpotCBMs += deal.SpotCBMs;
            totalSpotTEUs += deal.SpotTEUs;
            totalSpotKGs += deal.SpotKGs;
            totalSpotTonnes += deal.SpotTonnes;
        });

        var currency = $("#ddlCurrency").val();
        if (currency === "") {
            return;
        }
        if (currency !== null) {
            var aCurrency = currency.split("|");
            currencyCode = aCurrency[0];
            currencySymbol = aCurrency[1];
        }
        var currencyText = currencyCode + (currencySymbol && currencySymbol !== '' ? ("(" + currencySymbol + ")") : "");

        var $trFooterHeader = $("<tr/>");
        var colspan = 15;
        if (selectedStatus.toLowerCase() === "won" || selectedStatus.toLowerCase() === "lost") {
            colspan = 16;
        }

        $trFooterHeader.append($("<td/>", { "colspan": "" + colspan + "" }));

        // volumes
        var $tdVolume = $("<td/>", { "html": "Volumes" });
        if (isSpotDealReport) {
            $tdVolume.addClass("hide");
        }
        $trFooterHeader.append($tdVolume);

        var $tdSpotVolume = $("<td/>", { "html": "Spot Volumes" });
        if (!isSpotDealReport) {
            $tdSpotVolume.addClass("hide");
        }
        $trFooterHeader.append($tdSpotVolume);

        // revenue and profit
        var $tdRevenue = $("<td/>", { "html": "Revenue" });
        var $tdProfit = $("<td/>", { "html": "Profit" });
        if (isSpotDealReport) {
            $tdRevenue.addClass("hide");
            $tdProfit.addClass("hide");
        }
        $trFooterHeader.append($tdRevenue);
        $trFooterHeader.append($tdProfit);

        // spot revenue and profit
        var $tdSpotRevenue = $("<td/>", { "html": "Spot Revenue" });
        var $tdSpotProfit = $("<td/>", { "html": "Spot Profit" });
        if (!isSpotDealReport) {
            $tdSpotRevenue.addClass("hide");
            $tdSpotProfit.addClass("hide");
        }
        $trFooterHeader.append($tdSpotRevenue);
        $trFooterHeader.append($tdSpotProfit);

        $deals.append($trFooterHeader);

        var $trFooter = $("<tr/>");
        $trFooter.append($("<td/>", { "colspan": "" + colspan + "" }));

        // volumes
        var strVolumes = "";
        var $tdVolumeValues = $("<td/>");
       // strVolumes += formatNumber(parseInt(totalLBs)) + " LBs <br/>";
        strVolumes += formatNumber(parseInt(totalCBMs)) + " CBMs <br/>";
        strVolumes += formatNumber(parseInt(totalTEUs)) + " TEUs <br/>";
        strVolumes += formatNumber(parseInt(totalKGs)) + " KGs <br/>";
        strVolumes += formatNumber(parseInt(totalTonnes)) + " Tonnes <br/>";
        $tdVolumeValues.html(strVolumes);
        if (isSpotDealReport) {
            $tdVolumeValues.addClass("hide");
        }
        $trFooter.append($tdVolumeValues);

        // spot volumes
        var strSpotVolumes = "";
        var $tdSpotVolumeValues = $("<td/>");
       // strSpotVolumes += formatNumber(parseInt(totalSpotLBs)) + " LBs <br/>";
        strSpotVolumes += formatNumber(parseInt(totalSpotCBMs)) + " CBMs <br/>";
        strSpotVolumes += formatNumber(parseInt(totalSpotTEUs)) + " TEUs <br/>";
        strSpotVolumes += formatNumber(parseInt(totalSpotKGs)) + " KGs <br/>";
        strSpotVolumes += formatNumber(parseInt(totalSpotTonnes)) + " Tonnes <br/>";
        $tdSpotVolumeValues.html(strSpotVolumes);
        if (!isSpotDealReport) {
            $tdSpotVolumeValues.addClass("hide");
        }
        $trFooter.append($tdSpotVolumeValues);

        // revenue and profit
        var $tdTotalRevenue = $("<td/>", {
            "html": currencyText + " " + formatNumber(parseInt(totalRevenue))
        });
        var $tdTotalProfit = $("<td/>", {
            "html": currencyText + " " + formatNumber(parseInt(totalProfit))
        });
        if (isSpotDealReport) {
            $tdTotalRevenue.addClass("hide");
            $tdTotalProfit.addClass("hide");
        }
        $trFooter.append($tdTotalRevenue);
        $trFooter.append($tdTotalProfit);

        // spot revenue and profit
        var $tdTotalSpotRevenue = $("<td/>", {
            "html": currencyText + " " + formatNumber(parseInt(totalSpotRevenue))
        });
        var $tdTotalSpotProfit = $("<td/>", {
            "html": currencyText + " " + formatNumber(parseInt(totalSpotProfit))
        });
        if (!isSpotDealReport) {
            $tdTotalSpotRevenue.addClass("hide");
            $tdTotalSpotProfit.addClass("hide");
        }
        $trFooter.append($tdTotalSpotRevenue);
        $trFooter.append($tdTotalSpotProfit);

        $deals.append($trFooter);

        // calculated volumes
        var $trFooterCalculated = $("<tr/>");
        colspan = 14;
        if (selectedStatus.toLowerCase() === "won" || selectedStatus.toLowerCase() === "lost") {
            colspan = 15;
        }
        $trFooterCalculated.append($("<td/>", { "colspan": "" + colspan + "" }));

        $trFooterCalculated.append($("<td/>", {
            "html": "Calculated Volumes"
        }));

        // calculated volumes
        var strCalulatedVolumes = "";
        var totalCalculatedTEus = totalTEUs + (totalCBMs / 35);
        strCalulatedVolumes += formatNumber(parseInt(totalCalculatedTEus)) + " TEUs <br/>";
        var totalCalculatedTonnes = totalTonnes + (totalKGs * 0.001) + (totalLBs * 0.000453592);
        strCalulatedVolumes += formatNumber(parseInt(totalCalculatedTonnes)) + " CBMs <br/>";

        var $tdTotalCalculatedVolumes = $("<td/>", {
            "html": strCalulatedVolumes
        });
        if (isSpotDealReport) {
            $trFooterCalculated.addClass("hide");
        }
        $trFooterCalculated.append($tdTotalCalculatedVolumes);

        // calculated SPOT volumes
        var strCalulatedSpotVolumes = "";
        var totalCalculatedSpotTEus = totalSpotTEUs + (totalSpotCBMs / 35);
        strCalulatedSpotVolumes += formatNumber(parseInt(totalCalculatedSpotTEus)) + " TEUs <br/>";
        var totalCalculatedSpotTonnes = totalSpotTonnes + (totalSpotKGs * 0.001) + (totalSpotLBs * 0.000453592);
        strCalulatedSpotVolumes += formatNumber(parseInt(totalCalculatedSpotTonnes)) + " CBMs <br/>";

        var $tdSpotTotalCalculatedVolumes = $("<td/>", {
            "html": strCalulatedSpotVolumes
        });
        if (!isSpotDealReport) {
            $tdSpotTotalCalculatedVolumes.addClass("hide");
        }
        $trFooterCalculated.append($tdSpotTotalCalculatedVolumes);

        $trFooterCalculated.append($("<td/>"));
        $trFooterCalculated.append($("<td/>"));
        $trFooterCalculated.append($("<td/>", { "class": "hide" }));
        $trFooterCalculated.append($("<td/>", { "class": "hide" }));
        $deals.append($trFooterCalculated);
    };

    // get deal row html
    this.GetDealItemHtml = function (deal) {
        var $tr = $(reportItemHtml);
        $tr.attr("data-id", deal.DealId);
        // bind deal details
        $tr.find("[data-name='company-name']").html(deal.CompanyName);
        // company
        var companyDetailLink = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + deal.CompanyId + "&subscriberId=" + deal.SubscriberId;
        $tr.find("[data-name='company-name']").closest("a").attr("href", companyDetailLink);
        $tr.find("[data-name='company-name']").closest("a").attr("target", "_blank");
        $tr.find("[data-name='deal-name']").html(deal.DealName);
        var dealDetailLink = "/Deals/DealDetail/dealdetail.aspx?dealId=" + deal.DealId + "&dealsubscriberid=" + deal.SubscriberId;
        $tr.find("[data-name='deal-name']").closest("a").attr("href", dealDetailLink);
        $tr.find("[data-name='deal-name']").closest("a").attr("target", "_blank");

        if (selectedStatus.toLowerCase() === "won" || selectedStatus.toLowerCase() === "lost") {
            $tr.find(".td-wonlostreason").removeClass("hide");
            if (deal.ReasonWonLost === 'Reason') {
                deal.ReasonWonLost = '';
            }
            $tr.find(".td-wonlostreason").find("[data-name='wonlostreason']").html(deal.ReasonWonLost);
        }

        // spot vs normal deals - show hide table columns
        if (isSpotDealReport) {
            $tr.find("#td-spot-volume").removeClass("hide");
            $tr.find("#td-spot-revenue").removeClass("hide");
            $tr.find("#td-spot-profit").removeClass("hide");
            $tr.find("#td-volume").addClass("hide");
            $tr.find("#td-revenue").addClass("hide");
            $tr.find("#td-profit").addClass("hide");
        } else {
            $tr.find("#td-spot-volume").addClass("hide");
            $tr.find("#td-spot-revenue").addClass("hide");
            $tr.find("#td-spot-profit").addClass("hide");
            $tr.find("#td-volume").removeClass("hide");
            $tr.find("#td-revenue").removeClass("hide");
            $tr.find("#td-profit").removeClass("hide");
        }

        $tr.find("[data-name='deal-type']").html(deal.DealType);
        $tr.find("[data-name='sales-rep']").html(deal.SalesRepName);
        $tr.find("[data-name='location']").html(deal.LocationName);
        $tr.find("[data-name='sales-stage']").html(deal.SalesStage);

        // get appropriate date field from deal date type
        var dateType = $('#ddlDateType').val();
        var reportDate;

        switch (dateType) {
            case "DecisionDate":
                reportDate = deal.DateDecision;
                break;
            case "FirstShipment":
                reportDate = deal.DateFirstShipment;
                break;
            case "ContractEnd":
                reportDate = deal.DateContractEnd;
                break;
            case "CreatedDate":
                reportDate = deal.DateCreated;
                break;
            case "DateLost":
                reportDate = deal.DateLost;
                break;
            case "DateWon":
                reportDate = deal.DateWon;
                break;
            case "UpdatedDate":
                reportDate = deal.DateUpdated;
                break;
        }
        // format date
        //if (reportDate !== null && reportDate !== "null" && reportDate !== "") {
        //    var date = new Date(reportDate);
        //    reportDate = moment(date).format(dateFormat.toUpperCase());
        //}

        var LastActivityDate = deal.LastActivityDate ? moment(deal.LastActivityDate).format("DD-MMM-YY") : '';
        var NextActivityDate = deal.NextActivityDate ? moment(deal.NextActivityDate).format("DD-MMM-YY") : '';

        $tr.find("[data-name='date-type']").html(reportDate);

        $tr.find("[data-name='industry']").html(deal.Industry);
        $tr.find("[data-name='origins']").html(deal.Origins);
        $tr.find("[data-name='origins-countries']").html(deal.OriginCountries);
        $tr.find("[data-name='destinations']").html(deal.Destinations);
        $tr.find("[data-name='destination-countries']").html(deal.DestinationCountries);
        $tr.find("[data-name='shippers']").html(deal.ShipperNames);
        $tr.find("[data-name='last-update']").html(deal.DateUpdated);
        $tr.find("[data-name='last-activity-date']").html(LastActivityDate);
        $tr.find("[data-name='next-activity-date']").html(NextActivityDate);
        $tr.find("[data-name='updated-by']").html(deal.UpdatedBy);
        $tr.find("[data-name='consignees']").html(deal.ConsigneeNames);
        $tr.find("[data-name='services']").html(deal.Services);
        $tr.find("[data-name='comments']").html(deal.Comments);

        // volumes
        if (deal.LBs > 0) {
            $tr.find("[data-name='lbs']").html(formatNumber(deal.LBs) + " LBs");
        } else {
            $tr.find("[data-name='lbs']").remove();
        }
        if (deal.CBMs > 0) {
            $tr.find("[data-name='cbms']").html(formatNumber(deal.CBMs) + " CBMs");
        } else {
            $tr.find("[data-name='cbms']").remove();
        }
        if (deal.TEUs > 0) {
            $tr.find("[data-name='teus']").html(formatNumber(deal.TEUs) + " TEUs");
        } else {
            $tr.find("[data-name='teus']").remove();
        }
        if (deal.KGs > 0) {
            $tr.find("[data-name='kgs']").html(formatNumber(deal.KGs) + " KGs");
        } else {
            $tr.find("[data-name='kgs']").remove();
        }
        if (deal.Tonnes > 0) {
            $tr.find("[data-name='tonnes']").html(formatNumber(deal.Tonnes) + " Tonnes");
        } else {
            $tr.find("[data-name='tonnes']").remove();
        }

        // spot volumes
        if (deal.SpotLBs > 0) {
            $tr.find("[data-name='spot-lbs']").html(formatNumber(deal.SpotLBs) + " LBs");
        } else {
            $tr.find("[data-name='spot-lbs']").remove();
        }
        if (deal.SpotCBMs > 0) {
            $tr.find("[data-name='spot-cbms']").html(formatNumber(deal.SpotCBMs) + " CBMs");
        } else {
            $tr.find("[data-name='spot-cbms']").remove();
        }
        if (deal.SpotTEUs > 0) {
            $tr.find("[data-name='spot-teus']").html(formatNumber(deal.SpotTEUs) + " TEUs");
        } else {
            $tr.find("[data-name='spot-teus']").remove();
        }
        if (deal.SpotKGs > 0) {
            $tr.find("[data-name='spot-kgs']").html(formatNumber(deal.SpotKGs) + " KGs");
        } else {
            $tr.find("[data-name='spot-kgs']").remove();
        }
        if (deal.SpotTonnes > 0) {
            $tr.find("[data-name='spot-tonnes']").html(formatNumber(deal.SpotTonnes) + " Tonnes");
        } else {
            $tr.find("[data-name='spot-tonnes']").remove();
        }

        $tr.find("[data-name='revenue']").html(formatNumber(parseInt(deal.Revenue)));
        $tr.find("[data-name='spot-revenue']").html(formatNumber(parseInt(deal.SpotRevenue)));
        $tr.find("[data-name='profit']").html(formatNumber(parseInt(deal.Profit)));
        $tr.find("[data-name='spot-profit']").html(formatNumber(parseInt(deal.SpotProfit)));

        $tr.find("[data-name='profit-revenue-percentage']").html(deal.ProfitRevenuePercentage + "%");

        return $tr;
    };

    // set pagination
    this.SetPagination = function (totalPages) {
        var visiblePages = 1;
        if (totalPages > 4) {
            visiblePages = 4;
        } else {
            visiblePages = totalPages;
        }

        $('.pagination').twbsPagination({
            totalPages: totalPages,
            visiblePages: visiblePages,
            initiateStartPageClick: false,
            onPageClick: function (event, page) {
                currentPage = page;
                self.RetrieveDeals();
            },
            first: "<<",
            prev: "<",
            next: ">",
            last: ">>"
        });
        if (totalPages > 1)
            $(".pagination").removeClass('hide');
    };

    // get deal row html template
    this.GetReportRowHtml = function () {
        var html = "";
        $.ajax({
            cache: false,
            async: false,
            url: "/_templates/DealReportItem.html" + "?q=" + $.now(),
            success: function (data) {
                html = data;
            }
        });
        return html;
    };

};

// show the filters
$(document).ready(function () {
    $("#divReportFilter").removeClass("hide");
});
