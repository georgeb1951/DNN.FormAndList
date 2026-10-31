<%@ Control Language="C#"  Inherits="BCC.Modules.BCCForms.EditForm" CodeBehind="Form.ascx.cs" AutoEventWireup="false" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div runat="server" id="divForm"  class="dnnForm dnnClear bccForms bccf-form">

    <div runat="server" ID="EditFormPlaceholder"  />
    <%-- spam protection: provider widget (reCAPTCHA / Turnstile), its token, and the honeypot --%>
    <asp:Panel ID="gRecaptcha" CssClass="recaptcha-container bccf-captcha" runat="server" Visible="False" />
    <asp:HiddenField ID="hfCaptchaToken" runat="server" />
    <asp:PlaceHolder ID="phHoneypot" runat="server" />
    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" Text="Update" runat="server" resourcekey="cmdUpdate" cssclass="dnnPrimaryAction reCaptchaSubmit" />
        </li>
        <li> 
            <asp:LinkButton ID="cmdCancel" Text="Cancel" CausesValidation="False" resourcekey="cmdCancel" runat="server" cssclass="dnnSecondaryAction" />
        </li>
        <li>
            <asp:LinkButton ID="cmdDelete" Text="Delete" CausesValidation="False" resourcekey="cmdDelete" runat="server" CssClass="dnnSecondaryAction" />
        </li>
         <li>
                <asp:HyperLink runat="server" CssClass="dnnSecondaryAction" ID="cmdShowRecords" Visible="False"></asp:HyperLink>
          </li>
    </ul>
</div>
<%-- Kept outside divForm: after a successful submit the form is hidden and only this message shows. --%>
<div class="bccForms bccf-form-message">
    <div runat="server" ID="MessagePlaceholder" />
</div>
<script type="text/javascript">
/* Wrap your code in a function to avoid global scope and pass in any global objects */
/* globals jQuery, window, Sys */
(function($, Sys) {

    /* wire up any plugins or other logic here */

    function setUpMyModule() {
        if ($.fn.dnnPanels) { $('#<%=EditFormPlaceholder.ClientID%>').dnnPanels(); }
        if (!$.fn.datepicker) { return; }
        $('.fnl-datepicker').datepicker({
            monthNames:  [<%=LocalizeText("MonthNames")%>],
            dayNames:    [<%=LocalizeText("DayNames")%>],
            dayNamesMin: [<%=LocalizeText("DayNamesMin")%>],
            firstDay:    <%:LocalizeText("FirstDay")%>,
            dateFormat:  '<%:JsUiDatePattern %>'
        });
    }

    /* wire up the call to your function on document ready */
    $(document).ready(function() {

        setUpMyModule();

        /* Re-run after an UpdatePanel (partial rendering) request, when MS AJAX is on the page */
        if (Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
                setUpMyModule();
            });
        }
    });

}(jQuery, window.Sys))
</script>