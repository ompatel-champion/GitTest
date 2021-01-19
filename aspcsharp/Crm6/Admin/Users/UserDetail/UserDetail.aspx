<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserDetail.aspx.cs" Inherits="Crm6.Admin.Users.UserDetail.UserDetail" %>

<%@ Register TagPrefix="uc1" TagName="nav" Src="~/_usercontrols/nav.ascx" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>User Detail</title>
    <!--css custom -->
    <link href="userdetail-18-mar-2020.css" rel="stylesheet" />
    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="user-detail">

    <form runat="server">

        <!-- #main start -->
        <div id="main">

            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblLoadedUserId" runat="server" Text="0"></asp:Label>

            <!-- navbar mobile start-->
            <uc1:navmobile runat="server" />
            <!-- navbar mobile end-->

            <div class="container-fluid">
                <div class="row">

                    <!-- .page-container -->
                    <div class="page-container">

                        <!-- .page-content -->
                        <div class="page-content">

                            <!-- navbar start-->
                            <uc1:nav runat="server" ID="navSidebar" />
                            <!-- navbar end-->

                            <!-- #content -->
                            <div id="content" class="animated fadeIn">

                                <!-- .top-header -->
                                <header class="top-header globalHead">

                                    <div class="row">
                                        <div class="col-lg-7 col-md-12 clearfix userData">
                                            <div class="userImg FL">
                                                <img id="imgProfile" src="/_content/_img/no-pic.png" runat="server" />
                                            </div>

                                            <div class="userDetail FL">
                                                <h1 class="page-title">
                                                    <asp:Label ID="lblBreadcrumbUserName" runat="server"></asp:Label>
                                                    <p class="bread_crumb">
                                                        <a href="/Admin/Users/UserList/UserList.aspx">Users</a>
                                                        <span class="bread_sep">&rsaquo;&rsaquo;</span>
                                                        <span>User Detail</span>
                                                    </p>
                                                </h1>
                                            </div>
                                        </div>

                                        <div class="col-lg-5 col-md-12">
                                            <div class="text-right actBtns">
                                                <a href="javascript:void(0)" class="primary-btn new-user"><span class="edit-user">Edit User</span></a>
                                                <a href="/Admin/Users/SwitchUserCrmAdmin/SwitchUserCrmAdmin.aspx" class="secondary-btn">Switch to User</a>
                                            </div>
                                        </div>
                                    </div>
                                </header>
                                <!-- .top-header -->

                                <!-- Users Contact Content -->
                                <div class="wrapper">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="user-settings">
                                                <div class="row">
                                                    <div class="col-lg-6 col-md-12 col-left-box">
                                                        <div class="ibox basic-card">
                                                            <div class="ibox-title">
                                                                <h3 class="card-title"><i class="icon-contacts"></i>&nbsp&nbsp User Details</h3>
                                                                <a href="javascript:void(0)" class="edit_link"><span class="edit-user">Edit</span></a>
                                                            </div>
                                                            <div class="ibox-content ud-content">
                                                                <div class="row">
                                                                    <div class="col-md-12 col-sm-12 ud-left-box">
                                                                        <div class="ud-item">
                                                                            <div>
                                                                                <span class="ud-label">Name</span>
                                                                                <asp:Label ID="lblName" runat="server"></asp:Label>
                                                                            </div>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Job Title</span>
                                                                            <asp:Label ID="lblJobTitle" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Email</span>
                                                                            <a href="" class="hover-link" runat="server" id="aEmail">
                                                                                <asp:Label ID="lblEmail" runat="server"></asp:Label></a>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Fax</span>
                                                                            <asp:Label ID="lblFax" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Mobile</span>
                                                                            <asp:Label ID="lblMobile" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Billing Code</span>
                                                                            <asp:Label ID="lblBillingCode" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Address</span>
                                                                            <asp:Label ID="lblAddress" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Location</span>
                                                                            <asp:Label ID="lblLocationName" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Region</span>
                                                                            <asp:Label ID="lblRegion" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Spoken Languages</span>
                                                                            <asp:Label ID="lblSpokenLanguages" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">IP Address</span>
                                                                            <asp:Label ID="lblIpAddress" runat="server"></asp:Label>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6 col-md-12 col-right-box">
                                                        <div class="ibox basic-card">
                                                            <div class="ibox-title">
                                                                <h3 class="card-title"><i class="icon-settings"></i>&nbsp&nbsp App Settings</h3>
                                                            </div>
                                                            <div class="ibox-content ud-content">
                                                                <div class="row">
                                                                    <div class="col-md-12 col-sm-12 ud-left-box">
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Login Status</span>
                                                                            <asp:Label ID="lblLoginStatus" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Password</span>
                                                                            <asp:Label ID="lblPassword" runat="server" Text=""></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">User Roles</span>
                                                                            <asp:Label ID="lblUserRoles" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Last Login</span>
                                                                            <asp:Label ID="lblLastLogin" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Timezone</span>
                                                                            <asp:Label ID="lblTimezone" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Browser</span>
                                                                            <asp:Label ID="lblBrowser" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Currency</span>
                                                                            <asp:Label ID="lblCurrencyName" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Display Language</span>
                                                                            <asp:Label ID="lblDisplayLanguage" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Data Center</span>
                                                                            <asp:Label ID="lblDataCenter" runat="server"></asp:Label>
                                                                        </div>
                                                                        <div class="ud-item">
                                                                            <span class="ud-label">Screen Resolution</span>
                                                                            <asp:Label ID="lblScreenResolution" runat="server"></asp:Label>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- .col-md-12 -->
                                    </div>
                                    <!-- .row -->
                                </div>
                                <!-- .wrapper -->
                            </div>
                            <!-- #content end -->
                        </div>
                        <!-- .page-content -->
                    </div>
                    <!-- .page-container -->
                </div>
                <!--.row-->
            </div>
            <!--.container-fluid-->

            <!-- .container-fluid -->
        </div>
        <!-- #main end -->

    </form>

    <!-- js custom -->
    <script src="userdetail-04-apr-2020.js"></script>

</body>
</html>
