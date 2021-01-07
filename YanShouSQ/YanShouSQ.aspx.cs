using NNFormFwk;
using NNFormFwk.BizData;
using NNFPage.Codes;
using Process验收申请单.V1.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WF.Interface.WSTask;
using System.Data.Linq;
using NNFPage.WebForm.AuthorityManagement;

namespace NNFPage.Processes.YanShouSQ
{
    public partial class YanShouSQ : NNFPageGlobalType<YanShouSQ.MyGlobal, YanShouSQ.Entity>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //获取节点信息
            if (CurrentTask.StepName == STEPS.基建验收组 || CurrentTask.StepName == STEPS.设备验收组)
            {
                app1.Opion = "1456";
                //Files1.AllowEdit = true;
                //Files1.MinFiles = 2;
                //Files1.MaxFileSizeMB = 50;
            }
            app1.Title = "审批意见--" + CurrentTask.StepName;
            // 获取表单字段信息 
            var CS_GLOBAL = CurrentGlobalType.CS_GLOBAL;
        }
        public class Entity : NNFPageGlobalTypeEntity3<MyGlobal>
        {
            /// <summary>
            /// 表单加载前，初始化表单数据
            /// </summary>
            /// <param name="isReadOnly">是否只读，判断任务是否已经提交</param>
            /// <param name="task">任务信息</param>
            /// <param name="global"></param>
            public override void BeforeFormLoad(bool isReadOnly, Task2 task, MyGlobal global)
            {
                //判断流程实例号是否存在，不存在说明表单是创建状态，默认带出用户信息
                if (!task.IsIncidentExists)
                {
                    SSO.Helper hp = new SSO.Helper();
                    var hr = new HR.Interface.W2HR.HRSVC();
                    var people = hr.GetPeopleByIdOrLoginName(hp.LoginName);
                    var com = new Codes.ProcessHelpers().getComMsgByLogin(people.LoginName);
                    //var people = hr.GetPeopleByIdOrLoginName("test007");
                    global.ApplicantID = people.ID;
                    global.ApplicantNo = people.LoginName;
                    global.Applicant = people.Name;
                    global.ApplyDate = DateTime.Now.ToString("yyyy/MM/dd");
                    global.ApplyCompanyID = string.IsNullOrEmpty(global.ApplyCompanyID) ? com.ID : global.ApplyCompanyID; //people.CompanyInfo == null ? people.DepartmentInfo.ID : people.CompanyInfo.ID;
                    global.ApplyCompany = string.IsNullOrEmpty(global.ApplyCompany) ? com.Name : global.ApplyCompany; //people.CompanyInfo == null ? people.DepartmentInfo.Name : people.CompanyInfo.Name;
                    global.ApplyDepartmentID = people.DepartmentInfo == null ? people.CompanyInfo.ID : people.DepartmentInfo.ID;
                    global.ApplyDepartment = people.DepartmentInfo == null ? people.CompanyInfo.Name : people.DepartmentInfo.Name;
                    using (var ctx = new DataContext(SSO.Utilities.CfgHelper.GetConnectionString("W2", true)))
                    {
                        var DBName = ctx.ExecuteQuery<string>(@"select OtherA from HROCDepartment where id={0}", global.ApplyCompanyID).FirstOrDefault();
                        global.DBName = string.IsNullOrEmpty(global.DBName) ? (DBName) : global.DBName;
                    }

                }
                else {
                    if (isReadOnly)
                    {
                        //using (var db = new DBEntity.JieMeiDEVDataContext())
                        //{
                        //    string sql = "select IsOld from Biz_FI_YanShouSQ where SYS_INCIDENT=" + task.Incident;
                        //    var r = db.ExecuteQuery<string>(sql).First();
                        //    if (null != r)
                        //    {
                        //        global.IsOld = r;
                        //    }
                            
                        //}
                    }
                }
                //手动将task'的值更新到业务库
                try
                {
                    if (isReadOnly)
                    {
                        ProcessData.Manual(task.TaskId);
                    }
                }
                catch (Exception e) { }
            }
            /// <summary>
            /// 表单保存前
            /// </summary>
            /// <param name="isDraftMode">判断是否为保存草稿</param>
            /// <param name="task"></param>
            /// <param name="global"></param>
            /// <param name="appResult">审批结果</param>
            /// <param name="afterSent"></param>
            public override void BeforeFormSave(bool isDraftMode, Task2 task, MyGlobal global, InternalFormApproveResult appResult, List<Action<NNFSentEventArgs>> afterSent)
            {
                #region 获取"采购负责人"审批节点的审批人（采购部 李智文） 2020/01/04 luxu

                List<HR.Interface.W2HR.People> users3 = AuthGeneralConfig.SearchAproveUser("验收申请单", 72, global.ApplyCompanyID, global.ApplyDepartmentID, "验收申请单采购负责人", "", "", 0, 0, 0);
                //审批人对象清单转换为审批人ID清单
                List<string> userIDs3 = users3.Select(a => a.ID).ToList();
                var listcg = new List<string>();
                foreach (var itemcg in userIDs3)
                {
                    HR.Interface.W2HR.HRSVC hr = new HR.Interface.W2HR.HRSVC();
                    var peoplecg = hr.GetPeopleByIdOrLoginName(itemcg.Trim());
                    listcg.Add(Codes.RecipientHelper.FormatUltimusUser(peoplecg.LoginName));
                }
                global.CaiGouPeoples = listcg.ToArray();

                #endregion
                
                #region 流程提交后调用
                afterSent.Add((NNFSentEventArgs arg) =>
                {
                    if (task.IsFirstStep)
                    {

                    }
                });
                #endregion
                #region 判断是否保存草稿
                if (!isDraftMode)
                {
                    //将审批结果复制到变量
                    if (appResult != null)
                    {
                        global.AppResult = appResult.Option.ToString();
                    }

                    HR.Interface.W2HR.HRSVC hr1 = new HR.Interface.W2HR.HRSVC();

                    #region 超过两万抄送审计部门 20201019胡丹
                    foreach (var d in global.HTMsgs)
                    {
                        var HTJine = Convert.ToDouble(d.ContractMoney);
                        if (HTJine >= 20000)
                         {
                            List<HR.Interface.W2HR.People> users2 = AuthGeneralConfig.SearchAproveUser("验收申请", 65, global.ApplyCompanyID, global.ApplyDepartmentID, "", "", "", 0, 0, 0);
                            //审批人对象清单转换为审批人ID清单
                            List<string> userIDs2 = users2.Select(a => a.ID).ToList();
                            var listSJ = new List<string>();
                            foreach (var itemSJ in userIDs2)
                            {
                                var people = hr1.GetPeopleByIdOrLoginName(itemSJ.Trim());
                                listSJ.Add(Codes.RecipientHelper.FormatUltimusUser(people.LoginName));
                            }
                            global.CopyToAuditor = listSJ.ToArray();
                         }  
                    }
                    #endregion
                    //获取人员信息（更具人员ID或登录名）
                    //HR.Interface.W2HR.HRSVC hr = new HR.Interface.W2HR.HRSVC();
                    //var p1 = hr.GetPeopleByIdOrLoginName(global.ApplicantID);
                    //if (p1.SupervisorInfo == null)
                    //{
                    //    throw new NNFormFwk.Exceptions.NNFMessageException(global.Applicant + "没有配置上级领导");
                    //}
                    //发起人
                    //var n2 = new Codes.TaskHelper().GetActiveTaskCount(task.ProcessName, task.Incident, STEPS.采购部部长);
                    //global.FlagSN = global.ContractSN;  //保存合同SN号

                    

                    global.IsOld = "0";

                    if (global.isTools == "1") {
                        string[] arr = global.SupNo.Split(',');
                        var list = new List<string>();
                        foreach (var item in arr)
                        {
                            list.Add(Codes.RecipientHelper.FormatUltimusUser(item));
                        }
                        global.SheBeiPeoples = list.ToArray();
                    }
                    else
                    {
                        //基建验收组
                        string[] arr = global.SupNo.Split(',');
                        var list = new List<string>();
                        foreach (var item in arr)
                        {
                            list.Add(Codes.RecipientHelper.FormatUltimusUser(item));
                        }
                        global.JiJianPeoples = list.ToArray();
                    }

                    if (task.StepName == "财务验收确认")
                    {
                        if (Convert.ToInt32(global.AppResult) == 1)
                        {
                            global.FlagSN = global.ContractSN;  //保存合同SN号
                        }
                    }

                    using (var db = new DBEntity.JieMeiDEVDataContext())
                    {
                        var userlist = db.ExecuteQuery<string>(@"SELECT [ApproveUserID] FROM [JieMeiDEV].[dbo].[CWYS_JM] where CompanyID={0}", global.ApplyCompanyID).ToList();
                        var list = new List<string>();
                        var hr = new HR.Interface.W2HR.HRSVC();
                        foreach (var user in userlist)
                        {
                            var peo = hr.GetPeopleByIdOrLoginName(user);
                            if (peo != null)
                            {
                                list.Add(Codes.RecipientHelper.FormatUltimusUser(peo.LoginName));
                            }
                        }
                        if (list.Count <= 0)
                        {
                            throw new NNFormFwk.Exceptions.NNFMessageException("没有为公司：" + global.ApplyCompany + "配置财务验收人");
                        }
                        else {
                            global.CWYS = list.ToArray();
                        }
                    }

                    if (task.StepName == STEPS.基建验收组)
                    {
                        int n2 = 0;
                        int i = 0;
                        using (var ctx = new System.Data.Linq.DataContext(SSO.Utilities.CfgHelper.GetConnectionString("UltimusDB", true)))
                        {
                            try
                            {
                                i = ctx.ExecuteQuery<int>("SELECT count(*) as cnt FROM [UltimusDB].[dbo].[TASKS] where PROCESSNAME={0} and INCIDENT={1} and STEPLABEL={2} and STATUS=3", task.ProcessName, task.Incident, STEPS.基建验收组).First();
                                n2 = ctx.ExecuteQuery<int>("SELECT count(*) as cnt FROM [UltimusDB].[dbo].[TASKS] where PROCESSNAME={0} and INCIDENT={1} and STEPLABEL={2}", task.ProcessName, task.Incident, STEPS.基建验收组).First();
                            }
                            catch { }
                        }
                        if (n2 == i + 1)
                        {

                            global.TaskNumber = "1";
                        }
                        else
                        {
                            global.TaskNumber = "0";
                        }
                    }

                    if (task.StepName == STEPS.设备验收组)
                    {
                        int n2 = 0;
                        int i = 0;
                        using (var ctx = new System.Data.Linq.DataContext(SSO.Utilities.CfgHelper.GetConnectionString("UltimusDB", true)))
                        {
                            try
                            {
                                i = ctx.ExecuteQuery<int>("SELECT count(*) as cnt FROM [UltimusDB].[dbo].[TASKS] where PROCESSNAME={0} and INCIDENT={1} and STEPLABEL={2} and STATUS=3", task.ProcessName, task.Incident, STEPS.设备验收组).First();
                                n2 = ctx.ExecuteQuery<int>("SELECT count(*) as cnt FROM [UltimusDB].[dbo].[TASKS] where PROCESSNAME={0} and INCIDENT={1} and STEPLABEL={2}", task.ProcessName, task.Incident, STEPS.设备验收组).First();
                            }
                            catch { }
                        }
                        if (n2 == i + 1)
                        {

                            global.TaskNumber = "1";
                        }
                        else
                        {
                            global.TaskNumber = "0";
                        }
                    }

                    global.ApplyPeople = string.Format("USER:org=_W2OC_ultimus,user={0}", "ultimus/" + global.ApplicantNo);
                }
                #endregion
                
            }
        }

        private static string[] 保存数据节点 = new[] { "all" };
        public class DBTEST : ProcessData<MyGlobal>
        {
            protected override string 给业务表起个名字()
            {
                return "FI_YanShouSQ";
            }
            protected override string[] 哪些变量不用存业务表()
            {
                return new[] { "", "" };
            }
            protected override When2Save 何时保存业务表()
            {
                return new When2Save() { IncidentCompleted = true, IncidentStart = true, StepsCompleted = 保存数据节点 };
            }
        }

        //使用这个必须在bpm stution中加入变量 CS_GLOBAL
        public class MyGlobal : GlobalType
        {
            public string SN { get; set; } //单号
            public string IsOld { get; set; } //判断是否是历史单据
            public string DBName { get; set; }//DB
            public string ApplyDate { get; set; }//申请时间           
            public string ApplicantID { get; set; }
            public string ApplicantNo { get; set; }
            public string Applicant { get; set; }//申请人
            public string ApplyCompanyID { get; set; }//所属公司
            public string ApplyCompany { get; set; }//所属公司
            public string ApplyDepartmentID { get; set; }//所属部门
            public string ApplyDepartment { get; set; }//所属部门
            public string UploadFiles { get; set; }//文件上次ID 
            public string isToolsID { get; set; }//记录值 
            public string ContractSN { get; set; }//关联合同申请
            public string FlagSN { get; set; }//标记
            public string SupID { get; set; }
            public string SupNo { get; set; }
            public string SupName { get; set; }//验收人名称
            #region
            //设备类参数
            public string Incident { get; set; }//SYS_INCIDENT
            public string ToolsContractName { get; set; }//合同名称
            public string ToolsContractNo { get; set; }//合同编号
            public string ToolsContractMoney { get; set; }//合同金额
            public string ToolsNo { get; set; }//出厂编号
            public string ToolsName { get; set; }//设备名称
            public string ToolsType { get; set; }// 型号规格
            public string ToolsPrice { get; set; }//价格
            public string FactoryName { get; set; }//生产厂家
            public string EntryDate { get; set; }//进场日期
            public string UsePlace { get; set; }//使用场所
            public string ToolsInfo { get; set; }//主要技术参数(技术要求)
            public string ToolSupID { get; set; }//
            public string ToolSupNo { get; set; }//
            public string ToolSupName { get; set; }//验收人员
            //基建类参数
            public string ProjectName { get; set; }//工程名称
            public string ProjectPlace { get; set; }//施工地点
            public string BuildCompany { get; set; }//施工单位
            public string YanShouDate { get; set; }//验收日期
            public string ContractName { get; set; }//合同名称
            public string ContractNo { get; set; }//合同编号
            public string ContractMoney { get; set; }//合同金额
            public string ProjectInfo { get; set; }//施工项目清单
            #endregion

            public WLMXDetail[] WLMXDetails { get; set; } //物料明细行
            public HTMsg[] HTMsgs { get; set; } //合同汇总信息明细行 
        }
        public class WLMXDetail
        {
            public string FID { get; set; }//采购申请明细ID
            public string UID { get; set; }//当前行号
            public string WLMX_MaterialName { get; set; }//物料名称
            public string WLMX_MaterialNo { get; set; }//物料编码
            public string WLMX_SpecificationsModels { get; set; }//规格型号
            public string WLMX_UnitName { get; set; }//单位编码
            public string WLMX_UnitNo { get; set; }//单位名称
            public string WLMX_TaxPrice { get; set; }//含税单价
            public string WLMX_Quantity { get; set; }//数量
            public string WLMX_Amount { get; set; }//金额
            public string SourceSN { get; set; }//
            public string SourceIncident { get; set; }//
        }

        public class HTMsg
        {
            public string ContractSN { get; set; }//合同单号
            public string ContractName { get; set; }//合同名称
            public string ContractNo { get; set; }//合同编号
            public string ContractMoney { get; set; }//合同金额

            //基建合同信息
            public string ProjectName { get; set; }//工程名称
            public string ProjectPlace { get; set; }//施工地点
            public string BuildCompany { get; set; }//施工单位
            public string YanShouDate { get; set; }//验收日期
            public string ProjectInfo { get; set; }//施工项目清单

            //设备合同信息
            public string ToolsNo { get; set; }//出厂编号
            public string FactoryName { get; set; }//生产厂家
            public string EntryDate { get; set; }//进场日期
            public string UsePlace { get; set; }//使用场所
            public string ToolsInfo { get; set; }//主要技术参数(技术要求)
        
        }

    }
}