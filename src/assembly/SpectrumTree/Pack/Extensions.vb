
Imports System.IO
Imports System.Runtime.CompilerServices

Namespace PackLib

    Public Module Extensions

        ''' <summary>
        ''' open reference database file writer
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="file"></param>
        ''' <param name="truncated">
        ''' clear all file content data after open the target file stream?
        ''' </param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Open(Of T As IReferencePack)(file As String, Optional truncated As Boolean = True) As IReferencePack
            Return Open(file, format:=GetType(T), truncated)
        End Function

        Public Function Open(file As String, ByRef format As Type, Optional truncated As Boolean = True) As IReferencePack
            Dim w As Stream = file.Open(FileMode.OpenOrCreate, doClear:=truncated, [readOnly]:=False)

            Select Case format
                Case GetType(MgfPack) : Return New MgfPack(w)
                Case GetType(SpectrumPack) : Return New SpectrumPack(w)
                Case Else
                    Throw New NotImplementedException(format.FullName)
            End Select
        End Function

    End Module
End Namespace