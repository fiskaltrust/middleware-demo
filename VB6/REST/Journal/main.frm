VERSION 5.00
Begin VB.Form journal 
   AutoRedraw      =   -1  'True
   Caption         =   "fiskaltrust.service VB6 journal example"
   ClientHeight    =   6220
   ClientLeft      =   170
   ClientTop       =   560
   ClientWidth     =   18610
   LinkTopic       =   "Form1"
   ScaleHeight     =   6220
   ScaleWidth      =   18610
   StartUpPosition =   3  'Windows Default
   Begin VB.Frame Frame5 
      Caption         =   "Journal Type"
      Height          =   615
      Left            =   360
      TabIndex        =   10
      Top             =   3960
      Width           =   2775
      Begin VB.ComboBox ComboCC 
         Height          =   315
         Left            =   120
         TabIndex        =   11
         Text            =   "AT DEP7"
         Top             =   240
         Width           =   2535
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
      Width           =   9135
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
      Left            =   3240
      TabIndex        =   0
      Top             =   5280
      Width           =   2535
   End
   Begin VB.Label Label1 
      Caption         =   "This example sends a journal request to a fiskaltrust.service via REST and downloads the full journal of the queue"
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
Private journalCase As Dictionary

Private WithEvents rest As WinHttp.WinHttpRequest
Attribute rest.VB_VarHelpID = -1

Private Sub close_Click()
    Unload Me
End Sub

Private Function create_journalcase_dictionary(journalCase As Dictionary)
    journalCase.Add "AT DEP7", (CDec(&H4154) * (16 ^ 12)) + 1
    journalCase.Add "DE Info", CDec(&H4445) * (16 ^ 12)
    journalCase.Add "FR Ticket", CDec(&H4652) * (16 ^ 12) + 1
    
End Function

Private Sub Form_Load()
    Set rest = New WinHttp.WinHttpRequest
    
    'load colloms for county code selection'
    ComboCC.AddItem "AT DEP7"
    ComboCC.AddItem "DE Info"
    ComboCC.AddItem "FR Ticket"
    
    'set values for journalType'
    Set journalCase = New Dictionary
    create_journalcase_dictionary journalCase
    
    
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
    If ComboCC.Text = "DE Info" Then
        ServiceURL = Set_URL(URL.Text, "json/V0/journal") 'German endpoint'
    Else
        ServiceURL = Set_URL(URL.Text, "json/journal")
    End If
    
    ServiceURL = ServiceURL & "?type=" & journalCase.Item(ComboCC.Text)
    ServiceURL = ServiceURL & "&from=0&to=0"
    
    'set URL and methode'
    rest.Open "POST", ServiceURL, True
    
    'set headers'
    rest.SetRequestHeader "Content-Type", "application/json"
    rest.SetRequestHeader "cashboxid", Trim(cashboxid.Text)
    rest.SetRequestHeader "accesstoken", Trim(accesstoken.Text)
    
    'send journal request'
    rest.Send ""
    output.Text = "Request sent" & vbCrLf
     
End Sub

Private Sub rest_OnResponseFinished()
    Dim response As Object
    output.Text = output.Text & "Status " & CStr(rest.Status)
    If rest.Status = 200 Then
        output.Text = output.Text & vbCrLf & rest.ResponseText & vbCrLf
        If ComboCC.Text = "AT DEP7" Then
            'print one object of journal response'
            Set response = JSON.parse(rest.ResponseText)
            MsgBox "Kassen-ID: " & response.Item("Belege-Gruppe")(1).Item("Kassen-ID"), vbInformation
        End If
    Else
        output.Text = output.Text & vbCrLf & rest.ResponseText
    End If
    
    'unlock input fields'
    MousePointer = vbdeault
    URL.Locked = False
    cashboxid.Locked = False
    accesstoken.Locked = False
    ComboCC.Locked = False
End Sub
