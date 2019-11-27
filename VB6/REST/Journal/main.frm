VERSION 5.00
Begin VB.Form journal 
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
      Caption         =   "This example sends a journal request to a fiskaltrust.service via REST and downloads the DEP7 of the queue"
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
'declare journaltype object'
Private Type Austria_type
    Status As String
    RKSV_DEP_Export As String
End Type
Private Type Germany_type
    Status As String
    GdPdU_Export As String
End Type
Private Type France_type
    Status As String
    Ticket As String
    Payment_Prove As String
    Invoice As String
    Grand_Total As String
    Bill As String
    Archive As String
    Log As String
    Copy As String
    Training As String
End Type
Private Type Jtype
    AT As Austria_type
    DE As Germany_type
    FR As France_type
End Type

Private journalType As Jtype

Private WithEvents rest As WinHttp.WinHttpRequest
Attribute rest.VB_VarHelpID = -1

Private Sub close_Click()
    Unload Me
End Sub

Private Sub Form_Load()
    Set rest = New WinHttp.WinHttpRequest
    
    'load colloms for county code selection'
    ComboCC.AddItem "AT"
    ComboCC.AddItem "DE"
    ComboCC.AddItem "FR"
    
    'set values for journalType'
    journalType.AT.Status = "4707387510509010944"
    journalType.AT.RKSV_DEP_Export = "4707387510509010945"
    journalType.DE.Status = "4919338167972134912"
    journalType.DE.GdPdU_Export = "4919338167972134913"
    journalType.FR.Status = "5067112530745229312"
    journalType.FR.Ticket = "5067112530745229313"
    journalType.FR.Payment_Prove = "5067112530745229314"
    journalType.FR.Invoice = "5067112530745229315"
    journalType.FR.Grand_Total = "5067112530745229316"
    journalType.FR.Bill = "5067112530745229317"
    journalType.FR.Archive = "5067112530745229318"
    journalType.FR.Log = "5067112530745229319"
    journalType.FR.Copy = "5067112530745229320"
    journalType.FR.Training = "5067112530745229321"
    
    
End Sub

Private Function Set_URL(ServiceURL As String, endpoint As String) As String
    
    ServiceURL = Trim(URL.Text)
    If Right(ServiceURL, 1) = "/" Then
        Set_URL = ServiceURL & endpoint
    Else
        Set_URL = ServiceURL & "/" & endpoint
    End If
End Function

Private Sub send_Click()
    
    'lock input fields'
    MousePointer = vbHourglass
    URL.Locked = True
    cashboxid.Locked = True
    accesstoken.Locked = True
    ComboCC.Locked = True
    
    Dim echo As String
    Dim ServiceURL As String
    
    Set rest = New WinHttp.WinHttpRequest
    
    'Set Methode and Add Parameter to URL'
    ServiceURL = Set_URL(URL.Text, "json/journal")
    ServiceURL = ServiceURL & "?type=" & journalType.AT.RKSV_DEP_Export
    ServiceURL = ServiceURL & "&from=0&to=0"
    
    'set URL and methode'
    rest.Open "POST", ServiceURL, True
    
    'set headers'
    rest.SetRequestHeader "Content-Type", "application/json"
    rest.SetRequestHeader "cashboxid", Trim(cashboxid.Text)
    rest.SetRequestHeader "accesstoken", Trim(accesstoken.Text)
    
    'send journal request'
    rest.send ""
    output.Text = "Request send" & vbCrLf
     
End Sub

Private Sub rest_OnResponseFinished()
    Dim response As Object
    output.Text = output.Text & "Status " & CStr(rest.Status)
    If rest.Status = 200 Then
        output.Text = output.Text & vbCrLf & rest.ResponseText
        Set response = JSON.parse(rest.ResponseText)
        'print one object of journal response'
        MsgBox "Kassen-ID: " & response.Item("Belege-Gruppe")(1).Item("Kassen-ID"), vbInformation
        
    End If
    
    'unlock input fields'
    MousePointer = vbdeault
    URL.Locked = False
    cashboxid.Locked = False
    accesstoken.Locked = False
    ComboCC.Locked = False
End Sub
