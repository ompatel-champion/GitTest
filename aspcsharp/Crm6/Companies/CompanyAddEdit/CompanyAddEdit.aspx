<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyAddEdit.aspx.cs" Inherits="Crm6.Companies.CompanyAddEdit.CompanyAddEdit" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <title></title>

    <!--css custom-->
    <link href="companyaddedit-13-feb-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="Company-add addEditPage">

    <form runat="server" id="divCompanySetup">

        <!-- #main start -->
        <div id="main">

            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCompanyId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCompanySubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblGuid" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblIsAdmin" runat="server" Text="0"></asp:Label>

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
                                                <asp:Label ID="lblBreadcrumbHeader" Text="Add/Edit Company" runat="server" CssClass=" m-t-sm "></asp:Label>
                                            </h1>
                                        <div onclick="window.history.back();" class="closeX" data-action="cancel-event"></div>
                                </header>
                                <!-- .top-header -->

                                <!-- Page Content -->
                                <div class="wrapper section-content addEditContent">

                                    <div id="CompanyAdd" class="CompanyAdd">
                                        
                                        <!-- First Row (input fields) -->
                                        <div class="row br-fields">
                                            <div class="col-md-4">
                                                <div class="form-group filled">
                                                    <label class="inputLabel language-entry">Company Name</label>
                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtCompanyName" MaxLength="100"></asp:TextBox>
                                                    <span class="error-text"></span>
                                                </div>
                                                <div class="form-group filled ">
                                                    <label class="inputLabel language-entry">Division</label>
                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtDivision" MaxLength="100"></asp:TextBox>
                                                    <span class="error-text"></span>
                                                </div>
                                                <div class="form-group filled ">
                                                    <label class="inputLabel language-entry">Address</label>
                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtAddress" TextMode="MultiLine" Rows="3" MaxLength="300"></asp:TextBox>
                                                    <span class="error-text"></span>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-8 col-left-box">
                                                        <div class="form-group filled ">
                                                            <label class="inputLabel language-entry">City</label>
                                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtCity" MaxLength="100"></asp:TextBox>
                                                            <span class="error-text"></span>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-4 col-right-box">
                                                        <div class="form-group filled ">
                                                            <label class="inputLabel language-entry">State/Province</label>
                                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtStateProvince" MaxLength="100"></asp:TextBox>
                                                            <span class="error-text"></span>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-8 col-left-box">
                                                        <div class="form-group filled ">
                                                            <label class="inputLabel language-entry">Country</label>
                                                            <asp:DropDownList runat="server" ID="ddlCountry" CssClass="form-control"></asp:DropDownList>
                                                               <span class="error-text"></span>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-4 col-right-box">
                                                        <div class="form-group filled ">
                                                            <label class="inputLabel language-entry">Postal Code</label>
                                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtPostalCode" axLength="100"></asp:TextBox>
                                                            <span class="error-text"></span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-8 form-fields">
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <div class="form-group filled ">
                                                            <label class="inputLabel language-entry">Company Type</label>
                                                            <asp:DropDownList runat="server" ID="ddlCompanyType" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                        <div class="form-group filled">
                                                            <label class="inputLabel language-entry">Owner</label>
                                                            <asp:DropDownList runat="server" ID="ddlOwner" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                        <div class="form-group   filled">
                                                            <label class="inputLabel language-entry">Industry</label>
                                                            <asp:DropDownList runat="server" ID="ddlIndustry" CssClass="form-control"></asp:DropDownList>
                                                            <span class="error-text"></span>
                                                        </div>
                                                        <div class="form-group filled ">
                                                            <label class="inputLabel language-entry">Company Code</label>
                                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtCompanyCode" MaxLength="100"></asp:TextBox>
                                                            <span class="error-text"></span>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-md-6 col-left-box">
                                                                <div class="form-group filled">
                                                                    <label class="inputLabel language-entry">Phone</label>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtPhone" MaxLength="100"></asp:TextBox>
                                                                    <span class="error-text"></span>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-6 col-right-box">
                                                                <div class="form-group filled ">
                                                                    <label class="inputLabel">Fax</label>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtFax" axLength="100"></asp:TextBox>
                                                                    <span class="error-text"></span>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="form-group filled">
                                                            <label class="inputLabel language-entry">Source</label>
                                                            <asp:DropDownList runat="server" ID="ddlSource" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-6">
                                                        <div class="form-group filled">
                                                            <label class="inputLabel language-entry">Website</label>
                                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtWebsite" MaxLength="100"></asp:TextBox>
                                                            <span class="error-text"></span>
                                                        </div>
                                                        <div class="form-group filled notes">
                                                            <label class="inputLabel language-entry">Comments</label>
                                                            <asp:TextBox CssClass="form-control" runat="server" ID="Notes" TextMode="MultiLine" Rows="3" MaxLength="300"></asp:TextBox>
                                                        </div>
                                                        <div class="form-group" id="divActiveCustomerContainer">
                                                            <asp:CheckBox runat="server" ID="chkActive" CssClass="i-checks"></asp:CheckBox>
                                                            <label for="chkActive" class="ML5 language-entry">Active</label>
                                                            <asp:CheckBox runat="server" ID="chkCustomer" CssClass="i-checks ML10"></asp:CheckBox>
                                                            <label for="chkCustomer" class="ML5 language-entry">Customer</label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                       
                                        <!-- Second Row (campaigns / upload logo image) -->
                                        <div class="row br-fields">
                                            <div class="col-md-8">
                                                <div class="form-group filled">
                                                    <label class="inputLabel language-entry">Campaigns</label>
                                                    <asp:DropDownList runat="server" ID="ddlCampaign" CssClass="form-control" multiple="multiple"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-4">
                                                <div class="form-group">
                                                    <div class="row no-gutters">
                                                         <div class="col-md-auto mr-2">
                                                            <div class="uploadBox">
                                                                <input id="fuCompanyLogo" name="fuProfileImage" type="file" class="hide" />
                                                                <img id="imgCompanyLogo" runat="server" src="/_content/_img/image-placeholder.png"/>
                                                            </div>
                                                        </div>
                                                        <div class="col-md d-flex flex-column justify-content-center">
                                                            <a id="btnUploadCompanyLogo" class="text-button" href="javascript:void(0)"><i class="fa fa-arrow-circle-o-up mr-1"></i><span class="language-entry">Upload Logo</span></a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- .wrapper -->
                                    <div class="footer-action footerBox">
                                        <div class="row align-items-end">
                                            <div class="col-md-3 col-sm-5 col-12 order-sm-1 order-3">
                                                <div class="form-btns">
                                                    <button type="button" class="delete-btn hide text-danger secondary-btn language-entry" id="btnDelete">Delete</button>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-sm-1 col-12 order-sm-2 order-2">
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-12 order-sm-3 order-1">
                                                <div class="form-btns">
                                                    <button type="button" class="primary-btn" id="btnSave">Save</button>
                                                    <button type="button" class="secondary-btn cancel-btn MR10 language-entry" id="btnCancel">Cancel</button>
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
    <script src="companyaddedit-26-mar-2020.js"></script>
</body>
</html>
