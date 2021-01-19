var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var $tbody = $("#tblQuotes>tbody");

$(function () {
    new quoteTranslations().Init();
    new quotePhrases().Init();
    new Quotes().Init();
    showQuotes();
});

var Quotes = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    }

    this.BindActions = function () {

        // new phrase
        $(".new-quote").unbind('click').click(function () {
            self.OpenEditDialog(0);
        });

        // edit
        $tbody.find('[data-action="edit"]').unbind('click').click(function () {
            var phraseId = $(this).closest("tr").attr("data-id");
            self.OpenEditDialog(phraseId);
        });

        //delete
        $tbody.find('[data-action="delete"]').unbind('click').click(function () {
            var phraseId = $(this).closest("tr").attr("data-id");
            self.Delete(phraseId);
        });
    }

    this.OpenEditDialog = function (id) {
        var iframeUrl = "/Admin/Quotes/QuotesAddEdit/QuotesAddEdit.aspx?id=" + id;

        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframequote" }).launchModal({
            title: "quote",
            modalClass: "modal-md",
            btnSuccessText: "Save",
            maxHeight: "200px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
                // save company action
                var frameWrapper = window.parent.document.getElementById("iframequote");
                var iframe = frameWrapper.getElementsByTagName("iframe")[0];
                iframe.contentWindow.Save();
            }
        });
    }


    this.Delete = function (id) {
        swal({
            title: translatePhrase("Delete Quote!"),
            text: translatePhrase("Are you sure you want to delete this quote?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }, function () {
            $.ajax({
                type: "GET",
                url: "/api/quote/Deletequote/?id=" + id + "&userId=" + userId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (response) {
                    if (response) {
                        location.reload();
                    }
                },
                beforeSend: function () { },
                error: function (request) { }
            });
        });
    }


}

var quotePhrases = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    }

    this.BindActions = function () {

        // new phrase
        $(".new-phrase").unbind('click').click(function () {
            self.OpenEditDialog(0);
        });

        // edit
        $tbody.find('[data-action="edit"]').unbind('click').click(function () {
            var phraseId = $(this).closest("tr").attr("data-id");
            self.OpenEditDialog(phraseId);
        });

        //delete
        $tbody.find('[data-action="delete"]').unbind('click').click(function () {
            var phraseId = $(this).closest("tr").attr("data-id");
            self.Delete(phraseId);
        });
    }

    this.OpenEditDialog = function (id) {
        var iframeUrl = "/Admin/Quotes/QuotesPhraseAddEdit/QuotesPhraseAddEdit.aspx?id=" + id;

        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframequotePhrase" }).launchModal({
            title: "quote  Phrase",
            modalClass: "modal-md",
            btnSuccessText: "Save",
            maxHeight: "200px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
                // save company action
                var frameWrapper = window.parent.document.getElementById("iframequotePhrase");
                var iframe = frameWrapper.getElementsByTagName("iframe")[0];
                iframe.contentWindow.Save();
            }
        });
    }


    this.Delete = function (id) {
        swal({
            title: translatePhrase("Delete Quote Phrase!"),
            text: translatePhrase("Are you sure you want to delete this quote phrase?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }, function () {
            $.ajax({
                type: "GET",
                url: "/api/quote/DeletequotePhrase/?id=" + id + "&userId=" + userId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (response) {
                    if (response) {
                        location.reload();
                    }
                },
                beforeSend: function () { },
                error: function (request) { }
            });
        });
    }


}

var quoteTranslations = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    }

    this.BindActions = function () {

        // edit
        $tbody.find('[data-action="edit"]').unbind('click').click(function () {
            var ltid = $(this).closest("tr").attr("data-id");
            var phrase = $(this).closest("tr").attr("data-phrase");
            self.OpenEditDialog(ltid, phrase);
        });

        //delete
        $tbody.find('[data-action="delete"]').unbind('click').click(function () {
            var phraseId = $(this).closest("tr").attr("data-id");
            // self.Delete(phraseId);
        });
    }

    this.OpenEditDialog = function (id, phrase) {
        var iframeUrl = "/Admin/Quotes/QuotesTranslationAddEdit/QuotesTranslationAddEdit.aspx?id=" + id + "&phrase=" + phrase + "&lcode=" + $("#ddlQuotes").val();
        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframequoteTranslation" }).launchModal({
            title: "quote Translation",
            modalClass: "modal-md",
            btnSuccessText: "Save",
            maxHeight: "300px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
                var frameWrapper = window.parent.document.getElementById("iframequoteTranslation");
                var iframe = frameWrapper.getElementsByTagName("iframe")[0];
                iframe.contentWindow.Save();
            }
        });
    }


    this.Delete = function (id) {
        swal({
            title: translatePhrase("Delete Quote Phrase!"),
            text: translatePhrase("Are you sure you want to delete this quote phrase?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }, function () {
            $.ajax({
                type: "GET",
                url: "/api/quote/DeletequotePhrase/?id=" + id + "&userId=" + userId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (response) {
                    if (response) {
                        location.reload();
                    }
                },
                beforeSend: function () { },
                error: function (request) { }
            });
        });
    }
}

function showquotePhrases() {
    $("#headerquoteTranslations").css("font-weight", "normal");
    $("#headerQuotes").css("font-weight", "normal");
    $("#headerquotePhrases").css("font-weight", "bold");
    $("#divquotePhrasesTab").show();
    $("#divquoteTranslationsTab").hide();
    $("#divQuotesTab").hide();
}

function showquoteTranslations() {
    $("#headerquoteTranslations").css("font-weight", "bold");
    $("#headerquotePhrases").css("font-weight", "normal");
    $("#headerQuotes").css("font-weight", "normal");
    $("#divquotePhrasesTab").hide();
    $("#divquoteTranslationsTab").show();
    $("#divQuotesTab").hide();
}

function showQuotes() {
    $("#headerQuotes").css("font-weight", "bold");
    $("#headerquotePhrases").css("font-weight", "normal");
    $("#headerquoteTranslations").css("font-weight", "normal");
    $("#divquotePhrasesTab").hide();
    $("#divquoteTranslationsTab").hide();
    $("#divQuotesTab").show();
}

function Reloadpage() {
    location.reload();
}