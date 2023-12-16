Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.TablePrinter
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.TablePrinter.Flags
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Linq

Public Class Match

    ''' <summary>
    ''' 1
    ''' </summary>
    <Column("Observed Mass")> Public Property ObservedMass As Double
    ''' <summary>
    ''' 2
    ''' </summary>
    <Column("Sequence")> Public Property Sequence As String
    ''' <summary>
    ''' 3
    ''' </summary>
    <Column("Start")> Public Property Start As Integer
    ''' <summary>
    ''' 4
    ''' </summary>
    <Column("End")> Public Property Ends As Integer
    ''' <summary>
    ''' 5
    ''' </summary>
    <Column("Length")> Public Property Length As Integer
    ''' <summary>
    ''' 6
    ''' </summary>
    <Column("5' End")> Public Property End5 As String
    ''' <summary>
    ''' 7
    ''' </summary>
    <Column("3' End")> Public Property End3 As String
    ''' <summary>
    ''' 8
    ''' </summary>
    <Column("Adduct")> Public Property Adduct As String
    ''' <summary>
    ''' 9
    ''' </summary>
    <Column("Theoretical Mass")> Public Property TheoreticalMass As Double
    ''' <summary>
    ''' 10
    ''' </summary>
    <Column("Error (ppm)")> Public Property Errorppm As Double
    ''' <summary>
    ''' 11
    ''' </summary>
    <Column("Name")> Public Property Name As String
    ''' <summary>
    ''' 12
    ''' </summary>
    <Column("Frequency")> Public Property Frequency As Double
    ''' <summary>
    ''' 13
    ''' </summary>
    <Column("1st Occurance")> Public Property f1StOccurance As String

    ''' <summary>
    ''' print table
    ''' </summary>
    ''' <param name="outputs"></param>
    ''' <param name="dev"></param>
    Public Shared Sub Print(outputs As IEnumerable(Of Match), dev As TextWriter)
        Dim content As ConsoleTableBaseData = ConsoleTableBaseData.FromColumnHeaders(
            "Observed Mass", "Sequence", "Start", "End", "Length", "5' End", "3' End",
            "Adduct", "Theoretical Mass", "Error (ppm)",
            "Name", "Frequency", "1st Occurance"
        )

        For Each hit As Match In outputs
            Call content.AppendLine(
                hit.ObservedMass, hit.Sequence, hit.Start, hit.Ends, hit.Length, hit.End5, hit.End3,
                hit.Adduct, hit.TheoreticalMass, hit.Errorppm,
                hit.Name, hit.Frequency, hit.f1StOccurance
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
