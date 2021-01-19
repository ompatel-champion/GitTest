<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DealAddEdit.aspx.cs" EnableEventValidation="false" Inherits="Crm6.Deals.DealAddEdit" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <title></title>
    <!--css custom-->
    <link href="dealaddedit-07.1-apr-2020.css" rel="stylesheet" />
    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="deal-add addEditPage">

    <form runat="server" id="divDealSetup">

        <!-- #main start -->
        <div id="main">

            <!-- hidden values -->
            <asp:Label CssClass="hide" ID="lblDealId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblDealSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCompanyId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblProposalDate" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblDecisionDate" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblFirstShipmentDate" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblContractEndDate" runat="server" Text="0"></asp:Label>
            <input type='hidden' id='tags' style='width: 300px' />

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
                                    <asp:Label ID="lblBreadcrumbHeader" Text="New Deal" runat="server" CssClass=" m-t-sm "></asp:Label></h1>
                                    <div onclick="window.history.back();" class="closeX" data-action="cancel-event"></div>
                                </header>
                                <!-- .top-header -->

                                <!-- Page Content -->
                                <div class="wrapper section-content addEditContent">

                                    <div id="DealAddEdit" class="DealAddEdit br-fields">
                                        <div class="row">

                                            <div class="col-lg-8 col-md-12">
                                                <div class="row">
                                                    <div class="col-md-6 form-fields">
                                                        <div class="form-group filled">
                                                            <label class="inputLabel">Deal Name</label>
                                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtDealName" MaxLength="100"></asp:TextBox>
                                                            <span class="error-text"></span>
                                                        </div>
                                                        <!-- company -->
                                                        <div class="form-group filled" id="divCompanyContainer">
                                                            <label class="inputLabel">Company</label>
                                                            <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-control"></asp:DropDownList>
                                                            <span class="error-text"></span>
                                                        </div>
                                                        <!-- contact -->
                                                        <div class="form-group filled">
                                                            <label class="inputLabel">Contact</label>
                                                            <asp:DropDownList runat="server" ID="ddlContact" CssClass="form-control"></asp:DropDownList>
                                                            <span class="error-text"></span>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-6 form-fields">
                                                        <!-- deal owner -->
                                                        <div class="form-group filled">
                                                            <label class="inputLabel">Deal Owner</label>
                                                            <asp:DropDownList runat="server" ID="ddlDealOwner" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                        <!-- deal type -->
                                                        <div class="form-group filled">
                                                            <label class="inputLabel">Deal Type</label>
                                                            <asp:DropDownList runat="server" ID="ddlDealType" CssClass="form-control"></asp:DropDownList>
                                                            <span class="error-text"></span>
                                                        </div>
                                                        <!-- incoterms -->
                                                        <div class="form-group filled">
                                                            <label class="inputLabel">Incoterms</label>
                                                            <asp:HiddenField ID="hdnIncoterms" runat="server" />
                                                            <asp:DropDownList runat="server" ID="ddlIncoterms" CssClass="form-control" Multiple="true"></asp:DropDownList>
                                                        </div>

                                                        <!-- won / lost reason-->
                                                        <div class="form-group hide iconField filled" id="divWonLostReason">
                                                            <label class="inputLabel">Won/Lost Reason</label>
                                                            <asp:DropDownList runat="server" ID="ddlWonLostReason" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-md-6 form-fields">
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <div class="row">
                                                                    <div class="col-xl-6 col-lg-12 col-md-12">
                                                                        <!-- proposal date -->
                                                                        <div class="form-group iconField filled">
                                                                            <div class="date-input">
                                                                                <label class="inputLabel">Proposal Date</label>
                                                                                <asp:TextBox CssClass="form-control " runat="server" ID="txtProposalDate" data-name="datepicker" MaxLength="20" autocomplete="none"></asp:TextBox>
                                                                                <i class="icon-calendar"></i>
                                                                                <span class="error-text"></span>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xl-6 col-lg-12 col-md-12">
                                                                        <div class="form-group iconField filled">
                                                                            <div class="date-input">
                                                                                <label class="inputLabel">First Shipment</label>
                                                                                <asp:TextBox CssClass="form-control " runat="server" ID="txtFirstShipmentDate" data-name="datepicker" MaxLength="20" autocomplete="none"></asp:TextBox>
                                                                                <i class="icon-calendar"></i>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-6 form-fields">
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <div class="row">
                                                                    <div class="col-xl-6 col-lg-12 col-md-12">
                                                                        <!-- decision date -->
                                                                        <div class="form-group iconField filled">
                                                                            <div class="date-input">
                                                                                <label class="inputLabel">Decision Date</label>
                                                                                <asp:TextBox CssClass="form-control " runat="server" ID="txtDecisionDate" data-name="datepicker" MaxLength="20" autocomplete="none"></asp:TextBox>
                                                                                <i class="icon-calendar"></i>
                                                                                <span class="error-text"></span>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xl-6 col-lg-12 col-md-12">
                                                                        <div class="form-group iconField filled">
                                                                            <div class="date-input">
                                                                                <label class="inputLabel">Contract End</label>
                                                                                <asp:TextBox CssClass="form-control " runat="server" ID="txtContractEndDate" data-name="datepicker" MaxLength="20" autocomplete="none"></asp:TextBox>
                                                                                <i class="icon-calendar"></i>
                                                                            </div>
                                                                            <span class="error-text"></span>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>

                                                <div class="row">
                                                    <div class="col-md-12 form-fields">

                                                        <!-- competitors -->
                                                        <div class="form-group filled">
                                                            <label class="inputLabel">Competitors</label>
                                                            <asp:HiddenField ID="hdnCompetitors" runat="server" />
                                                            <asp:DropDownList runat="server" ID="ddlCompetitors" CssClass="form-control" Multiple="true"></asp:DropDownList>
                                                        </div>

                                                        <!-- commodities -->
                                                        <div class="form-group filled">
                                                            <label class="inputLabel">Commodities</label>
                                                            <asp:HiddenField ID="hdnCommodities" runat="server" />
                                                            <asp:DropDownList runat="server" ID="ddlCommodities" CssClass="form-control" Multiple="true"></asp:DropDownList>
                                                        </div>

                                                        <!-- campaign -->
                                                        <div class="form-group filled">
                                                            <label class="inputLabel">Campaigns</label>
                                                            <asp:HiddenField ID="hdnCampaigns" runat="server" />
                                                            <asp:DropDownList runat="server" ID="ddlCampaign" CssClass="form-control"  Multiple="true"></asp:DropDownList>
                                                        </div>

                                                    </div>
                                                </div>

                                                <!-- .row -->
                                            </div>

                                            <div class="col-lg-4 col-md-12 form-fields">
                                                <div class="row">
                                                    <div class="col-md-12">

                                                        <!-- sales stage -->
                                                        <div class="form-group filled">
                                                            <label class="inputLabel">Deal Stage</label>
                                                            <asp:DropDownList runat="server" ID="ddlSalesStage" CssClass="form-control"></asp:DropDownList>
                                                            <span class="error-text"></span>
                                                        </div>
                                                         
                                                        <!-- industry -->
                                                        <div class="form-group filled">
                                                            <label class="inputLabel">Industry</label>
                                                            <asp:HiddenField ID="hdnIndustry" runat="server" />
                                                            <asp:DropDownList runat="server" ID="ddlIndustry" CssClass="form-control" ></asp:DropDownList>
                                                            <span class="error-text"></span>
                                                        </div>

                                                    </div>
                                                </div>

                                                <!-- comments -->
                                                <div class="form-group filled">
                                                    <label class="inputLabel">Comments</label>
                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtComments" TextMode="MultiLine" Rows="10" MaxLength="100"></asp:TextBox>
                                                </div>

                                            </div>
                                        </div>
                                        <!-- .col-md-12 -->
                                    </div>

                                    <!-- .wrapper -->
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
                                                    <button type="button" class="primary-btn" id="btnSave">Save</button>
                                                    <button type="button" class="secondary-btn cancel-btn MR10" id="btnCancel">Cancel</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
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

    <script src="/_content/_js/polyfill-loader-17-oct-2019.js"></script>
    <script src="dealaddedit-08-apr-2020.js"></script>

</body>
</html>
