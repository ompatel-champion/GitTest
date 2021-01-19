// set global values 
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var userIdGlobal = $("#lblUserIdGlobal").text();
var dateFormat = $("#lblDateFormat").text();
var currentPage = 1;
var $divNoItems = $("#divNoItems");
var $activities = $("#tblActivityReport>tbody");
var $datepickers = $("[data-name='datepicker']");
var $tblActivityReport = $("#tblActivityReport");
var $divReportContent = $("#divReportContent");
var datatableIntiated = false;
var $ddlCompany = $("#ddlCompany");
var $ddlLocations = $("#ddlLocations");
var $ddlUsers = $("#ddlUsers");
var $ddlCountry = $("#ddlCountry");


$(function () {
    new Report().Init();
});


var Report = function () {
    var self = this;

    this.Init = function () {
        self.InitSelect2DropDowns();
        // fix for form controls inside drop down menu
        $(".dropdown-menu").click(function (e) {
            e.stopPropagation();
        });
        // run report action
        $("#btnRunReport").unbind("click").click(function (e) {
            e.preventDefault();
            self.RunReport();
        });
        // company 
        $("#ddlCompany").select2({
            theme: "classic",
            minimumInputLength: 2,
            allowClear: true,
            placeholder: '',
            ajax: {
                url: function (obj) {
                    var keyword = obj.term ? obj.term : "";
                    return "/api/AutoComplete/?type=globalcompanywithpermission&UserId=" + userId + "&SusbcriberId=" + subscriberId + "&prefix=" + keyword.replace("&", "%26");
                },
                dataType: "json",
                timeout: 50000,
                type: "GET",
                data: '',
                processResults: function (data) {
                    var o = new Object();
                    o.name = "";
                    o.id = "";
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.dataObj.CompanyName,
                                id: item.id
                            };
                        })
                    };
                }
            }
        });
        // initialize date pickers
        $datepickers.datepicker({
            dateFormat: "dd MM, yy",
            autoclose: true
        }).on('changeDate', function () {
        });
        $datepickers.on('focus', function (e) {
            e.preventDefault();
            $(this).attr("autocomplete", "off");
        });
        // default dates  
        $("#txtDateFrom").val(moment().add(-1, 'months').format("DD MMMM, YYYY"));
        $("#txtDateTo").val(moment().format("DD MMMM, YYYY"));

        $ddlCountry.on("select2:unselect", function (e) {
            $ddlLocations.closest(".form-group").find(".select2-selection__choice__remove").each(function (i, obj) {
                $(this).click();
            });
            getLocations();
         
        });
        $ddlLocations.on("select2:unselect", function (e) {
            getSalesReps();
        });
        // select2 events
        if ($ddlCountry.length > 0) {
            $ddlCountry.on('select2:select', function () {
                getLocations(); 
            });
        }

        if ($ddlLocations.length > 0) {
            $ddlLocations.on('select2:select', function () {
                getSalesReps();
            });
        }


    };

    this.InitSelect2DropDowns = function () {
        if ($ddlLocations.length > 0)
            $ddlLocations.select2({ theme: "classic", allowClear: true, placeholder: translatePhrase("") });
        if ($ddlCountry.length > 0)
            $ddlCountry.select2({ theme: "classic", allowClear: true, placeholder: translatePhrase("") });
        if ($ddlUsers.length > 0)
            $ddlUsers.select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });

        $("#ddlCompetitors").select2({ theme: "classic", allowClear: true, placeholder: translatePhrase("") });
        $("#ddlCompany").select2({ theme: "classic", allowClear: true, placeholder: translatePhrase("") });
        $("#ddlCampaigns").select2({ theme: "classic", allowClear: true, placeholder: translatePhrase("") });
        $("#ddlDealType").select2({ allowClear: true, placeholder: translatePhrase("") });
        // if only one user means, user select got the logged in user, so hide the drop down
        var usersLength = $('#ddlUsers').children('option').length;
        if (usersLength === 1) {
            $("#userContainer").addClass("hide");
        }
        var $ddlTaskSalesReps = $("#ddlTaskSalesReps");
        $("#ddlSalesStage").select2({ placeholder: "Sales Stage", allowClear: false });
        $ddlTaskSalesReps.select2({ placeholder: "Sales Rep", allowClear: false });
        $("#ddlTaskRelatedDeal").select2({ placeholder: "Deal", allowClear: false });
        $("#ddlTaskRelatedContacts").select2({ placeholder: "Contacts", allowClear: false });

        $('#ddlTaskSalesReps').on("select2:select", function (e) {
            $("#ddlTaskSalesReps+.select2-container .select2-selection").removeClass('error');
        });
    };

    this.RunReport = function () {
        // retrieve activities
        currentPage = 1;
        new Activites().RetrieveActivities();
    };

    this.PrintReport = function () {
        // print report function goes here
    };
};


