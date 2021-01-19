<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DealDetail.aspx.cs" Inherits="Crm6.Deals.DealDetail.DealDetail" %>

<%@ Register TagPrefix="uc1" TagName="nav" Src="~/_usercontrols/nav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="navmobile" Src="~/_usercontrols/nav-mobile.ascx" %>
<%@ Register TagPrefix="uc1" TagName="CalendarEventAddEdit" Src="~/_usercontrols/CalendarEventAddEdit/CalendarEventAddEdit.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TaskAddEdit" Src="~/_usercontrols/TaskAddEdit/TaskAddEdit.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TaskNextLastActivity" Src="~/_usercontrols/TaskNextLastActivity/TaskNextLastActivity.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DetailTabTasks" Src="~/_usercontrols/DetailTabTasks/DetailTabTasks.ascx" %>
<%@ Register TagPrefix="uc1" TagName="AddSalesTeamMember" Src="~/_usercontrols/AddSalesTeamMember/AddSalesTeamMember.ascx" %>
<%@ Register TagPrefix="uc1" TagName="AddContact" Src="~/_usercontrols/AddContact/AddContact.ascx" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="robots" content="noindex">
    <title>Deal</title>
    <!-- css  -->
    <link href="/_content/_css/bundle/dropzone.css" rel="stylesheet" />
    <link href="dealdetail-08-apr-2020.css" rel="stylesheet" />
    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="detail-pg deals-page">

    <form runat="server">

        <!-- #main start -->
        <div id="main">

            <%-- hidden values --%>
            <asp:Label Style="display: none" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblUserIdGlobal" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblUsername" runat="server" Text=""></asp:Label>
            <asp:Label Style="display: none" ID="lblDealId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblDealSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblCompanyId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblCompanyOwnerId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblGlobalCompanyId" runat="server" Text="0"></asp:Label>

            <!-- navbar mobile -->
            <uc1:navmobile runat="server" />

            <div class="container-fluid">
                <div class="row">

                    <!-- .page-container-->
                    <div class="page-container">

                        <!-- .page-content-->
                        <div class="page-content">

                            <!-- navbar start -->
                            <uc1:nav runat="server" ID="navSidebar" />

                            <!-- #content start -->
                            <div id="content" class="animated fadeIn wrapper-outer">

                                <!-- .top-header -->
                                <header class="top-header">

                                    <!--top row-->
                                    <div class="clearfix">
                                        <!--breadcrumb-->
                                        <p class="bread_crumb">
                                            <a href="/Deals/DealList/DealList.aspx">Deals</a>
                                            <span class="bread_sep">&rsaquo;</span>
                                            <span>Deal Detail</span>
                                        </p>
                                        <!--edit deal button-->

                                        <%--TODO: cleanup styles--%>
                                        <div class="desktop-header-dropdown">
                                            <div class="dropdown-wrapper header-dropdown">
                                                <a class="edit-button">Edit</a>
                                            </div>
                                        </div>
                                    </div>

                                    <!--deal name-->
                                    <div class="clearfix head_info">
                                        <div class="page-title">
                                            <h1>
                                                <asp:Label ID="lblDealNameTop" runat="server"></asp:Label>
                                                <%--TODO: sales stage badge--%>
                                                <asp:Label ID="lblSalesStage" runat="server"></asp:Label>
                                            </h1>
                                        </div>
                                    </div>

                                    <!-- .mobile-header-dropdown -->
                                    <div class="mobile-header-dropdown">
                                        <div class="dropdown-wrapper header-dropdown">
                                            <a class="edit-button">Edit</a>
                                        </div>
                                    </div>

                                    <div class="panel-heading">
                                        <div class="panel-options">

                                            <%--desktop tabs nav--%>
                                            <div class="desktop-panel-nav">
                                                <ul class="nav nav-tabs " id="deal-tabs">
                                                    <li><a href="#tab-overview" data-toggle="tab" data-set="new_event" class="active"><span class="language-entry">Overview</span></a></li>
                                                    <%--<li data-type="quotes" class=""><a  href="#tab-quotes" data-toggle="tab" data-set="new_quote"><span class="language-entry">Quotes</span></a></li>--%>
                                                    <li data-type="events" class=""><a href="#tab-calendar-events" data-toggle="tab" data-set="new_event"><span class="language-entry">Events</span></a></li>
                                                    <li data-type="tasks" class=""><a href="#tab-tasks" id="tasksHeader" data-toggle="tab" data-set="new_task"><span class="language-entry">Tasks</span></a></li>
                                                    <li data-type="notes" id="liNotes"><a id="aNotes" href="#tab-notes" data-toggle="tab" data-set="new_note"><span class="language-entry">Notes</span></a></li>
                                                    <li data-type="documents" class=""><a href="#tab-documents" data-toggle="tab" data-set="new_doc"><span class="language-entry">Documents</span></a></li>
                                                    <li data-type="contacts" class=""><a href="#tab-deal-contacts" data-toggle="tab" data-set="new_contact"><span class="language-entry">Contacts</span></a></li>
                                                    <li data-type="salesteam" class=""><a href="#tab-sales-team" data-toggle="tab" data-set="new_team"><span class="language-entry">Sales Team</span></a></li>
                                                    <li data-type="activity" class=""><a href="#tab-activity" data-toggle="tab" data-set="new_activity"><span class="language-entry">Activity</span></a></li>
                                                </ul>
                                            </div>

                                            <%--mobile tabs nav--%>
                                            <div class="mobile-panel-nav">
                                                <div class="dropdown-wrapper panel-dropdown">
                                                    <div class="ae-dropdown dropdown">
                                                        <div class="ae-select">
                                                            <span class="ae-select-content"></span>
                                                            <span class="drop-icon-down"><i class="icon-angle-down"></i></span>
                                                            <span class="drop-icon-up"><i class="icon-angle-up"></i></span>
                                                        </div>
                                                        <ul id="deal-tabs" class="dropdown-nav ae-hide nav nav-tabs">
                                                            <li class='selected'><a href="#tab-overview" data-toggle="tab" data-set="new_deal" class="active"><span class="language-entry">Overview</span></a></li>
                                                            <%--<li data-type="quotes" class=""><a href="#tab-quotes" data-toggle="tab" data-set="new_quote"><span class="language-entry">Quotes</span></a></li>--%>
                                                            <li data-type="events" class=""><a href="#tab-calendar-events" data-toggle="tab" data-set="new_event"><span class="language-entry">Events</span></a></li>
                                                            <li data-type="tasks" class=""><a href="#tab-tasks" data-toggle="tab" data-set="new_task"><span class="language-entry">Tasks</span></a></li>
                                                            <li data-type="notes" id="liNotes"><a id="aNotes" href="#tab-notes" data-toggle="tab" data-set="new_note"><span class="language-entry">Notes</span></a></li>
                                                            <li data-type="documents" class=""><a href="#tab-documents" data-toggle="tab" data-set="new_doc"><span class="language-entry">Documents</span></a></li>
                                                            <li data-type="contacts" class=""><a href="#tab-deal-contacts" data-toggle="tab" data-set="new_contact"><span class="language-entry">Contacts</span></a></li>
                                                            <li data-type="salesteam" class=""><a href="#tab-sales-team" data-toggle="tab" data-set="new_team"><span class="language-entry">Sales Team</span></a></li>
                                                            <li data-type="activity" class=""><a href="#tab-activity" data-toggle="tab" data-set="new_activity"><span class="language-entry">Activity</span></a></li>
                                                        </ul>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </header>
                                <!-- .top-header -->

                                <!-- .wrapper -->
                                <div class="wrapper">

                                    <!--.section-content-->
                                    <div class="section-content">
                                        <div class="section-body">
                                            <div class="wrapper wrapper-content">
                                                <div class="row MB30">
                                                    <div class="col-md-12">
                                                        <div class="panel blank-panel">
                                                            <div class="panel-body">
                                                                <div class="tab-content">

                                                                    <!--#tab-overview-->
                                                                    <div class="tab-pane active" id="tab-overview">
                                                                        <div class="row">

                                                                            <div id="left-column-wrapper" class="col-xl-3 col-lg-4 col-md-4 col-sm-12 col-12 col-left-box">

                                                                                <%-- Task next and last activity cards --%>
                                                                                <uc1:TaskNextLastActivity runat="server" ID="TaskNextLastActivity"/>

                                                                                <!--deal detail - left bottom box-->
                                                                                <div class="ibox widget basic-card detail-card">
                                                                                    <div class="ibox-title">
                                                                                        <h3 class="card-title language-entry"><i class="icon-deals title-icon"></i>Deal Detail</h3>
                                                                                    </div>
                                                                                    <div class="ibox-content">
                                                                                        <!--industry-->
                                                                                        <div class="inner-wrp">
                                                                                            <div class="card-status card-label">INDUSTRY</div>
                                                                                            <div class="card-date">
                                                                                                <asp:Label runat="server" ID="lblIndustry"></asp:Label>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--commodities-->
                                                                                        <div class="inner-wrp">
                                                                                            <div class="card-status card-label">COMMODITIES</div>
                                                                                            <div class="card-date">
                                                                                                <asp:Repeater runat="server" ID="lblCommodities">
                                                                                                    <HeaderTemplate><ul class="detail-list"></HeaderTemplate>
                                                                                                    <ItemTemplate><li><%# Container.DataItem %></li></ItemTemplate>
                                                                                                    <FooterTemplate></ul></FooterTemplate>
                                                                                                </asp:Repeater>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--competitors-->
                                                                                        <div class="inner-wrp">
                                                                                            <div class="card-status card-label">COMPETITORS</div>
                                                                                            <div class="card-date">
                                                                                                <asp:Repeater runat="server" ID="lblCompetitors">
                                                                                                    <HeaderTemplate><ul class="detail-list"></HeaderTemplate>
                                                                                                    <ItemTemplate><%# Container.DataItem %></ItemTemplate>
                                                                                                    <FooterTemplate></ul></FooterTemplate>
                                                                                                </asp:Repeater>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--deal type-->
                                                                                        <div class="inner-wrp">
                                                                                            <div class="card-status card-label">DEAL TYPE</div>
                                                                                            <div class="card-date">
                                                                                                <asp:Label runat="server" ID="lblDealType"></asp:Label>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--incoterms-->
                                                                                        <div id="wrpIncoterms" class="inner-wrp" runat="server">
                                                                                            <div class="card-status card-label">INCOTERMS</div>
                                                                                            <div class="card-date">
                                                                                                <asp:Label runat="server" ID="lblIncoterms"></asp:Label>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--campaigns-->
                                                                                        <div id="wrpCampaigns" class="inner-wrp" runat="server">
                                                                                            <div class="card-status card-label">CAMPAIGNS</div>
                                                                                            <div class="card-date">
                                                                                                <asp:Label runat="server" ID="lblCampaigns"></asp:Label>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--last updated-->
                                                                                        <%--note: this section is hidden when there is no data--%>
                                                                                        <div class="inner-wrp">
                                                                                            <div class="card-status card-label">LAST UPDATED</div>
                                                                                            <div class="card-date">
                                                                                                <asp:Label runat="server" ID="lblLastUpdatedDate"></asp:Label>
                                                                                                <asp:Label runat="server" ID="lblLastUpdatedBy"></asp:Label>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--created-->
                                                                                        <%--note: this section is hidden when there is no data--%>
                                                                                        <div class="inner-wrp">
                                                                                            <div class="card-status card-label">CREATED</div>
                                                                                            <div class="card-date">
                                                                                                <asp:Label runat="server" ID="lblCreatedDate"></asp:Label>
                                                                                                <asp:Label runat="server" ID="lblCreatedBy"></asp:Label>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div id="wrpComments" class="inner-wrp" runat="server">
                                                                                            <div class="card-status card-label">COMMENTS</div>
                                                                                            <div class="card-date">
                                                                                                <asp:Label runat="server" ID="lblComments"></asp:Label>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>

                                                                            </div>

                                                                            <!--overview - center column-->
                                                                            <div class="col-xl-6 col-lg-8 col-md-8 col-sm-12 col-12 col-mid-box">

                                                                                <!--key dates - center column-->
                                                                                <div class="ibox middle-wrp middle-card">

                                                                                    <!--key dates header-->
                                                                                    <div class="outer-wrp">
                                                                                        <div class="ibox-title">
                                                                                            <h3 class="card-title language-entry"><i class="icon-calendar title-icon"></i>Key Dates</h3>
                                                                                        </div>
                                                                                    </div>

                                                                                    <%--TODO: edit dates in place--%>
                                                                                    <div class="ibox-content">
                                                                                        <div class="first-block inner-wrp-padding">
                                                                                            <div class="table-wrp">
                                                                                                <table style="width: 100%">
                                                                                                    <tr>
                                                                                                        <td class="col_td">
                                                                                                            <table style="width: 100%">
                                                                                                                <tr class="text-center">
                                                                                                                    <th>PROPOSAL</th>
                                                                                                                    <th>DECISION</th>
                                                                                                                </tr>
                                                                                                                <tr class="text-center">
                                                                                                                    <td>
                                                                                                                        <asp:Label runat="server" ID="lblProposalDate"></asp:Label>
                                                                                                                    </td>
                                                                                                                    <td>
                                                                                                                        <asp:Label runat="server" ID="lblDecisionDate"></asp:Label>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </td>
                                                                                                        <td class="col_td">
                                                                                                            <table style="width: 100%">
                                                                                                                <tr class="text-center">
                                                                                                                    <th>FIRST SHIPMENT</th>
                                                                                                                    <th>CONTRACT END</th>
                                                                                                                </tr>
                                                                                                                <tr class="text-center">
                                                                                                                    <td>
                                                                                                                        <asp:Label runat="server" ID="lblFirstShipmentDate"></asp:Label>
                                                                                                                    </td>
                                                                                                                    <td>
                                                                                                                        <asp:Label runat="server" ID="lblContractEndDate"></asp:Label>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>

                                                                                <!--lanes-->
                                                                                <div class="second-block ibox middle-wrp middle-card">

                                                                                    <!--lanes header-->
                                                                                    <div class="outer-wrp">
                                                                                        <div class="ibox-title clearfix">
                                                                                            <%--TODO: NEW lanes icon--%>
                                                                                            <h3 class="card-title language-entry"><i class="icon-Notes title-icon"></i>Lanes</h3>
                                                                                            <div class="ibox-tools FR">
                                                                                                <a class="edit_link" data-action="new-lane">
                                                                                                    <i class="icon-plus"></i><span>Lane</span>
                                                                                                </a>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <div class="ibox-content list-lanes">
                                                                                        <asp:Repeater runat="server" ID="rptLanes">
                                                                                            <ItemTemplate>
                                                                                                <div class="outer-wrp first-wrp inner-wrp-padding" data-id="<%# Eval("LaneId")%>" data-subscriber-id="<%# Eval("SubscriberId")%>">
                                                                                                    <div class="dl-title-wrp clearfix">

                                                                                                        <table class="lane-table" style="width: 100%; margin-bottom: 10px;">
                                                                                                            <tr>
                                                                                                                <td height="30">
                                                                                                                     <i class="<%# Eval("Service").ToString() == "Air" ? "icon-airplane": (Eval("Service").ToString().Contains("Road") ? "icon-road": (Eval("Service").ToString().Contains("Ocean") ? "icon-ocean":( Eval("Service").ToString().Contains("Logistics") ? "icon-logistics":"icon-Brokerage----White")) )%>  "></i>
                                                                                                                </td>
                                                                                                                <td>
                                                                                                                    <div class="left-wrp">
                                                                                                                        <span class="title">
                                                                                                                            <asp:Label runat="server" Text='<%# Eval("Service")%>'></asp:Label>
                                                                                                                        </span>
                                                                                                                    </div>
                                                                                                                </td>
                                                                                                                <td>&nbsp;</td>
                                                                                                                <td>
                                                                                                                    <div class="right-wrp">
                                                                                                                        <div class="ibox-tools edit-lnk-wrp">
                                                                                                                            <a class="text-blue edit_link" data-action="edit-note" href="/deals/laneaddedit/laneaddedit.aspx?laneId=<%# Eval("LaneId")%>">
                                                                                                                                <i class="icon-edit edit-icon"></i>
                                                                                                                            </a>
                                                                                                                        </div>
                                                                                                                    </div>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                    </div>
                                                                                                    <div class="table-wrp">
                                                                                                        <table class="lane-table" style="width: 100%; margin-bottom: 10px;">
                                                                                                            <tr  class="<%#  Eval("Service").ToString().Contains("Logistics") || Eval("Service").ToString().Contains("Brokerage")  ? "hide":"" %> ">
                                                                                                                <td>ORIGIN</td>
                                                                                                                <th>
                                                                                                                    <asp:Label
                                                                                                                        runat="server"
                                                                                                                        Text='<%#Eval("Service").ToString().Contains("Logistics") ? Eval("ServiceLocation") : Eval("OriginName")%>'>
                                                                                                                    </asp:Label>
                                                                                                                </th>
                                                                                                                <th>
                                                                                                                    <asp:Label
                                                                                                                        runat="server"
                                                                                                                        Text='<%#Eval("Service").ToString().Contains("Air") == false ? Eval("OriginUnlocoCode") : Eval("OriginIataCode")%>'>
                                                                                                                    </asp:Label>
                                                                                                                </th>
                                                                                                                <th>
                                                                                                                    <asp:Label
                                                                                                                        runat="server"
                                                                                                                        Text='<%#Eval("Service").ToString().Contains("Logistics") ? "" : Eval("OriginCountryName")%>'>
                                                                                                                    </asp:Label>
                                                                                                                </th>
                                                                                                            </tr>
                                                                                                            <tr class="darkClr <%#  Eval("Service").ToString().Contains("Logistics") || Eval("Service").ToString().Contains("Brokerage")  ? "hide":"" %> ">
                                                                                                                <td>DESTINATION</td>
                                                                                                                <th>
                                                                                                                    <asp:Label
                                                                                                                        runat="server"
                                                                                                                        Text='<%#Eval("Service").ToString().Contains("Logistics") ? "" : Eval("DestinationName")%>'>
                                                                                                                    </asp:Label>
                                                                                                                </th>
                                                                                                                <%--destination code--%>
                                                                                                                <th>
                                                                                                                    <%--IATA or UNLOCO Code--%>
                                                                                                                    <asp:Label
                                                                                                                        runat="server"
                                                                                                                        Text='<%#Eval("Service").ToString().Contains("Air") == false ? Eval("DestinationUnLocoCode") : Eval("DestinationIataCode")%>'>
                                                                                                                    </asp:Label>
                                                                                                                </th>
                                                                                                                <th>
                                                                                                                    <asp:Label
                                                                                                                        runat="server"
                                                                                                                        Text='<%#Eval("Service").ToString().Contains("Logistics") ? "" : Eval("DestinationCountryName")%>'>
                                                                                                                    </asp:Label>
                                                                                                                </th>
                                                                                                            </tr>
                                                                                                            <tr><td style="margin-top:20px"></td></tr>
                                                                                                            <tr>
                                                                                                                <td></td>
                                                                                                                <th>VOLUME</th>
                                                                                                                <th>REVENUE</th>
                                                                                                                <th>PROFIT</th>
                                                                                                            </tr>
                                                                                                            <tr class="darkClr">
                                                                                                                <td></td>
                                                                                                                <%--volume--%>
                                                                                                                <td class="small-td">
                                                                                                                    <asp:Label
                                                                                                                        runat="server"
                                                                                                                        CssClass="format-number"
                                                                                                                        Text='<%# Eval("VolumeAmount")%>'>
                                                                                                                    </asp:Label>
                                                                                                                    &nbsp
                                                                                                                    <asp:Label
                                                                                                                        runat="server"
                                                                                                                        Text='<%# Eval("VolumeUnit")%>'>
                                                                                                                    </asp:Label>
                                                                                                                    &nbsp
                                                                                                                    <%-- 
                                                                                                                    <asp:Label 
                                                                                                                        runat="server" 
                                                                                                                        Text='<%# Eval("ShippingFrequency")%>'>
                                                                                                                    </asp:Label>
                                                                                                                    --%>
                                                                                                                </td>
                                                                                                                <%--revenue amount--%>
                                                                                                                <td class="txt-left small-td">
                                                                                                                    <%# Eval("CurrencyCode")%>&nbsp
                                                                                                                    <asp:Label
                                                                                                                        runat="server"
                                                                                                                        CssClass="format-number"
                                                                                                                        Text='<%# Eval("Revenue")%>'>
                                                                                                                    </asp:Label>
                                                                                                                </td>
                                                                                                                <td class="small-td">
                                                                                                                    <%# Eval("CurrencyCode")%>&nbsp
                                                                                                                    <asp:Label
                                                                                                                        runat="server"
                                                                                                                        CssClass="format-number"
                                                                                                                        Text='<%# Eval("TotalLaneProfit")%>'>
                                                                                                                    </asp:Label>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </ItemTemplate>
                                                                                        </asp:Repeater>
                                                                                    </div>

                                                                                    <div class="no-data-wrp">
                                                                                        <div class="text-center PB30  PT30" id="noLanes" runat="server" visible="false">
                                                                                            <i class="icon-Notes"></i>
                                                                                            <p class="language-entry no-data">no lanes</p>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>

                                                                                <!--add note-->
                                                                                <div class="ibox overview-note note-textarea">
                                                                                    <div>
                                                                                        <asp:TextBox ID="txtNote" runat="server" TextMode="MultiLine" Rows="3" placeholder="note"></asp:TextBox>
                                                                                        <div class="add-note-btn" id="note-add">
                                                                                            <a href="javascript:void(0)" class="primary-btn btnAddNote language-entry">Add Note</a>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>

                                                                            </div>

                                                                            <!--company details - right column-->
                                                                            <div id="right-column-wrapper" class="col-xl-3 col-lg-4 col-md-4 col-sm-12 col-12 col-right-box">

                                                                                <div class="right-col">

                                                                                    <div class="ibox basic-card">
                                                                                        <div class="ibox-title">
                                                                                            <h3 class="card-title language-entry"><i class="icon-business title-icon"></i>Company</h3>
                                                                                            <div class="edit-link FR">
                                                                                                <a href="#" data-action="edit-company"><i class="icon-edit edit-icon"></i></a>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="ibox-content">
                                                                                            <div class="clearfix comp-info row no-gutters">
                                                                                                <div class="comp-logo col-auto align-self-center" runat="server" id="divCompanyLogo">
                                                                                                    <img id="imgCompany" runat="server" src="_img/cd-logo.png" />
                                                                                                </div>
                                                                                                <div class="col">
                                                                                                    <a class="comp-txt" href="javascript:void(0);" data-action='company-detail' runat="server">
                                                                                                        <span>
                                                                                                            <asp:Label ID="lblCompanyName" runat="server"></asp:Label>
                                                                                                        </span>
                                                                                                    </a>
                                                                                                </div>
                                                                                            </div>
                                                                                            <!--address-->
                                                                                            <div id="wrpAddress" class="info-icon" runat="server">
                                                                                                <div class="row-icon address-icon">
                                                                                                    <asp:Label runat="server" ID="lblAddress"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <!--phone-->
                                                                                            <div id="wrpPhone" class="info-icon" runat="server">
                                                                                                <a href="tel:704-993-3933" class="row-icon phone-icon">
                                                                                                    <asp:Label runat="server" ID="lblPhone"></asp:Label>
                                                                                                </a>
                                                                                            </div>
                                                                                            <!--fax-->
                                                                                            <div id="wrpFax" class="info-icon" runat="server">
                                                                                                <div class="row-icon fax-icon">
                                                                                                    <asp:Label runat="server" ID="lblFax"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <!--website-->
                                                                                            <div id="wrpWebsite" class="info-icon" runat="server">
                                                                                                <a runat="server" id="aLinkWebsite" class="row-icon web-icon" target="_blank">
                                                                                                    <asp:Label runat="server" ID="lblWebsite"></asp:Label>
                                                                                                </a>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <!--primary contact - right column-->
                                                                                    <div class="ibox basic-card" id="divPrimaryContactContainer" runat="server">
                                                                                        <div class="ibox-title">
                                                                                            <h3 class="card-title"><i class="icon-contacts title-icon"></i>Contact</h3>
                                                                                        </div>
                                                                                        <div class="ibox-content">
                                                                                            <div class="user-wrap clearfix">
                                                                                                <div class="user-img FL" id="divPrimaryContactImage" runat="server">
                                                                                                    <img src="/_content/_img/no-pic.png" runat="server" id="imgPrimaryContact">
                                                                                                </div>
                                                                                                <div class="user-info FL">
                                                                                                    <a href="#" class="user-name">
                                                                                                        <asp:Label runat="server" ID="lblPrimaryContactName"></asp:Label>
                                                                                                    </a>
                                                                                                    <div class="user-prof">
                                                                                                        <asp:Label runat="server" ID="lblPrimaryContactPosition"></asp:Label>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                            <!--address icon-->
                                                                                            <div class="info-icon">
                                                                                                <div class="row-icon address-icon">
                                                                                                    <asp:Label runat="server" ID="lblPrimaryContactCountry"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <!--phone icon-->
                                                                                            <div class="info-icon">
                                                                                                <a href="tel:" class="row-icon phone-icon user-tel" runat="server" id="aPrimaryContactPhone">
                                                                                                    <asp:Label runat="server" ID="lblPrimaryContactPhone"></asp:Label>
                                                                                                </a>
                                                                                            </div>
                                                                                            <!--email icon-->
                                                                                            <div class="info-icon">
                                                                                                <a href="mailto:" class=" row-icon email-icon user-mail hover-link" runat="server" id="aPrimaryContactEmail">
                                                                                                    <asp:Label runat="server" ID="lblPrimaryContactEmail"></asp:Label>
                                                                                                </a>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <!--sales owner - right column-->
                                                                                    <div class="ibox basic-card" id="divSalesOwnerContainer" runat="server">
                                                                                        <div class="ibox-title">
                                                                                            <h3 class="card-title"><i class="icon-users title-icon"></i>Sales Owner</h3>
                                                                                        </div>
                                                                                        <div class="ibox-content">
                                                                                            <div class="user-wrap clearfix">
                                                                                                <div class="user-img FL" id="divSalesOwnerImage" runat="server">
                                                                                                    <img src="/_content/_img/no-pic.png" runat="server" id="imgProfile">
                                                                                                </div>
                                                                                                <div class="user-info FL">
                                                                                                    <asp:Label runat="server" ID="lblSalesOwnerId" CssClass="hide" Text="0"></asp:Label>
                                                                                                    <div class="user-name">
                                                                                                        <asp:Label runat="server" ID="lblSalesRepName"></asp:Label>
                                                                                                    </div>
                                                                                                    <div class="user-prof">
                                                                                                        <asp:Label runat="server" ID="lblSalesRepPosition"></asp:Label>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                            <!--address icon-->
                                                                                            <div class="info-icon">
                                                                                                <div class="row-icon address-icon">
                                                                                                    <asp:Label runat="server" ID="lblSalesRepCountry"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <!--phone icon-->
                                                                                            <div class="info-icon">
                                                                                                <a href="tel:" class=" row-icon phone-icon user-tel" runat="server" id="aSalesRepPhone">
                                                                                                    <asp:Label runat="server" ID="lblSalesRepPhone"></asp:Label>
                                                                                                </a>
                                                                                            </div>
                                                                                            <!--email icon-->
                                                                                            <div class="info-icon">
                                                                                                <a href="mailto:" class="row-icon email-icon user-mail hover-link" id="aSalesRepEmail" runat="server">
                                                                                                    <asp:Label runat="server" ID="lblSalesRepEmail"></asp:Label>
                                                                                                </a>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                </div>
                                                                            </div>

                                                                        </div>
                                                                    </div>
                                                                    <!--#tab-overview-->

                                                                    <!--#tab-calendar-events-->
                                                                    <div class="tab-pane" id="tab-calendar-events">
                                                                        <div class="row">
                                                                            <div class="col-xl-12 col-lg-12 col-md-12">
                                                                                <div class="events-wrap ibox basic-card">
                                                                                    <div class="ibox-title clearfix">
                                                                                        <h3 class="card-title language-entry"><i class="icon-big-calendar title-icon"></i>Events</h3>
                                                                                        <div class="ibox-tools FR">
                                                                                            <a class="edit_link" data-action="add-event">
                                                                                                <i class="icon-plus"></i><span>Event</span>
                                                                                            </a>
                                                                                        </div>
                                                                                    </div>
                                                                                    <!-- #divEvents -->
                                                                                    <div class="events" id="divEvents"></div>
                                                                                    <div class="no-events empty-box empty_event hide">
                                                                                        <i class="icon-big-calendar"></i>
                                                                                        <p class="e-text">no events</p>
                                                                                        <div class="btn-wrp">
                                                                                            <a href="#" data-action="add-event" class="primary-btn W200">New Event</a>
                                                                                        </div>
                                                                                    </div>

                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <!--#tab-calendar-events-->

                                                                    <%-- Tasks Tab --%>
                                                                    <uc1:DetailTabTasks runat="server" ID="DetailTabTasks" DetailType="deal"/>

                                                                    <!--#tab-notes-->
                                                                    <div class="tab-pane" id="tab-notes">
                                                                        <div class="row">

                                                                            <!-- Notes List : left column -->
                                                                            <div class="col-lg-7 col-md-6  col-left-box">
                                                                                <div class="note-listing basic-card">
                                                                                    <div class="ibox fullHeight">
                                                                                        <div class="ibox-title">
                                                                                            <h3 class="card-title language-entry"><i class="icon-Notes title-icon"></i>Notes</h3>
                                                                                        </div>
                                                                                        <div id="divNotes" class="notes"></div>
                                                                                        <div class="no-notes empty-box hide tableDisplay">
                                                                                            <div class="tableCell">
                                                                                                <i class="icon-Notes"></i>
                                                                                                <p class="e-text language-entry">no notes</p>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <!-- Notes List : left column -->

                                                                            <!-- Add Note : right column -->
                                                                            <div class="col-lg-5 col-md-6 col-right-box">
                                                                                <div class="add-note add-note-wrp">
                                                                                    <form class="add-note-form" action="#">
                                                                                        <asp:TextBox ID="txtNoteNotesSection" runat="server" TextMode="MultiLine" Rows="3" placeholder="note"></asp:TextBox>
                                                                                        <div class="add-note-btn" id="note-add-actions">
                                                                                            <a href="javascript:void(0)" class="primary-btn btnAddNote language-entry">Add Note</a>
                                                                                        </div>
                                                                                        <div class="add-note-btn hide btn-wrp text-center" id="note-edit-actions">
                                                                                            <button type="button" style="float: none" class="secondary-btn cancel-btn btnNoteCancel language-entry">Cancel</button>
                                                                                            <a href="javascript:void(0)" class="primary-btn btnSaveNote col-md-4 language-entry">Save</a>
                                                                                        </div>
                                                                                    </form>
                                                                                </div>
                                                                            </div>
                                                                            <!-- Add Note : right column -->

                                                                        </div>
                                                                    </div>
                                                                    <!--#tab-notes-->

                                                                    <!--#tab-documents-->
                                                                    <div class="tab-pane" id="tab-documents">
                                                                        <div class="row">

                                                                            <!--Documents list : left column-->
                                                                            <div class="col-md-7 col-left-box">
                                                                                <div class="docListing basic-card">
                                                                                    <div class="ibox fullHeight">
                                                                                        <div class="ibox-title">
                                                                                            <h3 class="card-title language-entry"><i class="icon-Empty-Doc title-icon"></i>Documents</h3>
                                                                                        </div>
                                                                                        <div class="documents" id="divDocuments"></div>
                                                                                        <div class="no-docs empty-box hide no-deals-documents tableDisplay">
                                                                                            <div class="tableCell">
                                                                                                <i class="icon-Empty-Doc"></i>
                                                                                                <p class="e-text">no documents</p>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <!--Documents list : left column-->

                                                                            <!--Documents form : right column-->
                                                                            <div class="col-md-5 col-right-box">
                                                                                <div class="ibox uploadDoc basic-card">
                                                                                    <div class="ibox-title">
                                                                                        <h3 class="card-title language-entry">Upload Document</h3>
                                                                                    </div>
                                                                                    <div class="ibox-content">
                                                                                        <div class="add-document">
                                                                                            <asp:TextBox ID="txtDocumentTitle" CssClass="doc_title" runat="server" placeholder="title"></asp:TextBox>
                                                                                            <asp:TextBox ID="txtDocumentDescription" TextMode="MultiLine" Rows="3" CssClass="doc_description" runat="server" placeholder="description"></asp:TextBox>
                                                                                            <asp:Label runat="server" ID="lblDocUploadDetails" CssClass="hide"></asp:Label>
                                                                                            <div id="drop-wrp" class="drag-drop-wrp">
                                                                                                <div class="dz-message needsclick">
                                                                                                    <input id="fuUploadDocument" name="fuUploadDocument" type="file" class="hide" />
                                                                                                    <div id="drop_here" class="drop-here">
                                                                                                        <div>Drag and drop here</div>
                                                                                                    </div>
                                                                                                    <div class="or-sep">- or -</div>
                                                                                                    <button type="button" class="secondary-btn">Browse</button>
                                                                                                </div>
                                                                                            </div>
                                                                                            <!--Add Upload Document button-->
                                                                                            <a id="btnUploadDocument" class="primary-btn" href="javascript:void(0)">Upload Document</a>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <!--Documents form : right column-->

                                                                        </div>
                                                                    </div>
                                                                    <!--#tab-documents-->

                                                                    <!--#tab-deal-contacts-->
                                                                    <div class="tab-pane" id="tab-deal-contacts">
                                                                        <div class="row">
                                                                            <div class="col-md-12 contactBox">
                                                                                <div class="row no-gutters deal-contacts"></div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <!--#tab-deal-contacts-->

                                                                    <!--#tab-sales-team-->
                                                                    <div class="tab-pane" id="tab-sales-team">
                                                                        <div class="row">
                                                                            <div class="col-md-12 contactBox">
                                                                                <div class="row no-gutters deal-users"></div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <!--#tab-sales-team-->

                                                                    <!--#tab-activity-->
                                                                    <div class="tab-pane" id="tab-activity">
                                                                        <div class="row">
                                                                            <div class="col-md-12">
                                                                                <div class="list-table">
                                                                                    <table id="activity-datatable" class="activity-table">
                                                                                        <thead>
                                                                                            <tr>
                                                                                                <th>Date<i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i></th>
                                                                                                <th>Action</th>
                                                                                                <th>User</th>
                                                                                                <th>Description</th>
                                                                                            </tr>
                                                                                        </thead>
                                                                                        <tbody>
                                                                                            <asp:Repeater runat="server" ID="rptUserActivity">
                                                                                                <ItemTemplate>
                                                                                                    <tr>
                                                                                                        <td class="date">
                                                                                                            <asp:Label runat="server" Text='<%# Eval("UserActivityTimestamp", "{0:dd-MMM-yy - HH:mm}") %>'></asp:Label>
                                                                                                        </td>
                                                                                                        <td>
                                                                                                            <i class="icon-edit"></i><!--Deal Updated-->
                                                                                                            <i class="icon-contacts hide"></i><!--Contact Added-->
                                                                                                            <i class="icon-calendar hide"></i><!--Event Created-->
                                                                                                            <i class="icon-task hide"></i><!--Task Created-->
                                                                                                            <asp:Label runat="server" Text='<%# Eval("TaskName") %>'></asp:Label>
                                                                                                        </td>
                                                                                                        <td>
                                                                                                            <asp:Label runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
                                                                                                        </td>
                                                                                                        <td>
                                                                                                            <asp:Label runat="server" Text='<%# Eval("UserActivityMessage") %>'></asp:Label>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </ItemTemplate>
                                                                                            </asp:Repeater>
                                                                                        </tbody>
                                                                                    </table>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <!--#tab-activity-->

                                                                </div>
                                                                <!-- .tab-content -->
                                                            </div>
                                                            <!-- .panel-body -->
                                                        </div>
                                                        <!-- .blank-panel -->
                                                    </div>
                                                </div>
                                                <!-- .tabs-holder -->
                                            </div>
                                            <!-- .wrapper-content -->
                                        </div>
                                        <!-- .section-body -->
                                    </div>
                                    <!-- .section-content -->
                                </div>
                                <!-- .wrapper -->
                                <%--</form><!-- form end -->--%>
                            </div>
                            <!-- #content end -->

                            <%-- CALENDAR EVENT ADD/EDIT --%>
                            <uc1:CalendarEventAddEdit runat="server" ID="CalendarEventAddEdit" DetailType="deal"/>

                        </div>
                        <!-- .page-content -->
                        </div>
                        <!-- .page-container -->
                    </div>
                    <!-- .row -->
                </div>
                <!-- .container-fluid -->

                <%-- Task add/edit (modal popup) --%>
                <uc1:TaskAddEdit runat="server" ID="TaskAddEdit" />

                <%--add contact dialog--%>
                <uc1:AddContact runat="server" ID="AddContact" />

                <%--add sales team member dialog--%>
                <uc1:AddSalesTeamMember runat="server" ID="AddSalesTeamMember" />

            </div>
            <!-- #main end -->
        </div>

    </form>

    <!-- js libraries -->
    <script src="/_content/_js/polyfill-loader-17-oct-2019.js"></script>
    <script src="/_content/_js/cookies-201710301235.js"></script>
    <script src="/_content/_js/bundle/moment.js"></script>
    <script src="/_content/_js/fullcalendar.min.js"></script>

    <!-- page -->
    <script src="dealdetail-08-apr-2020.js"></script>

</body>
</html>
