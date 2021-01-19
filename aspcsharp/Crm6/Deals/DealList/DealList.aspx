<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DealList.aspx.cs" Inherits="Crm6.Deals.DealList" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="robots" content="noindex">
    <title>Deals</title>

    <!-- deals.css file -->
    <link href="deallist-08-apr-2020.css" rel="stylesheet" />

    <!-- favicon -->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>
<body class="deals-page">

    <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblDealId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblIsAdmin" runat="server" Text="1"></asp:Label>

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
                            <header class="top-header globalHead">

                                <div class="row">
                                    <div class="col-md-12 col-sm-12 pageInfo">
                                        <h1 class="page-title language-entry MB0 ">Deals</h1>

                                        <div class="filter-list">
                                            <a href="#" data-status="active" data-view="card" data-type-card="#active-deal" data-type-list="#deal-datatable" class="active-deal task-btn deals-link selected">ACTIVE</a>
                                            <a href="#" data-status="inactive" data-view="card" data-type-card="#inactive-deal" data-type-list="#deal-inc-datatable" class="inactive-deal task-btn deals-link">INACTIVE</a>
                                        </div>

                                        <div class="dropdown-wrapper panel-dropdown filter-dropdown">
                                            <div class="ae-dropdown dropdown">
                                                <div class="ae-select">
                                                    <span class="ae-select-content"></span>
                                                    <i class="icon-Angle-Down"></i>
                                                </div>
                                                <ul class="dropdown-nav ae-hide btn-deal-stage">
                                                    <li class="selected active iconBox" data-status="active" data-view="card" data-type-card="#active-deal" data-type-list="#deal-datatable">Active</li>
                                                    <li class="iconBox" data-status="inactive" data-view="card" data-type-card="#inactive-deal" data-type-list="#deal-inc-datatable">Inactive</li>
                                                    <li class="iconBox" data-status="won" data-view="card">Won</li>
                                                    <li class="iconBox" data-status="lost" data-view="card">Lost</li>
                                                    <li class="iconBox" data-status="stalled" data-view="card">Stalled</li>
                                                    <li class="iconBox" data-status="all" data-view="card">All</li>
                                                </ul>
                                            </div>
                                        </div>

                                        <span class="total-records"></span>

                                        <span class="form-elements">
                                            <div class="text-right">
                                                <form runat="server" class="advance-search">

                                                    <div class="search-form">
                                                        <div class="search-box">
                                                            <input type="text" id="txtKeyword" placeholder="Search" />
                                                            <a id="btnSearch" href="javascript:void(0)"><i class="icon-search"></i></a>
                                                        </div>
                                                        <a class="dotsbtn"><i class="icon-Menu-Dots"></i></a>
                                                    </div>

                                                    <span class="groupFilters">
                                                        <div class="select-box-wrp hide" id="divSalesStageDropdownContainer">
                                                            <asp:DropDownList ID="ddlSalesStage" runat="server" CssClass="form-control ">
                                                            </asp:DropDownList>
                                                        </div>

                                                        <span id="moreFilters">
                                                            <div class="select-box-wrp" id="divCountryDropdownContainer">
                                                                <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control ">
                                                                </asp:DropDownList>
                                                            </div>

                                                            <div class="select-box-wrp" id="divSalesRepDropdownContainer">
                                                                <asp:DropDownList ID="ddlSalesRep" runat="server" CssClass="form-control ">
                                                                </asp:DropDownList>
                                                            </div>

                                                            <div class="select-box-wrp" id="divLocationDropdownContainer">
                                                                <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control ">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </span>
                                                    </span>
                                                </form>
                                                <span class="right-wrap">
                                                    <div class="showView btn-group btn-view-type">
                                                        <a href="#" class="icon active" data-status="active" data-view="card" data-type-active="#active-deal" data-type-inactive="#inactive-deal"><i class="icon-TH"></i></a>
                                                        <a href="#" class="icon" data-status="active" data-view="list" data-type-active="#deal-datatable" data-type-inactive="#deal-inc-datatable"><i class="icon-table-view"></i></a>
                                                    </div>
                                                    <div class="btn-wrp add-new-btn">
                                                        <a href="#" class="edit_link btn-hover"><i class="icon-plus"></i>Deal</a>
                                                    </div>
                                                </span>
                                            </div>
                                        </span>
                                    </div>
                                </div>

                            </header>
                            <!-- .top-header -->

                            <!-- .wrapper -->
                            <div class="wrapper">

                                <!--.section-content-->
                                <div>
                                    <div class="section-body">
                                        <div class="wrapper wrapper-content">
                                            <div class="row">
                                                <div class="col-md-12 gridParent">
                                                    <!--.panel.blank-panel-->
                                                    <div class="panel blank-panel">
                                                        <!--.panel-body-->
                                                        <div class="panel-body">

                                                            <div class="deal-pg-section">
                                                                <div class="dealList">

                                                                    <!--#grid-view-->
                                                                    <div id="grid-view" class="deal-blk deal-view">

                                                                        <div id="active-deal" class="deal-cards">
                                                                            <div class="row">
                                                                            </div>
                                                                        </div>
                                                                        <!--#active-deal-->

                                                                        <div id="inactive-deal" class="deal-cards hide">
                                                                            <div class="row">
                                                                            </div>
                                                                        </div>
                                                                        <!--#inactive-deal-->

                                                                    </div>
                                                                    <!-- #grid-view -->

                                                                    <!--#list-view-->
                                                                    <div id="list-view" class="list-table ibox-content deal-table hide">
                                                                        <!--active deals-->
                                                                        <table id="deal-datatable" class="deal-toggle-tb dataTable">
                                                                            <thead>
                                                                                <tr>
                                                                                    <th data-field-name="dealname" data-sort-order="asc" class="nobg">DEAL NAME<i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i></th>
                                                                                    <th data-field-name="companyname">COMPANY</th>
                                                                                    <th data-field-name="location">LOCATION</th>
                                                                                    <th data-field-name="salesteam">SALES TEAM</th>
                                                                                    <th data-field-name="salesstagename" class="text-center">STAGE</th>
                                                                                    <th data-field-name="lastactivity" class="text-center">LAST ACTIVE</th>
                                                                                    <th data-field-name="decisiondate" class="text-center ddate">DECISION DATE</th>
                                                                                </tr>
                                                                            </thead>
                                                                            <tbody></tbody>
                                                                        </table>

                                                                        <!--inactive deals-->
                                                                        <table id="deal-inc-datatable" class="deal-toggle-tb dataTable hide">
                                                                            <thead>
                                                                                <tr>
                                                                                    <th data-field-name="dealname" data-sort-order="asc" class="nobg">DEAL NAME<i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i></th>
                                                                                    <th data-field-name="companyname">COMPANY</th>
                                                                                    <th data-field-name="location">LOCATION</th>
                                                                                    <th data-field-name="salesteam">SALES TEAM</th>
                                                                                    <th data-field-name="salesstagename" class="text-center">STAGE</th>
                                                                                    <th data-field-name="lastactivity" class="text-center">LAST ACTIVE</th>
                                                                                    <th data-field-name="decisiondate" class="text-center ddate">DECISION DATE</th>
                                                                                </tr>
                                                                            </thead>
                                                                            <tbody></tbody>
                                                                        </table>

                                                                        <%-- paging --%>
                                                                        <div style="text-align: center;">
                                                                            <ul class="pagination hide"></ul>
                                                                        </div>

                                                                    </div>
                                                                    <!-- No deals found -->
                                                                    <div id="divNoItems" class="hide">
                                                                        <div class="alert text-center">
                                                                            <p class="language-entry">No Deals Found</p>
                                                                            <div class="btn-wrp add-new-btn">
                                                                                <a href="javascript:void(0)" class="edit_link btn-hover new-contact"><i class="icon-plus"></i>Add Deal</a>
                                                                            </div>
                                                                        </div>
                                                                    </div> 

                                                                </div>
                                                                <!-- .dl-card-list-wrp -->
                                                            </div>
                                                            <!-- .deal-pg-section-->

                                                        </div>
                                                        <!-- .panel-body -->
                                                    </div>
                                                    <!-- .panel.blank-panel -->
                                                </div>
                                            </div>
                                            <!-- .row -->
                                        </div>
                                        <!-- .wrapper-content -->
                                    </div>
                                    <!-- .section-body -->
                                </div>
                                <!-- .section-content -->
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
        <!--.main-wrapper-->
    </div>
    <!-- #main end -->
</body>
</html>

<script src="/_content/_js/jquery.dataTables.min.js"></script>
<script src="/_content/_js/cookies-201710301235.js"></script>

<!-- js custom -->
<script src="deallist-08-apr-2020.js"></script>
