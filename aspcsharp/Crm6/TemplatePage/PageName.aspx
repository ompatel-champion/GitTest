<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageName.aspx.cs" Inherits="Crm6.Companies.PageName" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>PageName</title>

    <!-- css bundle -->
    <link href="/_content/_css/bundle.min.css" rel="stylesheet" />

    <!-- css custom -->
    <link href="pagename.css" rel="stylesheet" />
</head>
<body>

    <form runat="server">

        <!-- #main start -->
        <div id="main">

            <!-- navbar mobile -->
            <uc1:navmobile runat="server" />

            <div class="main-wrapper container-fluid">

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
                                <header class="top-header">

                                    <div class="row">
                                        <div class="col-md-6 col-sm-12 pageInfo">
                                            <h1 class="page-title">PageName</h1>
                                        </div>
                                    </div>
                                </header>
                                <!-- .top-header -->

                                <!-- Page Content -->
                                <div class="wrapper">
                                    <div class="row">
                                        <div class="col-md-12">
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
            <!--.container-fluid-->
        </div>
        <!-- #main end -->

    </form>
    <!-- form -->

    <!-- js bundle -->
    <script src="/_content/_js/bundle.min.js"></script>

    <!-- js custom -->
    <script src="pagename.js"></script>

</body>
</html>
