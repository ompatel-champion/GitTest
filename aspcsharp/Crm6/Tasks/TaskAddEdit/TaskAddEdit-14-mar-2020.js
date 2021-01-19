var $datepickers = $("[data-name='date']");
var subscriberId = parseInt($("#lblSubscriberId").text());
var activitySubscriberId = parseInt($("#lblActivitySubscriberId").text());
var activityId = parseInt($("#lblActivityId").text());
var userId = parseInt($("#lblUserId").text());
var userIdGlobal = parseInt($("#lblUserIdGlobal").text());
var $divTaskSetup = $("#divTaskSetup");
var $company = $("#txtTagCompany");
var $selectedCompanyJson = $("#txtTagCompanyId");
var $contact = $("#txtTagContact");
var $selectedContactJson = $("#txtTagContactId");
var $deal = $("#txtTagDeal");
var $selectedDealJson = $("#txtTagDealId");

$(document).ready(function () {
    new Task().Init();
    //new TagInputs().Init();
    //new PageSetter().Init();
});


var PageSetter = function () {
    var self = this;

    this.Init = function () {
        if (getQueryString("isSuccess") === 'true') {
            var $taskSuccess = $(".task-success");
            $taskSuccess.removeClass('hide');
            setTimeout(function () { $taskSuccess.addClass('hide'); }, 3000);
            // reload activities in parent page
            try { parent.RefreshActivities(); } catch (e) { /*ignore*/}
        }

        // initialize date pickers
        $datepickers.datepicker({
            format: "dd-MM-yy",
            autoclose: true
        }).on('changeDate', function () {
            $(this).valid();
        });
    };
};




var TagInputs = function () {
    var self = this;

    this.Init = function () {

        // init deal tags input
        self.InitDealsTagsInput();

        // init company tags input
        self.InitCompanyTagsInput();


        // init contact tags input
        $contact.setTagsinput({
            maxTags: 3,
            dataUrl: "/api/AutoComplete/?type=contact&SusbcriberId=" + activitySubscriberId,
            dataField: $selectedContactJson
        });
    };

    this.InitDealsTagsInput = function () {
        var companyId = getSelectedCompanyId();
        $deal.setTagsinput({
            maxTags: 1,
            dataUrl: "/api/AutoComplete/?type=deal&SusbcriberId=" + activitySubscriberId + "&GlobalCompanyId=" + companyId,
            dataField: $selectedDealJson,
            minLength: 0,
            noResultsText: "",
            itemRemoved: function () { },
            itemAdded: function () {
                // set company if not selected
                self.SetDealCompanyInTagsInput();
            }
        });
    };


    this.InitCompanyTagsInput = function () {
        $company.setTagsinput({
            maxTags: 1,
            dataUrl: "/api/AutoComplete/?type=company&SusbcriberId=" + activitySubscriberId,
            dataField: $selectedCompanyJson,
            noResultsText: "",
            itemRemoved: function () {
                $deal.tagsinput('destroy');
                self.InitDealsTagsInput();
            },
            itemAdded: function () {
                $deal.tagsinput('destroy');
                self.InitDealsTagsInput();
            }
        });

    };


    this.SetDealCompanyInTagsInput = function () {
        var companyId = getSelectedCompanyId();
        var dealId = 0;
        var selectedDeals = jQuery.parseJSON($selectedDealJson.val());
        if (selectedDeals.length > 0) {
            dealId = selectedDeals[0].id;
        }

        if (companyId === 0 && dealId > 0) {
            $.ajax({
                type: "GET",
                url: "/api/AutoComplete/GetDealCompany?dealId=" + dealId + "&subscriberId=" + activitySubscriberId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '',
                success: function (company) {
                    var companies = [];
                    companies.push(company);
                    $selectedCompanyJson.val(JSON.stringify(companies));

                    $company.tagsinput('destroy');
                    self.InitCompanyTagsInput();

                },
                beforeSend: function () {
                },
                error: function (request) {
                }
            });
        }
    };


};


