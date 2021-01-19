<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyDetail.aspx.cs" Inherits="Crm6.Companies.CompanyDetail.CompanyDetail" EnableEventValidation="false" %>

<%@ Register TagPrefix="uc1" TagName="nav" Src="~/_usercontrols/nav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="navmobile" Src="~/_usercontrols/nav-mobile.ascx" %>
<%@ Register TagPrefix="uc1" TagName="CalendarEventAddEdit" Src="~/_usercontrols/CalendarEventAddEdit/CalendarEventAddEdit.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TaskAddEdit" Src="~/_usercontrols/TaskAddEdit/TaskAddEdit.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TaskNextLastActivity" Src="~/_usercontrols/TaskNextLastActivity/TaskNextLastActivity.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DetailTabTasks" Src="~/_usercontrols/DetailTabTasks/DetailTabTasks.ascx" %>
<%@ Register TagPrefix="uc1" TagName="AddSalesTeamMember" Src="~/_usercontrols/AddSalesTeamMember/AddSalesTeamMember.ascx" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="robots" content="noindex">

    <title>Company</title>

    <!--css custom-->
    <link href="companydetail-07-apr-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="detail-pg deals-page">

    <form runat="server" id="divCompanyDetail" class="page-content">

        <!--#main start-->
        <div id="main">

            <!--hidden values-->
            <asp:Label Style="display: none" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblUserIdGlobal" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblCompanySubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblCompanyId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblCompanyOwnerId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblGlobalCompanyId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblUsername" runat="server" Text=""></asp:Label>

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
                                <form>
                                    <!-- .top-header -->
                                    <header class="top-header">

                                        <!--breadcrumb-->
                                        <div class="clearfix">

                                            <p class="bread_crumb">
                                                <a href="/Companies/CompanyList/CompanyList.aspx">Companies</a>
                                                <span class="bread_sep">&rsaquo;</span>
                                                <span>Company Detail</span>
                                            </p>

                                            <!-- .desktop-header -->
                                            <div class="desktop-header-dropdown">
                                                <div class="dropdown-wrapper header-dropdown">
                                                    <a class="edit-button" data-action="edit-company">Edit</a>
                                                    <asp:Button CssClass="primary-btn hide" OnClick="btnReassignCompany_Click" ID="btnReassignCompany" runat="server" Text="Reassign Company" />
                                                </div>
                                            </div>
                                            <!-- .desktop-header -->
                                        </div>
                                        <!--.bread_crumb-->

                                        <div class="clearfix head_info">
                                            <!--company logo-->
                                            <span class="company-logo FL" runat="server" id="divCompanyLogo">
                                                <img id="imgCompany" runat="server" src="../../_content/_img/cd-logo.png" />
                                            </span>

                                            <!--company name and division-->
                                            <div class="page-title">
                                                <h1>
                                                    <asp:Label ID="lblCompanyName" runat="server"></asp:Label>
                                                    <asp:Label class="page-title-division" ID="lblDivision" runat="server"></asp:Label>
                                                </h1>
                                            </div>
                                        </div>

                                        <!-- .mobile-header-dropdown -->
                                        <div class="mobile-header-dropdown">
                                            <div class="dropdown-wrapper header-dropdown">
                                                <a class="edit-button"  data-action="edit-company">Edit</a>
                                            </div>
                                        </div>

                                        <!--nav menu for tabs-->
                                        <div class="panel-heading">
                                            <div class="panel-options">
                                                <%--desktop tabs nav--%>
                                                <div class="desktop-panel-nav">
                                                    <ul class="nav nav-tabs " id="company-tabs">
                                                        <li><a href="#tab-overview" data-toggle="tab" data-set="new_event" class="active"><span class="language-entry">Overview</span></a></li>
                                                        <li data-type="deals" class=""><a href="#tab-deals" data-toggle="tab" data-set="new_deal"><span class="language-entry">Deals</span></a></li>
                                                        <%--<li data-type="quotes" class=""><a  href="#tab-quotes" data-toggle="tab" data-set="new_quote"><span class="language-entry">Quotes</span></a></li>--%>
                                                        <li data-type="events" class=""><a href="#tab-calendar-events" data-toggle="tab" data-set="new_event"><span class="language-entry">Events</span></a></li>
                                                        <li data-type="tasks" class=""><a href="#tab-tasks" id="tasksHeader" data-toggle="tab" data-set="new_task"><span class="language-entry">Tasks</span></a></li>
                                                        <li data-type="notes" id="liNotes"><a id="aNotes" href="#tab-notes" data-toggle="tab" data-set="new_note"><span class="language-entry">Notes</span></a></li>
                                                        <li data-type="documents" class=""><a href="#tab-documents" data-toggle="tab" data-set="new_doc"><span class="language-entry">Documents</span></a></li>
                                                        <li data-type="contacts" class=""><a href="#tab-company-contacts" data-toggle="tab" data-set="new_contact"><span class="language-entry">Contacts</span></a></li>
                                                        <li data-type="salesteam" class=""><a href="#tab-sales-team" data-toggle="tab" data-set="new_team"><span class="language-entry">Sales Team</span></a></li>
                                                        <li data-type="activity" class=""><a href="#tab-activity" data-toggle="tab" data-set="new_activity"><span class="language-entry">Activity</span></a></li>
                                                        <li data-type="relatedcompanies" class=""><a href="#tab-related-companies" data-toggle="tab" data-set="new_related_companies"><span class="language-entry">Related Companies</span></a></li>
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
                                                            <ul id="company-tabs" class="dropdown-nav ae-hide nav nav-tabs">
                                                                <li class='selected'><a href="#tab-overview" data-toggle="tab" data-set="new_deal" class="active"><span class="language-entry">Overview</span></a></li>
                                                                <li data-type="deals" class=""><a href="#tab-deals" data-toggle="tab" data-set="new_deal"><span class="language-entry">Deals</span></a></li>
                                                                <%--<li data-type="quotes" class=""><a href="#tab-quotes" data-toggle="tab" data-set="new_quote"><span class="language-entry">Quotes</span></a></li>--%>
                                                                <li data-type="events" class=""><a href="#tab-calendar-events" data-toggle="tab" data-set="new_event"><span class="language-entry">Events</span></a></li>
                                                                <li data-type="tasks" class=""><a href="#tab-tasks" data-toggle="tab" data-set="new_task"><span class="language-entry">Tasks</span></a></li>
                                                                <li data-type="notes" id="liNotes"><a id="aNotes" href="#tab-notes" data-toggle="tab" data-set="new_note"><span class="language-entry">Notes</span></a></li>
                                                                <li data-type="documents" class=""><a href="#tab-documents" data-toggle="tab" data-set="new_doc"><span class="language-entry">Documents</span></a></li>
                                                                <li data-type="contacts" class=""><a href="#tab-company-contacts" data-toggle="tab" data-set="new_contact"><span class="language-entry">Contacts</span></a></li>
                                                                <li data-type="salesteam" class=""><a href="#tab-sales-team" data-toggle="tab" data-set="new_team"><span class="language-entry">Sales Team</span></a></li>
                                                                <li data-type="activity" class=""><a href="#tab-activity" data-toggle="tab" data-set="new_activity"><span class="language-entry">Activity</span></a></li>
                                                                <li data-type="relatedcompanies" class=""><a href="#tab-related-companies" data-toggle="tab" data-set="new_related_companies"><span class="language-entry">Related Companies</span></a></li>
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
                                                    <div class="row tabs-holder">
                                                        <div class="col-md-12">
                                                            <!--.panel.blank-panel-->
                                                            <div class="panel blank-panel">
                                                                <!--.panel-body-->
                                                                <div class="panel-body">
                                                                    <!--.tab-content-->
                                                                    <div class="tab-content">

                                                                        <!--#tab-overview-->
                                                                        <div class="tab-pane active" id="tab-overview">
                                                                            <div class="row">
                                                                                <div class="col-xl-3 col-lg-4 col-md-4 col-sm-12 col-12 col-left-box">

                                                                                    <%-- Task next and last activity cards --%>
                                                                                    <uc1:TaskNextLastActivity runat="server" ID="TaskNextLastActivity"/>

                                                                                    <!--details - left column-->
                                                                                    <div class="ibox widget basic-card detail-card">
                                                                                        <div class="ibox-title">
                                                                                            <h3 class="card-title language-entry"><i class="icon-Notes title-icon"></i>Details</h3>
                                                                                        </div>
                                                                                        <div class="ibox-content">
                                                                                            <div class="inner-wrp">
                                                                                                <div class="card-status card-label">SOURCE</div>
                                                                                                <div class="card-date">
                                                                                                    <asp:Label runat="server" ID="lblLeadSource"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="inner-wrp">
                                                                                                <div class="card-status card-label">Type</div>
                                                                                                <div class="card-date">
                                                                                                    <asp:Label runat="server" ID="lblCompanyType"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="inner-wrp">
                                                                                                <div class="card-status card-label">Industry</div>
                                                                                                <div class="card-date">
                                                                                                    <asp:Label runat="server" ID="lblIndustry"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="inner-wrp">
                                                                                                <div class="card-status card-label">PROFILE UPDATED</div>
                                                                                                <div class="card-date">
                                                                                                    <asp:Label runat="server" ID="lblLastUpdatedDate"></asp:Label>
                                                                                                    <asp:Label runat="server" ID="lblLastUpdatedBy"></asp:Label>
                                                                                                </div>
                                                                                            </div>
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

                                                                                <div class="col-xl-6 col-lg-8 col-md-8 col-sm-12 col-12 col-mid-box">

                                                                                    <!--deals - center column-->
                                                                                    <div class="ibox middle-wrp middle-card second-block">

                                                                                        <!--deals header-->
                                                                                        <div class="outer-wrp">
                                                                                            <div class="ibox-title">
                                                                                                <h3 class="card-title language-entry"><i class="icon-deals title-icon"></i>Deals</h3>
                                                                                                <div class="ibox-tools FR">
                                                                                                    <a class="edit_link" data-action="new-deal">
                                                                                                        <i class="icon-plus"></i><span>Deal</span>
                                                                                                    </a>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>

                                                                                        <div class="ibox-content">

                                                                                            <!-- Fetch Deals -->
                                                                                            <div class="deals"></div>
                                                                                            <div class="overview-nodeals empty-box hide">
                                                                                                <i class="icon-Deal----Gray"></i>
                                                                                                <p class="e-text">no deals</p>
                                                                                                <div class="btn-wrp">
                                                                                                    <a href="javascript:void(0)" data-action="new-deal" class="primary-btn">New Deal</a>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>

                                                                                    </div>

                                                                                    <!--add note-->
                                                                                    <div class="ibox overview-note note-textarea">
                                                                                        <div class="form-group filled">
                                                                                            <asp:TextBox runat="server" ID="txtNote" TextMode="MultiLine" placeholder="note" Rows="3" MaxLength="300"></asp:TextBox>
                                                                                           <div class="add-note-btn" id="note-add">
                                                                                            <a href="javascript:void(0)" class="primary-btn btnAddNote language-entry">Add Note</a>
                                                                                        </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <!-- .col-mid-box -->

                                                                                <div class="col-xl-3 col-lg-4 col-md-4 col-sm-12 col-12 col-right-box">

                                                                                    <div class="right-col">

                                                                                        <!--company details - right column-->
                                                                                        <div class="ibox basic-card" ID="companyInfo" runat="server">
                                                                                            <div class="ibox-title">
                                                                                                <h3 class="card-title language-entry"><i class="icon-business title-icon"></i>Info</h3>
                                                                                                <div class="edit-link FR">
                                                                                                    <a href="javascript:void()" data-action="edit-company"><i class="icon-edit edit-icon"></i></a>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="ibox-content">
                                                                                                <!--address icon-->
                                                                                                <div class="info-icon">
                                                                                                    <div class="row-icon address-icon">
                                                                                                        <asp:Label runat="server" ID="lblAddress"></asp:Label>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <!--phone icon-->
                                                                                                <div class="info-icon" runat="server" Visible="<%# !string.IsNullOrEmpty(user.Phone) %>">
                                                                                                    <a href="tel:704-993-3933" class="row-icon phone-icon">
                                                                                                        <asp:Label runat="server" ID="lblPhone" Text="<%# user.Phone %>"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                                <!--fax icon-->
                                                                                                <div class="info-icon" runat="server" Visible="<%# !string.IsNullOrEmpty(user.Fax) %>">
                                                                                                    <div class="row-icon fax-icon">
                                                                                                        <asp:Label runat="server" ID="lblFax" Text="<%# user.Fax %>"></asp:Label>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <!--website icon-->
                                                                                                <div class="info-icon" runat="server" id="divCompanyWebsiteContainer">
                                                                                                    <a runat="server" id="aLinkWebsite" class="row-icon web-icon" target="_blank">
                                                                                                        <asp:Label runat="server" ID="lblWebsite"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--company details -->

                                                                                        <!--primary contact - right column-->
                                                                                        <div class="ibox basic-card" id="divPrimaryContactContainer" runat="server">
                                                                                            <div class="ibox-title">
                                                                                                <h3 class="card-title"><i class="icon-contacts title-icon"></i>Primary Contact</h3>
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
                                                                                                    <a href="tel:" runat="server" id="aPrimaryContactPhone" class="row-icon phone-icon user-tel">
                                                                                                        <asp:Label runat="server" ID="lblPrimaryContactPhone"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                                <!--email icon-->
                                                                                                <div class="info-icon">
                                                                                                    <a href="mailto:" runat="server" id="aPrimaryContactEmail" class=" row-icon email-icon user-mail hover-link">
                                                                                                        <asp:Label runat="server" ID="lblPrimaryContactEmail"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--primary contact -->

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
                                                                                                <!--location (city, country)-->
                                                                                                <div id="wrpSalesRepLocation" class="info-icon" runat="server">
                                                                                                    <div class="row-icon address-icon">
                                                                                                        <asp:Label runat="server" ID="lblSalesRepLocation"></asp:Label>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <!--email icon-->
                                                                                                <div id="wrpSalesRepEmail" class="info-icon" runat="server">
                                                                                                    <a href="#" class="row-icon email-icon user-mail hover-link" id="aSalesRepEmail" runat="server">
                                                                                                        <asp:Label runat="server" ID="lblSalesRepEmail"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                                <!--phone-->
                                                                                                <div id="wrpSalesRepPhone" class="info-icon" runat="server">
                                                                                                    <a href="#" class=" row-icon phone-icon user-tel" id="aSalesRepPhone" runat="server">
                                                                                                        <asp:Label runat="server" ID="lblSalesRepPhone"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                                <!--mobile phone-->
                                                                                                <div id="wrpSalesRepMobilePhone" class="info-icon" runat="server">
                                                                                                    <a href="#" class=" image-icon mobile-icon user-tel" id="aSalesRepMobilePhone" runat="server">
                                                                                                        <asp:Label runat="server" ID="lblSalesRepMobilePhone"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--sales owner -->
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <!--#tab-overview-->

                                                                        <!--#tab-deals-->
                                                                        <div class="tab-pane" id="tab-deals">

                                                                            <div class="deals-acts row no-gutters">
                                                                                <div class="col-6">
                                                                                    <div class="text-left">
                                                                                        <div class="deal-btn-wrp filter-wrap">
                                                                                            <a href="#" data-status="active" data-view="card" data-type-card="#active-deal" data-type-list="#deal-datatable" class="active-deal deals-link active">Active</a>
                                                                                            <a href="#" data-status="inactive" data-view="card" data-type-card="#inactive-deal" data-type-list="#deal-inc-datatable" class="inactive-deal deals-link">Inactive</a>
                                                                                        </div>
                                                                                        <div class="select-box-wrp hide">
                                                                                            <asp:DropDownList ID="ddlSalesStage" runat="server" CssClass="form-control "></asp:DropDownList>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="col-6">
                                                                                    <div class="text-right clearfix">
                                                                                        <div class="btn-wrp add-new-btn">
                                                                                            <a href="javascript:void(0)" data-action="new-deal" class="edit_link btn-hover"><i class="icon-plus"></i>Deal</a>
                                                                                        </div>
                                                                                        <div class="showView">
                                                                                            <a href="#" class="icon active" data-status="active" data-view="card" data-type-active="#active-deal" data-type-inactive="#inactive-deal"><i class="icon-TH"></i></a>
                                                                                            <a href="#" class="icon" data-status="active" data-view="list" data-type-active="#deal-datatable" data-type-inactive="#deal-inc-datatable"><i class="icon-table-view"></i></a>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>

                                                                            <!-- .ibox-content -->
                                                                            <div class="ibox-content">
                                                                                <div class="dealList" id="divDeals">

                                                                                    <!--#grid-view-->
                                                                                    <div id="grid-view" class="deal-blk deal-view MT15">
                                                                                        <!--#active-deal-->
                                                                                        <div id="active-deal" class="deal-cards">
                                                                                            <div class="row"></div>
                                                                                        </div>
                                                                                        <!--#inactive-deal-->
                                                                                        <div id="inactive-deal" class="deal-cards hide">
                                                                                            <div class="row"></div>
                                                                                        </div>
                                                                                    </div>
                                                                                    <!--#grid-view-->

                                                                                    <!--#list-view-->
                                                                                    <div id="list-view" class="list-table ibox-content deal-table MT15 hide">
                                                                                        <table id="deal-datatable" class="deal-toggle-tb  dataTable no-footer">
                                                                                            <thead>
                                                                                                <tr>
                                                                                                    <th data-field-name="dealname" data-sort-order="asc" class="nobg">DEAL NAME<i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i></th>
                                                                                                    <th data-field-name="companyname">COMPANY</th>
                                                                                                    <th>LOCATION</th>
                                                                                                    <th>SALES TEAM</th>
                                                                                                    <th data-field-name="salesstagename" class="text-center">STAGE</th>
                                                                                                    <th data-field-name="lastactivity" class="text-center">LAST ACTIVE</th>
                                                                                                    <th data-field-name="decisiondate" class="text-center ddate">DECISION DATE</th>
                                                                                                </tr>
                                                                                            </thead>
                                                                                            <tbody></tbody>
                                                                                        </table>

                                                                                        <table id="deal-inc-datatable" class="deal-toggle-tb  dataTable no-footer">
                                                                                            <thead>
                                                                                                <tr>
                                                                                                    <th data-field-name="dealname" data-sort-order="asc" class="nobg">DEAL NAME<i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i></th>
                                                                                                    <th data-field-name="companyname">COMPANY</th>
                                                                                                    <th>LOCATION</th>
                                                                                                    <th>SALES TEAM</th>
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
                                                                                    <!--#list-view-->
                                                                                </div>
                                                                                <!--#divDeals-->
                                                                            </div>
                                                                            <!-- .ibox-content -->

                                                                        </div>
                                                                        <!--#tab-deals-->

                                                                        <!-- #tab-quotes -->
                                                                        <%--
                                                                        <div class="tab-pane" id="tab-quotes">
																			<div class="row">
																				<div class="col-md-12">
																					<div class="ibox quotes">
																						<form class="" action="#">
																							<div class="row">
																								<div class="col-xl-12 col-lg-12 col-md-12">
																									<div class="text-right">
																										<div class="select-wrap">
																											<select id="sales-sales-rep" class="custom-select2">
																												<option value="">All Reps</option>
																											</select>
																										</div>
																										<div class="select-wrap">
																											<select id="select-branch" class="custom-select2">
																												<option value="">All Branches</option>
																											</select>
																										</div>
																										<div class="select-wrap">
																											<select id="select-status" class="custom-select2">
																												<option value="">All Statuses</option>
																											</select>
																										</div>
																										<div class="btn-wrp add-new-btn">
																											<a href="#" class="edit_link btn-hover"><i class="icon-plus"></i>Quote</a>
																										</div>
																									</div>
																								</div><!-- .search dropdowns -->
																							</div>
																						</form>
																						
																						<!-- #quotes-datatable -->
																						<div class="list-table quotes-table">
																							<table id="quotes-datatable">
																								<thead>
																									<tr>
																										<th>SALES OWNER</th>
																										<th>BRANCH</th>
																										<th>CODE</th>
																										<th>ROUTE</th>
																										<th>VALID UNTIL</th>
																										<th class="text-center">PIECES</th>
																										<th class="text-center">WEIGHT</th>
																										<th class="text-center">INCOTERM</th>
																										<th class="text-center">STATUS</th>
																									</tr>
																								</thead>
																								<tbody>
																									<asp:Repeater runat="server" ID="rptQuotes">
																										<ItemTemplate>
																											<tr>
																												<td>
																													<asp:Label runat="server" Text='<%# Eval("CustomerName")%>'></asp:Label>
																												</td>
																												<td>
																													<asp:Label runat="server" Text='<%# Eval("BranchName")%>'></asp:Label>
																												</td>
																												<td>
																													<asp:Label runat="server" Text='<%# Eval("QuoteCode")%>'></asp:Label>
																												</td>
																												<td>
																													<asp:Label runat="server" Text='<%# Eval("Destination")%>'></asp:Label>
																												</td>
																												<td>
																													<asp:Label runat="server" Text='<%# Eval("CreatedDate", "{0:dd-MMM-yy}") %>'></asp:Label>
																												</td>
																												<td>
																													<asp:Label runat="server" Text='<%# Eval("ValidTo", "{0:dd-MMM-yy}") %>'></asp:Label>
																												</td>
																												<td class="text-center">
																													<asp:Label runat="server" Text='<%# Eval("TotalPackages")%>'></asp:Label>
																												</td>
																												<td class="text-center">
																													<asp:Label runat="server" Text='<%# Eval("TotalWeight")%>'></asp:Label>
																												</td>
																												<td class="text-center">
																													<asp:Label runat="server" Text='<%# Eval("Incoterm")%>'></asp:Label>
																												</td>
																												<td class="text-center">
																													<a href="#" class="border-status MW110 PR"><asp:Label runat="server" Text='<%# Eval("QuoteStatus")%>'></asp:Label></a>
																												</td>
																											</tr>
																										</ItemTemplate>
																									</asp:Repeater>
																									
																								</tbody>
																							</table>
																						</div><!-- #quotes-datatable -->
																					</div>
																				</div>
																			</div>
																		</div>
                                                                        --%>
                                                                        <!-- #tab-quotes -->

                                                                        <!--#tab-calendar-events-->
                                                                        <div class="tab-pane" id="tab-calendar-events">
                                                                            <div class="row">
                                                                                <div class="col-xl-12 col-lg-12 col-md-12 col-right-box">
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
                                                                        <uc1:DetailTabTasks runat="server" ID="DetailTabTasks" DetailType="company"/>

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
                                                                                            <div class="no-notes empty-box tableDisplay hide">
                                                                                                <div class="tableCell">
                                                                                                    <i class="icon-Notes"></i>
                                                                                                    <p class="e-text">no notes</p>
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
                                                                                            <div class="no-docs empty-box hide tableDisplay no-company-documents">
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

                                                                        <!--#tab-company-contacts-->
                                                                        <div class="tab-pane" id="tab-company-contacts">
                                                                            <div class="row">
                                                                                <div class="col-md-12 contactBox">
                                                                                    <div class="row no-gutters company-contacts">
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <!--#tab-company-contacts-->

                                                                        <!--#tab-sales-team-->
                                                                        <div class="tab-pane" id="tab-sales-team">
                                                                            <div class="row">
                                                                                <div class="col-md-12 contactBox">
                                                                                    <div class="row no-gutters company-users">
                                                                                    </div>
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
                                                                                                    <th>Date<i class="icon-Ascending"></i></th>
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
                                                                                                            <td><i class="icon-edit"></i>
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

                                                                        <!--#tab-related-companies-->
                                                                        <div class="tab-pane" id="tab-related-companies">
                                                                            <header style="margin-bottom: 15px;">
                                                                                <div class="row">
                                                                                    <div class="col-md-12 col-sm-12 pageInfo">
                                                                                        <span class="total-records">
                                                                                            <asp:Label ID="lblRecordCount" runat="server" CssClass="text-muted  m-t-md record-count" Text=""></asp:Label>
                                                                                        </span>

                                                                                        <div class="text-right btns-holder">
                                                                                            <span id="pnlRelatedCompanyButtons">
                                                                                                <asp:LinkButton ID="btnRemoveSelectedRelatedCompanies" OnClick="btnRemoveRelatedCompanies_Click" CssClass="btn-wrp primary-btn" Text="Remove Selected" runat="server"></asp:LinkButton>
                                                                                                <asp:LinkButton ID="btnRequestAccess" CssClass="btn-wrp primary-btn" Text="Request Access" OnClick="btnRequestAccess_Click" runat="server"></asp:LinkButton>
                                                                                                <span class="btn-wrp add-new-btn add-comp-wrap desk-btn">
                                                                                                    <a href="#" data-action="open-change-relation-type" data-toggle="modal" data-target="#changeRelationType" class="edit_link btn-hover change-relation-type">Change Relation Type</a>
                                                                                                </span>
                                                                                                <span class="btn-wrp add-new-btn add-comp-wrap desk-btn">
                                                                                                    <a href="#" data-action="open-related-cpmpany-setup-" data-toggle="modal" data-target="#relatedCompanySetup" class="edit_link btn-hover new-related-company"><i class="icon-plus"></i>Company</a>
                                                                                                </span>
                                                                                            </span>
                                                                                            <div class="search-form hide">
                                                                                                <div class="search-box">
                                                                                                    <asp:Panel ID="pnl1" runat="server" DefaultButton="btnSearch">
                                                                                                        <asp:TextBox runat="server" Style="z-index: 0;" CssClass="form-control" ID="txtKeyword" placeholder="Search"></asp:TextBox>
                                                                                                        <asp:LinkButton ID="btnSearch" OnClick="btnSearch_Click" runat="server"><i class="icon-search"></i></asp:LinkButton>
                                                                                                    </asp:Panel>
                                                                                                </div>
                                                                                            </div>

                                                                                            
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </header>
                                                                            <div class="row">
                                                                                <div class="col-md-12">
                                                                                    <div class="list-table" id="tblRelatedCompanies" runat="server">
                                                                                        <table class="related-companies-table">
                                                                                            <thead>
                                                                                                <tr>
                                                                                                    <th class=""><span>
                                                                                                        <asp:CheckBox ForeColor="White" CssClass="th_checkbox" runat="server" Text="."></asp:CheckBox></span>
                                                                                                    </th>
                                                                                                    <th>Related Company Name<i class="icon-Ascending"></i></th>
                                                                                                    <th>City</th>
                                                                                                    <th>Country</th>
                                                                                                    <th>Owner</th>
                                                                                                    <th>Relation Type</th>
                                                                                                    <th></th>
                                                                                                    <th></th>
                                                                                                </tr>
                                                                                            </thead>
                                                                                            <tbody>
                                                                                                <asp:Repeater runat="server" ID="rptRelatedCompanies" OnItemDataBound="rptRelatedCompanies_ItemDataBound">
                                                                                                    <ItemTemplate>
                                                                                                        <tr>
                                                                                                            <td class="">
                                                                                                                <span>
                                                                                                                    <asp:CheckBox ForeColor="White" runat="server" Text="." CssClass="chk_related_companies"></asp:CheckBox>
                                                                                                                </span>
                                                                                                                <asp:Label Visible="false" runat="server" Text='<%# Eval("LinkCompanyToCompanyId") %>'></asp:Label>
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <asp:Label runat="server" Text='<%# Eval("CompanyName") %>'></asp:Label>
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <asp:Label runat="server" Text='<%# Eval("City") %>'></asp:Label>
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <asp:Label runat="server" Text='<%# Eval("Country") %>'></asp:Label>
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <asp:Label runat="server" Text='<%# Eval("Owner") %>'></asp:Label>
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <asp:Label runat="server" Text='<%# Eval("RelationType") %>'></asp:Label>
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <asp:LinkButton runat="server" OnClick="linkRequestAccess_Click" Text="Request Access"></asp:LinkButton>
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <asp:LinkButton runat="server" OnClick="linkView_Click" Text="View"></asp:LinkButton>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </ItemTemplate>
                                                                                                </asp:Repeater>
                                                                                            </tbody>
                                                                                        </table>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <asp:Panel runat="server" ID="pnlNoRelatedCompanies">
                                                                                <div class="no-related-companies empty-box empty_event tableDisplay">
                                                                                    <div class="tableCell">
                                                                                        <i class="icon-task"></i>
                                                                                        <p class="e-text">no related companies</p>
                                                                                    </div>
                                                                                </div>
                                                                            </asp:Panel>

                                                                        </div>
                                                                    </div>
                                                                    <!--#tab-related-companies-->

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
                                    <!-- .wrapper -->
                                </form>
                                <!-- form end -->
                            </div>
                            <!-- #content end -->

                            <%-- CALENDAR EVENT ADD/EDIT --%>
                            <uc1:CalendarEventAddEdit runat="server" ID="CalendarEventAddEdit" />

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

            <%--add sales team member dialog--%>
            <uc1:AddSalesTeamMember runat="server" ID="AddSalesTeamMember" DetailType="company"/>

            <div class="modal inmodal" id="relatedCompanySetup" tabindex="-1" role="dialog" style="display: none;" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content animated fadeIn">
                        <div class="modal-header">
                            <h4 class="modal-title">Add Related Company</h4>
                            <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">x</span></button>
                        </div>
                        <div class="modal-body white-bg" style="min-height: 300px;">
                            <div class="form-group">
                                <label class="inputLabel">Select Company</label>
                                <asp:DropDownList ID="ddlRelatedCompany" CssClass="custom-select2" runat="server"></asp:DropDownList>
                            </div>
                            <div class="form-group filled">
                                <label class="inputLabel">Link Type</label>
                                <asp:DropDownList runat="server" ID="ddlLinkType" CssClass="form-control" Width="200"></asp:DropDownList>
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
                        <div class="modal-footer">
                            <a class="primary-btn" id="btnAddRelatedCompany">Save</a>
                            <button type="button" class="secondary-btn" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal inmodal" id="changeRelationType" tabindex="-1" role="dialog" style="display: none;" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content animated fadeIn">
                        <div class="modal-header">
                            <h4 class="modal-title">Change Relation Type</h4>
                            <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">x</span></button>
                        </div>
                        <div class="modal-body white-bg" style="min-height: 300px;">

                            <div class="form-group">
                                <div class="form-group filled">
                                    <label class="inputLabel">Link Type</label>
                                    <asp:DropDownList runat="server" ID="ddlChangeRelationType" CssClass="form-control" Width="200"></asp:DropDownList>
                                </div>
                            </div>

                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="primary-btn" ID="btnChangeRelationType" OnClick="btnChangeRelationType_Click" runat="server" Text="Save"></asp:LinkButton>
                            <button type="button" class="secondary-btn" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>

        </div>
        <!-- #main end -->

    </form>

    <!-- js libraries -->
    <script src="/_content/_js/bundle/dropzone.js"></script>
    <script src="/_content/_js/bundle/moment.js"></script>
    <script src="/_content/_js/fullcalendar.min.js"></script>
    <script src="/_content/_js/jquery.ajaxfileupload.js"></script>
    
    <!-- page -->
    <script src="companydetail-07-apr-2020.js"></script>

</body>
</html>
