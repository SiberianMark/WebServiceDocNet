using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using Esquel.GEK.QI.DataAccess;


namespace Esquel.GEK.QI.PickRepair
{
    /// <summary>
    /// QI坯布挑修项目APP接口汇总B版
    /// </summary>
    [WebService(Namespace = "http://www.esquel.com/GEKQIPickRepair")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class BaseService : System.Web.Services.WebService
    {
        /// <summary>
        /// 员工登录:用户登录("工卡条码或工卡卡号");
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>string格式{"result":true/false,"message":"","userRight","1,2,3,4"}</returns>
        [WebMethod]
        public String userLogin(string userID)
        {
            List<string> result = new List<string>();

            SqlParameter[] parameters = new SqlParameter[]{
                        new SqlParameter("@MachineNo", SqlDbType.Char, 8) { Direction = ParameterDirection.Input, Value = machineNo}};
            DataTable table = SqlHelper.ExecuteDataTableByStoredProcedure("dbo.Usp_QIGetTopNBatchList", parameters);
            foreach (DataRow row in table.Rows)
            {
                result.Add(row[0].ToString());
            }

            return "{\"result\":true/false,\"message\":\"\",\"userRight\",\"1,2,3,4\"}";
        }
	
        /// <summary>
        /// 获取员工信息("工卡条码或工卡卡号");
        /// </summary>
        /// <param name="userid"></param>
        /// <returns>员工信息</returns>
        [WebMethod]
        public string getUserInfo(string userid)
        {
            return "{ \"result\":true/false, \"message\",\"\", \"userName\", \"\"}";
        }
	
        /// <summary>
        /// 新增用户权限("工卡条码或工卡卡号","权限1,权限2,…");
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <returns>是否新增成功</returns>
        [WebMethod]
        public string addRight(string userid,string right)
        {
            return "{ \"result\":true/false, \"message\":\"\" }";
        }	
        /// <summary>
        /// 坯布接收：获取坯布信息
        /// </summary>
        /// <param name="fabric_No">坯布布号</param>
        /// <returns>品名,称重,卡号</returns>
        [WebMethod]
        public string getrtRawInfo(string fabric_No)
        {
            return "{ \"result\":true/false, \"message\":\"\",\"GK_No\":\"\" , \"Weight\":\"\" , \"Car_No\":\"\" }";
        }

        /// <summary>
        /// 更新坯布已接收状态
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="fabric_No">卷号</param>
        /// <param name="status">状态</param>
        /// <param name="receive_time">接收时间</param>
        /// <returns>更新接收状态是否成功</returns>
        [WebMethod]
        public string updateRawReceiveStatus(string userid, string fabric_No, string status, DateTime receive_time)
        {
            return "{ \"result\":true/false, \"message\":\"\" }";
        }


        /// <summary>
        /// 开始挑修：获取卷号信息
        /// </summary>
        /// <param name="fabric_No">卷号</param>
        /// <returns>品名, 称重, 卡号, 状态, 接收时间</returns>
        [WebMethod]
        public string getRepairRawInfo(string fabric_No)
        {
            return "{ \"result\":true/false, \"message\":\"\", \"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\",\"Receive_Time\":\"\"}";
        }

        /// <summary>
        /// 更新已开挑信息
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="fabric_No">卷号</param>
        /// <param name="status">状态</param>
        /// <param name="repair_Time">开挑时间</param>
        /// <returns>更新已开挑信息是否成功</returns>
        [WebMethod]
        public string updateRepairRawInfo(string userid, string fabric_No, string status, DateTime repair_Time)
        {
            return "{ \"result\":true/false, \"message\":\"\"}";
        }

        /// <summary>
        /// 待挑修WIP
        /// </summary>
        /// <param name="query">查询语句</param>
        /// <returns>WIP结果</returns>
        [WebMethod]
        public string getRepairingRawInfo(string query)
        {
            return "{ \"result\":true/false, \"message\":\"\",\"list-QryResult\":[{\"fabric_No\":\"\",\"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\",\"Receive_Time\":\"\"},{…},{…}]}";
        }

        /// <summary>
        /// 坯布完成挑修：获取卷号信息
        /// </summary>
        /// <param name="fabric_No">卷号</param>
        /// <returns></returns>
        [WebMethod]
        public string getFabricInfo(string fabric_No)
        {
            return "{\"result\":true/false, \"message\":\"\", \"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\",\"Receive_Time\":\"\"}";
        }

        /// <summary>
        /// 更新完成挑修
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="fabric_No">卷号</param>
        /// <param name="status"></param>
        /// <param name="repaired_Time"></param>
        /// <returns></returns>
        [WebMethod]
        public string updateRawRepairedStatus(string userid, string fabric_No, string status, DateTime repaired_Time)
        {
            return "{ \"result\":true/false, \"message\":\"\" }";
        }

        /// <summary>
        /// 已完成挑修WIP
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebMethod]
        public string getRepairedRawInfo(string query)
        {
            return "{ \"result\":true/false, \"message\":\"\",\"list-QtyResult\":[{\"fabric_No\":\"\",\"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status\":\"\",\"Receive_Time\":\"\"},{…},{…}]}";
        }


        /// <summary>
        /// 取消挑修：取消挑修状态类型
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string cancleRepairRawType()
        {
            return "{ \"result\":true/false, \"message\":\"\",\"Status_Code\":\"\",\"Status_Name\":\"\" }";
        }

       
        /// <summary>
        /// 取消坯布状态
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="fabric_No"></param>
        /// <param name="cancle_Time"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [WebMethod]
        public string cancleRawType(string userid, string fabric_No, DateTime cancle_Time, string status)
        {
            return "{ \"result\":true/false, \"message\":\"\" }";
        }

        /// <summary>
        /// 送布：获取备注类别
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string getRemarkType()
        {
            return "{ \"result\":true/false, \"message\":\"\",\"RemarkType_Code\":\"\",\"RemarkType_Name\":\"\"}";
        }

        /// <summary>
        /// 获取送布资料
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string getSendedFabricDetail()
        {
            return "{ \"result\":true/false, \"message\":\"\",\"list-QtyResult\":[{\"fabric_No\":\"\",\"GK_No\":\"\",\"Weight\":\"\",\"Car_No\":\"\",\"Status_Name\":\"\",\"Receive_Time\":\"\"},{…},{…}]}";
        }
	
        /// <summary>
        /// 送布
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="sendStatus">送布状态</param>
        /// <param name="send_Date">送布时间</param>
        /// <param name="fabric_No">卷号</param>
        /// <returns></returns>
        [WebMethod]
        public string sendFabric(string userid, string sendStatus, DateTime send_Date, string fabric_No)
        {
            return "{ \"result\":true/false, \"message\":\"\" }";
        }

        /// <summary>
        /// 获取花型图——添加返回花型图数据
        /// </summary>
        /// <returns></returns>
        public string getPattren()
        {
            return "";
        }

    }
}
