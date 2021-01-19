<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContactList.aspx.cs" Inherits="Crm6.Contacts.ContactList" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Contacts</title>

    <!--css custom-->
    <link href="contactlist-07-apr-2020.css" rel="stylesheet" />
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />

</head>

<body class="contact-list">

    <form runat="server">

        <%-- hidden values --%>
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

        <!-- #main start -->
        <div id="main">

            <!-- navbar mobile start-->
            <uc1:navmobile runat="server" />
            <!-- navbar mobile end-->

            <div class="main-wrapper container-fluid">
                <div class="row">

                    <!-- .page-container -->
                    <div class="page-container">

                        <!-- .page-content -->
                        <div class="page-content">

                            <!-- navbar start-->
                            <uc1:nav runat="server" ID="navSidebar" />
                            <!-- navbar end-->

                            <!-- #content -->
                            <div id="content" class="animated fadeIn">

                                <!-- .top-header -->
                                <header class="top-header globalHead">

                                    <div class="row">
                                        <div class="col-md-5 col-sm-5 pageInfo">
                                            <h1 class="page-title language-entry  pull-left">Contacts
                                                <span class="total-records">
                                                    <asp:Label ID="lblRecordCount" runat="server" CssClass="FontSize11 text-muted m-t-md record-count" Text=""></asp:Label>
                                                </span>
                                            </h1>
                                        </div>

                                        <div class="col-md-7 col-sm-7" style="position:static;">
                                            <div class="text-right">
                                                <div class="search-box">
                                                    <asp:TextBox runat="server" CssClass="form-control " ID="txtKeyword" placeholder="Search"></asp:TextBox>
                                                    <a id="btnSearch" href="javascript:void(0)"><i class="icon-search"></i></a>
                                                </div>
                                                <div class="btn-wrp add-new-btn">
                                                    <a href="#" class="edit_link btn-hover new-contact"><i class="icon-plus"></i>Contact</a>
                                                    <a class="btn-hover" title="Export as Excel" id="btnExcel"><i class="fas fa-download"></i></a>
                                                </div>
                                                <div class="showView btn-group btn-view-type hide">
                                                    <%--card view button--%>
                                                    <button class="icon" data-view-type="card" type="button" data-placement="bottom" title="Card View"><i class="icon-TH"></i></button>
                                                    <%--table view button--%>
                                                    <button class="icon active" data-view-type="table" type="button" data-placement="bottom" title="Table View"><i class="icon-table-view"></i></button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="bottom-row row hide">
                                        <div class="col-md-12">
                                            <div class="search-form">
                                                <div class="select-box-wrp hide">
                                                    <%-- sales rep dropdown --%>
                                                    <asp:DropDownList runat="server" ID="ddlSalesReps" CssClass="form-control"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </header>
                                <!-- .top-header -->

                                <!-- Company Page Content -->
                                <div class="wrapper">
                                    <div class="row">
                                        <div class="col-md-12">

                                            <!-- #company-list-view -->
                                            <div class="list-table contact-table">

                                                <div id="divContacts">
                                                    <%-- table view --%>
                                                    <div id="divTableView">
                                                        <table class="table list-table no-footer contacts-table" id="tblContacts">
                                                            <thead>
                                                                <tr>
                                                                    <th class="language-entry" data-field-name="contactname"
                                                                        data-sort-order="asc">Name
                                                                        <i class="sort icon-Ascending"><span class="path1"></span><span class="path2"></span></i>
                                                                    </th>
                                                                    <th class="language-entry" data-field-name="phone">Phone</th>
                                                                    <th class="language-entry" data-field-name="email">Email</th>
                                                                    <th class="language-entry" data-field-name="companyname">Company</th>
                                                                    <th class="language-entry" data-field-name="contactlocation">Location</th>
                                                                    <th class="language-entry">Sales Team</th>
                                                                    <th class="language-entry" data-field-name="lastactivity">Last Activity</th>
                                                                    <th></th>
                                                                </tr>
                                                            </thead>
                                                            <tbody></tbody>
                                                        </table>
                                                    </div>

                                                    <%-- contact cards --%>
                                                    <div class="contact-cards"></div>
                                                    <div class="clearfix"></div>
                                                </div>
                                                <%-- no contacts --%>
                                                <div id="divNoItems" class="hide">
                                                    <div class="alert text-center">
                                                        <p class="language-entry">No Contacts Found</p>
                                                        <div class="btn-wrp add-new-btn">
                                                            <a href="javascript:void(0)" class="edit_link btn-hover new-contact">Add Contact</a>
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

    </form>
    <!-- form -->
</body>

<script src="/_content/_js/cookies-201710301235.js"></script>

<!-- js custom -->
<script src="contactlist-16-mar-2020.js"></script>

</html>
