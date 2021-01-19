<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubscriberSetup.aspx.cs" Inherits="Crm6.Subscribers.SubscriberSetup" %> 

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Setup New Subscriber</title>
    <style>
        body {
            background-color: #ffffff;
        }
    </style>
</head>

<body>

    <form runat="server">

        <div id="divSubscriberSetup" class="ibox ibox-content">
            <div class="col-md-12">
                <div id="divErrors" runat="server" class="alert alert-danger" visible="false">
                    <p>Please correct the following errors.</p>
                    <ul class="MT10">
                        <asp:PlaceHolder ID="plhldrErrors" runat="server"></asp:PlaceHolder>
                    </ul>
                </div>

                <div id="divSuccess" runat="server" class="alert alert-success" visible="false">
                    Subscriber created successfully.
                </div>
            </div>

            <div class="col-md-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-3 control-label required">Company</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtCompanyName" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">Sub-Domain</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtSubDomain" MaxLength="30"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">First Name</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtFirstName" MaxLength="30"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">Last Name</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtLastName" MaxLength="30"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">Email</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtEmail" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">Password</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtPassword" TextMode="Password" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">Address</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtAddress" MaxLength="500"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label class="col-sm-3 control-label required">City</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtCity" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">State / Province</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtStateProvince" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label class="col-sm-3 control-label required">PostalCode</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtPostalCode" MaxLength="20"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">Country</label>
                        <div class="col-sm-9">
                            <asp:DropDownList CssClass="form-control" runat="server" ID="ddlCountry"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">Data Center</label>
                        <div class="col-sm-9">
                            <asp:DropDownList CssClass="form-control" runat="server" ID="ddlDataCenter"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">Time Zone</label>
                        <div class="col-sm-9">
                            <asp:DropDownList CssClass="form-control" runat="server" ID="ddlTimeZone"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label required">Language</label>
                        <div class="col-sm-9">
                            <asp:DropDownList CssClass="form-control" runat="server" ID="ddlLanguage"></asp:DropDownList>
                        </div>
                    </div>

                </div>
            </div>
            <div class="clearfix"></div>

            <!-- Cancel / Update Buttons -->
            <asp:Button runat="server" ID="btnSetupSubscriber" Text="Setup Subscriber" OnClick="btnSetupSubscriber_Click" />

        </div>

    </form>

    <script src="setupsubscriber.js?q=" <%= Guid.NewGuid().ToString() %> type="text/javascript"></script>

</body>
</html>
