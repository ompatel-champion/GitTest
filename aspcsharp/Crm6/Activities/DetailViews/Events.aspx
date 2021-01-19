<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Events.aspx.cs" Inherits="Crm6.Activities.DetailViews.Events" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Events</title>
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
            <div class="">
                <div class="inspinia-timeline" id="divEvents"> 
                </div>
            </div>
        </div>

    </form>

    <script src="/_content/_js/jquery.metisMenu.js"></script>
    <script src="/_content/_js/jquery.slimscroll.min.js"></script>

    <script>

        $(function () {
            var qr = getQueryString("qr");
            var $divEvents = $("#divEvents");

            $.ajax({
                type: "POST",
                url: "/api/Activity/GetActivityChartEvents",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: qr,
                success: function (events) { 
                    removeSpinner();
                    $.each(events, function (i, ele) {

                        var $feedElement = $("<div/>", { "class": "feed-element PB0" });
                        $feedElement.append($("<a/>", { "class": "pull-left", "html": "<i class=\"fa fa-calendar\"></i>" }));

                        var $mediaBody = $("<div/>", { "class": "media-body" });
                        $feedElement.append($mediaBody);
                        $mediaBody.append($("<small/>", { "class": "text-muted pull-right event-startdate", "html": moment(ele.StartDateTimeStr, 'DD-MMM-YY HH:mm').format("ddd, DD MMMM, YYYY HH:mm")}));
                        $mediaBody.append($("<strong class=\"MB5\">" + ele.CalendarEvent.Subject + "</strong>"));
                        $mediaBody.append($("<div class =\"clearfix MB5\"></div>"));
                        if (ele.DealName && ele.DealName !== "") {
                            $mediaBody.append($(" <p class=\"FontSize10 MB5\"><i style='color:#1e92d2;margin-right:6px' class=\"icon-deals\"></i> " + ele.DealName + "</p>"));
                        }
                         
                        if (ele.CompaniesStr && ele.CompaniesStr !== "") {
                            $mediaBody.append($("<p class=\"FontSize10 MB5\">Companies: " + ele.CompaniesStr + "</p>"));
                        }
                        if (ele.ContactsStr && ele.ContactsStr !== "") {
                            $mediaBody.append($("<p class=\"FontSize10 MB5\">Contacts: " + ele.ContactsStr + "</p>"));
                        }
                        $feedElement.append($("<div class=\"hr-line-dashed MT10 MB10\"></div>"));
                        $divEvents.append($feedElement);  
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
