<%@ Page Title="" Language="C#" MasterPageFile="~/Master/A4Style.Master" AutoEventWireup="true" CodeBehind="AssetsTransferSQ.aspx.cs" Inherits="NNFPage.Processes.AssetsTransferSQ.AssetsTransferSQ" %>
<asp:Content ID="Content4" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="beforeA4" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

   
    <div id="Info" class="easyui-panel" title="基本信息" data-options="collapsible:true">
        <div nnflayout="3">
            <input type="hidden" localform="ApplicantID" />
            <input type="hidden" localform="ApplicantNo" />
            <input type="hidden" localform="DBName" />
            <input class="easyui-textbox" nnfedititem="单据编号" localform="SN" data-options="prompt:'自动生成',readonly:true " />
            <input type="hidden" localform="DepartmentID" />
            <input class="easyui-textbox" nnfedititem="所属部门" localform="DepartmentName" data-options="prompt:'自动生成',readonly:true" />
            <input type="hidden" localform="ApplicantID" />
            <input class="easyui-textbox" nnfedititem="申请人" localform="ApplicantName" data-options="prompt:'自动生成',readonly:true"/>
            <input class="easyui-datebox" nnfedititem="制单日期" localform="ApplyDate" data-options="prompt:'自动生成',readonly:true"/>
            <input class="easyui-combobox" nnfedititem="资产所属公司" nnf-size="1" localform="CompanyID,CompanyName" data-options="prompt:'自动生成'" />
            <input class="easyui-textbox" nnfedititem="资产存放地点" localform="AssetLocationName" data-options="required:true" />
        </div>
    </div>


    <div id="Info2" class="easyui-panel" title="申请信息" data-options="collapsible:true">
        <div nnflayout="6">
            <input type="hidden" localform="DepartmentIDOld" />
            <input type="hidden" localform="DepartmentIDNew" />
            <input class="easyui-textbox" nnfedititem="资产名称" localform="AssetName" nnf-size="3"  data-options="required:true"/>
            <input class="easyui-textbox" nnfedititem="规格型号" localform="Model" nnf-size="3"data-options="required:true"/>
            <input class="easyui-textbox" nnfedititem="资产类别" localform="AssetType" nnf-size="2" data-options="required:true,editable:false,onClickButton:function(){SelectAssetType();}" buttontext="选择" />
            <input class="easyui-textbox" nnfedititem="原使用部门" localform="DepartmentNameOld" nnf-size="2" data-options="required:true,editable:false,onClickButton:function(){SelectDeptOld();}" buttontext="选择" /> 
            <input class="easyui-textbox" nnfedititem="拟转入部门"  localform="DepartmentNameNew" nnf-size="2" data-options="required:true,editable:false,onClickButton:function(){SelectDeptNew();}" buttontext="选择" /> 
            <input class="easyui-textbox" nnfedititem="转出部门申请原因" nnf-size="6" localform="TransferReasons" data-options="prompt:'请注明申请原因:',multiline:true,required:true" style="height: 90px;"/>
        
        </div>
    </div>

     <div id="Info3" class="easyui-panel" title="资产信息" data-options="collapsible:true">
        <div nnflayout="3">
          
              <input class="easyui-textbox" nnfedititem="资产编号" localform="AssetNumber" data-options="required:true,readonly:false "/>
              <input class="easyui-datebox" nnfedititem="购置时间" localform="BuyTime" data-options="required:true,readonly:false " />
              <input class="easyui-textbox" nnfedititem="折旧年限" localform="ZJNX" data-options="readonly:false " />
              <input class="easyui-textbox" nnfedititem="账面原值" localform="BeforeValue"data-options="readonly:false " />
              <input class="easyui-textbox" nnfedititem="账面净值" localform="NowValue" data-options="readonly:false " />
              <input class="easyui-textbox" nnfedititem="备注信息" nnf-size="6" localform="Note" data-options="prompt:'在此添加补充备注:',multiline:true,required:true" style="height: 90px;"/>
        </div>
    </div>

    <nnf:LiteFiles MaxFileSizeMB="0" runat="server" ID="Files1" EntityPropertyName="G.UploadFiles" Title="附件"></nnf:LiteFiles>
    <nnf:App runat="server" ID="app1" Visible="true" Opion="12456" Title="审批意见"></nnf:App>
    <nnf:AppHistory runat="server" ID="appHistory1" Visible="true" Title="审批意见列表"></nnf:AppHistory>

    

</asp:Content>
