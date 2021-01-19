
// *** First Freight CRM - global script library ***
// NOTE!: Add your new reusable methods to this library object and create sub object categories/groups as needed.
var FFGlobal = new function() {

    var self = this;

    //documents specific object (added to from doc specific .js files) - these methods apply to multiple documents
    //e.g. docs-detail.js adds a detail object with methods used by the detail pages (DealDetail.aspx, ContactDetail.aspx, CompanyDetail.aspx, etc...)
    self.docs = new _docs(self); function _docs(parent) {}

    //utility methods
    self.utils = new _utils(self); function _utils(parent) {

        var self = this;

        //string methods
        self.string = new _string(self); function _string(parent) {

            var self = this;

            //get intitials based on the provided string - pass a string or object of properties {'str', 'seperator', etc...}
            //note: whitespace is ignored and for now this method is hard coded to return only the first and last word initials.
            self.getInititals = function(props) {
                props = props||{};
                if (typeof props==="string") props = {str:props};
                var seperator = props.seperator||'';
                var str = (props.str.trim()).split(/\s+/).map(function(val, index, arr){
                    if (index===0 || index===arr.length-1) return val[0]
                }).join(seperator)||'';
                if (str) str += seperator;
                return str;
            }

            //capitalize the first letter of a string
            self.capitalize = function(str) {
                return str.charAt(0).toUpperCase()+str.substring(1); 
            }

            //return true if the string is non numeric and not empty
            self.isNonNumeric = function(str) {
                return str && !$.isNumeric(str);
            }
        }
        
        //html methods
        self.html = new _html(self); function _html(parent) {
            
            var self = this;

            //add word break opportunites (if a regex is not provided the word break points are added after all '-' and '@' chars)
            //note: props can be a string or object of properties
            self.addWordBreakOpportunities = function(props) {
                props = ((typeof props==="string")?{content:props}:props)||{};
                return (props.content||'').replace(props.regex||/(\-|\@)/g,"$1<wbr/>");
            }
        }

        //number methods
        self.number = new _number(self); function _number(parent) {

            var self = this;
                               
            //formatting methods
            self.format = new _format(self); function _format(parent) {

                var self = this;

                //format a number as currency
                self.currency = function(num, symbol) {
                    return (symbol!==undefined?symbol:'$')+(Math.round(num)+'').replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
                }

                //format a number as a percentage
                self.percent = function(num) {
                    return Math.round(num*100)+"%";
                }
            }
        }

        //date methods
        self.date = new _date(self); function _date(parent) {
            
            var self = this;

            //get the day ordinal (e.g. 1st, 2nd, 3rd... 31st)
            self.getDayOrdinal = function(day) {
                var ordinal = "";
                if (day>3 && day<21) ordinal = "th";
                else {
                    switch (day%10) {
                        case 1:  ordinal = "st"; break;
                        case 2:  ordinal = "nd"; break;
                        case 3:  ordinal = "rd"; break;
                        default: ordinal = "th"; break;
                    }
                }
                return day+ordinal;
            }
        }

        //sort methods
        self.sort = new _sort(self); function _sort(parent) {

            var self = this;

            //sort an object by key list (returns a new object with the key values matching the reference keys array order)
	        //optionally set the 'addUnmatched' prop true to add any unmatched obj key/value pairs to the return object
            self.byKeyList = function(props) {
                props = props||{};
                var obj = props.obj;
                var refKeys = props.refKeys;
                var rObj = {};
                $.each(refKeys, function(i, key) {
                    if (obj[key]!==undefined) rObj[key] = obj[key];
                });
                if (props.addUnmatched) {
                    var add = [];
                    for (var i in obj) {
                        if ($.inArray(i, refKeys)===-1) add.push({name:i, obj:obj[i]});
                    }
                    if (add.length) {
                        add.sort(function(a,b){
                            return a.name.localeCompare(b.name);
                        });
                        $.each(add, function(i, item) {
                            rObj[item.name] = item.obj;
                        });
                    }
                }
		        return rObj;
	        }

            //sort by object value
            self.byObjVal = function(props) {
                props = props||{};
                var obj = props.obj;
                var keyName = props.keyName;
                var sortArry = [];
                for (var i in obj) sortArry.push({keyName:i, obj:obj[i]});
                if (props.compare) sortArry.sort(function(a,b) {
                    return props.compare(a.obj,b.obj);
                })
                var rObj = {};
                $.each(sortArry, function(i, item) {
                    rObj[item.keyName] = item.obj;
                });
                return rObj;
            }
        }

        //media query methods
        self.mediaQuery = new _media_query(self); function _media_query(parent) {

            var self = this;

            //move an element from one parent to another based on a media query
            self.moveElm = function(props) {
                var activeQuery = null;
                var allQuery = null;
                var queries = props.queries = props.queries||{};
                for (var i in queries) {
                    var query = queries[i];
                    query.id = i;
                    query.match = query.match||"";
                    if (query.match.toLowerCase()==="all") allQuery = query;
                }
                $(document).ready(function() {
                    move();
                });
                $(window).resize(function() {
                    move();
                });

                function move() {
                    var matched = false;
                    for (var i in queries) {
                        var query = queries[i];
                        if (query.match && query.match.toLowerCase()!=="all" && window.matchMedia(query.match).matches) {
                            _move(query);
                            matched = true;
                            break;
                        }
                    }
                    if (!matched && allQuery) _move(allQuery);

                    function _move(query) {
                        if (query.id!==activeQuery) {
                            query.$parent.append(props.$elm);
                            activeQuery = query.id;
                        }
                    }
                }
            }
        }
    }

    //svg methods
    self.svg = new _svg(self); function _svg(parent) {

        var self = this;

        //create svg
        self.create = new _create(self); function _create(parent) {

            var self = this;

            //create profile pic initials
            self.profilePicInitials = function(props) {
                props = props||{};
                if (typeof props==="string") props = {str:props};
                var COLORS = {
                    text: 'white',
                    background: {
                        "medium dark blue": '#1184C3',
                        "dark grey": '#3E4955',
                        "dark blue": '#0C629C',
                        "medium green": '#1FC39F',
                        "light grey": '#A2AFBD',
                        "dark green": '#039280',
                        "medium darker blue": '#1874A8',
                        "medium grey": '#6D7B8C',
                        "medium blue": '#1E92D2'
                    }
                }
                if (props.initials && (typeof props.initials==="string"||props.initials.name)) {
                    var pKeys = Object.keys(COLORS.background);
                    var index = props.initials.colorIndex?props.initials.colorIndex%pKeys.length:Math.floor(Math.random()*pKeys.length);
                    var bgColor = COLORS.background[pKeys[index]];
                    var svgStr = '<svg \
                        viewBox="0 0 100 100" \
                        xmlns="http://www.w3.org/2000/svg" \
                        style=" \
                            color:'+COLORS.textColor+'; \
                            text-transform:uppercase; \
                            background-color:'+bgColor+' \
                        " \
                    >';
	                    svgStr += '<text \
                            x="50%" \
                            y="50%" \
                            dominant-baseline="central" \
                            text-anchor="middle" \
                            fill="white" \
                            font-size="50" \
                            font-family="Arial, Helvetica, Sans-Serif" \
                        >'+_get_initials()+'</text>';
                    svgStr += '</svg>';
                    if (/imageurl/i.test(props.type)) svgStr = 'data:image/svg+xml;base64,'+btoa(svgStr);
                    return svgStr;
                }
                
                //get the string intitals
                function _get_initials() {
                    return (typeof props.initials==="string")?props.initials:FFGlobal.utils.string.getInititals({
                        str: props.initials.name,
                        seperator: props.initials.seperator
                    });
                }
            }
        }
    }

    //profile methods
    self.profile = new _profile(self); function _profile(parent) {

        var self = this;
        
        //pic methods (image or SVG initials)
        self.pic = new _pic(self); function _pic(parent) {
            
            var self = this;

            //create profile pic, returns the content and optionally appends to a provided wrapper. Pass an image URL string or optional object of properties {'imageURL', '$wrapper', 'initials', etc.}
            //note: if a URL image string is not provided and an initials property is defined, an SVG image with the initials will be created and returned.
            //note: if both a URL string and initials property is not provided than a default avatar placeholder image will be returned.
            self.create = function(props) {
                props = props||{};
                if (typeof props==="stirng") props = {imageURL:props};
                var imageURL = props.imageURL || FFGlobal.svg.create.profilePicInitials({
                    type: 'imageURL',
                    initials: props.initials
                }) || PLACEHOLDER_IMG_URL;
                var $image = props.$image||$("<img />");
                $image.attr("src", imageURL);
                var wrapper = props.$wrapper; if (wrapper) {
                    if (wrapper.overwrite && wrapper.$elm) wrapper.$elm.empty();
                    (wrapper.$elm||wrapper).append($image);
                }
                var elm = props.$elm
                return $image;
            }

            //create pic using an API url (C# code)
            //note: the standard self.create will still get called but will typically be supplied with an image or SVG initials returned by the API call
            self.createAPI = function(props) {
                props = props||{};
                var data = props.data;
                var type = data.type||"user";
                var $userimg = $("<div />", { "class": "profile-img-wrapper" });
                if (/external/i.test(type)) {
                    self.create({
                        $wrapper: {
                            $elm: $userimg,
                            overwrite: true
                        },
                        initials: {
                            name: data.name,
                            seperator: '',
                            colorIndex: data.id||data.colorIndex
                        }
                    });
                }
                else {
                    var apiUrl = '/api/User/GetUserProfilePic/?userId='+data.id+'&subscriberId='+data.subscriberId+(type?'&type='+type:""); 
                    $.getJSON(apiUrl, function (response) {
                        self.create({
                            $wrapper: {
                                $elm: $userimg,
                                overwrite: true
                            },
                            imageURL: response,
                            initials: {
                                name: data.name,
                                seperator: '',
                                colorIndex: data.id||data.colorIndex
                            }
                        });
                    });
                }
                props.$wrapper.append($userimg);
            }
        }
    }
}


