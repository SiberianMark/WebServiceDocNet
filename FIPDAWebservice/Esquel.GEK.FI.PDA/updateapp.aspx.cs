using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Esquel.GEK.QI.DataAccess;

namespace Esquel.GEK.FI.PDA
{
    public partial class updateapp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //string absURI = HttpContext.Current.Request.Url.AbsoluteUri;
            //string webRootURI = absURI.Substring(0, absURI.LastIndexOf("/"));
            //string downloadURL = webRootURI + "/" + "12345.xml";

            //lblUploadDetail.Text = "ApplicationPath: " + HttpContext.Current.Request.ApplicationPath + "<br />";
            //lblUploadDetail.Text += "Server.MapPath: " + HttpContext.Current.Server.MapPath(".") + "<br />";
            //lblUploadDetail.Text += "Request.Url.Host: " + HttpContext.Current.Request.Url.Host + "<br />";
            //lblUploadDetail.Text += "Request.Url.AbsoluteUri: " + HttpContext.Current.Request.Url.AbsoluteUri + "<br />";
            //lblUploadDetail.Text += "downloadURL: " + downloadURL + "<br />";
            
        }

        /// <summary>
        /// 上传文件，并更新版本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            string strName = FileUpload1.PostedFile.FileName;//使用fileupload控件获取上传文件的文件名
            int selIndex = dlUpdateType.SelectedIndex;
            string test = dlUpdateType.SelectedItem.Value;
            if (strName != "")//如果文件名存在
            {
                bool fileOK = false;
                int i = strName.LastIndexOf(".");//获取。的索引顺序号，在这里。代表文件名字与后缀的间隔
                string kzm = strName.Substring(i);//获取文件扩展名的另一种方法 string fileExtension = System.IO.Path.GetExtension(FileUpload1.FileName).ToLower();
                
                //string newName = Guid.NewGuid().ToString();//生成新的文件名，保证唯一性
                int NameIndex = 0;
                if (strName.Contains("\\"))
                {
                    NameIndex = strName.LastIndexOf("\\");
                }
                else if (strName.Contains("/"))
                {
                    NameIndex = strName.LastIndexOf("/");
                }
                string newName = strName.Substring(NameIndex);      // 保存的文件名采用上传的文件名！
                //string newName = "esquelQI";//生成新的文件名，保证唯一性

                string xiangdui = @"~\update\";//设置文件相对网站根目录的保存路径 ，~号表示当前目录，在此表示根目录下的update文件夹
                string juedui = Server.MapPath("~\\update\\");//设置文件保存的本地目录绝对路径，对于路径中的字符“＼”在字符串中必须以“＼＼”表示，因为“＼”为特殊字符。或者可以使用上一行的给路径前面加上＠
                string newFileName = juedui + newName;
                if (FileUpload1.HasFile)//验证 FileUpload 控件确实包含文件
                {
                    // String[] allowedExtensions = { ".gif", ".png", ".bmp", ".jpg", ".txt" };
                    String[] allowedExtensions = { ".apk" };
                    for (int j = 0; j < allowedExtensions.Length; j++)
                    {
                        if (kzm == allowedExtensions[j])
                        {
                            fileOK = true;
                        }
                    }
                }
                if (fileOK)
                {
                    try
                    {
                        // 判定该路径是否存在
                        if (!Directory.Exists(juedui))
                            Directory.CreateDirectory(juedui);
                        //Label3.Text = xiangdui + newName + kzm;
                        FileUpload1.PostedFile.SaveAs(newFileName);//将文件存储到服务器上

                        lblMessage.Text = "文件上传成功.";
                        // Label1.Text = newFileName;     //为了能看清楚我们提取出来的图片地址，在这使用label
                        lblUploadDetail.Text = "<b>原文件路径：</b>" + FileUpload1.PostedFile.FileName + "<br />" +
                                          "<b>文件大小：</b>" + FileUpload1.PostedFile.ContentLength + "字节<br />" +
                                          "<b>文件类型：</b>" + FileUpload1.PostedFile.ContentType + "<br />";

                        updateVersion(selIndex, newName);
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "文件上传失败." + ex.Message;
                    }
                }
                else
                {
                    lblMessage.Text = "只能够上传Android的apk安装文件.";
                }
            }
        }

        /// <summary>
        /// 更新版本
        /// </summary>
        /// <param name="appID">更新程序类型：0为成检PDA安装程序， 1为坯布挑修PDA安装程序</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool updateVersion(int appID, string fileName)
        {
            string absURI = HttpContext.Current.Request.Url.AbsoluteUri;
            string webRootURI = absURI.Substring(0, absURI.LastIndexOf("/"));
            string downloadURL = webRootURI + "/update/" + fileName.Trim(new char[]{'/','\\'});

            AppInfo FIApp = new AppInfo();
            if (appID == 0)
            {
                FIApp.AppID = 0;
                FIApp.App_Name = "FIPDA";
                FIApp.App_Desc = "成检组板PDA";
                FIApp.App_Version = "V1.0"; // 默认V1.0,若为更新则存储过程中自动获取最新版本号更新上去
                FIApp.Download_URL = downloadURL;
                FIApp.Local_URL = "";
                FIApp.Using_Dept = "FI";
                FIApp.Vender = "富士康";
                FIApp.Maintainer = "IT";
            }
            else if (appID == 1)
            {
                FIApp.AppID = 1;
                FIApp.App_Name = "FIMendPDA";
                FIApp.App_Desc = "QI挑修PDA";
                FIApp.App_Version = "V1.0"; // 默认V1.0,若为更新则存储过程中自动获取最新版本号更新上去
                FIApp.Download_URL = downloadURL;
                FIApp.Local_URL = "";
                FIApp.Using_Dept = "FI";
                FIApp.Vender = "富士康";
                FIApp.Maintainer = "IT";
            }
            else
            {
                FIApp.AppID = -1;
                FIApp.App_Name = "Unknown";
                FIApp.App_Desc = "Unknown";
                FIApp.App_Version = "V1.0"; // 默认V1.0,若为更新则存储过程中自动获取最新版本号更新上去
                FIApp.Download_URL = "";
                FIApp.Local_URL = "";
                FIApp.Using_Dept = "Unknown";
                FIApp.Vender = "Unknown";
                FIApp.Maintainer = "IT";
            }
            SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@AppID", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = FIApp.AppID},
                        new SqlParameter("@App_Name", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = FIApp.App_Name},
                        new SqlParameter("@App_Desc", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Input,  Value = FIApp.App_Desc},
                        new SqlParameter("@App_Version", SqlDbType.NVarChar, 10) { Direction = ParameterDirection.Input,  Value = FIApp.App_Version},
                        new SqlParameter("@Download_URL", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Input,  Value = FIApp.Download_URL},
                        new SqlParameter("@Local_URL", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Input,  Value = FIApp.Local_URL},
                        new SqlParameter("@Using_Dept", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Input,  Value = FIApp.Using_Dept},
                        new SqlParameter("@Vender", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Input,  Value = FIApp.Vender},
                        new SqlParameter("@Maintainer", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Input,  Value = FIApp.Maintainer}};
            int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("usp_FISaveAppInfo", parameters);

            if (updateResult > 0)
            {
                return true;
            }
            return false;
        }
    }

    class AppInfo
    {
        public int AppID { get; set; }
        public string App_Name { get; set; }
        public string App_Desc { get; set; }
        public string App_Version { get; set; }
        public string Download_URL { get; set; }
        public string Local_URL { get; set; }
        public string Using_Dept { get; set; }
        public string Vender { get; set; }
        public string Maintainer { get; set; }
    }
}