
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var globalUserId = parseInt($("#lblUserIdGlobal").text());
var $ddlSalesStage = $("#ddlSalesStage");

$(function () {
    new Events().Init();
    new ActivityChart().Init();
    new Tasks().Init();
});

var Events = function () {
    var self = this;
    var $noEvents = $(".empty_event");
    var $divEvents = $("#divEvents");

    this.Init = function () {
        self.RetrieveEvents();

        new CalendarEvent().Init();
    };

    this.RetrieveEvents = function () {
        $.ajax({
            type: "GET",
            url: "/api/calendarevent/GetUserEventsForActivities?subscriberId=" + subscriberId + "&userId=" + userId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '',
            success: function (events) {
                $divEvents.html("");
                if (events.length > 0) {
                    $noEvents.addClass('hide');
                    self.BindEvents(events);
                    new CalendarEvent().Init();
                } else {
                    $noEvents.removeClass('hide');
                }
            },
            beforeSend: function () {
                // TODO: use new spinner
                $divEvents.html('<div class="text-center  MB50 MT50"><img src="/_content/_img/loading_20.gif" class="MR10" />' + "Loading Calendar..." + '</span ></div>');
            },
            error: function (request) {
            }
        });
    };

    this.BindEvents = function (events) {
        $divEvents.html("");
        // iterate through and bind events
        $.each(events, function (i, event) {
            var $eventItem = self.GetEventItemHtml(event);
            $divEvents.append($eventItem);
        });
    };

    this.GetEventItemHtml = function (event) {
        var $eventItem = $("<div/>", { "class": "event-wrap", "data-event-id": event.ActivityId });

        // title
        var $titleWrap = $("<div/>", { "class": "event-title" });

        var $aEventSubject = $('<a href="javascript:void(0)" class="hover-link">' + event.Subject + '</a>');
        $aEventSubject.attr("data-action", "edit-event");
        $aEventSubject.attr("event-id", event.ActivityId);
        $aEventSubject.attr("event-subscriber-id", event.SubscriberId);
        $titleWrap.append($aEventSubject);

        var $detailRow = $("<div/>", { "class": "row" });
        $eventItem.append($detailRow);

        var $detailRowCol4 = $("<div/>", { "class": "col-md-12 col-sm-12 event-info" });
        $detailRow.append($detailRowCol4);

        $detailRowCol4.append($titleWrap);

        var $detailTimeWrp = $("<div/>", { "class": "ev-datetime" });
        $detailRowCol4.append($detailTimeWrp);

        // date
        var $detailRowDate = $("<div/>", { "class": "ev-date", "html": moment(event.StartDateTime).format("ddd, DD MMMM, YYYY") + " @ " + moment(event.StartDateTime).format("HH:mm A") });
        $detailTimeWrp.append($detailRowDate);

        // detail location wrap
        if (event.Location && event.Location !== "") {
            var $detailLocWrp = $("<div/>", { "class": "ev-loc" });
            $detailRowCol4.append($detailLocWrp);

            var $detailLocation = $("<div/>", { "class": "ev-adds", "html": event.Location });
            $detailLocWrp.append($detailLocation);
        }

        // company name
        if (event.CompanyName && event.CompanyName !== "") {
            var $detailCompanyName = $("<div/>", { "class": "PL25", "html": event.CompanyName });
            $detailRowCol4.append($detailCompanyName);
        }

        // event category
        if (event.CategoryName !== "") {
            $detailEventType = $("<div/>", { "class": "PL25", "html": event.CategoryName });
            $detailRowCol4.append($detailEventType);
        }

        // Right column - guests
        var $detailRowRightCol = $("<div/>", { "class": "col-md-5 col-sm-5 event-info" });
        $detailRow.append($detailRowRightCol);

        if (event.Invites && event.Invites !== "") {
            var $midEventGuestWrp = $("<div/>", { "class": "ev-guests" });
            $detailRowRightCol.append($midEventGuestWrp);

            //var $midEventGuests = $("<div/>", { "class": "guest-title", "html": "Guests" });
            //$midEventGuestWrp.append($midEventGuests);

            var $eventGuest = $("<div/>", { "class": "ev-guest", "html": event.Invites });
            $midEventGuestWrp.append($eventGuest);
        }

        return $eventItem;
    };


    this.DeleteEvent = function (eventId, $eventItem) {
        swal({
            title: translatePhrase("Delete Event!"),
            text: translatePhrase("Are you sure you want to delete this event?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/CalendarEvent/DeleteCalendarEvent/?calendarEventId=" +
                        eventId +
                        "&userId=" +
                        userId +
                        "&subscriberId=" +
                        subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        swal.close();
                        if (response) {
                            $eventItem.fadeOut("slow", function () {
                                $eventItem.remove();
                                if ($divEvents.find(".event-item").length === 0) {
                                    $noEvents.removeClass('hide');
                                } else {
                                    $noEvents.addClass('hide');
                                }
                            });
                        }

                        new RunSync();
                    },
                    beforeSend: function () {
                        // TODO: new spinner
                        swal({ text: "Please Wait...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: true });
                    },
                    error: function (request) { swal.close(); }
                });
            }
        });
    };
};

