var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var isAdmin = $("#lblIsAdmin").text() === "1";
var cardsCurrentPage = 1;
var tableCurrentPage = 1;
var tableSortOrder = "dealname asc";
var $dealsListHolder = $("#list-view");
var $dealsCardsHolder = $("#grid-view");
var $activeDealCards = $("#active-deal");
var $inactiveDealCards = $("#inactive-deal");
var $activeDealList = $("#deal-datatable");
var $inactiveDealList = $("#deal-inc-datatable");
var $ddlSalesStage = $('#ddlSalesStage');
var $divSalesStageDropdownContainer = $("#divSalesStageDropdownContainer");
var $divSalesRepDropdownContainer = $("#divSalesRepDropdownContainer");
var $divLocationDropdownContainer = $("#divLocationDropdownContainer");
var $divCountryDropdownContainer = $("#divCountryDropdownContainer");
var activeInactiveStatus = "active";
var dataView = "card";
var $ulDealStage = $(".btn-deal-stage");
var subscriberSalesStages = [];
var activeInactiveChanged = false;
var currentStatus = "";

$(document).ready(function () {
    // get susbcriber sales stages
    $("#ddlSalesStage > option").each(function () {
        if (this.text !== "All") {
            subscriberSalesStages.push(this.text);
        }
    });

    loadSalesStages();

    // add deal button
    $(".add-new-btn").find("a").attr("href", "/deals/dealaddedit/dealaddedit.aspx?dealId=0&dealsubscriberid=" + subscriberId + "&from=deallist");

    // deals : cards & list view
    $('.showView a.icon').unbind("click").click(function (e) {
        e.preventDefault();
        $(this).parent().find('a.icon').removeClass('active');
        $(this).addClass('active');
        toggleCardVsListVisibility();
    });

    $("#ddlSalesStage").select2({ placeholder: "Sales Stage", allowClear: true });
    $("#ddlSalesRep").select2({ placeholder: "Sales Rep", allowClear: true });
    $("#ddlLocation").select2({ placeholder: "Location", allowClear: true });
    $("#ddlCountry").select2({ placeholder: "Country", allowClear: true });

    // new deal
    $(".new-deal").unbind('click').click(function () {
        OpenAddEditDeal(0);
    });

    // search button
    $("#btnSearch").unbind("click").click(function () {
        LoadDeals();
    });

    $("#txtKeyword").on('keydown', function (e) {
        if (e.which === 13) {
            LoadDeals();
            return false;
        }
    });

    // load deals - check if card or table is user default using cookie
    var ffdealslisttype = getCookie("ffdealslisttype");

    // If no cookie set - default to card view
    if (ffdealslisttype === "") {
        ffdealslisttype = "card";
    }

    // default list type
    if (ffdealslisttype === "list") {
        $(".showView a[data-view='list']").click();
    } else {
        $(".showView a[data-view='card']").click();
    }
    $("#ddlSalesRep").change(function () {
        LoadDeals();
    });

    $("#ddlLocation").change(function () {
        LoadDeals();
    });

    $("#ddlCountry").change(function () {
        LoadDeals();
    });
});


