<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserProfile.aspx.cs" Inherits="Crm6.Users.UserProfile" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Profile</title>
    <!--css custom-->
    <link href="userprofile-18-mar-2020.css" rel="stylesheet" />
    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body>

    <form runat="server" id="form">
        <div id="main">
            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0" Style="display: none;"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0" Style="display: none;"></asp:Label>
            <asp:Label CssClass="hide" ID="lblGuid" runat="server" Text="0" Style="display: none;"></asp:Label>

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
                                    <h1 class="page-title">
                                        <!--<asp:Label ID="lblHeaderUsername" runat="server"></asp:Label>-->
                                        Profile
                                    </h1>
                                </header>

                                <!-- user profile content wrapper -->
                                <div id="divUserProfile" class="wrapper">
                                    <div class="row no-gutters">
                                        
                                        <!-- profile picture column -->
                                        <div class="col-md-3 col-profile-pic">
                                            <div class="ibox ibox-main widget basic-card">
                                                <div class="ibox-content text-center">
                                                    <div class="imgProfile-wrapper">
                                                        <img id="imgProfile" runat="server" alt="Profile Image" class="img-circle" src="/_content/_img/no-pic.png?w=80&h=80&mode=crop"/>
                                                    </div>
                                                    <div class="user-info user-info-name"><h4><strong><asp:Label ID="lblUserFullName" runat="server"></asp:Label></strong></h4></div>
                                                    <div class="user-info user-info-job-title"><asp:Label ID="lblJobTitle" runat="server"></asp:Label></div>
                                                    <div class="user-info user-info-company"><asp:Label ID="lblLocationAndCompany" runat="server"></asp:Label></div>
                                                    <div class="btn-wrapper-upload-profile-image text-center">
                                                        <input id="fuProfileImage" name="fuProfileImage" type="file" class="hide" />
                                                        <a id="btnUploadProfileImage" href="javascript:void(0)"><i class="fa fa-camera m-r-xs text-button"></i><span class="language-entry">Upload Picture</span></a>
                                                    </div>
                                                    <div class="text-center">
                                                        <div class="form-btns">
                                                            <asp:LinkButton class="primary-btn btn-log-out language-entry" OnClick="btnLogout_Click" runat="server" Text="Log Out"></asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- tabs column -->
                                        <div class="col-md-9 col-profile-details">
                                            <div class="ibox ibox-main widget basic-card">
                                                <div class="ibox-content">
                                                    <div class="panel panel-tabs blank-panel d-flex flex-column">

                                                        <!-- tabs -->
                                                        <div class="panel-heading">
                                                            <div class="panel-options">
                                                                <ul class="nav nav-tabs">
                                                                    <li class="active"><a data-toggle="tab" href="#tab-basic" class="active language-entry">Details</a></li>
                                                                    <li class="tab-change-password"><a data-toggle="tab" href="#tab-change-password" class="language-entry">Password</a></li>
                                                                    <li class="tab-sync"><a data-toggle="tab" href="#tab-sync" class="language-entry">Sync</a></li>
                                                                </ul>
                                                            </div>
                                                        </div>

                                                        <!-- content -->
                                                        <div class="panel-body align-self-center d-flex flex-grow-1 mw-100">
                                                            <div class="tab-content align-self-center mw-100">

                                                                <!-- basic detail -->
                                                                <div id="tab-basic" class="tab-pane tab-main fade show active">
                                                                    <div class="ibox">
                                                                        <div class="ibox-content">
                                                                            <!-- inputs -->
                                                                            <div class="row no-gutters">
                                                                                <div class="col-sm-6 col-left">
                                                                                    <div class="col-wrapper">
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
                                                                                            <label class="inputLabel language-entry">Email Address</label>
                                                                                            <asp:TextBox ID="txtEmailAddress" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                                            <span class="error-text"></span>
                                                                                        </div>
                                                                                        <div class="form-group filled">
                                                                                            <label class="inputLabel language-entry">Job Title</label>
                                                                                            <asp:TextBox ID="txtJobTitle" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                                        </div>
                                                                                        <div class="form-group filled">
                                                                                            <label class="inputLabel language-entry">Phone</label>
                                                                                            <asp:TextBox ID="txtPhone" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                                        </div>
                                                                                        <div class="form-group filled">
                                                                                            <label class="inputLabel language-entry">Mobile</label>
                                                                                            <asp:TextBox ID="txtMobile" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                                        </div>
                                                                                        <div class="form-group filled">
                                                                                            <label class="inputLabel language-entry">Fax</label>
                                                                                            <asp:TextBox ID="txtFax" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                                        </div>
                                                                                        <div class="form-group filled">
                                                                                            <label class="inputLabel language-entry">Location</label>
                                                                                            <asp:DropDownList ID="ddlLocation" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                                            <span class="error-text"></span>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <div class="col-sm-6">
                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">District</label>
                                                                                        <asp:DropDownList ID="ddlDistrict" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                                     </div>
                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Country</label>
                                                                                        <asp:DropDownList ID="ddlCountry" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                                        <span class="error-text"></span>
                                                                                     </div>
                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Region</label>
                                                                                        <asp:DropDownList ID="ddlRegions" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                                        <span class="error-text"></span>
                                                                                     </div>
                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Currency</label>
                                                                                        <asp:DropDownList ID="ddlCurrency" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                                        <span class="error-text"></span>
                                                                                    </div>
                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Languages Spoken</label>
                                                                                        <asp:TextBox ID="txtLanguagesSpoken" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                                     </div>
                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">CRM Display Language</label>
                                                                                        <asp:DropDownList ID="ddlDisplayLanguage" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                                        <span class="error-text"></span>
                                                                                    </div>
                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Timezone</label>
                                                                                        <asp:DropDownList ID="ddlTimezone" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                                        <span class="error-text"></span>
                                                                                    </div>
                                                                                    <div class="form-group filled hide">
                                                                                        <label class="inputLabel language-entry">Date Format</label>
                                                                                        <asp:DropDownList ID="ddlDateFormat" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                                        <span class="error-text"></span>
                                                                                    </div>
                                                                                    <div class="form-group filled hide">
                                                                                        <label class="inputLabel language-entry">Report Date Format</label>
                                                                                        <asp:DropDownList ID="ddlReportDateFormat" CssClass="form-control portal-select2" runat="server"></asp:DropDownList>
                                                                                        <span class="error-text"></span>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <!-- buttons -->
                                                                            <div class="buttons row no-gutters">
                                                                                <div class="col-auto">
                                                                                    <div class="form-btns">
                                                                                        <button type="button" class="primary-btn language-entry" id="btnSave">Save</button>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <!-- change password -->
                                                                <div id="tab-change-password" class="tab-pane tab-main fade">
                                                                    <div class="ibox">
                                                                        <div class="ibox-content">
                                                                            <!-- inputs -->
                                                                            <div class="form-horizontal">
                                                                                <div class="form-group filled">
                                                                                    <label class="inputLabel language-entry">Current Password</label>
                                                                                    <asp:TextBox ID="txtCurrentPassword" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                                    <span class="error-text"></span>
                                                                                </div>
                                                                                <div class="form-group filled">
                                                                                    <label class="inputLabel language-entry">New Password</label>
                                                                                    <asp:TextBox ID="txtNewPassword" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                                    <span class="error-text"></span>
                                                                                </div>
                                                                                <div class="form-group filled">
                                                                                    <label class="inputLabel language-entry">Confirm Password</label>
                                                                                    <asp:TextBox ID="txtConfirmPassword" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                                                                    <span class="error-text"></span>
                                                                                </div>
                                                                            </div>
                                                                            <!-- buttons -->
                                                                            <div class="buttons row no-gutters">
                                                                                <div class="col"></div>
                                                                                <div class="col-auto">
                                                                                    <div class="form-btns">
                                                                                        <button type="button" class="primary-btn language-entry" id="btnUpdatePassword">Update Password</button>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <!-- sync -->
                                                                <div id="tab-sync" class="tab-pane fade">
                                                                    <div class="ibox">
                                                                        <div class="ibox-content">
                                                                            <div class="form-horizontal">
                                                                                <div class="row" id="divSyncOptions">

                                                                                    <%--Office 365--%>
                                                                                    <div class="col-sm-4 col-item sync-office365">
                                                                                        <div class="ibox-content text-center">
                                                                                            <div class="sync-exchange-image m-b-sm">
                                                                                                <img alt="image" class="W100P" src="/_content/_img/sync/office365.png">
                                                                                            </div>
                                                                                            <div class="text-center">
                                                                                                <a id="btnDisableO365Sync" class="primary-btn hide language-entry">Disable </a>
                                                                                                <a id="btnEditO365Sync" class=" primary-btn mt-2  hide language-entry">Edit </a>
                                                                                                <a id="btnActivateO365Sync" class=" primary-btn hide language-entry">Activate</a>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <%--Exchange--%>
                                                                                    <div class="col-sm-4 col-item sync-exchange">
                                                                                        <div class="ibox-content text-center">
                                                                                            <div class="sync-exchange-image m-b-sm">
                                                                                                <img alt="image" class="W100P" src="/_content/_img/sync/exchange.png">
                                                                                            </div>
                                                                                            <div class="text-center">
                                                                                                <a id="btnDisableExchangeSync" class="primary-btn hide language-entry">Disable </a>
                                                                                                <a id="btnEditExchangeSync" class=" primary-btn mt-2 hide language-entry">Edit </a>
                                                                                                <a id="btnActivateExchangeSync" class="primary-btn hide language-entry">Activate</a>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <%--Google--%>
                                                                                    <div class="col-sm-4 col-item sync-google">
                                                                                        <div class="ibox-content text-center">
                                                                                            <div class="sync-exchange-image m-b-sm">
                                                                                                <img alt="image" class="W100P" src="/_content/_img/sync/google.png">
                                                                                            </div>
                                                                                            <div class="text-center">
                                                                                                <a id="btnDisableGoogleSync" class=" primary-btn hide language-entry">Disable </a>
                                                                                                <asp:Button ID="btnActivateGoogleSync"
                                                                                                    OnClick="btnActivateGoogleSync_Click"
                                                                                                    UseSubmitBehavior="false" 
                                                                                                    runat="server"
                                                                                                    Text="Activate" CssClass="primary-btn hide" />
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                </div>

                                                                                <div class="clearfix MB40"></div>

                                                                                <!--Office 365-->
                                                                                <div id="divO365Settings" class="vendor hide">
                                                                                    <h3 class="MB10 language-entry">Configure Office 365 Sync</h3>
                                                                                    <p class="text-muted FontSize12 language-entry">Enter your credentials and click activate to enable Office 365 Sync</p>
                                                                                    <div class="hr-line-dashed"></div>
                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Email</label>
                                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtO365Email" MaxLength="100"></asp:TextBox>
                                                                                        <span class="error-text"></span>
                                                                                    </div>

                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Password</label>
                                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtO365Password" MaxLength="100"></asp:TextBox>
                                                                                        <span class="error-text"></span>
                                                                                    </div>
                                                                                    <div class="buttons row no-gutters">
                                                                                        <div class="col"></div>
                                                                                        <div class="col-auto">
                                                                                            <div class="form-btns">
                                                                                                <button type="button" class="primary-btn language-entry" id="btnSaveOffice365SyncSettings">Save & Activate</button>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>

                                                                                <!--Microsoft Exchange-->
                                                                                <div id="divExchangeSettings" class="vendor hide">
                                                                                    <h3 class="MB10 language-entry">Configure Exchange Sync</h3>
                                                                                    <p class="text-muted FontSize12 language-entry">Enter your credentials and click activate to enable Exchange Sync</p>
                                                                                    <div class="hr-line-dashed"></div>

                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Email</label>
                                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtExchangeEmail" MaxLength="100"></asp:TextBox>
                                                                                        <span class="error-text"></span>
                                                                                    </div>

                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">UserName</label>
                                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtExchangeUserName" MaxLength="100"></asp:TextBox>
                                                                                        <span class="error-text"></span>
                                                                                    </div>

                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Password</label>
                                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtExchangePassword" MaxLength="100"></asp:TextBox>
                                                                                        <span class="error-text"></span>
                                                                                    </div>
                                                                                    <div class="buttons row no-gutters">
                                                                                        <div class="col"></div>
                                                                                        <div class="col-auto">
                                                                                            <div class="form-btns">
                                                                                                <button type="button" class="primary-btn language-entry" id="btnSaveExchangeSyncSettings">Save & Activate</button>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>

                                                                                <!--Google-->
                                                                                <div id="divGoogleSettings" class="vendor hide">
                                                                                    <h3 class="MB10 language-entry">Configure Google Sync</h3>
                                                                                    <p class="text-muted FontSize12 language-entry">Enter your credentials and click activate to enable Google Sync</p>
                                                                                    <div class="hr-line-dashed"></div>

                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Email</label>
                                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtGoogleEmail" MaxLength="100"></asp:TextBox>
                                                                                        <span class="error-text"></span>
                                                                                    </div>

                                                                                    <div class="form-group filled">
                                                                                        <label class="inputLabel language-entry">Password</label>
                                                                                        <asp:TextBox CssClass="form-control" runat="server" ID="txtGooglePassword" MaxLength="100"></asp:TextBox>
                                                                                        <span class="error-text"></span>
                                                                                    </div>
                                                                                    <div class="buttons row no-gutters">
                                                                                        <div class="col"></div>
                                                                                        <div class="col-auto">
                                                                                            <div class="form-btns">
                                                                                                <button type="button" class="primary-btn language-entry" id="btnSaveGoogleSyncSettings">Save & Activate</button>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>

                                                                                <!--Sync Summary-->
                                                                                <div id="divSyncSummary" class="hide">
                                                                                    
                                                                                    <!--Header (title / button)-->
                                                                                    <div class="row no-gutters">
                                                                                        <div class="col d-flex flex-column justify-content-center">
                                                                                            <h3 class="m-0">Sync Log</h3>
                                                                                            <div class="form-group filled m-0">
                                                                                                <label class="control-label language-entry sync-log-label">
                                                                                                    Last Appointment Sync Date - 
                                                                                                    <span class="last-appointment-sync-date font-noraml">29 January 2018 @ 18:25</span>
                                                                                                </label>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="col-auto d-flex flex-column justify-content-center">
                                                                                            <a id="btnSync" href="javascript:void(0)"><i class="fa fa-arrow-circle-o-down m-r-xs text-button"></i><span class="language-entry">Sync Now</span></a>
                                                                                         </div>
                                                                                    </div>    

                                                                                    <!--content (history / errors)-->
                                                                                    <div class="ibox float-e-margins MT10">
                                                                                        <div class="ibox-content">

                                                                                            <!--tabs-->
                                                                                            <div class="panel blank-panel">
                                                                                                <div class="panel-heading P0" style="padding-top: 20px !important;">
                                                                                                    <div class="panel-options">
                                                                                                        <ul class="nav nav-tabs">
                                                                                                            <li class="active"><a data-toggle="tab" href="#tab-sync-history" class="active language-entry">History</a></li>
                                                                                                            <li class=""><a data-toggle="tab" href="#tab-sync-errors" class="language-entry">Errors</a></li>
                                                                                                        </ul>
                                                                                                    </div>
                                                                                                </div>

                                                                                                <!--content-->
                                                                                                <div class="sync-summary-content panel-body">
                                                                                                    <div class="tab-content">
                                                                                                        
                                                                                                        <!--history-->
                                                                                                        <div id="tab-sync-history" class="tab-pane fade show active">
                                                                                                            <div class="text-center loading language-entry">Loading Sync History...</div>
                                                                                                            <table class="table table-hover" id="tblSyncHistory">
                                                                                                                <tbody>
                                                                                                                </tbody>
                                                                                                            </table>
                                                                                                        </div>
                                                                                                        
                                                                                                         <!--errors-->
                                                                                                        <div id="tab-sync-errors" class="tab-pane fade">
                                                                                                            <table class="table table-hover" id="tblSyncErrors">
                                                                                                                <tbody>
                                                                                                                </tbody>
                                                                                                            </table>
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

<!-- js libraries -->
<script src="/_content/_js/jquery.metisMenu.js"></script>
<script src="/_content/_js/jquery.slimscroll.min.js"></script>
<script src="/_content/_js/jquery.ajaxfileupload.js"></script>
<!-- js custom -->
<script src="userprofile-05-apr-2020.js"></script>

</html>
