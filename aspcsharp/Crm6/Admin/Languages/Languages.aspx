<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Languages.aspx.cs" Inherits="Crm6.Admin.Languages.Languages" %>

<%@ Register Src="~/_usercontrols/nav.ascx" TagPrefix="uc1" TagName="nav" %>
<%@ Register Src="~/_usercontrols/nav-mobile.ascx" TagPrefix="uc1" TagName="navmobile" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Languages</title>
    <link href="languages-01-mar-2020.css" rel="stylesheet" />
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
</head>

<body>
    <form runat="server">

        <!--#main start-->
        <div id="main">

            <!--hidden values-->
            <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
            <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>

            <!--navbar mobile-->
            <uc1:navmobile runat="server" />

            <div class="container-fluid">
                <div class="row">

                    <!--.page-container-->
                    <div class="page-container">

                        <!--.page-content-->
                        <div class="page-content">

                            <!--navbar desktop-->
                            <uc1:nav runat="server" ID="navSidebar" />

                            <!-- #content -->
                            <div id="content" class="animated fadeIn">

                                <!--top-header-->
                                <header class="top-header">

                                    <h1 class="page-title">Languages</h1>

                                    <!--menu tabs -->
                                    <div class="row">
                                        <div class="col-md-12 border-tabs">
                                            <div id="divLanguagesTabHeader" data-id="#divLanguagesTab" class="btab active language-entry">Languages</div>
                                            <div id="divLanguagePhrasesTabHeader" data-id="#divLanguagePhrasesTab" class="btab language-entry">Phrases</div>
                                            <div id="divTranslationsTabHeader" data-id="#divTranslationsTab" class="btab language-entry">Translations</div>
                                        </div>

                                        <!-- .mobile-panel-nav -->
                                        <div class="mobile-panel-nav">
                                            <div class="dropdown-wrapper panel-dropdown">
                                                <div class="ae-dropdown dropdown">
                                                    <div class="ae-select">
                                                        <span class="ae-select-content"></span>
                                                        <span class="drop-icon-down"><i class="icon-angle-down"></i></span>
                                                        <span class="drop-icon-up"><i class="icon-angle-up"></i></span>
                                                    </div>
                                                    <ul id="cl-tabs" class="dropdown-nav ae-hide nav nav-tabs">
                                                        <li class="selected"><a data-id="#divLanguagesTab"><span class="language-entry active">Languages</span></a></li>
                                                        <li class=""><a data-id="#divPhrasesTab"><span class="language-entry">Phrases</span></a></li>
                                                        <li class=""><a data-id="#divTranslationsTab"><span class="language-entry">Translations</span></a></li>
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- .mobile-panel-nav -->
                                    </div>

                                </header>
                                <!--top-header-->

                                <div class="wrapper">

                                    <%--languages--%>
                                    <div id="divLanguagesTab" class="btab-content active" runat="server">

                                        <div class="col-md-12">
                                            <div id="divLanguages" class="tab-list list-table">
                                                <!-- new-language -->
                                                <div class="ibox">
                                                    <div class="ibox-content">
                                                        <div class="new-language">
                                                            <div class="row">
                                                                <div class="col-md-4">
                                                                    <asp:TextBox ID="txtLanguageCode" CssClass="form-control" placeholder='Code' runat="server"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <asp:TextBox ID="txtLanguageName" CssClass="form-control" placeholder='Language Name' runat="server"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <asp:LinkButton runat="server" class="language-entry primary-btn" OnClick="btnAddLanguage_Click" ID="btnAddLanguage">
                                                                            <i class="icon-plus" style="color: white; font-weight: bold"></i>
                                                                            Language
                                                                    </asp:LinkButton>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="clearfix"></div>
                                                    </div>
                                                </div>

                                                <!-- languages list -->
                                                <div id="divLanguageTable">
                                                    <table class="table table-hover" id="tblLanguages">
                                                        <thead>
                                                            <tr>
                                                                <th style="width: 350px">Code 
                                                                </th>
                                                                <th>Language Name 
                                                                </th>
                                                                <th style="text-align: center; width: 100px"></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater runat="server" ID="rptLanguages">
                                                                <ItemTemplate>
                                                                    <tr data-id='<%# Eval("LanguageId") %>'
                                                                        data-current-language-name="<%# Eval("LanguageName") %>"
                                                                        data-current-language-code="<%# Eval("LanguageCode") %>">
                                                                        <td>
                                                                            <asp:TextBox
                                                                                CssClass="form-control language-code"
                                                                                Text='<%# Eval("LanguageCode") %>'
                                                                                runat="server">
                                                                            </asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox
                                                                                CssClass="form-control language-name"
                                                                                Text='<%# Eval("LanguageName") %>'
                                                                                runat="server">
                                                                            </asp:TextBox>
                                                                        </td>
                                                                        <td style="text-align: center;">
                                                                            <a class="delete-item" data-action="delete">
                                                                                <i class="icon-Delete"></i>
                                                                            </a>

                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>
                                                    </table>
                                                </div>

                                            </div>
                                        </div>
                                    </div>

                                    <%--language phrases--%>
                                    <div id="divLanguagePhrasesTab" class="btab-content" runat="server">
                                        <div class="col-md-12">
                                            <div id="divLanguagePhrases" class="tab-list list-table">
                                                <div class="ibox new-phrase">
                                                    <div class="ibox-content">
                                                        <div class="row">
                                                            <div class="col-md-4">
                                                                <asp:TextBox ID="txtNewPhrase" CssClass="form-control" placeholder='Phrase' runat="server"></asp:TextBox>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <a class="language-entry primary-btn" id="btnAddPhrase">
                                                                    <i class="icon-plus" style="color: white; font-weight: bold"></i>
                                                                    Phrase
                                                                </a>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <input type="text" style="z-index: 0;" class="form-control " id="txtKeywordPhrase" placeholder="Search" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>


                                                <!-- language phrases list -->
                                                <div id="items" style="padding-top: 20px!important;">
                                                    <table class="table table-hover" id="tblLanguagePhrases">
                                                        <tbody>
                                                        </tbody>
                                                    </table>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <%--language translations--%>
                                <div id="divTranslationsTab" class="btab-content" runat="server">
                                    <div class="col-md-12">
                                        <div id="divTranslations" class="tab-list list-table">

                                            <div id="divLanguageTranslations" class="ibox">
                                                <div class="ibox-content">
                                                    <div class="row">
                                                        <div class="col-md-4">
                                                            <input type="text" class="form-control pull-left" id="txtTranslationsKeyword" placeholder="Search" />
                                                        </div>
                                                        <div class="col-md-4">
                                                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlLanguages">
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlStatus">
                                                                <asp:ListItem Text="New" Selected="True" Value="New"></asp:ListItem>
                                                                <asp:ListItem Text="Not Verified" Value="Not Verified"></asp:ListItem>
                                                                <asp:ListItem Text="Verified" Value="Verified"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div>
                                                <div id="divLanguageTranslationsList">
                                                    <%-- language translations list --%>
                                                    <div class="language-table ibox">
                                                        <div class="ibox-content">
                                                            <table class="table table-hover" id="tblLanguageTranslations">
                                                                <thead>
                                                                    <tr>
                                                                        <th>Phrase</th>
                                                                        <th style="width: 600px;">Translation</th>
                                                                        <th style="width: 130px;" class="TAC"></th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody id="tblLanguagesBody">
                                                                </tbody>
                                                            </table>
                                                        </div>
                                                    </div>

                                                    <!-- paging -->
                                                    <div style="text-align: center;">
                                                        <ul class="pagination hide">
                                                        </ul>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <!--spinner-->
                                <div id="divSpinner" class="hide">
                                    <div class="ajax-modal">
                                        <div class="ibox ibox-content text-center ajax-modal-txt">
                                            <div class="spinner"></div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                            <!-- .wrapper -->
                        </div>
                        <!-- #content end -->
                    </div>
                    <!-- .page-content -->
                </div>
                <!-- .page-container -->
            </div>
            <!--.row-->
        </div>
        <!--.container-fluid-->


    </form>
</body>

<script src="/_content/_js/polyfill-loader-17-oct-2019.js"></script>
<script src="languages-01-apr-2020.js"></script>

</html>
