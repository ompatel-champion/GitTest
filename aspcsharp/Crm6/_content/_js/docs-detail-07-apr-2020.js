// *** First Freight CRM - docs specific methods ***

// detail - applies to detail pages (DealDetail, ContactDetail, CompanyDetail, etc...)
FFGlobal.docs.detail = new _doc(FFGlobal.docs); function _doc(parent) {

    var self = this;

    //detail tabs (events, tasks, notes, etc...)
    //methods for each tab type are added/prototyped by the tab user control
    self.tabs = new _tabs(self); function _tabs(parent) {
        var self = this;
    }

    //content methods (add / edit html content)
    self.content = new _content(self); function _content(parent) {

        var self = this;

        //add new item card
        self.addNewItemCard = function (props) {
            props = props || {};
            var link = props.link || {};
            var ih = "";
            ih = '<div class="col-md-6 col-sm-12 col-12 add-contact-box contact-item" >';
                ih += '<div class="ac-wrapper basic-card contact-card" >';
                    ih += '<div class="btn-wrap">';
                        ih += '<a ' + (link.modalTarget ? 'data-toggle="modal" data-target="#' + link.modalTarget + '" ' : '');
                        ih += link.href ? 'href="' + link.href + '" ' : '';
                        ih += 'class="primary-btn">' + (link.content || '') + "</a>";
                    ih += '</div>';
                 ih += '</div>';
            ih += '</div>';
            props.$wrapper.append($(ih));
        }

        //add contact or user card
        self.addContactOrUserCard = function (props) {
            props = props || {};
            var data = props.data;
            //container and inner wrappers
            var $userContainer = $("<div/>", { "class": "col-md-6 col-sm-12 col-12 contact-item", "data-id": data.id });
            var $uprofileWrp = $("<div/>", { "class": "basic-card contact-card" });
            $userContainer.append($uprofileWrp);
            var $uprofile = $("<div/>", { "class": "cc-wrapper" });
            $uprofileWrp.append($uprofile);
            var $upwrp = $("<div/>", { "class": "cc-content row no-gutters" });
            $uprofile.append($upwrp);

            //add and append content
            _add_profile_pic();
            _add_info();
            _add_modify_links();
            props.$wrapper.append($userContainer);

            //add profile pic or initials
            function _add_profile_pic() {
                var $userimg = $("<div/>", { "class": "cc-img col-auto" });
                FFGlobal.profile.pic.create({
                    $wrapper: $userimg,
                    imageURL: (data.profile.pic || {}).DocumentUrl,
                    initials: {
                        name: data.name,
                        seperator: '',
                        colorIndex: data.id
                    }
                });
                $upwrp.append($userimg);
            }

            //add user and contact info (middle)
            function _add_info() {
                var $infoWrapper = $("<div/>", { "class": "cc-info col" });
                _add_name();
                _add_profile_details();
                _add_contact_details();
                _add_sales_role_flag();
                $upwrp.append($infoWrapper);

                //name and optional owner status
                function _add_name() {
                    var $wrapper = $("<div/>", { "class": "cc-name" });
                    if (props.type === "user") {
                        var $contactName = $("<span/>", { "class": "title-blue", "html": data.name });
                        $wrapper.append($contactName);
                        if (data.isOwner) $wrapper.append($("<span/>", { "class": "flag-owner", "html": " - Owner" }))
                    }
                    else {
                          $contactName = $("<a/>", {
                            "class": "hover-link",
                            "href": "/Contacts/ContactDetail/ContactDetail.aspx?contactId=" + data.id + "&subscriberid=" + data.subscriberId,
                            "html": data.name
                        });
                        $wrapper.append($contactName);
                    }
                    $infoWrapper.append($wrapper);
                }

                //profile details (job title - email - contact type)
                function _add_profile_details() {
                    var $wrapper = $("<div/>", { "class": "cc-profile" });
                    var isNonNumeric = FFGlobal.utils.string.isNonNumeric;
                    var hasItem = false;
                    if (isNonNumeric(data.profile.title)) {
                        var $title = $("<span/>", { "class": "cc-title", "html": data.profile.title });
                        $wrapper.append($title);
                        hasItem = true;
                    }
                    //data.profile.decisionRole = "Decision Role";
                    if (isNonNumeric(data.profile.decisionRole)) {
                        if (hasItem) _add_divider();
                        var $role = $("<span/>", { "class": "cc-skill", "html": data.profile.decisionRole });
                        $wrapper.append($role);
                        hasItem = true;
                    }
                    if (isNonNumeric(data.profile.type)) {
                        if (hasItem) _add_divider();
                        var $contactType = $("<span/>", { "class": "cc-type", "html": data.profile.type });
                        $wrapper.append($contactType);
                        hasItem = true;
                    }
                    if (isNonNumeric(data.profile.location)) {
                        if (hasItem) _add_divider();
                        var $location = $("<span/>", { "class": "cc-skill", "html": data.profile.location });
                        $wrapper.append($location);
                        hasItem = true;
                    }
                    if (isNonNumeric(data.email)) {
                        if (hasItem) _add_divider();
                        var $email = $("<a/>", {
                            "class": "cc-mail cc-item",
                            "href": "mailto:" + data.email,
                            "html": FFGlobal.utils.html.addWordBreakOpportunities(data.email)
                        });
                        $wrapper.append($email);
                    }
                    $infoWrapper.append($wrapper);

                    //add divider
                    function _add_divider() {
                        $wrapper.append($("<span/>", {
                            class: "cc-divider",
                            html: '&nbsp;&nbsp;<span class="line"></span>&nbsp;&nbsp;'
                        }));
                    }
                }

                //add contact details (address / phone)
                function _add_contact_details() {
                    var $wrapper = $("<div/>", { "class": "cc-address" });
                    _add_address();
                    _add_phone_numbers();
                    $infoWrapper.append($wrapper);

                    //add the address
                    function _add_address() {
                        if (data.address || data.city || data.country) {
                            var $useradd = $("<div/>", { "class": "row-icon address-icon" });
                            $wrapper.append($useradd);
                            if (data.address) {
                                $useradd.append($("<span/>", { "class": "", "html": data.address + ", " }));
                            }
                            if (data.city) {
                                $useradd.append($("<span/>", { "class": "", "html": data.city }));
                                if (data.country)
                                    $useradd.append($("<span/>", { "class": "", "html": ", " }));
                            }
                            if (data.country) {
                                $useradd.append($("<span/>", { "class": "", "html": data.country }));
                            }
                            $wrapper.append($useradd);
                        }
                    }

                    //add the phone numbers (business / mobile)
                    function _add_phone_numbers() {
                        if (data.phone || data.mobilePhone) {
                            var $phoneWrapper = $("<div/>", { class: "row no-gutters" });
                            if (data.phone) {
                                var $phone = $("<div/>", {
                                    class: "col-auto row-icon phone-icon",
                                    html: data.phone
                                });
                                $phoneWrapper.append($phone);
                            }
                            if (data.mobilePhone && data.mobilePhone !== data.phone) {
                                var $mobile = $("<div/>", {
                                    class: "col-auto image-icon mobile-icon ml-3",
                                    html: data.mobilePhone
                                });
                                $phoneWrapper.append($mobile);
                            }
                            $wrapper.append($phoneWrapper);
                        }
                    }
                }

                //add the sales role flag
                function _add_sales_role_flag() {
                    if (props.type === "user" && data.salesTeamRole) {
                        var $flag = $("<div/>", {
                            "class": "flag-role",
                            "html": data.salesTeamRole
                        });
                        $infoWrapper.append($flag);
                    }
                }
            }

            //edit and remove links (upper right corner)
            function _add_modify_links() {
                var $wrapper = $("<div/>", { "class": "cc-nav" });
                if (props.type === "user") {
                    if (!props.data.isOwner) {
                        var editLink = (props.links||{}).edit; if (editLink) {
                            var $edit = $("<a/>", {
                                "href": "javascript:void(0)",
                                "class": "del-cont",
                                "data-toggle": "modal",
                                "data-target": "#" + editLink.modalTarget,
                                "data-modal-props": _get_modal_props(),
                                "html": "edit"
                            });
                            $edit.unbind("click").click(function () {
                                if (props.modifyCallback) props.modifyCallback('edit');
                            });
                            $wrapper.append($edit);
                            $wrapper.append(" - ");
                        }
                    }
                    var $remove = $("<a/>", {
                        "href": "javascript:void(0)",
                        "class": "del-cont",
                        "html": "remove"
                    });
                    $remove.unbind("click").click(function () {
                        if (props.modifyCallback) props.modifyCallback('delete');
                    });
                    $wrapper.append($remove);
                }
                else {
                    var $link = $("<a/>", {
                        "href": '/Contacts/ContactAddEdit/ContactAddEdit.aspx?contactId=' + data.id + '&subscriberid=' + data.subscriberId,
                        "class": "edit-cont",
                        "html": "edit"
                    });
                    $wrapper.append($link);
                    if (!props.hideRemove) {
                        $wrapper.append(" - ");    
                        var $remove = $("<a/>", {
                            "href": "javascript:void(0)",
                            "class": "del-cont",
                            "html": "remove"
                        });
                        $remove.unbind("click").click(function () {
                            if (props.modifyCallback) props.modifyCallback('delete');
                        });
                        $wrapper.append($remove);
                    }
                }
                $uprofile.append($wrapper)

                function _get_modal_props() {
                    return JSON.stringify({
                        type: 'edit',
                        userType: props.type,
                        id: data.userIdGlobal,
                        name: data.name,
                        role: data.salesTeamRole
                    });
                }
            }
        }

        //adds an event list item to the provided wrapper (props.$wrapper)
        self.addEventListItem = function (props) {
            var event = props.event;
            var $eventItem = $("<div/>", { "class": "event-wrap", "data-event-id": event.ActivityId });
            // delete event link
            var $aDelete = $("<a/>", { "href": "javascript:void(0)", "class": "delete-link pull-right", "data-action": "delete-event" });
            $aDelete.append($("<i/>", { "class": "icon-Delete" }));
            $aDelete.unbind("click").click(function () {
                if (props.deleteCallback) props.deleteCallback({
                    activityId: event.ActivityId,
                    $eventItem: $eventItem
                });
            });
            $eventItem.append($aDelete);
            // title
            var $titleWrap = $("<div/>", { "class": "event-title" });
            $eventItem.append($titleWrap);
            var $aEventSubject = $('<a href="javascript:void(0)" class="hover-link">' + event.Subject + '</a>');
            $aEventSubject.attr("data-action", "edit-event");
            $aEventSubject.attr("event-id", event.ActivityId);
            $aEventSubject.attr("event-subscriber-id", event.SubscriberId);
            $titleWrap.append($aEventSubject);
            var $detailRow = $("<div/>", { "class": "row" });
            $eventItem.append($detailRow);
            var $leftWrap = $("<div/>", { "class": "col-md-4 event-info" });
            $detailRow.append($leftWrap);
            // detail time wrap
            var $detailTimeWrp = $("<div/>", { "class": "ev-datetime" });
            $leftWrap.append($detailTimeWrp);
            // date
            var $detailRowDate = $("<div/>", { "class": "ev-date", "html": moment(event.StartDateTime).format("ddd, DD MMMM, YYYY") });
            $detailTimeWrp.append($detailRowDate);
            // time
            var $detailRowTime = $("<div/>", { "class": "ev-time", "html": moment(event.StartDateTime).format("HH:mm A") });
            $detailTimeWrp.append($detailRowTime);
            // detail location wrap
            var $detailLocWrp = $("<div/>", { "class": "ev-loc" });
            $leftWrap.append($detailLocWrp);
            // location
            if (event.Location && event.Location !== "") {
                var $detailLocation = $("<div/>", { "class": "ev-adds", "html": event.Location });
                $detailLocWrp.append($detailLocation);
            }
            // event type
            var $detailRowType = $("<div/>", { "class": "ev-type", "html": event.CategoryName });
            $detailLocWrp.append($detailRowType);
            // mid column
            var $midWrap = $("<div/>", { "class": "col-md-4 event-info midcol" });
            $detailRow.append($midWrap);
            // mid column - description
            if (event.Description && event.Description !== "") {
                var $midEventDesCol = $("<div/>", { "class": "ev-text", "html": event.Description });
                $midWrap.append($midEventDesCol);
            }
            // right column
            var $rightWrap = $("<div/>", { "class": "col-md-4 event-info" });
            $detailRow.append($rightWrap);
            // right column - guests
            if (event.Invites && event.Invites !== "") {
                var $EventGuestWrp = $("<div/>", { "class": "ev-guests" });
                $rightWrap.append($EventGuestWrp);

                var $eventGuest = $("<div/>", { "class": "ev-guest", "html": event.Invites });
                $EventGuestWrp.append($eventGuest);
            }
            props.$wrapper.append($eventItem);
        }

        //adds deal overview item content to the provided wrapper (props.$wrapper)
        self.addDealOverviewItem = function (props) {
            var deal = props.deal;
            var dealDetailUrl = "/Deals/DealDetail/dealdetail.aspx?dealId=" + deal.DealId + "&dealsubscriberId=" + deal.SubscriberId;
            var revenue = formatNumber(parseInt(deal.RevenueUSD));
            var profit = formatNumber(parseInt(deal.ProfitUSD));
            var revenueSpot = formatNumber(parseInt(deal.RevenueUSDSpot));
            var profitSpot = formatNumber(parseInt(deal.ProfitUSDSpot));
            var StatusClass = "";
            var SalesStageName = deal.SalesStageName.trim();

            if (SalesStageName === "Qualifying") {
                StatusClass = "lblue";
            } else if (SalesStageName === "Negotiation") {
                StatusClass = "mblue";
            } else if (SalesStageName === "Final Negotiation") {
                StatusClass = "";
            } else if (SalesStageName === "Trial Shipment") {
                StatusClass = "green";
            } else if (SalesStageName === "Lost") {
                StatusClass = "red";
            } else if (SalesStageName === "Won") {
                StatusClass = "green";
            } else if (SalesStageName === "Stalled") {
                StatusClass = "grey";
            }

            var dealItem = '<div class="outer-wrp" data-id="' + deal.DealId + '">';
            dealItem += '      <div class="title-wrp">';
            dealItem += '        <div class="ibox-title clearfix">';
            dealItem += '          <div class="it-holder FL">';
            dealItem += '               <a href="' + dealDetailUrl + '" class="hover-link mcard-title">' + deal.DealName + '</a>';
            dealItem += '               <div class="mcard-text">RFQ - Quote</div>';
            dealItem += '          </div>';
            dealItem += '          <div class="button-wrp FR">';
            dealItem += '              <a href="' + dealDetailUrl + '" target="_blank" class="border-status ' + StatusClass + '">' + deal.SalesStageName.toUpperCase() + '</a>';
            dealItem += '          </div>';
            dealItem += '        </div>';
            dealItem += '      </div>';
            dealItem += '      <div class="table-deals">';
            dealItem += '          <table class="table">';
            dealItem += '             <thead>';
            dealItem += '                <tr>';
            dealItem += '                    <th colspan="2">KEY DATES</th>';
            // dealItem += '                    <th colspan="2">VOLUMES</th>';
            dealItem += '                    <th colspan="2">FINANCIALS</th>';
            dealItem += '                </tr>';
            dealItem += '             </thead>';
            dealItem += '             <tbody>';
            dealItem += '                <tr>';
            dealItem += '                   <td class="BR0">Decision</td>';
            dealItem += '                   <td class="TAR">' + moment(deal.DecisionDate).format("DD-MMM-YY") + '</td>';
            dealItem += '                   <td class="BR0"></td>';
            //   dealItem += '                   <td>TEUs</td>';
            dealItem += '                   <td class="BR0">Revenue</td>';
            dealItem += '                   <td class="BR0 TAR data-deal-revenue">USD ' + revenue + '</td>';
            dealItem += '                </tr>';
            dealItem += '                <tr>';
            dealItem += '                   <td class="BR0">Proposal</td>';
            dealItem += '                 	<td class="TAR">' + moment(deal.DateProposalDue).format("DD-MMM-YY") + '</td>';
            dealItem += '                   <td class="BR0"></td>';
            //  dealItem += '                   <td></td>';
            dealItem += '                   <td class="BR0">Profit</td>';
            dealItem += '                   <td class="BR0 TAR data-deal-profit">USD ' + profit + '</td>';
            dealItem += '                </tr>';

            if (deal.RevenueUSDSpot > 0 || deal.profitUSDSpot) {
                dealItem += '                <tr>';
                dealItem += '                   <td class="BR0"></td>';
                dealItem += '                 	<td class="TAR"></td>';
                dealItem += '                   <td class="BR0"></td>';
                // dealItem += '                   <td></td>';
                dealItem += '                   <td class="BR0">Revenue Spot</td>';
                dealItem += '                   <td class="BR0 TAR data-deal-revenue-spot">USD ' + revenueSpot + '</td>';
                dealItem += '                </tr>';
                dealItem += '                <tr>';
                dealItem += '                   <td class="BR0"></td>';
                dealItem += '                 	<td class="TAR"></td>';
                dealItem += '                   <td class="BR0"></td>';
                // dealItem += '                   <td></td>';
                dealItem += '                   <td class="BR0">Profit Spot</td>';
                dealItem += '                   <td class="BR0 TAR data-deal-profit-spot">USD ' + profitSpot + '</td>';
                dealItem += '                </tr>';
            }

            dealItem += '            </tbody>';
            dealItem += '         </table>';
            dealItem += '     </div>';
            dealItem += ' </div>';
            props.$wrapper.append($(dealItem));
        }

        //adds deal card content to the provided wrapper (props.$wrapper)
        self.addDealCard = function (props) {
            var deal = props.deal;
            var delaDetailLink = "/Deals/DealDetail/dealdetail.aspx?dealId=" +
                deal.DealId +
                "&dealsubscriberId=" +
                deal.SubscriberId;
            var $divDealContent = $("<div />", { "class": "grid-box", "data-id": deal.DealId, "onclick": "window.open('" + delaDetailLink + "','_self')" });
            var $dealHeader = $('<div class="cd-head"><div class="cd-title"><a href="'
                + delaDetailLink + '" class="hover-link" title="' + deal.DealName + '">' + deal.DealName + '</a></div><div class="cd-company" title="' + deal.CompanyName + '">' + deal.CompanyName + '<div class="cd-quote">'
                + deal.DealType + '</div></div></div>');
            $divDealContent.append($dealHeader);
            var $divDeal = $("<div />");

            // next event
           
            var $tbl = $("<table />", { "class": "cd-event" });
            var $tbody = $("<tbody />");
            var $tr = $("<tr />");
            if (deal.NextActivityDate) {
                var $tr2 = $("<tr />");
                $tr.append($("<td />", { "colspan": "2", "class": "card-label", "html": "<i class='icon-calendar'></i>Next Event" }));
                $tr2.append($("<td />", { "colspan": "2", "html": deal.NextActivityDate ? moment(deal.NextActivityDate).format("ddd, DD MMMM, YYYY") + ' at ' + moment(deal.NextActivityDate).format("hh:mm A") + ' EST' : "" }));
                $tbody.append($tr);
                $tbody.append($tr2);
                $tbl.append($tbody);
                $divDealContent.append($tbl);
            }

            // revenue/profit
            $tbl = $("<table />", { "class": "cd-table cd-profit" });
            var $thead = $("<thead />");
            $tr = $("<tr />");
            $tr.append($("<th />", { "colspan": "2", "html": "FINANCIALS" }));
            $thead.append($tr);
            $tbl.append($thead);
            $tbody = $("<tbody />");
            $tr = $("<tr />");
            $tr.append($("<td />", { "html": "<span>Revenue - <span><span>$" + formatNumber(deal.RevenueUSD.toFixed(0)) + "</span>" }));
            $tr.append($("<td />", { "html": "<span>Profit - <span><span>$" + formatNumber(deal.ProfitUSD.toFixed(0)) + "</span>" }));
            $tbody.append($tr);
            $tbl.append($tbody);
            $divDealContent.append($tbl);

            //// volumes 
            //var volumes = [];
            //if (deal.CBMs > 0) { volumes.push(formatNumber(deal.CBMs.toFixed(0)) + " CBMs"); }
            //if (deal.Kgs > 0) { volumes.push(formatNumber(deal.Kgs.toFixed(0)) + " Kgs"); }
            //if (deal.Lbs > 0) { volumes.push(formatNumber(deal.Lbs.toFixed(0)) + " Lbs"); }
            //if (deal.TEUs > 0) { volumes.push(formatNumber(deal.TEUs.toFixed(0))+ " TEUs"); }
            //if (deal.Tonnes > 0) { volumes.push(formatNumber(deal.Tonnes.toFixed(0))+ " Tonnes"); }
            //if (volumes.length > 0) { 
            //    $tbl = $("<table />", { "class": "cd-table" });
            //    $thead = $("<thead />");
            //    $tr = $("<tr />");
            //    $tr.append($("<th />", { "colspan": "2", "html": "VOLUMES" }));
            //    $thead.append($tr);
            //    $tbl.append($thead);
            //    $tbody = $("<tbody />");

            //    $tr = $("<tr />");
            //    $tr.append($("<td />", { "html": "<span>" + volumes.join(", ") + "</span>" }));
            //    $tbody.append($tr);
            //    $tbl.append($tbody);
            //    $divDealContent.append($tbl);
            //}
            //$thead.append($tr);
            //$tbl.append($thead);
            //$tbody = $("<tbody />");
            //$tr = $("<tr />");
            //$tr.append($("<td />", { "html": "<span>Air</span>" }));
            //$tr.append($("<td />", { "html": "<span>Ocean</span>" }));
            //$tbody.append($tr);
            //$tbl.append($tbody);
            //$divDealContent.append($tbl);

            // sales rep owner and team
            $salesTeam = $("<table/>", {
                class: "cd-table",
                html: '<tr><th>SALES TEAM</th></tr>'
            });
            if (deal.SalesRepName) $salesTeam.append($("<tr>", {
                html: '<td>' + deal.SalesRepName + ' - <span class="saleowner">Owner</span></td>'
            }))
            $.each(deal.SalesTeam.split(","), function (i, name) {
                if ($.trim(name) !== deal.SalesRepName) {
                    $salesTeam.append($("<tr>", {
                        html: '<td>' + name + '</td>'
                    }));
                }
            });
            $divDealContent.append($salesTeam);

            $divDeal.append($divDealContent);
            props.$wrapper.append($divDeal);
        }
    }
}