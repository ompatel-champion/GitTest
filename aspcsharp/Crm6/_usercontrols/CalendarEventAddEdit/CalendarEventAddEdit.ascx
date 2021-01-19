<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarEventAddEdit.ascx.cs"
    Inherits="Crm6._usercontrols.CalendarEventAddEdit.CalendarEventAddEdit" %>

<!-- css libraries -->
<link href="/_content/_css/fullcalendar/fullcalendar.css" rel="stylesheet">
<link href="/_content/_css/fullcalendar/fullcalendar.print.css" rel='stylesheet' media='print'>
<link href="/_content/_css/bundle/jquery.timepicker.css" rel="stylesheet" />
<link href="/_content/_css/summernote-lite.css" rel="stylesheet" />
<link href="/_content/_css/bundle/dropzone.css" rel="stylesheet" />

<!-- css custom -->
<link href="/_usercontrols/CalendarEventAddEdit/calendareventaddedit-24-mar-2020.css" rel="stylesheet" />

<div runat="server" class="hide section-event-edit cal-evpg">

    <!-- #content -->
    <div id="content">

        <!-- .top-header -->
        <header class="top-header globalHead">
            <h1 class="page-title">New Event</h1>
            <div class="closeX" data-action="cancel-event"></div>
        </header>
        <!-- .top-header -->


        <div class="wrapper">
            <!--.section-content-->
            <div class="section-content cal-page">
                <div class="section-body hidden" id="divCalendarEventWrapper">
                    <div class="wrapper wrapper-content">

                        <div class="row">

                            <!-- .left-content -->
                            <div class="left-content col-md-8">


                                <div class="row">
                                    <!-- .left-box start -->
                                    <div class="left-box col-lg-6 col-md-12">
                                        <div class="form-group filled">
                                            <label class="inputLabel">Event Title</label>
                                            <input type="text" class="input-field" id="txtEventTitle" maxlength="100" />
                                            <span class="error-text"></span>
                                        </div>

                                        <div class="form-group filled" id="divEventCompanyContainer">
                                            <label class="inputLabel">Company</label>
                                            <select id="ddlCompany">
                                                <option value=""></option>
                                            </select>
                                        </div>
                                    </div>

                                    <!-- .right-box start -->
                                    <div class="right-box col-lg-6 col-md-12">
                                        <div class="form-group filled">
                                            <label class="inputLabel">Event Type</label>
                                            <select id="ddlCategory" class="form-control" style="font-family: Arial, FontAwesome;color:#006696;">
                                                <option>Select Event Type</option>
                                            </select>
                                            <span class="error-text"></span>
                                        </div>

                                        <div class="form-group filled">
                                            <label class="inputLabel">Location</label>
                                            <input type="text" class="input-field" id="txtLocation" maxlength="100">
                                        </div>
                                    </div>

                                    <div class="right-box col-lg-12 col-md-12 hide" id="divDealContainer">
                                        <div class="form-group filled">
                                            <label class="inputLabel">Deals</label>
                                            <span class="hdnDealIds"></span>
                                            <select id="ddlDeal" multiple="true">
                                                <option value=""></option>
                                            </select>
                                        </div>
                                    </div>

                                </div>

                                <div class="row">

                                    <!-- .left-box start -->
                                    <div class="left-box col-lg-6 col-md-12">

                                        <!-- start date time -->
                                        <div class="row">
                                            <div class="col-md-5 col-left-box">
                                                <div class="form-group filled">
                                                    <label class="inputLabel">Start</label>
                                                    <input type="text" data-name="date" class="input-field date-picker" id="txtStartDate" placeholder="">
                                                    <span class="error-text"></span>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-mid-box timeBox">
                                                <div class="form-group filled">
                                                    <label class="inputLabel">Time</label>
                                                    <i class="icon-clock"></i>
                                                    <input type="text" data-name="time" id="ddlStartTime" placeholder="" class="input-field time-picker ui-timepicker-input" autocomplete="off">
                                                    <span class="error-text"></span>
                                                </div>
                                            </div>
                                            <div class="col-md-4 col-right-box">
                                                <div class="form-group">
                                                    <div class="chk-box-wrp">
                                                        <input type="checkbox" id="chkAllDay" />
                                                        <label for="chkAllDay"><a href="javascript:void(0)">All day</a></label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- end date time -->
                                        <div class="row">
                                            <div class="col-md-5 col-left-box">
                                                <div class="form-group filled">
                                                    <label class="inputLabel">End</label>
                                                    <input type="text" data-name="date" class="input-field date-picker" id="txtEndDate" placeholder="">
                                                    <span class="error-text"></span>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-mid-box timeBox">
                                                <div class="form-group filled">
                                                    <label class="inputLabel">Time</label>
                                                    <i class="icon-clock"></i>
                                                    <input type="text" data-name="time" id="ddlEndTime" placeholder="" class="input-field time-picker ui-timepicker-input" autocomplete="off">
                                                    <span class="error-text"></span>
                                                </div>
                                            </div>
                                            <div class="col-md-4 col-right-box">
                                                <div class="form-group ">
                                                    <div class="chk-box-wrp">
                                                        <input type="checkbox" id="repeats" />
                                                        <label for="repeats"><span class="doc-title">Repeats</span></label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- repeats -->
                                        <div class="form-group ddRepeat">
                                            <label class="inputLabel">Repeats</label>
                                            <select id="ddlRepeat">
                                                <option value="Daily">Daily</option>
                                                <option value="Weekly">Weekly</option>
                                                <option value="Monthly">Same Day Each Month</option>
                                            </select>
                                        </div>

                                        <!-- reminder -->
                                        <div class="row">
                                            <div class="col-md-6 col-left-box">
                                                <div class="form-group filled ddlReminder">
                                                    <label class="inputLabel">Reminder</label>
                                                    <select id="ddlReminder" class="form-control">
                                                        <option value=""></option>
                                                        <option value="5">5 Minutes</option>
                                                        <option value="10">10 Minutes</option>
                                                        <option value="15">15 Minutes</option>
                                                        <option value="30">30 Minutes</option>
                                                        <option value="60">1 hour</option>
                                                    </select>
                                                </div>
                                            </div>
                                            <div class="col-md-6 col-right-box">
                                                <div class="form-group remindCheckboxes">
                                                    <div class="chk-box-wrp">
                                                        <input type="checkbox" id="Text" />
                                                        <label for="Text"><span class="doc-title">Text</span></label>
                                                    </div>
                                                    <div class="chk-box-wrp">
                                                        <input type="checkbox" id="Email" />
                                                        <label for="Email"><span class="doc-title">Email</span></label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- visibility -->
                                        <div class="row">
                                            <div class="col-md-6 col-left-box">
                                                <div class="form-group filled">
                                                    <label class="inputLabel">Visibility</label>
                                                    <select id="ddlPublicPrivate" class="form-control">
                                                        <option value=""></option>
                                                        <option value="Public">Public</option>
                                                        <option value="Private">Private</option>
                                                    </select>
                                                    <span class="error-text"></span>
                                                </div>
                                            </div>
                                            <div class="col-md-6 col-right-box">
                                                <div class="form-group filled">
                                                    <label class="inputLabel">Appear As</label>
                                                    <select id="ddlBusyFree" class="form-control">
                                                        <option value=""></option>
                                                        <option value="Busy">Busy</option>
                                                        <option value="Free">Free</option>
                                                    </select>
                                                    <span class="error-text"></span>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Notifications -->
                                        <div class="form-group ddNotify hide">
                                            <div class="chk-box-wrp">
                                                <input type="checkbox" id="sendNotifications"><label for="sendNotifications"><div class="doc-title">Send Notification</div>
                                                </label>
                                            </div>
                                            <div class="notify-box">
                                                <div class="row">
                                                    <div class="col-md-4 col-4">
                                                        <input type="text" class="input-field" id="txtSN" placeholder="15" maxlength="100">
                                                    </div>
                                                    <div class="col-md-4 col-4 pl-0">
                                                        <select id="ddlSNtime">
                                                            <option value="Minutes">Minutes</option>
                                                            <option value="Hours">Hours</option>
                                                        </select>
                                                    </div>
                                                    <div class="col-md-4 col-4 pl-0">
                                                        <div class="chk-box-wrp">
                                                            <input type="checkbox" id="snCheckbox"><label for="snCheckbox"><div class="doc-title">Notification</div>
                                                            </label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-4 col-4 ">
                                                        <input type="text" class="input-field" id="txtSE" placeholder="15" maxlength="100">
                                                    </div>
                                                    <div class="col-md-4 col-4 pl-0">
                                                        <select id="ddlSEtime">
                                                            <option value="Minutes">Minutes</option>
                                                            <option value="Hours">Hours</option>
                                                        </select>
                                                    </div>
                                                    <div class="col-md-4 col-4 pl-0">
                                                        <div class="chk-box-wrp">
                                                            <input type="checkbox" id="seCheckbox"><label for="seCheckbox"><div class="doc-title">Email</div>
                                                            </label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- attachments -->
                                        <div class="section-attachments">
                                            <a data-action="event-attachments" class="a-event-attachments"><i class="note-icon-link"></i>&nbsp;&nbsp;Attachments</a>
                                            <input data-action="fupload-event-attachments" name="fuUploadDocument" type="file" class="hide">
                                            <ul class="attachments"></ul>
                                        </div>

                                    </div>
                                    <!-- .left-box end -->

                                    <!-- .right-box start -->
                                    <div class="right-box  col-lg-6 col-md-12">

                                        <!-- .user-container -->
                                        <div class="user-container">
                                            <div class="header">
                                                <select id="ddlInvites" class="form-control"></select>
                                            </div>

                                            <div class="clearfix"></div>

                                            <!-- .user-list -->
                                            <div class="users-list">
                                                <table class="table table-hover" id="tblInvites">
                                                    <tbody></tbody>
                                                </table>
                                            </div>
                                            <!-- .user-list -->
                                        </div>
                                        <!-- .user-container -->

                                    </div>
                                    <!-- .right-box end -->
                                </div>
                                <!-- .row -->

                                <!-- .row -->
                                <div class="row">
                                    <!-- .bottom-box -->
                                    <div class="col-md-12 bottom-box">
                                        <textarea id="txtEventDescription" name="txtEventDescription" class="input-field summernote"></textarea>
                                    </div>
                                </div>
                                <!-- .row -->
                            </div>
                            <!-- .left-content -->

                            <!-- .right-content -->
                            <div class="right-content col-md-4 col-right-box">
                                <!-- .event-edit-calendar -->
                                <div class="event-edit-calendar ev-fcal calendar"></div>
                            </div>
                            <!-- .right-content -->

                        </div>
                    </div>

                    <!-- .footer-box -->
                    <div class="footer-box">
                        <div class="row">
                            <div class="col-md-12">
                                <button type="button" class="primary-btn save-btn" id="btnSave" data-action="save-event">Save</button>
                                <button type="button" class="secondary-btn cancel-btn" data-action="cancel-event">Cancel</button>
                                <button type="button" class="text-danger delete-btn secondary-btn hide" data-action="delete-event">Delete</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<!-- js libraries -->
<script src="/_content/_js/datepair.js"></script>
<script src="/_content/_js/bundle/jquery.timepicker.js"></script>
<script src="/_content/_js/summernote.js"></script>
<script src="/_content/_js/summernote-lite.js"></script>
<script src="/_content/_js/bundle/dropzone.js"></script>
<script src="/_content/_js/jquery.slimscroll.min.js"></script>
<script src="/_content/_js/bundle/moment.js"></script>
<script src="/_content/_js/fullcalendar.min.js"></script>

<!-- js custom -->
<script src="/_usercontrols/CalendarEventAddEdit/calendareventaddedit-08-apr-2020.js"></script>
