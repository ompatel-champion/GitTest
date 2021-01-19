<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportDeals.aspx.cs" Inherits="Crm6.Import.ImportDeals" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/footer.ascx" TagPrefix="uc1" TagName="footer" %>


<!DOCTYPE html>
<html>

<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="0" />
    <title>Import Deals Excel File</title>
    <link href="/_content/_css/bundle/bootstrap.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/animate.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/style-07-apr-2020.css " rel="stylesheet" />
    <link href="/_content/_css/bundle/sweetalert.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/select2.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/select2-bootstrap.min.css" rel="stylesheet" />
</head>

<body>

    <form runat="server" id="form">

        <div id="wrapper">

            <%-- hidden values --%>
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblGuid" runat="server" Text="0"></asp:Label>

            <%-- navbar --%>
            <uc1:nav runat="server" ID="nav" />

            <div id="page-wrapper" class="gray-bg">
                <%-- header --%>


                <!--breadcrumb-->
                <div class="  wrapper border-bottom white-bg page-heading">
                    <div class="col-sm-10">
                        <h2>
                            <asp:Label ID="lblHeaderUsername" runat="server"></asp:Label></h2>
                        <ol class="breadcrumb">
                            <li>
                                <a href="/Dashboards/SalesRepDashboard/SalesRepDashboard.aspx" class="language-entry">Home</a>
                            </li>
                            <li class="active">
                                <a href="/Admin/AdminPanel.aspx" class="language-entry">Admin</a>
                            </li>
                            <li class="active">
                                <strong class="language-entry">Import Deals Excel File</strong>
                            </li>
                        </ol>
                    </div>
                    <div class="col-sm-2"></div>
                </div>

                <%-- content --%>
                <div class="wrapper wrapper-content animated fadeInRight  ">

                    <div class="col-md-3 PR5 PL5">
                        <div class="ibox float-e-margins">
                            <div>
                                <div class="text-center form-group m-t-md">
                                    <asp:DropDownList runat="server" ID="ddlSalesTeam"></asp:DropDownList>
                                </div>
                                <div class="text-center form-group m-t-md">
                                    <input id="fuHkgExcel" name="fuHkgExcel" type="file" class="hide" />
                                    <a id="btnUploadHkgExcel" class="btn btn-primary btn-sm" href="javascript:void(0)">
                                        <i class="fa fa-camera m-r-xs"></i>
                                        <span class="language-entry">Upload Deals Excel File</span>
                                    </a>
                                    <asp:Label runat="server" CssClass="hide" ID="lblUploadedFileUri" Text=""></asp:Label>
                                    <div id="div_uploaded_file" class="uploaded_file hide m-t-sm" runat="server" style="display: inline-block; text-align: center">
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="hr-line-dashed"></div>
                                    <a id="btnProcessFile" class="btn btn-primary btn-sm hide" href="javascript:void(0)">Process File</a>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <%-- footer --%>
                <uc1:footer runat="server" ID="footer" />
            </div>
        </div>

    </form>
</body>
<script src="/_content/_js/jquery.metisMenu.js"></script>
<script src="/_content/_js/jquery.slimscroll.min.js"></script>
<script src="/_content/_js/jquery.ajaxfileupload.js"></script>
<script src="importdeals.js"></script>

</html>

<script>
    // clear storage
    function clearlocalStorage() {
        'use strict';
        localStorage.setItem('language_data', '');
        localStorage.setItem('language_code', '');
    }
</script>
