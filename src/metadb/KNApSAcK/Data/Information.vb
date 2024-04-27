#Region "Microsoft.VisualBasic::0935adcd3a70edfaf718cffffff51fc9, G:/mzkit/src/metadb/KNApSAcK//Data/Information.vb"

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

    '   Total Lines: 37
    '    Code Lines: 24
    ' Comment Lines: 8
    '   Blank Lines: 5
    '     File Size: 1.09 KB


    '     Class Information
    ' 
    '         Properties: Biosynthesis, CAS, CID, formula, img
    '                     InChICode, InChIKey, mw, name, Organism
    '                     query, SMILES
    ' 
    '         Function: ToString
    ' 
    '     Class Organism
    ' 
    '         Properties: Family, Kingdom, Species
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Data

    Public Class Information

        Public Property name As String()
        Public Property formula As String
        Public Property mw As Double
        Public Property CAS As String()
        Public Property CID As String
        Public Property InChIKey As String
        Public Property InChICode As String
        Public Property SMILES As String
        Public Property Biosynthesis As String
        Public Property Organism As Organism()
        ''' <summary>
        ''' data uri
        ''' </summary>
        ''' <returns></returns>
        Public Property img As String
        ''' <summary>
        ''' the query source keyword
        ''' </summary>
        ''' <returns></returns>
        Public Property query As String

        Public Overrides Function ToString() As String
            Return name(Scan0)
        End Function

    End Class

    Public Class Organism
        Public Property Kingdom As String
        Public Property Family As String
        Public Property Species As String
    End Class
End Namespace
