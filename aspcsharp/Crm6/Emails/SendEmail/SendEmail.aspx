
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendEmail.aspx.cs" Inherits="Crm6.Emails.SendEmail" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Send Email</title>
    <link href="/_content/_css/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" /> 
    <link href="/_content/_css/bootstrap-tagsinput.css" rel="stylesheet" />
    <link href="/_content/_css/summernote/summernote.css" rel="stylesheet">
    <link href="/_content/_css/summernote/summernote-bs3.css" rel="stylesheet">
    <link href="sendemail.css" rel="stylesheet" />
</head>

<body>
    <form runat="server">
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblDealId" runat="server" Text="0"></asp:Label>

        <div id="divEmailSetup" class="ibox ibox-content">
            <div class="col-md-12">
                <div class="mail-box">

                    <div class="mail-body">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="col-sm-2 control-label">To:</label>
                                <div class="col-sm-10">
                                    <asp:TextBox ID="txtTagUser" runat="server" CssClass="form-control" placeholder="type user name..."></asp:TextBox>
                                    <asp:TextBox ID="txtTagUserId" runat="server" CssClass="form-control hide"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label">Subject:</label>
                                <div class="col-sm-10">
                                    <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="mail-text">
                        <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Height="300" mLength="500"></asp:TextBox>
                        <div class="clearfix"></div>
                    </div>
                    <div class="clearfix"></div>
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
    <!-- tinymce -->
    <script src="//cdn.tinymce.com/4/tinymce.min.js"></script>
    <link href="/_content/_css/tinymce/skin.min.css" rel="stylesheet" />
    <script src="/_content/_js/tinymce/tinymcefy.js"></script>

    <script src="/_content/_js/summernote/summernote.min.js"></script>


    <script src="sendemail.js?q=" <%= Guid.NewGuid().ToString() %>></script>

</body>
</html>
