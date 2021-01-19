// set global values
var recordsPerPage = 300;
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var currentPage = 1;
var $divUsers = $("#divUsers");
var $userCards = $(".user-cards");
var $userTable = $(".user-table");
var viewType = $(".btn-biew-type .btn-black").attr('data-view-type');
var $tblUsers = $("#tblUsers");
var tableSortOrder = "username asc";

$(function () {
    new Users().Init();
});

var Users = function () {
    var self = this;

    this.Init = function () {
        // retrieve users
        currentPage = 1;
        $("#ddlCountry").select2({ placeholder: "Country" });
        $("#ddlCountry").change(function () {
            currentPage = 1;
            self.RetrieveUsers();
        });
        // new user
        $(".new-user").unbind('click').click(function () {
            self.GoToAddEditUser(0);
        });
        // view type actions
        $(".btn-view-type button").unbind('click').click(function () {
            // set active button
            $(".btn-view-type button").removeClass('btn-black').addClass('btn-white');
            $(this).removeClass('btn-white').addClass('btn-black');
            // retrieve users
            self.RetrieveUsers();
        });
        $(".btn-view-type button[data-view-type='table']").click();
        // search button
        $("#btnSearch").unbind("click").click(function () {
            currentPage = 1;
            self.RetrieveUsers();
        });
        // login enabled/disabled toggle
        $(".toggle-switch-login-enabled").unbind("change").change(function () {
            currentPage = 1;
            self.RetrieveUsers();
        });
        $('.dd-menu-item-login-enabled').on('click keydown', function (e) {
            if (!e.keyCode || e.keyCode == 13) {
                if (!$(e.target).closest('input').get(0) && !$(e.target).hasClass('toggle-switch-slider')) {
                    var inputElm = $('.toggle-switch-login-enabled input');
                    $(".toggle-switch-login-enabled input").prop('checked', !inputElm.is(":checked"));
                    inputElm.trigger("change");
                }
            }
        });
        // export excel
        $(".dd-menu-item-export").unbind("click").click(function () {
            self.ExportToExcel();
        });
        // sort - table header click
        new UserTable().InitSort();
        //add options menu
        self.menu.add('options');
    };

    this.SetViewType = function () {
        // toggle the view
        var viewType = $(".btn-view-type .btn-black").attr('data-view-type');
        if (viewType === 'table') {
            $userCards.addClass('hide');
            $userTable.removeClass('hide');
        } else {
            $userCards.removeClass('hide');
            $userTable.addClass('hide');
        }
    };

    // export to excel
    this.ExportToExcel = function () {
        {
            self.SetViewType();

            // set filters
            var filters = new Object();
            filters.SubscriberId = subscriberId;
            filters.CurrentPage = currentPage;
            filters.RecordsPerPage = 2000;
            filters.LoginEnabled = $('.toggle-switch-login-enabled input').is(":checked");
            filters.Keyword = $("#txtKeyword").val();
            filters.ShowAdmin = true;
            filters.SortBy = "firstname asc";
            filters.SortBy = tableSortOrder;
            filters.UserId = userId;
            if ($("#ddlCountry").val() !== 'Country')
                filters.CountryName = $("#ddlCountry").val();

            // clear rows
            //$userCards.html("");
            //var $tbody = $tblUsers.find("tbody");
            //$tbody.html("");

            // AJAX to retrieve users
            $.ajax({
                type: "POST",
                url: "/api/user/ExportToExcel",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(filters),
                success: function (resultLink) {
                    removeSpinner();
                    // download the file
                    window.location = resultLink;
                },
                beforeSend: function () {
                    addSpinner();
                },
                error: function () {
                }
            });
        }
    };

    // retrieve users
    this.RetrieveUsers = function () {
        self.SetViewType();

        // set the filters
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.CurrentPage = currentPage;
        filters.RecordsPerPage = 50;
        filters.LoginEnabled = $('.toggle-switch-login-enabled input').is(":checked");
        filters.Keyword = $("#txtKeyword").val();
        filters.ShowAdmin = false;
        filters.SortBy = "firstname asc";
        filters.SortBy = tableSortOrder;
        filters.UserId = userId;
        if ($("#ddlCountry").val() !== 'Country')
            filters.CountryName = $("#ddlCountry").val();

        // clear the rows
        $userCards.html("");
        var $tbody = $tblUsers.find("tbody");
        $tbody.html("");

        // AJAX to retrieve users
        $.ajax({
            type: "POST",
            url: "/api/user/GetUsers",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(filters),
            success: function (users) {
                removeSpinner();
                $(".total-records").html(formatNumber(users.length) + " users");
                if (users.length > 0) {
                    // bind users
                    var viewType = $(".btn-view-type .btn-black").attr('data-view-type');
                    if (viewType === 'table') {
                        new UserTable().BindUsers(users);
                    } else if (viewType === 'card') {
                        new UserCards().BindUsers(users);
                    }
                    // show users div
                    $divUsers.removeClass('hide');
                } else {
                    // show users div
                    $divUsers.removeClass('hide');
                }
            },
            beforeSend: function () {
                // loading spinner
                addSpinner();
            },
            error: function () {
            }
        });
    };

    // open add/edit user
    this.GoToAddEditUser = function (userId) {
        var url = "/Admin/Users/UserAddEdit/UserAddEdit.aspx?userId=" + userId;
        location.href = url;
    };

    // open user profile
    this.GoToUserProfile = function (userId) {
        var url = "/Admin/Users/UserDetail/UserDetail.aspx?userId=" + userId;
        location.href = url;
    };

    this.DeleteUser = function (uid, $ele) {
        swal({
            title: translatePhrase("Delete User!"),
            text: translatePhrase("Are you sure you want to delete this user?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        },
            function () {
                $.ajax({
                    type: "GET",
                    url: "/api/user/DeleteUser/?userId=" + uid + "&loggedInUserId=" + userId,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data) {
                            self.RetrieveUsers();
                        }
                        removeSpinner();
                    },
                    beforeSend: function () {
                        addSpinner();
                    }
                });
            });
    };

    // menu object (options, etc...) note: to make a menu item sticky (wont close the menu when clicking) add the 'data-menu-sticky' attribute to the menu item element
    this.menu = new function () {

        var self = this;
        this.active = {};

        //initialize
        (function () {
            $(document).click(function (e) {
                if ((!$(e.target).closest('[data-menu-name],[data-menu-sticky]').get(0)) && (Object.keys(self.active).length)) {
                    for (var i in self.active) _toggle(self.active[i]);
                }
            });
        })();

        //add a menu by name (e.g. 'options')
        this.add = function (name) {
            var $elm = $(".btn-bar-btn-" + name);
            $elm.attr("data-menu-name", name);
            var menuElm = $('.btn-bar-btn-' + name + ' dd-menu'); if (menuElm) {
                // options
                $elm.unbind('click').click(function (e) {
                    for (var i in self.active) {
                        if (self.active[i].attr('data-menu-name') !== name) _toggle(self.active[i]);
                    }
                    if (!$(e.target).closest('[data-menu-sticky]').get(0)) _toggle($(this));
                });
                $elm.keydown(function (e) {
                    if (e.keyCode === 13 && !$(e.target).closest('[data-menu-sticky]').get(0)) _toggle($(this));
                });
            }
        };

        //toggle a menu (provide the elm or the name of the menu, e.g. 'options')
        function _toggle($elm) {
            if (typeof $elm === "string") $elm = $('.btn-bar-btn-' + name);
            var name = $elm.attr('data-menu-name');
            var isActive = $elm.hasClass('btn-bar-btn-active');
            $elm[(isActive ? "remove" : "add") + "Class"]('btn-bar-btn-active');
            if (isActive) delete self.active[name]; else self.active[name] = $elm;
        }
    };
};

