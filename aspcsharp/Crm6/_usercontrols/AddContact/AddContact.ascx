<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddContact.ascx.cs" Inherits="Crm6._usercontrols.AddContact.AddContact" %>

<%--add contact dialog--%>
<div class="modal inmodal" id="addContactDialog" tabindex="-1" role="dialog" style="display: none;" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content animated fadeIn">
            <div class="modal-header">
                <h4 class="modal-title">Add Contact</h4>
                <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">x</span></button>
            </div>
            <div class="modal-body white-bg">
                <div class="form-group MT20 MB20">
                    <asp:DropDownList ID="ddlContacts" data-id="ddlContacts" CssClass="custom-select2" runat="server"></asp:DropDownList>
                    <input type="text" id="txtContactSearch" class="form-control W100P hide" placeholder="search">
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" id="btnAddContactSave" class="primary-btn">Save</button>
                <button type="button" class="secondary-btn" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<!-- control specific -->
<script src="/_usercontrols/AddContact/addcontact-apr-06-2020.js"></script>