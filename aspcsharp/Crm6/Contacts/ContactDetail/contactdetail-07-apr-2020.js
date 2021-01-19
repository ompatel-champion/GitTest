var subscriberId = parseInt($("#lblSubscriberId").text());
var contactSubscriberId = parseInt($("#lblContactSubscriberId").text());
var contactId = parseInt($("#lblContactId").text());
var contactName = $("#lblContactName").text();
var userId = parseInt($("#lblUserId").text());
var companyId = parseInt($("#lblCompanyId").text());
var companySubscriberId = parseInt($("#lblCompanySubscriberId").text());
var globalCompanyId = parseInt($("#lblGlobalCompanyId").text());
var companyName = $("#lblCompanyName").text();
var globalUserId = parseInt($("#lblUserIdGlobal").text());
var $revenue = $(".revenue");
var currentPage = 1;
var recordsPerPage = 30;
var cardsCurrentPage = 1;
var tableSortOrder = "createddate desc";
var $dealsListHolder = $("#list-view");
var $dealsCardsHolder = $("#grid-view");
var $activeDealCards = $("#active-deal");
var $inactiveDealCards = $("#inactive-deal");
var $activeDealList = $("#deal-datatable");
var $inactiveDealList = $("#deal-inc-datatable");
var activeInactiveStatus = "active";
var dataView = "card";
var activeInactiveChanged = false;
Dropzone.autoDiscover = false;
var salesStages = $("#lblSalesStages").text().split(",");
var contactOwnerId = parseInt($("#lblContactOwnerId").text());
// notes
var $noteAddActions = $("#note-add-actions");
var $noteEditActions = $("#note-edit-actions");
var $noteAddWrap = $(".add-note");

var docUserData = {
    subscriberId: subscriberId,
    contactSubscriberId: contactSubscriberId,
    userId: userId,
    globalUserId: globalUserId,
    companyId: companyId,
    companySubscriberId: companySubscriberId,
    globalCompanyId: globalCompanyId,
    companyName: companyName,
    contactId: contactId,
    contactName: contactName
}

$(function () {
    // edit contact
    $("[data-action='edit-contact']").unbind("click").click(function () {
        location.href = "/Contacts/ContactAddEdit/ContactAddEdit.aspx?contactId=" + contactId +
            "&subscriberid=" + contactSubscriberId +
            "&refId=" + contactId + "&from=edit-contact";
    });

    // edit company
    $("[data-action='edit-company']").unbind("click").click(function () {
        location.href = "/Companies/CompanyAddEdit/CompanyAddEdit.aspx?companyId=" + companyId +
            "&subscriberId=" + companySubscriberId +
            "&refId=" + contactId + "&from=edit-contact";
    });

    $("[data-action='company-detail']").unbind('click').click(function () {
        var companyDetailUrl = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + companyId + "&subscriberId=" + companySubscriberId;
        location.href = companyDetailUrl;
    });

    // add note on key down
    $('#txtNote').on('keydown', function (e) {
        if (e.which === 13) {
            $noteAddWrap.attr("note-id", 0);
            $noteEditActions.addClass("hide");
            $noteAddActions.removeClass("hide");
            // validate and save
            if ($("#txtNote").val().trim() !== '')
                new Notes().SaveNote();
        }
    });

    new Contact().Init();
    new Deals().Init();
    new Notes().Init();
});

