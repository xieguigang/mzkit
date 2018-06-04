Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text

''' <summary>
''' Spectra matrix encoder helper for mysql/csv
''' </summary>
Public Module SpectraEncoder

    Public Delegate Function Encoder(Of T)(mzData As T()) As String

    Public Function GetEncoder(Of T)(getX As Func(Of T, Double), getY As Func(Of T, Double)) As Encoder(Of T)
        Return Function(matrix As T()) As String
                   Dim table$ = matrix _
                       .Select(Function(m)
                                   Return {getX(m), getY(m)}.JoinBy(ASCII.TAB)
                               End Function) _
                       .JoinBy(vbCrLf)
                   Dim bytes As Byte() = TextEncodings.UTF8WithoutBOM.GetBytes(table)
                   Dim base64$ = bytes.ToBase64String

                   Return base64
               End Function
    End Function

    Public Function Decode(base64 As String) As (x#, y#)()
        Dim bytes As Byte() = Convert.FromBase64String(base64)
        Dim table$ = TextEncodings.UTF8WithoutBOM.GetString(bytes)
        Dim fragments$() = table.LineTokens
        Dim matrix = fragments _
            .Select(Function(r)
                        With r.Split(ASCII.TAB)
                            Return (x:=Val(.ByRef(0)), y:=Val(.ByRef(1)))
                        End With
                    End Function) _
            .ToArray

        Return matrix
    End Function
End Module
