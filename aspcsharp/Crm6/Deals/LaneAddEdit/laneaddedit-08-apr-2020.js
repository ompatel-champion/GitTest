var subscriberId = parseInt($("#lblSubscriberId").text());
var userId = $("#lblUserId").text();
var laneId = parseInt($("#lblLaneId").text());
var dealId = parseInt($("#lblDealId").text());
var initialOrginLoad = false;
var initialDestinationLoad = false;

var destinationLocationSelected = false;
var originLocationSelected = false;

$(function () {
    // init tab actions
    initTabActions();
    // load the lane
    new Lanes().Init();
    $("#btnAir").click();
});


var Lanes = function () {
    var self = this;
    var $ddlVolumeUnit = $("#ddlVolumeUnit");  

    this.Init = function () {
        if (laneId > 0) {
            $("#btnSaveLane").html("Save Lane");
            $("#btnDelete").removeClass("hide");

        }

        self.UpdateDropdownOptions();

        // init actions
        self.InitActions();

        // init
        self.SetupSelect2Dropdowns();

        // init actions
        self.RetrieveLane();
    };

    // Delete Lane
    this.DeleteLane = function () {
        swal({
            title: translatePhrase("Delete Lane!"),
            text: translatePhrase("Are you sure you want to delete this lane?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/lane/DeleteLane/?laneId=" +
                        laneId +
                        "&userId=" +
                        userId +
                        "&laneSubscriberId=" +
                        subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        if (response) {
                            $("#btnCancel").click();
                        } else {
                            removeSpinner();
                        }
                    },
                    beforeSend: function () { addSpinner(); },
                    error: function () { }
                });
            }
        });
    };

    // Calculate Profit
    this.CalculateProfit = function () {
        var revenue = 0;
        if ($.isNumeric($("#txtRevenue").val())) {
            revenue = parseInt($("#txtRevenue").val());
        }

        var profitTypeValue = 0;
        if ($.isNumeric($("#txtProfit").val())) {
            profitTypeValue = parseFloat($("#txtProfit").val());
        }

        var volumes = 0;
        if ($.isNumeric($("#txtVolume").val())) {
            volumes = parseInt($("#txtVolume").val());
        }

        var selectedVolume = ($("#ddlVolumeUnit").val() + "").toLowerCase();
        var profitType = $("#ddlProfitType").val() + "";
        var profit = profitTypeValue;

        if (profitTypeValue > 0) {
            switch (profitType.toLowerCase()) {
                case "percentage":
                    profit = revenue * profitTypeValue / 100;
                    break;
                case "per kg":
                    if (selectedVolume === 'kgs') {
                        profit = volumes * profitTypeValue;
                    } else if (selectedVolume === 'lbs') {
                        profit = volumes * profitTypeValue * 0.453592;
                    } else if (selectedVolume === 'tonnes') {
                        profit = volumes * 1000 * profitTypeValue;
                    }
                    break;
                case "per cbm":
                    if (selectedVolume === 'cbms') {
                        profit = volumes * profitTypeValue;
                    } else if (selectedVolume === 'kgs') {
                        profit = volumes * profitTypeValue;
                    } else if (selectedVolume === 'tonnes') {
                        profit = volumes * 1000 * profitTypeValue;
                    } else {
                        profit = profit;
                    }
                    break;
                case "per lb":
                    if (selectedVolume === 'lbs') {
                        profit = volumes * profitTypeValue;
                    } else if (selectedVolume === 'kgs') {
                        profit = volumes * profitTypeValue * 2.20462;
                    } else if (selectedVolume === 'tonnes') {
                        profit = volumes * 1000 * profitTypeValue;
                    }
                    break;
                case "lbs":
                    if (selectedVolume === 'lbs') {
                        profit = volumes * profitTypeValue;
                    } else if (selectedVolume === 'tonnes') {
                        profit = volumes * 1000 * profitTypeValue;
                    }
                    break;
                case "per teu":
                    if (selectedVolume === 'teus') {
                        profit = volumes * profitTypeValue;
                    } else if (selectedVolume === 'kgs') {
                        profit = volumes * profitTypeValue;
                    } else if (selectedVolume === 'tonnes') {
                        profit = volumes * 1000 * profitTypeValue;
                    } else {
                        profit = profit;
                    }
                    break;
                case "flat amount":
                case "flat rate":
                    profit = profitTypeValue;
                    break;
                case "per pallet":
                    if (selectedVolume === 'kgs') {
                        profit = volumes * profitTypeValue;
                    } else if (selectedVolume === 'tonnes') {
                        profit = volumes * 1000 * profitTypeValue;
                    } else {
                        profit = 0;
                    }
                    break;
                case "per container":
                    if (selectedVolume === 'teus') {
                        profit = volumes * profitTypeValue;
                    } else {
                        profit = profit;
                    } break;
                case "per truck":
                    if (selectedVolume === 'trucks') {
                        profit = volumes * profitTypeValue;
                    } else {
                        profit = profit;
                    } break;
                case "per sq ft":
                    if (selectedVolume === 'square feet') {
                        profit = volumes * profitTypeValue;
                    } break;
                case "per sq meter":
                    if (selectedVolume === 'square meters') {
                        profit = volumes * profitTypeValue;
                    } break;
                case "per sq box":
                    if (selectedVolume === 'boxes') {
                        profit = volumes * profitTypeValue;
                    } break;
                default:
                    profit = revenue * profitTypeValue / 100;

                    if (selectedVolume === 'teus') {
                        profit = volumes * profitTypeValue;
                    } else if (selectedVolume === 'kgs') {
                        profit = volumes * profitTypeValue;
                    } else if (selectedVolume === 'tonnes') {
                        profit = volumes * 1000 * profitTypeValue;
                    } else {
                        profit = profit;
                    }

                    break;
            }
        }

        function formatNumber(profit) {
            profit += '';
            x = profit.split('.');
            x1 = x[0];
            x2 = x.length > 1 ? '.' + x[1] : '';
            var rgx = /(\d+)(\d{3})/;
            while (rgx.test(x1)) {
                x1 = x1.replace(rgx, '$1' + ',' + '$2');
            }
            return x1 + x2;
        }

        $("#txtCurrency").val(formatNumber(profit.toFixed(0)));
    };

    // Update volume, profit, shipment frequency dropdowns
    this.UpdateDropdownOptions = function () {
        var service = $('.btn-group button.active').attr('data-name');

        $("#ddlService").val(service);
        $("#ddlVolumeUnit").html("");
        $("#ddlProfitType").html("");
        $("#ddlShipmentFrequency").html("");

        switch (service) {
            case "Air":

                // volume units
                $("#ddlVolumeUnit").append($('<option>', { value: 'Tonnes', text: 'Tonnes' }));
                $("#ddlVolumeUnit").append($('<option>', { value: 'KGs', text: 'KGs' }));

                // profit types
                $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));
                $("#ddlProfitType").append($('<option>', { value: 'Per KG', text: 'Per KG' }));
                // $("#ddlProfitType").append($('<option>', { value: 'LBS', text: 'LBS' }));
                $(".profitPrefix").html("%");

                // shipment frequency
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Month', text: 'Per Month' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Year', text: 'Per Year' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Week', text: 'Per Week' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Spot', text: 'Spot' }));

                $("#ddlShipmentFrequency").val("Per Month");
                $("#txtVolume").focus();

                break;

            case "Brokerage":

                // volume units
                $("#ddlVolumeUnit").append($('<option>', { value: 'KGs', text: 'KGs' }));
                $("#ddlVolumeUnit").append($('<option>', { value: 'LBs', text: 'LBs' }));

                // profit types 

                $("#ddlProfitType").append($('<option>', { value: 'Per KG', text: 'Per KG' }));
                $("#ddlProfitType").append($('<option>', { value: 'Per LB', text: 'Per LB' }));
                $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));

                // shipment frequency
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per File', text: 'Per File' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Shipment', text: 'Per Shipment' }));

                if ($(".profitPrefix").html() === "%")
                    $(".profitPrefix").html($(".revenuePrefix").html());
                break;

            case "Ocean":

                // load ocean
                var loadOcean = $("#ddlLoadOcean").val();

                // volume units
                if (loadOcean === "fcl") {
                    $("#ddlVolumeUnit").append($('<option>', { value: 'TEUs', text: 'TEUs' }));
                } else if (loadOcean === "lcl") {
                    $("#ddlVolumeUnit").append($('<option>', { value: 'TEUs', text: 'TEUs' }));
                    $("#ddlVolumeUnit").append($('<option>', { value: 'CBMs', text: 'CBMs' }));
                } else {
                    $("#ddlVolumeUnit").append($('<option>', { value: 'CBMs', text: 'CBMs' }));
                }

                // profit types
                $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));
                if (loadOcean === "fcl") {
                    $("#ddlProfitType").append($('<option>', { value: 'Per Container', text: 'Per Container' }));
                }
                $(".profitPrefix").html("%");

                // shipment frequency
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Month', text: 'Per Month' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Year', text: 'Per Year' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Week', text: 'Per Week' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Spot', text: 'Spot' }));

                $("#ddlShipmentFrequency").val("Per Month");

                break;

            case "Ocean LCL":

                // volume units
                $("#ddlVolumeUnit").append($('<option>', { value: 'TEUs', text: 'TEUs' }));
                $("#ddlVolumeUnit").append($('<option>', { value: 'CBMs', text: 'CBMs' }));

                // profit types
                $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));
                $(".profitPrefix").html("%");
                break;

            case "ROR":

                // volume unit
                $("#ddlVolumeUnit").append($('<option>', { value: 'CBMs', text: 'CBMs' }));

                // profit types
                $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));
                $("#ddlProfitType").append($('<option>', { value: 'Per CBM', text: 'Per CBM' }));
                $(".profitPrefix").html("%");
                break;

            case "Road":
                // load road
                var loadRaod = $("#ddlLoadRoad").val();

                // volume units 
                if (loadRaod === "ftl") {
                    $("#ddlVolumeUnit").append($('<option>', { value: 'Trucks', text: 'Trucks' }));
                } else if (loadRaod === "ltl") {
                    $("#ddlVolumeUnit").append($('<option>', { value: 'KGs', text: 'KGs' }));
                    $("#ddlVolumeUnit").append($('<option>', { value: 'LBS', text: 'LBS' }));
                } else {
                    $("#ddlVolumeUnit").append($('<option>', { value: 'KGs', text: 'KGs' }));
                    $("#ddlVolumeUnit").append($('<option>', { value: 'Trucks', text: 'Trucks' }));
                    $("#ddlVolumeUnit").append($('<option>', { value: 'LBS', text: 'LBS' }));
                }


                //profit types
                if (loadRaod === "ftl") {
                    $("#ddlProfitType").append($('<option>', { value: 'Per Truck', text: 'Per Truck' }));
                    $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                    $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));
                } else if (loadRaod === "ltl") {
                    $("#ddlProfitType").append($('<option>', { value: 'Per KG', text: 'Per KG' }));
                    $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                    $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));
                    $("#ddlProfitType").append($('<option>', { value: 'Per LB', text: 'Per LB' }));
                } else {
                    $("#ddlProfitType").append($('<option>', { value: 'Per KG', text: 'Per KG' }));
                    $("#ddlProfitType").append($('<option>', { value: 'Per Truck', text: 'Per Truck' }));
                    $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                    $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));
                    $("#ddlProfitType").append($('<option>', { value: 'Per LB', text: 'Per LB' }));
                }


                if ($(".profitPrefix").html() === "%")
                    $(".profitPrefix").html($(".revenuePrefix").html());


                // shipment frequency
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Month', text: 'Per Month' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Year', text: 'Per Year' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Week', text: 'Per Week' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Spot', text: 'Spot' }));

                $("#ddlShipmentFrequency").val("Per Month");

                break;

            case "Road LTL":

                // volume units
                $("#ddlVolumeUnit").append($('<option>', { value: 'KGs', text: 'KGs' }));
                $("#ddlVolumeUnit").append($('<option>', { value: 'LBs', text: 'LBs' }));

                //profit types
                $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));
                $("#ddlProfitType").append($('<option>', { value: 'Per KG', text: 'Per KG' }));
                $("#ddlProfitType").append($('<option>', { value: 'Per LB', text: 'Per LB' }));
                $(".profitPrefix").html("%");
                break;

            case "Logistics":

                // volume units
                $("#ddlVolumeUnit").append($('<option>', { value: 'KGs', text: 'KGs' }));
                $("#ddlVolumeUnit").append($('<option>', { value: 'LBs', text: 'LBs' }));
                $("#ddlVolumeUnit").html($("#ddlVolumeUnit").find('option').sort(function (x, y) {
                    return $(x).text() > $(y).text() ? 1 : -1;
                }));

                $("#ddlVolumeUnit").val("KGs");

                // profit types
                $("#ddlProfitType").append($('<option>', { value: 'Per KG', text: 'Per KG' }));
                $("#ddlProfitType").append($('<option>', { value: 'Per LB', text: 'Per LB' }));

                $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));


                //Sets default value.
                $('#ddlProfitType').val('Per KG');
                $('#ddlProfitType').trigger('change');

                // shipment frequency
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Month', text: 'Per Month' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Year', text: 'Per Year' }));
                $("#ddlShipmentFrequency").append($('<option>', { value: 'Per Week', text: 'Per Week' }));

                $("#ddlShipmentFrequency").val("Per Month");

                if ($(".profitPrefix").html() === "%")
                    $(".profitPrefix").html($(".revenuePrefix").html());

                break;

            case "Warehouse":

                // volume units
                $("#ddlVolumeUnit").append($('<option>', { value: 'CBMs', text: 'CBMs' }));
                $("#ddlVolumeUnit").append($('<option>', { value: 'Pallets', text: 'Pallets' }));

                // profit types
                $("#ddlProfitType").append($('<option>', { value: 'Percentage', text: 'Percentage' }));
                $("#ddlProfitType").append($('<option>', { value: 'Flat Rate', text: 'Flat Rate' }));
                $("#ddlProfitType").append($('<option>', { value: 'Per CBM', text: 'Per CBM' }));
                $("#ddlProfitType").append($('<option>', { value: 'Per Pallet', text: 'Per Pallet' }));
                $(".profitPrefix").html("%");
                break;

            default:
        }

        // set volume unit
        var volUnit = $("#ddlVolumeUnit").val();
        if (volUnit !== '' && volUnit !== undefined) {
            $("#ddlVolumeUnit").val(volUnit);

        } else {
            $("#ddlVolumeUnit").val($('#ddlVolumeUnit option:first-child').val());
        }

        // set profit unit
        var profitMeasure = $("#ddlProfitType").val();
        if (profitMeasure !== '' && profitMeasure !== undefined) {
            $("#ddlProfitType").val(profitMeasure);

        } else {
            $("#ddlProfitType").val($('#ddlProfitType option:first-child').val());
        }
    };

    // Render select dropdowns
    this.SetupSelect2Dropdowns = function () {
        $("#ddlSpecialRequirements").select2({ theme: "classic", placeholder: "Add requirement...", minimumResultsForSearch: -1 });
        $("#ddlReceive").select2({ theme: "classic", placeholder: "Add receivable...", minimumResultsForSearch: -1 });
        $("#ddlCurrency").select2({ minimumResultsForSearch: -1 });
        $("#ddlServiceLocation").select2({ placeholder: "Add location...", minimumResultsForSearch: -1 });
        $("#ddlOriginCountry,#ddlDestinationCountry").select2({ placeholder: "Country", minimumResultsForSearch: -1 });
        $("#ddlShipmentFrequency,#ddlLoadOcean,#ddlLoadOcean,#ddlLoadRoad").select2({ minimumResultsForSearch: -1 });
        $("#ddlOriginRegion,#ddlDestinationRegion").select2({ placeholder: "Region", minimumResultsForSearch: -1 });
        $("#ddlVolumeUnit,#ddlProfitType,#ddlShipmentFrequency").select2({ minimumResultsForSearch: -1 });
        $("#ddlProfitType").select2({ width: '100%', minimumResultsForSearch: -1 });

        // origin location
        self.InitOriginLocation();

        // destination location
        self.InitDestinationLocation();

        $('.select2-search__field').width("100%");
    };

    this.InitDestinationLocation = function () {
        $("#ddlDestinationLocation").select2({
            minimumResultsForSearch: 25,
            minimumInputLength: -1,
            placeholder: "Location",
            ajax: {
                url: function (obj) {
                    var keyword = obj.term ? obj.term : "";
                    var service = $('.btn-group button.active').attr('data-name');
                    return "/api/dropdown/GetServiceLocations?countryCodes=" +
                        $("#ddlDestinationCountry").val() +
                        "&service=" + service +
                        "&keyword=" +
                        keyword;
                },
                dataType: "json",
                timeout: 50000,
                type: "GET",
                data: '',
                processResults: function (data) {
                    return {
                        results: $.map(data,
                            function (item) {
                                return {
                                    text: item.SelectText,
                                    id: item.SelectValue + "|" + item.CountryCode + "|" + item.CountryName
                                };
                            })
                    };
                }
            }
        });
    };


    this.InitOriginLocation = function () {
        $("#ddlOriginLocation").select2({
            minimumResultsForSearch: 25,
            minimumInputLength: -1,
            placeholder: "Location",
            ajax: {
                url: function (obj) {
                    var keyword = obj.term ? obj.term : "";
                    var service = $('.btn-group button.active').attr('data-name');
                    var originCountry = $("#ddlOriginCountry").val();
                    // getSelectedCountryCode
                    return "/api/dropdown/GetServiceLocations?countryCodes=" + originCountry + "&service=" + service + "&keyword=" + keyword;
                },
                dataType: "json",
                timeout: 50000,
                type: "GET",
                data: '',
                processResults: function (data) {
                    return {
                        results: $.map(data,
                            function (item) {
                                return {

                                    text: item.SelectText,
                                    id: item.SelectValue + "|" + item.CountryCode + "|" + item.CountryName
                                };
                            })
                    };
                }
            }
        });
    };

    this.InitActions = function () {
        // currency change - set the symbol
        setCurrencySymbol($("#ddlCurrency").val());
        $("#ddlCurrency").change(function () {
            setCurrencySymbol($(this).val());
        });

        $("#ddlDestinationLocation").on('select2:select', function (e) {
            if (!initialDestinationLoad) {
                var location = $(this).val();
                if (location !== null) {
                    var locArr = location.split('|');
                    if (locArr.length === 3) {
                        var countryCode = locArr[1];
                        // set country 
                        if ($("#ddlDestinationCountry").val() === "") {
                            destinationLocationSelected = true;
                            $("#ddlDestinationCountry").val(countryCode).trigger("change");
                        }
                        self.LoadRegionsByCountry(countryCode, $("#ddlDestinationRegion"));
                    }
                }
            }
            initialDestinationLoad = false;
        });

        $("#ddlDestinationRegion").on('change', function (e) {
            self.LoadCountriesByRegion($("#ddlDestinationRegion").val(), $("#ddlDestinationCountry"));
        });


        $("#ddlOriginLocation").on('select2:select', function (e) {
            if (!initialOrginLoad) {
                var location = $("#ddlOriginLocation").val();
                if (location !== null) {
                    var locArr = location.split('|');
                    if (locArr.length === 3) {
                        var countryCode = locArr[1];
                        // set country 
                        if ($("#ddlOriginCountry").val() === "") {
                            originLocationSelected = true;
                            $("#ddlOriginCountry").val(countryCode).trigger("change");
                        }
                        self.LoadRegionsByCountry(countryCode, $("#ddlOriginRegion"));
                    }
                }
            }
            initialOrginLoad = false;
        });

        $("#ddlOriginRegion").on('change', function (e) {
            self.LoadCountriesByRegion($("#ddlOriginRegion").val(), $("#ddlOriginCountry"));
        });

        // enable/disable origin location
        self.EnableDisableOriginLocation(laneId !== 0);
        $("#ddlOriginCountry").on('select2:select', function (e) { 
            self.EnableDisableOriginLocation();
        });
        $("#ddlService").on('select2:select', function (e) {  
            self.EnableDisableOriginLocation();
        });

        // enable/disable destination location
        self.EnableDisableDestinationLocation(laneId !== 0);
        $("#ddlDestinationCountry").on('select2:select', function (e) {  
            self.EnableDisableDestinationLocation();
        });
        $("#ddlService").on('select2:select', function (e) {   
            self.EnableDisableDestinationLocation();
        });

        // Calculate profit
        self.CalculateProfit();

        var selectedVolume = $("#lblVolumeUnit").text();
        if (selectedVolume !== '') {
            $("#ddlVolumeUnit").val(selectedVolume);
        }

        // save lane
        $("#btnSaveLane").unbind("click").click(function () {
            self.SaveLane();
        });

        // save lane
        $("#btnCancel, .closeX").unbind("click").click(function () {
            location.href = "/Deals/DealDetail/dealdetail.aspx?dealId=" + dealId + "&dealsubscriberId=" + subscriberId;
        });


        // delete lane
        $("#btnDelete").unbind("click").click(function () {
            self.DeleteLane();
        });

    };



    this.LoadRegionsByCountry = function (countryCode, ddl) { 
        if (countryCode && countryCode !== "") {
            $.ajax({
                type: "GET",
                url: "/api/region/GetRegionsForDropdown?subscriberId=" + subscriberId + "&countryCode=" + countryCode,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (response) {
                    if (response.length > 0)
                        ddl.val(response[0].SelectText).trigger('change.select2');
                    // ddl.val(response[0].SelectText).trigger('change');
                },
                error: function () { }
            });
        }

    };

    this.LoadCountriesByRegion = function (regionName, ddl) {

        if (regionName !== null) {
            ddl.select2({
                minimumResultsForSearch: -1,
                minimumInputLength: 0,
                placeholder: "Country",
                ajax: {
                    url: function (obj) {
                        return "/api/region/GetCountriesByRegionForDropdown?subscriberId=" + subscriberId + "&regionName=" + encodeURIComponent( regionName);
                    },
                    dataType: "json",
                    timeout: 50000,
                    type: "GET",
                    data: '',
                    processResults: function (data) {
                        return {
                            results: $.map(data,
                                function (item) {
                                    return {
                                        text: item.SelectText,
                                        id: item.SelectValue
                                    };
                                })
                        };
                    }
                }
            });

            ddl.empty().trigger("change");
            if (ddl.attr('id') === "ddlOriginCountry") {
                $("#ddlOriginLocation").empty().trigger("change");
            } else if (ddl.attr('id') === "ddlDestinationCountry") {
                $("#ddlDestinationLocation").empty().trigger("change");
            }
        } else {
            ddl.empty().trigger("change");
        }
    };


    this.EnableDisableOriginLocation = function () {
        var originCountry = $("#ddlOriginCountry").val(); 
        if (originCountry !== "0") {
            // clear origin location
            if (!originLocationSelected) {
                $("#ddlOriginLocation").val("0").trigger("change");
            }
            self.LoadRegionsByCountry(originCountry, $("#ddlOriginRegion"));
        }
        originLocationSelected = false;
    };


    this.EnableDisableDestinationLocation = function () {
        var destinationCountry = $("#ddlDestinationCountry").val();
        if (destinationCountry !== "0") {
            // clear destination location
            if (!destinationLocationSelected) {
                $("#ddlDestinationLocation").val("0").trigger("change");
            }
            self.LoadRegionsByCountry(destinationCountry, $("#ddlDestinationRegion"));
        }
        destinationLocationSelected = false;
    };

    // Save Lane
    this.SaveLane = function () {
        $("#divLaneAddEdit").find(".error").removeClass("error");

        // validate
        if (!$.isNumeric($("#txtProfit").val()) || parseFloat($("#txtProfit").val()) < 0) {
            $("#txtProfit").addClass("error");
        }
        if (!$.isNumeric($("#txtRevenue").val()) || parseFloat($("#txtRevenue").val()) < 0) {
            $("#txtRevenue").addClass("error");
        }
        // if revenue is over one million, show error
        if ($.isNumeric($("#txtRevenue").val()) && $("#ddlCurrency").val() === "USD" && parseFloat($("#txtRevenue").val()) > 5000000) {
            $("#txtRevenue").addClass("error");
        }
        if (!$.isNumeric($("#txtVolume").val()) || parseFloat($("#txtVolume").val()) < 0) {
            $("#txtVolume").addClass("error");
        }

        if ($("#divLaneAddEdit").find(".error").length > 0) {
            return;
        }
        var lane = new Object();

        if ($("#ddlCurrency").val() !== "" && $("#ddlCurrency").val() !== null) {
            lane.CurrencyCode = $("#ddlCurrency").val().split('|')[0];
        }

        // destination location
        if ($("#ddlDestinationCountry").val() !== "" && $("#ddlDestinationCountry").val() !== null) {
            lane.DestinationCountryCode = $("#ddlDestinationCountry").val();
        }

        // origin location
        if ($("#ddlOriginCountry").val() !== "" && $("#ddlOriginCountry").val() !== null) {
            lane.OriginCountryCode = $("#ddlOriginCountry").val();
        }

        lane.DestinationRegionName = $("#ddlDestinationRegion").val();
        lane.OriginRegionName = $("#ddlOriginRegion").val();
        lane.ProfitPercent = $("#txtProfit").val();
        lane.ProfitUnitOfMeasure = $("#ddlProfitType").val();
        lane.Revenue = parseFloat($("#txtRevenue").val());
        lane.DealId = dealId;
        lane.LaneId = laneId;
        var service = $("#divNav").find('.btn-group button.active').attr('data-name');
        switch (service.toLowerCase()) {
            case "air":
                lane.Service = "Air";
                break;
            case "ocean":
                lane.Service = "Ocean " + $("#ddlLoadOcean").val().toUpperCase();
                break;
            case "brokerage":
                lane.Service = "Brokerage";
                break;
            case "ror":
            case "road":
            case "road ltl":
                lane.Service = "Road " + $("#ddlLoadRoad").val().toUpperCase();
                break;
            case "warehouse":
                lane.Service = "Warehouse";
                break;
            case "logistics":
                lane.Service = "Logistics";
                break;
            default: break;
        }

        lane.Comments = $("#txtComments").val();
        lane.ConsigneeCompany = $("#txtConsigneeName").val();
        lane.ShipperCompany = $("#txtOriginShipper").val();
        lane.ShippingFrequency = $("#ddlShipmentFrequency").val();
        lane.SubscriberId = subscriberId;
        lane.UpdateUserId = userId;
        lane.VolumeAmount = parseFloat($("#txtVolume").val());
        lane.VolumeUnit = $ddlVolumeUnit.val();

        if (!lane.ProfitUnitOfMeasure !== "Percentage") {
            lane.ProfitAmount = parseFloat($("#txtProfit").val());
        }

        if (lane.Service === "Air") {
            if ($("#ddlOriginLocation").val() !== "" && $("#ddlOriginLocation").val() !== null) {
                lane.OriginIataCode = $("#ddlOriginLocation").val();
            }
            lane.DestinationIataCode = $("#ddlDestinationLocation").val();
        } else {

            if ($("#ddlOriginLocation").val() !== "" && $("#ddlOriginLocation").val() !== null) {
                lane.OriginUnlocoCode = $("#ddlOriginLocation").val();
            }
            lane.DestinationUnlocoCode = $("#ddlDestinationLocation").val();
        }

        if ($('#ddlReceive').val() !== '') {
            lane.ReceiveFrom3PL = $("#ddlReceive").val().join(",");
        }
        if ($('#ddlSpecialRequirements').val() !== '') {
            lane.SpecialRequirements = $("#ddlSpecialRequirements").val().join(",");
        }
        if ($('#ddlServiceLocation').val() !== '') {
            lane.ServiceLocation = $("#ddlServiceLocation").val();
        }

        lane.RequiresBarcode = $('#chkRequireBarcode').is(":checked");
        lane.TrackAndTrace = $('#chkTracking').is(":checked");
        lane.CustomerPickUpAtWarehouse = $('#chkCustomerPickup').is(":checked");

        $.ajax({
            type: "POST",
            url: "/api/lane/SaveLane/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(lane),
            success: function (response) {
                removeSpinner();
                if (response > 0) {
                    location.href = "/Deals/DealDetail/dealdetail.aspx?dealId=" + dealId + "&dealsubscriberId=" + subscriberId;
                }
                else {
                    swal({
                        title: "Saving Failed",
                        text: "Saving Failed",
                        type: "error",
                        showConfirmButton: true,
                        allowOutsideClick: false
                    });
                }
            },
            beforeSend: function () {
                addSpinner();
            },
            error: function () { }
        });
    };

    this.RetrieveLane = function () {
        if (laneId > 0) {
            $(".page-title").html("Edit Lane");
            $.ajax({
                type: "GET",
                url: "/api/lane/GetLane/?laneId=" + laneId + "&subscriberId=" + subscriberId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (response) {
                    swal.close();
                    if (response) {
                        self.BindLane(response);
                    }
                },
                beforeSend: function () {
                    swal({
                        text: "Retrieving Lane" + "...",
                        title: "<img src='/_content/_img/loading_40.gif'/>",
                        showConfirmButton: false,
                        allowOutsideClick: false,
                        html: false
                    });
                },
                error: function () { }
            });
        }
    };

    this.BindLane = function (laneModel) {

        // bind lane
        var lane = laneModel.Lane;

        // set deal id
        dealId = lane.DealId;

        // set the service
        switch (lane.Service.toLowerCase()) {
            case "air":
                $("#btnAir").click();
                break;
            case "ocean lcl":
                $("#ddlLoadOcean").val("lcl").trigger("change");
                $("#btnOcean").click();
                break;
            case "ocean fcl":
                $("#ddlLoadOcean").val("fcl").trigger("change");
                $("#btnOcean").click();
                break;
            case "ocean roro-breakbulk":
            case "ocean roro":
                $("#ddlLoadOcean").val("roro").trigger("change");
                $("#btnOcean").click();
                break;
            case "brokerage":
                $("#btnBrokerage").click();
                break;
            case "road ":
            case "road ltl":
                $("#ddlLoadRoad").val("ltl").trigger("change");
                $("#btnRoad").click();
                break;
            case "road ftl":
                $("#ddlLoadRoad").val("ftl").trigger("change");
                $("#btnRoad").click();
                break;
            case "road cbm":
                $("#ddlLoadRoad").val("cbm").trigger("change");
                $("#btnRoad").click();
                break;
            case "road expedited":
                $("#ddlLoadRoad").val("expedited").trigger("change");
                $("#btnRoad").click();
                break;
            case "warehouse":
                $("#btnOcean").click();
                break;
            case "logistics":
                $("#btnLogistics").click();
                break;
            default: break;
        }

        $("#txtRevenue").val(lane.Revenue);
        $("#txtProfit").val(lane.ProfitPercent);
        $("#txtVolume").val(lane.VolumeAmount);
        $("#ddlShipmentFrequency").val(lane.ShippingFrequency).trigger("change");

        var volumeUnit = $('#ddlVolumeUnit option').filter(function () {
            return this.value.toLowerCase() === lane.VolumeUnit.toLowerCase();
        }).attr('value');
        $('#ddlVolumeUnit').val(volumeUnit).trigger("change");
        $("#lblVolumeUnit").val(lane.VolumeUnit);
        $("#txtOriginShipper").val(lane.ShipperCompany);
        $("#txtConsigneeName").val(lane.ConsigneeCompany);
        $("#txtComments").val(lane.Comments);
        $("#ddlCurrency").val(lane.CurrencyCode).trigger("change");

        $("#ddlProfitType").val(lane.ProfitUnitOfMeasure).trigger("change");
        $("#ddlOriginRegion").val(lane.OriginRegionName).trigger("change.select2");
        $("#ddlOriginCountry").val(lane.OriginCountryCode).trigger("change.select2");
        $("#ddlDestinationRegion").val(lane.DestinationRegionName).trigger("change.select2");
        $("#ddlDestinationCountry").val(lane.DestinationCountryCode).trigger("change.select2");

        if (lane.ReceiveFrom3PL && lane.ReceiveFrom3PL !== '')
            $('#ddlReceive').val(lane.ReceiveFrom3PL.split(',')).trigger('change');
        if (lane.SpecialRequirements && lane.SpecialRequirements !== '')
            $('#ddlSpecialRequirements').val(lane.SpecialRequirements.split(',')).trigger('change');
        if (lane.ServiceLocation && lane.ServiceLocation !== '')
            $('#ddlServiceLocation').val(lane.ServiceLocation).trigger('change');

        if (lane.RequiresBarcode)
            $('#chkRequireBarcode').prop('checked', true);
        if (lane.TrackAndTrace)
            $('#chkTracking').prop('checked', true);
        if (lane.CustomerPickUpAtWarehouse)
            $('#chkCustomerPickup').prop('checked', true);



        // set origin location
        if (laneModel.OriginLocation !== null) {
            initialOrginLoad = true;
            $('#ddlOriginLocation').append($('<option>',
                {
                    value: laneModel.OriginLocation.SelectValue,
                    text: laneModel.OriginLocation.SelectText
                }));
            originLocationSelected = true;
            $("#ddlOriginLocation").val(laneModel.OriginLocation.SelectValue).trigger("change.select2");

        } else {
            initialOrginLoad = false;
        }

        // set destination location 
        if (laneModel.DestinationLocation !== null) {
            initialDestinationLoad = true;
            $('#ddlDestinationLocation').append($('<option>',
                {
                    value: laneModel.DestinationLocation.SelectValue,
                    text: laneModel.DestinationLocation.SelectText
                }));
            destinationLocationSelected = true;
            $("#ddlDestinationLocation").val(laneModel.DestinationLocation.SelectValue).trigger("change.select2");

        } else
            initialDestinationLoad = false;

        // init actions
        self.InitActions();
    };

};

