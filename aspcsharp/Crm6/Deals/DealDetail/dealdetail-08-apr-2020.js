var subscriberId = parseInt($("#lblSubscriberId").text());
var userId = $("#lblUserId").text();
var dealId = $("#lblDealId").text();
var dealName = $("#lblDealNameTop").text();
var companyId = $("#lblCompanyId").text();
var companyName = $("#lblCompanyName").text();
var globalUserId = parseInt($("#lblUserIdGlobal").text());
var currentPage = 1;
var recordsPerPage = 30;
var $newLane = $("[data-action='new-lane']");
var $deleteLane = $("[data-action='delete-lane']");
var dealSubscriberId = parseInt($("#lblDealSubscriberId").text());
var isAdmin = $("#lblIsAdmin").text() === "1";
var dealOwnerId = $("#lblDealOwnerId").text();
var globalCompanyId = $("#lblGlobalCompanyId").text();
var salesOwnerId = parseInt($("#lblSalesOwnerId").text());
var currentLaneId = 0;
Dropzone.autoDiscover = false;

var docUserData = {
    subscriberId: subscriberId,
    userId: userId,
    globalUserId: globalUserId,
    companyId: companyId,
    globalCompanyId: globalCompanyId,
    companyName: companyName,
    dealId: dealId
}


$(function () {
    // Overview tab - Deal Detail - if no value for a field hide it
    jQuery('#tab-overview .detail-card .ibox-content .inner-wrp').each(function () {
        var getText = $(this).find('.card-date').text();
        getText = getText.replace(/\s/g, '');
        if (getText === '-' || getText === '') {
            $(this).hide();
        }
    });

    // edit deal button
    $(".edit-button").attr("href", "/deals/dealaddedit/dealaddedit.aspx?dealId=" + dealId + "&dealsubscriberid=" + dealSubscriberId + "&from=dealdetail");

    if (dealId > 0 && subscriberId !== dealSubscriberId) {
        $(".edit-company").addClass("hide");
        $(".edit-primary-contact").addClass("hide");
    }

    var isPopup = getQueryString("popup");
    if (isPopup && isPopup === "1") {
        $(".header-bar").remove();
        $(".navbar-default").remove();
        $(".footer").remove();
        $("#page-wrapper").css("margin", "0");
        $("#page-wrapper").css("width", "100%");
    }

    // new lane click
    $newLane.unbind("click").click(function () {
        location.href = "/deals/laneaddedit/laneaddedit.aspx?laneId=0&dealId=" + dealId;
    });

    $("[data-action='edit-company']").unbind('click').click(function () {
        location.href = "/Companies/CompanyAddEdit/CompanyAddEdit.aspx?companyId=" + companyId + "&subscriberId=" + dealSubscriberId + "&from=dealdetail&fromId=" + dealId;
    });

    $("[data-action='company-detail']").unbind('click').click(function () {
        var companyDetailUrl = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + companyId + "&subscriberId=" + dealSubscriberId;
        location.href = companyDetailUrl;
    });

    // delete lane
    $deleteLane.unbind("click").click(function () {
        var $outerWrap = $(this).closest(".outer-wrp");
        deleteLane($outerWrap);
    });

    // set add event action attributes
    $("[data-action='add-event']").attr("deal-id", dealId);
    $("[data-action='add-event']").attr("deal-name", dealName);
    $("[data-action='add-event']").attr("data-global-company-id", globalCompanyId);
    $("[data-action='add-event']").attr("company-name", companyName);

    new Deal().Init();
    new Deal().SetSalesStagesTimeline();
    new Notes().Init();

    $("#deal-tabs").find("li").unbind("click").click(function () {
        $('.single-dh-tpanel').hide();
        var dataType = $(this).attr("data-type");
        var isLoaded = $(this).attr("data-loaded");
        if (dataType !== "" && !isLoaded) {
            switch (dataType) {
                case "events":
                    new Events().Init();
                    break;
                case "tasks":
                    new FFGlobal.docs.detail.tabs.tasks({
                        type: "deal",
                        data: docUserData
                    }).init();
                    break;
                case "notes":
                    new Notes().Init();
                    break;
                case "contacts":
                    new Contacts().Init();
                    break;
                case "documents":
                    new Documents().Init();
                    break;
                case "salesteam":
                    new Users().Init();
                    break;
                case "activity":
                    break;
                default:
                    $('.single-dh-tpanel').show();
                    break;
            }
        }
    });

    // format numbers
    $(".format-number").each(function () {
        var $formatNumber = $(this);
        if ($.isNumeric($formatNumber.html())) {
            var formattedNumber = formatNumber($formatNumber.html());
            $formatNumber.html(formattedNumber);
        }
    });
});


