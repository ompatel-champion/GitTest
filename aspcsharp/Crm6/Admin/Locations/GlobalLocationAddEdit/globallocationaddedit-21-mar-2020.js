var subscriberId = parseInt($("#lblSubscriberId").text());
var globalLocationId = parseInt($("#lblGlobalLocationId").text());
var userId = parseInt($("#lblUserId").text());
var $divGlobalLocationSetup = $("#divGlobalLocationSetup");
var searchKeyword = getQueryString("keyword");
// fields
var $ddlCountry = $("#ddlCountry");
var $txtLocationCode = $("#txtLocationCode");
var $txtLocationName = $("#txtLocationName");


$(function () {
    // setup page
    new PageSetup().Init();
    // initialize global locations
    new GlobalLocation().Init();

    if (globalLocationId) {
        $('title').text('Edit Global Location');
    } else {
        $('title').text('Add Global Location');
    }

    if (!searchKeyword) {
        searchKeyword = "";
    }
});


var PageSetup = function () {
   
    this.Init = function () {

        $("#btnSave").unbind("click").click(function () {
            $divGlobalLocationSetup.submit();
        });

        $("#btnCancel").unbind("click").click(function (event) {
            location.href = "/Admin/Locations/Locations.aspx?tab=globallocations&keyword=" + searchKeyword;
        });

        if (globalLocationId > 0) {
            $("#btnDelete").removeClass("hide");
            $("#btnDelete").unbind("click").click(function () {
                new GlobalLocation().DeleteGlobalLocation();
            });
        }

        $ddlCountry.select2({ "placeholder": 'Country...', minimumResultsForSearch: 25 });
    };
};


var GlobalLocation = function () {
    var self = this;

    this.Init = function () {
        // init validator
        self.InitSaveValidator();
    };

    this.InitSaveValidator = function () {
        // select2 dropdown validator
        $ddlCountry.on('select2:select', function (evt) { $(this).valid(); });

        // add the rule here
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return arg !== value && value !== null && value !== "0";
        }, "");

        // validate
        $divGlobalLocationSetup.validate({
            rules: {
                txtLocationName: { required: true },
                txtLocationCode: { required: true },
                ddlCountry: { valueNotEquals: "null" }
            }, messages: {
                txtLocationName: "Enter the location name",
                txtLocationCode: "Enter the location code",
                ddlCountry: "Select a country"
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveGlobalLocation();
            }
        });
    };

    this.SaveGlobalLocation = function () {
        // set global location
        var globalLocation = new Object();
        globalLocation.GlobalLocationId = globalLocationId;
        globalLocation.CountryName = $ddlCountry.val();
        globalLocation.LocationCode = $txtLocationCode.val();
        globalLocation.LocationName = $txtLocationName.val();
        globalLocation.UpdateUserId = userId;
         
        globalLocation.Airport = $("#chkAirport").is(":checked");
        globalLocation.InlandPort = $("#chkInlandPort").is(":checked");
        globalLocation.MultiModal = $("#chkMultiModal").is(":checked");
        globalLocation.RailTerminal = $("#chkRailTerminal").is(":checked");
        globalLocation.RoadTerminal = $("#chkRoadTerminal").is(":checked");
        globalLocation.SeaPort = $("#chkSeaport").is(":checked");


        // set request object
        var request = { GlobalLocation: globalLocation, SubscriberId: subscriberId };
        // AJAX to save the global location 
        $.ajax({
            type: "POST",
            url: "/api/globallocation/SaveGlobalLocation",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(request),
            success: function (response) {
                $("#divSpinner").addClass("hide");
                if (response > 0) {
                    window.location.href = "/Admin/Locations/Locations.aspx?tab=globallocations&keyword=" + searchKeyword;
                } else {
                    alert("Error saving global location!");
                }
            }, beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteGlobalLocation = function () {
        swal({
            title: translatePhrase("Delete Global Location!"),
            text: translatePhrase("Are you sure you want to delete this global location?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!"),
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/globallocation/deletegloballocation/?globallocationid=" + globalLocationId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $("#divSpinner").addClass("hide");
                        if (data) {
                            location.href = "/Admin/Locations/Locations.aspx?tab=globallocations&keyword=" + searchKeyword;
                        }
                    }, beforeSend: function () {
                        $("#divSpinner").removeClass("hide");
                    }
                });
            }
        });
    };

};