// toggle card/list and active/inactive actions
function toggleCardVsListVisibility(selector) {
    selector = selector || null;
    $("#divNoItems").addClass('hide');
    dataView = $('.showView a.icon.active').attr('data-view');
    if (selector) activeInactiveStatus = $(selector + ' .selected').attr('data-status');
    else activeInactiveStatus = $('.filter-dropdown ul li.selected').attr('data-status');
    loadSalesStages();
    if (dataView === 'card') {
        $('.filter-list').show();
        $('.filter-dropdown').hide();
        $('.total-records').addClass('textCenter');

        setCookie("ffdealslisttype", "card", 1000);

        $divSalesStageDropdownContainer.addClass("hide");
        $dealsListHolder.addClass("hide");
        $dealsCardsHolder.removeClass("hide");
        if (activeInactiveStatus === "inactive" || activeInactiveStatus === "won" || activeInactiveStatus === "lost" || activeInactiveStatus === "stalled") {
            $inactiveDealCards.removeClass("hide");
            $activeDealCards.addClass("hide");
        } else {
            $activeDealCards.removeClass("hide");
            $inactiveDealCards.addClass("hide");
        }
    } else {
        $('.filter-list').hide();
        $('.filter-dropdown').show();
        $('.total-records').removeClass('textCenter');
        setCookie("ffdealslisttype", "list", 1000);

        $dealsCardsHolder.addClass("hide");
        $dealsListHolder.removeClass("hide");
        //  $divSalesStageDropdownContainer.removeClass("hide");
        $("#divSalesRepDropdownContainer").removeClass("hide");
        $("#divLocationDropdownContainer").removeClass("hide");
        $("#divCountryDropdownContainer").removeClass("hide");

        if (activeInactiveStatus === "inactive" || activeInactiveStatus === "won" || activeInactiveStatus === "lost" || activeInactiveStatus === "stalled") {
            $inactiveDealList.removeClass("hide");
            $activeDealList.addClass("hide");
        }
        else {
            $activeDealList.removeClass("hide");
            $inactiveDealList.addClass("hide");
        }
    }
    LoadDeals();
}


function loadSalesStages() {
    dataView = $('.showView a.icon.active').attr('data-view');
    var selectedStatus = '';
    if (dataView === "list") {
        selectedStatus = $('.filter-dropdown ul li.selected').attr('data-status');
        activeInactiveStatus = $('.filter-dropdown ul li.selected').attr('data-status');

        $ulDealStage.html("");
        var $liActive = $("<li class=\"active\" data-status=\"active\">Active</li>");
        if (activeInactiveStatus === "active")
            $liActive.addClass("selected");
        var $liInactive = $("<li class=\"inactive\" data-status=\"inactive\">Inactive</li>");
        if (activeInactiveStatus === "inactive")
            $liInactive.addClass("selected");

        $ulDealStage.append($liActive);
        $ulDealStage.append($liInactive);


    } else {
        selectedStatus = $('.filter-list a.selected').attr('data-status');
        activeInactiveStatus = $('.filter-list a.selected').attr('data-status');
    }


    if (activeInactiveStatus === "all") {
        if (currentStatus === "inactive" || currentStatus === "won" || currentStatus === "lost" || currentStatus === "stalled") {
            activeInactiveStatus = "inactive";
        } else {
            activeInactiveStatus = "active";
        }
    } else {
        if (activeInactiveStatus === "inactive" || activeInactiveStatus === "won" || activeInactiveStatus === "lost" || activeInactiveStatus === "stalled") {
            activeInactiveStatus = "inactive";
        } else {
            activeInactiveStatus = "active";
        }
    }


    if (dataView === "list") {
        $.each(subscriberSalesStages, function (i, ele) {
            var $li = $('<li>', { value: ele, text: ele, "data-status": ele });
            if (selectedStatus.toLowerCase() === ele.toLowerCase())
                $li.addClass("selected");
            $ulDealStage.append($li);
        });

        var $liWon = $('<li>', { value: "Won", text: "Won", "data-status": "won" });
        if (selectedStatus.toLowerCase() === "won") {
            $liWon.addClass("selected");
        }
        $ulDealStage.append($liWon);
        var $liLost = $('<li>', { value: "Lost", text: "Lost", "data-status": "lost" });
        if (selectedStatus.toLowerCase() === "lost") {
            $liLost.addClass("selected");
        }
        $ulDealStage.append($liLost);
        var $liStalled = $('<li>', { value: "Stalled", text: "Stalled", "data-status": "stalled" });
        if (selectedStatus.toLowerCase() === "stalled") {
            $liStalled.addClass("selected");
        }
        $ulDealStage.append($liStalled);

        // add All 
        var $liAll = $('<li>', { value: "All", text: "All", "data-status": "all" });
        if (selectedStatus.toLowerCase() === "all") {
            $liAll.addClass("selected");
        }
        $ulDealStage.append($liAll);

    } else {
        if (selectedStatus !== "inactive" && selectedStatus !== "active") {
            $liActive.addClass("selected");
        } else if (selectedStatus === "inactive") {
            $('.filter-list a').removeClass('selected');
            $('.filter-list a.inactive-deal').addClass('selected');
        } else if (selectedStatus === "active") {
            $('.filter-list a').removeClass('selected');
            $('.filter-list a.active-deal').addClass('selected');
        }
    }
    initDropdownChange();
}