var Deal = function () {
    var self = this;

    this.Init = function () {

        self.InitDealDateActions();

        // mark as won
        $("[data-action='mark-won']").unbind('click').click(function () {
            self.OpenMarkWonLostDialog(true);
        });

        // mark as lost
        $("[data-action='mark-lost']").unbind('click').click(function () {
            self.OpenMarkWonLostDialog(false);
        });
    };

    this.InitDealDateActions = function () {
        // initialize dates
        $(".deal-dates").find("[data-name='datepicker']").datepicker({
            format: "dd MM, yyyy",
            autoclose: true
        }).attr("readonly", "readonly").on("changeDate",
            function (e) {
                if (e !== null) {
                    var dateType = "";
                    var $dateEle = $(this).closest("[data-name='datepicker']");

                    if ($dateEle.hasClass("proposal-date"))
                        dateType = "proposal-date";
                    if ($dateEle.hasClass("decision-date"))
                        dateType = "decision-date";
                    if ($dateEle.hasClass("first-shipment-date"))
                        dateType = "first-shipment-date";
                    if ($dateEle.hasClass("contract-end-date"))
                        dateType = "contract-end-date";

                    // AJAX to update the date
                    $.ajax({
                        type: "GET",
                        url: "/api/deal/updatedealdate?dealId=" +
                            dealId +
                            "&datetype=" +
                            dateType +
                            "&dateValue=" +
                            moment(e.date).format("DD-MM-YY") +
                            "&userId=" +
                            userId,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        timeout: 50000,
                        data: '',
                        success: function (resposne) {
                            $dateEle.removeClass("FontSize12").addClass("FontSize14");
                            $dateEle.html(moment(e.date).format("DD-MMM-YY"));
                            if (moment() > moment(e.date)) {
                                $dateEle.removeClass("text-navy").addClass("text-danger");
                            } else {
                                $dateEle.removeClass("text-danger").addClass("text-navy");
                            }
                            new Events().RetrieveEvents();
                        },
                        beforeSend: function () {
                            $dateEle.html('<img src="/_content/_img/loading_20.gif" class="m-r-sm" /></span >');
                        },
                        error: function (request, status, error) {
                        }
                    });
                }
            });
    };

    this.SetSalesStagesTimeline = function () {
        // get sales stages time line
        $.ajax({
            type: "GET",
            url: "/api/deal/GetDealSalesStageTimeline?subscriberId=" + dealSubscriberId + "&dealId=" + dealId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (resposne) {
                // bind sales stage time line
                self.BindSalesStageTimeline(resposne);
            },
            beforeSend: function () {
            },
            error: function () {
            }
        });
    };

    this.BindSalesStageTimeline = function (salesStages) {
        var currentSalesStage = parseInt($("#lblSalesStage").text());
        $.each(salesStages,
            function (i, salesStage) {
                var $li = $(".sales-stage-timeline").find("li[data-sales-stage-id=" + salesStage.SalesStageId + "]");
                var salesStageId = parseInt($li.attr("data-sales-stage-id"));
                if (salesStageId <= currentSalesStage) {
                    var $daysCount = $li.find(".days-count").html(" - " + salesStage.DaysInStage + " day(s)");
                }
            });
    };

    this.OpenDealAddEditDialog = function () {
        // TODO: is this used???
        location.href = "/Deals/DealAddEdit/DealAddEdit.aspx?dealId=" + dealId + "&dealsubscriberId=" + dealSubscriberId;
        return;
    };

    this.OpenMarkWonLostDialog = function (won) {
        var iframeUrl = "/Deals/MarkAsWonLost/MarkAsWonLost.aspx?dealId=" + dealId + "&won=" + (won ? "1" : "0");
        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframeMarkWonLost" }).launchModal({
            title: (won ? "Mark As Won" : "Mark As Lost") + " - " + $("#lblDealName").text(),
            modalClass: "modal-md",
            btnSuccessText: won ? "Mark As Won" : "Mark As Lost",
            maxHeight: "400px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () {
                var frameWrapper = window.parent.document.getElementById("iframeMarkWonLost");
                var iframe = frameWrapper.getElementsByTagName("iframe")[0];
                iframe.contentWindow.PerformWonLostAction();
            }
        });
    };
};


