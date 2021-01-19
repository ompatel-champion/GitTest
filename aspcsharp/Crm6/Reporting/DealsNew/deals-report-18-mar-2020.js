// set global values
var recordsPerPage = 20;
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
//CurrencyCode
var currencyCode = $("#lblCurrencyCode").text();
var currencySymbol = $("#lblCurrencySymbol").text();
//LanguageCode
//DateFormat
var dateFormat = $("#lblDateFormat").text();
//lblDateFormatMask


var currentPage = 1;
var $divReportContent = $("#divReportContent");
var $divNoItems = $("#divNoItems");
var $deals = $("#tblDealsReport>tbody");
var $datepickers = $("[data-name='datepicker']");
var $tblDealsReport = $("#tblDealsReport");
var datatableIntiated = false;
var selectedStatus = "";
var isFromDashboard = false;

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
        self.ToggleAdvancedFilter();

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
            self.RunReport();
        });

        // advanced filter
        $("#advanced-filter-tab").unbind("click").click(function () {
			var filter = '<div class="row MB20"><div class="col-md-2 col-left-box"><a class="deleteFilter" onclick="deleteFilter(this)">✖</a><select class="select2dd" id="ddlAndOr"><option value="and">And</option><option value="or">OR</option></select></div><div class="col-md-3 col-mid-box"><select class="select2dd" id="ddlServices"><option value="Services">Services</option><option value="Destination Countries">Destination Countries</option></select></div><div class="col-md-3 col-mid-box"><select class="select2dd" id="ddlInclude"><option value="Includes">Includes</option><option value="Does Not Include">Does Not Include</option></select></div><div class="col-md-4 col-right-box"><select class="select2ddMultiple" id="ddlOtherServices" Multiple="true"><option value="Some Services">Some Services</option><option value="Some Other Services">Some Other Services</option><option value="Some Other Services">Some Other Services</option></select></div></div>';
			$('.filterFields').append(filter);
			$(".select2dd").select2();
			$(".select2ddMultiple").select2({ "theme": "classic", minimumResultsForSearch: Infinity });
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

        // Services
        $("#ddlService").change(function () {
            self.PopulateLocationsDropDown("Origin");
            self.PopulateLocationsDropDown("Destination");
        });

        // Origin countries
        $("#ddlOriginCountry").change(function () {
            self.PopulateLocationsDropDown("Origin");
        });

        // Destination countries
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
            //format: "dd-MM-yy",
            // date format based on user's settings
            format: dateFormat,
            autoclose: true
        }).on('changeDate', function () {
        });

        if ($("#divSalesStageButtons").find(".btn-success").html() === ("Active")) {
            $("#ddlSalesStage").closest(".col-md-3").removeClass("hide");
        } else {
            $("#ddlSalesStage").closest(".col-md-3").addClass("hide");

        }

        // status
        $("#divSalesStageButtons").find("button").unbind("click").click(function () {
            $("#divSalesStageButtons").find("button").removeClass("btn-success").addClass("btn-white");
            $(this).removeClass("btn-white").addClass("btn-success");

            if ($(this).html() === "Active") {
                $("#ddlSalesStage").closest(".col-md-3").removeClass("hide");
            } else {
                $("#ddlSalesStage").closest(".col-md-3").addClass("hide");
            }
        });

        // years/monthly
        $("#divYearsVsMonthly").find("button").unbind("click").click(function () {
            $("#divYearsVsMonthly").find("button").removeClass("btn-primary").addClass("btn-white");
            $(this).removeClass("btn-white").addClass("btn-primary");
        });


        $("#divYearsVsMonthly").find("button").removeClass("btn-primary").addClass("btn-white");
        var defaulrShipperFrequency = $("#lblShipmentFrequency").text();
        if (defaulrShipperFrequency === "Per Month") {
            $("#btnMonthly").removeClass("btn-white").addClass("btn-primary");
        } else {
            $("#btnYearly").removeClass("btn-white").addClass("btn-primary");
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
                    $("#divSalesStageButtons").find("button").removeClass("btn-success").addClass("btn-white");
                    $btnSalesstage.removeClass("btn-white").addClass("btn-success");
                }
            } else if (salesstage === "Inactive") {
                $btnSalesstage = $("#divSalesStageButtons").find("button[data-type='" + salesstage + "']");
                if ($btnSalesstage.length > 0) {
                    $("#divSalesStageButtons").find("button").removeClass("btn-success").addClass("btn-white");
                    $btnSalesstage.removeClass("btn-white").addClass("btn-success");
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
        $("#divSalesStageButtons").select2();
        $("#ddlBtw").select2();
        $("#ddlCurrency").select2({placeholder:'United States Dollar ($USD)'});
        $("#ddlSalesStage").select2({  placeholder: { id: "0", text: '' }, allowClear: true });
        $("#ddlDateType").select2({  placeholder: { id: "0", text: translatePhrase("Date Type") } });
        $("#ddlService").select2({  placeholder: { id: "0", text: translatePhrase("Services") } });
        $("#ddlDealType").select2({  placeholder: { id: "0", text: translatePhrase("Deal Types") } });
        $("#ddlIndustry").select2({  placeholder: { id: "0", text: translatePhrase("Industries") } });
        $("#ddlUser").select2({  placeholder: { id: "0", text: translatePhrase("Users") } });
        $("#ddlCampaigns").select2({  placeholder: { id: "0", text: translatePhrase("Campaigns") } });
        $("#ddlLocations").select2({  placeholder: { id: "0", text: translatePhrase("Locations") } });
        $("#ddlDistricts").select2({  placeholder: { id: "0", text: translatePhrase("Districts") } });
        $("#ddlUserCountry").select2({  placeholder: { id: "0", text: translatePhrase("User Countries") } });
        $("#ddlOriginCountry").select2({  placeholder: { id: "0", text: translatePhrase("Origin Countries") } });
        $("#ddlOriginLocation").select2({  placeholder: { id: "0", text: translatePhrase("Origin Locations") } });
        $("#ddlDestinationCountry").select2({  placeholder: { id: "0", text: translatePhrase("Destination Countries") } });
        $("#ddlDestinationLocation").select2({  placeholder: { id: "0", text: translatePhrase("Destination Locations") } });
        $("#ddlShipper").select2({  placeholder: { id: "0", text: translatePhrase("Shipper") }, allowClear: true });
        $("#ddlConsignee").select2({  placeholder: { id: "0", text: translatePhrase("Consignee") }, allowClear: true });
        $("#ddlCustomer").select2({  placeholder: { id: "0", text: translatePhrase("Customer") }, allowClear: true });
    };

    this.PopulateLocationsDropDown = function (originOrDestination) {
        // Services
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
            placeHolderText = translatePhrase("Origin Locations");
            $dropdown = $("#ddlOriginLocation");
        } else {
            placeHolderText = translatePhrase("Destination Locations");
            $dropdown = $("#ddlDestinationLocation");
        }
        // clear items
        //$dropdown.html("");
        //$.each(locations, function (i, location) {
        //    var selectValue = location.SelectValue;
        //    var selectText = location.SelectText;
        //    var o = new Option(selectText, selectValue);
        //    $dropdown.append(o);
        //});

        $dropdown.select2({
            minimumInputLength: 3,
            placeholder: translatePhrase("" + originOrDestination + " Location"),
            
            ajax: {
                url: function (obj) {
                    if (!obj.term) {
                        obj.term = "";
                    }
                    var servicesStr = services.join(",");
                    var countryCodesStr = countryCodes.join(",");
                    return "/api/Report/GetLocations?keyword=" + obj.term + "&services=" + servicesStr + "&countryCodes=" + countryCodesStr;
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
            placeholder: translatePhrase("District"),
            
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
            placeholder: translatePhrase("Locations"),
            
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
        $('#tab-advance-filters').toggle(300);
        // toggle drop icon
        var $ele1 = $('#toggleHintExpandText');
        var $ele2 = $('#toggleHintHideText');
        var $ele3 = $('#advanced-filter-tab');
        if ($ele1.hasClass("hide")) {
            $ele1.removeClass("hide");
            $ele2.addClass("hide");
            $ele3.removeClass("active");
            $('#toggletext').html(translatePhrase("+ Add Filter"));
        } else {
            $ele2.removeClass("hide");
            $ele1.addClass("hide");
            $ele3.addClass("active");
            $('#toggletext').html("+ Add Filter");
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

        // SubscriberId
        filters.SubscriberId = subscriberId;
        // Logged in UserId
        filters.UserId = userId;
        filters.CurrencyCode = currencyCode;
        // Sales Stages
        filters.SalesStages = [];
        filters.IsFromDashboard = isFromDashboard;

        selectedStatus = $("#divSalesStageButtons").find(".btn-success").html();
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
        } if (selectedStatus === 'Inactive') {
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

        // Date Type, Date From, Date To
        filters.DateType = $('#ddlDateType').val(); 
        filters.DateFrom = moment($("#txtDateFrom").datepicker("getDate")).isValid() ? moment($("#txtDateFrom").datepicker("getDate")).format("DD-MM-YY") : null;
        filters.DateTo = moment($("#txtDateTo").datepicker("getDate")).isValid() ? moment($("#txtDateTo").datepicker("getDate")).format("DD-MM-YY") : null;

        // Services
        filters.ServiceTypes = [];
        $("#ddlService :selected").each(function (i, selected) {
            var service = $(selected).val().toLowerCase();
            if (service !== "0") {
                filters.ServiceTypes.push(service);
            }
        });

        // LOCATION CODE
        filters.LocationCodes = [];
        $("#ddlLocations :selected").each(function (i, selected) {
            var location = $(selected).val();
            if (location !== "0" && location !== "") {
                filters.LocationCodes.push(location);
            }
        });

        // DISTRICT CODE
        filters.DistrictCodes = [];
        $("#ddlDistricts :selected").each(function (i, selected) {
            var district = $(selected).val();
            if (district !== "0" && district !== "") {
                filters.DistrictCodes.push(district);
            }
        });

        // USER COUNTRY NAME
        filters.CountryNames = [];
        $("#ddlUserCountry :selected").each(function (i, selected) {
            var country = $(selected).val();
            if (country !== "0" && country !== "") {
                filters.CountryNames.push(country);
            }
        });

        // Deal Types
        filters.DealTypes = [];
        $("#ddlDealType :selected").each(function (i, selected) {
            var dealType = $(selected).val();
            if (dealType !== "0") {
                filters.DealTypes.push(dealType);
            }
        });

        // Industries
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

        

        // Sales Reps UserIds
        filters.UserIds = [];
        $("#ddlUser :selected").each(function (i, selected) {
            var user = $(selected).val();
            if (user !== "0") {
                filters.UserIds.push(user);
            }
        });
         

        // Origin Countries
        filters.OriginCountryCodes = [];
        $("#ddlOriginCountry :selected").each(function (i, selected) {
            var country = $(selected).val();
            if (country !== "0") {
                filters.OriginCountryCodes.push(country);
            }
        });

        // Origin Locations
        filters.OriginLocationCodes = [];
        $("#ddlOriginLocation :selected").each(function (i, selected) {
            var location = $(selected).val();
            if (location !== "0") {
                filters.OriginLocationCodes.push(location);
            }
        });

        // Destination Countries
        filters.DestinationCountryCodes = [];
        $("#ddlDestinationCountry :selected").each(function (i, selected) {
            var country = $(selected).val();
            if (country !== "0") {
                filters.DestinationCountryCodes.push(country);
            }
        });

        // Destination Locations
        filters.DestinationLocationCodes = [];
        $("#ddlDestinationLocation :selected").each(function (i, selected) {
            var location = $(selected).val();
            if (location !== "0") {
                filters.DestinationLocationCodes.push(location);
            }
        });

        // Shipper Name
        //filters.ShipperName = $('#ddlShipper').val();

        //// Consignee Name
        //filters.ConsigneeName = $('#ddlConsignee').val();

        //// Company Name
        //filters.CompanyName = $('#ddlCustomer').val();

        // shipping frequency 
        if ($("#btnMonthly").hasClass("btn-primary"))
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
        $(".total-record-count").html("0 deal(s) found");
         
        // AJAX to retrieve deals
        $.ajax({
            type: "POST",
            url: "/api/Report/GetDealsReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 600000,
            data: JSON.stringify(filters),
            success: function (dealsResponse) {
                isFromDashboard = false;
                // remove loading message 
                swal.close();
                if (dealsResponse.Deals.length > 0) {

                    // bind deals
                    self.BindDeals(dealsResponse.Deals);

                    $(".total-record-count").html(dealsResponse.RecordCount + " deal(s) found");

                    //  var tblHeight = dealsResponse.Deals.length > 10 ? 650 : 400;
                    $divReportContent.removeClass("hide");
                    $("#btnExcel").removeClass("hide");
                    //excel  
                    $("#btnExcel").attr("href", dealsResponse.ExcelUri).attr("download", "deal_report.xlsx");
                    $divNoItems.addClass('hide');
                } else {
                    $divReportContent.addClass('hide');
                    $divNoItems.removeClass('hide');
                }

            },
            beforeSend: function () {
                // add loading 
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
        $trFooterHeader.append($("<td/>", { "colspan": "" + colspan + "", "style": "background-color:#f2f2f2" }));
        $trFooterHeader.append($("<td/>", { "html": "Volumes", "style": "background-color:#f2f2f2" }));
        $trFooterHeader.append($("<td/>", { "html": "Spot Volumes", "style": "background-color:#f2f2f2" }));
        $trFooterHeader.append($("<td/>", { "html": "Revenue", "style": "background-color:#f2f2f2" }));
        $trFooterHeader.append($("<td/>", { "html": "Profit", "style": "background-color:#f2f2f2" }));
        $trFooterHeader.append($("<td/>", { "html": "Spot Revenue", "style": "background-color:#f2f2f2" }));
        $trFooterHeader.append($("<td/>", { "html": "Spot Profit", "style": "background-color:#f2f2f2" }));
        $deals.append($trFooterHeader);



        var $trFooter = $("<tr/>");
        $trFooter.append($("<td/>", { "colspan": "" + colspan + "", "style": "background-color:#f2f2f2" }));

        // volumes
        var strVolumes = "";
        strVolumes += formatNumber(parseInt(totalLBs)) + " LBs <br/>";
        strVolumes += formatNumber(parseInt(totalCBMs)) + " CBMs <br/>";
        strVolumes += formatNumber(parseInt(totalTEUs)) + " TEUs <br/>";
        strVolumes += formatNumber(parseInt(totalKGs)) + " KGs <br/>";
        strVolumes += formatNumber(parseInt(totalTonnes)) + " Tonnes <br/>";
        $trFooter.append($("<td/>", { "html": strVolumes, "style": "background-color:#f2f2f2" }));


        // spot volumes
        var strSpotVolumes = "";
        strSpotVolumes += formatNumber(parseInt(totalSpotLBs)) + " LBs <br/>";
        strSpotVolumes += formatNumber(parseInt(totalSpotCBMs)) + " CBMs <br/>";
        strSpotVolumes += formatNumber(parseInt(totalSpotTEUs)) + " TEUs <br/>";
        strSpotVolumes += formatNumber(parseInt(totalSpotKGs)) + " KGs <br/>";
        strSpotVolumes += formatNumber(parseInt(totalSpotTonnes)) + " Tonnes <br/>";
        $trFooter.append($("<td/>", { "html": strSpotVolumes, "style": "background-color:#f2f2f2" }));

        // revenue and profit
        $trFooter.append($("<td/>", {
            "html": currencyText + " " + formatNumber(parseInt(totalRevenue)),
            "style": "background-color:#f2f2f2"
        }));

        $trFooter.append($("<td/>", {
            "html": currencyText + " " + formatNumber(parseInt(totalProfit)),
            "style": "background-color:#f2f2f2"
        }));

        // spot revenue and profit
        $trFooter.append($("<td/>", {
            "html": currencyText + " " + formatNumber(parseInt(totalSpotRevenue)),
            "style": "background-color:#f2f2f2"
        }));

        $trFooter.append($("<td/>", {
            "html": currencyText + " " + formatNumber(parseInt(totalSpotProfit)),
            "style": "background-color:#f2f2f2"
        }));
        $deals.append($trFooter);


        // calculated volumes
        var $trFooterCalculated = $("<tr/>");
        colspan = 14;
        if (selectedStatus.toLowerCase() === "won" || selectedStatus.toLowerCase() === "lost") {
            colspan = 15;
        }

        $trFooterCalculated.append($("<td/>", { "colspan": "" + colspan + "", "style": "background-color:#f2f2f2" }));

        $trFooterCalculated.append($("<td/>", {
            "html": "Calculated Volumes", "style": "background-color:#f2f2f2"
        }));

        var strCalulatedVolumes = "";
        var totalCalculatedTEus = totalTEUs + (totalCBMs / 35);
        strCalulatedVolumes += formatNumber(parseInt(totalCalculatedTEus)) + " TEUs <br/>";
        var totalCalculatedTonnes = totalTonnes + (totalKGs * 0.001) + (totalLBs * 0.000453592);
        strCalulatedVolumes += formatNumber(parseInt(totalCalculatedTonnes)) + " CBMs <br/>";

        $trFooterCalculated.append($("<td/>", {
            "html": strCalulatedVolumes, "style": "background-color:#f2f2f2"
        }));


        var strCalulatedSpotVolumes = "";
        var totalCalculatedSpotTEus = totalSpotTEUs + (totalSpotCBMs / 35);
        strCalulatedSpotVolumes += formatNumber(parseInt(totalCalculatedSpotTEus)) + " TEUs <br/>";
        var totalCalculatedSpotTonnes = totalSpotTonnes + (totalSpotKGs * 0.001) + (totalSpotLBs * 0.000453592);
        strCalulatedSpotVolumes += formatNumber(parseInt(totalCalculatedSpotTonnes)) + " CBMs <br/>";

        $trFooterCalculated.append($("<td/>", {
            "html": strCalulatedSpotVolumes, "style": "background-color:#f2f2f2"
        }));

        $trFooterCalculated.append($("<td/>", { "style": "background-color:#f2f2f2" }));
        $trFooterCalculated.append($("<td/>", { "style": "background-color:#f2f2f2" }));
        $trFooterCalculated.append($("<td/>", { "style": "background-color:#f2f2f2" }));
        $trFooterCalculated.append($("<td/>", { "style": "background-color:#f2f2f2" }));
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

        $tr.find("[data-name='date-type']").html(reportDate);

        $tr.find("[data-name='industry']").html(deal.Industry);
        $tr.find("[data-name='origins']").html(deal.Origins);
        $tr.find("[data-name='origins-countries']").html(deal.OriginCountries);
        $tr.find("[data-name='destinations']").html(deal.Destinations);
        $tr.find("[data-name='destination-countries']").html(deal.DestinationCountries);
        $tr.find("[data-name='shippers']").html(deal.ShipperNames);
        $tr.find("[data-name='last-update']").html(deal.DateUpdated);
        $tr.find("[data-name='last-activity-date']").html(deal.LastActivityDate);
        $tr.find("[data-name='next-activity-date']").html(deal.NextActivityDate);
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

function deleteFilter(field){
	$(field).parent('.col-md-2').parent('.row').remove();
}