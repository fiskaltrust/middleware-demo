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

Private Function build_journalType(CountyCode As String) As String
    Dim journalType As Variant
    Dim temp As Integer
    Dim Jtype As Variant
    Dim Char() As Byte
    Dim char1 As Byte
    Dim char2 As Byte
    Dim Jreturn As String
    
    Jtype = 1
    journalType = 0
    Char = StrConv(CountyCode, vbFromUnicode)
    
    char1 = Char(0)
    char2 = Char(1)
    
    temp = CInt(char1)
    journalType = journalType + (temp * (2 ^ 56)) 'Add first letter of county code'
    temp = CInt(char2)
    journalType = journalType + (temp * (2 ^ 48)) 'Add second letter of county code
    'MsgBox VtS(journalType), vbInformation
    journalType = journalType + 1 'Add journal type'
    'MsgBox VtS(journalType), vbInformation
    build_journalType = VtS(journalType)

End Function


Private Sub send_Click()
    MousePointer = vbHourglass
    'txtQuery.Locked = True'
    Dim echo As String
    Dim ServiceURL As String
    Dim journalType As String
    
    Set rest = New WinHttp.WinHttpRequest
    
    'Set Methode and Add Parameter to URL'
    ServiceURL = Set_URL(URL.Text, "json/journal")
    journalType = build_journalType(ComboCC.Text)
    MsgBox journalType, vbInformation
    ServiceURL = ServiceURL & "?type=" & journalType
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
        'MsgBox rest.ResponseText, vbInformation
        Set response = JSON.parse(rest.ResponseText)
        MsgBox "Kassen-ID: " & response.Item("Belege-Gruppe")(1).Item("Kassen-ID"), vbInformation
        
    End If
    MousePointer = vbDefault
End Sub
