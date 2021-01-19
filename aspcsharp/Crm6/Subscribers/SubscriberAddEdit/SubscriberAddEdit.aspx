<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubscriberAddEdit.aspx.cs" Inherits="Crm6.Subscribers.SubscriberAddEdit" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title></title>
    <link href="/_content/_css/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" />
    <link href="/_content/_css/bootstrap-tagsinput.css" rel="stylesheet" />
    <link href="subscriberaddedit.css" rel="stylesheet" />
</head>

<body>
    <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblLocationId" runat="server" Text="0"></asp:Label>
    <asp:Label CssClass="hide" ID="lblGuid" runat="server" Text="0"></asp:Label>

    <form runat="server" id="divSubscriber">

        <div class="ibox ibox-content">

            <div class="col-md-6">
                <div class="form-horizontal">

                    <div class="form-group">
                        <label class="col-sm-3 control-label  language-entry">Company Name</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtCompanyName" placeholder="" MaxLength="100"></asp:TextBox>
                            <span class="error-text"></span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label  language-entry">Contact Name</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtContactName" placeholder="" MaxLength="100"></asp:TextBox>
                            <span class="error-text"></span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label  language-entry">Email</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtEmail" placeholder="" MaxLength="100"></asp:TextBox>
                            <span class="error-text"></span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Address</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtAddress" placeholder="" MaxLength="200"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">City</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtCity" placeholder="" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">State / Province</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtStateProvince" placeholder="" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Postal Code</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtPostalCode" placeholder="" MaxLength="20"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Country</label>
                        <div class="col-sm-9">
                            <asp:DropDownList runat="server" ID="ddlCountry" CssClass="form-control"></asp:DropDownList>
                        </div>
                    </div>

                </div>
            </div>

            <div class="col-md-6">
                <div class="form-horizontal">

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Phone</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtPhone" placeholder="" MaxLength="20"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Comments</label>
                        <div class="col-sm-9">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtComments" placeholder="" MaxLength="4000"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label language-entry">Company Logo</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="txtLocationImageLink" runat="server" CssClass="form-control hide"></asp:TextBox>
                            <a class="change-location-pic btn btn-success language-entry" href="javascript:void(0)">Choose</a>

                            <input id="fuLocationImage" name="fuLocationImage" type="file" class="hide" />

                            <div class="clearfix"></div>
                            <div id="div_uploaded_location_image" class="uploaded_location_image hide m-t-sm" runat="server" style="display: inline-block; text-align: center">
                                <img id="img_uploaded_location_image" runat="server" class="W150 img-thumbnail" />
                                <div class="m-t-xs">
                                    <a id="btn_delete_image" runat="server" class="text-danger"><i class="fa fa-times m-r-xs"></i><span class="language-entry">Delete image</span></a>
                                </div>
                            </div>
                            <div class="clearfix"></div>
                        </div>
                    </div>

                </div>
            </div>

            <div class="clearfix"></div>
        </div>

    </form>

    <script src="/_content/_js/jquery.metisMenu.js"></script>
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>
    <script src="/_content/_js/jquery-ui-1.12.1.min.js"></script>
    <script src="/_content/_js/bootstrap3-typeahead.js"></script>
    <script src="/_content/_js/bootstrap-tagsinput.js"></script>
    <script src="/_content/_js/jquery.ajaxfileupload.js"></script>

    <script src="subscriberaddedit-18-mar-2020.js"></script>

</body>
</html>
