var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var companyId = $("#lblCompanyId").text();
var $ddlUser = $("#ddlUser");

$(function () {

    $("#btnReassign").unbind("click").click(function () {
        new ReassignCompany().Reassign();
    });

    $("#btnCancel").unbind("click").click(function () {
        var url = "/Companies/CompanyList/CompanyList.aspx";
        location.href = url;
    });
    new ReassignCompany().Init();
});


var ReassignCompany = function () {
    var self = this;

    this.Init = function () {

        $ddlUser.select2({
            placeholder: translatePhrase("Select User"),
            height: "150px",
            ajax: {
                url: function (obj) {
                    var keyword = obj.term ? obj.term : "";
                    return "/api/dropdown/GetUsers?subscriberId=" + subscriberId + "&keyword=" + keyword;
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

    this.Reassign = function () {
        $ddlUser.closest(".form-group").find(".error").html("");
        if ($ddlUser.val() !== null && $ddlUser.val() !== "" && $ddlUser.val() !== "0") {
            var assignedUserId = parseInt($ddlUser.val());

            $.ajax({
                type: "GET",
                url: "/api/company/ReassignCompany/?companyid=" + companyId + "&subscriberId=" + subscriberId
                + "&userId=" + assignedUserId + "&assignedBy=" + userId,
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    swal.close();
                    var url = "/Companies/CompanyList/CompanyList.aspx";
                    location.href = url;
                }, beforeSend: function () {
                    swal({ text: translatePhrase("Please wait...") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: false });
                }, error: function (request, status, error) {
                    alert(JSON.stringify(request))
                }
            });

        } else {
            $ddlUser.closest(".form-group").find(".error").html("Please select a user.");
        }

    }
}

 