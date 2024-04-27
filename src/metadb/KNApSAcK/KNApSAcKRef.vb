#Region "Microsoft.VisualBasic::cb039683b715d160e9d73271796b643c, G:/mzkit/src/metadb/KNApSAcK//KNApSAcKRef.vb"

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

    '   Total Lines: 36
    '    Code Lines: 26
    ' Comment Lines: 4
    '   Blank Lines: 6
    '     File Size: 1.22 KB


    ' Class KNApSAcKRef
    ' 
    '     Properties: CAS, exact_mass, formula, glycosyl, InChI
    '                 InChIKey, name, SMILES, term, xrefId
    ' 
    '     Function: CreateLossElementList, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.Linq

Public Class KNApSAcKRef : Implements IExactMassProvider

    Public Property xrefId As String
    Public Property name As String
    Public Property formula As String
    Public Property exact_mass As Double Implements IExactMassProvider.ExactMass
    Public Property CAS As String
    Public Property SMILES As String
    Public Property InChI As String
    Public Property InChIKey As String

    ''' <summary>
    ''' glycosyl count n
    ''' </summary>
    ''' <returns></returns>
    Public Property glycosyl As String()
    Public Property term As String

    Public Function CreateLossElementList() As Dictionary(Of String, Integer)
        Return glycosyl _
            .SafeQuery _
            .Select(Function(gly) gly.GetTagValue(" ", trim:=True)) _
            .ToDictionary(Function(a) a.Value,
                          Function(a)
                              Return Integer.Parse(a.Name)
                          End Function)
    End Function

    Public Overrides Function ToString() As String
        Return $"[{term}] {name}"
    End Function

End Class
