using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LYH.ClassLib.IO;
using Esquel.GEK.QI.DataAccess;
using System.Data;
using System.Data.SqlClient;

namespace Esquel.GEK.FI.PDA
{
    public partial class IPConfig : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ConfigHelper.initConfigFile("Web.config");

            if (!IsPostBack)
            {
                ImportIP();
            }
            
            SaveIP();
        }

        public void ImportIP()
        {
            /*
            string[] IP_List = new string[4];
            string[] IP_Keys = new string[] { "IP1", "IP2", "IP3", "IP4" };
            for (int i = 0; i < IP_List.Length; i++)
            {
                IP_List[i] = ConfigHelper.GetAppConfig(IP_Keys[i]);
            }
            */

            string[] IP_No = new string[4];
            string[] IP_List = new string[4];
            string selText = "SELECT PrinterNo, IP FROM SystemDB..pbPrinterList  WHERE Department='FI' AND PrinterSystem='组板PDA' ORDER BY PrinterNo ASC";
            DataTable dt = SqlHelper.RunQuery(selText);
            if (dt != null && dt.Rows.Count > 0)
            {
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    IP_No[i] = dr["PrinterNo"].ToString();
                    IP_List[i++] =dr["IP"].ToString();
                }
            }
            if (IP_List.Length >= 4)
            {
                No1.InnerText = IP_No[0];
                No2.InnerText = IP_No[1];
                No3.InnerText = IP_No[2];
                No4.InnerText = IP_No[3];

                IP1.Value = IP_List[0];
                IP2.Value = IP_List[1];
                IP3.Value = IP_List[2];
                IP4.Value = IP_List[3];
            }
        }

        public void SaveIP()
        {
            string[] IP_No = new string[4];
            string[] IP_List = new string[4];

            IP_No[0] = Request.Form["No1"];
            IP_No[1] = Request.Form["No2"];
            IP_No[2] = Request.Form["No3"];
            IP_No[3] = Request.Form["No4"];

            IP_List[0] = Request.Form["IP1"];
            IP_List[1] = Request.Form["IP2"];
            IP_List[2] = Request.Form["IP3"];
            IP_List[3] = Request.Form["IP4"];

            for (int i = 0; i < IP_List.Length; i++)
            {
                if ((IP_List[i] == null) || (IP_No[i]==null)) return;
            }

            string updateText = "";
            for (int i = 0; i < IP_List.Length; i++)
            {
                updateText += " UPDATE SystemDB..pbPrinterList SET IP='" + IP_List[i] + "' WHERE PrinterNo=" + IP_No[i] + " AND Department='FI' AND PrinterSystem='组板PDA'";
            }

            int updateFlag = SqlHelper.RunCommand(updateText);
            if (updateFlag > 0)
            {
                info.InnerText = "Successed.";
            }
            else
            {
                info.InnerText = "Failed.";
            }

        }
    }
}