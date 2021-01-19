// set global values
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var $divExchangeSyncLog = $("#$divExchangeSyncLog");
var $divNoItems = $("#divNoItems");
var $exchangeSyncLogItemsTable = $(".sync-log-table");


$(function () {
    // init exchange sync log
   new ExchangeSyncLogEnries().Init();
});


var ExchangeSyncLogEnries = function () {
    var self = this;

    // init exchange sync log
    this.Init = function () {
    }
 
}

