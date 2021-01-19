<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserAddEdit.aspx.cs" Inherits="Crm6.Users.UserAddEdit" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <title></title>

    <!--css custom-->
    <link href="useraddedit-03-mar-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="skin-1">
    <form runat="server" id="divUserEdit">

        <!-- #main start -->
        <div id="main">

            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblEditingUserId" runat="server" Text="0"></asp:Label>
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
                                    <h1 class="page-title">
                                        <asp:Label ID="lblHeaderUsername" runat="server" Text="New User"></asp:Label>
                                    </h1>
                                    <div onclick="window.history.back();" class="closeX" data-action="cancel-event"></div>
                                </header>

                                <!-- Users Page Content -->
                                <div class="wrapper">

                                    <!-- section (name, email, job title, etc...) -->
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="edit-wrapper section-content">
                                                <div class="edit-block">
                                                    <div class="row">
                                                        <div class="col-md-4 edit-box">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">First Name</label>
                                                                <asp:TextBox ID="txtFirstName" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                <span class="error-text"></span>
                                                            </div>
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Last Name</label>
                                                                <asp:TextBox ID="txtLastName" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                <span class="error-text"></span>
                                                            </div>
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Email </label>
                                                                <asp:TextBox ID="txtEmailAddress" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                <span class="error-text"></span>
                                                            </div>
                                                        </div>

                                                        <div class="col-md-4 edit-box">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Job Title</label>
                                                                <asp:TextBox ID="txtJobTitle" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                            </div>
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Billing Code</label>
                                                                <asp:TextBox ID="txtBillingCode" CssClass="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                            <div class="edit-field">
                                                                <div class="upload-box row no-gutters">
                                                                     <div class="upload-img col-auto mr-3">
                                                                        <div class="uploadBox">
                                                                            <input id="fuProfileImage" type="file" class="hide" />
                                                                            <img id="imgProfile" runat="server" src="/_content/_img/no-pic.png"/>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col d-flex flex-column justify-content-center">
                                                                        <a id="btnUploadProfileImage" class="language-entry" href="javascript:void(0)"><i class="fa fa-arrow-circle-o-up mr-1"></i><span class="language-entry">Upload Image</span></a>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-md-4 edit-box">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Business Phone</label>
                                                                <asp:TextBox ID="txtPhone" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                            </div>
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Mobile Phone</label>
                                                                <asp:TextBox ID="txtMobile" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                            </div>
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Fax</label>
                                                                <asp:TextBox ID="txtFax" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- section (location, address, city, etc...) -->
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="edit-wrapper section-content">
                                                <div class="edit-block">
                                                    <div class="row">
                                                        <div class="col-md-4 edit-box">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Location</label>
                                                                <asp:DropDownList ID="ddlLocation" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                <span class="error-text"></span>
                                                            </div>

                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Address</label>
                                                                <asp:TextBox ID="txtAddress" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-md-4 edit-box">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">City</label>
                                                                <asp:TextBox ID="txtCity" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                            </div>
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">State/Province</label>
                                                                <asp:TextBox ID="txtStateProvince" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-md-4 edit-box">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Postcode</label>
                                                                <asp:TextBox ID="txtPostcode" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                            </div>
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Country</label>
                                                                <asp:DropDownList ID="ddlCountry" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                <span class="error-text"></span>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>

                                    <!-- section (languages, date format, currency, etc...) -->
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="edit-wrapper section-content">
                                                <div class="edit-block">
                                                    <div class="row">
                                                        <div class="col-md-4 edit-box">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Display Language</label>
                                                                <asp:DropDownList ID="ddlDisplayLanguage" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                <span class="error-text"></span>
                                                            </div>
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Spoken Languages</label>
                                                                <asp:TextBox ID="txtSpokenLanguage" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-md-4 edit-box">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Timezone</label>
                                                                <asp:DropDownList ID="ddlTimezone" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                <span class="error-text"></span>
                                                            </div>
                                                            <div class="form-group hide">
                                                                <label class="inputLabel language-entry">Report Date Format</label>
                                                                <asp:DropDownList ID="ddlReportDateFormat" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                <span class="error-text"></span>
                                                            </div>
                                                        </div>

                                                        <div class="col-md-4 edit-box">
                                                            <div class="form-group filled">
                                                                <label class="inputLabel language-entry">Currency</label>
                                                                <asp:DropDownList ID="ddlCurrency" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                <span class="error-text"></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>

                                    <!-- section (password, user roles, etc...) -->
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="edit-wrapper section-content">
                                                <div class="ibox">
                                                    <div class="ibox-content MT20 MB50">
                                                        <div class="form-horizontal">

                                                            <!-- login details -->
                                                            <div class="login-details">

                                                                <!-- password and user roles -->
                                                                <div class="row no-gutters">
                                                                    <div class="col-2">
                                                                        <div class="form-group form-group-login-enabled">
                                                                            <asp:CheckBox runat="server" CssClass="i-checks" ID="chkLoginEnabled" />
                                                                            <span class="language-entry">Login Enabled</span>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-4">
                                                                        <div class="form-group form-group-password filled">
                                                                            <label class="inputLabel language-entry">Password</label>
                                                                            <asp:TextBox ID="txtPassword" CssClass="form-control W200" runat="server" Text=""></asp:TextBox>
                                                                            <span class="error-text"></span>
                                                                        </div>
                                                                    </div>
                                                                    <div class="clearfix"></div>
                                                                     
                                                                </div>
                                                                <div class="form-group form-group-user-roles MT10">
                                                                    <label class="control-label language-entry">User Roles</label>
                                                                    <ul class="user-roles list-unstyled">
                                                                        <asp:DropDownList ID="ddlUserRoles" runat="server">
                                                                        </asp:DropDownList>
                                                                    </ul>
                                                                    <span class="error-text"></span>
                                                                </div>

                                                                <!-- manager sales group (hidden until 'sales manager' is checked) -->
                                                                <div class="form-group form-group-sales-reps hide" id="divManagerSalesReps">
                                                                    <asp:Label ID="lblSelectedManagerSalesReps" CssClass="hide" runat="server" Text=""></asp:Label>
                                                                    <label class="control-label language-entry">Sales Reps</label>
                                                                    <div>
                                                                        <div class="row" id="multiselect-container">
                                                                            <div class="col-md-5">
                                                                                <select name="from" id="multiselect" class="form-control" size="8" multiple="multiple">
                                                                                </select>
                                                                            </div>
                                                                            <div class="col-md-2 PT20">
                                                                                <button type="button" id="multiselect_rightSelected" class="btn btn-primary">></button>
                                                                                <button type="button" id="multiselect_rightAll" class="btn btn-primary">>></button>
                                                                                <button type="button" id="multiselect_leftSelected" class="btn btn-primary"><</button>
                                                                                <button type="button" id="multiselect_leftAll" class="btn btn-primary"><<</button>
                                                                            </div>
                                                                            <div class="col-md-5">
                                                                                <select name="to" id="multiselect_to" class="form-control" size="8" multiple="multiple">
                                                                                </select>
                                                                            </div>
                                                                        </div>
                                                                        <span class="error-text"></span>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>

                                                        <div class="clearfix"></div>
                                                    </div>

                                                    <!-- buttons -->
                                                    <div class="buttons row no-gutters footerBox">
                                                        <div class="col"></div>
                                                        <div class="col-auto">
                                                            <div class="form-btns">
                                                                <button type="button" class="secondary-btn cancel-btn language-entry" id="btnCancel">Cancel</button>
                                                            </div>
                                                        </div>
                                                        <div class="col-auto">
                                                            <div class="form-btns">
                                                                <button type="button" class="primary-btn language-entry" id="btnSave">Save</button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

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
        <!-- #main end -->

    </form>
</body>

<!--js libraries-->
<script src="/_content/_js/jquery.ajaxfileupload.js"></script>
<script src="/_content/_js/dual-list-box.js"></script>
<script src="/_content/_js/multiselect.js"></script>

<!--js custom-->
<script src="useraddedit-04-apr-2020.js"></script>

</html>
