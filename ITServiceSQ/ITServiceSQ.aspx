<%@ Page Title="" Language="C#" MasterPageFile="~/Master/A4Style.Master" AutoEventWireup="true" CodeBehind="ITServiceSQ.aspx.cs" Inherits="NNFPage.Processes.ITServiceSQ.ITServiceSQ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="beforeA4" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="easyui-panel" title="表单流程说明：">
        <span style="color: red; font-weight: bold; font-size: 13px;">
            1.同一系统类型的问题填写一个单子,不同类的分开填写.
             <br />
            2.在【解决方案设计】、【问题处理】节点，当实际处理人指派错误，可重新选择正确处理人，然后选择转签.
            <br />
            3.涉及流程新增和流程变更，请走【流程变更审批单】，审批完结后会自动发起本单
        </span>
    </div>
    <div id="Info" class="easyui-panel" title="基础信息" data-options="collapsible:true">
        <div nnflayout="4">
            <input type="hidden" localform="ApplicantID" />
            <input type="hidden" localform="ApplicantNo" />
            <input type="hidden" localform="ApplyDepartmentID" />
            <input type="hidden" localform="ApplyCompanyID" />
            <input class="easyui-textbox" nnfedititem="单据编号" localform="SN" data-options="prompt:'自动生成',readonly:true" />
            <input class="easyui-datebox" nnfedititem="制单日期" localform="ApplyDate" data-options="prompt:'自动生成',readonly:true" />
            <input class="easyui-textbox" nnfedititem="制单人" localform="Applicant" data-options="prompt:'自动生成',readonly:true " />
            <input class="easyui-textbox" nnfedititem="所属部门" localform="ApplyDepartment" data-options="prompt:'自动生成',readonly:true " />
            <input class="easyui-textbox" nnfedititem="所属公司" localform="ApplyCompany" data-options="prompt:'自动生成',readonly:true " />
            <input class="easyui-textbox" nnfedititem="任务状态" localform="Status" data-options="required:true,editable:false" />
            <input class="easyui-textbox" nnfedititem="关联流程变更" localform="ProcessChanegSQ" buttontext="选择" data-options="prompt:'自动生成',readonly:true,editable:false,onClickButton:function(){ SelectRelatinSQ()}" />
            
            
        </div>
    </div>

    <div id="Info2" class="easyui-panel" title="反馈信息" data-options="collapsible:true">
        <div nnflayout="4">
            <%--<input class="easyui-combobox" nnfedititem="反馈类别" localform="FeedbackType" data-options="required:true,editable:false,valueField: 'label',textField: 'value',
		                            data: [{
			                            label: '硬件网络',
			                            value: '硬件网络'
		                            },{
			                            label: '系统故障',
			                            value: '系统故障'
		                            },{
			                            label: '数据处理',
			                            value: '数据处理'
		                            },{
			                            label: '权限申请',
			                            value: '权限申请'
		                            },{
			                            label: '流程优化',
			                            value: '流程优化'
		                            },{
			                            label: '功能完善',
			                            value: '功能完善'
		                            }]" />           --%>
            <input type="hidden" localform="SysID" />
            <input class="easyui-textbox" nnfedititem="反馈类别" localform="SystemType" buttontext="选择" data-options="editable:false,required:true,onClickButton:function(){SelectSystemType()}" />
            <input class="easyui-textbox" nnfedititem="反馈详情" localform="ModuleType" buttontext="选择" data-options="editable:false,required:true,onClickButton:function(){SelectModuleType()}" />
            <%--<input class="easyui-textbox" nnfedititem="单据类别" localform="BillType" buttontext="选择" data-options="editable:false,onClickButton:function(){SelectBillType()}" />--%>
            <input class="easyui-textbox" nnfedititem="具体内容" localform="Content" nnf-size="4" data-options="multiline:true,required:true" style="height:100px" />
        </div>
    </div>

    <div id="Info3" class="easyui-panel" title="处理信息" data-options="collapsible:true">
        <div nnflayout="4">
            <input class="easyui-combobox" nnfedititem="处理方式" localform="HandleType" data-options="required:false,disabled:true, editable:false,valueField: 'label',textField: 'value',
		                            data: [{
			                            label: '直接回复',
			                            value: '直接回复',                                        
		                            },{
			                            label: '后续处理',
			                            value: '后续处理'
		                            }]" />            
        </div>
        <div nnflayout="4" id="HandoverInfo">
            <input type="hidden" localform="HandoverNo" />
            <input type="hidden" localform="HandoverID" />
            <input type="hidden" localform="DeveloperNo" />
            <input type="hidden" localform="DeveloperID" />
            <input class="easyui-textbox" nnfedititem="后续对接人" localform="Handover" buttontext="选择" data-options="editable:false,disabled:true,onClickButton:function(){SelectHandover()}" />
            <input class="easyui-textbox" nnfedititem="开发协助人" localform="Developer" buttontext="选择" data-options="editable:false,disabled:true,onClickButton:function(){SelectDeveloper()}" />
            <input class="easyui-datebox" nnfedititem="预计开始日期" localform="PlanStartDate" data-options="editable:false,disabled:true" />
            <input class="easyui-datebox" nnfedititem="预计结束日期" localform="PlanEndDate" data-options="editable:false,disabled:true" />
            <input class="easyui-textbox" nnfedititem="预计工时" localform="PlanHours" data-options="disabled:true" />
            <input class="easyui-datebox" nnfedititem="实际开始日期" localform="ActualStartDate" data-options="editable:false,disabled:true" />
            <input class="easyui-datebox" nnfedititem="实际结束日期" localform="ActualEndDate" data-options="editable:false,disabled:true" />
            <input class="easyui-textbox" nnfedititem="实际工时" localform="ActualHours" data-options="disabled:true" />
        </div>
        <div nnflayout="4">
            <input class="easyui-textbox" nnfedititem="处理说明" localform="HandleExplain" nnf-size="4" data-options="multiline:true,required:false,disabled:true" style="height:100px" />
        </div>
        <div id="info4" nnflayout="4">
            <%--<input class="nnf-options" nnfedititem="处理结果评分*" nnf-size="2" data-options="required:true,radio:true,options:'★(非常差)  ,★★(差)  ,★★★(一般)  ,★★★★(好)  ,★★★★★(非常好),',localform:'A1,A2,A3,A4,A5'" />--%>
            <input class="nnf-options" nnfedititem="处理结果评分*" nnf-size="4" data-options="required:false,radio:true,options:'5:★★★★★(非常好)  ,4:★★★★(好)  ,3:★★★(一般)  ,2:★★(差)  ,1:★(非常差),',localform:'score'" />
            <input class="easyui-textbox" nnfedititem="改善建议（建议从服务态度、质量、效率方面提出改善建议）" localform="Appraise" nnf-size="4" data-options="multiline:true,required:false,disabled:true" style="height:100px" />
        </div>

    </div>

    <nnf:LiteFiles MaxFileSizeMB="0" runat="server" ID="Files1" EntityPropertyName="G.UploadFiles" Title="附件"></nnf:LiteFiles>
    <nnf:App runat="server" ID="app1" Visible="true" Opion="123456" Title="审批意见"></nnf:App>
    <nnf:AppHistory runat="server" ID="appHistory1" Visible="true" Title="审批意见列表"></nnf:AppHistory>
</asp:Content>

