var subscriberId = parseInt($("#lblSubscriberId").text());
var locationId = parseInt($("#lblLocationId").text());
var userId = parseInt($("#lblUserId").text());
var $divLocationSetup = $("#divLocationSetup");

// fields
var $txtAddress = $("#txtAddress");
var $txtCity = $("#txtCity");
var $txtComments = $("#txtComments");
var $ddlCountry = $("#ddlCountry");
var $ddlDistrict = $("#ddlDistrict");
var $txtFax = $("#txtFax");
var $txtLocationCode = $("#txtLocationCode");
var $txtLocationName = $("#txtLocationName");
var $ddlLocationType = $("#ddlLocationType");
var $txtPhone = $("#txtPhone");
var $txtPostalCode = $("#txtPostalCode");
var $ddlRegion = $("#ddlRegion");
var $txtStateProvince = $("#txtStateProvince");


$(function () {
    // setup page
    new PageSetup().Init();
    // initialize locations
    new Location().Init();

    if (locationId) {
        $('title').text('Edit Location');
    } else {
        $('title').text('Add Location');
    }
});


var PageSetup = function () {
    var self = this;

    this.Init = function () {

        $("#btnSave").unbind("click").click(function () {
            $divLocationSetup.submit();
        });

        $("#btnCancel").unbind("click").click(function (event) {
             location.href = "/Admin/Locations/Locations.aspx?tab=locations";
		});

        if (locationId > 0) {
            $("#btnDelete").removeClass("hide");
            $("#btnDelete").unbind("click").click(function () {
                new Location().DeleteLocation();
            });
        }

        self.SetupSelect2Dropdowns();
    };

    this.SetupSelect2Dropdowns = function () {
        $ddlLocationType.select2({ "placeholder": 'Location Type...', minimumResultsForSearch: Infinity, allowClear: true });
        $ddlCountry.select2({ "placeholder": 'Country...', minimumResultsForSearch: Infinity });
        $ddlDistrict.select2({ "placeholder": 'District...', minimumResultsForSearch: Infinity });
        $ddlRegion.select2({ "placeholder": 'Region...', minimumResultsForSearch: Infinity });
    };
};


var Location = function () {
    var self = this;

    this.Init = function () {
        // init validator
        self.InitSaveValidator();
    };

    this.InitSaveValidator = function () {
        // select2 dropdown validator
        $ddlLocationType.on('select2:select', function (evt) { $(this).valid(); });
        $ddlCountry.on('select2:select', function (evt) { $(this).valid(); });
        $ddlDistrict.on('select2:select', function (evt) { $(this).valid(); });
        $ddlRegion.on('select2:select', function (evt) { $(this).valid(); });

        // add the rule here
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return arg !== value && value !== null && value !== "0";
        }, "");

        // validate
        $divLocationSetup.validate({
            rules: {
                txtLocationName: { required: true },
                txtLocationCode: { required: true },
                ddlLocationType: { valueNotEquals: "null" },
                ddlCountry: { valueNotEquals: "null" },
                ddlRegion: { valueNotEquals: "null" }
            }, messages: {
                txtLocationName: "Please enter the location name",
                txtLocationCode: "Please enter the location code",
                ddlLocationType: "Please select a location type",
                ddlCountry: "Please select a country",
                ddlRegion: "Please select a region"
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveLocation();
            }
        });
    };

    this.SaveLocation = function () {
        var location = new Object();
        location.LocationId = locationId;
        location.SubscriberId = subscriberId;
        location.Address = $txtAddress.val();
        location.City = $txtCity.val();
        location.Comments = $txtComments.val();
        location.CountryName = $ddlCountry.val();
        location.DistrictCode = $ddlDistrict.val();
        location.Fax = $txtFax.val();
        location.LocationCode = $txtLocationCode.val();
        location.LocationName = $txtLocationName.val();
        location.LocationType = $ddlLocationType.val();
        location.Phone = $txtPhone.val();
        location.PostalCode = $txtPostalCode.val();
        location.StateProvince = $txtStateProvince.val();
        location.RegionName = $ddlRegion.val();
        location.UpdateUserId = userId;

        var request = new Object();
        request.Location = location;

        // AJAX to save the location 
        $.ajax({
            type: "POST",
            url: "/api/location/SaveLocation",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(request),
            success: function (response) {
                // remove spinner
                $("#divSpinner").addClass("hide");
                if (response > 0) {
                    window.location.href = "/Admin/Locations/Locations.aspx?tab=locations";
                } else {
                    alert("Error saving location!");
                }
            }, beforeSend: function () {
                // add spinner
                $("#divSpinner").removeClass("hide");
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteLocation = function () {
        swal({
            title: translatePhrase("Delete Location!"),
            text: translatePhrase("Are you sure you want to delete this location?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!"),
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/location/deletelocation/?locationid=" + locationId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        // remove spinner
                        $("#divSpinner").addClass("hide");
                        if (data) {
                            location.href = "/Admin/Locations/Locations.aspx?tab=locations";
                        }
                    }, beforeSend: function () {
                        // add spinner
                        $("#divSpinner").removeClass("hide");
                    }
                });
            }
        });
    };

};
