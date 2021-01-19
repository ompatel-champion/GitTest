<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExchangeSyncErrorLog.aspx.cs" Inherits="Crm6.ExchangeSyncErrorLog" %>

<%@ Register Src="~/_usercontrols/footer.ascx" TagPrefix="uc1" TagName="footer" %>
<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Exchange Sync Error Log</title>
    <link href="exchangesyncerrorlog.css" rel="stylesheet" />
</head>

<body class="skin-1">
    <form runat="server">
        <div id="wrapper">
            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

            <%-- navbar --%>
            <uc1:nav runat="server" ID="nav" />

            <div id="page-wrapper" class="gray-bg">
                <%-- header --%>

                <!--breadcrumb-->
                <div class="row wrapper border-bottom white-bg page-heading">
                    <div class="col-sm-12">
                        <h2>Exchange Sync Error Log</h2>
                        <ol class="breadcrumb">
                            <li>
                                <a href="/Dashboards/SalesRepDashboard/SalesRepDashboard.aspx">Home</a>
                            </li>
                            <li>
                                <a href="/Sync/Sync.aspx">Sync</a>
                            </li>
                            <li class="active">
                                <strong>Exchange Sync Error Log</strong>
                            </li>
                        </ol>
                    </div>
                </div>

                <%-- content --%>
                <div class="wrapper wrapper-content animated fadeInRight row">
                    <div id="divSyncErrorLog">
                        <%-- sync error log list --%>
                        <div class="synclog-table ibox">
                            <div class="ibox-content">
                                <table class="table table-hover" id="tblSyncLog">
                                    <thead>
                                        <tr>
                                            <th>Date</th>
                                            <th>Error Code</th>
                                            <th>Message</th>
                                            <th>Routine</th>
                                            <th>Type</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptSyncLog" runat="server">
                                            <ItemTemplate>
                                                <tr data-id="<%# Eval("SyncErrorLogId") %>">
                                                    <td><%# Eval("ErrorDateTime") %></td>
                                                    <td><%# Eval("ErrorCode") %></td>
                                                    <td><%# Eval("ErrorMessage") %></td>
                                                    <td><%# Eval("RoutineName") %></td>
                                                    <td><%# Eval("RoutineType") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>

                        <!-- paging -->
                        <div style="text-align: center;">
                            <ul class="pagination hide">
                            </ul>
                        </div>
                    </div>

                    <%-- no sync error log entries --%>
                    <div id="divNoItems" runat="server" visible="false">
                        <div class="alert alert-warning text-center PT50">
                            <i class="fa fa-4x fa-building text-warning m-b-md"></i>
                            <p>no sync error log entries found</p>
                        </div>
                    </div>
                </div>

                <div class="clearfix MT30"></div>
                
                <%--Back Button--%>

                <%-- footer --%>
                <uc1:footer runat="server" ID="footer" />

            </div>
        </div>
    </form>
</body>

<script src="/_content/_js/jquery.metisMenu.js"></script>
<script src="/_content/_js/jquery.slimscroll.min.js"></script>

<script src="exchangesyncerrorlog.js"></script>

</html>
