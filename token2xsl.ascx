<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Language="C#" Inherits="BCC.Modules.BCCForms.Token2Xsl"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Codebehind="Token2Xsl.ascx.cs" AutoEventWireup="false" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- Custom JavaScript Registration --%>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/lib/codemirror.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/lib/codemirror.js" Priority="101" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/mode/xml/xml.js" Priority="102" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/mode/javascript/javascript.js" Priority="102" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/mode/css/css.js" Priority="102" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/mode/htmlmixed/htmlmixed.js" Priority="103" />


<script type="text/javascript">
    /* Opens the "hidden columns" help in a popup window. */
    function OpenHelpWindow(url) {
        window.open(url, '', 'width=800,height=800,location=no,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no');
        return false;
    }

    /* Inserts the column token chosen in <source> at the cursor in <target>; wraps it in
       <td></td> when tags is "True". Clears the generated XSL (<clear>), which is now stale. */
    function AddCurrentItemIntoTextBox(e, target, source, clear, tags) {
        if (e && e.preventDefault) { e.preventDefault(); }
        var ctrlTarget = document.getElementById(target);
        var ctrlSource = document.getElementById(source);
        if (!ctrlTarget || !ctrlSource) { return false; }
        var text = tags === "True" ? "<td>" + ctrlSource.value + "</td>" : ctrlSource.value;
        if (typeof ctrlTarget.selectionStart === "number") {
            var start = ctrlTarget.selectionStart, end = ctrlTarget.selectionEnd;
            ctrlTarget.value = ctrlTarget.value.substring(0, start) + text + ctrlTarget.value.substring(end);
            ctrlTarget.selectionStart = ctrlTarget.selectionEnd = start + text.length;
        } else {
            ctrlTarget.value += text;
        }
        ctrlTarget.focus();
        var ctrlToClear = document.getElementById(clear);
        if (ctrlToClear) { ctrlToClear.value = ""; }
        return false;
    }

    jQuery(function ($) {
        var setupModule = function () {
            var area = $("textarea[id$='<%= txtXslScript.ClientID %>']")[0];
            if (!area || !window.CodeMirror || $(area).next('.CodeMirror').length) { return; }
            CodeMirror.fromTextArea(area, {
                lineNumbers: true,
                matchBrackets: true,
                lineWrapping: true,
                mode: "text/xml"
            });
        };

        setupModule();

        /* re-run after an UpdatePanel request, when MS AJAX is on the page */
        if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(setupModule);
        }
    });
</script>

