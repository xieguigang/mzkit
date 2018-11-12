Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Public Class GCMSJson

    Public Property name As String
    Public Property times As Double()
    Public Property tic As Double()
    Public Property ms As ms1_scan()()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetTIC() As NamedCollection(Of ChromatogramTick)
        Return New NamedCollection(Of ChromatogramTick) With {
            .Name = name,
            .Value = times _
                .Select(Function(time, i)
                            Return New ChromatogramTick(time, tic(i))
                        End Function) _
                .ToArray
        }
    End Function
End Class
