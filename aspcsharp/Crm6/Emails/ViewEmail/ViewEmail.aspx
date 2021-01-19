<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewEmail.aspx.cs" Inherits="Crm6.Emails.ViewEmail" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
     <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>View Email</title>
    <link href="/_content/_css/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" /> 
    <link href="viewemail.css" rel="stylesheet" />
</head>

<body>

    <form runat="server">
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblEmailId" runat="server" Text="0"></asp:Label>

        <div id="divEmailSetup" class="ibox ibox-content">
            <div class="col-md-12">
                <div class="mail-box">

                <div class="mail-body">
                    <div class="form-horizontal"  >
                        <div class="form-group"><label class="col-sm-2 control-label">To:</label>
                            <div class="col-sm-10"><input type="text" class="form-control" value="alex.smith@corporat.com"></div>
                        </div>
                        <div class="form-group"><label class="col-sm-2 control-label">Subject:</label>
                            <div class="col-sm-10"><input type="text" class="form-control" value=""></div>
                        </div>
                    </div>
                </div>
                    <div class="mail-text h-200">

                    <div class="summernote">
                        <h3>Hello Jonathan! </h3>
                        dummy text of the printing and typesetting industry. <strong>Lorem Ipsum has been the industry's</strong> standard dummy text ever since the 1500s,
                        when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic
                        typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with
                        <br/>
                        <br/>
                    </div>

                    <div class="clearfix"></div>
                </div>

                <div class="mail-body text-right tooltip-demo">
                    <a href="mailbox.html" class="btn btn-sm btn-primary" data-toggle="tooltip" data-placement="top" title="Send"><i class="fa fa-reply"></i> Send</a>
                    <a href="mailbox.html" class="btn btn-white btn-sm" data-toggle="tooltip" data-placement="top" title="Discard email"><i class="fa fa-times"></i> Discard</a>
                    <a href="mailbox.html" class="btn btn-white btn-sm" data-toggle="tooltip" data-placement="top" title="Move to draft folder"><i class="fa fa-pencil"></i> Draft</a>
                </div>
                <div class="clearfix"></div>
                </div>     </div>
            <div class="clearfix"></div>
        </div>
    </form>

    <script src="/_content/_js/jquery.metisMenu.js"></script>
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>

    <%-- jquery UI --%>
    <script src="/_content/_js/jquery-ui-1.12.1.min.js"></script>

    <!-- type ahead -->
    <script src="/_content/_js/bootstrap3-typeahead.js"></script>
    <script src="/_content/_js/bootstrap-tagsinput.js"></script> 
    <script src="/_content/_js/summernote/summernote.min.js"></script>
    <script src="viewemail.js?q=" <%= Guid.NewGuid().ToString() %>></script>

</body>
</html>
