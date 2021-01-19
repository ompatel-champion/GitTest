// set global values
var recordsPerPage = 28;
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var currentPage = 1;
var $divNoItems = $("#divNoItems");
var $companytbody = $("#tblCompanies>tbody");
var $divCardView = $("#divCardView");
var $divTableView = $("#divTableView");
var $pagination = $(".pagination-ele");
var viewType = "table";
var userroles = $("#lblUserRole").text();
var $btnViewType = $(".btn-view-type");
var $divComanies = $("#divComanies");
var $divGlobalCompanies = $("#divGlobalCompanies");
var $divLoading = $("#divLoading");
var isAdmin = $("#lblIsAdmin").text() === "1";
var tableSortOrder = "companyname asc";
var companiesloaded = false;
var totalPages = 1;
var accessibleCompanyIds = [];
var SelectedFilterType = "ALL";

$(function () {
    // initialize companies
    new Companies().Init();
    new GlobalCompanies().Init();
    //move element query (moves the filters from the top header row to the bottom header row depending on window width)
    new FFGlobal.utils.mediaQuery.moveElm({
        $elm: $(".search-filters"),
        queries: {
            "top-row": {
                $parent: $(".top-row-col.col-filters"),
                match: "all"
            },
            "bottom-row": {
                $parent: $("#query-parent-search-filters"),
                match: "(max-width: 1290px)"
            }
        }
    });
    //move element query (moves the filters from the top header row to the bottom header row depending on window width)
    new FFGlobal.utils.mediaQuery.moveElm({
        $elm: $(".col-global-search .global-search"),
        queries: {
            "top-row": {
                $parent: $(".col-global-search"),
                match: "all"
            },
            "bottom-row": {
                $parent: $("#query-parent-global-search"),
                match: "(max-width: 530px)"
            }
        }
    });
});

