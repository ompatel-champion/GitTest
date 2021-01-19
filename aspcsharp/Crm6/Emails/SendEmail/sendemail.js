
var subscriberId = parseInt($("#lblSubscriberId").text());
var userId = parseInt($("#lblUserId").text());
var dealId = parseInt($("#lblDealId").text());
var $userContact = $("#txtTagUser");
var $selectedUserContactJson = $("#txtTagUserId");

$(document).ready(function () {
    new PageSetter().Init();
});


var PageSetter = function () {
    var self = this;

    this.Init = function () {
        // setup the text editor
        // init
        $("#txtMessage").tinymcefy({ tags: '', height: 400 }).show();
        try {
            tinymce.activeEditor.setContent('');
        } catch (e) {

        }

        // init user tags input
        $userContact.setTagsinput({
            maxTags: 3,
            dataUrl: "/api/AutoComplete/?type=user,contact&SusbcriberId=" + subscriberId,
            dataField: $selectedUserContactJson
        });
    }
}


var Email = function () {
    var self = this;
    this.ValidateSend = function () {
        return true;
    }

    this.SendEmail = function () {
        if (self.ValidateSend()) {

            // setup email
            var email = new Object();
            email.Subject = $("#txtSubject").val();
            email.SubscriberId = subscriberId;
            email.DealId = dealId;
            email.HtmlBody = $("#txtMessage").val();
            email.FromUser = userId;
            email.SendUserIds = []; 
            email.SendContactIds = [];
            var selectedUsersContacts = jQuery.parseJSON($selectedUserContactJson.val());
            if (selectedUsersContacts.length > 0) {
                $.each(selectedUsersContacts, function (i, obj) { 
                    alert(obj.type)
                    if(obj.type === 'user')
                        email.SendUserIds.push(obj.id); 
                    else if (obj.type === 'contact')
                        email.SendContactIds.push(obj.id); 
                }); 
            }
             
            alert(JSON.stringify(email)); 
            var $divEmailSetup = $("#divEmailSetup"); 
            $.ajax({
                type: "POST",
                url: "/api/email/SendEmail",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 50000,
                data: JSON.stringify(email),
                success: function (response) {
                    if (parseInt(response) > 0) {
                        parent.RefreshParent();
                    } else {
                        alert("Email send error");
                    }

                    bindLoadingMsg("", $divEmailSetup, false);
                }, beforeSend: function () {
                    //add loading message
                    bindLoadingMsg("Sending email...", $divEmailSetup, true);
                }, error: function (request, status, error) {
                }
            });
        }
    }
}


function sendEmail() {
    new Email().SendEmail();
}
