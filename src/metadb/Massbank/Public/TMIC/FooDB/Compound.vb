#Region "Microsoft.VisualBasic::3554498aee7554ec5bb186a207b76c95, metadb\Massbank\Public\TMIC\FooDB\Compound.vb"

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
    '    Code Lines: 33 (82.50%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (17.50%)
    '     File Size: 1.39 KB


    '     Class Compound
    ' 
    '         Properties: annotation_quality, cas_number, description, id, kingdom
    '                     klass, moldb_inchi, moldb_inchikey, moldb_iupac, moldb_mono_mass
    '                     moldb_smiles, name, public_id, state, subklass
    '                     superklass
    ' 
    '         Function: LoadJSONDb, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TMIC.FooDB

    Public Class Compound

        Public Property id As String
        Public Property public_id As String
        Public Property name As String
        Public Property state As String
        Public Property annotation_quality As String
        Public Property description As String
        Public Property cas_number As String
        Public Property moldb_smiles As String
        Public Property moldb_inchi As String
        Public Property moldb_mono_mass As String
        Public Property moldb_inchikey As String
        Public Property moldb_iupac As String
        Public Property kingdom As String
        Public Property superklass As String
        Public Property klass As String
        Public Property subklass As String

        Public Overrides Function ToString() As String
            Return $"[{public_id}] {name}"
        End Function

        Public Shared Iterator Function LoadJSONDb(file As Stream) As IEnumerable(Of Compound)
            Dim s As New StreamReader(file)
            Dim str As Value(Of String) = ""

            Do While Not (str = s.ReadLine) Is Nothing
                Yield CStr(str).LoadJSON(Of Compound)
            Loop
        End Function

    End Class
End Namespace
