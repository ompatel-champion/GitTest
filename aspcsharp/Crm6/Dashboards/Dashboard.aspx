<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Crm6.Dashboards.Dashboard" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="robots" content="noindex">
    <title>Dashboard</title>

    <!-- css custom -->
    <link href="dashboard-20-mar-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class=" ">

    <form runat="server">

        <%--hidden values--%>
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblUsername" runat="server" Text=""></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

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
                            <div id="content" class="animated fadeIn">

                                <!-- .top-header -->
                                <header class="top-header">

                                    <div class="row">

                                        <div class="col-md-12">
                                            <h1 class="page-title language-entry pull-left MR30">Dashboard</h1>
                                            <div class="dashboard-status filter-wrap hide">
                                                <a href="javascrit:void(0)" data-status="active" class="active-deal task-btn deals-link active">Active</a>
                                                <a href="javascrit:void(0)" data-status="inactive" data-view="card" class="inactive-deal task-btn deals-link language-entry">Inactive</a>
                                            </div>
                                            <div class="search-form">
                                                <div class="search-box hide">
                                                    <asp:TextBox ID="txtKeyword" placeholder="Search" runat="server"></asp:TextBox>
                                                    <a id="btnSearch" href="javascript:void(0)"><i class="icon-search"></i></a>
                                                </div>
                                                <div class="select-box-wrp">
                                                    <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control ">
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="select-box-wrp">
                                                    <asp:DropDownList ID="ddlLocations" runat="server" CssClass="form-control ">
                                                    </asp:DropDownList>
                                                </div>

                                                <div class="select-box-wrp">
                                                    <asp:DropDownList ID="ddlSalesRep" runat="server" CssClass="form-control ">
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="select-box-wrp hide">
                                                    <asp:DropDownList ID="ddlDateType" placeholder="Date Type" runat="server" CssClass="form-control ">
                                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                                        <asp:ListItem Value="CreatedDate" Text="Created"></asp:ListItem>
                                                        <asp:ListItem Value="UpdatedDate" Text="Last Update"></asp:ListItem>
                                                        <asp:ListItem Value="DecisionDate" Text="Decision Date"></asp:ListItem>
                                                        <asp:ListItem Value="ContractEnd" Text="Contract End"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                                <div class="search-box input-daterange input-group hide" id="datepicker">
                                                    <span class="inp-wrap">
                                                        <input type="text" class="input-sm form-control" name="start" runat="server" id="txtDateFrom" />
                                                        <i class="icon-calendar"></i>
                                                    </span>

                                                    <span class="input-group-addon">to</span>

                                                    <span class="inp-wrap">
                                                        <input type="text" class="input-sm form-control" name="end" runat="server" id="txtDateTo" />
                                                        <i class="icon-calendar"></i>
                                                    </span>
                                                </div>
                                                <div class="select-box-wrp upbtn-wrap">
                                                    <a class="primary-btn" id="btnUpdate">Update</a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="bottom-row row">
                                    </div>
                                </header>
                                <!-- .top-header -->

                                <!-- .wrapper -->
                                <div class="wrapper">

                                    <!--.section-content-->
                                    <div>
                                        <div class="section-body">
                                            <div class="wrapper wrapper-content">

                                                <div class="row four-boxes" id="salesStageRevenueContainer"></div>

                                                <div class="row">

                                                    <!--sales forecast by stage-->
                                                    <div class="col-lg-6 PR0 forecastStage">
                                                        <div class="ibox float-e-margins">
                                                            <div class="ibox-title">
                                                                <h5 class="language-entry">Forecast by Stage</h5> 
                                                            </div>

                                                            <div class="ibox-content">
                                                                <div id="divForecastByStage" style="height: 340px">
                                                                    <div class="m-b-lg m-t-lg text-muted text-center language-entry">
                                                                        <img src="/_content/_img/loading_20.gif" />&nbsp;&nbsp;Loading...
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!--deals by industry-->
                                                    <div class="col-lg-6">
                                                        <div class="ibox float-e-margins">
                                                            <div class="ibox-title">
                                                                <h5 class="language-entry">Profit by Industry</h5> 
                                                            </div>

                                                            <div class="ibox-content">
                                                                <div id="divDealsByIndustryChart" style="height: 340px">
                                                                    <div class="m-b-lg m-t-lg text-muted text-center language-entry">
                                                                        <img src="/_content/_img/loading_20.gif" />&nbsp;&nbsp;Loading...
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <!--air tonnage by location-->
                                                    <div class="col-lg-6 PR0">
                                                        <div class="ibox float-e-margins">
                                                            <div class="ibox-title">
                                                                <h5 class="language-entry">Air Tonnage by Location</h5>
                                                                <div class="ibox-tools">
                                                                    <div class="filter-wrap hide">
                                                                        <a href="#" class="task-btn deals-link active language-entry">Country</a>
                                                                        <a href="#" class="task-btn deals-link language-entry">Sales Stage</a>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="ibox-content">
                                                                <div id="divAirByLocationChart" style="height: 340px">
                                                                    <div class="m-b-lg m-t-lg text-muted text-center language-entry">
                                                                        <img src="/_content/_img/loading_20.gif" />&nbsp;&nbsp;Loading...
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!--ocean teus by location-->
                                                    <div class="col-lg-6">
                                                        <div class="ibox float-e-margins">
                                                            <div class="ibox-title">
                                                                <h5 class="language-entry">Ocean TEUs by Location</h5>
                                                                <div class="ibox-tools">
                                                                    <div class="filter-wrap hide">
                                                                        <a href="#" class="task-btn deals-link active language-entry">Country</a>
                                                                        <a href="#" class="task-btn deals-link language-entry">Sales Stage</a>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="ibox-content">
                                                                <div id="divOceanByLocationChart" style="height: 340px">
                                                                    <div class="m-b-lg m-t-lg text-muted text-center language-entry">
                                                                        <img src="/_content/_img/loading_20.gif" />&nbsp;&nbsp;Loading...
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <!--sales forecast by location-->
                                                    <div class="col-lg-6 PR0">
                                                        <div class="ibox float-e-margins">
                                                            <div class="ibox-title">
                                                                <h5 class="language-entry">Forecast by Location</h5> 
                                                            </div>

                                                            <div class="ibox-content">
                                                                <div id="divSalesForecastByLocationChart" style="height: 340px">
                                                                    <div class="m-b-lg m-t-lg text-muted text-center language-entry">
                                                                        <img src="/_content/_img/loading_20.gif" />&nbsp;&nbsp;Loading...
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!--sales forecast by country-->
                                                    <div class="col-lg-6  ">
                                                        <div class="ibox float-e-margins">
                                                            <div class="ibox-title">
                                                                <h5 class="language-entry">Forecast by Country</h5> 
                                                            </div>

                                                            <div class="ibox-content">
                                                                <div id="divSalesForecastByCountryChart" style="height: 340px">
                                                                    <div class="m-b-lg m-t-lg text-muted text-center language-entry">
                                                                        <img src="/_content/_img/loading_20.gif" />&nbsp;&nbsp;Loading...
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">

                                                    <!--sales forecast by sales rep-->
                                                    <div class="col-lg-6 PR0">
                                                        <div class="ibox float-e-margins">
                                                            <div class="ibox-title">
                                                                <h5 class="language-entry">Forecast by Sales Rep</h5> 
                                                            </div>

                                                            <div class="ibox-content">
                                                                <div id="divSalesForecastBySalesRepChart" class="chart-wrapper" style="height: 340px">
                                                                    <div class="m-b-lg m-t-lg text-muted text-center language-entry">
                                                                        <img src="/_content/_img/loading_20.gif" />&nbsp;&nbsp;Loading...
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
            </div>
        </div>
    </form>

    <script src="/_content/_js/bundle/jquery.timepicker.js"></script>
    <!-- library for funnel report -->
    <script src="/_content/_js/amcharts/amcharts.js"></script>
    <script src="/_content/_js/amcharts/funnel.js"></script>
    <!-- google charts -->
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <!-- am charts -->
    <script src="https://www.amcharts.com/lib/3/ammap.js"></script>
    <script src="https://www.amcharts.com/lib/3/maps/js/worldLow.js"></script>
    <script src="https://www.amcharts.com/lib/3/plugins/export/export.min.js"></script>
    <script src="https://www.amcharts.com/lib/3/themes/light.js"></script>

    <!-- js custom -->
    <script src="dashboard-14-mar-2020.js"></script>

</body>
</html>