var Contacts = function () {
    var self = this;
    var $dealContacts = $(".deal-contacts");
    var $li = $("li[data-type='contacts']");
    var $dialog = $('#addContactDialog');
    var $ddlContacts = $('.modal [data-id="ddlContacts"]');

    this.Init = function () {
        // load deal contacts
        self.LoadDealContacts();
    };

    // load deal contacts
    this.LoadDealContacts = function () {
        $.ajax({
            type: "GET",
            url: "/api/deal/GetDealContacts?dealId=" + dealId + "&subscriberId=" + dealSubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (contacts) {
                $dealContacts.html("");
                $li.attr("data-loaded", true);
                self.BindDealContacts(contacts);
                FFGlobal.docs.detail.content.addNewItemCard({
                    type: "contact",
                    $wrapper: $dealContacts,
                    link: {
                        modalTarget: "addContactDialog",
                        content: 'Add Contact'
                    }
                });
            },
            beforeSend: function () {
                $dealContacts.html('<div class="text-center data-loader"><img src="/_content/_img/loading_20.gif" /></div>');
            },
            error: function (request) {
            }
        });
    };

    // bind deal contacts
    this.BindDealContacts = function (contacts) {
        $dealContacts.html("");
        if (contacts.length > 0) {
            $.each(contacts, function (i, contactModel) {
                var contact = contactModel.Contact;
                FFGlobal.docs.detail.content.addContactOrUserCard({
                    type: 'contact',
                    $wrapper: $dealContacts,
                    data: {
                        subscriberId: contact.SubscriberId,
                        id: contact.ContactId,
                        name: contact.ContactName,
                        address: contact.BusinessAddress,
                        city: contact.BusinessCity,
                        country: contact.BusinessCountry,
                        email: contact.Email,
                        phone: contact.BusinessPhone,
                        mobilePhone: contact.MobilePhone,
                        profile: {
                            pic: contactModel.ProfilePicture,
                            title: contact.Title,
                            decisionRole: contact.DecisionRole,
                            type: contact.ContactType
                        }
                    },
                    modifyCallback: function (type) {
                        if (type === "delete") self.DeleteDealContact(contact.ContactId);
                    }
                });
            });
        }
    };

    $("#btnAddContactSave").unbind("click").click(function () {
        var dealContact = new Object();
        dealContact.UpdateUserId = userId;
        dealContact.SubscriberId = dealSubscriberId;
        dealContact.DealId = dealId;
        dealContact.ContactId = $ddlContacts.val();

        $.ajax({
            type: "POST",
            url: "/api/deal/adddealcontact",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(dealContact),
            success: function () {
                self.LoadDealContacts();
                $dialog.modal('toggle');
            },
            beforeSend: function () {
            },
            error: function (request, status, error) { }
        });
    });

    // delete deal contact
    this.DeleteDealContact = function (contactId) {
        swal({
            title: translatePhrase("Remove Contact?"),
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#ea7d7d",
            confirmButtonText: translatePhrase("Remove"),
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/deal/DeleteDealContact/?contactId=" +
                        contactId +
                        "&dealId=" +
                        dealId +
                        "&userId=" +
                        userId +
                        "&dealSubscriberId=" +
                        dealSubscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data) self.LoadDealContacts();
                        removeSpinner();
                    },
                    beforeSend: function () {
                        addSpinner();
                    },
                    error: function (request) {
                        removeSpinner();
                        alert(JSON.stringify(request));
                    }
                });
            }
        });
    };
};