var RunSync = function () {
    $.ajax({
        type: "GET",
        url: "/api/sync/dosync/?userId=" + userId + "&subscriberId=" + subscriberId,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {},
        success: function (response) {
           
        },
        beforeSend: function () {
            
        },
        error: function (request) { }
    });
};


var Tasks = function () {
    var self = this;
    var $activeTasks = $("#active-task");
    var $completedTasks = $("#completed-task");
    var taskType = "active";
    var $noTasks = $(".no-tasks");
    var $divTasks = $("#divTasks");

    self.Init = function () {
        $("#divEditTask").hide();
        $("#divListTasks").show();
        $('.deal-btn-wrp a').unbind("click").click(function (e) {
            e.preventDefault();
            self.TaskToggleClick($(this));
        });

        $("[data-action='add-task']").unbind('click').click(function () { 
            $("#ddlTaskSalesReps").val(globalUserId).trigger("change"); 
        });

        $('#addTaskDialog').on('shown.bs.modal', function () {
            $("#txtTaskTitle").focus();
        });
        self.RetrieveTasks();
    };
    
    self.TaskToggleClick = function ($currentAtag) {
        $currentAtag.parent().find('a').removeClass('active');
        $currentAtag.addClass('active');
        var dataLoaded = false;
        if ($currentAtag.attr("data-value") === "Active") {
            $activeTasks.removeClass("hide");
            $completedTasks.addClass("hide");
            taskType = "active";
            dataLoaded = $activeTasks.hasClass("data-loaded");
        } else {
            $activeTasks.addClass("hide");
            $completedTasks.removeClass("hide");
            taskType = "completed";
            dataLoaded = $completedTasks.hasClass("data-loaded");
        }
        if (!dataLoaded)
            self.RetrieveTasks();
    };
    
    self.RetrieveTasks = function () {
        var request = new Object();
        request.SubscriberId = subscriberId;
        request.UserId = userId;
        request.UserIdGlobal = globalUserId;
        request.LoggedinUserId = userId;
        request.RecordsPerPage = 10;
        request.CurrentPage = 1;
        request.SortBy = "createddate desc";
        request.Completed = taskType !== "active";
        request.DueDateFrom = moment()+"";
        var $currentDiv = taskType === "active" ? $activeTasks : $completedTasks;
        $currentDiv.html("");
        $.ajax({
            type: "POST",
            url: "/api/Task/GetTasks",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (tasks) {
                $currentDiv.html("");
                $currentDiv.addClass("data-loaded");
                if (tasks.length > 0) {
                    $noTasks.addClass("hide");
                    $divTasks.removeClass("hide");
                    self.BindTasks(tasks, $currentDiv);
                } else {
                    $noTasks.removeClass("hide");
                    $divTasks.addClass("hide");
                }
            },
            beforeSend: function () {
                $currentDiv.html('<div class="text-center MT30 MB30"><img src="/_content/_img/loading_20.gif" class="MR10" />Loading Tasks...</div>');
            },
            error: function () {
            }
        });
    };
    
    self.BindTasks = function (tasks, $currentDiv) {
        $.each(tasks, function (i, task) {
            if (!task.Completed) {
                var $taskItem = self.GetTaskItemHtml(task, i);
                $currentDiv.append($taskItem);
            }
        });
    };
    
    self.GetTaskItemHtml = function (task, i) {
        var $outerWrap = $("<div />", { "class": "task-wrap", "data-id": task.TaskId, "data-subscriber-id": task.SubscriberId });
        
        // completed check box
        var inputId = "activity"+task.ActivityId;
        var $input = $("<input/>", {
            "id": inputId,
            "type": "checkbox",
            "data-props": _get_attribute_props()
        });
        $outerWrap.append($input);
        $input.change(function() {
            var props = $(this).data('props');
            self.ToggleCompleted({
                state: $(this).is(':checked'),
                $input: $(this),
                activityId: props.activityId,
                subscriberId: props.subscriberId
            });
        });

        // title 
        var $lableTitle = $("<label/>", { "for": inputId });
        var $aTitle = $("<a />", { 
            "class": "task-title hover-link",
            "href": "javascript:void(0)",
            "data-toggle": "modal",
            "data-target": "#addTaskDialog",
            "data-modal-props": _get_attribute_props(),
            "html": task.TaskName
        });
        $lableTitle.append($aTitle);
        $outerWrap.append($lableTitle);
        
        //detials
        var $detailRow = $("<div/>", { "class": "row" });
        $outerWrap.append($detailRow);
        var $detailRowCol4 = $("<div/>", { "class": "col-md-6 col-sm-6 event-info" });
        $detailRow.append($detailRowCol4);

        // due date
        var $dueDate = $("<div />", { "class": "task-day" });
        $dueDate.append($("<span/>", { "class": "clearfix", "html": "<i class='icon-clock iconbox'></i><span class='task-txt'>" + moment(task.DueDate).format("ddd, DD MMMM, YYYY")+'</span>'}));
        $detailRowCol4.append($dueDate);

        // description
        var $description = $("<div />", { "class": "task-content" });
        if (task.Description !== "") {
            $description.append($("<span/>", { "class": "clearfix", "html": "<i class='icon-Notes iconbox'></i><span class='task-txt'>" + task.Description +'</span>' }));
            $detailRowCol4.append($description);
        }

        var $detailRowRightCol = $("<div/>", { "class": "col-md-5 col-sm-5 event-info" });
        $detailRow.append($detailRowRightCol);
        if (task.CompanyName !== "" && task.CompanyName !== null) {
            var company = $("<div />");
            company.append($("<span/>", { "class": "clearfix", "html": "<i class='icon-business iconbox'></i><span class='task-txt'>" + task.CompanyName +'</span>'}));
            $detailRowRightCol.append(company);
        }

        if (task.ContactNames !== "" && task.ContactNames !== null) {
            var contact = $("<div />");
            contact.append($("<span/>", { "class": "clearfix", "html": "<i class='icon-users iconbox'></i><span class='task-txt'>" + task.ContactNames +'</span>'}));
            $detailRowRightCol.append(contact);
        }

        if (task.DealNames !== "" && task.DealNames !== null) {
            var deal = $("<div />");
            deal.append($("<span/>", { "class": "clearfix", "html": "<i class='icon-deals iconbox'></i><span class='task-txt'>" + task.DealNames +'</span>'}));
            $detailRowRightCol.append(deal);
        }
        return $outerWrap;

        function _get_attribute_props() {
            var props = {
                type: 'edit',
                activityId: task.ActivityId,
                subscriberId: task.SubscriberId
            };
            return JSON.stringify(props);
        }
    };
    
    self.ToggleCompleted = function (props) {
        $.ajax({
            type: "GET",
            url: "/api/task/ToggleTaskCompleted/?" +
                "taskId=" + props.activityId +
                "&state=" + props.state +
                "&userId=" + userId +
                "&subscriberId=" + props.subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {},
            success: function (response) {
                if (response) _show_success_alert(props.state);
                else _show_error_alert();
                removeSpinner();
            },
            beforeSend: function () { addSpinner(); },
            error: function () { 
                _show_error_alert();
                removeSpinner();
            }
        });
        
        function _show_error_alert() {
            props.$input.prop("checked", !props.state);
            swal({
                title: "Error: The task status was not updated.",
                type: "error",
                confirmButtonColor: "#ea7d7d",
                confirmButtonText: "OK",
                closeOnConfirm: false,
                showCloseButton: true,
                reverseButtons: false
            });
        }

        function _show_success_alert(state) {
            swal({
                title: "Task Status: "+(state?"Completed":"Active"),
                type: "success",
                confirmButtonColor: "#ea7d7d",
                confirmButtonText: "OK",
                closeOnConfirm: false,
                showCloseButton: true,
                reverseButtons: false
            });
        }
    };
};

