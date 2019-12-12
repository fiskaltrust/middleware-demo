VERSION 5.00
Begin VB.Form sign 
   Caption         =   "Form1"
   ClientHeight    =   6225
   ClientLeft      =   165
   ClientTop       =   555
   ClientWidth     =   14925
   LinkTopic       =   "Form1"
   ScaleHeight     =   6225
   ScaleWidth      =   14925
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton cmdCash 
      Caption         =   "Send cash transaction"
      Height          =   615
      Left            =   2160
      TabIndex        =   13
      Top             =   5160
      Width           =   2535
   End
   Begin VB.CommandButton cmdStart 
      Caption         =   "Send start receipt"
      Height          =   615
      Left            =   5160
      TabIndex        =   12
      Top             =   4080
      Width           =   2535
   End
   Begin VB.Frame Frame5 
      Caption         =   "Country code"
      Height          =   615
      Left            =   360
      TabIndex        =   10
      Top             =   3960
      Width           =   1215
      Begin VB.ComboBox ComboCC 
         Height          =   315
         Left            =   120
         TabIndex        =   11
         Text            =   "DE"
         Top             =   240
         Width           =   975
      End
   End
   Begin VB.CommandButton close 
      Caption         =   "Close"
      Height          =   615
      Left            =   5160
      TabIndex        =   9
      Top             =   5160
      Width           =   2535
   End
   Begin VB.TextBox output 
      Height          =   5775
      Left            =   9120
      Locked          =   -1  'True
      MultiLine       =   -1  'True
      TabIndex        =   8
      Top             =   240
      Width           =   5535
   End
   Begin VB.Frame Frame3 
      Caption         =   "Accesstoken"
      Height          =   615
      Left            =   360
      TabIndex        =   5
      Top             =   3000
      Width           =   8535
      Begin VB.TextBox accesstoken 
         Height          =   285
         Left            =   120
         TabIndex        =   6
         Text            =   "BJCvKkJY9YeLuOerD1tr7tbuixq+5bwcwgC2Yq2zWHVgaCGIlPQBEuOdtOlgSuGd3/02RXEhdDbQW8QO+LY9cPM="
         Top             =   240
         Width           =   8295
      End
   End
   Begin VB.Frame Frame2 
      Caption         =   "CashBoxID"
      Height          =   615
      Left            =   360
      TabIndex        =   3
      Top             =   2160
      Width           =   8535
      Begin VB.TextBox cashboxid 
         Height          =   285
         Left            =   120
         TabIndex        =   4
         Text            =   "862b15b4-85cc-4774-bcc4-f45d6bb90e42"
         Top             =   240
         Width           =   8295
      End
   End
   Begin VB.Frame Frame1 
      Caption         =   "Service URL"
      Height          =   615
      Left            =   360
      TabIndex        =   1
      Top             =   1320
      Width           =   8535
      Begin VB.TextBox URL 
         Height          =   285
         Left            =   120
         TabIndex        =   2
         Text            =   "http://localhost:1200/rest/"
         Top             =   240
         Width           =   8295
      End
   End
   Begin VB.CommandButton cmdZero 
      Caption         =   "Send zero request"
      Height          =   615
      Left            =   2160
      TabIndex        =   0
      Top             =   4080
      Width           =   2535
   End
   Begin VB.Label Label1 
      Caption         =   "This example sends a sign request to a fiskaltrust.service via REST"
      Height          =   735
      Left            =   360
      TabIndex        =   7
      Top             =   360
      Width           =   8535
   End
End
Attribute VB_Name = "sign"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private signCase As Dictionary
Private ChargeItemCase As Dictionary
Private PayItemCase As Dictionary
Private sign As Dictionary


Private WithEvents rest As WinHttp.WinHttpRequest
Attribute rest.VB_VarHelpID = -1

Private Sub close_Click()
    Unload Me
End Sub

Private Function create_receiptcase_dictionary(signCase As Dictionary)
    Dim AT As Dictionary
    Set AT = New Dictionary
    AT.add "unknown", ((&H4154) * (16 ^ 12))
    AT.add "cash_transaction", 1
    AT.add "zero_receipt", 2
    AT.add "start_receipt", 3
    
    Dim DE As Dictionary
    Set DE = New Dictionary
    DE.add "unknown", ((&H4445) * (16 ^ 12))
    DE.add "pos-receipt", 1
    DE.add "zero_receipt", 2
    'DE Flags
    DE.add "implicit", ((&HA0000) * (16 ^ 4))
    
    Dim FR As Dictionary
    Set FR = New Dictionary
    FR.add "unknown", ((&H4652) * (16 ^ 12))
    FR.add "zero_receipt", 15
    FR.add "start_receipt", 16
    
    signCase.add "AT", AT
    signCase.add "DE", DE
    signCase.add "FR", FR
    
