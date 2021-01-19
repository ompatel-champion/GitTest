var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var $ddlLocations = $("#ddlLocations");
var $ddlSalesRep = $("#ddlSalesRep");
var _fontName = "'Open Sans', sans-serif";
var _fontSize = 13;
var _color = '#666666';
var _gChartFontStyle = { color: _color, fontFamily: _fontName, fontSize: _fontSize };
var _gChartLegendFontStyle = {
    color: _color,
    fontFamily: _fontName,
    fontSize: "12px",
}
var _gChartAnnotationFontStyle = {
    color: _color,
    fontFamily: _fontName,
    fontSize: "12px",
}
// init Google APIs
google.charts.load('current', { 'packages': ['corechart', 'bar'] });
google.charts.setOnLoadCallback(_drawCharts);


$(function () {

    // active & inactive toggles 
    $('.dashboard-status a').unbind("click").click(function (e) { 
        e.preventDefault();
        $(this).parent().find('a.task-btn').removeClass('active');
        $(this).addClass('active');
        _drawCharts();
    });

    $ddlLocations.select2({ placeholder: "Location", allowClear: true });
    $("#ddlCountry").select2({ placeholder: "Country", allowClear: true });
    $("#ddlSalesRep").select2({ placeholder: "Sales Rep", allowClear: true });
    $("#ddlDateType").select2({ placeholder: "Date Type", allowClear: true });
    $('#divDashboardFilters .input-daterange').datepicker({});
    $("#btnUpdate").unbind("click").click(function () {
        _drawCharts();
    });

    $('#ddlCountry').on('select2:select', function (e) {
        getLocations($('#ddlCountry').val());
    });
    $('#ddlCountry').on('select2:unselecting', function (e) {
        getLocations("");
    });

    $('#ddlLocations').on('select2:select', function (e) {
        getSalesReps($('#ddlLocations').val());
    });
    $('#ddlLocations').on('select2:unselecting', function (e) {
        getSalesReps("");
    });
    
});

// dashboard charts
function _drawCharts() {
    drawSalesForecastByStage();
    drawIndustryChart();
    drawSalesForecastBySalesRepStageChart();
    drawSalesForecastByLocationChart();
    drawSalesForecastByCountryChart();
    new VolumesByLocation().GetVolumesByLocation();
}


// binds top sales stage rev/profit boxes on dashboard
var SalesStageRevenue = function () {
    this.BindSalesStageRevenues = function (salesStageRevenues) {

        var $salesStageContainer = $("#salesStageRevenueContainer");
        $salesStageContainer.html("");

        $.each(salesStageRevenues,
            function (i, salesStageRevenue) {

                var $divCol = $("<div />", { "class": "col-xl-3 col-lg-6 col-md-6 col-sm-6" });
                if (i !== 3) {
                    $divCol.addClass("PR0");
                }

                var $ibox = $("<div />",
                    { "class": "ibox float-e-margins sales-stage", "data-sales-stage": salesStageRevenue.SalesStage });

                var $divHeader = $("<div />", { "class": "dcard-header" });

                var $titleWrap = $("<div />", { "class": "ibox-title" });

                switch (salesStageRevenue.SalesStage) {
                    case "Qualifying": $titleWrap.addClass("LBlue"); break;
                    case "Negotiation": $titleWrap.addClass("MBlue"); break;
                    case "Final Negotiation": $titleWrap.addClass("DBlue"); break;
                    case "Trial Shipment": $titleWrap.addClass("Green"); break;
                    case "Won": $titleWrap.addClass("DGreen"); break;
                    case "Lost": $titleWrap.addClass("DRed"); break;
                    case "Stalled": $titleWrap.addClass("DGrey"); break;
                    default: $titleWrap.addClass("LBlue"); break;
                }
                $divHeader.append($titleWrap);

                var $h3 = $("<h3 />", { "class": "card-title language-entry", "html": salesStageRevenue.SalesStage });
                $titleWrap.append($h3);

                var $spanCount = $("<span />", { "class": "card-count", "html": salesStageRevenue.DealCount });
                $titleWrap.append($spanCount);

                $ibox.append($divHeader);

                var $iboxContent = $("<div />", { "class": "ibox-content" });

                var curencySymbol = salesStageRevenue.CurrencySymbol
                    ? salesStageRevenue.CurrencySymbol
                    : salesStageRevenue.CurrencyCode;

                var $iboxTitle = $("<div />", { "class": "" });
                var $h5 = $("<h5>" + salesStageRevenue.SalesStage + "</h5>");
                $iboxTitle.append($h5);
                $iboxTitle.append($("<div />", { "class": "hr-line-dashed MB5" }));

                var $row = $("<div />", { "class": "row" });

                // revenue
                var $revenueCol1 = $("<div />", { "class": "col-md-4 col-4" });
                var $small = $("<p />", { "html": "Revenue", "class": "" });
                $revenueCol1.append($small);
                $row.append($revenueCol1);

                var $revenueCol2 = $("<div />", { "class": "col-md-8 col-8" });
                var $h2Revenue = $("<h3 />",
                    {
                        "class": "pull-right",
                        "html": curencySymbol + " " + formatNumber(salesStageRevenue.Revenue.toFixed(0))
                    });
                $revenueCol2.append($h2Revenue);
                $row.append($revenueCol2);
                $row.append($("<div />", { "class": "clearfix" }));

                //profit
                var $profitCol1 = $("<div />", { "class": "col-md-4 col-4" });
                $profitCol1.append($("<p />", { "html": "Profit", "class": "" }));
                $row.append($profitCol1);

                var $profitCol2 = $("<div />", { "class": "col-md-8  col-8" });
                var $h2Profit = $("<h3 />",
                    {
                        "class": "pull-right",
                        "html": curencySymbol + " " + formatNumber(salesStageRevenue.Profit.toFixed(0))
                    });
                $profitCol2.append($h2Profit);
                $row.append($profitCol2);
                $iboxContent.append($row);

                // append title and content to the i-box
                //$ibox.append($iboxTitle);
                $ibox.append($iboxContent);

                $divCol.append($ibox);
                // column
                $salesStageContainer.append($divCol);
            });

        $salesStageContainer.find(".sales-stage").unbind("click").click(function () {
            var salesStage = $(this).attr("data-sales-stage");
            var filtersStr = getReportQueryString();
            var url = "/Reporting/Deals/DealsReport.aspx?salesstage=" + salesStage + "&" + filtersStr;
            window.open(url, '_blank');
        });
    };
};

