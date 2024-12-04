
Imports System.IO

Namespace PackLib

    Public Module Extensions

        ''' <summary>
        ''' open reference database file writer
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function Open(Of T As IReferencePack)(file As String) As IReferencePack
            Dim w As Stream = file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)

            Select Case GetType(T)
                Case GetType(MgfPack) : Return New MgfPack(w)
                Case GetType(SpectrumPack) : Return New SpectrumPack(w)
                Case Else
                    Throw New NotImplementedException(GetType(T).FullName)
            End Select
        End Function

    End Module
End Namespace