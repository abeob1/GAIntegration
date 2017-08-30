Module modSalesOrder

    Private oButton As SAPbouiCOM.Button
    Private oStatic As SAPbouiCOM.StaticText
    Private oEdit As SAPbouiCOM.EditText
    Private oCombo As SAPbouiCOM.ComboBox
    Private oMatrix As SAPbouiCOM.Matrix
    Private oRecordSet As SAPbobsCOM.Recordset

#Region "Add Button"
    Private Sub FormModification(ByVal oForm As SAPbouiCOM.Form)
        Dim sFuncName As String = "FormModification"
        Dim sErrDesc As String = String.Empty
        Try
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            Dim oItem, objItem As SAPbouiCOM.Item
            oItem = oForm.Items.Add("btnWoDet", SAPbouiCOM.BoFormItemTypes.it_BUTTON)
            objItem = oForm.Items.Item("2")
            oItem.Width = objItem.Width
            oItem.Height = objItem.Height
            oItem.Top = objItem.Top
            oItem.Left = objItem.Left + objItem.Width + 10

            oButton = oItem.Specific
            oButton.Caption = "WO Details"

            oItem = oForm.Items.Add("etWoDocNo", SAPbouiCOM.BoFormItemTypes.it_EDIT)
            objItem = oForm.Items.Item("222")
            oItem.Width = objItem.Width
            oItem.Height = objItem.Height
            oItem.Top = objItem.Top + 15
            oItem.Left = objItem.Left
            oItem.Enabled = False

            oEdit = oItem.Specific
            oEdit.DataBind.SetBound(True, "ORDR", "U_WODOCNUM")

            oItem = oForm.Items.Add("stWoDocNo", SAPbouiCOM.BoFormItemTypes.it_STATIC)
            objItem = oForm.Items.Item("230")
            oItem.Width = objItem.Width
            oItem.Height = objItem.Height
            oItem.Top = objItem.Top + 15
            oItem.Left = objItem.Left

            oStatic = oItem.Specific
            oStatic.Caption = "Work Order Details"

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
        End Try

    End Sub
#End Region
#Region "Open Work Order Details"
    Private Sub OpenWorkOrderDetails(ByVal objForm As SAPbouiCOM.Form)
        Dim iCount As Integer
        Dim sDocNum, sSeries, sSORefNo, sSql, sWODetlNo, sSOStatus As String
        oEdit = objForm.Items.Item("8").Specific
        sDocNum = oEdit.Value
        oCombo = objForm.Items.Item("88").Specific
        sSeries = oCombo.Selected.Value
        oEdit = objForm.Items.Item("14").Specific
        sSORefNo = oEdit.Value
        oCombo = objForm.Items.Item("81").Specific
        sSOStatus = oCombo.Selected.Value

        sSql = "SELECT COUNT(*)MNO , U_DOCNUM FROM [@AE_WODET] WHERE U_SODOCNUM = '" & sDocNum & "' AND U_SOSERIES = '" & sSeries & "' AND ISNULL(U_SOREFNO,'') = '" & sSORefNo & "' " & _
               " GROUP BY U_DOCNUM "
        oRecordSet = p_oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRecordSet.DoQuery(sSql)
        If oRecordSet.RecordCount > 0 Then
            iCount = oRecordSet.Fields.Item("MNO").Value
            sWODetlNo = oRecordSet.Fields.Item("U_DOCNUM").Value
        End If
        System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet)


        If iCount > 0 Then
            OpenWoDetlFormFindMode(objForm, sWODetlNo, sSOStatus)
        Else
            OpenWOForm(objForm)
        End If

    End Sub
