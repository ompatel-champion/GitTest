<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="Crm6.Admin.Settings" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>
<%@ Register TagPrefix="settings" TagName="EventCategories" Src="~/Admin/Settings/EventCategories/EventCategories.ascx" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Settings</title>
    <!--css custom-->
    <link href="settings-18-mar-2020.css" rel="stylesheet" />
    <link href="/_content/_css/spectrum.css" rel="stylesheet" />
    <link href="eventcategories/eventcategories-21-mar-2020.css" rel="stylesheet" />
    <!-- favicon -->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="settings">

    <form runat="server">

        <!--#main start-->
        <div id="main">

            <!--hidden values-->
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
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

                            <!-- #content -->
                            <div id="content" class="animated fadeIn">

                                <!--top-header-->
                                <header class="top-header">
                                    
                                    <h1 class="page-title">Settings</h1>
                                    
                                    <!--menu tabs -->
                                    <div class="row">

                                        <div class="col-md-12 border-tabs">
                                            <div id="divCommoditiesSegmentsTabHeader" data-id="#divCommoditiesTab" class="btab active language-entry">Commodities</div>
                                            <div id="divCompanySegmentsTabHeader" data-id="#divCompanySegmentsTab" class="btab language-entry">Company Segments</div>
                                            <div id="divCompanyTypesTabHeader" data-id="#divCompanyTypesTab" class="btab language-entry">Company Types</div>
                                            <div id="divCompetitorsTabHeader" data-id="#divCompetitorsTab" class="btab language-entry">Competitors</div>
                                            <div id="divContactTypesTabHeader" data-id="#divContactTypesTab" class="btab language-entry">Contact Types</div>
                                            <div id="divDealTypesTabHeader" data-id="#divDealTypesTab" class="btab language-entry">Deal Types</div>
                                            <div id="divIndustriesTabHeader" data-id="#divIndustriesTab" class="btab language-entry">Industries</div>
                                            <div id="divLostReasonsTabHeader" data-id="#divLostReasonsTab" class="btab language-entry">Lost Reasons</div>
                                            <div id="divSalesTeamRolesTabHeader" data-id="#divSalesTeamRolesTab" class="btab language-entry">Sales Roles</div>
                                            <div id="divSalesStagesTabHeader" data-id="#divSalesStagesTab" class="btab language-entry">Stages</div>
                                            <div id="divSourcesTabHeader" data-id="#divSourcesTab" class="btab language-entry">Sources</div>
                                            <div id="divTagsTabHeader" data-id="#divTagsTab" class="btab language-entry">Tags</div>
                                            <div id="divWonReasonsTabHeader" data-id="#divWonReasonsTab" class="btab language-entry">Won Reasons</div>
                                            <div id="divEventCategoriesTabHeader" data-id="#divEventCategoriesTab" class="btab language-entry">Event Categories</div>
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
                                                    <ul id="cl-tabs" class="dropdown-nav ae-hide nav nav-tabs">
                                                        <li class='selected'><a data-id="#divCommoditiesTab"><span class="language-entry active">Commodities</span></a></li>
                                                        <li class=""><a data-id="#divCompanySegmentsTab"><span class="language-entry">Company Segments</span></a></li>
                                                        <li class=""><a data-id="#divCompanyTypesTab"><span class="language-entry">Company Types</span></a></li>
                                                        <li class=""><a data-id="#divCompetitorsTab"><span class="language-entry">Competitors</span></a></li>
                                                        <li class=""><a data-id="#divContactTypesTab"><span class="language-entry">Contact Types</span></a></li>
                                                        <li class=""><a data-id="#divDealTypesTab"><span class="language-entry">Deal Types</span></a></li>
                                                        <li class=""><a data-id="#divIndustriesTab"><span class="language-entry">Industries</span></a></li>
                                                        <li class=""><a data-id="#divLostReasonsTab"><span class="language-entry">Lost Reasons</span></a></li>
                                                        <li class=""><a data-id="#divSalesStagesTab"><span class="language-entry">Sales Stages</span></a></li>
                                                        <li class=""><a data-id="#divSalesTeamRolesTab"><span class="language-entry">Sales Team Role</span></a></li>
                                                        <li class=""><a data-id="#divSourcesTab"><span class="language-entry">Sources</span></a></li>
                                                        <li class=""><a data-id="#divTagsTab"><span class="language-entry">Tags</span></a></li>
                                                        <li class=""><a data-id="#divWonReasonsTab"><span class="language-entry">Won Reasons</span></a></li>
                                                        <li class=""><a data-id="#divEventCategoriesTab"><span class="language-entry">Event Categories</span></a></li>
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                </header>
                                <!--top-header-->

                                <div class="wrapper MT15">

                                    <!--commodities-->
                                    <div id="divCommoditiesTab" class="btab-content active" runat="server">

                                        <div id="divCommodities" class="tab-list list-table">

                                            <!-- commodities list -->
                                            <div id="items">
                                                <table class="table table-hover" id="commodities">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Commodities</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptCommodities">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("CommodityId") %>' data-current-commodity="<%# Eval("CommodityName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control commodity-name" Text='<%# Eval("CommodityName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <!-- new-commodity -->
                                            <div class="new-commodity new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewCommodity" CssClass="form-control" placeholder='Commodity' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddCommodity" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Commodity</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                    <!--company segments-->
                                    <div id="divCompanySegmentsTab" class="btab-content" runat="server">

                                        <div id="divCompanySegments" class="tab-list list-table">

                                            <!-- company-segment-list -->
                                            <div id="items">
                                                <table class="table table-hover" id="company-segments">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Company Segments</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptCompanySegments">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("CompanySegmentId") %>' data-current-company-segment="<%# Eval("SegmentName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control company-segment-name" Text='<%# Eval("SegmentName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <!-- new-company-segment -->
                                            <div class="new-company-segment new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewCompanySegment" CssClass="form-control" placeholder='Company Segment' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddCompanySegment" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Company Segment</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                    <!--company types-->
                                    <div id="divCompanyTypesTab" class="btab-content" runat="server">

                                        <!--company types list-->
                                        <div id="divCompanyTypes" class="tab-list list-table">
                                            <div id="items">
                                                <table class="table table-hover" id="company-types">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Company Types</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptCompanyTypes">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("CompanyTypeId") %>' data-current-company-type-name="<%# Eval("CompanyTypeName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control company-type-name" Text='<%# Eval("CompanyTypeName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <%-- new-company-type --%>
                                            <div class="new-company-type new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewCompanyType" CssClass="form-control" placeholder='Company Type' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddCompanyType" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Company Type</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <!--competitors-->
                                    <div id="divCompetitorsTab" class="btab-content" runat="server">

                                        <!--competitors list-->
                                        <div id="divCompetitors" class="tab-list list-table">
                                            <div id="items">
                                                <table class="table table-hover" id="competitors">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Competitors</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptCompetitors">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("CompetitorId") %>' data-current-competitor-name="<%# Eval("CompetitorName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control competitor-name" Text='<%# Eval("CompetitorName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <%-- new-competitor --%>
                                            <div class="new-competitor new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewCompetitor" CssClass="form-control" placeholder='Competitor' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddCompetitor" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Competitor</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <!--contact types-->
                                    <div id="divContactTypesTab" class="btab-content" runat="server">

                                        <!--contact types list-->
                                        <div id="divContactTypes" class="tab-list list-table">
                                            <div id="items">
                                                <table class="table table-hover" id="contact-types">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Contact Types</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptContactTypes">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("ContactTypeId") %>' data-current-contact-type="<%# Eval("ContactTypeName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control contact-type-name" Text='<%# Eval("ContactTypeName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <!--new-contact type-->
                                            <div class="new-contact-page new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewContactType" CssClass="form-control" placeholder='Contact Type' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddContactType" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Contact Type</a>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <!--deal types-->
                                    <div id="divDealTypesTab" class="btab-content" runat="server">

                                        <!--deal types list-->
                                        <div id="divDealTypes" class="tab-list list-table">
                                            <div id="items">
                                                <table class="table table-hover" id="deal-types">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Deal Types</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptDealTypes">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("DealTypeId") %>' data-current-deal-type="<%# Eval("DealTypeName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control deal-type-name" Text='<%# Eval("DealTypeName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <!--new-deal type-->
                                            <div class="new-deal-page new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewDealType" CssClass="form-control" placeholder='Deal Type' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddDealType" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Deal Type</a>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <!--industries-->
                                    <div id="divIndustriesTab" class="btab-content" runat="server">

                                        <!--industries list-->
                                        <div id="divIndustries" class="tab-list list-table">
                                            <div id="items">
                                                <table class="table table-hover" id="industries">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Industries</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptIndustries">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("IndustryId") %>' data-current-industry="<%# Eval("IndustryName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control industry-name" Text='<%# Eval("IndustryName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <%-- new-industry --%>
                                            <div class="new-industry new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewIndustry" CssClass="form-control" placeholder='Industry' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddIndustry" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Industry</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <!--lost reasons-->
                                    <div id="divLostReasonsTab" class="btab-content" runat="server">

                                        <!--lost reasons list-->
                                        <div id="divLostReasons" class="tab-list list-table">
                                            <div id="items">
                                                <table class="table table-hover" id="lost-reasons">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Lost Reasons</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptLostReasons">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("LostReasonId") %>' data-current-lost-reason="<%# Eval("LostReasonName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control lost-reason" Text='<%# Eval("LostReasonName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <%-- new-lost-reason --%>
                                            <div class="new-lost-reason new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewLostReason" CssClass="form-control" placeholder='Lost Reason' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddLostReason" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Lost Reason</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                     <!--sales stages-->
                                    <div id="divSalesStagesTab" class="btab-content" runat="server">

                                        <div id="divSalesStages" class="tab-list list-table">
                                            <div id="items">
                                                <table class="table table-hover" id="sales-stages">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Stage Name</th>
                                                            <th class="language-entry">Deal Completion %</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptSalesStages">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("SalesStageId") %>' data-current-sales-stage="<%# Eval("SalesStageName") %>" data-current-sales-percentage="<%# Eval("StagePercentage") %>">
                                                                    <td colspan="2">
                                                                        <div class="item-holder">
                                                                            <i class="move-item icon-move text-primary handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control sales-stage-name" Text='<%# Eval("SalesStageName") %>' runat="server"></asp:TextBox>
                                                                            <asp:TextBox CssClass="form-control text-center numbersOnly sales-percentage" Text='<%# Eval("StagePercentage") %>' runat="server"></asp:TextBox>
                                                                            <span class="input-group-addon percent-label">%</span>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <%-- new-stage --%>
                                            <div class="new-stage new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewSalesStage" placeholder='Stage Name' runat="server"></asp:TextBox>
                                                    <asp:TextBox ID="txtSalesStagePercentage" placeholder='0' CssClass="numbersOnly form-control text-center sales-percentage" runat="server"></asp:TextBox>
                                                    <span class="input-group-addon percent-label">%</span>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddSalesStage" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Stage</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>
                                        <!-- #divSalesStages -->
                                    </div>

                                     <!--sales team roles-->
                                    <div id="divSalesTeamRolesTab" class="btab-content" runat="server">

                                        <div id="divSalesTeamRoles" class="tab-list list-table">

                                            <!-- Sales Team Roles list -->
                                            <div id="items">
                                                <table class="table table-hover" id="sales-team-roles">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Sales Team Roles</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptSalesTeamRoles">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("SalesTeamRoleId") %>' data-current-sales-team-role="<%# Eval("SalesTeamRole1") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control sales-team-role" Text='<%# Eval("SalesTeamRole1") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <!-- new-sales-team-role -->
                                            <div class="new-sales-team-role new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewSalesTeamRole" CssClass="form-control" placeholder='Sales Team Role' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddSalesTeamRole" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Sales Team Role</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                    <!--sources-->
                                    <div id="divSourcesTab" class="btab-content" runat="server">

                                        <!--sources list-->
                                        <div id="divSources" class="tab-list list-table">
                                            <div id="items">
                                                <table class="table table-hover" id="sources">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Sources</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptSources">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("SourceId") %>' data-current-source-name="<%# Eval("SourceName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control source-name W100P" Text='<%# Eval("SourceName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <!--new-source-->
                                            <div class="new-source-name new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewSource" CssClass="form-control" placeholder='Source Type' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddSource" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Source</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <!--tags-->
                                    <div id="divTagsTab" class="btab-content" runat="server">

                                        <!--tags list-->
                                        <div id="divTags" class="tab-list list-table">
                                            <div id="items">
                                                <table class="table table-hover" id="tags">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Tags</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptTags">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("TagId") %>' data-current-tag="<%# Eval("TagName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control tag-name" Text='<%# Eval("TagName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <%-- new-tag  --%>
                                            <div class="new-tag new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewTag" CssClass="form-control" placeholder='Tag' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddTag" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Tag</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                    <!--won reasons-->
                                    <div id="divWonReasonsTab" class="btab-content" runat="server">

                                        <!--won reasons list-->
                                        <div id="divWonReasons" class="tab-list list-table">
                                            <div id="items">
                                                <table class="table table-hover" id="won-reasons">
                                                    <thead>
                                                        <tr>
                                                            <th class="language-entry">Won Reasons</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="rptWonReasons">
                                                            <ItemTemplate>
                                                                <tr data-id='<%# Eval("WonReasonId") %>' data-current-won-reason="<%# Eval("WonReasonName") %>">
                                                                    <td>
                                                                        <div class="item-holder">
                                                                            <i class="text-primary move-item icon-move handle" title="Drag up and down to change the order"></i>
                                                                            <asp:TextBox CssClass="form-control won-reason" Text='<%# Eval("WonReasonName") %>' runat="server"></asp:TextBox>
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <%-- new-won-reason --%>
                                            <div class="new-won-reason new-block">
                                                <div class="form-group clearfix">
                                                    <asp:TextBox ID="txtNewWonReason" CssClass="form-control txtNewWonReason" placeholder='Won Reason' runat="server"></asp:TextBox>
                                                    <span class="add-new-btn">
                                                        <a class="language-entry" id="btnAddWonReason" style="color: white; font-weight: bold"><i class="icon-plus" style="color: white; font-weight: bold"></i>Won Reason</a>
                                                    </span>
                                                </div>
                                            </div>

                                        </div>

                                    </div>
                                
                                    <!--won reasons-->
                                    <div id="divEventCategoriesTab" class="btab-content" runat="server">
                                        <settings:EventCategories runat="server" />
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

<!-- promise polyfill -->
<script src="../../_content/_js/polyfill-loader-17-oct-2019.js"></script>
<script src="settings-12-feb-2020.js"></script>
<script src="/_content/_js/spectrum.js"></script>
<script src="eventcategories/eventcategories-23-mar-2020.js"></script>

</html>
