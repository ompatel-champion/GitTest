<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Crm6.Login" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="robots" content="noindex">
    <title>CRM</title>
    <!--css libraries-->
    <link href="/_content/_css/bundle/bootstrap.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/animate.min.css" rel="stylesheet" />
    <link href="/_content/_css/bundle/style-07-apr-2020.css " rel="stylesheet" />
    <!--manifest-->
    <link rel="manifest" href="/manifest.json">
    <!--favicon-->
    <link href="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAcCAYAAABh2p9gAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAZiS0dEAP8A/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAuZJREFUSMetlc1qG1cUx3/n3lEiarcdQwuGQpk3sNQXqGTavWsotN5YcuJCV5b6ApKfQIEuAo0TjTZxV/E+UEZ9AtlP4CF0FQrSIqYGae7pYmbk8Ucs9ePs7sy5v/s/53/uDCwRq4+f1T5sHG0tk2uWSWIGauiXG0+D/wWoQgPwPSm1/jOw3HgaiMhuttwAWO+NgvXeKPhXQM+UejefWfAfWBv5vZH/j4Cre887wNa8dAgBjLUVIPjImNbSwJVHRweg3TlMiS/6jwcAAh0ArlpxP3Dl0dGBKE/mD5RJotM6wOc/nx0AQXbKBGDn9dvKt6/fVu4Eru4979yEqbr6ZfhjvN4bBajMVSNyCuBE/JK1J1vR2Pdu9+x6mYlO57AH1kbA3AiTJIcpVxpA8MFs1jLvh+mgpLZ6AxZcKdfDuF2Nv4vGgYh8mSmteACrjWe1Igz08KK/34V05u6CvTnY6AJIknQRCTKzPjYAaqRfVPbuRQr7rDeq3AfbicYdKTptbeit7P3SkGyDKnFJvVauzFp7UnTUOffNH+3qEOD7aLyLc1ctgvi4vjbwRM0ukpmm2pyEzcmtMlVj41z9Tbsa5zBxLiz4OVFj6gAeQi07Yvgu3B+u90aUjOkWYOfGuc24XY13ojFAC+eK13GMMZu/1tfia3OYXyug+DHAOLddgHWLMIVzjNl8WV87neejDNO37gzI1RVH47QA6xTMi/4y5osiDMBTdQMRU0tIJgBizAaqed9y1dUbsPD4q0+a3BHmIvwhVNWwoMpPHZJh3K7GO7/9Cc69Kuw5eR9s3sOL/n6zTHmSjw6AS2SQ5TRygxTOnTE/cU/MTZmEzbRk9PfU/ll6gJirwTXmMHdzIbDg6hNUR/laVGtZ34bH9bUBC+IWMG5XJ8a57Rmef+2FteEiWFrZHRGnNyIrTWLQYDadni0DXPwbdckAYFoqxQtzlwG+/PrTropE5ctLfwnecj/6hyLbibVLAf8Gfl1insx7W38AAAASdEVYdEVYSUY6T3JpZW50YXRpb24AMYRY7O8AAAAASUVORK5CYII=" rel="icon" type="image/x-icon" />
    <!--iPhone iPad Safari icon-->
    <link rel="apple-touch-icon" href="_content/_img/apple-touch-icon.png">
</head>

<body class="act-formpg login-fpage">

    <form runat="server">
        <div class="loginColumns container">
            <div class="form-table">
                <div class="form-cell">
                    <asp:TextBox runat="server" ID="txtScreenResolution" CssClass="hide"></asp:TextBox>
                    <div class="row">
                        <div class="col-md-6 logo-holder">
                            <img src="_content/_img/logo.svg" runat="server" id="imgLogo" alt="" data-pin-nopin="true" class="img-responsive" />
                        </div>

                        <div class="col-md-6 form-fields">

                            <div class="form-group">
                                <h2 class="form-title">Log In</h2>
                            </div>

                            <div class="alert-msgs clearfix">
                                <div id="divError" runat="server" class="alert alert-danger" visible="false">Email address or Password is invalid.</div>
                            </div>

                            <div class="form-group LH20">
                                <asp:TextBox ID="txtUsername" TextMode="Email" runat="server" CssClass="form-control" placeholder="Email" required></asp:TextBox>
                            </div>
                            
                            <div class="form-group LH20">
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password" required></asp:TextBox>
                                <a class="forgot-link" href="ForgotPassword.aspx">Forgot?</a>
                            </div>

                            <div class="form-group">
                                <asp:Button ID="btnLogin" runat="server" CssClass="primary-btn" Text="Login" OnClick="btnLogin_Click" />
                            </div>

                            <%--<div class="form-group form-msg">
                                Not a member? <a href="#">Sign Up</a>
                            </div>--%>

                        </div>
                    </div>

                    <div class="row login-footer">
                        <div class="col-md-6 col-sm-6 text-left">
                            <a href="Privacy.aspx">Privacy Policy</a>
                            <a href="Terms.aspx">Terms & Conditions</a> 
                        </div>       
                        <div class="col-md-6 col-sm-6 text-right" title="09.apr.2020-17:39" data-toggle="tooltip">     
                            Copyright First Freight CRM &copy <%= DateTime.Now.ToString("yyyy") %>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form> 
    <!-- js libraries -->
    <script src="/_content/_js/bundle/jquery-3.4.1.min.js"></script>
    <script src="/_content/_js/bundle/bootstrap.min.js"></script>

    <script>
        $(function () {
            // set screen resoltion
            $("#txtScreenResolution").val(screen.width + "x" + screen.height);
            // clear language local storage
            localStorage.setItem('language_data', '');
            localStorage.setItem('language_code', '');
        });
    </script>

    <script>
        if ('serviceWorker' in navigator) {
            console.log("Register service worker...");
            navigator.serviceWorker.register('service-worker.js')
                .then(function (reg) {
                    console.log("SUCCESS: Service worker registered.");
                }).catch(function (err) {
                    console.log("ERROR: Service worker did not register: ", err)
                });
        }
    </script>

    <script src='https://cdn.logrocket.io/LogRocket.min.js' crossorigin='anonymous'></script>
    <script>window.LogRocket && window.LogRocket.init('nnz2fs/crm6');</script>

</body>
</html>
