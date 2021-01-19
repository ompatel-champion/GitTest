var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var userIdGlobal = $("#lblUserIdGlobal").text();
var $tblLocations = $("#tblLocations");
var $tblDistricts = $("#tblDistricts");
var $tblRegions = $("#tblRegions");


// library (local utility type methods)
const lib = {
    // kebab-case to camelCase
    kebabToCamel: function (str) {
        return str.replace(/-([a-z])/g, function (a) { return a[1].toUpperCase(); });
    }
};



// border tabs
$(function () { 
    $('.border-tabs .btab,.nav-tabs a').click(function () { 
        var tcID = $(this).attr('data-id');
        $('.btab-content').hide();
        $(tcID).fadeIn();
        $('.border-tabs .btab').removeClass('active');
        $(this).addClass('active');

        $('.searchBlock').hide();
        if (tcID === '#divLocationsTab') {
            $('#locationSearch').show();
        }
        else if (tcID === '#divDistrictsTab') {
            $('#districtSearch').show();
        }
        else if (tcID === '#divGlobalLocationsTab') {
            $('#globalLocationSearch').show();
            $('#txtGlobalKeyword').focus();
        }
        else if (tcID === '#divRegionsTab') {
            $('#regionSearch').show();
        }
    });

    // listen for enter key and perform search
    $('#txtKeyword').keypress(function (event) {
        var keycode = event.keyCode ? event.keyCode : event.which;
        if (keycode === '13') {
            $("#btnSearch")[0].click();
            return false;
        }
    });



    // panel dropdown menu
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


// init
$(function () {
    new Locations().Init();
    new Districts().Init();
    new GlobalLocations().Init();
    new Regions().Init();
});



var Locations = function () {
    var self = this;

    // init locations
    this.Init = function () {
        // new location
        $(".new-location").unbind('click').click(function () {
            location.href = "/Admin/Locations/LocationAddEdit/LocationAddEdit.aspx?locationId=0";
        });
        self.BindActions();
    };

    this.BindActions = function () {
        $tblLocations.find("tr").each(function () {
            var $tr = $(this);
            // edit location
            $tr.find(".edit-location").unbind('click').click(function () {
                var locationId = $(this).closest("tr").attr("data-id");
                location.href = "/Admin/Locations/LocationAddEdit/LocationAddEdit.aspx?locationId=" + locationId;
            });
        });
    };
};


var Districts = function () {
    var self = this;

    // init district
    this.Init = function () {
        // new district
        $(".new-district").unbind('click').click(function () {
            location.href = "/Admin/Locations/DistrictAddEdit/DistrictAddEdit.aspx?districtid=0";
        });
        self.BindActions();
    };

    this.BindActions = function () {
        $tblDistricts.find("tr").each(function () {
            var $tr = $(this);
            // edit district
            $tr.find("[data-action='edit']").unbind('click').click(function () {
                var districtid = $(this).closest("tr").attr("data-id");
                location.href = "/Admin/Locations/DistrictAddEdit/DistrictAddEdit.aspx?districtid=" + districtid;
            });
            // delete district
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var districtId = $(this).closest("tr").attr("data-id");
                self.DeleteDistrict(districtId);
            });
        });
    },

        this.DeleteDistrict = function (districtId) {
            swal({
                title: translatePhrase("Delete District!"),
                text: translatePhrase("Are you sure you want to delete this district?"),
                type: "error",
                showCancelButton: true,
                confirmButtonColor: "#f27474",
                confirmButtonText: translatePhrase("Yes, Delete!")
            }, function () {
                $.ajax({
                    type: "GET",
                    url: "/api/district/DeleteDistrict/?districtId=" + districtId + "&userId=" + userId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        // remove spinner
                        $divSpinner.addClass("hide");
                        if (response) {
                            location.reload();
                        }
                    },
                    beforeSend: function () {
                        // add spinner
                        $divSpinner.removeClass("hide");
                    },
                    error: function (request) { }
                });
            });
        };
};


