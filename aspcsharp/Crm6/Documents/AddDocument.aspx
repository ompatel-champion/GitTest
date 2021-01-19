<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddDocument.aspx.cs" Inherits="Crm6.Document.AddDocument" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Add/Edit Document</title>
    <style>
        body {
            background-color: #ffffff;
        }
    </style>
</head>

<body>
    <form runat="server">
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblDocumentId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblDealId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblContactId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblCompanyId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblGlobalCompanyId" runat="server" Text="0"></asp:Label>
        
        <div class="ibox ibox-content">

            <div class="col-md-12">
                <div class="row P0" style="height: 100% !important;">

                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="col-md-2 language-entry">Title</div>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" CssClass="form-control" ID="txtDocumentTitle"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-2 language-entry">Description</div>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" CssClass="form-control" TextMode="MultiLine" ID="txtDocumentDescription"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-2"></div>
                            <div class="col-md-10">
                                <a class="btn btn-primary btn-sm language-entry" id="btnUploadDocument" style="margin-top: -3px;">Upload Document</a>
                                <input id="fuUploadDocument" name="fuUploadDocument" type="file" class="hide" />
                                <div style="border: 1px solid #f2f2f2; padding: 9px;" class="ML10 uploaded-document hide">
                                    <span class="hide doc-path" runat="server" id="docPath"></span>
                                    <a href="javascipt:void(0)" class="doc-name" runat="server" id="docLink"></a>
                                    <a href="javascript:void(0);" data-action="delete" title="Delete Document" class="m-l-sm">
                                        <i class="fa fa-trash-o text-danger"></i>
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="clearfix"></div>
                </div>
            </div>
        </div>

    </form>

    <script src="adddocument.js?q=" <%= Guid.NewGuid().ToString() %>></script>

</body>
</html>
