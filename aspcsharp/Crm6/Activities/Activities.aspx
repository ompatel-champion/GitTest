<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Activities.aspx.cs" Inherits="Crm6.Activities.Activities" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>
<%@ Register TagPrefix="uc1" TagName="CalendarEventAddEdit" Src="~/_usercontrols/CalendarEventAddEdit/CalendarEventAddEdit.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TaskAddEdit" Src="~/_usercontrols/TaskAddEdit/TaskAddEdit.ascx" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="robots" content="noindex">
    <title>Activity</title>
    <!-- css libraries -->
    <link href="/_content/_css/chartist.min.css" rel="stylesheet" />
    <!-- css custom -->
    <link href="activity-07-apr-2020.css" rel="stylesheet" />
    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="actvity-pg">
    <form runat="server">

        <%--hidden values--%>
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label Style="display: none" ID="lblUserIdGlobal" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblUsername" runat="server" Text="0"></asp:Label>

        <!-- #main start -->
        <div id="main">

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
                            <div id="content" class="animated fadeIn wrapper-outer">

                                <!-- .top-header -->
                                <header class="top-header globalHead">
                                    <h1 class="page-title language-entry">Activity <span class="sub-title"></span></h1>
                                </header>
                                <!-- .top-header -->

                                <!-- .wrapper -->
                                <div class="wrapper">

                                    <!--.section-content-->
                                    <div>
                                        <div class="section-body">
                                            <div class="wrapper wrapper-content">

                                                <div class="row">
                                                    <div class="col-lg-6 col-left-box">

                                                        <div class="event-listing basic-card">
                                                            <div class="ibox">
                                                                <div class="ibox-title clearfix">
                                                                    <div class="title-wrap FL">
                                                                        <h3 class="card-title language-entry"><i class="icon-calendar title-icon"></i>Calendar</h3>
                                                                    </div>
                                                                    <div class="ibox-tools FR">
                                                                        <div class="btn-wrp add-new-btn">
                                                                            <a href="#" class="edit_link btn-hover" data-action="add-event"><i class="icon-plus"></i>Event</a>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="ibox-content" id="divEventsContainer">
                                                                    <div id="divEvents" class="scroll events"></div>

                                                                    <div class=" empty-box empty_event hide">
                                                                        <i class="icon-calendar"></i>
                                                                        <p class="e-text">no events</p>
                                                                    </div>
                                                                </div>

                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6 col-right-box">
                                                        <div class="note-list-wrp basic-card">
                                                            <div class="ibox">

                                                                <div class="ibox-title clearfix">
                                                                    <div class="title-wrap FL">
                                                                        <h3 class="card-title language-entry"><i class="icon-task title-icon"></i>Tasks</h3>
                                                                    </div>
                                                                    <div class="ibox-tools FR">
                                                                        <div class="btn-wrp add-new-btn">
                                                                            <a href="#" class="edit_link btn-hover" data-toggle="modal" data-target="#addTaskDialog" data-action="add-task"><i class="icon-plus"></i>Task</a>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="ibox-content" id="divTasksContainer">
                                                                    <div id="divListTasks">
                                                                        <!--#divTasks-->
                                                                        <div id="divTasks" class="scroll">
                                                                            <!-- #active-tasks -->
                                                                            <div id="active-task" class="task-items"></div>
                                                                        </div>

                                                                        <div class="no-tasks empty-box empty_event hide">
                                                                            <i class="icon-task"></i>
                                                                            <p class="e-text">no tasks</p>
                                                                            <div class="btn-wrp">
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <!-- .act-chart -->
                                                <div class="row act-chart">
                                                    <div class="col-xl-12 col-lg-12 col-md-12">
                                                        <div class="basic-card ibox">
                                                            <div class="ibox-title clearfix">
                                                                <div class="title-wrap FL">
                                                                    <h3 class="card-title language-entry"><i class="icon-Activity title-icon"></i>Activity Trend</h3>
                                                                </div>
                                                                <div class="ibox-tools chart-btns">
                                                                    <div class="filter-wrap">
                                                                        <a href="#" id="one-month" data-month-count="1" data-action="" class="active">1 Month</a>
                                                                        <a href="#" id="three-months" data-month-count="3" data-action="" class="">3 Months</a>
                                                                        <a href="#" id="six-months" data-month-count="6" data-action="" class="">6 Months</a>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="ibox-content">
                                                                <div id="divChartLoading" class="hide">
                                                                    <div class="ibox ibox-content text-center  no-borders">
                                                                        <div class="loader"></div>
                                                                        <p class="language-entry MT20">Please wait...</p>
                                                                    </div>
                                                                </div>
                                                                <div class="row" id="divActivityChart">
                                                                    <div class="col-md-4">
                                                                        <table class="act-ctable" id="tblActivities">
                                                                        </table>
                                                                    </div>

                                                                    <div class="col-md-8">
                                                                        <div class="ct-chart">
                                                                            <canvas id="activity-chart"></canvas>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                                <!-- .row -->

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%-- CALENDAR EVENT ADD/EDIT --%>
                            <uc1:CalendarEventAddEdit runat="server" ID="CalendarEventAddEdit" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%-- Task add/edit (modal popup) --%>
        <uc1:TaskAddEdit runat="server" ID="TaskAddEdit" />
        

    </form>

    <!-- js libraries -->
    <script src="/_content/_js/Chart.bundle.js"></script>
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>
    <script src="/_content/_js/bundle/moment.js"></script>
    <script src="/_content/_js/fullcalendar.min.js"></script>
    <script src="/_content/_js/cookies-201710301235.js"></script>

    <!-- js custom -->
    <script src="activity-07-apr-2020.js"></script>

</body>
</html>
