// set global values
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var dateFormat = $("#lblDateFormat").text();
var currentPage = 1;
var $divNoItems = $("#divNoItems");
var $userActivity = $("#tblUserActivityReport>tbody");
var $datepickers = $("[data-name='datepicker']");
var $tblUserActivityReport = $("#tblUserActivityReport");
var $divReportContent = $("#divReportContent");
var datatableIntiated = false;

$(function () {

    // company 
    $("#ddlCompanies").select2({
        minimumInputLength: 2,
        allowClear: true,
        theme: "classic",
        placeholder: '',
        ajax: {
            url: function (obj) {
                var keyword = obj.term ? obj.term : "";
                return "/api/AutoComplete/?type=company&SusbcriberId=" + subscriberId + "&prefix=" + keyword.replace("&", "%26");
            },
            dataType: "json",
            timeout: 50000,
            type: "GET",
            data: '',
            processResults: function (data) {
                var o = new Object();
                o.name = "";
                o.id = "";
                //  data.push(o);
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
    $("#ddlDeals").select2({
        minimumInputLength: 0,
        allowClear: true,
        theme: "classic",
        placeholder: '',
        ajax: {
            url: function (obj) {
                var keyword = obj.term ? obj.term : "";
                return "/api/AutoComplete/?type=deal&SusbcriberId=" + subscriberId + "&prefix=" + keyword;
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
        }
    });

    // contacts
    $("#ddlContacts").select2({
        minimumInputLength: 0,
        allowClear: true,
        theme: "classic",
        placeholder: '',
        ajax: {
            url: function (obj) {
                var keyword = obj.term ? obj.term : "";
                return "/api/AutoComplete/?type=contact&SusbcriberId=" + subscriberId + "&prefix=" + keyword;
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
        }
    });


    // initialize report
    new Report().Init();
});


var Report = function () {
    var self = this;

    this.Init = function () {
        self.InitSelect2DropDowns();
        // fix for form controls inside drop down menu
        $(".dropdown-menu").click(function (e) {
            e.stopPropagation();
        });
        // run report action
        $("#btnRunReport").unbind("click").click(function (e) {
            e.preventDefault();
            self.RunReport();
        });

         

        $datepickers.datepicker({
            dateFormat: "dd MM, yy",
            format: "ddd, DD MMMM, YYYY",
            autoclose: true
        }).on('changeDate', function () {
        });

        $datepickers.on('focus', function (e) {
            e.preventDefault();
            $(this).attr("autocomplete", "off");
        });

        // default dates  
        $("#txtDateFrom").val(moment().add(-1, 'months').format("ddd, DD MMMM, YYYY"));
        $("#txtDateTo").val(moment().format("ddd, DD MMMM, YYYY")); 

    };

    this.InitSelect2DropDowns = function () {
        $("#ddlUsers").select2({ theme: "classic", allowClear: true, placeholder: "" }); 
    };

    this.RunReport = function () {
        currentPage = 1;
        new UserActivity().RetrieveUserActivity();
    };

    this.PrintReport = function () {
        // print report function goes here
    };

};


var UserActivity = function () {
    var self = this;
    var reportItemHtml = "";

    // get filters
    this.GetFilter = function () {
        // set the filters
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.UserId = userId;
        filters.DateFrom = $("#txtDateFrom").val();
        filters.DateTo = $("#txtDateTo").val();
        filters.UserIds = [];
        filters.CompanyIds = [];
        filters.ContactIds = [];
        filters.DealIds = [];

        $("#ddlUsers :selected").each(function (i, selected) {
            var user = $(selected).val();
            if (user !== "0") {
                filters.UserIds.push(user);
            }
        });

        $("#ddlDeals :selected").each(function (i, selected) {
            var user = $(selected).val();
            if (user !== "0") {
                filters.DealIds.push(user);
            }
        });

        $("#ddlCompanies :selected").each(function (i, selected) {
            var user = $(selected).val();
            if (user !== "0") {
                filters.CompanyIds.push(user);
            }
        });

        $("#ddlContacts :selected").each(function (i, selected) {
            var user = $(selected).val();
            if (user !== "0") {
                filters.ContactIds.push(user);
            }
        });

        return filters;
    };

    // retrieve companies
    this.RetrieveUserActivity = function () {
        // set the filters
        var filters = self.GetFilter();
        $divReportContent.addClass('hide');
        $userActivity.html("");
        $("#btnExcel").addClass("hide");

        // AJAX to retrieve companies
        $.ajax({
            type: "POST",
            url: "/api/Report/GetUserActivityReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 300000,
            data: JSON.stringify(filters),
            success: function (userActivityResponse) {
                removeSpinner();
                if (userActivityResponse.Activities.length > 0) {

                    self.BindUserActivity(userActivityResponse.Activities);

                    $(".total-record-count").html("<span>"+userActivityResponse.Activities.length + " user activity found"+"</span>");

                    // set the table as a data table 
                    $divReportContent.removeClass("hide");

                    // excel
                    $("#btnExcel").removeClass("hide");
                    $("#btnExcel").unbind("click").click(function () {
                        location.href = userActivityResponse.ExcelUri;
                    });

                    $divNoItems.addClass('hide');
                } else {
                    $divNoItems.removeClass('hide');
                }
            },
            beforeSend: function () {
                addSpinner();
            },
            error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    // bind companies
    this.BindUserActivity = function (userActivities) {
        // get template for company row
        reportItemHtml = self.GetReportRowHtml();
        // iterate and bind userActivities
        $.each(userActivities, function (i, userActivity) {
            var $tr = self.GetUserActivityItemHtml(userActivity);
            $userActivity.append($tr);
        });
    };

    // get company row html
    this.GetUserActivityItemHtml = function (userActivity) {
        var $tr = $(reportItemHtml);
        $tr.attr("data-id", userActivity.UserId);
        // bind details
        $tr.find("[data-name='username']").html(userActivity.UserName);
        $tr.find("[data-name='useractivitymessage']").html(userActivity.UserActivityMessage);
        $tr.find("[data-name='calendareventsubject']").html(userActivity.CalendarEventSubject);
        $tr.find("[data-name='companyname']").html(userActivity.CompanyName);
        $tr.find("[data-name='contactname']").html(userActivity.ContactName);
        $tr.find("[data-name='dealname']").html(userActivity.DealName);
        $tr.find("[data-name='notecontent']").html(userActivity.NoteContent);
        $tr.find("[data-name='taskname']").html(userActivity.TaskName);
        $tr.find("[data-name='useractivitytimestamp']").html(moment(userActivity.UserActivityTimestamp).format("MMMM DD, YYYY"));

        return $tr;
    };

    this.GetReportRowHtml = function () {
        var html = "";
        $.ajax({
            cache: false,
            async: false,
            url: "/_templates/UserActivityReport.html" + "?q=" + $.now(),
            success: function (data) {
                html = data;
            }
        });
        return html;
    };
};

function printTable() {
    var divToPrint = document.getElementById("tblUserActivityReport");
    newWin = window.open("");
    newWin.document.write(divToPrint.outerHTML);
    newWin.print();
    newWin.close();
}


function getSelectedCompanyId() {
    try {
        return $("#ddlCompanies").val();
    } catch (e) {
        // ignore
    }
    return -100;
}


// show the filters
$(document).ready(function () {
    $("#divReportFilter").removeClass("hide");
});
