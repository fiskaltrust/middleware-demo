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
      Left            =   480
      TabIndex        =   9
      Top             =   5280
      Width           =   2175
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
         Text            =   "BJ6ZufH6hcCHmu2yzc9alH45FjdlCUT1YDlAf83gTydHKj1ZWcMibPlheky1WLMc+E9WeHYanQ8vS5oCirhI6Ck="
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
         Text            =   "a37ce376-62be-42c6-b560-1aa0a6700211"
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
         Text            =   " https://signaturcloud-sandbox.fiskaltrust.at "
         Top             =   240
         Width           =   8295
      End
   End
   Begin VB.CommandButton send 
      Caption         =   "Send request"
      Height          =   615
      Left            =   3240
      TabIndex        =   0
      Top             =   5280
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

Private WithEvents rest As WinHttp.WinHttpRequest
Attribute rest.VB_VarHelpID = -1

Private Sub close_Click()
    Unload Me
End Sub

Private Function create_receiptcase_dictionary(signCase As Dictionary)
    Dim AT As Dictionary
    Set AT = New Dictionary
    AT.Add "unknown", "4707387510509010944"
    AT.Add "zero_receipt", "4707387510509010946"
    
    Dim DE As Dictionary
    Set DE = New Dictionary
    DE.Add "unknown", "4919338167972134912"
    DE.Add "zero_receipt", "4919338167972134914"
    
    Dim FR As Dictionary
    Set FR = New Dictionary
    FR.Add "unknown", "5067112530745229312"
    FR.Add "zero_receipt", "5067112530745229327"
    
    signCase.Add "AT", AT
    signCase.Add "DE", DE
    signCase.Add "FR", FR
    
End Function

Private Sub Form_Load()
    Set rest = New WinHttp.WinHttpRequest
    
    'load colloms for county code selection'
    ComboCC.AddItem "AT"
    ComboCC.AddItem "DE"
    ComboCC.AddItem "FR"
    
    Set signCase = New Dictionary
    create_receiptcase_dictionary signCase
    
End Sub

Private Function Set_URL(ServiceURL As String, endpoint As String) As String
    
    ServiceURL = Trim(URL.Text)
    If Right(ServiceURL, 1) = "/" Then
        Set_URL = ServiceURL & endpoint
    Else
        Set_URL = ServiceURL & "/" & endpoint
    End If
End Function

Private Function Set_object(sign As Dictionary)
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
    sign.Add "ftReceiptCase", signCase.Item(ComboCC.Text).Item("zero_receipt")
    
End Function

Private Sub send_Click()
    
    'lock input fields'
    MousePointer = vbHourglass
    URL.Locked = True
    cashboxid.Locked = True
    accesstoken.Locked = True
    ComboCC.Locked = True
    
    Dim ServiceURL As String
    
    'Set rest = New WinHttp.WinHttpRequest
    Dim sign As Dictionary
    Set sign = New Dictionary
    Set_object sign
    
    
    'Set Methode and Add Parameter to URL'
    ServiceURL = Set_URL(URL.Text, "json/sign")
    
    'set URL and methode'
    rest.Open "POST", ServiceURL, True
    
    'set headers'
    rest.SetRequestHeader "Content-Type", "application/json"
    rest.SetRequestHeader "cashboxid", Trim(cashboxid.Text)
    rest.SetRequestHeader "accesstoken", Trim(accesstoken.Text)
    
    'send journal request'
    rest.send JSON.toString(sign)
    output.Text = "Request sent" & vbCrLf
     
End Sub

Private Sub rest_OnResponseFinished()
    Dim response As Object
    output.Text = output.Text & "Status " & CStr(rest.Status) & vbCrLf
    If rest.Status = 200 Then
        
        Set response = JSON.parse(rest.ResponseText)
        'print one object of journal response'
        output.Text = output.Text & "State: " & response.Item("ftState")
        
    End If
    
    'unlock input fields'
    MousePointer = vbdeault
    URL.Locked = False
    cashboxid.Locked = False
    accesstoken.Locked = False
    ComboCC.Locked = False
End Sub
