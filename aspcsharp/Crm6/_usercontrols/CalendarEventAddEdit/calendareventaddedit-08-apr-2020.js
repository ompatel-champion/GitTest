var subscriberId = parseInt($("#lblSubscriberId").text());
var userId = parseInt($("#lblUserId").text());
var globalUserId = parseInt($("#lblUserIdGlobal").text());
var $btnDelete = $("#btnDelete");
var $chkAllDay = $("#chkAllDay");
var $company = $("#txtTagCompany");
var $contact = $("#txtTagContact");
var $deal = $("#txtTagDeal");
var $duration = $("[data-name='duration']");
var $durationId = $("[data-name='durationId']");
var $selectedCompanyJson = $("#txtTagCompanyId");
var $selectedContactJson = $("#txtTagContactId");
var $selectedDealJson = $("#txtTagDealId");
var $tagInviteIds = $("#txtTagInviteIds");
var $tagInvites = $("#txtTagInvite");
var $remindCheckboxes = $(".remindCheckboxes");
var $tblInvitesBody = $("#tblInvites tbody");
var $txtExternalAttendee = $("#txtExternalAttendee");
var $datepickers = $("[data-name='date']");
var $timepickers = $("[data-name='time']");
var username = $("#lblUsername").text();
var globalCompanyId = 0;
var changeMinutes = 30;
var changeDays = 0;
var $ddlInvites = $("#ddlInvites");
var $divCalendarEventWrapper = $("#divCalendarEventWrapper");
var categories = [];
var eventArgs = null;
var isDisableEventCompany = window.location.href.toLocaleLowerCase().indexOf("companydetail.aspx") > -1 ||
    window.location.href.toLocaleLowerCase().indexOf("contactdetail.aspx") > -1 ||
    window.location.href.toLocaleLowerCase().indexOf("dealdetail.aspx") > -1;
var isDisableEventDeal = window.location.href.toLocaleLowerCase().indexOf("dealdetail.aspx") > -1;
var $divEventCompanyContainer = $("#divEventCompanyContainer");
var $divDealContainer = $("#divDealContainer");


$(function () {
    eventArgs = {
        eventId: 0,
        eventSubscriberId: subscriberId,
        userSubscriberId: subscriberId,
        userId: userId,
        globalUserId: globalUserId,
        username: username,
        companyId: 0,
        globalcompanyId: globalCompanyId,
        companyName: "",
        date: moment(),
        contactId: 0,
        isModal: false,
        categoryName: "",
        eventChanged: false
    };
    new CalendarEvent().Init();
    new CalendarUsers().InitInvitesDropdown();
});


