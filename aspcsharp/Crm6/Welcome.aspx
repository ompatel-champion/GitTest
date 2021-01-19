<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="Crm6.Welcome" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Welcome Aboard</title>


    <!-- Morris Charts -->
    <link href="/_content/_css/morris-0.4.3.min.css" rel="stylesheet">
    <!-- Gritter -->
    <link href="/_content/_css/jquery.gritter.css" rel="stylesheet">
    <link rel="stylesheet" href="https://www.amcharts.com/lib/3/plugins/export/export.css" type="text/css" media="all" />
    <link href="/_content/_css/select2.min.css" rel="stylesheet" />
    <link href="/_content/_css/select2-custom.css" rel="stylesheet" />

    <style>
        .video-container {
            max-width: 685px;
            padding: 20px;
            background: #f2f2f2;
            margin: 0 auto;
        }

            .video-container iframe {
                width: 100%;
                height: 365px;
            }
    </style>
</head>

<body class="skin-1">
    <form runat="server">
        <div id="wrapper">
            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCountryId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCountryCode" runat="server" Text=""></asp:Label>

            <!-- navbar mobile -->
            <uc1:navmobile runat="server" />

            <div id="page-wrapper" class="gray-bg">

                <!-- navbar desktop -->
                <uc1:nav runat="server" ID="navSidebar" />

                <!--country name-->
                <div class="row wrapper border-bottom white-bg page-heading">
                    <div class="col-sm-9">
                        <h2>Welcome Aboard</h2>
                    </div>
                    <div class="col-sm-3">
                    </div>
                </div>

                <%-- content --%>
                <div class="wrapper wrapper-content animated fadeInRight row PB40">

                    <div class="row">
                        <div class="col-lg-12 ">
                            <div class="ibox float-e-margins">
                                <div class="ibox-content">
                                    <div class="video-container text-center">
                                        <iframe
                                            src="https://support.firstfreight.com/videos/getting-started-video-slide-show " />
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

<script src="/_content/_js/jquery.metisMenu.js"></script>
<script src="/_content/_js/jquery.slimscroll.min.js"></script>

</html>

