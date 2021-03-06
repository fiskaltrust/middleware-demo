VERSION 5.00
Begin VB.Form echo 
   Caption         =   "fiskaltrust.service VB6 echo example"
   ClientHeight    =   5880
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   14790
   LinkTopic       =   "Form1"
   ScaleHeight     =   5880
   ScaleWidth      =   14790
   StartUpPosition =   3  'Windows Default
   Begin VB.ComboBox ComboCC 
      Height          =   280
      Left            =   720
      TabIndex        =   12
      Text            =   "DE"
      Top             =   5040
      Width           =   855
   End
   Begin VB.Frame Frame5 
      Caption         =   "Country"
      Height          =   610
      Left            =   600
      TabIndex        =   13
      Top             =   4800
      Width           =   1090
   End
   Begin VB.CommandButton close 
      Caption         =   "Close"
      Height          =   615
      Left            =   6600
      TabIndex        =   11
      Top             =   4800
      Width           =   2175
   End
   Begin VB.TextBox output 
      Height          =   5415
      Left            =   9120
      Locked          =   -1  'True
      MultiLine       =   -1  'True
      TabIndex        =   10
      Top             =   240
      Width           =   5535
   End
   Begin VB.Frame Frame4 
      Caption         =   "Echo Message"
      Height          =   615
      Left            =   360
      TabIndex        =   7
      Top             =   3840
      Width           =   8535
      Begin VB.TextBox Message 
         Height          =   285
         Left            =   120
         TabIndex        =   8
         Top             =   240
         Width           =   8295
      End
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
         Top             =   240
         Width           =   8295
      End
   End
   Begin VB.CommandButton send 
      Caption         =   "Send request"
      Height          =   615
      Left            =   3120
      TabIndex        =   0
      Top             =   4800
      Width           =   2535
   End
   Begin VB.Label Label1 
      Caption         =   "This example sends an echo request to the fiskaltrust.Service via REST"
      Height          =   735
      Left            =   360
      TabIndex        =   9
      Top             =   360
      Width           =   8535
   End
End
Attribute VB_Name = "echo"
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
    
    'load colloms for county code selection'
    ComboCC.AddItem "AT"
    ComboCC.AddItem "DE"
    ComboCC.AddItem "FR"
End Sub

Private Function Set_URL(URL As String, endpoint As String) As String
    If InStr(1, URL, "rest") Then
        URL = Replace(URL, "rest", "http", 1, -1, vbTextCompare)
    End If
    
    Dim ServiceURL As String
    ServiceURL = Trim(URL)
    If Right(ServiceURL, 1) = "/" Then
        Set_URL = ServiceURL & endpoint
    Else
        Set_URL = ServiceURL & "/" & endpoint
    End If
End Function

Private Sub send_Click()
    MousePointer = vbHourglass
    
    Dim echo As String
    Dim ServiceURL As String
    
    Set rest = New WinHttp.WinHttpRequest
    
    'set URL and methode'
    
    
    If ComboCC = "DE" Then
        ServiceURL = Set_URL(URL.Text, "json/V0/echo") 'German endpoint'
    Else
        ServiceURL = Set_URL(URL.Text, "json/echo")
    End If
    
    rest.Open "POST", ServiceURL, True
    
    'set headers'
    rest.SetRequestHeader "Content-Type", "application/json"
    rest.SetRequestHeader "cashboxid", Trim(cashboxid.Text)
    rest.SetRequestHeader "accesstoken", Trim(accesstoken.Text)
    
    'set echo message'
    echo = Trim(Message.Text)
    rest.Send """" & echo & """"
    
End Sub

Private Sub rest_OnResponseFinished()
    output.Text = "Status " & CStr(rest.Status)
    If rest.Status = 200 Then
        output.Text = output.Text & vbCrLf & rest.ResponseText & vbCrLf
    End If
    MousePointer = vbDefault
End Sub
