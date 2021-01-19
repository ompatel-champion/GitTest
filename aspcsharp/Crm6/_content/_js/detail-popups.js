
function openpopup(type, id, title) { 
    switch (type) {
        case "deal-detail":
            dealDetail();
            break;
        case "contact-detail": 
            contactDetail();
            break;
        case "company-detail":   
            companyDetail();
            break;
        default:
    }

    function dealDetail() {
        var iframeUrl = "/Deals/DealDetail/DealDetail.aspx?dealId=" + id + "&popup=1";
        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframeDealDetail" }).launchModal({
            title: "",
            modalHeaderClass: "hide",
            modalClass: "modal-1200",
            btnSuccessText: "",
            maxHeight: "630px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () { 
            }
        });
    }

    function companyDetail() { 
        var iframeUrl = "/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + id + "&popup=1";
        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframeCompanyDetail" }).launchModal({
            title: "",
            modalHeaderClass: "hide",
            modalClass: "modal-1200",
            btnSuccessText: "",
            maxHeight: "630px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () { 
            }
        }); 
    }


    function contactDetail() {
        var iframeUrl = "/Contacts/ContactDetail/ContactDetail.aspx?contactId=" + id + "&popup=1";
        var $wrapper = $("<div/>", { "class": "modalWrapper", "id": "iframeContactDetail" }).launchModal({
            title: "",
            modalHeaderClass: "hide",
            modalClass: "modal-1200",
            btnSuccessText: "",
            maxHeight: "630px",
            scrollBody: true,
            iframeUrl: iframeUrl,
            fnSuccess: function () { 
            }
        }); 
    }


}


