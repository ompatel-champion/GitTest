<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KPIs.aspx.cs" Inherits="Crm6.Reporting.Kpis.Kpis" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Sales Rep KPIs Report</title>
    <!-- css custom -->
    <link href="kpis-02-apr-2020.css" rel="stylesheet" />
     <!-- favicon -->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="addEditPage">
    <form runat="server">
        <div id="main">
            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblUserIdGlobal" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

            <%--Load from Session--%>
            <asp:Label CssClass="hide" ID="lblDateFormat" runat="server" Text="dd MM, yyyy"></asp:Label>
            <asp:Label CssClass="hide" ID="lblDateFormatMask" runat="server" Text="0"></asp:Label>

            <!--navbar mobile-->
            <uc1:navmobile runat="server" />

            <div class="container-fluid">
                <div class="row">

                    <!--.page-container-->
                    <div class="page-container reportPage">

                        <!--.page-content-->
                        <div class="page-content">

                            <!--navbar desktop-->
                            <uc1:nav runat="server" ID="navSidebar" />

                            <!- #content-->
                            <div id="content" class="animated fadeIn">

                                <!--.top-header-->
                                <header class="top-header globalHead">
                                    <h1 class="page-title">Sales Rep Performance KPIs</h1>
                                </header>

                                <!--Page Content-->
                                <div class="wrapper  br-fields">

                                    <div class="ibox MB20 MT15">
                                        <div class="ibox-content PB0">
                                            <%-- activities by date range report --%>

                                            <div id="divReport">
                                                <%-- events report filter --%>
                                                <div id="divReportFilter">

                                                    <div class="row MT10">
                                                        <div class="col" id="countryContainer" runat="server">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">Country</label>
                                                                <asp:DropDownList runat="server" ID="ddlCountry" placeholder="Select Country" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col" id="locationContainer" runat="server">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">Location</label>
                                                                <asp:DropDownList runat="server" ID="ddlLocation" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col" id="Div1" runat="server">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">Sales Rep</label>
                                                                <asp:DropDownList runat="server" ID="ddlSalesReps" CssClass="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <%--Date Range--%>
                                                        <div class="col">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">From</label>
                                                                <div class="input-group">
                                                                    <span class="input-group-addon"> </span>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtDateFrom" placeholder="" data-name="datepicker" MaxLength="50"></asp:TextBox>
                                                                    <i class="icon-calendar"></i>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel">To</label>
                                                                <div class="input-group">
                                                                    <span class="input-group-addon"> </span>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtDateTo" placeholder="" data-name="datepicker" MaxLength="50"></asp:TextBox>
                                                                    <i class="icon-calendar"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                       
                                                    </div>
                                                </div>

                                                <div class="clearfix"></div>

                                                <div class="row footer-actions">
                                                    <div class="col-md-5 col-sm-5">
                                                        <h4 class="font-bold total-record-count"></h4>
                                                    </div>
                                                    <div class="col-md-7 col-sm-7">
                                                        <div id="divToolbar">
                                                            <button id="btnRunReport" class="primary-btn language-entry">Run Report</button>
                                                            <a id="btnExcel" href="javascript:void(0)" class="primary-btn  language-entry hide">Excel</a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="ibox MB50 hide" id="divReportContent">
                                        <div class="ibox-content P0">

                                            <%-- deals report header --%>
                                            <table class="table table-hover" id="tblKpisReport">
                                                <thead>
                                                    <tr>
                                                        <th class=""><span class="language-entry">User</span></th>
                                                        <th class=""><span class="language-entry">Country</span></th>
                                                        <th class=""><span class="language-entry">Location</span></th>
                                                        <th class="W70 text-center"><span class="language-entry">Logins</span></th>
                                                        <th class="W70 text-center"><span class="language-entry">Meetings</span> </th>
                                                        <th class="W70 text-center"><span class="language-entry">Tasks</span> </th>
                                                        <th class="W70 text-center"><span class="language-entry">Notes</span> </th>
                                                        <th class="W70 text-center"><span class="language-entry">New Deals</span></th>
                                                        <th class="W70 text-center"><span class="language-entry">Won Deals</span></th>
                                                        <th class="W70 text-center"><span class="language-entry">Lost Deals</span></th>
                                                    </tr>
                                                </thead>

                                                <tbody>
                                                    <%-- render report rows --%>
                                                </tbody>

                                                <tfoot>
                                                    <%-- render report footer --%>
                                                </tfoot>
                                            </table>
                                        </div>

                                    </div>
                                    <%-- no records --%>
                                    <div id="divNoItems" class="hide">
                                        <div class="alert alert-warning text-center PT50">
                                            <i class="fa fa-4x fa-building text-warning m-b-md"></i>
                                            <p>
                                                <label class="language-entry">No records found</label>
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
    </form>
</body>

<script src="kpis-04-apr-2020.js"></script>

</html>
