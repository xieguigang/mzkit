Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace GCMS

    ''' <summary>
    ''' 定量离子模型
    ''' </summary>
    Public Class QuantifyIon : Implements INamedValue

        Public Property id As String Implements INamedValue.Key
        Public Property rt As DoubleRange
        ''' <summary>
        ''' 保留指数
        ''' </summary>
        ''' <returns></returns>
        Public Property ri As Double
        Public Property ms As ms2()

        Public Overrides Function ToString() As String
            Return $"Dim {id} As [{rt.Min}, {rt.Max}]"
        End Function

        Public Shared Function FromIons(ions As IEnumerable(Of MSLIon), rtwin As Double) As IEnumerable(Of QuantifyIon)
            Return ions _
                .Select(Function(ion)
                            Return New QuantifyIon With {
                                .id = ion.Name,
                                .ms = ion.Peaks,
                                .rt = New DoubleRange(ion.RT - rtwin, ion.RT + rtwin)
                            }
                        End Function)
        End Function

    End Class
End Namespace