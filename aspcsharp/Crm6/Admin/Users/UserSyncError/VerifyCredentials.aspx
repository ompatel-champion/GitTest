<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerifyCredentials.aspx.cs" Inherits="Crm6.Admin.Users.UserSyncError.VerifyCredentials" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>
<%@ Register Src="~/_usercontrols/footer.ascx" TagPrefix="uc1" TagName="footer" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Verify Sync Credentials</title>
    <!--css custom-->
    <link href="verify-credentials-25-jan-2020.css" rel="stylesheet" />
</head>

<body>

    <form runat="server" id="form">
        <div id="main">
            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblGuid" runat="server" Text="0"></asp:Label>

            <%-- navbar --%>
            <uc1:navmobile runat="server" />

            <div id="main-wrapper container-fluid">

                <div class="row">

                    <!-- .page-container -->
                    <div class="page-container">

                        <!-- .page-content -->
                        <div class="page-content">

                            <!-- nav sidebar desktop -->
                            <uc1:nav runat="server" ID="nav1" />

                            <!-- #content -->
                            <div id="content" class="animated fadeIn">
                                <!-- .top-header -->
                                <header class="top-header globalHead">
                                    <h1 class="page-title">
                                         Verify Sync Credentials
                                    </h1>
                                </header>

                                <!-- verify credentials content wrapper -->
                                <div id="divVerifyCredentials" class="wrapper">

                                    <div class="row no-gutters">
                                        <div class="col-md-6">
                                            <div class="panel blank-panel">
                                                <div class="ibox float-e-margins">
                                                    <div class="ibox-content">

                                                        <div class="form-horizontal">

                                                            <div class="row" id="divSyncOptions">
                                                                <div class="col-md-12 text-center">
                                                                    <div class="alert alert-success success-message hide">
                                                                        Your sync credentials have been successfully verified
                                                                    </div>
                                                                    <div class="alert alert-danger error-message">
                                                                        Office 365 Login Failure - Enter your Office 365 login email and password
                                                                    </div>

                                                                    <%--Office 365--%>
                                                                    <div class="col-md-12 sync-office365">
                                                                        <div class="ibox-content">
                                                                            <div class="m-b-sm">
                                                                                <img alt="image" src="/_content/_img/sync/office365.png">
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <%--Exchange--%>
                                                                    <div class="col-md-4 sync-exchange hide">
                                                                        <div class="ibox-content text-center">
                                                                            <div class="m-b-sm">
                                                                                <img alt="image" src="/_content/_img/sync/exchange.png">
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <%--Google--%>
                                                                    <div class="col-md-4 sync-google hide">
                                                                        <div class="ibox-content text-center">
                                                                            <div class="m-b-sm">
                                                                                <img alt="image" src="/_content/_img/sync/google.png">
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                </div>
                                                            </div>

                                                            <%--Office 365--%>
                                                            <div id="divO365Settings">
                                                                <p class="text-muted FontSize12">Enter your Office 365 email and password to enable Sync</p>
                                                                <div class="hr-line-dashed"></div>
                                                                <div class="form-group">
                                                                    <label class="col-sm-3 control-label language-entry">Email</label>
                                                                    <div class="col-sm-9">
                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtO365Email" placeholder="" Width="350" MaxLength="100"></asp:TextBox>
                                                                        <span class="error-text"></span>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-sm-3 control-label language-entry">Password</label>
                                                                    <div class="col-sm-9">
                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtO365Password" Width="350" TextMode="Password" placeholder="" MaxLength="50"></asp:TextBox>
                                                                        <span class="error-text"></span>
                                                                    </div>
                                                                </div>
                                                                <!-- buttons -->
                                                                <div class="buttons row no-gutters">
                                                                    <div class="col-auto">
                                                                        <div class="form-btns">
                                                                            <button type="button" style="margin-left:30px" class="delete-btn text-danger secondary-btn pull-left language-entry" data-action="disable-sync">Disable Sync</button>
                                                                            <button type="button" style="margin-left:15px" class="primary-btn pull-right language-entry" id="btnSaveOffice365SyncSettings">Verify</button>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                            </div>

                                                            <%--Micorosoft Exchange--%>
                                                            <div id="divExchangeSettings" class="hide MB20">
                                                                <p class="text-muted FontSize12">Enter your credentials and click verify to enable Exchange Sync</p>
                                                                <div class="hr-line-dashed"></div>
                                                                <div class="form-group">
                                                                    <label class="col-sm-3 control-label language-entry">Email</label>
                                                                    <div class="col-sm-9">
                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtExchangeEmail" Width="300" placeholder="" MaxLength="100"></asp:TextBox>
                                                                        <span class="error-text"></span>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-sm-3 control-label language-entry">UserName</label>
                                                                    <div class="col-sm-9">
                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtExchangeUserName" Width="300" placeholder="" MaxLength="100"></asp:TextBox>
                                                                        <span class="error-text"></span>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-sm-3 control-label language-entry">Password</label>
                                                                    <div class="col-sm-9">
                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtExchangePassword" Width="300" TextMode="Password" placeholder="" MaxLength="100"></asp:TextBox>
                                                                        <span class="error-text"></span>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="portal-footer">
                                                                        <a class=" text-danger btn btn-danger  m-l-md  pull-left" data-action="disable-sync" id="">Disable Sync</a>

                                                                        <a id="btnSaveExchangeSyncSettings" class="btn btn-success pull-right MR30">Verify Credentials</a>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <%--Google--%>
                                                            <div id="divGoogleSettings" class="hide MB20">
                                                                <div class="row">
                                                                    <div class="portal-footer">
                                                                        <a class=" text-danger btn btn-danger  m-l-md  pull-left" data-action="disable-sync" id="">Disable Sync</a>
                                                                        <asp:Button ID="btnActivateGoogleSync" OnClick="btnActivateGoogleSync_Click" UseSubmitBehavior="false" runat="server" Text="Verify Access" CssClass="btn btn-sm  btn-white" />
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
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
</body>

<!-- js libraries -->
<script src="/_content/_js/jquery.metisMenu.js"></script>
<script src="/_content/_js/jquery.slimscroll.min.js"></script>
<script src="/_content/_js/jquery.ajaxfileupload.js"></script>

<!-- js custom -->
<script src="verify-credentials-25-jan-2020.js"></script>

</html>

