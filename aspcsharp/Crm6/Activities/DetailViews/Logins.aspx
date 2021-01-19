<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logins.aspx.cs" Inherits="Crm6.Activities.DetailViews.Logins" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Logins</title>
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

        <div class="ibox ibox-content P0 MB0">
            <div class="col-md-12 PR0 PL0">
                <div class="list-table">
                    <table class="table table-bordered MB0" id="tblLogins">
                        <tbody> 
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </form>

    <script src="/_content/_js/jquery.metisMenu.js"></script>
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>

    <script>
        $(function () {
            var qr = getQueryString("qr");
            var $tblLogins = $("#tblLogins");
            var $tbody = $tblLogins.find("tbody");
            $.ajax({
                type: "POST",
                url: "/api/Activity/GetActivityChartLogins",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: qr,
                success: function (logins) {
                    $tbody.html("");
                    removeSpinner();
                    var i = 0;
                    $.each(logins, function (i, ele) {
                        i += 1;
                        var $tr = $("<tr/>")
                        $tr.append($("<td/>", { "class": "text-center W100", "html": i }));
                        $tr.append($("<td/>", { "html":  moment(ele.UserActivityTimestamp).format("ddd, DD MMMM, YYYY @ HH:mm") }));
                        $tbody.append($tr);
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
