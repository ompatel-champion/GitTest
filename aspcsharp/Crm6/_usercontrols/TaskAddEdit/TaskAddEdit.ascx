<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskAddEdit.ascx.cs" Inherits="Crm6._usercontrols.TaskAddEdit.WebUserControl1" %>

<!-- css custom -->
<link href="/_usercontrols/TaskAddEdit/taskaddedit-24-mar-2020.css" rel="stylesheet" />

<div class="modal inmodal" id="addTaskDialog" role="dialog" style="display: none;" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content animated fadeIn">
            <div class="modal-header">
                <h4 class="modal-title">New Task</h4>
                <button id="TaskAddEdit_btnTaskModalClose" type="button" class="close" data-dismiss="modal"><span aria-hidden="true">x</span></button>
            </div>
            <div class="modal-body white-bg" style="min-height: 300px;">
                <div class="ibox-content br-fields">
                    <div class="form-group">
                        <label class="inputLabel">Task Name</label>
                        <input id="TaskAddEdit_txtName" class="required" type="text" placeholder="">
                    </div>
                    <div class="form-group">
                        <label class="inputLabel">Description</label>
                        <textarea id="TaskAddEdit_txtDescription" class="required" placeholder=""></textarea>
                    </div>
                    <div class="task-fields">
                        <div class="row">
                            <div class="col-md-6 col-left-box">
                                <div class="form-group">
                                    <label class="inputLabel">Due Date</label>
                                    <input type="text" id="TaskAddEdit_txtDueDate" name="due-date" class="required" placeholder="" />
                                </div>
                            </div>
                            <div class="col-md-6 col-right-box">
                                <div class="form-group">
                                    <label class="inputLabel">Assigned To</label>
                                    <asp:DropDownList ID="ddlSalesRep" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row MB20">
                            <div class="col-md-12 ">
                                <div class="form-group">
                                    <label class="inputLabel">Company</label>
                                    <asp:DropDownList ID="ddlCompany" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-md-6 col-left-box">
                                <div class="form-group">
                                    <label class="inputLabel">Deal</label>
                                    <asp:DropDownList ID="ddlDeal" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-6 col-right-box">
                                <div class="form-group">
                                    <label class="inputLabel">Contact</label>
                                    <asp:DropDownList ID="ddlContact" CssClass="custom-select2" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                        </div>

                    </div>
                </div>
            </div>

            <div class="modal-footer footer-action">
                <button id="TaskAddEdit_btnTaskCancel" type="button" class="secondary-btn" data-dismiss="modal">Cancel</button>
                <div class="btn-wrp" id="task-add-actions">
                    <a id="TaskAddEdit_btnTaskAdd" class="primary-btn">Add Task</a>
                </div>
            </div>
        </div>
    </div>
</div>

<script src="/_content/_js/bundle/moment.js"></script>
<!-- js custom -->
<script src="/_usercontrols/TaskAddEdit/taskaddedit-06-apr-2020.js"></script>