<div class="dnnForm bccForms bccf-xslgen">
    <div class="bccf-align-end">
        <asp:HyperLink ID="hlpColumns" runat="server" resourcekey="cmdColumnsHelp" NavigateUrl="#"
            CssClass="bccf-command" />
    </div>

    <dnn:SectionHead ID="dshHtml" IsExpanded="true" Section="tblHtml" IncludeRule="True"
        ResourceKey="dshHtml" runat="server" CssClass="dnnFormSectionHead" />
    <div id="tblHtml" runat="server">

        <%-- List view --%>
        <dnn:SectionHead ID="dshListView" IsExpanded="true" Section="dvListview" IncludeRule="false"
            ResourceKey="dshListView" runat="server" CssClass="bccf-label" />
        <div id="dvListview" runat="server" class="bccf-section">
            <div class="dnnFormItem">
                <dnn:Label ID="plListType" runat="server" Text="List type" ControlName="ddlListTemplateType" Suffix=":" />
                <div class="bccf-field-group">
                    <asp:DropDownList ID="ddlListTemplateType" runat="server" AutoPostBack="True">
                        <asp:ListItem Value="table" Selected="true" resourcekey="table" />
                        <asp:ListItem Value="div" resourcekey="div" />
                        <asp:ListItem Value="p" resourcekey="p" />
                        <asp:ListItem Value="ol" resourcekey="ol" />
                        <asp:ListItem Value="ul" resourcekey="ul" />
                        <asp:ListItem Value="nothing" resourcekey="nothing" />
                    </asp:DropDownList>
                    <asp:LinkButton ID="cmdRebuildContent" resourcekey="cmdRebuildContent" runat="server"
                        CssClass="dnnTertiaryAction bccf-inline-action" />
                </div>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plDelimiter" runat="server" Text="Delimiter symbol" ControlName="txtListTemplateDelimiter" Suffix=":" />
                <asp:TextBox ID="txtListTemplateDelimiter" runat="server" Columns="10" CssClass="bccf-num">;</asp:TextBox>
            </div>
            <asp:Label ID="lblTemplateError" runat="server" CssClass="dnnFormMessage dnnFormError" Visible="False" />

            <div class="dnnFormItem">
                <dnn:Label ID="plHeaderList" runat="server" ResourceKey="plHeaderList" ControlName="ddlHeaders" Suffix=":" />
                <div class="bccf-field-group">
                    <asp:DropDownList ID="ddlHeaders" runat="server" />
                    <asp:HyperLink ID="addHeader" resourcekey="cmdInsert" runat="server"
                        CssClass="dnnTertiaryAction bccf-inline-action" NavigateUrl="#" />
                </div>
            </div>
            <div class="bccf-code">
                <asp:Label ID="lblListTemplateHead" runat="server" CssClass="bccf-code-wrap" />
                <asp:TextBox ID="txtHeaderTemplate" runat="server" Rows="3" TextMode="MultiLine" CssClass="bccf-mono" />
            </div>

            <div class="dnnFormItem">
                <dnn:Label ID="plColumns" runat="server" Text="Available Columns" ControlName="ddlColumnsForListView" Suffix=":" />
                <div class="bccf-field-group">
                    <asp:DropDownList ID="ddlColumnsForListView" runat="server" />
                    <asp:HyperLink ID="addColumnToListTemplate" resourcekey="cmdInsert" runat="server"
                        CssClass="dnnTertiaryAction bccf-inline-action" NavigateUrl="#" />
                    <asp:HyperLink ID="addColumnWithTagsToListTemplate" resourcekey="cmdAddColumnWithTags" runat="server"
                        CssClass="dnnTertiaryAction bccf-inline-action" NavigateUrl="#" />
                </div>
            </div>
            <div class="bccf-code">
                <asp:TextBox ID="txtListTemplate" runat="server" Rows="15" TextMode="MultiLine" CssClass="bccf-mono" />
                <asp:Label ID="lblListTemplateFooter" runat="server" CssClass="bccf-code-wrap" />
            </div>
        </div>

        <%-- Detail view --%>
        <dnn:SectionHead ID="dshDetailView" IsExpanded="false" Section="dvDetailView" IncludeRule="false"
            ResourceKey="dshDetailView" runat="server" CssClass="bccf-label" />
        <div id="dvDetailView" runat="server" class="bccf-section">
            <div class="dnnFormItem">
                <dnn:Label ID="plColumns2" runat="server" ResourceKey="plColumns" ControlName="ddlColumnsForDetailView" Suffix=":" />
                <div class="bccf-field-group">
                    <asp:DropDownList ID="ddlColumnsForDetailView" runat="server" />
                    <asp:HyperLink ID="addColumnToDetailTemplate" resourcekey="cmdInsert" runat="server"
                        CssClass="dnnTertiaryAction bccf-inline-action" NavigateUrl="#" />
                    <asp:LinkButton ID="cmdRebuildDetail" resourcekey="cmdRebuildContent" runat="server"
                        CssClass="dnnTertiaryAction bccf-inline-action" />
                </div>
            </div>
            <div class="bccf-code">
                <asp:TextBox ID="txtDetailTemplate" runat="server" Rows="15" TextMode="MultiLine" CssClass="bccf-mono" />
            </div>
        </div>

        <%-- Notification e-mail (only when editing the e-mail template) --%>
        <dnn:SectionHead ID="dshTrackingEmail" IsExpanded="true" Section="dvTrackingEmail"
            IncludeRule="false" ResourceKey="dshTrackingEmail" runat="server" CssClass="bccf-label" Visible="false" />
        <div id="dvTrackingEmail" runat="server" visible="false" class="bccf-section">
            <div class="dnnFormItem">
                <dnn:Label ID="plColumns3" runat="server" ResourceKey="plColumns" ControlName="ddlColumnsForTrackingEmail" Suffix=":" />
                <div class="bccf-field-group">
                    <asp:DropDownList ID="ddlColumnsForTrackingEmail" runat="server" />
                    <asp:HyperLink ID="addColumnToTrackingEmail" resourcekey="cmdInsert" runat="server"
                        CssClass="dnnTertiaryAction bccf-inline-action" NavigateUrl="#" />
                    <asp:LinkButton ID="cmdRebuildTrackingEmail" resourcekey="cmdRebuildContent" runat="server"
                        CssClass="dnnTertiaryAction bccf-inline-action" />
                </div>
            </div>
            <div class="bccf-code">
                <asp:TextBox ID="txtTrackingEmailTemplate" runat="server" Rows="15" TextMode="MultiLine" CssClass="bccf-mono" />
            </div>
        </div>

        <%-- Options --%>
        <dnn:SectionHead ID="dshOptions" IsExpanded="false" Section="dvOptions" IncludeRule="false"
            ResourceKey="dshOptions" runat="server" CssClass="bccf-label" />
        <div id="dvOptions" runat="server" class="bccf-section">
            <div class="dnnFormItem">
                <dnn:Label ID="lblEnableSorting" runat="server" ControlName="chkEnableSorting" ResourceKey="lblEnableSorting" Suffix=":" />
                <asp:CheckBox ID="chkEnableSorting" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblEnablePaging" runat="server" ControlName="chkEnablePaging" ResourceKey="lblEnablePaging" Suffix=":" />
                <asp:CheckBox ID="chkEnablePaging" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblEnableSearch" runat="server" ControlName="chkEnableSearch" ResourceKey="lblEnableSearch" Suffix=":" />
                <asp:CheckBox ID="chkEnableSearch" runat="server" />
                <asp:Label runat="server" ID="lblSearchIsObsolete" resourcekey="lblSearchIsObsolete" Text="(Obsolete)" CssClass="bccf-value-muted" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblShowDetailView" runat="server" ControlName="chkShowDetailView" ResourceKey="lblShowDetailView" Suffix=":" />
                <asp:CheckBox ID="chkShowDetailView" runat="server" AutoPostBack="True" />
            </div>
        </div>

        <asp:Label ID="lblXslScriptError" runat="server" CssClass="dnnFormMessage dnnFormError" Visible="False" />
        <ul class="dnnActions dnnClear">
            <li><asp:Button ID="cmdGenerateXslt" resourcekey="btnGenerateXslt" runat="server" CssClass="dnnPrimaryAction" /></li>
        </ul>
    </div>

    <%-- Generated XSL --%>
    <dnn:SectionHead ID="dshXslt" IsExpanded="false" Section="tblXslt" IncludeRule="True"
        ResourceKey="dshXslt" runat="server" CssClass="dnnFormSectionHead" />
    <div id="tblXslt" runat="server" class="bccf-section bccf-code">
        <asp:TextBox ID="txtXslScript" runat="server" Rows="30" TextMode="MultiLine" CssClass="bccf-mono" />
    </div>

    <%-- Save --%>
    <dnn:SectionHead ID="dshSave" IsExpanded="false" Section="tblSave" IncludeRule="True"
        ResourceKey="dshSave" runat="server" CssClass="dnnFormSectionHead" />
    <div id="tblSave" runat="server" class="bccf-section">
        <asp:Label ID="lblSaveXslError" runat="server" CssClass="dnnFormMessage dnnFormError" Visible="False" />
        <div class="dnnFormItem">
            <dnn:Label ID="plFolderName" runat="server" ControlName="txtFolderName" Suffix=":" />
            <asp:TextBox ID="txtFolderName" runat="server" Enabled="False" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plFileName" runat="server" ControlName="txtFileName" Suffix=":" />
            <div class="bccf-field-group">
                <asp:TextBox ID="txtFileName" runat="server" Enabled="False" />
                <asp:LinkButton ID="cmdSaveFile" resourcekey="cmdSaveFile" runat="server"
                    CssClass="dnnPrimaryAction bccf-inline-action" Enabled="false" />
            </div>
        </div>
        <asp:Panel ID="panConfirm" runat="server" Visible="False" CssClass="dnnFormMessage dnnFormWarning">
            <asp:Label ID="lblConfirm" resourcekey="lblConfirm" runat="server" />
            <asp:LinkButton ID="cmdConfirmOverwriteFile" CssClass="dnnPrimaryAction bccf-inline-action" resourcekey="Yes" runat="server" />
            <asp:LinkButton ID="cmdDenyOverwriteFile" CssClass="dnnSecondaryAction bccf-inline-action" resourcekey="No" runat="server" />
        </asp:Panel>
    </div>

    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton ID="cmdBack" resourcekey="cmdBack" runat="server" CssClass="dnnSecondaryAction"
            Text="Back" CausesValidation="False" /></li>
    </ul>
</div>
