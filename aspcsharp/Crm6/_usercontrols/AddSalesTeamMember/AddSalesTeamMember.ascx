<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddSalesTeamMember.ascx.cs" Inherits="Crm6._usercontrols.AddSalesTeamMember.AddSalesTeamMember" %>

<%--add sales team member dialog--%>
<div class="modal inmodal" id="addSalesTeamMemberDialog" tabindex="-1" role="dialog" style="display: none;" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content animated fadeIn">
            <div class="modal-header">
                <h4 class="modal-title">Add Sales Team Member</h4>
                <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">x</span></button>
            </div>
            <div class="modal-body white-bg">
                <div class="form-group filled MT20 MB20">
                    <label class="inputLabel language-entry">Sales Team Member</label>
                    <asp:DropDownList ID="ddlSalesTeamMember" data-id="ddlSalesTeamMember" CssClass="custom-select2" runat="server"></asp:DropDownList>
                    <span class="error-text"></span>
                </div>
                <div class="form-group filled MT20 MB20">
                    <label class="inputLabel language-entry">Role</label>
                    <asp:DropDownList ID="ddlSalesTeamRole" data-id="ddlSalesTeamRole" CssClass="custom-select2" runat="server"></asp:DropDownList>
                    <span class="error-text"></span>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" id="addSalesTeamButtonSave" class="primary-btn">Save</button>
                <button type="button" class="secondary-btn" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<!-- control specific -->
<script src="/_usercontrols/AddSalesTeamMember/addsalesteammember-apr-03-2020.js"></script>
