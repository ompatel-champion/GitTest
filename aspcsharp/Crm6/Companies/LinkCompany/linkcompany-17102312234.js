var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var companyId = $("#lblCompanyId").text();
var $ddlCompany = $("#ddlCompany");
var $ddlLinkTypes = $("#ddlLinkTypes");

$(function () {
    new LinkedCompany().Init();
});


var LinkedCompany = function () {
    var self = this;

    this.Init = function () {
        self.InitSaveValidator();
         
        $ddlLinkTypes.select2({ theme: "classic" });
        $ddlCompany.select2({
            minimumInputLength: 1,
            theme: "classic",
            placeholder: translatePhrase("Select Company"),
            height: "150px",
            ajax: {
                url: function (obj) {
                    var keyword = obj.term ? obj.term : "";
                    return "/api/dropdown/GetCompanies?subscriberId=" + subscriberId + "&keyword=" + keyword;
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
                            }
                        })
                    };
                }
            }
        });
    }

    this.InitSaveValidator = function () {

        // select2 dropdown validator
        $ddlCompany.on('select2:select', function (evt) { $(this).valid(); });
        $ddlLinkTypes.on('select2:select', function (evt) { $(this).valid(); }); 

        // add the rule here
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return (arg != value && value !== null) && value !== null && value !== "0" && value !== "";
        }, "");

        // validate
        $("#divLinkCompanySetup").validate({
            rules: { 
                ddlCompany: { valueNotEquals: "null" },
                ddlLinkTypes: { valueNotEquals: "null" }, 
            }, messages: { 
                ddlCompany: translatePhrase("Please select a Company"),
                ddlLinkTypes: translatePhrase("Please select a Link Type"), 
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveLinkedCompany();
            }
        });
    }

    this.SaveLinkedCompany = function () { 

        var company = new Object();
        company.UpdateUserId = userId;
        company.SubscriberId = subscriberId;
        company.CompanyId = companyId;
        company.LinkedCompanyId = $ddlCompany.val();
        company.LinkType = $ddlLinkTypes.find("option:selected").text();

        $.ajax({
            type: "POST",
            url: "/api/company/linkcompany",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(company),
            success: function (resposne) {
                parent.LoadLinkedCompanies();
            }, beforeSend: function () {
            }, error: function (request, status, error) { }
        });
    }
}


function SaveLinkedCompany() {
    $("#divLinkCompanySetup").submit(); 
}