<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MarkAsWonLost.aspx.cs" Inherits="Crm6.Deals.MarkAsWonLost" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mark as Won/Lost</title>
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
        <asp:Label CssClass="hide" ID="lblDealId" runat="server" Text="0"></asp:Label> 
        <asp:Label CssClass="hide" ID="lblIsWon" runat="server" Text="0"></asp:Label>
        <div class="ibox ibox-content">
            <div class="col-md-12">
                <div class="row P0" style="height: 100% !important;">

                    <div class="form-horizontal MT30">
                        <div class="form-group">
                            <div class="col-md-2"><label>Reason</label></div>
                            <div class="col-md-10">
                                <asp:DropDownList runat="server" CssClass="form-control MT10" ID="ddlWonLostReason"> 
                                </asp:DropDownList>
                            </div>
                        </div> 
                    </div>
                    <asp:Button runat="server" ID="btnSave" OnClick="btnSave_Click" CssClass="hide" />
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </form>

     <script src="markaswonlost.js"></script>

</body>
</html>
