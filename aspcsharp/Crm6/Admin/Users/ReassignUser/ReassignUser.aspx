<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReassignUser.aspx.cs" Inherits="Crm6.Admin.ReassignUser" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reassign User</title>

    <!-- css custom -->
    <link href="reassignuser-30-jan-2020.css" rel="stylesheet" />

    <!-- favicon -->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="re-users">

    <form runat="server">

        <!-- #main start -->
        <div id="main">

            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

            <!-- navbar mobile -->
            <uc1:navmobile runat="server" />

            <div class="container-fluid">
                <div class="row">

                    <!-- .page-container -->
                    <div class="page-container">

                        <!-- .page-content -->
                        <div class="page-content">

                            <!-- navbar desktop -->
                            <uc1:nav runat="server" ID="navSidebar" />

                            <!-- #content -->
                            <div id="content" class="animated fadeIn">

                                <!-- .top-header -->
                                <header class="top-header globalHead">
                                    <h1 class="page-title language-entry">Reassign User</h1>
                                </header>
                                <!-- .top-header -->

                                <div class="wrapper">
                                    <div class="row">
                                        <div class="col-md-12">

                                            <!-- .default-block -->
                                            <div class="default-block">

                                                <div class="dblk-text">
                                                    Select departing and incoming user to transfer assignment of all CRM data to a new sales rep.<br />
                                                </div>

                                                <!-- .explanation -->
                                                <div class="explanation">

                                                    <div class="dblk-text">
                                                        <b>This will transfer <span>ALL CRM data</span>,<br />
                                                            FROM a <span>Departing CRM User</span><br />
                                                            TO an <span>Active CRM User</span></b>
                                                    </div>

                                                    <div class="explanation-details">
                                                        <ul>
                                                            <li class="language-entry">Companies</li>
                                                            <li class="language-entry">Contacts</li>
                                                            <li class="language-entry">Deals</li>
                                                            <li class="language-entry">Notes</li>
                                                            <li class="language-entry">Upcoming Calendar Events</li>
                                                            <li class="language-entry">Upcoming Tasks</li>
                                                            <%--<li class="language-entry">Documents</li>--%>
                                                        </ul>
                                                    </div>
                                                </div>

                                                <div class="ru-dropdowns row">
                                                    <%--Dropdown for departing user--%>
                                                    <div class="col-sm-6">
                                                        <label for="ddlDepartingUser">Departing User *</label>
                                                        <div class="form-group">
                                                            <asp:DropDownList runat="server" ID="ddlDepartingUser" CssClass="W400 form-control"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <%--Dropdown for new user--%>
                                                    <div class="col-sm-6">
                                                        <label for="ddlNewUser">New User *</label>
                                                        <div class="form-group">
                                                            <asp:DropDownList runat="server" ID="ddlNewUser" CssClass="W400 form-control"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>

                                                <!-- buttons -->
                                                <div class="row buttons no-gutters">
                                                    <div class="col"></div>
                                                    <div class="col-auto">
                                                        <div class="form-btns">
                                                            <button type="button" class="secondary-btn cancel-btn" id="btnCancel">Cancel</button>
                                                        </div>
                                                    </div>
                                                    <div class="col-auto">
                                                        <div class="form-btns">
                                                            <button type="button" class="primary-btn" id="btnReassign">Reassign</button>
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                            <!-- .default-block -->
                                        </div>
                                    </div>
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

    <!-- js custom -->
    <script src="reassignuser-25-jan-2020.js"></script>

</body>
</html>
