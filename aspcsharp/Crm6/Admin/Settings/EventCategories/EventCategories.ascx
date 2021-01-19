<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventCategories.ascx.cs" Inherits="Crm6.Admin.EventCategories.EventCategories" %>



                                    <div id="divEventCategories">
                                        <div class="ibox">
                                            <div class="ibox-content">
                                                <div id="items" class="list-table MT15">
                                                    <table class="table table-condensed" id="companyEventCategories">
                                                        <thead>
                                                            <tr>
                                                                <th class="text-uppercase"><span class="">Color</span></th>
                                                                <th class="text-uppercase"><span class="">Category Name</span></th>
                                                                <th></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater runat="server" ID="rptCompanyEventCategories">
                                                                <ItemTemplate>
                                                                    <tr data-id='<%# Eval("EventCategoryId") %>' data-current-event-category="<%# Eval("EventCategoryId") %>">
                                                                        <td class="color-picker">
                                                                            <asp:TextBox CssClass="form-control color-name" Text='<%# Eval("CategoryColor") %>' runat="server"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox CssClass="form-control category-name" Text='<%# Eval("CategoryName") %>' runat="server"></asp:TextBox>
                                                                        </td>
                                                                        <td class="text-center">
                                                                            <a class="delete-item" data-action="delete"><i class="icon-Delete"></i></a>
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>
                                                    </table>
                                                </div>
                                                <%-- new event category --%>
                                                <div class="new-category form-inline">
                                                    <div class="form-group">
                                                        <div class="color-picker"><asp:TextBox ID="txtNewColorCode" CssClass="form-control color-name" placeholder='Color Code' runat="server"></asp:TextBox></div>
                                                        <asp:TextBox ID="txtNewCategory" CssClass="form-control" placeholder='Category' runat="server"></asp:TextBox>
                                                        <a class="btn btn-primary" id="btnAddCategory">Add Event Category</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

