#Region "Microsoft.VisualBasic::734968d82192cfce8edc985f52eb0f8b, mzmath\Oligonucleotide_MS\OligonucleotideComposition\OligonucleotideCompositionOutput.vb"

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

    '   Total Lines: 89
    '    Code Lines: 49 (55.06%)
    ' Comment Lines: 32 (35.96%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (8.99%)
    '     File Size: 2.80 KB


    ' Class OligonucleotideCompositionOutput
    ' 
    '     Properties: ErrorPpm, Modification, ObservedMass, OfBases, OfpA
    '                 OfpC, OfpG, OfpV, TheoreticalMass
    ' 
    '     Sub: Print, SetBaseNumber
    ' 
    ' /********************************************************************************/

#End Region

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
