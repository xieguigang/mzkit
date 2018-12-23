#Region "Microsoft.VisualBasic::9589414a62f5f6849269d0dc64cdb1bc, Massbank\Public\TMIC\HMDB\MetaReference\RepositoryExtensions.vb"

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

    '     Module RepositoryExtensions
    ' 
    '         Function: EnumerateNames, PopulateHMDBMetaData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports XmlLinq = Microsoft.VisualBasic.Text.Xml.Linq.Data

Namespace TMIC.HMDB.Repository

    Public Module RepositoryExtensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function PopulateHMDBMetaData(Xml As String) As IEnumerable(Of MetaReference)
            Return XmlLinq.LoadXmlDataSet(Of MetaReference)(
                XML:=Xml,
                typeName:=NameOf(metabolite),
                xmlns:="http://www.hmdb.ca",
                forceLargeMode:=True
            )
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function EnumerateNames(metabolite As MetaReference) As IEnumerable(Of String)
            Return {metabolite.name}.AsList +
                metabolite.synonyms.synonym +
                metabolite.iupac_name
        End Function
    End Module
End Namespace
