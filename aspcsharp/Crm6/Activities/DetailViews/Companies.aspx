<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Companies.aspx.cs" Inherits="Crm6.Activities.DetailViews.Companies" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Deals</title>

    <style>
        body {
            background-color: #ffffff;
        }

        #intercom-container {
            display: none !important;
        }
    </style>

</head>

<body class="white-bg">
    <form runat="server">
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

        <div class="ibox ibox-content P0">
            <div class="">

                <div class="inspinia-timeline" id="divCompanies">
                    <asp:Repeater ID="rptCompanies" runat="server">
                        <ItemTemplate> 
                            <div class="feed-element PB0">
                                <a class="pull-left"><i class="fa fa-file-o text-primary"></i></a>
                                <div class="media-body">
                                    <small class="FontSize10 pull-right MB5 text-navy">Created on  <%#  Eval("CreatedDateStr")  %>  
                                    </small>
                                    <strong class="MB5"><%#  Eval("Note.NoteContent")%></strong>
                                    <div class="clearfix MB5"></div>
                                    <p class="FontSize10 MB5">Deal: <%#  Eval("DealName")%></p>
                                    <p class="FontSize10 MB5">Company: <%#  Eval("CompanyName")%></p>
                                    <p class="FontSize10 MB5">Contact: <%#  Eval("ContactName")%></p>
                                </div>
                                <div class="hr-line-dashed MT10 MB10"></div>
                            </div>

                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>

    </form>

    <script src="/_content/_js/jquery.metisMenu.js"></script>
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>

    <script>

        $(function () {
            var qr = getQueryString("qr");
            var $divCompanies = $("#divCompanies");

            $.ajax({
                type: "POST",
                url: "/api/Activity/GetActivityChartCompanies",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: qr,
                success: function (companies) {
                     removeSpinner();
                    $.each(companies, function (i, ele) {
                        var $feedElement = $("<div/>", { "class": "feed-element PB0" });
                        $feedElement.append($("<a/>", { "class": "pull-left", "html": "<i class=\"fa fa-file-o text-primary\"></i>" }));
                        var $mediaBody = $("<div/>", { "class": "media-body" });
                        $feedElement.append($mediaBody);
                        $mediaBody.append($("<small/>", { "class": "FontSize10 pull-right MB5 text-navy", "html": ele.CreatedDateStr }));
                        $mediaBody.append($("<strong class=\"MB5\">" + ele.CompanyName + "</strong>")); 
                        $divCompanies.append($feedElement);
                    });
                },
                beforeSend: function () {
                    addSpinner();
                },
                error: function () {
                }
            });
        });

    </script>

</body>
</html>