var Task = function () {
    var self = this;

    this.Init = function () {


        self.InitTaskSaveValidator();
    };

    this.InitTaskSaveValidator = function () {

        // add the rule here
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            return (arg !== value && value !== null) && value !== null && value !== "0";
        }, "");

        // validate
        $divTaskSetup.validate({
            rules: {
                txtTaskTitle: { required: true },
                txtDueDate: {
                    required: true,
                    date: true
                }
            }, messages: {
                txtTaskTitle: translatePhrase("Please enter the task title"),
                txtDueDate:
                {
                    required: translatePhrase("Please select the due date"),
                    date: translatePhrase("Invalid Date")
                }
            },
            errorPlacement: function ($error, $element) {
                var name = $element.attr("name");
                $element.closest('.form-group').find(".error-text").append($error);
            },
            submitHandler: function () {
                self.SaveTask();
            }
        });
    };


    this.SaveTask = function () {

        if ($("#txtTagCompany").val() === "") {
            $("#txtTagCompany").closest('.form-group').find(".error-text").html("<label>Please select a company.</label>");
            return;
        } else {
            $("#txtTagCompany").closest('.form-group').find(".error-text").html("");
        }

        var taskModel = new Object();
        var task = new Object();
        var taskCompanies = [];
        var taskContacts = [];

        task.ActivityId = activityId;
        task.SubscriberId = subscriberId;
        task.UserId = userId;
        task.UserIdGlobal = userIdGlobal; 
        task.UpdateUserId = userId;
        task.TaskName = $("#txtTaskTitle").val();
        task.Description = $("#txtTaskDescription").val();
        task.DueDate = moment($("#txtDueDate").datepicker("getDate")).format("DD-MMM-YY");
        task.Completed = $("#chkCompleted").is(":checked");

        // set linked data
        if ($selectedDealJson.val() !== '') {
            var selectedDeals = jQuery.parseJSON($selectedDealJson.val());
            if (selectedDeals.length > 0) {
                task.DealIds = selectedDeals[0].id;
            }
        }

        // selected companies 
        if ($selectedCompanyJson.val() !== '') {
            var selectedCompanies = jQuery.parseJSON($selectedCompanyJson.val());
            if (selectedCompanies.length > 0) {
                $.each(selectedCompanies, function (i, company) {
                    var c = new Object();
                    c.CompanyIdGlobal = company.id;
                    taskCompanies.push(c);
                });
            }
        }

        // selected contacts
        if ($selectedContactJson.val() !== '') {
            var selectedContacts = jQuery.parseJSON($selectedContactJson.val());
            if (selectedContacts.length > 0) {
                $.each(selectedContacts, function (i, contact) {
                    var c = new Object();
                    c.ContactId = contact.id;
                    taskContacts.push(c);
                });
            }
        }

        // set final object
        taskModel.Task = task;
        taskModel.Companies = taskCompanies;
        taskModel.Contacts = taskContacts;

        // AJAX to save the task
        $.ajax({
            type: "POST",
            url: "/api/task/SaveTask",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(taskModel),
            success: function (taskId) {
                if (parseInt(taskId) > 0) {
                    // reload task with success message
                    //var url = updateQueryStringParameter(location.href, "isSuccess", "true");
                    //url = updateQueryStringParameter(url, "taskId", taskId);
                    try { parent.RefreshTasks(); } catch (e) { /*ignore*/ }
                    try { parent.RefreshActivities(); } catch (e) { /*ignore*/ }
                } else {
                    alert("Task Save Error");
                }

                swal.close();

            }, beforeSend: function () {
                //add loading message
                swal({ text: translatePhrase("Saving task") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: true });
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

};


function saveTask() {
    $divTaskSetup.submit();
}


function getSelectedCompanyId() {
    try {
        return $.parseJSON($selectedCompanyJson.val())[0].id;
    } catch (e) {
        /*ignore*/
    }
    return 0;
}


function deleteTask() {
    swal({
        title: translatePhrase("Delete Task!"),
        text: translatePhrase("Are you sure you want to delete this task?"),
        type: "error",
        showCancelButton: true,
        confirmButtonColor: "#f27474",
        confirmButtonText: translatePhrase("Yes, Delete!")
    }, function () {
        $.ajax({
            type: "GET",
            url: "/api/task/DeleteTask/?taskId=" + taskId + "&userId=" + userId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {},
            success: function (response) {
                if (response) {
                    swal.close();
                    parent.RefreshTasks();
                }
            },
            beforeSend: function () {
                swal({ text: translatePhrase("Please wait") + "...", title: "<img src='/_content/_img/loading_40.gif'/>", showConfirmButton: false, allowOutsideClick: false, html: true });

            },
            error: function () { }
        });
    });
}