<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="Crm6.Calendar.Calendar" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>
<%@ Register Src="~/_usercontrols/CalendarEventAddEdit/CalendarEventAddEdit.ascx" TagPrefix="uc1" TagName="CalendarEventAddEdit" %>
<%@ Register TagPrefix="uc1" TagName="TaskAddEdit" Src="~/_usercontrols/TaskAddEdit/TaskAddEdit.ascx" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <title>Calendar</title>

    <!--css libraries-->
    <link href="/_content/_css/fullcalendar/fullcalendar.css" rel="stylesheet">
    <link href="/_content/_css/fullcalendar/fullcalendar.print.css" rel='stylesheet' media='print'>

    <!--css custom-->
    <link href="calendar-21-mar-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="cal-page">

    <form runat="server">

        <%-- hidden values --%>
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblUserIdGlobal" runat="server" Text="0"></asp:Label> 
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblUsername" runat="server" Text=""></asp:Label>

        <!-- #main start -->
        <div id="main">

            <!-- navbar mobile -->
            <uc1:navmobile runat="server" />

            <div class="main-wrapper container-fluid">
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

                                    <div class="row no-gutters">
                                        <div class="col-auto pageInfo">
                                            <h1 class="page-title language-entry">Calendar</h1>
                                        </div>
                                        <div class="col column-filters align-self-center">
                                            <div class="row no-gutters">
                                                <div class="col-auto column">
                                                    <div class="form-group filled">
                                                        <asp:CheckBox runat="server" ID="chkEvents" Checked="true"></asp:CheckBox>
                                                        <label for="chkEvents" class="language-entry">Events</label>
                                                    </div>
                                                </div>
                                                <div class="col-auto column">
                                                    <div class="form-group filled">
                                                        <asp:CheckBox runat="server" ID="chkTasks"></asp:CheckBox>
                                                        <label for="chkTasks" class="language-entry">Tasks</label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-auto col-dropdown">
                                            <div class="select-box-wrp" id="userContainer">
                                                <asp:DropDownList ID="ddlUsers" runat="server" class="form-control"></asp:DropDownList>
                                            </div>
                                            <div class="headerBtns">
                                                <div class="btn-wrp add-new-btn btn-task desktopBtn">
                                                    <a href="#" class="edit_link btn-hover" data-toggle="modal" data-target="#addTaskDialog" data-action="add-task"><i class="icon-plus"></i>Task</a>
                                                </div>
                                                <div class="btn-wrp add-new-btn">
                                                    <a href="javascript:void(0)" data-action="add-event" class="edit_link btn-hover"><i class="icon-plus"></i>Event</a>
                                                </div>
                                                <div class="btn-wrp add-new-btn btn-task mobileBtn">
                                                    <a href="#" class="edit_link btn-hover" data-toggle="modal" data-target="#addTaskDialog" data-action="add-task"><i class="icon-plus"></i>Task</a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </header>
                                <!-- .top-header -->

                                <!-- Calendar Content -->
                                <div class="wrapper section-content">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div id='calendar'></div>
                                        </div>
                                        <!-- .col-md-12 -->
                                    </div>
                                    <!-- .row -->
                                </div>
                                <!-- .wrapper -->

                            </div>

                            <%-- CALENDAR EVENT ADD/EDIT --%>
                            <uc1:CalendarEventAddEdit runat="server" ID="CalendarEventAddEdit" />

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

        <div id="divLoading" class="hide">
            <div class="ajax-modal">
                <div class="ibox ibox-content text-center ajax-modal-txt">
                    <div class="loader"></div>
                    <div class="language-entry ajax-modal-title">Loading Events</div>
                </div>
            </div>
        </div>

        <%-- Task add/edit (modal popup) --%>
        <uc1:TaskAddEdit runat="server" ID="TaskAddEdit" />

    </form>


    <!-- #eventModal start-->
    <div id="fc_eventModal" title="Event Details" style="display: none;">
        <div class="fc-ev-wrap">
            <div class="fc-ev-meta icon-clock">
                <div class="fc-ev-day fc-ev-title">Friday, 19 July</div>
                <div class="fc-ev-time fc-ev-txt">6:00 - 7:00pm Eastern Standard Time</div>
            </div>

            <div class="fc-ev-meta icon-map-pin">
                <div class="fc-ev-loc fc-ev-title">4125 Whereever Way, Carlsbad CA, 35812</div>
                <div class="fc-ev-type fc-ev-txt">Meeting</div>
            </div>

            <div class="fc-ev-meta icon-business">
                <div class="fc-ev-comp fc-ev-title">Yuyu Solar Technologies Inc.</div>
                <div class="fc-ev-cinfo fc-ev-txt">Porsche shipping to NA</div>
            </div>

            <div class="fc-ev-meta  icon-description">
                <div class="fc-ev-descp fc-ev-txt">Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa</div>
            </div>

            <div class="fc-ev-meta icon-contacts">
                <div class="fc-ev-gcount fc-ev-title">3 Guests</div>
                <ul class="fc-ev-glist">
                    <li class="clearfix">
                        <div class="fc-ev-gimg">
                            <img src="/_content/_img/no-pic.png">
                        </div>
                        <div class="fc-ev-ginfo"><span class="fc-ev-gnm">Dean Martin</span><span class="fc-ev-gprof">Organizer</span></div>
                    </li>
                    <li class="clearfix">
                        <div class="fc-ev-gimg">
                            <img src="/_content/_img/no-pic.png">
                        </div>
                        <div class="fc-ev-ginfo"><span class="fc-ev-gnm">Micah Scanlon</span><span class="fc-ev-gprof">Required</span></div>
                    </li>
                    <li class="clearfix">
                        <div class="fc-ev-gimg">
                            <img src="/_content/_img/no-pic.png">
                        </div>
                        <div class="fc-ev-ginfo"><span class="fc-ev-gnm">Sharon McAllister</span><span class="fc-ev-gprof">Optional</span></div>
                    </li>
                </ul>
            </div>
        </div>

        <div class="fc-ev-acts">
            <a href="#" class="fc-ev-delete"><i class="icon-Delete"></i></a>
            <a href="#" class="fc-ev-edit">Edit</a>
        </div>
    </div>
    
    <!-- js libraries -->
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>
    <script src="/_content/_js/bundle/moment.js"></script>
    <script src="/_content/_js/fullcalendar.min.js"></script>
    <script src="/_content/_js/summernote.js"></script>
    <script src="/_content/_js/jquery.ajaxfileupload.js"></script>

    <!-- js custom -->
    <script src="calendar-23-mar-2020.js"></script>

</body>
</html>
