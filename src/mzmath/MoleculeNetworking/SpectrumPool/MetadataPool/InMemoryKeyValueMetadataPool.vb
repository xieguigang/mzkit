#Region "Microsoft.VisualBasic::e59563265a13ff5b7b78ad82c8ac3f5d, mzmath\MoleculeNetworking\SpectrumPool\MetadataPool\InMemoryKeyValueMetadataPool.vb"

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

    '   Total Lines: 60
    '    Code Lines: 46 (76.67%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (23.33%)
    '     File Size: 1.81 KB


    '     Class InMemoryKeyValueMetadataPool
    ' 
    '         Properties: AllClusterMembers, Depth, RootId
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: HasGuid
    ' 
    '         Sub: (+2 Overloads) Add, SetRootId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Namespace PoolData

    Public Class InMemoryKeyValueMetadataPool : Inherits MetadataProxy

        Dim data As Dictionary(Of String, Metadata)

        Dim m_rootId As String
        Dim m_depth As Integer

        Default Public Overrides ReadOnly Property GetById(id As String) As Metadata
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return data(id)
            End Get
        End Property

        Public Overrides ReadOnly Property AllClusterMembers As IEnumerable(Of Metadata)
            Get
                Return data.Values
            End Get
        End Property

        Public Overrides ReadOnly Property Depth As Integer
            Get
                Return m_depth
            End Get
        End Property

        Public Overrides ReadOnly Property RootId As String
            Get
                Return m_rootId
            End Get
        End Property

        Sub New(data As Dictionary(Of String, Metadata), depth As Integer)
            Me.data = data
            Me.m_depth = depth
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Sub Add(id As String, metadata As Metadata)
            Call data.Add(id, metadata)
        End Sub

        Public Overrides Function HasGuid(id As String) As Boolean
            Return data.ContainsKey(id)
        End Function

        Public Overrides Sub SetRootId(hashcode As String)
            m_rootId = hashcode
        End Sub

        Public Overrides Sub Add(id As String, score As Double, align As AlignmentOutput, pval As Double)

        End Sub
    End Class
End Namespace
