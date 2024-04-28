#Region "Microsoft.VisualBasic::6462ecdedce0732e44adf6faedc50471, G:/mzkit/src/metadb/Massbank//MetaLib/Match/ExactMassSearch.vb"

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

    '   Total Lines: 76
    '    Code Lines: 54
    ' Comment Lines: 4
    '   Blank Lines: 18
    '     File Size: 2.76 KB


    '     Class ExactMassSearch
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateIndex, QueryByMass
    ' 
    '     Structure MassQuery
    ' 
    '         Properties: ExactMass
    ' 
    '         Function: ToString
    ' 
    '     Structure MassCompares
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) CompareTo, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables

Namespace MetaLib

    ''' <summary>
    ''' handler for exact mass of the metabolites annotation on MS1 level.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class ExactMassSearch(Of T As IExactMassProvider)

        ReadOnly index As OrderSelector(Of MassCompares)

        Private Sub New(index As OrderSelector(Of MassCompares))
            Me.index = index
        End Sub

        Public Function QueryByMass(query As Double, ppm As Double) As IEnumerable(Of T)
            Dim da As Double = ChemicalData.PpmToMassDelta(query, ppm)
            Dim min As New MassQuery With {.mass = query - da}
            Dim max As New MassQuery With {.mass = query + da}

            Return index _
                .SelectByRange(New MassCompares(min), New MassCompares(max)) _
                .Select(Function(target)
                            Return DirectCast(target.obj, T)
                        End Function)
        End Function

        Public Shared Function CreateIndex(data As IEnumerable(Of T)) As ExactMassSearch(Of T)
            Return New ExactMassSearch(Of T)(New OrderSelector(Of MassCompares)(data.Select(Function(a) New MassCompares With {.obj = a})))
        End Function
    End Class

    Friend Structure MassQuery : Implements IExactMassProvider

        Dim mass As Double

        Public ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
            Get
                Return mass
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return mass.ToString("F4")
        End Function

    End Structure

    Public Structure MassCompares : Implements IComparable(Of Double), IComparable

        Dim obj As IExactMassProvider

        Sub New(x As IExactMassProvider)
            obj = x
        End Sub

        Public Overrides Function ToString() As String
            Return obj.ToString
        End Function

        Public Function CompareTo(other As Double) As Integer Implements IComparable(Of Double).CompareTo
            Return obj.ExactMass.CompareTo(other)
        End Function

        Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            If TypeOf obj Is MassCompares Then
                Return Me.obj.ExactMass.CompareTo(DirectCast(obj, MassCompares).obj.ExactMass)
            Else
                Return -1
            End If
        End Function
    End Structure
End Namespace
