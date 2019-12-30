#Region "Microsoft.VisualBasic::d91754733471e431e1fe192d8e9d1b17, DATA\TargetedMetabolomics\MRM\RawFile.vb"

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

    '     Class RawFile
    ' 
    '         Properties: samples, standards
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MRM.Models

    ''' <summary>
    ''' mzML raw files from wiff converts output
    ''' </summary>
    Public Class RawFile

        Public Property samples As String()
        Public Property standards As String()

        Public ReadOnly Property allSamples As String()
            Get
                Return standards.AsList + samples
            End Get
        End Property

        Sub New(directory$, Optional patternOfRefer$ = ".+[-]CAL[-]?\d+")
            Dim mzML As String() = directory _
                .ListFiles("*.mzML") _
                .ToArray

            standards = mzML _
                .Where(Function(path)
                           Return hasPatternOf(path, patternOfRefer)
                       End Function) _
                .ToArray
            samples = mzML _
                .Where(Function(path)
                           Return Not hasPatternOf(path, patternOfRefer)
                       End Function) _
                .ToArray
        End Sub

        Private Shared Function hasPatternOf(path$, pattern As String) As Boolean
            Return Not path _
                .BaseName _
                .Match(pattern, RegexICSng) _
                .StringEmpty
        End Function

        Public Overrides Function ToString() As String
            Return $"{samples.Length} samples with {standards.Length} reference point."
        End Function
    End Class
End Namespace
