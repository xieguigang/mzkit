#Region "Microsoft.VisualBasic::e18a6eb0f20c3c5dd16a930b3f061620, metadb\Chemoinformatics\GenericCompound.vb"

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

    '   Total Lines: 40
    '    Code Lines: 18 (45.00%)
    ' Comment Lines: 14 (35.00%)
    '    - Xml Docs: 92.86%
    ' 
    '   Blank Lines: 8 (20.00%)
    '     File Size: 1.26 KB


    ' Interface GenericCompound
    ' 
    ' 
    ' 
    ' Interface IMetabolite
    ' 
    '     Properties: CrossReference
    ' 
    ' Interface ICompoundClass
    ' 
    '     Properties: [class], kingdom, molecular_framework, sub_class, super_class
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' a generic compound model for metaDNA modelling
''' </summary>
''' <remarks>
''' A union of the abstract interface model:
''' 
''' + <see cref="IReadOnlyId"/>: for the unique database reference id;
''' + <see cref="IExactMassProvider"/>: for exact mass value of the metabolite
''' + <see cref="ICompoundNameProvider"/>: for the metabolite common name
''' + <see cref="IFormulaProvider"/>: for provides the chemical formula string
''' </remarks>
Public Interface GenericCompound
    Inherits IReadOnlyId
    Inherits IExactMassProvider
    Inherits ICompoundNameProvider
    Inherits IFormulaProvider

End Interface

Public Interface IMetabolite(Of T As ICrossReference) : Inherits GenericCompound, ICompoundClass

    Property CrossReference As T

End Interface

''' <summary>
''' 主要是取自HMDB数据库之中的代谢物分类信息
''' </summary>
Public Interface ICompoundClass

    Property kingdom As String
    Property super_class As String
    Property [class] As String
    Property sub_class As String
    Property molecular_framework As String

End Interface
