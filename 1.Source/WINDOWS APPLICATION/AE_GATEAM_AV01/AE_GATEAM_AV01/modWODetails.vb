Module modWODetails

    Private objForm, oForm As SAPbouiCOM.Form
    Private oEdit As SAPbouiCOM.EditText
    Private oMatrix As SAPbouiCOM.Matrix
    Private oCheck As SAPbouiCOM.CheckBox
    Private sSQL As String
    Private oRecordSet As SAPbobsCOM.Recordset
    Private sDocNum As String

#Region "Open Work Order Details Form in Find Mode"
    Public Sub OpenWoDetlFormFindMode(ByVal objForm As SAPbouiCOM.Form, ByVal sWoDetlDocNo As String, ByVal sSOFormStatus As String)
        Dim sFuncName As String = "OpenWoDetlFormFindMode"
        Dim oForm As SAPbouiCOM.Form
        Dim sErrDesc As String = String.Empty

        Try
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

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

            oMatrix = oForm.Items.Item("11").Specific
            oMatrix.AutoResizeColumns()

            oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
            oEdit = oForm.Items.Item("8").Specific
            oEdit.Value = sWoDetlDocNo
            sDocNum = sWoDetlDocNo

            oEdit = oForm.Items.Item("12").Specific
            oEdit.Value = sSOFormStatus

            oForm.Items.Item("12").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)

            LoadDatas(objForm, oForm, sSOFormStatus)

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

            oForm.Update()
            oForm.Freeze(False)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
        End Try
    End Sub
#End Region
#Region "Load Datas"
    Private Sub LoadDatas(ByVal oSOForm As SAPbouiCOM.Form, ByVal oForm As SAPbouiCOM.Form, ByVal sSOFormStatus As String)
        Dim sFuncName As String = "LoadDatas"
        Dim objMatrix As SAPbouiCOM.Matrix

        Try
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            oMatrix = oForm.Items.Item("11").Specific
            objMatrix = oSOForm.Items.Item("38").Specific

            Dim sWOLine, sWOItemCode As String
            Dim sSOLine, sSOItemCode As String
            Dim bLineExists As Boolean = False

a:
            For i As Integer = 1 To oMatrix.RowCount
                sWOLine = oMatrix.Columns.Item("V_6").Cells.Item(i).Specific.value
                sWOItemCode = oMatrix.Columns.Item("V_5").Cells.Item(i).Specific.value

                For j As Integer = 1 To objMatrix.VisualRowCount
                    sSOLine = objMatrix.Columns.Item("110").Cells.Item(j).Specific.value
                    sSOItemCode = objMatrix.Columns.Item("1").Cells.Item(j).Specific.value
                    If sSOItemCode <> "" Then
                        If sSOLine = sWOLine And sSOItemCode = sWOItemCode Then
                            bLineExists = True
                            Exit For
                        Else
                            bLineExists = False
                        End If
                    End If
                Next

                If bLineExists = False Then
                    oMatrix.DeleteRow(i)

                    oEdit = oForm.Items.Item("12").Specific
                    oEdit.Value = sSOFormStatus

                    GoTo a
                End If
            Next

            For i As Integer = 1 To objMatrix.VisualRowCount
                sSOLine = objMatrix.Columns.Item("110").Cells.Item(i).Specific.value
                sSOItemCode = objMatrix.Columns.Item("1").Cells.Item(i).Specific.value
                If sSOItemCode <> "" Then
                    For j As Integer = 1 To oMatrix.RowCount
                        sWOLine = oMatrix.Columns.Item("V_6").Cells.Item(j).Specific.value
                        sWOItemCode = oMatrix.Columns.Item("V_5").Cells.Item(j).Specific.value

                        If sSOLine = sWOLine And sSOItemCode = sWOItemCode Then
                            bLineExists = True
                            Exit For
                        Else
                            bLineExists = False
                        End If
                    Next

                    If bLineExists = False Then
                        oMatrix.AddRow()
                        oMatrix.Columns.Item("V_-1").Cells.Item(oMatrix.RowCount).Specific.value = i
                        oMatrix.Columns.Item("V_6").Cells.Item(oMatrix.RowCount).Specific.value = objMatrix.Columns.Item("110").Cells.Item(i).Specific.value
                        oMatrix.Columns.Item("V_5").Cells.Item(oMatrix.RowCount).Specific.value = objMatrix.Columns.Item("1").Cells.Item(i).Specific.value
                        
                        oMatrix.Columns.Item("V_4").Cells.Item(oMatrix.RowCount).Specific.value = objMatrix.Columns.Item("U_Jobno").Cells.Item(i).Specific.value
                        oMatrix.Columns.Item("V_3").Cells.Item(oMatrix.RowCount).Specific.value = objMatrix.Columns.Item("U_Drawingno").Cells.Item(i).Specific.value

                        oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific.value = 0.0
                        oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific.value = 0.0
                        oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific.value = ""


                        oEdit = oForm.Items.Item("12").Specific
                        oEdit.Value = sSOFormStatus

                    End If
                End If
            Next

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
        End Try
    End Sub