// Lanes - Tab Actions
function initTabActions() {
    $('#divNav .btn-group button').click(function () {
        $('#divNav .btn-group button').removeClass('active');
        $(this).addClass('active');
        $('#divVolumeRevenueProfit .loadOcean-col').hide();
        $('#divOriginDestination .row.logi-info').hide();
        $('#divOriginDestination .row.org-des').css('visibility', 'visible').show();
        $('#divOriginDestination').removeClass('textHeight');
        $('#divVolumeRevenueProfit .loadRoad-col').hide();
        $('#divBrokerage').hide();
        $('#ddlVolumeUnit+.select2').show();
        $('.vol-col #txtVolume').removeClass('large');
    });

    $('#btnBrokerage').click(function () {
        $('#divOriginDestination .row.org-des').hide();
        $('#divOriginDestination').addClass('textHeight');
        // $('#ddlVolumeUnit+.select2').hide();
        $('#divBrokerage').show();
        // $('.vol-col #txtVolume').addClass('large');
    });

    $('#btnLogistics').click(function () {
        $('#divOriginDestination .row.logi-info').css('display', 'flex');
        $('#divOriginDestination .row.org-des').hide();
    });

    $('#btnOcean').click(function () {
        $('#divVolumeRevenueProfit .loadOcean-col').show();
    });

    $('#btnRoad').click(function () {
        $('#divVolumeRevenueProfit .loadRoad-col').show();
    });

    $('#btnCancel,#btnOcean,#btnRoad,#btnLogistics,#btnBrokerage, #btnAir').click(function () {
        Clear();
        var object = new Lanes();
        object.UpdateDropdownOptions();
    });

    $("#ddlLoadOcean, #ddlLoadRoad").change(function () {
        var object = new Lanes();
        object.UpdateDropdownOptions();
    });

    $('#txtVolume, #txtProfit, #txtRevenue').bind('blur', function () {
        var selectedVolume = $("#txtVolume").val();
        var profitType = $("#txtProfit").val();
        var revenue = parseInt($("#txtRevenue").val());
        if (selectedVolume !== "" && profitType !== "" && revenue !== "") {
            var object = new Lanes();
            object.CalculateProfit();
        }
        if ($("#txtProfit").val() === "")
            $("#txtProfit").val("");
    });

    $("#txtProfit").focus(function () {
        if ($(this).val() === "0")
            $(this).val("");
    });



    $("#txtProfit").change(function () {
        CheckPercentage();
    });

    $("#ddlProfitType").change(function () {
        var object = new Lanes();
        object.CalculateProfit();

        CheckPercentage();
    });

    $("#ddlVolumeUnit").change(function () {
        var object = new Lanes();
        object.CalculateProfit();
    });

    function CheckPercentage() {
        if ($("#ddlProfitType").val() === "Percentage") {
            $(".profitPrefix").html("%");
            var profit = $("#txtProfit").val();

            if ($.isNumeric(profit) === false || profit < 0 || profit > 100) {
                $("#txtProfit").val("");
            }
        } else {
            $(".profitPrefix").html($(".revenuePrefix").html());
        }
    }

    function Clear() {
        $("input[type=text]").not("#txtOriginShipper").val("");
        $("textarea").val("");
        $("input[type=text],textarea").removeClass('error');
        $('#select2-ddlOriginRegion-container,#select2-ddlDestinationRegion-container,#select2-ddlOriginCountry-container,#select2-ddlDestinationCountry-container').removeAttr("title");
        $('#select2-ddlOriginRegion-container,#select2-ddlDestinationRegion-container,#select2-ddlOriginCountry-container,#select2-ddlDestinationCountry-container').text("");
        $('#select2-ddlOriginRegion-container,#select2-ddlDestinationRegion-container').append('<span class="select2-selection__placeholder">Region</span>');
        $('#select2-ddlOriginCountry-container,#select2-ddlDestinationCountry-container').append('<span class="select2-selection__placeholder">Country</span>');
        $("#select2-ddlOriginLocation-container,#select2-ddlDestinationLocation-container").removeAttr("title");
        $("#select2-ddlOriginLocation-container,#select2-ddlDestinationLocation-container").text("");
        $('#select2-ddlOriginLocation-container,#select2-ddlDestinationLocation-container').append('<span class="select2-selection__placeholder">Location</span>');
        $('#ddlReceive,#ddlSpecialRequirements').val([]);
    }

    $("#ddlCurrency").change(function () {
        setCurrencySymbol($(this).val());
    });

}

