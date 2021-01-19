var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var $tbody = $(".wrapper");

// library methods
const lib = {
    // save or reset an edited input field (typically triggered on field blur)
    saveOrResetField: function (type, elm, saveFunc) {
        const elm_tr = elm.closest("tr");
        const lastVal = elm_tr.attr("data-current-" + type);
        const curVal = elm.val();
        if (curVal !== lastVal) {
            if (curVal !== '') {
                saveFunc(elm_tr);
                elm_tr.attr("data-current-" + type, curVal);
            }
            else elm.val(lastVal);
        }
    }
};


$(function () {
    new LanguageTranslations().Init();
    new LanguagePhrases().Init();
    new Languages().Init();
    // mobile - tabs panel dropdown menu
    $('.panel-dropdown .ae-select-content').text($('.panel-dropdown .dropdown-nav > li.selected').text());
    var newOptions = $('.panel-dropdown .dropdown-nav > li');
    newOptions.click(function () {
        $('.panel-dropdown .ae-select-content').text($(this).text());
        $('.panel-dropdown .dropdown-nav > li').removeClass('selected');
        $(this).addClass('selected');
    });
    var aeDropdown = $('.panel-dropdown .ae-dropdown');
    aeDropdown.click(function () {
        $('.panel-dropdown .dropdown-nav').toggleClass('ae-hide');
        $('.panel-dropdown .ae-select').toggleClass('drop-open');
    });
});



