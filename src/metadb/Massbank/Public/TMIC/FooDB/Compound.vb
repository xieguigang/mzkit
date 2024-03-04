#Region "Microsoft.VisualBasic::2ad778be8ab671197d9d39e29b82bcad, mzkit\src\metadb\FooDB\FooDB\Compound.vb"

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

'   Total Lines: 22
'    Code Lines: 21
' Comment Lines: 0
'   Blank Lines: 1
'     File Size: 808 B


' Class Compound
' 
'     Properties: annotation_quality, cas_number, description, id, kingdom
'                 klass, moldb_inchi, moldb_inchikey, moldb_iupac, moldb_mono_mass
'                 moldb_smiles, name, public_id, state, subklass
'                 superklass
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

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
    End Class
End Namespace