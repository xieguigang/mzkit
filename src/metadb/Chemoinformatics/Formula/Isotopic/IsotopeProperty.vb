#Region "Microsoft.VisualBasic::8f3fa4851342b7d72a35d8f0547351d1, metadb\Chemoinformatics\Formula\Isotopic\IsotopeProperty.vb"

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

    '   Total Lines: 102
    '    Code Lines: 55
    ' Comment Lines: 12
    '   Blank Lines: 35
    '     File Size: 3.43 KB


    '     Class IupacDatabase
    ' 
    '         Properties: ElementName2AtomElementProperties, Id2AtomElementProperties
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: LoadDefault
    ' 
    '     Class IsotopeProperty
    ' 
    '         Properties: ElementProfile, ExactMass, Formula, IsotopeProfile
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class AtomProperty
    ' 
    '         Properties: AtomElementProperties, ElementName, ElementNumber, IsotopicPeaks, IupacID
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class AtomElementProperty
    ' 
    '         Properties: ElementName, ExactMass, ID, NaturalRelativeAbundance, NominalMass
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Formula.IsotopicPatterns

    ''' <summary>
    ''' This is the storage of IUPAC queries described in 'IUPAC.txt' of Resources folder of MSDIAL assembry.
    ''' Each chemical element such as C, N, O, S has the generic list of IupacChemicalElement.cs.
    ''' This Iupac.cs has the queries of each chemical element detail as the dictionary.
    ''' </summary>
    Public Class IupacDatabase


        Public Property Id2AtomElementProperties As New Dictionary(Of Integer, AtomElementProperty())()

        Public Property ElementName2AtomElementProperties As New Dictionary(Of String, AtomElementProperty())()
        Public Sub New()
        End Sub

        Public Shared Function LoadDefault() As IupacDatabase
            Dim iupac As New IupacDatabase
            Dim idIndex = iupac.Id2AtomElementProperties
            Dim symbolIndex = iupac.ElementName2AtomElementProperties

            For Each element As Element In Element.MemoryPopulateElements
                Dim symbol As String = element.symbol
                Dim id As Integer = element.id
                Dim atoms As AtomElementProperty() = element.isotopes _
                    .Select(Function(i)
                                Return New AtomElementProperty With {
                                    .ElementName = symbol,
                                    .ExactMass = i.Mass,
                                    .NaturalRelativeAbundance = i.Prob,
                                    .NominalMass = i.NumNeutrons,
                                    .ID = id
                                }
                            End Function) _
                    .ToArray

                idIndex.Add(element.id, atoms)
                symbolIndex.Add(symbol, atoms)
            Next

            Return iupac
        End Function
    End Class

    Public Class IsotopeProperty

        Public Sub New()
        End Sub

        Public Property Formula As Formula = New Formula()

        Public Property ExactMass As Double

        Public Property ElementProfile As List(Of AtomProperty) = New List(Of AtomProperty)()

        Public Property IsotopeProfile As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
    End Class


    ''' <summary>
    ''' This is the storage of the properties for natural isotope ratio calculation.
    ''' This element is prepared for each chemical element such as C, H, N, O, S included in internal IUPAC queries.
    ''' </summary>
    Public Class AtomProperty

        Public Sub New()
        End Sub

        Public Property IupacID As Integer

        Public Property ElementName As String

        Public Property ElementNumber As Integer

        Public Property AtomElementProperties As AtomElementProperty()

        Public Property IsotopicPeaks As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

    End Class


    ''' <summary>
    ''' This is the storage of natural abundance or mass properties of each chemical element such as 12C, 13C etc.
    ''' </summary>

    Public Class AtomElementProperty

        Public Property ID As Integer


        Public Property ElementName As String = String.Empty


        Public Property NominalMass As Integer


        Public Property NaturalRelativeAbundance As Double


        Public Property ExactMass As Double
    End Class
End Namespace
