<%@ Control Language="C#"  Inherits="BCC.Modules.BCCForms.Settings" AutoEventWireup="True" CodeBehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm bccForms bccf-settings">
    <div class="dnnFormItem">
        <asp:CheckBox ID="chkEditOwnData" runat="server" resourcekey="EditOwnData"
            Text="User is only allowed to manipulate his own items." />
    </div>
    <div class="dnnFormItem">
        <asp:CheckBox ID="chkInputFiltering" runat="server" resourcekey="InputFiltering"
            Text="Filter input for markup code or script input. Note: Filtering is always enabled for Anonymous." />
    </div>
    <div class="dnnFormItem">
        <asp:CheckBox ID="chkDisplayColumns" runat="server" resourcekey="DisplayColumns"
            Text="Negate permission/feature DisplayColumns for Administators." />
    </div>
    <div class="dnnFormItem">
        <asp:CheckBox ID="chkHideSystemColumns" runat="server" resourcekey="DisplaySystemColumns" />
    </div>
    <div class="dnnFormItem">
        <asp:CheckBox ID="chkPrivateColumns" runat="server" resourcekey="PrivateColumns"
            Text="Negate permission/feature PrivateColumns for Administators." />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblUserRecordQuota" runat="server" ResourceKey="plUserRecordQuota" Suffix=":" ControlName="txtUserRecordQuota" />
        <asp:TextBox ID="txtUserRecordQuota" runat="server" MaxLength="4" CssClass="bccf-num" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblForceCaptcha" runat="server" Suffix=":" ControlName="ddlCaptcha" />
        <asp:DropDownList ID="ddlCaptcha" runat="server" />
    </div>
    <%-- shown by the script below for the provider modes; inline display:none so they stay
         hidden even if the host page doesn't load module.css --%>
    <div class="dnnFormItem siteKeyRow" style="display:none;">
        <dnn:Label ID="lblSiteKey" runat="server" Suffix=":" ControlName="txtSiteKey" />
        <asp:TextBox ID="txtSiteKey" runat="server" />
    </div>
    <div class="dnnFormItem secretKeyRow" style="display:none;">
        <dnn:Label ID="lblSecretKey" runat="server" Suffix=":" ControlName="txtSecretKey" />
        <asp:TextBox ID="txtSecretKey" runat="server" />
    </div>
    <div class="dnnFormItem minScoreRow" style="display:none;">
        <dnn:Label ID="lblMinScore" runat="server" Suffix=":" ControlName="txtMinScore" />
        <asp:TextBox ID="txtMinScore" runat="server" MaxLength="4" CssClass="bccf-num" />
        <asp:RegularExpressionValidator ID="valMinScore" runat="server" ControlToValidate="txtMinScore"
            ValidationExpression="^(0(\.\d+)?|1(\.0+)?)$" Display="Dynamic"
            CssClass="dnnFormMessage dnnFormError" resourcekey="valMinScore" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblHoneypot" runat="server" Suffix=":" ControlName="chkHoneypot" />
        <asp:CheckBox ID="chkHoneypot" runat="server" />
    </div>
</div>

<script type="text/javascript">
    $(document).ready(function () {
        var $mode = $('#<%=ddlCaptcha.ClientID %>');
        function update(speed) {
            var mode = $mode.val();
            var usesKeys = mode === 'ReCaptchaV2' || mode === 'ReCaptchaV3' || mode === 'Turnstile';
            $('.siteKeyRow, .secretKeyRow')[usesKeys ? 'show' : 'hide'](speed);
            $('.minScoreRow')[mode === 'ReCaptchaV3' ? 'show' : 'hide'](speed);
        }
        update(0);
        $mode.change(function () { update('slow'); });
    });
</script>
