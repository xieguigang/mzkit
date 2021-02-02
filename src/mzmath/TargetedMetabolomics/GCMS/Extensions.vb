#Region "Microsoft.VisualBasic::88ece210996cd260620bff2f9853c3b2, TargetedMetabolomics\GCMS\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: ContentTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace GCMS

    Public Module Extensions

        Public Function ContentTable(msl As IEnumerable(Of MSLIon),
                                     contents As Dictionary(Of String, Double),
                                     [IS] As Dictionary(Of String, String)) As ContentTable

            Dim ionLevels As New Dictionary(Of String, SampleContentLevels)
            Dim ref As New Dictionary(Of String, Standards)
            Dim ISnames As Index(Of String) = [IS].Values.Distinct.Indexing

            For Each ion As MSLIon In msl.Where(Function(i) Not i.Name Like ISnames)
                ref(ion.Name) = New Standards With {
                    .Name = ion.Name,
                    .ID = ion.Name,
                    .Factor = 1,
                    .C = contents,
                    .[IS] = [IS].TryGetValue(ion.Name),
                    .ISTD = .IS
                }
                ionLevels(ion.Name) = New SampleContentLevels(contents, directMap:=True)
            Next

            Dim ISlist As Dictionary(Of String, [IS]) = ISnames.Objects _
                .ToDictionary(Function(id) id,
                              Function(id)
                                  Return New [IS] With {
                                      .CIS = 1,
                                      .ID = id,
                                      .name = id
                                  }
                              End Function)

            Return New ContentTable(ionLevels, ref, ISlist)
        End Function
    End Module
End Namespace
