<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompaniesReport.aspx.cs" Inherits="Crm6.Reporting.Companies.CompaniesReport" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Companies Report</title>
    <!-- css custom -->
    <link href="companies-report-21-mar-2020.css" rel="stylesheet" />
    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>


<body class="addEditPage">
    <form runat="server">
        <div id="main">
            <%-- hidden values --%>
            <asp:Label CssClass="hide" Style="color: white" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" Style="color: white" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" Style="color: white" ID="lblUsername" runat="server" Text=""></asp:Label>

            <!--navbar mobile-->
            <uc1:navmobile runat="server" />

            <div class="container-fluid">
                <div class="row">
                    <!--navbar desktop-->
                    <uc1:nav runat="server" ID="navSidebar" />
                    <!--.page-container-->
                    <div class="page-container reportPage">

                        <!--.page-content-->
                        <div class="page-content">

                            <div id="content" class="animated fadeIn">

                                <!--.top-header-->
                                <header class="top-header globalHead">
                                    <h1 class="page-title">Companies</h1>
                                </header>

                                <!--Page Content-->
                                <div class="wrapper br-fields">
                                    <div class="ibox MB20 MT15">
                                        <div class="ibox-content  PB0 PT10">

                                            <div id="divReport">

                                                <%-- companies report filter --%>
                                                <div>
                                                    <div class="row MT10">
                                                        <div class="col-md-6 col-lg-3">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">Country</label>
                                                                <asp:DropDownList runat="server" ID="ddlCountry" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-6 col-lg-3">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">Industry</label>
                                                                <asp:DropDownList runat="server" ID="ddlIndustry" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-md-6 col-lg-3">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">Competitor</label>
                                                                <asp:DropDownList runat="server" ID="ddlCompetitor" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-6 col-lg-3">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">Campaign</label>
                                                                <asp:DropDownList runat="server" ID="ddlCampaign" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-6 col-lg-3">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">Source</label>
                                                                <asp:DropDownList runat="server" ID="ddlSource" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-6 col-lg-3">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">Status</label>
                                                                <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="clearfix"></div>

                                                <%--toolbar--%>
                                                <div class="row  footer-actions">
                                                    <div class="col-md-5 col-sm-5">
                                                        <h4 class="font-bold total-record-count"></h4>
                                                    </div>
                                                    <div class="col-md-7  col-sm-7">
                                                        <div id="divToolbar">
                                                            <button id="btnRunReport" class="primary-btn language-entry">Run Report</button>
                                                            <a id="btnExcel" class="primary-btn language-entry hide">Excel</a>
                                                        </div>
                                                    </div>

                                                </div>

                                            </div>
                                        </div>
                                    </div>


                                    <%--report body--%>
                                    <div class="ibox MB50 MT10 MT20 hide" id="divReportContent">
                                        <div class="ibox-content P0" style="height: 600px">

                                            <table class="table table-hover" id="tblCompaniesReport">

                                                <%-- report header --%>
                                                <thead>
                                                    <tr>
                                                        <th class=""><span class="language-entry">Company</span></th>
                                                        <th class="W150"><span class="language-entry">City</span></th>
                                                        <th class="W150"><span class="language-entry">Country</span></th>
                                                        <th class="W150"><span class="language-entry">Industry</span></th>
                                                        <th class="W200"><span class="language-entry">Source</span></th>
                                                        <th class="W200"><span class="language-entry">Customer Status</span></th>
                                                        <th class="W200"><span class="language-entry">Competitor</span></th>
                                                        <th class="W200"><span class="language-entry">Campaign</span></th>
                                                        <th class="W150"><span class="language-entry">Telephone</span></th>
                                                        <th class="W150"><span class="language-entry">Fax</span> </th>
                                                        <th class="W300"><span class="language-entry">Address</span></th>
                                                        <th class="W200"><span class="language-entry">Created</span></th>
                                                        <th class="W200"><span class="language-entry">Last Activity</span></th>
                                                        <th class="W100"></th>
                                                    </tr>
                                                </thead>

                                                <%-- companies rows --%>
                                                <tbody></tbody>

                                                <%-- companies footer --%>
                                                <tfoot></tfoot>

                                            </table>

                                        </div>

                                        <%-- no companies --%>
                                        <div id="divNoItems" class="hide">
                                            <div class="alert alert-warning text-center PT50">
                                                <i class="fa fa-4x fa-building text-warning m-b-md"></i>
                                                <p>
                                                    <label class="language-entry">no companies found</label>
                                                </p>
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
<script src="companies-report-18-mar-2020.js"></script>

</html>
