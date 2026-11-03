<%@ Control Language="C#" Explicit="True" Inherits="BCC.Modules.BCCForms.Configuration"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" CodeBehind="Configuration.ascx.cs" AutoEventWireup="false" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register Src="Controls/Fields.ascx" TagName="Fields" TagPrefix="fnl" %>

<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- Custom JavaScript Registration --%>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/lib/codemirror.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/lib/codemirror.js" Priority="101" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/mode/xml/xml.js" Priority="102" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/mode/javascript/javascript.js" Priority="102" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/mode/css/css.js" Priority="102" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/mode/htmlmixed/htmlmixed.js" Priority="103" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/components/CodeEditor/addon/display/autorefresh.js" Priority="103" />

<div class="dnnForm bccf-config" id="bccfConfig">
    <ul class="dnnAdminTabNav dnnClear">
        <li><a href="#ssSchemaSettings">
            <asp:Label resourcekey="SchemaTab" runat="server" /></a></li>
        <li><a href="#ssPageSettings">
            <asp:Label resourcekey="BehaviorSettings" runat="server" /></a></li>
    </ul>
    <div id="ssSchemaSettings">
        <h2 class="dnnFormSectionHead"><a href="">
            <asp:Label runat="server" resourcekey="ModuleSettingsHeading" /></a></h2>
        <fieldset>
            <div runat="server" id="udtSection" class="bccf-section">
                <div>
                    <asp:Label runat="server" resourcekey="ModuleSettingsTeaser" />
                </div>

                <fnl:Fields ID="Fields" runat="server" />
                <div class="dnnFormItem">
                    <dnn:Label ID="plPrivacy" ControlName="chkExcludeFromSearch" runat="server" />
                    <asp:CheckBox ID="chkExcludeFromSearch" runat="server" />
                </div>
            </div>
        </fieldset>

    </div>
    <div id="ssPageSettings" class="ssPageSettings">
        <h2 class="dnnFormSectionHead"><a href="">
            <asp:Label runat="server" resourcekey="DisplaySettings" /></a></h2>
        <fieldset>
            <div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblList" runat="server" Suffix="" resourcekey="plUsage" ControlName="rblList" />

                    <asp:RadioButtonList ID="rblUsageListForm" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons">
                        <asp:ListItem Value="List" resourcekey="ListMode"></asp:ListItem>
                        <asp:ListItem Value="Form" resourcekey="FormMode"></asp:ListItem>
                        <asp:ListItem Value="FormAndList" resourcekey="FormAndListMode"></asp:ListItem>
                        <asp:ListItem Value="ListAndForm" resourcekey="ListAndFormMode"></asp:ListItem>
                    </asp:RadioButtonList>

                    <div class="dnnFormItem bccf-settings-warning dnnClear" runat="server" id="rememberSettings">
                        <div class="dnnFormMessage dnnFormWarning">
                            <asp:Label resourcekey="RememberSettings" runat="server" />
                        </div>
                    </div>
                </div>

                <div class="dnnFormItem">
                    <dnn:Label ID="plUseButtons" runat="server" ControlName="chkUseButtons" Suffix="?" />
                    <asp:CheckBox ID="chkUseButtons" runat="server" />
                </div>

            </div>
        </fieldset>

        <h2 class="dnnFormSectionHead"><a href="" onclick="setTimeout(function() { editor.refresh();},100);">
            <asp:Label runat="server" resourcekey="FormSettings" /></a></h2>
        <fieldset>
            <div id="plainFormSettingSet" runat="server">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblOnSubmission" runat="server" Suffix="" resourcekey="rblOnSubmission" ControlName="rblOnSubmission" />
                    <asp:RadioButtonList ID="rblOnSubmission" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Vertical" AutoPostBack="true">
                        <asp:ListItem Value="Form" resourcekey="OnSubmissionForm" />
                        <asp:ListItem Value="Text" resourcekey="OnSubmissionText" />
                        <asp:ListItem Value="Redirect" resourcekey="OnSubmissionRedirect" />
                    </asp:RadioButtonList>
                </div>
                <div class="dnnFormItem" id="rowOnSubmissionRedirect" runat="server">
                    <dnn:Label ID="lblOnSubmissionRedirect" runat="server" Suffix="" resourcekey="lblOnSubmissionRedirect" ControlName="urlOnSubmissionRedirect" />
                    <div class="bccf-indent">
                        <Portal:URL ID="urlOnSubmissionRedirect" runat="server" ShowTrack="False" ShowNewWindow="False" ShowLog="False" ShowFiles="false" ShowUrls="true" ShowTabs="true" Width="200" />
                    </div>
                </div>
                <div class="dnnFormItem" id="rowSubmissionText" runat="server">
                    <dnn:Label ID="lblSub" runat="server" Suffix="" resourcekey="plSubmissionText" ControlName="teSubmissionSuccess" />
                    <div class="dnnLeft">
                        <dnn:TextEditor ID="teSubmissionSuccess" runat="server" Height="250" Width="400" />
                    </div>
                </div>
            </div>
            <div class="dnnFormItem">
                <dnn:Label runat="server" ID="lblEnableFormTemplate" ControlName="chkEnableFormTemplate" resourcekey="EnableFormTemplate" />
                <asp:CheckBox ID="chkEnableFormTemplate" runat="server" AutoPostBack="True" />
            </div>
            <div class="dnnFormItem" runat="server" id="divFormTemplate">
                <dnn:Label runat="server" ID="lblFormTemplate" ControlName="txtFormTemplate" resourcekey="FormTemplate" />
                <asp:TextBox runat="server" ID="txtFormTemplate" TextMode="MultiLine" Rows="15" Columns="90"  />
   
                <span class="bccf-commands">
                    <asp:LinkButton ID="cmdGenerateFormTemplate" runat="server" CssClass="dnnSecondaryAction"
                        resourcekey="GenerateFormTemplate" />
                </span>
            </div>
        </fieldset>


        <h2 class="dnnFormSectionHead"><a href="">
            <asp:Label runat="server" resourcekey="ListSettings" /></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:Label ID="plRenderMethod" runat="server" Suffix=":" ControlName="renderMethodRadioButtonList" />
                <asp:RadioButtonList ID="renderMethodRadioButtonList" CssClass="dnnFormRadioButtons" runat="server"
                    AutoPostBack="True">
                    <asp:ListItem resourcekey="DataGrid" Value="DataGrid" Selected="True" />
                    <asp:ListItem resourcekey="XslTrans" Value="XslUserDefinedStyleSheet" />
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem" id="rowUserDefined" runat="server">
                <dnn:Label ID="plUserDefinedStyleSheet" runat="server" Suffix=":" ControlName="XslUserDefinedUrlControl" />
                <div class="bccf-indent">
                    <Portal:URL ID="XslUserDefinedUrlControl" runat="server" FileFilter="xsl,xslt" Required="True" ShowTrack="False"
                        ShowNewWindow="False" ShowLog="False" ShowFiles="True" ShowUrls="False" ShowTabs="False" Width="200" />
                    <div class="BelowUrlControl bccf-commands">
                        <asp:LinkButton ID="cmdEditXSL" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdEditXSL" />
                        <asp:LinkButton CssClass="dnnPrimaryAction" resourcekey="cmdShowXSLGenerator" ID="cmdGenerateXSL" runat="server" />
                    </div>
                </div>
            </div>
            <div class="dnnFormItem">
                <asp:Label ID="lblSorting" resourcekey="Sorting" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plSort" runat="server" Suffix=":" ControlName="cboSortField" />
                <asp:DropDownList ID="cboSortField" runat="server" DataValueField="UserDefinedFieldId" DataTextField="FieldTitle" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plOrder" runat="server" Suffix=":" ControlName="cboSortOrder" />
                <asp:DropDownList ID="cboSortOrder" runat="server">
                    <asp:ListItem resourcekey="Ascending" Value="ASC" />
                    <asp:ListItem resourcekey="Descending" Value="DESC" />
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plPaging" runat="server" Suffix=":" ControlName="cboPaging" />
                <asp:DropDownList ID="cboPaging" runat="server">
                    <asp:ListItem resourcekey="NoPaging" Value="" />
                    <asp:ListItem Value="1">1</asp:ListItem>
                    <asp:ListItem Value="5">5</asp:ListItem>
                    <asp:ListItem Value="10">10</asp:ListItem>
                    <asp:ListItem Value="15">15</asp:ListItem>
                    <asp:ListItem Value="20">20</asp:ListItem>
                    <asp:ListItem Value="25">25</asp:ListItem>
                    <asp:ListItem Value="30">30</asp:ListItem>
                    <asp:ListItem Value="40">40</asp:ListItem>
                    <asp:ListItem Value="50">50</asp:ListItem>
                </asp:DropDownList>
            </div>

            <asp:Label ID="lblSearchAndFilter" resourcekey="SearchAndFilter" runat="server" />
            <div class="dnnFormItem">
                <dnn:Label ID="plFilterDataSet" runat="server" ControlName="txtFilter" Suffix=":" />
                <asp:TextBox ID="txtFilter" runat="server" />
                <span class="bccf-commands">
                    <asp:HyperLink CssClass="bccf-command" ID="hlpToken2" runat="server" resourcekey="cmdTokensHelp" NavigateUrl="#" />
					<asp:HyperLink CssClass="bccf-command" ID="hlpColumns2" runat="server" resourcekey="cmdColumnsHelp" NavigateUrl="#" />
                </span>
            </div>
            <div class="dnnFormItem" id="rowSearchBox" runat="server">
                <dnn:Label ID="plShowSearchTextBox" runat="server" ControlName="chkShowSearchTextBox" Suffix="?" />
                <asp:CheckBox ID="chkShowSearchTextBox" runat="server" AutoPostBack="true" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plSimpleSearch" runat="server" ControlName="chkSimpleSearch" Suffix="?" />
                <asp:CheckBox ID="chkSimpleSearch" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plShowNoRecordsUntilSearch" runat="server" ControlName="chkShowNoRecordsUntilSearch" Suffix="?" />
                <asp:CheckBox ID="chkShowNoRecordsUntilSearch" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plURLSearch" runat="server" ControlName="chkURLSearch" Suffix="?" />
                <asp:CheckBox ID="chkURLSearch" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plTopCount" runat="server" ControlName="txtTopCount" Suffix=":" />
                <asp:TextBox ID="txtTopCount" runat="server" CssClass="bccf-num" />
                <asp:RangeValidator ID="errTopCount" runat="server" ControlToValidate="txtTopCount" Type="Integer" MinimumValue="1" MaximumValue="1000" Display="Static" resourcekey="ErrTopCount" />
            </div>
        </fieldset>

        <h2 class="dnnFormSectionHead"><a href="">
            <asp:Label runat="server" resourcekey="EmailSettings" /></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:Label ID="plTrackingOn" runat="server" Suffix=":" ControlName="txtTrackingTrigger" />
                <table class="dnnLeft dnnFormRadioButtons">
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkTrackingOnNew" runat="server" resourcekey="TrackingOnNew" /></td>
                        <td>
                            <asp:CheckBox ID="chkTrackingOnUpdate" runat="server" resourcekey="TrackingOnUpdate" /></td>
                        <td>
                            <asp:CheckBox ID="chkTrackingOnDelete" runat="server" resourcekey="TrackingOnDelete" /></td>
                    </tr>
                </table>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plTrackingEmailAddresses" runat="server" Suffix=":" ControlName="txtTrackingEmailAddresses" />
                <asp:TextBox ID="txtTrackingEmailAddresses" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plTrackingEmail_CC" runat="server" Suffix=":" ControlName="txtTrackingEmail_CC" />
                <asp:TextBox ID="txtTrackingEmail_CC" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plTrackingEmail_BCC" runat="server" Suffix=":" ControlName="txtTrackingEmail_BCC" />
                <asp:TextBox ID="txtTrackingEmail_BCC" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plTrackingEmail_replyto" runat="server" Suffix=":" ControlName="txtTrackingEmail_replyto" />
                <asp:TextBox ID="txtTrackingEmail_replyto" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plTrackingEmail_from" runat="server" Suffix=":" ControlName="txtTrackingEmail_from" />
                <asp:TextBox ID="txtTrackingEmail_from" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plEmailSubject" runat="server" Suffix=":" ControlName="txtTrackingSubject" />
                <asp:TextBox ID="txtTrackingSubject" runat="server" MaxLength="998" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plTrackingMessage" runat="server" Suffix=":" ControlName="txtTrackingMessage" />
                <div class="dnnLeft">
                    <dnn:TextEditor ID="teTrackingMessage" runat="server" Height="250" Width="400" />
                </div>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plOnNew" runat="server" Suffix=":" ControlName="txtOnNew" resourcekey="plOnNew" />
                <asp:TextBox ID="txtOnNew" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plOnUpdate" runat="server" Suffix=":" ControlName="txtOnUpdate" resourcekey="plOnUpdate" />
                <asp:TextBox ID="txtOnUpdate" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plOnDelete" runat="server" Suffix=":" ControlName="txtOnDelete" resourcekey="plOnDelete" />
                <asp:TextBox ID="txtOnDelete" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plBody" runat="server" Suffix=":" ControlName="rblBodyType" />
                <asp:RadioButtonList ID="rblBodyType" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" AutoPostBack="True">
                    <asp:ListItem Value="Auto" resourcekey="Bodytype_Auto"></asp:ListItem>
                    <asp:ListItem Value="XslScript" resourcekey="Bodytype_Xsl"></asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem" id="rowTrackingScript" runat="server">
                <dnn:Label ID="plTrackingScript" runat="server" Suffix=":" ControlName="XslUserDefinedUrlControl" />
                <Portal:URL ID="XslTracking" runat="server" FileFilter="xsl,xslt" Required="True" ShowTrack="False" ShowNewWindow="False" ShowLog="False" ShowFiles="True" ShowUrls="False" ShowTabs="False" Width="200" />
                <div class="BelowUrlControl bccf-commands">
                    <asp:LinkButton ID="cmdEditEmail" runat="server" CssClass="bccf-command" resourcekey="cmdEditTrackingMessage" OnClick="cmdEditEmail_Click" />
				<asp:LinkButton CssClass="bccf-command" resourcekey="cmdGenerateTrackingMessage" ID="cmdGenerateEmail" runat="server" />
                </div>
            </div>

        </fieldset>

    </div>


    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdSaveAndBack" />
        </li>
        <li>
            <asp:LinkButton ID="cmdCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" CausesValidation="False" />
        </li>
    </ul>

</div>

<script type="text/javascript">

    function OpenHelpWindow(url) {
        window.open(url, '', 'width=800, height=800, location=no, menubar=no, resizable=yes, scrollbars=yes, status=no, toolbar=no');
        window.event.returnValue = false;
    }

    var editor;


    jQuery(function ($) {
        var mimeType =  "text/xml";

        var setupModule = function () {
           editor = CodeMirror.fromTextArea($("textarea[id$='<%= txtFormTemplate.ClientID %>']")[0], {
                lineNumbers: false,
                matchBrackets: true,
                lineWrapping: true,
                autofocus: true,
                mode: mimeType
            });
           
        };

        setupModule();

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {

            // note that this will fire when _any_ UpdatePanel is triggered,
            // which may or may not cause an issue
            setupModule();

        });
    });
</script>
