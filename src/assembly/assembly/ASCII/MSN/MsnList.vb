#Region "Microsoft.VisualBasic::d594a29fe27c44a5564365b1714b7757, assembly\ASCII\MSN\MsnList.vb"

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

    '     Class MsnList
    ' 
    '         Properties: activation, c, id, instrument, ionMode
    '                     ms2, msn, mz_range, type
    ' 
    '         Function: GetMsnList, MsnList, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Emit.Marshal
Imports ANSI = Microsoft.VisualBasic.Text.ASCII

Namespace ASCII.MSN

    Public Class MsnList

        Public Property id As String
        Public Property msn As String
        Public Property type As String
        Public Property ionMode As String
        Public Property c As String
        Public Property instrument As String
        Public Property activation As String
        Public Property mz_range As DoubleRange
        Public Property ms2 As ms2()

        Public Overrides Function ToString() As String
            Return id
        End Function

        Friend Shared Function GetMsnList(file As String) As MsnList()
            Dim ions As MsnList() = file _
                .IterateAllLines _
                .SkipWhile(Function(l) l.First = "#"c) _
                .Split(Function(line) line.First = ">"c, DelimiterLocation.NextFirst) _
                .Where(Function(b) Not b.IsNullOrEmpty) _
                .Select(AddressOf MsnList) _
                .ToArray

            Return ions
        End Function

        Private Shared Function MsnList(block As String()) As MsnList
            Dim i As Pointer(Of String) = block(Scan0).Split(ANSI.TAB)
            Dim ms2 As ms2() = block _
                .Skip(1) _
                .Select(Function(l)
                            Dim mzInto As Double() = l _
                                .Split(ANSI.TAB) _
                                .Select(Function(c) Val(c)) _
                                .ToArray
                            Dim fragment As New ms2 With {
                                .mz = mzInto(Scan0),
                                .intensity = mzInto(1),
                                .quantity = .intensity
                            }

                            Return fragment
                        End Function) _
                .ToArray

            Return New MsnList With {
                .id = ++i,
                .msn = ++i,
                .type = ++i,
                .ionMode = ++i,
                .c = ++i,
                .instrument = ++i,
                .activation = ++i,
                .mz_range = (++i) _
                    .Split("-"c) _
                    .Select(AddressOf Val) _
                    .ToArray,
                .ms2 = ms2
            }
        End Function

    End Class
End Namespace
