using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Esquel.GEK.QI.DataAccess;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Esquel.GEK.FI.PDA
{
    /// <summary>
    /// QI成检项目APP接口
    /// </summary>
    [WebService(Namespace = "http://www.esquel.com/GEKFIPDA")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class BaseService : System.Web.Services.WebService
    {
        #region 成检PDA接口
        /// <summary>
        /// 接口返回字符串格式
        /// </summary>
        private static readonly string templateResult = "{\"result\":@result,\"message\":\"@message\"@value }";
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
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "").Replace("@value", "");

            /*
            initXmlHelper();
            string queryText = xmlHelperClass.GetData("search", "name", "checkUserAccount");
            SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@userID", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input, Value = userID}};
            DataTable table = SqlHelper.ExecuteDataTableByQuery(queryText, parameters);
             * */
            
            SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@userID", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input, Value = userID}};
            DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_ExistsSystemUser", parameters);
            
            try
            {
                if (table.Rows.Count > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", "");
                }
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "不存在当前员工，请输入正确员工号").Replace("@value", "");
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
        [WebMethod(Description = "扫描小标签获取信息")]
        public string getSmallLabelnfo(string barcode)
        {
            // 默认返回值为空
            string value = ",\"fabric_No\":\"\"";       // 卷号
            value += ", \"batch_No\":\"\"";     // 缸号
            value += ", \"weight\":\"\"";       // 重量
            value += ", \"processname\":\"\"";  // 工序

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "").Replace("@value", value);

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
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有当前条码的相关标签信息").Replace("@value", value);
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
        /// <returns>string格式{“result”:true/false,”message”:””,”位置”:[ {”location”:”A1”},{”location”:”A2”},{”location”:”A3”}] }</returns>
        [WebMethod(Description = "获取位置列表")]
        public string getLocationList()
        {
            // 默认返回值
            string value = ",\"locations\":\"[{\"location\":\"\"}]\"";  

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "").Replace("@value", value);

            try
            {
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetBanLocationList", null);

                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"locations\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"location\":\"" + dr["location"].ToString() + "\"},";          // 码长
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
        /// 获取大标签信息
        /// </summary>
        /// <param name="barcode">大标签条码</param>
        /// <returns></returns>
        [WebMethod(Description = "获取大标签信息")]
        public string getBigLabelInfo(string barcode)
        {//EXEC Usp_FIGetDefectPointMax :@Batch_No,:@Fabric_String

            // 默认返回值为空
            string value = ",\"fabric_No\":\"\"";         // 卷号
            value += ", \"ppo_No\":\"\"";             // PPO_No
            value += ", \"ppo_No\":\"\"";              // 用途 Usage
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
            value += ", \"Shade_Lot\":\"\"";               // 色级

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "该条码找不到对应的标签信息").Replace("@value", value);



            // 由缸号+布号查找大标签信息
            try
            {
                // 根据barcode布号查找出缸号
                SqlParameter[] parameters0 = new SqlParameter[]{
                        new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = barcode}};
                DataTable table0 = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetBatchNoByFabricNo", parameters0);
                if (table0 == null || table0.Rows.Count < 1)
                {
                    return returnResult;
                }
                string batch_No = table0.Rows[0]["Batch_No"].ToString();


                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Batch_No", SqlDbType.Char, 8) { Direction = ParameterDirection.Input,  Value = batch_No},
                        new SqlParameter("@Fabric_String", SqlDbType.VarChar, 6000) { Direction = ParameterDirection.Input,  Value = barcode}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetDefectPointMax", parameters);

                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"fabric_No\":\"" + table.Rows[0]["Fabric_No"].ToString() + "\"";         // 卷号
                    value += ", \"ppo_No\":\"" + table.Rows[0]["PPO_No"].ToString() + "\"";             // PPO_No
                    value += ", \"ppo_No\":\"" + table.Rows[0]["Usage"].ToString() + "\"";              // 用途 Usage
                    value += ", \"batch_No\":\"" + table.Rows[0]["Batch_No"].ToString() + "\"";         // 缸号
                    value += ", \"DnType\":\"" + table.Rows[0]["dn_type"].ToString() + "\"";            // 装单类型
                    value += ", \"gk_No\":\"" + table.Rows[0]["gk_no"].ToString() + "\"";               // 品名
                    value += ", \"combo_ID\":\"" + table.Rows[0]["Combo_ID"].ToString() + "\"";         // Combo_ID
                    value += ", \"combo\":\"" + table.Rows[0]["Combo"].ToString() + "\"";               // Combo
                    value += ", \"Seriation_No\":\"" + table.Rows[0]["Seriation_No"].ToString() + "\""; // 序号
                    value += ", \"grade\":\"" + table.Rows[0]["Grade"].ToString() + "\"";               // Grade等级
                    value += ", \"width\":\"" + table.Rows[0]["Width"].ToString() + "\"";               // Width幅宽
                    value += ", \"ozyd\":\"" + table.Rows[0]["OZYD"].ToString() + "\"";                 // OZYD克重
                    value += ", \"KG_allow_AF\":\"" + table.Rows[0]["aff_Weight"].ToString() + "\"";    // 
                    value += ", \"KG_allow\":\"" + table.Rows[0]["Allow_Weight"].ToString() + "\"";     // 
                    value += ", \"KG_foc\":\"" + table.Rows[0]["FOC_Weight"].ToString() + "\"";         // 
                    value += ", \"KG_allow_bf\":\"" + table.Rows[0]["Weight"].ToString() + "\"";        // 
                    value += ", \"YD_allow_AF\":\"" + table.Rows[0]["aff_Quantity"].ToString() + "\"";    // 
                    value += ", \"YD_allow\":\"" + table.Rows[0]["Allow_Quantity"].ToString() + "\"";     // 
                    value += ", \"YD_foc\":\"" + table.Rows[0]["FOC_Quantity"].ToString() + "\"";         // 
                    value += ", \"YD_allow_bf\":\"" + table.Rows[0]["Quantity"].ToString() + "\"";        // 
                    value += ", \"Deldate\":\"" + table.Rows[0]["Delivery_Date"].ToString() + "\"";     // 交付日期
                    value += ", \"Destination\":\"" + table.Rows[0]["Destination"].ToString() + "\"";   // 交付地
                    value += ", \"Defect_name\":\"" + table.Rows[0]["Defect_name"].ToString() + "\"";   // 疵点名称
                    value += ", \"defect_Point\":\"" + table.Rows[0]["defect_Point"].ToString() + "\"";   // 疵点分数
                    value += ", \"FBatch_no\":\"" + table.Rows[0]["FBatch_no"].ToString() + "\"";       // FBatch_no
                    value += ", \"data_colorde\":\"" + table.Rows[0]["data_colorde"].ToString() + "\"";       // data_colorde
                    value += ", \"customer\":\"" + table.Rows[0]["customer_code"].ToString() + "\"";       // customer
                    value += ", \"Print_Date\":\"" + DateTime.Now.ToString() + "\"";                      // 打印时间
                    value += ", \"Flag\":\"" + table.Rows[0]["Flag"].ToString() + "\"";                      // Flag
                    value += ", \"Shade_Lot\":\"" + table.Rows[0]["Shade"].ToString() + "\"";               // 色级

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
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@Fabric_No", SqlDbType.VarChar, 8000) { Direction = ParameterDirection.Input,  Value = fabric_No}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIBoardInfo", parameters);

                if (table != null && table.Rows.Count > 0)
                {
                    if (table.Rows[0]["Dn_Type"].ToString().ToUpper() == "E")
                    {
                        returnResult = templateResult.Replace("@result", "true").Replace("@message", "该卷为多余布").Replace("@value", "");
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
        /// 获取待组板信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [WebMethod(Description = "获取待组板信息")]
        public string getFIDNBanInfo(string barcode)
        {
            // 默认返回值为空
            string value = ",\"fabric_No\":\"\"";   // 卷号
            value += ", \"weight\":\"\"";           // 重量

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@fabric_No", SqlDbType.VarChar, 8000) { Direction = ParameterDirection.Input,  Value = barcode}};
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_FIGetDNBanInfoByFabricNo", parameters);

                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"fabric_No\":\"" + table.Rows[0]["fabric_no"].ToString() + "\"";   // 卷号
                    value += ", \"weight\":\"" + table.Rows[0]["Weight"].ToString() + "\"";       // 重量

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);

                }
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有对应的待组板信息").Replace("@value", value);
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
        /// <returns></returns>
        [WebMethod(Description = "WIP汇总查询")]
        public string getWIPSummary(int ppoType)
        {
            // 默认返回值为空
            string value = ",\"sumarry\":[{\"PrcSort\":\"\"";     // 部门
            value += ", \"ProcessName\":\"\"";      // 工序
            value += ", \"Roll_Count\":\"\"";       // 卷数
            value += ", \"Weight\":\"\"";           // 重量
            value += ", \"Quantity\":\"\"}]";         // 码长

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

                if (dtWIPTotal != null && dtWIPTotal.Rows.Count > 0)
                {
                    value = ",\"sumarry\":[";
                    foreach (DataRow dr in dtWIPTotal.Rows)
                    {
                        value += "{\"PrcSort\":\"" + dr["PrcSort"].ToString() + "\"";               // 部门
                        value += ", \"ProcessName\":\"" + dr["ProcessName"].ToString() + "\"";      // 工序
                        value += ", \"Roll_Count\":\"" + dr["Roll_Count"].ToString() + "\"";        // 卷数
                        value += ", \"Weight\":\"" + dr["Weight"].ToString() + "\"";                // 重量
                        value += ", \"Quantity\":\"" + dr["Quantity"].ToString() + "\"},";          // 码长
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
        /// 待组板WIP查询：以缸为单位显示汇总：缸号、重量、卷数、位置
        /// </summary>
        /// <param name="ppoType">订单类别：0.所有订单,1.大货订单，2.样板订单</param>
        /// <returns></returns>
        [WebMethod(Description = "待组板WIP查询")]
        public string getWIPDetailInDNBan(int ppoType)
        {
            // 默认返回值为空
            string value = ",\"BanWIPdetail\":[{\"batch_No\":\"\"";     // 缸号
            value += ", \"weight\":\"\"";           // 重量
            value += ", \"roll_Count\":\"\"";       // 卷数
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
                    value = ",\"BanWIPdetail\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"batch_No\":\"" + dr["Batch_No"].ToString() + "\"";     // 缸号
                        value += ", \"weight\":\"" + dr["weight"].ToString() + "\"";        // 重量
                        value += ", \"roll_Count\":\"" + dr["Roll_Count"].ToString() + "\"";       // 卷数
                        value += ", \"location\":\"" + dr["Location"].ToString() + "\"},";         // 位置
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
            string value = ",\"WIPdetail\":[{\"fabric_No\":\"\"";     // 卷号
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
                        value += "{\"fabric_No\":\"" + dr["Fabric_NO"].ToString() + "\"";           // 卷号
                        value += ", \"processname\":\"" + dr["Processname"].ToString() + "\"";      // 工序
                        value += ", \"Weight\":\"" + dr["Weight"].ToString() + "\"";                // 重量
                        value += ", \"location\":\"" + dr["Location"].ToString() + "\"},";          // 位置
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
        /// <param name="fabric_No">退布布号，若有多个布号则布号以,串接</param>
        /// <param name="barCode">条码,即退布单号</param>
        /// <param name="returnReason">退布原因</param>
        /// <returns></returns>
        [WebMethod(Description = "接收仓库退布，同步保存回KMIS")]
        public string FIFabricApplyFromST(string userName, string fabric_No, string barCode, string returnReason)
        {//
            returnResult = templateResult.Replace("@result", "false").Replace("@message", "").Replace("@value", "");
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@operator", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Input,  Value = userName},
                        new SqlParameter("@FabNostr", SqlDbType.VarChar, 4000) { Direction = ParameterDirection.Input,  Value = fabric_No},
                        new SqlParameter("@BackReason", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Input,  Value = returnReason},
                        new SqlParameter("@Flag", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Input,  Value = "RECEIVE"},
                        new SqlParameter("@Note_No", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output,  Value = barCode}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("USP_FIFabricApplyFromST", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    if (table.Rows[0]["Msg"].ToString() == "OK")
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
        #endregion


        #region 坯布挑修接口
        /// <summary>
        /// 员工登录:采用工号登录;
        /// </summary>
        /// <param name="workerID"></param>
        /// <returns>string格式{"result":true/false,"message":"","userRight","1,2,3,4"}</returns>
        [WebMethod(Description = "员工工号(条码)登录")]
        public String workerLogin(string workerID)
        {
            // 默认返回值为空
            string value = ",\"userRight\":\"\"";       // 用户权限

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该工人信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@workerID", SqlDbType.Char, 3) { Direction = ParameterDirection.Input,  Value = workerID}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIGetWorkerRight", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"userRight\":\""+table.Rows[0]["Right"].ToString()+"\"";
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
        /// 获取员工信息("工卡条码或工卡卡号");
        /// </summary>
        /// <param name="workerID"></param>
        /// <returns>员工信息</returns>
        [WebMethod(Description = "工号获取员工信息")]
        public string getUserInfo(string workerID)
        {
            // 默认返回值
            string value = ",\"userName\":\"\"";       // 用户权限

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该工人信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@workerID", SqlDbType.Char, 3) { Direction = ParameterDirection.Input,  Value = workerID}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIGetWorkerRight", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"userName\":\"" + table.Rows[0]["Name"].ToString() + "\"";
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
        /// 新增用户权限("工卡条码或工卡卡号","权限1,权限2,…");
        /// </summary>
        /// <param name="workerID">用户ID</param>
        /// <returns>是否新增成功</returns>
        [WebMethod(Description = "更新用户权限")]
        public string addRight(string workerID, string right)
        {
            // 默认返回值
            string value = "";       // 用户权限

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该工人信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@workerID", SqlDbType.Char, 3) { Direction = ParameterDirection.Input,  Value = workerID}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("usp_FIUpdateWorkerRight", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "更新工人权限失败").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }
        /// <summary>
        /// 坯布接收：获取待接收的坯布信息
        /// </summary>
        /// <param name="fabric_No">坯布布号</param>
        /// <returns>品名,称重,卡号</returns>
        [WebMethod(Description = "坯布接收：获取坯布信息")]
        public string getrtRawInfo(string fabric_No)
        {
            // 默认返回值
            string value = ",\"GK_No\":\"\" , \"Weight\":\"\" , \"Car_No\":\"\"" ;       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该布号信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIGetRTRawInfo", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"GK_No\":\"" + table.Rows[0]["GK_NO"].ToString() + "\"";
                    value = ",\"Weight\":\"" + table.Rows[0]["Weight"].ToString() + "\"";
                    value = ",\"Car_No\":\"" + table.Rows[0]["CarNo"].ToString() + "\"";
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
        /// 更新坯布已接收状态
        /// </summary>
        /// <param name="workerID">用户ID</param>
        /// <param name="fabric_No">卷号</param>
        /// <param name="status">状态</param>
        /// <param name="receive_time">接收时间</param>
        /// <returns>更新接收状态是否成功</returns>
        [WebMethod(Description = "更新坯布已接收状态")]
        public string updateRawReceiveStatus(string workerID, string fabric_No, string status, DateTime receive_time)
        {//usp_FIUpdateRawReceiveStatus
            // 默认返回值
            string value = "";       // 用户权限

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@workerID", SqlDbType.Char, 3) { Direction = ParameterDirection.Input,  Value = workerID},
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No},
                    new SqlParameter("@status", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = status},
                    new SqlParameter("@receive_time", SqlDbType.DateTime) { Direction = ParameterDirection.Input,  Value = receive_time}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("usp_FIUpdateRawReceiveStatus", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "更新坯布已接收状态失败").Replace("@value", value);
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
        /// <returns>品名, 称重, 卡号, 状态, 接收时间</returns>
        [WebMethod(Description = "开始挑修：获取卷号信息")]
        public string getRepairRawInfo(string fabric_No)
        {//usp_FIGetRepairRawInfo
            // 默认返回值
            string value = ",\"GK_No\":\"\" , \"Weight\":\"\" , \"Car_No\":\"\", \"Status\":\"\", \"Receive_Time\":\"\"";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关挑修信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIGetRepairRawInfo", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"GK_No\":\"" + table.Rows[0]["GK_NO"].ToString() + "\"";
                    value = ",\"Weight\":\"" + table.Rows[0]["Weight"].ToString() + "\"";
                    value = ",\"Car_No\":\"" + table.Rows[0]["CarNo"].ToString() + "\"";
                    value = ",\"Status\":\"" + table.Rows[0]["Weight"].ToString() + "\"";
                    value = ",\"Receive_Time\":\"" + table.Rows[0]["CarNo"].ToString() + "\"";
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
        /// 更新已开挑信息
        /// </summary>
        /// <param name="workerID">用户ID</param>
        /// <param name="fabric_No">卷号</param>
        /// <param name="status">状态</param>
        /// <param name="repair_Time">开挑时间</param>
        /// <returns>更新已开挑信息是否成功</returns>
        [WebMethod(Description = "更新已开挑信息")]
        public string updateRepairRawInfo(string workerID, string fabric_No, string status, DateTime repair_Time)
        {
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布挑修信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@workerID", SqlDbType.Char, 3) { Direction = ParameterDirection.Input,  Value = workerID},
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = workerID},
                    new SqlParameter("@status", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = workerID},
                    new SqlParameter("@repaire_time", SqlDbType.DateTime) { Direction = ParameterDirection.Input,  Value = workerID}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("usp_FIUpdateRawRepaireStatus", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "更新开挑信息失败").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 待挑修WIP
        /// </summary>
        /// <param name="query">查询语句</param>
        /// <returns>WIP结果</returns>
        [WebMethod(Description = "待挑修WIP")]
        public string getRepairingRawInfo(string query)
        {
            // 默认返回值
            string value = ",\"list-QryResult\":[{\"fabric_No\":\"\",\"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\",\"Receive_Time\":\"\"}]";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关WIP信息").Replace("@value", value);

            try
            {
                DataTable table = SqlHelper.ExecuteDataTableByQuery(query, null);


                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"list-QryResult\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"fabric_No\":\"" + dr["fabric_No"].ToString() + "\"";               // 部门
                        value += ", \"GK_No\":\"" + dr["GK_No"].ToString() + "\"";                      // 工序
                        value += ", \"Weight\":\"" + dr["Weight"].ToString() + "\"";                    // 卷数
                        value += ", \"Car_No\":\"" + dr["Car_No"].ToString() + "\"";                    // 重量
                        value += ", \"Status\":\"" + dr["Status"].ToString() + "\"";                    // 重量
                        value += ", \"Receive_Time\":\"" + dr["Receive_Time"].ToString() + "\"},";      // 码长
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
            //return "{ \"result\":true/false, \"message\":\"\",\"list-QryResult\":[{\"fabric_No\":\"\",\"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\",\"Receive_Time\":\"\"},{…},{…}]}";
        }

        /// <summary>
        /// 坯布完成挑修：获取卷号信息
        /// </summary>
        /// <param name="fabric_No">卷号</param>
        /// <returns></returns>
        [WebMethod(Description = "坯布完成挑修：获取卷号信息")]
        public string getFabricInfo(string fabric_No)
        {
            // 默认返回值
            string value = ", \"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\"";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关坯布信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIGetFinishedRepairRawInfo", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"GK_No\":\"" + table.Rows[0]["GK_NO"].ToString() + "\"";
                    value = ",\"Weight\":\"" + table.Rows[0]["Weight"].ToString() + "\"";
                    value = ",\"Car_No\":\"" + table.Rows[0]["CarNo"].ToString() + "\"";
                    value = ",\"Status\":\"" + table.Rows[0]["Weight"].ToString() + "\"";
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
            //return "{\"result\":true/false, \"message\":\"\", \"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\",\"Receive_Time\":\"\"}";
        }

        /// <summary>
        /// 更新完成挑修
        /// </summary>
        /// <param name="workerID">用户ID</param>
        /// <param name="fabric_No">卷号</param>
        /// <param name="status"></param>
        /// <param name="repaired_Time"></param>
        /// <returns></returns>
        [WebMethod(Description = "更新完成挑修")]
        public string updateRawRepairedStatus(string workerID, string fabric_No, string status, DateTime repaired_Time)
        {
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布挑修信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@workerID", SqlDbType.Char, 3) { Direction = ParameterDirection.Input,  Value = workerID},
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No},
                    new SqlParameter("@status", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = status},
                    new SqlParameter("@repaire_time", SqlDbType.DateTime) { Direction = ParameterDirection.Input,  Value = repaired_Time}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("usp_FIUpdateFinishedRawRepaireStatus", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "更新完成挑修信息失败").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
        }

        /// <summary>
        /// 已完成挑修WIP
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebMethod(Description = "已完成挑修WIP")]
        public string getRepairedRawInfo(string query)
        {
            // 默认返回值
            string value = ",\"list-QtyResult\":[{\"fabric_No\":\"\",\"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\",\"Receive_Time\":\"\"}]";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关WIP信息").Replace("@value", value);

            try
            {
                DataTable table = SqlHelper.ExecuteDataTableByQuery(query, null);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"list-QryResult\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"fabric_No\":\"" + dr["fabric_No"].ToString() + "\"";               // 部门
                        value += ", \"GK_No\":\"" + dr["GK_No"].ToString() + "\"";                      // 工序
                        value += ", \"Weight\":\"" + dr["Weight"].ToString() + "\"";                    // 卷数
                        value += ", \"Car_No\":\"" + dr["Car_No"].ToString() + "\"";                    // 重量
                        value += ", \"Status\":\"" + dr["Status"].ToString() + "\"";                    // 重量
                        value += ", \"Receive_Time\":\"" + dr["Receive_Time"].ToString() + "\"},";      // 码长
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
            //return "{ \"result\":true/false, \"message\":\"\",\"list-QtyResult\":[{\"fabric_No\":\"\",\"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\",\"Receive_Time\":\"\"},{…},{…}]}";
        }


        /// <summary>
        /// 取消挑修：取消挑修状态类型
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "取消挑修：取消挑修状态类型")]
        public string cancleRepairRawType()
        {
            // 默认返回值
            string value = ",\"Status_Code\":\"\",\"Status_Name\":\"\"";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "").Replace("@value", value);

            try
            {
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIGetCancleRepairRawType", null);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"Status_Code\":\"" + table.Rows[0]["Status_Code"].ToString() + "\"";
                    value += ",\"Status_Name\":\"" + table.Rows[0]["Status_Name"].ToString() + "\"";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "没有相关挑修数据可取消").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
            //return "{ \"result\":true/false, \"message\":\"\",\"Status_Code\":\"\",\"Status_Name\":\"\" }";
        }


        /// <summary>
        /// 取消坯布状态
        /// </summary>
        /// <param name="workerID"></param>
        /// <param name="fabric_No"></param>
        /// <param name="cancle_Time"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [WebMethod(Description = "取消坯布状态")]
        public string cancleRawType(string workerID, string fabric_No, DateTime cancle_Time, string status)
        {
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@workerID", SqlDbType.Char, 3) { Direction = ParameterDirection.Input,  Value = workerID},
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No},
                    new SqlParameter("@cancle_time", SqlDbType.DateTime) { Direction = ParameterDirection.Input,  Value = cancle_Time},
                    new SqlParameter("@status", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = status}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("usp_FIUpdateCancleRawType", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "取消坯布状态失败").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
            //return "{ \"result\":true/false, \"message\":\"\" }";
        }

        /// <summary>
        /// 送布：获取备注类别
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "送布：获取备注类别")]
        public string getRemarkType()
        {
            // 默认返回值
            string value = ",\"RemarkType_Code\":\"\",\"RemarkType_Name\":\"\"";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关送布信息").Replace("@value", value);

            try
            {
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIGetRemarkType", null);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"RemarkType_Code\":\"" + table.Rows[0]["RemarkType_Code"].ToString() + "\"";
                    value = ",\"RemarkType_Name\":\"" + table.Rows[0]["RemarkType_Name"].ToString() + "\"";

                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
            //return "{ \"result\":true/false, \"message\":\"\",\"RemarkType_Code\":\"\",\"RemarkType_Name\":\"\"}";
        }

        /// <summary>
        /// 获取送布资料
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "获取送布资料")]
        public string getSendedFabricDetail()
        {
            // 默认返回值
            string value = ",\"list-QtyResult\":[{\"fabric_No\":\"\",\"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status_Name\":\"\",\"Receive_Time\":\"\"}]";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关WIP信息").Replace("@value", value);

            try
            {
                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("usp_FIGetSendedFabricDetail", null);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"list-QryResult\":[";
                    foreach (DataRow dr in table.Rows)
                    {
                        value += "{\"fabric_No\":\"" + dr["fabric_No"].ToString() + "\"";               // 
                        value += ", \"GK_No\":\"" + dr["GK_No"].ToString() + "\"";                      // 
                        value += ", \"Weight\":\"" + dr["Weight"].ToString() + "\"";                    //
                        value += ", \"Car_No\":\"" + dr["Car_No"].ToString() + "\"";                    // 
                        value += ", \"Status_Name\":\"" + dr["Status_Name"].ToString() + "\"";          // 
                        value += ", \"Receive_Time\":\"" + dr["Receive_Time"].ToString() + "\"},";      // 
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
            //return "{ \"result\":true/false, \"message\":\"\",\"list-QtyResult\":[{\"fabric_No\":\"\",\"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status_Name\":\"\",\"Receive_Time\":\"\"},{…},{…}]}";
        }

        /// <summary>
        /// 送布
        /// </summary>
        /// <param name="workerID">用户ID</param>
        /// <param name="sendStatus">送布状态</param>
        /// <param name="send_Date">送布时间</param>
        /// <param name="fabric_No">卷号</param>
        /// <returns></returns>
        [WebMethod(Description = "送布")]
        public string sendFabric(string workerID, string sendStatus, DateTime send_Date, string fabric_No)
        {
            // 默认返回值
            string value = "";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该卷号送布信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@workerID", SqlDbType.Char, 3) { Direction = ParameterDirection.Input,  Value = workerID},
                    new SqlParameter("@fabric_No", SqlDbType.Char, 12) { Direction = ParameterDirection.Input,  Value = fabric_No},
                    new SqlParameter("@sendStatus", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Input,  Value = sendStatus},
                    new SqlParameter("@send_Date", SqlDbType.DateTime) { Direction = ParameterDirection.Input,  Value = send_Date}};

                int updateResult = SqlHelper.ExecNonQueryByStroedProcedure("usp_FISendFabric", parameters);
                if (updateResult > 0)
                {
                    returnResult = templateResult.Replace("@result", "true").Replace("@message", "").Replace("@value", value);
                }
                else
                {
                    returnResult = templateResult.Replace("@result", "false").Replace("@message", "送布失败").Replace("@value", value);
                }

                return returnResult;
            }
            catch
            {
                return returnResult;
            }
            //return "{ \"result\":true/false, \"message\":\"\" }";
        }

        /// <summary>
        /// 获取花型图——添加返回花型图数据
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "获取花型图")]
        public string getFabricPattren(string batch_No)
        {//Height, Color_Abbr, Color 
            // 默认返回值
            string value = ",\"Height\":\"\",\"Color_Abbr\":\"\",\"Color\":\"\"";       // 

            returnResult = templateResult.Replace("@result", "false").Replace("@message", "当前没有该坯布的相关花型图信息").Replace("@value", value);

            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@batch_No", SqlDbType.Char, 8) { Direction = ParameterDirection.Input,  Value = batch_No},
                    new SqlParameter("@InspectType", SqlDbType.Char, 2) { Direction = ParameterDirection.Input,  Value = "FI"}};

                DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("Usp_QIGetStripColor", parameters);
                if (table != null && table.Rows.Count > 0)
                {
                    value = ",\"Height\":\"" + table.Rows[0]["Height"].ToString() + "\"";
                    value = ",\"Color_Abbr\":\"" + table.Rows[0]["Color_Abbr"].ToString() + "\"";
                    value = ",\"Color\":\"" + table.Rows[0]["Color"].ToString() + "\"";

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
    }
}