// mobile menu hide/show on trigger
$(document).ready(function () {
    $('a.mobile-trigger').click(function (event) {
        event.stopPropagation();
        if (!$(this).hasClass('active')) {
            $('.mobile-header .mob-menu').stop(true, true).slideDown(300);
            $(this).addClass('active');
        } else {
            $('.mobile-header .mob-menu').stop(true, true).slideUp(300);
            $(this).removeClass('active');
        }
    });
});


// hide/show dropdowns
$('a.dotsbtn').click(function () {
    if (!$(this).hasClass('active')) {
        $(this).addClass('active');
        if ($(window).width() <= 565) {
            $('.top-header .pageInfo .advance-search').addClass('show').show();
        } else {
            $('.top-header .pageInfo .advance-search').addClass('show');
        }
    } else {
        $(this).removeClass('active');
        if ($(window).width() <= 565) {
            $('.top-header .pageInfo .advance-search').removeClass('show').hide();
        } else {
            $('.top-header .pageInfo .advance-search').removeClass('show');
        }
    }
});


// bottom menu hide/show as per viewport height
$(document).ready(function () {
    sideBarAction();
});


$(window).resize(function () {
    sideBarAction();
});


function sideBarAction() {
    var topMenuHt = $('#sidebar .top-nav').outerHeight(true);
    var bottomMenuHt = 180;
    var totalHt = topMenuHt + bottomMenuHt + 20;
    var windowHt = $(window).height();
    if (windowHt > totalHt) {
        $('#sidebar .bottom-info').removeClass('overflow');
    } else {
        if (!$('#sidebar .bottom-info').hasClass('overflow')) {
            $('#sidebar .bottom-info').addClass('overflow');
        }
    }
}