#End Region
#Region "Load Work Order Details Form"
    Private Sub OpenWOForm(ByVal objForm As SAPbouiCOM.Form)
        Dim sFuncName As String = "OpenWOForm"
        Dim oForm As SAPbouiCOM.Form
        Dim sErrDesc As String = String.Empty
        Dim sSQL As String = String.Empty
        Dim sDocNum As String = String.Empty
        Dim sSOSeries As String = String.Empty
        Dim sSORefNo As String = String.Empty

        Try
            LoadFromXML("WorkOrderDetails.srf", p_oSBOApplication)
            oForm = p_oSBOApplication.Forms.Item("WODET")
            oForm.Visible = True
            oForm.Freeze(True)
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
            oForm.EnableMenu("6913", False) 'User Defined windows
            oForm.EnableMenu("1290", False) 'Move First Record
            oForm.EnableMenu("1288", False) 'Move Next Record
            oForm.EnableMenu("1289", False) 'Move Previous Record
            oForm.EnableMenu("1291", False) 'Move Last Record
            oForm.EnableMenu("1281", False) 'Find Record
            oForm.EnableMenu("1282", False) 'Add New Record
            oForm.EnableMenu("1292", False) 'Add New Row

            oForm.DataBrowser.BrowseBy = "8"

            sSQL = "SELECT ISNULL(MAX(U_DOCNUM),0) + 1 [DOCNUM] FROM [@AE_WODET]"
            oRecordSet = p_oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRecordSet.DoQuery(sSQL)
            If oRecordSet.RecordCount > 0 Then
                oEdit = oForm.Items.Item("8").Specific
                oEdit.Value = oRecordSet.Fields.Item("DOCNUM").Value
            End If
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet)

            oEdit = objForm.Items.Item("8").Specific
            sDocNum = oEdit.Value
            oCombo = objForm.Items.Item("88").Specific
            sSOSeries = oCombo.Selected.Value
            oEdit = objForm.Items.Item("14").Specific
            sSORefNo = oEdit.Value

            oEdit = oForm.Items.Item("4").Specific
            oEdit.Value = sDocNum
            oEdit = oForm.Items.Item("6").Specific
            oEdit.Value = sSORefNo
            oEdit = oForm.Items.Item("10").Specific
            oEdit.Value = sSOSeries

            LoadWorkOrderMatrix(objForm, oForm)

            oMatrix = oForm.Items.Item("11").Specific
            oMatrix.AutoResizeColumns()

            oForm.Freeze(False)
            oForm.Update()
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
        End Try
    End Sub
#End Region
#Region "Load Work order Details Matrix"
    Private Sub LoadWorkOrderMatrix(ByVal oSOForm As SAPbouiCOM.Form, ByVal oForm As SAPbouiCOM.Form)
        Dim sFuncName As String = "LoadWorkOrderMatrix"
        Dim sErrDesc As String = String.Empty
        Dim objMatrix As SAPbouiCOM.Matrix

        Try
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            oMatrix = oForm.Items.Item("11").Specific
            oMatrix.Clear()

            objMatrix = oSOForm.Items.Item("38").Specific
            If objMatrix.VisualRowCount > 0 Then
                For i As Integer = 1 To objMatrix.VisualRowCount
                    If objMatrix.Columns.Item("1").Cells.Item(i).Specific.value <> "" Then
                        oMatrix.AddRow()
                        oMatrix.Columns.Item("V_-1").Cells.Item(i).Specific.value = i
                        oMatrix.Columns.Item("V_6").Cells.Item(i).Specific.value = objMatrix.Columns.Item("110").Cells.Item(i).Specific.value
                        oMatrix.Columns.Item("V_5").Cells.Item(i).Specific.value = objMatrix.Columns.Item("1").Cells.Item(i).Specific.value
                        oMatrix.Columns.Item("V_4").Cells.Item(i).Specific.value = objMatrix.Columns.Item("U_Jobno").Cells.Item(i).Specific.value
                        oMatrix.Columns.Item("V_3").Cells.Item(i).Specific.value = objMatrix.Columns.Item("U_Drawingno").Cells.Item(i).Specific.value
                    End If
                Next
            End If

            oForm.Items.Item("12").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("4").Enabled = False
            oForm.Items.Item("6").Enabled = False
            oForm.Items.Item("8").Enabled = False
            oForm.Items.Item("10").Enabled = False
            oForm.Items.Item("7").Visible = False
            oForm.Items.Item("8").Visible = False
            oForm.Items.Item("9").Visible = False
            oForm.Items.Item("10").Visible = False

            oMatrix.Columns.Item("V_6").Editable = False
            oMatrix.Columns.Item("V_6").Visible = False
            oMatrix.Columns.Item("V_5").Editable = False
            oMatrix.Columns.Item("V_4").Editable = False
            oMatrix.Columns.Item("V_3").Editable = False
            oMatrix.AutoResizeColumns()

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With SUCCESS", sFuncName)
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With ERROR", sFuncName)
        End Try
    End Sub
