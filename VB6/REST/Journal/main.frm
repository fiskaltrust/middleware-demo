VERSION 5.00
Begin VB.Form journal 
   Caption         =   "Form1"
   ClientHeight    =   6225
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   14790
   LinkTopic       =   "Form1"
   ScaleHeight     =   6225
   ScaleWidth      =   14790
   StartUpPosition =   3  'Windows Default
   Begin VB.Frame Frame5 
      Caption         =   "County code"
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
      Caption         =   "This .."
      Height          =   735
      Left            =   360
      TabIndex        =   7
      Top             =   360
      Width           =   8535
   End
End
Attribute VB_Name = "journal"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'Private Const HTTPREQUEST_PROXYSETTING_PROXY& = 2

Private WithEvents rest As WinHttp.WinHttpRequest
Attribute rest.VB_VarHelpID = -1

Private Sub close_Click()
    Unload Me
End Sub

Private Sub Form_Load()
    Set rest = New WinHttp.WinHttpRequest
    
    'load colloms'
    ComboCC.AddItem "AT"
    ComboCC.AddItem "DE"
    ComboCC.AddItem "FR"
End Sub

Private Function Set_URL(ServiceURL As String, endpoint As String) As String
    
    ServiceURL = Trim(URL.Text)
    If Right(ServiceURL, 1) = "/" Then
        Set_URL = ServiceURL & endpoint
    Else
        Set_URL = ServiceURL & "/" & endpoint
    End If
End Function

Private Function VtS(num As Variant) As String 'Convert Variant to String'
    Dim resault As String
    
    Do
        resault = num(1)
        
    Loop While num > 9
    
    
End Function

Private Function build_journalType(CountyCode As String) As String
    Dim journalType As Variant
    Dim temp As Integer
    Dim char() As Byte
    Dim char1 As Byte
    Dim char2 As Byte
    
    journalType = 0
    char = StrConv(CountyCode, vbFromUnicode)
    
    char1 = char(0)
    char2 = char(1)
    
    temp = CInt(char1)
    'MsgBox temp, vbInformation
    journalType = journalType + (temp * (2 ^ 56)) 'shift first letter to beginning'
    temp = CInt(char2)
    'MsgBox temp, vbInformation
    journalType = journalType + (temp * (2 ^ 48)) 'shift second after first'
    'MsgBox journalType, vbInformation
    journalType = journalType + 1 'Add journal type'
    
    'MsgBox journalType, vbInformation
    
    build_journalType = VtS(journalType)

End Function


Private Sub send_Click()
    'MousePointer = vbHourglass
    'txtQuery.Locked = True'
    Dim echo As String
    Dim ServiceURL As String
    Dim journalType As String
    
    Set rest = New WinHttp.WinHttpRequest
    
    'Set Methode and Add Parameter to URL'
    ServiceURL = Set_URL(URL.Text, "json/journal")
    journalType = build_journalType(ComboCC.Text)
    ServiceURL = ServiceURL & "?type=" & journalType
    ServiceURL = ServiceURL & "&from=0&to=0"
    
    'set URL and methode'
    rest.Open "POST", ServiceURL, True
    
    'set headers'
    rest.SetRequestHeader "Content-Type", "application/json"
    rest.SetRequestHeader "cashboxid", Trim(cashboxid.Text)
    rest.SetRequestHeader "accesstoken", Trim(accesstoken.Text)
    
    'send journal request'
    rest.Send ""
    
    rest.WaitForResponse
    If rest.Status = 200 Then
        output.Text = "Status " & CStr(rest.Status) & vbCrLf & rest.ResponseText
    Else
        output.Text = "Status " & CStr(rest.Status)
    End If
    
    
End Sub
