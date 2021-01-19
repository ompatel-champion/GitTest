<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailTabTasks.ascx.cs" Inherits="Crm6._usercontrols.DetailTabTasks.DetailTabTasks" %>

<!-- css custom -->
<link href="/_usercontrols/DetailTabTasks/detailtabtasks-08-apr-2020.css" rel="stylesheet" />

<!--#tab-tasks-->
<div class="tab-pane" id="tab-tasks">
    <div class="row">

        <!--left hand tasks list-->
        <div class="col-md-7 col-left-box">
            <div class="task-listing basic-card">
                <div class="ibox MB0 fullHeight">
                    <div class="ibox-title clearfix">
                        <div class="title-wrap FL">
                            <h3 class="card-title language-entry"><i class="icon-task title-icon"></i>Tasks</h3>
                        </div>
                        <div class="ibox-tools FR">
                            <!--Active Completed buttons-->
                            <div class="filter-wrap">
                                <a data-value="Active" id="aActiveTasks" class="task-btn active-task deals-link active">Active</a>
                                <a data-value="Completed" id="aCompletedTasks" class="task-btn completed-task deals-link">Completed</a>
                            </div>
                        </div>
                    </div>

                    <!--#divTasks-->
                    <div class="tasks" id="divTasks">
                        <form class="task-form">
                            <!-- #active-tasks -->
                            <div id="active-task" class="task-items"></div>
                            <!-- #completed-task -->
                            <div id="completed-task" class="task-items hide"></div>
                        </form>
                    </div>
                    <!--#divTasks-->
                </div>
            </div>
        </div>
        <!--left hand tasks list-->

        <!--right hand add / edit task form-->
        <div class="col-md-5 col-right-box">
            <div class="ibox basic-card add-task-wrp">
                <div class="ibox-title">
                    <h3 class="card-title save-task-header language-entry">New Task</h3>
                </div>
                <div class="ibox-content">
                    <form class="add-task" action="#">
                        <div class="row">
                            <div class="col-md-12 col-box">
                                <div class="form-group filled">
                                    <label class="inputLabel">Task Name</label>
                                    <input id="txtTaskTitle" class="task-name" type="text" name="task_name">
                                </div>
                            </div>
                            <div class="col-md-12 col-box">
                                <div class="form-group filled">
                                    <label class="inputLabel">Description</label>
                                    <textarea id="txtTaskDescription" class="task-descp"></textarea>
                                </div>
                            </div>
                        </div>
                        <div class="task-fields">
                            <div class="row">
                                <div class="col-md-6 col-left-box">
                                    <div class="form-group filled">
                                        <label class="inputLabel">Due Date</label>
                                        <input type="text" id="txtDueDate" name="due_date"  />
                                    </div>
                                </div>
                                <div class="col-md-6 col-right-box">
                                    <div class="form-group filled">
                                        <label class="inputLabel">Assigned</label>
                                        <asp:DropDownList ID="ddlTaskSalesReps" data-id="ddlTaskSalesReps" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div id="typeFieldsGroupDeal" class="row" runat="server">
                                <div class="col-md-12 col-box">
                                    <div class="form-group filled">
                                        <label class="inputLabel">Contact</label>
                                        <asp:DropDownList ID="ddlTaskRelatedContactForDeal" data-id="ddlTaskRelatedContactForDeal" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div id="typeFieldsGroupCompany" class="row" runat="server">
                                <div class="col-md-6 col-left-box">
                                    <div class="form-group filled">
                                        <label class="inputLabel">Deal</label>
                                        <asp:DropDownList ID="ddlTaskRelatedDealForCompany" data-id="ddlTaskRelatedDealForCompany" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-6 col-right-box">
                                    <div class="form-group filled">
                                        <label class="inputLabel">Contact</label>
                                        <asp:DropDownList ID="ddlTaskRelatedContactForCompany" data-id="ddlTaskRelatedContactForCompany" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div id="typeFieldsGroupContact" class="row" runat="server">
                                <div class="col-md-12 col-box">
                                    <div class="form-group filled">
                                        <label class="inputLabel">Deal</label>
                                        <asp:DropDownList ID="ddlTaskRelatedDealForContact" data-id="ddlTaskRelatedDealForContact" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="btn-wrp" id="task-add-actions">
                            <a class="primary-btn" id="btnAddTask">Add Task</a>
                        </div>
                        <div class="btn-wrp hide" id="task-edit-actions">
                            <a href="javascript:void(0)" class="primary-btn btnSaveTask">Save Task</a>
                            <a href="javascript:void(0)" class="primary-btn btnTaskCancel">Cancel</a>
                            <div class="clearfix"></div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>
<!--#tab-tasks-->

<!-- control specific -->
<script src="/_usercontrols/DetailTabTasks/detailtabtasks-31-mar-2020.js"></script>