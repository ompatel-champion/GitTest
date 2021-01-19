var subscriberId = $("#lblSubscriberId").text(); 
var userId = $("#lblUserId").text();
var dealId = $("#lblDealId").text();
var contactId = parseInt($("#lblContactId").text());
var companyId = parseInt($("#lblCompanyId").text());
var documentId = parseInt($("#lblDocumentId").text());
var globalCompanyId = parseInt($("#lblGlobalCompanyId").text());



$(function () {
    new FileUpload().Init();
});


//initialize and bind file upload
var FileUpload = function () {
    var self = this;
    var baseUploadUrl = '/_handlers/temp-file-upload-handler.ashx?';
    var $bDocument = $("#btnUploadDocument");
    var $iptDocument = $("#fuUploadDocument");
    var $uploadedDocument = $(".uploaded-document");

    //trigger file upload click event
    $bDocument.click(function () { $iptDocument.click(); });



    this.Init = function () {
        // initialize upload logo
        $iptDocument.ajaxfileupload({
            action: baseUploadUrl + "ref=" + $("#lblGuid").text() + "&folder=dealDocument&delete=true",
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
                    self.ShowUploadedDcoument(res[i]);
                }

                //remove uploading msg
                $(".file-uploading-loader").remove();
            },
            onStart: function () {
                $('<span class="file-uploading-loader m-sm"><img src="/_content/_img/loading_20.gif" class="m-r-xs" />Uploading...</span>').insertAfter($bDocument);
            }
        });


        // add document
        $("#btnAddDocument").unbind('click').click(function () {
            self.AddDocument();
        });

        // cancel document form
        $("#btnClearDocumentUpload").unbind('click').click(function () {
            $("#txtDocumentTitle").val("");
            $("#txtDocumentDescription").val("");
            self.ClearUplaodedDocument();
        });

        // show document 
        if ($uploadedDocument.find(".doc-name").html() !== '') {
            $uploadedDocument.addClass("inline");
            $uploadedDocument.removeClass("hide");
        }

    };



    this.ShowUploadedDcoument = function (document) {
        $uploadedDocument.find(".doc-name").html(document.FileName);
        $uploadedDocument.find(".doc-name").attr("href", "/DownloadDocument.aspx?file=" + document.Uri);
        $uploadedDocument.find(".doc-path").html(document.Uri);

        $uploadedDocument.attr("blob-reference", document.BlobReference);
        $uploadedDocument.attr("container-reference", document.ContainerReference);

        $uploadedDocument.addClass("inline");
        $uploadedDocument.removeClass("hide");
         
        self.SetActions();
    }



    this.SetActions = function () {
        // bind delete action
        $("[data-action='delete']").unbind('click').click(function () {
            self.ClearUplaodedDocument();
        });
    }



    this.ClearUplaodedDocument = function () {
        $uploadedDocument.find(".doc-name").html("");
        $uploadedDocument.find(".doc-name").attr("href", "");
        $uploadedDocument.find(".doc-path").html("");
        $uploadedDocument.removeClass("inline");
        $uploadedDocument.addClass("hide");
    }



    this.AddDocument = function () {
        if ($("#txtDocumentTitle").val() === "") {
            swal({ title: "Please enter a document title!", type: "warning", showCancelButton: false });
        } else if ($uploadedDocument.find(".doc-path").html() === "") {
            swal({ title: "Please upload a document!", type: "warning", showCancelButton: false });
        } else { 
            var doc = new Object();
            doc.SubscriberId = subscriberId;
            doc.UploadedBy = userId;  
            doc.Title = $("#txtDocumentTitle").val();
            doc.Description = $("#txtDocumentDescription").val(); 
            doc.DocumentId = documentId;
            doc.CompanyIdGlobal = globalCompanyId;

            // check if this is a new file or saving the same file
            var blobReference = $uploadedDocument.attr("blob-reference");
            var containerReference = $uploadedDocument.attr("container-reference"); 
            if (blobReference != '' && containerReference != '') {
                // new file upload
                doc.DocumentBlobReference = blobReference;
                doc.DocumentContainerReference = containerReference;
                doc.FileName = $uploadedDocument.find(".doc-name").html();
                doc.UploadUrl = $uploadedDocument.find(".doc-path").html();
            } else {
                // no new file uploaded
            }
              
            if (dealId > 0) {
                doc.DocumentTypeId = 4;
                doc.DealId = dealId;
            } else if (contactId > 0) {
                doc.DocumentTypeId = 5;
                doc.ContactId = contactId;
            } else if (companyId > 0) {
                doc.DocumentTypeId = 6;
                doc.CompanyId = companyId;
            }
             
            var docList = [];
            docList.push(doc);

             
            // save note
            $.ajax({
                type: "POST",
                url: "/api/document/SaveDocuments",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(docList),
                success: function (response) {
                    parent.RefreshDocuments();
                },
                beforeSend: function () {
                },
                error: function (request) {
                    alert(JSON.stringify(request));
                }
            });
        }

    } 
}


function saveDocument() {
    new FileUpload().AddDocument();
}