// Slim Scroll
$(document).ready(function () {
    $('.actvity-pg #divEventsContainer').slimscroll({
        size: '8px',
        height: '300px',
        color: '#dde3ea',
        railOpacity: 1,
        alwaysVisible: true,
        distance: '9px'
    });

    $('.actvity-pg #divTasksContainer').slimscroll({
        size: '8px',
        height: '300px',
        color: '#dde3ea',
        railOpacity: 1,
        alwaysVisible: true,
        distance: '9px'
    });
});


var ActivityChart = function () {
    var self = this;
    var chart;
    var $divChartLoading = $("#divChartLoading");
    var $divActivityChart = $("#divActivityChart");

    this.Init = function () {
        // chart date click
        $(".chart-btns").find("a").unbind("click").click(function (event) {
            event.preventDefault();
            $('.chart-btns a').removeClass('active');
            $(this).addClass('active');

            var dataMonthCount = parseInt($(this).attr("data-month-count"));
            var startDate = moment().subtract(dataMonthCount, 'months').startOf('month');
            var endDate = moment().subtract(1, 'months').endOf('month');
            retrieveChartData(startDate, endDate);
        });
        // load last month
        $(".chart-btns").find("a[data-month-count=1]").click();
    };

    function retrieveChartData(startDate, endDate) {
        var request = new Object();
        request.SubscriberId = subscriberId;
        request.UserId = userId;
        request.StartDate = startDate;
        request.EndDate = endDate;
        $.ajax({
            type: "POST",
            url: "/api/Activity/GetActivityChartData",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (chartData) {
                $divChartLoading.addClass("hide");
                $divActivityChart.removeClass("hide");
                if (chart)
                    chart.destroy();
                // bind chart table
                bindChartTable(chartData, request);
                // bind actions
                self.BindActions(request);
                // load the chart
                loadChart(chartData);
            },
            beforeSend: function () {
                $divChartLoading.removeClass("hide");
                $divActivityChart.addClass("hide");
            },
            error: function () {
            }
        });
    }

    function bindChartTable(chartData) {
        var $tblActivities = $("#tblActivities");
        $tblActivities.html("");

        $.each(chartData.ActivityCountByTypes, function (i, e) {
            var $tr = $("<tr>");
            $tr.append($("<td/>", { "html": e.ActivityType }));
            switch (e.ActivityType) {
                case "New Deals": $tr.append($("<td/>", { "html": "<a data-name='new-deals'>" + formatNumber(e.ActivityCount) + "<a/>" })); break;
                case "Won Deals": $tr.append($("<td/>", { "html": "<a data-name='won-deals'>" + formatNumber(e.ActivityCount) + "<a/>" })); break;
                case "Lost Deals": $tr.append($("<td/>", { "html": "<a data-name='lost-deals'>" + formatNumber(e.ActivityCount) + "<a/>" })); break;
                case "Tasks": $tr.append($("<td/>", { "html": "<a data-name='tasks'>" + formatNumber(e.ActivityCount) + "<a/>" })); break;
                case "Events": $tr.append($("<td/>", { "html": "<a data-name='events'>" + formatNumber(e.ActivityCount) + "<a/>" })); break;
                case "New Companies": $tr.append($("<td/>", { "html": "<a data-name='new-companies'>" + formatNumber(e.ActivityCount) + "<a/>" })); break;
                case "Logins": $tr.append($("<td/>", { "html": "<a data-name='logins' >" + formatNumber(e.ActivityCount) + "<a/>" })); break;
                case "Notes": $tr.append($("<td/>", { "html": "<a data-name='notes'>" + formatNumber(e.ActivityCount) + "<a/>" })); break;
                default:
            }

            $tblActivities.append($tr);
        });


    }

    function loadChart(chartData) {

        var MONTHS = [];
        var DATA = [];
        var count = 0;
        $.each(chartData.ChartData, function (i, e) {

            if (count <= 2) {
                MONTHS.push(moment(e.ActivityDate).format("MMM DD"));
            } else {
                MONTHS.push(moment(e.ActivityDate).format("DD"));
            }

            DATA.push({
                t: e.ActivityDate,
                y: e.ActivityCount
            });

            count++;
        });

        var config = {
            type: 'line',
            data: {
                labels: MONTHS,
                datasets: [{
                    label: '',
                    borderColor: '#acdef6',
                    borderWidth: '0',
                    backgroundColor: 'rgba(172,222,246,.9)',
                    pointRadius: '0',
                    data: DATA
                }]
            },
            options: {
                legend: {
                    display: false
                },
                responsive: true,
				maintainAspectRatio: false,
                title: {
                    display: false,
                    text: 'Chart.js Line Chart - Stacked Area'
                },
                tooltips: {
                    mode: 'index'
                },
                hover: {
                    mode: 'index'
                },
                scales: {
                    xAxes: [{
                        scaleLabel: {
                            display: false,
                            labelString: 'Month'
                        },
                        ticks: {
                            fontFamily: "'Open Sans', sans-serif",
                            fontColor: "#6d7b8c",
                            fontSize: 13,
                            autoSkip: true,
                            autoSkipPadding: 10,
                            maxRotation: 0,
                        },
                        gridLines: { color: "#bfcad7" }
                    }],
                    yAxes: [{
                        stacked: true,
                        scaleLabel: {
                            display: true,
                            labelString: '',
                            fontSize: 14,
                            fontColor: '#3e4955'
                        },
                        ticks: {
                            fontFamily: "'Open Sans', sans-serif",
                            fontColor: "#6d7b8c",
                            fontStyle: '400',
                            fontSize: 13,
                            autoSkip: true,
                            padding: 3,
                            autoSkipPadding: 20,
                            stepSize: 30
                        },
                        gridLines: { color: "#bfcad7" }
                    }]
                }
            }
        };

        var ctx = document.getElementById('activity-chart').getContext('2d');
        chart = new Chart(ctx, config);
    }



    this.BindActions = function (request) {

        var $tblActivities = $("#tblActivities");
        $tblActivities.find("[data-name='logins']").unbind("click").click(function () {
            var val = parseInt($(this).html());
            if (val > 0) {
                self.OpenLoginsDetailPopup(request);
            }
        });
        $tblActivities.find("[data-name='events']").unbind("click").click(function () {
            var val = parseInt($(this).html());
            if (val > 0) {
                self.OpenMeetingsDetailPopup(request);
            }
        });
        $tblActivities.find("[data-name='tasks']").unbind("click").click(function () {
            var val = parseInt($(this).html());
            if (val > 0) {
                self.OpenTasksDetailPopup(request);
            }
        });
        $tblActivities.find("[data-name='new-deals']").unbind("click").click(function () {
            var val = parseInt($(this).html());
            if (val > 0) {
                self.OpenDealsDetailPopup(request, 'New');
            }
        });
        $tblActivities.find("[data-name='won-deals']").unbind("click").click(function () {
            var val = parseInt($(this).html());
            if (val > 0) {
                self.OpenDealsDetailPopup(request, 'Won');
            }
        });
        $tblActivities.find("[data-name='lost-deals']").unbind("click").click(function () {
            var val = parseInt($(this).html());
            if (val > 0) {
                self.OpenDealsDetailPopup(request, 'Lost');
            }
        });
        $tblActivities.find("[data-name='notes']").unbind("click").click(function () {
            var val = parseInt($(this).html());
            if (val > 0) {
                self.OpenNotesDetailPopup(request);
            }
        });

        $tblActivities.find("[data-name='new-companies']").unbind("click").click(function () {
            var val = parseInt($(this).html());
            if (val > 0) {
                self.OpenCompanyDetailPopup(request);
            }
        });


    };


    this.OpenLoginsDetailPopup = function (request) {

        var iframeUrl = "/Activities/DetailViews/Logins.aspx?qr=" + JSON.stringify(request);
        $("<div/>", { "class": "modalWrapper", "id": "iframeDetails" }).launchModal({
            title: "Logins",
            modalClass: "modal-md",
            btnSuccessText: "",
            btnSuccessClass: "primary-btn",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

    this.OpenMeetingsDetailPopup = function (request) {

        var iframeUrl = "/Activities/DetailViews/Events.aspx?qr=" + JSON.stringify(request);
        $("<div/>", { "class": "modalWrapper", "id": "iframeDetails" }).launchModal({
            title: "Meetings",
            modalClass: "modal-md",
            btnSuccessText: "",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

    this.OpenTasksDetailPopup = function (request) {

        var iframeUrl = "/Activities/DetailViews/Tasks.aspx?qr=" + JSON.stringify(request);
        $("<div/>", { "class": "modalWrapper", "id": "iframeDetails" }).launchModal({
            title: "Tasks",
            modalClass: "modal-md",
            btnSuccessText: "",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

    this.OpenDealsDetailPopup = function (request, status) {

        var iframeUrl = "/Activities/DetailViews/Deals.aspx?status=" + status + "&qr=" + JSON.stringify(request);
        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframeDetails" }).launchModal({
            title: status + " Deals ",
            modalClass: "modal-md",
            btnSuccessText: "",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

    this.OpenNotesDetailPopup = function (request) {
        var iframeUrl = "/Activities/DetailViews/Notes.aspx?qr=" + JSON.stringify(request);
        $("<div/>", { "class": "modalWrapper", "id": "iframeDetails" }).launchModal({
            title: "Notes",
            modalClass: "modal-md",
            btnSuccessText: "",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

    this.OpenCompanyDetailPopup = function (request) {
        var iframeUrl = "/Activities/DetailViews/Companies.aspx?qr=" + JSON.stringify(request);
        $("<div/>", { "class": "modalWrapper", "id": "iframeDetails" }).launchModal({
            title: "Companies",
            modalClass: "modal-md",
            btnSuccessText: "",
            maxHeight: "600px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
            }
        });
    };

};

function RefreshEvents() {
    new Events().RetrieveEvents();
}
