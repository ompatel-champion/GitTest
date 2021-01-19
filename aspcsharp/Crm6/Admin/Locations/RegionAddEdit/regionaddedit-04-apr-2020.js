var subscriberId = parseInt($("#lblSubscriberId").text());
var regionId = parseInt($("#lblRegionId").text());
var userId = parseInt($("#lblUserId").text());
var userIdGlobal = $("#lblUserIdGlobal").text();
var $divRegionSetup = $("#divRegionSetup"); 
var $txtRegionName = $("#txtRegionName"); 


$(function () { 
    // setup page
    new PageSetup().Init();
    // initialize regions
    new Regions().Init();

    if (regionId > 0) {
        $('title').text('Edit Region');
    } else {
        $('title').text('Add Region');
    }
});


var PageSetup = function () {
 
    this.Init = function () {

        $("#btnSave").unbind("click").click(function () {
            $divRegionSetup.submit();
        });

        $("#btnCancel").unbind("click").click(function (event) {
            location.href = "/Admin/Locations/Locations.aspx?tab=regions";
        });

        if (regionId > 0) {
            $("#btnDelete").removeClass("hide");
            $("#btnDelete").unbind("click").click(function () {
                new Regions().DeleteRegion();
            });
        }
    };
};


var Regions = function () {
    var self = this;

    this.Init = function () {
        // init validator
        self.InitSaveValidator();
    };

    this.InitSaveValidator = function () {
        // add the rule here
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return arg !== value && value !== null && value !== "0";
        }, "");

        // validate
        $divRegionSetup.validate({
            rules: {
                txtRegion: { required: true }
            }, messages: {
                txtRegion: "Please enter the region name"
            },
            errorPlacement: function ($error, $element) { 
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveRegion();
            }
        });
    };

    this.SaveRegion = function () {
        var region = new Object();
        region.RegionId = regionId;
        region.SubscriberId = subscriberId;
        region.RegionName = $txtRegionName.val(); 
        region.UpdateUserIdGlobal = userIdGlobal;

        // AJAX to save the region
        $.ajax({
            type: "POST",
            url: "/api/region/SaveRegion",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(region),
            success: function (response) {
                // remove spinner
                $("#divSpinner").addClass("hide");
                if (parseInt(response) > 0) {
                    location.href = "/Admin/Locations/Locations.aspx?tab=regions";
                } else {
                    alert("Error saving region.");
                }
            }, beforeSend: function () {
                // add spinner
                $("#divSpinner").removeClass("hide");
            }, error: function (request ) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteRegion = function () {
        swal({
            title: translatePhrase("Delete Region!"),
            text: translatePhrase("Are you sure you want to delete this region?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!"),
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/region/deleteregion/?regionid=" + regionId + "&userIdGlobal=" + userIdGlobal + "&subscriberId=" + subscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        // remove spinner
                        $("#divSpinner").addClass("hide");
                        if (data) {
                            location.href = "/Admin/Locations/Locations.aspx?tab=regions";
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
