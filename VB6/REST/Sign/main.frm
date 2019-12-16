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
   Begin VB.Frame Frame4 
      Caption         =   "POS System ID"
      Height          =   615
      Left            =   360
      TabIndex        =   15
      Top             =   3480
      Width           =   8535
      Begin VB.TextBox POSSID 
         Height          =   285
         Left            =   120
         TabIndex        =   3
         Top             =   240
         Width           =   8295
      End
   End
   Begin VB.CommandButton cmdCash 
      Caption         =   "Send cash transaction"
      Height          =   615
      Left            =   2160
      TabIndex        =   7
      Top             =   5160
      Width           =   2535
   End
   Begin VB.CommandButton cmdStart 
      Caption         =   "Send start receipt"
      Height          =   615
      Left            =   5160
      TabIndex        =   6
      Top             =   4320
      Width           =   2535
   End
   Begin VB.Frame Frame5 
      Caption         =   "Country code"
      Height          =   615
      Left            =   360
      TabIndex        =   14
      Top             =   4320
      Width           =   1215
      Begin VB.ComboBox ComboCC 
         Height          =   315
         Left            =   120
         TabIndex        =   4
         Top             =   240
         Width           =   975
      End
   End
   Begin VB.CommandButton close 
      Caption         =   "Close"
      Height          =   615
      Left            =   5160
      TabIndex        =   8
      Top             =   5160
      Width           =   2535
   End
   Begin VB.TextBox output 
      Height          =   5775
      Left            =   9120
      Locked          =   -1  'True
      MultiLine       =   -1  'True
      TabIndex        =   13
      Top             =   240
      Width           =   5535
   End
   Begin VB.Frame Frame3 
      Caption         =   "Accesstoken"
      Height          =   615
      Left            =   360
      TabIndex        =   11
      Top             =   2640
      Width           =   8535
      Begin VB.TextBox accesstoken 
         Height          =   285
         Left            =   120
         TabIndex        =   2
         Top             =   240
         Width           =   8295
      End
   End
   Begin VB.Frame Frame2 
      Caption         =   "CashBoxID"
      Height          =   615
      Left            =   360
      TabIndex        =   10
      Top             =   1800
      Width           =   8535
      Begin VB.TextBox cashboxid 
         Height          =   285
         Left            =   120
         TabIndex        =   1
         Top             =   240
         Width           =   8295
      End
   End
   Begin VB.Frame Frame1 
      Caption         =   "Service URL"
      Height          =   615
      Left            =   360
      TabIndex        =   9
      Top             =   960
      Width           =   8535
      Begin VB.TextBox URL 
         Height          =   285
         Left            =   120
         TabIndex        =   0
         Top             =   240
         Width           =   8415
      End
   End
   Begin VB.CommandButton cmdZero 
      Caption         =   "Send zero request"
      Height          =   615
      Left            =   2160
      TabIndex        =   5
      Top             =   4320
      Width           =   2535
   End
   Begin VB.Label Label1 
      Caption         =   "This example sends a sign request to a fiskaltrust.service via REST"
      Height          =   375
      Left            =   360
      TabIndex        =   12
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
Private ftstate As Dictionary
Private sign As Dictionary

Private WithEvents rest As WinHttp.WinHttpRequest
Attribute rest.VB_VarHelpID = -1

Private Sub close_Click()
    Unload Me
End Sub

Private Function create_receiptcase_dictionary(signCase As Dictionary)
    Dim AT_cases As New Dictionary
    AT_cases.add "unknown", CDec(CDec(&H4154) * (16 ^ 12))
    AT_cases.add "cash_transaction", CDec(CDec(&H4154) * (16 ^ 12)) + 1
    AT_cases.add "zero_receipt", CDec(CDec(&H4154) * (16 ^ 12)) + 2
    AT_cases.add "start_receipt", CDec(CDec(&H4154) * (16 ^ 12)) + 3
    
    Dim AT_flags As New Dictionary
    AT_flags.add "out of service", CDec(&H10000)
    
    Dim AT As New Dictionary
    AT.add "case", AT_cases
    AT.add "flag", AT_flags
    
    Dim DE_cases As New Dictionary
    DE_cases.add "unknown", CDec(CDec(&H4445) * (16 ^ 12))
    DE_cases.add "pos_receipt", CDec(CDec(&H4445) * (16 ^ 12)) + 1
    DE_cases.add "zero_receipt", CDec(CDec(&H4445) * (16 ^ 12)) + 2
    
    Dim DE_flags As New Dictionary
    DE_flags.add "implicit", CDec(CDec(&H10000) * (16 ^ 4))
    
    Dim DE As New Dictionary
    DE.add "case", DE_cases
    DE.add "flag", DE_flags
    
    Dim FR_cases As New Dictionary
    FR_cases.add "unknown", CDec(CDec(&H4652) * (16 ^ 12))
    FR_cases.add "zero_receipt", CDec(CDec(&H4652) * (16 ^ 12)) + 15
    FR_cases.add "start_receipt", CDec(CDec(&H4652) * (16 ^ 12)) + 16
    
    Dim FR_flags As New Dictionary
    FR_flags.add "out of service", CDec(&H10000)
    
    Dim FR As New Dictionary
    FR.add "case", FR_cases
    FR.add "flag", FR_flags
    
    signCase.add "AT", AT
    signCase.add "DE", DE
    signCase.add "FR", FR
    