//forcast by stage
function drawSalesForecastByStage() {
    var filters = getFilters();

    // AJAX to get profit by sales stage 
    $.ajax({
        type: "POST",
        url: "/api/Dashboard/GetSalesForecastByStage",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(filters),
        success: function (salesForecastByStages) {
            // get chart data
            if (salesForecastByStages.length === 0) {
                $("#divForecastByStage").html("");
                var $noResult = $("<div />", {
                    "class": "text-center PT30",
                    "html": "<i class='text-warning fa-stack fa fa-list fa-2x'></i><p>No Data Found</p>"
                });
                $("#divForecastByStage").append($noResult);
                var $salesStagContainer = $("#salesStageRevenueContainer");
                $salesStagContainer.html("");
                return;
            }

            // bind top sales stage revenue/profit boxes
            new SalesStageRevenue().BindSalesStageRevenues(salesForecastByStages);

            var symbol = "$";
            if (salesForecastByStages.length > 0) {
                symbol = salesForecastByStages[0].CurrencySymbol;
            }

            var chart;
            var configChart = function () {
                chart = new AmCharts.AmFunnelChart();
                chart.titleField = "SalesStage";
                chart.valueField = "Profit";
                chart.marginRight = 120;
                chart.marginTop = 10;
                chart.labelPosition = "right";
                chart.dataProvider = salesForecastByStages;
                chart.neckWidth = "40%";
                chart.neckHeight = "30%";
                chart.percentPrecision = 0;
                chart.balloonText = "Profit: " + symbol + " [[value]] / [[percents]]%";
                // show revenue
                chart.labelText = "[[title]]";
                chart.showZeroSlices = true;
                chart.fontFamily = _fontName;
                chart.color = _color;
                chart.fontSize = _fontSize;
                chart.valueRepresents = 'area';
                chart.addListener("clickSlice", function (e) {
                    var chart = e.chart;
                    var salesStage = e.dataItem.title;
                    var filtersStr = getReportQueryString();
                    var url = "/Reporting/Deals/DealsReport.aspx?salesstage=" + salesStage + "&" + filtersStr;
                    window.open(url, '_blank');
                });

                // init
                chart.addListener("init", function () {
                    $("#divForecastByStage a").remove();
                });

                chart.creditsPosition = "bottom-left";
                chart.write('divForecastByStage');
            };

            if (AmCharts.isReady) {
                configChart();
            } else {
                AmCharts.ready(configChart);
            }

            $("#divForecastByStage").css("cursor", "pointer");
        },
        beforeSend: function () {
        },
        error: function () {
        }
    });
}