#End Region
#Region "Delete Sub Forms Data"
    Private Sub DeleteSubFormData(ByVal objForm As SAPbouiCOM.Form)
        Dim sFuncName As String = "DeleteSubFormData"
        Dim sErrDesc As String = String.Empty
        Dim sSQL As String = String.Empty

        Try
            Dim sDocNum, sSeries, sSORefNo, sWODetlNo As String

            oEdit = objForm.Items.Item("8").Specific
            sDocNum = oEdit.Value
            oCombo = objForm.Items.Item("88").Specific
            sSeries = oCombo.Selected.Value
            oEdit = objForm.Items.Item("14").Specific
            sSORefNo = oEdit.Value
            oEdit = objForm.Items.Item("etWoDocNo").Specific
            sWODetlNo = oEdit.Value

            oRecordSet = p_oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            sSQL = "DELETE FROM [@AE_WODET1] WHERE DocEntry = (SELECT DocEntry FROM [@AE_WODET] WHERE U_DOCNUM = '" & sWODetlNo & "' " & _
                   " AND U_SODOCNUM = '" & sDocNum & "' AND U_SOSERIES = '" & sSeries & "' AND U_SOREFNO = '" & sSORefNo & "') "
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Executing Query " & sSQL, sFuncName)
            oRecordSet.DoQuery(sSQL)

            sSQL = "DELETE FROM [@AE_WODET] WHERE U_DOCNUM = '" & sWODetlNo & "' " & _
                   " AND U_SODOCNUM = '" & sDocNum & "' AND U_SOSERIES = '" & sSeries & "' AND U_SOREFNO = '" & sSORefNo & "' "
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Executing Query " & sSQL, sFuncName)
            oRecordSet.DoQuery(sSQL)

            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
        End Try

    End Sub
#End Region

#Region "Item Event"
    Public Sub SalesOrder_SBO_ItemEvent(ByVal FormUID As String, ByVal pval As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean, ByVal objForm As SAPbouiCOM.Form)
        Dim sFuncName As String = "BP_SBO_ItemEvent"
        Dim sErrDesc As String = String.Empty
        Try
            If pval.Before_Action = True Then
                Select Case pval.EventType
                    Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                        objForm = p_oSBOApplication.Forms.GetForm(pval.FormTypeEx, pval.FormTypeCount)
                        If pval.ItemUID = "btnWoDet" Then
                            oMatrix = objForm.Items.Item("38").Specific
                            If oMatrix.VisualRowCount > 1 Then
                                If oMatrix.Columns.Item("1").Cells.Item(1).Specific.value = "" Then
                                    p_oSBOApplication.StatusBar.SetText("Atleast one item should enter in Sales order", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    BubbleEvent = False
                                    Exit Sub
                                Else
                                    OpenWorkOrderDetails(objForm)
                                End If
                            End If
                        ElseIf pval.ItemUID = "2" Then
                            If objForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                DeleteSubFormData(objForm)
                            End If
                        End If

                End Select
            Else
                Select Case pval.EventType
                    Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                        objForm = p_oSBOApplication.Forms.GetForm(pval.FormTypeEx, pval.FormTypeCount)
                        FormModification(objForm)

                    Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                        objForm = p_oSBOApplication.Forms.GetForm(pval.FormTypeEx, pval.FormTypeCount)
                        If pval.ItemUID = "1" Then
                            If pval.Action_Success = True Then
                                'DelUncheckValues(objForm)
                            End If
                        End If


                End Select
            End If
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Throw New ArgumentException(sErrDesc)
        End Try
    End Sub
#End Region
#Region "Menu Event"
    Public Sub SalesOrder_SBO_MenuEvent(ByVal pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Dim sFuncName As String = "SalesOrder_SBO_MenuEvent"
        Dim sErrDesc As String = String.Empty

        Try
            If pVal.BeforeAction = False Then
                Dim objForm As SAPbouiCOM.Form
                If pVal.MenuUID = "1287" Then
                    objForm = p_oSBOApplication.Forms.Item(p_oSBOApplication.Forms.ActiveForm.UniqueID)
                    objForm.Items.Item("etWoDocNo").Specific.value = ""
                    objForm.Items.Item("16").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    objForm.Items.Item("etWoDocNo").Enabled = False
                Else
                    objForm = p_oSBOApplication.Forms.Item(p_oSBOApplication.Forms.ActiveForm.UniqueID)
                    objForm.Items.Item("16").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    objForm.Items.Item("etWoDocNo").Enabled = False
                End If
            End If
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With ERROR", sFuncName)
        End Try
    End Sub
#End Region

End Module
