// global language translation variables
var vLanguageCode;
var vLanguageData;
var userId = parseInt($("#lblUserId").text());
var subscriberId = parseInt($("#lblSubscriberId").text());
var $pageContent = $(".page-content");

$(function () { 
    'use strict';
    // translate page title
    try { document.title = translatePhrase(document.title); } catch (e) { }
    // perform translation of all labels
    performLanguageTranslation();
});


// initialize language translation
function initLanguageTranslation() {

    // check if Language Data is already in localStorage  
    var vLocalStorageLanguageData = localStorage.getItem('language_data');

    // LanguageCode Matches localStorage
    if (vLocalStorageLanguageData === '' || vLocalStorageLanguageData === null || vLocalStorageLanguageData === 'undefined') {
         
        // Retrieve Language Data using Ajax 
        var response = ''; 
        // Get JSON Language Phrase Translation Data Dictionary using Ajax
        var vUrl = '/api/language/GetLanguageTranslations?userId=' + userId + '&subscriberId=' + subscriberId;

        $.ajax({
            type: 'GET',
            url: vUrl,
            async: false,
            contentType: "application/json; charset=utf-8",
            data: '',
            dataType: 'json',
            success: function (data) {
               
                response = data;
            }
        });

        // NEED TO VERIFY RESPONSE DATA IS CORRECT
        if (vLanguageData !== 'undefined') { 
            vLanguageData = response.LanguageTranslations;
            // save language data to localStorage
            localStorage.setItem('language_data', JSON.stringify(response.LanguageTranslations));
            localStorage.setItem('language_code', response.LanguageCode);
        }
    } else {
        // Get LanguageData JSON from localStorage JSON (vLocalStorageLanguageData)
        vLanguageData = JSON.parse(vLocalStorageLanguageData);
    }

    // Return JSON
    return vLanguageData;
}


function performLanguageTranslation() {
    'use strict';
    // get language data from local storage
    var vLanguageData = initLanguageTranslation();

    // get language code from localStorage
    var vLocalStorageLanguageCode = localStorage.getItem('language_code');

    if (vLocalStorageLanguageCode === 'en-US') {
        // us english -don't translate
    } else {
        // translate text for all controls on a web form using localStorage language phrase data dictionary
        if (vLanguageData !== '' || vLanguageData !== null || vLanguageData !== 'null' || vLanguageData !== 'undefined' || vLanguageData !== undefined || typeof vLanguageData !== 'undefined' ) {
          
            $(".language-entry").each(function (i) {
                var vEnglishPhrase = $(this).html();
                // translate english phrase using json language phrase
                var translatedPhrases = $.grep(vLanguageData, function (n, i) {
                    return n.LanguagePhrase.toLowerCase() === vEnglishPhrase.toLowerCase();
                });
                if (translatedPhrases.length > 0) {
                    // valid translation - change text with language-entry class
                    $(this).html(translatedPhrases[0].Translation);
                }
            });
            $(".btn").each(function (i) {
                var vEnglishPhrase = $(this).val();
                // translate english phrase using json language phrase
                var translatedPhrases = $.grep(vLanguageData, function (n, i) {
                    return n.LanguagePhrase.toLowerCase() === vEnglishPhrase.toLowerCase();
                });
                if (translatedPhrases.length > 0) {
                    // valid tTranslation - change button text with language-entry class
                    $(this).val(translatedPhrases[0].Translation);
                }
            });
            $('[placeholder]').each(function (index, el) { 
                var vEnglishPhrase = $(el).attr('placeholder'); 
                // translate english phrase using json language phrase
                var translatedPhrases = $.grep(vLanguageData, function (n, i) {
                    return n.LanguagePhrase.toLowerCase() == vEnglishPhrase.toLowerCase();
                });
                if (translatedPhrases.length > 0) {
                    // valid tTranslation - change placehold text with language-entry class
                    $(el).attr('placeholder', translatedPhrases[0].Translation);
                }
            });
        }
    }
    // show page after translation is done
    $pageContent.removeClass("hide");
}


function translatePhrase(vEnglishPhrase) {
    'use strict';
    var vTranslatedPhrase = vEnglishPhrase;

    // get language data from local storage
    var vLanguageData = initLanguageTranslation();

    // get stored language code
    var vLocalStorageLanguageCode = localStorage.getItem('language_code');

    // only translate if not us english language code
    if (vLocalStorageLanguageCode !== 'en-US') {
        // verify language phrase data dictionary exists
        if (vLanguageData !== '' || vLanguageData !== 'null' || vLanguageData !== null || vLanguageData !== 'undefined' || vLanguageData !== undefined) {
            // translate english phrase using json language phrase
            var translatedPhrases = $.grep(vLanguageData, function (n, i) {
                return n.LanguagePhrase.toLowerCase() == vEnglishPhrase.toLowerCase();
            });

            if (translatedPhrases.length > 0) {
                // valid translation
                vTranslatedPhrase = translatedPhrases[0].Translation;
            } else {
                // no translation - return english phrase
                vTranslatedPhrase = vEnglishPhrase;
            }
        }
    } else {
        // us-EN - don't translate phrase
        vTranslatedPhrase = vEnglishPhrase;
    }
    if (vTranslatedPhrase === undefined) {
        // if undefined - Use english phrase
        vTranslatedPhrase = vEnglishPhrase;
    }
    return vTranslatedPhrase;
}


function clearlocalStorage() {
    'use strict';
    localStorage.setItem('language_data', '');
    localStorage.setItem('language_code', '');
}