var Companies = function () {
    var self = this;
    this.Init = function () {

        // select2
        $("#ddlCountry").select2({ placeholder: "Country", allowClear: true, width: '100%' });

        // new company
        $(".new-company").unbind('click').click(function () {
            location.href = "/Companies/CompanyAddEdit/CompanyAddEdit.aspx?from=companylist";
        });

        currentPage = 1;
        var pg = getQueryString("page");
        if (pg && pg !== '') {
            currentPage = parseInt(pg);
        }

        // export excel
        $("#btnExcel").unbind("click").click(function () {
            self.ExportToExcel();
        });

        // focus on search textbox when global company lookup modal is shown
        $("#globalCompanyLookup").on('shown.bs.modal', function (event) {
            $(".modal").removeAttr("tabindex");
            $(".modal").find("#txtGlobalCompanySearch").focus();
        });

        // view type actions
        $(".btn-view-type button").unbind('click').click(function () {
            // set active button
            currentPage = 1;
            $(".btn-view-type button").removeClass('active');
            $(this).addClass('active');

            // retrieve companies 
            self.RetrieveCompanies();
            $(document).scrollTop();
        });

        // company type actions
        //$(".btn-company-type button, .btn-company-type li").unbind('click').click(function () {
        //    // set active button
        //    currentPage = 1;
        //  //  $(".btn-company-type button, .btn-company-type li").removeClass('active');
        //  //  $(this).addClass('active');
        //    // retrieve companies 
        //    self.RetrieveCompanies();
        //    $(document).scrollTop();
        //});

        // check if card or table is user default using cookie
        var ffcompanylisttype = getCookie("ffcompanylisttype");

        // If no cookie set - default to table view
        if (ffcompanylisttype === "") {
            ffcompanylisttype = "table";
        }

        $divTableView.addClass('hide');
        $pagination.addClass('hide');

        // default list type
        if (ffcompanylisttype === "table") {
            $(".btn-view-type button[data-view-type='table']").click();
        } else {
            $(".btn-view-type button[data-view-type='card']").click();
        }

        $("#ddlCompanyType").select2({ theme: "classic" });
        $("#ddlStatus").select2({ theme: "classic" });

        // search button
        $(".btn-search").unbind("click").click(function () {
            self.DoSearch();
        });


        $("#txtKeyword,#txtCity,#txtPostalCode").keypress(function (e) {
            if (e.which === 13) {
                self.DoSearch();
                return false;    //<---- Add this line
            }
        });

        $("#ddlCountry").change(function () {
            self.DoSearch();
        });

        // sort - table header click
        new CompanyTable().InitSort();
    };

    this.SetupViewType = function () {
        // toggle the view
        viewType = $(".btn-view-type .active").attr('data-view-type');
        if (viewType === 'table') {
            setCookie("ffcompanylisttype", "table", 1000);
            // table view
            $("#divCardView").addClass('hide');
            $("#divTableView").removeClass('hide');
        } else {
            setCookie("ffcompanylisttype", "card", 1000);
            // card view
            $("#divCardView").removeClass('hide');
            $("#divTableView").addClass('hide');
        }
    };

    // export to excel
    this.ExportToExcel = function () {
        //location.href = activityResponse.ExcelUri;
        {
            self.SetViewType();

            // set the filters
            var filters = new Object();
            filters.SubscriberId = subscriberId;
            filters.CurrentPage = currentPage;
            filters.RecordsPerPage = 2000;
            filters.LocationName = $("#ddlLocation").val();
            filters.LoginEnabled = $("#ddlLoginEnabled").val();
            filters.Keyword = $("#txtKeyword").val();
            filters.City = $("#txtCity").val();
            filters.PostalCode = $("#txtPostalCode").val();
            filters.CountryName = $("#ddlCountry").val();
            filters.ShowAdmin = true;
            filters.SortBy = "firstname asc";
            filters.SortBy = tableSortOrder;

            // clear the rows
            $userCards.html("");
            var $tbody = $tblUsers.find("tbody");
            $tbody.html("");

            // AJAX to retrieve users
            $.ajax({
                type: "POST",
                url: "/api/Company/ExportToExcel",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(filters),
                success: function () {
                    // remove loading message
                    swal.close();
                },
                beforeSend: function () {
                    //add loading message
                    swal({
                        text: translatePhrase("Loading Users") + "...",
                        title: "<img src='/_content/_img/loading_40.gif'/>",
                        showConfirmButton: false,
                        allowOutsideClick: false,
                        html: false
                    });

                },
                error: function () {
                }
            });
        }
    };

    // retrieve companies
    this.RetrieveCompanies = function () {

        // set-up view type
        self.SetupViewType();

        // set the filters
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.UserId = userId;
        filters.RecordsPerPage = recordsPerPage;
        filters.Keyword = $("#txtKeyword").val();
        filters.CurrentPage = currentPage;
        filters.CompanyType = $("#ddlCompanyType").val();
        filters.City = $("#txtCity").val();
        filters.PostalCode = $("#txtPostalCode").val();
        filters.CountryName = $("#ddlCountry").val();
        filters.SortBy = tableSortOrder;


        var companyType = $('.filter-dropdown ul li.selected').attr('data-status');
        if (companyType === "inactive") {
            filters.FilterType = "INACTIVE";
        } else if (companyType === "active-customer") {
            filters.IsCustomer = true;
            filters.FilterType = "ALL";
        } else if (companyType === "inactive-customer") {
            filters.IsCustomer = true;
            filters.FilterType = "INACTIVE";
        } else if (companyType === "active") {
            filters.FilterType = "ALL";
        }
        SelectedFilterType = filters.FilterType;

        // show/hide next activity date
        if (SelectedFilterType === "INACTIVE") {
            $("th[data-field-name='nextactivity']").addClass("hide");
        } else {
            $("th[data-field-name='nextactivity']").removeClass("hide");
        }

        // clear the rows 
        $(".pagination").addClass('hide');

        if (currentPage === 1) {
            $companytbody.html("");
            $divCardView.html("");
        }

        accessibleCompanyIds = [];

        // api to retrieve companies
        $.ajax({
            type: "POST",
            url: "/api/Company/GetCompaniesGlobal",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 100000,
            data: JSON.stringify(filters),
            success: function (companyResponse) {
                // remove loading message
                removeSpinner();
                $(".total-records").html(formatNumber(companyResponse.Records) + " record"+(!companyResponse.Records||companyResponse.Records>1?"s":""));
                if (companyResponse.Companies.length > 0) {
                    totalPages = companyResponse.TotalPages;
                    if (viewType === "table") {
                        // bind companies
                        new CompanyTable().BindCompanies(companyResponse);
                    } else {
                        // bind cards
                        new CompanyCards().BindCards(companyResponse);
                    }

                    // bind companies
                    $divNoItems.addClass('hide');
                    $pagination.removeClass('hide');
                } else {
                    $divTableView.addClass('hide');
                    if (currentPage === 1) {
                        $divNoItems.removeClass('hide');
                    }
                    $pagination.addClass('hide');
                    $divCardView.addClass('hide');
                }
                companiesloaded = true;
            }, beforeSend: function () {
                addSpinner();
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };


    // open add company modal
    this.OpenAddEditCompany = function (companyId, companyName, sid) {
        location.href = "/Companies/CompanyAddEdit/CompanyAddEdit.aspx?companyId=" + companyId + "&subscriberId=" + sid + "&pg=" + currentPage;
    };


    this.DeleteCompany = function (companyId, $ele) {
        swal({
            title: translatePhrase("Delete Company!"),
            text: translatePhrase("Are you sure you want to delete this company?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }, function () {
            $.ajax({
                type: "GET",
                url: "/api/company/DeleteCompany/?companyid=" + companyId + "&userId=" + userId,
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data) {
                        $ele.fadeOut();
                    }
                }
            });
        });
    };

    this.DoSearch = function () {
        $divComanies.removeClass("hide");
        $("#divCardView").addClass('hide');
        $("#divTableView").addClass('hide');
        $divNoItems.addClass('hide');
        $btnViewType.removeClass("hide");
        currentPage = 1;
        self.RetrieveCompanies();
    };

    this.ClaimCompany = function (companyId, sid) {
        swal({
            title: translatePhrase("Claim company!"),
            text: translatePhrase("Are you sure you want to claim this company?"),
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#F8AC59",
            confirmButtonText: translatePhrase("Yes, Claim!"),
            closeOnConfirm: true
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/company/ClaimCompany/?companyid=" + companyId + "&userId=" + userId + "&subscriberId=" + sid,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data) {
                            new Companies().RetrieveCompanies();
                        }
                    }
                });
            }
        });
    };

    this.ReassignCompany = function (companyId) {
        var url = "/Companies/Reassign/ReassignCompany.aspx?companyId=" + companyId;
        location.href = url;
        return;
    };

    this.RequestAccess = function (companyId, companySubscriberId, $td) {
        swal({
            title: translatePhrase("Request Access!"),
            text: translatePhrase("Are you sure you want to request access for this company?"),
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#F8AC59",
            confirmButtonText: translatePhrase("Yes!"),
            closeOnConfirm: true
        }, function () {
            $.ajax({
                type: "GET",
                url: "/api/company/RequestAccess/?companyid=" + companyId + "&userId=" + userId + "&companySubscriberId=" + companySubscriberId + "&userSubscriberId=" + subscriberId,
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data) {
                        $td.html("");
                        $td.append($("<p/>", { "html": "Requested", "class": "text-navy" }));
                        swal({
                            title: "Request has been sent to company owner!",
                            type: "success",
                            showCancelButton: false
                        });
                    }
                }
            });
        });
    };
};


