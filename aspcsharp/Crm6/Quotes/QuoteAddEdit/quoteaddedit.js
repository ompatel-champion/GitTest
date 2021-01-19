var subscriberId = parseInt($("#lblSubscriberId").text());
var dealId = parseInt($("#lblDealId").text());
var userId = parseInt($("#lblUserId").text());
var $divQuoteSetup = $("#divQuoteSetup");
var $dealInner = $("#dealInner");
var $ddlCompany = $("#ddlCompany");
var $ddlSalesOwner = $("#ddlSalesOwner");

$(function () {
    new PageSetup().Init();
});


var PageSetup = function () {
    var self = this;

    this.Init = function () {
        self.SetupSelect2Dropdowns();
        self.InitDatePickers();
    };

    this.SetupSelect2Dropdowns = function () {

        $("#ddlSalesOwner").select2({ theme: "classic" });
    };

    this.InitDatePickers = function () {
        // initialize dates
        $("[data-name='datepicker']").datepicker({
            format: "dd MM, yyyy",
            autoclose: true,
            todayHighlight: true,
            onClose: function (dateText, inst) {
                $(this).attr("disabled", false);
            },
            beforeShow: function (input, inst) {
                $(this).attr("disabled", true);
            }
        }).on("changeDate", function (e) {
            $(this).valid();
        });
    };

    $("#btnSave").unbind("click").click(function () {
        var quote = new Object();

        quote.CompanyId = $("#ddlCompany").val();
        quote.DealId = $("#ddlDeal").val();
        quote.BranchCode = $("#txtBranch").val();
        quote.IncotermText = $("#txtTerms").val();
        quote.QuoteCode = $("#txtCode").val();
        quote.TotalPackages = $("#txtPieces").val();
        quote.SubscriberId = subscriberId;
        quote.CreatedUserId = $("#lblUserId").val();
        quote.QuoteId = parseInt($("#lblQuoteId").text());

        // AJAX to save the district
        $.ajax({
            type: "POST",
            url: "/api/quote/SaveQuote",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(quote),
            success: function (response) {
                swal.close();
                if (parseInt(response) > 0) {
                    location.href = "/Quotes/Quotes.aspx";
                } else {
                    alert("Error saving quote!");
                }
            }, beforeSend: function () {
                //add loading message
                swal({ text: translatePhrase("Saving Quote") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: true });
            }, error: function (request, status, error) {
                alert(JSON.stringify(request))
            }
        });
    });
};

function bindLoadingMsg(msg, $parent, binsert, topMargin) {
    //delete existing
    $parent.find(".loading-msg").remove();

    if (binsert) {
        //create loading
        var $loading = $('<div class="loading-msg text-center"></div>');
        var $spinner = spinkit.getSpinner(spinkit.spinerTypes.fadingCircle);
        $spinner.css("margin-top", (topMargin && topMargin !== "" ? topMargin : "10px"));
        $loading.append($spinner);
        $loading.append($("<div class='loading-msg m-t-xs'>" + msg + "</div>"));
        $loading.appendTo($parent);
    }
}


