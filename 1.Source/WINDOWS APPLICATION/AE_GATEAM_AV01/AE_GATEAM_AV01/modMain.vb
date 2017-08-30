Module modMain

    Public p_oApps As SAPbouiCOM.SboGuiApi
    Public p_oEventHandler As clsEventHandler
    Public WithEvents p_oSBOApplication As SAPbouiCOM.Application
    Public p_oDICompany As SAPbobsCOM.Company
    Public p_oUICompany As SAPbouiCOM.Company
    Public sFuncName As String
    Public sErrDesc As String


    Public p_iDebugMode As Int16
    Public p_iErrDispMethod As Int16
    Public p_iDeleteDebugLog As Int16
    Public Const RTN_SUCCESS As Int16 = 1
    Public Const RTN_ERROR As Int16 = 0

    Public Const DEBUG_ON As Int16 = 1
    Public Const DEBUG_OFF As Int16 = 0

    Public Const ERR_DISPLAY_STATUS As Int16 = 1
    Public Const ERR_DISPLAY_DIALOGUE As Int16 = 2

    <STAThread()>
    Sub Main(ByVal args() As String)

        ''Dim oApp As Application
        Dim sconn As String = String.Empty
        ''If (args.Length < 1) Then
        ''    oApp = New Application
        ''Else
        ''    oApp = New Application(args(0))
        ''End If

        sFuncName = "Main()"
        Try
            p_iDebugMode = DEBUG_ON
            p_iErrDispMethod = ERR_DISPLAY_STATUS

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Addon startup function", sFuncName)
            p_oApps = New SAPbouiCOM.SboGuiApi
            'sconn = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056"
            'p_oApps.Connect(args(0))
            p_oApps.Connect(args(0))

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Initializing public SBO Application object", sFuncName)
            p_oSBOApplication = p_oApps.GetApplication

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Retriving SBO application company handle", sFuncName)
            p_oUICompany = p_oSBOApplication.Company


            p_oDICompany = New SAPbobsCOM.Company
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Retrived SBO application company handle", sFuncName)
            If Not p_oDICompany.Connected Then
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectDICompSSO()", sFuncName)
                If ConnectDICompSSO(p_oDICompany, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
            End If

            'Call WriteToLogFile_Debug("Calling DisplayStatus()", sFuncName)
            'Call DisplayStatus(Nothing, "Addon starting.....please wait....", sErrDesc)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Creating Event handler class", sFuncName)
            p_oEventHandler = New clsEventHandler(p_oSBOApplication, p_oDICompany)

            'If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling AddMenuItems()", sFuncName)
            'p_oEventHandler.AddMenuItems()

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling SetApplication Function", sFuncName)
            ' Call p_oEventHandler.SetApplication(sErrDesc)

            CreateTable()
            RegisterUDO()

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Addon started successfully", "Main()")

            p_oSBOApplication.StatusBar.SetText("Addon Started Successfully", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            System.Windows.Forms.Application.Run()

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
        End Try


    End Sub

    Private Sub CreateTable()
        Try

            'CREATE UDF FOR SALES ORDER SCREEN
            addField("ORDR", "WODOCNUM", "WORK ORDER DOCNUM", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "")

            'CREATE UDF FOR SALES ORDER DETAILS
            addField("RDR1", "WOCOST", "Work Order Cost", SAPbobsCOM.BoFieldTypes.db_Float, 100, SAPbobsCOM.BoFldSubTypes.st_Price, "", "", "")
            addField("RDR1", "WOQTY", "Work Order Qty", SAPbobsCOM.BoFieldTypes.db_Float, 100, SAPbobsCOM.BoFldSubTypes.st_Quantity, "", "", "")
            addField("RDR1", "WOHRS", "Work Order Hours", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Time, "", "", "")

            'CREATE UDO TABLE FOR TOOLS CATEGORY SELECTION AND LINE
            CreateUDOTable("AE_WODET", "WORK ORDER DETAILS", SAPbobsCOM.BoUTBTableType.bott_Document)
            addField("@AE_WODET", "DOCNUM", "DOCUMENT NUMBER", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "")
            addField("@AE_WODET", "SODOCNUM", "SO DOCNUM", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "")
            addField("@AE_WODET", "SOSERIES", "SO SERIES CODE", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "")
            addField("@AE_WODET", "SOREFNO", "SO REF NO", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "")
            
            CreateUDOTable("AE_WODET1", "WORK ORDER LINE", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            addField("@AE_WODET1", "BASELINE", "BASE LINE NO", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "")
            addField("@AE_WODET1", "ITEMCODE", "ITEM CODE", SAPbobsCOM.BoFieldTypes.db_Alpha, 40, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "")
            addField("@AE_WODET1", "JOBNO", "JOB NO", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "")
            addField("@AE_WODET1", "DRAWINGNO", "DRAWING NO", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "")
            addField("@AE_WODET1", "WOCOST", "WORK ORDER COST", SAPbobsCOM.BoFieldTypes.db_Float, 100, SAPbobsCOM.BoFldSubTypes.st_Price, "", "", "")
            addField("@AE_WODET1", "WOQTY", "WORK ORDER QTY", SAPbobsCOM.BoFieldTypes.db_Float, 100, SAPbobsCOM.BoFldSubTypes.st_Quantity, "", "", "")
            addField("@AE_WODET1", "WOHRS", "WORK ORDER HOURS", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Time, "", "", "")

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Throw New ArgumentException(sErrDesc)
        End Try
    End Sub

    Private Function RegisterUDO() As Boolean
        sFuncName = "RegisterUDO()"
        sErrDesc = String.Empty

        Try
            If Not (CreateUDODocumentChild("UDO_WODET", "WORK ORDER DETAILS", "AE_WODET", "AE_WODET1", 1, "U_DOCNUM", "U_SODOCNUM")) Then Return False

            Return True

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Throw New ArgumentException(sErrDesc)
        End Try
    End Function

End Module
