var $calendar = $('#calendar');
var $ddlUsers = $("#ddlUsers");
var userId = $("#lblUserId").text();
var userIdGlobal = $("#lblUserIdGlobal").text();
var subscriberId = $("#lblSubscriberId").text();
var $divLoading = $("#divLoading");
var $chkEvents = $("#chkEvents");
var $chkTasks = $("#chkTasks");

$(document).ready(function () {
    $('a.fc-ev-delete').click(function (e) {
        e.preventDefault();
        swal({
            title: translatePhrase("Delete Event?"),
            text: translatePhrase("Are you sure you want to delete this event?"),
            type: "error",
            showCancelButton: true,
            reverseButtons: true,
            showCloseButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!"),
            html: 'This event will be deleted from your calendar. <div class="chk-box-wrp"><input type="checkbox" id="sendcanclemsg" class="i-checks" checked><label for="sendcanclemsg"><div class="doc-title">Send cancellation message to attendees.</div></label></div>',
            icon: ''
        });
    });

    // init Select2
    $ddlUsers.select2({});

    // if only one user means, user select got the logged in user, so hide the dropdown
    var usersLength = $ddlUsers.children('option').length;

    if (usersLength === 1) {
        $("#userContainer").addClass("hide");
    }

    // TODO: BLUE
    // init iChecks
    $('.i-checks').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green'
    });

    /* initialize external events
     -----------------------------------------------------------------*/
    $('#external-events div.external-event').each(function () {
        // store data so the calendar knows to render an event upon drop
        $(this).data('event', {
            title: $.trim($(this).text()),  // use the element's text as the event title
            stick: true                     // maintain when user navigates (see docs on the renderEvent method)
        });

        // make the event draggable using jQuery UI
        $(this).draggable({
            zIndex: 1111999,
            revert: true,                   // will cause the event to go back to its
            revertDuration: 0               //  original position after the drag
        });
    });
    new Calendar().Init();
    new CalendarEvent().Init();

    // user change - refetch the events for user
    $ddlUsers.change(function () {
        //userId = $ddlUsers.val();
        RefetchEvents();
    });

    $chkEvents.change(function () { RefetchEvents();; });
    $chkTasks.change(function () { RefetchEvents();; });

    // run sync
    RunSync();
});

var Calendar = function () {
    var self = this;

    this.Init = function () {

        // initialize the calendar
        var date = new Date();
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();

        $calendar.fullCalendar({
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,agendaWeek,agendaDay'
            },
            // day click
            dayClick: dayClick,
            editable: true,
            droppable: true, // this allows things to be dropped onto the calendar
            drop: function () {
            },
            eventDrop: eventDrop,
            // get events
            events: fetchEvents,
            eventClick: function (event, jsEvent, view) {
                eventClick(event);
            },
            eventRender: function (event, element) {
                var limit = 23;
                if (event.title && event.title.length > limit) {
                    element.find('.fc-title').text(event.title.substr(0, limit) + '...');
                }

                if (event.description && event.description && event.description !== '') {
                    var descriptionArr = event.description.split("|");
                    if (descriptionArr.length > 0 && descriptionArr[0] !== 'null') {
                        element.find('.fc-title').append("<span class='fc-company'>" + descriptionArr[0] + "</span>");
                    }

                }

                var ntoday = new Date().getTime();
                var eventEnd = moment(event.end).valueOf();
                var eventStart = moment(event.start).valueOf();
                if (!event.end) {
                    if (eventStart < ntoday) {
                        element.addClass("past-event");
                        element.children().addClass("past-event");
                    } else {
                        element.addClass("future-event");
                        element.children().addClass("future-event");
                    }
                }
                else {
                    if (eventEnd < ntoday) {
                        element.addClass("past-event");
                        element.children().addClass("past-event");
                    } else {
                        element.addClass("future-event");
                        element.children().addClass("future-event");
                    }
                }
            }
        });

        function fetchEvents(start, end, timezone, callback) {
            var request = new Object();
            request.SubscriberId = $("#lblSubscriberId").text();
            request.DateFrom = moment(start).format("DD-MMM-YY");
            request.DateTo = moment(end).format("DD-MMM-YY");
            request.UserId = userId;
            request.OwnerUserIdGlobal = $("#ddlUsers").val();
            request.ActivityTypes = [];
            if ($chkEvents.is(":checked")) {
                request.ActivityTypes.push("EVENT");
            }
            if ($chkTasks.is(":checked")) {
                request.ActivityTypes.push("TASK");
            }

            $.ajax({
                type: "POST",
                url: "/api/activity/GetActivitiesForCalendar/",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(request),
                success: function (response) {
                    removeSpinner();
                    var calendarEvents = [];
                    $.each(response,
                        function (i, item) {
                            calendarEvents.push(self.GetEventObjExtended(new Object(), item));
                        });
                    // set calendar events
                    callback(calendarEvents);
                },
                beforeSend: function () {
                    addSpinner();
                },
                error: function (request) {
                    alert(JSON.stringify(request));
                }
            });
        }

        /* ---------- event click event show edit pop up ---------- */
        function eventClick(event, jsEvent, view) {
            var descriptionArr = (event.description + "").split("|");
            if (descriptionArr.length > 1) {
                if (descriptionArr[1] === "EVENT") {
                    new CalendarEvent().AddUpdateEvent(event.id, subscriberId);
                } else {
                    $('#addTaskDialog').modal('show');
                    loadTask(event.id, subscriberId);
                }
            } else {
                new CalendarEvent().AddUpdateEvent(calEvent.id, subscriberId);
            }
        }

        /* ---------- day click event show context menu ---------- */
        function dayClick(date, jsEvent, view) {
            new CalendarEvent().AddUpdateEvent(0, subscriberId, date);
        }

        function eventDrop(event) {
            var descriptionArr = (event.description + "").split("|");
            if (descriptionArr.length > 1) {
                if (descriptionArr[1] === "EVENT") {
                    updateEventDate(event.id, event.start);
                } else {
                    ChangeActivityTaskDate(event.id, event.start);
                }
            } else {
                updateEventDate(event.id, event.start);
            }
        }
    };

    this.GetEventObjExtended = function (event, dbitem) {
        if (dbitem.ActivityType === "EVENT")
            event.title = dbitem.Subject;
        else if (dbitem.ActivityType === "TASK")
            event.title = dbitem.TaskName;

        event.description = dbitem.CompanyName + "|" + dbitem.ActivityType;
        event.end = moment(dbitem.EndDateTime);
        event.allDay = dbitem.IsAllDay; 
        event.id = dbitem.ActivityId;

        // set dates
        if (dbitem.ActivityType === "EVENT") {
            var startDate = moment(dbitem.StartDateTime);
            var endDate = null;
            if (!dbitem.IsAllDay) {
                // set end date
                if (dbitem.Duration !== "0") {
                    endDate = moment(startDate);
                    endDate.add(dbitem.Duration, 'm');
                }
            } else {
                event.end = startDate;
            }
            event.start = startDate;
        } else if (dbitem.ActivityType === "TASK") {
            startDate = moment(dbitem.ActivityDate);
            event.start = startDate;
            event.allDay = true;
        }

        // set calendar event color from crm category
        event.backgroundColor = "#" + dbitem.CategoryColor;
        event.borderColor = "#" + dbitem.CategoryColor;

        if (!dbitem.CategoryColor) {
            if (dbitem.EventType === 'Meeting') {
                event.backgroundColor = "#0C629C";
                event.borderColor = "#0C629C";
            } else if (dbitem.EventType === 'Call') {
                event.backgroundColor = "#5ABDED";
                event.borderColor = "#5ABDED";
            } else {
                event.backgroundColor = "#329932";
                event.borderColor = "#329932";
            }
        }
        return event;
    };

    this.OpenEventAddEditPopup = function (eventid, date) {
        location.href = "/CalendarEvents/CalendarEventAddEdit/CalendarEventAddEdit.aspx?eventId=" +
            eventid +
            "&date=" +
            moment(date).format("DD-MM-YY HH:mm");
    };
};


