// set global values
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var userIdGlobal = $("#lblUserIdGlobal").text();
var dateFormat = $("#lblDateFormat").text();
var $divNoItems = $("#divNoItems");
var $kpiDataBody = $("#tblKpisReport>tbody");
var $datepickers = $("[data-name='datepicker']");
var $tblKPIReport = $("#tblKpisReport");
var $divReportContent = $("#divReportContent");
var datatableIntiated = false;


$(function () {
    new Report().Init();
});


var Report = function () {
    var self = this;

    this.Init = function () {

        self.InitSelect2DropDowns();

        // fix for form controls inside drop down menu
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

        // initialize date pickers
        $datepickers.datepicker({
            dateFormat: "dd MM, yy",
            autoclose: true
        }).on('changeDate', function () { });

        $datepickers.on('focus', function (e) {
            e.preventDefault();
            $(this).attr("autocomplete", "off");
        });

        $("#txtDateFrom").datepicker("setDate", moment().add(-1, 'months').toDate());
        $("#txtDateTo").datepicker("setDate", moment().toDate());

    };


    this.InitSelect2DropDowns = function () {
        $("#ddlCountry").select2({ allowClear: true, placeholder: translatePhrase("Select Country") });
        $("#ddlSalesReps").select2({ placeholder: translatePhrase("Select User"), allowClear: true, });


        $("#ddlCountry").on('select2:select', function (evt) {
            $("#ddlLocation").val("").trigger("change");
            $("#ddlCountry").val($('#ddlCountry').val()).trigger("change");
        });

        // location
        $("#ddlLocation").select2({
            allowClear: true,
            placeholder: translatePhrase("Select Location"),
            ajax: {
                url: function (obj) {
                    var country = $("#ddlCountry").val();
                    if (!obj.term) {
                        obj.term = "";
                    }
                    return "/api/Dropdown/GetLocations?subscriberId=" + subscriberId + "&userId=" + userId
                        + "&countryNames=" + country + "&districtCodes=&keyword=" + obj.term;
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


        //$("#ddlLocation").on('select2:select', function (evt) {
        //    var locationCode = $("#ddlLocation").val();
        //    var locationText = $("#ddlLocation option:selected").text();
        //    var country = $("#ddlCountry").val();
            
        //    if (locationCode !== '' && locationText !== '' && country === "") {
        //        // AJAX to get the country

        //        $.ajax({
        //            type: "GET",
        //            url: "/api/Location/GetLocationCountry?locationCode=" + locationCode + "&locationText=" + locationText + "&subscriberId=" + subscriberId,
        //            contentType: "application/json; charset=utf-8",
        //            dataType: "json",
        //            timeout: 300000,
        //            data: '',
        //            success: function (countryName) {
        //                if (countryName !== '') {
        //                    $("#ddlCountry").val(countryName).trigger("change");
        //                }
        //            },
        //            beforeSend: function () {
        //            },
        //            error: function (request) {
        //                alert(JSON.stringify(request));
        //            }
        //        });

        //    }
        //});


    };

    this.RunReport = function () {
        new KpiReport().RetrieveKPIData();
    };
};


var KpiReport = function () {
    var self = this;
    var reportItemHtml = "";

    // get filters
    this.GetFilter = function () {
        // set the filters
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.DateFrom = moment($("#txtDateFrom").datepicker("getDate")).isValid() ? moment($("#txtDateFrom").datepicker("getDate")).format("YYYY-MM-DD 00:00") : null;
        filters.DateTo = moment($("#txtDateTo").datepicker("getDate")).isValid() ? moment($("#txtDateTo").datepicker("getDate")).format("YYYY-MM-DD 23:59") : null;
        filters.CountryName = $("#ddlCountry").val();
        filters.LocationCode = $("#ddlLocation").val();
        filters.UserId = userId;
        filters.SalesRepId = $("#ddlSalesReps").val();
        return filters;
    };

    // retrieve KPI report data
    this.RetrieveKPIData = function () {

        // set the filters
        var filters = self.GetFilter();
        $divReportContent.addClass('hide');
        $kpiDataBody.html("");

        $("#btnExcel").addClass("hide");
        $("#btnPrint").addClass("hide");

        // AJAX to retrieve KPI data
        $.ajax({
            type: "POST",
            url: "/api/Report/GetSalesrepKpiReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 300000,
            data: JSON.stringify(filters),
            success: function (kpiResponse) {
                removeSpinner();
                // bind sales team
                self.BindSalesTeam(kpiResponse.SalesReps);

                // bind excel
                $("#btnExcel").removeClass("hide");
                $("#btnExcel").unbind("click").click(function () {
                    location.href = kpiResponse.ExcelUri;
                });

                // print
                $("#btnPrint").removeClass("hide");
                $("#btnPrint").unbind("click").click(function () {
                    printTable();
                });

                // set the table as a data table
                $divReportContent.removeClass("hide");
                $divNoItems.addClass('hide');
            },
            beforeSend: function () {
                addSpinner();
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };

    // bind sales team
    this.BindSalesTeam = function (salesTeam) {
        // get template for deal row
        reportItemHtml = self.GetReportRowHtml();
        $.each(salesTeam, function (i, salesRep) {
            var $tr = self.GetKpiItemHtml(salesRep);
            $kpiDataBody.append($tr);
            self.BindActions($tr);
        });
    };

    // get KPI row html
    this.GetKpiItemHtml = function (salesRep) {
        var $tr = $(reportItemHtml);
        $tr.attr("data-id", salesRep.SalesRepId);
        $tr.find("[data-name='salesrep']").html(salesRep.SalesRepName);
        $tr.find("[data-name='location']").html(salesRep.Location);
        $tr.find("[data-name='country']").html(salesRep.Country);
        $tr.find("[data-name='meetings']").html(salesRep.Events);
        if (salesRep.Events === 0) {
            $tr.find("[data-name='meetings']").addClass("a-no-data");
        }
        $tr.find("[data-name='notes']").html(salesRep.Notes);
        if (salesRep.Notes === 0) {
            $tr.find("[data-name='notes']").addClass("a-no-data");
        }
        $tr.find("[data-name='tasks']").html(salesRep.Tasks);
        if (salesRep.Tasks === 0) {
            $tr.find("[data-name='tasks']").addClass("a-no-data");
        }
        $tr.find("[data-name='logins']").html(salesRep.Logins);
        if (salesRep.Logins === 0) {
            $tr.find("[data-name='logins']").addClass("a-no-data");
        }
        $tr.find("[data-name='newdeals']").html(salesRep.NewDeals);
        if (salesRep.NewDeals === 0) {
            $tr.find("[data-name='newdeals']").addClass("a-no-data");
        }
        $tr.find("[data-name='wondeals']").html(salesRep.WonDeals);
        if (salesRep.WonDeals === 0) {
            $tr.find("[data-name='wondeals']").addClass("a-no-data");
        }
        $tr.find("[data-name='lostdeals']").html(salesRep.LostDeals);
        if (salesRep.LostDeals === 0) {
            $tr.find("[data-name='lostdeals']").addClass("a-no-data");
        }
        return $tr;
    };

    this.BindActions = function ($tr) {
        $tr.find("[data-name='logins']").unbind("click").click(function () {
            var val = parseInt($tr.find("[data-name='logins']").html());
            if (val > 0) {
                self.OpenLoginsDetailPopup($tr);
            }
        });
        $tr.find("[data-name='meetings']").unbind("click").click(function () {
            var val = parseInt($tr.find("[data-name='meetings']").html());
            if (val > 0) {
                self.OpenMeetingsDetailPopup($tr);
            }
        });
        $tr.find("[data-name='tasks']").unbind("click").click(function () {
            var val = parseInt($tr.find("[data-name='tasks']").html());
            if (val > 0) {
                self.OpenTasksDetailPopup($tr);
            }
        });
        $tr.find("[data-name='newdeals']").unbind("click").click(function () {
            var val = parseInt($tr.find("[data-name='newdeals']").html());
            if (val > 0) {
                self.OpenDealsDetailPopup($tr, 'New');
            }
        });
        $tr.find("[data-name='wondeals']").unbind("click").click(function () {
            var val = parseInt($tr.find("[data-name='wondeals']").html());
            if (val > 0) {
                self.OpenDealsDetailPopup($tr, 'Won');
            }
        });
        $tr.find("[data-name='lostdeals']").unbind("click").click(function () {
            var val = parseInt($tr.find("[data-name='lostdeals']").html());
            if (val > 0) {
                self.OpenDealsDetailPopup($tr, 'Lost');
            }
        });
        $tr.find("[data-name='notes']").unbind("click").click(function () {
            var val = parseInt($tr.find("[data-name='notes']").html());
            if (val > 0) {
                self.OpenNotesDetailPopup($tr);
            }
        });
    };

    this.OpenLoginsDetailPopup = function ($tr) {
        var salesRepId = $tr.attr("data-id");
        var filters = new KpiReport().GetFilter();

        var iframeUrl = "/Reporting/KPIs/DetailViews/Logins.aspx?userId=" + salesRepId;
        if (filters.CountryCode && filters.CountryCode !== "") {
            iframeUrl += "&countrycode=" + filters.CountryCode;
        }
        if (filters.LocationId > 0) {
            iframeUrl += "&locationId=" + (filters.LocationId === null ? 0 : filters.LocationId);
        }

        iframeUrl += "&datefrom=" + (filters.DateFrom === null ? "" : filters.DateFrom) + "&dateto=" + (filters.DateTo === null ? "" : filters.DateTo);

        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframeLogins" }).launchModal({
            title: "Logins - " + $tr.find("[data-name='salesrep']").html(),
            modalClass: "modal-md",
            btnSuccessText: "",
            btnSuccessClass: "primary-btn",
            btnCloseText: "Close",
            btnCloseClass: "secondary-btn",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

    this.OpenMeetingsDetailPopup = function ($tr) {
        var salesRepId = $tr.attr("data-id");
        var filters = new KpiReport().GetFilter();
        var iframeUrl = "/Reporting/KPIs/DetailViews/Events.aspx?userId=" + salesRepId;
        iframeUrl += "&datefrom=" + (filters.DateFrom === null ? "" : filters.DateFrom) +
            "&dateto=" + (filters.DateTo === null ? "" : filters.DateTo);

         $("<div/>", { "class": "modalWrapper", "id": "iframeMeetings" }).launchModal({
            title: "Meetings - " + $tr.find("[data-name='salesrep']").html(),
            modalClass: "modal-md",
            btnSuccessText: "",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

    this.OpenTasksDetailPopup = function ($tr) {
        var salesRepId = $tr.attr("data-id");
        var filters = new KpiReport().GetFilter();
        var iframeUrl = "/Reporting/KPIs/DetailViews/Tasks.aspx?userId=" + salesRepId;
        iframeUrl += "&datefrom=" + (filters.DateFrom === null ? "" : filters.DateFrom) +
            "&dateto=" + (filters.DateTo === null ? "" : filters.DateTo);

        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframeTasks" }).launchModal({
            title: "Tasks - " + $tr.find("[data-name='salesrep']").html(),
            modalClass: "modal-md",
            btnSuccessText: "",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

    this.OpenDealsDetailPopup = function ($tr, status) {
        var salesRepId = $tr.attr("data-id");
        var filters = new KpiReport().GetFilter();
        var iframeUrl = "/Reporting/KPIs/DetailViews/Deals.aspx?userId=" + salesRepId + "&status=" + status;
        iframeUrl += "&datefrom=" + (filters.DateFrom === null ? "" : filters.DateFrom) +
            "&dateto=" + (filters.DateTo === null ? "" : filters.DateTo);

        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframeTasks" }).launchModal({
            title: status + " Deals - " + $tr.find("[data-name='salesrep']").html(),
            modalClass: "modal-md",
            btnSuccessText: "",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

    this.OpenNotesDetailPopup = function ($tr) {
        var salesRepId = $tr.attr("data-id");
        var filters = new KpiReport().GetFilter();
        var iframeUrl = "/Reporting/KPIs/DetailViews/Notes.aspx?userId=" + salesRepId;
        iframeUrl += "&datefrom=" + (filters.DateFrom === null ? "" : filters.DateFrom) +
            "&dateto=" + (filters.DateTo === null ? "" : filters.DateTo);

        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframeNotes" }).launchModal({
            title: "Notes - " + $tr.find("[data-name='salesrep']").html(),
            modalClass: "modal-md",
            btnSuccessText: "",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

    // get kpi row html template
    this.GetReportRowHtml = function () {
        var html = "";
        $.ajax({
            cache: false,
            async: false,
            url: "/_templates/KpiReportReport.html" + "?q=" + $.now(),
            success: function (data) {
                html = data;
            }
        });
        return html;
    };
};


function printTable() {
    var divToPrint = document.getElementById("tblKPIReport");
    newWin = window.open("");
    newWin.document.write(divToPrint.outerHTML);
    newWin.print();
    newWin.close();
}


function getFilters() {
    return new KpiReport().GetFilter();
}
