var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var $ddlRegion = $("#CountriesToRegions_ddlRegions");
var $divSpinner = $("#divSpinner");

// init
$(function () {
    new PageSetup().Init();
    $ddlRegion.select2({});
    $('#multiselect').multiselect();
});

var PageSetup = function () {

    this.Init = function () {
        var groupedCountries = [];
        var previousRegionId = '';

        $.ajax({
            type: "GET",
            url: '/api/CountryRegions/GetCountries?subscriberid=' + subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            timeout: 50000,
            data: '',
            success: function (response) {
                groupedCountries = response; 
                // manager ids 
                $.each(response["available"], function (i, obj) {
                    $("#multiselect").append(new Option(obj.SelectText, obj.SelectValue));
                });

                var regionId = $ddlRegion.val();
                $.each(groupedCountries[regionId], function (i, obj) {
                    $("#multiselect_to").append(new Option(obj.SelectText, obj.SelectValue));
                });
            },
            beforeSend: function () { },
            error: function () { }
        });

        $ddlRegion.change(function () {
            var regionId = $(this).val();

            if (previousRegionId !== '') {
                groupedCountries[previousRegionId] = [];
                $("#multiselect_to > option").each(function () {
                    var newone = { SelectText: this.text, SelectValue: this.value, Selected: true };
                    groupedCountries[previousRegionId].push(newone);
                });
            }
            previousRegionId = regionId;

            $("#multiselect_to").empty();
            $.each(groupedCountries[regionId], function (i, obj) {
                $("#multiselect_to").append(new Option(obj.SelectText, obj.SelectValue));
            });
        });

        $("#btnSave").unbind("click").click(function () {
            var regionId = $ddlRegion.val();

            groupedCountries[regionId] = [];
            $("#multiselect_to > option").each(function () {
                var newone = { SelectText: this.text, SelectValue: this.value, Selected: true };
                groupedCountries[regionId].push(newone);
            });

            groupedCountries['available'] = [];
            $("#multiselect > option").each(function () {
                var newone = { SelectText: this.text, SelectValue: this.value, Selected: false };
                groupedCountries['available'].push(newone);
            });

            $.ajax({
                type: "POST",
                url: "/api/CountryRegions/UpdateCountries",
                data: JSON.stringify({ GroupedCountries: groupedCountries, UserId: userId, SubscriberId: subscriberId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function ( ) {
                    $divSpinner.addClass("hide");
                }, beforeSend: function () {
                    $divSpinner.removeClass("hide");
                },
                error: function ( ) {
                    console.log('error');
                    $divSpinner.addClass("hide");
                }
            });
        });
    };

};
