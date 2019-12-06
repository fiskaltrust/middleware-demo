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
         Text            =   "AT"
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
         Text            =   "http://fiskaltrust.free.beeceptor.com/"
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
Private zero As Boolean


Private WithEvents rest As WinHttp.WinHttpRequest
Attribute rest.VB_VarHelpID = -1

Private Sub close_Click()
    Unload Me
End Sub

Private Function VtS(num As Variant) As String 'Convert Variant to String'
    Dim resault As String
    resault = ""
    Dim dec As String
    Dim counter As Integer
    Dim zeros As Integer
    Dim temp As Integer
    
    'get length of decimal'
    dec = CStr(num)
    counter = CInt(Right(dec, 2))

    'build string'
    Dim index As Integer
    Dim j As Integer
    For index = 1 To counter
        j = 0
        dec = CStr(num)
        resault = resault & Left(dec, 1)
        num = num - (CInt(Left(dec, 1)) * (10 ^ (counter - (index - 1))))
        dec = CStr(num)
        If InStr(1, dec, "E") = 0 Then
            resault = resault & CStr(num)
            Exit For
        End If
        zeros = (counter - (index)) - (CInt(Mid(dec, InStr(1, dec, "E") + 1)))

        While j < zeros
            resault = resault & "0"
            index = index + 1
            j = j + 1
        Wend
    Next
    VtS = resault
End Function

Private Function create_receiptcase_dictionary(signCase As Dictionary)
    Dim AT As Dictionary
    Set AT = New Dictionary
    AT.Add "unknown", 4.70738751E+18 + 509010944 '64bit unsigned integer cant be hardcode otherwise because of IDE
    AT.Add "RKSV", 4.70738751E+18 + 509010945
    AT.Add "zero_receipt", 4.70738751E+18
    AT.Add "start_receipt", 4.70738751E+18 ' + 509010947
    'AT.Item("zero_receipt") = AT.Item("zero_receipt") + 509010946
    'MsgBox VtS(AT.Item("start_receipt"))
    
    Dim DE As Dictionary
    Set DE = New Dictionary
    DE.Add "unknown", 4.919338167E+18 ' + 972134912
    DE.Add "standart", 4.919338167E+18 ' + 972134913
    DE.Add "zero_receipt", 4919338167# * (10 ^ 9) + 972134914 '       4.919338167E+18 + 972134914
    'DE.Item("zero_receipt") = DE.Item("zero_receipt") + 972134914
    'DE.Item("zero_receipt") = DE.Item("zero_receipt") + 2
    MsgBox VtS(DE.Item("zero_receipt"))
    
    Dim FR As Dictionary
    Set FR = New Dictionary
    FR.Add "unknown", 5.06711253E+18 + 745229312
    FR.Add "zero_receipt", 5.06711253E+18 + 745229327
    
    signCase.Add "AT", AT
    signCase.Add "DE", DE
    signCase.Add "FR", FR
    
End Function

Private Function create_ChargeItemCase_dictionary(ChargeItemCase As Dictionary)
    Dim AT As Dictionary
    Set AT = New Dictionary
    AT.Add "unknown", 4.70738751E+18 + 509010944 '64bit unsigned integer cant be hardcode otherwise because of IDE
    AT.Add "undefined_10", 4.70738751E+18 + 509010945
    
    Dim DE As Dictionary
    Set DE = New Dictionary
    DE.Add "unknown", 4.919338167E+18 + 972134912
    
    Dim FR As Dictionary
    Set FR = New Dictionary
    FR.Add "unknown", 5.06711253E+18 + 745229312
    
    ChargeItemCase.Add "AT", AT
    ChargeItemCase.Add "DE", DE
    ChargeItemCase.Add "FR", FR
    
End Function