function ReloadPage() {
    "use strict";
    window.location.reload(true);
}


$(function () {
    // set the page navigations
    new PageSetter().SetPageNavs();
    // Heap Analytics
    initHeapAnalytics();
});


function initHeapAnalytics() {
    window.heap = window.heap || [], heap.load = function (e, t) {
        window.heap.appid = e, window.heap.config = t = t || {};
        var r = t.forceSSL || "https:" === document.location.protocol, a = document.createElement("script"); a.type = "text/javascript", a.async = !0, a.src = (r ? "https:" : "http:") + "//cdn.heapanalytics.com/js/heap-" + e + ".js"; var n = document.getElementsByTagName("script")[0]; n.parentNode.insertBefore(a, n);
        for (var o = function (e) {
            return function () {
                heap.push([e].concat(Array.prototype.slice.call(arguments, 0)))
            };
        },
            p = ["addEventProperties", "addUserProperties", "clearEventProperties", "identify", "removeEventProperty", "setEventProperties", "track", "unsetEventProperty"], c = 0; c < p.length; c++)heap[p[c]] = o(p[c])
    };
    heap.load("2420880320");
}


var PageSetter = function () {

    var self = this;

    // Menu Navigation
    this.SetPageNavs = function () {
        // calendar
        $(".nav-calendar").unbind("click").click(function () {
            location.href = "/Calendar/Calendar.aspx";
        });

        // contacts
        $(".nav-contacts").unbind("click").click(function () {
            location.href = "/Contacts/ContactList/ContactList.aspx";
        });

        // dashboard
        //$(".nav-dashboard").unbind("click").click(function () {
        //    location.href = "/Dashboard/SalesRepDashboard/SalesRepDashboard.aspx";
        //    //location.href = "/Reporting/ReportList.aspx";
        //});

        // deals
        $(".nav-deals").unbind("click").click(function () {
            if (isMobile()) {
                location.href = "/Mobile/Deals/DealList/DealList.aspx";
            } else {
                location.href = "/Deals/DealList/DealList.aspx";
            }
        });

        // companies
        $(".nav-companies").unbind("click").click(function () {
            location.href = "/Companies/CompanyList/CompanyList.aspx";
        });

        // global companies
        $(".nav-global-companies").unbind("click").click(function () {
            location.href = "/GlobalCompanies/GlobalCompanyList/GlobalCompanyList.aspx";
        });

    };

    // Loads the content into the iFrame
    this.ShowContent = function (vUrl, $aNav) {
        // set active nav
        $aNav.closest("ul").find("li").removeClass("active");
        $aNav.closest("li").addClass("active");
        // load iFrame content
        var iframe = document.getElementById("IframeContent");
        iframe.src = vUrl;
    };

};