var UserTable = function () {
    var self = this;
    var $tbody = $tblUsers.find("tbody");
    $tbody.html("");

    // iterate and bind users
    this.BindUsers = function (users) {
        $.each(users, function (i, user) {
            var $tr = self.GetUserItemHtml(i, user);
            $tbody.append($tr);
        });
    };

    // get user item html
    this.GetUserItemHtml = function (i, user) {

        var $tr = $("<tr/>", { "data-id": user.UserId });

        // user name
        var $tdUserName = $("<td/>");
        var userDetailPageUrl = "/Admin/Users/UserDetail/UserDetail.aspx?userId=" + user.UserId;
        var $aUserName = $("<a/>",
            {
                "class": " hover-link",
                "href": userDetailPageUrl,
                "html": user.FirstName + " " + user.LastName
            });
        $tdUserName.append($aUserName);
        $tr.append($tdUserName);

        // email address
        var $tdEmail = $("<td/>");
        if (user.EmailAddress && user.EmailAddress !== '') {
            $tdEmail.append($("<p />",
                {
                    "class": "m-t-xs",
                    "html": "<a class='user-email hover-link ' href='mailto:" +
                        user.EmailAddress +
                        "'>" +
                        (user.EmailAddress.replace(/\@/, "@<wbr>")) +
                        "</a>"
                }));
        }
        $tr.append($tdEmail);

        // location
        var $tdLocationName = $("<td/>");
        $tdLocationName.append($("<p />", { "html": user.LocationName }));
        $tr.append($tdLocationName);

        // country
        var $tdCountry = $("<td/>");
        $tdCountry.append($("<p />", { "html": user.CountryName }));
        $tr.append($tdCountry);

        // job title
        var $tdJobtitle = $("<td/>");
        if (user.Title && user.Title !== '') {
            $tdJobtitle.append($("<p />", { "html": user.Title }));
        } else {
            $tdJobtitle.append($("<span />", { "html": "-" }));
        }
        $tr.append($tdJobtitle);

        var date = new Date(user.LastLoginDate);

        const monthNames = ["January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        ];

        var dateString = date.getDate() + "-" + monthNames[date.getMonth()].substr(0, 3) + "-" + date.getFullYear().toString().substr(2, 2) + " " + date.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true });

        // last login
        var $tdLastLogin = $("<td/>");
        if (user.LastLoginDate && user.LastLoginDate !== '') {
            var $lastLoginDate = $("<p />",
                {
                    "html": "<a class='a-phone-number text-muted'" +
                        dateString +
                        "'>" +
                        dateString +
                        "</a>"
                });
            $tdLastLogin.append($lastLoginDate);
        } else {
            $tdLastLogin.append($("<span />", { "html": "-" }));
        }
        $tr.append($tdLastLogin);

        // actions
        var $tdActions = $("<td/>", {
            "class": "action-cell"
        });
        var $linkEdit = $("<a/>", {
            "html": 'View',
            "title": "View User",
            "class": "hover-link",
            "href": "javascript:void(0);"
        });
        $tdActions.append($linkEdit);
        $linkEdit.unbind('click').click(function () {
            new Users().GoToUserProfile(user.UserId);
        });
        $tr.append($tdActions);
        return $tr;
    };

    this.InitSort = function () {
        var $tbl = $("#tblUsers");

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

                // do the search again 
                new Users().RetrieveUsers();
            }
        });
    };
};

