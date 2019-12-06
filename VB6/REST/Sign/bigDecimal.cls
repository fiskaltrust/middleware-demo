VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "bigDecimal"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Private dec() As Byte
Private hex() As Byte

Private Sub Class_Initialize()
    
End Sub

Public Property Set value(number As String, codec As Integer) 'codec 10 or 16
    If codec = 10 Then
        'decimal
        
    
    ElseIf codec = 16 Then
        'hexadecimal
    
    Else
        MsgBox "set value: wrong codec= " & codec
    End If
End Property

Private Sub HexToDec()
    'save hex to decimal property
    
End Sub

Private Sub DecToHex()
    'save decimal to hex property

End Sub

Public Property Let Add(number As String, codec As Integer) 'codec 10 or 16
    If codec = 10 Then
        'decimal
        
    
    ElseIf codec = 16 Then
        'hexadecimal
    
    Else
        MsgBox "Add: wrong codec= " & codec
    End If
End Property

Public Property Let Subtract(number As String, codec As Integer) 'codec 10 or 16
    If codec = 10 Then
        'decimal
        
    
    ElseIf codec = 16 Then
        'hexadecimal
    
    Else
        MsgBox "Subtract: wrong codec= " & codec
    End If
End Property

Public Property Get hex() As String

End Property

Public Property Get dec() As String

End Property


