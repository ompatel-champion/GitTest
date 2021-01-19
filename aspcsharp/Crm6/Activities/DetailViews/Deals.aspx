<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Deals.aspx.cs" Inherits="Crm6.Activities.DetailViews.Deals" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Deals</title>
    <link href="/_content/_css/bundle/bootstrap.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/animate.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/style-07-apr-2020.css " rel="stylesheet" />

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
            <div class="list-table">
                <table class="table table-deals MB0" id="tblDeals">
                    <tbody>
                        <asp:Repeater ID="rptDeals" runat="server">
                            <ItemTemplate>

                                <tr class="tr-deal">
                                    <td class="">
                                        <p class="MB0"><%#  Eval("CompanyName")%></p>
                                        <a target="_blank" href="/Deals/DealDetail/dealdetail.aspx?dealId=<%# Eval("DealId")%>&dealsubscriberId=<%# Eval("SubscriberId")%>">
                                            <%#  Eval("DealName")%>
                                        </a>
                                        <p class="MB0 FontSize10 text-muted">
                                            <%#  string.IsNullOrEmpty( Eval("WonLostDateStr").ToString()) ? "" : 
                                                    ((Eval("SalesStageName").ToString() == "Won" ? "Won Date: " : (Eval("SalesStageName").ToString() == "Lost"  ? "Lost Date":""))  
                                                                                     + (Eval("WonLostDateStr") +  ( Eval("WonLostReason") is DBNull ||  Eval("WonLostReason").ToString() == "Select Reason" ? "": (" | " + Eval("WonLostReason")))))  %>
                                        </p>
                                    </td>
                                    <td class="text-left">
                                        <p class="MB5 FontSize10 text-muted">Next Activity: <%#  Eval("NextActivityDateStr")%></p>
                                        <p class="MB5 FontSize10 text-muted">Last Activity: <%#  Eval("LastActivityDateStr")%></p>
                                        <p class="MB5 FontSize10 text-muted">Created: <%#  Eval("CreatedDateStr")%></p>

                                    </td>
                                    <td>
                                        <label class="label label-success FontSize8"><%#  Eval("SalesStageName")%></label>
                                    </td>
                                    <td class="text-right W130 hide">
                                        <p class="">
                                            <em class="text-navy m-r-xs FontSize8">Revenue</em><br>
                                            <span></span><span data-type="deal-revenue">
                                                <%#  String.Format("{0:N}", float.Parse( Eval("Revenue").ToString()))   %>
                                            </span>
                                        </p>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>

    </form>

    <script src="/_content/_js/jquery.metisMenu.js"></script>
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>

    <script>
        $(function () {
            var qr = getQueryString("qr");
            var status = getQueryString("status");
            var $tblDeals = $("#tblDeals");
            var $tbody = $tblDeals.find("tbody");

            var url = "";
            switch (status) {
                case "Won":
                    url = "/api/Activity/GetActivityChartWonDeals";
                    break;
                case "Lost":
                    url = "/api/Activity/GetActivityChartLostDeals";
                    break;
                case "New":
                    url = "/api/Activity/GetActivityChartNewDeals";
                    break;
                default:
            }

            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: qr,
                success: function (deals) {
                    $tbody.html("");
                    removeSpinner();
                    $.each(deals, function (i, ele) {
                        var $tr = $("<tr/>");
                        var $td = $("<td/>");
                        $td.append($("<p class=\"MB0\">" + ele.CompanyName + "</p>"));
                        $td.append($("<a target=\"_blank\" href=\"/Deals/DealDetail/dealdetail.aspx?dealId=" + ele.DealId + "&dealsubscriberId=" + ele.SubscriberId + "\"><span class=\"MB0 FontSize12\">" + ele.DealName + "</span></a>"));

                        var $p = $("<p/>", { "class": "MB0 FontSize10 text-muted" });
                        var str = "";
                        if (ele.WonLostDateStr && ele.WonLostDateStr !== "") {
                            str = (ele.SalesStageName === "Won" ? "Won Date: " : "Lost Date: ") + moment(ele.WonLostDateStr, 'DD-MMM-YY').format("DD-MMM-YY") ;
                        }
                        if (ele.WonLostReason !== "" && ele.WonLostReason !== "Select Reason") {
                            str += " | " + ele.WonLostReason;
                        }
                        $p.html(str);
                        $td.append($p);
                        $tr.append($td);

                        // dates
                        var $td = $("<td/>", { "class": "text-left" });
                        if (ele.NextActivityDateStr && ele.NextActivityDateStr !== "" && ele.NextActivityDateStr !== "-") {
                            $td.append($("<p class=\"FontSize10 text-muted\">Next Activity: " + ele.NextActivityDateStr + "</p>"));
                        }
                        if (ele.LastActivityDateStr && ele.LastActivityDateStr !== "" && ele.LastActivityDateStr !== "-") {
                            $td.append($("<p class=\"FontSize10 text-muted\">Last Activity: " + ele.LastActivityDateStr + "</p>"));
                        }
                        if (ele.CreatedDateStr && ele.CreatedDateStr !== "" && ele.CreatedDateStr !== "-") {
                            $td.append($("<p class=\"FontSize10 text-muted\">Created: " + moment(ele.CreatedDateStr, 'DD-MMM-YY').format("DD-MMM-YY")  + "</p>"));
                        }
                        $tr.append($td);

                        // sales stage
                        var $td = $("<td/>", { "html": "<label class=\"label label-success FontSize8 MB0\">" + ele.SalesStageName + "</label>" });
                        $tr.append($td);

                        // revenue
                        var $td = $("<td/>", { "class": "text-right W130 hide" });
                        var $p = $("<p/>");
                        $p.append($("<em class=\"text-navy m-r-xs FontSize8\">Revenue</em>"));
                        $p.append($("<br/>"));
                        $p.append($("<span data-type=\"deal-revenue\">" + formatNumber(ele.Revenue) + "</span>"));
                        $td.append($p);
                        $tr.append($td);
                        $tbody.append($tr);
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
