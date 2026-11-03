<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Fields.ascx.cs" Inherits="BCC.Modules.BCCForms.Controls.Fields" %>
<%@ Import Namespace="BCC.Modules.BCCForms.Components" %>

<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Import Namespace="DotNetNuke.Entities.Icons" %>
<%@ Import Namespace="BCC.Modules.BCCForms.Components" %>
<%@ Register src="Field.ascx" tagname="Field" tagprefix="fnl" %>
<asp:HiddenField ID="FieldOrder" runat="server" />
<asp:DataGrid ID="grdFields" DataKeyField="UserDefinedFieldId"
			    CellPadding="2" AutoGenerateColumns="False" GridLines="None" 
			    runat="server" Width="100%" CssClass="Sortable" >
    
    <AlternatingItemStyle  CssClass="dnnGridAltItem" />
    <ItemStyle  CssClass="dnnGridItem" />
    <HeaderStyle CssClass="dnnGridHeader" />
	<Columns>
		<asp:TemplateColumn>
			<ItemStyle Wrap="False"></ItemStyle>
			<ItemTemplate>
                <a href="<%# HttpUtility.HtmlEncode(EditUrl((int) Eval(FieldsTableColumn.Id ))) %>"><img IconKey="Edit" alt="Edit" resourcekey="cmdEdit"  runat="server"/></a>
			<asp:ImageButton ID="cmdDeleteUserDefinedField" runat="server" CausesValidation="false"
			                        CommandName="Delete" IconKey="Delete" AlternateText="Delete" resourcekey="cmdDelete"
			                        Visible='<%#DataTypeByName((string) Eval(FieldsTableColumn.Type)).IsUserDefinedField%>' />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Title">
<ItemStyle CssClass="Draggable" Width="35%" />
			<ItemTemplate>
			    <asp:Label  runat="server" Text='<%#Shorten((string) Eval(FieldsTableColumn.Title))%>'  />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Type">
			
			<ItemStyle  />
			<ItemTemplate>
			    <asp:Label ID="Label2" runat="server" Text='<%#GetTypeName((string) Eval(FieldsTableColumn.Type))%>' />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Required">
			<HeaderStyle HorizontalAlign="Center" ></HeaderStyle>
			<ItemStyle HorizontalAlign="Center" ></ItemStyle>
			<ItemTemplate>
			    <%# YesNoMark(Eval(FieldsTableColumn.Required)) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Visible">
			<HeaderStyle HorizontalAlign="Center" />
			<ItemStyle HorizontalAlign="Center"  />
			<ItemTemplate>
			    <%# YesNoMark(Eval(FieldsTableColumn.Visible)) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Private">
			<HeaderStyle HorizontalAlign="Center"/>
			<ItemStyle HorizontalAlign="Center"  />
			<ItemTemplate>
			    <%# YesNoMark(Eval(FieldsTableColumn.IsPrivate)) %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Searchable">
			<HeaderStyle HorizontalAlign="Center"  />
			<ItemStyle HorizontalAlign="Center"  />
			<ItemTemplate>
			    <asp:PlaceHolder runat="server" Visible="<%#DataTypeByName((string) Eval(FieldsTableColumn.Type)).SupportsSearch%>"><%# YesNoMark(Eval("Searchable")) %></asp:PlaceHolder>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</asp:DataGrid>

<ul class="dnnActions dnnClear">
	<li>
        <asp:HyperLink ID="cmdAddField" CssClass="dnnPrimaryAction" runat="server" ResourceKey="cmdAddField"/>
    </li>
</ul>

<script type="text/javascript">
    (function ($) {
        $(document).ready(function () {
            $('#bccfConfig')
                .dnnTabs()
                .dnnPanels();
            $(".Sortable tbody")
                .sortable({
                    items: 'tr:has([id])',
                    placeholder: 'ui-state-highlight',
                    helper: function (e, tr) {
                        var $originals = tr.children();
                        var $helper = tr.clone();
                        $helper.children().each(function (index) {
                            // Set helper cell sizes to match the original sizes
                            $(this).width($originals.eq(index).width());
                        });
                        return $helper;
                    },
                    update: function (event, ui) {
                        var serial = $(this).sortable('toArray');
                        $('#<%=FieldOrder.ClientID%>').val(serial);
                        __doPostBack('__Page', 'FieldOrder');
                    }
                })
                .disableSelection();
        });
        } (jQuery));
  </script>