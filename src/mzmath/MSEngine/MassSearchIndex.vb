Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Algorithm

Public Interface IMassSearch

    ''' <summary>
    ''' Provides a mass value and then populate all related metabolite reference data
    ''' </summary>
    ''' <param name="mass"></param>
    ''' <returns></returns>
    Function QueryByMass(mass As Double) As IEnumerable

End Interface

''' <summary>
''' A simple implements of exact mass search index
''' </summary>
Public Class MassSearchIndex(Of T As IExactMassProvider) : Implements IMassSearch

    ''' <summary>
    ''' mass tolerance value for match sample mz and threocal mz
    ''' </summary>
    ReadOnly tolerance As Tolerance
    ReadOnly massIndex As BlockSearchFunction(Of T)
    ReadOnly activator As Func(Of Double, Object)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mass"></param>
    ''' <param name="activator">
    ''' returns object due to the reason of reflection not working
    ''' well on build a dynamics delegate type
    ''' </param>
    ''' <param name="tolerance">
    ''' tolerance used for filter mass hit, not the tolerance of build search index
    ''' </param>
    Sub New(mass As IEnumerable(Of T), activator As Func(Of Double, Object), tolerance As Tolerance)
        ' 20220512
        '
        ' too small tolerance error will cause too much elements to
        ' sort
        ' and then will cause the error of 
        ' Stack overflow.
        ' Repeat 3075 times: 
        ' --------------------------------
        '   at Microsoft.VisualBasic.ComponentModel.Algorithm.QuickSortFunction
        '
        ' pipeline has been test for MS-imaging data analysis
        '
        Me.massIndex = New BlockSearchFunction(Of T)(
            data:=mass,
            eval:=Function(m) m.ExactMass,
            tolerance:=1.5,
            factor:=3
        )
        Me.activator = activator
        Me.tolerance = tolerance
    End Sub

    Public Function QueryByMass(mass As Double) As IEnumerable(Of T)
        Dim query As T = activator(mass)
        Dim pop = massIndex _
            .Search(query) _
            .Where(Function(d)
                       Return tolerance(d.ExactMass, mass)
                   End Function)

        Return pop
    End Function

    Public Overrides Function ToString() As String
        Return tolerance.ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function IMassSearch_QueryByMass(mass As Double) As IEnumerable Implements IMassSearch.QueryByMass
        Return QueryByMass(mass)
    End Function
End Class