// nav bar
$(function () {
    try {
        // set mouse hover
        $("ul.nav>li").hover(
            function () {
                var $this = $(this);
                if (!$this.hasClass("active")) {
                    var $img = $this.find(".nav-icon");
                    if ($img.length > 0) {
                        var imgSrc = $img.attr("src");
                        var deselectedSrc = imgSrc.replace("_24px", "_deselected_24px");
                        deselectedSrc = deselectedSrc.replace("_25px", "_deselected_25px");
                        $img.attr("src", deselectedSrc);
                    }
                }
            },

            function () {
                var $this = $(this);
                if (!$this.hasClass("active")) {
                    var $img = $this.find(".nav-icon");
                    if ($img.length > 0) {
                        var imgSrc = $img.attr("src");
                        var deselectedSrc = imgSrc.replace("_deselected_24px", "_24px");
                        deselectedSrc = deselectedSrc.replace("_deselected_25px", "_25px");
                        $img.attr("src", deselectedSrc);
                    }
                }
            }

        );

        // active img
        var $img = $("ul.nav>li.active").find(".nav-icon");
        if ($img.length > 0) {
            var imgSrc = $img.attr("src");
            var deselectedSrc = imgSrc.replace("_24px", "_deselected_24px");
            deselectedSrc = deselectedSrc.replace("_25px", "_deselected_25px");
            $img.attr("src", deselectedSrc);
        }

        $(".nav-header").unbind("click").click(function () {
            location.href = "/Admin/Users/UserProfile/UserProfile.aspx";
        });

        // TODO: console.log
    } catch (e) {}
});


function isMobile() {
    var isMobile = false; //initiate as false
    // device detection
    if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent)
        || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0, 4)))
        isMobile = true;
    return isMobile;
}


window.intercomSettings = {
    app_id: "y3oan1ik",
    custom_launcher_selector: '.intercom-launcher-discovery-container' // this added to show only button on load
};


(function () {
    var w = window;
    var ic = w.Intercom;
    if (typeof ic === "function") {
        ic('reattach_activator');
        ic('update', intercomSettings);
    } else {
        var d = document;
        var i = function () { i.c(arguments) };
        i.q = [];
        i.c = function (args) { i.q.push(args) };
        w.Intercom = i;

        function l() {
            var s = d.createElement('script');
            s.type = 'text/javascript';
            s.async = true;
            s.src = 'https://widget.intercom.io/widget/y3oan1ik';
            var x = d.getElementsByTagName('script')[0];
            x.parentNode.insertBefore(s, x);
        }

        if (w.attachEvent) {
            w.attachEvent('onload', l);
        } else {
            w.addEventListener('load', l, false);
        }
    }
})
    ();

