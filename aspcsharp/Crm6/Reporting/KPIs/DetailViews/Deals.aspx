<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Deals.aspx.cs" Inherits="Crm6.Reporting.KPIs.DetailViews.Deals" %>

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
            <div class="col-md-12  ">
                <table class="table table-deals">
                    <tbody>
                        <asp:Repeater ID="rptDeals" runat="server">
                            <ItemTemplate>
                                <tr class="tr-deal">
                                    <td class="">
                                        <strong>
                                            <p class="MB0"><strong><%#  Eval("CompanyName")%></strong></p>
                                        </strong>
                                        <a target="_blank" href="/Deals/DealDetail/dealdetail.aspx?dealId=<%# Eval("DealId")%>&dealsubscriberId=<%# Eval("SubscriberId")%>">
                                            <strong class="MB0 FontSize12"><%#  Eval("DealName")%></strong>
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

</body>
</html>