var Events = function () {
    var self = this;
    var $noEvents = $(".empty_event");
    var $divEvents = $("#divEvents");
    var $li = $("li[data-type='events']");

    this.Init = function () {
        self.RetrieveEvents();
    };

    this.RetrieveEvents = function () {
        $.ajax({
            type: "GET",
            url: "/api/calendarevent/GetDealEvents?dealId=" + dealId + "&subscriberId=" + dealSubscriberId + "&userId=" + userId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '',
            success: function (events) {
                $divEvents.html("");
                $li.attr("data-loaded", true);
                if (events.length > 0) {
                    // bind events
                    $noEvents.addClass('hide');
                    self.BindEvents(events);
                    new CalendarEvent().Init();
                }
                else {
                    $noEvents.removeClass('hide');
                }
            },
            beforeSend: function () {
                $divEvents.html('<div class="text-center data-loader"><img src="/_content/_img/loading_20.gif"/></div>');
            },
            error: function (request) {
            }
        });
    };

    this.BindEvents = function (events) {
        $divEvents.html("");
        $.each(events, function (i, event) {
            FFGlobal.docs.detail.content.addEventListItem({
                $wrapper: $divEvents,
                event: event,
                deleteCallback: function (props) {
                    self.DeleteEvent(props.activityId, props.$eventItem);
                }
            });
        });
    };

    this.DeleteEvent = function (activityId, $eventItem) {
        swal({
            title: "Delete Event!",
            text: "Are you sure you want to delete this event?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#ea7d7d",
            confirmButtonText: "Yes, Delete!"
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/CalendarEvent/DeleteCalendarEvent/?calendarEventId=" +
                        activityId +
                        "&userId=" +
                        userId +
                        "&subscriberId=" +
                        dealSubscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        if (response) {
                            $eventItem.fadeOut("slow", function () {
                                $eventItem.remove();
                                if ($divEvents.find(".event-wrap").length === 0) {
                                    $noEvents.removeClass('hide');
                                } else {
                                    $noEvents.addClass('hide');
                                }
                            });
                        }

                        new RunSync();

                        removeSpinner();

                    },
                    beforeSend: function () {
                        addSpinner();
                    },
                    error: function () {
                        removeSpinner();
                        swal.close();
                    }
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

var Notes = function () {
    var self = this;
    var $divNotes = $("#divNotes");
    var $noNotes = $(".no-notes");
    var $li = $("li[data-type='notes']");
    var $noteAddActions = $("#note-add-actions");
    var $noteEditActions = $("#note-edit-actions");
    var $noteAddWrap = $(".add-note-wrp");

    this.Init = function () {

        // add note function
        $(".btnAddNote").unbind('click').click(function () {
            self.SaveNote();
        });

        // edit save note function
        $(".btnSaveNote").unbind('click').click(function () {
            self.SaveNote();
        });

        // cancel save note
        $(".btnNoteCancel").unbind('click').click(function () {
            $noteAddWrap.attr("note-id", 0);
            $noteEditActions.addClass("hide");
            $noteAddActions.removeClass("hide");
            $("#txtNote").val("");
            $("#txtNoteNotesSection").val("");
        });

        $('#txtNote').on('keydown', function (e) {
            if (e.which === 13) {
                $noteAddWrap.attr("note-id", 0);
                $noteEditActions.addClass("hide");
                $noteAddActions.removeClass("hide");
                // validate and save
                if ($("#txtNote").val().trim() !== '')
                    self.SaveNote();
            }
        });

        // retrieve notes
        self.RetrieveNotes();
    };

    this.SaveNote = function () {
        if ($("#txtNote").val().trim() === '' && $("#txtNoteNotesSection").val().trim() === '') {
            if ($("#txtNoteNotesSection").val().trim() === '')
                swal({
                    title: "Enter Note!",
                    type: "warning",
                    showCancelButton: false
                });
        } else {
            var note = new Object();
            note.ActivityId = parseInt($noteAddWrap.attr("note-id"));
            note.NoteContent = $("#txtNote").val() !== '' ? $("#txtNote").val() : $("#txtNoteNotesSection").val();
            note.SubscriberId = dealSubscriberId;
            note.UserId = userId;
            note.UserIdGlobal = globalUserId;
            note.CompanyIdGlobal = globalCompanyId;
            note.UpdateUserId = userId;
            note.DealIds = dealId;
            note.UpdateUserIdGlobal = globalUserId;
            note.OwnerUserIdGlobal = globalUserId;

            // save note
            $.ajax({
                type: "POST",
                url: "/api/note/savenote",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(note),
                success: function () {
                    removeSpinner();
                    // set edit note texts to empty
                    $("#txtNote").val("");
                    currentPage = 1;
                    // show add controls
                    $noteAddWrap.attr("note-id", 0);
                    $noteEditActions.addClass("hide");
                    $noteAddActions.removeClass("hide");
                    // retrieve notes
                    new Notes().RetrieveNotes();
                },
                beforeSend: function () {
                    addSpinner();
                },
                error: function (request) {
                    removeSpinner();
                    alert(JSON.stringify(request));
                }
            });
        }
    };

    this.RetrieveNotes = function () {
        var request = new Object();
        request.UserId = userId;
        request.UserIdGlobal = globalUserId;
        request.SubscriberId = dealSubscriberId;
        request.GlobalCompanyId = globalCompanyId;
        request.DealId = dealId;
        request.RecordsPerPage = 20;
        request.CurrentPage = 1;
        request.SortBy = "createddate desc";

        // load notes
        $.ajax({
            type: "POST",
            url: "/api/note/GetNotes",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (reponse) {
                $li.attr("data-loaded", true);

                if (reponse.Notes.length > 0) {
                    $noNotes.addClass('hide');
                    // bind notes
                    self.BindNotes(reponse.Notes);
                } else {
                    $divNotes.html("");
                    $noNotes.removeClass('hide');
                }
            },
            beforeSend: function () {
                $divNotes.html('<div class="text-center data-loader"><img src="/_content/_img/loading_20.gif" /></div>');
            },
            error: function (request) {
            }
        });
    };

    this.BindNotes = function (notes) {
        $divNotes.html("");
        // iterate through and bind notes
        $.each(notes,
            function (i, note) {
                var $noteItem = self.GetNoteItemHtml(note);
                $divNotes.append($noteItem);
            });
    };

    this.GetNoteItemHtml = function (note) {
        var $outerWrap = $("<div />", { "class": "note-wrap" });

        // edit link
        var $aEdit = $("<a/>", { "href": "javascript:void(0)", "class": "edit-link edit-note", "data-action": "edit-note" });
        $aEdit.append($("<i/>", { "class": "icon-edit edit-icon" }));
        $outerWrap.append($aEdit);

        $aEdit.unbind("click").click(function () {
            // show edit controls
            $noteAddWrap.attr("note-id", note.ActivityId);
            $noteEditActions.removeClass("hide");
            $noteAddActions.addClass("hide");
            // set note content
            $("#txtNoteNotesSection").val(note.NoteContent);
        });

        // delete link
        var $aDelete = $("<a/>", { "href": "javascript:void(0)", "class": "delete-link", "data-action": "delete-note" });
        $aDelete.append($("<i/>", { "class": "icon-Delete" }));
        $aDelete.unbind("click").click(function () {
            self.DeleteNote(note.ActivityId, $outerWrap);
        });
        $outerWrap.append($aDelete);

        // created date
        var $createdDate = $("<div />", { "class": "note-date" });
        $createdDate.append($("<span/>", { "html": moment(note.CreatedDate).format("ddd, DD MMMM, YYYY") }));
        $outerWrap.append($createdDate);

        // note content
        var $noteContent = $("<div />", { "class": "note-content" });
        $noteContent.append($("<span/>", { "html": note.NoteContent.replace(/\n/g, "<br />") }));
        $outerWrap.append($noteContent);

        // added by
        var $addedBy = $("<div />", { "class": "note-auth" });
        $addedBy.append($("<span/>", { "html": "Added by " + note.CreatedUserName }));
        $outerWrap.append($addedBy);

        return $outerWrap;
    };

    this.DeleteNote = function (noteId, $noteItemWrap) {
        swal({
            title: "Delete Note!",
            text: "Are you sure you want to delete this note?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#ea7d7d",
            confirmButtonText: "Yes, Delete!",
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/note/DeleteNote/?noteId=" + noteId + "&globalUserId=" + globalUserId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        if (response) {
                            $noteItemWrap.fadeOut();
                            new Notes().RetrieveNotes();
                        }
                    },
                    beforeSend: function () { },
                    error: function (request) { }
                });
            }
        });
    };
};