//profit by industry
function drawIndustryChart() {
    var filters = getFilters();
    // AJAX to get the deals by industry  
    $.ajax({
        type: "POST",
        url: "/api/Dashboard/GetDealsByIndustry",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(filters),
        success: function (dealsByIndustries) {
            if (dealsByIndustries.length === 0) {
                $("#divDealsByIndustryChart").html("");
                var $noResult = $("<div />", {
                    "class": "text-center PT30",
                    "html": "<i class='text-warning fa-stack fa fa-list fa-2x'></i><p>No Data Found</p>"
                });
                $("#divDealsByIndustryChart").append($noResult);
            }

            var eleId = "divDealsByIndustryChart";

            //create data table
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Industry');
            data.addColumn('number', 'Profit');
            data.addColumn({ type: 'string', role: 'tooltip' });

            // currency symbol
            var curencySymbol = "$";
            if (dealsByIndustries.length > 0) {
                curencySymbol = dealsByIndustries[0].CurrencySymbol ? dealsByIndustries[0].CurrencySymbol : dealsByIndustries[0].CurrencyCode;
            }

            // loop through data and create arrays
            $.each(dealsByIndustries, function (i, t) {
                var profit = 'Profit: ' + curencySymbol + ' ' + formatNumber(parseInt(t.Profit).toString());
                data.addRows([[t.Industry, t.Profit, profit]]);
            });

            // set formatter
            var formatter;
            if (dealsByIndustries.length > 0) {
                // set formatter
                formatter = new google.visualization.NumberFormat({ prefix: curencySymbol + " " });
                formatter.format(data, 1);
                // options
                var options = {
                    is3D: true,
                    title: 'Deals By Industry',
                    //width: '100%',
                    height: '300',
                    chartArea: { width: "100%", height: "100%" },
                    titleTextStyle: _gChartFontStyle,
                    legend: { textStyle: _gChartFontStyle },
                    tooltip: { textStyle: _gChartFontStyle }
                };
                // create the chart
                var chart = new google.visualization.PieChart(document.getElementById(eleId));
                chart.draw(data, options);

                // click event
                google.visualization.events.addListener(chart, 'select', function () {
                    var selection = chart.getSelection();
                    if (selection.length) {
                        var row = selection[0].row;
                        var industry = data.getValue(row, 0);
                        var activeInactiveStatus = $('.filter-wrap a.task-btn.active').attr('data-status');
                        var salesstage = activeInactiveStatus === "active" ? "Active" : "Inactive";
                        var filtersStr = getReportQueryString();
                        var url = "/Reporting/Deals/DealsReport.aspx?industry=" + encodeURIComponent(industry) + "&salesstage=" + salesstage + "&" + filtersStr;
                        window.open(url, '_blank');
                    }
                });

                google.visualization.events.addListener(chart, 'onmouseover', function () {
                    $("#divDealsByIndustryChart").css('cursor', 'pointer');
                });
                google.visualization.events.addListener(chart, 'onmouseout', function () {
                    $("#divDealsByIndustryChart").css('cursor', 'default');
                });
            }
        },
        beforeSend: function () {
        },
        error: function () {
        }
    });
}

//sales forcast by location
function drawSalesForecastByLocationChart() {
    var filters = getFilters();
    $.ajax({
        type: "POST",
        url: "/api/Dashboard/GetSalesForecastByLocation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(filters),
        success: function (salesForecastByLocations) {
            // create data table
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'LocationName');
            data.addColumn('number', 'Revenue');
            data.addColumn({ type: 'string', role: 'locationcode' });
            data.addColumn({ type: 'string', role: 'tooltip' });

            // loop through data and create arrays
            $.each(salesForecastByLocations, function (i, t) {
                var formattedRevenue = formatNumber(parseInt(t.Revenue));
                var formattedProfit = formatNumber(parseInt(t.Profit)); 
                data.addRows([[t.LocationName, parseInt(t.Revenue), t.LocationCode, "Revenue: " + t.CurrencySymbol + formattedRevenue + " / " + "Profit: " + t.CurrencySymbol + formattedProfit]]);

            });

            var prefix = "";
            if (salesForecastByLocations.length > 0) {
                prefix = salesForecastByLocations[0].CurrencySymbol;
            }

            var formatter = new google.visualization.NumberFormat(
                { negativeColor: 'red', negativeParens: true, pattern: prefix + ' ###,###' });

            var height = data.getNumberOfRows() * 20 + 30;
            $("#divSalesForecastByLocationChart").css("height", height);

            // var formatter = new google.visualization.NumberFormat({ prefix: prefix });
            formatter.format(data, 1);
            var view = new google.visualization.DataView(data);
            var options = {
                width: '100%',
                height: height,
                bar: { groupWidth: "100%" },
                chartArea: { width: "85%", height: '93%', top: 0, left: '40%' },
                vAxis: { textStyle: _gChartFontStyle },
                hAxis: { textStyle: _gChartFontStyle, gridlines: { count: 3 } }
            };
            var chart = new google.visualization.BarChart(document.getElementById("divSalesForecastByLocationChart"));
            chart.draw(view, options);

            // click event
            google.visualization.events.addListener(chart, 'select', function () {
                var selection = chart.getSelection();
                if (selection.length) {
                    var row = selection[0].row;
                    var url = "";
                    var filtersStr = getReportQueryString(); 
                    if (filtersStr.indexOf("locationcode=") > -1) {
                        url = "/Reporting/Deals/DealsReport.aspx?" + filtersStr;
                    } else {
                        var locationcode = data.getValue(row, 2);
                        url = "/Reporting/Deals/DealsReport.aspx?locationcode=" + locationcode + "&" + filtersStr;
                    }
                    window.open(url, '_blank');
                }
            });

            google.visualization.events.addListener(chart, 'onmouseover', function () {
                $("#divSalesForecastByLocationChart").css('cursor', 'pointer');
            });
            google.visualization.events.addListener(chart, 'onmouseout', function () {
                $("#divSalesForecastByLocationChart").css('cursor', 'default');
            });
        },
        beforeSend: function () {
        },
        error: function () {
        }
    });
}

