<%@ control language="C#" autoeventwireup="true" codebehind="nav.ascx.cs" inherits="Crm6._usercontrols.nav" %>

<%--MENU SELECTED: #47C3F1    FF Logo light blue--%>
<%--MENU HOVER:    #3BA6F9    FF Logo medium blue --%>
<%--MENU HOVER:    #0F77BC    FF Logo dark blue --%>
</head>
<nav class="navbar-default navbar-static-side" role="navigation" style="z-index: 100">
    <div id="sidebar">

        <a id="logo" href="javascript:void(0)">
            <img src="/_content/_img/logo.svg" runat="server" id="imgCompanyLogo" alt="First Freight Logo" />
        </a>

        <!-- .top-info -->
        <a href="/Admin/Users/UserProfile/UserProfile.aspx">
            <div class="top-info">
                <div class="user-info">
                    <img runat="server" id="imgUserProfilePic" src="../_content/_img/no-pic.png" />
                    <div class="user-nm">
                        <asp:Label runat="server" ID="lblUserName" Text=""></asp:Label>
                    </div>
                </div>
            </div>
        </a>
        <ul class="left-navigation top-nav">
            <li id="liActivity" runat="server">
                <a href="/Activities/Activities.aspx">
                    <i class="icon-Activity"></i><span>Activity</span>
                </a>
            </li>
            <li id="liDashboard" runat="server">
                <a href="/Dashboards/Dashboard.aspx" runat="server" id="aDashboard">
                    <i class="icon-dashboard"></i><span>Dashboard</span>
                </a>
            </li>
           <!-- <li id="liQuotes" runat="server">
                <a href="/Quotes/Quotes.aspx">
                    <i class="icon-quotes"></i><span>Quotes</span>
                </a>
            </li> -->
            <li id="liDeals" runat="server">
                <a href="/Deals/DealList/DealList.aspx">
                    <i class="icon-deals"></i><span>Deals</span>
                </a>
            </li>
            <li class="active" id="liCompanies" runat="server">
                <a href="/Companies/CompanyList/CompanyList.aspx">
                    <i class="icon-business"></i><span>Companies</span>
                </a>
            </li>
            <li id="liContacts" runat="server">
                <a href="/Contacts/ContactList/ContactList.aspx"><i class="icon-contacts"></i><span>Contacts</span></a>
            </li>
            <li id="liCalendar" runat="server">
                <a href="/Calendar/Calendar.aspx">
                    <i class="icon-calendar"></i><span>Calendar</span>
                </a>
            </li>
            <li id="liReports" runat="server">
                <a href="/Reporting/ReportList.aspx">
                    <i class="icon-reports"></i><span>Reports</span>
                </a>
            </li>
            <li id="liSupport" runat="server">
                <a href="http://support.firstfreight.com">
                    <i class="icon-Help"></i><span>Help</span>
                </a>
            </li>

            <li id="liAdmin" runat="server" class="menu-sep">Admin</li>

            <li id="liUsers" runat="server">
                <a href="/Admin/Users/UserList/UserList.aspx" class="nav-admin">
                    <i class="icon-users"></i><span>Users</span>
                </a>
            </li>
            <li id="liLocations" runat="server">
                <a href="/Admin/Locations/Locations.aspx" class="nav-admin">
                    <i class="icon-map-small"></i><span>Locations</span>
                </a>
            </li>
            <li id="liLanguages" runat="server">
                <a href="/Admin/Languages/Languages.aspx" class="nav-admin">
                    <i class="icon-map-small"></i><span>Languages</span>
                </a>
            </li>
            <li id="liSettings" runat="server">
                <a href="/Admin/Settings/Settings.aspx" class="nav-admin">
                    <i class="icon-settings"></i><span>Settings</span>
                </a>
            </li>
             <li id="liCampaigns" runat="server">
                <a href="/Admin/Campaigns/CampaignList/Campaigns.aspx" class="nav-admin">
                    <i class="icon-task"></i><span>Campaigns</span>
                </a>
            </li>
        </ul>
    </div>
</nav>
