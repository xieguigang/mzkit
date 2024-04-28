#Region "Microsoft.VisualBasic::b4f2f86ddb030b61f2926ec9ed9c8a29, E:/mzkit/src/metadb/Chemoinformatics//Formula/Isotopic/Isotope.vb"

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

    '   Total Lines: 26
    '    Code Lines: 10
    ' Comment Lines: 12
    '   Blank Lines: 4
    '     File Size: 680 B


    '     Class Isotope
    ' 
    '         Properties: Mass, NumNeutrons, Prob
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Formula.IsotopicPatterns

    Public Class Isotope

        ''' <summary>
        ''' Exact Mass
        ''' </summary>
        ''' <returns></returns>
        Public Property Mass As Double
        ''' <summary>
        ''' Natural Relative Abundance
        ''' </summary>
        ''' <returns></returns>
        Public Property Prob As Double
        ''' <summary>
        ''' Nominal Mass
        ''' </summary>
        ''' <returns></returns>
        Public Property NumNeutrons As Integer

        Public Overrides Function ToString() As String
            Return $"[{NumNeutrons}] {Prob}"
        End Function

    End Class
End Namespace
