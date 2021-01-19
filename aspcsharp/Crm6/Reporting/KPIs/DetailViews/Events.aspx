<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Events.aspx.cs" Inherits="Crm6.Reporting.KPIs.DetailViews.Events" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Events</title>

    <style>
        body {
            background-color: #ffffff !important;
        }

        #intercom-container {
            display: none !important;
        }
    </style>

</head>

<body class="">
    <form runat="server">
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

        <div class="ibox ibox-content P0">
            <div class="col-md-12  ">
                <div class="inspinia-timeline P10">
                    <asp:Repeater ID="rptEvents" runat="server">
                        <ItemTemplate>
                            <div class="PB0">
                                <div class="media-body">
                                    <small class="text-muted pull-right">
                                        <%# Convert.ToDateTime( Eval("ActivityDate").ToString()).ToString("dd MMM, yyyy @ HH:mm")  %>  
                                    </small>
                                    <strong class="MB5"><%#  Eval("Subject")%></strong>
                                    <div class="clearfix MB5"></div>
                                    <p class="FontSize10 MB5"><%# string.IsNullOrEmpty(Eval("DealNames") + "") ? "" : "Deals: " +  Eval("DealNames")%></p>
                                    <p class="FontSize10 MB5"><%# string.IsNullOrEmpty( Eval("CompanyName") + "") ? "" : "Company: " +  Eval("CompanyName")%></p>
                                    <p class="FontSize10 MB5"><%# string.IsNullOrEmpty( Eval("ContactNames") + "") ? "" : "Contacts: " +  Eval("ContactNames")%></p>
                                </div>
                                <hr />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>

    </form>

    <script src="/_content/_js/jquery.metisMenu.js"></script>
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>

</body>
</html>