Private Function create_PayItemCase_dictionary(PayItemCase As Dictionary)
    Dim AT As Dictionary
    Set AT = New Dictionary
    AT.Add "default", 4.70738751E+18 + 509010944 '64bit unsigned integer cant be hardcode otherwise because of IDE
    
    Dim DE As Dictionary
    Set DE = New Dictionary
    DE.Add "default", 4.919338167E+18 + 972134912
    
    Dim FR As Dictionary
    Set FR = New Dictionary
    FR.Add "default", 5.06711253E+18 + 745229312
    
    PayItemCase.Add "AT", AT
    PayItemCase.Add "DE", DE
    PayItemCase.Add "FR", FR
    
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
    
    Dim ChargeItem As Collection
    Set ChargeItem = New Collection
    Dim PayItem As Collection
    Set PayItem = New Collection
    
    sign.Add "ftcashboxid", Trim(cashboxid.Text)
    sign.Add "cbTerminalID", "1"
    sign.Add "cbReceiptReference", "1"
    sign.Add "cbChargeItems", ChargeItem
    sign.Add "cbPayItems", PayItem
    sign.Add "cbReceiptMoment", Format(Now, "mm/dd/yyyy") & "Z" & Format(Now, "hh:mm:ss")
    sign.Add "ftReceiptCase", receipt_case
    
    Set init_zero = sign
End Function

Private Function init_sign(receipt_case As String) As Dictionary
    Dim sign As New Dictionary
    
    'set chargeitems
    Dim ChargeItem As Dictionary
    Set ChargeItem = New Dictionary
    ChargeItem.Add "Quantity", 10#
    ChargeItem.Add "Description", "Food"
    ChargeItem.Add "Amount", 5#
    ChargeItem.Add "VATRate", 10#
    ChargeItem.Add "ftChargeItemCase", ChargeItemCase.Item(ComboCC.Text).Item("undefined_10")
    ChargeItem.Add "ProductNumber", "1"
    
    Dim ChargeItems As Collection
    Set ChargeItems = New Collection
    ChargeItems.Add ChargeItem
    
    'set Payitems
    Dim PayItem As Dictionary
    Set PayItem = New Dictionary
    PayItem.Add "Quantity", 10#
    PayItem.Add "Description", "Cash"
    PayItem.Add "Amount", 5#
    PayItem.Add "ftPayItemCase", PayItemCase.Item(ComboCC.Text).Item("default")
    
    Dim PayItems As Collection
    Set PayItems = New Collection
    PayItems.Add PayItem
    
    sign.Add "ftcashboxid", Trim(cashboxid.Text)
    sign.Add "cbTerminalID", "1"
    sign.Add "cbReceiptReference", "1"
    sign.Add "cbChargeItems", ChargeItems
    sign.Add "cbPayItems", PayItems
    sign.Add "cbReceiptMoment", Format(Now, "mm/dd/yyyy") & "Z" & Format(Now, "hh:mm:ss")
    sign.Add "ftReceiptCase", receipt_case
    
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
    Set sign = init_zero(VtS(signCase.Item(ComboCC.Text).Item("zero_receipt")))
    
    'send sign request'
    zero = True
    output.Text = "Request sent" & vbCrLf
    MsgBox VtS(signCase.Item(ComboCC.Text).Item("zero_receipt"))
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
    Set sign = init_zero(VtS(signCase.Item(ComboCC.Text).Item("start_receipt")))
    
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
        Set sign = init_sign(VtS(signCase.Item(ComboCC.Text).Item("standart") + 4294967296#)) 'create Implicite Receipt case
    Else
        Set sign = init_sign(VtS(signCase.Item(ComboCC.Text).Item("RKSV")))
    End If
    
    
    
    'send sign request'
    output.Text = "Request sent" & vbCrLf
    rest.Send JSON.toString(sign)
End Sub

Private Sub rest_OnResponseFinished()
    Dim response As Object
    output.Text = output.Text & "Status " & CStr(rest.Status) & vbCrLf
    If rest.Status = 200 And zero = False Then
        'output.Text = output.Text & rest.ResponseText & vbCrLf
        Set response = JSON.parse(rest.ResponseText)
        'print one object of journal response'
        output.Text = output.Text & "State: " & response.Item("ftState") & vbCrLf
        For Each Signature In response.Item("ftSignatures")
            If (Signature.Item("ftSignatureType") <> "4707387510509010944") Then
                output.Text = output.Text & "Signature: " & Signature.Item("data") & vbCrLf
            End If
        Next
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
