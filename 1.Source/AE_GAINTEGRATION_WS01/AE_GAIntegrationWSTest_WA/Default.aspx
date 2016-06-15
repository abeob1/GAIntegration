<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="AE_GAIntegrationWSTest_WA._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to GA Integration Web Service Test!
    </h2>
    <p>
       
    <asp:FileUpload id="FileUploadControl" runat="server" />
    <asp:Button runat="server" id="UploadButton" text="Upload" onclick="UploadButton_Click" />
    <br /><br />
        <asp:TextBox ID="txtResult" runat="server" TextMode="MultiLine" Height="126px" 
            Width="862px"></asp:TextBox>


    

       
    </p>
    <p>
    <asp:Label runat="server" id="StatusLabel" Font-Bold="true" text="Upload status: " />
    </p>
    
</asp:Content>
