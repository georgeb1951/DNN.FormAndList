<%@ Control Language="C#" Inherits="BCC.Modules.BCCForms.List" AutoEventWireup="false" CodeBehind="List.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Import Namespace="DotNetNuke.Entities.Icons" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<div class="dnnForm dnnClear bccForms bccf-list">
    <asp:Panel ID="panSearch" CssClass="bccf-search" runat="server" Visible="false" DefaultButton="cmdSearch">
        <asp:PlaceHolder ID="phSearchSentence" runat="server"/>
        <asp:LinkButton ID="cmdSearch" CssClass="dnnPrimaryAction" runat="server" resourcekey="cmdSearch" />
        <asp:LinkButton ID="cmdResetSearch" CssClass="dnnSecondaryAction" runat="server" resourcekey="cmdResetSearch" />
        <dnn:Label ID="plSearch" runat="server" Text="" />
        <asp:Label ID="lblNoRecordsFound" CssClass="bccf-search-message" runat="server" Visible="False" resourcekey="lblNoRecordsFound" />
    </asp:Panel>
    <div class="bccf-table-wrap">
    <asp:GridView ID="grdData" runat="server" Visible="False" EnableViewState="False"
                  PagerSettings-Visible="false" GridLines="None" AutoGenerateColumns="False"
                  AllowSorting="True" CssClass="dnnGrid bccf-grid" UseAccessibleHeader="True">
        <AlternatingRowStyle CssClass="dnnGridAltItem" />
        <RowStyle CssClass="dnnGridItem" />
        <HeaderStyle CssClass="dnnGridHeader" />
        <Columns>
            <asp:TemplateField ItemStyle-CssClass="bccf-edit" HeaderStyle-CssClass="bccf-edit">
                <ItemTemplate>
                    <asp:Label runat="server"
                               Text='<%# EditLinkHtml(DataBinder.Eval(Container, "DataItem.EditLink")) %>'>
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    </div>
    <dnn:PagingControl ID="ctlPagingControl" runat="server" totalrecord="0" PageSize="1" CurrentPage="1" Visible="False" Mode="PostBack" />
    <asp:PlaceHolder ID="XslOutput" runat="server" Visible="False" />
    <div runat="server" ID="placeholderActions" Visible="false" >
        <ul class="dnnActions dnnClear">
            <li>
                <asp:HyperLink runat="server" CssClass="dnnPrimaryAction" ID="ActionLink" Text="Action"></asp:HyperLink>
            </li>
        </ul>
    </div>
</div>