$(document).ready(function () {
    // Mobile - Panel Dropdown Menu
    $('.panel-dropdown .ae-select-content').text($('.panel-dropdown .dropdown-nav > li.selected').text());
    var newOptions = $('.panel-dropdown .dropdown-nav > li');
    newOptions.click(function () {
        var dataType = $(this).attr("data-type");
        var isLoaded = $(this).attr("data-loaded");
        if (dataType !== "" && !isLoaded) {
            switch (dataType) {
                case "deals":
                    new DealsTab().Init();
                    break;
                case "events":
                    new Events().Init();
                    break;
                case "tasks":
                    new FFGlobal.docs.detail.tabs.tasks({
                        type: "contact",
                        data: docUserData
                    }).init();
                    break;
                case "notes":
                    new Notes().Init();
                    break;
                case "documents":
                    new Documents().Init();
                    break;
                case "salesteam":
                    new Users().Init();
                    break;
                default:
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
});

$(function () {
    $("#contact-tabs").find("li").unbind("click").click(function () {
        var dataType = $(this).attr("data-type");
        var isLoaded = $(this).attr("data-loaded");
        if (dataType !== "" && !isLoaded) {
            switch (dataType) {
                case "events":
                    new Events().Init();
                    break;
                case "tasks":
                    new FFGlobal.docs.detail.tabs.tasks({
                        type: "contact",
                        data: docUserData
                    }).init();
                    break;
                case "notes":
                    new Notes().Init();
                    break;
                case "deals":
                    new DealsTab().Init();
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
                default:
            }
        }
    });
});


var Contact = function () {
    var self = this;
    this.Init = function () {
        self.RetrieveContact();
    };

    this.RetrieveContact = function () {
        $.ajax({
            type: "GET",
            url: "/api/Contact/GetContact?contactId=" + contactId + "&subscriberId=" + contactSubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (contact) {
                var $addEventButton = $("[data-action='add-event']");
                $addEventButton.attr("data-global-company-id", globalCompanyId);
                $addEventButton.attr("company-name", companyName);
                $addEventButton.attr("data-contact-id", contact.Contact.ContactId);
                $addEventButton.attr("contact-name", contact.Contact.FirstName + ' ' + contact.Contact.LastName);
            }, beforeSend: function () {
            }, error: function (request) {
            }
        });
    };
};


var Deals = function () {
    var self = this;
    var $noDeals = $(".overview-nodeals");
    var $dealsLoading = $(".deals-loading");
    var $deals = $(".deals");

    this.Init = function () {
        // get contact deals
        self.GetContactDeals();

        // new deal
        $("[data-action='new-deal']").unbind('click').click(function () {
            self.GoToDealAddEdit(0, companySubscriberId);
        });
    };

    this.GetContactDeals = function () {
        isAjaxLoading = true;
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.UserId = userId;
        filters.SortBy = "DealName asc";
        filters.ContactId = contactId;
         $.ajax({
            type: "POST",
            url: "/api/deal/getdeals",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(filters),
            success: function (response) {
                $deals.html("");
                isAjaxLoading = false;
                $dealsLoading.addClass('hide');
                if (response.Deals.length > 0) {
                    $noDeals.addClass('hide');
                    self.BindDeals(response.Deals);
                } else {
                    $noDeals.removeClass('hide');
                }
            },
            beforeSend: function () {
                $deals.html('<div class= "ibox-content text-center"><div class="text-center data-loader"><img src="/_content/_img/loading_20.gif" /></span ></div></div>');
            },
            error: function (request) {
                isAjaxLoading = false;
            }
        });
    };

    this.BindDeals = function (deals) {
        $deals.html("");
        $.each(deals, function (i, deal) {
            FFGlobal.docs.detail.content.addDealOverviewItem({
                $wrapper: $deals,
                deal: deal
            });
        });

        $deals.find(".outer-wrp").each(function () {
            var $this = $(this);
            var dealId = $(this).attr("data-id");
            $.ajax({
                type: "GET",
                url: "/api/deal/GetDealRevenueFromUserCurrency?dealId=" + dealId + "&userId=" + userId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: '',
                success: function (resposne) {
                    $this.find(".data-deal-revenue").html(resposne.CurrencySymbol + formatNumber(parseInt(resposne.Revenue)));
                    $this.find(".data-deal-profit").html(resposne.CurrencySymbol + formatNumber(parseInt(resposne.Profit)));
                },
                beforeSend: function () {
                },
                error: function (request, status, error) {
                    alert(JSON.stringify(request));
                }
            });
        });
    };

    this.GoToDealAddEdit = function (dealId, dealsubscriberId) {
        location.href = "/Deals/dealaddedit/dealaddedit.aspx?dealId=" + dealId +
            "&contactId=" + contactId +
            "&dealsubscriberId=" + dealsubscriberId +
            "&from=contactdetail&refId=" + contactId;
    };

    this.DeleteDeal = function (dealId, $tr) {
        swal({
            title: translatePhrase("Delete Deal!"),
            text: translatePhrase("Are you sure you want to delete this deal?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/deal/DeleteDeal/?dealId=" + dealId + "&userId=" + userId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data) {
                            $tr.fadeOut();
                        }
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
        var request = new Object();
        request.SubscriberId = contactSubscriberId;
        request.ContactId = contactId;
        request.UserId = userId;
        $.ajax({
            type: "POST",
            url: "/api/calendarevent/GetContactCalendarEvents",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (events) {
                $divEvents.html("");
                $li.attr("data-loaded", true);
                if (events.length > 0) {
                    // bind events
                    $noEvents.addClass('hide');
                    self.BindEvents(events);
                    new CalendarEvent().Init();
                } else {
                    $noEvents.removeClass('hide');
                }
            },
            beforeSend: function () {
                $divEvents.html('<div class="text-center data-loader"><img src="/_content/_img/loading_20.gif" /></span ></div>');
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
                deleteCallback: function(props) {
                    self.DeleteEvent(props.activityId, props.$eventItem);
                }
            });
        });
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
                    url: "/api/CalendarEvent/DeleteCalendarEvent/?calendarEventId="+
                        eventId+
                        "&userId="+
                        userId+
                        "&subscriberId="+
                        contactSubscriberId,
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

        // retrieve notes
        self.RetrieveNotes();
    };

    this.SaveNote = function () {
        if ($("#txtNote").val().trim() === '' && $("#txtNoteNotesSection").val().trim() === '') {
            if ($("#txtNoteNotesSection").val().trim() === '')
                swal({
                    title: "Please enter the note!",
                    type: "warning",
                    showCancelButton: false
                });
        } else {
            var note = new Object();
            note.ActivityId = parseInt($noteAddWrap.attr("note-id"));
            note.NoteContent = $("#txtNote").val() !== '' ? $("#txtNote").val() : $("#txtNoteNotesSection").val();
            note.SubscriberId = contactSubscriberId;
            note.CompanyIdGlobal = globalCompanyId;
            note.ContactIds = contactId;
            note.UserId = userId;
            note.UserIdGlobal = globalUserId;
            note.UpdateUserId = userId;
            note.UpdateUserIdGlobal = globalUserId;
            note.OwnerUserIdGlobal = globalUserId;
            note.Invites = [{
                InviteType: "contact",
                AttendeeType: "Required",
                ContactId: contactId,
                ContactName: contactName,
                SubscriberId: subscriberId
            }];
            
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
                    $("#txtNoteNotesSection").val("");
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
                    alert(JSON.stringify(request));
                }
            });
        }
    };

    this.RetrieveNotes = function () {
        var request = new Object();
        request.UserId = userId;
        request.UserIdGlobal = globalUserId;
        request.SubscriberId = contactSubscriberId;
        request.ContactId = contactId;
        request.GlobalCompanyId = globalCompanyId;
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
        //iterate through and bind notes 
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
            title: translatePhrase("Delete Note!"),
            text: translatePhrase("Are you sure you want to delete this note?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
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
    var $noDocuments = $(".no-contact-documents");
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
            url: baseUploadUrl + "ref=" + $("#lblGuid").text() + "&folder=vompanyDocument&delete=true",
            addRemoveLinks: true,
            maxFiles: 1,
            success: function (file, response) {
                var result = jQuery.parseJSON(decodeURI(response));
                var document = result[0];
                $("#lblDocUploadDetails").text(document.FileName + "|" + document.BlobReference + "|" + document.ContainerReference);
                //file.previewElement.classList.add("dz-success");
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
        request.RefId = contactId;
        request.DocTypeId = 5;
        request.SubscriberId = contactSubscriberId;
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

        // delete icon 
        var $delete = $('<a class="delete-link delete-document FR" data-action="delete" > <i class="icon-Delete delete-icon"></i></a >');
        $outerWrap.append($delete);
        $delete.unbind("click").click(function () {
            self.DeleteDocument(document.DocumentId, $outerWrap);
        });

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
            title: translatePhrase("Delete Document!"),
            text: translatePhrase("Are you sure you want to delete this document?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/document/Delete/?id=" + id + "&userid=" + userId + "&subscriberId=" + contactSubscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        removeSpinner();
                        if (data) {
                            $outerWrap.fadeOut();
                            // reload activities/documents
                            new Documents().RetrieveDocuments();
                        }
                    },
                    error: function (request) {
                        alert(JSON.stringify(request));
                    }, beforeSend: function () {
                        addSpinner();
                    }
                });
            }
        });
    };

    this.UploadDocument = function () {
        if ($("#txtDocumentTitle").val() === "") {
            swal({ title: "Please enter the document title!", type: "warning", showCancelButton: false });
            return;
        }
        if ($("#lblDocUploadDetails").text() === "") {
            swal({ title: "Please select a document!", type: "warning", showCancelButton: false });
            return;
        }

        var doc = new Object();
        doc.SubscriberId = contactSubscriberId;
        doc.CompanyId = companyId;
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

        doc.DocumentTypeId = 5;
        doc.ContactId = contactId;

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
                removeSpinner();
                // clear controls
                $("#txtDocumentTitle").val("");
                $("#txtDocumentDescription").val("");
                $("#txtDocumentTitle").val("");
                Dropzone.forElement("#drop-wrp").removeAllFiles(true);
                // retrieve documents
                new Documents().RetrieveDocuments();
            },
            beforeSend: function () {
                addSpinner();
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };
};


