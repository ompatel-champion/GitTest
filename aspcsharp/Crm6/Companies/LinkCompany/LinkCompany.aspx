<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LinkCompany.aspx.cs" Inherits="Crm6.Companies.LinkCompany.LinkCompany" %>

<!DOCTYPE html>

<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Link Company</title>
    <link href="/_content/_css/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" />
    <link href="/_content/_css/bootstrap-tagsinput.css" rel="stylesheet" />
    <link href="linkcompany.css" rel="stylesheet" />
</head>

<body>
    <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblCompanyId" runat="server" Text="0"></asp:Label>

    <form runat="server" id="divLinkCompanySetup" class="page-content hide">

        <div class="ibox ibox-content">

            <div class="col-md-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-4 control-label language-entry">Company</label>
                        <div class="col-sm-8">
                            <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-control"></asp:DropDownList>
                            <span class="error-text"></span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4 control-label language-entry">Link Type</label>
                        <div class="col-sm-8">
                            <asp:DropDownList runat="server" ID="ddlLinkTypes" CssClass="form-control"></asp:DropDownList>
                            <span class="error-text"></span>
                        </div>
                    </div>
                </div>
            </div>

            <div class="clearfix"></div>

        </div>

    </form>

    <script src="/_content/_js/bootstrap3-typeahead.js"></script>
    <script src="/_content/_js/bootstrap-tagsinput.js"></script>
    <script src="/_content/_js/jquery.ajaxfileupload.js"></script>

    <script src="linkcompany-17102312234.js"></script>

</body>
</html>
