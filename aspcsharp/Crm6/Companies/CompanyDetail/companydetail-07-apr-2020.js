var currentPage = 1;
var recordsPerPage = 30;
var isAjaxLoading = false;
var companyId = parseInt($("#lblCompanyId").text());
var companySubscriberId = parseInt($("#lblCompanySubscriberId").text());
var globalCompanyId = parseInt($("#lblGlobalCompanyId").text());
var globalUserId = parseInt($("#lblUserIdGlobal").text());
var companyName = $("#lblCompanyName").text();
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var isAdmin = $("#lblIsAdmin").text() === "1";
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
var subscriberSalesStages = [];
var activeInactiveChanged = false;
var $ddlSalesStage = $('#ddlSalesStage'); 

var salesOwnerId = parseInt($("#lblSalesOwnerId").text());
Dropzone.autoDiscover = false;

var docUserData = {
    subscriberId: subscriberId,
    userId: userId,
    globalUserId: globalUserId,
    companyId: companyId,
    companySubscriberId: companySubscriberId,
    globalCompanyId: globalCompanyId,
    companyName: companyName
}

$(function () {

    $(".th_checkbox input:checkbox").prop('checked', false);

    // get susbcriber sales stages
    $("#ddlSalesStage > option").each(function () {
        if (this.text !== "All") {
            subscriberSalesStages.push(this.text);
        }
    });

    // Overview tab - Details - if no value for a field hide it
    jQuery('#tab-overview .detail-card .ibox-content .inner-wrp').each(function () {
        var getText = $(this).find('.card-date').text();
        getText = getText.replace(/\s/g, '');
        if (getText == '-' || getText == '') {
            $(this).hide();
        }
    });

    $("#ddlSalesStage").select2({ placeholder: "Sales Stage", allowClear: false });
    // set add event action attributes 
    $("[data-action='add-event']").attr("data-global-company-id", globalCompanyId);
    $("[data-action='add-event']").attr("company-name", companyName);

    // edit company
    $("[data-action='edit-company']").unbind("click").click(function () {
        location.href = "/Companies/CompanyAddEdit/CompanyAddEdit.aspx?companyId=" + companyId + "&subscriberId=" + companySubscriberId + "&from=companydetail";
    });

    // open new company contact if passed
    var newContact = getQueryString("newcontact");
    if (newContact) {
        new Contacts().OpenContactAddEditDialog(0, $("#lblCompanyId").text());
    }

    $("#globalCompanyLookup").on('shown.bs.modal', function (event) {
        $(".modal").removeAttr("tabindex");
        $(".modal").find("#txtGlobalCompanySearch").focus();
    });

    $("#txtGlobalCompanySearch").keyup(function () {
        if ($("#txtGlobalCompanySearch").val().length > 2) {
            GlobalCompanySearch();
        }
    });

    $(".th_checkbox input:checkbox").change(function () {
        if ($(".th_checkbox input:checked").length > 0) {
            $(".chk_related_companies input:checkbox").prop('checked', true);
            $("#pnlRelatedCompanyButtons").removeClass("hide");
        } else {
            $(".chk_related_companies input:checkbox").prop('checked', false);
            $("#pnlRelatedCompanyButtons").addClass("hide");
        }
    });

    $(".chk_related_companies input:checkbox").change(function () {
        if ($(".chk_related_companies input:checked").length > 0) {
            $("#pnlRelatedCompanyButtons").removeClass("hide");
        } else {
            $("#pnlRelatedCompanyButtons").addClass("hide");
        }
    });

    // load deals
    new Deals().Init();

    $("#company-tabs").find("li").unbind("click").click(function () {
        var dataType = $(this).attr("data-type");
        var isLoaded = $(this).attr("data-loaded");
        if (dataType !== "" && !isLoaded) {
            switch (dataType) {
                case "events":
                    new Events().Init();
                    break;
                case "tasks":
                    new FFGlobal.docs.detail.tabs.tasks({
                        type: "company",
                        data: docUserData
                    }).init();
                    break;
                case "notes":
                    // new Notes().Init();
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
                case "relatedcompanies":
                    window.location.hash = "#relatedcompanies";
                    break;
                default:
            }
        }
    });

    $(".btnAddNote").unbind('click').click(function () {
        new Notes().SaveNote();
    });

    // check query string for hashes and load the correct tab
    var hash = location.href.substring(location.href.indexOf('#'));
    if (hash === "#contacts") {
        $("#company-tabs").find("li[data-type='contacts']").find("a").click();
    } else if (hash === "#deals") {
        $("#company-tabs").find("li[data-type='deals']").find("a").click();
    } else if (hash === "#relatedcompanies") {
        $("#company-tabs").find("li[data-type='relatedcompanies']").find("a").click();
    }

    // related companies
    $("#relatedCompanySetup").on('shown.bs.modal', function () {
        initRelatedCompanyDropdown();
    });

    // init note actions 
    new Notes().Init();
    new Notes().InitFirstTabNoteActions();
});

