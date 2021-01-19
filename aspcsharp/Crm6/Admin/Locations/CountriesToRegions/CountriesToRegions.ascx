<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CountriesToRegions.ascx.cs" Inherits="Crm6.Admin.CountriesToRegions.CountriesToRegionsControl" %>

<div class="form-group form-group-sales-reps" id="divManagerCountriesRegions">
    <asp:Label ID="lblSelectedManagerCountriesRegions" CssClass="hide" runat="server" Text=""></asp:Label>

    <div>
        <div class="row">
            <div class="col-md-5">
            </div>
            <div class="col-md-2">
            </div>
            <div class="col-md-5">
                <label class="control-label language-entry">Regions</label>
                <asp:DropDownList runat="server" ID="ddlRegions" />
            </div>
        </div>

        <div class="row" id="multiselect-container">

            <div class="col-md-5">
                <label class="control-label language-entry">Countries With <b><u>NO</u></b> Regions Selected</label>
                <select name="from" id="multiselect" class="form-control" size="8" multiple="multiple">
                </select>
            </div>
            <div class="col-md-2 PT50 TAC">
                <button type="button" id="multiselect_rightSelected" class="MT5 primary-btn">></button>
                <button type="button" id="multiselect_rightAll" class="MT5 primary-btn">>></button>
                <button type="button" id="multiselect_leftSelected" class="MT5 primary-btn"><</button>
                <button type="button" id="multiselect_leftAll" class="MT5 primary-btn"><<</button>
            </div>
            <div class="col-md-5">
                <label class="control-label language-entry">Region Countries</label>
                <select name="to" id="multiselect_to" class="form-control" size="8" multiple="multiple">
                </select>
            </div>
        </div>
        <span class="error-text"></span>
    </div>
    <div class="row buttons no-gutters">
        <div class="col-auto">
            <div class="form-btns">
                <button type="button" class="primary-btn" id="btnSave">Save</button>
            </div>
        </div>
    </div>

</div>