var Documents = function () {
    var self = this;
    var $noDocuments = $(".no-deals-documents");
    var $tabDocuments = $("#tab-documents");
    var $divDocuments = $("#divDocuments");
    var baseUploadUrl = '/_handlers/temp-file-upload-handler.ashx?';

    this.Init = function () {
        // add document
        $("#btnUploadDocument").unbind('click').click(function () {
            self.UploadDocument();
        });

        // initialize dropzone
        $("#drop-wrp").dropzone({
            url: baseUploadUrl + "ref=" + $("#lblGuid").text() + "&folder=dealDocument&delete=true",
            addRemoveLinks: true,
            maxFiles: 1,
            success: function (file, response) {
                var result = jQuery.parseJSON(decodeURI(response));
                var document = result[0];
                $("#lblDocUploadDetails").text(document.FileName + "|" + document.BlobReference + "|" + document.ContainerReference);
            
				var ext = document.FileName.split('.').pop();
				if (ext == "pdf") {
					$('#drop_here').find(".dz-image").prepend('<i class="fa fa-file-pdf-o"></i>');
				} else if (ext == "doc" || ext == 'docx' || ext == 'odt') {
					$('#drop_here').find(".dz-image").prepend('<i class="fa fa-file-word-o"></i>');
				} else if (ext == "xls" || ext == 'xlsx' || ext == 'csv' || ext == 'ods') {
					$('#drop_here').find(".dz-image").prepend('<i class="fa fa-file-excel-o"></i>');
				} else if (ext == "ppt" || ext == "pptx") {
					$('#drop_here').find(".dz-image").prepend('<i class="fa fa-file-powerpoint-o"></i>');
				} else if (ext != "png" && ext != "bmp" && ext != "jpg" && ext != "jpeg" && ext != "gif") {
					$('#drop_here').find(".dz-image").prepend('<i class="fa fa-file"></i>');
				}
			},
            error: function (file, response) {

            }, maxfilesexceeded: function (file) {
                this.removeAllFiles();
                this.addFile(file);
            }
        });

        // retrieve documents
        self.RetrieveDocuments();
    };

    this.RetrieveDocuments = function () {
        // set document request object
        var request = new Object();
        request.RefId = dealId;
        request.DocTypeId = 4;
        request.SubscriberId = dealSubscriberId;
        $divDocuments.html("");

        // load deal documents
        $.ajax({
            type: "POST",
            url: "/api/document/GetDocumentByType",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (documents) {
                $tabDocuments.attr("data-loaded", true);
                // bind documents
                if (documents.length > 0) {
                    $noDocuments.addClass('hide');
                    self.BindDocuments(documents);
                } else {
                    $divDocuments.html("");
                    $noDocuments.removeClass('hide');
                }
            },
            beforeSend: function () {
                $divDocuments.html('<div class="text-center data-loader"><img src="/_content/_img/loading_20.gif" /></div>');
            },
            error: function (request) {
            }
        });
    };

    this.BindDocuments = function (documents) {
        $divDocuments.html("");
        // iterate through and bind document
        $.each(documents,
            function (i, document) {
                var $documentItem = self.GetDocumentItemHtml(document);
                $divDocuments.append($documentItem);
            });
    };

    this.GetDocumentItemHtml = function (document) {
        var $outerWrap = $("<div />", { "class": "doc-wrap" });

        $outerWrap.click(function () {
            addSpinner();
            setTimeout(removeSpinner, 2000);
        });

        // delete link
        var $aDelete = $("<a/>", { "href": "javascript:void(0)", "class": "pull-right delete-link", "data-action": "delete-document" });
        $aDelete.append($("<i/>", { "class": "icon-Delete" }));
        $aDelete.unbind("click").click(function () {
            self.DeleteDocument(document.DocumentId, $outerWrap);
        });
        $outerWrap.append($aDelete);

        // uploaded date
        var $noteinfo = $("<div />", { "class": "doc-date" });
        var $spanDate = $("<span />", { "html": moment(document.CreatedDate).format("ddd, DD MMMM, YYYY - HH:mm") });
        $noteinfo.append($spanDate);
        $outerWrap.append($noteinfo);

        // title
        var $docTitle = $("<a />", { "class": "doc-title", "href": document.DocumentUrl, "html": document.Title });
        $outerWrap.append($docTitle);

        // description
        var $docDescription = $("<div />", { "class": "doc-content" });
        var $spanDescription = $("<span />", { "html": document.Description });
        $docDescription.append($spanDescription);
        $outerWrap.append($docDescription);

        // uploaded by
        var $addedBy = $("<div />", { "class": "doc-auth" });
        var $spanAdded = $("<span />", { "html": "Added by " + document.UploadedByName });
        $addedBy.append($spanAdded);
        $outerWrap.append($addedBy);

        // file name
        var $aFileName = $("<a />", { "class": "doc-file hover-link", "href": document.DocumentUrl });
        var imgIcon = $("<i/>", { "class": "icon-PDF----Blue file-icon" });
        $aFileName.append(imgIcon);
        $aFileName.append($("<span />", { "html": document.FileName, "class": "note-file" }));
        $outerWrap.append($aFileName);

        return $outerWrap;
    };

    this.DeleteDocument = function (id, $outerWrap) {
        swal({
            title: "Delete Document!",
            text: "Are you sure you want to delete this document?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#ea7d7d",
            confirmButtonText: "Yes, Delete!",
            closeOnConfirm: false
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/document/Delete/?id=" + id + "&userid=" + userId + "&subscriberId=" + dealSubscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        // TODO: use new spinner
                        swal.close();
                        if (data) {
                            $outerWrap.fadeOut();
                            // reload activities/documents
                            new Documents().RetrieveDocuments();
                        }
                    },
                    error: function (request) {
                    }, beforeSend: function () {
                        // TODO: use new spinner
                        swal({ text: translatePhrase("Deleting Document") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: false });
                    }
                });
            }
        });
    };

    this.UploadDocument = function () {
        if ($("#txtDocumentTitle").val() === "") {
            swal({ title: "Enter the document title!", type: "warning", showCancelButton: false });
            return;
        }
        if ($("#lblDocUploadDetails").text() === "") {
            swal({ title: "Select a document!", type: "warning", showCancelButton: false });
            return;
        }

        var doc = new Object();
        doc.SubscriberId = subscriberId;
        doc.UploadedBy = userId;
        doc.Title = $("#txtDocumentTitle").val();
        doc.Description = $("#txtDocumentDescription").val();

        // check if this is a new file or saving the same file
        var blobReference = $("#lblDocUploadDetails").text().split("|")[1];
        var containerReference = $("#lblDocUploadDetails").text().split("|")[2];
        if (blobReference !== '' && containerReference !== '') {
            // new file upload
            doc.DocumentBlobReference = blobReference;
            doc.DocumentContainerReference = containerReference;
            doc.FileName = $("#lblDocUploadDetails").text().split("|")[0];
        } else {
            // no new file uploaded
        }

        doc.DocumentTypeId = 4;
        doc.DealId = dealId;

        var docList = [];
        docList.push(doc);

        // save document
        $.ajax({
            type: "POST",
            url: "/api/document/SaveDocuments",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(docList),
            success: function () {
                // TODO: use new spinner
                swal.close();
                // clear controls
                $("#txtDocumentTitle").val("");
                $("#txtDocumentDescription").val("");
                $("#txtDocumentTitle").val("");
                Dropzone.forElement("#drop-wrp").removeAllFiles(true);
                // retrieve documents
                new Documents().RetrieveDocuments();
            },
            beforeSend: function () {
                // TODO: use new spinner
                swal({ text: translatePhrase("Uploading Document") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: false });
            },
            error: function (request) {
            }
        });
    };
};


