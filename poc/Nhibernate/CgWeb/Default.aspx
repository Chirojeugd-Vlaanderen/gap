<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CgWeb._Default" %>

<asp:Content ID="inhoud" ContentPlaceHolderID="mainContent" runat="server">
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
<br />
<br />
    <asp:GridView ID="persoonsInfoGrid" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="PersoonID" 
                DataNavigateUrlFormatString="PersoonPagina.aspx?Id={0}" DataTextField="PersoonID" 
                HeaderText="PersoonID" />
            <asp:BoundField DataField="VoorNaam" HeaderText="Voornaam" />
            <asp:BoundField DataField="Naam" HeaderText="Naam" />
            <asp:BoundField DataField="Categorieen" HeaderText="Categorieën" />
            <asp:BoundField DataField="StraatNaam" HeaderText="Straat" />
            <asp:BoundField DataField="HuisNr" HeaderText="Nr." />
            <asp:BoundField DataField="Bus" HeaderText="Bus" />
            <asp:BoundField DataField="PostNr" HeaderText="Postnr." />
            <asp:BoundField DataField="SubGemeente" HeaderText="Gemeente" />
            <asp:BoundField DataField="GeboorteDatum" HeaderText="Geboortedatum" />
            <asp:BoundField DataField="TelefoonNummer" HeaderText="Telefoonnr." />
            <asp:TemplateField HeaderText="E-mail">
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" 
                        NavigateUrl='<%# Eval("EMail", "mailto:{0}") %>' Text='<%# Eval("EMail") %>'></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
</asp:GridView>
&nbsp;
</asp:Content>