$(function () {
    initICheck();
});


// iCheck
function initICheck() {
    try {
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green'
        });
    } catch (e) { }
}


// validate email address
function validateEmail(sEmail) {
    var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (filter.test(sEmail)) {
        return true;
    }
    else {
        return false;
    }
}


// get query string
function getQueryString(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}


// create guid
function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}


// update/add query string
function updateQueryStringParameter(uri, key, value) {
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    else {
        return uri + separator + key + "=" + value;
    }
}


// create/hide a loading msg
function bindFullScreenLoadingMsg(msg, binsert, topMargin) {
    //delete existing
    $(".loading-overlay").remove();
    if (binsert) {
        //create loading
        var $loading = $('<div class="loading-overlay"></div>');
        var $spinner = spinkit.getSpinner(spinkit.spinerTypes.fadingCircle);
        $spinner.css("margin-top", (topMargin ? topMargin : "20%"));
        $loading.append($spinner);
        $loading.append($("<div class='loading-msg m-t-xs'>" + msg + "</div>"));
        $loading.appendTo("body");
    }
}


// init tags
(function ($) {
    $.fn.setTagsinput = function (options) {
        //default options.
        var defaults = {
            dataUrl: "",
            dataField: null,
            maxTags: 25,
            noResultsText: "",
            minLength: 1,
            itemAdded: function () { return },
            itemRemoved: function () { return }
        };

        //merge
        var settings = $.extend({}, defaults, options);
        var $tagsinput = $(this);
        var $dataField = settings.dataField;

        //set url
        if (settings.dataUrl.indexOf("?") >= 0) settings.dataUrl = settings.dataUrl + "&";
        else settings.dataUrl = settings.dataUrl + "?";
        $tagsinput.tagsinput({
            itemValue: "id",
            itemText: "name",
            maxTags: settings.maxTags,
            typeahead: {
                autoSelect: false,
                items: 25,
                noResultsText: settings.noResultsText,
                minLength: settings.minLength,
                source: function (query) {
                    return $.ajax({
                        url: settings.dataUrl + "prefix=" + query + "",
                        beforeSend: function () { $tagsinput.tagsinput('input').addClass('typeahead-loading'); },
                        success: function () { $tagsinput.tagsinput('input').removeClass('typeahead-loading'); }
                    });
                }
            }
        });

        //get the input of tags input
        var $typingInput = $tagsinput.tagsinput('input');

        //show all items when min length is 0
        if (settings.minLength === 0) {
            $typingInput.click(function () {
                $typingInput.typeahead('lookup', '');
            });
            $typingInput.focus(function () {
                $typingInput.typeahead('lookup', '');
            });
        }

        //sync data with data field
        function syncDataField() {
            //array
            var itemlist = [];
            $dataField.val("");

            $tagsinput.next().find(".tag").each(function () {

                //create the object and assign data
                var tmpObj = new Object();
                tmpObj.name = $(this).data("item").name;
                tmpObj.id = $(this).data("item").id;
                tmpObj.type = $(this).data("item").type;

                //insert in to array
                itemlist.push(tmpObj);

                //add it in to the field
                $dataField.val(JSON.stringify(itemlist));
            });

            //hide the input box if you have reached the maximum tags
            if (itemlist.length >= settings.maxTags)
                $typingInput.addClass("hide");
            else
                $typingInput.removeClass("hide");
        }

        //add and remove events to sync selected data with the data field
        $tagsinput.on('itemAdded', function (event) {
            syncDataField();
            //on item added function
            settings.itemAdded(event);

        }).on('itemRemoved', function (event) {
            syncDataField();

            //on item removed function
            settings.itemRemoved(event);
        });

        //set data field items as tags
        try {
            var jsonData = $.parseJSON($dataField.val());
            $.each(jsonData, function (i, item) {
                $tagsinput.tagsinput('add', item);
            });
        } catch (e) {
            /*ignore*/
        }
    };

})(jQuery);


