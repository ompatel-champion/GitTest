<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="Crm6.ResetPassword" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reset Password</title>
    <!-- css libraries -->
    <link href="_content/_css/bundle/bootstrap.min.css" rel="stylesheet" />
    <link href="_content/_css/bundle/animate.min.css" rel="stylesheet" />
    <link href="_content/_css/bundle/style-07-apr-2020.css " rel="stylesheet" />
</head>

<body class="act-formpg reset-fpage">

    <form runat="server">
        <div class="passwordBox animated fadeInDown container">
            <div class="form-table">
                <div class="form-cell">

                    <div class="row">
                        <div class="col-md-6 logo-holder">
                            <img src="_content/_img/logo.svg" runat="server" id="imgLogo" alt="" data-pin-nopin="true" class="img-responsive" />
                        </div>

                        <div class="col-md-6 form-fields">

                            <div class="form-group">
                                <h2 class="form-title">Reset Password</h2>
                            </div>

                            <div class="alert-msgs clearfix">
                                <div class="alert alert-danger" id="divInvalid" runat="server" visible="false">
                                    Invalid Request! Link has expired.
                                </div>

                                <div class="alert alert-danger" id="divError" runat="server" visible="false">
                                    Error! Please contact administrator.
                                </div>

                                <div id="divSuccess" runat="server" class="alert alert-success" visible="false">
                                    Password has been updated successfully.
                                </div>
                            </div>

                            <div class="form-group">
                                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control" placeholder="New Password" required></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password" CssClass="form-control" placeholder="Confirm Password" required></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <asp:Button ID="btnResetPassword" OnClientClick="return validate()" OnClick="btnResetPassword_Click" CssClass="primary-btn" Text="Reset Password" runat="server"></asp:Button>
                            </div>

                        </div>
                    </div>
                    <!-- .row -->

                </div>
                <!-- .form-cell -->
            </div>
            <!-- .form-table -->
        </div>
        <!-- .loginColumns -->
    </form>

    <!-- .form-footer Section -->
    <div class="form-footer">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 col-sm-6">
                    <a href="#">Privacy Policy</a>
                    <a href="#">Terms & Conditions</a>
                </div>
                <div class="col-md-6  col-sm-6 text-right" title="13.june.2019-17:48" data-toggle="tooltip">
                    Copyright FirstFreight CRM &copy <%= DateTime.Now.ToString("yyyy") %>
                </div>
            </div>
        </div>
    </div>

    <!-- js libraries -->
    <script src="/_content/_js/bundle/jquery-3.4.1.min.js"></script>
    <script src="/_content/js/bundle/bootstrap.min.js"></script>

    <!-- Language Translation -->
    <script src="/_content/_js/bundle/languagetranslation-28-mar-2020.js"></script>

    <script>
        function validate() {
            $("#txtPassword").removeClass("error");
            $("#txtConfirmPassword").removeClass("error");

            if ($("#txtPassword").val() === '') {
                $("#txtPassword").addClass("error");
                return false;
            }
            if ($("#txtConfirmPassword").val() === '') {
                $("#txtConfirmPassword").addClass("error");
                return false;
            }
            if ($("#txtConfirmPassword").val() !== $("#txtPassword").val()) {

                $("#txtConfirmPassword").addClass("error");
                $("#txtPassword").addClass("error");
                return false;
            }
        }
    </script>
</body>
</html>
