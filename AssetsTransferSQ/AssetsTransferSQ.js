function formLoaded(isReadOnly, theObjE, task, isFirstStep, isIncidentExists) {
    //资产所属公司下拉框的值
    $("[localform='CompanyID,CompanyName']").combobox({
        data: theObjE.Company,
        valueField: 'Value',
        textField: 'Text',
        onSelect: function (row) {
            $("[localform=DBName]").val(row.DBName);
            //$("[localform=CompanyID]").val(row.Value);
            //$("[localform=CompanyName]").textbox("setValue", row.Text);
            
        },
        onChange: function (x) {

        },
        

    });
    
    var editMode = 0; //默认不可编辑   
    if (isFirstStep && !isReadOnly)//表单申请节点并且控件不是只读，那么设置为可编辑
    {
        editMode = 1; //可编辑

        $("#Info,#Info2,#Info3").localform("load", theObjE.G);

    } else {
        editMode = 0;
        $("#Info,#Info2,#Info3").localform("disable");}
    if (task.StepName == "财务部门" && !isReadOnly) {
            editMode = 0; //不可编辑

            $("[localform=AssetNumber]").textbox("enable");
            $("[localform=BuyTime]").textbox("enable");
            $("[localform=ZJNX]").textbox("enable");
            $("[localform=BeforeValue]").textbox("enable");
            $("[localform=NowValue]").textbox("enable");
            $("[localform=Note]").textbox("enable");
    }


    $("#Info,#Info2,#Info3").localform("load", theObjE.G);

}


//选择资产类别
function SelectAssetType() {
   
    var DBNAME = $("[localform=DBName]").val();

        nnfDialogShow({
            href: "../../NNFext/Dialog.aspx?" + "dlg=selAssetTypeID&DBName=" + DBNAME, data: "", title: "选择", width: 600, height: 400, draggable: true
        }, function (data, o) {
            if (data) {
                $("[localform=AssetType]").textbox("setValue", data[0].FName);

            }
        });
}


function preSubmit(core, isSend) {

    var appresult = window.nnfGetCurrentOption ? window.nnfGetCurrentOption() : 1;
    if (appresult == 1) {
        if (isSend) {
            if (!$("#Info,#Info2,#Info3").localform("validate")) {
                return;
            }
        }
        $.extend(theObjE.G, $("#Info,#Info2,#Info3").localform("save"));
        core();//真正保存数据到服务器的代码
    } else {
        $.extend(theObjE.G, $("#Info,#Info2,#Info3").localform("save"));
        core();//真正保存数据到服务器的代码
    }
}




//选择原部门
function SelectDeptOld() {
    selopen2($("[localform=DepartmentNameOld]").val(), "0-1-3-A01", function (result, isCanceled) {
        if (!isCanceled && result) {
            $("[localform=DepartmentNameOld]").textbox("setValue", result.ShortName); //部门名称
            $("[localform=DepartmentIDOld]").val(result.IDStr);//部门ID
        }
    });
}
//选择拟转入部门
function SelectDeptNew() {
    selopen2($("[localform=DepartmentNameNew]").val(), "0-1-3-A01", function (result, isCanceled) {
        if (!isCanceled && result) {
            $("[localform=DepartmentNameNew]").textbox("setValue", result.ShortName); //部门名称
            $("[localform=DepartmentIDNew]").val(result.IDStr);//部门ID
        }
    });
}

