Imports System.Runtime.InteropServices

Namespace mzData.mzWebCache

    ''' <summary>
    ''' a unify reader interface for <see cref="BinaryStreamReader"/> read 
    ''' data from file or read data from ``mzPack`` in-memory data object.
    ''' </summary>
    Public Interface IMzPackReader

        ''' <summary>
        ''' get all scan ms1 data its scan id collection
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property EnumerateIndex As IEnumerable(Of String)
        ''' <summary>
        ''' the source file name of current raw data file
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property source As String
        ReadOnly Property rtmax As Double
        Function ReadScan(scan_id As String, Optional skipProducts As Boolean = False) As ScanMS1
        Function GetMetadata(id As String) As Dictionary(Of String, String)

        Sub ReadChromatogramTick(scanId As String,
                                 <Out> ByRef scan_time As Double,
                                 <Out> ByRef BPC As Double,
                                 <Out> ByRef TIC As Double)
        Function hasMs2(Optional sampling As Integer = 64) As Boolean

    End Interface
End Namespace