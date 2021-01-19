<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ActivitiesByDateRangeReport.aspx.cs" Inherits="Crm6.Reporting.ActivitiesByDateRangeReport" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<%@ Register Src="~/_usercontrols/CalendarEventAddEdit/CalendarEventAddEdit.ascx" TagPrefix="uc1" TagName="CalendarEventAddEdit" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Activity by Date Range Report</title>
    <link href="activities-by-daterange-report-09-apr-2020.css" rel="stylesheet" />
    <!-- favicon -->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="addEditPage">
    <form runat="server">
        <div id="main">
            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblUserIdGlobal" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblUsername" runat="server" Text=""></asp:Label>

            <%--Load from Session--%>
            <asp:Label CssClass="hide" ID="lblDateFormat" runat="server" Text="dd MM, yyyy"></asp:Label>
            <asp:Label CssClass="hide" ID="lblDateFormatMask" runat="server" Text="0"></asp:Label>

            <!--navbar mobile-->
            <uc1:navmobile runat="server" />

            <div class="container-fluid">
                <div class="row">

                    <!--.page-container-->
                    <div class="page-container reportPage">

                        <!--.page-content-->
                        <div class="page-content">

                            <!--navbar desktop-->
                            <uc1:nav runat="server" ID="navSidebar" />

                            <!- #content-->
                            <div id="content" class="animated fadeIn">

                                <!--.top-header-->
                                <header class="top-header globalHead">
                                    <h1 class="page-title language-entry">Activities By Date Range</h1>
                                </header>

                                <!--Page Content-->
                                <div class="wrapper br-fields">

                                    <div class="ibox MB20 MT15">
                                        <div class="ibox-content PB0 PT10">
                                            <%-- activities by date range report --%>

                                            <div id="divReport">

                                                <%-- events report filter --%>
                                                <div id="divReportFilter" class="hide">

                                                    <div class="row MT10">
                                                        <%--Date Range - From--%>
                                                        <div class="col-lg-3">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">From</label>
                                                                <div class="input-group">
                                                                    <span class="input-group-addon"></span>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtDateFrom" data-name="datepicker" MaxLength="50"></asp:TextBox>
                                                                    <i class="icon-calendar"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <%--Date Range - To--%>
                                                        <div class="col-lg-3">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">To</label>
                                                                <div class="input-group">
                                                                    <span class="input-group-addon"></span>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtDateTo" data-name="datepicker" MaxLength="50"></asp:TextBox>
                                                                    <i class="icon-calendar"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <%--Check boxes to select Calendar Events, Tasks and Notes--%>
                                                        <div class="col-lg-6">
                                                            <div class="form-group filter-check-boxes">
                                                                <span class="MR20">
                                                                    <asp:CheckBox CssClass="i-checks m-r-xs language-entry" runat="server" Checked="true" ID="chkCalendarEvents"></asp:CheckBox>
                                                                    Events
                                                                </span>
                                                                <span class="MR20">
                                                                    <asp:CheckBox CssClass="i-checks m-r-xs language-entry" runat="server" Checked="true" ID="chkTasks"></asp:CheckBox>
                                                                    Tasks
                                                                </span>
                                                                <span class="MR20">
                                                                    <asp:CheckBox CssClass="i-checks m-r-xs language-entry" runat="server" Checked="true" ID="chkNotes"></asp:CheckBox>
                                                                    Notes
                                                                </span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row ">

                                                        <div class="col-md-6" id="divCountries" runat="server">
                                                            <div class="form-group filled"  >
                                                                <label class="inputLabel language-entry">Countries</label>
                                                                <asp:DropDownList runat="server" ID="ddlCountry" CssClass="form-control" multiple="multiple"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-6"  runat="server" id="divLocations">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Locations</label>
                                                                <asp:DropDownList runat="server" ID="ddlLocations" CssClass="form-control" multiple="multiple"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-12" id="userContainer">
                                                            <div class="form-group filled" runat="server" id="divUsers">
                                                                <label class="inputLabel language-entry">Users</label>
                                                                <asp:DropDownList runat="server" ID="ddlUsers" CssClass="form-control" multiple="multiple"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row ">
                                                        <div class="col-md-6">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Competitors</label>
                                                                <asp:DropDownList runat="server" ID="ddlCompetitors" CssClass="form-control" multiple="multiple"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Companies</label>
                                                                <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-control" multiple="multiple">
                                                                    <asp:ListItem Value=""></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Campaigns</label>
                                                                <asp:DropDownList runat="server" ID="ddlCampaigns" CssClass="form-control" multiple="multiple"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Deal Type</label>
                                                                <asp:DropDownList runat="server" ID="ddlDealType" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>

                                            </div>

                                            <div class="clearfix"></div>
                                            <div class="row footer-actions">
                                                <div class="col-md-5 col-sm-5">
                                                    <h4 class="font-bold total-record-count"></h4>
                                                </div>
                                                <div class="col-md-7 col-sm-7">
                                                    <div id="divToolbar">
                                                        <button id="btnRunReport" class="primary-btn language-entry">Run Report</button>
                                                        <a id="btnExcel" class="primary-btn  language-entry hide">Excel</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="ibox MB50 hide" id="divReportContent">
                                        <div class="ibox-content P0 list-table">

                                            <%-- deals report header --%>
                                            <table class="table table-hover" id="tblActivityReport">
                                                <thead>
                                                    <tr>
                                                        <th class="W300"><span class="language-entry">User</span></th>
                                                        <th class="W300"><span class="language-entry">Date</span></th>
                                                        <th class="W300 header-event-type"><span class="language-entry">Event Type</span></th>
                                                        <th class="W400"><span class="language-entry">Subject</span></th>
                                                        <th class="W400"><span class="language-entry">Description</span> </th>
                                                        <th class="W400"><span class="language-entry">Companies | Deals | Contacts</span></th>
                                                        <th class="W400"><span class="language-entry">Campaigns</span></th>
                                                        <th class="W300"><span class="language-entry">Deal Type</span></th>
                                                        <th class="W300"><span class="language-entry">Competitors</span></th>
                                                        <th class="W300"><span class="language-entry">Created</span></th>
                                                        <th class="W300"><span class="language-entry">Updated</span></th>
                                                        <th class="header-edit"></th>
                                                    </tr>
                                                </thead>
                                                <%--$activities--%>
                                                <tbody>
                                                    <%-- render events rows --%>
                                                </tbody>

                                                <tfoot>
                                                    <%-- render events footer --%>
                                                </tfoot>
                                            </table>
                                            <%-- no deals --%>
                                        </div>
                                    </div>

                                    <div id="divNoItems" class="hide">
                                        <div class="alert alert-warning text-center PT50"> 
                                            <p>
                                                <label class="language-entry">No Activities Found</label>
                                            </p>
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

        <div class="modal inmodal" id="addTaskDialog" role="dialog" style="display: none;" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content animated fadeIn">
                    <div class="modal-header">
                        <h4 class="modal-title language-entry">New Task</h4>
                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">x</span></button>
                    </div>

                    <div class="modal-body white-bg" style="min-height: 300px;">
                        <div class="ibox-content br-fields">
                            <div class="form-group">
                                <label class="inputLabel language-entry">Task name</label>
                                <input id="txtTaskTitle" class="task-name required" type="text" name="task_name" placeholder="">
                            </div>

                            <div class="form-group">
                                <label class="inputLabel language-entry">Add description</label>
                                <textarea id="txtTaskDescription" class="task-descp required" placeholder=""></textarea>
                            </div>

                            <div class="task-fields">
                                <div class="row">
                                    <div class="col-md-6 col-left-box">
                                        <div class="form-group">
                                            <label class="inputLabel language-entry">Due Date</label>
                                            <input type="text" id="txtDueDate" name="due_date" class="required" placeholder="" />
                                        </div>
                                    </div>
                                    <div class="col-md-6 col-right-box">
                                        <div class="form-group">
                                            <label class="inputLabel language-entry">Assign To<span class="req">*</span></label>
                                            <asp:DropDownList ID="ddlTaskSalesReps" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 col-left-box">
                                        <div class="form-group">
                                            <label class="inputLabel language-entry">Related Deal</label>
                                            <asp:DropDownList ID="ddlTaskRelatedDeal" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-6 col-right-box">
                                        <div class="form-grou">
                                            <label class="inputLabel language-entry">Related Contacts</label>
                                            <asp:DropDownList ID="ddlTaskRelatedContacts" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="modal-footer footer-action">
                        <button type="button" class="secondary-btn language-entry" data-dismiss="modal">Close</button>
                        <div class="btn-wrp" id="task-add-actions">
                            <a class="primary-btn" id="btnAddTask language-entry">Add Task</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>


    </form>
</body>

<!-- js custom -->
<script src="activities-by-daterange-report-10-apr-2020.js"></script>

</html>
