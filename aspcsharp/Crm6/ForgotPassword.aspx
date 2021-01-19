<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="Crm6.ForgotPassword" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Forgot Password</title>

    <!-- css libraries -->
    <link href="/_content/_css/bundle/bootstrap.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/animate.min.css" rel="stylesheet" />
    <link href="_content/_css/bundle/style-07-apr-2020.css " rel="stylesheet" />

     <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
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
                                <h2 class="form-title">Forgot password</h2>
                                <div class="form-descp">
                                    Enter your email address and we will send you a link to reset the password.
                                </div>
                            </div>

                            <div class="alert-msgs clearfix">
                                <div class="alert alert-danger" id="divError" runat="server" visible="false">
                                    Error - Please contact administrator.
                                </div>

                                <div id="divSuccess" class="alert alert-success" runat="server" visible="false">
                                    If an account matched the email address, you should receive an email with instructions how to reset your password shortly.
                                </div>
                            </div>

                            <div class="form-group">
                                <asp:TextBox runat="server" TextMode="Email" ID="txtEmailAddress" CssClass="form-control" placeholder="Email Address"></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <asp:Button ID="btnSendResetLink" OnClientClick="return validate()" OnClick="btnSendResetLink_Click" CssClass="primary-btn" Text="Send Reset Link" runat="server"></asp:Button>
                            </div>

                            <div class="form-group form-msg">
                                <a href="Login.aspx">Back To Login</a>
                            </div>

                        </div>
                    </div>
                    <!-- .row -->

                    <div class="row login-footer">
                        <div class="col-md-6 col-sm-6 text-left">
                            <a href="Privacy.aspx">Privacy Policy</a>
                            <a href="Terms.aspx">Terms & Conditions</a>
                        </div>
                        <div class="col-md-6 col-sm-6 text-right" title="11.nov.2019-22:36" data-toggle="tooltip">
                            Copyright First Freight CRM &copy <%= DateTime.Now.ToString("yyyy") %>
                        </div>
                    </div>

                </div>
                <!-- .form-cell -->
            </div>
            <!-- .form-table -->
        </div>
        <!-- .loginColumns -->
    </form>


    <!-- js libraries -->
    <script src="/_content/_js/bundle/jquery-3.4.1.min.js"></script>
    <script src="/_content/_js/bundle/bootstrap.min.js"></script>

    <!-- Language Translation -->
    <script src="/_content/_js/bundle/languagetranslation-28-mar-2020.js"></script>

    <script>
        function validate() {
            $("#txtEmailAddress").removeClass("error");
            if ($("#txtEmailAddress").val() === '' || !validateEmail($("#txtEmailAddress").val())) {
                $("#txtEmailAddress").addClass("error");
                return false;
            }
            return true;
        }

    </script>
</body>
</html>
