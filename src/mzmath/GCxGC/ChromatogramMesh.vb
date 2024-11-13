Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Scripting.Expressions

''' <summary>
''' A 2d chromatogram mesh grid data that consists with the GCxGC chromatogram data
''' </summary>
Public Class ChromatogramMesh

    Public Property scan2D As Chromatogram2DScan()

End Class

Public Class Chromatogram2DScan : Implements IReadOnlyId, INamedValue
    Public Property scan_time As Double
    Public Property intensity As Double
    Public Property scan_id As String Implements INamedValue.Key, IReadOnlyId.Identity

    ''' <summary>
    ''' chromatogram data 2d
    ''' </summary>
    ''' <returns></returns>
    Public Property chromatogram As ChromatogramTick()

    Default Public ReadOnly Property getTick(i As DoubleRange) As ChromatogramTick()
        Get
            Return chromatogram.Where(Function(a) i.IsInside(a.Time)).ToArray
        End Get
    End Property

    Default Public ReadOnly Property getTick(i As Integer) As ChromatogramTick
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _chromatogram(i)
        End Get
    End Property

    Public ReadOnly Property size As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return chromatogram.Length
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(t1 As Double)
        scan_time = t1
    End Sub

    Sub New(t1 As Double, id As String)
        scan_id = id
        scan_time = t1
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function times() As Double()
        Return chromatogram.Select(Function(t) t.Time).ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"{intensity.ToString("G3")}@{scan_time.ToString("F2")}"
    End Function
End Class