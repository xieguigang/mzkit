Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.TablePrinter
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.TablePrinter.Flags
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' digest
''' </summary>
Public Class TheoreticalDigestMass

    ' outputwrite(0, 0) = "Sequence"
    ' outputwrite(0, 1) = "Start"
    ' outputwrite(0, 2) = "End"
    ' outputwrite(0, 3) = "Length"
    ' outputwrite(0, 4) = "5' End"
    ' outputwrite(0, 5) = "3' End"
    ' outputwrite(0, 6) = "Theoretical Mass"
    ' outputwrite(0, 7) = "Name"

    ''' <summary>
    ''' 1
    ''' </summary>
    <Column("Sequence")> Public Property Sequence As String
    ''' <summary>
    ''' 2
    ''' </summary>
    <Column("Start")> Public Property Start As Integer
    ''' <summary>
    ''' 3
    ''' </summary>
    <Column("End")> Public Property Ends As Integer
    ''' <summary>
    ''' 4
    ''' </summary>
    <Column("Length")> Public Property Length As Integer
    ''' <summary>
    ''' 5
    ''' </summary>
    <Column("5' End")> Public Property End5 As String
    ''' <summary>
    ''' 6
    ''' </summary>
    <Column("3' End")> Public Property End3 As String
    ''' <summary>
    ''' 7
    ''' </summary>
    <Column("Theoretical Mass")> Public Property TheoreticalMass As Double
    ''' <summary>
    ''' 8
    ''' </summary>
    <Column("Name")> Public Property Name As String

    Default Public ReadOnly Property Item(i As Integer) As Object
        Get
            Select Case i
                Case 1 : Return Sequence
                Case 2 : Return Start
                Case 3 : Return Ends
                Case 4 : Return Length
                Case 5 : Return End5
                Case 6 : Return End3
                Case 7 : Return TheoreticalMass
                Case 8 : Return Name

                Case Else
                    Throw New NotImplementedException(i)
            End Select
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Sequence}({Length}bits) [{Start}...{Ends}] 5':{End5} 3':{End3}  {TheoreticalMass}@{Name}"
    End Function

    ''' <summary>
    ''' print table
    ''' </summary>
    ''' <param name="outputs"></param>
    ''' <param name="dev"></param>
    Public Shared Sub Print(outputs As IEnumerable(Of TheoreticalDigestMass), dev As TextWriter)
        Dim content As ConsoleTableBaseData = ConsoleTableBaseData.FromColumnHeaders(
            "Sequence", "Start", "End", "Length", "5' End", "3' End", "Theoretical Mass", "Name"
        )

        For Each hit As TheoreticalDigestMass In outputs
            Call content.AppendLine(
                hit.Sequence, hit.Start, hit.Ends, hit.Length,
                hit.End5, hit.End3,
                hit.TheoreticalMass,
                hit.Name
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
