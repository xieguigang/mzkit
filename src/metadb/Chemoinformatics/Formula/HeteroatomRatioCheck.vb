#Region "Microsoft.VisualBasic::669da9f698064d603a7aaa94b1b1c43f, src\metadb\Chemoinformatics\Formula\HeteroatomRatioCheck.vb"

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

    '     Module HeteroatomRatioCheck
    ' 
    '         Function: HeteroatomRatioCheck
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Formula

    Module HeteroatomRatioCheck

        <Extension>
        Public Function HeteroatomRatioCheck(formula As Formula) As Boolean
            Dim C As Integer = formula!C

            If C = 0 Then
                ' 不支持这种检查规则，则跳过
                Return True
            End If

            If formula!H / C > 6 Then Return False
            If formula!F / C > 6 Then Return False
            If formula!Cl / C > 2 Then Return False
            If formula!Br / C > 2 Then Return False
            If formula!N / C > 4 Then Return False
            If formula!O / C > 3 Then Return False
            If formula!P / C > 2 Then Return False
            If formula!S / C > 3 Then Return False
            If formula!Si / C > 1 Then Return False

            Return True
        End Function
    End Module
End Namespace
