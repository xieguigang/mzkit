Namespace mzData.mzWebCache

    ''' <summary>
    ''' An abstract mzpack data model for the ms spectrum data 
    ''' </summary>
    Public Interface IMZPack

        Property MS As ScanMS1()
        Property Application As FileApplicationClass
        Property source As String
        Property metadata As Dictionary(Of String, String)

    End Interface
End Namespace