function LoadDeals() {
    if (dataView === 'card') {
        cardsCurrentPage = 1;
        new DealsCards().Init();
    } else {
        tableCurrentPage = 1;
        new DealsTable().Init();
    }
}


var DealsCards = function () {
    var self = this;
    var $divCardView = $("#active-deal-card-view");
    var $cardViewContent = $divCardView.find('.row');

    // initialize card view
    this.Init = function () {
        self.SetInterface();
    };

    // set interface
    this.SetInterface = function () {
        $(".total-records").html("");

        var dealStages;
        if (activeInactiveStatus === "active") {
            dealStages = self.GetStages();
            $divCardView = $("#active-deal");
            $cardViewContent = $divCardView.find('.row');
        }
        else if (activeInactiveStatus === "all") {
            dealStages = self.GetStages();
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
        else if (activeInactiveStatus === "inactive") {
            dealStages = [];
            objWon = new Object();
            objWon.SelectText = "Won";
            dealStages.push(objWon);
            objLost = new Object();
            objLost.SelectText = "Lost";
            dealStages.push(objLost);
            objStalled = new Object();
            objStalled.SelectText = "Stalled";
            dealStages.push(objStalled);
            $divCardView = $("#inactive-deal");
            $cardViewContent = $divCardView.find('.row');
        }
        else if (activeInactiveStatus === "won") {
            dealStages = [];
            objWon = new Object();
            objWon.SelectText = "Won";
            dealStages.push(objWon);
            $divCardView = $("#inactive-deal");
            $cardViewContent = $divCardView.find('.row');
        }
        else if (activeInactiveStatus === "lost") {
            dealStages = [];
            objLost = new Object();
            objLost.SelectText = "Lost";
            dealStages.push(objLost);
            $divCardView = $("#inactive-deal");
            $cardViewContent = $divCardView.find('.row');
        }
        else if (activeInactiveStatus === "stalled") {
            dealStages = [];
            objStalled = new Object();
            objStalled.SelectText = "Stalled";
            dealStages.push(objStalled);
            $divCardView = $("#inactive-deal");
            $cardViewContent = $divCardView.find('.row');
        }

        $cardViewContent.html("");

        $.each(dealStages, function (i, ele) {

            var $divColumn = $("<div />", {
                "class": "col-xl-3 col-lg-6 col-md-6 col-3 deals-box column",
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
                // TODO: Subscriber Specific for Active Stages
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
        var $divCardView = $("#active-deal");

        if (activeInactiveStatus === "all") {
            $("#inactive-deal").addClass('scrollBox');
        } else {
            $("#inactive-deal").removeClass('scrollBox');
        }

        if (activeInactiveStatus === "all" || activeInactiveStatus === "inactive" || activeInactiveStatus === "won" || activeInactiveStatus === "lost" || activeInactiveStatus === "stalled") {
            $divCardView = $("#inactive-deal");
        }
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
            filters.SortOrder = "DealName asc";
            filters.Keyword = $("#txtKeyword").val();
            filters.SalesStages = [];
            filters.SalesStages.push(dealStage);
            if ($("#ddlSalesRep").val() !== '')
                filters.SalesRepId = $("#ddlSalesRep").val();

            if ($("#ddlLocation").val() !== '')
                filters.Location = $("#ddlLocation").val();

            if ($("#ddlCountry").val() !== '')
                filters.CountryName = $("#ddlCountry").val();

            $.ajax({
                type: "POST",
                url: "/api/deal/getdeals",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(filters),
                success: function (response) {
                    // remove loading message
                    // TODO: use new spinner
                    bindLoadingMsg("", $dealsContainer, false);
                    $dealsCount.html(response.Records.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                    $dealsCount.attr("deals-count", response.Records);
                    $column.attr("total-pages", response.TotalPages);
                    // bind deals
                    self.BindDeals(response.Deals, $dealsContainer);
                    //set column height
                    self.SetColumnHeight($column);
                    // initialize sortable
                    if (activeInactiveStatus === "active")
                        self.InitSortable();
                },
                beforeSend: function () {
                    //add loading message
                    // TODO: use new spinner
                    bindLoadingMsg(translatePhrase("Loading deals") + "...", $dealsContainer, true);
                },
                error: function () { }
            });
        }
    };

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

    // bind deal boxes
    this.BindDeals = function (deals, $dealsContainer) {
        if (deals.length > 0) {
            $.each(deals, function (i, deal) {
                FFGlobal.docs.detail.content.addDealCard({
                    $wrapper: $dealsContainer,
                    deal: deal
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

        var $divCardView = $("#active-deal");
        if (activeInactiveStatus === "inactive") {
            $divCardView = $("#inactive-deal");
        }

        var $columnNew = $divCardView.find(".column[data-stage='" + newStage + "']");
        var newDealsCount = parseInt($columnNew.find(".card-count").attr("deals-count"));
        var $columnOld = $divCardView.find(".column[data-stage='" + oldStage + "']");
        var oldDealsCount = parseInt($columnOld.find(".card-count").attr("deals-count"));

        // set old deals count
        $columnOld.find(".card-count").html((oldDealsCount - 1));
        $columnOld.find(".card-count").attr("deals-count", (oldDealsCount - 1));

        // set new deals count
        $columnNew.find(".card-count").html((newDealsCount + 1));
        $columnNew.find(".card-count").attr("deals-count", (newDealsCount + 1));

        //set column height
        self.SetColumnHeight($columnNew);
        self.SetColumnHeight($columnOld);

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

                var $dealsCount = $colHeader.find(".card-count");
                $dealsCount.html("");

                //set old values again
                $columnOld.find(".card-count").html(oldDealsCount);
                $columnOld.find(".card-count").attr("deals-count", oldDealsCount);

                // set new deals count
                $columnNew.find(".card-count").html(newDealsCount);
                $columnNew.find(".card-count").attr("deals-count", newDealsCount);
            }
        });
    };

    // initialize sortable
    this.InitSortable = function () {
        var oldStage = "";
        var newStage = "";
        var element = "[class*=grid-box]";
        var handle = "div.grid-box:not(.empty-box)";
        var connect = "[class*=grid-wrap]";
        var $oldDealContaner;
        var $newDealContaner;

        $(".grid-wrap").sortable(
            {
                handle: handle,
                connectWith: connect,
                tolerance: 'pointer',
                forcePlaceholderSize: true,
                start: function (event, ui) {
                    ui.item.addClass('tilt');
                    // monitor tilt direction
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
                    // add an empty box for a deal card
                    new DealsCards().AddEmptyDroppableArea($oldDealContaner);
                    new DealsCards().AddEmptyDroppableArea($newDealContaner);
                    // update deal stage
                    self.UpdateDealSalesStage(dealId, newStage, oldStage);
                }
            })
            .disableSelection();
    };

    // monitor tilt direction and switch between classes accordingly
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
    var $noItems = $("#divNoItems");
    var $dealsWrapper = $("#list-view");

    this.Init = function () {
        // retrieve deals
        self.RetrieveDeals();
        // sales stage change event
        self.InitSalesStagesActions();
    };

    this.RetrieveDeals = function () {
        // set filters
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.RecordsPerPage = 20;
        filters.CurrentPage = tableCurrentPage;
        filters.SortBy = tableSortOrder;
        filters.Keyword = $("#txtKeyword").val();
        filters.UserId = userId;
        if ($("#ddlSalesRep").val() !== '')
            filters.SalesRepId = $("#ddlSalesRep").val();

        if ($("#ddlLocation").val() !== '')
            filters.Location = $("#ddlLocation").val();

        if ($("#ddlCountry").val() !== '')
            filters.CountryName = $("#ddlCountry").val();


        // var selectedStatus = $("#ddlSalesStage").val();
        filters.SalesStages = [];
        // sales stages
        var selectedStatus = $('.filter-dropdown ul li.selected').attr('data-status').toLowerCase();
        currentStatus = selectedStatus;
        switch (selectedStatus) {
            case "active":
                $.each(subscriberSalesStages, function (i, ele) {
                    filters.SalesStages.push(ele.toLowerCase());
                });
                break;
            case "inactive":
                filters.SalesStages.push("won");
                filters.SalesStages.push("lost");
                filters.SalesStages.push("stalled");
                break;
            case "all": break;
            default:
                filters.SalesStages.push(selectedStatus);
        }

        //// if active include all the stages
        //if (activeInactiveStatus === 'active') {
        //    if ($ddlSalesStage.val() === "All") {
        //        $ddlSalesStage.find("option").each(function (i, selected) {
        //            var salesStage = $(selected).val();
        //            if (salesStage !== "All") {
        //                filters.SalesStages.push(salesStage);
        //            }
        //        });
        //    } else
        //        filters.SalesStages.push(selectedStatus);
        //} else {
        //    if ($ddlSalesStage.val() === "All") {
        //        $ddlSalesStage.find("option").each(function (i, selected) {
        //            var salesStage = $(selected).val();
        //            if (salesStage !== "All") {
        //                filters.SalesStages.push(salesStage);
        //            }
        //        });
        //    } else
        //        filters.SalesStages.push(selectedStatus);
        //}

        if (activeInactiveStatus === "inactive" || activeInactiveStatus === "won" || activeInactiveStatus === "lost" || activeInactiveStatus === "stalled") {
            $inactiveDealList.removeClass("hide"); // set current body
            $currentBody = $inactiveListBody;

        } else {
            $currentBody = $activeListBody;
            $activeDealList.removeClass("hide");

        }

        // clear table rows
        if (tableCurrentPage === 1) {
            $activeListBody.html("");
            $inactiveListBody.html("");
        }

        $.ajax({
            type: "POST",
            url: "/api/deal/getdeals",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(filters),
            success: function (response) {
                if (activeInactiveStatus === "inactive" || activeInactiveStatus === "won" || activeInactiveStatus === "lost" || activeInactiveStatus === "stalled") {
                    $inactiveDealList.attr("total-pages", response.TotalPages);
                } else {
                    $activeDealList.attr("total-pages", response.TotalPages);
                }
                $(".total-records").html(formatNumber(response.Records));
                if (response.Deals.length > 0) {
                    self.BindDeals(response.Deals);
                    self.InitSort();
                    $dealsWrapper.removeClass('hide');
                    $noItems.addClass('hide');
                } else {
                    $dealsWrapper.addClass('hide');
                    $noItems.removeClass('hide');
                }
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
        var $cityName = $("<div />", { "class": "company", "html": deal.CityName });
        var $countryName = $("<span />", { "html": deal.CountryName });
        $tdCountry.append($cityName);
        $tdCountry.append($countryName);
        $tr.append($tdCountry);

        var salesTeamUsers = deal.SalesTeam.split(",");

        // sales team 
        var finalSalesTeamList = [];
        var tempSalesTeamList = [];

        $.each(salesTeamUsers, function (i, ele) {
            if ($.trim(ele) !== deal.SalesRepName) {
                tempSalesTeamList.push(ele);
            }
        });

        var $tdSalesTeam = $("<td />");
        if (deal.SalesRepName) {
            if (tempSalesTeamList.length === 0) {
                finalSalesTeamList.push(deal.SalesRepName + "<br /> <span class='saleowner'>Owner</span>");
            } else {
                finalSalesTeamList.push(deal.SalesRepName + " <span class='saleowner'>Owner</span>");
            }
        }

        $.each(salesTeamUsers, function (i, ele) {
            if ($.trim(ele) !== deal.SalesRepName) {
                finalSalesTeamList.push(ele);
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
                $tdDecisionDate.html(moment(deal.DecisionDate).format("DD-MMM-YY"));
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

    this.InitSalesStagesActions = function () {
        var salesStageEventName = "change.salesStage";
        // sales stage change event
        $("#ddlSalesStage").unbind(salesStageEventName).bind(salesStageEventName, function () {
            if (!activeInactiveChanged) {
                self.RetrieveDeals();
            }
            activeInactiveChanged = false;
        });
    };

    this.InitSort = function () {
        var $tbl = activeInactiveStatus === "active" ? $activeDealList : $inactiveDealList;

        $tbl.find("th").unbind("click").click(function () {
            var $this = $(this);
            var currentSortOrder = "";
            var sortFieldName = $this.attr("data-field-name");
            if (sortFieldName && sortFieldName !== '') {
                // check if already any sort going on
                var $sortitem = $this.find(".sort");
                if ($sortitem && $sortitem !== null) {
                    // already sorting using this field - check if ASC or DESC
                    currentSortOrder = $sortitem.closest("th").attr("data-sort-order");
                    currentSortOrder = currentSortOrder === "asc" ? "desc" : "asc";
                    tableSortOrder = sortFieldName + " " + currentSortOrder;
                } else {
                    // NOT sorting using this field - use ASC
                    tableSortOrder = sortFieldName + " " + currentSortOrder;
                }
                // remove current sort up/down icons
                $tbl.find(".sort").remove();
                if (currentSortOrder === "asc")
                    $this.append('<i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i>');
                else 
                    $this.append('<i class="sort icon-Descending"><span class="path1"></span><span class="path2"></span></i>');

                $this.attr("data-sort-order", currentSortOrder);

                // do the search again
                tableCurrentPage = 1;
                self.RetrieveDeals();
            }
        });
    };
};


// when scroll bar hits the bottom reload more deals
function initScrollLoader() {

    if (dataView === 'card') {
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
                    new DealsCards().RetrieveStageDeals(stage);
                }
            });
        }
    } else {
        nearToBottom = 100;
        if ($(window).scrollTop() + $(window).height() > $(document).height() - nearToBottom) {
            tableCurrentPage = tableCurrentPage + 1;
            // total pages
            var totalPages = 1;
            if (activeInactiveStatus === "active") {
                totalPages = parseInt($activeDealList.attr("total-pages"));
            } else {
                totalPages = parseInt($inactiveDealList.attr("total-pages"));
            }
            if (tableCurrentPage <= totalPages) {
                new DealsTable().RetrieveDeals();
            }
        }
    }
}


function DeleteDeal(dealId, $ele) {
    swal({
        title: translatePhrase("Delete Deal!"),
        text: translatePhrase("Are you sure you want to delete this deal?"),
        type: "error",
        showCancelButton: true,
        confirmButtonColor: "#f27474",
        confirmButtonText: translatePhrase("Yes, Delete!")
    }, function () {
        $.ajax({
            type: "GET",
            url: "/api/deal/DeleteDeal/?dealId=" + dealId + "&userId=" + userId,
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                // TODO: use new spinner
                swal.close();
                if (data) {
                    $ele.fadeOut();
                }
            }, beforeSend: function () {
                // TODO: use new spinner
                swal({ text: translatePhrase("Deleting Deal") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: true });
            }
        });
    });
}


function OpenAddEditDeal(dealId, dealName, dealsubscriberId) {
    location.href = "/Deals/DealAdd/DealAdd.aspx?dealId=" + dealId + "&dealsubscriberId=" + dealsubscriberId;
    return;
}


function RefreshParent() {
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // get rid of excess padding
    $('body').removeAttr('style');
    // reload the deal
    LoadDeals();
}


function GoToDealDetail(dealId) {
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // get rid of excess padding
    $('body').removeAttr('style');
    // go to deal detail
    location.href = "/Deals/DealDetail/dealdetail.aspx?dealId=" + dealId + "&newlane=1";
}

$(window).resize(function () {
    var $divCardView = $("#active-deal");
    if (activeInactiveStatus === "inactive") {
        $divCardView = $("#inactive-deal");
    }
    var colCount = $divCardView.find(".column").length;
    // change column width
    var widthPercentage = $(window).width() < 1024 ? 100 : 100 / colCount;
    $divCardView.find(".column").css("width", widthPercentage + "%");
});

$(window).scroll(function () {
    initScrollLoader();
});

// active & inactive toggles
$('.filter-list a.task-btn').unbind("click").click(function (e) {
    e.preventDefault();
    $(this).parent().find('a.task-btn').addClass('selected');
    $(this).removeClass('selected');
    // load sales stages on active/inactive change
    activeInactiveChanged = true;
    loadSalesStages();
    toggleCardVsListVisibility('.filter-list');
});


function bindLoadingMsg(msg, $parent, binsert, topMargin) {
    //delete existing
    $parent.find(".loading-msg").remove();

    if (binsert) {
        //create loading
        // TODO: use new spinner
        var $loading = $('<div class="loading-msg text-center"></div>');
        var $spinner = spinkit.getSpinner(spinkit.spinerTypes.fadingCircle);
        $spinner.css("margin-top", topMargin && topMargin !== "" ? topMargin : "10px");
        $loading.append($spinner);
        $loading.append($("<div class='loading-msg m-t-xs'>" + msg + "</div>"));
        $loading.appendTo($parent);
    }
}

// onClick options list 
$(document).ready(function () {
    initDropdownChange();
});


function initDropdownChange() {
    $('.panel-dropdown .ae-select-content').html($('.panel-dropdown .dropdown-nav > li.selected').html());
    var newOptions = $('.panel-dropdown .dropdown-nav > li');
    newOptions.unbind("click").click(function () {
        var getClass = $(this).attr('data-status');
        getClass = getClass.toLowerCase().replace(/\s/g, '');
        $('.panel-dropdown .ae-select-content').html($(this).html()).removeClass().addClass('ae-select-content ' + getClass);
        $('.panel-dropdown .dropdown-nav > li').removeClass('selected');
        $(this).addClass('selected');
        activeInactiveChanged = true;
        loadSalesStages();
        toggleCardVsListVisibility();
    });
    var aeDropdown = $('.panel-dropdown .ae-dropdown');
    aeDropdown.unbind("click").click(function () {
        $('.panel-dropdown .dropdown-nav').toggleClass('ae-hide');
        $('.panel-dropdown .ae-select').toggleClass('drop-open');
    });

    $(document).mouseup(function (e) {
        var container = $(".filter-dropdown");
        // if target of the click isn't the container / child
        if (!container.is(e.target) && container.has(e.target).length === 0) {
            if ($('.panel-dropdown .ae-select').hasClass('drop-open'))
                $('.panel-dropdown .ae-dropdown').trigger('click');
        }
    });

    $('a.dotsbtn').click(function () {
        if ($(this).hasClass('active')) {
            $('#moreFilters').hide();
            $(this).removeClass('active');
            $('header.globalHead').removeClass('extend');
        }
        else {
            $('#moreFilters').show();
            $(this).addClass('active');
            if ($(window).width() <= 1570) {
                $('header.globalHead').addClass('extend');
            }
        }
    });
}
