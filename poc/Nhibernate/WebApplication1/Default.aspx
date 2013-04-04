<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
%>
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
