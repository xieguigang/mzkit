#Region "Microsoft.VisualBasic::9326f4356587219b01e932fcda406536, metadb\Lipidomics\ILipidGenerator.vb"

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

    '   Total Lines: 30
    '    Code Lines: 22
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 1.16 KB


    ' Interface ILipidGenerator
    ' 
    '     Function: CanGenerate, Generate
    ' 
    '     Class LipidGenerator
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: CanGenerate, Generate
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Linq


Public Interface ILipidGenerator
        Function CanGenerate(lipid As ILipid) As Boolean
        Function Generate(lipid As Lipid) As IEnumerable(Of ILipid)
    End Interface

    Public Class LipidGenerator
        Implements ILipidGenerator
        Public Sub New(totalChainGenerator As ITotalChainVariationGenerator)
            Me.totalChainGenerator = totalChainGenerator
        End Sub

        Public Sub New()
            Me.New(New TotalChainVariationGenerator(minLength:=6, begin:=3, [end]:=3, skip:=3))

        End Sub

        Private ReadOnly totalChainGenerator As ITotalChainVariationGenerator

        Public Function CanGenerate(lipid As ILipid) As Boolean Implements ILipidGenerator.CanGenerate
            Return lipid.ChainCount >= 1
        End Function

        Public Function Generate(lipid As Lipid) As IEnumerable(Of ILipid) Implements ILipidGenerator.Generate
            Return lipid.Chains.GetCandidateSets(totalChainGenerator).[Select](Function(chains) New Lipid(lipid.LipidClass, lipid.Mass, chains))
        End Function
    End Class