var Activites = function () {
    var self = this;
    var reportItemHtml = "";

    // get filters
    this.GetFilter = function () {
        // set the filters
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.LoggedInUserId = userId;
        filters.GlobalUserId = userIdGlobal;
        // activity types
        filters.ActivityTypes = [];
        if ($("#chkCalendarEvents").is(":checked")) { filters.ActivityTypes.push("events"); }
        if ($("#chkTasks").is(":checked")) { filters.ActivityTypes.push("tasks"); }
        if ($("#chkNotes").is(":checked")) { filters.ActivityTypes.push("notes"); }
        // Date From, Date To
        filters.DateFrom = moment($("#txtDateFrom").datepicker("getDate")).isValid() ? moment($("#txtDateFrom").datepicker("getDate")).format("YYYY-MM-DD") : null;
        filters.DateTo = moment($("#txtDateTo").datepicker("getDate")).isValid() ? moment($("#txtDateTo").datepicker("getDate")).format("YYYY-MM-DD") : null;
        //other filter fields
        var val = $("#ddlDealType :selected").val(); if (val) filters.DealType = val;

        // global users
        var globalUserIds = [];
        if ($('#ddlUsers').length) {
            if ($('#ddlUsers option').length > 0) {

                $('#ddlUsers option:selected').each(function (i, selected) {
                    var val = $(selected).val();
                    if (val) globalUserIds.push(val);
                });

                // no users selected - so pass all the users
                if (globalUserIds.length === 0) {
                    filters.NoUsersSelectedInForGlobalCampaigns = true;
                    $('#ddlUsers option').each(function (i, uid) {
                        var val = parseInt($(uid).val());
                        if (val && val > 0) globalUserIds.push(val);
                    });
                }
            }
        } else {
            globalUserIds.push(userIdGlobal);
        }

        filters.GlobalUserIds = globalUserIds;

        // global companies
        var globalCompanyIds = [];
        $('#ddlCompany option:selected').each(function (i, selected) {
            var val = $(selected).val();
            if (val) globalCompanyIds.push(val);
        });
        filters.GlobalComapnyIds = globalCompanyIds;

        //note: setting UserIds to an empty array includes all users by default
        $.each([
            "Competitors",
            "Categories",
            "Locations",
            "Country",
            "Company",
            "Campaigns"
        ], function (i, type) {
            var fieldId = type.fieldId || type;
            var filterId = type.filterId || type;
            var filter = [];
            $('#ddl' + fieldId + ' :selected').each(function (i, selected) {
                var val = $(selected).val();
                if (val) filter.push(val);
            });
            if (filter.length) filters[filterId] = filter;
            else if (type.defVal) filters[filterId] = type.defVal;
        });
        return filters;
    };

    // retrieve activities
    this.RetrieveActivities = function () {
        // set the filters
        var filters = self.GetFilter();


        $divReportContent.addClass('hide');
        $activities.html("");
        $("#btnExcel").addClass("hide");
        $(".total-record-count").html("");

        if (filters.GlobalUserIds.length === 0) {
            $(".total-record-count").html("0 activities found");
            $divNoItems.removeClass('hide');
            return;
        }
        if (filters.ActivityTypes.length === 0) {
            $(".total-record-count").html("0 activities found");
            $divNoItems.removeClass('hide');
            swal({
                title: "Please select at least one activity type.",
                type: "error",
                showCancelButton: false
            });
            return;
        }
        // AJAX to retrieve activities
        $.ajax({
            type: "POST",
            url: "/api/Report/GetActivitiesByDateRangeReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 1200000,
            data: JSON.stringify(filters),
            success: function (activityResponse) {
                removeSpinner();
                if (activityResponse.Activities.length > 0) {
                    // bind activities
                    self.BindActivities(activityResponse.Activities);
                    $(".total-record-count").html(activityResponse.Activities.length + " activities found");
                    // set the table as a data table
                    $divReportContent.removeClass("hide");
                    //excel
                    $("#btnExcel").removeClass("hide");
                    $("#btnExcel").unbind("click").click(function () {
                        location.href = activityResponse.ExcelUri;
                    });
                    $divNoItems.addClass('hide');
                } else {
                    $divNoItems.removeClass('hide');
                }
            },
            beforeSend: function () {
                addSpinner();
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };


    // bind activities
    this.BindActivities = function (activities) {
        // get template for activity row
        reportItemHtml = self.GetReportRowHtml();
        // iterate and bind activities
        $.each(activities, function (i, activity) {
            var $tr = self.GetActivityItemHtml(activity);
            $activities.append($tr);
        });
        // init event add/edit
        new CalendarEvent().Init();
    };

    // get activity row html
    this.GetActivityItemHtml = function (activity) {
        var $tr = $(reportItemHtml);
        $tr.attr("data-id", activity.ActivtyId);
        // bind activity details
        $tr.find("[data-name='activity-type']").html(activity.ActivityType + (activity.Category && activity.Category !== 0 ? "<br/>" + activity.Category : ""));
        $tr.find("[data-name='subject']").html(activity.ActivityType === "EVENT" ? activity.Subject : activity.TaskName);
        $tr.find("[data-name='user']").html(activity.User);
        $tr.find("[data-name='activity-date']").html(activity.ActivityDateStr);
        $tr.find("[data-name='location']").html(activity.LocationName);
        $tr.find("[data-name='description']").html(formatContent("html", activity.Description));
        $tr.find("[data-name='completed']").html(activity.ActivityType === "Task" ? (activity.Completed ? "Yes" : "No") : "");
        $tr.find("[data-name='percentage']").html(activity.Percentage);
        $tr.find("[data-name='createddate']").html(moment(activity.CreatedDate).format("DD-MMM-YY @ HH:mm"));
        $tr.find("[data-name='lastupdated']").html(moment(activity.LastUpdatedDate).format("DD-MMM-YY @ HH:mm"));
        $tr.find("[data-name='deal-type']").html(activity.DealType);
        $tr.find("[data-name='competitors']").html(activity.Competitors);
        $tr.find("[data-name='campaigns']").html(activity.Campaigns);

        // deal 
        if (activity.Deals && activity.Deals !== "" > 0) {
            var $span = $("<span/>", { "html": activity.Deals });
            $tr.find("[data-name='deal']").append($span);
        } else {
            $tr.find("[data-name='deal']").html("");
        }
        // companies
        if (activity.CompanyName !== null) {
            var $p = $("<p/>");
            if (activity.CompanyId > 0) {
                $a = $("<a/>", { "html": activity.CompanyName });
                var companyDetailLink = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + activity.CompanyId + "&subscriberId=" + activity.CompanySubscriberId;
                $a.attr("href", companyDetailLink);
                $a.attr("target", "_blank");
                $p.append($a);
            } else {
                $p.html(activity.CompanyName);
            }
            $tr.find("[data-name='companies']").append($p);
        } else {
            $tr.find("[data-name='companies']").html("");
        }
        // contacts
        if (activity.ContactsStr !== null && activity.ContactsStr !== "") {
            $span = $("<a/>", { "html": activity.ContactsStr });
            $tr.find("[data-name='contacts']").append($span);
        } else {
            $tr.find("[data-name='contacts']").html("");
        }
        if (activity.ActivityType === "Note") {
            $tr.find("[data-name='edit-activity']").addClass("hide");
        }
        if (activity.ActivityType === "Event") {
            $tr.find("[data-name='edit-activity']").attr("data-action", "edit-event");
            $tr.find("[data-name='edit-activity']").attr("event-id", activity.ActivtyId);
            $tr.find("[data-name='edit-activity']").attr("event-subscriber-id", activity.SubscriberId);
            $tr.find("[data-name='edit-activity']").attr("data-name", "");
        } else if (activity.ActivityType === "Task") {
            $tr.find("[data-name='edit-activity']").attr("data-toggle", "modal");
            $tr.find("[data-name='edit-activity']").attr("data-action", "add-task");
        }
        $tr.find("[data-name='edit-activity']").unbind("click").click(function () {
            var activityId = $(this).closest("tr").attr("data-id");
            if (activity.ActivityType === "Task") {
                self.OpenTaskAddEditDialog(activityId);
            }
        });
        return $tr;

        //format content
        function formatContent(type, content) {
            var HTML_MAX_LENGTH = 20;
            type = (type || '').toLowerCase();
            var rc = content;
            switch (type) {
                case "html":
                    if (/\</.test(rc) && /\>/.test(rc)) rc = $("<div>").html(rc).text();
                    if (rc.length > HTML_MAX_LENGTH) rc = rc.substring(0, HTML_MAX_LENGTH) + "...";
            }
            return rc;
        }
    };

    // get activity by date range row html template
    this.GetReportRowHtml = function () {
        var html = "";
        $.ajax({
            cache: false,
            async: false,
            url: "/_templates/ActivitiesByDateRangeReport.html" + "?q=" + $.now(),
            success: function (data) {
                html = data;
            }
        });
        return html;
    };

    this.OpenTaskAddEditDialog = function (activityId) {
        var taskName = "";
        if (parseInt(activityId) > 0) {
            taskName = "Edit Task";
        }
        var iframeUrl = "/Tasks/TaskAddEdit/TaskAddEdit.aspx?taskId=" + activityId;
        $("<div/>", { "class": "modalWrapper", "id": "iframeTaskAddEdit" }).launchModal({
            title: taskName,
            modalClass: "modal-lg",
            btnSuccessText: "Save",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
                // save event action
                var frameWrapper = window.parent.document.getElementById("iframeTaskAddEdit");
                var iframe = frameWrapper.getElementsByTagName("iframe")[0];
                iframe.contentWindow.saveTask();
            }
        });
    };


    this.OpenEventAddEditDialog = function (eventId) {
        location.href = "/CalendarEvents/CalendarEventAddEdit/CalendarEventAddEdit.aspx?eventId=" + eventId;
    };
};


function getLocations() {
    // set the request object
    var request = { SubscriberId: subscriberId, UserId: userId };
    request.CountryCodes = $ddlCountry.val();

    // AJAX to retrieve
    $.ajax({
        type: "POST",
        url: "/api/Dropdown/GetUserLocations",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(request),
        success: function (locations) {
            $ddlLocations.html("");
            $ddlLocations.append($('<option>', { value: "", text: "" }));
            $.each(locations, function (i, ele) {
                $ddlLocations.append($('<option>', { value: ele.SelectValue, text: ele.SelectText }));
            });
            getSalesReps();
        },
        beforeSend: function () {
        },
        error: function () {
        }
    });
}

function getSalesReps() {

    // set the request object
    var request = { SubscriberId: subscriberId };
    request.LocationCodes = $ddlLocations.val();
    request.CountryCodes = $ddlCountry.val();

    if (request.LocationCodes.length === 0) {
        $('#ddlLocations option').each(function (i, uid) {
            var val = $(uid).val();
            if (val && val !== "") request.LocationCodes.push(val);
        });
    }
    if (request.CountryCodes.length === 0) {
        $('#ddlCountry option').each(function (i, uid) {
            var val = $(uid).val();
            if (val && val !== "") request.CountryCodes.push(val);
        });
    }


    // AJAX to retrieve
    $.ajax({
        type: "POST",
        url: "/api/Dropdown/GetAllSalesReps",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(request),
        success: function (salesReps) {
            removeSpinner();
            $ddlUsers.html("");
            //$ddlUsers.append($('<option>', { value: "", text: "" }));
            $.each(salesReps, function (i, ele) {
                $ddlUsers.append($('<option>', { value: ele.SelectValue, text: ele.SelectText }));
            });
        },
        beforeSend: function () {
            addSpinner();
        },
        error: function () {
            removeSpinner();
        }
    });
}


function printTable() {
    var divToPrint = document.getElementById("tblActivityReport");
    newWin = window.open("");
    newWin.document.write(divToPrint.outerHTML);
    newWin.print();
    newWin.close();
}

function RefreshEvents() {
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // do the following to get rid of the padding
    $('body').removeAttr('style');

    $("#chkCalendarEvents").iCheck("check");
    $("#chkTasks").iCheck("check");
    $("#chkNotes").iCheck("check");
    currentPage = 1;
    new Report().RunReport();
}

// show the filters
$(document).ready(function () {
    $("#divReportFilter").removeClass("hide");
});