var GlobalLocations = function () {
    var self = this;
    var $tblGlobalLocations = $("#tblGlobalLocations");
    var $tbody = $tblGlobalLocations.find("tbody");
    var $divNoGlobalLocations = $("#divNoGlobalLocations");
    var $divGlobalLocaitionBeforeSearch = $("#divGlobalLocaitionBeforeSearch");



    // init global locations
    this.Init = function () {

        // search click
        $("#aGlobalSearch").unbind("click").click(function () {
            $tblGlobalLocations.addClass("hide");
            $tbody.html("");
            if ($.trim($("#txtGlobalKeyword").val()).length > 1) {
                self.LoadGlobalLocations();
            } else {
                $("#txtGlobalKeyword").addClass("error");
            }
        });


        $("#txtGlobalKeyword").on('keyup', function (e) {
            if (e.keyCode === 13) {
                $("#aGlobalSearch").click();
            }
        });

        // check for the passed QS tab and set it
        const qsTab = getQueryString("tab");
        if (qsTab) {
            if (qsTab === "globallocations") {
                var elm = $("[data-id='#divGlobalLocationsTab']");

                elm.click();
                elm.addClass("active");
                if (getQueryString("keyword") && getQueryString("keyword").length > 1) {
                    $("#txtGlobalKeyword").val(getQueryString("keyword"));
                    new GlobalLocations().LoadGlobalLocations();
                }

            }
        }


        // new global location
        $(".new-global-location").unbind('click').click(function () {
            location.href = "/Admin/Locations/GlobalLocationAddEdit/GlobalLocationAddEdit.aspx?globalLocationId=0";
        });
        self.BindActions();
    };

    this.BindActions = function () {
        $tblGlobalLocations.find("tr").each(function () {
            var $tr = $(this);
            // edit global location
            $tr.find(".edit-location").unbind('click').click(function () {
                var locationId = $(this).closest("tr").attr("data-id");
                location.href = "/Admin/Locations/GlobalLocationAddEdit/GlobalLocationAddEdit.aspx?globalLocationId=" + locationId;
            });
        });
    };


    this.LoadGlobalLocations = function () {
        $("#txtGlobalKeyword").removeClass("error");
        var request = new Object();
        request.Keyword = $.trim($("#txtGlobalKeyword").val());
        request.UserId = userId;
        $.ajax({
            type: "POST",
            url: "/api/GlobalLocation/GetGlobalLocations/",
            data: JSON.stringify(request),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                removeSpinner();
                try { $(".locations").find("#divSpinner").remove(); } catch (e) {/*ignore*/ }
                if (data.GlobalLocations.length > 0) {
                    $tblGlobalLocations.removeClass("hide");
                    $divGlobalLocaitionBeforeSearch.addClass("hide");
                    $divNoGlobalLocations.addClass("hide");
                    self.BindLocations(data.GlobalLocations);
                }
                else {
                    $tblGlobalLocations.addClass("hide");
                    $divGlobalLocaitionBeforeSearch.addClass("hide");
                    $divNoGlobalLocations.removeClass("hide");
                }

            }, beforeSend: function () {
                addSpinner();
            }
        });
    };


    this.BindLocations = function (locations) {

        $tbody.html("");
        $.each(locations, function (i, obj) {
            var $tr = $("<tr>", { "data-id": obj.GlobalLocationId });
            $tr.append($("<td/>", { "html": obj.LocationName }));
            $tr.append($("<td/>", { "html": obj.LocationCode }));
            $tr.append($("<td/>", { "html": obj.CountryName }));

            var $td = $("<td/>", { "class": "text-center" });
            if (obj.Airport)
                $td.append($("<img>", { "src": "/_content/_img/icons/temp-check.png" }));
            $tr.append($td);

            $td = $("<td/>", { "class": "text-center" });
            $chk = $("<input>", { "type": "checkbox", "class": "" });
            if (obj.InlandPort)
                $td.append($("<img>", { "src": "/_content/_img/icons/temp-check.png" }));
            $tr.append($td);

            $td = $("<td/>", { "class": "text-center" });
            $chk = $("<input>", { "type": "checkbox", "class": "" });
            if (obj.MultiModal)
                $td.append($("<img>", { "src": "/_content/_img/icons/temp-check.png" }));
            $tr.append($td);

            $td = $("<td/>", { "class": "text-center" });
            $chk = $("<input>", { "type": "checkbox", "class": "" });
            if (obj.RailTerminal)
                $td.append($("<img>", { "src": "/_content/_img/icons/temp-check.png" }));
            $tr.append($td);

            $td = $("<td/>", { "class": "text-center" });
            $chk = $("<input>", { "type": "checkbox", "class": "" });
            if (obj.RoadTerminal)
                $td.append($("<img>", { "src": "/_content/_img/icons/temp-check.png" }));
            $tr.append($td);

            $td = $("<td/>", { "class": "text-center" });
            $chk = $("<input>", { "type": "checkbox", "class": "" });
            if (obj.SeaPort) {
                $td.append($("<img>", { "src": "/_content/_img/icons/temp-check.png" }));
            }
            $tr.append($td);

            // action cell
            $td = $("<td/>", { "class": "text-center action-cell" });
            var $aEdit = $("<a/>", { "class": "hover-link edit-global-location", "title": "Edit Global Location", "data-action": "edit", "html": "Edit" });
            $td.append($aEdit);

            $aEdit.attr("href", "/Admin/Locations/GlobalLocationAddEdit/GlobalLocationAddEdit.aspx?globalLocationId=" + obj.GlobalLocationId + "&keyword=" + $.trim($("#txtGlobalKeyword").val()));
            $tr.append($td);

            $tbody.append($tr);
        });
    };

};



var Regions = function () {
    var self = this;

    // init region
    this.Init = function () {

        // new region
        $(".new-region").unbind('click').click(function () {
            location.href = "/Admin/Locations/RegionAddEdit/RegionAddEdit.aspx?regionid=0";
        });
        self.BindActions();
    };

    this.BindActions = function () {
        $tblRegions.find("tr").each(function () {
            var $tr = $(this);
            // edit region
            $tr.find("[data-action='edit']").unbind('click').click(function () {
                var regionId = $(this).closest("tr").attr("data-id");
                location.href = "/Admin/Locations/RegionAddEdit/RegionAddEdit.aspx?regionid=" + regionId;
            });
            // delete region
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var regionId = $(this).closest("tr").attr("data-id");
                self.DeleteRegion(regionId);
            });
        });
    },

        this.DeleteRegion = function (regionId) {
            swal({
                title: translatePhrase("Delete Region!"),
                text: translatePhrase("Are you sure you want to delete this region?"),
                type: "error",
                showCancelButton: true,
                confirmButtonColor: "#f27474",
                confirmButtonText: translatePhrase("Yes, Delete!")
            }).then(function (value) {
                if (value.value) {
                    $.ajax({
                        type: "GET",
                        url: "/api/region/DeleteRegion/?regionId=" + regionId + "&userIdGlobal=" + userIdGlobal + "&subscriberId=" + subscriberId,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: {},
                        success: function (response) { 
                            if (response) {
                                location.reload();
                            }
                        },
                        beforeSend: function () { 
                        },
                        error: function () { }
                    });
                }
            });
        };
};

