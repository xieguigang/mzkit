Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.TablePrinter
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.TablePrinter.Flags
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Linq

Public Class OligonucleotideCompositionOutput

    ''' <summary>
    ''' 1
    ''' </summary>
    <Column("Observed Mass")> Public Property ObservedMass As Double
    ''' <summary>
    ''' 2
    ''' </summary>
    <Column("Theoretical Mass")> Public Property TheoreticalMass As Double
    ''' <summary>
    ''' 3
    ''' </summary>
    <Column("Error (ppm)")> Public Property ErrorPpm As Double
    ''' <summary>
    ''' 4
    ''' </summary>
    <Column("# of pA")> Public Property OfpA As Integer
    ''' <summary>
    ''' 5
    ''' </summary>
    <Column("# of pG")> Public Property OfpG As Integer
    ''' <summary>
    ''' 6
    ''' </summary>
    <Column("# of pC")> Public Property OfpC As Integer
    ''' <summary>
    ''' 7
    ''' </summary>
    <Column("# of pV")> Public Property OfpV As Integer
    ''' <summary>
    ''' 8
    ''' </summary>
    <Column("Modification")> Public Property Modification As String
    ''' <summary>
    ''' 10
    ''' </summary>
    <Column("# of Bases")> Public Property OfBases As Integer

    Friend Sub SetBaseNumber(i As Integer, n As Integer)
        Select Case i
            Case 1 : OfpA = n
            Case 2 : OfpG = n
            Case 3 : OfpC = n
            Case 4 : OfpV = n

            Case Else
                Throw New OutOfMemoryException(i)
        End Select
    End Sub

    ''' <summary>
    ''' print table
    ''' </summary>
    ''' <param name="outputs"></param>
    ''' <param name="dev"></param>
    Public Shared Sub Print(outputs As IEnumerable(Of OligonucleotideCompositionOutput), dev As TextWriter)
        Dim content As ConsoleTableBaseData = ConsoleTableBaseData.FromColumnHeaders(
            "Observed Mass", "Theoretical Mass", "Error (ppm)",
            "# of pA", "# of pG", "# of pC", "# of pV", "Modification",
            "",
            "# of Bases"
        )

        For Each hit As OligonucleotideCompositionOutput In outputs
            Call content.AppendLine(
                hit.ObservedMass, hit.TheoreticalMass, hit.ErrorPpm,
                hit.OfpA, hit.OfpG, hit.OfpC, hit.OfpV, hit.Modification,
                "",
                hit.OfBases
            )
        Next

        Call ConsoleTableBuilder _
            .From(content) _
            .WithFormat(ConsoleTableBuilderFormat.Minimal) _
            .Export _
            .ToString() _
            .DoCall(AddressOf dev.WriteLine)
        Call dev.Flush()
    End Sub

End Class