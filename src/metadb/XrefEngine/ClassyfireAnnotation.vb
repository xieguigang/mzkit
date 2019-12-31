#Region "Microsoft.VisualBasic::e649f915e9fb6a9ee6ff4dab0ff61fb5, DATA\XrefEngine\ClassyfireAnnotation.vb"

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

    ' Class ClassyfireAnnotation
    ' 
    '     Properties: ChemOntID, CompoundID, ParentName, Smiles
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region


''' <summary>
''' Parser for file download from https://jcheminf.biomedcentral.com/articles/10.1186/s13321-016-0174-y
''' </summary>
Public Class ClassyfireAnnotation

    ''' <summary>
    ''' Compound id in given database.
    ''' </summary>
    ''' <returns></returns>
    Public Property CompoundID As String
    Public Property Smiles As String
    ''' <summary>
    ''' 分类的term id
    ''' </summary>
    ''' <returns></returns>
    Public Property ChemOntID As String
    Public Property ParentName As String

    Public Overrides Function ToString() As String
        Return $"Dim {CompoundID} As {ChemOntID} = ""{ParentName}"""
    End Function
End Class

