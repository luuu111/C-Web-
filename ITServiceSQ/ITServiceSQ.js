function formLoaded(isReadOnly, theObjE, task, isFirstStep, isIncidentExists) {
    //关联流程变更审批单
    $("[localform=ProcessChanegSQ]").textbox({
        onChange: function () {
            var ProcessChanegSQ = $("[localform=ProcessChanegSQ]").textbox('getValue');
            if (ProcessChanegSQ != null && ProcessChanegSQ != "") {
                $("[localform='ProcessChanegSQ']").parent().parent().children().first().html("<a href='../../Common/ViewOriginal.aspx?SN=" + ProcessChanegSQ + "'  target='_blank' >关联流程变更单原单[<font style='color:red'>点击查看原单</font>]</a>");
            } else {
                $("[localform='ProcessChanegSQ']").parent().parent().children().first().html("关联流程变更单");
            }
        }
    });

    var editMode = 0; //默认不可编辑
    //ComboboxLoad();

    if (isFirstStep && !isReadOnly)//表单申请节点并且控件不是只读，那么设置为可编辑
    {
        editMode = 1; //可编辑       
        $("#info4").localform("disable");
    }
    else {
        $("#Info,#Info2,#Info3").localform("disable");
    }

    if (theObjE.G.HandleType != "" && theObjE.G.HandleType != null) {
        if (theObjE.G.HandleType == "直接回复") {
            $("#HandoverInfo").hide();
        }
        else {
            $("#HandoverInfo").show();
        }
    }

    if (task.StepName == "系统负责人" && !isReadOnly) {
        //$("[localform='SystemType']").textbox({ required: true, disabled: false, editable: false });
        //$("[localform='ModuleType'],[localform='BillType']").textbox({ disabled: false });
        $("[localform='HandleType']").combobox({ required: true, editable: false, disabled: false });
        $("[localform='HandleType']").combobox({
            onChange: function (n, o) {
                if (n != null) {
                    if (n == "直接回复") {
                        $("#HandoverInfo").hide()
                        //$("#HandoverInfo").localform("disable");
                        $("[localform='HandleExplain']").textbox({ required: true, disabled: false })
                        $("[localform='Handover']").textbox("setValue", "");
                        $("[localform='HandoverNo'],[localform='HandoverID']").val("");
                        $("[localform='Handover']").textbox({ required: false, disabled: true, editable: false });
                    }
                    else {
                        $("#HandoverInfo").show();
                        //$("#HandoverInfo").localform("enable");
                        $("[localform='Handover']").textbox({ required: true, disabled: false, editable: false });
                        $("[localform='HandleExplain']").textbox({ required: false, disabled: true });
                        $("[localform='HandleExplain']").textbox("setValue", "");
                    }
                }
            }
        })
    }
    if (task.StepName == "解决方案设计" && !isReadOnly) {
        $("[localform='Handover']").textbox({ required: true, disabled: false });
        $("[localform='HandleExplain']").textbox({ required: true, disabled: false });
        $("[localform='Developer']").textbox({ required: false, disabled: false });
        $("[localform='PlanStartDate'],[localform='PlanEndDate']").datebox({ required: true, disabled: false, editable: false });
        $("[localform='PlanHours']").textbox({ required: true, disabled: false });
    }
    if (task.StepName == "具体处理" && !isReadOnly) {
        $("[localform='HandleExplain']").textbox({ required: true, disabled: false });
        $("[localform='ActualStartDate'],[localform='ActualEndDate']").datebox({ required: true, disabled: false, editable: false });
        $("[localform='ActualHours']").textbox({ required: true, disabled: false });
    }
    if (task.StepName == "问题处理" && !isReadOnly) {
        $("[localform='Developer']").textbox({ required: false, disabled: false });
        $("[localform='Handover']").textbox({ required: true, disabled: false });
        $("[localform='HandleExplain']").textbox({ required: true, disabled: false });
        $("[localform='PlanStartDate'],[localform='PlanEndDate']").datebox({ required: true, disabled: false, editable: false });
        $("[localform='PlanHours']").textbox({ required: true, disabled: false });
        $("[localform='ActualStartDate'],[localform='ActualEndDate']").datebox({ required: true, disabled: false, editable: false });
        $("[localform='ActualHours']").textbox({ required: true, disabled: false });
    }
    if (task.StepName == "申请人复核" && !isReadOnly) {
        $("#info4").localform("enable");
    }
    




    $("#Info,#Info2,#Info3").localform("load", theObjE.G);

}

function ComboboxLoad() {
    $("[localform='SystemType']").combobox({
        data: theObjE.SystemClass,
        valueField: 'Value',
        textField: 'Text',
        onSelect: function (row) {
        },
    });

}

function SelectRelatinSQ() {
    nnfDialogShow({ href: "../../NNFext/Dialog.aspx?dlg=itfwlcbg" + "&EmpNo=" + $("[localform=ApplicantNo]").val(), data: "", title: "选择流程变更单", width: 600, height: 400, draggable: true },
        function (data, o) {
            if (data) {
                $("[localform='ProcessChanegSQ']").textbox("setValue", data[0].tSN);               
            }
        });
}