function RunSync() {
    $.ajax({
        type: "GET",
        url: "/api/sync/dosync/?userId=" + userId + "&subscriberId=" + subscriberId,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {},
        success: function (response) {
            // swal.close(); 
        },
        beforeSend: function () {
            // swal({ text: translatePhrase("Sync is running, this might take some time") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: true });
        },
        error: function (request) { }
    });
}

function RefreshEvents() {
    // remove dialog
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // do the following to get rid of the padding
    $('body').removeAttr('style');
    RefetchEvents();
}

function deleteEvent(eventId) {
    swal({
        title: translatePhrase("Delete Event!"),
        text: translatePhrase("Are you sure you want to delete this event?"),
        type: "error",
        showCancelButton: true,
        reverseButtons: true,
        showCloseButton: true,
        confirmButtonColor: "#f27474",
        confirmButtonText: translatePhrase("Yes, Delete!"),
        html: 'This event will be deleted from your calendar. <div class="chk-box-wrp"><input type="checkbox" id="sendcanclemsg" class="i-checks" checked><label for="sendcanclemsg"><div class="doc-title">Send cancellation message to attendees.</div></label></div>',
        icon: ''
    },
        function () {
            $.ajax({
                type: "GET",
                url: "/api/CalendarEvent/DeleteCalendarEvent/?calendarEventId=" + eventId + "&userId=" + userId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (response) {
                    // remove dialog
                    $(".modalWrapper").remove();
                    // remove model-open to get the scroll bar of the parent
                    $('body').removeClass('modal-open');
                    // do the following to get rid of the padding
                    $('body').removeAttr('style');

                    if (response) {
                        swal({
                            title: translatePhrase("Event deleted successfully!"),
                            type: "success",
                            showCancelButton: false
                        });
                        new RunSync();
                        RefetchEvents();
                    }
                },
                beforeSend: function () {
                    swal({
                        text: translatePhrase("Deleting event") + "...",
                        title: "<img src='/_content/_img/loading_40.gif'/>",
                        showConfirmButton: false,
                        allowOutsideClick: false, html: true
                    });
                },
                error: function (request) { }
            });
        });
}

function RefetchEvents() {
    $calendar.fullCalendar('refetchEvents');
}

// change the event date on event drag and drop
function updateEventDate(eventId, startDateTime) {
    var event = new Object();
    event.ActivityId = eventId;
    event.SubscriberId = subscriberId;
    event.UpdateUserId = userId;
    event.StartDateTime = moment(startDateTime).format("DD-MMM-YY HH:mm");

    $.ajax({
        type: "POST",
        url: "/api/CalendarEvent/UpdateCalendarEventDates",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 50000,
        data: JSON.stringify(event),
        success: function () {
            new RunSync();
            removeSpinner();
        }, beforeSend: function () {
        }, error: function () {
            addSpinner();
        }
    });
}


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


// change the task date on task drag and drop
function ChangeActivityTaskDate(activityId, dueDate) {
    var task = new Object();
    task.ActivityId = activityId;
    task.SubscriberId = subscriberId;
    task.UpdateUserId = userId;
    task.DueDate = moment(dueDate).format("DD-MMM-YY");
    $.ajax({
        type: "POST",
        url: "/api/Activity/ChangeActivityTaskDate",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 50000,
        data: JSON.stringify(task),
        success: function () {
            removeSpinner();
        }, beforeSend: function () {
        }, error: function () {
            addSpinner();
        }
    });
}
