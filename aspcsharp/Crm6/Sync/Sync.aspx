<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sync.aspx.cs" Inherits="Crm6.Sync" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/footer.ascx" TagPrefix="uc1" TagName="footer" %>


<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Sync</title>
    <link href="/_content/_css/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" />
    <link href="/_content/_css/bootstrap-tagsinput.css" rel="stylesheet" />
    <style>
        body {
            background-color: #ffffff;
        }

        .bootstrap-tagsinput {
            padding: 8px 12px 6px;
            min-height: 38px;
        }
    </style>
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
                    <div class="col-sm-10">
                        <h2>Sync</h2>
                        <ol class="breadcrumb">
                            <li>
                                <a href="/Dashboards/SalesRepDashboard/SalesRepDashboard.aspx">Home</a>
                            </li>
                            <li class="active">
                                <strong>Sync</strong>
                            </li>
                        </ol>
                    </div>
                    <div class="col-sm-2"></div>
                </div>

                <%-- content --%>
                <div class="wrapper wrapper-content animated fadeInRight row">
                    <div class="col-md-12">
                        <div class="ibox float-e-margins">
                            <div class="ibox-content">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label language-entry">Office 365 Email</label>
                                        <div class="col-sm-9">
                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtEmail" placeholder="" MaxLength="100"></asp:TextBox>
                                            <span class="error-text"></span>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-3 control-label language-entry">Office 365 Password</label>
                                        <div class="col-sm-9">
                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtPassword" placeholder="" MaxLength="100"></asp:TextBox>
                                            <span class="error-text"></span>
                                        </div>
                                    </div>
                                    <div class="hr-line-dashed"></div>
                                    <div class="form-group">
                                        <div class="col-sm-12">
                                            <asp:Button runat="server" CssClass="btn btn-default pull-left" ID="btnSyncLog" OnClick="btnSyncLog_Click" Text="Sync Log" />
                                            <asp:Button runat="server" CssClass="btn btn-default pull-left m-l-xs" ID="btnSyncErrorLog" OnClick="btnSyncErrorLog_Click" Text="Sync Error Log" />
                                            <asp:Button runat="server" CssClass="btn btn-success pull-right" ID="btnSync" OnClick="btnSync_Click" Text="Sync Office 365 with CRM" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <%-- footer --%>
                <uc1:footer runat="server" ID="footer" />

            </div>
        </div>

    </form>

    <script src="/_content/_js/jquery.metisMenu.js"></script>
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>
    <script src="/_content/_js/jquery-ui-1.12.1.min.js"></script>

    <!-- type ahead -->
    <script src="/_content/_js/bootstrap3-typeahead.js"></script>
    <script src="/_content/_js/bootstrap-tagsinput.js"></script>

    <script src="sync.js"></script>

</body>

</html>