End Function

Private Function create_ChargeItemCase_dictionary(ChargeItemCase As Dictionary)
    Dim AT As Dictionary
    Set AT = New Dictionary
    AT.add "unknown", CDec(CDec(&H4154) * (16 ^ 12))
    AT.add "undefined_10", CDec(CDec(&H4154) * (16 ^ 12)) + 1
    
    Dim DE As Dictionary
    Set DE = New Dictionary
    DE.add "unknown", CDec(CDec(&H4445) * (16 ^ 12))
    
    Dim FR As Dictionary
    Set FR = New Dictionary
    FR.add "unknown", CDec(CDec(&H4652) * (16 ^ 12))
    
    ChargeItemCase.add "AT", AT
    ChargeItemCase.add "DE", DE
    ChargeItemCase.add "FR", FR
    
End Function

Private Function create_PayItemCase_dictionary(PayItemCase As Dictionary)
    Dim AT As Dictionary
    Set AT = New Dictionary
    AT.add "default", CDec(CDec(&H4154) * (16 ^ 12))
    
    Dim DE As Dictionary
    Set DE = New Dictionary
    DE.add "default", CDec(CDec(&H4445) * (16 ^ 12))
    
    Dim FR As Dictionary
    Set FR = New Dictionary
    FR.add "default", CDec(CDec(&H4652) * (16 ^ 12))
    
    PayItemCase.add "AT", AT
    PayItemCase.add "DE", DE
    PayItemCase.add "FR", FR
    
End Function

Private Function create_ftstate_dictionary(PayItemCase As Dictionary)
    Dim AT As Dictionary
    Set AT = New Dictionary
    AT.add "ready", CDec(CDec(&H4154) * (16 ^ 12))
    
    Dim DE As Dictionary
    Set DE = New Dictionary
    DE.add "ready", CDec(CDec(&H4445) * (16 ^ 12))
    
    Dim FR As Dictionary
    Set FR = New Dictionary
    FR.add "ready", CDec(CDec(&H4652) * (16 ^ 12))
    
    ftstate.add "AT", AT
    ftstate.add "DE", DE
    ftstate.add "FR", FR
    
End Function

Private Sub Form_Load()
    Set rest = New WinHttp.WinHttpRequest
    
    'load colloms for county code selection'
    ComboCC.AddItem "AT"
    ComboCC.AddItem "DE"
    ComboCC.AddItem "FR"
    
    'create cases dictionary
    Set signCase = New Dictionary
    create_receiptcase_dictionary signCase
    Set ChargeItemCase = New Dictionary
    create_ChargeItemCase_dictionary ChargeItemCase
    Set PayItemCase = New Dictionary
    create_PayItemCase_dictionary PayItemCase
    Set ftstate = New Dictionary
    create_ftstate_dictionary ftstate
     
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

Private Function init_zero(receipt_case As Variant) As Dictionary 'zero and start receipt have the same body except the receipt case
    Dim sign As New Dictionary
    
    Dim ChargeItem As Collection
    Set ChargeItem = New Collection
    Dim PayItem As Collection
    Set PayItem = New Collection
    
    'fill dictionary with content
    sign.add "ftcashboxid", Trim(cashboxid.Text)
    sign.add "ftPosSystemId", Trim(POSSID.Text)
    sign.add "cbTerminalID", "1"
    sign.add "cbReceiptReference", "1"
    sign.add "cbChargeItems", ChargeItem
    sign.add "cbPayItems", PayItem
    sign.add "cbReceiptMoment", Format(Now, "mm/dd/yyyy") & "Z" & Format(Now, "hh:mm:ss")
    sign.add "ftReceiptCase", receipt_case
    
    Set init_zero = sign
End Function

