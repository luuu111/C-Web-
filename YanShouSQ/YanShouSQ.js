function show2(SNS) {
    nnfDialogShow({ href: "../../Common/SNList.aspx?" + "SNS=" + SNS, data: "", title: "查看合同申请", width: 600, height: 400, draggable: true, buttons: "" }, function (data, o) {
    });
}
function formLoaded(isReadOnly, theObjE, task, isFirstStep, isIncidentExists) {
    blur();//焦点事件
    //$("[localform=ContractSN]").textbox({
    //    onChange: function () {
    //        var ContractSN = $("[localform=ContractSN]").textbox('getValue');
    //        if (ContractSN != null && ContractSN != "") {
    //            $("[localform='ContractSN']").parent().parent().children().first().html("<a onclick='show2(\"" + ContractSN + "\")'>关联合同申请[<font style='color:red'>点击查看原单</font>]</a>");
    //            //$("[localform='PurchaseRequisition']").parent().parent().children().first().html("<a onclick='show2(\"../../../Common/ViewOriginal.aspx?SN=" + PurchaseRequisition + "\")'>关联采购申请[点击查看原单]</a>");
    //        } else {
    //            $("[localform='ContractSN']").parent().parent().children().first().html("关联合同申请");
    //        }
    //    }
    //});
    $("#ToolsInfo").panel("close");
    $("#ProjectInfo").panel("close");
    $("[localform=ContractSN]").textbox({
        onChange: function () {
            var ContractSN = $("[localform=ContractSN]").textbox('getValue');
            if (ContractSN != null && ContractSN != "") {
                $("[localform='ContractSN']").parent().parent().children().first().html("<a href='../../../Common/ViewOriginal.aspx?SN=" + ContractSN + "'  target='_blank' >关联合同申请[<font style='color:red'>点击查看原单</font>]</a>");
          } else {
                $("[localform='ContractSN']").parent().parent().children().first().html("关联合同申请");
            }
       }
    });
    var editMode = 0; //默认不可编辑  if (!$("[localform = 'IsPlan']").switchbutton("options").checked)
   

    
    if (isFirstStep && !isReadOnly)//表单申请节点并且控件不是只读，那么设置为可编辑
    {
        editMode = 1; //可编辑
        //验收类型开关
        $("[localform='isTools']").switchbutton({
            onChange: function (checked) {
                if (checked) {
                    theObjE.G.HTMsgs = [];
                    HTGrid.initGrid({
                        _datas: theObjE.G.HTMsgs,
                        _editMode: editMode
                    });
                    $("#WLMXInfo").panel("open");
                    //隐藏基建信息
                    $("#nnfegHTGrid").datagrid("hideColumn", "ProjectName")
                    $("#nnfegHTGrid").datagrid("hideColumn", "BuildCompany")
                    $("#nnfegHTGrid").datagrid("hideColumn", "ProjectPlace")
                    $("#nnfegHTGrid").datagrid("hideColumn", "YanShouDate")
                    $("#nnfegHTGrid").datagrid("hideColumn", "ProjectInfo")
                    ////显示设备信息
                    $("#nnfegHTGrid").datagrid("showColumn", "ToolsNo")
                    $("#nnfegHTGrid").datagrid("showColumn", "FactoryName")
                    $("#nnfegHTGrid").datagrid("showColumn", "EntryDate")
                    $("#nnfegHTGrid").datagrid("showColumn", "UsePlace")
                    $("#nnfegHTGrid").datagrid("showColumn", "ToolsInfo")
                    $("#sb").show();
                    $("#jj").hide();
                    $("#sb").localform("enable");
                    $("#jj").localform("disable");
                }
                else {
                    theObjE.G.WLMXDetails = [];
                    WLMXGrid.initGrid({
                        _datas: theObjE.G.WLMXDetails,
                        _editMode: 0
                    });
                    theObjE.G.HTMsgs = [];
                    HTGrid.initGrid({
                        _datas: theObjE.G.HTMsgs,
                        _editMode: editMode
                    });
                    $("#WLMXInfo").panel("close");
                    //显示基建信息

                    $("#nnfegHTGrid").datagrid("showColumn", "ProjectName")
                    $("#nnfegHTGrid").datagrid("showColumn", "BuildCompany")
                    $("#nnfegHTGrid").datagrid("showColumn", "ProjectPlace")
                    $("#nnfegHTGrid").datagrid("showColumn", "YanShouDate")
                    $("#nnfegHTGrid").datagrid("showColumn", "ProjectInfo")
                    //隐藏设备信息
                    $("#nnfegHTGrid").datagrid("hideColumn", "ToolsNo")
                    $("#nnfegHTGrid").datagrid("hideColumn", "FactoryName")
                    $("#nnfegHTGrid").datagrid("hideColumn", "EntryDate")
                    $("#nnfegHTGrid").datagrid("hideColumn", "UsePlace")
                    $("#nnfegHTGrid").datagrid("hideColumn", "ToolsInfo")
                    $("#sb").hide();
                    $("#jj").show();
                    $("#jj").localform("enable");
                    $("#sb").localform("disable");
                }
            }
        });
       
        //$("#grid").datagrid("showColumn", "ECFHStatus")   $("#ZS").show();
    }
    else {
        $("#BaseInfo,#ToolsInfo,#ProjectInfo").localform("disable");
      
    }

    HTGrid.initGrid({
        _datas: theObjE.G.HTMsgs,
        _editMode: editMode,
        _changed: function () {
            if ($("[localform='isTools']").switchbutton("options").checked) {
                var filter = "";
                var billnolst = new Array();
                if (theObjE.G.HTMsgs.length > 0) {
                    $.each(theObjE.G.HTMsgs, function (index, n) {
                        billnolst.push(n.ContractSN);
                    });
                    filter = billnolst.join("','");
                }
                $.post('/api/PD/GetContractWLMX', { SNs: filter }, function (d, s) {
                    theObjE.G.WLMXDetails = d.list;
                    WLMXGrid.initGrid({
                        _datas: theObjE.G.WLMXDetails,
                        _editMode: 0
                    });
                });
            }
        }
    });

    WLMXGrid.initGrid({
        _datas: theObjE.G.WLMXDetails,
        _editMode: 0
    });

     //设置隐藏与显示;
    if (theObjE.G.isTools=="1") {
        $("#WLMXInfo").panel("open");
        //隐藏基建信息
        $("#nnfegHTGrid").datagrid("hideColumn", "ProjectName")
        $("#nnfegHTGrid").datagrid("hideColumn", "BuildCompany")
        $("#nnfegHTGrid").datagrid("hideColumn", "ProjectPlace")
        $("#nnfegHTGrid").datagrid("hideColumn", "YanShouDate")
        $("#nnfegHTGrid").datagrid("hideColumn", "ProjectInfo")
        ////显示设备信息
        $("#nnfegHTGrid").datagrid("showColumn", "ToolsNo")
        $("#nnfegHTGrid").datagrid("showColumn", "FactoryName")
        $("#nnfegHTGrid").datagrid("showColumn", "EntryDate")
        $("#nnfegHTGrid").datagrid("showColumn", "UsePlace")
        $("#nnfegHTGrid").datagrid("showColumn", "ToolsInfo")
        $("#sb").show();
        $("#jj").hide();
        $("#sb").localform("enable");
        $("#jj").localform("disable");

    }
    else {
        $("#WLMXInfo").panel("close");
        //显示基建信息

        $("#nnfegHTGrid").datagrid("showColumn", "ProjectName")
        $("#nnfegHTGrid").datagrid("showColumn", "BuildCompany")
        $("#nnfegHTGrid").datagrid("showColumn", "ProjectPlace")
        $("#nnfegHTGrid").datagrid("showColumn", "YanShouDate")
        $("#nnfegHTGrid").datagrid("showColumn", "ProjectInfo")
        //隐藏设备信息
        $("#nnfegHTGrid").datagrid("hideColumn", "ToolsNo")
        $("#nnfegHTGrid").datagrid("hideColumn", "FactoryName")
        $("#nnfegHTGrid").datagrid("hideColumn", "EntryDate")
        $("#nnfegHTGrid").datagrid("hideColumn", "UsePlace")
        $("#nnfegHTGrid").datagrid("hideColumn", "ToolsInfo")
        $("#sb").hide();
        $("#jj").show();
        $("#jj").localform("enable");
        $("#sb").localform("disable");
    }
    
    $("#BaseInfo,#ToolsInfo,#ProjectInfo").localform("load", theObjE.G);

    if (isReadOnly) {
        if (theObjE.G.IsOld == "1") {
            $("#HTHZInfo").panel("close");
            if (theObjE.G.isTools == "0") {
                $("#WLMXInfo").panel("close");
                $("#ProjectInfo").panel("open");
            } else {
                $("#ToolsInfo").panel("open");
            }
        }
    }
}

