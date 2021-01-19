<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tasks.aspx.cs" Inherits="Crm6.Activities.DetailViews.Tasks" %>

<!DOCTYPE html>
<html>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Tasks</title>
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
                <div class="inspinia-timeline P10" id="divTasks">
                    <asp:Repeater ID="rptTasks" runat="server">
                        <ItemTemplate>
                            <div class="feed-element PB0">
                                <a class="pull-left"><i class="fa fa-tasks text-primary"></i></a>
                                <div class="media-body">
                                    <small class="text-muted pull-right">Due on <%# Eval("DueDate")  %>  
                                    </small>
                                    <strong class="MB5"><%#  Eval("Task.TaskName")%></strong>
                                    <div class ="clearfix MB5"></div>
                                    <p class=" MB5 FontSize10">Deal: <%#  Eval("DealName")%></p>
                                    <p class="MB5 FontSize10">Companies: <%#  Eval("CompaniesStr")%></p>
                                    <p class="MB5 FontSize10">Contacts: <%#  Eval("ContactsStr")%></p>
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
            var $divTasks = $("#divTasks");
            $.ajax({
                type: "POST",
                url: "/api/Activity/GetActivityChartTasks",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: qr,
                success: function (events) { 
                    removeSpinner();
                    $.each(events, function (i, ele) {

                        var $feedElement = $("<div/>", { "class": "feed-element PB0" });
                        $feedElement.append($("<a/>", { "class": "pull-left", "html": "<i class=\"fa fa-tasks text-primary\"></i>" }));

                        var $mediaBody = $("<div/>", { "class": "media-body" });
                        $feedElement.append($mediaBody);
                        $mediaBody.append($("<small/>", { "class": "text-muted pull-right", "html": "Due on " + ele.DueDate }));
                        $mediaBody.append($("<strong class=\"MB5\">" + ele.Task.TaskName + "</strong>"));
                        $mediaBody.append($("<div class =\"clearfix MB5\"></div>"));
                        if (ele.DealName && ele.DealName !== "") {
                            $mediaBody.append($(" <p class=\"FontSize10 MB5\">Deal: " + ele.DealName + "</p>"));
                        }
                         
                        if (ele.CompaniesStr && ele.CompaniesStr !== "") {
                            $mediaBody.append($("<p class=\"FontSize10 MB5\">Companies: " + ele.CompaniesStr + "</p>"));
                        }
                        if (ele.ContactsStr && ele.ContactsStr !== "") {
                            $mediaBody.append($("<p class=\"FontSize10 MB5\">Contacts: " + ele.ContactsStr + "</p>"));
                        }
                        $feedElement.append($("<div class=\"hr-line-dashed MT10 MB10\"></div>"));
                        $divTasks.append($feedElement);  
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

