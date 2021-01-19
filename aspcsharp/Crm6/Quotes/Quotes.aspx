<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Quotes.aspx.cs" Inherits="Crm6.Admin.Quotes.Quotes" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Quotes</title>
    <!-- css custom -->
    <link href="quotes.css" rel="stylesheet" />
    <!-- favicon -->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body>
    <form runat="server">
        <div id="wrapper">
            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

            <!-- navbar mobile -->
            <uc1:navmobile runat="server" />

            <div id="page-wrapper" class="gray-bg">

                <!-- navbar desktop -->
                <uc1:nav runat="server" ID="navSidebar" />

                <%-- header --%>
                <div class="wrapper wrapper-content animated fadeInRight row">
                    <div id="divQuotesTab" class="col-md-12" runat="server">
                        <div class="col-md-10">
                            <h3>Quotes</h3>
                        </div>
                        <%-- content --%>
                        <div class="tab-pane deals-page" id="tab-quotes">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="ibox quotes-wrp">
                                        <form class="" action="#">
                                            <div class="row">

                                                <!-- .search-box -->
                                                <div class="col-xl-4 col-lg-4 col-md-12">
                                                    <div class="search-box">
                                                 <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
                                                        <asp:ImageButton runat="server" ID="btnSearch" OnClick="btnSearch_Click" ImageUrl="/_content/img/search-icon.png"></asp:ImageButton>
                                                    </div>
                                                </div>

                                                <!-- .search dropdowns -->
                                                <div class="col-xl-8 col-lg-8 col-md-12">
                                                    <div class="right-wrp">
                                                        <div class="select-box-wrp">
                                                            <asp:DropDownList ID="ddlSalesReps" CssClass="custom-select2" runat="server">
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="select-box-wrp">
                                                            <asp:DropDownList ID="ddlBranch" CssClass="custom-select2" runat="server">
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="select-box-wrp">
                                                            <asp:DropDownList ID="ddlStatus" CssClass="custom-select2" runat="server">
                                                            </asp:DropDownList>
                                                        </div>
                                                        <asp:Button Text="Go" runat="server" ID="btnGo" OnClick="btnGo_Click" />
                                                        <asp:Button CssClass="btn btn-primary" Text="New Portrix Quote" ID="btnNewPortrixQuote" OnClick="btnNewPortrixQuote_Click" runat="server" />
                                                        <div class="btn-wrp add-new-btn">
                                                            <a href="QuoteAddEdit/QuoteAddEdit.aspx" class="edit_link btn-hover"><i class="icon-plus"></i>&nbsp;Quote</a>
                                                        </div>
                                                    </div>
                                                </div>

                                                <!-- .search dropdowns -->
                                            </div>
                                        </form>

                                        <!-- #quotes-datatable -->
                                        <div class="table-wrp">
                                            <table id="quotes-datatable" class="mtop-15">
                                                <thead>
                                                    <tr>
                                                        <th>COMPANY</th>
                                                        <th>DEAL</th>
                                                        <th>SALES OWNER</th>
                                                        <th>BRANCH</th>
                                                        <th>CODE</th>
                                                        <th>ROUTE</th>
                                                        <th>SUBMITTED</th>
                                                        <th>VALID THRU</th>
                                                        <th class="text-center">PIECES</th>
                                                        <th class="text-center">WEIGHT</th>
                                                        <th class="text-center">TERMS</th>
                                                        <th class="text-center">STATUS</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater runat="server" ID="rptQuotes">
                                                        <ItemTemplate>
                                                            <tr class="odd">
                                                                <td>
                                                                    <asp:Label runat="server" Text='<%# Eval("CompanyName")%>'></asp:Label></td>
                                                                <td>
                                                                    <asp:Label runat="server" Text='<%# Eval("DealName")%>'></asp:Label></td>
                                                                <td>
                                                                    <asp:Label runat="server" Text='<%# Eval("CustomerName")%>'></asp:Label></td>
                                                                <td>
                                                                    <asp:Label runat="server" Text='<%# Eval("BranchName")%>'></asp:Label></td>
                                                                <td>
                                                                    <asp:Label runat="server" Text='<%# Eval("QuoteCode")%>'></asp:Label></td>
                                                                <td>
                                                                    <asp:Label runat="server" Text='<%# Eval("Destination")%>'></asp:Label></td>
                                                                <td>
                                                                    <asp:Label runat="server" Text='<%# Eval("CreatedDate", "{0:dd-MMM-yy}") %>'></asp:Label></td>
                                                                <td>
                                                                    <asp:Label runat="server" Text='<%# Eval("ValidTo", "{0:dd-MMM-yy}") %>'></asp:Label></td>
                                                                <td class="text-center">
                                                                    <asp:Label runat="server" Text='<%# Eval("TotalPackages")%>'></asp:Label></td>
                                                                <td class="text-center">
                                                                    <asp:Label runat="server" Text='<%# Eval("TotalWeight")%>'></asp:Label></td>
                                                                <td class="text-center">
                                                                    <asp:Label runat="server" Text='<%# Eval("Incoterm")%>'></asp:Label></td>
                                                                <td class="text-center"><a href="#" class="submit">
                                                                    <asp:Label runat="server" Text='<%# Eval("QuoteStatus")%>'></asp:Label></a></td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>
                                        </div>
                                        <!-- #quotes-datatable -->
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

<script src="/_content/_js/jquery.dataTables.min.js"></script>
<script src="/_content/_js/cookies-201710301235.js"></script>

<!-- js custom -->
<script src="quotes.js"></script>

</html>
