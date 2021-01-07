using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NNFormFwk;
using NNFormFwk.BizData;
using WF.Interface.WSTask;
using Process固定资产转移申请单.V1.Types;
using Microsoft.Office.Interop.Word;
using NNFormFwk.Exceptions;
using System.Web.Services.Discovery;
using System.Data.Linq;
using NNFPage.WebForm.AuthorityManagement;
using NNFPage.Codes;

namespace NNFPage.Processes.AssetsTransferSQ
{
    public partial class AssetsTransferSQ : NNFPageGlobalType<AssetsTransferSQ.MyGlobal, AssetsTransferSQ.Entity>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //获取节点信息
            if (CurrentTask.StepName == STEPS.Begin)
            {
                Files1.AllowEdit = true;
                //Files1.MinFiles = 1;

            }
            app1.Opion = "12456";
            var CS_GLOBAL = CurrentGlobalType.CS_GLOBAL;
        }
        public class Entity : NNFPageGlobalTypeEntity3<MyGlobal>
        {
            #region 资产归属公司
            public object Company
            {
                get
                {
                    using (var ctx = new DataContext(SSO.Utilities.CfgHelper.GetConnectionString("W2", true)))
                    {
                        var Companylist = ctx.ExecuteQuery<NNFPage.Codes.W2Helper.CompanyInfo>(@"select ID,Name,OtherA from HROCDepartment where id like 'C%' and ParentID is not null order by IntID").ToList();
                        return Companylist.Select(o => new { Text = o.Name, Value = o.ID, DBName = o.OtherA });
                    }
                }
            }
            #endregion

            /// <summary>
            /// 表单加载事件
            /// </summary>
            /// <param name="isReadOnly">是否只读，判断任务是否已经提交</param>
            /// <param name="task">任务信息</param>
            /// <param name="global"></param>
            public override void BeforeFormLoad(bool isReadOnly, Task2 task, MyGlobal global)
            {
                
           
                
            
                if (task.IsFirstStep && !task.IsIncidentExists)
                {

                    //自动带出申请人信息
                    SSO.Helper hp = new SSO.Helper();
                    global.ApplicantID = hp.UserID;
                    global.ApplicantName = hp.Name;
                    HR.Interface.W2HR.HRSVC hr = new HR.Interface.W2HR.HRSVC();
                    var people = hr.GetPeopleByIdOrLoginName(hp.UserID);
                    var com = new Codes.ProcessHelpers().getComMsgByLogin(people.LoginName);
                    global.DepartmentID = (people.DepartmentInfo ?? people.CompanyInfo).ID;
                    global.DepartmentName = (people.DepartmentInfo ?? people.CompanyInfo).Name;
                    global.ApplyDate = DateTime.Now.ToString("yyyy/MM/dd");
                    global.CompanyID = string.IsNullOrEmpty(global.CompanyID) ? com.ID : global.CompanyID; //people.CompanyInfo == null ? people.DepartmentInfo.ID : people.CompanyInfo.ID;
                    global.CompanyName = string.IsNullOrEmpty(global.CompanyName) ? com.Name : global.CompanyName; //people.CompanyInfo == null ? people.DepartmentInfo.Name : people.CompanyInfo.Name;
                    using (var ctx = new DataContext(SSO.Utilities.CfgHelper.GetConnectionString("W2", true)))
                    {
                        var DBName = ctx.ExecuteQuery<string>(@"select OtherA from HROCDepartment where id={0}", com.ID).FirstOrDefault();
                        global.DBName = string.IsNullOrEmpty(global.DBName) ? (DBName) : global.DBName;
                    }
                }
            }
            /// <summary>
            /// 表单提交事件
            /// </summary>
            /// <param name = "isDraftMode" > 判断是否为保存草稿 </ param >
            /// < param name="task"></param>
            /// <param name = "global" ></ param >
            /// < param name="appResult">审批结果</param>
            /// <param name = "afterSent" ></ param >
            public override void BeforeFormSave(bool isDraftMode, Task2 task, MyGlobal global, InternalFormApproveResult appResult, List<Action<NNFSentEventArgs>> afterSent)
            {
                #region 流程提交后调用
                afterSent.Add((NNFSentEventArgs arg) =>
                {
                });
                #endregion

                #region 判断是否为保存草稿 
                if (!isDraftMode && global.AppResult == "1")//注意这里的AppResult=="1"必须要有
                {
                    #region 保存审批变量
                    if (appResult != null)
                    {
                        global.AppResult = appResult.Option.ToString();
                    }
                    #endregion

                    var hr = new HR.Interface.W2HR.HRSVC();
                    var people = hr.GetPeopleByIdOrLoginName(global.ApplicantID);


                    if (task.IsFirstStep)
                    {


                        #region 删除中间表记录行 
                        using (var db = new DBEntity.JieMeiDEVDataContext())
                        {
                            var dl = db.DeptLevelMsg.Where(c => c.ProcessName == task.ProcessName && c.Incident == task.Incident).ToList();
                            if (dl.Count > 0)
                            {
                                db.DeptLevelMsg.DeleteAllOnSubmit(dl);
                                db.SubmitChanges();
                                
                            }
                        }
                        #endregion

                        #region 原部门审核
                        try
                        {
                            string error = string.Empty;
                            var ls = new Codes.ProcessHelpers().DeptLevelWhenEndByDept(task.ProcessName, task.Incident, global.ApplicantID,global.DepartmentIDOld, out error);
                            if (!string.IsNullOrEmpty(error))
                            {//表示方法调用错误、申请人信息获取错误或者相关部门未配置审批人
                                throw new NNFMessageException(error);
                            }

                            global.Approversold = string.Join(",", ls.ToArray());
                            global.BMSP_Old = Codes.RecipientHelper.FormatUltimusUser(ls[0]);
                        }
                        catch (Exception e)
                        {
                            throw new NNFMessageException(e.StackTrace);
                        }
                        #endregion

                        #region 拟转入部门审核
                        try
                        {
                            string error = string.Empty;
                            var ls = new Codes.ProcessHelpers().DeptLevelWhenEndByDept(task.ProcessName, task.Incident, global.ApplicantID,global.DepartmentIDNew, out error);
                            if (!string.IsNullOrEmpty(error))
                            {//表示方法调用错误、申请人信息获取错误或者相关部门未配置审批人
                                throw new NNFMessageException(error);
                            }

                            global.Approversnew = string.Join(",", ls.ToArray());
                            global.BMSP_New= Codes.RecipientHelper.FormatUltimusUser(ls[0]);
                        }
                        catch (Exception e)
                        {
                            throw new NNFMessageException(e.StackTrace);
                        }
                        #endregion



                        #region 从通用配置表获取财务部门资产管理人员审批
                        List<HR.Interface.W2HR.People> users3 = AuthGeneralConfig.SearchAproveUser("固定资产转移申请单", 70, global.CompanyID, global.DepartmentID, "资产管理", "", "", 0, 0, 0);
                        //审批人对象清单转换为审批人ID清单
                        List<string> userIDs3 = users3.Select(a => a.ID).ToList();
                        var listcw = new List<string>();
                        foreach (var itemcw in userIDs3)
                        {
                            var peoplecw = hr.GetPeopleByIdOrLoginName(itemcw.Trim());
                            listcw.Add(Codes.RecipientHelper.FormatUltimusUser(peoplecw.LoginName));
                        }
                        global.CW_Manager = listcw.ToArray();

                        #endregion


                        #region 根据资产类别获取资产管理人员
                        using (var cxt = new DBEntity.JieMeiDEVDataContext())
                        {
                            var sql = string.Format(@"select ApproveUserID from T_BD_PDZiChanZG where MaterialTypeName='{0}' and DBName='{1}'", global.AssetType,global.DBName);
                            var zczg = cxt.ExecuteQuery<string>(sql).ToList();
                            if (zczg == null)
                            {
                                throw new NNFMessageException(global.AssetType + ":没有配置资产管理人员");
                            }
                            var listzc = new List<string>();
                            foreach (var t in zczg)
                            {
                                string[] s = t.Split(',');
                                foreach (var k in s)
                                {
                                    var peo2 = hr.GetPeopleByIdOrLoginName(k);
                                    listzc.Add(Codes.RecipientHelper.FormatUltimusUser(peo2.LoginName));
                                }
                            }
                            global.AssetManager = listzc.ToArray();
                        }
                        #endregion

                        #region 是否股份公司判断
                        if (global.CompanyID == "CF737BE07DFA9F547") { global.IsGF = "1"; } else { global.IsGF = "0"; }
                        #endregion


                        #region 子公司总经理审批人
                        if (global.IsGF == "0")
                        {
                            var ZGSManager = hr.GetDepartmentManagersByDepartmentId(global.CompanyID);
                            if (ZGSManager == null) { throw new NNFMessageException(global.CompanyName + ":没有配置公司负责人"); }
                            var ZGSlist = new List<string>();
                            foreach (var A in ZGSManager)
                            {
                                ZGSlist.Add(Codes.RecipientHelper.FormatUltimusUser(A.LoginName));
                            }
                            global.SubsidiaryManager = ZGSlist.ToArray();
                        }
                        #endregion

                    }

                    if (task.StepName == STEPS.原部门)
                    {
                        string sp = "";
                        string error = "";
                        var isEnd = new Codes.ProcessHelpers().DeptLevelIsEnd(task.ProcessName, task.Incident, task.StepId, task.StepName, global.Approversold, out sp, out error);
                        if (!string.IsNullOrEmpty(error))
                        {//表示方法调用错误
                            throw new NNFMessageException(error);
                        }

                        if (isEnd)
                        {//表示部门审批结束。可以结束部门的循环审批
                            global.IsEnd_Old = "1";
                        }
                        else
                        {//表示部门循环未审批结束，任然需要经过下一层审批
                            global.IsEnd_Old = "0";
                            global.BMSP_Old = sp;//未下一层审批的人 赋值
                        }
                    }
                    

                    if (task.StepName == STEPS.拟转入部门)
                    {
                        string sp = "";
                        string error = "";
                        var isEnd = new Codes.ProcessHelpers().DeptLevelIsEnd(task.ProcessName, task.Incident, task.StepId, task.StepName, global.Approversnew, out sp, out error);
                        if (!string.IsNullOrEmpty(error))
                        {
                            throw new NNFMessageException(error);
                        }

                        if (isEnd)
                        {
                            global.IsEnd_New = "1";
                        }
                        else
                        {
                            global.IsEnd_New = "0";
                            global.BMSP_New = sp;
                        }
                    }

                }
                #endregion

            }
        }

        
        public class DBTSET : ProcessData<MyGlobal>
        {
            protected override string 给业务表起个名字()
            {
                return "Buss_AssetsTransferSQ";
                
            }
            private static string[] 保存数据节点 = new[] { "all" };
            protected override string[] 哪些变量不用存业务表()
            {
                return new[] { "", "" };
            }
            protected override When2Save 何时保存业务表()
            {
                return new When2Save() { IncidentCompleted = true, IncidentStart = true, StepsCompleted = 保存数据节点 };

            }

        }

        //须在bpm studio中加入变量 CS_GLOBAL
        public class MyGlobal : GlobalType
        {

            public string SN { get; set; }//单据编号
            public string DepartmentID { get; set; }//所属部门
            public string DepartmentName { get; set; }//所属部门
            public string ApplicantNo { get; set; }//申请人
            public string ApplicantID { get; set; }//申请人
            public string ApplicantName { get; set; }//申请人
            public string ApplyDate { get; set; }//制单日期
            public string CompanyName { get; set; }//公司名称
            public string CompanyID { get; set; }
            public string AssetLocationID { get; set; }//资产存放地点
            public string AssetLocationName { get; set; }
            public string AssetType { get; set; }//资产类别，获取资产管理人用
            public string DBName { get; set; }
            public string Approversold { get; set; }
            public string Approversnew { get; set; }
            public string AssetName { get; set; }//资产名称
            public string Model { get; set; }//规格型号
            public string DepartmentNameOld { get; set; }//原使用部门
            public string DepartmentIDOld { get; set; }//原使用部门
            public string DepartmentNameNew { get; set; }//拟转入部门
            public string DepartmentIDNew { get; set; }//拟转入部门
            public string TransferReasons { get; set; }//转出部门申请原因
            public string AssetNumber { get; set; }//资产编号
            public string BuyTime{ get; set; }//购置时间
            public string ZJNX { get; set; }//折旧年限
            public string BeforeValue { get; set; }//账面原值
            public string NowValue { get; set; }//账面净值
            public string Note { get; set; }//备注信息
            public string UploadFiles { get; set; }//文件上传
        }
        public class DBTEST
        {

        }

    }


}
