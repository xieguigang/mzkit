#Region "Microsoft.VisualBasic::6b871aff6c7af5691eaba59eb670b458, metadb\Massbank\Public\TMIC\HMDB\Tables\Model.vb"

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

    '   Total Lines: 39
    '    Code Lines: 30 (76.92%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (23.08%)
    '     File Size: 1.43 KB


    '     Class MetaInfo
    ' 
    '         Properties: CAS, chebi, HMDB, KEGG
    ' 
    '     Class BriefTable
    ' 
    '         Properties: AdultConcentrationAbnormal, AdultConcentrationNormal, ChildrenConcentrationAbnormal, ChildrenConcentrationNormal, disease
    '                     NewbornConcentrationAbnormal, NewbornConcentrationNormal, Sample, water_solubility
    ' 
    '         Function: Clone
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace TMIC.HMDB

    Public Class MetaInfo : Inherits MetaLib.Models.MetaInfo

        Public Property HMDB As String
        Public Property KEGG As String
        Public Property chebi As String
        Public Property CAS As String

    End Class

    Public Class BriefTable : Inherits MetaInfo
        Implements ICloneable

        Public Property Sample As String
        Public Property disease As String()
        Public Property water_solubility As String

        <Column("concentration (children-normal)")>
        Public Property ChildrenConcentrationNormal As String()
        <Column("concentration (children-abnormal)")>
        Public Property ChildrenConcentrationAbnormal As String()
        <Column("concentration (adult-normal)")>
        Public Property AdultConcentrationNormal As String()
        <Column("concentration (adult-abnormal)")>
        Public Property AdultConcentrationAbnormal As String()
        <Column("concentration (newborn-normal)")>
        Public Property NewbornConcentrationNormal As String()
        <Column("concentration (newborn-abnormal)")>
        Public Property NewbornConcentrationAbnormal As String()

        Public Function Clone() As Object Implements ICloneable.Clone
            Return MemberwiseClone()
        End Function
    End Class

End Namespace
