<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        Naam:<br />
        <asp:TextBox ID="naamTextBox" runat="server" 
            ontextchanged="naamTextBox_TextChanged"></asp:TextBox>
        <br />
        Voornaam:<br />
        <asp:TextBox ID="voorNaamTextBox" runat="server" 
            ontextchanged="voorNaamTextBox_TextChanged"></asp:TextBox>
        <br />
        Geboortedatum:<asp:Calendar ID="geboorteDatumCalendar" runat="server" 
            onselectionchanged="geboorteDatumCalendar_SelectionChanged">
        </asp:Calendar>
        <br />
        <asp:GridView ID="telefoonNrsGridView" runat="server" 
            AutoGenerateColumns="False" DataKeyNames="CommunicatieVormID" 
            onrowcancelingedit="telefoonNrsGridView_RowCancelingEdit" 
            onrowdeleting="telefoonNrsGridView_RowDeleting" 
            onrowediting="telefoonNrsGridView_RowEditing" 
            onrowupdating="telefoonNrsGridView_RowUpdating">
            <Columns>
                <asp:CommandField ShowDeleteButton="True" />
                <asp:TemplateField HeaderText="Telefoonnr.">
                    <EditItemTemplate>
                        <asp:TextBox ID="telefoonNrTextBox" runat="server" Text='<%# Bind("Nummer") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Nummer") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True" />
            </Columns>
            <EmptyDataTemplate>
                &nbsp;
            </EmptyDataTemplate>
        </asp:GridView>
        <br />
        <asp:TextBox ID="nieuwTelefoonNrTextBox" runat="server"></asp:TextBox>
        <asp:Button ID="telefoonNrToevoegenButton" runat="server" 
            onclick="telefoonNrToevoegenButton_Click" Text="Telefoonnr. toevoegen" />
        <br />
        <br />
        <asp:Button ID="bewarenKnop" runat="server" Text="Bewaren" 
            onclick="bewarenKnop_Click" />
&nbsp;<asp:Button ID="herstellenKnop" runat="server" Text="Herstellen" 
            onclick="herstellenKnop_Click" />
    
    </div>
    </form>
</body>
</html>