var CompanyTable = function () {
    var self = this;

    // bind companies
    this.BindCompanies = function (companyResponse) {
        var companies = companyResponse.Companies;
        // $companytbody.html("");
        // iterate and bind companies
        accessibleCompanyIds = companyResponse.AccessibleCompanyIds;
        $.each(companies, function (i, company) {
            //  alert("asdas")
            var $tr = self.GetCompanyItemHtml(company);
            $companytbody.append($tr);
        });
        $divTableView.removeClass("hide");
    };

    // get company box html
    this.GetCompanyItemHtml = function (company) {

        var hasPermission = true;

        var companyDetailLink = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + company.CompanyId + "&subscriberId=" + company.SubscriberId;
        var $tr = $("<tr/>", { "data-id": company.CompanyId });

        // company name
        var $tdCompanyNameAddress = $("<td/>", { "data-toggle": "tooltip", "title": "View Company" });
        var companyDetailPageUrl = companyDetailLink;
        //var $pCompanyName = $("<p/>", { "class": "font-bold FontSize13", "html": company.CompanyName });
        var $pCompanyName = $("<a/>", { "class": "comp-title", "html": company.CompanyName });

        $tdCompanyNameAddress.append($pCompanyName);
        $tr.append($tdCompanyNameAddress);

        // division
        //if (company.Division && company.Division !== '') {
        //    $tdCompanyNameAddress.append($("<p/>", { "html": company.Division }));
        //}

        // city
        var cityLocations = [];

        if (company.City && company.City !== '') {
            cityLocations.push(company.City);
        }
        if (company.CountryName && company.CountryName !== '')
            cityLocations.push(company.CountryName);

        var $tdCity = $("<td/>", { "class": "W100", "data-toggle": "tooltip", "title": "view company" });
        if (company.City && company.City !== '') {
            var $city = $("<p />", { "html": company.City });
            $tdCity.append($city);
        } else {
            $tdCity.append($("<span />", { "html": "-" }));
        }
        $tr.append($tdCity);

        // country
        var $tdCountry = $("<td/>", { "class": "W100", "data-toggle": "tooltip", "title": "view company" });
        if (company.CountryName && company.CountryName !== '') {
            var $country = $("<p />", { "html": company.CountryName });
            $tdCountry.append($country);
        } else {
            $tdCountry.append($("<span />", { "html": "-" }));
        }
        $tr.append($tdCountry);

        // sales team 
        var $tdSalesTeam = $("<td/>", { "class": "W200" });
        if (company.SalesTeam && company.SalesTeam !== "") {
            // sales team   
            var finalSalesTeamList = [];
            var salesTeamUsers = company.SalesTeam.split(",");
            $.each(salesTeamUsers, function (i, ele) {
                if ($.inArray($.trim(ele), finalSalesTeamList)) {
                    finalSalesTeamList.push($.trim(ele));
                }
            });
            $tdSalesTeam.append($("<p />", { "data-type": "sales-team", "html": finalSalesTeamList.join("<br />") }));
        }
        $tr.append($tdSalesTeam);


        // last activity date
        var $tdLastActivityDate = $("<td />", { "class":"W150 text-center", "data-toggle": "tooltip", "title": "view company" });
        if (company.LastActivityDate) {
            $tdLastActivityDate.append($("<p />", { "html": moment(company.LastActivityDate).format("DD-MMM-YY") }));
        }
        $tr.append($tdLastActivityDate);

        // next activity date
        if (SelectedFilterType !== "INACTIVE") {
            var $tdNextActivityDate = $("<td />", { "class": "W150 text-center","data-toggle": "tooltip", "title": "view company" });
            if (company.NextActivityDate) {
                $tdNextActivityDate.append($("<p />", { "html": moment(company.NextActivityDate).format("DD-MMM-YY") }));
            }
            $tr.append($tdNextActivityDate);
        }

    


        // actions
        var $tdActions = $("<td/>", { "class": "action-cell W100  text-center" });
        var $aEdit = "";
        var $imgEdit = "";

        // claim
        if (SelectedFilterType === "INACTIVE") {
            // if (selectedFilterType == "INACTIVE" || !company.Active) {
            if (isAdmin) {

                $aEdit = $("<a />", { "class": "btn-edit hover-link", "title": "View Company", "data-toggle": "tooltip" });
                //$imgEdit = $("<img/>", { "class": "img-action-edit", "src": "/_content/_img/icons/edit.png" });
                //$aEdit.append($imgEdit);
                $tdActions.append($aEdit);
                $aEdit.unbind('click').click(function () {
                    new Companies().OpenAddEditCompany(company.CompanyId, company.CompanyName, company.SubscriberId);
                });

                var $aReassign = $("<a />", { "html": "Reassign", "class": "btn-assign hover-link" });
                $aReassign.unbind("click").click(function () {
                    new Companies().ReassignCompany(company.CompanyId);
                });
                $tdActions.append($aReassign);

                $tr.unbind("click").click(function (e) {
                    var $imgEdit = $(this).find(".btn-edit").find("img");
                    if (e.target === $imgEdit[0])
                        return false;


                    var $btnClaim = $(this).find(".btn-claim");
                    if (e.target === $btnClaim[0])
                        return false;

                    var $btnAssign = $(this).find(".btn-assign");
                    if (e.target === $btnAssign[0])
                        return false;

                    location.href = companyDetailLink;
                    return true;
                });
            } else {
                var $aClaim = $("<a />", { "html": "Claim", "class": "btn-claim hover-link" });
                $aClaim.unbind("click").click(function () {
                    new Companies().ClaimCompany(company.CompanyId, company.SubscriberId);
                });
                $tdActions.append($aClaim);
            }
        } else {
            $aEdit = $("<a />", { "html": "View", "class": "btn-edit hover-link", "title": "Edit Company", "data-toggle": "tooltip" });
            $tdActions.append($aEdit);

            $aEdit.unbind('click').click(function () {
                new Companies().OpenAddEditCompany(company.CompanyId, company.CompanyName, company.SubscriberId);
            });

            // view 
            $tr.unbind("click").click(function (e) {
                var $imgEdit = $(this).find(".btn-edit").find("img");
                if (e.target === $imgEdit[0])
                    return false;

                var $btnClaim = $(this).find(".btn-claim");
                if (e.target === $btnClaim[0])
                    return false;

                var $btnAssign = $(this).find(".btn-assign");
                if (e.target === $btnAssign[0])
                    return false;

                location.href = companyDetailLink;
                return true;
            });
        }

        $tr.append($tdActions);

        return $tr;
    };

    this.InitSort = function () {
        $("#tblCompanies>thead>tr>th").unbind("click").click(function () {
            var $this = $(this);
			var currentSortOrder = "";
            var sortFieldName = $this.attr("data-field-name");
            if (sortFieldName && sortFieldName !== '') {
                var sortorder = "asc";
                // check if already any sort going on
                var $sortitem = $this.find(".sort");
                if ($sortitem && $sortitem !== null) {
                    // already sorting using this field - check if ASC or DESC 
                    currentSortOrder = $sortitem.closest("th").attr("data-sort-order");
                    currentSortOrder = currentSortOrder === "asc" ? "desc" : "asc";
                    tableSortOrder = sortFieldName + " " + currentSortOrder;
                } else {
                    // NOT sorting using this field - use ASC
                    sortorder = "asc";
                    tableSortOrder = sortFieldName + " " + sortorder;
                }
				
				// remove current sort up/down icons
				$("#tblCompanies>thead>tr>th").find(".sort").remove();
				
				if (currentSortOrder === "asc")
                    $this.append('<i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i>');
                else 
                    $this.append('<i class="sort icon-Descending"><span class="path1"></span><span class="path2"></span></i>');
				
                $this.attr("data-sort-order", currentSortOrder);

                // do the search again
                var currentPage = 1;
                new Companies().RetrieveCompanies();
            }
        });
    };
};


