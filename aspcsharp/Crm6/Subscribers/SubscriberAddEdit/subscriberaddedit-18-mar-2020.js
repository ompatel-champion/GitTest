var subscriberId = parseInt($("#lblSubscriberId").text());
var userId = parseInt($("#lblUserId").text()); 
var $divSubscriber = $("#divSubscriber");
var $txtCompanyName = $("#txtCompanyName");
var $txtContactName = $("#txtContactName");
var $txtEmail = $("#txtEmail");
var $ddlCountry = $("#ddlCountry");


$(function () {
    // setup page
    new PageSetup().Init();
    // initialize locations
    new Location().Init();
    // initialize file upload
    new FileUpload().init();

});


var PageSetup = function () {
    var self = this;

    this.Init = function () {
        self.SetupSelect2Dropdowns();
    }
     
    this.SetupSelect2Dropdowns = function () {  
        $ddlCountry.select2({ theme: "classic" });
    }

    if (subscriberId) {
        $('title').text('Edit Subscriber');
    } else {
        $('title').text('Add Subscriber');
    }
}


var Location = function () {
    var self = this;

    this.Init = function () {
        // bind company logo for subscriber
        //var logoPicPath = $("#txtLogoImageLink").val();
        //if (logoPicPath !== '') {
        //    new FileUpload().BindImage(logoPicPath);
        //}
        // init validator
        self.InitSaveValidator();
    }


    this.InitSaveValidator = function () {
        // select2 dropdown validator
        $ddlCountry.on('select2:select', function (evt) { $(this).valid(); });

        // add the rule here
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return (arg !== value && value !== null) && value !== null && value !== "0";
        }, "");

        // validate
        $divLocationSetup.validate({
            rules: {
                txtCompanyName: { required: true },
                txtContactName: { required: true },
                txtEmail: { required: true },
                ddlCountry: { valueNotEquals: "null" }
            }, messages: {
                txtCompanyName: "Please enter the company name",
                txtContactName: "Please enter the contact name", 
                txtEmail: "Please enter the email", 
                ddlCountry: "Please select a country"
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveSubscriber();
            }
        });
    }


    this.SaveSubscriber = function () {
        var subscriber = new Object();
        subscriber.SubscriberId = subscriberId;
        subscriber.CompanyName = $txtCompanyName.val();
        subscriber.ContactName = $txtContactName.val();
        subscriber.Email = $txtEmail.val();
        subscriber.Address = $("#txtAddress").val();
        subscriber.City = $("#txtCity").val(); 
        subscriber.PostalCode = $("#txtPostalCode").val();
        subscriber.CountryName = $ddlCountry.val();
        subscriber.Phone = $("#txtPhone").val();
        subscriber.Comments = $("#txtComments").val();

        //// company logo
        //var profilePic = new Object();
        //if ($("#txtLogoImageLink").val() !== '') {
        //    logoPic.UploadUrl = $("#txtLogoImageLink").val();
        //    logoPic.RefId = subscriberId;
        //} else {
        //    logoPic = null;
        //}

        var request = new Object();
        request.Subscriber = subscriber;
        request.LogoPic = LogoPic;

        // AJAX to save the subscriber
        var $divSubscriber= $("#divSubscriberp");
        $.ajax({
            type: "POST",
            url: "/api/subscriber/SaveSubscriber",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(request),
            success: function (response) {

                bindLoadingMsg("", $divSubscriber, false);
                if (parseInt(response) > 0)
                {
                    // close the pop up and goto subscriber detail page
                    try { parent.RefreshParent(); } catch (e) { }
                }
                else
                {
                    alert("Error saving subscriber!");
                }
            }, beforeSend: function () {
                //add loading message
                bindLoadingMsg("Saving Subscriber...", $divSubscriber, true);
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });

    }

}


//initialize and bind file upload
var FileUpload = function () {
    var self = this;

    var baseUploadUrl = '/_handlers/temp-file-upload-handler.ashx?';

    //buttons
    var $bLogo = $(".change-logo-pic");
    //inputs
    var $iptLogo = $("#fuLogoImage");
    //trigger file upload click event
    $bLogo.click(function () { $iptLogo.click(); });

    //get containers
    var $containerLogo = $(".uploaded_logo_image");

    //initialize file uploads
    function deleteUploadedFile(guid) {
        $.ajax({
            type: "GET",
            url: "/api/document/Delete/?id=" + guid,
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (!data) alert("Opps!! something went wrong when deleting the file. Please try again.");
            }
        });
    }


    function bindUploadLogoEvents() {
        //delete 
        $containerLogo.find(".fa").parent().unbind("click").bind("click", function () {
            var $this = $(this);
            //open the confirmation popup
            var $wrapper = $("<div/>", { "class": "modalWrapper" }).launchModal({
                title: 'Confirm Deletion',
                bodyHtml: '<h3 class="text-center">Are you sure you want to delete the company logo?</h3>',
                btnSuccessClass: 'btn-danger',
                btnSuccessText: 'Delete',
                fnSuccess: function () {
                    //perform delete
                    if ($this && $this.attr("data-name"))
                        deleteUploadedFile($this.attr("data-name"));

                    //hide   
                    $wrapper.find(".modal").modal('hide');
                    //set default once deleted
                    $containerLogo.find("img").attr("src", "");
                    $containerLogo.addClass("hide");
                    $this.removeAttr("data-name");
                    $("#txtLogoImageLink").val("");
                }
            });
        });
    }


    this.init = function () {
        //init upload logo
        $iptLogo.ajaxfileupload({
            action: baseUploadUrl + "ref=" + $("#lblGuid").text() + "&folder=logo&delete=true",
            validate_extensions: true,
            onComplete: function (data) {
                //check the invalid msg
                if (data.message !== "") {
                    alert(data.message); return;
                }
                //get the json response
                var res = data.response;

                //loop through and create the files
                for (var i = 0; i < res.length; i++) {
                    self.BindImage(res[i].path);
                }

                //bind events
                bindUploadLogoEvents();

                //remove uploading msg
                $(".file-uploading-loader").remove();
            },
            onStart: function () {
                $('<span class="file-uploading-loader m-sm"><img src="/_content/_img/loading_20.gif" class="m-r-xs" />Uploading...</span>').insertAfter($bProfile);
            }
        });

        //bind events
        bindUploadLogoEvents();

    }


    this.BindImage = function (imagePath) {
        //show uploaded image container
        $containerLogo.removeClass("hide");
        //assign image url
        $containerLogo.find("img").attr("src", imagePath);
        $containerLogo.find("img").attr("data-name", imagePath);
        $("#txtLogoImageLink").val(imagePath);
    }

}


function SaveSubscriber() {
    $divSubscriber.submit();
}


function bindLoadingMsg(msg, $parent, binsert, topMargin) {
    //delete existing
    $parent.find(".loading-msg").remove();

    if (binsert) {
        //create loading
        var $loading = $('<div class="loading-msg text-center"></div>');
        var $spinner = spinkit.getSpinner(spinkit.spinerTypes.fadingCircle);
        $spinner.css("margin-top", (topMargin && topMargin != "" ? topMargin : "10px"));
        $loading.append($spinner);
        $loading.append($("<div class='loading-msg m-t-xs'>" + msg + "</div>"));
        $loading.appendTo($parent);
    }
}