Private Function init_sign(receipt_case As Variant) As Dictionary
    Dim sign As New Dictionary
    
    'set chargeitems
    Dim ChargeItem As Dictionary
    Set ChargeItem = New Dictionary
    ChargeItem.add "Quantity", 10#
    ChargeItem.add "Description", "Food"
    ChargeItem.add "Amount", 5#
    ChargeItem.add "VATRate", 10#
    ChargeItem.add "ftChargeItemCase", ChargeItemCase.Item(ComboCC.Text).Item("undefined_10")
    ChargeItem.add "ProductNumber", "1"
    
    Dim ChargeItems As Collection
    Set ChargeItems = New Collection
    ChargeItems.add ChargeItem
    
    'set Payitems
    Dim PayItem As Dictionary
    Set PayItem = New Dictionary
    PayItem.add "Quantity", 10#
    PayItem.add "Description", "Cash"
    PayItem.add "Amount", 5#
    PayItem.add "ftPayItemCase", PayItemCase.Item(ComboCC.Text).Item("default")
    
    Dim PayItems As Collection
    Set PayItems = New Collection
    PayItems.add PayItem
    
    'fill dictionary with content
    sign.add "ftcashboxid", Trim(cashboxid.Text)
    sign.add "ftPosSystemId", Trim(POSSID.Text)
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
    Set sign = init_zero(signCase.Item(ComboCC.Text).Item("case").Item("zero_receipt"))
    
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
    Set sign = init_zero(signCase.Item(ComboCC.Text).Item("case").Item("start_receipt"))
    
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
    If ComboCC.Text = "DE" Then
        Set sign = init_sign(flag(signCase.Item(ComboCC.Text).Item("case").Item("pos_receipt"), signCase.Item(ComboCC.Text).Item("flag").Item("implicit"))) 'create Implicite Receipt case
    Else
        Set sign = init_sign(signCase.Item(ComboCC.Text).Item("case").Item("cash_transaction"))
    End If
    
    'send sign request'
    output.Text = "Request sent" & vbCrLf
    rest.Send JSON.toString(sign)
End Sub

Private Function flag(target_case As Variant, new_flag As Variant) As Variant
    Dim new_upper As Variant
    Dim case_upper As Variant
    Dim new_lower As Variant
    Dim case_lower As Variant
    Dim CC As Variant
    Dim cases As Variant
    
    CC = CDec(Fix(target_case / (16 ^ 12)) * (16 ^ 12))
    new_upper = CDec(Fix(new_flag / (16 ^ 8)))
    case_upper = CDec(Fix((target_case - CC) / (16 ^ 8)))
    new_lower = CDec(Fix(new_flag / (16 ^ 4)) - (new_upper * (16 ^ 4)))
    case_lower = CDec(Fix((target_case - CC) / (16 ^ 4)) - (flags_upper * (16 ^ 4)))
    cases = ((target_case - case_upper * (16 ^ 8)) - case_lower * (16 ^ 4)) - CC
    flag = CDec((case_lower Or new_lower) * (16 ^ 4)) + ((case_upper Or new_upper) * (16 ^ 8)) + CC + cases
End Function

Private Function check_flag(target_case As Variant, try_flag As Variant) As Boolean
    Dim try_lower As Variant
    Dim flags_lower As Variant
    Dim try_upper As Variant
    Dim flags_upper As Variant
    Dim CC As Variant
    
    CC = (CDec(Fix(target_case / (16 ^ 12))) \ 1) * (16 ^ 12)
    try_upper = (Fix(try_flag / (16 ^ 8))) \ 1
    flags_upper = (Fix((target_case - CC) / (16 ^ 8))) \ 1
    try_lower = (Fix(try_flag / (16 ^ 4)) - (try_upper * (16 ^ 4))) \ 1
    flags_lower = (Fix((target_case - CC) / (16 ^ 4)) - (try_upper * (16 ^ 4))) \ 1
    
    If (try_upper And flags_upper) = try_upper And (try_lower And flags_lower) = try_lower Then
        check_flag = True
    Else
        check_flag = False
    End If
End Function

Private Sub rest_OnResponseFinished()
    Dim response As Object
    output.Text = output.Text & "Status " & CStr(rest.Status) & vbCrLf
    If rest.Status = 200 Then
        Set response = JSON.parse(rest.ResponseText)
        'print one object of journal response'
        If Not response Is Nothing And response.Exists("ftState") Then
            output.Text = output.Text & "State: " & response.Item("ftState") & vbCrLf
            For Each Signature In response.Item("ftSignatures")
                output.Text = output.Text & "Signature: " & Signature.Item("data") & vbCrLf
            Next
            'check ftState
            If response.Item("ftState") = ftstate.Item(ComboCC.Text).Item("ready") Then
                'do something
            End If
            
        Else
            output.Text = output.Text & "Body:" & vbCrLf & rest.ResponseText
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
