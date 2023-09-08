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