End Function

Private Function create_ChargeItemCase_dictionary(ChargeItemCase As Dictionary)
    Dim AT As Dictionary
    Set AT = New Dictionary
    AT.add "unknown", "4707387510509010944" '0x4154 0000 0000 0000
    AT.add "undefined_10", 1
    
    Dim DE As Dictionary
    Set DE = New Dictionary
    DE.add "unknown", "4919338167972134912" '0x4445 0000 0000 0000
    
    Dim FR As Dictionary
    Set FR = New Dictionary
    FR.add "unknown", "506711253E745229312" '0x4652 0000 0000 0000
    
    ChargeItemCase.add "AT", AT
    ChargeItemCase.add "DE", DE
    ChargeItemCase.add "FR", FR
    
End Function

Private Function create_PayItemCase_dictionary(PayItemCase As Dictionary)
    Dim AT As Dictionary
    Set AT = New Dictionary
    AT.add "default", "4707387510509010944" '0x4154 0000 0000 0000
    
    Dim DE As Dictionary
    Set DE = New Dictionary
    DE.add "default", "4919338167972134912" '0x4445 0000 0000 0000
    
    Dim FR As Dictionary
    Set FR = New Dictionary
    FR.add "default", "506711253E745229312" '0x4652 0000 0000 0000
    
    PayItemCase.add "AT", AT
    PayItemCase.add "DE", DE
    PayItemCase.add "FR", FR
    
End Function

Private Sub Form_Load()
    Set rest = New WinHttp.WinHttpRequest
    
    'load colloms for county code selection'
    ComboCC.AddItem "AT"
    ComboCC.AddItem "DE"
    ComboCC.AddItem "FR"
    
    Set signCase = New Dictionary
    create_receiptcase_dictionary signCase
    Set ChargeItemCase = New Dictionary
    create_ChargeItemCase_dictionary ChargeItemCase
    Set PayItemCase = New Dictionary
    create_PayItemCase_dictionary PayItemCase
    
    Dim test As New case_handler
    test.value signCase.Item("DE").Item("unknown"), signCase.Item("DE").Item("zero_receipt")
    test.flag signCase.Item("DE").Item("implicit")
    test.flag 175921860444160#
    MsgBox test.toString
    
    
End Sub

Private Function set_request_parameter()
    'Set Methode and Add Parameter to URL'
    Dim ServiceURL As String
    ServiceURL = Trim(URL.Text)
    If Right(ServiceURL, 1) = "/" Then
        ServiceURL = ServiceURL & "json/sign"
    Else
        ServiceURL = ServiceURL & "/" & "json/sign"
    End If
    
    'set URL and methode'
    rest.Open "POST", ServiceURL, True
    
    'set headers'
    rest.SetRequestHeader "Content-Type", "application/json"
    rest.SetRequestHeader "cashboxid", Trim(cashboxid.Text)
    rest.SetRequestHeader "accesstoken", Trim(accesstoken.Text)
End Function

Private Function init_zero(receipt_case As String) As Dictionary
    Dim sign As New Dictionary
    
    Dim ChargeItem As collection
    Set ChargeItem = New collection
    Dim PayItem As collection
    Set PayItem = New collection
    
    sign.add "ftcashboxid", Trim(cashboxid.Text)
    sign.add "cbTerminalID", "1"
    sign.add "cbReceiptReference", "1"
    sign.add "cbChargeItems", ChargeItem
    sign.add "cbPayItems", PayItem
    sign.add "cbReceiptMoment", Format(Now, "mm/dd/yyyy") & "Z" & Format(Now, "hh:mm:ss")
    sign.add "ftReceiptCase", receipt_case
    
    Set init_zero = sign
End Function

