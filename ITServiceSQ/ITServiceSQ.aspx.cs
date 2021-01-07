using NNFormFwk;
using NNFormFwk.BizData;
using NNFormFwk.Exceptions;
using NNFPage.Codes;
using ProcessIT服务申请单.V1.Types;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WF.Interface.WSTask;

namespace NNFPage.Processes.ITServiceSQ
{
    public partial class ITServiceSQ : NNFPageGlobalType<ITServiceSQ.MyGlobal, ITServiceSQ.Entity>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //获取节点信息
            if (1 == 1)   //所有审批节点皆可上传附件
            {
                Files1.AllowEdit = true;
                //Files1.MinFiles = 1;
                //Files1.MaxFileSizeMB = 50;
            }

            if (CurrentTask.StepName == STEPS.系统负责人|| CurrentTask.StepName == STEPS.部门审批||CurrentTask.StepName == STEPS.信息部审核)
            {
                app1.Opion = "12456";
            }            
            if (CurrentTask.StepName == STEPS.解决方案审核 || CurrentTask.StepName == STEPS.解决审核)
            {
                app1.Opion = "12456";
                app1.OptionDisagreeItems = "退回上一步";
            }
            if (CurrentTask.StepName == STEPS.解决方案设计 || CurrentTask.StepName == STEPS.问题处理)
            {
                app1.Opion = "12456";
                app1.OptionDisagreeItems = "退回系统负责人";
            }
            if(CurrentTask.StepName == STEPS.具体处理)
            {
                app1.Opion = "1256";
            }
            if (CurrentTask.StepName == STEPS.申请人复核)
            {
                app1.Opion = "1456";
                app1.OptionDisagreeItems = "退回系统负责人";
            }
            app1.Title = "审批意见--" + CurrentTask.StepName;
            // 获取表单字段信息 
            var CS_GLOBAL = CurrentGlobalType.CS_GLOBAL;

        }

        public class Entity : NNFPageGlobalTypeEntity3<MyGlobal>
        {
            #region 系统类别
            public object SystemClass
            {
                get
                {
                    using (var ctx = new DBEntity.JieMeiDEVDataContext())
                    {
                        var sql = string.Format(@"select SystemName from  ID_SystemFZR ");
                        var result = ctx.ExecuteQuery<SystemType>(sql).ToList();
                        return result.Select(o => new { Text = o.SystemName, Value = o.SystemName });
                    }
                }
            }
            #endregion
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
                    global.ApplicantID = people.ID;
                    global.ApplicantNo = people.LoginName;
                    global.Applicant = people.Name;
                    global.ApplyDate = DateTime.Now.ToString("yyyy/MM/dd");
                    var com = new Codes.ProcessHelpers().getComMsgByLogin(people.LoginName);
                    global.ApplyCompanyID =com.ID;
                    global.ApplyCompany =com.Name ;
                    //global.ApplyCompanyID = people.CompanyInfo == null ? people.DepartmentInfo.ID : people.CompanyInfo.ID;
                    //global.ApplyCompany = string.IsNullOrEmpty(global.ApplyCompany) ? people.CompanyInfo.Name : global.ApplyCompany;
                    global.ApplyDepartmentID = people.DepartmentInfo == null ? (people.CompanyInfo.ID) : people.DepartmentInfo.ID;
                    global.ApplyDepartment = people.DepartmentInfo == null ? people.CompanyInfo.Name : people.DepartmentInfo.Name;
                    global.Status = global.Status == null ? "问题反馈中" : global.Status;

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

                    if (global.AppResult == "2")
                    {
                        if (!string.IsNullOrEmpty(global.IsAuto) && global.IsAuto == "1")
                        {
                            global.IsAuto = "0";
                            global.IsAutoAgain = "1";
                        }
                    }

                    //获取人员信息（更具人员ID或登录名）
                    HR.Interface.W2HR.HRSVC hr = new HR.Interface.W2HR.HRSVC();
                    var p1 = hr.GetPeopleByIdOrLoginName(global.ApplicantID);

                    #region 部门审批 2020/04/26 调整为直属上级一二负责人审批 tukebao
                    if(global.AppResult == "1")
                    {                       
                        if (task.IsFirstStep)
                        {
                            try
                            {
                                string error = string.Empty;
                                var ls = new Codes.ProcessHelpers().getZSSJ(global.ApplicantID);
                                if (!string.IsNullOrEmpty(error))
                                {
                                    throw new NNFMessageException(error);
                                }

                                if (ls.Count == 0)
                                {
                                    global.IsPassDept = "1";
                                }
                                else
                                {
                                    global.IsPassDept = "0";
                                    global.Approvers = string.Join(",", ls.ToArray());
                                    global.BMSP = Codes.RecipientHelper.FormatUltimusUser(ls[0]);
                                }
                            }
                            catch (Exception e)
                            {
                                throw new NNFMessageException(e.StackTrace);
                            }
                        }

                        if (task.StepName == STEPS.部门审批)
                        {
                            string sp = "";
                            string error = "";
                            var isEnd = new Codes.ProcessHelpers().DeptLevelIsEnd(task.ProcessName, task.Incident, task.StepId, task.StepName, global.Approvers, out sp, out error);
                            if (!string.IsNullOrEmpty(error))
                            {
                                throw new NNFMessageException(error);
                            }

                            if (isEnd)
                            {
                                global.IsEnd = "1";
                            }
                            else
                            {
                                global.IsEnd = "0";
                                global.BMSP = sp;
                            }
                        }

                        #region 废弃 2020/04/26 tukebao
                        //if (task.IsFirstStep)
                        //{
                        //    if (global.FeedbackType == "流程调整")
                        //    {
                        //        try
                        //        {
                        //            global.FBType = "1";
                        //            string error = string.Empty;
                        //            var ls = new Codes.ProcessHelpers().DeptLevelWhenEnd(task.ProcessName, task.Incident, global.ApplicantID, out error);
                        //            if (!string.IsNullOrEmpty(error))
                        //            {
                        //                throw new NNFMessageException(error);
                        //            }

                        //            if (ls.Count == 0)
                        //            {
                        //                global.IsPassDept = "1";
                        //            }
                        //            else
                        //            {
                        //                global.IsPassDept = "0";
                        //                global.Approvers = string.Join(",", ls.ToArray());
                        //                global.BMSP = Codes.RecipientHelper.FormatUltimusUser(ls[0]);
                        //            }
                        //        }
                        //        catch (Exception e)
                        //        {
                        //            throw new NNFMessageException(e.StackTrace);
                        //        }



                        //    }
                        //    else
                        //    {
                        //        global.FBType = "0";
                        //        global.IsPassDept = "1";
                        //    }
                        //}
                        #endregion
                    }

                    #endregion

                    #region 获取系统负责人
                    if (task.IsFirstStep && global.AppResult == "1")
                    {
                        using (var db = new DBEntity.JieMeiDEVDataContext())
                        {
                            var sql = string.Format(@"select UserID from ID_SystemFZR where SysID='{0}'", global.SysID);
                            var result = db.ExecuteQuery<string>(sql).ToList();
                            if (result != null && result.Count != 0)
                            {
                                var list = new List<string>();
                                foreach (var l in result)
                                {
                                    var p = hr.GetPeopleByIdOrLoginName(l);
                                    list.Add(Codes.RecipientHelper.FormatUltimusUser(p.LoginName));
                                }
                                global.SystemFZR = list.ToArray();
                            }
                            else
                            {
                                throw new NNFormFwk.Exceptions.NNFMessageException(global.SystemType + ": 没有配置系统负责人");
                            }

                        }
                    }

                    #endregion

                    #region 是否运维服务
                    if (global.SystemType == "运维服务")
                    {
                        global.IsYWFW = "1";
                    }
                    else
                    {
                        global.IsYWFW = "0";
                    }
                    #endregion

                    #region 判断是否选择了开发协助人员
                    if (!string.IsNullOrEmpty(global.Developer))
                    {
                        global.IsKaiFa = "1";
                    }
                    else
                    {
                        global.IsKaiFa = "0";
                    }
                    #endregion

                    #region 查找信息部审批人员(DE6731279F3BC6442) 2020/04/30 新增 tukebao
                    if (task.StepName==STEPS.系统负责人 && global.AppResult == "1")
                    {
                        var NowDeptID = "DE6731279F3BC6442";

                        //获取当前部门的 第一负责人
                        var dm = hr.GetDepartmentManagersByDepartmentId(NowDeptID);

                        //获取当前部门的 第二负责人
                        //var dfm = hr.GetDepartmentFenGuanByDepartmentId(NowDeptID);
                        var dfm = Codes.ProcessHelpers.NewGetDeptInfo(NowDeptID);

                        var ls = new List<string>();

                        if (dm.Count() == 0)
                        {
                            throw new NNFMessageException("信息管理部未配置部门负责人");
                        }
                        else
                        {                                                       

                            if (dfm.Count() != 0)
                            {
                                foreach(var l2 in dfm)
                                {
                                    ls.Add(l2.LoginName);
                                }
                            }
                            foreach (var l in dm)
                            {
                                ls.Add(l.LoginName);
                            }
                        }
                        global.Approvers2 = string.Join(",", ls.ToArray());
                        global.BMSP2 = Codes.RecipientHelper.FormatUltimusUser(ls[0]);
                    }

                    if (task.StepName == STEPS.信息部审核)
                    {
                        string sp = "";
                        string error = "";
                        var isEnd = new Codes.ProcessHelpers().DeptLevelIsEnd(task.ProcessName, task.Incident, task.StepId, task.StepName, global.Approvers2, out sp, out error);
                        if (!string.IsNullOrEmpty(error))
                        {
                            throw new NNFMessageException(error);
                        }

                        if (isEnd)
                        {
                            global.IsEnd2 = "1";
                        }
                        else
                        {
                            global.IsEnd2 = "0";
                            global.BMSP2 = sp;
                        }
                    }
                    

                    #endregion

                    #region 处理方式
                    if (global.HandleType == "直接回复")
                    {
                        global.DealType = "1";
                    }
                    else
                    {
                        global.DealType = "0";
                    }
                    #endregion

                    #region 获取后续对接人 解决方案设计步骤
                    if (global.HandleType == "后续处理" && global.Handover != "")
                    {
                        string[] arr = global.HandoverNo.Split(',');
                        var list = new List<string>();
                        foreach (var item in arr)
                        {
                            list.Add(Codes.RecipientHelper.FormatUltimusUser(item));
                        }
                        global.SystemWHZY = list.ToArray();
                        global.Handler = list.ToArray();
                        // 获取具体处理人，有开发协助人时，取开发人员，否则原对接人 20201022 hudan 
                        if (global.Developer != null && global.Developer != "") {
                            string[] arr1 = global.DeveloperNo.Split(',');
                            var list1 = new List<string>();
                            foreach (var item1 in arr1)
                            {
                                list1.Add(Codes.RecipientHelper.FormatUltimusUser(item1));
                            }
                            global.Handler = list1.ToArray();
                        }
                      
                    }
                    #endregion

                  

                    #region 申请人复核
                    global.ApplyCheck = Codes.RecipientHelper.FormatUltimusUser(global.ApplicantNo);
                    if (task.StepName == "申请人复核")
                    {
                         if (string.IsNullOrEmpty(global.score))
                          {
                             throw new NNFMessageException("请对处理结果进行评价");
                          }
                         if (Convert.ToInt32(global.score) <= 3 && global.Appraise=="")
                        {
                            throw new NNFMessageException("您对此次服务不满意吗（低于4星）？请在改善建议中写下宝贵意见，我们后续改进！");

                        }
                          
                    }
                        #endregion

                    #region 任务状态
                    if (task.StepName == STEPS.部门审批 && global.AppResult == "1" && global.IsEnd == "1")
                    {
                        global.Status = "问题分类中";
                    }                   
                    
                    if (task.StepName == STEPS.系统负责人 && global.AppResult == "1" && global.IsYWFW=="1")
                    {
                        global.Status = "问题处理中";
                    }
                    if (task.StepName == STEPS.系统负责人 && global.AppResult == "1" && global.IsYWFW == "0")
                    {
                        global.Status = "等待处理中";
                    }

                    if (task.StepName == STEPS.信息部审核 && global.AppResult == "1" && global.IsEnd2 == "1" && global.HandleType == "直接回复")
                    {
                        global.Status = "验收中";
                    }
                    if (task.StepName == STEPS.信息部审核 && global.AppResult == "1" && global.IsEnd2 == "1" && global.HandleType=="后续处理")
                    {
                        global.Status = "问题处理中";
                    }
                    
                    if ((task.StepName == STEPS.解决方案设计 || task.StepName == STEPS.解决方案审核 || task.StepName==STEPS.问题处理) && global.AppResult == "1")
                    {
                        global.Status = "问题处理中";
                    }

                    if ((task.StepName==STEPS.解决审核 || task.StepName == STEPS.具体处理) && global.AppResult == "1")
                    {
                        global.Status = "验收中";
                    }
                    if (task.StepName == STEPS.申请人复核 && global.AppResult == "1")
                    {
                        global.Status = "审核通过";
                    }
                    if (task.StepName == STEPS.申请人复核 && global.AppResult == "4")
                    {
                        global.Status = "审核未通过，继续处理";
                    }

                    if((task.StepName== "部门审批" || task.StepName == "系统负责人" || task.StepName=="信息部审核" || task.StepName=="问题处理" || task.StepName=="解决审核" || task.StepName == "解决方案设计" || task.StepName == "解决方案审核" || task.StepName == "具体处理") && global.AppResult == "2")
                    {
                        global.Status = "问题反馈中";
                    }
                    if ((task.StepName == "部门审批" || task.StepName == "系统负责人" || task.StepName == "信息部审核" ) && global.AppResult == "4")
                    {
                        global.Status = "任务已取消";
                    }
                    if ((task.StepName == "解决方案设计" || task.StepName == "问题处理" || task.StepName=="具体处理") && global.AppResult=="4")
                    {
                        global.Status = "问题分类中";
                    }
                    if ((task.StepName == "解决方案审核" || task.StepName == "解决审核") && global.AppResult == "4")
                    {
                        global.Status = "问题处理中";
                    }
                    #endregion


                }
                #endregion
            }
        }

        private static string[] 保存数据节点 = new[] { "" };
        public class DBTEST : ProcessData<MyGlobal>
        {
            protected override string 给业务表起个名字()
            {
                return "ID_ITServerSQ";
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
        public class MyGlobal : GlobalType
        {
            #region 基础信息
            public string SN { get; set; } //单号
            public string ApplyDate { get; set; }//申请时间           
            public string ApplicantID { get; set; }
            public string ApplicantNo { get; set; }
            public string Applicant { get; set; }//制单人
            public string ApplyCompanyID { get; set; }//所属公司
            public string ApplyCompany { get; set; }//所属公司
            public string ApplyDepartmentID { get; set; }//所属部门
            public string ApplyDepartment { get; set; }//所属部门            
            public string Status { get; set; }//任务状态
            public string UploadFiles { get; set; }//文件上传
            public string Approvers { get; set; }// 需求部门动态审批
            public string ProcessChanegSQ { get; set; }//关联流程变更单
            public string Approvers2 { get; set; }//信息部动态审批
            #endregion

            #region 反馈信息
            public string FeedbackType { get; set; }//反馈类别
            //public string SystemID { get; set; }
            public string SysID { get; set; }
            public string SystemType { get; set; }//系统类别
            //public string ModuleID { get; set; }
            public string ModuleType { get; set; }//功能模块类别
            //public string BillID { get; set; }
            public string BillType { get; set; }//单据类别
            public string Content { get; set; }//具体内容
            #endregion

            #region 处理信息
            public string HandleType { get; set; }//处理方式
            public string HandoverNo { get; set; }
            public string HandoverID { get; set; }
            public string Handover { get; set; }//后续对接人
            public string DeveloperNo { get; set; }
            public string DeveloperID { get; set; }
            public string Developer { get; set; }//开发协助人
            
            public string PlanStartDate { get; set; }//预计开始日期
            public string PlanEndDate { get; set; }//预计结束日期
            public string PlanHours { get; set; }//预计工时
            public string ActualStartDate { get; set; }//实际开始日期
            public string ActualEndDate { get; set; }//实际结束日期
            public string ActualHours { get; set; }//实际工时
            public string HandleExplain { get; set; }//处理说明
            public string Appraise { get; set; }//处理说明
            //public string A1 { get; set; }//1分
            //public string A2 { get; set; }//2分
            //public string A3 { get; set; }//3分
            //public string A4 { get; set; }//4分
            //public string A5 { get; set; }//5分
            public string score { get; set; }//得分

            #endregion


        }

        public class SystemType
        {
            public string CompanyID { get; set; }
            public string CompanyName { get; set; }
            public string UserID { get; set; }
            public string UserNo { get; set; }
            public string UserName { get; set; }
            public string SystemName { get; set; }
        }
    }
}