<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContactAddEdit.aspx.cs" Inherits="Crm6.Contacts.ContactAddEdit" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <title></title>

    <!--css custom-->
    <link href="contactaddedit-15-mar-2020.css" rel="stylesheet" />

    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body class="addEditPage">

    <form runat="server" id="divContactSetup">

        <!-- #main start -->
        <div id="main">

            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblContactId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCompanyId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblGlobalCompanyId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblGuid" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblIsQuickAdd" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblLanguageCode" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblContactSubscriberId" runat="server" Text="0"></asp:Label>

            <!-- navbar mobile -->
            <uc1:navmobile runat="server" />

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
                                        <asp:Label ID="lblBreadcrumbHeader" Text="New Contact" runat="server" CssClass=" m-t-sm "></asp:Label>
                                    </h1>
                                    <div onclick="window.history.back();" class="closeX" data-action="cancel-event"></div>
                                </header>
                                <!-- .top-header -->

                                <!-- Page Content -->
                                <div class="section-content addEditContent">

                                    <div id="ContactAdd" class="ContactAdd">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="row">
                                                    <div class="col-md-4 form-fields br-fields prof-fields">
                                                        <div class="row">
                                                            <div class="col-sm-auto col-left-box">
                                                                <div class="form-group avatarBox">
                                                                    <img id="img_uploaded_profile_image" src="/_content/_img/no-pic.png" runat="server" class="img-thumbnail" />
                                                                    <input class="file-upload" type="file" accept="image/*" />
                                                                    <asp:TextBox ID="txtProfileImageLink" runat="server" CssClass="form-control hide"></asp:TextBox>
                                                                    <input id="fuProfileImage" name="fuProfileImage" type="file" class="hide" />
                                                                    <div id="div_uploaded_profile_image" class="uploaded_profile_image m-t-sm hide" runat="server" style="display: inline-block; text-align: center">
                                                                        <div class="m-t-xs">
                                                                            <a id="btn_delete_logo" runat="server" class="text-danger"><span class="language-entry">x</span></a>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm col-mid-box">
                                                                <div class="form-group filled">
                                                                    <label class="inputLabel language-entry">First Name</label>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtFirstName" MaxLength="100" autocomplete="none"></asp:TextBox>
                                                                    <span class="error-text"></span>
                                                                </div>
                                                            </div>

                                                            <div class="col-sm col-right-box">
                                                                <div class="form-group filled">
                                                                    <label class="inputLabel language-entry">Last Name</label>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtLastName" placeholder="" autocomplete="none" MaxLength="100"></asp:TextBox>
                                                                    <span class="error-text"></span>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="form-group filled">
                                                            <label class="inputLabel language-entry">Email</label>
                                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtEmail" placeholder="" autocomplete="none" MaxLength="100"></asp:TextBox>
                                                            <span class="error-text"></span>
                                                        </div>

                                                        <div class="form-group filled">
                                                            <label class="inputLabel language-entry">Address</label>
                                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtAddress" placeholder="" autocomplete="none" TextMode="MultiLine" Rows="3" MaxLength="100"></asp:TextBox>
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

                                                    <div class="col-md-8">
                                                        <div class="row">
                                                            <div class="col-md-6 br-fields form-fields">

                                                                <div class="form-group filled">
                                                                    <label class="inputLabel language-entry">Company</label>
                                                                    <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-control"></asp:DropDownList>
                                                                    <span class="error-text"></span>
                                                                </div>

                                                                <div class="form-group filled">
                                                                    <label class="inputLabel language-entry">Job Title</label>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtJobTitle" placeholder="" autocomplete="none" MaxLength="100"></asp:TextBox>
                                                                    <span class="error-text"></span>
                                                                </div>

                                                                <div class="form-group filled chkActive hide">
                                                                    <asp:CheckBox runat="server" ID="chkCompanyAddress"></asp:CheckBox>
                                                                    <label for="chkCompanyAddress" class="language-entry">Use Company Address</label>
                                                                </div>

                                                                <div class="row">
                                                                    <div class="col-md-6 col-left-box">
                                                                        <div class="form-group filled">
                                                                            <label class="inputLabel language-entry">Business Phone</label>
                                                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtBusinessPhone" autocomplete="none" placeholder="" MaxLength="100"></asp:TextBox>
                                                                            <span class="error-text"></span>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-md-6 col-right-box">
                                                                        <div class="form-group filled">
                                                                            <label class="inputLabel language-entry">Mobile Phone</label>
                                                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtMobile" autocomplete="none" placeholder="" MaxLength="100"></asp:TextBox>
                                                                            <span class="error-text"></span>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="form-group filled">
                                                                    <label class="inputLabel language-entry">Contact Type</label>
                                                                    <asp:DropDownList runat="server" ID="ddlContactType" CssClass="form-control"></asp:DropDownList>
                                                                    <span class="error-text"></span>
                                                                </div>

                                                                <div class="form-group filled">
                                                                    <label class="inputLabel language-entry">Interests</label>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="txtHobbies" placeholder="" autocomplete="none" MaxLength="100"></asp:TextBox>
                                                                    <span class="error-text"></span>
                                                                </div>
                                                            </div>

                                                            <div class="col-md-6 br-fields form-fields">
                                                                
                                                                <div class="form-group filled">
                                                                    <label class="inputLabel language-entry language-entry">Owner</label>
                                                                    <asp:DropDownList runat="server" ID="ddlOwner" CssClass="form-control"></asp:DropDownList>
                                                                </div>

                                                                <div class="form-group filled">
                                                                    <label class="inputLabel language-entry">Previous Employers</label>
                                                                    <asp:HiddenField ID="hdnPreviousEmployers" runat="server" />
                                                                    <asp:DropDownList runat="server" ID="ddlPreviousEmployers" CssClass="form-control" Multiple="true"></asp:DropDownList> 
                                                                    <span class="error-text"></span>
                                                                </div>

                                                                <div class="row">
                                                                    <div class="col-md-6 col-left-box">
                                                                        <div class="form-group filled">
                                                                            <label class="inputLabel language-entry">Birthday Month</label>
                                                                            <asp:DropDownList runat="server" ID="ddlBirthdayMonth" CssClass="form-control"></asp:DropDownList>
                                                                            <span class="error-text"></span>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-md-6 col-right-box">
                                                                        <div class="form-group filled">
                                                                            <label class="inputLabel language-entry">Birthday Day</label>
                                                                            <asp:TextBox runat="server" ID="txtBirthdayDay" CssClass="form-control"></asp:TextBox>
                                                                            <span class="error-text"></span>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="row checklist">
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group filled chkActive">
                                                                            <asp:CheckBox runat="server" ID="OktoEmail"></asp:CheckBox>
                                                                            <label for="OktoEmail" class="language-entry">Ok to Email</label>
                                                                        </div>
                                                                        <div class="form-group filled chkActive">
                                                                            <asp:CheckBox runat="server" ID="Married"></asp:CheckBox>
                                                                            <label for="Married" class="language-entry">Married</label>
                                                                        </div>
                                                                        <div class="form-group filled chkActive">
                                                                            <asp:CheckBox runat="server" ID="HasChildren"></asp:CheckBox>
                                                                            <label for="HasChildren" class="language-entry">Has Children</label>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group filled chkActive">
                                                                            <asp:CheckBox runat="server" ID="OktoCall"></asp:CheckBox>
                                                                            <label for="OktoCall" class="language-entry">Ok to Call</label>
                                                                        </div>
                                                                        <div class="form-group filled chkActive">
                                                                            <asp:CheckBox runat="server" ID="HolidayCard"></asp:CheckBox>
                                                                            <label for="HolidayCard" class="language-entry">Holiday Card</label>
                                                                        </div>
                                                                        <div class="form-group filled chkActive">
                                                                            <asp:CheckBox runat="server" ID="FormerEmployee"></asp:CheckBox>
                                                                            <label for="FormerEmployee" class="language-entry">Former Employee</label>
                                                                        </div>
                                                                    </div>

                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <div class="form-group filled">
                                                                    <label class="inputLabel language-entry">Comments</label>
                                                                    <asp:TextBox CssClass="form-control" runat="server" ID="Notes" placeholder="" autocomplete="none" TextMode="MultiLine" Rows="3" MaxLength="100"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>

                                                    </div>


                                                </div>
                                                <!-- .row -->

                                            </div>
                                            <!-- .col-md-8 -->
                                        </div>
                                        <!-- .col-md-12 -->
                                    </div>
                                    <!-- .row -->

                                    <div class="footer-action footerBox">
                                        <div class="row align-items-end">
                                            <div class="col-md-4 col-sm-5 col-12">
                                                <div class="form-btns">
                                                    <button type="button" class="delete-btn hide text-danger secondary-btn language-entry" id="btnDelete">Delete</button>
                                                </div>
                                            </div>
                                            <div class="col-md-4 col-sm-1 col-12">
                                            </div>
                                            <div class="col-md-4 col-sm-6 col-12">
                                                <div class="form-btns">
                                                    <button type="button" class="primary-btn language-entry" id="btnSave">Save</button>
                                                    <button type="button" class="secondary-btn cancel-btn MR10 language-entry" id="btnCancel">Cancel</button>
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

    <script src="/_content/_js/jquery.validate.min.js"></script>
    <script src="/_content/_js/jquery.ajaxfileupload.js"></script>
    <script src="/_content/_js/polyfill-loader-17-oct-2019.js"></script>

    <!-- js custom -->
    <script src="contactaddedit-05-apr-2020.js"></script>

</body>
</html>
