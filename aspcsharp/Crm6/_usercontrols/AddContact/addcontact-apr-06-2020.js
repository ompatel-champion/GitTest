(function() {

    var initialized = false;
    var fields = {};

    //ready
    $(function() {
        var $dialog = $('#addContactDialog');
        $dialog.on('show.bs.modal', function (e) {
            init();
        });
        $dialog.on('hidden.bs.modal', function () {
             fields.$ddlContacts.val(null).trigger('change');
        });
    });

    function init() {
        if (!initialized) {
            fields.$ddlContacts = $('.modal [data-id="ddlContacts"]');
            fields.$ddlContacts.select2({
                minimumInputLength: 0,
                minimumResultsForSearch: Infinity,
                allowClear: true,
                placeholder: 'Select Contact',
                ajax: {
                    url: function (obj) {
                        var keyword = obj.term ? obj.term : "";
                        return '/api/AutoComplete/?' +
                            'type=companycontacts' +
                            '&globalcompanyId=' + docUserData.globalCompanyId +
                            '&SusbcriberId=' + docUserData.subscriberId +
                            '&prefix=' + keyword;
                    },
                    dataType: "json",
                    timeout: 50000,
                    type: "GET",
                    data: '',
                    processResults: function (data) {
                        return {
                            results: $.map(data, function (item) {
                                return {
                                    text: item.name,
                                    id: item.id,
                                    dataObj: item.dataObj
                                };
                            })
                        };
                    }
                }
            });
        }
    }
})();