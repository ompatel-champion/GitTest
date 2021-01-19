<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskNextLastActivity.ascx.cs" Inherits="Crm6._usercontrols.TaskNextLastActivity.TaskNextLastActivity" %>

<!-- css custom -->
<link href="/_usercontrols/TaskNextLastActivity/tasknextlastactivity-apr-07-2020.css" rel="stylesheet" />

<!--next activity - left column -->
<%--note: this card is hidden when there is no data--%>
<div id="nextCard" class="ibox widget basic-card next-card" runat="server">
    <div class="ibox-title">
        <i class="icon-double-arrow"></i>
        <h3 class="card-title language-entry">Next</h3>
    </div>
    <div class="ibox-content">
        <asp:Label runat="server" ID="lblNextActivityTaskType" class="card-label"></asp:Label>
        <div class="inner-wrp">
            <div>
                <a id="linkNextActivity" runat="server" href="javascript:void(0);" class="hover-link link-next-activity">
                    <asp:Label runat="server" ID="lblUpcomingEventDescription"></asp:Label>
                </a>
            </div>
            <div class="info-row clearfix">
                <div class="time-icon">
                    <div class="card-date">
                        <asp:Label runat="server" ID="lblUpcomingEventTime"></asp:Label>
                    </div>
                    <div class="card-slabel card-time">
                        <asp:Label runat="server" ID="lblUpcomingEventStartEndTime"></asp:Label>
                    </div>
                </div>
            </div>
            <div id="wrpLocationAndType" class="info-row clearfix" runat="server">
                <div class="adds-icon">
                    <div class="card-adds ellipsis-txt">
                        <asp:Label runat="server" ID="lblUpcomingEventLocation"></asp:Label>
                    </div>
                    <div class="card-slabel card-type">
                        <asp:Label runat="server" ID="lblUpcomingEventType"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<!--last activity - left column -->
<%--note: this card is hidden when there is no data--%>
<div id="activityCard" class="ibox widget basic-card last-card" runat="server">
    <div class="ibox-title">
        <i class="icon-Activity"></i>
        <h3 class="card-title language-entry">Last Activity</h3>
    </div>
    <div class="ibox-content">
        <asp:Label runat="server" ID="lblLastActivityTaskType" class="card-label"></asp:Label>
        <div class="inner-wrp">
            <div>
                <a runat="server" id="linkLastActivity" href="javascript:void(0);" class="hover-link">
                    <asp:Label runat="server" ID="lblLastActivitySubject"></asp:Label>
                </a>
                <div id="wrpNote" runat="server" class="note-wrp">
                    <asp:Label runat="server" ID="lblNote"></asp:Label>
                </div>
            </div>
            <p>
                <asp:Label runat="server" ID="lblLastActivityCreatedDate"></asp:Label>
                <asp:Label runat="server" ID="lblLastActivityCreatedBy"></asp:Label>
            </p>
        </div>
    </div>
</div>
