#Region "Microsoft.VisualBasic::ce308f21dd4d22b5596c9f85e0e95046, G:/mzkit/src/mzmath/SingleCells//File/MatrixWriter/SpotWriter.vb"

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

    '   Total Lines: 69
    '    Code Lines: 38
    ' Comment Lines: 19
    '   Blank Lines: 12
    '     File Size: 2.03 KB


    ' Class SpotWriter
    ' 
    '     Properties: sPos
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetLabelOffsetIndex, GetSpotOffSets
    ' 
    '     Sub: AddSpot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute

Public Class SpotWriter

    ReadOnly bin As BinaryWriter
    ReadOnly spot_index As New List(Of SpatialIndex)
    ReadOnly label_index As New List(Of (String, Long))

    ''' <summary>
    ''' get current stream position
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property sPos As Long
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return bin.BaseStream.Position
        End Get
    End Property

    Sub New(bin As BinaryWriter)
        Me.bin = bin
    End Sub

    ''' <summary>
    ''' for a better perfermance of binary data file seek operation
    ''' the scan data is in structrue of:
    ''' 
    ''' ```
    '''   x,  y,  z,intensity,label_string
    ''' i32,i32,i32,  f64 * n,string
    ''' ```
    ''' 
    ''' so, for seek a ion intensity value will be in fast speed
    ''' </summary>
    ''' <remarks>
    ''' <see cref="MatrixReader.LoadCurrentSpot()"/>
    ''' </remarks>
    Public Sub AddSpot(spot As PixelData)
        Dim label As String = If(spot.label, "")
        Dim pos As Long = sPos

        Call spot_index.Add(New SpatialIndex(spot.X, spot.Y, spot.Z, pos))
        Call label_index.Add((label, pos))

        Call bin.Write(spot.X)
        Call bin.Write(spot.Y)
        Call bin.Write(spot.Z)

        ' intensity vector size equals to the mz features
        For Each i As Double In spot.intensity
            Call bin.Write(i)
        Next

        Call bin.Write(label)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSpotOffSets() As IEnumerable(Of SpatialIndex)
        Return spot_index
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetLabelOffsetIndex() As IEnumerable(Of (String, Long))
        Return label_index
    End Function

End Class