var Users = function () {
    var self = this;
    var $dealUsers = $(".deal-users");
    var $li = $("li[data-type='salesteam']");
    var $addSalesTeamDialog = $("#addSalesTeamMemberDialog");
    var $ddlSalesTeam = $('.modal [data-id="ddlSalesTeamMember"]');
    var $ddlSalesTeamRole = $('.modal [data-id="ddlSalesTeamRole"]');

    this.Init = function () {
        $("#addSalesTeamButtonSave").unbind("click").click(function () {
            self.AddEditSalesTeamMember();
        });
        self.LoadDealUsers();
    };

    this.LoadDealUsers = function () {
        $.ajax({
            type: "GET",
            url: "/api/deal/GetDealUsers?dealId=" + dealId + "&subscriberId=" + dealSubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '',
            success: function (users) {
                $li.attr("data-loaded", true);
                self.BindUsers(users);
                FFGlobal.docs.detail.content.addNewItemCard({
                    type: "user",
                    $wrapper: $dealUsers,
                    link: {
                        modalTarget: "addSalesTeamMemberDialog",
                        content: "Add Sales Team Member"
                    }
                });
            },
            beforeSend: function () {
                $dealUsers.html('<div class="text-center data-loader"><img src="/_content/_img/loading_20.gif" /></div>');
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.BindUsers = function (users) {
        $dealUsers.html("");
        if (users.length > 0) {
            $.each(users, function (i, userModel) {
                var user = userModel.User;
                FFGlobal.docs.detail.content.addContactOrUserCard({
                    type: 'user',
                    $wrapper: $dealUsers,
                    links: {
                        edit: {
                            modalTarget: "addSalesTeamMemberDialog"
                        }
                    },
                    data: {
                        subscriberId: user.SubscriberId,
                        id: user.UserId,
                        userIdGlobal: user.UserIdGlobal,
                        name: user.FullName,
                        address: user.Address,
                        city: user.City,
                        country: user.Country,
                        email: user.EmailAddress,
                        phone: user.Phone,
                        mobilePhone: user.MobilePhone,
                        isOwner: salesOwnerId === user.UserId,
                        salesTeamRole: (userModel.LinkUseToDeal || {}).SalesTeamRole,
                        profile: {
                            pic: userModel.ProfilePicture,
                            title: user.Title,
                            location: user.LocationName,
                        }
                    },
                    modifyCallback: function (type) {
                        if (type === "delete") self.DeleteDealUser(user.UserId, user.SubscriberId);
                    }
                });
            });
        }
    };

    this.DeleteDealUser = function (deleteUserId, deleteUserSubscriberId) {
        swal({
            title: translatePhrase("Remove Member?"),
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#ea7d7d",
            confirmButtonText: translatePhrase("Remove"),
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/deal/DeleteDealUser/?deleteUserId=" +
                        deleteUserId +
                        "&dealId=" +
                        dealId +
                        "&userId=" +
                        userId +
                        "&dealSubscriberId=" +
                        dealSubscriberId +
                        "&userSubscriberId=" +
                        deleteUserSubscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data) self.LoadDealUsers();
                        removeSpinner();
                    },
                    beforeSend: function () {
                        addSpinner();
                    },
                    error: function (request) {
                        removeSpinner();
                        alert(JSON.stringify(request));
                    }
                });
            }
        });
    };

    this.AddEditSalesTeamMember = function () {
        $ddlSalesTeam.closest(".form-group").find(".error-text").html("");
        $ddlSalesTeamRole.closest(".form-group").find(".error-text").html("");
        $ddlSalesTeam.closest(".form-group").find(".select2-selection--single").removeClass("error-border");
        $ddlSalesTeamRole.closest(".form-group").find(".select2-selection--single").removeClass("error-border");
        var hasErrors = false;
        if ($ddlSalesTeam.val() === null || $ddlSalesTeam.val() === "0" || $ddlSalesTeam.val() === "") {
            $ddlSalesTeam.closest(".form-group").find(".error-text").html("Select Sales Team User");
            $ddlSalesTeam.closest(".form-group").find(".select2-selection--single").addClass("error-border");
            hasErrors = true;
        }
        if ($ddlSalesTeamRole.val() === null || $ddlSalesTeamRole.val() === "0" || $ddlSalesTeamRole.val() === "") {
            $ddlSalesTeamRole.closest(".form-group").find(".error-text").html("Select User Role");
            $ddlSalesTeamRole.closest(".form-group").find(".select2-selection--single").addClass("error-border");
            hasErrors = true;
        }
        if (!hasErrors) {
            var request = new Object();
            request.DealId = dealId;
            request.DealSubscriberId = dealSubscriberId;
            request.UpdatedBy = userId;
            request.GlobalUserId = $ddlSalesTeam.val();
            request.SalesTeamRole = $ddlSalesTeamRole.val();
            $.ajax({
                type: "POST",
                url: "/api/deal/AddDealSalesTeam",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(request),
                success: function () {
                    removeSpinner();
                    self.LoadDealUsers();
                    $addSalesTeamDialog.modal('toggle');
                },
                beforeSend: function () {
                    addSpinner();
                },
                error: function () {
                    removeSpinner();
                    alert(JSON.stringify(request));
                }
            });
        }
    };
};


