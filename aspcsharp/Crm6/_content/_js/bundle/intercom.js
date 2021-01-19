// intercom functions

function updateCustomAttributes(attributes) {
    if (window.Intercom === undefined) {
        return false;
    } else {
        window.Intercom("update", attributes);
        return true;
    }
};


function trackEvent(eventName, metadata) {
    if (window.Intercom === undefined) {
        return false;
    } else {
        window.Intercom('trackEvent', eventName, metadata);
        return true;
    }
};


// intercom widget
function loadIntercomWidget() {
    var w = window;
    var ic = w.Intercom;
    if (typeof ic === "function") {
        ic('reattach_activator');
        ic('update', intercomSettings);
    } 
    else
    {
        var d = document;
        var i = function() { i.c(arguments) };
        i.q = [];
        i.c = function(args) { i.q.push(args) };
        w.Intercom = i;
        function l() {
            var s = d.createElement('script');
            s.type = 'text/javascript';
            s.async = true;
            // test
            s.src = 'https://widget.intercom.io/widget/jgrb3sow';
            // production
            //s.src = 'https://widget.intercom.io/widget/y3oan1ik';
            var x = d.getElementsByTagName('script')[0];
            x.parentNode.insertBefore(s, x);
        }
        if (w.attachEvent) {
            w.attachEvent('onload', l);
        } else {
            w.addEventListener('load', l, false);
        }
    }
};


// run once the app initializes - DOM is ready
function setupIntercomUser() {
    // get json data from ajax call + update intercom
    $.ajax({
        type: "GET",
        url: "/api/intercom/getIntercomUserData?userId=" + userId,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data:"",
        success: function (response) {
            updateCustomAttributes({
                app_id: "y3oan1ik",                     // intercom app id
                //access_token: response.AccessToken,   // intercom access token
                created_at: response.createdDateTime,   // unix timestamp
                email: response.EmailAddress,           // email for CRM user
                name: response.UserName,                // full name for CRM user
                user_id: userId,                        // CRM UserId (ToString)
                "company":
                {
                    "name": response.subscriberName,    // Subscriber company name for CRM user
                    "id": response.subscriberId         // SubscriberId for CRM user
                },
                "City": response.city,                  // city for CRM user
                "Country": response.country,            // country for CRM user
                "Job Title": response.job_title,        // title for CRM user
                "Timezone": response.timeZone,          // timezone for CRM user
                "Web Url": response.webUrl              // CRM subscriber's web URL (if custom - otherwise is crm6.firstfreight.com)
            });
        },
        error: function (request) { }
    });
};


// message 2
// The first time a User signs in
//TODO: ?? trial account or new user for existing subscriber
function trackFirstLogin(userId) {
    updateCustomAttributes({
        'user_id': userId,
        'Journey Stage': 'Onboarding',
        'Journey Step': 2
    });
    trackEvent('first login', {});
}


// message 3
// Trigger when a user changes their basic details
function trackBasicDetailsChange(userId) {
    updateCustomAttributes({
        'user_id': userId,
        'Journey Stage': 'Onboarding',
        'Journey Step': 3
    });
    trackEvent('basic details changed', {});
}


// message 4
// trigger when a user changes their password
function trackPasswordChange(userId) {
    updateCustomAttributes({
        'user_id': userId,
        'Journey Stage': 'Onboarding',
        'Journey Step': 4
    });
    trackEvent('password changed', {});
}


// message 5
// trigger when a user activates Sync feature
function trackActivateSync(userId) {
    updateCustomAttributes({
        'user_id': userId,
        'Journey Stage': 'Onboarding',
        'Journey Step': 5
    });
    trackEvent('activated sync', {});
}


// message 6
// trigger when a user creates a new company the first time
function trackAddCompany(userId) {
    updateCustomAttributes({
        'user_id': userId,
        'Journey Stage': 'Onboarding',
        'Journey Step': 6
    });
    trackEvent('added a new company', {});
}


// message 7
// trigger when a user creates a new deal the first time
function trackAddDeal(userId) {
    updateCustomAttributes({
        'user_id': userId,
        'Journey Stage': 'Onboarding',
        'Journey Step': 7
    });
    trackEvent('added a new deal', {});
}


// message 8 and 9
// trigger when a user creates a new calendar event the first time
function trackAddCalendarEvent(userId) {
    updateCustomAttributes({
        'user_id': userId,
        'Journey Stage': 'Onboarding',
        'Journey Step': 8
    });
    trackEvent('added a new calendar event', {});
}


// message 10
// trigger when a user runs a report the first time
function trackRunReport(userId) {
    updateCustomAttributes({
        'user_id': userId,
        'Journey Stage': 'Onboarding',
        'Journey Step': 10
    });
    trackEvent('ran a report', {});
}


// message 11
// trigger when a user adds a sales team colleague the first time
function trackAddSalesTeamColleague(userId) {
    updateCustomAttributes({
        'user_id': userId,
        'Journey Stage': 'Onboarding',
        'Journey Step': 11
    });
    trackEvent('added a sales team colleague', {});
}


// message 12
// trigger when a user logs in via mobile the first time
function trackSignedInViaMobile(userId) {
    updateCustomAttributes({
        'user_id': userId,
        'Journey Stage': 'Onboarding',
        'Journey Step': 12
    });
    trackEvent('signed in via mobile', {});
}

