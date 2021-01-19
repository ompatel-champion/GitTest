// set global values
var recordsPerPage = 30;
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var currentPage = 1;
var $divContacts = $("#divContacts");
var $divNoItems = $("#divNoItems");
var $contactsCards = $(".contact-cards");
var $contactsTable = $(".contact-table");
var viewType = $(".btn-view-type .active").attr('data-view-type');
var tableSortOrder = "contactname asc";
var contactsloaded = false;
var totalPages = 1; 


$(function () {
    // init contacts
    new Contacts().Init();
});


var Contacts = function () {
    var self = this;

    // init contacts
    this.Init = function () {
        // retrieve contacts
        currentPage = 1;

        // new contact
        $(".new-contact").unbind('click').click(function () {
            self.OpenAddEditContact(0);
        });

        // check if card or table is user default using cookie
        var ffcontactlisttype = getCookie("ffcontactlisttype");

        // If no cookie set - default to table view
        if (ffcontactlisttype === "") {
            ffcontactlisttype = "table";
        }

        // search button
        $("#btnSearch").unbind("click").click(function () {
            currentPage = 1;
            self.RetrieveContacts();
        });

        // export excel
        $("#btnExcel").unbind("click").click(function () {
            self.ExportToExcel();
        });

        $("#ddlSalesReps").select2({ placeholder: "Sales Rep", allowClear: true });

        // sort - table header click
        new ContactTable().InitSort();

        // retrieve contacts
        self.RetrieveContacts();
    };

    this.SetViewType = function () {
        // toggle the view
        var viewType = $(".btn-view-type .active").attr('data-view-type');

        if (viewType === 'table') {
            setCookie("ffcontactlisttype", "table", 1000);
            // list view
            $contactsCards.addClass('hide');
            $contactsTable.removeClass('hide');
        } else {
            setCookie("ffcontactlisttype", "card", 1000);
            // card view
            $contactsCards.removeClass('hide');
            $contactsTable.addClass('hide');
        }
    };

    // export contacts as Excel
    this.ExportToExcel = function () {
        self.SetViewType();

        // set the filters
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.RecordsPerPage = 20000;
        filters.Keyword = $("#txtKeyword").val();
        filters.CurrentPage = currentPage;
        filters.UserId = userId;
        filters.CompanyId = 0;
        filters.SortBy = tableSortOrder;

        // API to retrieve contacts
        $.ajax({
            type: "POST",
            url: "/api/contact/ExportToExcel",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(filters),
            success: function (resultLink) {
                // remove spinner
                removeSpinner();

                window.location = resultLink;
            }, beforeSend: function () {
                // loading spinner
                addSpinner();
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    // retrieve contacts
    this.RetrieveContacts = function () {
        self.SetViewType();

        // set the filters
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.RecordsPerPage = recordsPerPage;
        filters.Keyword = $("#txtKeyword").val();
        filters.CurrentPage = currentPage;
        filters.UserId = userId;
        filters.CompanyId = 0;
        filters.SortBy = tableSortOrder;

        // clear the rows
        if (currentPage === 1) {
            var $tbody = $("#tblContacts").find("tbody");
            $tbody.html("");
            $contactsCards.html("");
        }
 
        // API to retrieve contacts
        $.ajax({
            type: "POST",
            url: "/api/contact/GetContacts",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(filters),
            success: function (contactResponse) { 
                // remove spinner
                removeSpinner();

                $(".record-count").html(formatNumber(contactResponse.Records) + " records");
                if (contactResponse.Contacts.length > 0) {
                    totalPages = contactResponse.TotalPages;

                    // bind contacts list
                    new ContactTable().BindContacts(contactResponse.Contacts);

                    // bind contacts
                    $divContacts.removeClass('hide');
                    $divNoItems.addClass('hide');

                } else {
                    $divContacts.addClass('hide');
                    if (currentPage === 1) {
                        $divNoItems.removeClass('hide');
                    }
                }
                contactsloaded = true;
            }, beforeSend: function () {
                // loading spinner
                addSpinner();
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    this.OpenAddEditContact = function (cid) {
        var contactId = parseInt(cid);
        location.href = "/Contacts/ContactAddEdit/ContactAddEdit.aspx?contactId=" + contactId + "&pg=" + currentPage;
    };

    this.OpenContactDetail = function (cid) {
        var contactId = parseInt(cid);
        location.href = "/Contacts/ContactDetail/ContactDetail.aspx?contactId=" + cid;
    };

    // delete contact
    this.DeleteContact = function (contactId, subscriberId, $ele) {
        swal({
            title: "Delete Contact!",
            text: "Are you sure you want to delete this contact?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#ea7d7d",
            confirmButtonText: "Yes, Delete!",
            closeOnConfirm: true
        }, function () {
            $.ajax({
                type: "GET",
                url: "/api/contact/DeleteContact/?contactId=" + contactId + "&userId=" + userId + "&subscriberId=" + subscriberId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {},
                success: function (response) {
                    if (response) {
                        $ele.fadeOut();
                        self.RetrieveContacts();
                    }
                },
                beforeSend: function () { },
                error: function (request) { }
            });
        });
    };
};


var ContactTable = function () {
    var self = this;
    var $tbody = $("#tblContacts").find("tbody");

    // iterate and bind contacts
    this.BindContacts = function (contacts) {
        $.each(contacts, function (i, contact) {
            var $tr = self.GetContactItemHtml(i, contact);
            $tbody.append($tr);
        });
    };

    this.GetContactItemHtml = function (i, contact) {
        var $tr = $("<tr/>", { "data-id": contact.ContactId });
        var $tdContactNameAddress = $("<td/>");
        $tr.append($tdContactNameAddress);

        // contact name
        var contactDetailPageUrl = "/Contacts/ContactDetail/ContactDetail.aspx?contactId=" + contact.ContactId;
        var $pName = $("<p/>");
        var $aContactName = $("<a/>", {
            "class": "cont-title hover-link", "href": contactDetailPageUrl,
            "html": contact.FirstName + " " + contact.LastName
        });
        $pName.append($aContactName);
        $tdContactNameAddress.append($pName);

        // job title
        if (contact.Title !== '') {
            var $pJobTitle = $("<p/>", { "class": "jobtitle", "html": contact.Title });
            $tdContactNameAddress.append($pJobTitle); 
        } 

        // address
        var address = [];
        if (contact.BusinessStateProvince && contact.BusinessStateProvince !== '') {
            address.push(contact.BusinessStateProvince);
        }
        if (contact.BusinessPostalCode && contact.BusinessPostalCode !== '') {
            address.push(contact.BusinessPostalCode);
        }

        var $pAddressPhone = $("<p/>");
        if (address.length > 0) {
            var $address = $("<span/>", { "class": "m-r-sm", "html": "<i class=\"text-navy  fa fa-map-marker m-t-xs m-r-xs\"></i>" + address.join(", ") });
            $pAddressPhone.append($address);
        }

        // phone
		var $tdPhone = $("<td/>");
        if (contact.BusinessPhone && contact.BusinessPhone !== '') {
            var $phone = $("<span />", { "html": "<a class='a-phone-number text-muted' href='tel:" + contact.BusinessPhone + "'>" + contact.BusinessPhone + "</a>" });
            $tdPhone.append($phone);
        }
        $tr.append($tdPhone);
		
		// email
		var $tdEmail = $("<td/>");
		if (contact.Email && contact.Email !== '') {
            var $Email = $("<span />", { "html": "<a class='a-phone-number' href='mailto:" + contact.Email + "'>" + FFGlobal.utils.html.addWordBreakOpportunities(contact.Email) + "</a>" });
            $tdEmail.append($Email);
        }
		$tr.append($tdEmail);
         
        // company
        var $tdCompany = $("<td/>");
        var $pCompany = $("<p/>");
        var companyDetailPageUrl = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + contact.CompanyId + "&subscriberId=" + contact.SubscriberId;

        // var companyDetailPageUrl = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + contact.CompanyId;
        var companyName = (contact.CompanyName && contact.CompanyName !== '') ? contact.CompanyName : "";
        var $aCompnanyName = $("<a/>", { "class": "text-muted", "href": companyDetailPageUrl, "html": companyName });
        $pCompany.append($aCompnanyName);
        $tdCompany.append($pCompany);
        $tr.append($tdCompany);

        // city
        var $tdCity = $("<td/>", { "class": "" });
        if (contact.BusinessCity && contact.BusinessCity !== '') {
            var $city = $("<span />", { "html": contact.BusinessCity+', '});
            $tdCity.append($city);
        }
        $tr.append($tdCity);
		
		// users - sales team
        var $tdContactUsers = $("<td/>", { "class": "", "html": (contact.SalesTeam ? contact.SalesTeam : "-") });
        $tr.append($tdContactUsers);

        // country
        if (contact.BusinessCountry && contact.BusinessCountry !== '') {
            var $country = $("<span />", { "html": contact.BusinessCountry });
        } 
        $tdCity.append($country);

        // last acticity date
        var $tdLastActivityDate = $("<td />");
        if (contact.LastActivityDate) {
            $tdLastActivityDate.append($("<p />", { "html": moment(contact.LastActivityDate).format("DD-MMM-YY") }));
        }
        $tr.append($tdLastActivityDate);

        // actions
        var $tdActions = $("<td/>", { "class": "action-cell text-center" });

        var $imgEdit = $("<a/>", { "class": "hover-link", "href": "javascript:void(0)", "html": "View" });
        $tdActions.append($imgEdit);

        $imgEdit.unbind('click').click(function () {
            new Contacts().OpenContactDetail(contact.ContactId);
        });

        $tr.append($tdActions);

        return $tr;
    };

    this.InitSort = function () {
        var $tbl = $("#tblContacts");

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
                $this.append('<i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i>');
                $this.attr("data-sort-order", currentSortOrder);

                // do the search again
                tableCurrentPage = 1;

                new Contacts().RetrieveContacts();
            }
        }); 
    };

};


var ContactCards = function () {
    var self = this;

    // bind contacts
    this.BindContacts = function (contacts) {
        // iterate and bind contacts
        var $mainRow = $("<div/>", { "class": "row" });
        $.each(contacts, function (i, contact) {
            var $column = self.GetContactItemHtml(i, contact);
            $contactsCards.append($column);
        });

        $contactsCards.masonry('destroy')
        // apply masonry
        $contactsCards.masonry({
            itemSelector: '.col-lg-4'
        });

        $contactsCards.masonry('layout');
    };

    // get contact box html
    this.GetContactItemHtml = function (i, contact) {

        var contactDetailPageUrl = "/Contacts/ContactDetail/ContactDetail.aspx?contactId=" + contact.ContactId;
        var $column = $("<div/>", { "class": "col-lg-4 PR5 PL5" });

        $column.unbind('click').click(function (e) {

            // edit
            var $edit = $(this).find(".btn-edit");
            if (e.target === $edit[0]) return false;

            // email
            var $email = $(this).find(".a-email");
            if (e.target === $email[0]) return false;

            // phone
            var $phoneNumber = $(this).find(".a-phone-number");
            if (e.target === $phoneNumber[0]) return false;


            var $delete = $(this).find(".btn-delete");
            if (e.target === $delete[0]) return false;

            // go to contact detail page
            location.href = contactDetailPageUrl;
            return true;
        });

        var $contactBox = $("<div/>", { "class": "contact-box" });
        var $aContact = $("<a/>", { "href": "javascript:void(0)" });

        // icons
        var $divActions = $("<div/>", { "class": "m-t-sm btn-group" });
        var $btnEdit = $("<a/>", { "class": "btn btn-white  btn-xs", "href": "javascript:void(0)", "html": "<i class='btn-edit fa fa-pencil text-primary'></i>" });
        $divActions.append($btnEdit);

        var $btnDelete = $("<a/>", { "class": "btn btn-xs btn-white", "href": "javascript:void(0)", "html": "<i class='btn-delete fa fa-trash text-danger'></i>" });
        $divActions.append($btnDelete);

        $btnDelete.unbind('click').click(function () {
            new Contacts().DeleteContact(contact.ContactId, contact.SubscriberId, $contactBox);
        });

        $btnEdit.unbind('click').click(function () {
            new Contacts().OpenAddEditContact(contact.ContactId);
        });

        // right column
        var $divRight = $("<div/>", { "class": "col-sm-12" });
        var $h3 = $("<h3/>", { "html": "<strong>" + contact.FirstName + " " + contact.LastName + "</strong>" });
        $divRight.append($h3);

        // job title
        if (contact.Title && contact.Title !== '') {
            var $jobTitleCompany = $("<div/>", { "class": "m-t-xs m-b-sm font-bold", "html": contact.Title + ", " });

            // company name
            var companyDetailPageUrl = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + contact.CompanyId + "&subscriberId=" + contact.SubscriberId;
            var $companyName = $("<a />", { "href": companyDetailPageUrl, "html": contact.CompanyName });
            $jobTitleCompany.append($companyName);

            $divRight.append($jobTitleCompany);
        }

        // email
        if (contact.Email && contact.Email !== '') {
            var $emailAddress = $("<p />", { "html": "<a class='text-muted a-email' href='mailto:" + contact.Email + "'><i class=\"text-navy fa fa-envelope m-r-xs\"></i>" + contact.Email + "</a>" });
            $divRight.append($emailAddress);
        }

        // address
        if (contact.BusinessAddress && contact.BusinessAddress !== '') {
            var $address = $("<p/>", { "html": "<i class=\"text-navy fa fa-map-marker m-t-xs m-r-xs\"></i>" + contact.BusinessAddress });
            $divRight.append($address);
        }

        var address = [];
        if (contact.BusinessCity && contact.BusinessCity !== '') {
            address.push(contact.BusinessCity);
        }
        if (contact.BusinessCountry && contact.BusinessCountry !== '') {
            address.push(contact.BusinessCountry);
        }
        if (contact.BusinessStateProvince && contact.BusinessStateProvince !== '') {
            address.push(contact.BusinessStateProvince);
        }
        if (contact.BusinessPostalCode && contact.BusinessPostalCode !== '') {
            address.push(contact.BusinessPostalCode);
        }

        if (address.length > 0) {
            var $addressMore = $("<p/>", { "html": address.join(", ") });
            $divRight.append($addressMore);
        }

        // sales team
        if (contact.SalesTeam && contact.SalesTeam != "") {
            $divRight.append($("<p/>", { "class": " m-t-sm", "html": "<small class='text-navy'>Sales Team: </small>" + contact.SalesTeam }));
        }

        if (contact.BusinessPhone && contact.BusinessPhone !== '') {
            var $phone = $("<p />", { "html": "<a class='text-muted a-phone-number' href='tel:" + contact.BusinessPhone + "'><i class=\"text-navy fa fa-phone m-t-xs m-r-xs\"></i>" + contact.BusinessPhone + "</a>" });
            $divRight.append($phone);
        }

        $aContact.append($divRight);
        $aContact.append($("<div/>", { "class": "clearfix" }));

        $contactBox.append($aContact);
        $column.append($contactBox);
        return $column;
    };
};


function getProfileImage($img, contactid) {
    $.getJSON("/api/contact/getcontactprofilepic/?contactid=" + contactid, function (response) {
        if (response !== '') {
            $img.attr("src", response + "?w=80&h=80&mode=crop");
        } else {
            $img.attr("src", "/_content/_img/no-pic.png?w=80&h=80&mode=crop");
        }
    });
}
 

function initScrollLoader() {
    var nearToBottom = 10;
    if ($(window).scrollTop() + $(window).height() > $(document).height() - nearToBottom) {
        if (contactsloaded) {
            currentPage = currentPage + 1;
            if (totalPages >= currentPage) {
                new Contacts().RetrieveContacts();
            }
        }
    }
}

$(window).scroll(function () {
    initScrollLoader();
});