//sales forcast by country
function drawSalesForecastByCountryChart() {

    var filters = getFilters();
    $.ajax({
        type: "POST",
        url: "/api/Dashboard/GetSalesForecastByCountry",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(filters),
        success: function (salesForecastByCountries) {
            // create data table
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'CountryName');
            data.addColumn('number', 'Revenue');
            data.addColumn({ type: 'string', role: 'countrycode' });
            data.addColumn({ type: 'string', role: 'tooltip' });

            // loop through data and create arrays
            $.each(salesForecastByCountries, function (i, t) {
                var formattedRevenue = formatNumber(parseInt(t.Revenue));
                var formattedProfit = formatNumber(parseInt(t.Profit)); 
                data.addRows([[t.CountryName, parseInt(t.Revenue), t.CountryCode, "Revenue: " + t.CurrencySymbol + formattedRevenue + " / " + "Profit: " + t.CurrencySymbol + formattedProfit]]);
            });

            var prefix = "";
            if (salesForecastByCountries.length > 0) {
                prefix = salesForecastByCountries[0].CurrencySymbol;
            }

            var formatter = new google.visualization.NumberFormat(
                { negativeColor: 'red', negativeParens: true, pattern: prefix + ' ###,###' });

            var height = data.getNumberOfRows() * 20 + 30;
            $("#divSalesForecastByCountryChart").css("height", height);

            // var formatter = new google.visualization.NumberFormat({ prefix: prefix });
            formatter.format(data, 1);
            var view = new google.visualization.DataView(data);
            var options = {
                width: '100%',
                height: height,
                bar: { groupWidth: "100%" },
                chartArea: { width: "85%", height: '93%', top: 0, left: '40%' },
                vAxis: { textStyle: _gChartFontStyle },
                hAxis: { textStyle: _gChartFontStyle, gridlines: { count: 3 } }
            };
            var chart = new google.visualization.BarChart(document.getElementById("divSalesForecastByCountryChart"));
            chart.draw(view, options);

            // click event
            google.visualization.events.addListener(chart, 'select', function () {
                var selection = chart.getSelection();
                if (selection.length) {
                    var row = selection[0].row;
                    //  var countrycode = data.getValue(row, 2);
                    //  var url = "/Reporting/Deals/DealsReport.aspx?countryCode=" + countrycode;
                    //  window.open(url, '_blank');

                    var url = "";
                    var filtersStr = getReportQueryString();
                    if (filtersStr.indexOf("countryName=") > -1) {
                        url = "/Reporting/Deals/DealsReport.aspx?" + filtersStr;
                    } else {
                        var countrycode = data.getValue(row, 2);
                        url = "/Reporting/Deals/DealsReport.aspx?countryName=" + countrycode + "&" + filtersStr;
                    }
                    window.open(url, '_blank');
                }
            });

            google.visualization.events.addListener(chart, 'onmouseover', function () {
                $("#divSalesForecastByCountryChart").css('cursor', 'pointer');
            });
            google.visualization.events.addListener(chart, 'onmouseout', function () {
                $("#divSalesForecastByCountryChart").css('cursor', 'default');
            });
        },
        beforeSend: function () {
        },
        error: function () {
        }
    });
}

