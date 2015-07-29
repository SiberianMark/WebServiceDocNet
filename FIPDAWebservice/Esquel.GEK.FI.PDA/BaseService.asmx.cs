using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Esquel.GEK.QI.DataAccess;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web.Script.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LYH.ClassLib.IO;

namespace Esquel.GEK.FI.PDA
{
    /// <summary>
    /// QI项目APP接口
    /// </summary>
    [WebService(Namespace = "http://www.esquel.com/GEKFIPDA")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriquptService]
    public class BaseService : System.Web.Services.WebService
    {
        /// <summary>
        /// 接口返回字符串格式
        /// </summary>
        private static readonly string templateResult = "{\"result\":@result,\"message\":\"@message\"@value }";
        #region 成检PDA接口
        /// <summary>
        /// 接口返回值
        /// </summary>
        private string returnResult = "";

        /// <summary>
        /// 员工登录
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [WebMethod(Description="验证员工登录")]
        public string userLogin(string userID)
        {
            // 默认返回值
            string value = ",\"Name\":\"\"";       // 
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "用户登录失败").Replace("@value", "");

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@userID", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input, Value = userID}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_ExistsSystemUser", parameters);

                if (table.Rows.Count > 0)
                {
                    value = ",\"Name\":\""+table.Rows[0]["UserName"].ToString()+"\""; 
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 获取打印机IP列表
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [WebMethod(Description="获取打印机IP配置列表，isActive：true表示可用的，false表示正在占用的")]
        public string getPrintersIPList(bool isActive)
        {//{""result"":true,""message"":"""",""IPList"":[{""Numbe"":""1"",""IP"":""192.168.X.XX""},{""Numbe"":""1"",""IP"":""192.168.X.XX""}]}"

            // 默认返回值
            string value = ",\"IPList\":[{\"Number\":\"\",\"IP\":\"\"}]";       // 
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "无相关数据，请检查").Replace("@value", value);

            try
            {
                int activeFlag = 0;
                if (!isActive) activeFlag = 1;
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Department", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = "FI"},
                        new SqlParameter("@PrinterSystem", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Input,  Value = "组板PDA"},
                        new SqlParameter("@Is_Active", SqlDbType.Bit) { Direction = ParameterDirection.Input,  Value = activeFlag} };

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetValidPrinters", parameters);

                if (table != null && table.Rows.Count > 0)
                {// 
                    value = ",\"IPList\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"Number\":\"" + dr["PrinterNo"].ToString() + "\"";
                        value += ",\"IP\":\"" + dr["IP"].ToString() + "\"},";
                    }
                    value = value.TrimEnd(new char[] { ',' });
                    value += "]";


                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }

