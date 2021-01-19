<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Locations.aspx.cs" Inherits="Crm6.Admin.Locations" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<%@ Register TagPrefix="loctab" TagName="CountriesToRegions" Src="~/Admin/Locations/CountriesToRegions/CountriesToRegions.ascx" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Locations</title>

    <!--css custom-->
    <link href="locations-05-apr-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="locations">
    <form runat="server">

        <!--#main start-->
        <div id="main">

            <!--hidden values-->
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label> 
            <asp:Label CssClass="hide" ID="lblUserIdGlobal" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

            <!--navbar mobile-->
            <uc1:navmobile runat="server" />

            <div class="container-fluid">
                <div class="row">

                    <!--.page-container-->
                    <div class="page-container">

                        <!--.page-content-->
                        <div class="page-content">

                            <!--navbar desktop-->
                            <uc1:nav runat="server" ID="navSidebar" />

                            <!-- #content-->
                            <div id="content" class="animated fadeIn">

                                <!--.top-header-->
                                <header class="top-header">
                                    <div class="row">
                                        <div class="col-md-3">
                                            <h1 class="page-title">Locations</h1>
                                        </div>

                                        <div id="locationSearch" class="col-md-9 searchBlock">
                                            <div class="list-table-btn-bar row no-gutters align-items-center">
                                                <div class="col"></div>
                                                <div class="col-auto">
                                                    <div class="search-form location-search">
                                                        <div class="search-box">
                                                            <asp:TextBox runat="server" Style="z-index: 0;" Width="200" CssClass="form-control " ID="txtKeyword" placeholder="Search"></asp:TextBox>
                                                            <asp:LinkButton ID="btnSearch" runat="server" OnClick="btnSearch_Click"><i class="icon-search"></i></asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-auto">
                                                    <div class="add-new-btn pull-right">
                                                        <a href="javascript:void(0)" class="new-location primary-btn"><i class="icon-plus"></i>Location</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="districtSearch" class="col-md-9 searchBlock">
                                            <div class="list-table-btn-bar row no-gutters align-items-center">
                                                <div class="col"></div>
                                                <div class="col-auto">
                                                    <div class="add-new-btn pull-right">
                                                        <a href="javascript:void(0)" class="new-district primary-btn"><i class="icon-plus"></i>District</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="globalLocationSearch" class="col-md-9 searchBlock">
                                            <div class="list-table-btn-bar row no-gutters align-items-center">
                                                <div class="col"></div>
                                                <div class="col-auto">
                                                    <div class="search-form">
                                                        <div class="search-box">
                                                            <asp:TextBox runat="server" Style="z-index: 0;" Width="200" CssClass="form-control " ID="txtGlobalKeyword" placeholder="enter min 2 letters"></asp:TextBox>
                                                            <a id="aGlobalSearch" runat="server"><i class="icon-search"></i></a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-auto">
                                                    <div class="add-new-btn pull-right">
                                                        <a id="btnGlobalSearch" href="javascript:void(0)" class="new-global-location primary-btn"><i class="icon-plus"></i>Global Location</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="regionSearch" class="col-md-9 searchBlock">
                                            <div class="list-table-btn-bar row no-gutters align-items-center">
                                                <div class="col"></div>
                                                <div class="col-auto">
                                                    <div class="add-new-btn pull-right">
                                                        <a href="javascript:void(0)" class="new-region  primary-btn"><i class="icon-plus"></i>Region</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <!--desktop nav -->
                                        <div class="col-md-12 border-tabs">
                                            <div data-id="#divLocationsTab" class="btab language-entry active">Company Locations</div>
                                            <div data-id="#divDistrictsTab" class="btab language-entry">Districts</div>
                                            <div data-id="#divGlobalLocationsTab" class="btab language-entry">Global Locations</div>
                                            <div data-id="#divRegionsTab" class="btab language-entry">Regions</div>
                                            <div data-id="#divCountriesRegionsTab" class="btab language-entry">Countries To Regions</div>
                                        </div>
                                        <!-- .mobile-panel-nav -->
                                        <div class="mobile-panel-nav">
                                            <div class="dropdown-wrapper panel-dropdown">
                                                <div class="ae-dropdown dropdown">
                                                    <div class="ae-select">
                                                        <span class="ae-select-content"></span>
                                                        <span class="drop-icon-down"><i class="icon-angle-down"></i></span>
                                                        <span class="drop-icon-up"><i class="icon-angle-up"></i></span>
                                                    </div>
                                                    <ul id="ll-tabs" class="dropdown-nav ae-hide nav nav-tabs">
                                                        <li class='selected'><a data-id="#divLocationsTab" class="active"><span class="language-entry">Company Locations</span></a></li>
                                                        <li class=""><a data-id="#divDistrictsTab"><span class="language-entry">Districts</span></a></li>
                                                        <li class='selected'><a data-id="#divGlobalLocationsTab" class="active"><span class="language-entry">Global Locations</span></a></li>
                                                        <li class=""><a data-id="#divRegionsTab"><span class="language-entry">Regions</span></a></li>
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                        <!--.mobile-panel-nav-->
                                    </div>
                                </header>
                                <!--.top-header-->

                                <!--Page Content-->
                                <div class="wrapper MT15">

                                    <!--locations-->
                                    <div id="divLocationsTab" class="btab-content active">

                                        <!-- #divLocationsTab -->
                                        <div id="divLocations" class="tab-list list-table">

                                            <!--location list-->
                                            <table class="table list-table table-hover" id="tblLocations">
                                                <thead>
                                                    <tr>
                                                        <th>Name</th>
                                                        <th>Code</th>
                                                        <th>Type</th>
                                                        <th>Address</th>
                                                        <th>Country</th>
                                                        <th>Phone</th>
                                                        <th class="text-center"></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater ID="rptLocations" runat="server">
                                                        <ItemTemplate>
                                                            <tr data-id="<%# Eval("LocationId") %>">
                                                                <td><%# Eval("LocationName") %></td>
                                                                <td><%# Eval("LocationCode") %></td>
                                                                <td><%# Eval("LocationType") %></td>
                                                                <td><%# Eval("Address") %></td>
                                                                <td><%# Eval("CountryName") %></td>
                                                                <td><%# Eval("Phone") %></td>
                                                                <td class="text-right action-cell">
                                                                    <a class="hover-link edit-location" title="Edit Location" data-action="edit">Edit</a>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>

                                            <!--no locations-->
                                            <div id="divNoItems" class="no-items empty-box" runat="server" visible="false">
                                                <i class="icon-Deal----Gray"></i>
                                                <p class="e-text">no locations</p>
                                                <div class="btn-wrp">
                                                    <a href="#" class="primary-btn new-location">Add Location</a>
                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                    <!--districts -->
                                    <div id="divDistrictsTab" class="btab-content" runat="server">

                                        <!-- #divDistrictsTab -->
                                        <div id="divDistricts" class="list-table">

                                            <!--district list-->
                                            <table id="tblDistricts" class="table table-hover">
                                                <thead>
                                                    <tr>
                                                        <th>Name</th>
                                                        <th>Code</th>
                                                        <th>Country</th>
                                                        <th class="text-center W140"></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater ID="rptDistricts" runat="server">
                                                        <ItemTemplate>
                                                            <tr data-id="<%# Eval("DistrictId") %>">
                                                                <td><%# Eval("DistrictName") %></td>
                                                                <td><%# Eval("DistrictCode") %></td>
                                                                <td><%# Eval("CountryName") %></td>
                                                                <td class="text-right action-cell">
                                                                    <a class="hover-link edit-district" title="Edit District" data-action="edit">Edit</a>
                                                                    <a class="delete-item" title="Delete District" data-action="delete"><i class="icon-Delete"></i></a>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>

                                        </div>

                                        <!--no districts-->
                                        <div runat="server" visible="false">
                                            <div class="alert alert-warning text-center PT50">
                                                <i class="fa fa-4x fa-building text-warning m-b-md"></i>
                                                <p>no sales districts</p>
                                                <a class="btn btn-success new-district m-t-sm" href="javascript:void(0)">Add Districts</a>
                                            </div>
                                        </div>
                                    </div>

                                    <!--locations-->
                                    <div id="divGlobalLocationsTab" class="btab-content">

                                        <!-- #divGlobalLocationsTab -->
                                        <div id="divGlobalLocations" class="list-table  ">

                                            <!--global location list-->
                                            <table class="table list-table table-hover hide" id="tblGlobalLocations">
                                                <thead>
                                                    <tr>
                                                        <th style="width: 350px!important">Location</th>
                                                        <th style="width: 100px!important">Code</th>
                                                        <th style="width: 200px!important">Country</th>
                                                        <th class="text-center">Air</th>
                                                        <th class="text-center">Inland Port</th>
                                                        <th class="text-center">Multi Modal</th>
                                                        <th class="text-center">Rail</th>
                                                        <th class="text-center">Road</th>
                                                        <th class="text-center">Seaport</th>
                                                        <th style="width: 100px!important"  class="text-center"></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                </tbody>
                                            </table>

                                            <!--no global locations-->
                                            <div id="divNoGlobalLocations" class="no-items details-box hide"> 
                                                <p class="e-text">no global locations</p> 
                                            </div>

                                            <!--search global locations-->
                                            <div id="divGlobalLocaitionBeforeSearch" class="details-box"> 
                                                <p class="e-text">Please use the above text box to search global locations</p> 
                                            </div>

                                        </div>

                                    </div>
                                    <!--regions-->
                                    <div id="divRegionsTab" class="btab-content" runat="server">

                                        <!-- #divRegionsTab -->
                                        <div id="divRegions" class="list-table">

                                            <!--region list-->
                                            <div class="region-table ibox">
                                                <div class="ibox-content">
                                                    <table class="table table-hover" id="tblRegions">
                                                        <thead>
                                                            <tr>
                                                                <th>Name</th> 
                                                                <th class="text-center W140"></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="rptRegions" runat="server">
                                                                <ItemTemplate>
                                                                    <tr data-id="<%# Eval("RegionId") %>">
                                                                        <td><%# Eval("RegionName") %></td> 
                                                                        <td class="text-right action-cell">
                                                                            <a class="hover-link edit-region" title="Edit Region" data-action="edit">Edit</a>
                                                                            <a class="delete-item" title="Delete Region" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>

                                        </div>

                                        <!--no regions-->
                                        <div runat="server" visible="false">
                                            <div class="alert alert-warning text-center PT50">
                                                <i class="fa fa-4x fa-building text-warning m-b-md"></i>
                                                <p>no regions</p>
                                                <a class="btn btn-success new-region m-t-sm" href="javascript:void(0)">Add Region</a>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="divCountriesRegionsTab" class="btab-content" runat="server">
                                        <loctab:CountriesToRegions id="CountriesToRegions" runat="server" />
                                    </div>

                                        <!--spinner-->
                                    <div id="divSpinner" class="hide">
                                        <div class="ajax-modal">
                                            <div class="ibox ibox-content text-center ajax-modal-txt">
                                                <div class="spinner"></div>
                                            </div>
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
</body>

<!-- js custom -->
<script src="locations-05-apr-2020.js"></script>  
<script src="CountriesToRegions/regioncountries-05-apr-2020.js"></script>
<script src="/_content/_js/multiselect.js"></script>
<script src="/_content/_js/dual-list-box.js"></script>
</html>
