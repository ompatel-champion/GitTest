// set global values
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var $divLocations = $("#divLocations");
var $divNoItems = $("#divNoItems");
var $locationsCards = $(".location-cards");
var $locationsTable = $(".location-table");
var viewType = $(".btn-biew-type .btn-success").attr('data-view-type');


$(function () {
    // init location
   new Locations().Init();
});


var Locations = function () {
    var self = this;

    // init locations
    this.Init = function () {
        // new location
        $(".new-location").unbind('click').click(function () {
            self.OpenAddEditLocation(0);
        });
        $("[data-action='edit']").unbind('click').click(function () {
            self.OpenAddEditLocation($(this).closest("tr").attr("data-id"));
        }); 
    }
 
    // open add location
    this.OpenAddEditLocation = function (lid) {
        var locationId = parseInt(lid);
        var iframeUrl = "/Admin/Locations/LocationAddEdit/LocationAddEdit.aspx?locationId=" + locationId;
        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframeLocationtAddEdit" }).launchModal({
            title: (locationId > 0 ? "Edit" : "Add New") + " Location",
            modalClass: "modal-1100",
            btnSuccessText: "Save",
            maxHeight: "550px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
                var frameWrapper = window.parent.document.getElementById("iframeLocationAddEdit");
                var iframe = frameWrapper.getElementsByTagName("iframe")[0];
                iframe.contentWindow.SaveLocation();
            }
        });
    }
}

 
// gets location picture
function getLocationImage($img, locationId) {
    $.getJSON("/api/location/getlocationpic/?locationid=" + locationId, function (response) {
        if (response !== '') {
            $img.attr("src", response + "?w=80&h=80&mode=crop");
        } else {
            $img.attr("src", "/_content/_img/no-pic.png?w=80&h=80&mode=crop");
        }
    });
}


function RefreshParent() {
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // do the following to get rid of the padding
    $('body').removeAttr('style');

    // reload locations
    location.reload();
}
