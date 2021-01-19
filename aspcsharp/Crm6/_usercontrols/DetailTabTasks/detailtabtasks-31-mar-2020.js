//tasks tab (deals, companies, contact detail pages)
FFGlobal.docs.detail.tabs.tasks = function(props) {
            
    var self = this;
    self.type = props.type;
    self.data = props.data||{};
    var $activeTasks = $("#active-task");
    var $completedTasks = $("#completed-task");
    var $li = $("li[data-type='tasks']");
    var $noteAddTaskWrap = $("#tab-tasks").find(".add-task");
    var $saveTaskHeader = $(".save-task-header");
    var $taskAddActions = $("#task-add-actions");
    var $taskEditActions = $("#task-edit-actions");
    var $tabtasks = $("#tab-tasks");
    var taskType = "active";
    var $dropDownLists;

    self.init = function() {
        _init_drop_downs();
        _init_date_picker();        
        _attach_event_listeners();
        self.retrieveTasks();

        function _init_drop_downs() {
            var capType = FFGlobal.utils.string.capitalize(self.type);
            $dropDownLists = {
                salesRep: $('[data-id="ddlTaskSalesReps"]'),
                deal: $('[data-id="ddlTaskRelatedDealFor'+capType+'"]'),
                contact: $('[data-id="ddlTaskRelatedContactFor'+capType+'"]')
            }
            $dropDownLists.salesRep.select2({ placeholder: "Sales Rep", allowClear: false });
            $dropDownLists.deal.select2({ placeholder: "Deal", allowClear: false });
            $dropDownLists.contact.select2({});
        }

        function _attach_event_listeners() {
            $("#btnAddTask").unbind("click").click(function () {
                self.saveTask();
            });
            // edit save task function
            $(".btnSaveTask").unbind('click').click(function () {
                self.saveTask();
            });
            // cancel save task
            $(".btnTaskCancel").unbind('click').click(function () {
                self.resetFields();
                $noteAddTaskWrap.attr("task-id", 0);
                $taskAddActions.removeClass("hide");
                $taskEditActions.addClass("hide");
                $saveTaskHeader.html("New Task");
            });
            $('.filter-wrap a').unbind("click").click(function (e) {
                e.preventDefault();
                self.showTasksActiveOrCompleted($(this));
            });
            $("[data-action='add-task']").unbind('click').click(function () {
                self.OpenTaskAddEditDialog(0);
            });
        }

        function _init_date_picker() {
            $("#txtDueDate").datepicker({
                showOn: "both",
                inline: true,
                beforeShow: function (input, inst) {
                    $('#ui-datepicker-div').addClass("due_datepicker");
                },
                buttonText: "<i class='icon-calendar'></i>"
            });
            $("#txtDueDate").on('focus', function (e) {
                e.preventDefault();
                $(this).attr("autocomplete", "off");
            });
            if ($(".custom-select2").length > 0) {
                $(".custom-select2").select2({ allowClear: true, placeholder: "" });
            }
        }
    }

    self.showTasksActiveOrCompleted = function ($currentAtag) {
        $currentAtag.parent().find('a').removeClass('active');
        $currentAtag.addClass('active');
        taskType = $currentAtag.attr("data-value").toLowerCase()||'active';
        var $tasks = taskType==="active"?$activeTasks:$completedTasks;
        $tasks.removeClass("hide");
        (taskType==="active"?$completedTasks:$activeTasks).addClass("hide");
        if (!$tasks.hasClass("data-loaded")) self.retrieveTasks();
    };

    self.retrieveTasks = function () {
        var data = self.data;
        var request = new Object();
        request.SubscriberId = data.subscriberId;
        request.ContactId = data.contactId;
        request.CompanyIdGlobal = data.globalCompanyId;
        request.LoggedinUserId = data.userId;
        request.UserId = data.userId;
        request.RecordsPerPage = 20;
        request.CurrentPage = 1;
        request.SortBy = "createddate desc";
        request.Completed = taskType !== "active";
        var $tasks = taskType==="active"?$activeTasks:$completedTasks;
        $.ajax({
            type: "POST",
            url: "/api/Task/GetTasks",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (tasks) {
                $tasks.html("");
                $li.attr("data-loaded", true);
                $tasks.addClass("data-loaded");
                if (tasks.length > 0) self.bindTasks(tasks, $tasks);
                else self.addNoItems($tasks);
            },
            beforeSend: function () {
                $tasks.html('<div class="text-center data-loader"><img src="/_content/_img/loading_20.gif" /></div>');
            },
            error: function () { }
        });
    };

    self.bindTasks = function (tasks, $currentDiv) {
        $.each(tasks, function (i, task) {
            var $taskItem = self.getTaskItemHtml(task, i);
            $currentDiv.append($taskItem);
        });
    };

    self.getTaskItemHtml = function (task, i) {
        var $outerWrap = $("<div />", { "class": "task-wrap", "data-id": task.ActivityId });

        // inner wrap
        var $innerWrap = $("<div />", { "class": "task-info clearfix" });
        $outerWrap.append($innerWrap);

        // left box
        var $leftBox = $("<div />", { "class": "left-tbox FL" });
        $innerWrap.append($leftBox);

        // due date
        var $dueDate = $("<div />", { "class": "task-day" });
        $dueDate.append($("<span/>", { "html": moment(task.DueDate).format("ddd, DD MMMM, YYYY") }));
        $leftBox.append($dueDate);

        // completed check box
        var chbId = "chb"+task.ActivityId;
        var $input = $("<input/>", {
            "id": chbId,
            "type": "checkbox",
            "checked": task.Completed
        });
        $leftBox.append($input);
        $input.change(function(e) {
            var $item = $(e.currentTarget).closest(".task-wrap");
            var id = $item.attr("data-id");
            self.toggleTaskCompleted(e.currentTarget.checked, id, $item);
        });

        // title
        var $lableTitle = $("<label/>", { "for": chbId });
        var $aTitle = $("<a />", { "class": "task-title hover-link", "href": "javascript:void(0)", "html": task.TaskName });
        $lableTitle.append($aTitle);
        $aTitle.unbind("click").click(function () {
            self.showEditControls(task);
        });
        $leftBox.append($lableTitle);

        // description
        var $description = $("<div />", { "class": "task-content" });
        $description.append($("<span/>", { "html": task.Description }));
        $leftBox.append($description);

        // right box
        var $rightBox = $("<div />", { "class": "right-tbox FR" });
        $innerWrap.append($rightBox);

         // company name
        if (/contact|deal/.test(self.type)) {
            if (task.CompanyName && task.CompanyName !== '') {
                $address = $("<div />", { "class": "ta-adds" });
                $addsTitle = $("<div />", { "class": "ta-title", "html": task.CompanyName });
                $address.append($addsTitle);
                $rightBox.append($address);
            }
        }

        // contacts / invites
        if (/company|deal/.test(self.type)) {
            if (task.ContactNames && task.ContactNames !== '') {
                $guests = $("<div />", { "class": "ta-guest-wrp" });
                $guestTtl = $("<div />", { "class": "ta-gtitle", "html": task.ContactNames });
                $guests.append($guestTtl);
                $rightBox.append($guests);
            }
        }

        // edit button
        if (!task.Completed) {
            var $editTask = $("<a />", { "class": "edit-link MT5", "href": "javascript:void(0)", "data-action": "edit", "html": "<i class='icon-edit edit-icon'></i>" });
            $outerWrap.append($editTask);
            $editTask.unbind("click").click(function () {
                self.showEditControls(task);
            });
        }

        // delete link
        var $aDelete = $("<a/>", { "href": "javascript:void(0)", "class": "delete-link MT5", "data-action": "delete-task" });
        $aDelete.append($("<i/>", { "class": "icon-Delete" }));
        //  $aDelete.append($("<span/>", { "html": "delete" }));
        $aDelete.unbind("click").click(function () {
            self.deleteTask(task.ActivityId, $outerWrap);
        });
        $outerWrap.append($aDelete);

        return $outerWrap;
    };

    self.addNoItems = function ($currentDiv) {
        $currentDiv.append($('<div class="no-tasks empty-box tableDisplay"><div class="tableCell"><i class="icon-task"></i><p class="e-text">no tasks</p></div></div>'));
    };

    self.showEditControls = function (t) {
        $.ajax({
            type: "GET",
            url: "/api/Task/GetTask?taskId=" + t.ActivityId + "&subscriberId=" + t.SubscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '',
            success: function (task) {
                removeSpinner();
                if (task !== null) {
                    $taskAddActions.addClass("hide");
                    $taskEditActions.removeClass("hide");
                    $noteAddTaskWrap.attr("task-id", task.Task.ActivityId);
                    $("#txtTaskTitle").val(task.Task.TaskName);
                    $("#txtTaskDescription").val(task.Task.Description);
                    $saveTaskHeader.html("Edit Task");
                    $('#txtDueDate').datepicker("setDate", moment(task.Task.DueDate).toDate());
                    $dropDownLists.salesRep.val(task.Task.OwnerUserIdGlobal).trigger("change");
                    if (/contact|company/.test(self.type) && task.Task.DealIds) $dropDownLists.deal.val(task.Task.DealIds).trigger("change");
                    if (/deal|company/.test(self.type) && task.Task.ContactIds) $dropDownLists.contact.val(task.Task.ContactIds).trigger("change");
                }
            },
            beforeSend: function () {
                addSpinner();
            },
            error: function () {
                swal.close();
            }
        });
    };

    self.removeTaskItemElm = function ($item) {
        $item.fadeOut("slow", function () {
            $item.remove();
            var $tasks = taskType === "active" ? $activeTasks : $completedTasks;
            if ($tasks.find(".task-wrap").length === 0) self.addNoItems($tasks);
            (taskType === "active" ? $completedTasks : $activeTasks).removeClass("data-loaded");
        });
    };

    self.toggleTaskCompleted = function (state, taskId, $item) {
        $.ajax({
            type: "GET",
            url: "/api/task/ToggleTaskCompleted/?"+
                "&taskId="+ taskId +
                "&state=" + state +
                "&userId=" + userId +
                "&subscriberId=" + subscriberId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {},
            success: function (response) {
                if (response) {
                    self.removeTaskItemElm($item);
                    self.resetFields();
                }
                removeSpinner();
            },
            beforeSend: function () {
                addSpinner();
            },
            error: function () {
                removeSpinner();
            }
        });
    };

    self.deleteTask = function (taskId, $item) {
        swal({
            title: translatePhrase("Delete Task!"),
            text: translatePhrase("Are you sure you want to delete this task?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/task/DeleteTask/?taskId=" +
                        taskId +
                        "&userId=" +
                        userId +
                        "&subscriberId=" +
                        self.data.subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        if (response) self.removeTaskItemElm($item);
                        removeSpinner();
                    },
                    beforeSend: function () {
                        addSpinner();
                    },
                    error: function () {
                        removeSpinner();
                    }
                });
            }
        });
    };

    self.saveTask = function () {
        if ($("#txtTaskTitle").val() === "") {
            swal({ title: "Please enter the task title!", type: "warning", showCancelButton: false });
            return;
        }
        if ($("#txtDueDate").val() === "") {
            swal({ title: "Please select the due date!", type: "warning", showCancelButton: false });
            return;
        }
        if (!moment($("#txtDueDate").val()).isValid()) {
            swal({ title: "Invalid due date!", type: "warning", showCancelButton: false });
            return;
        }
        if ($dropDownLists.salesRep.val() === "" || $dropDownLists.salesRep.val() === "0") {
            swal({ title: "Please select the sales rep!", type: "warning", showCancelButton: false });
            return;
        }
        var data = self.data;
        var task = new Object();
        task.ActivityId = parseInt($noteAddTaskWrap.attr("task-id"));
        task.SubscriberId = subscriberId;
        task.UserIdGlobal = globalUserId;
        task.UpdateUserIdGlobal = globalUserId;
        task.OwnerUserIdGlobal = parseInt($dropDownLists.salesRep.val());
        task.TaskName = $("#txtTaskTitle").val();
        task.Description = $("#txtTaskDescription").val();
        task.DueDate = moment($("#txtDueDate").datepicker("getDate")).format("DD-MMM-YY");
        task.DealIds = self.type=="deal" ? data.dealId : $dropDownLists.deal.val() === "" ? 0 : parseInt($dropDownLists.deal.val());
        task.CompanyId = companyId;
        task.CompanyIdGlobal = globalCompanyId;
        var taskModel = {
            Task: task,
            Invites: [{
                InviteType: "contact",
                AttendeeType: "Required",
                ContactId: data.contactId,
                ContactName: data.contactName,
                SubscriberId: data.subscriberId
            }]
        }
        if (/company|deal/.test(self.type)) {
            taskModel.Invites[0].ContactId = $dropDownLists.contact.val();
            taskModel.Invites[0].ContactName = $dropDownLists.contact.select2('data')[0].text;
        }
        $.ajax({
            type: "POST",
            url: "/api/task/SaveTask",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(taskModel),
            success: function (taskId) {
                removeSpinner();
                if (parseInt(taskId) > 0) {
                    self.resetFields();
                    $noteAddTaskWrap.attr("task-id", 0);
                    $taskAddActions.removeClass("hide");
                    $taskEditActions.addClass("hide");
                    // load tasks
                    $activeTasks.removeClass("data-loaded");
                    self.showTasksActiveOrCompleted($tabtasks.find('.filter-wrap').find('.active-task'));

                } else {
                    alert("Task Save Error");
                }
                swal.close();
            }, beforeSend: function () {
                addSpinner();
            }, error: function (request, status, error) {
                alert(JSON.stringify(request));
            }
        });
    };

    self.resetFields = function() {
        $("#txtTaskTitle").val("");
        $("#txtTaskDescription").val("");
        $("#txtDueDate").val("");
        if (/deal|company/.test(self.type)) $dropDownLists.contact.val("").trigger("change");
        if (/contact|company/.test(self.type)) $dropDownLists.deal.val("").trigger("change");
        $dropDownLists.salesRep.val(globalUserId).trigger("change");
    }
}