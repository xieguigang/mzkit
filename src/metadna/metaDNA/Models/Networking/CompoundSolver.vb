#Region "Microsoft.VisualBasic::959d558fd9865d3ed77a99f46451378e, metadna\metaDNA\Models\Networking\CompoundSolver.vb"

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

    '   Total Lines: 67
    '    Code Lines: 30
    ' Comment Lines: 29
    '   Blank Lines: 8
    '     File Size: 3.31 KB


    ' Class CompoundSolver
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+3 Overloads) CreateIndex, Wraps
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

<Assembly: InternalsVisibleTo("mzkit")>

''' <summary>
''' the metabolite annotation search engine for the kegg metabolites
''' </summary>
Public NotInheritable Class CompoundSolver : Inherits MSSearch(Of GenericCompound)

    ''' <summary>
    ''' constructor for the kegg compound collection
    ''' </summary>
    ''' <param name="compounds"></param>
    ''' <param name="types"></param>
    ''' <param name="tolerance"></param>
    Private Sub New(compounds As IEnumerable(Of GenericCompound), types As MzCalculator(),
                    tolerance As Tolerance,
                    mass_range As DoubleRange)

        Call MyBase.New(compounds, tolerance, types, mass_range:=mass_range)
    End Sub

    ''' <summary>
    ''' constructor for the kegg compound collection
    ''' </summary>
    ''' <param name="compounds"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Wraps(compounds As IEnumerable(Of Compound)) As IEnumerable(Of KEGGCompound)
        Return compounds.Where(Function(c) c.exactMass > 0).Select(Function(c) New KEGGCompound With {.KEGG = c})
    End Function

    ''' <summary>
    ''' constructor for the generic compound collection, example as: pubchem, hmdb, lipidmaps, etc
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="compounds"></param>
    ''' <param name="types"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Shared Function CreateIndex(Of T As GenericCompound)(compounds As IEnumerable(Of T), types As MzCalculator(), tolerance As Tolerance, Optional mass_range As DoubleRange = Nothing) As MSSearch(Of GenericCompound)
        Return New CompoundSolver(compounds.Select(Function(c) DirectCast(c, GenericCompound)), types, tolerance, mass_range)
    End Function

    ''' <summary>
    ''' constructor for the kegg compound collection
    ''' </summary>
    ''' <param name="compounds"></param>
    ''' <param name="types"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    Public Overloads Shared Function CreateIndex(compounds As IEnumerable(Of KEGGCompound), types As MzCalculator(), tolerance As Tolerance, Optional mass_range As DoubleRange = Nothing) As MSSearch(Of GenericCompound)
        Return New CompoundSolver(compounds.Select(Function(c) DirectCast(c, GenericCompound)), types, tolerance, mass_range)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Shared Function CreateIndex(compounds As IEnumerable(Of Compound), types As MzCalculator(), tolerance As Tolerance, Optional mass_range As DoubleRange = Nothing) As MSSearch(Of GenericCompound)
        Return CreateIndex(Wraps(compounds), types, tolerance, mass_range)
    End Function
End Class
