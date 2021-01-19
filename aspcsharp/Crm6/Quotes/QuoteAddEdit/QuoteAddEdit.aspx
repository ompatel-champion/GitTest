<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuoteAddEdit.aspx.cs" Inherits="Crm6.Admin.Quotes.QuoteAddEdit" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Add/Edit Quote</title>
    <!-- css libraries -->
    <link href="../../_content/_css/bootstrap-tagsinput.css" rel="stylesheet" />
    <!-- css custom -->
    <link href="quoteaddedit.css" rel="stylesheet" />
</head>

<body>
    <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblDealId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblDealSubscriberId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblCompanyId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblQuoteId" runat="server" Text="0"></asp:Label>

     <!-- navbar mobile -->
    <uc1:navmobile runat="server" />

    <form runat="server" id="divQuoteSetup">

        <div class="ibox ibox-content page-content hide MB0 PB0" id="dealInner">
            <div class="col-md-6">
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Company</label>
                        <div class="col-sm-9">
                            <asp:DropDownList runat="server" ID="ddlCompany" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Deal</label>
                        <div class="col-sm-9">
                            <asp:DropDownList runat="server" ID="ddlDeal" CssClass="form-control"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Sales Owner</label>
                        <div class="col-sm-9">
                            <asp:DropDownList runat="server" ID="ddlSalesOwner" CssClass="form-control"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Branch</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtBranch" placeholder="" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Terms</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtTerms" placeholder="" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Code</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtCode" placeholder="" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Route</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtRoute" placeholder="" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Submitted</label>
                        <div class="col-sm-9">
                            <div class="input-group">
                                <span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                <asp:TextBox CssClass="form-control " runat="server" ID="txtSubmitted" data-name="datepicker" MaxLength="50"></asp:TextBox>
                            </div>
                            <span class="error-text"></span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Valid Thru</label>
                        <div class="col-sm-9">
                            <div class="input-group">
                                <span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                <asp:TextBox CssClass="form-control " runat="server" ID="txtValidThru" data-name="datepicker" MaxLength="50"></asp:TextBox>
                            </div>
                            <span class="error-text"></span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Pieces</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtPieces" placeholder="" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-12">
            <div class="ibox-content MB30">
                <div class="form-horizontal">
                    <a class="btn btn-default" href="/Quotes/Quotes.aspx">Back</a>
                    <a id="btnSave" class="btn btn-success pull-right">Save</a>
                </div>
                <div class="clearfix"></div>
            </div>
        </div>

    </form>

    <script src="/_content/_js/bootstrap-datepicker.js"></script>
    <script src="/_content/_js/datepair.js"></script>
    <script src="/_content/_js/bundle/jquery.timepicker.js"></script>

    <!-- js custom -->
    <script src="quoteaddedit.js"></script>
    
</body>
</html>
