#Region "Microsoft.VisualBasic::261cabde8980fe51cf59c8e4d01e5e40, G:/mzkit/src/mzmath/Oligonucleotide_MS//MS_Peak/TheoreticalDigestMass.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 104
    '    Code Lines: 54
    ' Comment Lines: 40
    '   Blank Lines: 10
    '     File Size: 3.28 KB


    ' Class TheoreticalDigestMass
    ' 
    '     Properties: End3, End5, Ends, Length, Name
    '                 Sequence, Start, TheoreticalMass
    ' 
    '     Function: ToString
    ' 
    '     Sub: Print
    ' 
    ' /********************************************************************************/

#End Region

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