// init type ahead
(function ($) {
    $.fn.setAutocomplete = function (options) {
        //default options.
        var defaults = {
            dataUrl: "",
            idField: null,
            minLength: 1,
            clearOnSelect: false,
            onselect: function () { return; },
            source: null
        };

        //merge
        var settings = $.extend({}, defaults, options);

        var $typeahead = $(this);
        var $idField = settings.idField;

        //add attribute
        $typeahead.attr("autocomplete", "off");

        //set url
        if (settings.dataUrl.indexOf("?") >= 0) settings.dataUrl = settings.dataUrl + "&";
        else settings.dataUrl = settings.dataUrl + "?";

        //init
        $typeahead.typeahead({
            //set the source  
            autoSelect: false,
            items: 25,
            minLength: settings.minLength,
            source: function (query, process) {
                if (settings.source !== null) return process(settings.source);
                else {
                    $.ajax({
                        url: settings.dataUrl + "prefix=" + query + "",
                        beforeSend: function () {
                            $typeahead.addClass('typeahead-loading');
                        },
                        success: function (data) {
                            $typeahead.removeClass('typeahead-loading');
                            return process(data);
                        }
                    });
                }
                // ReSharper disable once NotAllPathsReturnValue
            },
            updater: function (item) {

                //assign the id to id field
                if ($idField) $idField.val(item.id);

                //on select function
                settings.onselect(item);

                if (settings.clearOnSelect) return "";
                else return item;
            }
        });

        //click and blur events
        $typeahead.click(function () {
            //select all text
            $(this).select();

            //show all items when min length is 0
            if (settings.minLength === 0) $typeahead.typeahead('lookup', '');


        }).blur(function () {
            //remove selected id
            if ($(this).val() === "" && $idField) $idField.val("0");
        });
    };
})(jQuery);


