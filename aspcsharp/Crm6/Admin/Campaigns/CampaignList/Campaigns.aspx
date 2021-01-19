<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Campaigns.aspx.cs" Inherits="Crm6.Admin.Campaigns" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Campaigns</title>

    <!--css custom-->
    <link href="campaigns-02-mar-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="locations">
    <form runat="server">

        <!-- #main start -->
        <div id="main">

            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

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
									    <div class="col-md-3 col-5"><h1 class="page-title">Campaigns</h1></div>
                                        <div class="col-md-9 col-7">
                                            <div class="add-new-btn pull-right">
                                                <a href="javascript:void(0)" class="new-campaign primary-btn"><i class="icon-plus"></i>Campaign</a>
                                            </div>
                                        </div>
                                    </div>
                                </header>
                                <!-- .top-header -->

                                <!--Page Content -->
                                <div class="wrapper">

                                    <!--campaigns-->
                                    <div id="divCampaignsTab" class="btab-content active">

                                        <!--button bar - tab descrip/search/add-->
                                        <div class="list-table-btn-bar row no-gutters align-items-center">
                                            <div class="col">
                                               <span class="descrip">Marketing campaigns drive conversion of Companies and Deals.</span>
                                            </div>
                                        </div>

                                        <!-- #divCampaignsTab -->
                                        <div id="divCampaigns" class="tab-list list-table">

                                            <%-- campaign list --%>
                                            <table class="table table-hover" id="tblCampaigns">
                                                <thead>
                                                    <tr>
                                                        <th>Campaign Name</th>
                                                        <th>Type</th>
                                                        <th>Start Date</th>
                                                        <th>End Date</th>
                                                        <th>Owner</th>
                                                        <th>Status</th>
                                                        <th class="text-center"></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater ID="rptCampaigns" runat="server">
                                                        <ItemTemplate>
                                                            <tr data-id="<%# Eval("CampaignId") %>">
                                                                <td><%# Eval("CampaignName") %></td>
                                                                <td><%# Eval("CampaignType") %></td> 
                                                                <td><%# Convert.ToDateTime( Eval("StartDate")).ToString("dd-MMM-yy") %></td>
                                                                <td><%# Convert.ToDateTime( Eval("EndDate")).ToString("dd-MMM-yy") %></td> 
                                                                <td><%# Eval("CampaignOwnerName") %></td>
                                                                <td><%# Eval("CampaignStatus") %></td>
                                                                <td class="text-right action-cell">
                                                                    <a class="hover-link edit-campaign" title="Edit Campaign" data-action="edit">Edit</a>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>

                                            <%-- no campaigns --%>
                                            <div id="divNoItems" class="no-items empty-box" runat="server" visible="false">
                                                <i class="icon-Deal----Gray"></i>
                                                <p class="e-text">No campaigns found</p>
                                                <div class="btn-wrp">
                                                    <a href="#" class="primary-btn new-location">Add Campaign</a>
                                                </div>
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
<script src="campaigns-07-jan-2020.js"></script>
</html>
