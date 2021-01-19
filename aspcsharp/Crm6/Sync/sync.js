var subscriberId = parseInt($("#lblSubscriberId").text());
var userId = parseInt($("#lblUserId").text());
var $divSync = $("#divSync");

$(function () {
    // initialize sync
    new Sync().Init();
});


var Sync = function () {
    var self = this;

    this.Init = function () {
        // init validator
        // self.InitSaveValidator();
    }


    this.Sync = function () {

        // AJAX to Sync Office 365 to CRM for User
        var $divSync = $("#divSync");
        $.ajax({
            type: "POST",
            url: "/api/sync/SyncUser",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(request),
            success: function (response) {
                bindLoadingMsg("", $divSync, false);
                if (parseInt(response) > 0) {
                    // show sync result message

                    try { parent.RefreshParent(); } catch (e) { }
                } else {
                    alert("Error syncing Office 365 with CRM!");
                }
            }, beforeSend: function () {
                //add loading message
                bindLoadingMsg("Syncing CRM with Office 365...", $divSync, true);
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    }
}


function bindLoadingMsg(msg, $parent, binsert, topMargin) {
    //delete existing
    $parent.find(".loading-msg").remove();

    if (binsert) {
        //create loading
        var $loading = $('<div class="loading-msg text-center"></div>');
        var $spinner = spinkit.getSpinner(spinkit.spinerTypes.fadingCircle);
        $spinner.css("margin-top", (topMargin && topMargin != "" ? topMargin : "10px"));
        $loading.append($spinner);
        $loading.append($("<div class='loading-msg m-t-xs'>" + msg + "</div>"));
        $loading.appendTo($parent);
    }
}

