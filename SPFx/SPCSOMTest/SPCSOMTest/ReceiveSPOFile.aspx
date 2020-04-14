<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceiveSPOFile.aspx.cs" Inherits="SPCSOMTest.ReceiveSPOFile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox runat="server" ID="txtFileUrl" Text="/sites/SPTeamGroup/Shared Documents/General/202001_월간보고.docx"></asp:TextBox>
            <asp:Button runat="server" ID="btnGetFile" OnClick="btnGetFile_Click" Text="파일정보가져오기" />
            <asp:Button runat="server" ID="btnUpdate" OnClick="btnUpdate_Click" Text="파일이동 및 작성자 변경" />
            <asp:Button runat="server" ID="btnDownload" OnClick="btnDownload_Click" Text="파일다운로드" />
            <asp:Label runat="server" ID="lblFileInfo"></asp:Label>
        </div>
    </form>
</body>
</html>
