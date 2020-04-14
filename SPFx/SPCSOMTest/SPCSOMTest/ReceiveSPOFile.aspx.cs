using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;

namespace SPCSOMTest
{
    public partial class ReceiveSPOFile : System.Web.UI.Page
    {
        public ClientContext ClContext { get; set; }

        public void GetClContext(string webUrl)
        {
            if (ClContext == null)
            {
                ClContext = new ClientContext(webUrl);

                //사용자 인증
                string loginEmail = "voyagesj@wspw.onmicrosoft.com";
                string strPassword = "@NEf0rAll";
                SecureString securePassword = new SecureString();
                foreach (var chr in strPassword) securePassword.AppendChar(chr);
                ClContext.Credentials = new SharePointOnlineCredentials(loginEmail, securePassword);
            }            
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string connectSiteUrl = ConfigurationManager.AppSettings["ConnectSiteUrl"];
            GetClContext(connectSiteUrl);
        }

        //작성자 업데이트
        protected void UpdateUserField(ClientContext ClContext, List list, Guid itemUniqueID)
        {
            Microsoft.SharePoint.Client.ListItem item = list.GetItemByUniqueId(itemUniqueID);
            User updateUser = ClContext.Web.EnsureUser("user1@wspw.onmicrosoft.com");
            ClContext.Load(updateUser);
            ClContext.ExecuteQuery();
            
            do {
                Thread.Sleep(1000);
                item = list.GetItemByUniqueId(itemUniqueID);
                ClContext.Load(item);

                
                item["Author"] = updateUser;
                item["Editor"] = updateUser;
                item.UpdateOverwriteVersion();

                ClContext.ExecuteQuery();
            
            }
            while (((FieldUserValue)item["Author"]).LookupId != updateUser.Id);
            
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            Web web = ClContext.Web;
            ClContext.Load(web);
            ClContext.ExecuteQuery();

            File spfile = web.GetFileByUrl(txtFileUrl.Text);
            ClContext.Load(spfile);
            ClContext.ExecuteQuery();

            List splist = spfile.ListItemAllFields.ParentList;
            ClContext.Load(splist);
            ClContext.ExecuteQuery();

            Microsoft.SharePoint.Client.ListItem splistitem = splist.GetItemByUniqueId(spfile.UniqueId);
            ClContext.Load(splistitem);
            ClContext.ExecuteQuery();

            //파일 이동
            string destinationFolderstring = splistitem["FileDirRef"] as string;
            destinationFolderstring = destinationFolderstring.Remove(destinationFolderstring.LastIndexOf('/'), destinationFolderstring.Length - destinationFolderstring.LastIndexOf('/'));
            destinationFolderstring = destinationFolderstring + "/완료";

            Folder destinationFoler = web.GetFolderByServerRelativeUrl(destinationFolderstring);
            try
            {
                ClContext.Load(destinationFoler);
                ClContext.ExecuteQuery();
            }
            catch
            {
                destinationFoler = web.Folders.Add(destinationFolderstring);
                ClContext.ExecuteQuery();
            }

            ClContext.Load(destinationFoler);
            ClContext.ExecuteQuery();

            spfile.MoveTo(destinationFoler.ServerRelativeUrl + "/" + spfile.Name, MoveOperations.Overwrite);
            ClContext.ExecuteQuery();

            File updateFile = web.GetFileByUrl(destinationFoler.ServerRelativeUrl + "/" + spfile.Name);
            ClContext.Load(updateFile);
            ClContext.ExecuteQuery();
            //작성자 업데이트
            UpdateUserField(ClContext, splist, updateFile.UniqueId);
        }




        //파일 다운로드
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            Web web = ClContext.Web;
            ClContext.Load(web);
            ClContext.ExecuteQuery();

            File spfile = web.GetFileByUrl(txtFileUrl.Text);
            ClContext.Load(spfile);
            ClContext.ExecuteQuery();

            //파일 다운로드
            string downloadFolderPath = @"C:\Users\spfarm\Downloads\";
            if (System.IO.Directory.Exists(downloadFolderPath)) System.IO.Directory.CreateDirectory(downloadFolderPath);
            GetFileBinary_ClientOM(ClContext, spfile.ServerRelativeUrl, string.Concat(downloadFolderPath,spfile.Name));
        }

        //파일 다운로드
        protected void GetFileBinary_ClientOM(ClientContext clContext, string fileUrlPath, string localFileNamePath)
        {

            int position = 1;
            int bufferSize = 200000;

            using (FileInformation fileInfo = File.OpenBinaryDirect(clContext, fileUrlPath))
            {
                if (System.IO.File.Exists(localFileNamePath)) System.IO.File.Delete(localFileNamePath);
                fileInfo.Stream.ReadTimeout = 120000;
                Byte[] readBuffer = new Byte[bufferSize];

                using (System.IO.Stream s = System.IO.File.Create(localFileNamePath))
                {
                    while (position > 0)
                    {
                        position = fileInfo.Stream.Read(readBuffer, 0, bufferSize);
                        s.Write(readBuffer, 0, position);
                        readBuffer = new Byte[bufferSize];

                    }
                    fileInfo.Stream.Flush();
                    s.Flush();

                }
            }
        }


        //파일정보 가져오기
        protected void btnGetFile_Click(object sender, EventArgs e)
        {
            Web web = ClContext.Web;
            ClContext.Load(web);
            ClContext.ExecuteQuery();

            File spfile = web.GetFileByUrl(txtFileUrl.Text);
            ClContext.Load(spfile);
            ClContext.ExecuteQuery();

            List splist = spfile.ListItemAllFields.ParentList;
            ClContext.Load(splist);
            ClContext.ExecuteQuery();

            Microsoft.SharePoint.Client.ListItem splistitem = splist.GetItemByUniqueId(spfile.UniqueId);
            ClContext.Load(splistitem);
            ClContext.ExecuteQuery();

            Dictionary<string, object> fields = splistitem.FieldValues;
            foreach (string fieldKey in fields.Keys)
            {
                lblFileInfo.Text += fieldKey + ":: " + splistitem[fieldKey];
                lblFileInfo.Text += "<br>";
            }

        }
    }
}