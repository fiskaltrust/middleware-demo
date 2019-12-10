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
Private num() As Integer
Private base As Integer
Private CC As String

base = 10

Public Property Set country(number As String)
    CC = number
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
End Property

Public Property Set flag(new_flag As Variant)
    
    
End Property

Private Function add(flag_obj As bigDecimal)
    Dim overload As Integer
    overload = 0
    Dim sum As Integer
    Dim index As Integer
    index = 0
    Dim temp As Integer
    
    For Each Item In flag_obj.numbers
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

Private Function Subtract(target As bigDecimal)
    Dim underload As Integer
    underload = 0
    Dim resault As Integer
    Dim index As Integer
    index = 0
    Dim substractor() As Integer
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

Public Function check_flag(flag As Variant) As Boolean
    Dim flagspace_obj As New bigDecimal
    Dim flagspace As Variant
    Dim country_obj As New bigDecimal
    Set flagspace_obj = Me
    country_obj.Set_value CC, 10
    flagspace_obj.Subtract country
    flagspace = flagspace_obj.toVariant
    If (flagspace And flag) = flag Then
        check_flag = True
    Else
        check_flag = False
    End If
End Function

Private Property Get toVariant() As Variant
    Dim resault As Variant
    Dim index As Integer
    
    For Each Item In num
        resault = resault + (Item * 10 ^ (index))
        index = index + 1
    Next
    toVariant = resault
End Property

Public Property Get toString() As String
    Dim output As String
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