//air tonnage by location / ocean TEUs by location
var VolumesByLocation = function () {

    var self = this;

    this.GetVolumesByLocation = function () {

        var filters = getFilters();
        $.ajax({
            type: "POST",
            url: "/api/Dashboard/GetVolumesByLocation",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(filters),
            success: function (volumes) {
                self.DrawOceanVolumesByLocation(volumes);
                self.DrawAirVolumesByLocation(volumes);
            },
            beforeSend: function () {
            },
            error: function () {
            }
        });
    };

    this.DrawAirVolumesByLocation = function (volumes) {
        //create data table
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Location');
        data.addColumn('number', 'Volumes');
        data.addColumn({ type: 'string', role: 'Location' });
        data.addColumn({ type: 'string', role: 'tooltip' });

        //loop through data and create arrays
        $.each(volumes, function (i, t) {
            if (t.AirTonnage > 0)
                data.addRows([[t.LocationName, parseInt(t.AirTonnage), t.LocationCode, formatNumber(parseInt(t.AirTonnage)) + " Tonnes"]]);
        });
        var formatter = new google.visualization.NumberFormat({ suffix: ' Tonnes' });
        formatter.format(data, 0);
        var view = new google.visualization.DataView(data);

        var options = {
            width: '100%',
            height: 300,
            bar: { groupWidth: "60%" },
            chartArea: { width: "85%", height: '93%', top: 0, left: '40%' },
            vAxis: { textStyle: _gChartFontStyle },
            hAxis: { textStyle: _gChartFontStyle, gridlines: { count: 3 } }
        };
        var chart = new google.visualization.BarChart(document.getElementById("divAirByLocationChart"));
        chart.draw(view, options);

        // click event
        google.visualization.events.addListener(chart, 'select', function () {
            var selection = chart.getSelection();
            if (selection.length) {
                var row = selection[0].row;

                var url = "";
                var filtersStr = getReportQueryString();
                if (filtersStr.indexOf("locationcode=") > -1) {
                    url = "/Reporting/Deals/DealsReport.aspx?service=Air&" + filtersStr;
                } else {
                    var locationCode = data.getValue(row, 2);
                    url = "/Reporting/Deals/DealsReport.aspx?service=Air&locationcode=" + locationCode + "&" + filtersStr;
                }
                window.open(url, '_blank');
            }
        });

        google.visualization.events.addListener(chart, 'onmouseover', function () {
            $("#divAirByLocationChart").css('cursor', 'pointer');
        });
        google.visualization.events.addListener(chart, 'onmouseout', function () {
            $("#divAirByLocationChart").css('cursor', 'default');
        });
    };

    this.DrawOceanVolumesByLocation = function (volumes) {
        //create data table
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Location');
        data.addColumn('number', 'Volumes');
        data.addColumn({ type: 'string', role: 'Location' });
        data.addColumn({ type: 'string', role: 'tooltip' });

        //loop through data and create arrays
        $.each(volumes, function (i, t) {
            if (t.OceanTeus > 0)
                data.addRows([[t.LocationName, t.OceanTeus, t.LocationCode, formatNumber(t.OceanTeus) + " Teus"]]);
        });

        var formatter = new google.visualization.NumberFormat({ suffix: ' Teus' });
        formatter.format(data, 0);
        var view = new google.visualization.DataView(data);

        var options = {
            width: '100%',
            height: 300,
            bar: { groupWidth: "60%" },
            chartArea: { width: "85%", height: '93%', top: 0, left: '40%' },
            vAxis: { textStyle: _gChartFontStyle },
            hAxis: { textStyle: _gChartFontStyle, gridlines: { count: 3 } }
        };
        var chart = new google.visualization.BarChart(document.getElementById("divOceanByLocationChart"));
        chart.draw(view, options);

        // click event
        google.visualization.events.addListener(chart, 'select', function () {
            var selection = chart.getSelection();
            if (selection.length) {
                var row = selection[0].row;
                var url = "";
                var filtersStr = getReportQueryString();
                if (filtersStr.indexOf("locationcode=") > -1) {
                    url = "/Reporting/Deals/DealsReport.aspx?service=Ocean&" + filtersStr;
                } else {
                    var locationCode = data.getValue(row, 2);
                    url = "/Reporting/Deals/DealsReport.aspx?service=Ocean&locationcode=" + locationCode + "&" + filtersStr;
                }
                window.open(url, '_blank');
            }
        });

        google.visualization.events.addListener(chart, 'onmouseover', function () {
            $("#divOceanByLocationChart").css('cursor', 'pointer')
        });
        google.visualization.events.addListener(chart, 'onmouseout', function () {
            $("#divOceanByLocationChart").css('cursor', 'default')
        });
    };
};