// bind models
(function ($) {
    // set modal size when scrollable
    function setModalSize($modal, biframe, maxHeight, appendToParent) {
        var windowHeight = appendToParent ? $(window.parent).innerHeight() : $(window).innerHeight();

        var bodyHeight = $(window).innerHeight() - ($modal.find(".modal-header").outerHeight() + $modal.find(".modal-footer").outerHeight() + 60);
        if (bodyHeight < 300) bodyHeight = 300;

        // take the body element
        var $ele = $modal.find(".modal-body");

        // if iframe is there set the body height and set the iframe as the element
        // alert("bodyHeight:" + bodyHeight)
        // alert("maxHeight:" + maxHeight) 
        if (biframe) {
            if (isMobile()) {
                $ele.css({ "height": "1px!important", "max-height": "100%!important" });
            } else {
                $ele.css({ "height": bodyHeight + "px", "max-height": maxHeight });
            }
            $ele = $ele.find("iframe");
        }

        // set styles for the body
        if (isMobile()) {
            $ele.css({
                "height": "1px!important",
                "overflow-y": "scroll",
                "max-height": "100%!important"
            });
        } else {
            $ele.css({
                "height": bodyHeight + "px",
                "overflow-y": "scroll",
                "max-height": maxHeight
            });
        }
    }

    $.fn.launchModal = function (options) {
        // default options.
        var defaults = {
            title: "",
            titleInfo: "",
            titleInfoClass: "",
            iconClass: "",
            bodyHtml: "",
            iframeUrl: "",
            maxHeight: "",
            scrollBody: false,
            modalClass: "",
            modalHeaderClass: "",
            btnCloseText: "Close",
            btnCloseClass: "secondary-btn",
            btnSuccessText: 'Save',
            btnSuccessClass: 'primary-btn',
            btnMoreText: '',
            btnMoreClass: 'primary-btn',
            fnSuccess: function () { },
            fnCancel: function () { },
            fnMore: function () { },
            appendToParent: false
        };

        // merge
        var settings = $.extend({}, defaults, options);
        var $this = $(this);

        // modal html
        var strHtml = "";
        strHtml += '<div style="display: none;" class="modal inmodal" id="modalPopup" tabindex="-1" role="dialog" aria-hidden="true">';
        strHtml += '    <div class="modal-dialog ' + settings.modalClass + '">';
        strHtml += '        <div class="modal-content animated fadeIn">';
        strHtml += '            <div class="modal-header ' + settings.modalHeaderClass + '">';
        strHtml += '                <button type="button" class="close closeX" data-backdrop="static" data-dismiss="modal"></button>';
        if (settings.iconClass !== '')
            strHtml += '            <i class="fa ' + settings.iconClass + ' modal-icon"></i>';
        strHtml += '                <h4 class="modal-title">' + translatePhrase(settings.title) + '</h4>';
        if (settings.titleInfo !== '')
            strHtml += '            <p class="title-info ' + settings.titleInfoClass + '">' + translatePhrase(settings.titleInfo) + '</p>';
        strHtml += '            </div>';
        strHtml += '            <div class="modal-body">';
        strHtml += '                <p>' + settings.bodyHtml + '</p>';
        strHtml += '            </div>';
        strHtml += '            <div class="modal-footer">';
        if (settings.btnSuccessText !== '')
            strHtml += '                <button type="button" class=" pull-right btnAction btn ' + settings.btnSuccessClass + ' pull-right">' + translatePhrase(settings.btnSuccessText) + '</button>';
        if (settings.btnCloseText !== '')
            strHtml += '        <p class="MT5 pull-right">    <button type="button" class="btnClose btn ' + settings.btnCloseClass + '" data-dismiss="modal">' + settings.btnCloseText + '</button> </p>';

        if (settings.btnMoreText !== '')
            strHtml += '                <button type="button" class="btnMore btn ' + settings.btnMoreClass + '">' + translatePhrase(settings.btnMoreText) + '</button>';
        strHtml += '            </div>';
        strHtml += '        </div>';
        strHtml += '    </div>';
        strHtml += '</div>';

        // append modal wrapper to body
        $this.html(strHtml);
        if (settings.appendToParent)
            $this.appendTo($(window.parent.document.body));
        else
            $this.appendTo('body');

        // modal
        var $modal = $this.find('#modalPopup');

        // iframe
        if (settings.iframeUrl !== "") {
            var $mBody = $modal.find(".modal-body");
            $mBody.addClass("iframe-added").html("");
            // set loading spinner
            var $spinner = spinkit.getSpinner(spinkit.spinerTypes.circle);
            $spinner.css({ position: "absolute", top: "30%", "left": "48.5%" });
            $mBody.append($spinner);
            // define iframe
            var $iframe = $("<iframe/>", {
                id: "frm_popup",
                src: settings.iframeUrl,
                frameBorder: "0",
                width: "100%"
            });

            //append
            $iframe.appendTo($mBody);

            //add attr
            $modal.attr("iframe-loading", true);

            //iframe loaded
            $iframe.ready(function () {
                //remove the spinner
                $modal.find(".sk-circle").remove();
                $modal.removeAttr("iframe-loading");
            });
        }

        // MODAL HIDDEN
        $modal.on('hidden.bs.modal', function () {
            setTimeout(function () {
                $this.remove();
            }, 100);
            // model hidden before close call the cancel function
            // settings.fnCancel();
        });

        // MODAL SHOW
        $modal.on('show.bs.modal', function () {

            // close button
            $modal.find('.btnClose').click(function () {
                settings.fnCancel();
            });

            // action button
            $modal.find('.btnAction').click(function () {
                settings.fnSuccess();
            });

            // left button 
            $modal.find('.btnMore').click(function () {
                settings.fnMore();
            });

        });

        // MODAL SHOWN
        $modal.on('shown.bs.modal', function () {
            // focus the first input of the modal
            $modal.find("input").first().focus();

            // scroll body
            if (settings.scrollBody === true || settings.iframeUrl !== "") {
                // set modal size
                setModalSize($modal, (settings.iframeUrl === "" ? false : true), settings.maxHeight, settings.appendToParent);

                // do it when resize
                $(window).resize(function () { setModalSize($modal, (settings.iframeUrl === "" ? false : true), settings.maxHeight); });
            }
        });

        // SHOW MODAL
        $modal.modal({
            backdrop: 'static',
            show: true
        });

        // return
        return $this;
    };

})(jQuery);


// bind i-checks
(function ($) {
    $('.i-checks.check-all').on('ifChecked',
        function () {
            $("[data-group='" + $(this).attr("data-group") + "']").iCheck('check');
        }).on('ifUnchecked',
            function () {
                $("[data-group='" + $(this).attr("data-group") + "']").iCheck('uncheck');
            });
});


// init number controls
$(function () {
    IntiNumberControls();
});


