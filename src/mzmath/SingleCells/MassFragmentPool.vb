Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Public Class MassFragmentPool : Implements INamedValue, IMassFragmentPool

    Public Property sample As String Implements INamedValue.Key, IMassFragmentPool.sample
    Public Property fragments As ms2() Implements IMassFragmentPool.fragments

    Public Overrides Function ToString() As String
        Return sample
    End Function

End Class

Public Interface IMassFragmentPool

    ReadOnly Property sample As String
    ReadOnly Property fragments As ms2()

End Interface

Public Class MSnFragmentProvider
    Implements IMassFragmentPool

    Public ReadOnly Property sample As String Implements IMassFragmentPool.sample
        Get
            Return raw.source
        End Get
    End Property

    Public ReadOnly Property fragments As ms2() Implements IMassFragmentPool.fragments
        Get
            Return raw.MS _
                .Select(Function(m1) m1.products) _
                .IteratesALL _
                .Select(Function(m2) m2.GetMs) _
                .IteratesALL _
                .ToArray
        End Get
    End Property

    ReadOnly raw As IMZPack

    Sub New(raw As IMZPack)
        Me.raw = raw
    End Sub

    Public Overrides Function ToString() As String
        Return sample
    End Function

End Class