#End Region
#Region "Update Rows in Sales Order"
    Private Sub UpdateRows(ByVal objForm As SAPbouiCOM.Form, ByVal oSOForm As SAPbouiCOM.Form, ByVal iFormCount As Integer)
        Dim sFuncName As String = "UpdateRows"
        Dim sErrDesc As String = String.Empty
        Dim oSOMatrix As SAPbouiCOM.Matrix
        Dim sSOFormStatus As String = String.Empty

        Try
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            Dim sLine, sSOITem, sItemCode, sSOline As String
            Dim sSODocNum, sSOSeries As String

            oEdit = objForm.Items.Item("12").Specific
            sSOFormStatus = oEdit.Value
            oEdit = objForm.Items.Item("4").Specific
            sSODocNum = oEdit.Value
            oEdit = objForm.Items.Item("10").Specific
            sSOSeries = oEdit.Value

            oSOForm.Freeze(True)

            If sSOFormStatus = "3" Or sSOFormStatus = "4" Then
                oMatrix = objForm.Items.Item("11").Specific
                oSOMatrix = oSOForm.Items.Item("38").Specific

                oRecordSet = p_oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                For i As Integer = 1 To oMatrix.RowCount
                    sLine = oMatrix.Columns.Item("V_6").Cells.Item(i).Specific.value
                    sItemCode = oMatrix.Columns.Item("V_5").Cells.Item(i).Specific.value
                    If sItemCode <> "" Then

                        Dim sWOHours As String
                        Dim dWoCost, dWOQty As Double
                        dWoCost = oMatrix.Columns.Item("V_2").Cells.Item(i).Specific.value
                        dWOQty = oMatrix.Columns.Item("V_1").Cells.Item(i).Specific.value
                        sWOHours = oMatrix.Columns.Item("V_0").Cells.Item(i).Specific.value

                        sSQL = "UPDATE RDR1 SET U_WOCOST = '" & dWoCost & "',U_WOQTY = '" & dWOQty & "', U_WOHRS = '" & sWOHours & "' " & _
                               " WHERE DocEntry = (SELECT DocEntry FROM ORDR WHERE DocNum = '" & sSODocNum & "' AND Series = '" & sSOSeries & "') " & _
                               " AND LineNum = '" & sLine & "' AND ItemCode = '" & sItemCode & "'"
                        oRecordSet.DoQuery(sSQL)

                    End If
                Next
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet)
            Else
                oMatrix = objForm.Items.Item("11").Specific
                oSOMatrix = oSOForm.Items.Item("38").Specific

                For i As Integer = 1 To oMatrix.RowCount
                    sLine = oMatrix.Columns.Item("V_6").Cells.Item(i).Specific.value
                    sItemCode = oMatrix.Columns.Item("V_5").Cells.Item(i).Specific.value
                    If sItemCode <> "" Then

                        For j As Integer = 1 To oSOMatrix.VisualRowCount
                            sSOline = oSOMatrix.Columns.Item("110").Cells.Item(j).Specific.value
                            sSOITem = oSOMatrix.Columns.Item("1").Cells.Item(j).Specific.value
                            If sSOITem <> "" Then
                                If sSOITem = sItemCode And sSOline = sLine Then
                                    oSOMatrix.Columns.Item("U_WOCOST").Cells.Item(j).Specific.value = oMatrix.Columns.Item("V_2").Cells.Item(i).Specific.value
                                    oSOMatrix.Columns.Item("U_WOQTY").Cells.Item(j).Specific.value = oMatrix.Columns.Item("V_1").Cells.Item(i).Specific.value
                                    oSOMatrix.Columns.Item("U_WOHRS").Cells.Item(j).Specific.value = oMatrix.Columns.Item("V_0").Cells.Item(i).Specific.value
                                    Exit For
                                End If
                            End If
                        Next

                    End If
                Next
            End If

            oSOForm.Freeze(False)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
        Catch ex As Exception
            oSOForm.Freeze(False)
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
        End Try
    End Sub