var Users = function () {
    var self = this;
    var $contactUsers = $(".contact-users");
    var $liSalesTeam = $("li[data-type='salesteam']");
    var $addSalesTeamDialog = $("#addSalesTeamMemberDialog");
    var $ddlSalesTeam = $('.modal [data-id="ddlSalesTeamMember"]');
    var $ddlSalesTeamRole = $('.modal [data-id="ddlSalesTeamRole"]');

     this.Init = function () {
        $("#addSalesTeamButtonSave").unbind("click").click(function () {
            self.AddEditSalesTeamMember();
        });
        self.LoadContactUsers();
    };

    this.LoadContactUsers = function () {
        $.ajax({
            type: "GET",
            url: "/api/contact/GetContactUsers?contactId=" + contactId + "&subscriberId=" + contactSubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '',
            success: function (users) {
                $liSalesTeam.attr("data-loaded", true);
                self.BindUsers(users);
                FFGlobal.docs.detail.content.addNewItemCard({
                    type: "user",
                    $wrapper: $contactUsers,
                    link: {
                        modalTarget: "addSalesTeamMemberDialog",
                        content: "Add Sales Team Member"
                    }
                });
            },
            beforeSend: function () {
                $contactUsers.html('<div class= "text-center data-loader"><img src="/_content/_img/loading_20.gif" /></span ></div>');
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };

    // bind users
    this.BindUsers = function (users) {
        $contactUsers.html("");
        if (users.length > 0) {
            $.each(users, function (i, data) {
                var user = data.User; if (user) {
                    FFGlobal.docs.detail.content.addContactOrUserCard({
                        type: 'user',
                        $wrapper: $contactUsers,
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
                            isOwner: contactOwnerId === user.UserId,
                            salesTeamRole: (data.SalesTeamMemberData||{}).SalesTeamRole,
                            profile: {
                                pic: data.ProfilePicture,
                                title: user.Title,
                                location: user.LocationName,
                            }
                        },
                        modifyCallback: function (type) {
                            if (type === "delete") self.DeleteContactUser(user.UserId, user.SubscriberId);
                        }
                    });
                }
            });
        }
    };

    this.DeleteContactUser = function (deleteUserId, sid) {
        swal({
            title: translatePhrase("Remove Member?"),
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Remove"),
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/contact/DeleteContactUser/?deleteUserId=" +
                        deleteUserId +
                        "&contactId=" +
                        contactId +
                        "&userId=" +
                        userId +
                        "&subscriberId=" +
                        sid,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data) self.LoadContactUsers();
                        removeSpinner();
                    }, beforeSend: function () {
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
            request.UpdatedBy = userId;
            request.ContactSubscriberId = contactSubscriberId; 
            request.ContactId = contactId;
            request.GlobalUserId = $ddlSalesTeam.val();
            request.SalesTeamRole = $ddlSalesTeamRole.val();
            $.ajax({
                type: "POST",
                url: "/api/contact/AddContactUser",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(request),
                success: function () {
                    removeSpinner();
                    self.LoadContactUsers();
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


var DealsTab = function () {
    var self = this;

    this.Init = function () {

        // deals : cards & list view
        $('.showView a.icon').unbind("click").click(function (e) {
            e.preventDefault();
            $(this).parent().find('a.icon').removeClass('active');
            $(this).addClass('active');
            self.ToggleCardVsListVisibility();
        });

        // active & inactive toggles
        $('.deal-btn-wrp a.deals-link').unbind("click").click(function (e) {
            e.preventDefault();
            $(this).parent().find('a.deals-link').removeClass('active');
            $(this).addClass('active');
            // load sales stages on active/inactive change
            activeInactiveChanged = true;
            self.ToggleCardVsListVisibility();
        });

        new DealsCards().Init();

    };

    // toggle card/list and active/inactive actions
    this.ToggleCardVsListVisibility = function () {
        dataView = $('.showView a.icon.active').attr('data-view');
        activeInactiveStatus = $('.deal-btn-wrp a.deals-link.active').attr('data-status');
        if (dataView === 'card') {
            // $(".select-box-wrp").addClass("hide");
            $dealsListHolder.addClass("hide");
            $dealsCardsHolder.removeClass("hide");
            if (activeInactiveStatus === "active") {
                $activeDealCards.removeClass("hide");
                $inactiveDealCards.addClass("hide");
            } else {
                $inactiveDealCards.removeClass("hide");
                $activeDealCards.addClass("hide");
            }
        } else {
            // $(".select-box-wrp").removeClass("hide");
            $dealsCardsHolder.addClass("hide");
            $dealsListHolder.removeClass("hide");
            if (activeInactiveStatus === "active") {
                $activeDealList.removeClass("hide");
                $inactiveDealList.addClass("hide");
            } else {
                $inactiveDealList.removeClass("hide");
                $activeDealList.addClass("hide");
            }
        }
        self.LoadDeals();
    };

    // load deals
    self.LoadDeals = function () {
        if (dataView === 'card') {
            cardsCurrentPage = 1;
            new DealsCards().Init();
        } else {
            new DealsTable().Init();
        }
    };
};


var DealsCards = function () {
    var self = this;
    var $divCardView = $("#active-deal");
    var $cardViewContent = $divCardView.find('.row');

    // initialize card view
    this.Init = function () {
        self.SetInterface();
    };

    // set interface
    this.SetInterface = function () {
        var dealStages;

        if (activeInactiveStatus === "active") {
            dealStages = self.GetStages();
            $divCardView = $("#active-deal");
            $cardViewContent = $divCardView.find('.row');
        }
        else {
            dealStages = [];
            var objWon = new Object();
            objWon.SelectText = "Won";
            dealStages.push(objWon);
            var objLost = new Object();
            objLost.SelectText = "Lost";
            dealStages.push(objLost);
            var objStalled = new Object();
            objStalled.SelectText = "Stalled";
            dealStages.push(objStalled);
            $divCardView = $("#inactive-deal");
            $cardViewContent = $divCardView.find('.row');
        }
        $cardViewContent.html("");

        $.each(dealStages, function (i, ele) {
            var $divColumn = $("<div />", {
                "class": "col-xl-3 col-lg-6 col-md-6 col-sm-12 deals-box column",
                "data-stage": ele.SelectText
            });

            var $divDealCard = $("<div />", { "class": "deal-card" });

            if (i === 0) { $divColumn.addClass("col-left-box"); }
            else if (i === dealStages.length - 1) { $divColumn.addClass("col-right-box"); }
            else { $divColumn.addClass("col-mid-box"); }

            // header
            var $divHeader = $("<div />", { "class": "dcard-header" });
            var $titleWrap = $("<div />", { "class": "ibox-title" });

            switch (ele.SelectText) {
                case "Qualifying": $titleWrap.addClass("LBlue"); break;
                case "Negotiation": $titleWrap.addClass("MBlue"); break;
                case "Final Negotiation": $titleWrap.addClass("DBlue"); break;
                case "Trial Shipment": $titleWrap.addClass("Green"); break;
                case "Won": $titleWrap.addClass("DGreen"); break;
                case "Lost": $titleWrap.addClass("DRed"); break;
                case "Stalled": $titleWrap.addClass("DGrey"); break;
                default: $titleWrap.addClass("LBlue"); break;
            }
            $divHeader.append($titleWrap);

            var $h3 = $("<h3 />", { "class": "card-title language-entry", "html": ele.SelectText });
            $titleWrap.append($h3);

            var $spanCount = $("<span />", { "class": "card-count", "html": "" });
            $titleWrap.append($spanCount);

            $divDealCard.append($divHeader);

            // grid-box
            var $divContainer = $("<div />", { "class": "grid-wrap", "data-stage": ele.SelectText });
            $divDealCard.append($divContainer);

            // append column to main content
            $divColumn.append($divDealCard);
            $cardViewContent.append($divColumn);
        });

        // retrieve leads for each column
        $divCardView.find('.column').each(function () {
            var $this = $(this);
            var stage = $this.attr("data-stage");
            cardsCurrentPage = 1;
            self.RetrieveStageDeals(stage);
        });
    };

    // retrieve deals for sales stages
    this.RetrieveStageDeals = function (dealStage) {
        var $column = $divCardView.find(".column[data-stage='" + dealStage + "']");

        if ($column.length > 0) {
            // set col header values to empty
            var $colHeader = $column.find(".ibox-title");
            var $dealsCount = $colHeader.find(".card-count");
            $dealsCount.html("");

            // grid-box
            var $dealsContainer = $column.find(".grid-wrap");

            if (cardsCurrentPage === 1) {
                // if the first page clear the content 
                $dealsContainer.html("");
            }

            // filters
            var filters = new Object();
            filters.SubscriberId = subscriberId;
            filters.UserId = userId;
            filters.RecordsPerPage = 10;
            filters.CurrentPage = cardsCurrentPage;
            filters.SortBy = "DealName asc";
            filters.SalesStages = [];
            filters.SalesStages.push(dealStage);
            filters.ContactId = contactId;

            $.ajax({
                type: "POST",
                url: "/api/deal/getdeals",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(filters),
                success: function (response) {
                    // remove loading message
                    bindLoadingMsg("", $dealsContainer, false);

                    $dealsCount.html(response.Records);
                    $dealsCount.attr("deals-count", response.Records);

                    $column.attr("total-pages", response.TotalPages);

                    // bind deals
                    self.BindDeals(response.Deals, $dealsContainer);

                    //set column height
                    //self.SetColumnHeight($column);

                    // initialize sortable
                    if (activeInactiveStatus === "active") {
                        self.InitSortable();
                    }
                },
                beforeSend: function () {
                    //add loading message
                    bindLoadingMsg(translatePhrase("Loading deals") + "...", $dealsContainer, true);
                },
                error: function () { }
            });
        }
    };

    /*
    // set column height
    this.SetColumnHeight = function ($column) {
        if ($(window).width() > 767) {
            var $dealsContainer = $column.find(".grid-wrap");
            var dealsCount = $dealsContainer.find(".grid-box").length;
            var colStyle = $column.attr("style");

            var height = (230 * dealsCount);

            height = $(window).height() < 1024 ? (height + 30) : (height + 155);
            $column.attr("style", (colStyle?colStyle+";":"") + "height:" + height + "px");
        }
    };
    */

    // bind deal boxes
    this.BindDeals = function (deals, $dealsContainer) {
        // iterate and bind deals
        if (deals.length > 0) {
            $.each(deals, function (i, deal) {
                FFGlobal.docs.detail.content.addDealCard({
                    $wrapper: $dealsContainer,
                    deal: deal
                });
            });

            $dealsContainer.find(".grid-box").each(function () {
                var $this = $(this);
                var dealId = $(this).attr("data-id");
                $.ajax({
                    type: "GET",
                    url: "/api/deal/GetDealRevenueFromUserCurrency?dealId=" + dealId + "&userId=" + userId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    timeout: 50000,
                    data: '',
                    success: function (resposne) {
                        $this.find(".data-deal-revenue").html(resposne.CurrencySymbol + formatNumber(parseInt(resposne.Revenue)));
                        $this.find(".data-deal-profit").html(resposne.CurrencySymbol + formatNumber(parseInt(resposne.Profit)));
                    },
                    beforeSend: function () {
                    },
                    error: function (request, status, error) {
                        alert(JSON.stringify(request));
                    }
                });
            });
        }

        // add an empty box
        self.AddEmptyDroppableArea($dealsContainer);
    };

    this.AddEmptyDroppableArea = function ($dealsContainer) {
        var dealsCount = $dealsContainer.find(".grid-box:not(.no-deal)").length;
        if (dealsCount > 0) {
            // remove droppable area
            $dealsContainer.find(".no-deal").remove();
        }
        else {
            $noDeals = '<div class="grid-box no-deal text-center"><p>No ' + activeInactiveStatus + ' Deals</p></div>';
            $dealsContainer.append($noDeals);
        }
    };

    // gets profile picture
    this.GetProfileImage = function ($img, contactid) {
        $.getJSON("/api/contact/getcontactprofilepic/?contactid=" + contactid,
            function (response) {
                if (response !== '') {
                    $img.attr("src", response + "?w=100&h=100&mode=crop");
                } else {
                    $img.attr("src", "/_content/_img/no-pic.png?w=30&h=30&mode=crop");
                }
            });
    };

    // get deal stages
    this.GetStages = function () {
        var salesStages = [];
        $.ajax({
            type: "GET",
            url: "/api/salesstage/GetSalesStagesForDropdown?subscriberId=" + subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            timeout: 50000,
            data: '',
            success: function (resposne) {
                salesStages = resposne;
            },
            beforeSend: function () {
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
        return salesStages;
    };

    // update sales stage on drag and drop
    this.UpdateDealSalesStage = function (dealId, newStage, oldStage) {
        if (newStage === oldStage) {
            return;
        }

        var $divCardView = $(".deal-cards");
        var $columnNew = $divCardView.find(".column[data-stage='" + newStage + "']");
        var newDealsCount = parseInt($columnNew.find(".title-no").attr("deals-count"));
        var $columnOld = $divCardView.find(".column[data-stage='" + oldStage + "']");
        var oldDealsCount = parseInt($columnOld.find(".title-no").attr("deals-count"));

        // set old deals count
        $columnOld.find(".title-no").html(oldDealsCount - 1);
        $columnOld.find(".title-no").attr("deals-count", oldDealsCount - 1);

        // set new deals count
        $columnNew.find(".title-no").html((newDealsCount + 1));
        $columnNew.find(".title-no").attr("deals-count", newDealsCount + 1);

        //set column height
        //self.SetColumnHeight($columnNew);
        //self.SetColumnHeight($columnOld);

        // set the data URL
        var dataUrl = "/api/deal/UpdateDealSalesStage/?dealId=" + dealId +
            "&salesStage=" + newStage +
            "&subscriberId=" + subscriberId +
            "&userId=" + userId;
        // ajax to update the status
        $.ajax({
            type: "GET",
            url: dataUrl,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: '',
            success: function (response) {
            },
            beforeSend: function () {
            },
            error: function (request, status, error) {
                //set old values again
                $columnOld.find(".deals-count").html(oldDealsCount);
                $columnOld.find(".deals-count").attr("deals-count", oldDealsCount);
                // set new deals count
                $columnNew.find(".deals-count").html(newDealsCount);
                $columnNew.find(".deals-count").attr("deals-count", newDealsCount);
            }
        });
    };

    // initialize sortable
    this.InitSortable = function () {
        var oldStage = "";
        var newStage = "";
        var element = "[class*=grid-box]";
        var handle = "div.grid-box:not(.no-deal)";
        var connect = "[class*=grid-wrap]";
        var $oldDealContaner;
        var $newDealContaner;

        $(".grid-wrap").sortable(
            {
                handle: handle,
                connectWith: connect,
                tolerance: 'pointer',
                // revert: true,
                forcePlaceholderSize: true,
                //  opacity: 0.8,
                start: function (event, ui) {
                    ui.item.addClass('tilt');
                    //// Start monitoring tilt direction
                    self.Tilt_direction(ui.item);
                    var $item = ui.item;
                    var dealId = $item.find(".grid-box").attr("data-id");
                    oldStage = $item.closest(".column").attr("data-stage");
                    $oldDealContaner = $item.closest(".grid-wrap");
                },
                stop: function (event, ui) {
                    ui.item.removeClass("tilt");
                    var $item = ui.item;
                    var dealId = $item.find(".grid-box").attr("data-id");
                    newStage = $item.closest(".column").attr("data-stage");
                    $newDealContaner = $item.closest(".grid-wrap");

                    // add an empty box
                    new DealsCards().AddEmptyDroppableArea($oldDealContaner);
                    new DealsCards().AddEmptyDroppableArea($newDealContaner);

                    // update status
                    self.UpdateDealSalesStage(dealId, newStage, oldStage);
                }
            })
            .disableSelection();
    };

    // when scroll bar hits the bottom reload more deals
    this.InitScrollLoader = function () {
        var $divCardView = $("#active-deal");
        if (activeInactiveStatus === "inactive") {
            $divCardView = $("#inactive-deal");
        }
        var nearToBottom = 100;
        if ($(window).scrollTop() + $(window).height() > $(document).height() - nearToBottom) {
            cardsCurrentPage = cardsCurrentPage + 1;
            // load more stages
            $divCardView.find('.column').each(function () {
                var $this = $(this);
                var stage = $this.attr("data-stage");
                var totalpages = parseInt($this.attr("total-pages"));
                if (cardsCurrentPage <= totalpages) {
                    self.RetrieveStageDeals(stage);
                }
            });
        }
    };

    // Monitor tilt direction and switch between classes accordingly
    this.Tilt_direction = function (item) {
        var left_pos = item.position().left,
            move_handler = function (e) {
                if (e.pageX >= left_pos) {
                    item.addClass("right");
                    item.removeClass("left");
                } else {
                    item.addClass("left");
                    item.removeClass("right");
                }
                left_pos = e.pageX;
            };
        $("html").bind("mousemove", move_handler);
        item.data("move_handler", move_handler);
    };
};


var DealsTable = function () {
    var self = this;
    var $currentBody = null;
    var $activeListBody = $activeDealList.find("tbody");
    var $inactiveListBody = $inactiveDealList.find("tbody");
    var $noDeals = $("#noDeals");
    var $dealsContainer = $(".dealList");
    var currentPage = 1;

    this.Init = function () {
        self.RetrieveDeals();
        self.InitSort();
    };

    this.RetrieveDeals = function () {
        $currentBody = activeInactiveStatus === "active" ? $activeListBody : $inactiveListBody;
        $activeDealList.addClass("hide");
        $inactiveDealList.addClass("hide");
        $noDeals.addClass("hide");
        $activeListBody.html("");
        $inactiveListBody.html("");
        $(".pagination").addClass('hide');

        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.RecordsPerPage = recordsPerPage;
        filters.CurrentPage = currentPage;
        filters.SortBy = tableSortOrder;
        filters.UserId = userId;
        filters.ContactId = contactId;
        filters.SalesStages = []; 
        $.each(activeInactiveStatus==='active'?salesStages:["won","lost","stalled"], function(i, val){
            filters.SalesStages.push(val);
        });

        $.ajax({
            type: "POST",
            url: "/api/deal/getdeals",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(filters),
            success: function (response) {
                bindLoadingMsg("", $dealsContainer, false);
                if (response.Deals.length > 0) {
                    activeInactiveStatus==="active"? $activeDealList.removeClass("hide"):$inactiveDealList.removeClass("hide");
                    self.BindDeals(response.Deals);
                }
                else $noDeals.removeClass('hide');
            },
            beforeSend: function () {
                bindLoadingMsg(translatePhrase("Loading deals") + "...", $dealsContainer, true);
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.BindDeals = function (deals) {
        // iterate and bind deals
        $.each(deals, function (i, deal) {
            var $tr = self.GetDealRowHtml(deal);
            $currentBody.append($tr);
        });
    };

    this.GetDealRowHtml = function (deal) {
        var $tr = $("<tr />", { "data-id": deal.DealId });

        // deal name
        var $tdDealName = $("<td />", { "class": "" });
        var $aDealName = $("<a />", { "href": "javascript:void(0)", "class": "hover-link", "html": deal.DealName });
        $aDealName.attr("href", "/Deals/DealDetail/dealdetail.aspx?dealId=" + deal.DealId + "&dealsubscriberId=" + deal.SubscriberId);
        $tdDealName.append($aDealName);
        if (deal.DealType && deal.DealType !== '') {
            $tdDealName.append($("<span/>", { 'class': 'd-block', "html": deal.DealType }));
        }
        $tr.append($tdDealName);

        // deal company
        var $tdCompany = $("<td />");
        var companyDetailUrl = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + deal.CompanyId + "&subscriberId=" + deal.SubscriberId;
        var $aCompanyName = $("<a />", { "href": companyDetailUrl, "class": "ltxt-color", "html": deal.CompanyName });
        $tdCompany.append($aCompanyName);
        if (deal.PrimaryContactName && deal.PrimaryContactName !== '') {
            var $contactName = $("<span />", { 'class': 'd-block', "html": deal.PrimaryContactName, "data-primary-contact-id": deal.PrimaryContactId });
            $tdCompany.append($contactName);
        }
        $tr.append($tdCompany);

        // deal city/Country
        var $tdCountry = $("<td />");
        var $cityName = $("<div />", { "class": "company", "html": "City" });
        var $countryName = $("<span />", { "html": "Country" });
        $tdCountry.append($cityName);
        $tdCountry.append($countryName);
        $tr.append($tdCountry);

        // sales team
        var $tdSalesTeam = $("<td />");
        $tdSalesTeam.append($("<div />", { "data-type": "sales-team", "html": deal.SalesTeam }));
        $tr.append($tdSalesTeam);

        // sales stage
        var $tdSalesStage = $("<td />", { "data-sales-stage-id": deal.SalesStageId, "class": "text-center" });
        var $tdSalesStatus = $("<div />", { "class": "border-status", "html": deal.SalesStageName });
        $tdSalesStage.append($tdSalesStatus);

        var SalesStageName = deal.SalesStageName.trim();

        switch (SalesStageName) {
            case "Qualifying": $tdSalesStatus.addClass("lblue"); break;
            case "Negotiation": $tdSalesStatus.addClass("mblue"); break;
            case "Final Negotiation": $tdSalesStatus.addClass(""); break;
            case "Trial Shipment": $tdSalesStatus.addClass("green"); break;
            case "Lost": $tdSalesStatus.addClass("red"); break;
            case "Won": $tdSalesStatus.addClass("dgreen"); break;
            case "Stalled": $tdSalesStatus.addClass("grey"); break;
            default:
        }
        $tr.append($tdSalesStage);

        // last activity date
        var $tdLastActivityDate = $("<td />", { "class": "text-center" });
        if (deal.LastActivityDate) {
            $tdLastActivityDate.html(moment(deal.LastActivityDate).format("DD-MMM-YY"));
        }
        $tr.append($tdLastActivityDate);

        // date
        var $tdDecisionDate = $("<td />", { "class": "text-center" });
        var $thDecisionDate = $(".th-decision-date");
        if (deal.DecisionDate) {
            $tdDecisionDate.html(moment(deal.DecisionDate).format("DD-MMM-YY"));
        }
        $tr.append($tdDecisionDate);
        return $tr;
    };

    this.InitSort = function () {
        $("#tblDeals>thead>tr>th").unbind("click").click(function () {
            var $this = $(this);
            var sortFieldName = $this.attr("data-field-name");
            if (sortFieldName && sortFieldName !== '') {
                var sortorder = "asc";
                // check if already any sort going on
                var $sortitem = $this.find(".sort-item");
                if ($sortitem && $sortitem !== null) {
                    // already sorting using this field - check if ASC or DESC 
                    var currentSortOrder = $sortitem.hasClass("fa-sort-asc") ? "asc" : "desc";
                    // new sort order
                    sortorder = currentSortOrder === "asc" ? "desc" : "asc";
                    tableSortOrder = sortFieldName + " " + sortorder;
                } else {
                    // NOT sorting using this field - use ASC
                    sortorder = "asc";
                    tableSortOrder = sortFieldName + " " + sortorder;
                }

                // remove current sort up/down icons
                $("#tblDeals>thead>tr>th").find(".sort-item").remove();

                // add new sort up/down item
                if (sortorder === "asc") {
                    $this.append($("<i class=\"pull-right sort-item text-primary m-t-xs fa fa-sort-asc\"></i>"));
                } else {
                    $this.append($("<i class=\"pull-right sort-item text-primary m-t-xs fa  fa-sort-desc\"></i>"));
                }

                // do the search again
                var currentPage = 1;
                self.RetrieveDeals();
            }
        });
    };
};


function loadUserProfilePicture() {
    $(".user-profile-pic").each(function () {
        var $img = $(this);
        var userId = $img.attr("data-id");
        $.getJSON("/api/user/getuserprofilepic/?userId=" + userId,
            function (response) {
                if (response !== '') {
                    $img.attr("src", response + "?w=80&h=80&mode=crop");
                } else {
                    $img.attr("src", "/_content/_img/no-pic.png?w=80&h=80&mode=crop");
                }
            });
    });
}


function loadRevenues() {
    var totalRevenue = 0.0;
    var currencyCode = "$";
    $("[data-type='deal-revenue']").each(function () {
        var $this = $(this);
        var dealId = $this.attr("deal-id");

        $.ajax({
            type: "GET",
            url: "/api/deal/GetDealRevenue/?dealId=" + dealId + "&userId=" + userId,
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                currencyCode = data.CurrencySymbol;
                totalRevenue += data.Revenue;
                $this.html(data.CurrencySymbol + " " + formatNumber(parseInt(data.Revenue)));
                // total revenue
                $revenue.html(currencyCode + " " + formatNumber(parseInt(totalRevenue)));
            }
        });
    });
}


function RefreshEvents() {
    currentPage = 1;
    new Events().RetrieveEvents();
}


function RefreshParent() {
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // do the following to get rid of the padding
    $('body').removeAttr('style');
    // load page
    location.reload();
}

function GoToDealDetail(dealId) {
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // do the following to get rid of the padding
    $('body').removeAttr('style');

    // go to deal detail
    location.href = "/Deals/DealDetail/dealdetail.aspx?dealId=" + dealId + "&newlane=1";
}


// Event calendar
$(document).ready(function () {
    var disable_date = ["2018-11-24", "2018-11-25", "2018-11-26", "2018-12-5", "2019-1-20"];
    var date = new Date();

    if ($(".calendar-wrp #datepicker").length > 0) {
        $(".calendar-wrp #datepicker").datepicker({
            dateFormat: 'yy-mm-dd',
            beforeShowDay: function (date) {
                var m = date.getMonth(), d = date.getDate(), y = date.getFullYear();
                for (i = 0; i < disable_date.length; i++) {
                    if ($.inArray(y + '-' + (m + 1) + '-' + d, disable_date) !== -1) {
                        return [true, 'ui-state-event', ''];
                    }
                }
                return [true];
            },

            numberOfMonths: [3, 1],
            minDate: 0
        });

        var debounce;
        $(window).resize(function () {
            clearTimeout(debounce);
            if ($(window).width() < 768) {
                debounce = setTimeout(function () {
                    debounceDatepicker(1)
                },
                    250);
            } else {
                debounce = setTimeout(function () {
                    debounceDatepicker(3)
                },
                    250);
            }
        }).trigger('resize');

        function debounceDatepicker(no) {
            if ($("#datepicker").length > 0) {
                $("#datepicker").datepicker("option", "numberOfMonths", no);
            }
        }
    }
});


// Deals - list view data table
$(document).ready(function () {
    if ($('#deal-listtable').length > 0) {
        var dt_list = $('#deal-listtable').DataTable({
            "paging": false,
            "info": false,
            "searching": true
        });

        $('#searchdata').on('keyup change', function () {
            dt_list.search($(this).val()).draw();
        });

        $('.companies-table').on('click', 'tbody tr', function () {
            var dataHref = $(this).data('href');
            window.location.href = dataHref;
        });

    }

    if ($('#deal-ltable-inc').length > 0) {
        var dt_deal = $('#deal-ltable-inc').DataTable({
            "paging": false,
            "info": false,
            "searching": false
        });
    }

    if ($('#contact-actlist').length > 0) {
        var dt_cont = $('#contact-actlist').DataTable({
            "paging": false,
            "info": false,
            "searching": false
        });

        $('.contacts-table').on('click', 'tbody tr', function () {
            var dataHref = $(this).data('href');
            window.location.href = dataHref;
        });
    }

});

// Tasks - datepicker
$(document).ready(function () {
    if ($(".custom-select2").length > 0) {
        $(".custom-select2").select2({ allowClear: true, placeholder: "" });
    }
	
	if($('#activity-datatable tbody tr').length <= 0){
		$('.empty_activity').removeClass('hide');
		$('#activity-datatable').hide();
	}
});

//move the overview columns (this moves the right column under the left column for certain window sizes)
(function() {
    var isLeft = null;
    var $left = $('#tab-overview .col-left-box');
    var $right = $('#tab-overview .col-right-box');
    var $rightWrapper = $('#tab-overview .col-right-box .right-col');
    $(document).ready(function() {
        position();
    });
    $(window).resize(function() {
        position();
    });

    function position() {
        var docWidth = $(document).width();
        var moveLeft = window.matchMedia("(min-width: 921px) and (max-width: 1199px)").matches;
        if (moveLeft!==isLeft) {
            (moveLeft?$left:$right).append($rightWrapper);
            isLeft = moveLeft;
        }
    }
})();