var CalendarEvent = function () {

    this.Init = function () {
        $("[data-action='add-event']").unbind("click").click(function () {
            var companyName = "";
            if (typeof $(this).attr('company-name') !== typeof undefined && $(this).attr('company-name') !== false) {
                companyName = $(this).attr('company-name');
            }
            var gCompanyId = 0;
            if (typeof $(this).attr('data-global-company-id') !== typeof undefined && $(this).attr('data-global-company-id') !== false) {
                gCompanyId = parseInt($(this).attr('data-global-company-id'));
            }
            var dealName = "";
            if (typeof $(this).attr('deal-name') !== typeof undefined && $(this).attr('deal-name') !== false) {
                dealName = $(this).attr('deal-name');
            }
            var dealId = 0;
            if (typeof $(this).attr('deal-id') !== typeof undefined && $(this).attr('deal-id') !== false) {
                dealId = parseInt($(this).attr('deal-id'));
            }
            var contactName = "";
            if (typeof $(this).attr('contact-name') !== typeof undefined && $(this).attr('contact-name') !== false) {
                contactName = $(this).attr('contact-name');
            }
            var contactId = 0;
            if (typeof $(this).attr('data-contact-id') !== typeof undefined && $(this).attr('data-contact-id') !== false) {
                contactId = parseInt($(this).attr('data-contact-id'));
            }

            eventArgs = {
                eventId: 0,
                eventSubscriberId: subscriberId,
                userSubscriberId: subscriberId,
                userId: userId,
                username: username,
                companyId: 0,
                globalUserId: globalUserId,
                companyName: companyName,
                globalcompanyId: gCompanyId,
                date: moment(),
                contactId: contactId,
                contactName: contactName,
                categoryName: "",
                dealId: dealId,
                dealName: dealName,
                eventChanged: false
            };
            _bindControlEvents();
        });

        $("[data-action='edit-event']").unbind("click").click(function () {
            eventArgs = {
                eventId: $(this).attr("event-id"),
                eventSubscriberId: $(this).attr("event-subscriber-id"),
                userSubscriberId: subscriberId,
                globalUserId: globalUserId,
                userId: userId,
                username: username,
                eventChanged: false
            };
            _bindControlEvents();
        });

        $('.users-list').slimScroll({
            height: '214px',
            color: '#dde3ea',
            railColor: '#f9fafb',
            size: '6px',
            distance: '3px',
            railOpacity: 1,
            alwaysVisible: true,
            opacity: 1
        });

    };

    this.AddUpdateEvent = function (eventId, eventSubscriberId, dt) {
        eventArgs = {
            eventId: eventId,
            eventSubscriberId: eventSubscriberId,
            userSubscriberId: eventSubscriberId,
            userId: userId,
            globalUserId: globalUserId,
            username: username,
            companyId: 0,
            companyName: "",
            globalcompanyId: 0,
            date: moment(),
            contactId: 0,
            contactName: "",
            categoryName: "",
            dealId: 0,
            dealName: "",
            eventChanged: false
        };
        _bindControlEvents(dt);
    };

    // bind/initialize control events
    function _bindControlEvents(dt) {

        $(".section-event-edit").addClass("hide");

        // initialize calendar
        $('.calendar').fullCalendar({
            defaultView: 'agendaDay',
            allDaySlot: false,
            minTime: "8:00:00",
            maxTime: "18:00:00",
            height: "auto",
            slotLabelFormat: 'H:mm',
            displayEventEnd: true,
            eventRender: function (event, element) {
                if (event.description && event.description !== '') {
                    element.find('.fc-title').append("<br/>" + event.description);
                }
            },
            eventAfterRender: function (event, element) {
                if (!event.allDay) {
                    try {
                        $(element).find("span").css("display", "inline");
                    } catch (e) { /*ignore*/ }
                }
            },
            titleFormat: 'ddd, DD MMMM, YYYY',
            header: {
                left: 'title',
                center: '',
                right: ''
            }
        });

        // reminder checkbox
        $remindCheckboxes.addClass("hide");

        if (eventArgs.eventId > 0) {
            $(".event-header").html("Update Event");
            $("[data-action='delete-event']").removeClass("hide");
            $("[data-action='save-event']").addClass("fullWidth");
        } else {
            $(".event-header").html("New Event");
        }

        $(".wrapper-outer").addClass("hide");
        $(".bootstrap-tagsinput").find("input").blur(function () {
            $(this).val("");
        });

        // init users
        new CalendarUsers().Init();
        // init attachment
        new CalendarEventAttachment().Init();

        $("#ddlDeal").empty().trigger("change");

        // if event id is passed load the event
        if (eventArgs.eventId > 0) {
            _loadCalendarEvent();

            // display event section
            $(".section-event-edit").removeClass("hide");
        }
        else {
            // set the company
            if (eventArgs.globalcompanyId > 0) {
                var company = new Option(eventArgs.companyName, eventArgs.globalcompanyId, false, true);
                $("#ddlCompany").append(company).trigger('change');
                $divDealContainer.removeClass("hide");
            }
            // set the deal
            if (eventArgs.dealId > 0) {
                var deal = new Option(eventArgs.dealName, eventArgs.dealId, false, true);
                $("#ddlDeal").append(deal).trigger('change');
                $divDealContainer.removeClass("hide");
            }

            $("#ddlPublicPrivate").val("Public").trigger('change');
            $("#ddlBusyFree").val("Busy").trigger('change');

            // set the contact
            if (eventArgs.contactId > 0) {
                var item = new Object();
                item.type = "contact";
                item.name = eventArgs.contactName;
                item.id = eventArgs.contactId;

                item.dataObj = new Object();
                item.dataObj.FirstName = eventArgs.contactName.split(" ").length > 0 ? eventArgs.contactName.split(" ")[0] : "";
                item.dataObj.LastName = eventArgs.contactName.split(" ").length > 1 ? eventArgs.contactName.split(" ")[1] : "";
                item.dataObj.ContactId = eventArgs.contactId;
                item.dataObj.SubscriberId = eventArgs.eventSubscriberId;
                item.invitetype = "contact";
                item.attendeetype = "Required";
                new CalendarUsers().AddToTable(item);
            }
            // set start and end date
            var date = moment();
            if (moment(dt).isValid()) {
                // if date passed set the date 
                date = moment(dt).local();
                date.hour(moment().hour());
                date.minutes(moment().minutes());
            }

            var rd = roundDate(date, moment.duration(30, "minutes"), "ceil");
            $("#txtStartDate").val(rd.format("DD-MMM-YY"));
            $("#ddlStartTime").val(rd.format("hh:mma")).trigger("change");
            var endDateTime = rd.add(30, 'minutes');
            $("#txtEndDate").val(endDateTime.format("DD-MMM-YY"));
            $("#ddlEndTime").val(endDateTime.format("hh:mma")).trigger("change");
            fullCalendarAddEvent(false);

            // display event section
            $(".section-event-edit").removeClass("hide");
        }

        // cancel event
        $("[data-action='cancel-event']").unbind("click").click(function () {
            $divCalendarEventWrapper.addClass("hidden");
            $(".section-event-edit").addClass("hide");
            $(".wrapper-outer").removeClass("hide");
            $("html, body").animate({ scrollTop: 0 }, "slow");
            _clearControls();
        });

        // save event
        $("[data-action='save-event']").unbind("click").click(function () {
            _saveCalendarEvent();
        });

        // delete event
        $("[data-action='delete-event']").unbind("click").click(function () {
            _deleteCalendarEvent();
        });

        // initialize date pickers
        $datepickers.datepicker({
            dateFormat: "dd-M-y",
            autoclose: true,
            showOn: "both",
            inline: true,
            beforeShow: function () {
                $('#ui-datepicker-div').addClass("due_datepicker");
            },
            buttonText: "<i class='icon-calendar'></i>"
            //  selectedDate: moment($("#txtEventDate").val())
        }).on('change', function () {
            eventArgs.eventChanged = true;
            changeStartEndDate($(this));
            // setup event on calendar
            fullCalendarAddEvent();
            //$(this).valid();
        });
        $('#ui-datepicker-div').addClass("due_datepicker");

        $datepickers.on('focus', function (e) {
            e.preventDefault();
            $(this).attr("autocomplete", "off");
        });

        // initialize time pickers
        $timepickers.timepicker({
            minTime: '7:00am',
            maxTime: '11:00pm',
            step: 15,
            scrollDefault: '9:00am',
            formatTime: "h:i A"
        }).on('change', function () {
            eventArgs.eventChanged = true;
            changeTimeDuration($(this));
            fullCalendarAddEvent();
        });

        // Edit Description
        $('#txtEventDescription').summernote({
            placeholder: 'Add a description',
            tabsize: 2,
            height: 78,
            minHeight: 'auto',
            tooltip: false,
            maxHeight: null,
            toolbar: [
                ['style', ['bold', 'italic', 'underline']],
                ['para', ['ul', 'ol']],
                ['link', ['link']]
            ],
            popover: {
                image: [],
                link: [],
                air: []
            }
        });

        // Edit Description - Blur Event
        $('#txtEventDescription').on('summernote.blur', function () {
            var content = $('#txtEventDescription').summernote('text');
        });

        $(".ddlInviteType").select2({ containerCssClass: "without-bg" });
        $("#ddlRepeat").select2({});
        $("#ddlSNtime").select2({});
        $("#ddlSEtime").select2({});
        $("#ddlTimezone").select2({ containerCssClass: "timezone without-bg" });
        $('#ddlEventType').select2({ width: '100%', minimumResultsForSearch: -1, allowClear: true });
        $('#ddlPublicPrivate').select2({ width: '100%', minimumResultsForSearch: -1 });
        $('#ddlBusyFree').select2({ width: '100%', minimumResultsForSearch: -1 });
        $('#ddlReminder').select2({
            width: '100%',
            minimumResultsForSearch: -1,
            allowClear: true,
            placeholder: ""
        });

        // reminder change
        $('#ddlReminder').on("select2:select", function (e) {
            var value = $(this).val();
            if (value !== "") {
                $remindCheckboxes.removeClass("hide");
            } else
                $remindCheckboxes.addClass("hide");
        });

        $('#ddlReminder').on("select2:unselecting", function (e) {
            $remindCheckboxes.addClass("hide");
        });

        // invites drop down
        new CalendarUsers().InitInvitesDropdown();

        if (isDisableEventCompany) {
            var $wrapper = $("#ddlCompany").closest(".form-group");
            if ($wrapper.get(0)) $wrapper.addClass("disabled");
        }
        // global company
        $("#ddlCompany").select2({
            minimumInputLength: 2,
            allowClear: true,
            placeholder: '',
            disabled: typeof isDisableEventCompany !== 'undefined' && isDisableEventCompany ? true : false,
            ajax: {
                url: function (obj) {
                    var keyword = obj.term ? obj.term : "";
                    return "/api/AutoComplete/?type=globalcompanywithpermission&UserId=" + eventArgs.userId + "&SusbcriberId=" + eventArgs.eventSubscriberId + "&prefix=" + keyword.replace("&", "%26");
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

        // deal
        initDealDropDown();

        // company change
        $('#ddlCompany').on("select2:select", function (e) {
            $("#ddlDeal").closest(".form-group").find(".select2-selection__choice__remove").each(function (i, obj) {
                $(this).click();
            });
            // var selected_element = $(e.currentTarget);
            // var select_val = selected_element.val();

            $divDealContainer.removeClass("hide");
        });

        $("#ddlCompany").on("select2:unselecting", function (e) {
            $("#ddlDeal").closest(".form-group").find(".select2-selection__choice__remove").each(function (i, obj) {
                $(this).click();
            });
            $divDealContainer.addClass("hide");
        });


        // deal change
        $('#ddlDeal').on("select2:select", function (e) {
            if (e.params.data) {
                if (e.params.data.dataObj) {
                    var company = new Option(e.params.data.dataObj.CompanyName, e.params.data.dataObj.CompanyIdGlobal, false, true);
                    $("#ddlCompany").append(company).trigger('change');
                }
            }
        });

        $('#chkAllDay').unbind("change").change(function () {
            if ($(this).is(":checked")) {
                $(".timeBox").hide();
                $(".end-date-section").hide();
                $(".datetimepicker").addClass("W100P");
                $(".datetimepicker").find(".date-picker").addClass("right-gray-border");
            } else {
                $(".timeBox").show();
                $(".end-date-section").show();
                $(".datetimepicker").removeClass("W100P");
                $(".datetimepicker").find(".date-picker").removeClass("right-gray-border");
            }
            eventArgs.eventChanged = true;
            fullCalendarAddEvent();
        });

        // Checkbox Event - Repeat
        $('#repeats').change(function () {
            var $wrapper = $(".cal-evpg .ddRepeat");
            var $ddlRepeat = $("#ddlRepeat");
            if (this.checked) {
                $ddlRepeat.val("Weekly").trigger("change");
                $wrapper.show();
            } else {
                $ddlRepeat.val(null).trigger("change");
                $wrapper.hide();
            }
        });

        // Checkbox Event - Send Notifications
        $('#sendNotifications').change(function () {
            if (this.checked) {
                $(".cal-evpg .notify-box").show();
                $(".cal-evpg .ddlReminder").hide();
                $('.calendar').fullCalendar('removeEvents');
            } else {
                $(".cal-evpg .notify-box").hide();
                $(".cal-evpg .ddlReminder").show();
                fullCalendarAddEvent();
            }
        });

        // text title blur event
        $("#txtEventTitle").unbind("blur").blur(function () { fullCalendarAddEvent(); eventArgs.eventChanged = true; });
        $("#txtLocation").unbind("blur").blur(function () { fullCalendarAddEvent(); eventArgs.eventChanged = true; });

        // get event categories
        _getEventCategories();

        // get timezones
        _getTimezones();

        $divCalendarEventWrapper.removeClass("hidden");

        // turn off autocomplete on focus
        $("#txtEventTitle,#txtLocation").on('focus', function (e) {
            e.preventDefault();
            $(this).attr("autocomplete", "off");
        });

        // focus title
        $("#txtEventTitle").focus();
    }


    function initDealDropDown() {
        $("#ddlDeal").select2({
            tags: true,
            disabled: typeof isDisableEventDeal !== 'undefined' && isDisableEventDeal ? true : false,
            "theme": "classic",
            ajax: {
                url: function (obj) {
                    var keyword = obj.term ? obj.term : "";
                    return "/api/AutoComplete/?type=globalcompanydealswithpermission&SusbcriberId=" + eventArgs.eventSubscriberId + "&UserId=" + eventArgs.userId + "&GlobalCompanyId=" + getSelectedCompanyId() + "&prefix=" + keyword;
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
            }, createTag: function (tag) {
                return null;
            }
        });
    }


    function changeTimeDuration($ele) {
        var id = $ele.attr('id');
        $(".error").removeClass("error");
        $(".form-group").removeClass("has-error");
        $(".error-text").html("");
        if (id === "ddlEndTime" && !$("#chkAllDay").is(":checked")) {
            var startDateTime = moment($("#txtStartDate").val());
            if (moment($("#ddlStartTime").val(), 'h:mm A').isValid()) {
                var time = moment($("#ddlStartTime").val(), 'h:mm A');
                startDateTime.hour(time.hour());
                startDateTime.minutes(time.minutes());
            }
            var endDateTime = moment($("#txtEndDate").val());
            if (!event.IsAllDay && moment($("#ddlEndTime").val(), 'h:mm A').isValid()) {
                time = moment($("#ddlEndTime").val(), 'h:mm A');
                endDateTime.hour(time.hour());
                endDateTime.minutes(time.minutes());
            }
            var minutesDiff = endDateTime.diff(startDateTime, 'minutes');
            if (minutesDiff < 0) {
                $("#txtEndDate").closest(".form-group").addClass("has-error");
                $("#txtEndDate").closest(".form-group").find(".error-text").html("Event must end after it starts.");
                return;
            }
        }
        if (id === "ddlStartTime") {
            var selectedTime = moment($("#ddlStartTime").val(), 'h:mm a');
            endDateTime = selectedTime.add(changeMinutes, 'minutes');
            $("#ddlEndTime").val(endDateTime.format("h:mma"));
        } else if (id === "ddlEndTime") {
            var endTime = moment($("#ddlEndTime").val(), 'h:mm a');
            var startTime = moment($("#ddlStartTime").val(), 'h:mm a');
            changeMinutes = endTime.diff(startTime, 'minutes');
        }
    }


    function changeStartEndDate($ele) {
        var id = $ele.attr('id');
        // validate dates
        $(".error").removeClass("error");
        $(".form-group").removeClass("has-error");
        $(".error-text").html("");
        if (id === "txtEndDate" && !$("#chkAllDay").is(":checked")) {
            var startDateTime = moment($("#txtStartDate").val());
            if (moment($("#ddlStartTime").val(), 'h:mm a').isValid()) {
                var time = moment($("#ddlStartTime").val(), 'h:mm a');
                startDateTime.hour(time.hour());
                startDateTime.minutes(time.minutes());
            }
            var endDateTime = moment($("#txtEndDate").val());
            if (!event.IsAllDay && moment($("#ddlEndTime").val(), 'h:mm a').isValid()) {
                time = moment($("#ddlEndTime").val(), 'h:mm a');
                endDateTime.hour(time.hour());
                endDateTime.minutes(time.minutes());
            }
            var minutesDiff = endDateTime.diff(startDateTime, 'minutes');
            if (minutesDiff < 0) {
                $("#txtEndDate").closest(".form-group").addClass("has-error");
                $("#txtEndDate").closest(".form-group").find(".error-text").html("Event must end after it starts.");
                return;
            }
        }
        if (id === "txtStartDate") {
            var selectedDate = moment($("#txtStartDate").val(), 'DD-MMM-YY');
            if (changeDays === 0) {
                $("#txtEndDate").val(selectedDate.format('DD-MMM-YY'));
            } else {
                endDateTime = selectedDate.add(changeDays, 'days');
                $("#txtEndDate").val(endDateTime.format("DD-MMM-YY"));
            }
        }
        else if (id === "txtEndDate") {
            var endDate = moment($("#txtEndDate").val(), 'DD-MMM-YY');
            var startDate = moment($("#txtStartDate").val(), 'DD-MMM-YY');
            var dayDiff = endDate.diff(startDate, 'days');
            if (dayDiff >= 0) {
                changeDays = endDate.diff(startDate, 'days');
            }
        }
    }


    function _getEventCategories() {
        $('#ddlCategory').html("");
        $.ajax({
            type: "GET",
            url: "/api/CalendarEvent/GetEventCategories?subscriberId=" + eventArgs.eventSubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (categories) {
                $('#ddlCategory').append('<option value=""></option>');

                var meetingExternalCategory = "";
                $.each(categories, function (i, ele) {
                    $('#ddlCategory').append('<option data-color="' + ele.CategoryColor + '" data-name= "' + ele.CategoryName + '" style="color: #' + ele.CategoryColor + '" value="' + ele.CategoryName + '">&nbsp; &nbsp;' + ele.CategoryName + '</option>');
                    if (ele.CategoryName === "Meeting External")
                        meetingExternalCategory = "Meeting External";
                });
                var color = $('#ddlCategory').find(":selected").css("color");
                if (eventArgs.categoryName !== "") {
                    $('#ddlCategory').val(eventArgs.categoryName).trigger("change");
                } else {
                    // Meeting External
                    if (meetingExternalCategory !== "") {
                        $('#ddlCategory').val(meetingExternalCategory).trigger("change");
                    }
                }
                $('#ddlCategory').css("color", color);
                try {
                    color = $('#ddlCategory').find(":selected").css("color");
                    $('#ddlCategory').css("color", color);
                    fullCalendarAddEvent();
                } catch (e) {
                    /*ignore*/
                }

                $('#ddlCategory').select2({
                    templateResult: formatCategoryOption,
                    templateSelection: formatCategoryOption,
                    minimumResultsForSearch: -1,
                    allowClear: true,
                    placeholder: translatePhrase("Select Category")
                });

                // on change event
                $('#ddlCategory').on("select2:select", function (e) {
                    var val = $('#ddlCategory').val();
                    //  eventArgs.eventChanged = true;
                    fullCalendarAddEvent();
                });

            }, beforeSend: function () {
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    }


    function _getTimezones() {
        $('#ddlTimezone').html("");
        $.ajax({
            type: "GET",
            url: "/api/Dropdown/GetTimeZones?userId=" + eventArgs.userId + "&subscriberId=" + eventArgs.userSubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (timezones) {
                var tZoneId = 0;
                $.each(timezones, function (i, ele) {
                    $('#ddlTimezone').append('<option value="' + ele.SelectValue + '">' + ele.SelectText + '</option>');
                    if (ele.Selected) {
                        tZoneId = ele.SelectValue;
                    }
                });
                if (tZoneId > 0) {
                    $('#ddlTimezone').val(tZoneId).trigger("change");
                }
            }, beforeSend: function () {
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
        $('#ddlCategory').unbind("change").change(function () {
            var color = $('#ddlCategory').find(":selected").css("color");
            $('#ddlCategory').css("color", color);
        });
    }


    // load calendar event
    function _loadCalendarEvent() {
        $(".attachments").html("");
        $tblInvitesBody.html("");
        $.ajax({
            type: "GET",
            url: "/api/CalendarEvent/GetCalendarEvent?calendarEventId=" + eventArgs.eventId + "&userId=" + eventArgs.userId + "&subscriberId=" + eventArgs.eventSubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 100000,
            data: '',
            success: function (eventModal) {
                if (eventModal !== null) {
                    _bindCalendarEvent(eventModal);
                    removeSpinner();
                }
            }, beforeSend: function () {
                addSpinner();
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    }


    // bind calendar event
    function _bindCalendarEvent(eventModal) {
        var event = eventModal.CalendarEvent;
        $(".section-event-edit").find(".page-title").html("Edit Event");
        $("#txtEventTitle").val(event.Subject);
        $("#txtEventDescription").summernote("code", event.Description);
        $("#txtLocation").val(event.Location);
        $("#txtStartDate").val(moment(event.StartDateTime).format("DD-MMM-YY"));
        $("#ddlStartTime").val(moment(event.StartDateTime).format("hh:mma"));

        if (event.IsAllDay) {
            $chkAllDay.prop('checked', true);
            $(".timeBox").hide();
            $(".end-date-section").hide();
            $(".datetimepicker").addClass("W100P");
            $(".datetimepicker").find(".date-picker").addClass("right-gray-border");

            if (moment(event.EndDateTime).isValid()) {
                $("#txtEndDate").val(moment(event.EndDateTime).format("DD-MMM-YY"));
            } else {
                $("#txtEndDate").val(moment(event.StartDateTime).format("DD-MMM-YY"));
            }
        } else {
            $(".timeBox").show();
            $(".end-date-section").show();
            $(".datetimepicker").removeClass("W100P");
            $(".datetimepicker").find(".date-picker").removeClass("right-gray-border");

            $("#txtEndDate").val(moment(event.EndDateTime).format("DD-MMM-YY"));
            $("#ddlEndTime").val(moment(event.EndDateTime).format("hh:mma"));
        }

        if (event.PublicPrivate !== null)
            $("#ddlPublicPrivate").val(event.PublicPrivate).trigger("change");
        else
            $("#ddlPublicPrivate").val("Public").trigger("change");


        if (event.BusyFree !== null)
            $("#ddlBusyFree").val(event.BusyFree).trigger("change");
        else
            $("#ddlBusyFree").val("Busy").trigger("change");

        if (event.ReminderMinutes !== null)
            $("#ddlReminder").val(event.ReminderMinutes).trigger("change");
        else
            $("#ddlReminder").val("10").trigger("change");

        $remindCheckboxes.removeClass("hide");

        // company
        if (event.CompanyIdGlobal > 0) {
            var company = new Option(event.CompanyName, event.CompanyIdGlobal, false, true);
            $("#ddlCompany").append(company).trigger('change');
            $divDealContainer.removeClass("hide");
        } else {
            $('#ddlCompany').val(null).trigger('change');
        }

        // deal   
        if (eventModal.Deals && eventModal.Deals.length > 0) {
            $.each(eventModal.Deals, function (i, obj) {
                var deal = new Option(obj.DealName, obj.DealId, false, true);
                $("#ddlDeal").append(deal).trigger('change');
                $divDealContainer.removeClass("hide");
            });
        }

        try {
            $('#ddlEventType').val(event.EventType).trigger("change");
        } catch (e) {
            /*ignore*/
        }

        // category
        if (event.CategoryName !== "") {
            eventArgs.categoryName = event.CategoryName;
            try {
                $('#ddlCategory').val(event.CategoryName).trigger("change");
                var color = $('#ddlCategory').find(":selected").css("color");
                $('#ddlCategory').css("color", color);
                fullCalendarAddEvent();
            } catch (e) { /*ignore*/ }
        }

        // documents 
        if (eventModal.Documents && eventModal.Documents.length > 0) {
            new CalendarEventAttachment().BindEventDocuments(eventModal.Documents);
        }

        // set date and time change auto fill 
        changeTimeDuration($("#ddlEndTime"));
        changeStartEndDate($("#txtEndDate"));

        // load invites 
        if (eventModal.Invites && eventModal.Invites.length > 0)
            new CalendarUsers().LoadCalendarEventInvites(eventModal.Invites);
    }


    // save calendar event
    function _saveCalendarEvent() {
        $(".error").removeClass("error");
        $(".form-group").removeClass("error");

        // validate
        if ($("#txtEventTitle").val() === "") {
            $("#txtEventTitle").addClass("error");
        }

        if ($("#chkAllDay").is(":checked")) {
            if (!moment($("#txtStartDate").val()).isValid()) {
                $("#txtStartDate").addClass("error");
            }
        } else {
            // check if the start date is lower than the end date
            var startDateTime = moment($("#txtStartDate").val());
            if (moment($("#ddlStartTime").val(), 'h:mm A').isValid()) {
                var time = moment($("#ddlStartTime").val(), 'h:mm A');
                startDateTime.hour(time.hour());
                startDateTime.minutes(time.minutes());
            }
            var endDateTime = moment($("#txtEndDate").val());
            if (!event.IsAllDay && moment($("#ddlEndTime").val(), 'h:mm A').isValid()) {
                time = moment($("#ddlEndTime").val(), 'h:mm A');
                endDateTime.hour(time.hour());
                endDateTime.minutes(time.minutes());
            }
            var minutesDiff = endDateTime.diff(startDateTime, 'minutes');
            if (minutesDiff < 0) {
                $("#txtEndDate").closest(".form-group").addClass("has-error");
                $("#txtEndDate").closest(".form-group").find(".error-text").html("Event must end after it starts.");
                $("#txtEndDate").addClass("error");
            }

            if (!moment($("#txtStartDate").val()).isValid() || !moment($("#ddlStartTime").val(), 'h:mma').isValid()) {
                $("#txtStartDate").addClass("error");
            }
        }

        if ($(".error").length > 0) {
            return;
        }

        if (eventArgs.eventChanged) {
            // check if there is any other attendees (internal users) other than the organizer
            var internalAttendees = 0;
            var externalAttendees = 0;
            $tblInvitesBody.find(".ddlInviteType").not(this).each(function (i, ele) {
                var attendeeType = $(this).val();
                if (attendeeType !== "Organizer") {
                    var $tr = $(this).closest("tr");
                    var dataType = $tr.attr("data-type");
                    if (dataType === "user") {
                        internalAttendees += 1;
                    }
                    if (dataType === "contact" || dataType === "external") {
                        externalAttendees += 1;
                    }
                }
            });

            var notifyExternalAttendees = false;
            var notifyInternalAttendees = false;
            if (internalAttendees > 0 || externalAttendees > 0) {
                var title = "Send Invitations";
                var text = "Do you want to send invitations to all attendees?";
                // ask whether to notify or not
                if (eventArgs.eventId > 0) {
                    title = "Event has been updated.";
                    text = "Do you want to re-send invitations to all attendees?";
                }

                if (internalAttendees === 0 && externalAttendees > 0) {
                    title = "Attendees from outside your company.";
                    text = "You have some attendees from outside the company. Would you like to send invitations to them?";
                }

                swal({
                    title: "Send Invitation Email to Attendees?",
                    type: "warning",
                    confirmButtonColor: "#ea7d7d",
                    confirmButtonText: "Send",
                    cancelButtonText: "Don\'t Send",
                    cancelButtonClass: 'left-cancel',
                    closeOnConfirm: false,
                    showCloseButton: true,
                    showCancelButton: true,
                    reverseButtons: true
                }).then(function (result) {

                    if (internalAttendees === 0 && externalAttendees > 0) {
                        notifyInternalAttendees = false;
                        notifyExternalAttendees = result && result.value === true;
                        _ajaxEventSave(notifyInternalAttendees, notifyExternalAttendees);
                    } else if (internalAttendees > 0 && externalAttendees > 0) {
                        if (result && result.value === true) {
                            notifyInternalAttendees = false;
                            notifyExternalAttendees = false;
                            _ajaxEventSave(notifyInternalAttendees, notifyExternalAttendees);
                        } else {
                            notifyInternalAttendees = true;
                            title = "Attendees from outside your company.";
                            text = "You have some attendees from outside the company. Would you like to send invitations to them?";
                            swal({
                                title: title,
                                text: text,
                                type: "info",
                                showCancelButton: true,
                                confirmButtonColor: "#3AA6F9",
                                confirmButtonText: "Do Not Send",
                                cancelButtonText: "Send",
                                closeOnConfirm: false,
                                closeOnCancel: false
                            }).then(function (rr) {
                                notifyExternalAttendees = (rr && rr.value === true) ? false : true;
                                _ajaxEventSave(notifyInternalAttendees, notifyExternalAttendees);
                            });
                        }
                    } else if (internalAttendees > 0 && externalAttendees === 0) {
                        notifyInternalAttendees = result && result.value === true;
                        notifyExternalAttendees = false;
                        _ajaxEventSave(notifyInternalAttendees, notifyExternalAttendees);
                    } else {
                        _ajaxEventSave(notifyInternalAttendees, notifyExternalAttendees);
                    }
                });
            }
            else {
                _ajaxEventSave(false, false);
            }
        }
        else {
            _ajaxEventSave(false, false);
        }
    }


    function _ajaxEventSave(notifyInternalAttendees, notifyExternalAttendees) {

        var eventModel = new Object();
        var event = new Object();

        event.ActivityId = eventArgs.eventId;
        event.SubscriberId = eventArgs.eventSubscriberId;
        event.UpdateUserId = eventArgs.userId;
        event.UpdateUserIdGlobal = eventArgs.globalUserId;
        event.OwnerUserId = eventArgs.userId;
        event.OwnerUserIdGlobal = eventArgs.globalUserId;
        event.Subject = $("#txtEventTitle").val();
        event.Description = $("#txtEventDescription").val();
        event.IsAllDay = $("#chkAllDay").is(":checked");
        event.Location = $("#txtLocation").val();
        event.CategoryName = $("#ddlCategory option:selected").attr("data-name");
        event.BusyFree = $("#ddlBusyFree").val();
        event.PublicPrivate = $("#ddlPublicPrivate").val();
        event.ReminderMinutes = $("#ddlReminder").val();

        var repeatType = $("#ddlRepeat").val();

        if (repeatType !== "") {
            event.IsRecurring = true;
            event.ReoccurrenceIncrementType = repeatType;
        } else {
            event.IsRecurring = false;
        }

        event.EventTimeZone = "";
        event.CompanyIdGlobal = $("#ddlCompany").val();
        // start date time
        event.StartDateTime = moment($("#txtStartDate").val());
        if (!event.IsAllDay && moment($("#ddlStartTime").val(), 'h:mma').isValid()) {
            var time = moment($("#ddlStartTime").val(), 'h:mm a');
            event.StartDateTime.hour(time.hour());
            event.StartDateTime.minutes(time.minutes());
        }
        event.StartDateTime = moment(event.StartDateTime).format("YYYY-MM-DD HH:mm");

        // end date time
        if (!event.IsAllDay) {
            event.EndDateTime = moment($("#txtEndDate").val());
            if (!event.IsAllDay && moment($("#ddlEndTime").val(), 'h:mma').isValid()) {
                time = moment($("#ddlEndTime").val(), 'hh:mma');
                event.EndDateTime.hour(time.hour());
                event.EndDateTime.minutes(time.minutes());
            }
            event.EndDateTime = moment(event.EndDateTime).format("YYYY-MM-DD HH:mm");
        }

        // set linked data
        if ($("#ddlDeal").val() !== '') {
            event.DealIds = $("#ddlDeal").val().join(",");
        }

        // set final object
        eventModel.CalendarEvent = event;
        eventModel.Invites = new CalendarUsers().GetSelectedInvites();
        eventModel.Documents = new CalendarEventAttachment().GetSelectedDocuments();
        eventModel.NotifyInternalAttendees = notifyInternalAttendees;
        eventModel.NotifyExternalAttendees = notifyExternalAttendees;

        // AJAX to save the deal
        $.ajax({
            type: "POST",
            url: "/api/CalendarEvent/SaveCalendarEvent",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(eventModel),
            success: function (eid) {

                // if calendar aspx do not hide the swal
                removeSpinner();

                if (parseInt(eid) > 0) {
                    // back to parent
                    $divCalendarEventWrapper.addClass("hidden");
                    $(".section-event-edit").addClass("hide");
                    $(".wrapper-outer").removeClass("hide");
                    $("html, body").animate({ scrollTop: 0 }, "slow");

                    // load events
                    try { RefreshEvents(); } catch (e) {/*ignore*/ }
                    try { RefreshActivities(); } catch (e) { /*ignore*/ }

                    eventId = eid;
                    _clearControls();
                    new RunSync();
                } else {
                    alert("Event Save Error");
                }

            }, beforeSend: function () {
                addSpinner();
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    }


    function _deleteCalendarEvent() {
        var deleteRecurring = false;

        $.ajax({
            type: "GET",
            url: "/api/Activity/IsActivityRecurring/?activityId=" +
                eventArgs.eventId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {},
            success: function (response) {

                var confirmButtonText;
                var cancelButtonText;
                var text;

                //If recurring event
                if (response) {
                    confirmButtonText = "This Event Only";
                    cancelButtonText = "All Recurring";
                    text = "Would you only like to delete this event, or all recurring instances of this event?";
                } else {
                    confirmButtonText = "Yes, Delete";
                    cancelButtonText = "Cancel";
                    text = "Are you sure you want to delete this event?";
                }

                swal({
                    title: "Delete Event!",
                    type: "warning",
                    confirmButtonColor: "#cd2b1e",
                    confirmButtonText: confirmButtonText,
                    cancelButtonText: cancelButtonText,
                    closeOnConfirm: false,
                    showCloseButton: true,
                    showCancelButton: true,
                    reverseButtons: true,
                    html: text
                }).then(function (result) {

                    //If it's not a recurring event and cancel is pressed, do nothing.
                    if (response === false && result.value === false) {

                    }
                    //Otherwise, if it's a recurring event OR the user clicked the confirm option, proceed.
                    else if (response || result.value) {

                        var deleteRecurring = false;

                        //If it's a recurring event and the user clicked the recurring option, then this means he wants to 
                        //delete all the recurring events.
                        if (response && !result.value) {
                            deleteRecurring = true;
                        }

                        $.ajax({
                            type: "GET",
                            url: "/api/CalendarEvent/DeleteCalendarEvent/?calendarEventId=" +
                                eventArgs.eventId +
                                "&deleteRecurring=" +
                                deleteRecurring +
                                "&userId=" +
                                eventArgs.userId +
                                "&subscriberId=" +
                                eventArgs.eventSubscriberId,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            data: {},
                            success: function (response) {
                                swal.close();
                                if (response) {
                                    _clearControls();
                                    // back to parent
                                    $(".section-event-edit").addClass("hide");
                                    $(".wrapper-outer").removeClass("hide");
                                    $("html, body").animate({ scrollTop: 0 }, "slow");
                                    // load events
                                    try { RefreshEvents(); } catch (e) { /*ignore*/ }
                                    try { RefreshActivities(); } catch (e) { /*ignore*/ }

                                    new RunSync();
                                }
                            },
                            beforeSend: function () {
                                swal({ text: translatePhrase("Deleting event") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: false });
                            },
                            error: function (request) { }
                        });
                    }
                });
            },
            beforeSend: function () {
            },
            error: function (request) { }
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


    function _clearControls() {
        $("#txtEventTitle").val("");
        // $("#txtEventDescription").val("");
        $("#txtLocation").val("");
        $("#txtStartDate").val("");
        $("#ddlStartTime").val("");
        $("#txtEndDate").val("");
        $("#ddlEndTime").val("");
        $('#txtEventDescription').summernote('code', '<p><br></p>');
        $tblInvitesBody.html("");
        var $attachments = $(".attachments");
        $attachments.html("");
        $("#chkAllDay").prop('checked', false);
        $('#ddlCompany').val(null).trigger('change');
        $("#ddlDeal").empty().trigger("change");
        changeMinutes = 30;
        changeDays = 0;

        $divDealContainer.addClass("hide");
        $("#ddlStartTime").show();
        $("#ddlEndTime").show();
        $(".end-date-section").show();
        $(".datetimepicker").removeClass("W100P");
        $(".datetimepicker").find(".date-picker").removeClass("W100P");
        $(".datetimepicker").find(".date-picker").removeClass("right-gray-border");
    }

};


var CalendarUsers = function () {
    var self = this;
    var settings = eventArgs;

    this.Init = function () {
        // load invites
        self.LoadInvites();
    };

    this.InitInvitesDropdown = function () {
        $("#ddlInvites").select2({
            minimumInputLength: 2,
            allowClear: true,
            placeholder: translatePhrase("Add Guests"),
            templateResult: formatInviteOption,
            tags: true,
            createTag: function (tag) {
                return {
                    id: tag.term,
                    text: tag.term,
                    // add indicator
                    isNew: true
                };
            },
            ajax: {
                url: function (obj) {
                    var keyword = obj.term ? obj.term : "";
                    return "/api/AutoComplete/?type=calendarinvite&SusbcriberId=" + eventArgs.eventSubscriberId + "&GlobalCompanyId=" + getSelectedCompanyId() + "&prefix=" + keyword; // + "&companyId=" + getSelectedCompanyId(),
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
                                dataObj: item.dataObj,
                                type: item.type
                            };
                        })
                    };
                }
            }
        });

        $('#ddlInvites').on("select2:select", function (e) {
            if (e.params.data) {
                if (e.params.data.dataObj) {
                    var obj = new Object();
                    obj.name = e.params.data.text;
                    obj.type = e.params.data.type;
                    obj.id = e.params.data.id;
                    if (e.params.data.type === "user") {
                        obj.invitetype = "user";
                        obj.attendeetype = "Required";
                    } else if (e.params.data.type === "contact") {
                        obj.invitetype = "contact";
                        obj.attendeetype = "Required";
                    }
                    obj.dataObj = e.params.data.dataObj;
                    new CalendarUsers().AddToTable(obj);
                    eventArgs.eventChanged = true;
                } else {
                    // validate email address
                    var emailAddress = e.params.data.text;
                    if (!validateEmail(emailAddress)) {
                        swal("Invalid Email Address!", "", "warning");
                    } else {
                        obj = new Object();
                        obj.type = "external";
                        obj.invitetype = "external";
                        obj.attendeetype = "Optional";
                        obj.email = emailAddress;
                        obj.name = emailAddress;
                        new CalendarUsers().AddToTable(obj);
                        eventArgs.eventChanged = true;
                    }
                }
            }
            $('#ddlInvites').val(null).trigger('change');
        });

    };

    this.AddToTable = function (item) {
        var found = $tblInvitesBody.find("tr[data-type='" + item.type + "'][data-id=" + item.id + "]").length > 0;
        if (item.type === "external") {
            found = $tblInvitesBody.find("tr[data-type='" + item.type + "'][email='" + item.name + "']").length > 0;
        }
        if (!found) {
            var $tr = $("<tr/>", { "data-type": item.type, "data-id": item.id, "data-name": item.name });

            // td profile image
            var $tdProfileImage = $("<td/>", { "class": "W50 text-center" });
            if (item.type !== "external") {
                FFGlobal.profile.pic.createAPI({
                    $wrapper: $tdProfileImage,
                    data: {
                        subscriberId: subscriberId,
                        id: item.id,
                        type: item.type,
                        name: item.text
                    }
                });
            } else {
                FFGlobal.profile.pic.createAPI({
                    $wrapper: $tdProfileImage,
                    data: {
                        subscriberId: subscriberId,
                        id: 0,
                        type: item.type,
                        name: item.name
                    }
                });
            }
            $tr.append($tdProfileImage);

            // td name
            var $tdName = $("<td/>", { "class": "vertical-middle" });
            var name = item.name || "";
            if (name.length > 30) name = name.substring(0, 27) + "...";
            var $name = $("<p />", { "class": "text-black MB0", "html": name });
            $tdName.append($name);
            $tr.append($tdName);

            // invite type
            var $spanInviteType = $("<span/>", { "class": "vertical-middle invite-type" });
            var $ddlInviteType = $("<select />", { "class": "ddlInviteType" });
            $spanInviteType.append($ddlInviteType);
            if (item.attendeetype === "Organizer") {
                $spanInviteType.addClass("hide");
            }

            // type
            var $userType = $("<span />", { "class": "user-type" });

            $userType.addClass("user-type");
            if (item.type === "contact") {
                //$userType.html("Existing Contact");
                $ddlInviteType.append($("<option value=\"Required\">Required</option>"));
                $ddlInviteType.append($("<option selected='true'  value=\"Optional\">Optional</option>"));
            } else if (item.type === "user") {
                $userType.addClass("text-navy");
                if (item.attendeetype === "Organizer") {
                    $userType.html("Organizer");
                    $userType.removeClass("text-navy").addClass("text-success");
                } else {
                    $userType.html("Internal Contact");
                }

                $ddlInviteType.append($("<option selected='true' value=\"Required\">Required</option>"));
                $ddlInviteType.append($("<option value=\"Optional\">Optional</option>"));
                $ddlInviteType.append($("<option value=\"Organizer\">Organizer</option>"));
            } else {
                $userType.addClass("text-muted");
                $userType.html("External Contact");
                $ddlInviteType.append($("<option selected='true' value=\"Required\">Required</option>"));
                $ddlInviteType.append($("<option value=\"Optional\">Optional</option>"));
            }
            $ddlInviteType.val(item.attendeetype);

            if (item.attendeetype === "Organizer") {
                //  $ddlInviteType.attr('disabled', 'disabled');
            }
            $ddlInviteType.unbind("change").change(function () {
                var val = $(this).val();
                if (val === "Organizer") {
                    // making someone an organizer - revert the other organizer to 'required'
                    $tblInvitesBody.find(".ddlInviteType").not(this).each(function (i, ele) {
                        var attendeeType = $(this).val();
                        if (attendeeType === "Organizer") {
                            $(this).closest("tr").find(".user-type").html("Internal Contact");
                            $(this).closest("tr").find(".user-type").removeClass("text-success").addClass("text-navy");
                            $(this).closest("tr").find(".invite-type").removeClass("hide");
                            $(this).val("Required").trigger("change");
                        }
                    });

                    $(this).closest("tr").find(".user-type").html("Organizer");
                    $(this).closest("tr").find(".user-type").removeClass("text-navy");
                    $(this).closest("tr").find(".invite-type").addClass("hide");

                    $(this).prop("disabled", true);
                } else {
                    $(this).prop("disabled", false);
                }
            });

            if (item.type === "external") {
                $tr.attr("email", item.email);
            }

            $tdName.append($userType);
            $tdName.append($spanInviteType);
            $ddlInviteType.select2({ containerCssClass: "without-bg", minimumResultsForSearch: -1 });

            var $tdAction = $("<td/>", { "class": "W30 text-center" });

            var $aDelete = $("<a />", { "class": "delete-link", "html": "<i class='icon-Delete'></i>", "data-action": "remove-attendee" });
            $aDelete.unbind("click").click(function () {
                var attendeeType = $(this).closest("tr").find(".ddlInviteType").val();
                if (attendeeType === "Organizer") {
                    swal("You cannot remove the Organizer from an event. Please make someone else an organizer before deleting this user.", "", "warning");
                } else {
                    //  eventArgs.eventChanged = true;
                    $tr.remove();
                    // if only one user hide the delete button
                    if ($tblInvitesBody.find("tr").length < 2) {
                        $tblInvitesBody.find("[data-action='remove-attendee']").addClass("hide");
                    } else {
                        $tblInvitesBody.find("[data-action='remove-attendee']").removeClass("hide");
                    }
                }
            });

            $tdAction.append($aDelete);
            $tr.append($tdAction);
            $tblInvitesBody.append($tr);
            // if only one user hide the delete button
            if ($tblInvitesBody.find("tr").length < 2) {
                $tblInvitesBody.find("[data-action='remove-attendee']").addClass("hide");
            } else {
                $tblInvitesBody.find("[data-action='remove-attendee']").removeClass("hide");
            }
        }
        $(".user-container").find("[data-role='remove']").click();

    };

    this.GetSelectedInvites = function () {
        var invites = [];
        $tblInvitesBody.find("tr").each(function (i, ele) {
            var $tr = $(ele);
            var obj = new Object();
            obj.AttendeeType = $tr.find(".ddlInviteType").val();
            var dataType = $tr.attr("data-type");
            obj.InviteType = dataType;
            if (dataType === "user") {
                obj.UserIdGlobal = $tr.attr("data-id");
                obj.UserName = $tr.attr("data-name");
                obj.SubscriberId = $tr.attr("data-subscriber-id");
            } else if (dataType === "contact") {
                obj.ContactId = $tr.attr("data-id");
                obj.ContactName = $tr.attr("data-name");
                obj.SubscriberId = $tr.attr("data-subscriber-id");
            } else if (dataType === "external") {
                obj.Email = $tr.attr("email");
            }
            invites.push(obj);
        });
        return invites;
    };

    this.LoadCalendarEventInvites = function () {
        $.ajax({
            type: "GET",
            url: "/api/CalendarEvent/GetCalendarEventInvites?calendarEventId=" + eventArgs.eventId + "&subscriberId=" + eventArgs.eventSubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '',
            success: function (invites) {
                $tblInvitesBody.html("");
                $.each(invites, function (i, obj) {
                    var item = new Object();
                    if (obj.UserIdGlobal > 0) {
                        item.type = "user";
                        item.name = obj.UserName;
                        item.id = obj.UserIdGlobal;
                        item.dataObj = new Object();
                        item.dataObj.FirstName = obj.UserName.split(" ").length > 0 ? obj.UserName.split(" ")[0] : "";
                        item.dataObj.LastName = obj.UserName.split(" ").length > 1 ? obj.UserName.split(" ")[1] : "";
                        item.dataObj.GlobalUserId = obj.UserIdGlobal;
                        item.dataObj.SubscriberId = obj.SubscriberId;
                    } else if (obj.ContactId > 0) {
                        item.type = "contact";
                        item.name = obj.ContactName;
                        item.id = obj.ContactId;
                        item.dataObj = new Object();
                        item.dataObj.FirstName = obj.ContactName.split(" ").length > 0 ? obj.ContactName.split(" ")[0] : "";
                        item.dataObj.LastName = obj.ContactName.split(" ").length > 1 ? obj.ContactName.split(" ")[1] : "";
                        item.dataObj.ContactId = obj.ContactId;
                        item.dataObj.SubscriberId = obj.SubscriberId;
                    } else {
                        item.type = "external";
                        item.id = 0;
                        item.name = obj.Email;
                        item.email = obj.Email;
                    }
                    item.invitetype = obj.InviteType;
                    item.attendeetype = obj.AttendeeType;
                    // item.invitetype = "Required";
                    self.AddToTable(item);
                });
            },
            beforeSend: function () {
            },
            error: function () {
            }
        });
    };

    this.LoadInvites = function () {
        if (eventArgs.eventId > 0) {
            /*ignore*/
        } else {
            // bind creating user as the organizer
            var item = new Object();
            if (settings.userId > 0) {
                item.type = "user";
                item.invitetype = "user";
                item.attendeetype = "Organizer";
                item.name = eventArgs.username;
                item.id = eventArgs.globalUserId;
                item.dataObj = new Object();
                item.dataObj.GlobalUserId = eventArgs.globalUserId;
                item.dataObj.SubscriberId = eventArgs.userSubscriberId;
                self.AddToTable(item);
            }
        }
    };
};


var CalendarEventAttachment = function () {
    var self = this;
    var baseUploadUrl = '/_handlers/temp-file-upload-handler.ashx?';
    var $bDocument = $("[data-action='event-attachments']");
    var $uploadedDocument = $(".uploaded-document");

    this.Init = function () {
        $bDocument.unbind("click").click(function () {
            var uploadFiles = [];
            var uploadFileNames = [];
            Dropzone.autoDiscover = false;

            $htmlContent = '<div id="uploadsPreview"></div><form class="attachment-form dropzone" enctype="multipart/form-data" action="#"><div id="drop-wrp" class="drag-drop-wrp"><div class="dz-message needsclick" ><div id="drop_here" class="font-14"><div>Drag and drop here</div></div><div class="font-14 lyt-color">- or -</div><button type="button" class="upload-btn">Choose from computer</button></div></div></form>';

            swal({
                title: "Attach File to Event",
                type: "warning",
                confirmButtonText: "Attach File",
                confirmButtonColor: '#bfcad7',
                closeOnConfirm: false,
                showCloseButton: true,
                showCancelButton: true,
                reverseButtons: true,
                customClass: 'swal-attachment',
                html: 'Select a file to upload.' + $htmlContent,
                onBeforeOpen: function () {
                    var formUploadzone = new Dropzone(".attachment-form", { previewsContainer: "#uploadsPreview", addRemoveLinks: true });

                    formUploadzone.on("addedfile", function (file) {
                        $('#uploadsPreview').show();
                        $('.attachment-form').hide();
                        $('.swal-attachment .swal2-confirm').addClass('active');
                    });

                    formUploadzone.on("sending", function (file) {
                        uploadFiles.push(file);
                        uploadFileNames.push(file.name);
                    });

                    formUploadzone.on("removedfile", function (file) {
                        var file_index = uploadFileNames.indexOf(file.name);
                        if (file_index > -1) {
                            uploadFileNames.splice(file_index, 1);
                        }

                        if (uploadFileNames.length === 0) {
                            $('#uploadsPreview').hide();
                            $('.attachment-form').show();
                            $('.swal-attachment .swal2-confirm').removeClass('active');
                        }
                    });
                },
                preConfirm: function () {
                    var $attachments = $('.attachments');
                    var $filesHTML = "";

                    var fileData = new window.FormData();
                    fileData.append('file', uploadFiles[0]);
                    // AJAX to upload
                    $.ajax({
                        type: "POST",
                        url: baseUploadUrl + "ref=" + guid() + "&folder=eventAttachment&delete=true",
                        data: fileData,
                        processData: false,
                        contentType: false,
                        success: function (data) {
                            removeSpinner();
                            // parse JSON result
                            var response = JSON.parse(data);
                            var doc = response[0];
                            self.ShowUploadedDcoument(doc);

                        }, error: function (err) {
                            alert(err.statusText);
                        }, beforeSend: function () {
                            // add loading message 
                            addSpinner();
                        }
                    });

                    //$.each(uploadFiles, function (index, value) {
                    //    $filesHTML += '<li class="uploaded-file">' + value + '<a onclick="deleteFile(this);" class="remove-file"></a></li>';
                    //});
                }
            });
        });
    };

    this.ShowUploadedDcoument = function (document) {
        var $attachments = $(".attachments");
        var $att = $("<li/>", { "class": "uploaded-file attachment rectangle" });
        var filename = document.FileName;
        if (filename.length > 20) {
            filename = filename.substring(0, 17) + "...";
        }
        $att.append($("<a/>", { "href": document.Uri, "class": "name-of-file-doc", "html": filename }));
        $att.append($("<a/>", { "data-action": "delete", "class": "remove-file icon-Delete" }));
        $att.attr("doc-path", document.Uri);
        $att.attr("file-name", document.FileName);
        $att.attr("blob-reference", document.BlobReference);
        $att.attr("container-reference", document.ContainerReference);
        $att.addClass("inline");
        $attachments.append($att);
        self.SetActions();
    };

    this.BindEventDocuments = function (documents) {
        var $attachments = $(".attachments");
        $attachments.html("");
        $.each(documents, function (i, doc) {
            var $att = $("<li/>", { "class": "uploaded-file attachment rectangle" });
            var filename = doc.FileName;
            if (filename.length > 20) {
                filename = filename.substring(0, 17) + "...";
            }
            $att.append($("<a/>", { "href": doc.DocumentUrl, "class": "name-of-file-doc", "html": filename }));
            $att.append($("<a/>", { "data-action": "delete", "class": "remove-file icon-Delete" }));
            $att.attr("doc-path", doc.DocumentUrl);
            $att.attr("file-name", doc.FileName);
            $att.attr("doc-id", doc.DocumentId);
            $att.addClass("inline");
            $attachments.append($att);
        });
        self.SetActions();
    };

    this.SetActions = function () {
        // bind delete action
        $("[data-action='delete']").unbind('click').click(function () {
            self.DeleteDocument($(this).closest(".attachment"));
        });
    };

    this.DeleteDocument = function ($attachment) {
        var id = parseInt($attachment.attr("doc-id"));
        if (id > 0) {
            swal({
                title: translatePhrase("Delete the document!"),
                text: translatePhrase("Are you sure you want to delete this document?"),
                type: "error",
                showCancelButton: true,
                confirmButtonColor: "#f27474",
                confirmButtonText: translatePhrase("Yes, Delete!")
            }).then(function (result) {
                if (result.value) {
                    $.ajax({
                        type: "GET",
                        url: "/api/document/Delete/?id=" + id + "&userid=" + eventArgs.userId,
                        data: {},
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data) {
                                $attachment.remove();
                            }
                        }
                    });
                }
            });
        } else {
            $attachment.remove();
        }
    };

    this.GetSelectedDocuments = function () {
        var $attachments = $(".attachments");
        var attachments = [];
        $attachments.find(".attachment").each(function (i, ele) {
            var $attachment = $(ele);
            var doc = new Object();
            var blobReference = $attachment.attr("blob-reference");
            var containerReference = $attachment.attr("container-reference");
            if (blobReference !== '' && containerReference !== '') {
                // new file upload
                doc.DocumentBlobReference = blobReference;
                doc.DocumentContainerReference = containerReference;
                doc.FileName = $attachment.attr("file-name");
                doc.Title = $attachment.attr("file-name");
                doc.UploadUrl = $attachment.attr("doc-path");
                doc.SubscriberId = eventArgs.userSubscriberId;
                doc.UploadedBy = eventArgs.userId;
                doc.CalendarEventId = eventArgs.eventId;
                attachments.push(doc);
            }
        });
        return attachments;

    };

};

function getSelectedCompanyId() {
    try {
        var id = $("#ddlCompany").val();
        if ($.isNumeric(id)) {
            return id;
        }

    } catch (e) {
        // ignore
    }
    return -100;
}




/* ---------- fetch event from db ---------- */
function fetchEvents(start, end, timezone, callback) {
    var request = new Object();
    request.SubscriberId = eventArgs.userSubscriberId;
    request.DateFrom = moment(start).format("DD-MMM-YY");
    request.DateTo = moment(end).format("DD-MMM-YY");
    request.UserId = eventArgs.userId;
    request.OwnerUserId = eventArgs.userId;

    $.ajax({
        type: "POST",
        url: "/api/calendarevent/GetCalendarEvents/",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(request),
        success: function (response) {
            //  swal.close();
            var calendarEvents = [];
            $.each(response, function (i, item) {
                calendarEvents.push(getEventObj(new Object(), item));
            });
            // set calendar events
            callback([]);
        },
        beforeSend: function () {
        },
        error: function (request) {
            alert(JSON.stringify(request));
        }
    });
}


function getEventObj(event, dbitem) {
    event.title = dbitem.Subject;
    event.description = dbitem.Description;
    event.end = moment(dbitem.EndDateTime);
    event.allDay = dbitem.IsAllDay;
    event.id = dbitem.CalendarEventId;
    // set dates
    var startDate = moment(dbitem.StartDateTime);
    if (!dbitem.IsAllDay) {
        event.end === moment(dbitem.EndDateTime);
    } else {
        event.end = startDate;
    }
    event.start = startDate;

    // event color
    var color = $('#ddlCategory').find(":selected").css("color");
    if (color !== '') {
        event.backgroundColor = color;
        event.borderColor = color;
    }
    return event;
}


function fullCalendarAddEvent() {
    try {
        var selectedDate = moment($("#txtStartDate").val());
        if (!selectedDate.isValid()) {
            return;
        }
        //var selectedDate = moment($("#txtEventDate").datepicker("getDate"));
        $('.calendar').fullCalendar('gotoDate', selectedDate);

        // create temp event
        var startDate = selectedDate;
        if (!event.IsAllDay && moment($("#ddlStartTime").val(), 'h:mm a').isValid()) {
            var time = moment($("#ddlStartTime").val(), 'h:mm a');
            startDate.hour(time.hour());
            startDate.minutes(time.minutes());
        }

        // set end date
        var endDate = null;
        if (!$("#chkAllDay").is(":checked")) {
            endDate = moment($("#txtEndDate").val());
            if (!event.IsAllDay && moment($("#ddlEndTime").val(), 'h:mm a').isValid()) {
                time = moment($("#ddlEndTime").val(), 'h:mm a');
                endDate.hour(time.hour());
                endDate.minutes(time.minutes());
            }
        }

        // create the event
        createEvent(startDate, endDate, $chkAllDay.is(":checked"));

    } catch (e) {
        /*ignore*/
    }
}


function createEvent(eventStartDate, eventEndDate, isAllDay) {
    $('.calendar').fullCalendar('removeEvents');
    var event = new Object();
    event.id = 1;
    event.title = $("#txtEventTitle").val();
    event.allDay = isAllDay;
    event.start = eventStartDate;

    if (!isAllDay) {
        event.end = eventEndDate;
    }

    var color = $('#ddlCategory').find(":selected").css("color");
    if (color !== null) {
        var hexColor = hexc(color);
        event.backgroundColor = hexColor;
        event.borderColor = hexColor;
    }

    // render event
    $('.calendar').fullCalendar('renderEvent', event);
}


function roundDate(date, duration, method) {
    return moment(Math[method]((+date) / (+duration)) * (+duration));
}


function formatCategoryOption(opt) {
    if (!opt.id) {
        return opt.text;
    }
    var color = $(opt.element).data('color');
    if (!color) {
        return opt.text;
    } else {
        var $opt = $(
            '<span><i style="color:#' + color + '" class="fa fa-square"></i> ' + opt.text + '</span>'
        );
        return $opt;
    }
}

function formatInviteOption(opt) {
    if (opt.id && !opt.isNew) {
        var $wrapper = $("<div/>", { "class": "invites-select-item row no-gutters" });
        //profile pic
        var $divProfile = $("<div/>", { "class": "col-auto" });
        FFGlobal.profile.pic.createAPI({
            $wrapper: $divProfile,
            data: {
                subscriberId: subscriberId,
                id: opt.id,
                type: opt.type,
                name: opt.text
            }
        });
        $wrapper.append($divProfile);
        //content
        var $divContent = $("<div/>", { "class": "col" });
        var name = opt.text;
        if (name.length > 30) name = name.substring(0, 27) + "...";
        var $name = $("<p />", { "class": "font-weight-bold", "html": name });
        $divContent.append($name);
        if (opt.type === 'user' || opt.type === 'contact') {
            var $cityCountry = $("<p />", { "class": "FontSize11" });
            var $email = $("<p />", { "class": "FontSize11" });
            if (opt.type === 'user') {
                // email
                $email.html(opt.dataObj.EmailAddress);
                $divContent.append($email);
                // city & country
                var arr = [];
                if (opt.dataObj.City && opt.dataObj.City !== null) {
                    arr.push(opt.dataObj.City);
                }
                if (opt.dataObj.CountryName && opt.dataObj.CountryName !== null) {
                    arr.push(opt.dataObj.CountryName);
                }
                if (arr.length > 0) {
                    var cityCountry = arr.join(", ");
                    if (cityCountry.length > 30) {
                        cityCountry = cityCountry.substring(0, 27) + "...";
                    }
                    $cityCountry.html(cityCountry);
                    $divContent.append($cityCountry);
                }
            } else if (opt.type === 'contact') {
                // email
                $email.html(opt.dataObj.Email);
                $divContent.append($email);
                // city & country
                arr = [];
                if (opt.dataObj.BusinessCity && opt.dataObj.BusinessCity !== null) {
                    arr.push(opt.dataObj.BusinessCity);
                }
                if (opt.dataObj.BusinessCountry && opt.dataObj.BusinessCountry !== null) {
                    arr.push(opt.dataObj.BusinessCountry);
                }
                if (arr.length > 0) {
                    cityCountry = arr.join(", ");
                    if (cityCountry.length > 30) {
                        cityCountry = cityCountry.substring(0, 27) + "...";
                    }
                    $cityCountry.html(cityCountry);
                    $divContent.append($cityCountry);
                }
            }
        }
        $wrapper.append($divContent);
        return $wrapper;
    }
}


// create guid
function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}


function deleteFile(thisele) {
    $(thisele).parent('li.uploaded-file').remove();
}


function hexc(colorval) {
    var parts = colorval.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    delete (parts[0]);
    for (var i = 1; i <= 3; ++i) {
        parts[i] = parseInt(parts[i]).toString(16);
        if (parts[i].length === 1) parts[i] = '0' + parts[i];
    }
    color = '#' + parts.join('');
    return color;
}


// Sticky footer
var stickyInterval = setInterval(calendarEventShow, 50);
function calendarEventShow() {
    if ($('#divCalendarEventWrapper').is(':visible')) {
        var windowHeight = $(window).height();
        var headerHeight = $('header.top-header').outerHeight();
        var contentHeight = $('#divCalendarEventWrapper .wrapper.wrapper-content').outerHeight();
        var footerHeight = $('#divCalendarEventWrapper .footer-box').outerHeight();
        var totalHeight = headerHeight + contentHeight + footerHeight + 10;

        if (windowHeight < totalHeight) {
            $('#divCalendarEventWrapper .footer-box').addClass('fixed');
        }
        else {
            $('#divCalendarEventWrapper .footer-box').removeClass('fixed');
        }
        clearInterval(stickyInterval);
    }
}
