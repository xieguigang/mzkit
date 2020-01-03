#Region "Microsoft.VisualBasic::a557dbdf3dbadacaa85c1e7503c0b9fd, src\assembly\assembly\ASCII\MSN\FileReader.vb"

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

    '     Module FileReader
    ' 
    '         Function: GetIons
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ASCII.MSN

    Public Module FileReader

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data">File path of the ``*.msn-list.txt``</param>
        ''' <returns></returns>
        Public Iterator Function GetIons(data As String) As IEnumerable(Of MGF.Ions)
            Dim metaPeaksFile As String = $"{data.ParentPath}/{data.BaseName.BaseName}.peak-table.txt"
            Dim peaks As PeakTable() = PeakTable.ParseTable(metaPeaksFile)
            Dim ions As Dictionary(Of String, MsnList) = MsnList.GetMsnList(data).ToDictionary(Function(p) p.id)
            Dim ionMsn As MsnList

            For Each peak As PeakTable In peaks
                ionMsn = ions.TryGetValue(peak.id)

                If ionMsn Is Nothing Then
                    Call $"Missing msn peaks for '{peak.id}'!".Warning
                    Continue For
                End If

                Dim ion As New MGF.Ions With {
                    .Peaks = ionMsn.ms2,
                    .Accession = peak.id
                }

                Yield ion
            Next
        End Function
    End Module
End Namespace