//sales forcast by sales rep stage
function drawSalesForecastBySalesRepStageChart() {
    var filters = getFilters();
    var _STAGE_COLORS = {
        'qualifying': '#ff1001',
        'negotiation': '#ff6701',
        'trial shipment': '#ff9e02',
        'final negotiation': '#fcd203'
    }
    $.ajax({
        type: "POST",
        url: "/api/Dashboard/GetSalesForecastBySalesRepStage",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(filters),
        success: function (response) {
            if (response.length) {
                var salesReps = _organize_sales_rep_data(response);
                var salesRepIds = Object.keys(salesReps); if (salesRepIds.length) {
                    var $wrapper = $("#divSalesForecastBySalesRepChart");
                    var currencySymbol = response[0].CurrencySymbol||'$';
                    //add columns
                    var data = new google.visualization.DataTable();
                    data.addColumn('string', 'Sales Rep');
                    var stageNames = Object.keys(salesReps[salesRepIds[0]].stages);
                    $.each(stageNames, function(i, stageName) {
                        data.addColumn('number', stageName);
                        data.addColumn({ role: 'tooltip' });
                    });
                    //add rows
                    var annotationData = [];
                    var salesRepIndex=0; for (var i in salesReps) {
                        var salesRep = salesReps[i];
                        var row = [salesRep.name];
                        for (var j in salesRep.stages) {
                            var revenue = salesRep.stages[j].Revenue||0;
                            var profit = salesRep.stages[j].Profit||0;
                            row.push(revenue);
                            var tooltipStr = "Revenue: "+FFGlobal.utils.number.format.currency(revenue, currencySymbol)+"\r\n";
                            tooltipStr += "Profit: "+FFGlobal.utils.number.format.currency(profit, currencySymbol);
                            tooltipStr += " ("+FFGlobal.utils.number.format.percent(profit/revenue)+")";
                            row.push(tooltipStr);
                        }
                        var aStr = "";
                        aStr = '<span class="text-line">';
                            aStr += '<span class="text-line-title">Revenue:</span> '+FFGlobal.utils.number.format.currency(salesRep.totals.revenue, currencySymbol);
                        aStr += '</span>';
                        aStr += '</br>';
                        aStr += '<span class="text-line">';
                            aStr += '<span class="text-line-title">Profit:</span> '+FFGlobal.utils.number.format.currency(salesRep.totals.profit, currencySymbol);
                            aStr += ' (<span class="text-highlight">'+FFGlobal.utils.number.format.percent(salesRep.totals.profit/salesRep.totals.revenue)+'</span>)';
                        aStr += '</span>';
                        annotationData.push({
                            id: 'bar#'+(Object.keys(salesRep.stages).length-1)+'#'+salesRepIndex,
                            content: aStr
                        });
                        data.addRow(row);
                        salesRepIndex++;
                    }
                    // format values
                    var formatter = new google.visualization.NumberFormat({
                        negativeColor: 'red',
                        negativeParens: true,
                        pattern: currencySymbol + '###,###'
                    });
                    formatter.format(data, 1);
                    // set options
                    var _BAR_HEIGHT = 40;
                    var _HEADER_HEIGHT = 60;
                    var _FOOTER_HEIGHT = 30;
                    var height = (data.getNumberOfRows()*_BAR_HEIGHT) + _FOOTER_HEIGHT + _HEADER_HEIGHT;
                    $wrapper.css("height", height);
                    var view = new google.visualization.DataView(data);
                    var options = {
                        height: height,
                        isStacked: true,
                        bar: { groupWidth: "75%" },
                        chartArea: {
                            top: _HEADER_HEIGHT,
                            left: "25%",
                            height: height-_FOOTER_HEIGHT - _HEADER_HEIGHT,
                            width: "60%"
                        },
                        legend: {
                            position: "top", 
                            alignment: "start",
                            maxLines: 2,
                            textStyle: _gChartLegendFontStyle
                        },
                        annotations: {
                            alwaysOutside: true,
                            style: "point",
                            textStyle: _gChartAnnotationFontStyle,
                        },
                        vAxis: {
                            textStyle: _gChartFontStyle, 
                        },
                        hAxis: {
                            textStyle: _gChartFontStyle,
                            gridlineColor: '#c0c0c0',
                            baselineColor: "#a0a0a0"
                        },
                        colors: ['#ff1001', '#ff6701', '#ff9e02', '#fcd203']
                    };
                    // draw the chart
                    var chart = new google.visualization.BarChart($wrapper.get(0));
                    chart.draw(view, options);
                    //create annotation data
                    _add_annotations({
                        $wrapper: $wrapper,
                        chart: chart,
                        data: annotationData
                    });

                    // chart events
                    google.visualization.events.addListener(chart, 'select', function () {
                        var selection = chart.getSelection();
                        if (selection.length) {
                            var row = selection[0].row; 
                            var url = "";
                            var filtersStr = getReportQueryString();
                            if (filtersStr.indexOf("userIds=") > -1) {
                                url = "/Reporting/Deals/DealsReport.aspx?" + filtersStr;
                            } else {
                                var userId = data.getValue(row, 2);
                                url = "/Reporting/Deals/DealsReport.aspx?userIds=" + userId + "&" + filtersStr;
                            }
                            window.open(url, '_blank');
                        }
                    });
                    google.visualization.events.addListener(chart, 'onmouseover', function (e) {
                        if (e.row!==null) {
                            $wrapper.css({
                                cursor: "pointer"
                            });
                        }
                    });
                    google.visualization.events.addListener(chart, 'onmouseout', function () {
                        $wrapper.css({
                            cursor: ""
                        });
                    });
                }
            }
        }
    });

    //add annotations
    function _add_annotations(props) {
        props = props||{};
        var _GAP = 10;
        var data = props.data;
        var chart = props.chart;
        var layout = chart.getChartLayoutInterface();
        $.each(data, function(i, item){
            var boundingBox = layout.getBoundingBox(item.id); if (boundingBox) {
                $annotation = $("<div/>", {
                    class: "chart-annotation",
                    html: item.content,
                    css: {
                        top: boundingBox.top+"px",
                        left: (boundingBox.left+boundingBox.width+_GAP)+"px"
                    }
                });
                props.$wrapper.append($annotation);
                var aHeight = $annotation.get(0).offsetHeight;
                $annotation.css({
                    top: (((boundingBox.height-aHeight)/2)+boundingBox.top)+"px"
                });
            }
        });
    }

    //organize sales rep data from the provided src data (typically response data)
    function _organize_sales_rep_data(srcData) {
        var data = {};
        var stageNames = {};
        //organize the srcData
        $.each(srcData, function(i, item){
            var salesRep = data["_"+item.UserId] = data["_"+item.UserId]||{
                name: item.SalesRep,
                totals: {
                    revenue: 0,
                    profit: 0
                },
                stages: {}
            };
            var salesStageLower = item.SalesStage.toLowerCase();
            salesRep.stages[salesStageLower] = stageNames[salesStageLower] = item;
            salesRep.totals.revenue += item.Revenue;
            salesRep.totals.profit += item.Profit;
        });
        //fill in missing stage names and sort
        for (var i in data) {
            var salesRep = data[i];
            for (var j in stageNames) {
                salesRep.stages[j] = salesRep.stages[j] || {revenue:0, profit:0}
            }
            salesRep.stages = FFGlobal.utils.sort.byKeyList({
                obj: salesRep.stages,
                refKeys: [
                    'qualifying',
                    'negotiation',
                    'trial shipment',
                    'final negotiation'
                ],
                addUnmatched: true
            });
        }
        //sort the sales reps by total revenue (descending)
        data = FFGlobal.utils.sort.byObjVal({
            obj: data,
            compare: function(a,b) {
                return b.totals.revenue-a.totals.revenue;
            }
        });
        return data;
    }
}

