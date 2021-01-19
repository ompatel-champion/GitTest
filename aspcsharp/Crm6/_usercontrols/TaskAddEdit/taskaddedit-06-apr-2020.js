//docuemnt ready
$(function () {

    var subscriberId = $("#lblSubscriberId").text();
    var userId = $("#lblUserId").text();
    var userIdGlobal = $("#lblUserIdGlobal").text();
    var fields = {
        $txtName: $("#TaskAddEdit_txtName"),
        $txtDescription: $("#TaskAddEdit_txtDescription"),
        $txtDueDate: $("#TaskAddEdit_txtDueDate"),
        $ddlDeal: $("#TaskAddEdit_ddlDeal"),
        $ddlCompany: $("#TaskAddEdit_ddlCompany"),
        $ddlContact: $("#TaskAddEdit_ddlContact"),
        $ddlSalesRep: $("#TaskAddEdit_ddlSalesRep")
    };

    //initialize
    (function () {
        _init_drop_down_lists();
        _add_date_picker(fields.$txtDueDate);
        _add_event_listeners();

        function _add_event_listeners() {
            $("#TaskAddEdit_btnTaskAdd").unbind("click").click(function () {
                save();
            });
            $("#TaskAddEdit_btnTaskCancel").unbind('click').click(function () {
                resetTaskFields();
            });
            $("#TaskAddEdit_btnTaskModalClose").unbind('click').click(function () {
                resetTaskFields();
            });
            $('#addTaskDialog').on('show.bs.modal', function (e) {
                var $button = $(e.relatedTarget);
                var props = $button.data('modal-props') || {};
                var $title = $("#addTaskDialog .modal-title");
                if (props.type=="edit") {
                    $title.html("Edit Task");
                    loadTask(props.activityId, props.subscriberId);
                }
                else {
                    fields.$ddlSalesRep.val(userIdGlobal).trigger('change');  
                    $title.html("New Task");
                }
            });
            $('#addTaskDialog').on('hidden.bs.modal', function () {
                resetTaskFields();
            });
        }

        function _init_drop_down_lists() {
            fields.$ddlSalesRep.select2({});
            fields.$ddlContact.select2({
                allowClear: true,
                placeholder: "",
                ajax: {
                    url: function (obj) {
                        return "/api/AutoComplete/?type=globalcompanycontact&SusbcriberId=" + subscriberId + "&UserId=" + userId + "&GlobalCompanyId=" + (fields.$ddlCompany.val() || 0) + "&prefix=" + (obj.term || '');
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
            fields.$ddlDeal.select2({
                allowClear: true,
                tags: true,
                placeholder: "",
                ajax: {
                    url: function (obj) {
                        return "/api/AutoComplete/?type=globalcompanydealswithpermission&SusbcriberId=" + subscriberId + "&UserId=" + userId + "&GlobalCompanyId=" + (fields.$ddlCompany.val() || 0) + "&prefix=" + (obj.term || '');
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
            fields.$ddlCompany.select2({
                minimumInputLength: 2,
                allowClear: true,
                placeholder: "",
                ajax: {
                    url: function (obj) {
                        return "/api/AutoComplete/?type=globalcompanywithpermission&UserId=" + userId + "&SusbcriberId=" + subscriberId + "&prefix=" + (obj.term || '').replace("&", "%26");
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
                                    id: item.id
                                };
                            })
                        };
                    }
                }
            });
        }

        function _add_date_picker(props) {
            props = (!props.$wrapper ? { $wrapper: props } : props) || {};
            var $wrapper = props.$wrapper;
            if ($wrapper && $wrapper.length > 0) {
                $('#ui-datepicker-div').addClass("due_datepicker");
                $wrapper.datepicker({
                    showOn: "both",
                    inline: true,
                    autoclose: true,
                    format: "dd-MM-yy",
                    beforeShow: function () {
                        $('#ui-datepicker-div').addClass("due_datepicker");
                    },
                    buttonText: "<i class='icon-calendar'></i>"
                });
                $wrapper.datepicker({
                    showOn: "both",
                    inline: true,
                    beforeShow: function () {
                        $('#ui-datepicker-div').addClass("due_datepicker");
                    },
                    buttonText: "<i class='icon-calendar'></i>"
                });
                $wrapper.on('focus', function (e) {
                    e.preventDefault();
                    $(this).attr("autocomplete", "off");
                });
            }
        }
    })();

    //save the task
    function save() {
        $('.form-group .required').on('keyup blur', function (e) {
            if ($(this).val() !== "") {
                $(this).removeClass('error');
            } else {
                $(this).addClass('error');
            }
        });
        if (fields.$txtName.val() === "") {
            fields.$txtName.addClass('error');
        } else {
            fields.$txtName.removeClass('error');
        }
        if (fields.$txtDescription.val() === "") {
            fields.$txtDescription.addClass('error');
        } else {
            fields.$txtDescription.removeClass('error');
        }
        if (fields.$txtDueDate.val() === "") {
            fields.$txtDueDate.addClass('error');
        } else {
            fields.$txtDueDate.removeClass('error');
        }
        if (!moment(fields.$txtDueDate.val()).isValid()) {
            fields.$txtDueDate.addClass('error');
        }
        else {
            fields.$txtDueDate.removeClass('error');
        }
        if (fields.$ddlSalesRep.val() === "" || fields.$ddlSalesRep.val() === "0") {
            $("#TaskAddEdit_ddlSalesRep+.select2-container .select2-selection").addClass('error');
        } else {
            $("#TaskAddEdit_ddlSalesRep+.select2-container .select2-selection").removeClass('error');
        }
        if (fields.$txtName.val() === "" || fields.$txtDescription.val() === "" || fields.$txtDueDate.val() === "" || fields.$ddlSalesRep.val() === "" || fields.$ddlSalesRep.val() === "0") {
            return;
        }
        var taskModel = {
            task: {
                SubscriberId: subscriberId,
                UserIdGlobal: userIdGlobal,
                OwnerUserIdGlobal: parseInt(fields.$ddlSalesRep.val()),
                UpdateUserIdGlobal: userIdGlobal,
                TaskName: fields.$txtName.val(),
                Description: fields.$txtDescription.val(),
                DueDate: moment(fields.$txtDueDate.datepicker("getDate")).format("DD-MMM-YY"),
                CompanyIdGlobal: fields.$ddlCompany.val() === "" ? 0 : parseInt(fields.$ddlCompany.val()),
                DealIds: fields.$ddlDeal.val() === "" ? 0 : parseInt(fields.$ddlDeal.val())
            }
        };
        var selectedContact = fields.$ddlContact.select2('data');
        if (selectedContact.length > 0) {
            var contacts = [{
                AttendeeType: "contact",
                ContactId: parseInt(fields.$ddlContact.val()),
                ContactName: selectedContact[0].text,
                SubscriberId: subscriberId
            }];
            taskModel.Invites = contacts;
        }
        var activityId = parseInt($("#TaskAddEdit_btnTaskAdd").attr("activity-id"));
        if (activityId > 0) {
            taskModel.task.ActivityId = activityId;
        }
        $.ajax({
            type: "POST",
            url: "/api/task/SaveTask",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(taskModel),
            success: function (activityId) {
                if (parseInt(activityId) > 0) resetTaskFields();
                else alert("Task Save Error");
                removeSpinner();
                $('#addTaskDialog').modal('toggle');
                // reload calendar
                try { RefetchEvents(); } catch (e) {/*ignore*/ }
                // reload tasks in activities
                try { new Tasks().RetrieveTasks(); } catch (e) {/*ignore*/ }
            }, beforeSend: function () {
                addSpinner();
            }, error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    }

    //reset/clear the task fields
    function resetTaskFields() {
        fields.$txtName.val("");
        fields.$txtDescription.val("");
        fields.$txtDueDate.val("");
        fields.$ddlDeal.val(null).trigger('change');
        fields.$ddlCompany.val(null).trigger('change');
        fields.$ddlContact.val(null).trigger('change');
        fields.$ddlSalesRep.val(null).trigger('change');
        $("#TaskAddEdit_btnTaskAdd").attr("activity-id", 0);
    }

    // load tasks
    function loadTask(activityId, subscriberId) {
        $("#TaskAddEdit_btnTaskAdd").html("Save Task");
        // load task
        $.ajax({
            type: "GET",
            url: "/api/Task/GetTask?taskId=" + activityId + "&subscriberId=" + subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '',
            success: function (task) {
                if (task !== null) {
                    $("#TaskAddEdit_btnTaskAdd").attr("activity-id", task.Task.ActivityId);
                    fields.$txtName.val(task.Task.TaskName);
                    fields.$txtDescription.val(task.Task.Description);
                    fields.$txtDueDate.datepicker("setDate", moment(task.Task.DueDate).toDate());
                    fields.$ddlSalesRep.val(task.Task.OwnerUserIdGlobal).trigger("change");

                    // set the company
                    if (task.Task.CompanyIdGlobal > 0) {
                        var company = new Option(task.Task.CompanyName, task.Task.CompanyIdGlobal, false, true);
                        fields.$ddlCompany.append(company).trigger('change');
                    }
                    // set the deal 
                    if (task.Task.DealIds && task.Task.DealIds !== "0") {
                        var deal = new Option(task.Task.DealNames, task.Task.DealIds, false, true);
                        fields.$ddlDeal.append(deal).trigger('change');
                    }
                    // set the contact
                    if (task.Task.ContactIds !== null && task.Task.ContactIds !== "") {
                        var contact = new Option(task.Task.ContactNames, task.Task.ContactIds, false, true);
                        fields.$ddlContact.append(contact).trigger('change');
                    }
                }
                removeSpinner();
            },
            beforeSend: function () {
                addSpinner();
            },
            error: function () {
                removeSpinner();
            }
        });
    }
});
