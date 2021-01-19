<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="footer.ascx.cs" Inherits="Crm6._usercontrols.footer" %>
 
<div class="footer">
    <%--<div id="go-to-top">
        <a class="go-to-top" href="#top">
            <i class="fa fa-arrow-up"></i>
        </a>
    </div>--%>
    <small>
        Copyright <strong>First Freight</strong> &copy; <%= DateTime.Now.ToString("yyyy")%>
    </small> 
</div>

<style>

     #go-to-top {
         position: fixed;
         bottom: 20px;
         right: 20px;
         z-index: 100;
         color: white;
     }

    .go-to-top {
        height: 38px;
        width: 38px;
        display: block;
        background: #1ab394;
        padding: 9px 8px;
        text-align: center;
        color: #fff;
        border-radius: 50%;
    }

    .go-to-top:hover {
        color: white;
        background: #1ab394;
    }

    .go-to-top:active,  .go-to-top:visited  {
        color: white;
        background: #1ab394;
    }

</style>
