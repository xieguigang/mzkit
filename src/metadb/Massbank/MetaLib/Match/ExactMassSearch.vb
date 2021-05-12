Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables

Namespace MetaLib

    Public Class ExactMassSearch(Of T As IExactmassProvider)

        ReadOnly index As OrderSelector(Of MassCompares)

        Private Sub New(index As OrderSelector(Of MassCompares))
            Me.index = index
        End Sub

        Public Function QueryByMass(query As Double, ppm As Double) As IEnumerable(Of T)
            Dim da As Double = ChemicalData.PpmToMassDelta(query, ppm)
            Dim min As New MassQuery With {.mass = query - da}
            Dim max As New MassQuery With {.mass = query + da}

            Return index.SelectByRange(New MassCompares(min), New MassCompares(max))
        End Function

        Public Shared Function CreateIndex(data As IEnumerable(Of T)) As ExactMassSearch(Of T)
            Return New ExactMassSearch(Of T)(New OrderSelector(Of MassCompares)(data.Select(Function(a) New MassCompares With {.obj = a})))
        End Function
    End Class

    Friend Structure MassQuery : Implements IExactmassProvider

        Dim mass As Double

        Public ReadOnly Property ExactMass As Double Implements IExactmassProvider.ExactMass
            Get
                Return mass
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return mass.ToString("F4")
        End Function

    End Structure

    Public Structure MassCompares : Implements IComparable(Of Double), IComparable

        Dim obj As IExactmassProvider

        Sub New(x As IExactmassProvider)
            obj = x
        End Sub

        Public Overrides Function ToString() As String
            Return obj.ToString
        End Function

        Public Function CompareTo(other As Double) As Integer Implements IComparable(Of Double).CompareTo
            Return obj.ExactMass.CompareTo(other)
        End Function

        Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            If TypeOf obj Is MassCompares Then
                Return Me.obj.ExactMass.CompareTo(DirectCast(obj, MassCompares).obj.ExactMass)
            Else
                Return -1
            End If
        End Function
    End Structure
End Namespace