<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesTeamAccessByEmail.aspx.cs" Inherits="Crm6.Companies.SalesTeamAccessByEmail.SalesTeamAccessByEmail" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>


<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Request Company Access</title>
</head>

<body>
    <form runat="server" id="divCompanySetup" class="page-content">

        <div id="wrapper" class="page-content hide">
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblCompanyId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblGuid" runat="server" Text="0"></asp:Label>

            <div class="main-wrapper container-fluid">
                <%-- header --%>
                 <div class="row">
                    <!-- .page-container -->
                    <div class="page-container">

                        <!-- .page-content -->
                        <div class="page-content">

                            <%-- navbar --%>
                            <uc1:nav runat="server" ID="nav" />

                            <div id="content" class="animated fadeIn">
                              
                                <%-- content --%>
                                <div id="" class="wrapper section-content">
                                    <div class="row">
                                        <div class="col-sm-12 success-box" id="divSuccessBox" runat="server" visible="false">
                                            <div class="ibox">
                                                <div class="ibox-content text-center PT30 PB30">
                                                    <i class="fa fa-4x text-navy fa-check-circle-o"></i>
                                                    <h2 class="MB20 MT20" id="successMessage" runat="server"></h2>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-sm-12 success-box" id="divErrorBox" runat="server" visible="false">
                                            <div class="ibox">
                                                <div class="ibox-content text-center PT30 PB30">
                                                    <i class="fa fa-4x text-navy fa-check-circle-o"></i>
                                                    <h2 class="MB20 MT20" runat="server">Error adding this user the sales team. </h2>
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

</html>