var CompanyCards = function () {
    var self = this;

    this.BindCards = function (companyResponse) {
        var companies = companyResponse.Companies;
        // $divCardView.html("");
        // iterate and bind companies
        var $mainRow;
        $.each(companies, function (i, company) {
            if (i % 3 === 0) {
                $mainRow = $("<div/>", { "class": "row no-gutters" });
                $divCardView.append($mainRow);
            }
            var $column = self.GetCompanyItemHtml(i, company, companyResponse.AccessibleCompanyIds);
            $mainRow.append($column);
        });
    };

    // get company box html
    this.GetCompanyItemHtml = function (i, company, accessibleCompanyIds) {

        var hasPermission = true; 

        var companyDetailLink = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + company.CompanyId + "&subscriberId=" + company.SubscriberId;
        var $column = $("<div/>", { "class": "col-md-4 column" });

        if (SelectedFilterType !== "INACTIVE") {
            $column.unbind('click').click(function (e) {
                var $imgEdit = $(this).find(".btn-edit").find("img");
                if (e.target === $imgEdit[0]) return false;
                var $imgDelete = $(this).find(".btn-delete").find("img");
                if (e.target === $imgDelete[0]) return false;
                location.href = companyDetailLink;
                return true;
            });
        } else {
            if (isAdmin && company.Active) { 
                $column.unbind('click').click(function (e) {
                    location.href = companyDetailLink;
                    return true;
                });
            }
        }

        var $companyBox = $("<div/>", { "class": "contact-box clearfix" });

        var $aCompany = $("<a/>", { "href": "javascript:void(0)" });

        //company name
        var $h3 = $("<h3/>", { "class": "box-title", "html": company.CompanyName });
        $aCompany.append($h3);

        // left column
        var $divLeft = $("<div/>", { "class": "box-details" });

        // Title
        $divLeft.append($aCompany);

        // icons
        var $divActions = $("<div/>", { "class": "box-acts" });

        //   if (selectedFilterType == "INACTIVE" || !company.Active) {

        if (SelectedFilterType === "INACTIVE") {
            //if (!company.Active) {
            if (isAdmin) {
                var $aReassign = $("<a />", { "html": "Reassign", "class": "view" });
                $aReassign.unbind("click").click(function () {
                    new Companies().ReassignCompany(company.CompanyId);
                });
                $divActions.append($aReassign);
            } else {
                var $aClaim = $("<a />", { "html": "Claim", "class": "view" });
                $aClaim.unbind("click").click(function () {
                    new Companies().ClaimCompany(company.CompanyId, company.SubscriberId);
                });
                $divActions.append($aClaim);
            }
        } else {
            if (hasPermission) {
                var $aView = $("<a />", { "html": "View", "class": "view" });
                $divActions.append($aView);
                $aView.unbind("click").click(function () {
                    location.href = companyDetailLink;
                });
                 
            }

        }

        //$aCompany.append($divLeft);

        // right column
        var $divRight = $("<div/>", { "class": "box-image" });
        var $img = $("<img/>", { "alt": "image", "src": "" });

        // company status 
        var status = "Inactive";
        if (company.IsCustomer) {
            status = "Customer";
        } else if (company.Active) {
            status = "Active";
        }
        $divRight.append($("<div/>", { "class": "box-status", "html": status }));

        // division
        if (company.Division && company.Division !== '') {
            $divRight.append($("<div/>", { "html": company.Division }));
        }

        // image logo
        getCompanyLogo($img, company.CompanyId, $divRight);

        // actions
        $divRight.append($divActions);

        // set the address
        if (company.Address && company.Address !== '') {
            var $address = $("<div/>", { "class": "box-adds box-icon", "html": company.Address });
            //$divLeft.append($address);
        }

        var address = [];
        if (company.City && company.City !== '') {
            address.push(company.City);
        }
        if (company.StateProvince && company.StateProvince !== '') {
            address.push(company.StateProvince);
        }
        if (company.PostalCode && company.PostalCode !== '') {
            address.push(company.PostalCode);
        }
        if (company.CountryName && company.CountryName !== '') {
            address.push(company.CountryName);
        }

        if (address.length > 0) {
            var $addressMoere = $("<div/>", { "html": address.join(", ") });
            $address.append($addressMoere);
        }

        // phone
        if (company.Phone && company.Phone !== '') {
            var $phone = $("<div />", { "class": "box-phone box-icon", "html": "<a class='a-phone-number' href='tel:" + company.Phone + "'>" + company.Phone + "</a>" });
            $divLeft.append($phone);
        }

        // sales team
        if (company.SalesTeam && company.SalesTeam !== "") {
            $divLeft.append($("<div/>", { "class": "box-team", "html": "<span class='bt-title'>Sales Team: </span><span class='bt-list'>" + company.SalesTeam + "</span>" }));
        }

        // company types
        var $divTypes = $("<div/>", { "class": "box-type" });
        if (company.CompanyTypes && company.CompanyTypes !== "" && company.CompanyTypes !== "0") {
            var companyTypesArr = company.CompanyTypes.split(",");
            $.each(companyTypesArr, function (i, cType) {
                $divTypes.append($("<label/>", { "html": cType }));
            });
            $divLeft.append($divTypes);
        }

        //$aCompany.append($divLeft);
        //$aCompany.append($("<div/>", { "class": "clearfix" }));

        $companyBox.append($divRight);
        $companyBox.append($divLeft);

        $column.append($companyBox);
        return $column;
    };
};