Private Function init_sign(receipt_case As String) As Dictionary
    Dim sign As New Dictionary
    Dim base As New bigDecimal
    Dim flag As New bigDecimal
    
    'set chargeitems
    Dim ChargeItem As Dictionary
    Set ChargeItem = New Dictionary
    ChargeItem.add "Quantity", 10#
    ChargeItem.add "Description", "Food"
    ChargeItem.add "Amount", 5#
    ChargeItem.add "VATRate", 10#
    base.Set_value ChargeItemCase.Item(ComboCC.Text).Item("unknown"), 10
    flag.Set_value CStr(ChargeItemCase.Item(ComboCC.Text).Item("undefined_10")), 10
    base.add flag
    ChargeItem.add "ftChargeItemCase", base.toString
    ChargeItem.add "ProductNumber", "1"
    
    Dim ChargeItems As collection
    Set ChargeItems = New collection
    ChargeItems.add ChargeItem
    
    'set Payitems
    Dim PayItem As Dictionary
    Set PayItem = New Dictionary
    PayItem.add "Quantity", 10#
    PayItem.add "Description", "Cash"
    PayItem.add "Amount", 5#
    PayItem.add "ftPayItemCase", PayItemCase.Item(ComboCC.Text).Item("default")
    
    Dim PayItems As collection
    Set PayItems = New collection
    PayItems.add PayItem
    
    sign.add "ftcashboxid", Trim(cashboxid.Text)
    sign.add "cbTerminalID", "1"
    sign.add "cbReceiptReference", "1"
    sign.add "cbChargeItems", ChargeItems
    sign.add "cbPayItems", PayItems
    sign.add "cbReceiptMoment", Format(Now, "mm/dd/yyyy") & "Z" & Format(Now, "hh:mm:ss")
    sign.add "ftReceiptCase", receipt_case
    
    Set init_sign = sign
End Function

Private Sub cmdZero_Click()
    
    'lock input fields'
    MousePointer = vbHourglass
    URL.Locked = True
    cashboxid.Locked = True
    accesstoken.Locked = True
    ComboCC.Locked = True
    
    'set URL and Header
    set_request_parameter
    
    'set JSON content
    Dim base As New bigDecimal
    Dim flag As New bigDecimal
    base.Set_value signCase.Item(ComboCC.Text).Item("unknown"), 10
    flag.Set_value CStr(signCase.Item(ComboCC.Text).Item("zero_receipt")), 10
    base.add flag
    Set sign = init_zero(base.toString)
    
    'send sign request'
    output.Text = "Request sent" & vbCrLf
    rest.Send JSON.toString(sign)
End Sub

Private Sub cmdStart_Click()
    
    'lock input fields'
    MousePointer = vbHourglass
    URL.Locked = True
    cashboxid.Locked = True
    accesstoken.Locked = True
    ComboCC.Locked = True
    
    'set URL and Header
    set_request_parameter
    
    'set JSON content
    Dim base As New bigDecimal
    Dim flag As New bigDecimal
    base.Set_value signCase.Item(ComboCC.Text).Item("unknown"), 10
    flag.Set_value CStr(signCase.Item(ComboCC.Text).Item("start_receipt")), 10
    base.add flag
    Set sign = init_zero(base.toString)
    
    'send sign request'
    output.Text = "Request sent" & vbCrLf
    rest.Send JSON.toString(sign)
End Sub

Private Sub cmdCash_Click()
    'lock input fields'
    MousePointer = vbHourglass
    URL.Locked = True
    cashboxid.Locked = True
    accesstoken.Locked = True
    ComboCC.Locked = True
    
    'set URL and Header
    set_request_parameter
    
    'set JSON content
    Dim base As New bigDecimal
    Dim flag As New bigDecimal
    base.Set_value signCase.Item(ComboCC.Text).Item("unknown"), 10
    
    
    If ComboCC.Text = "DE" Then
        flag.Set_value CStr(signCase.Item(ComboCC.Text).Item("implicit")), 10
        base.add flag
        Set sign = init_sign(base.toString)  'create Implicite Receipt case
    Else
        flag.Set_value CStr(signCase.Item(ComboCC.Text).Item("cash_transaction")), 10
        base.add flag
        Set sign = init_sign(base.toString)  'create Implicite Receipt case
    End If
    
    'send sign request'
    output.Text = "Request sent" & vbCrLf
    rest.Send JSON.toString(sign)
End Sub

Private Sub rest_OnResponseFinished()
    Dim response As Object
    output.Text = output.Text & "Status " & CStr(rest.Status) & vbCrLf
    If rest.Status = 200 Then
        Set response = JSON.parse(rest.ResponseText)
        'print one object of journal response'
        If Not response Is Nothing And response.Exists("ftState") Then
            output.Text = output.Text & "State: " & response.Item("ftState") & vbCrLf
            For Each Signature In response.Item("ftSignatures")
                If (Signature.Item("ftSignatureType") <> "4707387510509010944") Then
                output.Text = output.Text & "Signature: " & Signature.Item("data") & vbCrLf
                End If
            Next
        Else
            output.Text = output.Text & "Body" & vbCrLf & rest.ResponseText
        End If
    End If
    'destroy sign
    Set sign = Nothing
    
    'unlock input fields'
    MousePointer = vbdeault
    URL.Locked = False
    cashboxid.Locked = False
    accesstoken.Locked = False
    ComboCC.Locked = False
End Sub
