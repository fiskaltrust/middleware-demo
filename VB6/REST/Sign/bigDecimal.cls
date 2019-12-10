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
Private num() As Byte
Private base As Byte


Private Property Let set_base(new_base As Byte)
    If check_base(new_base) Then
        MsgBox "wrong base"
    Else
        base = new_base
    End If
    
End Property

Public Sub Set_value(number As String, new_base As Byte)
    If check_base(new_base) = False Then
        Exit Sub
    End If
    Dim temp As Byte
    ReDim num(0)
    
    base = new_base
    
    For i = 1 To (Len(number))
        temp = Asc(right(number, 1))
        number = left(number, Len(number) - 1)
        If temp < 58 And temp > 47 Then 'numbers
            ReDim Preserve num(UBound(num) + 1)
            num(i - 1) = temp - 48
        ElseIf temp > 64 And temp < 91 Then 'upper case
            ReDim Preserve num(UBound(num) + 1)
            num(i - 1) = temp - 55
        ElseIf temp > 96 And temp < 123 Then 'lowercase
            ReDim Preserve num(UBound(num) + 1)
            num(i - 1) = temp - 87
        Else
            MsgBox "input error", vbCritical
        End If
    Next
    
    
End Sub

Public Sub convertBase(new_base As Byte)
    If (new_base = base) Then
        Exit Sub
    End If
    Dim overload As Byte
    Dim buffer As Byte
    Dim index As Byte
    Dim temp As bigDecimal
    'Dim buffer As bigDecimal
    index = 0
    Set temp = Me
    ReDim num(0)
    
    
      
    
    
    base = newbase
End Sub

Public Function add(target As bigDecimal)
    Dim overload As Integer
    overload = 0
    Dim sum As Byte
    Dim index As Byte
    index = 0
    Dim temp As Byte
    
    If target.get_base <> base Then
        target.convertBase base
    End If
    
    
    For Each Item In target.numbers
        temp = num(index) + overload
        sum = Item + temp
        overload = 0
        While sum >= base
            sum = sum - base
            overload = overload + 1
        Wend
        If UBound(num) = index Then
            ReDim Preserve num(sum)
        Else
            num(index) = sum
        End If
        index = index + 1
    Next
    
End Function

Public Function Subtract(target As bigDecimal)
    Dim underload As Byte
    underload = 0
    Dim resault As Byte
    Dim index As Byte
    index = 0
    Dim substractor() As Byte
    substractor = target.numbers
    
    If target.get_base <> base Then
        target.convertBase base
    End If
    
    
    For Each Item In num
        If underload > 0 Then
            'if negative
            If (UBound(num) = index) Then
                Err.Raise -1, , , , "number would be negative"
            End If
            'calculate underload
            If underload > Item Then
                num(index) = Item + (base - underload)
                underload = 1
            Else
                num(index) = Item - underload
                underload = 0
            End If
        End If
        
        If index <= UBound(substractor) Then
            If Item < substractor(index) Then
                num(index) = num(index) + (base - underload)
                underload = underload + 1
            End If
            num(index) = num(index) - substractor(index)
        End If
        index = index + 1
        
    Next
    underload = 0
    'shorten num if needed
    For i = (UBound(num)) To 0 Step -1
       If num(i) = 0 Then
        underload = underload + 1
       End If
    Next
    ReDim Preserve num(UBound(num) - (underload - 1))
End Function

Public Property Get get_base() As Byte
    get_base = base
End Property

Public Property Get toString(Optional target_base As Byte) As String
    Dim printaible As New bigDecimal
    Dim output As String
    If target_base And base <> target_base Then
        Set printable = Me
        printable.convertBase base
        toString = printable.toString(target_base)
        Exit Property
    End If
    'MsgBox UBound(printable.num)
    For i = (UBound(num) - 1) To 0 Step -1
        
        If num(i) < 10 Then 'numbers
            output = output + ChrW$(num(i) + 48)
        ElseIf num(i) < 37 Then 'upper case
            output = output + ChrW$(num(i) + 55)
        Else
            MsgBox "Error toString", vbCritical
        End If
    Next
    toString = output
End Property

Public Property Get numbers() As Byte()
    numbers = num
End Property

Private Function check_base(new_num_base As Byte) As Boolean
    If new_num_base < 2 Or newnum_base > 37 Then
        check_base = False
    Else
        check_base = True
    End If
End Function



