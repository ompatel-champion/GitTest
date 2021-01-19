<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LaneAddEdit.aspx.cs" Inherits="Crm6.Deals.LaneAddEdit" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <title>Lane Add Edit</title>

    <!-- css custom -->
    <link href="laneaddedit-22-feb-2020.css" rel="stylesheet" />

    <!-- favicon -->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="lanepage">

    <form runat="server">

        <!-- #main start -->
        <div id="main">

            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblLaneId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblDealId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCompanyName" runat="server" Text="0"></asp:Label>
            <input type="hidden" id="ddlService" value="Air" />

            <!-- navbar mobile -->
            <uc1:navmobile runat="server" />

            <div class="main-wrapper container-fluid">
                <div class="row">

                    <!-- .page-container -->
                    <div class="page-container">

                        <!-- .page-content -->
                        <div class="page-content">

                            <!-- nav sidebar desktop -->
                            <uc1:nav runat="server" ID="navSidebar" />

                            <!-- #content -->
                            <div id="content" class="animated fadeIn">

                                <!-- .top-header -->
                                <header class="top-header globalHead">
                                    <h1 class="page-title">New Lane</h1>
                                    <div class="closeX" data-action="cancel-event"></div>
                                </header>
                                <!-- .top-header -->

                                <!-- Lane Add/Edit -->
                                <div id="divLaneAddEdit" class="wrapper">

                                    <div class="section-content">
                                        <div class="section-body">
                                            <!--nav bar-->
                                            <div id="divNav" class="divNav">
                                                <div class="btn-group" role="group">
                                                    <button id="btnAir" type="button" data-name="Air" class="btn btn-primary active"><i class="icon-airplane font-icon"></i>Air</button>
                                                    <button id="btnOcean" type="button" data-name="Ocean" class="btn btn-secondary"><i class="icon-ocean font-icon"></i>Sea</button>
                                                    <button id="btnRoad" type="button" data-name="Road" class="btn btn-secondary"><i class="icon-road font-icon"></i>Road</button>
                                                    <button id="btnLogistics" type="button" data-name="Logistics" class="btn btn-secondary"><i class="icon-logistics font-icon"></i>Logistics</button>
                                                    <button id="btnBrokerage" type="button" data-name="Brokerage" class="btn btn-secondary"><i class="icon-Brokerage----White font-icon"></i>Brokerage</button>
                                                </div>
                                            </div>
                                            <!-- .tabs-wrapper -->
                                            <div class="tabs-wrapper">
                                                <!--volume / revenue / profit-->
                                                <div id="divVolumeRevenueProfit">
                                                    <div class="row">
                                                        <div class="custom-col loadOcean-col col" style="display: none;">
                                                            <label class="label-title">Load</label>
                                                            <select id="ddlLoadOcean">
                                                                <!--ocean-->
                                                                <option value="fcl">FCL</option>
                                                                <option value="lcl">LCL</option>
                                                                <option value="roro">RoRo-Breakbulk</option>
                                                            </select>
                                                        </div>
                                                        <div class="custom-col loadRoad-col col" style="display: none;">
                                                            <label class="label-title">Load</label>
                                                            <select id="ddlLoadRoad">
                                                                <!--ocean-->
                                                                <option value="ftl">FTL</option>
                                                                <option value="ltl">LTL</option>
                                                                <option value="expedited">Expedited</option>
                                                            </select>
                                                        </div>
                                                        <div class="vol-col custom-col col-xl-3">
                                                            <label class="label-title">Volume<span class="req">*</span></label>
                                                            <div class="clearfix" style="position: relative;">
                                                                <input id="txtVolume" class="txtVolume" type="text" placeholder='Volume' />
                                                                <span class="errorText">Required</span>
                                                                <select id="ddlVolumeUnit">
                                                                    <option value="kgs">KGs</option>
                                                                    <option value="tonnes">Tonnes</option>
                                                                </select>
                                                                <select id="ddlShipmentFrequency">
                                                                    <option value="Per Month">Per Month</option>
                                                                    <option value="Per Year">Per Year</option>
                                                                    <option value="Per Week">Per Week</option>
                                                                    <option value="Spot">Spot</option>
                                                                </select>
                                                            </div>
                                                        </div>
                                                        <div class="custom-col col">
                                                            <label class="label-title">Revenue<span class="req">*</span></label>
                                                            <div class="input-holder">
                                                                <input id="txtRevenue" type="text" placeholder="X,XXX" />
                                                                <span class="errorText">Required</span>
                                                                <span class="prefix revenuePrefix">$</span>
                                                            </div>
                                                        </div>
                                                        <div class="custom-col col-xl-3">
                                                            <label class="label-title">Profit<span class="req">*</span></label>
                                                            <div class="clearfix">
                                                                <div class="input-holder txtProfit">
                                                                    <input id="txtProfit" type="text" placeholder="X,XXX" />
                                                                    <span class="errorText">Required</span>
                                                                    <span class="prefix profitPrefix">%</span>
                                                                </div>
                                                                <select id="ddlProfitType">
                                                                    <%--<option value="Percent">Percentage</option>
																	<option value="Flat Amount">Flat Amount</option>
																	<option value="Per Container">Per Container</option>
																	<option value="Per Lb">Per Lb</option>
																	<option value="Per Kg">Per Kg</option>
																	<option value="Per CBM">Per CBM</option>
																	<option value="Per Tonne">Per Tonne</option>
																	<option value="Per TEU">Per TEU</option>--%>
                                                                </select>
                                                            </div>
                                                        </div>
                                                        <div class="custom-col currency-col col order-1">
                                                            <asp:DropDownList runat="server" ID="ddlCurrency" CssClass="form-control"></asp:DropDownList>
                                                            <div class="input-holder txtCurrency">
                                                                <input id="txtCurrency" type="text" value="0" readonly />
                                                                <span class="prefix revenuePrefix">$</span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <!--origin + destination-->
                                                <div id="divOriginDestination">
                                                    <div class="row org-des">
                                                        <!--origin-->
                                                        <div id="divOrigin" class="col-md-6">
                                                            <div class="divOriginDestination">
                                                                <label class="label-title">Origin</label>
                                                                <div class="row first-row">
                                                                    <div class="input-wrap col-lg-6">
                                                                        <asp:TextBox ID="txtOriginShipper" placeholder="Shipper" runat="server"></asp:TextBox>
                                                                    </div>
                                                                    <div class="input-wrap col-lg-6">
                                                                        <asp:DropDownList runat="server" ID="ddlOriginRegion" CssClass="form-control"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="input-wrap col-lg-6">
                                                                        <asp:DropDownList runat="server" ID="ddlOriginCountry" CssClass="form-control"></asp:DropDownList>
                                                                    </div>
                                                                    <div class="input-wrap col-lg-6">
                                                                        <asp:DropDownList runat="server" ID="ddlOriginLocation" CssClass="form-control"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <!--destination-->
                                                        <div id="divDestination" class="col-md-6">
                                                            <div class="divOriginDestination">
                                                                <label class="label-title">Destination</label>
                                                                <div class="row first-row">
                                                                    <div class="input-wrap col-lg-6">
                                                                        <input id="txtConsigneeName" placeholder="Consignee" type="text" />
                                                                    </div>
                                                                    <div class="input-wrap col-lg-6">
                                                                        <asp:DropDownList runat="server" ID="ddlDestinationRegion" CssClass="form-control"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="input-wrap col-lg-6">
                                                                        <asp:DropDownList runat="server" ID="ddlDestinationCountry" CssClass="form-control"></asp:DropDownList>
                                                                    </div>
                                                                    <div class="input-wrap col-lg-6">
                                                                        <asp:DropDownList runat="server" ID="ddlDestinationLocation" CssClass="form-control"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row logi-info MT20">
                                                        <div class="col-xl-4 log-col">
                                                            <label class="label-title">Receive from 3PL<span class="req"></span></label>
                                                            <select multiple id="ddlReceive">
                                                                <option value="Confirmed Pick">Confirmed Pick</option>
                                                                <option value="Pick and Pack">Pick and Pack</option>
                                                                <option value="Storage">Storage</option>
                                                                <option value="Kitting">Kitting</option>
                                                                <option value="Cross Dock">Cross Dock</option>
                                                                <option value="Not Sure">Not Sure</option>
                                                            </select>
                                                            <div class="chk-box-wrp">
                                                                <input type="checkbox" id="chkRequireBarcode" name="chkRequireBarcode" value="Bike">
                                                                <label for="chkRequireBarcode" class="chk-label">Requires Barcode</label>
                                                            </div>
                                                        </div>
                                                        <div class="col-xl-4 log-col">
                                                            <label class="label-title">Service Location</label>
                                                            <asp:DropDownList runat="server" ID="ddlServiceLocation" CssClass="form-control"></asp:DropDownList>
                                                            <div class="chk-box-wrp">
                                                                <input type="checkbox" id="chkTracking" name="chkTracking" value="Bike">
                                                                <label class="chk-label" for="chkTracking">Track & Trace</label>
                                                            </div>
                                                        </div>
                                                        <div class="col-xl-4 log-col">
                                                            <label class="label-title">Special Requirements</label>
                                                            <select multiple id="ddlSpecialRequirements">
                                                                <option value="HAACP">HAACP</option>
                                                                <option value="Hazardous Goods">Hazardous Goods</option>
                                                                <option value="Temperature">Temperature</option>
                                                                <option value="Expiry - Batching">Expiry - Batching</option>
                                                                <option value="Bond 77G">Bond 77G</option>
                                                                <option value="Bond S79">Bond S79</option>
                                                            </select>
                                                            <div class="chk-box-wrp">
                                                                <input type="checkbox" id="chkCustomerPickup" name="chkCustomerPickup" value="Bike">
                                                                <label class="chk-label" for="chkCustomerPickup">Customer Pick Up at Warehouse</label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <!--only show if Brokerage button selected-->
                                                <div id="divBrokerage" style="display: none;"></div>
                                            </div>
                                            <!--footer-->
                                            <div class="section-footer">
                                                <label class="label-title">Comments</label>
                                                <textarea rows="4" cols="50" id="txtComments" placeholder="Add comment..."></textarea>
                                            </div>
                                            <div class="footer-action footerBox">
                                                <div class="row align-items-end">
                                                    <div class="col-md-4 col-sm-5 col-12">
                                                        <div class="form-btns">
                                                            <button type="button" class="delete-btn hide text-danger secondary-btn" id="btnDelete">Delete</button>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-4 col-sm-1 col-12">
                                                    </div>
                                                    <div class="col-md-4 col-sm-6 col-12">
                                                        <div class="form-btns">
                                                            <button type="button" class="primary-btn FR" id="btnSaveLane">Save</button>
                                                            <button type="button" class="secondary-btn cancel-btn MR10" id="btnCancel">Cancel</button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>



                                        </div>
                                        <!-- .tabs-wrapper -->
                                    </div>
                                    <!-- .section-content -->
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

    <!-- js custom -->
    <script src="/_content/_js/polyfill-loader-17-oct-2019.js"></script>
    <script src="laneaddedit-08-apr-2020.js"></script>

</body>
</html>
