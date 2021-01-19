var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var $divCampaigns = $("#divCampaigns");
var $divNoItems = $("#divNoItems");
var $tblCampaigns = $("#tblCampaigns");

// init
$(function () {
    new Campaigns().Init();
});


var Campaigns = function () {
    var self = this;

    // init campaign
    this.Init = function () {
        // new campaign
        $(".new-campaign").unbind('click').click(function () {
            location.href = "/Admin/Campaigns/CampaignAddEdit/CampaignAddEdit.aspx?campaignId=0";
        });
        self.BindActions(); 
    };

    this.BindActions = function () {
        $tblCampaigns.find("tr").each(function () {
            var $tr = $(this);
            // edit campaign
            $tr.find(".edit-campaign").unbind('click').click(function () {
                var campaignId = $(this).closest("tr").attr("data-id");
                location.href = "/Admin/Campaigns/CampaignAddEdit/CampaignAddEdit.aspx?campaignId=" + campaignId;
            });
        });
    };

};