#End Region

#Region "Item Event"
    Public Sub ToolsCate_SBO_ItemEvent(ByVal FormUID As String, ByVal pval As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean, ByVal objForm As SAPbouiCOM.Form)
        Dim sFuncName As String = "BP_SBO_ItemEvent"
        Dim sErrDesc As String = String.Empty
        Try
            If pval.Before_Action = True Then
                Select Case pval.EventType
                    Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                        objForm = p_oSBOApplication.Forms.GetForm(pval.FormTypeEx, pval.FormTypeCount)
                        If pval.CharPressed = "9" Then

                        End If

                    Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                        objForm = p_oSBOApplication.Forms.GetForm(pval.FormTypeEx, pval.FormTypeCount)
                        If pval.ItemUID = "1" Then
                            If objForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                oEdit = objForm.Items.Item("8").Specific
                                sDocNum = oEdit.Value
                                oForm = p_oSBOApplication.Forms.GetForm("139", pval.FormTypeCount)
                                oEdit = oForm.Items.Item("etWoDocNo").Specific
                                oEdit.Value = sDocNum
                                UpdateRows(objForm, oForm, pval.FormTypeCount)
                            ElseIf objForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                oEdit = objForm.Items.Item("8").Specific
                                sDocNum = oEdit.Value
                                oForm = p_oSBOApplication.Forms.GetForm("139", pval.FormTypeCount)
                                oEdit = oForm.Items.Item("etWoDocNo").Specific
                                oEdit.Value = sDocNum
                                UpdateRows(objForm, oForm, pval.FormTypeCount)
                            End If
                        End If

                End Select
            Else
                Select Case pval.EventType
                    Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                        objForm = p_oSBOApplication.Forms.GetForm(pval.FormTypeEx, pval.FormTypeCount)
                        If pval.ItemUID = "1" Then
                            If objForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                oForm = p_oSBOApplication.Forms.GetForm("139", pval.FormTypeCount)
                                oEdit = oForm.Items.Item("etWoDocNo").Specific
                                oEdit.Value = sDocNum
                                objForm.Close()
                            ElseIf objForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                oForm = p_oSBOApplication.Forms.GetForm("139", pval.FormTypeCount)
                                oEdit = oForm.Items.Item("etWoDocNo").Specific
                                oEdit.Value = sDocNum
                                objForm.Close()
                            ElseIf objForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                oForm = p_oSBOApplication.Forms.GetForm("139", pval.FormTypeCount)
                                oEdit = oForm.Items.Item("etWoDocNo").Specific
                                oEdit.Value = sDocNum
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

End Module