var Languages = function () {
    var self = this;

    this.Init = function () {
        $('.border-tabs .btab,.nav-tabs a').click(function () {
            var tcID = $(this).attr('data-id');
            $('.btab-content').hide();
            $(tcID).fadeIn();
            $('.border-tabs .btab').removeClass('active');
            $(this).addClass('active');
        });
        self.BindActions();
    };

    this.BindActions = function () {
        // bind delete language click
        $("#tblLanguages").find('[data-action="delete"]').unbind('click').click(function () {
            var phraseId = $(this).closest("tr").attr("data-id");
            self.Delete(phraseId);
        });
        $("#tblLanguages").find("tr").each(function () {
            var $tr = $(this);
            $tr.find(".language-code").unbind("blur").blur(function () {
                lib.saveOrResetField("language-code", $(this), self.SaveLanguage);
            });
            $tr.find(".language-name").unbind("blur").blur(function () {
                lib.saveOrResetField("language-name", $(this), self.SaveLanguage);
            });
        });
    };


    this.SaveLanguage = function ($tr) {
        var language = new Object();
        language.LanguageId = $tr.attr("data-id");
        language.LanguageCode = $tr.find(".language-code").val();
        language.LanguageName = $tr.find(".language-name").val();

        if (language.LanguageCode === "" || language.LanguageName === "") {
            if (language.LanguageCode === "")
                $tr.find(".language-code").addClass("error");
            if (language.LanguageName === "")
                $tr.find(".language-name").addClass("error");
            return;
        }

        // set json request
        var request = { SubscriberId: subscriberId, Language: language };
        $.ajax({
            type: "POST",
            url: "/api/Language/SaveLanguage?subscriberId=" + subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(request),
            success: function (languageId) {
                if (parseInt(languageId) > 0) {
                    $("#divSpinner").addClass("hide");
                    $tr.attr("data-current-language-name", language.LanguageName);
                    $tr.attr("data-current-language-code", language.LanguageCode);
                    $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };


    this.Delete = function (id) {
        swal({
            title: translatePhrase("Delete Language!"),
            text: translatePhrase("Are you sure you want to delete this language?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/Language/DeleteLanguage/?id=" + id + "&userId=" + userId + "&subscriberId=" + subscriberId,
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
            }
        });
    };

};


var LanguagePhrases = function () {
    var self = this;
    var $tblLanguagePhrases = $("#tblLanguagePhrases");
    var $tbody = $tblLanguagePhrases.find("tbody");

    this.Init = function () {

        // add phrase
        $("#btnAddPhrase").unbind("click").click(function () {
            self.SaveLanguagePhrase(null);
        });

        $("#txtNewPhrase").unbind("blur").blur(function () {
            $(this).removeClass("error"); 
        });

        // load languages  
        self.LoadLanguagePhrases();

    };


    this.LoadLanguagePhrases = function () {
        $("#txtKeywordPhrase").val("");

        // AJAX to retrieve language phrases
        $.ajax({
            type: "GET",
            url: "/api/Language/GetLanguagePhrases",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (data) {
                $("#divSpinner").addClass("hide");
                // bind language phrases
                self.BindLanguagePhrases(data);
                // bind actios
                self.BindActions();
            }, beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            }, error: function () {
            }
        });
    };


    this.BindLanguagePhrases = function (data) {
        $tbody.html("");
        $.each(data, function (i, languageData) {

            var $tr = $("<tr/>");
            //input
            var $td = $("<td/>");
            var $input = $("<input/>", { "type": "text", "class": "form-control language-phrase", "value": languageData.LanguagePhrase1 });
            $td.append($input);
            $tr.append($td);

            //   action
            $td = $("<td/>", { "class": "TAC", "style": "width: 200px;" });
            $aDelete = $("<a/>", { "html": "<i class=\"icon-Delete\"></i>", "class": "delete-item", "data-action": "delete" });
            $td.append($aDelete);

            $tr.append($td);
            $tr.data(languageData);
            $tbody.append($tr);
        });
    };


    this.BindActions = function () {
        // bind delete language phrase click
        $("#tblLanguagePhrases").find('[data-action="delete"]').unbind('click').click(function () {
            var $tr = $(this).closest("tr");
            var languagePhraseId = $tr.data().LanguagePhraseId;
            self.Delete(languagePhraseId, $tr);
        });

        $("#tblLanguagePhrases").find("tr").each(function () {
            var $tr = $(this);
            $tr.find(".language-phrase").unbind("blur").blur(function () {
                lib.saveOrResetField("language-phrase", $(this), self.SaveLanguagePhrase);
            });
        });

        $("#txtKeywordPhrase").keyup(function () {
            var text = $("#txtKeywordPhrase").val();
            $('tr', "#tblLanguagePhrases").each(function () {
                var value = $(this).find(":input").val();
                if (value.toLowerCase().indexOf(text.toLowerCase()) >= 0) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });
        });
    };


    this.SaveLanguagePhrase = function ($tr) {
        // set save request
        var languagePhrase = new Object();
        languagePhrase.SubscriberId = subscriberId;
        languagePhrase.UpdateUserId = userId;
        if ($tr === null) {
            $("#txtNewPhrase").removeClass("error");
            languagePhrase.LanguagePhrase1 = $("#txtNewPhrase").val();
            languagePhrase.LanguagePhraseId = 0;

            if (languagePhrase.LanguagePhrase1 === "") {
                $("#txtNewPhrase").addClass("error");
                return;
            }
        } else {
            $tbody.find(".language-phrase").removeClass("error");
            var languageData = $tr.data();
            languagePhrase.LanguagePhrase1 = $tr.find(".language-phrase").val();
            languagePhrase.LanguagePhraseId = languageData.LanguagePhraseId;

            if (languagePhrase.LanguagePhrase1 === "") {
                $tr.find(".language-phrase").addClass("error");
                return;
            }
        }

        $.ajax({
            type: "POST",
            url: "/api/Language/SaveLanguagePhrase",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(languagePhrase),
            success: function (languagePhraseId) {
                $("#txtNewPhrase").val("");
                if (parseInt(languagePhraseId) > 0) {
                    $("#divSpinner").addClass("hide");
                    self.LoadLanguagePhrases();
                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.Delete = function (id, $tr) {
        swal({
            title: translatePhrase("Delete Language Phrase!"),
            text: translatePhrase("Are you sure you want to delete this language phrase?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/Language/DeleteLanguagePhrase/?id=" + id + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        if (response) {
                            $tr.remove();
                        }
                    },
                    beforeSend: function () { },
                    error: function (request) { }
                });
            }
        });
    };

};


var LanguageTranslations = function () {
    var self = this;
    var $tblLanguageTranslations = $("#tblLanguageTranslations");
    var $tbody = $tblLanguageTranslations.find("tbody");
    var $ddlLanguages = $("#ddlLanguages");
    var $ddlStatus = $("#ddlStatus");

    this.Init = function () {

        // initialize select2
        $ddlLanguages.select2({ minimumInputLength: 0 });
        $ddlStatus.select2({ minimumResultsForSearch: -1 });
        $ddlLanguages.val("zh-CN").trigger("change");

        // load translations
        $("#divTranslationsTabHeader").unbind("click").click(function () {
            self.LoadTranslations();
        });


        // bind dropdown actions
        $ddlLanguages.on('select2:select', function () { self.LoadTranslations(); });
        $ddlStatus.on('select2:select', function () { self.LoadTranslations(); });

    };

    this.LoadTranslations = function () {
        $("#txtTranslationsKeyword").val("");

        // get variables
        var langaugeCode = $ddlLanguages.val();
        var status = $ddlStatus.val();
        // AJAX to retrieve language translations
        $.ajax({
            type: "GET",
            url: "/api/Language/GetLanguageTranslationsAdmin?langaugeCode=" + langaugeCode + "&status=" + status + "&subscriberId=" + subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (data) {
                $("#divSpinner").addClass("hide");
                // bind languages
                self.BindLanguages(data);
                // bind actios
                self.BindActions();
            }, beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            }, error: function () {
            }
        });
    };

    this.BindLanguages = function (data) {
        $tbody.html("");
        $.each(data, function (i, languageData) {

            var $tr = $("<tr/>");
            // language phrase
            $tr.append($("<td/>", { "html": languageData.LanguagePhrase }));
            // inout
            var $td = $("<td/>");
            var $input = $("<input/>", { "type": "text", "class": "language-translation", "value": languageData.Translation });
            $td.append($input);
            $tr.append($td);

            //   action
            $td = $("<td/>", { "class": "TAC" });

            // Not Verified 
            var status = $ddlStatus.val();
            if (status === "Not Verified") {
                // add verify button
                $aVerify = $("<a/>", { "html": "Verify", "class": "primary-btn MR10", "data-action": "verify" });
                $td.append($aVerify);
            }
            if (status === "New") {
                // add save button
                $aSave = $("<a/>", { "html": "Save", "class": "primary-btn MR10", "data-action": "save" });
                $td.append($aSave);
            }
            // delete action
            if (status !== "New") {
                $aDelete = $("<a/>", { "html": "<i class=\"icon-Delete\"></i>", "class": "delete-item", "data-action": "delete" });
                $td.append($aDelete);
            }

            $tr.append($td);
            $tr.data(languageData);
            $tbody.append($tr);
        });

    };

    this.BindActions = function () {

        // bind delete language translation click
        $tbody.find('[data-action="delete"]').unbind('click').click(function () {
            var $tr = $(this).closest("tr");
            var languageTranslationId = $tr.data().LanguageTranslationId;
            if (languageTranslationId > 0)
                self.Delete(languageTranslationId, $tr);
        });



        $tbody.find("tr").find(".language-translation").unbind("blur").blur(function () {
            $(this).removeClass("error");
            var status = $ddlStatus.val();
            if (status === "Verified") {
                lib.saveOrResetField("language-translation", $(this), self.SaveLanguageTranslation);
            }
        });


        // save button if the status is new
        $tbody.find('[data-action="save"]').unbind('click').click(function () {
            var $tr = $(this).closest("tr");
            self.SaveLanguageTranslation($tr);
        });


        // verify button if the status is new
        $tbody.find('[data-action="verify"]').unbind('click').click(function () {
            var $tr = $(this).closest("tr");
            self.VerifyLanguageTranslation($tr);
        });


        // search
        $("#txtTranslationsKeyword").keyup(function () {
            var text = $("#txtTranslationsKeyword").val();
            $('tr', $tbody).each(function () {
                var value1 = $(this).data().LanguagePhrase;
                if (value1.toLowerCase().indexOf(text.toLowerCase()) >= 0) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });
        });

    };

    this.SaveLanguageTranslation = function ($tr) {

        $tbody.find(".language-translation").removeClass("error");
        var status = $ddlStatus.val();
        var languageData = $tr.data();
        var language = new Object();
        if (languageData.LanguageTranslationId === 0) {
            language.LanguageCode = $ddlLanguages.val();
            language.LanguageName = languageData.LanguageName;
        } else {
            language.LanguageCode = languageData.LanguageCode;
        }
        language.LanguagePhrase = languageData.LanguagePhrase;
        language.LanguageTranslationId = languageData.LanguageTranslationId;
        language.Translation = $tr.find(".language-translation").val();
        language.UpdateUserId = userId;

        if (language.Translation === "") {
            $tr.find(".language-translation").addClass("error");
            return;
        }

        $.ajax({
            type: "POST",
            url: "/api/Language/SaveLanguageTranslation",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(language),
            success: function (languageId) {
                $("#divSpinner").addClass("hide");
                if (parseInt(languageId) > 0) {
                    switch (status) {
                        case "Not Verified":
                        case "Verified":
                            // set/bind the data object
                            var data = $tr.data();
                            data.LanguageTranslationId = languageId;
                            data.Translation = language.Translation;
                            data.LanguageCode = language.LanguageCode;
                            $tr.data(data);

                            // animate row 
                            $tr.effect("highlight", { color: "#D1EFE9" }, 1000);
                            break;
                        case "New":
                            $tr.remove();
                            break;
                        default:
                    }


                }
            },
            beforeSend: function () {
                $("#divSpinner").removeClass("hide");
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.VerifyLanguageTranslation = function ($tr) {

        $tbody.find(".language-translation").removeClass("error");
        var languageData = $tr.data();
        var language = new Object();
        if (languageData.LanguageTranslationId > 0) {
            language.LanguageTranslationId = languageData.LanguageTranslationId;
            language.VerifiedUserId = userId;
            language.Translation = $tr.find(".language-translation").val();
            language.UpdateUserId = userId;
            if (language.Translation === "") {
                $tr.find(".language-translation").addClass("error");
                return;
            }

            $.ajax({
                type: "POST",
                url: "/api/Language/VerifyLanguageTranslation",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(language),
                success: function (response) {
                    $("#divSpinner").addClass("hide");
                    if (response) {
                        $tr.remove();
                    }
                },
                beforeSend: function () {
                    $("#divSpinner").removeClass("hide");
                },
                error: function (request) {
                    alert(JSON.stringify(request));
                }
            });
        }
    };


    this.Delete = function (id, $tr) {
        swal({
            title: translatePhrase("Delete the Translation!"),
            text: translatePhrase("Are you sure you want to delete this translation?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/Language/DeleteLanguageTranslation/?id=" + id + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        $("#divSpinner").addClass("hide");
                        if (response) {
                            $tr.remove();
                        }
                    },
                    beforeSend: function () { $("#divSpinner").removeClass("hide"); },
                    error: function () { }
                });
            }
        });
    };

};


function Reloadpage() {
    location.reload();
}
