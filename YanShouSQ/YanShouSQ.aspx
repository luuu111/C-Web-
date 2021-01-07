<%@ Page Title="" Language="C#" MasterPageFile="~/Master/A4Style.Master" AutoEventWireup="true" CodeBehind="YanShouSQ.aspx.cs" Inherits="NNFPage.Processes.YanShouSQ.YanShouSQ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="beforeA4" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="BaseInfo" class="easyui-panel" title="基础信息" data-options="collapsible:true">
        <div nnflayout="4">
            <%--<input type="hidden" localform="Incident" />--%>
            <input type="hidden" localform="ApplicantID" />
            <input type="hidden" localform="ApplicantNo" />
            <input type="hidden" localform="DBName" />
            <input class="easyui-textbox" nnfedititem="单据编号" localform="SN" data-options="prompt:'自动生成',readonly:true" />
            <input class="easyui-datebox" nnfedititem="申请日期" localform="ApplyDate" data-options="prompt:'自动生成',readonly:true" />
            <input class="easyui-textbox" nnfedititem="申请人" localform="Applicant" data-options="prompt:'自动生成',readonly:true " />
            <input class="easyui-textbox" nnfedititem="部门名称" localform="ApplyDepartment" data-options="prompt:'自动生成',readonly:true " />
            <input type="hidden" localform="isToolsID" />
            <input class="easyui-switchbutton" nnfedititem="验收类型" localform="isTools" ontext="设备材料类" offtext="基建类" style="width: 100px" />
            <input type="hidden" localform="SupID" />
            <input type="hidden" localform="SupNo" />
            <input class="easyui-textbox" nnfedititem="指定验收人员*" localform="SupName" data-options="editable:false,required:true,onClickButton:function(){SelectUser();}" buttontext="选择" />
        </div>
    </div>

     <div id="ToolsInfo" class="easyui-panel" title="设备信息" data-options="collapsible:true">
        <div nnflayout="4">
            <input class="easyui-textbox" nnfedititem="合同名称*" nnf-size="2" localform="ToolsContractName" data-options="readonly:false" />
            <input class="easyui-textbox" nnfedititem="合同编号*" localform="ToolsContractNo" data-options="readonly:false" />
            <input class="easyui-numberbox" nnfedititem="合同金额*" localform="ToolsContractMoney" data-options="min:0,precision:2,readonly:false" />
            <input class="easyui-textbox" nnfedititem="设备名称*" localform="ToolsName" />
            <input class="easyui-textbox" nnfedititem="出厂编号*" localform="ToolsNo" />
            <input class="easyui-textbox" nnfedititem="规格型号*" localform="ToolsType" />
            <input class="easyui-numberbox" nnfedititem="价格*" localform="ToolsPrice" data-options="min:0,precision:2" />

            <input class="easyui-textbox" nnfedititem="生产厂家*" localform="FactoryName" />
            <input class="easyui-datebox" nnfedititem="进场时间*" localform="EntryDate" />
            <input class="easyui-textbox" nnfedititem="使用场所*" localform="UsePlace" />

            <input type="hidden" localform="ToolSupID" />
            <input type="hidden" localform="ToolSupNo" />
            <input class="easyui-textbox" nnfedititem="指定验收人员*" localform="ToolSupName" data-options="editable:false,onClickButton:function(){SelectUser2();}" buttontext="选择" />
        </div>
        <div nnflayout="4">
            <input class="easyui-textbox" nnfedititem="主要技术参数（技术要求）*" localform="ToolsInfo" nnf-size="4" data-options="multiline:true" style="height: 100px;" />
        </div>
    </div>
    
    <div id="ProjectInfo" class="easyui-panel" title="基建信息" data-options="collapsible:true">
        <div nnflayout="4">
            <input class="easyui-textbox" nnfedititem="合同名称*" nnf-size="2" localform="ContractName" data-options="readonly:true" />
            <input class="easyui-textbox" nnfedititem="合同编号*" localform="ContractNo" data-options="readonly:true" />
            <input class="easyui-numberbox" nnfedititem="合同金额*" localform="ContractMoney" data-options="readonly:true" />
            <input class="easyui-textbox" nnfedititem="工程名称*" nnf-size="2" localform="ProjectName" data-options="readonly:true" />
            <input class="easyui-textbox" nnfedititem="施工单位*" nnf-size="2" localform="BuildCompany" data-options="readonly:true" />
            <input class="easyui-textbox" nnfedititem="施工地点*" nnf-size="2" localform="ProjectPlace" data-options="required:true" />
            <input class="easyui-datebox" nnfedititem="验收日期*" localform="YanShouDate" data-options="required:true" />
            <input type="hidden" localform="SupID" />
            <input type="hidden" localform="SupNo" />
            <input class="easyui-textbox" nnfedititem="指定验收人员*" localform="SupName" data-options="editable:false,required:true,onClickButton:function(){SelectUser();}" buttontext="选择" />
        </div>
        <div nnflayout="4">
            <input class="easyui-textbox" nnfedititem="施工项目清单*" localform="ProjectInfo" nnf-size="4" data-options="multiline:true,required:true" style="height: 100px;" />
        </div>
    </div>

    <div id="HTHZInfo" class="easyui-panel" data-options="collapsible:true">
        <nnf:EasyGrid runat="server" ID="HTGrid" Title="合同明细" ShowFooter="True" DialogWidth="800px" DialogHeight="500px">
            <thead>
                <th data-options="field:'ContractSN',align:'center',width:130,formatter:rowformater">合同单号</th>
                <th data-options="field:'ContractName',align:'center',width:200">合同名称</th>
                <th data-options="field:'ContractNo',align:'center',width:130">合同编号</th>
                <th data-options="field:'ContractMoney',align:'center',width:80">合同金额</th>

                <th data-options="field:'ProjectName',align:'center',width:150">工程名称</th>
                <th data-options="field:'ProjectPlace',align:'center',width:150">施工地点</th>
                <th data-options="field:'BuildCompany',align:'center',width:150">施工单位</th>
                <th data-options="field:'YanShouDate',align:'center',width:80">验收日期</th>
                <th data-options="field:'ProjectInfo',align:'center',width:600">施工项目清单</th>

                <th data-options="field:'ToolsNo',align:'center',width:150">出厂编号</th>
                <th data-options="field:'FactoryName',align:'center',width:150">生产厂家</th>
                <th data-options="field:'EntryDate',align:'center',width:80">进场日期</th>
                <th data-options="field:'UsePlace',align:'center',width:150">使用场所</th>
                <th data-options="field:'ToolsInfo',align:'center',width:600">主要技术参数(技术要求)</th>
            </thead>
            <form>
                <div nnflayout="4">
                    <input class="easyui-textbox" nnfedititem="关联合同申请" nnf-size="3" localform="ContractSN" data-options="editable:false,required:true,onClickButton:function(){SelectHTSQ();}" buttontext="选择" />
                    <input class="easyui-numberbox" nnfedititem="合同金额*" localform="ContractMoney" data-options="min:0,precision:2,readonly:true,required:true" />
                    <input class="easyui-textbox" nnfedititem="合同名称*" nnf-size="2"localform="ContractName" data-options="readonly:true,required:true" />
                    <input class="easyui-textbox" nnfedititem="合同编号*" nnf-size="2" localform="ContractNo" data-options="readonly:true,required:true" />
                    
                </div>

                <div nnflayout="4" id="jj">
                    <input class="easyui-textbox" nnfedititem="工程名称*" nnf-size="2" localform="ProjectName" data-options="required:true" />
                    <input class="easyui-textbox" nnfedititem="施工单位*" nnf-size="2" localform="BuildCompany" data-options="required:true" />
                    <input class="easyui-textbox" nnfedititem="施工地点*" nnf-size="2" localform="ProjectPlace" data-options="required:true" />
                    <input class="easyui-datebox" nnfedititem="验收日期*" nnf-size="2" localform="YanShouDate" data-options="required:true" />
                    <input class="easyui-textbox" nnfedititem="施工项目清单*" localform="ProjectInfo" nnf-size="4" data-options="multiline:true,required:true" style="height: 100px;" />
                </div>

                <div nnflayout="4" id="sb">
                    <input class="easyui-textbox" nnfedititem="出厂编号*" nnf-size="2" localform="ToolsNo" data-options="readonly:false,required:false" />
                    <input class="easyui-textbox" nnfedititem="生产厂家*" nnf-size="2" localform="FactoryName" data-options="readonly:false,required:true" />
                    <input class="easyui-datebox" nnfedititem="进场时间*" nnf-size="2" localform="EntryDate" data-options="readonly:false,required:true"  />
                    <input class="easyui-textbox" nnfedititem="使用场所*" nnf-size="2" localform="UsePlace" data-options="readonly:false,required:true" />
                    <input class="easyui-textbox" nnfedititem="主要技术参数（技术要求）*" localform="ToolsInfo" nnf-size="4" data-options="multiline:true,required:true" style="height: 100px;" />
                </div>
            </form>
        </nnf:EasyGrid>
    </div>

    <div id="WLMXInfo" class="easyui-panel" data-options="collapsible:true">
        <nnf:EasyGrid runat="server" ID="WLMXGrid" Title="设备明细" ShowFooter="True" DialogWidth="600px" DialogHeight="400px">
            <thead>
                <th data-options="field:'WLMX_MaterialNo',align:'center',width:150">物料代码</th>
                <th data-options="field:'WLMX_MaterialName',align:'center',width:200">物料名称</th>
                <th data-options="field:'WLMX_SpecificationsModels',align:'center',width:150">规格型号</th>
                <th data-options="field:'WLMX_UnitNo',align:'center',width:100" hidden="hidden">单位编码</th>
                <th data-options="field:'WLMX_UnitName',align:'center',width:100">单位名称</th>
                <th data-options="field:'WLMX_TaxPrice',align:'center',width:100">含税单价</th>
                <th data-options="field:'WLMX_Quantity',align:'center',width:100">数量</th>
                <th data-options="field:'WLMX_Amount',align:'center',width:100">金额</th>
            </thead>
            <form>
                <div nnflayout="3">
                    <input class="easyui-textbox" nnfedititem="物料代码" localform="WLMX_MaterialNo" data-options="required:true,editable:false,onClickButton:function(){SelectMateriel();}" buttontext="选择" />
                    <input class="easyui-textbox" nnfedititem="物料名称" localform="WLMX_MaterialName" data-options="required:true,editable:false" />
                    <input class="easyui-textbox" nnfedititem="规格型号" localform="WLMX_SpecificationsModels" data-options="editable:false" />/>
                    <input hidden="hidden" localform="WLMX_UnitNo">
                    <input class="easyui-textbox" nnfedititem="单位名称" localform="WLMX_UnitName" data-options="required:true,editable:false,onClickButton:function(){SelectUnit();}" buttontext="选择" />
                    <input class="easyui-numberbox" nnfedititem="含税单价" localform="WLMX_TaxPrice" data-options="min:0,precision:4,required:true" />
                    <input class="easyui-numberbox" nnfedititem="数量" localform="WLMX_Quantity" data-options="min:0,precision:4,required:true" />
                    <input class="easyui-numberbox" nnfedititem="金额" localform="WLMX_Amount" data-options="required:true,editable:false" />
                </div>
            </form>
        </nnf:EasyGrid>
    </div>


   
    <nnf:LiteFiles MaxFileSizeMB="0" runat="server" ID="Files1" EntityPropertyName="G.UploadFiles" Title="附件"></nnf:LiteFiles>
    <nnf:App runat="server" ID="app1" Visible="true" Opion="12456" Title="审批意见"></nnf:App>
    <nnf:AppHistory runat="server" ID="appHistory1" Visible="true" Title="审批意见列表"></nnf:AppHistory>
</asp:Content>