function deleteLane($outerWrap) {
    swal({
        title: "Delete Lane!",
        text: "Are you sure you want to delete this lane?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#ea7d7d",
        confirmButtonText: "Yes, Delete!",
        closeOnConfirm: false
    }).then(function (result) {
        if (result.value) {
            $.ajax({
                type: "GET",
                url: "/api/lane/DeleteLane/?laneId=" + $outerWrap.attr("data-id") + "&userid=" + userId + "&laneSubscriberId=" + $outerWrap.attr("data-subscriber-id"),
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    swal.close();
                    if (data) {
                        $outerWrap.fadeOut();
                    }
                },
                error: function (request) {
                }, beforeSend: function () {
                    // TODO: use new spinner
                    swal({ text: translatePhrase("Deleting Lane") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: false });
                }
            });
        }
    });
}

/*
// gets profile picture
function loadUserProfilePicture() {
    $(".user-profile-pic").each(function () {
        var $img = $(this);
        var userId = $img.attr("data-id");
        $.getJSON("/api/user/getuserprofilepic/?userId=" + userId + "&subscriberId=" + subscriberId,
            function (response) {
                if (response !== '') {
                    $img.attr("src", response + "?w=80&h=80&mode=crop");
                } else {
                    $img.attr("src", "/_content/_img/no-pic.png?w=80&h=80&mode=crop");
                }
            });
    });
}
*/