//关联合同申请按钮
function SelectHTSQ() {
    var filter = "";
    var billnolst = new Array();
    if (theObjE.G.HTMsgs.length > 0) {
        $.each(theObjE.G.HTMsgs, function (index, n) {
            billnolst.push(n.ContractSN);
        });
        filter = billnolst.join(",");
    }
    //获取当前验收类型
    if (!$("[localform = 'isTools']").switchbutton("options").checked) {
        //基建类 
        nnfDialogShow({
            href: "../../NNFext/Dialog.aspx?dlg=zgcht&UserID=" + $("[localform=ApplicantID]").val() + "&Filter=" + filter, title: "选择合同"
        }, function (data, o) {
            if (data) {
                $("[localform=ContractSN]").textbox("setValue", data[0].tSN);//单号
                $("[localform='ProjectName']").textbox("setValue", data[0].EngineerName);  //工程名称
                $("[localform='BuildCompany']").textbox("setValue", data[0].SupplierName);  //施工单位
                $("[localform='ContractName']").textbox("setValue", data[0].ContractName);  //合同名称
                $("[localform='ContractNo']").textbox("setValue", data[0].ContractNo);  //合同编号
                $("[localform='ContractMoney']").textbox("setValue", data[0].ContractAmount);  //合同金额
            }
        });
    }
    else {
        //设备类
        nnfDialogShow({
            href: "../../NNFext/Dialog.aspx?dlg=zzcht&UserID=" + $("[localform=ApplicantID]").val() + "&Filter=" + filter, title: "选择合同"
        }, function (data, o) {
            if (data) {
                $("[localform=ContractSN]").textbox("setValue", data[0].tSN);//单号
                $("[localform='ContractName']").textbox("setValue", data[0].ContractName);  //合同名称
                $("[localform='ContractNo']").textbox("setValue", data[0].ContractNo);  //合同编号
                $("[localform='ContractMoney']").textbox("setValue", data[0].ContractAmount);  //合同金额
                $("[localform='FactoryName']").textbox("setValue", data[0].SupplierName);  //施工单位
            }
        });
    }

}

