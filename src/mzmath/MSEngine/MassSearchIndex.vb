Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Algorithm

''' <summary>
''' A simple implements of mass search index
''' </summary>
Public Class MassSearchIndex(Of T As IExactMassProvider)

    ''' <summary>
    ''' mass tolerance value for match sample mz and threocal mz
    ''' </summary>
    ReadOnly tolerance As Tolerance
    ReadOnly massIndex As BlockSearchFunction(Of T)
    ReadOnly activator As Func(Of Double, T)

    Sub New(mass As IEnumerable(Of T), activator As Func(Of Double, T), tolerance As Tolerance)
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
            tolerance:=1,
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
End Class
