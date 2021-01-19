<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReassignCompany.aspx.cs" Inherits="Crm6.Companies.Reassign.ReassignCompany" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reassign Company</title>
    <!--css custom-->
    <link href="reassigncompany-14-mar-2020.css" rel="stylesheet" />
    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body>
    <form runat="server" id="divReassignCompanySetup" class="page-content">
        <asp:Label Style="display: none" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label Style="display: none" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
        <asp:Label Style="display: none" ID="lblCompanyId" runat="server" Text="0"></asp:Label>

        <uc1:navmobile runat="server" />

        <div class="main-wrapper container-fluid">
            <div class="row">

                <!-- .page-container -->
                <div class="page-container">

                    <!-- .page-content -->
                    <div class="page-content">

                        <!-- nav sidebar desktop -->
                        <uc1:nav runat="server" ID="nav1" />

                        <!-- #content -->
                        <div id="content" class="animated fadeIn">

                            <!-- .top-header -->
                            <header class="top-header globalHead">
                                <div class="row">
                                    <div class="col-sm-12 pageInfo">
                                        <h1 class="page-title">
                                            <asp:Label ID="lblBreadcrumbHeader" Text="Reassign Company" runat="server" CssClass=" m-t-sm "></asp:Label>
                                        </h1>
                                    </div>
                                </div>
                            </header>

                            <div id="" class="wrapper section-content reassign">
                                <div class="row" style="padding-top: 10px;">
                                    <div class="col-lg-12 col-md-12">
                                        <div class="ibox" style="margin-bottom: 0px;">
                                            <div class="ibox-content">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <label class="col-sm-12 control-label language-entry"><b>Select a user to reassign this company</b></label>
                                                        <div class="col-sm-12">
                                                            <p class="text-muted">
                                                                This will assign the selected user as the company owner and assign all the deals,
                                                calendar events and tasks to this user.
                                                            </p>

                                                            <asp:DropDownList runat="server" ID="ddlUser" CssClass="form-control"></asp:DropDownList>
                                                            <span class="error-text">
                                                                <label class="error"></label>
                                                            </span>
                                                        </div>
                                                        <div class="clearfix"></div>
                                                    </div>

                                                    <!-- .footer-box -->
                                                    <div class="footer-action">
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <button type="button" class="primary-btn" id="btnReassign">Reassign</button>
                                                                <button type="button" class="secondary-btn cancel-btn" id="btnCancel">Cancel</button>

                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>

<!-- js custom -->
<script src="reassigncompany-14-mar-2020.js"></script>

</html>
