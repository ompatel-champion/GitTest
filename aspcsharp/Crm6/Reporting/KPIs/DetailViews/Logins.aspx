<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logins.aspx.cs" Inherits="Crm6.Reporting.KPIs.DetailViews.Logins" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Logins</title>
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
            <div class="col-md-12 PR0 PL0">
                <table class="table table-bordered">
                    <tbody>
                        <asp:Repeater ID="rptLogins" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="text-center W100">
                                        <%# Container.ItemIndex + 1 %>
                                    </td>
                                    <td>
                                        <%#  Convert.ToDateTime( Eval("UserActivityTimestamp")).ToString("dd MMMM, yyyy")  %>
                                    </td>
                                    <td class="W200">
                                        <%#  Convert.ToDateTime( Eval("UserActivityTimestamp")).ToString("HH:mm")  %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>
    </form>

</body>
</html>
