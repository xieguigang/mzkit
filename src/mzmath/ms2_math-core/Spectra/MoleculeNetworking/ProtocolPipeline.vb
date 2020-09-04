
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class ProtocolPipeline

    ReadOnly protocol As Protocols
    ReadOnly progress As Action(Of String)

    ReadOnly raw As PeakMs2()

    Dim nodes As IEnumerable(Of NetworkingNode)

    Sub New(protocol As Protocols, raw As PeakMs2(), progress As Action(Of String))
        Me.raw = raw
        Me.protocol = protocol
        Me.progress = progress
    End Sub

    Public Function ProduceNodes() As ProtocolPipeline
        nodes = protocol.ProduceNodes(raw)
        Return Me
    End Function

    Public Iterator Function Networking(Of T As {New, INamedValue, DynamicPropertyBase(Of Double)})() As IEnumerable(Of T)
        For Each row As NamedValue(Of Dictionary(Of String, Double)) In protocol.Networking(nodes, progress)
            Dim obj As New T With {.Key = row.Name}

            For Each homologous In row.Value
                Call obj.Add(homologous.Key, homologous.Value)
            Next

            Yield obj
        Next
    End Function
End Class