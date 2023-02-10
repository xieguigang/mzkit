#Region "Microsoft.VisualBasic::a939207789fc643b3f3360b28b259503, mzkit\src\mzmath\ms2_math-core\Ms1\Annotation\ExactMassProvider.vb"

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

    '   Total Lines: 20
    '    Code Lines: 11
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 360 B


    '     Interface IExactMassProvider
    ' 
    '         Properties: ExactMass
    ' 
    '     Interface ICompoundNameProvider
    ' 
    '         Properties: CommonName
    ' 
    '     Interface IFormulaProvider
    ' 
    '         Properties: Formula
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Ms1.Annotations

    Public Interface IExactMassProvider

        ReadOnly Property ExactMass As Double

    End Interface

    Public Interface ICompoundNameProvider

        ReadOnly Property CommonName As String

    End Interface

    Public Interface IFormulaProvider

        ReadOnly Property Formula As String

    End Interface
End Namespace
