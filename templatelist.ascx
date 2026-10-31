<%@ Control Language="C#"  Inherits="BCC.Modules.BCCForms.TemplateList"
            CodeBehind="TemplateList.ascx.cs" AutoEventWireup="false" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<div class="bccForms bccf-section bccf-toolbar">
    <asp:Label runat="server" ID="lblNewConfig" resourcekey="lblNewConfig">Start with a new </asp:Label> 
    <asp:HyperLink ID="cmdConfig" runat="server" CssClass="dnnPrimaryAction">
        <asp:Label ID="lblConfig" runat="server" resourcekey="lblConfig">Configuration</asp:Label>
    </asp:HyperLink>
    <asp:Label runat="server" ID="lblTemplate" resourcekey="lblTemplate">or choose a template:</asp:Label>
</div>
<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="TemplateDataSource"
              PagerSettings-Visible="false" GridLines="None" CssClass="dnnGrid bccf-grid">
    <AlternatingRowStyle CssClass="dnnGridAltItem" />

    <RowStyle CssClass="dnnGridItem" />
    <HeaderStyle CssClass="dnnGridHeader" />
    <Columns>
        <asp:TemplateField ShowHeader="False">
            <ItemTemplate>
                <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Select"
                                resourcekey="Select.Text" CssClass="dnnSecondaryAction"></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
        <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
    </Columns>
    <PagerSettings Visible="False" />
</asp:GridView>
<asp:ObjectDataSource ID="TemplateDataSource" runat="server" SelectMethod="TemplateList"
                      TypeName="BCC.Modules.BCCForms.Templates.TemplateController" />
<div>
    <asp:Label runat="server" ID ="lblCustomizeTemplate"></asp:Label>
    <dnn:CollectionEditorControl runat="server" ID="TemplateCustomValuesEditor" EditorDataField="Editor"
                                 EnableClientValidation="False" GroupByMode="None" NameDataField="Caption" ValueDataField="Value"
                                 LengthDataField="Length" VisibleDataField="TrueColumn" AutoGenerate="true" HelpDisplayMode="Never"
                                 ErrorStyle-CssClass="bccf-error" GroupHeaderStyle-CssClass="dnnFormSectionHead"
                                 GroupHeaderIncludeRule="True" HelpStyle-CssClass="Help"
                                 LabelStyle-CssClass="bccf-label">
    </dnn:CollectionEditorControl>
    <asp:LinkButton CssClass="dnnPrimaryAction" runat="server" ID="cmdApply" resourcekey="Select"
                    Visible="False"  OnClick="cmdApply_Click"/>
</div>