//指定验收人员
function SelectUser() {
    selopen2($("[localform='SupID']").val(), "0-0-1-0", function (result, isCanceled) {
        if (!isCanceled && result) {
            $("[localform='SupName']").textbox("setValue", result.NameStr2);
            $("[localform='SupID']").val(result.IDStr);
            $("[localform='SupNo']").val(result.ShortName);
        }
    });
}

function preSubmit(core, isSend) {
    if (isSend) {
        if (theObjE.G.HTMsgs.length == 0) {
            alert("合同明细不能为空！！！！！！！！！！");
            return;
        }

        if (!$("#BaseInfo").localform("validate")) {
            return;
        }
    }
    $.extend(theObjE.G, $("#BaseInfo").localform("save"));
    core();//真正保存数据到服务器的代码
}

//选择物料
function SelectMateriel() {
    if ($("[localform=DBName]").val()) {
        nnfDialogShow({ href: "../../NNFext/Dialog.aspx?" + "dlg=Material&DBName=" + $("[localform=DBName]").val(), data: "", title: "选择物料", width: 600, height: 400, draggable: true }, function (data, o) {
            if (data) {
                $("[localform=WLMX_MaterialNo]").textbox("setValue", data[0].FNumber);
                $("[localform=WLMX_MaterialName]").textbox("setValue", data[0].FName);
                $("[localform=WLMX_SpecificationsModels]").textbox("setValue", data[0].FModel);
                $("[localform=WLMX_UnitName]").textbox("setValue", data[0].FDefaultUnitName);
                $("[localform=WLMX_UnitNo]").val(data[0].FDefaultUnitNumber);
            }
        });
    }
}

//选择单位
function SelectUnit() {
    if ($("[localform=DBName]").val()) {
        nnfDialogShow({ href: "../../NNFext/Dialog.aspx?" + "dlg=Unit&DBName=" + $("[localform=DBName]").val() , data: "", title: "选择单位", width: 600, height: 400, draggable: true }, function (data, o) {
            if (data) {
                $("[localform=WLMX_UnitName]").textbox("setValue", data[0].FUnitName);
                $("[localform=WLMX_UnitNo]").val(data[0].FUnitNumber);
            }
        });
    } 
}

//光标离开事件
function blur() {
    $("input", $("[localform=WLMX_TaxPrice]").next("span")).blur(function () {
        var WLMX_TaxPrice = $("[localform=WLMX_TaxPrice]").numberbox('getValue');
        var WLMX_Quantity = $("[localform=WLMX_Quantity]").numberbox('getValue');
        var WLMX_Amount = parseFloat(WLMX_TaxPrice) * parseFloat(WLMX_Quantity);
        $("[localform=WLMX_Amount]").numberbox('setValue', WLMX_Amount);
    });
    $("input", $("[localform=WLMX_Quantity]").next("span")).blur(function () {
        var WLMX_TaxPrice = $("[localform=WLMX_TaxPrice]").numberbox('getValue');
        var WLMX_Quantity = $("[localform=WLMX_Quantity]").numberbox('getValue');
        var WLMX_Amount = parseFloat(WLMX_TaxPrice) * parseFloat(WLMX_Quantity);
        $("[localform=WLMX_Amount]").numberbox('setValue', WLMX_Amount);
    });

}

//单据跳转
function rowformater(value, row, index) {
    var url = "../../Common/ViewOriginal.aspx?SN=" + value;
    return '<a target="view_window" href=' + url + '>' + value + '</a>';
}