var UserCards = function () {
    var self = this;

    // bind users
    this.BindUsers = function (users) {
        // iterate and bind users
        var $mainRow;
        $.each(users,
            function (i, user) {
                if (i % 3 === 0) {
                    $mainRow = $("<div/>", { "class": "row" });
                    $userCards.append($mainRow);
                }
                var $column = self.GetUsertemHtml(i, user);
                $mainRow.append($column);
            });
    };

    // get user box html
    this.GetUsertemHtml = function (i, user) {

        var userDetailPageUrl = "/Admin/Users/UserDetail/UserDetail.aspx?userId=" + user.UserId;

        var $column = $("<div/>", { "class": "col-lg-4 PR5 PL5" });
        $column.unbind('click').click(function (e) {
            // edit
            var $edit = $(this).find(".btn-edit");
            if (e.target === $edit[0]) return false;
            // delete
            var $delete = $(this).find(".btn-delete");
            if (e.target === $delete[0]) return false;
            // email
            var $email = $(this).find(".a-email");
            if (e.target === $email[0]) return false;
            // phone
            var $phoneNumber = $(this).find(".a-phone-number");
            if (e.target === $phoneNumber[0]) return false;

            // go to user detail page
            location.href = userDetailPageUrl;
            return true;
        });

        var $userBox = $("<div/>", { "class": "contact-box P10" });
        var $aUser = $("<a/>", { "href": "javascript:void(0)" });

        // icons
        var $divActions = $("<div/>", { "class": "m-t-sm btn-group pull-right" });
        var $btnEdit = $("<a/>",
            {
                "class": "btn btn-xs btn-white btn-edit",
                "href": "javascript:void(0)",
                "html": "<i class='fa fa-pencil text-primary'></i>"
            });
        $divActions.append($btnEdit);
        $btnEdit.unbind('click').click(function () {
            new Users().GoToAddEditUser(user.UserId);
        });
        var $btnDelete = $("<a/>",
            {
                "class": "btn btn-xs btn-white btn-delete",
                "href": "javascript:void(0)",
                "html": "<i class='fa fa-trash text-danger'></i>"
            });
        $divActions.append($btnDelete);
        $btnDelete.unbind('click').click(function () {
            new Users().DeleteUser(user.UserId);
        });

        // right column
        var $divRight = $("<div/>", { "class": "col-sm-12 PT20" });

        $divRight.append($divActions);

        var $h3 = $("<h3/>", { "html": "<strong>" + user.FirstName + " " + user.LastName + "</strong>" });
        $divRight.append($h3);

        // job title
        if (user.Title && user.Title !== '') {
            $divRight.append($("<p />", { "html": user.Title }));
        }

        // phone
        if (user.Phone && user.Phone !== '') {
            var $userPhone = $("<p />",
                {
                    "html": "<a class='text-muted a-phone-number' href='tel:" +
                        user.Phone +
                        "'><i class=\"text-navy fa fa-phone m-t-xs m-r-xs\"></i>" +
                        user.Phone +
                        "</a>"
                });
            $divRight.append($userPhone);
        }

        // email address
        if (user.EmailAddress && user.EmailAddress !== '') {
            var $emailAddress = $("<p />",
                {
                    "html": "<a class='text-muted a-email' href='mailto:" +
                        user.EmailAddress +
                        "'><i class=\"text-navy fa fa-envelope m-r-xs\"></i>" +
                        user.EmailAddress +
                        "</a>"
                });
            $divRight.append($emailAddress);
        }

        $aUser.append($divRight);
        $aUser.append($("<div/>", { "class": "clearfix" }));

        $userBox.append($aUser);
        $column.append($userBox);
        return $column;
    };
};
