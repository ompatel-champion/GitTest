<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="Crm6.Users.UserList" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Users</title>

    <!--css custom-->
    <link href="userlist-07-apr-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="users">

    <form runat="server">

        <!-- #main start -->
        <div id="main">

            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

            <!-- navbar mobile start-->
            <uc1:navmobile runat="server" />
            <!-- navbar mobile end-->

            <div class="container-fluid">
                <div class="row">

                    <%-- card or list view --%>
                    <div class="btn-group btn-view-type pull-right m-l-sm" style="display: none">
                        <div class="hidden-xs"></div>
                        <button class="btn btn-black" data-view-type="card" type="button" title="Card View"><i class="icon-table-view"></i></button>
                        <button class="btn btn-white" data-view-type="table" type="button" title="Table View"><i class="icon-TH"></i></button>
                    </div>

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
                                        <div class="col-lg-3 pageInfo">
                                            <h1 class="page-title language-entry pull-left MR30">Users <small class="total-records"></small></h1>
                                        </div>
                                        <div class="col-lg-9">
                                            <div class="row no-gutters list-table-btn-bar">
                                                <div class="col"></div>
                                                <div class="col-auto mobileFull"
                                                    <!--search-->
                                                    <div class="row no-gutters search-boxes">
                                                        <div class="col-auto">
                                                            <div class="search-box-wrap">
                                                                <asp:TextBox runat="server" ID="txtKeyword" CssClass="W300" placeholder="Search"></asp:TextBox>
                                                                <a id="btnSearch" href="javascript:void(0)" title="Search" tabindex=-1><i class="icon-search"></i></a>
                                                            </div>
                                                        </div>
                                                        <div  class="col-auto">
                                                            <div class="select-box-wrp">
                                                                <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control W150">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    </div>
                                                
                                                <div class="col-auto">
                                                    <div class="row no-gutters usernav_dd">
                                               
                                                        <!--right side buttons-->
                                                        <div class="col-auto">
                                                            <div class="btn-wrp add-new-btn">
                                                                <a href="/Admin/Users/UserAddEdit/UserAddEdit.aspx" class="edit_link btn-hover"><i class="icon-plus"></i>User</a>
                                                            </div>
                                                        </div>
                                                        <div class="col-auto">
                                                            <div class="btn-bar-btn btn-bar-btn-options" title="Options">
                                                                <div class="edit_link btn-hover btn" tabindex=0><i class="icon-Menu-Dots text-wht"></i>
                                                                <div class="dd-menu">
                                                                   <div class="items-wrapper">
                                                                        <a href="/Admin/Users/SwitchUserCrmAdmin/SwitchUserCrmAdmin.aspx" class="dd-menu-item" tabindex=0>Switch User</a>
                                                                        <a href="/Admin/Users/ReassignUser/ReassignUser.aspx" class="dd-menu-item" tabindex=0>Reassign User</a>
                                                                        <div class="dd-menu-item dd-menu-item-export" tabindex=0>Download as Excel</div>
                                                                        <div class="dd-menu-item dd-menu-item-login-enabled" tabindex=0 data-menu-sticky>
                                                                            <label class="toggle-switch toggle-switch-login-enabled">
                                                                                <input type="checkbox" checked>
                                                                                <span class="toggle-switch-slider round"></span>
                                                                            </label>
                                                                            Login Enabled
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
                                </header>
                                <!-- .top-header -->

                                <!-- Users Page Content -->
                                <div class="wrapper">

                                    <!--button bar - search/filter and add/options-->
                                    <div id="divUsers" class="row">

                                        <div class="col-md-12">

                                            <!--#users-list-->
                                            <div id="users-list" class="list-table user-table">

                                                <!-- #tblUsers -->
                                                <table id="tblUsers" class="table list-table no-footer contacts-table">
                                                    <thead>
                                                        <tr style="cursor:pointer">
                                                            <th data-field-name="username" data-sort-order="asc" class="language-entry"><a>Name</a><i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i></th>
                                                            <th data-field-name="emailaddress" class="language-entry"><a>Email</a></th>
                                                            <th data-field-name="locationname" class="language-entry"><a>Location</a></th>
                                                            <th data-field-name="countryname" class="language-entry"><a>Country</a></th>
                                                            <th data-field-name="title" class="language-entry"><a>Job Title</a></th>
                                                            <th class="language-entry " style="cursor:default">Last Login</th>
                                                            <th></th>
                                                        </tr>
                                                    </thead>
                                                    <tbody></tbody>
                                                </table>

                                            </div>
                                            <!-- #users-list -->
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
        </div>
        <!-- #main end -->

    </form>

    <!--js custom-->
    <script src="userlist-04-apr-2020.js"></script>
</body>
</html>
