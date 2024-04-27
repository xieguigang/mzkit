#Region "Microsoft.VisualBasic::944b12375a4f3f48465a7a66c4c21d51, G:/mzkit/src/mzmath/MoleculeNetworking//SpectrumPool/MetadataPool/MetadataProxy.vb"

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
    '    Code Lines: 13
    ' Comment Lines: 17
    '   Blank Lines: 9
    '     File Size: 1.40 KB


    '     Class MetadataProxy
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Namespace PoolData

    ''' <summary>
    ''' a spectrum cluster node collection proxy
    ''' </summary>
    Public MustInherit Class MetadataProxy

        Default Public MustOverride ReadOnly Property GetById(id As String) As Metadata
        Public MustOverride ReadOnly Property AllClusterMembers As IEnumerable(Of Metadata)
        Public MustOverride ReadOnly Property Depth As Integer

        ''' <summary>
        ''' the root spectrum id in current cluster object
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride ReadOnly Property RootId As String

        Public MustOverride Sub Add(id As String, metadata As Metadata)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="score"></param>
        ''' <param name="align">
        ''' the root spectrum to current cluster node is nothing, due 
        ''' to the reason of no spectrum compares to it
        ''' </param>
        ''' <param name="pval"></param>
        Public MustOverride Sub Add(id As String, score As Double, align As AlignmentOutput, pval As Double)

        Public MustOverride Function HasGuid(id As String) As Boolean
        Public MustOverride Sub SetRootId(hashcode As String)

    End Class

End Namespace
