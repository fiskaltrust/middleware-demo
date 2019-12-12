VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "case_handler"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Private CC As Variant
Private case_value As Variant
Private flags As Variant

Public Function Value(new_CC As Variant, new_case_value As Variant)
    CC = new_CC
    case_value = new_case_value
End Function

Public Function flag(new_flag As Variant)
    Dim new_upper As Variant
    Dim flags_upper As Variant
    Dim new_lower As Variant
    Dim flags_lower As Variant
    
    new_upper = (Fix(new_flag / (16 ^ 8))) \ 1
    flags_upper = (Fix(flags / (16 ^ 8))) \ 1
    new_lower = (Fix(new_flag / (16 ^ 4)) - (new_upper * (16 ^ 4))) \ 1
    flags_lower = (Fix(flags / (16 ^ 4)) - (flags_upper * (16 ^ 4))) \ 1
    flags = ((flags_lower Or new_lower) * (16 ^ 4)) + ((flags_upper Or new_upper) * (16 ^ 8))
End Function

Public Function valueStr(new_str As String)
    Dim mask As Variant
    Dim str_var As Variant
    Dim str_shift As Variant
    str_var = strToVar(new_str)
    str_shift = Fix(str_var / (16 ^ 12))
    CC = str_shift * (16 ^ 12)
    
    Dim str() As Integer
    Dim CC_array() As Integer
    Dim flag_cases As Variant
    str = strToArray(new_str)
    CC_array = variantToArray(CC)
    Subtract str, CC_array
    flag_cases = arrayToVariant(str)
    flags = (Fix(flag_cases / (16 ^ 4))) * (16 ^ 4)
    case_value = flag_cases - flags
    
End Function

Public Property Get check_flag(try_flag As Variant) As Boolean
    Dim try_lower As Variant
    Dim flags_lower As Variant
    Dim try_upper As Variant
    Dim flags_upper As Variant
    Dim catch As Variant
    
    try_upper = (Fix(try_flag / (16 ^ 8))) \ 1
    flags_upper = (Fix(flags / (16 ^ 8))) \ 1
    try_lower = (Fix(try_flag / (16 ^ 4)) - (try_upper * (16 ^ 4))) \ 1
    flags_lower = (Fix(try_flag / (16 ^ 4)) - (try_upper * (16 ^ 4))) \ 1
    
    If (try_upper And flags_upper) = try_upper And (try_lower And flags_lower) = try_lower Then
        check_flag = True
    Else
        check_flag = False
    End If
End Property

Public Property Get toString() As String
    Dim output As String
    Dim collect() As Integer
    ReDim collect(0)
    add collect, variantToArray(CC)
    add collect, variantToArray(case_value)
    add collect, variantToArray(flags)
    For i = (UBound(collect) - 1) To 0 Step -1
        
        If collect(i) < 10 Then 'numbers
            output = output + ChrW$(collect(i) + 48)
        ElseIf collect(i) < 37 Then 'upper case
            output = output + ChrW$(collect(i) + 55)
        Else
            MsgBox "Error toString", vbCritical
        End If
    Next
    toString = output
End Property

Private Function strToVar(source As String) As Variant
    Dim temp As Integer
    Dim resault As Variant
    Dim temp_str As String
    temp_str = source
    
    For i = 0 To (Len(temp_str) - 1)
        temp = Asc(Left(temp_str, 1))
        temp_str = Right(temp_str, Len(temp_str) - 1)
        resault = resault + ((temp - 48) * (10 ^ (Len(temp_str))))
    Next
    strToVar = resault
End Function

Private Function strToArray(number As String) As Integer()
    
    Dim temp As Byte
    Dim target() As Integer
    Dim temp_str As String
    temp_str = number
    ReDim target(0)
    
    For i = 1 To (Len(temp_str))
        temp = Asc(Right(temp_str, 1))
        temp_str = Left(temp_str, Len(temp_str) - 1)
        If temp < 58 And temp > 47 Then 'numbers
            ReDim Preserve target(UBound(target) + 1)
            target(i - 1) = temp - 48
        Else
            MsgBox "input error", vbCritical
        End If
    Next
    strToArray = target
End Function

Private Function variantToArray(number As Variant) As Integer()
    Dim temp As Variant
    
    temp = number
    variantToArray = strToArray(variantToString(temp))
End Function

Private Function variantToString(num As Variant) As String 'Convert Variant to String'
    Dim resault As String
    resault = ""
    Dim dec As String
    Dim counter As Integer
    Dim zeros As Integer
    Dim buffer As Variant
    
    'get length of decimal'
    dec = CStr(num)
    counter = CInt(Right(dec, 2))

    'build string'
    If InStr(1, dec, "E") = 0 Then
        VtS = CStr(num)
        Exit Function
    End If
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

Private Function arrayToVariant(target() As Integer) As Variant
    Dim resault As Variant
    Dim index As Integer
    
    For Each Item In target
        resault = resault + (Item * 10 ^ (index))
        index = index + 1
    Next
    arrayToVariant = resault
End Function



Private Function add(sum() As Integer, Addend() As Integer)
    Dim overload As Integer
    overload = 0
    
    For i = 0 To UBound(Addend)
        sum(i) = Addend(i) + overload
        If (i = UBound(Addend) And sum(i) = 0) Then
            Exit For
        End If
        overload = 0
        While sum(i) >= 10
            sum(index) = sum(i) - 10
            overload = overload + 1
        Wend
        If UBound(sum) <= i Then
            ReDim Preserve sum(i + 1)
        End If
    Next
End Function

Private Function Subtract(Difference() As Integer, Subtrahend() As Integer)
    Dim underload As Integer
    underload = 0
    Dim resault As Integer
    Dim index As Integer
    index = 0
    
    
    For Each Item In Difference
        If underload > 0 Then
            'if negative
            If (UBound(Difference) = index) Then
                Err.Raise -1, , , , "number would be negative"
            End If
            'calculate underload
            If underload > Item Then
                Difference(index) = Item + (10 - underload)
                underload = 1
            Else
                Difference(index) = Item - underload
                underload = 0
            End If
        End If
        
        If index <= UBound(Subtrahend) Then
            If Item < Subtrahend(index) Then
                Difference(index) = Difference(index) + (10 - underload)
                underload = underload + 1
            End If
            Difference(index) = Difference(index) - Subtrahend(index)
        End If
        index = index + 1
        
    Next
    underload = 0
    'shorten Difference if needed
    For i = (UBound(Difference)) To 0 Step -1
       If Difference(i) = 0 Then
        underload = underload + 1
       End If
    Next
    ReDim Preserve Difference(UBound(Difference) - (underload - 1))
End Function
