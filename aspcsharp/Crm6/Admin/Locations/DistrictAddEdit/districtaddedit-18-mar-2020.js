var subscriberId = parseInt($("#lblSubscriberId").text());
var districtId = parseInt($("#lblDistrictId").text());
var userId = parseInt($("#lblUserId").text());
var $divDistrictSetup = $("#divDistrictSetup");

var $txtDistrictName = $("#txtDistrictName");
var $txtDistrictCode = $("#txtDistrictCode");
var $ddlCountry = $("#ddlCountry");


$(function () {
    // setup page
    new PageSetup().Init();
    // initialize districts
    new Districts().Init();

    if (districtId) {
        $('title').text('Edit District');
    } else {
        $('title').text('Add District');
    }
});


var PageSetup = function () {
    var self = this;

    this.Init = function () {

        $("#btnSave").unbind("click").click(function () {
            $divDistrictSetup.submit();
        });

        $("#btnCancel").unbind("click").click(function (event) {
            location.href = "/Admin/Locations/Locations.aspx?tab=districts";
        });

        if (districtId > 0) {
            $("#btnDelete").removeClass("hide");
            $("#btnDelete").unbind("click").click(function () {
                new Districts().DeleteDistrict();
            });
        }
        $ddlCountry.select2({ "placeholder": 'Country...', minimumResultsForSearch: Infinity});
    };
};


var Districts = function () {
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
            return arg !== value && value !== null && value !== null && value !== "0";
        }, "");

        // validate
        $divDistrictSetup.validate({
            rules: {
                txtDistrict: { required: true },
                txtDistrictCode: { required: true },
                ddlCountry: { valueNotEquals: "null" },
            }, messages: {
                txtDistrict: "Please enter the district name",
                txtDistrictCode: "Please enter the district code",
                ddlCountry: "Please select a country"
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveDistrict();
            }
        });
    };

    this.SaveDistrict = function () {
        var district = new Object();
        district.DistrictId = districtId;
        district.SubscriberId = subscriberId;
        district.DistrictName = $txtDistrictName.val();
        district.DistrictCode = $txtDistrictCode.val();
        district.CountryName = $ddlCountry.val();
        district.UpdateUserId = userId;

        // AJAX to save the district
        $.ajax({
            type: "POST",
            url: "/api/district/SaveDistrict",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(district),
            success: function (response) {
                // remove spinner
                $("#divSpinner").addClass("hide");
                if (parseInt(response) > 0) {
                    location.href = "/Admin/Locations/Locations.aspx?tab=districts";
                } else {
                    alert("Error saving district.");
                }
            },
            beforeSend: function () {
                // add spinner
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteDistrict = function () {
        swal({
            title: translatePhrase("Delete District!"),
            text: translatePhrase("Are you sure you want to delete this district?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!"),
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/district/deletedistrict/?districtid=" + districtId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        // remove spinner
                        $("#divSpinner").addClass("hide");
                        if (data) {
                            location.href = "/Admin/Locations/Locations.aspx?tab=districts";
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
