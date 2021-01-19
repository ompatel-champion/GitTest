<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notes.aspx.cs" Inherits="Crm6.Activities.DetailViews.Notes" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Notes</title>
    <link href="/_content/_css/bundle/bootstrap.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/animate.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/style-07-apr-2020.css " rel="stylesheet" />

    <style>
        body {
            background-color: #ffffff;
        }

        #intercom-container {
            display: none !important;
        }
    </style>

</head>

<body class="white-bg">
    <form runat="server">
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

        <div class="ibox ibox-content P0">
            <div class="col-md-12  ">
                <div class="inspinia-timeline P10" id="divNotes">
                    <asp:Repeater ID="rptNotes" runat="server">
                        <ItemTemplate>
                            <div class="feed-element PB0">
                                <a class="pull-left"><i class="fa fa-file-o text-primary"></i></a>
                                <div class="media-body">
                                    <small class="FontSize10 pull-right MB5 text-navy">Created on  <%#  Eval("CreatedDateStr")  %>  
                                    </small>
                                    <strong class="MB5"><%#  Eval("Note.NoteContent")%></strong>
                                    <div class="clearfix MB5"></div>
                                    <p class="FontSize10 MB5">Deal: <%#  Eval("DealName")%></p>
                                    <p class="FontSize10 MB5">Company: <%#  Eval("CompanyName")%></p>
                                    <p class="FontSize10 MB5">Contact: <%#  Eval("ContactName")%></p>
                                </div>
                                <div class="hr-line-dashed MT10 MB10"></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>

    </form>

    <script src="/_content/_js/jquery.metisMenu.js"></script>
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>

    <script>
        $(function () {
            var qr = getQueryString("qr");
            var $divNotes = $("#divNotes");
            $.ajax({
                type: "POST",
                url: "/api/Activity/GetActivityChartNotes",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: qr,
                success: function (notes) {
                    removeSpinner();
                    $.each(notes, function (i, ele) {

                        var $feedElement = $("<div/>", { "class": "feed-element PB0" });
                        $feedElement.append($("<a/>", { "class": "pull-left", "html": "<i class=\"fa fa-file-o text-primary\"></i>" }));

                        var $mediaBody = $("<div/>", { "class": "media-body" });
                        $feedElement.append($mediaBody);
                        $mediaBody.append($("<small/>", { "class": "FontSize10 pull-right MB5 text-navy", "html": ele.CreatedDateStr }));
                        $mediaBody.append($("<strong class=\"MB5\">" + ele.Note.NoteContent + "</strong>"));
                        $mediaBody.append($("<div class =\"clearfix MB5\"></div>"));
                        if (ele.DealName && ele.DealName !== "") {
                            $mediaBody.append($(" <p class=\"FontSize10 MB5\">Deal: " + ele.DealName + "</p>"));
                        }

                        if (ele.CompanyName && ele.CompanyName !== "") {
                            $mediaBody.append($("<p class=\"FontSize10 MB5\">Company: " + ele.CompanyName + "</p>"));
                        }
                        if (ele.ContactName && ele.ContactName !== "") {
                            $mediaBody.append($("<p class=\"FontSize10 MB5\">Contact: " + ele.ContactName + "</p>"));
                        }
                        $feedElement.append($("<div class=\"hr-line-dashed MT10 MB10\"></div>"));
                        $divNotes.append($feedElement);
                    });
                },
                beforeSend: function () {
                    addSpinner();
                },
                error: function () { 
                }
            });
        });
    </script>

</body>
</html>
