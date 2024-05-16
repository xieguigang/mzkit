#Region "Microsoft.VisualBasic::e844b729b888aea0c8c7b1893a742119, assembly\SpectrumTree\Pack\MzIonSearch.vb"

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

    '   Total Lines: 51
    '    Code Lines: 35
    ' Comment Lines: 7
    '   Blank Lines: 9
    '     File Size: 1.73 KB


    '     Class MzIonSearch
    ' 
    '         Properties: IsEmpty
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: QueryByMz, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Query
Imports Microsoft.VisualBasic.ComponentModel.Algorithm

Namespace PackLib

    Friend Class MzIonSearch

        ReadOnly mzIndex As BlockSearchFunction(Of IonIndex)
        ReadOnly da As Tolerance

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return mzIndex.size = 0
            End Get
        End Property

        Sub New(mz As IEnumerable(Of IonIndex), da As Tolerance)
            Me.da = da

            ' see dev notes about the mass tolerance in 
            ' MSSearch module
            mzIndex = New BlockSearchFunction(Of IonIndex)(
                data:=mz,
                eval:=Function(m) m.mz,
                tolerance:=1,
                factor:=3
            )
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{mzIndex.Keys.Length} ions m/z index key information, mzdiff: {da.ToString}"
        End Function

        ''' <summary>
        ''' query the spectrum reference tree nodes via parent m/z matched
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <returns></returns>
        Public Function QueryByMz(mz As Double) As IEnumerable(Of IonIndex)
            Dim query As New IonIndex With {.mz = mz}
            Dim result As IEnumerable(Of IonIndex) = mzIndex _
                .Search(query) _
                .Where(Function(d) da(d.mz, mz))

            Return result
        End Function
    End Class
End Namespace
