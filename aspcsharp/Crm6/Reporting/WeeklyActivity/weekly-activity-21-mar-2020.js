// set global values 
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var userIdGlobal = $("#lblUserIdGlobal").text();
var dateFormat = $("#lblDateFormat").text();
var currentPage = 1;
var $divNoItems = $("#divNoItems");
var $activities = $("#tblActivityReport>tbody");
var $tblActivityReport = $("#tblActivityReport");
var $divReportContent = $("#divReportContent");
var datatableIntiated = false;
var $ddlLocations = $("#ddlLocations");
var $ddlUsers = $("#ddlUsers");
var $ddlCountry = $("#ddlCountry");
var $ddlTaskSalesReps = $("#ddlTaskSalesReps");

$(function () {
    // initialize report
    new Report().Init();
});


var Report = function () {
    var self = this;

    this.Init = function () {

        // set current week
        $('#ddlYear').val(moment().year()).trigger('change');

        // init select2 dropdowns
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


        // export excel
        $("#btnExcel").unbind("click").click(function () {
            self.ExportToExcel();
        });


        // select2 events
        if ($ddlCountry.length > 0) {
            $ddlCountry.on('select2:select', function () {
                getLocations();
                getSalesReps();
            });
            $ddlCountry.on('select2:unselect', function () {
                getLocations();
                getSalesReps();
            });
        }

        if ($ddlLocations.length > 0) {
            getLocations();
            $ddlLocations.on('select2:select', function () {
                getSalesReps();
            });
            $ddlLocations.on('select2:unselect', function () {
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

        $("#ddlWeekOfYear").select2({ });
        $("#ddlYear").select2({ });
        $("#ddlCategories").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlLocation").select2({ theme: "classic", allowClear: true, placeholder: translatePhrase("") });
        $("#ddlUser").select2({ theme: "classic", placeholder: { id: "0", text: translatePhrase("") } });
        $("#ddlCountry").select2({ theme: "classic", allowClear: true, placeholder: translatePhrase("") });
        $("#ddlCampaigns").select2({  allowClear: true, placeholder: translatePhrase("") });

        // if only one user means, user select got the logged in user, so hide the drop down
        var usersLength = $('#ddlUser').children('option').length;
        if (usersLength === 1) {
            $("#userContainer").addClass("hide");
        }

        self.LoadMondays();
        $("#ddlYear").change(function () {
            self.LoadMondays();
        });

        // task popup
        $("#ddlSalesStage").select2({ placeholder: "Sales Stage", allowClear: false });
        $ddlTaskSalesReps.select2({ placeholder: "Sales Rep", allowClear: false });
        $("#ddlTaskRelatedDeal").select2({ placeholder: "Deal", allowClear: false });
        $("#ddlTaskRelatedContacts").select2({ placeholder: "Contacts", allowClear: false });
        $('#ddlTaskSalesReps').on("select2:select", function () {
            $("#ddlTaskSalesReps+.select2-container .select2-selection").removeClass('error');
        });

    };


    this.LoadMondays = function () {
        var year = $("#ddlYear").val();
        $.ajax({
            type: "GET",
            url: "/api/Dropdown/GetMondays?year=" + year,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 30000,
            data: '',
            success: function (response) {
                var mondays = response.Mondays;
                $.each(mondays, function (i, ele) {
                    $('#ddlWeekOfYear').append($("<option></option>")
                        .attr("value", ele)
                        .text(ele));
                });
                $('#ddlWeekOfYear').val(response.DefaultDate).trigger('change');
            },
            beforeSend: function () {
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
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

    this.ExportToExcel = function () {
        // export excel function goes here
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
         
        // Date  
        filters.DateMonday = $('#ddlWeekOfYear').val();


        // global users
        var globalUserIds = [];
        if ($('#ddlUsers option').length > 0) {
            $('#ddlUsers option:selected').each(function (i, selected) {
                var val = $(selected).val();
                if (val) globalUserIds.push(val);
            });

            // no users selected - so pass all the users
            if (globalUserIds.length === 0) {
                $('#ddlUsers option').each(function (i, uid) {
                    var val = parseInt($(uid).val());
                    if (val && val > 0) globalUserIds.push(val);
                });
            }
        }
        filters.GlobalUserIds = globalUserIds;
          
        $.each([
            "Categories",
            "Locations",
            "Country",
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
        // AJAX to retrieve activities
        $.ajax({
            type: "POST",
            url: "/api/Report/GetActivitiesByWeekReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 300000,
            data: JSON.stringify(filters),
            success: function (activityResponse) {
                removeSpinner();
                // bind activities
                self.BindActivities(activityResponse.ActivitiesByDay);


                //excel
                $("#btnExcel").removeClass("hide");
                $("#btnExcel").unbind("click").click(function () {
                    location.href = activityResponse.ExcelUri;
                });


                // set the table as a data table
                var tblHeight = activityResponse.ActivitiesByDay.length > 10 ? 650 : 400;
                $divReportContent.removeClass("hide");

                $divNoItems.addClass('hide');

            },
            beforeSend: function () {
                addSpinner();
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };


    // bind activities
    this.BindActivities = function (activities) {

        $.each(activities, function (i, activityByDay) {

            //day header
            var $tr = $("<tr/>");
            var $td = $("<td/>", { "colspan": "11", "class": "td-day-header", "html": activityByDay.DayOfWeekStr });
            $tr.append($td);
            $activities.append($tr);

            // get template for activity row
            reportItemHtml = self.GetReportRowHtml();

            if (activityByDay.Activities.length > 0) {
                // iterate and bind activities
                $.each(activityByDay.Activities, function (i, activity) {
                    var $tr = self.GetActivityItemHtml(activity);
                    $activities.append($tr);
                });
                // init event add/edit
                new CalendarEvent().Init();
            } else {
                $tr = $("<tr/>");
                $td = $("<td/>", { "colspan": "9", "html": "No activities found." });
                $tr.append($td);
                $activities.append($tr);
            }

        });
    };



    // get activity row html
    this.GetActivityItemHtml = function (activity) {
        var $tr = $(reportItemHtml);
        $tr.attr("data-id", activity.ActivityId);
        // bind activity details
        $tr.find("[data-name='activity-type']").html(activity.ActivityType);
        $tr.find("[data-name='subject']").html(activity.Subject);
        $tr.find("[data-name='user']").html(activity.User);
        $tr.find("[data-name='activity-date']").html(activity.ActivityDateStr);
        $tr.find("[data-name='location']").html(activity.LocationName);
        $tr.find("[data-name='description']").html(activity.Description);
        $tr.find("[data-name='completed']").html(activity.ActivityType === "Task" ? (activity.Completed ? "Yes" : "No") : "");
        $tr.find("[data-name='percentage']").html(activity.Percentage);
        $tr.find("[data-name='createddate']").html(moment(activity.CreatedDate).format("DD-MMM-YY @ HH:mm"));
        $tr.find("[data-name='lastupdated']").html(moment(activity.LastUpdatedDate).format("DD-MMM-YY @ HH:mm"));

        // deal id
        if (activity.DealIds > 0) {
            var $a = $("<a/>", { "html": activity.Deal });
            var dealDetailLink = "/Deals/DealDetail/dealdetail.aspx?dealId=" + activity.DealId + "&dealsubscriberid=" + activity.SubscriberId;
            $a.attr("href", dealDetailLink);
            $a.attr("target", "_blank");

            $tr.find("[data-name='deal']").append($a);
        } else {
            $tr.find("[data-name='deal']").html("");
        }

        // company  
        if (activity.CompanyId > 0) {
            $a = $("<a/>", { "html": activity.CompanyName });
            var companyDetailLink = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + activity.CompanyId + "&subscriberId=" + activity.CompanySubscriberId;
            $a.attr("href", companyDetailLink);
            $a.attr("target", "_blank");

            var $p = $("<p/>");
            $p.append($a);
            $tr.find("[data-name='companies']").append($p);
        } else {
            $tr.find("[data-name='companies']").html("");
        }

        // contacts
        if (activity.Contacts !== null) {
            $.each(activity.Contacts, function (i, contact) {
                var $a = $("<a/>", { "html": contact.name });
                var contactDetailLink = "/Contacts/ContactDetail/ContactDetail.aspx?contactid=" + contact.id;
                $a.attr("href", contactDetailLink);
                $a.attr("target", "_blank");

                var $p = $("<p/>");
                $p.append($a);
                $tr.find("[data-name='contacts']").append($p);
            });
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
        }

        $tr.find("[data-name='edit-activity']").unbind("click").click(function () {
            if (activity.ActivityType === "Task") {
                self.OpenTaskAddEditDialog(activity.ActivtyId);
            }
        });

        return $tr;
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

    this.OpenTaskAddEditDialog = function (taskId) {
        var taskName = "";
        if (parseInt(taskId) > 0) {
            taskName = "Edit Task";
        }
        var iframeUrl = "/Tasks/TaskAddEdit/TaskAddEdit.aspx?activityId=" + taskId;
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

    // AJAX to retrieve
    $.ajax({
        type: "POST",
        url: "/api/Dropdown/GetAllSalesReps",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(request),
        success: function (salesReps) {
            $ddlUsers.html("");
            $ddlUsers.append($('<option>', { value: "", text: "" }));
            $.each(salesReps, function (i, ele) {
                $ddlUsers.append($('<option>', { value: ele.SelectValue, text: ele.SelectText }));
            });
        },
        beforeSend: function () {
        },
        error: function () {
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
    new Activites().RetrieveActivities();
}


// show the filters
$(document).ready(function () {
    $("#divReportFilter").removeClass("hide");
});