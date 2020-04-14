using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using SPCSOMTest.Models;

namespace SPCSOMTest.Controllers
{
    public class FileDownloadController : ApiController
    {
        #region ||  Property  ||
        public ClientContext ClContext { get; set; }
        #endregion ||  Property  ||


        public FileDownloadController()
        {
            string connectSiteUrl = ConfigurationManager.AppSettings["ConnectSiteUrl"];
            GetClContext(connectSiteUrl);
        }

        #region ||  API  ||
        [HttpPost]
        [Route("api/TEST")]        
        public string GetFileInfo([FromBody]TestParams param)
        {
            string fileUrl = param.fileUrl;

            Web web = ClContext.Web;
            ClContext.Load(web);
            ClContext.ExecuteQuery();

            File spfile = web.GetFileByUrl(fileUrl);
            ClContext.Load(spfile);
            ClContext.ExecuteQuery();

            List splist = spfile.ListItemAllFields.ParentList;
            ClContext.Load(splist);
            ClContext.ExecuteQuery();

            Microsoft.SharePoint.Client.ListItem splistitem = splist.GetItemByUniqueId(spfile.UniqueId);
            ClContext.Load(splistitem);
            ClContext.ExecuteQuery();


            Dictionary<string, object> dict = splistitem.FieldValues;
            return MyDictionaryToJson(dict);
        }

        [HttpPost]
        [Route("api/DownloadFile")]
        public string DownloadFile([FromBody]TestParams param)
        {
            try
            {
                string fileUrl = param.fileUrl;

                Web web = ClContext.Web;
                ClContext.Load(web);
                ClContext.ExecuteQuery();

                File spfile = web.GetFileByUrl(fileUrl);
                ClContext.Load(spfile);
                ClContext.ExecuteQuery();

                //파일 다운로드
                string downloadFolderPath = @"D:\TESTS\DownLoadFiles\";
                if (System.IO.Directory.Exists(downloadFolderPath)) System.IO.Directory.CreateDirectory(downloadFolderPath);
                GetFileBinary_ClientOM(ClContext, spfile.ServerRelativeUrl, string.Concat(downloadFolderPath, spfile.Name));

                return "OK";
            }
            catch { throw; }
        }
        #endregion ||  API  ||

        #region ||  Methods  ||
        /// <summary>
        /// Load Client Context
        /// </summary>
        /// <param name="webUrl"></param>
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

        /// <summary>
        /// Dictionary to JSON
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        string MyDictionaryToJson(Dictionary<string, object> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
            return "{" + string.Join(",", entries) + "}";
        }

        /// <summary>
        /// 파일 다운로드
        /// </summary>
        /// <param name="clContext"></param>
        /// <param name="fileUrlPath"></param>
        /// <param name="localFileNamePath"></param>
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
        #endregion ||  Methods  ||

    }
}