            /*
            // 默认返回值
            string value = ",\"IPList\":[{\"Number\":\"\",\"IP\":\"\"}]";       // 

            //string[] ipList = new string[] { "192.168.71.245", "192.168.71.246", "192.168.71.247", "192.168.43.245" };
            //string[] ipList = new string[] { "192.168.71.245", "192.168.71.246", "192.168.212.247", "192.168.43.245" };

            ConfigHelper.initConfigFile("Web.config");
            string[] IP_List = new string[4];
            string[] IP_Keys = new string[]{"IP1", "IP2", "IP3", "IP4"};
            for (int i = 0; i < IP_List.Length; i++)
            {
                IP_List[i] = ConfigHelper.GetAppConfig(IP_Keys[i]);
            }
            value = ",\"IPList\":[";
            for (int i = 0; i < 4; i++)
            {
                value += "{\"Number\":\"" + (i + 1).ToString() + "\"";
                value += ",\"IP\":\"" + IP_List[i] + "\"},";
            }
            value = value.TrimEnd(new char[] { ',' });
            value += "]";


            //string[] ipList = new string[] { "192.168.71.245", "192.168.71.246", "192.168.71.247", "192.168.71.248" };
            //value = ",\"IPList\":[";
            //for (int i = 0; i < 4; i++ )
            //{
            //    value += "{\"Number\":\"" + (i+1).ToString() + "\"";
            //    value += ",\"IP\":\"" + ipList[i] + "\"},";
            //}
            //value = value.TrimEnd(new char[] { ',' });
            //value += "]";
            
            returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
            return returnResult;
             * */
        }

        /// <summary>
        /// 更新打印机的可用状态
        /// </summary>
        /// <param name="printerNo"></param>
        /// <returns></returns>
        [WebMethod(Description = "更新打印机的可用状态，printerNo：打印机编号,isValid：true表示更新为可用的，false表示更新为不可用")]
        public string updatePrinterStatus(int printerNo, bool isValid)
        {
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "更新打印机位信息失败").Replace("@value", value);

            try
            {
                int validFlag = 0;
                if (!isValid) validFlag = 1;
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@PrinterNo", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = printerNo},
                    new SqlParameter("@Department", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = "FI"},
                    new SqlParameter("@PrinterSystem", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Input,  Value = "组板PDA"},
                    new SqlParameter("@Is_Active", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = validFlag}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("Usp_FIUpdateDNPrinterStatus", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 扫描小标签获取信息
        /// </summary>
        /// <param name="barcode">小标签条码</param>
        /// <returns>string格式{“result”:true/false,”message”:””,”卷号”:””,”缸号”:””，”重量”:”” ”工序”:””}</returns>
        [WebMethod(Description = "扫描小标签获取信息——processname直接返回\"待组板\"时表示工序为待组板")]
        public string getSmallLabelnfo(string barcode)
        {
            // 默认返回值为空
            string value = ",\"fabric_No\":\"\"";       // 卷号
            value += ", \"batch_No\":\"\"";     // 缸号
            value += ", \"weight\":\"\"";       // 重量
            value += ", \"processname\":\"\"";  // 工序

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "无相关数据，请检查").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = barcode}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIGetInspectedLabel", parameters);

                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"fabric_No\":\"" + table.Rows[0]["Fabric_No"].ToString() + "\"";        // 卷号
                    value += ", \"batch_No\":\"" + table.Rows[0]["Batch_No"].ToString() + "\"";         // 缸号
                    value += ", \"weight\":\"" + table.Rows[0]["Weight"].ToString() + "\"";             // 重量
                    value += ", \"processname\":\"" + table.Rows[0]["ProcessName"].ToString() + "\"";   // 工序

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
            
        }

        /// <summary>
        /// 获取花型图——添加返回花型图数据
        /// </summary>
        /// <param name="batch_No">缸号</param>
        /// <returns></returns>
        [WebMethod(Description = "获取花型图")]
        public string getFabricPattren(string batch_No)
        {//Height, Color_Abbr, Color 
            // 默认返回值
            string value = ",\"StripColor\":[{\"Height\":\"\",\"Red\":\"\",\"Green\":\"\",\"Blue\":\"\"}]";       // 
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "无相关的花型图信息，请检查").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@batch_No", SqlDbType.Char, 8) { Direction = ParameterDirection.Input,  Value = batch_No},
                    new SqlParameter("@InspectType", SqlDbType.Char, 2) { Direction = ParameterDirection.Input,  Value = "FI"}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_QIGetStripColor", parameters);

                if (table != null && table.Rows.Count > 0)
                {// 花型图直接返回
                    value = ",\"StripColor\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"Height\":\"" + dr["Height"].ToString() + "\"";
                        value += ",\"Red\":\"" + dr["Red"].ToString() + "\"";
                        value += ",\"Green\":\"" + dr["Green"].ToString() + "\"";
                        value += ",\"Blue\":\"" + dr["Blue"].ToString() + "\"},";
                    }

                    value = value.Remove(value.Length - 1, 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                /*
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"Height\":\"" + table.Rows[0]["Height"].ToString() + "\"";
                    value += ",\"Color_Abbr\":\"" + table.Rows[0]["Color_Abbr"].ToString() + "\"";
                    value += ",\"Color\":\"" + table.Rows[0]["Color"].ToString() + "\"";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                 * */

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 获取大标签信息
        /// </summary>
        /// <param name="barcode">小标签条码</param>
        /// <returns></returns>
        [WebMethod(Description = "根据小标签条码获取所有大标签信息")]
        public string getBigLabelInfo(string barcode)
        {//EXEC Usp_FIGetDefectPointMax :@Batch_No,:@Fabric_String

            // 默认返回值为空
            string value = ",\"biglabellist\":[{\"fabric_No\":\"\"";         // 卷号
            value += ", \"ppo_No\":\"\"";             // PPO_No
            value += ", \"usage\":\"\"";              // 用途 Usage
            value += ", \"batch_No\":\"\"";         // 缸号
            value += ", \"DnType\":\"\"";            // 装单类型
            value += ", \"gk_No\":\"\"";               // 品名
            value += ", \"combo_ID\":\"\"";         // Combo_ID
            value += ", \"combo\":\"\"";               // Combo
            value += ", \"Seriation_No\":\"\""; // 序号
            value += ", \"grade\":\"\"";               // Grade等级
            value += ", \"width\":\"\"";               // Width幅宽
            value += ", \"ozyd\":\"\"";                 // OZYD克重
            value += ", \"KG_allow_AF\":\"\"";    // 
            value += ", \"KG_allow\":\"\"";     // 
            value += ", \"KG_foc\":\"\"";         // 
            value += ", \"KG_allow_bf\":\"\"";        // 
            value += ", \"YD_allow_AF\":\"\"";    // 
            value += ", \"YD_allow\":\"\"";     // 
            value += ", \"YD_foc\":\"\"";         // 
            value += ", \"YD_allow_bf\":\"\"";        // 
            value += ", \"Deldate\":\"\"";     // 交付日期
            value += ", \"Destination\":\"\"";   // 交付地
            value += ", \"Defect_name\":\"\"";   // 疵点名称
            value += ", \"defect_Point\":\"\"";   // 疵点分数
            value += ", \"FBatch_no\":\"\"";       // FBatch_no
            value += ", \"data_colorde\":\"\"";       // data_colorde
            value += ", \"customer\":\"\"";       // customer
            value += ", \"Print_Date\":\"\"";                      // 打印时间
            value += ", \"Show_LightPic\":\"\"";                      // 显示轻拿轻放图片
            value += ", \"Send_QA\":\"\"";                      // 显示送QA的字样 'QA'
            value += ", \"Flag\":\"\"";                      // Flag
            value += ", \"Shade_Lot\":\"\"";               // 色级
            value += ", \"Is_Odd\":\"\"}]";               // 色级

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "获取数据失败").Replace("@value", value);

            // 由缸号+布号查找大标签信息
            try
            {
                // 根据barcode布号查找出缸号
                SqlParameter[] parameters0 = new SqlParameter[]{
                        new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = barcode}};
                DataTable table0 = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetSplitedInfoByFabricNo", parameters0);
                if (table0 == null || table0.Rows.Count < 1)
                {
                    return returnResult;
                }
                // 小标签的缸号
                string batch_No = table0.Rows[0]["Batch_No"].ToString();
                // modify by liangyoh 2014-11-13

                string fabricString = "";
                foreach (DataRow dr in table0.Rows)
                {
                    fabricString += dr["Fabric_No"].ToString() + ",";
                }
                fabricString = fabricString.TrimEnd(new char[] { ',' });
                /*
                // 与小标签布号同缸的所有布号
                string fabricString = "";
                foreach (DataRow dr in table0.Rows)
                {
                    fabricString += dr["Fabric_No"].ToString() + ",";
                }
                fabricString = fabricString.Remove(fabricString.Length - 1, 1);
                */
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Batch_No", SqlDbType.Char, 8) { Direction = ParameterDirection.Input,  Value = batch_No},
                        new SqlParameter("@Fabric_String", SqlDbType.VarChar, 6000) { Direction = ParameterDirection.Input,  Value = fabricString}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetInspectedBigLabel", parameters);

                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"biglabellist\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"fabric_No\":\"" + dr["Fabric_No"].ToString() + "\"";         // 卷号
                        value += ", \"ppo_No\":\"" + dr["PPO_No"].ToString() + "\"";             // PPO_No
                        value += ", \"usage\":\"" + dr["Usage"].ToString() + "\"";              // 用途 Usage
                        value += ", \"batch_No\":\"" + dr["Batch_No"].ToString() + "\"";         // 缸号
                        value += ", \"DnType\":\"" + dr["dn_type"].ToString() + "\"";            // 装单类型
                        value += ", \"gk_No\":\"" + dr["gk_no"].ToString() + "\"";               // 品名
                        value += ", \"combo_ID\":\"" + dr["Combo_ID"].ToString() + "\"";         // Combo_ID
                        value += ", \"combo\":\"" + dr["Combo"].ToString() + "\"";               // Combo
                        value += ", \"Seriation_No\":\"" + dr["Seriation_No"].ToString() + "\""; // 序号
                        value += ", \"grade\":\"" + dr["Grade"].ToString() + "\"";               // Grade等级
                        value += ", \"width\":\"" + dr["Width"].ToString() + "\"";               // Width幅宽
                        value += ", \"ozyd\":\"" + dr["OZYD"].ToString() + "\"";                 // OZYD克重
                        value += ", \"KG_allow_AF\":\"" + dr["aff_Weight"].ToString() + "\"";    // 
                        value += ", \"KG_allow\":\"" + dr["Allow_Weight"].ToString() + "\"";     // 
                        value += ", \"KG_foc\":\"" + dr["FOC_Weight"].ToString() + "\"";         // 
                        value += ", \"KG_allow_bf\":\"" + dr["Weight"].ToString() + "\"";        // 
                        value += ", \"YD_allow_AF\":\"" + dr["aff_Quantity"].ToString() + "\"";    // 
                        value += ", \"YD_allow\":\"" + dr["Allow_Quantity"].ToString() + "\"";     // 
                        value += ", \"YD_foc\":\"" + dr["FOC_Quantity"].ToString() + "\"";         // 
                        value += ", \"YD_allow_bf\":\"" + dr["Quantity"].ToString() + "\"";        // 
                        value += ", \"Deldate\":\"" + string.Format("{0:yyyy-MM-dd}", (DateTime)dr["Delivery_Date"]) + "\"";     // 交付日期
                        value += ", \"Destination\":\"" + dr["Destination"].ToString() + "\"";   // 交付地
                        value += ", \"Defect_name\":\"" + dr["Defect_name"].ToString() + "\"";   // 疵点名称
                        value += ", \"defect_Point\":\"" + dr["defect_Point"].ToString() + "\"";   // 疵点分数
                        value += ", \"FBatch_no\":\"" + dr["FBatch_no"].ToString() + "\"";       // FBatch_no
                        value += ", \"data_colorde\":\"" + dr["data_colorde"].ToString() + "\"";       // data_colorde
                        value += ", \"customer\":\"" + dr["customer_code"].ToString() + "\"";       // customer
                        value += ", \"Print_Date\":\"" + string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + "\"";              // 打印时间 
                        
                        // 是否显示轻拿轻放的图片
                        if (dr["Mark"].ToString().ToUpper() == "LIGHT")
                        {
                            value += ", \"Show_LightPic\":\"Y\"";  
                        }
                        else
                        {
                            value += ", \"Show_LightPic\":\"N\""; 
                        }
                        // 是否送QA
                        if (dr["Send_QA"].ToString().ToUpper() == "QA")
                        {
                            value += ", \"Send_QA\":\"Y\"";  
                        }
                        else
                        {
                            value += ", \"Send_QA\":\"N\"";                      // 是否送QA
                        }
                        value += ", \"Flag\":\"" + dr["Flag"].ToString() + "\"";                      // Flag
                        value += ", \"Shade_Lot\":\"" + dr["Shade"].ToString() + "\"";                // 色级
                        value += ", \"Is_Odd\":\"" + dr["Is_Odd"].ToString() + "\"},";               // 是否多余布
                    }
                    value = value.Remove(value.Length - 1, 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 获取位置（如果存在位置列表）
        /// </summary>
        /// <returns>string格式{“result”:true/false,”message”:””,”locations”:[ {”location_code”:”A1”, "location_desc":"成检1区"}] }</returns>
        [WebMethod(Description = "获取位置列表, 返回位置代码和位置描述信息")]
        public string getLocationList()
        {
            // 默认返回值
            string value = ",\"locations\":\"[{\"location_code\":\"\", \"location_desc\":\"\"}]\"";

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有相关定位信息").Replace("@value", value);

            try
            {
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetBanLocationList", null);

                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"locations\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"location_code\":\"" + dr["Location_Code"].ToString() + "\",";          // 位置代码
                        value += "\"location_desc\":\"" + dr["Location_Name"].ToString() + "\"},";          // 位置名称Location_Name
                    }
                    value = value.Remove(value.Length - 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 是否多余布
        /// </summary>
        /// <param name="fabric_No"></param>
        /// <returns></returns>
        [WebMethod(Description = "验证是否为多余布")]
        public string isOddFabric(string fabric_No)
        {
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "该卷为非多余布").Replace("@value", "");
            try
            {
                /*
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Fabric_No", SqlDbType.VarChar, 8000) { Direction = ParameterDirection.Input,  Value = fabric_No}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIBoardInfo", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    if (table.Rows[0]["Dn_Type"].ToString().ToUpper() == "E")
                    {
                        returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", "");
                    }
                }
                 */

                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@FabricNo", SqlDbType.VarChar, 8000) { Direction = ParameterDirection.Input,  Value = fabric_No},
                        new SqlParameter("@DN_Type", SqlDbType.VarChar, 2) { Direction = ParameterDirection.ReturnValue,  Value = ""} };
                object rtValue = SqlHelper.ExecuteScalarByStoredProcedure("Udf_GetFIDNType", parameters);
                if (rtValue != null)
                {
                    if (rtValue.ToString().ToUpper() == "E")
                    {
                        returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", "");
                    }
                }
                

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 更新定位信息
        /// </summary>
        /// <param name="barcode">标签布号-条码</param>
        /// <param name="place">定位代码</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebMethod(Description="更新组板定位信息")]
        public string qiUpdPlace(string barcode, string place, string userId)
        {
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "更新小标签定位信息失败").Replace("@value", value);

            barcode = barcode.Replace("'", "");

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@userId", SqlDbType.Char, 3) { Direction = ParameterDirection.Input,  Value = userId},
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = barcode},
                    new SqlParameter("@location_code", SqlDbType.Char, 4) { Direction = ParameterDirection.Input,  Value = place}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("usp_FIUpdateBanLocation", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 获取待组板信息
        /// </summary>
        /// <param name="barcode">大标签条码</param>
        /// <returns></returns>
        [WebMethod(Description = "获取待组板信息")]
        public string getFIDNBanInfo(string barcode)
        {
            // 默认返回值为空
            string value = ",\"fabric_No\":\"\"";   // 卷号
            value += ", \"weight\":\"\"";           // 重量
            value += ", \"Is_Odd\":\"\"";           // 是否为多余布

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有对应的待组板信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@fabric_No", SqlDbType.VarChar, 8000) { Direction = ParameterDirection.Input,  Value = barcode}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetDNBanInfoByFabricNo", parameters);
               
                if (table != null && table.Rows.Count > 0)
                {
                    DataRow dr = table.Rows[0];
                    value = ",\"fabric_No\":\"" + dr["fabric_no"].ToString() + "\"";   // 卷号
                    value += ", \"weight\":\"" + dr["Weight"].ToString() + "\"";       // 重量
                    value += ", \"Is_Odd\":\"" + dr["Is_Odd"].ToString() + "\"";       // 是否为多余布

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);

                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 获取入仓单号
        /// </summary>
        /// <param name="barcode">大标签条码列表</param>
        /// <param name="userId">员工号</param>
        /// <returns></returns>
        [WebMethod(Description="根据大标签条码列表获取入仓单信息， 传入大标签的条码列表以,隔开")]
        public string getFIWarehousingEntry(string barcode, string userId)
        {//Usp_FIGetMaxSendNo
            // 默认返回值为空
            string value = ",\"warehousingEntryInfo\":[{\"fabric_No\":\"\"";    // 卷号
            value += ", \"note_No\":\"\"}]";                                     // 入仓单号

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有相关数据，请检查").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = null;
                string Note_No = "";

                // 控制不同装单类型的卷不能一并组板，直接影响到仓库的接收
                //DataTable tableLimitPayST = SqlHelper.RunQuery("SELECT distinct DN_Type FROM ");
                //if (tableLimitPayST != null && tableLimitPayST.Rows.Count > 0)
                //{
                //    Note_No = tableLimitPayST.Rows[0]["note_No"].ToString();
                //}

                // 检查分组对色的无法组板
                parameters = new SqlParameter[]{
                        new SqlParameter("@fabric_list", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Input,  Value = barcode}};
                DataTable tbFirstShade = SqlHelper.ExecuteDataTableByStoredProcedure("usp_fifindfirstshade", parameters);
                if (tbFirstShade != null && tbFirstShade.Rows.Count > 0)
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有ROLL TO ROLL 分组对色，无法组板，请联系QA！").Replace("@value", value);

                    return returnResult;
                }


                // 生成送布单号
                parameters = new SqlParameter[]{
                        new SqlParameter("@Int", SqlDbType.TinyInt) { Direction = ParameterDirection.Input,  Value = 0},
                        new SqlParameter("@Note_No", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output,  Value = ""}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMaxSendNo", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    Note_No = table.Rows[0]["note_No"].ToString();
                }

                // 根据工号获取工人中文名，传入组板操作人中
                string userInfo = userLogin(userId);
                string userName = userId;
                if (userInfo.ToLower().IndexOf("true") > 0)
                {
                    //JsonReader reader = new JsonTextReader(new StringReader(userInfo));
                    JObject jobject = JObject.Parse(userInfo);
                    string[] values = jobject.Properties().Select(item => item.Value.ToString()).ToArray();

                    if (values[2] != "")
                    {
                        userName = values[2];
                    }
                }


                // 成检组板 //qualitydb.dbo.usp_FIPayST :@Note_No,:@Operator,:@strFabric,:@Remark,:@Result output
                parameters = new SqlParameter[]{
                        new SqlParameter("@Note_No", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = Note_No},
                        new SqlParameter("@Operator", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = userName},
                        new SqlParameter("@strFabric", SqlDbType.VarChar, 8000) { Direction = ParameterDirection.Input,  Value = barcode},
                        new SqlParameter("@Remark", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = "入成品仓"},
                        new SqlParameter("@Result", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output,  Value = ""}};
                DataTable tableFIPayST = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIPayST", parameters);
                if (parameters[4].Value.ToString() != "OK")
                {
                    return value;
                }

                // 送TDC //:noteNo,:userName,:ReturnMsg
                /*
                if (sendTDC)
                {
                    parameters = new SqlParameter[]{
                        new SqlParameter("@Note_No", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Input,  Value = Note_No},
                        new SqlParameter("@userName", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = userId},
                        new SqlParameter("@ReturnMsg", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output,  Value = ""}};
                    DataTable tableSendTDC = SqlHelper.ExecuteDataTableByStoredProcedure("fabricstoreDB.dbo.usp_stFiToTdc", parameters);
                    if ((tableSendTDC == null) || (tableSendTDC.Rows.Count < 1) || (parameters[2].Value.ToString() != "OK"))
                    {
                        return value;
                    } 
                }
                 * /

                // 缸号的备注中存在“直出”字样的记录， 提示，这里不做
                /*
                    parameters = new SqlParameter[]{

                        new SqlParameter("@BatchNos", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Input,  Value = "？？"}};
                    DataTable tableSendTDC = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_GetZhiChuBatch", parameters);
                    if ((tableSendTDC == null) || (tableSendTDC.Rows.Count < 1) || (parameters[2].Value.ToString() != "OK"))
                    {
                        return value;
                    } 
                */

                // 更新位置信息
                string fabricStr = "";
                string[] fabricList = barcode.Split(new char[] { ',' });
                for (int i = 0; i < fabricList.Length; i++)
                {
                    fabricStr = "'" + fabricList[i] + "',";
                }
                fabricStr = fabricStr.TrimEnd(new char[] { ',' });
                string updatePlaceResult = qiUpdPlace(fabricStr, "", userId);



                // 入仓单信息
                parameters = new SqlParameter[]{
                        new SqlParameter("@Note_No", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Input,  Value = Note_No}};
                DataTable tableNoteInfo = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetNoteInfoFromSTIn", parameters);
                if (tableNoteInfo != null && tableNoteInfo.Rows.Count > 0)
                {
                    value = ",\"warehousingEntryInfo\":[";
                    foreach (DataRow dr in tableNoteInfo.Rows)
                    {
                        value += "{\"fabric_No\":\"" + dr["fabric_no"].ToString() + "\"";
                        value += ", \"note_No\":\"" + dr["note_no"].ToString() + "\"},";
                    }
                    value = value.Remove(value.Length - 1, 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// WIP汇总查询
        /// </summary>
        /// <param name="ppoType">订单类别：0.所有订单,1.大货订单，2.样板订单</param>
        /// <param name="sortFieldType">1.卷数排序， 2.重量排序， 3.码长排序</param>
        /// <param name="sortType">0.降序， 1.升序</param>
        /// <returns></returns>
        [WebMethod(Description = "WIP汇总查询——ppoType,订单类别：0.所有订单,1.大货订单，2.样板订单; sortFieldType:1.卷数排序， 2.重量排序， 3.码长排序； sortType：0.降序， 1.升序。")]
        public string getWIPSummary(int ppoType, int sortFieldType, int sortType)
        {
            // 默认返回值为空
            string value = ",\"sumarry\":[{\"PrcSort\":\"\"";     // 部门
            value += ",\"Info\":[{";
            value += ", \"ProcessName\":\"\"";      // 工序
            value += ", \"Roll_Count\":\"\"";       // 卷数
            value += ", \"Weight\":\"\"";           // 重量
            value += ", \"Quantity\":\"\"}]";       // 码长
            value += "}]";

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "").Replace("@value", value);

            if (ppoType != 1 && ppoType != 2)
            {
                ppoType = 0;
            }

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Flag", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = 1},
                        new SqlParameter("@PpoType", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = ppoType}};
                DataTable dtWIPTotal = SqlHelper.ExecuteDataTableByStoredProcedure("usp_GetQiWipDatas", parameters);

                //dtWIPTotal.Rows.Add(new object[] { "FI", "IT测试1", 20, 11.11, 20 });
                //dtWIPTotal.Rows.Add(new object[] { "FI", "IT测试2", 30, 22.22, 40 });

                if (dtWIPTotal != null && dtWIPTotal.Rows.Count > 0)
                {
                    // 先把不同的PrcSort保存起来
                    ArrayList prcSorts = new ArrayList();
                    string tempPrcSort = dtWIPTotal.Rows[0]["PrcSort"].ToString();
                    prcSorts.Add(tempPrcSort);
                    foreach(DataRow dr in dtWIPTotal.Rows)
                    {
                        if(!prcSorts.Contains(dr["PrcSort"].ToString()))
                        {
                            prcSorts.Add(dr["PrcSort"].ToString());
                        }
                    }


                    value = ",\"sumarry\":[";
                    for (int i = 0; i < prcSorts.Count; i++)
                    {
                        DataView dv = dtWIPTotal.DefaultView;
                        dv.RowFilter = "PrcSort='"+prcSorts[i]+"'";
                        if(sortFieldType==1)
                        {
                            if(sortType==0)
                                dv.Sort = "Roll_Count desc";
                            else
                                dv.Sort = "Roll_Count asc";
                        }
                        else if (sortFieldType == 2)
                        {
                            if (sortType == 0)
                                dv.Sort = "Weight desc";
                            else
                                dv.Sort = "Weight asc";
                        }
                        else
                        {
                            if (sortType == 0)
                                dv.Sort = "Quantity desc";
                            else
                                dv.Sort = "Quantity asc";
                        }

                        value += "{\"PrcSort\":\"" + prcSorts[i] + "\"";  // 部门
                        value += ", \"Info\":[";             
                        foreach (DataRowView drv in dv)
                        {
                            value += "{\"ProcessName\":\"" + drv["ProcessName"].ToString() + "\"";      // 工序
                            value += ", \"Roll_Count\":\"" + drv["Roll_Count"].ToString() + "\"";        // 卷数
                            value += ", \"Weight\":\"" + drv["Weight"].ToString() + "\"";                // 重量
                            value += ", \"Quantity\":\"" + drv["Quantity"].ToString() + "\"},";          // 码长
                        }
                        value = value.Remove(value.Length - 1, 1);
                        value += "]},";
                    }
                    value = value.Remove(value.Length - 1, 1);
                    value += "]";
                    /*
                    foreach (DataRow dr in dtWIPTotal.Rows)
                    {
                        value += "{\"PrcSort\":\"" + dr["PrcSort"].ToString() + "\"";               // 部门
                        value += ", \"Info\":[";
                        value += ", \"ProcessName\":\"" + dr["ProcessName"].ToString() + "\"";      // 工序
                        value += ", \"Roll_Count\":\"" + dr["Roll_Count"].ToString() + "\"";        // 卷数
                        value += ", \"Weight\":\"" + dr["Weight"].ToString() + "\"";                // 重量
                        value += ", \"Quantity\":\"" + dr["Quantity"].ToString() + "\"},";          // 码长
                    }
                    value = value.Remove(value.Length - 1);
                    value += "]";
                     * */

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 待组板WIP查询：以缸为单位显示汇总：缸号、重量、卷数
        /// </summary>
        /// <param name="ppoType">订单类别：0.所有订单,1.大货订单，2.样板订单</param>
        /// <param name="sortType">排序类别：0.按照重量降序排序,1.按照重量升序排序</param>
        /// <returns></returns>
        [WebMethod(Description = "待组板WIP查询")]
        public string getWIPDetailInDNBan(int ppoType, int sortType)
        {
            // 默认返回值为空
            string value = ",\"BanWIPdetail\":[{\"batch_No\":\"\"";     // 缸号
            value += ", \"weight\":\"\"";           // 重量
            value += ", \"fabric_No\":\"\"";       // 卷号
            value += ", \"location\":\"\"}]";       // 位置

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有待组板信息").Replace("@value", value);

            if (ppoType != 1 && ppoType != 2)
            {
                ppoType = 0;
            }

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ppoType", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = ppoType}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_GetQiWipDNBanDatas", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    DataView dv = table.DefaultView;
                    if (sortType == 0)
                        dv.Sort = "weight desc";
                    else
                        dv.Sort = "weight asc";

                    value = ",\"BanWIPdetail\":[";
                    foreach (DataRowView dr in dv)
                    {
                        value += "{\"batch_No\":\"" + dr["batch_No"].ToString() + "\"";     // 缸号
                        value += ", \"weight\":\"" + dr["weight"].ToString() + "\"";        // 重量
                        value += ", \"roll_Count\":\"" + dr["roll_Count"].ToString() + "\"},";       // 卷数
                        //value += ", \"fabric_No\":\"" + dr["fabric_No"].ToString() + "\"";       // 卷数
                        //value += ", \"location\":\"" + dr["Ban_Location_Code"].ToString() + "\"},";         // 位置
                    }
                    value = value.Remove(value.Length - 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 缸号查询WIP明细
        /// </summary>
        /// <param name="ppoType"></param>
        /// <param name="batch_No"></param>
        /// <returns></returns>
        [WebMethod(Description = "按缸号查询WIP明细")]
        public string getWIPDetailByBatchNo(int ppoType, string batch_No)
        {
            // 默认返回值为空
            string value = ",\"WIPdetail\":[{\"seriation_No\":\"\",\"fabric_No\":\"\"";     // 验布序号、卷号
            value += ", \"processname\":\"\"";      // 工序
            value += ", \"Weight\":\"\"";           // 重量
            value += ", \"location\":\"\"}]";       // 位置

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "该缸号没有相关WIP记录").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Batch_No", SqlDbType.Char, 8) { Direction = ParameterDirection.Input,  Value = batch_No},
                        new SqlParameter("@PpoType", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = ppoType}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_GetQiBatchNoWipDatas", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"WIPdetail\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"seriation_No\":\"" + dr["Seriation_No"].ToString() + "\"";           // 验布序号
                        value += ", \"fabric_No\":\"" + dr["Fabric_NO"].ToString() + "\"";           // 卷号
                        value += ", \"processname\":\"" + dr["Processname"].ToString() + "\"";      // 工序
                        value += ", \"Weight\":\"" + dr["Weight"].ToString() + "\"";                // 重量
                        value += ", \"location\":\"" + dr["Ban_Location_Code"].ToString() + "\"},";          // 位置
                    }
                    value = value.Remove(value.Length - 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }



        /// <summary>
        /// 获取卷号详细信息
        /// </summary>
        /// <param name="fabric_No">卷号</param>
        /// <returns></returns>
        [WebMethod(Description = "FIPDA获取卷号详细信息")]
        public string getFabricInfo(string fabric_No)
        {
            // 默认返回值
            string value = ", \"Remark\":\"\"";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关坯布信息").Replace("@value", value);

            try
            {

                // 根据fabric_No布号查找出缸号
                SqlParameter[] parameters0 = new SqlParameter[]{
                        new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No}};
                DataTable table0 = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetInspectedInfoByFabricNo", parameters0);
                if (table0 == null || table0.Rows.Count < 1)
                {
                    return returnResult;
                }
                // 小标签的缸号
                string batch_No = table0.Rows[0]["Batch_No"].ToString();
                // modify by liangyoh 2014-12-4

                
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@Inpect_Type", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Input,  Value = "FI"},
                    new SqlParameter("@Batch_No", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Input,  Value = batch_No},
                    new SqlParameter("@fabric_No", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = fabric_No}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_QIGetInspected_FabricMainInfo", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    if (table.Rows[0]["Remarks"] == null)
                    {
                        value = ",\"Remark\":\"N/A\"";
                    }
                    else if (table.Rows[0]["Remarks"].ToString().Trim() == "")
                    {
                        value = ",\"Remark\":\"N/A\"";
                    }
                    else
                    {
                        value = ",\"Remark\":\"" + table.Rows[0]["Remarks"].ToString().Replace("\"", "\\\"") + "\"";
                    }
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                
                /*
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@Batch_No", SqlDbType.Char, 8) { Direction = ParameterDirection.Input,  Value = ""},
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No},
                    new SqlParameter("@InspectType", SqlDbType.Char, 2) { Direction = ParameterDirection.Input,  Value = "FI"}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_QIGetInspectionLabel", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"Remark\":\"" + table.Rows[0]["Remarks"].ToString() + "\"";
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                */
                return returnResult;
            }
            catch
            {
                return returnResult;
            }
            //return "{\"result\":true/false, \"message\":\"\", \"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\",\"Receive_Time\":\"\"}";
        }

        /// <summary>
        /// 通过大标签条码获取退补单号
        /// </summary>
        /// <param name="fabric_No">大标签条码（布号）</param>
        /// <returns></returns>
        /// [WebMethod(Description = "通过大标签条码获取退布单号")]
        public string getNoteNoByFabricNo(string fabric_No)
        {//usp_FIGetNoteNoFromFabricNo
            // 默认返回值为空
            string value = ",\"Note_No\":\"\"";       // 退布单号
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有相关的退单号").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetNoteInfoFromST", parameters);

                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"Note_No\":\"" + table.Rows[0]["Note_No"].ToString() + "\"";        //退布单号
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        ///  退布原因
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "退布原因列表")]
        public string getReturnReasonList()
        {
            // 默认返回值为空
            string value = ",\"returnReason\":[{\"code\":\"\"";     // 原因编号
            value += ", \"description\":\"\"}]";       // 原因名称/描述


            returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有相关退布原因数据").Replace("@value", value);

            try
            {
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_GetQiReturnReason", null);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"returnReason\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"code\":\"" + dr["Return_Code"].ToString() + "\"";           // 原因编号
                        value += ", \"description\":\"" + dr["Return_Description"].ToString() + "\"},";   // 原因名称/描述
                    }
                    value = value.Remove(value.Length - 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }


        /// <summary>
        /// 单号验证
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        [WebMethod(Description = "验证退布单号是否存在")]
        public string isStNoteNoValid(string barCode)
        {
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "该单没有退布记录").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Note_No", SqlDbType.Char, 10) { Direction = ParameterDirection.Input,  Value = barCode}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIExistsBarCodeLabel", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }
        
        /// <summary>
        /// 接收仓库退布
        /// </summary>
        /// <param name="userName">操作人员，取用户NT账号</param>
        /// <param name="note_No">退布单号，若有多个布号则布号以,串接</param>
        /// <param name="returnReason">退布原因</param>
        /// <returns></returns>
        [WebMethod(Description = "接收仓库退布，同步保存回KMIS —— 退布单号，若接收多个则note_No以,串接")]
        public string FIFabricApplyFromST(string userId, string note_No, string location_code)
        {//USP_FIFabricPDAReceiveFromST
            
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "接收仓库退布失败").Replace("@value", "");

            string[] note_No_List = note_No.Replace("'", "").Split(new char[] { ',' });
            string errorMsg = "";
            //string fabric_No_List = "";

            try
            {
                for (int i = 0; i < note_No_List.Length; i++)
                {

                    //fabric_No_List = "";

                    SqlParameter[] parameters = new SqlParameter[]{
                            new SqlParameter("@operator", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Input,  Value = userId},
                            new SqlParameter("@Note_No", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Input,  Value = note_No_List[i]},
                            new SqlParameter("@location_code", SqlDbType.VarChar, 4) { Direction = ParameterDirection.Input,  Value = location_code}};

                    // modify by liangyoh 2014-11-15 
                    DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("USP_FIFabricPDAReceiveFromST", parameters);
                    if (table != null && table.Rows.Count > 0)
                    {
                        if (table.Rows[0]["Msg"].ToString() != "OK")
                        {
                            errorMsg += note_No_List[i] + "接收仓库退布失败:" + table.Rows[0]["Msg"] + "；";
                            break;
                        }

                    }
                    else
                    {
                        return templateResult.Replace("@result", "false").Replace("@message", "没有相关的仓库退布布号").Replace("@value", "");
                    }

                    /*
                    fabric_No_List = "";

                    SqlParameter[] parameters = new SqlParameter[]{
                            new SqlParameter("@Note_No", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Input,  Value = note_No_List[i]}};

                    // modify by liangyoh 2014-11-15 
                    DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("USP_FIFabricPDAReceiveFromST", parameters);
                    if (table != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow dr in table.Rows)
                        {
                            SqlParameter[] parameters1 = new SqlParameter[]{
                                new SqlParameter("@operator", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Input,  Value = userId},
                                new SqlParameter("@FabNostr", SqlDbType.VarChar, 4000) { Direction = ParameterDirection.Input,  Value = dr["fabric_No"].ToString()},
                                new SqlParameter("@BackReason", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Input,  Value = dr["Remark"].ToString()},
                                new SqlParameter("@Flag", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Input,  Value = "RECEIVE"},
                                new SqlParameter("@Note_No", SqlDbType.VarChar, 30) { Direction = ParameterDirection.InputOutput, Value=note_No_List[i]}};

                            DataTable table1 = SqlHelper.ExecuteDataTableByStoredProcedure("USP_FIFabricApplyFromST", parameters1);
                            if (table1 != null && table1.Rows.Count > 0)
                            {
                                if (table1.Rows[0]["Msg"].ToString() == "OK")
                                {
                                    string updataLocationResult = qiUpdPlace(fabric_No_List, location_code, userId);
                                    if (updataLocationResult.Contains("false"))
                                    {
                                        errorMsg += note_No_List[i] + "更新定位信息失败；";
                                        break;
                                    }
                                }
                                else
                                {
                                    errorMsg += note_No_List[i] + "接收仓库退布失败:" + table1.Rows[0]["Msg"] + "；";
                                    break;
                                }
                            }
                            else
                            {
                                errorMsg += note_No_List[i] + "接收仓库退布失败；";
                                break;
                            }
                        }

                    }
                    else
                    {
                        return templateResult.Replace("@result", "false").Replace("@message", "没有相关的仓库退布布号").Replace("@value", "");
                    }
                     * */
                }

                if (errorMsg != "")
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", errorMsg.TrimEnd(new char[] { '；' })).Replace("@value", "");
                }
                else
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", "");
                }

                return returnResult;
            }
            catch(Exception e)
            {
                returnResult = templateResult.Replace("@result", "false").Replace("@message", "接收仓库退布失败!").Replace("@value", "");
                return returnResult;
            }
        }

        /// <summary>
        /// 接收仓库退布
        /// </summary>
        /// <param name="userName">操作人员，取用户NT账号</param>
        /// <param name="barCode">退布布号，若有多个布号则布号以,串接</param>
        /// <param name="note_No">条码,即退布单号</param>
        /// <param name="returnReason">退布原因</param>
        /// <returns></returns>
        [WebMethod(Description = "接收仓库退布，同步保存回KMIS —— 退布布号，若接收多个布号则barCode以,串接")]
        public string FIFabricApplyFromST_SingleNoteNo(string userId, string barCode, string note_No, string location_code, string returnReason)
        {//
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "接收仓库退布失败").Replace("@value", "");

            // 清除所有'
            barCode = barCode.Replace("'", "");
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@operator", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Input,  Value = userId},
                        new SqlParameter("@FabNostr", SqlDbType.VarChar, 4000) { Direction = ParameterDirection.Input,  Value = barCode},
                        new SqlParameter("@BackReason", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Input,  Value = returnReason},
                        new SqlParameter("@Flag", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Input,  Value = "RECEIVE"},
                        new SqlParameter("@Note_No", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output,  Value = note_No}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("USP_FIFabricApplyFromST", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    if (table.Rows[0]["Msg"].ToString() == "OK")
                    {
                        returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", "");

                        string updataLocationResult = qiUpdPlace(barCode, location_code, userId);
                        if (updataLocationResult.Contains("false"))
                        {
                            returnResult = templateResult.Replace("@result", "true").Replace("@message", "更新定位信息失败").Replace("@value", "");
                        }
                    }
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 通过标签获取退仓单号和大标签信息
        /// </summary>
        /// <param name="barcode">大标签条码</param>
        /// <returns></returns>
        [WebMethod(Description = "通过大标签获取退仓单号和大标签信息")]
        public string getNoteInfoFromST(string barcode)
        {// 
            // 默认返回值为空
            string value = ",\"Note_No\":\"\"";       // 退布单号

            // 默认返回值为空
            value += ",barCodeList\":[{\"fabric_No\":\"\"}]";         // 卷号
            #region 其他信息不需要
            /*
            value += ", \"ppo_No\":\"\"";             // PPO_No
            value += ", \"usage\":\"\"";              // 用途 Usage
            value += ", \"batch_No\":\"\"";         // 缸号
            value += ", \"DnType\":\"\"";            // 装单类型
            value += ", \"gk_No\":\"\"";               // 品名
            value += ", \"combo_ID\":\"\"";         // Combo_ID
            value += ", \"combo\":\"\"";               // Combo
            value += ", \"Seriation_No\":\"\""; // 序号
            value += ", \"grade\":\"\"";               // Grade等级
            value += ", \"width\":\"\"";               // Width幅宽
            value += ", \"ozyd\":\"\"";                 // OZYD克重
            value += ", \"KG_allow_AF\":\"\"";    // 
            value += ", \"KG_allow\":\"\"";     // 
            value += ", \"KG_foc\":\"\"";         // 
            value += ", \"KG_allow_bf\":\"\"";        // 
            value += ", \"YD_allow_AF\":\"\"";    // 
            value += ", \"YD_allow\":\"\"";     // 
            value += ", \"YD_foc\":\"\"";         // 
            value += ", \"YD_allow_bf\":\"\"";        // 
            value += ", \"Deldate\":\"\"";     // 交付日期
            value += ", \"Destination\":\"\"";   // 交付地
            value += ", \"Defect_name\":\"\"";   // 疵点名称
            value += ", \"defect_Point\":\"\"";   // 疵点分数
            value += ", \"FBatch_no\":\"\"";       // FBatch_no
            value += ", \"data_colorde\":\"\"";       // data_colorde
            value += ", \"customer\":\"\"";       // customer
            value += ", \"Print_Date\":\"\"";                      // 打印时间
            value += ", \"Flag\":\"\"";                      // Flag
            value += ", \"Shade_Lot\":\"\"}]";               // 色级
             * */
            #endregion

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有相关的退单号").Replace("@value", value);

            try
            {
                // 根据barcode布号查找出缸号和退仓单号
                SqlParameter[] parameters0 = new SqlParameter[]{
                        new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = barcode}};
                DataTable table0 = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetNoteInfoFromSTOut", parameters0);
                if (table0 == null || table0.Rows.Count < 1)
                {
                    return returnResult;
                }
                // 当前布号对应的缸号，单号
                // modify by liangyoh 2014-11-15 列出该退仓单对应的所有卷号
                string batch_No = table0.Rows[0]["Batch_No"].ToString();
                string note_No = table0.Rows[0]["note_No"].ToString();

                if (table0 != null && table0.Rows.Count > 0)
                {
                    value = ",\"Note_No\":\"" + note_No + "\" ,\"barCodeList\":[";

                    foreach (DataRow dr in table0.Rows)
                    {
                        value += "{\"fabric_No\":\"" + dr["Fabric_No"].ToString() + "\"},";         // 卷号
                    }
                    value = value.Remove(value.Length - 1, 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                /*
                string fabricString = barcode;
                if (table0.Rows.Count > 0)
                {
                    foreach (DataRow dr in table0.Rows)
                    {
                        fabricString += dr["fabric_No"].ToString() + ",";
                    }
                }

                fabricString = fabricString.TrimEnd(new char[] { ',' });
                // 获取大标签信息
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Batch_No", SqlDbType.Char, 8) { Direction = ParameterDirection.Input,  Value = batch_No},
                        new SqlParameter("@Fabric_String", SqlDbType.VarChar, 6000) { Direction = ParameterDirection.Input,  Value = barcode}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetDefectPointMax", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"Note_No\":\"" + note_No + "\" ,\"barCodeList\":[";

                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"fabric_No\":\"" + dr["Fabric_No"].ToString() + "\"},";         // 卷号
                        #region 其他信息不需要
                        /*
                        value += ", \"ppo_No\":\"" + dr["PPO_No"].ToString() + "\"";             // PPO_No
                        value += ", \"usage\":\"" + dr["Usage"].ToString() + "\"";              // 用途 Usage
                        value += ", \"batch_No\":\"" + dr["Batch_No"].ToString() + "\"";         // 缸号
                        value += ", \"DnType\":\"" + dr["dn_type"].ToString() + "\"";            // 装单类型
                        value += ", \"gk_No\":\"" + dr["gk_no"].ToString() + "\"";               // 品名
                        value += ", \"combo_ID\":\"" + dr["Combo_ID"].ToString() + "\"";         // Combo_ID
                        value += ", \"combo\":\"" + dr["Combo"].ToString() + "\"";               // Combo
                        value += ", \"Seriation_No\":\"" + dr["Seriation_No"].ToString() + "\""; // 序号
                        value += ", \"grade\":\"" + dr["Grade"].ToString() + "\"";               // Grade等级
                        value += ", \"width\":\"" + dr["Width"].ToString() + "\"";               // Width幅宽
                        value += ", \"ozyd\":\"" + dr["OZYD"].ToString() + "\"";                 // OZYD克重
                        value += ", \"KG_allow_AF\":\"" + dr["aff_Weight"].ToString() + "\"";    // 
                        value += ", \"KG_allow\":\"" + dr["Allow_Weight"].ToString() + "\"";     // 
                        value += ", \"KG_foc\":\"" + dr["FOC_Weight"].ToString() + "\"";         // 
                        value += ", \"KG_allow_bf\":\"" + dr["Weight"].ToString() + "\"";        // 
                        value += ", \"YD_allow_AF\":\"" + dr["aff_Quantity"].ToString() + "\"";    // 
                        value += ", \"YD_allow\":\"" + dr["Allow_Quantity"].ToString() + "\"";     // 
                        value += ", \"YD_foc\":\"" + dr["FOC_Quantity"].ToString() + "\"";         // 
                        value += ", \"YD_allow_bf\":\"" + dr["Quantity"].ToString() + "\"";        // 
                        value += ", \"Deldate\":\"" + dr["Delivery_Date"].ToString() + "\"";     // 交付日期
                        value += ", \"Destination\":\"" + dr["Destination"].ToString() + "\"";   // 交付地
                        value += ", \"Defect_name\":\"" + dr["Defect_name"].ToString() + "\"";   // 疵点名称
                        value += ", \"defect_Point\":\"" + dr["defect_Point"].ToString() + "\"";   // 疵点分数
                        value += ", \"FBatch_no\":\"" + dr["FBatch_no"].ToString() + "\"";       // FBatch_no
                        value += ", \"data_colorde\":\"" + dr["data_colorde"].ToString() + "\"";       // data_colorde
                        value += ", \"customer\":\"" + dr["customer_code"].ToString() + "\"";       // customer
                        value += ", \"Print_Date\":\"" + DateTime.Now.ToString() + "\"";              // 打印时间
                        value += ", \"Flag\":\"" + dr["Flag"].ToString() + "\"";                      // Flag
                        value += ", \"Shade_Lot\":\"" + dr["Shade"].ToString() + "\"}]";               // 色级

                         * *
                        #endregion
                    }
                    value = value.Remove(value.Length - 1, 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                */
                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }




        /// <summary>
        /// 保存组板时打印入仓单失败的入仓单信息
        /// </summary>
        /// <param name="unprinted_Note_No"></param>
        /// <returns></returns>
        [WebMethod(Description = "保存打印异常的入仓单")]
        public string saveSTInNoteInfo(string unprinted_Note_No)
        {// 
            // 默认返回值为空
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "无法记录打印错误的入仓单信息").Replace("@value", value);

            try
            {
                //@Print_System VARCHAR(50),
                //@Print_Key  varchar(50),
                //@Print_Desc varchar(100),
                //@Print_Dept varchar(10),
                //@Printer  nvarchar(20),
                //@Error_Log  varchar(200)
                SqlParameter[] parameters0 = new SqlParameter[]{
                        new SqlParameter("@Print_System", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Input,  Value = "FIPDA"},
                        new SqlParameter("@Print_Key", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Input,  Value = unprinted_Note_No},
                        new SqlParameter("@Print_Desc", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Input,  Value = "组板打印入仓单"},
                        new SqlParameter("@Print_Dept", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Input,  Value = "FI"},
                        new SqlParameter("@Printer", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = "便携打印机"},
                        new SqlParameter("@Error_Log", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Input,  Value = "打印异常"}};
                DataTable dt = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FISavePrintErrorLog", parameters0);
                if (dt == null || dt.Rows.Count<=0)
                {
                    return returnResult;  
                }
                else if (dt.Rows[0]["Result"].ToString() == "OK")
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value); 
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 获取指定时间内入仓单，目前指定为8小时内的入仓单
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "获取指定时间内入仓单，目前指定为8小时内的入仓单")]
        public string getSTInNoteInfo()
        {// 若更改接口，则添加limitHour限制指定时间内的入仓单
            // 默认返回值为空
            string value = ",\"noteInfoList\":[{\"note_No\":\"\", \"roll_Count\":\"\", \"print_status\":\"\"}]";         // 入仓单

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有8小时内扫描打印的入仓单").Replace("@value", value);

            try
            {
                // 根据barcode布号查找出缸号和退仓单号
                SqlParameter[] parameters0 = new SqlParameter[]{
                        new SqlParameter("@limitHours", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = 8},
                        new SqlParameter("@print_Dept", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Input,  Value = "FI"}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetNoteInfoInHours", parameters0);
                if (table == null || table.Rows.Count < 1)
                {
                    return returnResult;
                }

                value = ",\"noteInfoList\":[";
                foreach (DataRow dr in table.Rows)
                {
                    value += "{\"note_No\":\"" + dr["Note_No"].ToString() + "\",";         // 卷号
                    value += "\"roll_Count\":\"" + dr["Roll_Count"].ToString() + "\",";         // 卷号
                    value += "\"print_status\":" + dr["Print_Status"].ToString() + "},";         // 卷号
                }
                value = value.Remove(value.Length - 1, 1);
                value += "]";

                returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 更新打印记录的状态，即清除打印异常的打印记录
        /// </summary>
        /// <param name="noteNoList">入仓单列表</param>
        /// <returns></returns>
        [WebMethod(Description = "更新打印记录的状态，即清除打印异常的打印记录")]
        public string updatePrintStatus(string noteNoList)
        {
            // 默认返回值
            string value = "";       //
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有需要更新的入仓单记录或更新失败，请检查").Replace("@value", value);

            string noteNoListStr = noteNoList.Trim().Replace("'", "");
            //string[] noteLists = noteNoListStr.Split(new char[] { ',' });

            //noteNoListStr = "";
            //for (int i = 0; i < noteLists.Length; i++)
            //{
            //    noteNoListStr += "'" + noteLists[i] + "'" + ",";
            //}
            //noteNoListStr = noteNoListStr.TrimEnd(new char[] { ',' });

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@noteNoList", SqlDbType.VarChar, 400) { Direction = ParameterDirection.Input,  Value = noteNoListStr}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("usp_FIUpdateNotePrintStatus", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }        
        #endregion

        /// <summary>
        /// 获取服务器上APP的最新版本
        /// </summary>
        /// <param name="appID">表示当前程序的程序ID：0表示成检PDA程序；1表示挑修PDA程序</param>
        /// <returns></returns>
        [WebMethod(Description = "获取服务器上APP的最新版本信息")]
        public string getVersion(int appID)
        {
            string value = ",\"version\":\"\", \"url\":\"\"";         // app的最新版本以及下载url

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "").Replace("@value", value);
            //// http://192.168.7.184/gekfipda/update
            try
            {
                SqlParameter[] parameters0 = new SqlParameter[]{
                        new SqlParameter("@appID", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = appID}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetLastVersion", parameters0);
                if (table == null || table.Rows.Count < 1)
                {
                    return returnResult;
                }


                if (table.Rows.Count > 0)
                {
                    value = ",\"version\":\"" + table.Rows[0]["App_Version"].ToString() + "\"";
                    value += ",\"url\":\"" + table.Rows[0]["Download_URL"].ToString() + "\"";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        #region 坯布挑修接口
        /// <summary>
        /// 员工登录:采用工号登录;
        /// </summary>
        /// <param name="appID">应用程序ID，0表示成检PDA；1表示挑修PDA</param>
        /// <param name="userID"></param>
        /// <returns>string格式{"result":true/false,"message":"","userRight","1,2,3,4"}</returns>
        [WebMethod(Description = "坯布挑修接口————员工工号登录，appID表示应用程序ID：0表示成检PDA；1表示挑修PDA")]
        public String userLogon(int appID, string userID)
        {
            // 默认返回值为空
            string value = ",\"UsersInfo\":[{\"name\":\"\",\"login_time\":\"\",\"userRight\":\"\"}]";       // 用户权限

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该员工信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@userID", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = userID}};

                if (appID == 1)
                {
                    DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMendUserInfo", parameters);
                    if (table != null && table.Rows.Count > 0)
                    {
                        value = "";
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            value += "{\"name\":\"" + table.Rows[i]["Name"].ToString() + "\"";
                            value += ",\"login_time\":\"" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\"";
                            value += ",\"userRight\":\"" + table.Rows[i]["UserRight"].ToString() + "\"}";
                            if (i != (table.Rows.Count - 1))
                            {
                                value = value + ",";
                            }
                        }
                        value = ",\"UsersInfo\":[" + value + "]";
                        returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                        
                    }

                    return returnResult;
                }
                else
                {
                    return userLogin(userID);
                }
            }
            catch
            {
                return returnResult;
            }
        }


        /// <summary>
        /// 新增用户权限("工卡条码或工卡卡号","权限1,权限2,…");
        /// </summary>
        /// <param name="userIDs">用户ID</param>
        /// <param name="userRight">用户权限</param>
        /// <returns>是否新增成功</returns>
        [WebMethod(Description = "坯布挑修接口————更新用户权限")]
        public string updateUserRight(string userIDs, string userRight)
        {
            // 默认返回值
            string value = "";       // 用户权限

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该员工信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@userIDs", SqlDbType.VarChar, 800) { Direction = ParameterDirection.Input,  Value = userIDs},
                    new SqlParameter("@userRight", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Input,  Value = userRight}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("Usp_FISaveMendUserRight", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "更新员工权限失败").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }
        ///// <summary>
        ///// 坯布接收：获取待接收的坯布信息
        ///// </summary>
        ///// <param name="fabric_No">坯布布号</param>
        ///// <returns>布号,车号,重量,紧急单</returns>
        ///// [WebMethod(Description = "坯布挑修：获取待接收坯布信息，urgent:表示是否急单，1表示急单，否则正常单； urgentLevel：0表示正常单；1表示当天及未来5天内OTD（列表粉红色标记）;2表示已过交期（列表红色标记）")]
        //private string getAcceptableRawFabricInfo(string fabric_No)
        //{//CD9260701400
        //    // 默认返回值
        //    string value = ",\"fabric_No\":\"\"  ,\"vehicle_No\":\"\" , \"weight\":\"\" , \"urgent\":\"\" , \"urgentLevel\":\"\"";       // 

        //    returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该布号信息").Replace("@value", value);

        //    try
        //    {
        //        SqlParameter[] parameters = new SqlParameter[]{
        //            new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No},
        //            new SqlParameter("@mend_type", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = 1}};

        //        DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMendRawFabricInfo", parameters);
        //        if (table != null && table.Rows.Count > 0)
        //        {
        //            value = ",\"fabric_No\":\"" + table.Rows[0]["Fabric_No"].ToString() + "\"";
        //            value += ",\"vehicle_No\":\"" + table.Rows[0]["Vehicle_No"].ToString() + "\"";
        //            value += ",\"weight\":\"" + table.Rows[0]["Weight"].ToString() + "\"";
        //            value += ",\"urgent\":\"" + table.Rows[0]["Is_Urgent"].ToString() + "\"";
        //            value += ",\"urgentLevel\":\"" + table.Rows[0]["UrgentLevel"].ToString() + "\"";
        //            returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
        //        }

        //        return returnResult;
        //    }
        //    catch
        //    {
        //        return returnResult;
        //    }
        //}

        /// <summary>
        /// 坯布接收
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="fabric_No_List">卷号列表</param>
        /// <returns>更新接收状态是否成功</returns>
        [WebMethod(Description = "坯布挑修接口————坯布接收")]
        public string acceptRawFabric(string userID, string fabric_No_List)
        {//usp_FIUpdateRawReceiveStatus
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "坯布接收失败").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@userID", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = userID},
                    new SqlParameter("@fabric_No_List", SqlDbType.VarChar, 800) { Direction = ParameterDirection.Input,  Value = fabric_No_List}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("Usp_FIReceiveMendRawFabric", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 开始挑修：获取卷号基本信息
        /// </summary>
        /// <param name="fabric_No">卷号</param>
        /// <returns>检查卷号是否可进行挑修</returns>
        [WebMethod(Description = "坯布挑修接口————扫描卷号获取基本信息，包括是否Hold、是否完成挑修，检查卷号是否可进行挑修. Mend_Type:0获取所有状态；1表示获取待接收；2表示获取待挑修；3表示获取在挑修，4表示获取已挑修")]
        public string getMendRawFabricInfo(string fabric_No, int mend_Type)
        {//usp_FIGetRepairRawInfo
            // 默认返回值
            string value = ",\"FabricList\":[{\"fabric_No\":\"\", \"vehicle_No\":\"\", \"weight\":\"\", \"urgent\":\"\", \"urgentLevel\":\"0\", \"mend_Status\":\"\", \"finished\":\"\", \"mend_Start\":\"\", \"mend_End\":\"\", \"is_Hold\":\"\", \"hold_Remark\":\"\", \"stay_time\":\"\"}]";       // 
            string Mend_Status = "";
            returnResult = templateResult.Replace("@result", "false").Replace("@message", Mend_Status).Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No},
                    new SqlParameter("@mend_type", SqlDbType.Int) { Direction = ParameterDirection.Input,  Value = mend_Type}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMendRawFabricInfo", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = "";
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        //string.Format("{0:G}", dt);//2005-11-5 14:23:23
                        value += "{\"fabric_No\":\"" + table.Rows[i]["Fabric_No"].ToString() + "\"";
                        value += ",\"weight\":\"" + table.Rows[0]["Weight"].ToString() + "\"";
                        value += ",\"vehicle_No\":\"" + table.Rows[i]["Vehicle_No"].ToString() + "\"";
                        value += ",\"urgent\":\"" + table.Rows[0]["Is_Urgent"].ToString() + "\"";
                        value += ",\"urgentLevel\":\"" + table.Rows[i]["UrgentLevel"].ToString() + "\"";
                        value += ",\"mend_Status\":\"" + table.Rows[i]["Mend_Status"].ToString() + "\"";
                        value += ",\"finished\":\"" + table.Rows[i]["Is_Finished"].ToString() + "\"";
                        //value += ",\"mend_Start\":\"" + table.Rows[i]["Mend_Start"].ToString() + "\"";
                        //value += ",\"mend_End\":\"" + table.Rows[i]["Mend_End"].ToString() + "\"";
                        value += ",\"mend_Start\":\"" + Convert.ToDateTime(table.Rows[i]["Mend_Start"]).ToString("yyyy-MM-dd HH:mm:ss") + "\"";
                        value += ",\"mend_End\":\"" + Convert.ToDateTime(table.Rows[i]["Mend_End"]).ToString("yyyy-MM-dd HH:mm:ss") + "\"";


                        value += ",\"is_Hold\":\"" + table.Rows[i]["Is_Hold"].ToString() + "\"";
                        value += ",\"hold_Remark\":\"" + table.Rows[i]["Hold_Remark"].ToString() + "\"";
                        value += ",\"stay_time\":\"" + table.Rows[i]["Stay_Time"].ToString() + "\"}";
                        if (i != (table.Rows.Count - 1))
                        {
                            value = value + ",";
                        }
                    }
                    value = ",\"BigFabricInfo\":\"" + table.Rows[0]["BigFabricNo"].ToString() + "\"" + ",\"FabricList\":[" + value + "]";
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                else
                {
                    try
                    {
                        SqlParameter[] parameter = new SqlParameter[] { new SqlParameter("@fabric_no", SqlDbType.Char, 12) { Direction = ParameterDirection.Input, Value = fabric_No }, };
                        DataTable tables = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMendFabricStatus", parameter);
                        if (tables != null && tables.Rows.Count > 0)
                        {
                            Mend_Status = "布号状态为"+tables.Rows[0]["Mend_Status"] +"，请检查";
                        }
                        returnResult = templateResult.Replace("@result", "false").Replace("@message", Mend_Status).Replace("@value", value);
                    }
                    catch
                    {
                           return returnResult;
                    }
                } 
                return returnResult;
                }
           
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 开始挑修：解除Hold绑定
        /// </summary>
        /// <param name="fabric_No">卷号</param>
        /// <param name="updateHoldRemarkFlag">解除是否需要更新Hold备注</param>
        /// <param name="holdRemark">解除Hold的备注信息</param>
        /// <returns>检查卷号是否可进行挑修</returns>
        [WebMethod(Description = "坯布挑修接口————开始挑修中更新Hold信息， updateHoldRemarkFlag：表示是否需要更新Hold备注； holdRemak：Hold备注")]
        public string updateHold(string fabric_No, bool is_Hold, bool updateHoldRemarkFlag, string holdRemark)
        {
            // 默认返回值
            string value = "";
            int holdFlag = 0;
            int updateFlag = 0;
            if (is_Hold)
            {
                holdFlag = 1;
            }
            if (updateHoldRemarkFlag) updateFlag = 1;

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "解Hold失败，请联系IT管理人员").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No},
                    new SqlParameter("@holdFlag", SqlDbType.Bit) { Direction = ParameterDirection.Input,  Value = holdFlag},
                    new SqlParameter("@updateHoldRemarkFlag", SqlDbType.Bit) { Direction = ParameterDirection.Input,  Value = updateFlag},
                    new SqlParameter("@holdRemark", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Input,  Value = holdRemark}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("Usp_FIUpdateMendHold", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 开始挑修：获取卷号信息
        /// </summary>
        /// <param name="fabric_No">卷号</param>
        /// <param name="reMendFlag">是否重新开挑</param>
        /// <returns>品名, 称重, 卡号, 状态, 接收时间</returns>
        [WebMethod(Description = "坯布挑修接口————开始挑修，获取卷号信息， reMendFlag：true表示重新挑修")]
        public string startMendRawFabric(string fabric_No, bool reMendFlag)
        {//usp_FIGetRepairRawInfo
            // 默认返回值

            string value = ",\"BigFabricInfo:\"\",\"fabricList\":[{\"fabric_No\":\"\", \"weight\":\"\", \"vehicle_No\":\"\", \"stay_time\":\"\"}]";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关挑修信息").Replace("@value", value);

            try
            {
                int remend = 0;
                if (reMendFlag) remend = 1;

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No},
                    new SqlParameter("@isReMend", SqlDbType.Bit) { Direction = ParameterDirection.Input,  Value = remend}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIStartMendRawFabric", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = "";
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        value += "{\"fabric_No\":\"" + table.Rows[i]["Fabric_No"].ToString() + "\"";
                        value += ",\"weight\":\"" + table.Rows[i]["Weight"].ToString() + "\"";
                        value += ",\"vehicle_No\":\"" + table.Rows[i]["Vehicle_No"].ToString() + "\"";
                        value += ",\"stay_time\":\"" + table.Rows[i]["Stay_Time"].ToString() + "\"}";
                        if (i != (table.Rows.Count - 1))
                        {
                            value = value + ",";

                        }
                    }
                    value = ",\"BigFabricInfo\":\"" + table.Rows[0]["BigFabricNo"].ToString() + "\"" + ",\"FabricList\":[" + value + "]";
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 坯布挑修：开始挑修，点击卷号获取卷号详细信息
        /// </summary>
        /// <param name="fabric_No"></param>
        /// <returns></returns>
        [WebMethod(Description = "坯布挑修接口————开始挑修，点击卷号获取卷号详细信息")]
        public string getMendRawFabricDetail(string fabric_No)
        {//Usp_FIGetMendtableRawFabricDetail
            // 默认返回值
            string value = ",\"ppo_No\":\"\", \"raw_GKNo\":\"\", \"customer\":\"\", \"deliveryPlace\":\"\", \"stay_time\":0, \"flyFlag\":\"N\",\"kn_Remark\":\"\",\"order_quantity\":\"0\", \"finished_quantity\":\"0\"";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关挑修信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No},};

                //DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMendtableRawFabricDetail", parameters);
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMendRawFabricInfoDetails", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"ppo_No\":\"" + table.Rows[0]["PPO_No"].ToString() + "\"";
                    value += ",\"raw_GKNo\":\"" + table.Rows[0]["Raw_No"].ToString() + "\"";
                    value += ",\"gk_No\":\"" + table.Rows[0]["GK_No"].ToString() + "\"";
                    value += ",\"customer\":\"" + table.Rows[0]["Customer"].ToString() + "\"";
                    value += ",\"deliveryPlace\":\"" + table.Rows[0]["Destination"].ToString() + "\"";
                    value += ",\"stay_time\":\"" + table.Rows[0]["Stay_Time"].ToString() + "\"";

                    // 2015-1-24 liangyoh 添加判定防飞花间，则显示备注
                    if (table.Rows[0]["KN_Remark"].ToString().Contains("防飞花间"))
                    {
                        value += ",\"flyFlag\":\"Y\"";
                    }
                    else
                    {
                        value += ",\"flyFlag\":\"N\"";
                    }
                    //value = ",\"strictType\":\"" + table.Rows[0]["strictType"].ToString() + "\"";
                    value += ",\"kn_Remark\":\"" + table.Rows[0]["KN_Remark"].ToString().Replace("\"", "\\\"") + "\"";
                    value += ",\"order_quantity\":\"" + table.Rows[0]["Require_Qty"].ToString() + "\"";
                    value += ",\"finished_quantity\":\"" + table.Rows[0]["Produce_Qty"].ToString() + "\"";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 坯布挑修：完成挑修g

        /// </summary>
        /// <param name="userID">员工工号</param>
        /// <param name="fabric_No_List">卷号列表</param>
        /// <param name="remark_List">挑修备注列表</param>
        /// <returns></returns>
        [WebMethod(Description = "坯布挑修接口————完成挑修，更新挑修信息, userId:员工工号, vehicle_No:车号")]
        public string finishMendRawFabric(string userID, string vehicle_No, string fabric_No_List, string remark_List)
        {//Usp_FIFinishedMendRawFabric
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "更新完成挑修信息失败").Replace("@value", value);

            // 清除'号
            fabric_No_List = fabric_No_List.Replace("\'", "");
            remark_List = remark_List.Replace("\'", "");

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@userID", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = userID},
                    new SqlParameter("@vechile_No", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Input,  Value = vehicle_No},
                    new SqlParameter("@fabric_No_List", SqlDbType.VarChar, 400) { Direction = ParameterDirection.Input,  Value = fabric_No_List},
                    new SqlParameter("@remark_List", SqlDbType.VarChar, 8000) { Direction = ParameterDirection.Input,  Value = remark_List}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("Usp_FIFinishedMendRawFabric", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }

        }

        ///// <summary>
        ///// 坯布完成挑修：获取卷号信息
        ///// </summary>
        ///// <param name="fabric_No">卷号</param>
        ///// <returns></returns>
        //[WebMethod(Description = "坯布完成挑修：获取卷号信息")]
        //public string getMendRawFabricInfo(string fabric_No)
        //{
        //    // 默认返回值
        //    string value = ", \"fabric_No\":\"\",\11111111111111111"weight\":\"\",\"vehicle_No\":\"\",\"stay_time\":\"\"";       // 

        //    returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关坯布信息").Replace("@value", value);

        //    try
        //    {
        //        SqlParameter[] parameters = new SqlParameter[]{
        //            new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No}};

        //        DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMendRawFabricInfo", parameters);
        //        if (table != null && table.Rows.Count > 0)
        //        {
        //            value = ",\"fabric_No\":\"" + table.Rows[0]["fabric_No"].ToString() + "\"";
        //            value = ",\"weight\":\"" + table.Rows[0]["weight"].ToString() + "\"";
        //            value = ",\"vehicle_No\":\"" + table.Rows[0]["vehicle_No"].ToString() + "\"";
        //            value = ",\"stay_time\":\"" + table.Rows[0]["stay_time"].ToString() + "\"";
        //            returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
        //        }

        //        return returnResult;
        //    }
        //    catch
        //    {
        //        return returnResult;
        //    }
        //}

        /// <summary>
        /// 挑修WIP汇总
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "坯布挑修接口————挑修WIP汇总")]
        public string getMendRawFabricWIP()
        {
            // 默认返回值
            string value = ",\"totalWIP\":[{\"process_name\":\"\",\"weight\":\"\",\"roll_Count\":\"\"}]";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关WIP信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@process_name", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = DBNull.Value}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMendRawFabricWIP", null);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"totalWIP\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{";
                        value += "\"process_name\":\"" + dr["Process_Name"].ToString() + "\"";               // 
                        value += ", \"weight\":\"" + dr["Weight"].ToString() + "\"";                      // 
                        value += ", \"roll_Count\":\"" + dr["Roll_Count"].ToString() + "\"";                     // 
                        value += "},";
                    }
                    value = value.Remove(value.Length - 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 获取挑修的备注列表
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "坯布挑修接口————获取挑修的备注列表")]
        public string getMendRemarks()
        {
            // 默认返回值
            string value = ",\"remarks\":[{\"remark\":\"飞花\"},{\"remark\":\"飞纱\"},{\"remark\":\"结头\"}]";       // 

            //returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关WIP信息").Replace("@value", value);
            returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);

            return returnResult;
        }
        /// <summary>
        /// 挑修WIP汇总：显示具体某个挑修状态下的详细信息
        /// </summary>
        /// <param name="process_name"></param>
        /// <returns></returns>
        [WebMethod(Description = "坯布挑修接口————挑修WIP汇总：显示具体某个挑修状态下的详细信息")]
        public string getMendRawFabricWIPDetail(string process_name)
        {
            // 默认返回值
            string value = ",\"WIPDetail\":[{\"fabric_No\":\"\",\"weight\":\"\",\"vehicle_No\":\"\",\"stay_time\":\"\"}]";       // 
            //{ "fabric_No":"CDB176800600", "weight":"312", "vehicle_No":"K403","stay_time":"8"}
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关WIP信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@process_name", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Input,  Value = process_name}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMendRawFabricWIPDetail", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"WIPDetail\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{";
                        value += "\"fabric_No\":\"" + dr["Fabric_No"].ToString() + "\"";               // 
                        value += ", \"weight\":\"" + dr["Weight"].ToString() + "\"";                      // 
                        value += ", \"vehicle_No\":\"" + dr["Vehicle_No"].ToString() + "\"";               // 
                        value += ", \"stay_time\":\"" + dr["Stay_Time"].ToString() + "\"";                     // 
                        value += "},";
                    }
                    value = value.Remove(value.Length - 1);
                    value += "]";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        ///// <summary>
        ///// 取消坯布挑修：获取可取消的坯布信息
        ///// </summary>
        ///// <param name="fabric_No"></param>
        ///// <returns></returns>
        //[WebMethod(Description = "取消坯布挑修：获取可取消的坯布信息")]
        //public string getFinishMendRawFabric(string fabric_No)
        //{
        //    // 默认返回值
        //    string value = ", \"fabric_No\":\"\",\"weight\":\"\",\"vehicle_No\":\"\",\"stay_time\":\"\",\"finished_time\":\"\"";       // 

        //    returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关坯布信息").Replace("@value", value);

        //    try
        //    {
        //        SqlParameter[] parameters = new SqlParameter[]{
        //            new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No}};

        //        DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetMendRawFabricInfo", parameters);
        //        if (table != null && table.Rows.Count > 0)
        //        {
        //            value = ",\"fabric_No\":\"" + table.Rows[0]["fabric_No"].ToString() + "\"";
        //            value = ",\"weight\":\"" + table.Rows[0]["weight"].ToString() + "\"";
        //            value = ",\"vehicle_No\":\"" + table.Rows[0]["vehicle_No"].ToString() + "\"";
        //            value = ",\"stay_time\":\"" + table.Rows[0]["stay_time"].ToString() + "\"";
        //            value = ",\"finished_time\":\"" + table.Rows[0]["finished_time"].ToString() + "\"";
        //            returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
        //        }

        //        return returnResult;
        //    }
        //    catch
        //    {
        //        return returnResult;
        //    }
        //}

        /// <summary>
        /// 取消坯布挑修：取消坯布挑修退布
        /// </summary>
        /// <param name="fabric_No_List"></param>
        /// <param name="preStatus"></param>
        /// <returns></returns>
        [WebMethod(Description = "坯布挑修接口————取消坯布挑修：退布， preStatus:表示退挑修的状态，只能退回待接收和待挑修")]
        public string returnMendRawFabric(string userID, string fabric_No_List, string preStatus)
        {
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "取消坯布挑修失败").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@userID", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = userID},
                    new SqlParameter("@fabric_No_List", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Input,  Value = fabric_No_List},
                    new SqlParameter("@Status", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Input,  Value = preStatus}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("Usp_FIReturnMendRawFabric", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }


        /// <summary>
        /// 送布
        /// </summary>
        /// <param name="workerID">用户ID</param>
        /// <param name="sendStatus">送布状态</param>
        /// <param name="send_Date">送布时间</param>
        /// <param name="fabric_No">卷号</param>
        /// <returns></returns>
        [WebMethod(Description = "坯布挑修接口————送布")]
        public string sendMendRawFabric(string userID, string vehicle_No, string fabric_No_List)
        {
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "送布失败").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@userID", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = userID},
                    new SqlParameter("@vehicle_No", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Input,  Value = vehicle_No},
                    new SqlParameter("@fabric_No_List", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No_List}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("Usp_FISendMendRawFabric", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
            //return "{ \"result\":true/false, \"message\":\"\" }";
        }

        #endregion
    }
}

