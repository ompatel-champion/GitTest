<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DealsReportNew.aspx.cs" Inherits="Crm6.Reporting.Deals.DealsReportNew" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Deals Report</title>
    <!-- css custom -->
    <link href="deals-report-18-mar-2020.css" rel="stylesheet" />
    <!-- favicon -->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />

</head>

<body class="skin-1">
    <form runat="server">
        <div id="main">
            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblUserRole" runat="server" Text="0"></asp:Label>

            <%--Load from Session--%>
            <asp:Label CssClass="hide" ID="lblDateFormat" runat="server" Text="dd MM, yyyy"></asp:Label>
            <asp:Label CssClass="hide" ID="lblDateFormatMask" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCompanyName" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCurrencyCode" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCurrencySymbol" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblShipmentFrequency" runat="server" Text="0"></asp:Label>

            <asp:Label CssClass="hide" ID="lblHasAdvancedFilters" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblShowLinkedOpportunities" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblIsSpotDealReports" runat="server" Text="0"></asp:Label>

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

                                    <div class="row">
                                        <div class="col-md-6 col-sm-12 pageInfo">
                                            <h1 class="page-title language-entry">Deals Report 
                                                <p class="bread_crumb">
                                                    <a href="/Reporting/ReportList.aspx">Reports</a>
                                                    <span class="bread_sep">›</span>
                                                    <span>Deals Report</span>
                                                </p>
                                            </h1>
                                            <span class="total-records"></span>
                                        </div>

                                        <div class="col-md-6 col-sm-12">
                                        </div>
                                    </div>
                                </header>

                                <div class="wrapper">
                                    <!--.section-content-->
                                    <div>
                                        <div class="section-body">
                                            <div class="wrapper wrapper-content">

                                                <%-- content --%>
                                                <div class=" ">

                                                    <div class="ibox MB20 br-fields">
                                                        <div class="ibox-content PB10" >

                                                            <%-- deals report filter --%>
                                                            <div id="divReportFilter">

                                                                <div id="divBasicFilter">

                                                                    <%--Sales Stage--%>
                                                                    <div class="row">

                                                                        <div class="col-md-3 col-left-box">
                                                                            <div class="form-group filled">
                                                                                <label class="inputLabel">Deal Status</label>
                                                                                <%--
                                                                                <div id="divSalesStageButtons" class="btn-group" role="group">
                                                                                    <button id="btnActive" type="button" data-type="Active" class="btn btn-success W80 language-entry">Active</button>
                                                                                    <button id="btnInactive" type="button" data-type="Inactive" class="btn btn-white W80 language-entry">Inactive</button>
                                                                                    <button id="btnWon" type="button" data-type="Won" class="btn btn-white W80 language-entry">Won</button>
                                                                                    <button id="btnLost" type="button" data-type="Lost" class="btn btn-white W80 language-entry">Lost</button>
                                                                                    <button id="btnStalled" type="button" data-type="Stalled" class="btn btn-white W80 language-entry">Stalled</button>
                                                                                    <button id="btnAll" type="button" data-type="All" class="btn btn-white W80 language-entry">All</button>
                                                                                </div>
                                                                                    --%>

                                                                                <select id="divSalesStageButtons">
                                                                                    <option data-type="Active">Active</option>
                                                                                    <option data-type="Inactive">Inactive</option>
                                                                                    <option data-type="Won">Won</option>
                                                                                    <option data-type="Lost">Lost</option>
                                                                                    <option data-type="Stalled">Stalled</option>
                                                                                    <option data-type="All">All</option>
                                                                                </select>
                                                                            </div>
                                                                        </div>
                                                                        <div class="col-md-4 col-mid-box ">
                                                                            <div class="form-group">
                                                                                <label class="inputLabel">Sales Stage</label>
                                                                                <div id="activeStagesContainer">
                                                                                    <asp:DropDownList runat="server" ID="ddlSalesStage" CssClass="form-control">
                                                                                        <asp:ListItem Text="Select Sales Stage" Value="0"></asp:ListItem>
                                                                                    </asp:DropDownList>
                                                                                </div>
                                                                            </div>
                                                                        </div>

                                                                        <div class="col-md-3 col-mid-box ddlCurrency">
                                                                             <asp:DropDownList runat="server" ID="ddlCurrency" CssClass="form-control"></asp:DropDownList>
                                                                        </div>

                                                                        <div class="col-md-2 col-right-box">
                                                                            <div id="divToolbar">
                                                                                <button id="btnRunReport" class="primary-btn language-entry">Run Report</button>
                                                                                <a id="btnExcel" href="javascript:void(0)" download="test001.xlsx" class="btn btn-primary language-entry hide">Excel</a>
                                                                            </div>
                                                                        </div>

                                                                        <%--
                                                                        <div class="col-md-2">
                                                                            <div id="divYearsVsMonthly" class="btn-group m-t-xs pull-right" role="group">
                                                                                <button id="btnMonthly" type="button" class="btn btn-xs btn-primary language-entry">Monthly</button>
                                                                                <button id="btnYearly" type="button" class="btn btn-xs btn-white language-entry">Yearly</button>
                                                                            </div>
                                                                        </div>
                                                                            --%>

                                                                    </div>
                                                                </div>

                                                                <%--Date Range--%>
                                                                <div class="row">
                                                                    <div class="col-md-3 col-left-box ">
                                                                        <div class="form-group">
                                                                            <asp:DropDownList runat="server" ID="ddlDateType" CssClass="form-control">
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-md-3 col-mid-box">
                                                                        <select id="ddlBtw">
                                                                           <option data-type="Active">Is Between</option>
                                                                        </select>
                                                                    </div>
                                                                    <div class="col-md-3 col-right-box dateFrom">
                                                                       <div class="form-group iconField">
                                                                            <div class="date-input">
                                                                                <asp:TextBox CssClass="form-control " runat="server" ID="txtDateFrom" placeholder="From" data-name="datepicker" MaxLength="50"></asp:TextBox>
                                                                                <i class="icon-calendar"></i>
                                                                            </div>
                                                                        </div>
                                                                        <span class="btwtext">and</span>
                                                                    </div>
                                                                    <div class="col-md-3">
                                                                        <div class="form-group iconField">
                                                                            <div class="date-input">
                                                                                <asp:TextBox CssClass="form-control " runat="server" ID="txtDateTo" placeholder="To" data-name="datepicker" MaxLength="50"></asp:TextBox>
                                                                                 <i class="icon-calendar"></i>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                            </div>

                                                            <div class="clearfix"></div>

                                                            <div class="row" id="divAdvancedFilter">
                                                                <div class="col-md-12">

                                                                    <div class="panel blank-panel">

                                                                        <div class="filterFields">
                                                                            
                                                                        </div>

                                                                        <div class="panel-heading P0" style="margin-top:-5px;">
                                                                            <div class="panel-options">
                                                                                <ul class="nav nav-tabs">
                                                                                    <li role="presentation" class="active">
                                                                                        <a href="#" class="add-filter" id="advanced-filter-tab" aria-controls="home" role="tab" data-toggle="tab" aria-expanded="true"><span id="toggletext">+ Add Filter</span>
                                                                                        </a>
                                                                                    </li>
                                                                                </ul>
                                                                            </div>
                                                                        </div>

                                                                        <div class="panel-body" style="padding: 0;">
                                                                            <div class="tab-content">
                                                                                <div class="tab-pane active" id="tab-advance-filters">
                                                                                    <div class="row">
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlService" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlDealType" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlIndustry" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlUser" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <div class="row">
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlUserCountry" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>

                                                                                        <div class="col-md-3" id="divDistrictContainer" runat="server">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlDistricts" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>

                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlLocations" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                         <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlCampaigns" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <div class="row">
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlOriginCountry" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlOriginLocation" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlDestinationCountry" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlDestinationLocation" CssClass="form-control" multiple="multiple">
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <div class="row hide">
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlShipper" CssClass="form-control">
                                                                                                    <asp:ListItem Text="Shipper" Value="0"></asp:ListItem>
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlConsignee" CssClass="form-control">
                                                                                                    <asp:ListItem Text="Consignee" Value="0"></asp:ListItem>
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group">
                                                                                                <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="form-control">
                                                                                                    <asp:ListItem Text="Customer" Value="0"></asp:ListItem>
                                                                                                </asp:DropDownList>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="col-md-3">
                                                                                            <div class="form-group PT10">
                                                                                                <asp:CheckBox CssClass="i-checks" ID="chkShowLinkedDeals" runat="server" /><label class="ML10">Show Linked Deals</label>
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

                                                    <div class="ibox MB50 hide" id="divReportContent">
                                                        <div class="ibox-content" style="height:500px">

                                                            <%-- deals report header --%>
                                                            <table class="table table-bordered " id="tblDealsReport">
                                                                <thead>
                                                                    <tr>
                                                                        <th>
                                                                            <span class="language-entry">Company</span><br />
                                                                            <span class="language-entry">Deal Name</span><br />
                                                                            <span class="language-entry">Deal Type</span>
                                                                        </th>
                                                                        <th>
                                                                            <span class="language-entry">Sales Rep</span><br />
                                                                            <span class="language-entry">Location</span>
                                                                        </th>
                                                                        <th><span class="language-entry">Sales Stage</span><br />
                                                                            <span class="language-entry">Industry</span>
                                                                        </th>
                                                                        <th id="th-wonlostreason" class="hide">
                                                                            <span class="language-entry">Won/Lost Reason</span>
                                                                        </th>
                                                                        <th>
                                                                            <span class="language-entry date-type">Decision Date</span>
                                                                        </th>
                                                                        <th>
                                                                            <span class="language-entry">Origin Countries</span>
                                                                        </th>
                                                                        <th>
                                                                            <span class="language-entry">Origins</span>
                                                                        </th>
                                                                        <th>
                                                                            <span class="language-entry">Destination Countries</span>
                                                                        </th>
                                                                        <th>
                                                                            <span class="language-entry">Destinations</span>
                                                                        </th>
                                                                        <th>
                                                                            <span class="language-entry">Shippers</span><br />
                                                                            <span class="language-entry">Consignees</span>
                                                                        </th>
                                                                        <th>
                                                                            <span class="language-entry">Last Update</span>
                                                                        </th>
                                                                        <th><span class="language-entry">Updated By</span></th>
                                                                        <th><span class="language-entry">Last Activity Date</span></th>
                                                                        <th><span class="language-entry">Next Activity Date</span></th>
                                                                        <th>
                                                                            <span class="language-entry">Services</span>
                                                                        </th> <th>
                                                                            <span class="language-entry">Comments</span>
                                                                        </th>
                                                                        <th class="W120"><span class="language-entry">Volume</span>
                                                                        </th>
                                                                        <th class="W120"><span class="language-entry">Spot Volume</span>
                                                                        </th>
                                                                        <th class="align-right"><span class="language-entry">Revenue</span><br />
                                                                            <%-- derive --%>
                                                                            <span class="currency-text language-entry"><%=CurrencyText %></span>
                                                                        </th>
                                                                        <th class="align-right"><span class="language-entry">Profit</span><br />
                                                                            <%-- derive --%>
                                                                            <span class="currency-text language-entry"><%=CurrencyText %></span>
                                                                        </th>

                                                                        <th class="align-right"><span class="language-entry">Spot Revenue</span><br />
                                                                            <%-- derive --%>
                                                                            <span class="currency-text language-entry"><%=CurrencyText %></span>
                                                                        </th>
                                                                        <th class="align-right"><span class="language-entry">Spot Profit</span><br />
                                                                            <%-- derive --%>
                                                                            <span class="currency-text language-entry"><%=CurrencyText %></span>
                                                                        </th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                                    <%-- render deals rows --%>
                                                                </tbody>

                                                                <tfoot>
                                                                    <%-- render deals footer --%>
                                                                </tfoot>
                                                            </table>

                                                            <!-- paging -->
                                                            <div style="text-align: center;">
                                                                <ul class="pagination hide">
                                                                </ul>
                                                            </div>
                                                        </div>

                                                        <%-- no deals --%>
                                                        <div id="divNoItems" class="hide">
                                                            <div class="alert alert-warning text-center PT50">
                                                                <i class="fa fa-4x fa-building text-warning m-b-md"></i>
                                                                <p>
                                                                    <label class="language-entry">No Deals Found</label>
                                                                </p>
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
</body>

<!-- date picker -->
<script src="/_content/_js/bootstrap-datepicker.js"></script>
<script src="/_content/_js/datepair.js"></script>
<script src="/_content/_js/bundle/jquery.timepicker.js"></script>
<script src="/_content/_js/bundle/moment.js"></script>

<%-- detail popups --%>
<script src="/_content/_js/detail-popups.js"></script>
<script src="deals-report-18-mar-2020.js"></script>

</html>
