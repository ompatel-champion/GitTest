<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportList.aspx.cs" Inherits="Crm6.Reporting.ReportList" %>

<%@ Register TagPrefix="uc1" TagName="nav" Src="~/_usercontrols/nav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="navmobile" Src="~/_usercontrols/nav-mobile.ascx" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reports</title>

    <!-- favicon -->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />

    <style>
        .ibox-content {
            background-color: #ffffff !important;
            color: inherit;
            padding: 20px 35px;
        }
    </style>

</head>

<body>
    <form runat="server">
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0" Style="display: none;"></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0" Style="display: none;"></asp:Label>

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
                            <div id="content" class="animated fadeIn">

                                <%-- header --%>
                                <header class="top-header globalHead">
                                        <h1 class="page-title language-entry">Reports</h1>
                                </header>

                                <%-- content --%>
                                <div class="wrapper reportList">
                                    <div class="row">
                                        <div class="col-xl-3 col-lg-6 col-md-6  col-sm-6 reportBox">
                                            <a href="Deals/DealsReport.aspx">
                                                <div class="icon"><i class="icon-deals"></i></div>
                                                <div class="ibox ibox-content text-center">
                                                    <h2 class="language-entry">Deals</h2>
                                                    <p class="text-muted language-entry">Detailed listing of deals</p>
                                                </div>
                                            </a>
                                        </div>
                                         <div class="col-xl-3 col-lg-6  col-md-6 col-sm-6   reportBox">
                                            <a href="/Reporting/Deals/DealsReport.aspx?spot=1">
                                                <div class="icon"><i class="icon-deals"></i></div>
                                                <div class="ibox ibox-content text-center">
                                                    <h2 class="language-entry">Spot Deals</h2>
                                                    <p class="text-muted language-entry">Detailed listing of spot deals</p>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="col-xl-3 col-lg-6  col-md-6  col-sm-6  reportBox">
                                            <a href="KPIs/KPIs.aspx">
                                                <div class="icon"><i class="icon-reports"></i></div>
                                                <div class="ibox ibox-content text-center">
                                                    <h2 class="language-entry">Sales Rep Performance</h2>
                                                    <p class="text-muted language-entry">Sales Rep Performance and KPI's</p>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="col-xl-3 col-lg-6  col-md-6  col-sm-6  reportBox">
                                            <a href="ActivitiesByDateRange/ActivitiesByDateRangeReport.aspx">
                                                <div class="icon"><i class="icon-calendar"></i></div>
                                                <div class="ibox ibox-content text-center">
                                                    <h2 class="language-entry">Activities By Date Range</h2>
                                                    <p class="text-muted language-entry">List of Sales Rep Activities by Date Range</p>
                                                </div>
                                            </a>
                                        </div>
                                        
                                        <div class="col-xl-3 col-lg-6  col-md-6  col-sm-6  reportBox">
                                            <a href="WeeklyActivity/WeeklyActivity.aspx">
                                                <div class="icon"><i class="icon-Activity"></i></div>
                                                <div class="ibox ibox-content text-center">
                                                    <h2 class="language-entry">Weekly Activities By Day</h2>
                                                    <p class="text-muted language-entry">Sales Rep Activities by Day - Meetings, Calls</p>
                                                </div>
                                            </a>
                                        </div>

                                         <div id="divCompaniesReport" runat="server" class="col-xl-3 col-lg-6 col-md-6  col-sm-6   reportBox"  visible="false">
                                            <a href="Companies/CompaniesReport.aspx">
                                                <div class="icon"><i class="icon-business"></i></div>
                                                <div class="ibox ibox-content text-center">
                                                    <h2 class="language-entry">Companies Report</h2>
                                                    <p class="text-muted language-entry">Companies - Filter by Competitor, Country, Industry or Source</p>
                                                </div>
                                            </a>
                                        </div>

                                        <div id="divUserActivityReport" runat="server" class="col-xl-3 col-lg-6 col-md-6 col-sm-6  reportBox" visible="false">
                                            <a href="UserActivity/UserActivityReport.aspx">
                                                <div class="icon"><i class="icon-users"></i></div>
                                                <div class="ibox ibox-content text-center">
                                                    <h2 class="language-entry">User Activity Report</h2>
                                                    <p class="text-muted language-entry">User Activity</p>
                                                </div>
                                            </a>
                                        </div>
                                    </div>

                                    <%--TODO: Only show for CRM Admin--%>
                                    <div class="row">
                                       
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>

</html>
