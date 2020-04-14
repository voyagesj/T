<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="APITest.aspx.cs" Inherits="SPCSOMTest.APITest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>

    <script src="https://code.jquery.com/jquery-3.4.1.min.js" integrity="sha256-CSXorXvZcTkaix6Yvo6HppcZGetbYMGWSFlBw8HfCJo=" crossorigin="anonymous"></script>
    <script type="text/javascript">
        function fn_TEST() { 
            
            var param = { "fileUrl" : "/sites/SPTeamGroup/Shared Documents/General/202001_월간보고.docx" };

            $.ajax({
                url: 'http://api.wspw.sharepoint.com/api/TEST',
                data:param,
                type: "POST",
                dataType: "json",
                header: { 'content-Type': 'Application/json;charset=utf-8' },
                async: false,
                success: function (data) {
                    alert(data);                    
                },
                error: function (result) {
                    alert(result.responseText);
                }
            });
        }

        function fn_DownloadFile() {
            
            var param = { "fileUrl": "/sites/SPTeamGroup/Shared Documents/General/202001_월간보고.docx" };

            $.ajax({
                url: 'http://api.wspw.sharepoint.com/api/DownloadFile',
                data: param,
                type: "POST",
                dataType: "json",
                header: { 'content-Type': 'Application/json;charset=utf-8' },
                async: false,
                success: function (data) {
                    alert(data);
                },
                error: function (result) {
                    alert(result.responseText);
                }
            });
        }
        

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <a href="javascript:fn_TEST();" >TEST</a>
            <br />
            <a href="javascript:fn_DownloadFile();" >fileDownload</a>
        </div>
    </form>
</body>
</html>