function getCompanyLogo($img, companyid, $divLeft) {
    var $pLogo = $("<p/>", { "class": "text-success-light MT10" });

    $.getJSON("/api/company/getcompanylogo/?companyid=" + companyid, function (response) {
        if (response !== '') {
            $img.attr("src", response + "?w=80&h=80&mode=crop");
            $divLeft.prepend($img);
        } else {
            $divLeft.prepend($pLogo);
        }
    });
}


function RefreshCompanies() {
    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // do the following to get rid of the padding
    $('body').removeAttr('style');
    // reload companies
    new Companies().DoSearch();
}


function GoToCompanyDetail(companyId) {

    $(".modalWrapper").remove();
    // remove model-open to get the scroll bar of the parent
    $('body').removeClass('modal-open');
    // do the following to get rid of the padding
    $('body').removeAttr('style');

    // go to deal detail
    location.href = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + companyId + "&newcontact=1";
}


function initScrollLoader() {
    var nearToBottom = 10;
    if ($(window).scrollTop() + $(window).height() > $(document).height() - nearToBottom) {
        // alert(currentPage)
        if (companiesloaded) {
            currentPage = currentPage + 1;
            if (totalPages >= currentPage) {
                new Companies().RetrieveCompanies();
            }
        }
    }
}


$(window).scroll(function () {
    initScrollLoader();
});


