<%@ Control Language="C#" CodeBehind="FieldSettings.ascx.cs" Inherits="BCC.Modules.BCCForms.Controls.FieldSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<%@ Import Namespace="BCC.Modules.BCCForms.Components" %>
<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <div id="Div1" class="dnnFormItem" runat="server"> 
            <dnn:Label id="Label" runat="server" 
                Text='<%# SelectedType.GetLocalization(  (string)Eval("Key") )%>'
                HelpText='<%# SelectedType.GetLocalization( String.Format("{0}.Help", Eval("Key")) )%>'  />
            <asp:TextBox ID="Textbox" runat="server" Width="400px"  Visible ='<%# Eval("SystemType").AsString() != "Boolean" %>' />
            <asp:CheckBox runat="server" ID ="Checkbox" Visible ='<%# Eval("SystemType").AsString() == "Boolean" %>'/>
        </div>
    </ItemTemplate>
</asp:Repeater>