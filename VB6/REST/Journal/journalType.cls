VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Jtype"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Private Type AT
        Status As String
        RKSV_DEP_Export As String
    End Type
Private Type DE
        Status As String
        GdPdU_Export As String
    End Type
Private Type FR
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


Private Type journalType
    AT As AT
    DE As DE
    FR As FR
End Type