function SelectSystemType() {
    var SystemType = $("[localform='SystemType']").textbox('getValue');
    if (SystemType != "" && SystemType != null) {
        if (confirm("您已选择过反馈类别,若选择新的类别将清空原有数据,请确定是否继续！")) {
            nnfDialogShow({ href: "../../NNFext/Dialog.aspx?dlg=systemtype", data: "", title: "选择反馈类别", width: 650, height: 400, draggable: true },
                function (data, o) {
                    if (data) {
                        $("[localform=SysID]").val(data[0].SysID);
                        $("[localform=SystemType]").textbox("setValue", data[0].SystemType);
                    }
                });
            $("[localform='SystemType']").textbox({
                onChange: function () {
                    $("[localform='ModuleType']").textbox("setValue", "");
                }
            })

        }
    }
    else {
        nnfDialogShow({ href: "../../NNFext/Dialog.aspx?dlg=systemtype", data: "", title: "选择反馈类别", width: 650, height: 400, draggable: true },
            function (data, o) {
                if (data) {
                    $("[localform=SysID]").val(data[0].SysID);
                    $("[localform=SystemType]").textbox("setValue", data[0].SystemType);
                }
            });
    }


}

function SelectModuleType() {
    //var type = $("[localform='SystemType']").textbox('getValue');
    var type = $("[localform='SysID']").val();
    if (type == "" || type == null) {
        alert("请先选择系统类别！");
    }
    else {
        nnfDialogShow({ href: "../../NNFext/Dialog.aspx?dlg=moduletype" + "&systemtype=" + type, data: "", title: "选择反馈详情", width: 700, height: 400, draggable: true },
            function (data, o) {
                if (data) {
                    $("[localform=ModuleType]").textbox("setValue", data[0].ModuleType);
                }
            });
        //var ModuleType = $("[localform='ModuleType']").textbox('getValue');
        //if (ModuleType != "" && ModuleType != null) {
        //    if (confirm("您已选择过功能模块类别,若选择新的类别将清空原有数据,请确认是否继续！")) {
        //        nnfDialogShow({ href: "../../NNFext/Dialog.aspx?dlg=moduletype" + "&systemtype=" + type, data: "", title: "选择模块类别", width: 600, height: 400, draggable: true },
        //            function (data, o) {
        //                if (data) {
        //                    $("[localform=ModuleType]").textbox("setValue", data[0].ModuleType);
        //                }
        //            });
        //        $("[localform='ModuleType']").textbox({
        //            onChange: function () {
        //                $("[localform='BillType']").textbox("setValue", "");
        //            }
        //        })
        //    }
        //}
        //else {
        //    nnfDialogShow({ href: "../../NNFext/Dialog.aspx?dlg=moduletype" + "&systemtype=" + type, data: "", title: "选择模块类别", width: 600, height: 400, draggable: true },
        //        function (data, o) {
        //            if (data) {
        //                $("[localform=ModuleType]").textbox("setValue", data[0].ModuleType);
        //            }
        //        });
        //}

    }

}
function SelectBillType() {
    var type = $("[localform='ModuleType']").textbox('getValue');
    if (type == "" || type == null) {
        alert("请先选择模块类别！");
    }
    else {
        nnfDialogShow({ href: "../../NNFext/Dialog.aspx?dlg=billtype" + "&moduletype=" + type, data: "", title: "选择具体类别", width: 600, height: 400, draggable: true },
            function (data, o) {
                if (data) {
                    $("[localform=BillType]").textbox("setValue", data[0].BillType);
                }
            });
    }
}

//判断处理方式
function SetHandleType() {
    var type = $("[localform='HandleType']").combobox('getValue');
    if (type != null && type != "") {
        if (type == "直接回复") {
            $("#HandoverInfo").hide()
            $("#HandoverInfo").localform("disable");
            $("[localform='HandleExplain']").textbox({ required: true, disabled: false })
            $("[localform='Handover']").textbox("setValue", "");
            $("[localform='HandoverNo'],[localform='HandoverID']").val("");
        }
        else {
            $("#HandoverInfo").show();
            //$("#HandoverInfo").localform("enable");
            $("[localform='Handover']").textbox({ required: true, disabled: false, editable: false })
            $("[localform='HandleExplain']").textbox({ required: false, disabled: true })
            $("[localform='HandleExplain']").textbox("setValue", "");
        }
    }
}

//获取后续对接人
function SelectHandover() {
    nnfDialogShow({ href: "../../NNFext/Dialog.aspx?dlg=systemdjr", data: "", title: "选择IT对接人", width: 650, height: 400, draggable: true },
        function (data, o) {
            if (data) {
                $("[localform='Handover']").textbox("setValue", data[0].UserName);
                $("[localform='HandoverID']").val(data[0].UserID);
                $("[localform='HandoverNo']").val(data[0].UserNo);
            }
        });
    //selopen2("", "0-1-1-0", function (result, isCanceled) {
    //    if (!isCanceled && result) {
    //        $("[localform='Handover']").textbox("setValue", result.NameStr2);
    //        $("[localform='HandoverID']").val(result.IDStr);
    //        $("[localform='HandoverNo']").val(result.ShortName);
    //    }
    //});
}

//获取开发协助人
function SelectDeveloper() {
    nnfDialogShow({ href: "../../NNFext/Dialog.aspx?dlg=systemdjr", data: "", title: "选择IT对接人", width: 650, height: 400, draggable: true },
        function (data, o) {
            if (data) {
                $("[localform='Developer']").textbox("setValue", data[0].UserName);
                $("[localform='DeveloperID']").val(data[0].UserID);
                $("[localform='DeveloperNo']").val(data[0].UserNo);
            }
        });
}

function preSubmit(core, isSend) {
    var appResult = window.nnfGetCurrentOption ? window.nnfGetCurrentOption() : 1;
    if (window.isJiaZhuanQian || appResult == 5 || appResult == 6 || appResult == 2 || appResult == 4 || appResult == 3) {
        core();
        return;
    }
    if (isSend) {
        if (!$("#Info,#Info2,#Info3").localform("validate")) {
            return;
        }
    }
    $.extend(theObjE.G, $("#Info,#Info2,#Info3").localform("save"));
    core();//真正保存数据到服务器的代码
}

