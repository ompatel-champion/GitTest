<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaskAddEdit.aspx.cs" Inherits="Crm6.Tasks.TaskAddEdit" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Add/Edit Task</title>

    <style>
        body {
            background-color: #ffffff;
            overflow-y: scroll;
        }
    </style>

</head>

<body>

    <form runat="server" id="divTaskSetup">
        <asp:Label CssClass="hide" ID="lblUserId" runat="server" Text="0"></asp:Label>
        <asp:Label Style="display: none" ID="lblUserIdGlobal" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblSubscriberId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblActivityId" runat="server" Text="0"></asp:Label>
        <asp:Label CssClass="hide" ID="lblCompletePercentage" runat="server" Text="0"></asp:Label> 
        <asp:Label CssClass="hide" ID="lblActivitySubscriberId" runat="server" Text="0"></asp:Label>

        <div class="ibox ibox-content">
            <div class="col-sm-12">
                <div class="alert alert-success hide task-success language-entry">Task saved successfully!</div>
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="col-sm-12">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtTaskTitle" placeholder="Title" MaxLength="100"></asp:TextBox>
                            <span class="error-text"></span>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-12">
                            <asp:TextBox CssClass="form-control" runat="server" ID="txtTaskDescription" placeholder="Description" TextMode="MultiLine" Rows="4" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-6">
                            <div class="input-group">
                                <span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                <asp:TextBox CssClass="form-control" runat="server" data-name="date" ID="txtDueDate" placeholder="due date..."></asp:TextBox>
                            </div>
                            <div class="clearfix"></div>
                            <div style="padding-left: 15px;">
                                <span class="error-text"></span>
                            </div>
                        </div>
                    </div>
                    <div class="form-group m-t-md">
                        <div class="col-sm-12 ">
                             <label class="control-label">
                                <asp:CheckBox ID="chkCompleted" runat="server" CssClass="i-checks" />
                                <span class="font-noraml m-l-xs">Completed</span>
                            </label> 
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-12">
                            <i class="fa icon-business"></i><label>Company</label>
                            <asp:TextBox ID="txtTagCompany" runat="server" CssClass="form-control" placeholder="Company..."></asp:TextBox>
                            <asp:TextBox ID="txtTagCompanyId" runat="server" CssClass="form-control hide"></asp:TextBox>
                            <span class="error-text"></span>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-12">
                            <i class ="icon-Deal----Active"></i><label>Deal</label>
                            <asp:TextBox ID="txtTagDeal" runat="server" CssClass="form-control" placeholder="Deal..."></asp:TextBox>
                            <asp:TextBox ID="txtTagDealId" runat="server" CssClass="form-control hide"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-12">
                              <i class ="icon-Contact---Active"></i><label>Contacts</label> 
                            <asp:TextBox ID="txtTagContact" runat="server" CssClass="form-control" placeholder="Contact..."></asp:TextBox>
                            <asp:TextBox ID="txtTagContactId" runat="server" CssClass="form-control hide"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="clearfix"></div>
        </div>
    </form>
    <script src="TaskAddEdit-14-mar-2020.js"></script>

</body>
</html>
