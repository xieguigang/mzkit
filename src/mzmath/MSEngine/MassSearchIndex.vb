#Region "Microsoft.VisualBasic::9437612d0e427638f8ff55b40d826cfc, mzmath\MSEngine\MassSearchIndex.vb"

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

    '   Total Lines: 82
    '    Code Lines: 38 (46.34%)
    ' Comment Lines: 34 (41.46%)
    '    - Xml Docs: 61.76%
    ' 
    '   Blank Lines: 10 (12.20%)
    '     File Size: 2.81 KB


    ' Interface IMassSearch
    ' 
    '     Function: QueryByMass
    ' 
    ' Class MassSearchIndex
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: IMassSearch_QueryByMass, QueryByMass, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Algorithm

Public Interface IMassSearch

    ''' <summary>
    ''' Provides a mass value and then populate all related metabolite reference data
    ''' </summary>
    ''' <param name="mass"></param>
    ''' <returns></returns>
    Function QueryByMass(mass As Double) As IEnumerable

End Interface

''' <summary>
''' A simple implements of exact mass search index
''' </summary>
Public Class MassSearchIndex(Of T As IExactMassProvider) : Implements IMassSearch

    ''' <summary>
    ''' mass tolerance value for match sample mz and threocal mz
    ''' </summary>
    ReadOnly tolerance As Tolerance
    ReadOnly massIndex As BlockSearchFunction(Of T)
    ReadOnly activator As Func(Of Double, Object)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mass"></param>
    ''' <param name="activator">
    ''' returns object due to the reason of reflection not working
    ''' well on build a dynamics delegate type
    ''' </param>
    ''' <param name="tolerance">
    ''' tolerance used for filter mass hit, not the tolerance of build search index
    ''' </param>
    Sub New(mass As IEnumerable(Of T), activator As Func(Of Double, Object), tolerance As Tolerance)
        ' 20220512
        '
        ' too small tolerance error will cause too much elements to
        ' sort
        ' and then will cause the error of 
        ' Stack overflow.
        ' Repeat 3075 times: 
        ' --------------------------------
        '   at Microsoft.VisualBasic.ComponentModel.Algorithm.QuickSortFunction
        '
        ' pipeline has been test for MS-imaging data analysis
        '
        Me.massIndex = New BlockSearchFunction(Of T)(
            data:=mass,
            eval:=Function(m) m.ExactMass,
            tolerance:=1.5,
            factor:=3
        )
        Me.activator = activator
        Me.tolerance = tolerance
    End Sub

    Public Function QueryByMass(mass As Double) As IEnumerable(Of T)
        Dim query As T = activator(mass)
        Dim pop = massIndex _
            .Search(query) _
            .Where(Function(d)
                       Return tolerance(d.ExactMass, mass)
                   End Function)

        Return pop
    End Function

    Public Overrides Function ToString() As String
        Return tolerance.ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function IMassSearch_QueryByMass(mass As Double) As IEnumerable Implements IMassSearch.QueryByMass
        Return QueryByMass(mass)
    End Function
End Class