function IntiNumberControls() {
    $(".numberWithDecimalOnly").keydown(function (event) {

        if (event.keyCode === 110 || event.keyCode === 190) {
            if (this.value.split('.').length > 1) {
                // allow one decimal point
                event.preventDefault();
            }
        } else {
            if (event.keyCode === 46 || event.keyCode === 8 || event.keyCode === 9 || event.keyCode === 27 || event.keyCode === 13 ||
                // Allow: Ctrl+A
                (event.keyCode === 65 && event.ctrlKey === true) ||

                // Allow: home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            } else {
                // Ensure that it is a number and stop the key press
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                }
            }
        }
    });

    $(".numbersOnly").keydown(function (event) {
        if (event.keyCode === 110 || event.keyCode === 190) {
            if (this.value.split('.').length > 0) {
                // allow one decimal point
                event.preventDefault();
            }
        } else {
            if (event.keyCode === 46 || event.keyCode === 8 || event.keyCode === 9 || event.keyCode === 27 || event.keyCode === 13 ||
                // Allow: Ctrl+A
                (event.keyCode === 65 && event.ctrlKey === true) ||

                // Allow: home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            } else {
                // Ensure that it is a number and stop the key press
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                }
            }
        }
    });

}

function getDurationJson() {
    return [
        { "id": 15, "name": "15mins" },
        { "id": 30, "name": "30mins" },
        { "id": 45, "name": "45mins" },
        { "id": 60, "name": "1h" },
        { "id": 75, "name": "1h 15mins" },
        { "id": 90, "name": "1h 30mins" },
        { "id": 105, "name": "1h 45mins" },
        { "id": 120, "name": "2h" },
        { "id": 150, "name": "2h 30mins" },
        { "id": 180, "name": "3h" },
        { "id": 240, "name": "4h" },
        { "id": 3000, "name": "5h" },
        { "id": 360, "name": "6h" },
        { "id": 420, "name": "7h" }
    ];
}


function bindLoadingMsg(msg, $parent, binsert, topMargin) {
    // delete existing
    $parent.find(".loading-msg").remove();

    if (binsert) {
        // create loading
        var $loading = $('<div class="loading-msg text-center"></div>');
        var $spinner = spinkit.getSpinner(spinkit.spinerTypes.fadingCircle);
        $spinner.css("margin-top", (topMargin && topMargin != "" ? topMargin : "10px"));
        $loading.append($spinner);
        $loading.append($("<div class='loading-msg m-t-xs'>" + msg + "</div>"));
        $loading.appendTo($parent);
    }
}

// Date & TimeZone Functions
function formatDate(date, format) {
    'use strict';
    format = format || 'MM/dd/yyyy';
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    var vDate = '';
    switch (format) {
        case 'MM/dd/yyyy':
            vDate = month + '/' + day + '/' + year;
            break;
        case 'dd/MM/yyyy':
            vDate = day + '/' + month + '/' + year;
            break;
        case 'dd.MM.yyyy':
            vDate = day + '.' + month + '.' + year;
            break;
    }
    return vDate;
}

function setDateControls() {
    'use strict';
    var vDateFormat = $('#lblDateFormat').val();
    var vDateFormatMask = $('#lblDateFormatMask').val();
    $('.datepicker').datepicker({
        format: vDateFormatMask,
        startDate: '-3d',
        autoclose: true
    }).on('changeDate', function (ev) {
        $(this).datepicker('hide');
    });
}

// get sales stage color
function getSalesStageHeaderColor(index) {
    switch (index) {
        case 1: return "#84c8fb";
        case 2: return "#97d0fc";
        case 3: return "#aad9fc";
        case 4: return "#6bbcfb";
        case 5: return "#53b1fa";
        case 6: return "#3AA6F9";
        default:
    }
}


function formatNumber(val) {
    while (/(\d+)(\d{3})/.test(val.toString())) {
        val = val.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
    }
    return val;
}


function addSpinner() {
    var $spinner = $("<div id=\"divSpinner\" class=\"\"> <div class=\"ajax-modal\"> <div class=\"ibox ibox-content text-center ajax-modal-txt\"> <div class=\"spinner\"></div> </div></div></div>");
    $("body").append($spinner);
}


function removeSpinner() {
    if ($("#divSpinner").length > 0)
        $("#divSpinner").remove();
}