function setCurrencySymbol(selectedCurrency) {
    var $revenueCurrency = $(".revenuePrefix");
    $revenueCurrency.html("");
    $.ajax({
        type: "GET",
        url: "/api/dropdown/GetCurrencySymbolFromCode?currencycode=" + selectedCurrency,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 50000,
        data: '',
        success: function (symbol) {
            if (symbol !== '') {
                $revenueCurrency.removeClass("hide");
                $revenueCurrency.html(symbol);
                if ($(".profitPrefix").html() !== "%")
                    $(".profitPrefix").html(symbol);
            } else {
                $revenueCurrency.html(selectedCurrency);
                if ($(".profitPrefix").html() !== "%")
                    $(".profitPrefix").html(selectedCurrency);
                //  $revenueCurrency.addClass("hide");
            }
        },
        beforeSend: function () {
        },
        error: function () {
        }
    });
}






// Lanes Page - Tab Actions
$(document).ready(function () {
    $('#divNav .btn-group button').click(function () {
        $('#divNav .btn-group button').removeClass('active');
        $(this).addClass('active');
        $('#divVolumeRevenueProfit .loadOcean-col').hide();
        $('#divOriginDestination .row.logi-info').hide();
        if ($(window).width() <= 767) {
            $('#divOriginDestination .row.org-des').show();
        } else {
            $('#divOriginDestination .row.org-des').css('visibility', 'visible').show();
        }
        $('#divOriginDestination').removeClass('textHeight');
        $('#divVolumeRevenueProfit .loadRoad-col').hide();
        $('#divBrokerage').hide();
    });
    $('#btnBrokerage').click(function () {
        if ($(window).width() <= 767) {
            $('#divOriginDestination .row.org-des').hide();
        } else {
            $('#divOriginDestination .row.org-des').hide();
        }
        $('#divOriginDestination').addClass('textHeight');
        $('#divBrokerage').show();
    });
    $('#btnLogistics').click(function () {
        $('#divOriginDestination .row.logi-info').css('display', 'flex');
        $('#divOriginDestination .row.org-des').hide();
    });
    $('#btnOcean').click(function () {
        $('#divVolumeRevenueProfit .loadOcean-col').show();
    });
    $('#btnRoad').click(function () {
        $('#divVolumeRevenueProfit .loadRoad-col').show();
    });

    $("input").keypress(function () {
        $(this).removeClass('error');
    });

    if (laneId === 0) {
        $("#txtOriginShipper").val($("#lblCompanyName").text());
    }

    $('#ddlProfitType').trigger('change');
});