/*
//forcast by sales rep
function drawSalesForecastBySalesRepChart() {
    var filters = getFilters();
    $.ajax({
        type: "POST",
        url: "/api/Dashboard/GetSalesForecastBySalesReps",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(filters),
        success: function (forecastBySalesReps) {
            // create data table
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Sales Rep');
            data.addColumn('number', 'Revenue');
            data.addColumn('number', 'Profit');
            data.addColumn({ type: 'string', role: 'annotation' });
            // add data rows
            $.each(forecastBySalesReps, function (i, t) {
                var formattedProfit = formatNumber(parseInt(t.Profit)); 
                var formattedRevenue = formatNumber(parseInt(t.Revenue));
                data.addRow([
                        t.SalesRep,
                        Math.max(0, t.Revenue),
                        Math.max(0, t.Profit),
                        Math.round((t.Profit/t.Revenue)*100)+"%"
                ]); 
            });
            // format values
            var prefix = forecastBySalesReps.length>0?forecastBySalesReps[0].CurrencySymbol:"";
            var formatter = new google.visualization.NumberFormat({
                negativeColor: 'red',
                negativeParens: true,
                pattern: prefix + ' ###,###'
            });
            formatter.format(data, 1);
            // set options
            var _FOOTER_HEIGHT = 56;
            var height = (data.getNumberOfRows()*24) + _FOOTER_HEIGHT;
            $("#divSalesForecastBySalesRepChart").css("height", height);
            var view = new google.visualization.DataView(data);
            var options = {
                height: height,
                isStacked: true,
                bar: { groupWidth: "80%" },
                chartArea: { 
                    top: 0,
                    left: "30%",
                    height: height-_FOOTER_HEIGHT
                },
                legend: { position: 'bottom', maxLines: 1 },
                vAxis: { textStyle: _gChartFontStyle },
                hAxis: { textStyle: _gChartFontStyle }
            };
            // draw the chart
            var chart = new google.visualization.BarChart($("#divSalesForecastBySalesRepChart").get(0));
            chart.draw(view, options);

            // click event
            google.visualization.events.addListener(chart, 'select', function () {
                var selection = chart.getSelection();
                if (selection.length) {
                    var row = selection[0].row; 

                    var url = "";
                    var filtersStr = getReportQueryString();
                    if (filtersStr.indexOf("userIds=") > -1) {
                        url = "/Reporting/Deals/DealsReport.aspx?" + filtersStr;
                    } else {
                        var userId = data.getValue(row, 2);
                        url = "/Reporting/Deals/DealsReport.aspx?userIds=" + userId + "&" + filtersStr;
                    }
                    window.open(url, '_blank');
                }
            });

            google.visualization.events.addListener(chart, 'onmouseover', function () {
                $("#divSalesForcastBySalesRepChart").css('cursor', 'pointer');
            });
            google.visualization.events.addListener(chart, 'onmouseout', function () {
                $("#divSalesForcastBySalesRepChart").css('cursor', 'default');
            });
        },
        beforeSend: function () {
        },
        error: function () {
        }
    });
}
*/

