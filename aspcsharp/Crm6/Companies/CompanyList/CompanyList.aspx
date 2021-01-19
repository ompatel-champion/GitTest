<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyList.aspx.cs" Inherits="Crm6.Companies.CompanyList" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Companies</title>
    <!--css custom-->
    <link href="companylist-07-apr-2020.css" rel="stylesheet" />
    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="company-list comp-pg company">

    <%-- hidden values --%>
    <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblUserRole" runat="server" Text=""></asp:Label>
    <asp:Label CssClass="hide" ID="lblIsAdmin" runat="server" Text="1"></asp:Label>

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

                        <!-- navbar dekstop -->
                        <uc1:nav runat="server" ID="navSidebar" />

                        <!-- #content -->
                        <div id="content" class="animated fadeIn">

                            <!-- .top-header -->
                            <header class="top-header globalHead">

                                <div class="row no-gutters top-row">
                                    <!-- column (title) -->
                                    <div class="col-auto col-title top-row-col">
                                        <h1 class="page-title language-entry MB0 ">Companies</h1>
                                    </div>
                                    <!-- column (active/inactive, record count) -->
                                    <div class="col-auto pageInfo top-row-col">
                                        <div class="dd_options">
                                            <div class="dropdown-wrapper panel-dropdown filter-dropdown">
                                                <div class="ae-dropdown dropdown">
                                                    <div class="ae-select">
                                                        <span class="ae-select-content"></span>
                                                        <i class="icon-Angle-Down"></i>
                                                    </div>
                                                    <ul class="dropdown-nav ae-hide btn-company-type">
                                                        <li data-status="active-customer">Active Customer</li>
                                                        <li data-status="inactive-customer">Inactive Customer</li>
                                                        <li class="selected" data-status="active">Active</li>
                                                        <li data-status="inactive">Inactive</li>
                                                    </ul>
                                                    <div class="total-records"></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <!--  column (global search) -->
                                    <div class="col align-self-center text-right col-global-search top-row-col">
                                        <div class="global-search">
                                            <a data-action="open-company-lookup" data-toggle="modal" data-target="#globalCompanyLookup"><i class="icon-search"></i>Global Companies</a>
                                        </div>
                                    </div>
                                    <!--  column (filters) -->
                                    <div class="col-auto col-filters top-row-col">
                                    </div>
                                    <!-- column (table/card view, add company buttons) -->
                                    <div class="col-auto top-row-col">
                                        <div class="showView btn-group btn-view-type">
                                            <button class="icon active" data-view-type="card" type="button" data-placement="bottom" title="Card View"><i class="icon-TH"></i></button>
                                            <button class="icon" data-view-type="table" type="button" data-placement="bottom" title="Table View"><i class="icon-table-view"></i></button>
                                        </div>
                                        <div class="btn-wrp add-new-btn add-comp-wrap">
                                            <a href="#" class="edit_link btn-hover new-company"><i class="icon-plus"></i>Company</a>
                                        </div>
                                    </div>
                                </div>
                                
                                <!-- media query move element container -->
                                <div id="query-parent-global-search"></div>

                                <!-- media query move element container -->
                                <div id="query-parent-search-filters">
                                    <!-- search filters -->
                                    <div class="form-elements search-filters">
                                        <form runat="server">
                                            <div class="search-form search">
                                                <div class="search-box">
                                                    <input type="text" id="txtKeyword" placeholder="Search" />
                                                    <a class="btn-search" href="javascript:void(0)"><i class="icon-search"></i></a>
                                                </div>
                                            </div>
                                            <div id="wrpCountry" class="search-form search-country" runat="server">
                                                <div class="search-box">
                                                    <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control ">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="search-form">
                                                <div class="search-box">
                                                    <input type="text" class="W150" id="txtCity" placeholder="City" />
                                                    <a class="btn-search" href="javascript:void(0)"><i class="icon-search"></i></a>
                                                </div>
                                            </div>
                                            <div class="search-form">
                                                <div class="search-box">
                                                    <input type="text" class="W150" id="txtPostalCode" placeholder="Postal Code" />
                                                    <a class="btn-search" href="javascript:void(0)"><i class="icon-search"></i></a>
                                                </div>
                                            </div>
                                        </form>
                                    </div>
                                </div>

                            </header>
                            <!-- .top-header -->

                            <!-- Company Page Content -->
                            <div class="wrapper">
                                <div class="row">
                                    <div class="col-md-12">

                                        <!-- #company-list-view -->
                                        <div class="list-table company-table">

                                            <div id="divCompanies">
                                                <%-- table view --%>
                                                <div id="divTableView">
                                                    <table class="table dataTable no-footer companies-table" id="tblCompanies">
                                                        <thead>
                                                            <tr>
                                                                <th class="nobg" data-sort-order="asc" data-field-name="companyname">Company
                                                                    <i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i>
                                                                </th>
                                                                <th data-field-name="City">City</th>
                                                                <th class="W150" data-field-name="CountryName">Country</th>
                                                                <th>Sales Team</th>
                                                                <th class="text-center" data-field-name="lastactivity">Last Activity</th>
                                                                <th class="text-center" data-field-name="nextactivity">Next Activity</th>
                                                                <th></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody></tbody>
                                                    </table>
                                                </div>

                                                <%-- company cards --%>
                                                <div id="divCardView"></div>

                                                <div class="clearfix"></div>

                                                <%-- no companies --%>
                                                <div id="divNoItems" class="hide">
                                                    <div class="alert text-center">
                                                        <p class="language-entry">No Companies Found</p>
                                                        <div class="btn-wrp add-new-btn">
                                                            <a href="javascript:void(0)" class="edit_link btn-hover new-company">Add Company</a>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div id="divLoading" class="hide">
                                                    <div class="ajax-modal">
                                                        <div class="ibox ibox-content text-center ajax-modal-txt">
                                                            <div class="loader"></div>
                                                            <div class="language-entry ajax-modal-title">Loading Companies</div>
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
    </div>
    <!-- #main end -->

    <%--global company lookup dialog--%>
    <div class="modal inmodal" id="globalCompanyLookup" tabindex="-1" role="dialog" style="display: none;" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content animated fadeIn">
                <div class="modal-header">
                
                    <div data-dismiss="modal" class="closeX" data-action="cancel-event"></div>
                    <h4 class="modal-title">Global Company Lookup</h4>
                </div>
                <div class="modal-body white-bg" style="min-height: 300px;">

                    <div class="form-group">
                        <input type="text" id="txtGlobalCompanySearch" class="form-control W100P" placeholder="Search">
                    </div>

                    <div id="divGlobalLoading" class="hide">
                        <div class="ibox ibox-content text-center  no-borders">
                            <div class="loader"></div>
                            <p class="language-entry MT20">Searching...</p>
                        </div>
                    </div>

                    <table class="table table-striped hide MT30" id="tblGlobalCompanies">
                        <tbody></tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <!-- form -->

    <!-- js libraries -->
    <script src="/_content/_js/jquery.dataTables.min.js"></script>

    <!-- js custom -->
    <script src="/_content/_js/cookies-201710301235.js"></script>
    <script src="companylist-06-apr-2020.js"></script>

</body>
</html>
