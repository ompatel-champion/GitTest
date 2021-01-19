var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var dealId = $("#lblDealId").text();
var isWon = $("#lblIsWon").text() === "1";

$(function () {
    // select2
    $("#ddlWonLostReason").select2({ theme: "classic" });
});


function PerformWonLostAction() { 
    $("#btnSave").unbind('click').click();
}