// set global values
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var dateFormat = $("#lblDateFormat").text();
var currentPage = 1;
var $divNoItems = $("#divNoItems");
var $companies = $("#tblCompaniesReport>tbody");
var $datepickers = $("[data-name='datepicker']");
var $tblCompaniesReport = $("#tblCompaniesReport");
var $divReportContent = $("#divReportContent");
var datatableIntiated = false;

$(function () {
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
    };

    this.InitSelect2DropDowns = function () {
        $("#ddlCampaign").select2({ allowClear: true, placeholder: translatePhrase(" ") });
        $("#ddlCompetitor").select2({ allowClear: true, placeholder: translatePhrase(" ") });
        $("#ddlCountry").select2({ allowClear: true, placeholder: translatePhrase(" ") });
        $("#ddlIndustry").select2({ allowClear: true, placeholder: translatePhrase(" ") });
        $("#ddlSource").select2({ allowClear: true, placeholder: translatePhrase(" ") });
        $("#ddlStatus").select2({allowClear: true, placeholder: translatePhrase("") });
    };

    this.RunReport = function () {
        currentPage = 1;
        new Companies().RetrieveCompanies();
    };

    this.PrintReport = function () {
        // print report function goes here
    };

};


var Companies = function () {
    var self = this;
    var reportItemHtml = "";

    // get filters
    this.GetFilter = function () {
        // set the filters
        var filters = new Object();
        filters.SubscriberId = subscriberId;
        filters.UserId = userId;
        filters.Country = $("#ddlCountry :selected").val();
        filters.Industry = $("#ddlIndustry :selected").text();
        filters.Campaign = $("#ddlCampaign :selected").text();
        filters.Competitor = $("#ddlCompetitor :selected").text();
        filters.Source = $("#ddlSource :selected").text();
        filters.Status = $("#ddlStatus :selected").text();

        return filters;
    };

    // retrieve companies
    this.RetrieveCompanies = function () {
        // set the filters
        var filters = self.GetFilter();
        $divReportContent.addClass('hide');
        $companies.html("");
        $("#btnExcel").addClass("hide");

        // AJAX to retrieve companies
        $.ajax({
            type: "POST",
            url: "/api/Report/GetCompaniesReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 300000,
            data: JSON.stringify(filters),
            success: function (companiesResponse) {
                removeSpinner();
                if (companiesResponse.Companies.length > 0) {

                    self.BindCompanies(companiesResponse.Companies);

                    $(".total-record-count").html('<span>'+companiesResponse.Companies.length + " companies found"+'</span>');

                    // set the table as a data table 
                    $divReportContent.removeClass("hide");

                    // excel
                    $("#btnExcel").removeClass("hide");
                    $("#btnExcel").unbind("click").click(function () {
                        location.href = companiesResponse.ExcelUri;
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
    this.BindCompanies = function (companies) {
        // get template for company row
        reportItemHtml = self.GetReportRowHtml();
        // iterate and bind companies
        $.each(companies, function (i, company) {
            var $tr = self.GetCompanyItemHtml(company);
            $companies.append($tr);
        });
    };

    // get company row html
    this.GetCompanyItemHtml = function (company) {
        var $tr = $(reportItemHtml);
        $tr.attr("data-id", company.CompanyId);
        // bind company details
        $tr.find("[data-name='company']").html(company.Company);
        $tr.find("[data-name='city']").html(company.City);
        $tr.find("[data-name='country']").html(company.Country);
        $tr.find("[data-name='industry']").html(company.Industry);
        $tr.find("[data-name='source']").html(company.Source && company.Source != "0" ? company.Source : "");
        $tr.find("[data-name='customerstatus']").html(company.Status);
        $tr.find("[data-name='competitor']").html(company.Competitor);
        $tr.find("[data-name='campaign']").html(company.Campaign);
        $tr.find("[data-name='telephone']").html(company.Telephone);
        $tr.find("[data-name='fax']").html(company.Fax);
        $tr.find("[data-name='address']").html(company.Address);
        $tr.find("[data-name='createddate']").html(moment(company.CreatedDate).format("MMMM DD, YYYY"));
        $tr.find("[data-name='lastactivity']").html(moment(company.LastActivity).format("MMMM DD, YYYY"));

        return $tr;
    };

    // get activity by date range row html template
    this.GetReportRowHtml = function () {
        var html = "";
        $.ajax({
            cache: false,
            async: false,
            url: "/_templates/CompaniesReport.html" + "?q=" + $.now(),
            success: function (data) {
                html = data;
            }
        });
        return html;
    };
};

function printTable() {
    var divToPrint = document.getElementById("tblCompaniesReport");
    newWin = window.open("");
    newWin.document.write(divToPrint.outerHTML);
    newWin.print();
    newWin.close();
}
