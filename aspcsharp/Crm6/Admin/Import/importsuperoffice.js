var userId = parseInt($("#lblUserId").text());
var subscriberId = parseInt($("#lblSubscriberId").text());
var $ddlDisplayLanguage = $("#ddlDisplayLanguage");

$(function () {
    // setup page
    new PageSetup().Init();
    // file upload
    new FileUpload().Init();
});


var PageSetup = function () {
    var self = this;
    this.Init = function () {
    }
}


// initialize and bind file upload
var FileUpload = function () {
    var self = this;
    var baseUploadUrl = '/_handlers/temp-file-upload-handler.ashx?';
    // buttons
    var $bSuperOffice = $("#btnUploadSuperOfficeExcel");
    // inputs
    var $iptSuperOffice = $("#fuSuperOfficeExcel");
    //trigger file upload click event
    $bSuperOffice.click(function () { $iptSuperOffice.click(); });

    // get containers
    var $container = $(".uploaded_file");

    this.Init = function () {

        // initialize upload
        $iptSuperOffice.ajaxfileupload({
            action: baseUploadUrl + "ref=" + $("#lblGuid").text() + "&folder=uploadSuperOfficec&delete=true",
            validate_extensions: false,
            onComplete: function (data) {

                //check the invalid message
                if (data.message !== "") {
                    return;
                }
                //get the JSON response
                var res = data.response;

                //loop through and create the files 
                for (var i = 0; i < res.length; i++) {
                    self.BindFile(res[i]);
                }

                //remove uploading msg
                $(".file-uploading-loader").remove();

            },
            onStart: function () {
                $('<span class="file-uploading-loader m-sm"><img src="/_content/_img/loading_20.gif" class="m-r-xs" />Uploading...</span>')
                    .insertAfter($bSuperOffice);
            }
        });
    }

    this.BindFile = function (uploadedFile) {
        // show uploaded file in container
        $container.removeClass("hide");
        $container.find("#lblUploadedFilename").attr("text", uploadedFile.Uri);
        $container.find("#lblUploadedFilename").attr("data-name", uploadedFile.FileName);
        $container.attr("blob-reference", uploadedFile.BlobReference);
        $container.attr("container-reference", uploadedFile.ContainerReference);
        $container.html(uploadedFile.FileName);

        $("#btnProcessFile").removeClass("hide");
        $("#btnProcessFile").unbind("click").click(function () {
            $.ajax({
                type: "GET",
                url: "/api/SuperOffice/SuperOfficeImport/?blobReference=" + uploadedFile.BlobReference + "&containerReference=" + uploadedFile.ContainerReference + "&subscriberId=" + subscriberId,
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: (60000 * 30),
                success: function (data) {
                     
                    swal({
                        title: translatePhrase("Users imported successfully!"),
                        type: "success",
                        showCancelButton: false
                    });
                },
                beforeSend: function () {
                    swal({ text: translatePhrase("Please wait") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: true });

                }, error: function (request, status, error) {
                    alert(JSON.stringify(request))
                }
            });
        });
    }

}