function GlobalCompanySearch() {
    $("#txtGlobalCompanySearch").removeClass("error");
    var searchTerm = $("#txtGlobalCompanySearch").val();
    var filters = new Object();
    filters.SubscriberId = subscriberId;
    filters.UserId = userId;
    filters.RecordsPerPage = recordsPerPage;
    filters.Keyword = $("#txtGlobalCompanySearch").val();

    // api to retrieve global companies
    $.ajax({
        type: "POST",
        url: "/api/Company/SearchGlobalCompanies",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 100000,
        data: JSON.stringify(filters),
        success: function (companyResponse) {
            // remove loading message
            $divGlobalLoading.addClass("hide");
            if (companyResponse.Companies.length > 0) {
                $tblGlobalCompanies.removeClass("hide");
                self.BindCompanies(companyResponse);
            } else {
                $tblGlobalCompanies.addClass("hide");
            }
        }, beforeSend: function () {
            $divGlobalLoading.removeClass("hide");
            $tblGlobalCompanies.addClass("hide");
        }, error: function (request, status, error) {
            alert(JSON.stringify(request));
        }
    });

    this.BindCompanies = function (companyResponse) {
        $globalCompanytbody.html("");
        var companies = companyResponse.Companies;
        accessibleCompanyIds = companyResponse.AccessibleCompanyIds;
        // iterate and bind companies
        $.each(companies, function (i, company) {
            //  alert("asdas")
            var $tr = self.GetCompanyItemHtml(company);
            $globalCompanytbody.append($tr);
        });
    };

    this.GetCompanyItemHtml = function (company) {

        var hasPermission = true;
        if (accessibleCompanyIds !== null) {
            //check if has permission
            var found = $.grep(accessibleCompanyIds, function (element, index) {
                return element === company.GlobalCompanyId;
            });
            hasPermission = found.length > 0;
        } else {
            hasPermission = false;
        }

        var companyDetailLink = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + company.CompanyId + "&subscriberId=" + company.SubscriberId;
        var $tr = $("<tr/>", { "data-id": company.CompanyId });

        // company name
        var $tdCompanyNameAddress = $("<td/>", { "class": "W300" });

        var $pCompanyName = $("<p/>", { "class": "font-bold FontSize13", "html": company.CompanyName, "data-subcriber-id": company.SubscriberId });
        $tdCompanyNameAddress.append($pCompanyName);
        $tr.append($tdCompanyNameAddress);

        // division
        if (company.Division && company.Division !== '') {
            $tdCompanyNameAddress.append($("<p/>", { "html": company.Division }));
        }

        // city
        var cityLocations = [];
        if (company.City && company.City !== '') {
            cityLocations.push(company.City);
        }
        if (company.CountryName && company.CountryName !== '')
            cityLocations.push(company.CountryName);

        if (cityLocations.length > 0) {
            var $p = $("<p />", { "html": cityLocations.join(", ") });
            $tdCompanyNameAddress.append($p);
        }

        // sales team
        var $tdSalesTeam = $("<td/>", { "class": "vertical-middle" });
        if (company.SalesTeam && company.SalesTeam !== "") {
            $tdSalesTeam.append($("<p/>", { "class": "", "html": "<span class='text-success FontSize12 m-l-sm'>Sales Team: </span>" + company.SalesTeam }));
        }
        $tr.append($tdSalesTeam);

        // actions
        var $tdActions = $("<td/>", { "class": "W120 vertical-middle action-cell  text-center" });

        if (hasPermission || isAdmin) {
            var $aView = $("<a />", { "html": "View", "class": "m-r-sm", "target": "_blank", "href": companyDetailLink });
            $tdActions.append($aView);

        } else {
            // if (company.SubscriberId == subscriberId) {
            // show request access
            var $aRequestAccess = $("<a />", { "html": "Request Access", "class": "m-r-sm" });
            $aRequestAccess.unbind("click").click(function () {
                new Companies().RequestAccess(company.CompanyId, company.SubscriberId, $tdActions);
            });
            $tdActions.append($aRequestAccess);
            //  }
        }

        $tr.append($tdActions);

        return $tr;

    };
};

