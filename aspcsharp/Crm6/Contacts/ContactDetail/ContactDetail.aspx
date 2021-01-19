<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContactDetail.aspx.cs" Inherits="Crm6.Contacts.ContactDetail.ContactDetail" %>

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

    <title>Contact</title>

    <!--css custom-->
    <link href="contactdetail-08-apr-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="detail-pg">
   
    <form runat="server">

        <!--#main start-->
        <div id="main">

            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label Style="display: none" ID="lblUserIdGlobal" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblUsername" runat="server" Text=""></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblContactSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCompanyId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblContactId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblGlobalCompanyId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCompanySubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSalesStages" runat="server" Text="0"></asp:Label>

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
                                    <header class="top-header globalHead">

                                        <!--breadcrumb-->
                                        <div class="clearfix">

                                            <p class="bread_crumb">
                                                <a href="/Contacts/ContactList/ContactList.aspx">Contacts</a>
                                                <span class="bread_sep">&rsaquo;</span>
                                                <span>Contact Detail</span>
                                            </p>

                                            <!-- .desktop-header -->
                                            <div class="desktop-header-dropdown">
                                                <div class="dropdown-wrapper header-dropdown">
                                                    <a class="edit-button" data-action="edit-contact">Edit</a>
                                                </div>
                                            </div>
                                            <!-- .desktop-header -->
                                        </div>
                                        <!--.bread_crumb-->

                                        <div class="clearfix head_info">
                                            <!--contact image-->
                                            <span class="user-thumb FL" runat="server" id="divContactImg">
                                                <img id="imgContact" runat="server" src="/_content/_img/no-pic.png" />
                                            </span>
                                            <!--contact and company name-->
                                            <div class="FL">
                                                <h1 class="page-title">
                                                    <asp:Label ID="lblContactName" runat="server"></asp:Label>
                                                    <asp:Label ID="lblCompanyNameTitle" CssClass="page-title-sub" runat="server"></asp:Label>
                                                </h1>
                                            </div>
                                        </div>

                                        <%--mobile quick add menu--%>
                                        <!--
                                        <div class="mobile-header-dropdown">
                                            <div class="dropdown-wrapper header-dropdown">
                                                <div class="ae-dropdown dropdown">
                                                    <div class="ae-select bg-dd-item btn-hover">
                                                        <i class="plus-icon icon-plus"></i><a href="#" class="ae-select-content"></a>
                                                        <span class="drop-icon-down"><i class="icon-angle-down"></i></span>
                                                        <span class="drop-icon-up"><i class="icon-angle-up"></i></span>
                                                    </div>
                                                    <ul class="dropdown-nav bg-selectdd ae-hide">
                                                        <li class="new_deal"><a><i class="plus-icon icon-plus"></i>Deal</a></li>
                                                        <li class='selected li_new_event'><a><i class="plus-icon icon-plus"></i>Event</a></li>
                                                        <li class="new_task"><a><i class="plus-icon icon-plus"></i>Task</a></li>
                                                        <li class="new_note"><a><i class="plus-icon icon-plus"></i>Note</a></li>
                                                        <li class="new_doc"><a><i class="plus-icon icon-plus"></i>Document</a></li>
                                                        <li class="new_contact"><a><i class="plus-icon icon-plus"></i>Contact</a></li>
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                        .mobile-header-dropdown -->

                                        <!--nav menu for tabs-->
                                        <div class="panel-heading">
                                            <div class="panel-options">
                                                <%--desktop tabs nav--%>
                                                <div class="desktop-panel-nav">
                                                    <ul class="nav nav-tabs " id="contact-tabs">
                                                        <li><a href="#tab-overview" data-toggle="tab" data-set="new_event" class="active"><span class="language-entry">Overview</span></a></li>
                                                        <li data-type="deals" class=""><a href="#tab-deals" data-toggle="tab" data-set="new_deal"><span class="language-entry">Deals</span></a></li>
                                                        <li data-type="events" class=""><a href="#tab-calendar-events" data-toggle="tab" data-set="new_event"><span class="language-entry">Events</span></a></li>
                                                        <li data-type="tasks" class=""><a href="#tab-tasks" data-toggle="tab" data-set="new_task"><span class="language-entry">Tasks</span></a></li>
                                                        <li data-type="notes" id="liNotes"><a id="aNotes" href="#tab-notes" data-toggle="tab" data-set="new_note"><span class="language-entry">Notes</span></a></li>
                                                        <li data-type="documents" class=""><a href="#tab-documents" data-toggle="tab" data-set="new_doc"><span class="language-entry">Documents</span></a></li>
                                                        <%--<li data-type="contacts" class=""><a href="#tab-contact-contacts" data-toggle="tab" data-set="new_contact"><span class="language-entry">Contacts</span></a></li>--%>
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
                                                            <ul id="contact-tabs" class="dropdown-nav ae-hide nav nav-tabs">
                                                                <li class='selected'><a href="#tab-overview" data-toggle="tab" data-set="new_deal" class="active"><span class="language-entry">Overview</span></a></li>
                                                                <li data-type="deals" class=""><a href="#tab-deals" data-toggle="tab" data-set="new_deal"><span class="language-entry">Deals</span></a></li>
                                                                <%--<li data-type="quotes" class=""><a href="#tab-quotes" data-toggle="tab" data-set="new_quote"><span class="language-entry">Quotes</span></a></li>--%>
                                                                <li data-type="events" class=""><a href="#tab-calendar-events" data-toggle="tab" data-set="new_event"><span class="language-entry">Events</span></a></li>
                                                                <li data-type="tasks" class=""><a href="#tab-tasks" data-toggle="tab" data-set="new_task"><span class="language-entry">Tasks</span></a></li>
                                                                <li data-type="notes" id="liNotes"><a id="aNotes" href="#tab-notes" data-toggle="tab" data-set="new_note"><span class="language-entry">Notes</span></a></li>
                                                                <li data-type="documents" class=""><a href="#tab-documents" data-toggle="tab" data-set="new_doc"><span class="language-entry">Documents</span></a></li>
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
                                                                                
                                                                                <!--left column -->
                                                                                <div class="col-xl-3 col-lg-4 col-md-4 col-sm-12 col-12 col-left-box">
                                                                                    
                                                                                    <!--contact info - left column-->
                                                                                    <div class="ibox basic-card">
                                                                                        <div class="ibox-title">
                                                                                            <h3 class="card-title language-entry"><i class="icon-Notes title-icon"></i>Info</h3>
                                                                                        </div>
                                                                                        <div class="ibox-content">
                                                                                                <!--address-->
                                                                                            <div id="wrpContactAddress" class="info-icon" runat="server">
                                                                                                <div class="row-icon address-icon">
                                                                                                    <asp:Label ID="lblContactAddress" Text="" CssClass="profile-data" runat="server"></asp:Label></span> 
                                                                                                </div>
                                                                                            </div>
                                                                                            <!--email-->
                                                                                            <div id="wrpContactEmail" class="info-icon" runat="server">
                                                                                                <a href="mailto:" class=" row-icon email-icon user-mail hover-link" runat="server" >
                                                                                                    <asp:Label ID="lblContactEmail" CssClass="profile-data" Text="" runat="server"></asp:Label>
                                                                                                </a>
                                                                                            </div>
                                                                                            <!--phone (business)-->
                                                                                            <div id="wrpBusinessPhone" class="info-icon" runat="server">
                                                                                                <a runat="server" href="tel:704-993-3933" class="row-icon phone-icon">
                                                                                                    <asp:Label ID="lblBusinessPhone" CssClass="profile-data" Text="" runat="server"></asp:Label>
                                                                                                </a>
                                                                                            </div>
                                                                                            <!--phone (mobile)-->
                                                                                            <div id="wrpMobilePhone" class="info-icon" runat="server">
                                                                                                <a runat="server" href="tel:704-993-3933" class="image-icon mobile-icon">
                                                                                                    <asp:Label ID="lblMobilePhone" CssClass="profile-data" Text="" runat="server"></asp:Label>
                                                                                                </a>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <%-- Task next and last activity cards --%>
                                                                                    <uc1:TaskNextLastActivity runat="server" ID="TaskNextLastActivity"/>

                                                                                    <!--details - left column-->
                                                                                    <div class="ibox widget basic-card detail-card">
                                                                                        <div class="ibox-title">
                                                                                            <h3 class="card-title language-entry"><i class="icon-Notes title-icon"></i>Details</h3>
                                                                                        </div>
                                                                                        <div class="ibox-content">
                                                                                            <div id="wrpContactType" class="inner-wrp" runat="server">
                                                                                                <div class="card-status card-label">TYPE</div>
                                                                                                <div class="card-date">
                                                                                                    <asp:Label runat="server" ID="lblContactType"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div id="wrpContactJobTitle" class="inner-wrp" runat="server">
                                                                                                <div class="card-status card-label">JOB TITLE</div>
                                                                                                <div class="card-date">
                                                                                                    <asp:Label ID="lblContactJobTitle" CssClass="profile-data" Text="" runat="server"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div id="wrpInterests" class="inner-wrp" runat="server">
                                                                                                <div class="card-status card-label">INTERESTS</div>
                                                                                                <div class="card-date">
                                                                                                    <asp:Label ID="lblInterests" CssClass="profile-data" Text="" runat="server"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div id="wrpContactBirthday" class="inner-wrp" runat="server">
                                                                                                <div class="card-status card-label">BIRTHDAY</div>
                                                                                                <div class="card-date">
                                                                                                    <asp:Label ID="lblContactBirthday" CssClass="profile-data" Text="" runat="server"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div id="wrpContactPrevEmployers" class="inner-wrp" runat="server">
                                                                                                <div class="card-status card-label">PREVIOUS EMPLOYERS</div>
                                                                                                <div class="card-date">
                                                                                                    <asp:Label ID="lblContactPrevEmployers" CssClass="profile-data" Text="" runat="server"></asp:Label>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="overview-checks inner-wrp">
                                                                                                <div class="row no-gutters">
                                                                                                    <div class="col-6 left-col">
                                                                                                        <div class="row no-gutters inner-wrp">
                                                                                                            <div class="col-auto">
                                                                                                                <div id="chkOkToEmail" runat="server"></div>
                                                                                                            </div>
                                                                                                            <div class="col label">EMAIL</div>
                                                                                                        </div>
                                                                                                        <div class="row no-gutters inner-wrp">
                                                                                                            <div class="col-auto">
                                                                                                                <div id="chkMarried" runat="server"></div>
                                                                                                            </div>                                                                                                
                                                                                                            <div class="col label">MARRIED</div>
                                                                                                        </div>
                                                                                                        <div class="row no-gutters inner-wrp">
                                                                                                            <div class="col-auto card-date">
                                                                                                                <div id="chkHolidayCard" runat="server"></div>
                                                                                                            </div>
                                                                                                            <div class="col label">HOLIDAY CARD</div>
                                                                                                        </div>
                                                                                                    </div>
                                                                                                    <div class="col-6 right-col">
                                                                                                        <div class="row no-gutters inner-wrp">
                                                                                                            <div class="col-auto">
                                                                                                                <div id="chkOkToCall" runat="server"></div>
                                                                                                            </div>
                                                                                                            <div class="col label">CALL</div>
                                                                                                        </div>
                                                                                                        <div class="row no-gutters inner-wrp">
                                                                                                            <div class="col-auto">
                                                                                                                <div id="chkHasChildren" runat="server"></div>
                                                                                                            </div>
                                                                                                            <div class="col label">CHILDREN</div>
                                                                                                        </div>
                                                                                                        <div class="row no-gutters inner-wrp">
                                                                                                            <div class="col-auto card-date">
                                                                                                                <div id="chkFormerEmployee" runat="server"></div>
                                                                                                            </div>
                                                                                                            <div class="col label">FORMER EMPLOYEE</div>
                                                                                                        </div>
                                                                                                    </div>
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
                                                                                
                                                                                <!--center column -->
                                                                                <div class="col-xl-6 col-lg-8 col-md-8 col-sm-12 col-12 col-mid-box">

                                                                                    <!--deals - center column-->
                                                                                    <div class="ibox middle-wrp middle-card">

                                                                                        <!--deals header-->
                                                                                        <div class="outer-wrp">
                                                                                            <div class="ibox-title">
                                                                                                <h3 class="card-title language-entry"><i class="icon-deals title-icon"></i>Deals</h3>
                                                                                            </div>
                                                                                        </div>

                                                                                        <div class="ibox-content">
                                                                                            <!-- Fetch Deals -->
                                                                                            <div class="deals"></div>
                                                                                            <div class="overview-nodeals empty-box hide">
                                                                                                <i class="icon-Deal----Gray"></i>
                                                                                                <p class="e-text">no deals</p>
                                                                                                <div class="btn-wrp">
                                                                                                    <a href="#" data-action="new-deal" class="primary-btn">New Deal</a>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>

                                                                                    </div>

                                                                                    <!--add note-->
                                                                                    <div class="ibox overview-note note-textarea">
                                                                                        <asp:TextBox ID="txtNote" runat="server" TextMode="MultiLine" Rows="3" placeholder="note"></asp:TextBox>
                                                                                    <div class="add-note-btn" id="note-add">
                                                                                            <a href="javascript:void(0)" class="primary-btn btnAddNote language-entry">Add Note</a>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                
                                                                                <!--right column -->
                                                                                <div class="col-xl-3 col-lg-4 col-md-4 col-sm-12 col-12 col-right-box">
                                                                                    <div class="right-col">
                                                                                        <!--company info - right column-->
                                                                                        <div class="ibox basic-card">
                                                                                            <div class="ibox-title">
                                                                                                <h3 class="card-title language-entry"><i class="icon-business title-icon"></i>Company Info</h3>
                                                                                                <div class="edit-link FR">
                                                                                                    <a href="#" data-action="edit-company"><i class="icon-edit edit-icon"></i></a>
                                                                                                </div>
                                                                                            </div>
                                                                                            <div class="ibox-content">
                                                                                                <!--logo/name-->
                                                                                                <div class="comp-info row no-gutters">
                                                                                                    <div class="comp-logo col-auto align-self-center" runat="server" id="divCompanyLogo">
                                                                                                        <img id="imgCompany" runat="server" src="_img/cd-logo.png" />
                                                                                                    </div>
                                                                                                    <div class="col">
                                                                                                        <a class="comp-txt" href="javascript:void(0);" data-action='company-detail' runat="server"><span>
                                                                                                            <asp:Label ID="lblCompanyName" runat="server"></asp:Label></span>
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
                                                                                                    <a runat="server" href="tel:704-993-3933" class="row-icon phone-icon">
                                                                                                        <asp:Label runat="server" ID="lblPhone"></asp:Label></a>
                                                                                                </div>
                                                                                                <!--fax-->
                                                                                                <div id="wrpFax" class="info-icon" runat="server">
                                                                                                    <div class="row-icon fax-icon">
                                                                                                        <asp:Label runat="server" ID="lblFax"></asp:Label>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <!--website-->
                                                                                                <div id="wrpLinkWebsite" class="info-icon" runat="server">
                                                                                                    <a runat="server" id="aLinkWebsite" class="row-icon web-icon" target="_blank">
                                                                                                        <asp:Label runat="server" ID="lblWebsite"></asp:Label></a>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                        <!--contact owner - right column-->
                                                                                        <div id="wrapContactOwnerCard" class="ibox basic-card" runat="server">
                                                                                            <div class="ibox-title">
                                                                                                <h3 class="card-title"><i class="icon-users title-icon"></i>Contact Owner</h3>
                                                                                            </div>
                                                                                            <div class="ibox-content">
                                                                                                <div class="user-wrap clearfix">
                                                                                                    <div class="user-img FL" id="divContactOwnerImage" runat="server">
                                                                                                        <img src="/_content/_img/no-pic.png" runat="server" id="imgProfile">
                                                                                                    </div>
                                                                                                    <div class="user-info FL">
                                                                                                        <asp:Label runat="server" ID="lblContactOwnerId" CssClass="hide" Text="0"></asp:Label>
                                                                                                        <div class="user-name">
                                                                                                            <asp:Label runat="server" ID="lblContactOwnerName"></asp:Label>
                                                                                                        </div>
                                                                                                        <div class="user-prof">
                                                                                                            <asp:Label runat="server" ID="lblContactOwnerPosition"></asp:Label>
                                                                                                        </div>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <!--location (city, country)-->
                                                                                                <div id="wrpContactOwnerLocation" class="info-icon" runat="server">
                                                                                                    <div class="row-icon address-icon">
                                                                                                        <asp:Label runat="server" ID="lblContactOwnerLocation"></asp:Label>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <!--email-->
                                                                                                <div id="wrpContactOwnerEmail" class="info-icon" runat="server">
                                                                                                    <a href="mailto:" class="row-icon email-icon user-mail hover-link" id="aSalesRepEmail" runat="server">
                                                                                                        <asp:Label runat="server" ID="lblContactOwnerEmail"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                                <!--phone-->
                                                                                                <div id="wrpContactOwnerPhone" class="info-icon" runat="server">
                                                                                                    <a href="tel:" class=" row-icon phone-icon user-tel" runat="server" id="aSalesRepPhone">
                                                                                                        <asp:Label runat="server" ID="lblContactOwnerPhone"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                                <!--mobile phone-->
                                                                                                <div id="wrpContactOwnerMobilePhone" class="info-icon" runat="server">
                                                                                                    <a href="tel:" class=" image-icon mobile-icon user-tel" runat="server" id="aSalesRepMobilePhone">
                                                                                                        <asp:Label runat="server" ID="lblContactOwnerMobilePhone"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>

                                                                            </div>
                                                                        </div>

                                                                        <!--#tab-deals-->
                                                                        <div class="tab-pane" id="tab-deals">

                                                                            <div class="deals-acts row no-gutters">
                                                                                <div class="col-6">
                                                                                    <div class="text-left">
                                                                                        <div class="deal-btn-wrp filter-wrap">
                                                                                            <a href="#" data-status="active" data-view="card" data-type-card="#active-deal" data-type-list="#deal-datatable" class="active-deal deals-link active">Active</a>
                                                                                            <a href="#" data-status="inactive" data-view="card" data-type-card="#inactive-deal" data-type-list="#deal-inc-datatable" class="inactive-deal deals-link">Inactive</a>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="col-6">
                                                                                    <div class="text-right clearfix">
                                                                                        <div class="btn-wrp add-new-btn">
                                                                                            <a href="#" data-action="new-deal" class="edit_link btn-hover"><i class="icon-plus"></i>Deal</a>
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
                                                                                                    <th class="nobg">DEAL NAME<i class="icon-Ascending"></i></th>
                                                                                                    <th>COMPANY</th>
                                                                                                    <th>LOCATION</th>
                                                                                                    <th>SALES TEAM</th>
                                                                                                    <th class="text-center">STAGE</th>
                                                                                                    <th class="text-center">LAST ACTIVE</th>
                                                                                                    <th class="text-center">DECISION DATE</th>
                                                                                                </tr>
                                                                                            </thead>
                                                                                            <tbody></tbody>
                                                                                        </table>

                                                                                        <table id="deal-inc-datatable" class="deal-toggle-tb  dataTable no-footer">
                                                                                            <thead>
                                                                                                <tr>
                                                                                                    <th class="nobg">DEAL NAME<i class="icon-Ascending"></i></th>
                                                                                                    <th>COMPANY</th>
                                                                                                    <th>LOCATION</th>
                                                                                                    <th>SALES TEAM</th>
                                                                                                    <th class="text-center">STAGE</th>
                                                                                                    <th class="text-center">LAST ACTIVE</th>
                                                                                                    <th class="text-center">DECISION DATE</th>
                                                                                                </tr>
                                                                                            </thead>
                                                                                            <tbody></tbody>
                                                                                        </table>

                                                                                        <%-- paging --%>
                                                                                        <div style="text-align: center;">
                                                                                            <ul class="pagination hide"></ul>
                                                                                        </div>

                                                                                        <%-- no deals --%>
                                                                                        <div id="noDeals" class="hide">
                                                                                            <div class="alert text-center">
                                                                                                <p class="language-entry">No Deals</p>
                                                                                            </div>
                                                                                        </div> 

                                                                                    </div>
                                                                                    <!--#list-view-->
                                                                                </div>
                                                                                <!--#divDeals-->
                                                                            </div>
                                                                            <!-- .ibox-content -->

                                                                        </div>
                                                                        <!--#tab-deals-->

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
                                                                        <uc1:DetailTabTasks runat="server" ID="DetailTabTasks" DetailType="contact"/>
                                                                        
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
                                                                                            <div class="no-docs empty-box hide no-contact-documents tableDisplay">
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

                                                                        <!--#tab-sales-team-->
                                                                        <div class="tab-pane" id="tab-sales-team">
                                                                            <div class="row">
                                                                                <div class="col-md-12 contactBox">
                                                                                    <div class="row no-gutters contact-users">
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
                                                                                                            <td>
                                                                                                                <asp:Label runat="server" Text='<%# Eval("UserActivityTimestamp", "{0:dd-MMM-yy - HH:mm}") %>'></asp:Label>
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <i class="icon-edit"></i><asp:Label runat="server" Text='<%# Eval("TaskName") %>'></asp:Label>
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
                                                                                        <div class="no-events empty-box empty_activity hide">
                                                                                            <i class="icon-Activity"></i>
                                                                                            <p class="e-text">no activity</p>
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
                                </form>
                                <!-- form end -->
                            </div>
                            <!-- #content end -->
                        </div>
                        <!-- .page-content -->

                        <%-- CALENDAR EVENT ADD/EDIT --%>
                        <uc1:CalendarEventAddEdit runat="server" ID="CalendarEventAddEdit" />

                        <%-- Task add/edit (modal popup) --%>
                        <uc1:TaskAddEdit runat="server" ID="TaskAddEdit" />

                        <%--add sales team member dialog--%>
                        <uc1:AddSalesTeamMember runat="server" ID="AddSalesTeamMember" DetailType="company"/>

                    </div>
                    <!-- .page-container -->
                </div>
                <!-- .row -->
            </div>
            <!-- .container-fluid -->
        </div>
        <!-- #main end -->
    </form>

    <!-- js libraries -->
    <script src="/_content/_js/jquery.ajaxfileupload.js"></script>
    <script src="/_content/_js/bundle/moment.js"></script>
    <script src="/_content/_js/fullcalendar.min.js"></script>
    
    <!-- page -->
    <script src="contactdetail-07-apr-2020.js"></script>

</body>
</html>
