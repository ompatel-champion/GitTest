<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GlobalLocationAddEdit.aspx.cs" Inherits="Crm6.Locations.GlobalLocationAddEdit" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <title></title>

    <!--css custom-->
    <link href="globallocationaddedit-19-mar-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="global-location-addedit">

    <form runat="server" id="divGlobalLocationSetup">

        <!-- #main start -->
        <div id="main">

            <!-- hidden values -->
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0" Style="display: none;"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0" Style="display: none;"></asp:Label>
            <asp:Label CssClass="hide" ID="lblGlobalLocationId" runat="server" Text="0" Style="display: none;"></asp:Label>

            <!-- navbar mobile start-->
            <uc1:navmobile runat="server" />
            <!-- navbar mobile end-->

            <div class="container-fluid">
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
                                    <h1 class="page-title">
                                        <asp:Label ID="lblBreadcrumbHeader" Text="Add Global Location" runat="server" CssClass=" m-t-sm "></asp:Label>
                                    </h1>
                                </header>
                                <!-- .top-header -->

                                <!-- page content -->
                                <div class="wrapper section-content">

                                    <div id="GlobalLocationAddEdit" class="GlobalLocationAddEdit">

                                        <!-- form fields -->
                                        <div class="row">

                                            <!-- left column -->
                                            <div class="col-md-12 form-fields">

                                                <div class="form-group filled">
                                                    <label class="inputLabel">Location</label>
                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtLocationName" placeholder="Location Name" MaxLength="100"></asp:TextBox>
                                                    <span class="error-text"></span>
                                                </div>

                                                <div class="form-group filled">
                                                    <label class="inputLabel">Code</label>
                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtLocationCode" placeholder="Location Code" MaxLength="50"></asp:TextBox>
                                                    <span class="error-text"></span>
                                                </div>

                                                <div class="form-group iconField filled">
                                                    <label class="inputLabel">Country</label>
                                                    <asp:DropDownList runat="server" ID="ddlCountry" CssClass="form-control"></asp:DropDownList>
                                                    <span class="error-text"></span>
                                                </div>

                                                <!-- checkboxes -->
                                                <div class="row checklist">
                                                    <div class="col-sm-6 ML40">
                                                        <div class="form-group filled chkActive">
                                                            <asp:CheckBox runat="server" ID="chkAirport"></asp:CheckBox>
                                                            <label for="chkAirport" class="language-entry">Airport</label>
                                                        </div>
                                                        <div class="form-group filled chkActive">
                                                            <asp:CheckBox runat="server" ID="chkInlandPort"></asp:CheckBox>
                                                            <label for="chkInlandPort" class="language-entry">Inland Port</label>
                                                        </div>
                                                        <div class="form-group filled chkActive">
                                                            <asp:CheckBox runat="server" ID="chkMultiModal"></asp:CheckBox>
                                                            <label for="chkMultiModal" class="language-entry">Multi-Modal</label>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-6 ML40">
                                                        <div class="form-group filled chkActive">
                                                            <asp:CheckBox runat="server" ID="chkRailTerminal"></asp:CheckBox>
                                                            <label for="chkRailTerminal" class="language-entry">Rail Terminal</label>
                                                        </div>
                                                        <div class="form-group filled chkActive">
                                                            <asp:CheckBox runat="server" ID="chkRoadTerminal"></asp:CheckBox>
                                                            <label for="chkRoadTerminal" class="language-entry">Road Terminal</label>
                                                        </div>
                                                        <div class="form-group filled chkActive">
                                                            <asp:CheckBox runat="server" ID="chkSeaport"></asp:CheckBox>
                                                            <label for="chkSeaport" class="language-entry">Seaport</label>
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>

                                        </div>

                                        <hr />

                                        <!-- buttons -->
                                        <div class="row buttons no-gutters">
                                            <div class="col-auto">
                                                <div class="form-btns">
                                                    <button type="button" class="delete-btn text-danger secondary-btn hide" id="btnDelete">Delete</button>
                                                </div>
                                            </div>
                                            <div class="col"></div>
                                            <div class="col-auto">
                                                <div class="form-btns">
                                                    <button type="button" class="secondary-btn cancel-btn" id="btnCancel">Cancel</button>
                                                </div>
                                            </div>
                                            <div class="col-auto">
                                                <div class="form-btns">
                                                    <button type="button" class="primary-btn" id="btnSave">Save</button>
                                                </div>
                                            </div>
                                        </div>

                                        <!--spinner-->
                                        <div id="divSpinner" class="hide">
                                            <div class="ajax-modal">
                                                <div class="ibox ibox-content text-center ajax-modal-txt">
                                                    <div class="spinner"></div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                </div>
                                <!-- page content end -->

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
<script src="globallocationaddedit-21-mar-2020.js"></script>

</html>
