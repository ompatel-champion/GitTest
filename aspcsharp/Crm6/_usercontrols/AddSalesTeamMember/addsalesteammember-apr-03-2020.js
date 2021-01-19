(function() {

    var initialized = false;
    var fields = {};

    //ready
    $(function() {
        var $dialog = $('#addSalesTeamMemberDialog');
        $dialog.on('show.bs.modal', function (e) {
            init();
            var $button = $(e.relatedTarget);
            var modalProps = $button.data('modal-props')||{};
            if (modalProps.type==="edit") {
                fields.$ddlMember.append(new Option(modalProps.name, modalProps.id, false, true)).trigger('change');
                fields.$ddlRole.val(modalProps.role).trigger("change");
            }
            fields.$ddlMember.closest('.form-group')[(modalProps.type==="edit"?'add':'remove')+'Class']("d-none");
            $dialog.attr("data-mode", modalProps.type=="edit"?"edit":"add");
        });
        $dialog.on('hidden.bs.modal', function () {
             fields.$ddlMember.val(null).trigger('change');
             fields.$ddlRole.val(null).trigger('change');
        });
    });

    function init() {
        if (!initialized) {
            fields.$ddlMember = $('.modal [data-id="ddlSalesTeamMember"]');
            fields.$ddlRole = $('.modal [data-id="ddlSalesTeamRole"]');
            fields.$ddlMember.select2({
                minimumInputLength: 0,
                minimumResultsForSearch: Infinity,
                allowClear: true,
                placeholder: 'Select User',
                ajax: {
                    url: function (obj) {
                        var keyword = obj.term ? obj.term : "";
                        return "/api/AutoComplete/?type=alluserwithglobaluserid&SusbcriberId=" + docUserData.subscriberId + "&prefix=" + keyword;
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
            fields.$ddlMember.on('select2:select', function () {
                $(this).closest(".form-group").find(".error-text").html("");
                $(this).closest(".form-group").find(".select2-selection--single").removeClass("error-border");
            });
            fields.$ddlRole.select2({
                placeholder: 'Select Role', width: '100%', minimumResultsForSearch: -1
            });
            fields.$ddlRole.on('select2:select', function () {
                $(this).closest(".form-group").find(".error-text").html("");
                $(this).closest(".form-group").find(".select2-selection--single").removeClass("error-border");
            });
        }
        initialized = true;
    }
})();