function MarkWonLostSuccess(isWon) {
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // do the following to get rid of the padding
    $('body').removeAttr('style');
    if (isWon === "1") {
        swal({
            title: "Deal marked as won!",
            type: "success",
            showCancelButton: false
        });
    }
    else {
        swal({
            title: "Deal marked as lost!",
            type: "error",
            showCancelButton: false
        });
    }
}

function RefreshLanes() {
    new Lanes().RetrieveLanes();
}

function RefreshEvents() {
    currentPage = 1;
    new Events().RetrieveEvents();
    new Deals().Init();
}

// onClick new options list of new select
$(document).ready(function () {
    //Panel Dropdown Menu
    $('.panel-dropdown .ae-select-content').text($('.panel-dropdown .dropdown-nav > li.selected').text());
    var newOptions = $('.panel-dropdown .dropdown-nav > li');
    newOptions.click(function () {
        var tabType = $(this).find('a').attr('data-set');

        if (tabType === 'new_deal') {
            $('.single-dh-tpanel').show();
        } else {
            $('.single-dh-tpanel').hide();
        }

        var dataType = $(this).attr("data-type");
        var isLoaded = $(this).attr("data-loaded");
        if (dataType !== "" && !isLoaded) {
            switch (dataType) {
                case "events":
                    new Events().Init();
                    break;
                case "tasks":
                    new FFGlobal.docs.detail.tabs.tasks({
                        type: "deal",
                        data: docUserData
                    }).init();
                    break;
                case "notes":
                    new Notes().Init();
                    break;
                case "documents":
                    new Documents().Init();
                    break;
                case "contacts":
                    new Contacts().Init();
                    break;
                case "salesteam":
                    new Users().Init();
                    break;
                case "activity":
                    break;
                default:
                    $('.single-dh-tpanel').show();
                    break;
            }
        }

        $('.panel-dropdown .ae-select-content').text($(this).text());
        $('.panel-dropdown .dropdown-nav > li').removeClass('selected');
        $(this).addClass('selected');
    });
    var aeDropdown = $('.panel-dropdown .ae-dropdown');
    aeDropdown.click(function () {
        $('.panel-dropdown .dropdown-nav').toggleClass('ae-hide');
        $('.panel-dropdown .ae-select').toggleClass('drop-open');
    });

    //move element query (moves the right column content to the left column for widths up to 1199px)
    new FFGlobal.utils.mediaQuery.moveElm({
        $elm: $("#right-column-wrapper .right-col"),
        queries: {
            "right-col": {
                $parent: $("#right-column-wrapper"),
                match: "all"
            },
            "left-col": {
                $parent: $("#left-column-wrapper"),
                match: "(max-width: 1199px)"
            }
        }
    });
});
