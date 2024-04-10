Imports System.Reflection
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS.AtomGroups
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Formula.MS

    ''' <summary>
    ''' This class is the storage of product ion assignment used in MS-FINDER program.
    ''' </summary>
    Public Class ProductIon

        Public Property Mass As Double

        Public Property Intensity As Double

        Public Property Formula As Formula

        Public Property IonMode As IonModes

        Public Property Smiles As String

        Public Property MassDiff As Double

        Public Property IsotopeDiff As Double

        Public Property Comment As String

        Public Property Name As String

        Public Property ShortName As String

        Public Property CandidateInChIKeys As List(Of String) = New List(Of String)()

        Public Property Frequency As Double

        Public Property CandidateOntologies As List(Of String) = New List(Of String)()

        Public Shared Iterator Function GetDefault() As IEnumerable(Of ProductIon)
            For Each container As Type In {
                GetType(Alkenyl), GetType(Alkyl), GetType(Amines),
                GetType(Glycosides), GetType(Ketones), GetType(Lipids),
                GetType(Others)
            }
                For Each atom_group As PropertyInfo In container.GetProperties(DataFramework.PublicShared)
                    Yield New ProductIon With {
                       .Formula = atom_group.GetValue(Nothing),
                       .IonMode = IonModes.Positive,
                       .Name = atom_group.Name,
                       .ShortName = .Name,
                       .Mass = .Formula.ExactMass
                    }

                    Yield New ProductIon With {
                       .Formula = atom_group.GetValue(Nothing),
                       .IonMode = IonModes.Negative,
                       .Name = atom_group.Name,
                       .ShortName = .Name,
                       .Mass = .Formula.ExactMass
                    }
                Next
            Next
        End Function
    End Class

    ''' <summary>
    ''' This class is the storage of each neutral loss information used in MS-FINDER program.
    ''' </summary>
    Public Class NeutralLoss

        Public Property MassLoss As Double

        Public Property PrecursorMz As Double

        Public Property ProductMz As Double

        Public Property PrecursorIntensity As Double

        Public Property ProductIntensity As Double

        Public Property MassError As Double

        Public Property Formula As Formula = New Formula()

        Public Property Iontype As IonModes

        Public Property Comment As String

        Public Property Smiles As String

        Public Property Frequency As Double

        Public Property Name As String

        Public Property ShortName As String

        Public Property CandidateInChIKeys As List(Of String) = New List(Of String)()

        Public Property CandidateOntologies As List(Of String) = New List(Of String)()

    End Class
End Namespace