function getFilters() {
    var request = new Object();
    request.UserId = userId;
    request.SubscriberId = subscriberId;
    request.Keyword = $("#txtKeyword").val();
    // sales rep ids
    request.SalesRepIds = [];
    if ($("#ddlSalesRep").val() === null || $("#ddlSalesRep").val() === "") {
        $("#ddlSalesRep > option").each(function () {
            if (this.value && this.value !== "")
                request.SalesRepIds.push(this.value);
        });
    } else {
        request.SalesRepIds.push($("#ddlSalesRep").val());
    }
    // country codes
    request.CountryCodes = [];
    if ($("#ddlCountry").val() === null || $("#ddlCountry").val() === "") {
        $("#ddlCountry > option").each(function () {
            if (this.value && this.value !== "")
                request.CountryCodes.push(this.value);
        });
    } else {
        request.CountryCodes.push($("#ddlCountry").val());
    }
    // location codes
    request.LocationCodes = [];
    if ($("#ddlLocations").val() === null || $("#ddlLocations").val() === "") {
        $("#ddlLocations > option").each(function () {
            if (this.value && this.value !== "")
                request.LocationCodes.push(this.value);
        });
    } else {
        request.LocationCodes.push($("#ddlLocations").val());
    }
    var activeInactiveStatus = $('.dashboard-status a.active').attr('data-status');
    
    request.Status = activeInactiveStatus === "active" ? "ACTIVE" : "INACTIVE";
     
    // Date Type, Date From, Date To
    request.DateType = $('#ddlDateType').val();
    request.DateFrom = moment($("#txtDateFrom").datepicker("getDate")).isValid() ? moment($("#txtDateFrom").datepicker("getDate")).format("DD-MM-YY") : null;
    request.DateTo = moment($("#txtDateTo").datepicker("getDate")).isValid() ? moment($("#txtDateTo").datepicker("getDate")).format("DD-MM-YY") : null;
    return request;
}


function getReportQueryString() {
    var strArr = [];

    // sales rep ids
    if ($("#ddlSalesRep").val() !== null && $("#ddlSalesRep").val() !== "") {
        strArr.push("userIds=" + $("#ddlSalesRep").val());
    }
    // country codes
    if ($("#ddlCountry").val() !== null && $("#ddlCountry").val() !== "") {
        strArr.push("countryName=" + $("#ddlCountry option:selected").text());
    }
    // location codes
    if ($("#ddlLocations").val() !== null && $("#ddlLocations").val() !== "") { 
        strArr.push("locationcode=" + $("#ddlLocations").val());
    }

    if (moment($("#txtDateFrom").datepicker("getDate")).isValid() || moment($("#txtDateTo").datepicker("getDate")).isValid()) {
        strArr.push("dtType=" + $("#ddlDateType").val());
        if (moment($("#txtDateFrom").datepicker("getDate")).isValid()) {
            strArr.push("dtFrom=" + moment($("#txtDateFrom").datepicker("getDate")).format("DD-MM-YY"));
        }
        if (moment($("#txtDateTo").datepicker("getDate")).isValid()) {
            strArr.push("dtTo=" + moment($("#txtDateTo").datepicker("getDate")).format("DD-MM-YY"));
        }
    }

    var returnStr = strArr.join("&");
    return returnStr;
}

function formatNumber(val) {
    while (/(\d+)(\d{3})/.test(val.toString())) {
        val = val.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
    }
    return val;
}


function getLocations(countryCode) {

    $.ajax({
        type: "GET",
        url: "/api/Dropdown/GetUserLocations?userId=" + userId + "&subscriberId=" + subscriberId + "&countryCode=" + countryCode,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '',
        success: function (locations) {
            $ddlLocations.html("");
            $ddlLocations.append($('<option>', { value: "", text: "" }));
            $.each(locations, function (i, ele) {
                $ddlLocations.append($('<option>', { value: ele.LocationCode, text: ele.LocationName }));
            });
        },
        beforeSend: function () {
        },
        error: function () {
        }
    });
}

function getSalesReps(locationCode) {

    $.ajax({
        type: "GET",
        url: "/api/Dropdown/GetSalesRepsByLocation?subscriberId=" + subscriberId + "&locationCode=" + locationCode,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '',
        success: function (salesReps) {
            $ddlSalesRep.html("");
            $ddlSalesRep.append($('<option>', { value: "", text: "" }));
            $.each(salesReps, function (i, ele) {
                $ddlSalesRep.append($('<option>', { value: ele.SelectValue, text: ele.SelectText }));
            });
        },
        beforeSend: function () {
        },
        error: function () {
        }
    });
}