var GlobalCompanies = function () {
    var self = this;
    var $txtGlobalCompanySearch = $("#txtGlobalCompanySearch");
    var $divGlobalCompanies = $("#divGlobalCompanies");
    var $divGlobalLoading = $("#divGlobalLoading");
    var $tblGlobalCompanies = $("#tblGlobalCompanies");
    var $globalCompanytbody = $("#tblGlobalCompanies>tbody");

    this.Init = function () {
        $("#btnSearchGlobalCompany").unbind("click").click(function () {
            //self.Search();
        });

        $txtGlobalCompanySearch.keyup(function () {
            if ($txtGlobalCompanySearch.val().length > 2) {
                self.Search();
            }
        });
    };

    this.Search = function () {
        $txtGlobalCompanySearch.removeClass("error");
        var searchTerm = $txtGlobalCompanySearch.val();
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.UserId = userId;
        filters.RecordsPerPage = recordsPerPage;
        filters.Keyword = $txtGlobalCompanySearch.val();

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
    };

    this.BindCompanies = function (companyResponse) {

        $globalCompanytbody.html("");
        var companies = companyResponse.Companies;
        accessibleCompanyIds = companyResponse.AccessibleCompanyIds;

        // iterate and bind companies
        $.each(companies, function (i, company) {
            var $tr = self.GetCompanyItemHtml(company);
            $globalCompanytbody.append($tr);
        });
        $divTableView.removeClass("hide");
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
        var $tdCompanyNameAddress = $("<td/>", { "class": "W300 titleBox" });
        var companyDetailPageUrl = companyDetailLink;
        //if (hasPermission) {
        //    var $aCompnanyName = $("<a/>", { "href": companyDetailPageUrl, "html": company.CompanyName, "target": "_blank" });
        //    var $pCompanyName = $("<p/>");
        //    $pCompanyName.append($aCompnanyName);
        //} else {
        //    if (isAdmin && company.Active) {
        //        companyDetailPageUrl = "/Companies/CompanyView/CompanyView.aspx?companyId=" + company.CompanyId + "&subscriberId=" + company.SubscriberId;
        //        var $aCompnanyName = $("<a/>", { "href": companyDetailPageUrl, "html": company.CompanyName, "target": "_blank" });
        //        var $pCompanyName = $("<p/>");
        //        $pCompanyName.append($aCompnanyName);
        //    } else {
        //        var $pCompanyName = $("<p/>");
        //        $pCompanyName.append($("<p/>", { "html": company.CompanyName }));
        //        if (company.SubscriberId == subscriberId) {
        //            // show request access

        //        }
        //    }
        //}

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
            $tdSalesTeam.append($("<p/>", { "class": "", "html": "<span class='text-success FontSize12 m-l-sm'>Sales Team</span>" + company.SalesTeam }));
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

        //$tdActions.append($("<span/>", { "html":"hasPermission:" + hasPermission + " ~ "}));
        //$tdActions.append($("<span/>", { "html": "accessibleCompanyIds:" + accessibleCompanyIds.length + " ~ " }));
        //$tdActions.append($("<span/>", { "html": "ComSubscriberId:" + company.SubscriberId + " ~ " }));

        $tr.append($tdActions);

        return $tr;

    };

};


// onClick options list 
$(document).ready(function () {
    $('.panel-dropdown .ae-select-content').html($('.panel-dropdown .dropdown-nav > li.selected').html());
    var newOptions = $('.panel-dropdown .dropdown-nav > li');
    newOptions.click(function () {
        $('.panel-dropdown .ae-select-content').html($(this).html());
        $('.panel-dropdown .dropdown-nav > li').removeClass('selected');
        $(this).addClass('selected');
        currentPage = 1;
        new Companies().RetrieveCompanies();
    });
    var aeDropdown = $('.panel-dropdown .ae-dropdown');
    aeDropdown.click(function () {
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
	
	$('a.dotsbtn').click(function()
	{
		if($(this).hasClass('active')){
			$('#moreFilters').hide();
			$(this).removeClass('active');
		}
		else{
			$('#moreFilters').show();
			$(this).addClass('active');
		}
	});
});
