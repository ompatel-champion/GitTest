<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FfAdminPanel.aspx.cs" Inherits="Crm6.FfAdmin.FfAdminPanel" %>

<%@ Register Src="~/_usercontrols/footer.ascx" TagPrefix="uc1" TagName="footer" %>
<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>FF Admin</title>
    <link href="/_content/_css/bundle/bootstrap.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/animate.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/style-07-apr-2020.css " rel="stylesheet" />
</head>

<body class="skin-1">
    <form runat="server">
        <div id="wrapper">
            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

            <%-- navbar --%>
            <uc1:nav runat="server" ID="nav" />

            <div id="page-wrapper" class="gray-bg">
                <%-- header --%>


                <!--breadcrumb-->
                <div class="row wrapper border-bottom white-bg page-heading">
                    <div class="col-sm-10">
                        <h2>FF Admin</h2>
                        <ol class="breadcrumb">
                            <li>
                                <a href="/Admin/FfAdmin/FfAdminPanel.aspx">Admin</a>
                            </li>
                        </ol>
                    </div>
                    <div class="col-sm-2"></div>
                </div>

                <%-- content --%>
                <div class="wrapper wrapper-content fadeInRight row">

                    <div class="col-sm-4">
                        <a href="/Subscribers/SubscriberSetup/SubscriberSetup.aspx">
                            <div class="ibox ibox-content  PT30 PB30  text-center">
                                <h2>Subscriber Setup</h2>
                            </div>
                        </a>
                    </div>

                    <%--Update All Global Users--%>
                    <div class="col-sm-4">
                        <a href="/Admin/FfAdmin/UpdateGlobalUsers.aspx">
                            <div class="ibox ibox-content PT30 PB30 text-center">
                                <h2>Update Global Users</h2>
                            </div>
                        </a>
                    </div>
                     
                    <%--Import Super Office--%>
                    <div class="col-sm-4">
                        <a href="../Import/ImportSuperOffice.aspx">
                            <div class="ibox ibox-content PT30 PB30 text-center">
                                <h2>Import Super Office</h2>
                            </div>
                        </a>
                    </div>

                    <div class="col-sm-4">
                        <a href="/Admin/FfAdmin/UpdateData.aspx">
                            <div class="ibox ibox-content PT30 PB30 text-center">
                                <h2>Update Data</h2>
                            </div>
                        </a>
                    </div>

                    <%--Import HKG Data--%>
                    <div class="col-sm-4">
                        <a href="/Admin/FfAdmin/ImportHkg/ImportHkgData.aspx">
                            <div class="ibox ibox-content PT30 PB30 text-center">
                                <h2>Import HKG Data</h2>
                            </div>
                        </a>
                    </div>


                    <%--Visa Global Data--%>
                    <div class="col-sm-4">
                        <a href="/Admin/FfAdmin/ImportVisaGlobal/ImportVisaGlobal.aspx">
                            <div class="ibox ibox-content PT30 PB30 text-center">
                                <h2>Import Visa Global Data</h2>
                            </div>
                        </a>
                    </div>


                    <%--Updtae Spot Deals Revenue And Profits --%>
                    <div class="col-sm-4">
                        <asp:Button ID="btnFixSpotDeals" runat="server" OnClick="btnFixSpotDeals_Click" Text="Fix Spot Deals" />
                    </div>


                    <%--Updtae Spot Deals Revenue And Profits --%>
                    <div class="col-sm-4">
                        <asp:Button ID="btnFixDeals" runat="server" OnClick="btnFixDeals_Click" Text="Fix Deals " />
                    </div>

                    <div class="col-sm-4">
                        <a id="btnUpdateActivities" class="btn btn-success">Update Activities</a>
                    </div>


                    
                    <div class="col-sm-4">
                        <p>Create missing global companies, missing global company id in comanies</p>
                        <a id="btnFixCompanyDataIssue" class="btn btn-success">Fix Company Data Issues</a>
                    </div>

                </div>

                <%-- footer --%>
                <uc1:footer runat="server" ID="footer" />

            </div>
        </div>
    </form>
</body>

<!-- Custom and plug-in JavaScript -->
<script src="/_content/_js/jquery-ui-1.12.1.min.js"></script>

    <script>
        $(function () {
            $("#btnUpdateActivities").click(function () {
                 $.ajax({
                    type: "GET",
                    url: "/api/Activity/UpdateActivities" ,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        swal.close(); 
                    },
                    beforeSend: function () {
                        // TODO: new spinner
                        swal({ text: "", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: false });
                    },
                    error: function (request) { swal.close(); }
                }); 
            });


            $("#btnFixCompanyDataIssue").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/api/Company/FixCompanyData",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function () {
                        swal.close();
                    },
                    beforeSend: function () {
                        // TODO: new spinner
                        swal({ text: "", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: false });
                    },
                    error: function (request) { swal.close(); }
                });
            });
            
        });
    </script>

</html>
