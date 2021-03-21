#Region "Microsoft.VisualBasic::64333acffeacdcfb35d10b90a1fdd718, src\mzmath\TargetedMetabolomics\MRM\IsomerismIonPairs.vb"

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

    '     Class IsomerismIonPairs
    ' 
    '         Properties: hasIsomerism, index, ions, target
    ' 
    '         Function: GetEnumerator, groupKey, IEnumerable_GetEnumerator, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

Namespace MRM.Models

    Public Class IsomerismIonPairs : Implements IEnumerable(Of IonPair)

        Public Property ions As IonPair()
        Public Property target As IonPair

        ''' <summary>
        ''' Get the chromatogram overlaps index by rt/ri order
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property index As Integer
            Get
                If ions.IsNullOrEmpty Then
                    Return Scan0
                Else
                    Dim vec As IonPair() = Me.ToArray

                    For i As Integer = 0 To vec.Length - 1
                        If vec(i).accession = target.accession Then
                            Return i
                        End If
                    Next

                    Throw New InvalidProgramException
                End If
            End Get
        End Property

        Public ReadOnly Property hasIsomerism As Boolean
            Get
                Return Not ions.IsNullOrEmpty
            End Get
        End Property

        Friend Function groupKey() As String
            Return Me.Select(Function(i) i.accession).JoinBy("|->|")
        End Function

        Public Overrides Function ToString() As String
            If hasIsomerism Then
                Return $"[{index}, rt:{target.rt} (sec)] {target}"
            Else
                Return target.ToString
            End If
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of IonPair) Implements IEnumerable(Of IonPair).GetEnumerator
            For Each i In ions.Join(target).OrderBy(Function(ion) ion.rt)
                Yield i
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