var Deals = function () {
    var self = this;
    var $noDeals = $(".overview-nodeals");
    var $dealsLoading = $(".deals-loading");
    var $deals = $(".deals");

    this.Init = function () {
        // get company deals
        self.GetCompanyDeals();

        // new deal
        $("[data-action='new-deal']").unbind('click').click(function () {
            self.GoToDealAddEdit(0, companyId, companySubscriberId);
        });
    };

    this.GetCompanyDeals = function () {
        isAjaxLoading = true;
        // load deals
        $.ajax({
            type: "GET",
            url: "/api/deal/GetCompanyDeals?companyId=" + companyId + "&subscriberId=" + companySubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '',
            success: function (deals) {
                $deals.html("");
                isAjaxLoading = false;
                $dealsLoading.addClass('hide');

                if (deals.length > 0) {
                    $noDeals.addClass('hide');
                    // bind deals
                    self.BindDeals(deals);
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
                    $this.find(".data-deal-revenue-spot").html(resposne.CurrencySymbol + formatNumber(parseInt(resposne.SpotRevenue)));
                    $this.find(".data-deal-profit-spot").html(resposne.CurrencySymbol + formatNumber(parseInt(resposne.SpotProfit)));
                },
                beforeSend: function () {
                },
                error: function (request, status, error) {
                    alert(JSON.stringify(request));
                }
            });
        });
    };

    this.GoToDealAddEdit = function (dealId, cid, dealsubscriberId) {
        location.href = "/Deals/dealaddedit/dealaddedit.aspx?dealId=" +
            dealId +
            "&companyId=" +
            cid +
            "&dealsubscriberId=" +
            dealsubscriberId + "&from=companydetail";
    };

    this.DeleteDeal = function (dealId, $tr) {
        swal({
            title: translatePhrase("Delete Deal!"),
            text: translatePhrase("Are you sure you want to delete this deal?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete Deal!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/deal/DeleteDeal/?dealId=" + dealId + "&userId=" + $("#lblUserId").text(),
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


var Contacts = function () {
    var self = this;
    var $companyContacts = $(".company-contacts");
    var $liContacts = $("li[data-type='contacts']");
    
    this.Init = function () {
        $companyContacts.html("");
        // retrieve company contacts
        self.LoadCompanyContacts();
    };

    this.LoadCompanyContacts = function () {
        // load company contacts
        $.ajax({
            type: "GET",
            url: "/api/Company/GetCompanyContacts?companyId=" + companyId + "&subscriberid=" + companySubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '',
            success: function (contacts) {
                $liContacts.attr("data-loaded", true);
                self.BindContacts(contacts);
                FFGlobal.docs.detail.content.addNewItemCard({
                    type: "contact",
                    $wrapper: $companyContacts,
                    link: {
                        href: '/contacts/contactaddedit/contactaddedit.aspx?subscriberid='+companySubscriberId+'&companyId='+companyId,
                        content: 'Add Contact'
                    }
                });
                contactActionMenu();
            },
            beforeSend: function () {
                $companyContacts.html('<div class="text-center data-loader"><img src="/_content/_img/loading_20.gif" /></span ></div>');
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.BindContacts = function (contacts) {
        $companyContacts.html("");
        if (contacts.length > 0) {
            $.each(contacts, function (i, contactModel) {
                var contact = contactModel.Contact;
                FFGlobal.docs.detail.content.addContactOrUserCard({
                    type: 'contact',
                    $wrapper: $companyContacts,
                    hideRemove: true,
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
                    }
                });
            });
        }
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
        request.SubscriberId = companySubscriberId;
        request.CompanyIdGlobal = globalCompanyId;
        request.UserId = userId;

        $.ajax({
            type: "POST",
            url: "/api/calendarevent/GetCompanyCalendarEvents",
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
                    url: "/api/CalendarEvent/DeleteCalendarEvent/?calendarEventId=" +
                        eventId +
                        "&userId=" +
                        userId +
                        "&subscriberId=" +
                        companySubscriberId,
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
    var $noteAddWrap = $(".add-note");

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

        // get notes list
        self.RetrieveNotes();

        // set focus to new note text field
        $("#txtNoteNotesSection").focus();

    };

    this.InitFirstTabNoteActions = function () {
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
    };

    this.SaveNote = function () {
        if ($("#txtNote").val().trim() === '' && $("#txtNoteNotesSection").val().trim() === '') {
            if ($("#txtNoteNotesSection").val().trim() === '')
                swal({
                    title: "Enter a note!",
                    type: "warning",
                    showCancelButton: false
                });
        } else {
            var note = new Object();
            note.ActivityId = parseInt($noteAddWrap.attr("note-id"));
            note.CompanyIdGlobal = globalCompanyId;
            note.NoteContent = $("#txtNote").val() !== '' ? $("#txtNote").val() : $("#txtNoteNotesSection").val();
            note.SubscriberId = companySubscriberId;
            note.UserId = userId;
            note.UserIdGlobal = globalUserId;
            note.UpdateUserId = userId;
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
        request.SubscriberId = companySubscriberId;
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
                    error: function () { }
                });
            }
        });
    };
};


var Documents = function () {
    var self = this;
    var $noDocuments = $(".no-company-documents");
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

        // set focus to new document title text field
        $("#txtDocumentTitle").focus();
    };

    this.RetrieveDocuments = function () {
        // set document request object
        var request = new Object();
        request.RefId = companyId;
        request.DocTypeId = 6;
        request.SubscriberId = companySubscriberId;
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
                // TODO: new spinner
                $divDocuments.html('<div class="text-center data-loader"><img src="/_content/_img/loading_20.gif" /></div>');
            },
            error: function (request) {
            }
        });
    };

    this.BindDocuments = function (documents) {
        $divDocuments.html("");
        // iterate through and bind documents
        $.each(documents,
            function (i, document) {
                var $documentItem = self.GetDocumentItemHtml(document);
                $divDocuments.append($documentItem);
            });
    };

    this.GetDocumentItemHtml = function (document) {

        // TODO: get all below html from server instead of parsing javascript
        var $outerWrap = $("<div />", { "class": "doc-wrap" });

        $outerWrap.click(function () {
            addSpinner();
            setTimeout(removeSpinner, 2000);
        });

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
                    url: "/api/document/Delete/?id=" + id + "&userid=" + userId + "&subscriberId=" + companySubscriberId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        swal.close();
                        if (data) {
                            $outerWrap.fadeOut();
                            // reload activities/documents
                            new Documents().RetrieveDocuments();
                        }
                    },
                    error: function (request) {
                        alert(JSON.stringify(request));
                    }, beforeSend: function () {
                        // TODO: new spinner
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
        doc.SubscriberId = companySubscriberId;
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

        doc.DocumentTypeId = 6;
        doc.CompanyId = companyId;
        doc.CompanyIdGlobal = globalCompanyId;

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
                // TODO: new spinner
                swal({ text: translatePhrase("Uploading Document") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: false });
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };
};


var Users = function () {
    var self = this;
    var $companyUsers = $(".company-users");
    var $liSalesTeam = $("li[data-type='salesteam']");
    var $addSalesTeamDialog = $("#addSalesTeamMemberDialog");
    var $ddlSalesTeam = $('.modal [data-id="ddlSalesTeamMember"]');
    var $ddlSalesTeamRole = $('.modal [data-id="ddlSalesTeamRole"]');

    this.Init = function () {
        $("#addSalesTeamButtonSave").unbind("click").click(function () {
            self.AddEditSalesTeamMember();
        });
        self.LoadCompanyUsers();
    };

    this.LoadCompanyUsers = function () {
        $.ajax({
            type: "GET",
            url: "/api/company/GetCompanyUsers?companyId=" + companyId + "&subscriberId=" + companySubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '',
            success: function (users) {
                $liSalesTeam.attr("data-loaded", true);
                self.BindUsers(users);
                FFGlobal.docs.detail.content.addNewItemCard({
                    type: "user",
                    $wrapper: $companyUsers,
                    link: {
                        modalTarget: "addSalesTeamMemberDialog",
                        content: "Add Sales Team Member"
                    }
                });
                contactActionMenu();
            },
            beforeSend: function () {
                $companyUsers.html('<div class= "text-center data-loader"><img src="/_content/_img/loading_20.gif" /></span ></div>');
            },
            error: function (request) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.BindUsers = function (users) {
        $companyUsers.html("");
        if (users.length > 0) {
            $.each(users, function (i, userModel) {
                var user = userModel.User;
                FFGlobal.docs.detail.content.addContactOrUserCard({
                    type: 'user',
                    $wrapper: $companyUsers,
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
                        salesTeamRole: (userModel.LinkGlobalCompanyGlobalUser||{}).SalesTeamRole,
                        profile: {
                            pic: userModel.ProfilePicture,
                            title: user.Title,
                            location: user.LocationName,
                        }
                    },
                    modifyCallback: function(type) {
                        if (type === "delete") self.DeleteCompanyUser(user.UserId, user.SubscriberId);
                    }
                });
            });
        }
    };

    this.DeleteCompanyUser = function (deleteUserId, sid) {
        swal({
            title: translatePhrase("Remove Member?"),
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Remove"),
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/company/DeleteCompanyUser/?deleteUserId=" +
                        deleteUserId +
                        "&companyId=" +
                        companyId +
                        "&userId=" +
                        userId +
                        "&companySubscriberId=" +
                        companySubscriberId +
                        "&deleteUserSubscriberId=" +
                        sid,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data) self.LoadCompanyUsers();
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
            var companyUser = new Object();
            companyUser.UpdatedBy = userId;
            companyUser.CompanyId = companyId;
            companyUser.CompanySubscriberId = companySubscriberId;
            companyUser.GlobalUserId = $ddlSalesTeam.val();
            companyUser.SalesTeamRole = $ddlSalesTeamRole.val();
            $.ajax({
                type: "POST",
                url: "/api/company/addcompanyuser",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(companyUser),
                success: function () {
                    removeSpinner();
                    self.LoadCompanyUsers();
                    $addSalesTeamDialog.modal('toggle');
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
};


function initRelatedCompanyDropdown() {
    // set the selected company here
    // company dropdown
    $("#ddlRelatedCompany").select2({
        minimumInputLength: 2,
        allowClear: true,
        placeholder: translatePhrase("Select Company"),
        ajax: {
            url: function (obj) {
                if (!obj.term) {
                    obj.term = "";
                }
                return "/api/dropdown/GetGlobalCompanies?subscriberId=" + companySubscriberId + "&keyword=" + obj.term;
            },
            dataType: "json",
            timeout: 50000,
            type: "GET",
            data: '',
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: item.SelectText,
                            id: item.SelectValue
                        };
                    })
                };
            }
        }
    });
}


$(function () {

    $("#btnAddRelatedCompany").unbind("click").click(function () {
        if ($.isNumeric($("#ddlRelatedCompany").val()) && parseInt($("#ddlRelatedCompany").val()) > 0) {
            var request = new Object();
            request.CompanyId = companyId;
            var selectedLinkType = $("#ddlLinkType option:selected").text();
            request.LinkType = selectedLinkType;
            request.LinkedCompanyId = $("#ddlRelatedCompany").val();
            request.SubscriberId = companySubscriberId;
            request.UpdateUserId = userId;

            var url = "/api/Company/LinkCompany";

            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 100000,
                data: JSON.stringify(request),
                success: function (response) {
                    if (response) {
                        $('#relatedCompanySetup').modal('hide');
                        // reload the page
                        window.location.hash = "#relatedcompanies";
                        location.reload();
                        // location.href = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + companyId + "&subscriberId=" + companySubscriberId + "#relatedcompanies";
                    } else {
                        swal({ title: "Error linking the related company!", type: "warning", showCancelButton: false });
                    }
                    removeSpinner();
                }, beforeSend: function () {
                    addSpinner();
                }, error: function (request) {
                    alert(JSON.stringify(request));
                    removeSpinner();
                }
            });
        } else {
            swal({ title: "Select the company!", type: "warning", showCancelButton: false });
        }

    });

});

function RefreshEvents() {
    currentPage = 1;
    new Events().RetrieveEvents();
}


function RefreshDeals() {
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // do the following to get rid of the padding
    $('body').removeAttr('style');
    // load deals
    new Deals().GetCompanyDeals();
}


function RefreshContacts() {
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // do the following to get rid of the padding
    $('body').removeAttr('style');
    // load the contacts
    new Contacts().LoadCompanyContacts();
}


function GoToDealDetail(dealId) {
    // go to deal detail
    location.href = "/Deals/DealDetail/dealdetail.aspx?dealId=" +
        dealId +
        "&newlane=1&dealsubscriberId=" +
        $("#lblCompanySubscriberId").text();
}


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
            loadSalesStages();
            self.ToggleCardVsListVisibility();
        });

        new DealsCards().Init();

    };

    // toggle card/list and active/inactive actions
    this.ToggleCardVsListVisibility = function () {
        dataView = $('.showView a.icon.active').attr('data-view');
        activeInactiveStatus = $('.deal-btn-wrp a.deals-link.active').attr('data-status');

        if (dataView === 'card') {
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

            // TODO: render server side - different per subscriber
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
            filters.SubscriberId = companySubscriberId;
            filters.UserId = userId;
            filters.RecordsPerPage = 10;
            filters.CurrentPage = cardsCurrentPage;
            filters.SortOrder = "DealName asc";
            filters.SalesStages = [];
            filters.SalesStages.push(dealStage);
            filters.CompanyId = companyId;

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
                    // TODO: new spinner
                    bindLoadingMsg(translatePhrase("Loading deals") + "...", $dealsContainer, true);
                },
                error: function () { }
            });
        }
    };

    /*
    // set column height
    this.SetColumnHeight = function ($column) {
        var $dealsContainer = $column.find(".grid-wrap");
        var dealsCount = $dealsContainer.find(".grid-box").length;
        var colStyle = $column.attr("style");
        var gridBoxht = $dealsContainer.find(".grid-box").outerHeight(true);

        var height = gridBoxht * dealsCount;

        height = $(window).height() < 1024 ? height + 30 : height + 155;
        //$column.attr("style", colStyle + ";height:" + height + "px !important;");
    };
    */

    // bind deal boxes
    this.BindDeals = function (deals, $dealsContainer) {
        // iterate and bind deals
        // TODO: get all below html from server instead of parsing javascript
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
                        $this.find(".data-deal-revenue-spot").html(resposne.CurrencySymbol + formatNumber(parseInt(resposne.SpotRevenue)));
                        $this.find(".data-deal-profit-spot").html(resposne.CurrencySymbol + formatNumber(parseInt(resposne.SpotProfit)));
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
            $noDeals = '<div class="grid-box has-deal no-deal text-center"><p></p></div>';
            $dealsContainer.append($noDeals);
        } else {
            $dealsContainer.find(".no-deal").remove();
            // add droppable area
            $noDeals = '<div class="grid-box no-deal text-center"><p>No ' + activeInactiveStatus + ' Deals</p></div>';
            $dealsContainer.append($noDeals);
        }
    };

    // get contact profile picture
    // TODO: is this used??
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
        // AJAX to update the status
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
                    // Start monitoring tilt direction
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
    var $noItems = $(".no-deals");
    var $dealsContainer = $(".dealList");
    var currentPage = 1;

    this.Init = function () {
        // retrieve deals
        self.RetrieveDeals();

        // sales stage change event
        self.InitSalesStagesActions();

        // sort - table header click
        self.InitSort();
    };

    this.RetrieveDeals = function () {
        // set current body
        $currentBody = activeInactiveStatus === "active" ? $activeListBody : $inactiveListBody;

        // set filters
        var filters = new Object();
        filters.SubscriberId = companySubscriberId;
        filters.RecordsPerPage = recordsPerPage;
        filters.CurrentPage = currentPage;
        filters.SortBy = tableSortOrder;
        filters.UserId = userId;
        filters.CompanyId = companyId;

        // sales stages
        filters.SalesStages = [];
        var selectedStatus = $("#ddlSalesStage").val();

        if (activeInactiveStatus === 'active') {
            if ($ddlSalesStage.val() === "All") {
                $ddlSalesStage.find("option").each(function (i, selected) {
                    var salesStage = $(selected).val();
                    if (salesStage !== "All" && salesStage !== null) {
                        filters.SalesStages.push(salesStage);
                    }
                });
            } else if (selectedStatus !== null)
                filters.SalesStages.push(selectedStatus);
        } else {
            if ($ddlSalesStage.val() === "All") {
                $ddlSalesStage.find("option").each(function (i, selected) {
                    var salesStage = $(selected).val();
                    if (salesStage !== "All" && salesStage !== null) {
                        filters.SalesStages.push(salesStage);
                    }
                });
            } else if (selectedStatus !== null)
                filters.SalesStages.push(selectedStatus);
        }

        $activeDealList.addClass("hide");
        $inactiveDealList.addClass("hide");

        // clear the rows
        $activeListBody.html("");
        $inactiveListBody.html("");
        $(".pagination").addClass('hide');

        $.ajax({
            type: "POST",
            url: "/api/deal/getdeals",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(filters),
            success: function (response) {
                if (activeInactiveStatus === "active") {
                    $activeDealList.removeClass("hide");
                }
                else {
                    $inactiveDealList.removeClass("hide");
                }
                bindLoadingMsg("", $dealsContainer, false);
                // set the total records
                $(".dl-record").html(formatNumber(response.Records) + " records");

                if (response.Deals.length > 0) {
                    // bind deals
                    self.BindDeals(response.Deals);
                    // set pagination
                    //  self.SetPagination(response.TotalPages);  
                    $noItems.addClass('hide');
                }
                else {
                    // hide deals table
                    // $divTableContent.addClass('hide');
                    $noItems.removeClass('hide');
                }
            },
            beforeSend: function () {
                // TODO: new spinner
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
            $tdDealName.append($("<span/>", { "html": deal.DealType }));
        }
        $tr.append($tdDealName);

        // deal company
        var $tdCompany = $("<td />");
        var companyDetailUrl = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + deal.CompanyId + "&subscriberId=" + deal.SubscriberId;
        var $aCompanyName = $("<a />", { "href": companyDetailUrl, "class": "company", "html": deal.CompanyName });
        $tdCompany.append($aCompanyName);
        if (deal.PrimaryContactName && deal.PrimaryContactName !== '') {
            var $contactName = $("<span />", { "html": deal.PrimaryContactName, "data-primary-contact-id": deal.PrimaryContactId });
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
        var finalSalesTeamList = [];
        var $tdSalesTeam = $("<td />");
        if (deal.SalesRepName) {
            finalSalesTeamList.push(deal.SalesRepName + " <span class='saleowner'>Owner</span>");
        }

        var salesTeamUsers = deal.SalesTeam.split(",");
        $.each(salesTeamUsers, function (i, ele) {
            if ($.trim(ele) !== deal.SalesRepName) {
                //finalSalesTeamList.push(ele);
            }
        });
        $tdSalesTeam.append($("<div />", { "data-type": "sales-team", "html": finalSalesTeamList.join("<br />") }));

        $tr.append($tdSalesTeam);

        // sales stage
        var $tdSalesStage = $("<td />", { "data-sales-stage-id": deal.SalesStageId, "class": "text-center" });
        var $tdSalesStatus = $("<div />", { "class": "border-status", "html": deal.SalesStageName });
        $tdSalesStage.append($tdSalesStatus);
        switch (deal.SalesStageName) {
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
        if (activeInactiveStatus === "active") {
            // decision date
            if (deal.DecisionDate) {
                $tdDecisionDate.html(moment(deal.DecisionDate).format("DD-MMM-yy"));
            }
        } else {
            if ($ddlSalesStage.val() === "All") {
                $thDecisionDate.html("DECISION DATE");
                // decision date
                if (deal.DecisionDate) {
                    $tdDecisionDate.html(moment(deal.DecisionDate).format("DD-MMM-YY"));
                }
            } else {
                // won date
                if ($ddlSalesStage.val() === "Won") {
                    $thDecisionDate.html("WON DATE");
                    if (deal.DateWon) {
                        $tdDecisionDate.html(moment(deal.DateWon).format("DD-MMM-YY"));
                    }
                } else if ($ddlSalesStage.val() === "Lost") {
                    $thDecisionDate.html("LOST DATE");
                    if (deal.DateLost) {
                        $tdDecisionDate.html(moment(deal.DateLost).format("DD-MMM-YY"));
                    }
                } else if ($ddlSalesStage.val() === "Stalled") {
                    $thDecisionDate.html("STALLED DATE");
                    if (deal.LastUpdate) {
                        $tdDecisionDate.html(moment(deal.LastUpdate).format("DD-MMM-YY"));
                    }
                }
            }
        }
        $tr.append($tdDecisionDate);
        return $tr;
    };

    this.SetPagination = function (totalPages) {
        var visiblePages = 1;
        if (totalPages > 4) {
            visiblePages = 4;
        } else {
            visiblePages = totalPages;
        }

        $('.pagination').twbsPagination({
            totalPages: totalPages,
            visiblePages: visiblePages,
            onPageClick: function (event, page) {
                currentPage = page;
                self.RetrieveDeals();
            },
            first: "<<",
            prev: "<",
            next: ">",
            last: ">>"
        });
        if (totalPages > 1)
            $(".pagination").removeClass('hide');
    };

    this.InitSalesStagesActions = function () {
        // sales stage change event
        $("#ddlSalesStage").unbind("change").change(function () {
            if (!activeInactiveChanged) {
                self.RetrieveDeals();
            }
            activeInactiveChanged = false;
        });
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

// TODO Dropdown Menu was removed - remove unused functions

// onClick new options list of new select
$(document).ready(function () {
    //Header Desktop Dropdown Menu
    var deskLink = $('.desktop-header-dropdown .header-dropdown .dropdown-nav > li.selected').find('a').attr('href');
    $('.desktop-header-dropdown .header-dropdown .ae-select-content').text($('.desktop-header-dropdown .header-dropdown .dropdown-nav > li.selected').text()).attr('href', deskLink);
    var newOptions = $('.desktop-header-dropdown .header-dropdown .dropdown-nav > li');
    newOptions.click(function () {
        $('.desktop-header-dropdown .header-dropdown .ae-select-content').text($(this).text());
        $('.desktop-header-dropdown .header-dropdown .dropdown-nav > li').removeClass('selected');
        $(this).addClass('selected');
        $('.desktop-header-dropdown .header-dropdown .dropdown-nav').toggleClass('ae-hide');
        $('.desktop-header-dropdown .header-dropdown .ae-select').toggleClass('drop-open');
    });
    var aeDropdown = $('.desktop-header-dropdown .header-dropdown .ae-dropdown .drop-icon-down,.desktop-header-dropdown .header-dropdown .ae-dropdown .drop-icon-up');
    aeDropdown.click(function () {
        $('.desktop-header-dropdown .header-dropdown .dropdown-nav').toggleClass('ae-hide');
        $('.desktop-header-dropdown .header-dropdown .ae-select').toggleClass('drop-open');
    });

    //Header Mobile Dropdown Menu
    var mobLink = $('.desktop-header-dropdown .header-dropdown .dropdown-nav > li.selected').find('a').attr('href');
    $('.mobile-header-dropdown .header-dropdown .ae-select-content').text($('.mobile-header-dropdown .header-dropdown .dropdown-nav > li.selected').text()).attr('href', mobLink);
    newOptions = $('.mobile-header-dropdown .header-dropdown .dropdown-nav > li');
    newOptions.click(function () {
        $('.mobile-header-dropdown .header-dropdown .ae-select-content').text($(this).text());
        $('.mobile-header-dropdown .header-dropdown .dropdown-nav > li').removeClass('selected');
        $(this).addClass('selected');
        $('.mobile-header-dropdown .header-dropdown .dropdown-nav').toggleClass('ae-hide');
        $('.mobile-header-dropdown .header-dropdown .ae-select').toggleClass('drop-open');
    });
    aeDropdown = $('.mobile-header-dropdown .header-dropdown .ae-dropdown .drop-icon-down,.mobile-header-dropdown .header-dropdown .ae-dropdown .drop-icon-up');
    aeDropdown.click(function () {
        $('.mobile-header-dropdown .header-dropdown .dropdown-nav').toggleClass('ae-hide');
        $('.mobile-header-dropdown .header-dropdown .ae-select').toggleClass('drop-open');
    });

    //Sidebar Dropdown Menu
    $('.sidebar-dropdown .ae-select-content').text($('.sidebar-dropdown .dropdown-nav > li.selected').text());
    newOptions = $('.sidebar-dropdown .dropdown-nav > li');
    newOptions.click(function () {
        $('.sidebar-dropdown .ae-select-content').text($(this).text());
        $('.sidebar-dropdown .dropdown-nav > li').removeClass('selected');
        $(this).addClass('selected');
    });
    aeDropdown = $('.sidebar-dropdown .ae-dropdown');
    aeDropdown.click(function () {
        $('.sidebar-dropdown .dropdown-nav').toggleClass('ae-hide');
        $('.sidebar-dropdown .ae-select').toggleClass('drop-open');
    });

    //Panel Dropdown Menu
    $('.panel-dropdown .ae-select-content').text($('.panel-dropdown .dropdown-nav > li.selected').text());
    newOptions = $('.panel-dropdown .dropdown-nav > li');
    newOptions.click(function () {

        var dataType = $(this).attr("data-type");
        var isLoaded = $(this).attr("data-loaded");

        switch (dataType) {
            case "events":
                new Events().Init();
                break;
            case "tasks":
                new FFGlobal.docs.detail.tabs.tasks({
                    type: "company",
                    data: docUserData
                }).init();
                break;
            case "notes":
                // new Notes().Init();
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
            case "relatedcompanies":
                window.location.hash = "#relatedcompanies";
                break;
            default:
        }

        $('.panel-dropdown .ae-select-content').text($(this).text());
        $('.panel-dropdown .dropdown-nav > li').removeClass('selected');
        $(this).addClass('selected');
    });
    aeDropdown = $('.panel-dropdown .ae-dropdown');
    aeDropdown.click(function () {
        $('.panel-dropdown .dropdown-nav').toggleClass('ae-hide');
        $('.panel-dropdown .ae-select').toggleClass('drop-open');
    });
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

// Company : Contacts Action Menu
function contactActionMenu() {
    $('.cc-nav a.open-menu').click(function (event) {
        event.preventDefault();
        if (!$(this).hasClass('active')) {
            $(this).next('.cc-actions').stop(true, true).fadeIn(300);
            $(this).addClass('active');
        } else {
            $(this).next('.cc-actions').stop(true, true).fadeOut(300);
            $(this).removeClass('active');
        }
    });
}


// Deals : Active & Inactive
$(document).ready(function () {
    $('.deal-btn-wrp a.deals-link').click(function (e) {
        e.preventDefault();

        var status = $(this).attr('data-status');
        $('.tab-icon-wrp a.icon').attr('data-status', status);

        var view = $(this).attr('data-view');
        var id = $(this).attr('data-type-' + view);

        if (view === 'card') {
            $('.deal-cards').hide();
            $('.deal-blk ' + id).show();
        } else {
            $('.deal-toggle-tb').hide();
            $(id).show();
        }
        $(this).parent().find('a.deals-link').removeClass('active');
        $(this).addClass('active');
    });
});


// Deals : Cards & List View
$(document).ready(function () {
    $('.showView a.icon').click(function (e) {
        e.preventDefault();
        var status = $(this).attr('data-status');
        var view = $(this).attr('data-view');
        $('.deal-btn-wrp a.deals-link').attr('data-view', view);

        var id = $(this).attr('data-type-' + status);

        if (status === 'active') {
            $('.deal-toggle-tb,.deal-cards').hide();
            $(id).show();
        } else {
            $('.deal-cards,.deal-toggle-tb').hide();
            $(id).show();
        }

        $(this).parent().find('a.icon').removeClass('active');
        $(this).addClass('active');
    });
});


// load deal sales stages dropdown
function loadSalesStages() {
    activeInactiveStatus = $('.deal-btn-wrp a.deals-link.active').attr('data-status');
    $ddlSalesStage.html("");
    $ddlSalesStage.append($('<option>', { value: "All", text: "All" }));
    if (activeInactiveStatus === "active") {
        $.each(subscriberSalesStages, function (i, ele) {
            $ddlSalesStage.append($('<option>', { value: ele, text: ele }));
        });
    } else {
        $ddlSalesStage.append($('<option>', { value: "Won", text: "Won" }));
        $ddlSalesStage.append($('<option>', { value: "Lost", text: "Lost" }));
        $ddlSalesStage.append($('<option>', { value: "Stalled", text: "Stalled" }));
    }
    $ddlSalesStage.val("All").trigger('change');
}


var getQueryString = function (field, url) {
    var href = url ? url : window.location.href;
    var reg = new RegExp('[?&]' + field + '=([^&#]*)', 'i');
    var string = reg.exec(href);
    return string ? string[1] : null;
};

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
