#Region "Microsoft.VisualBasic::d3f198c51bb00587f8a1e23b43d4db17, src\metadb\Massbank\Public\TMIC\HMDB\Tables\ChemicalDescriptor.vb"

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

    '     Class ChemicalDescriptor
    ' 
    '         Properties: acceptor_count, accession, bioavailability, descriptors, donor_count
    '                     formal_charge, ghose_filter, kegg_id, logp, mddr_like_rule
    '                     melting_point, name, number_of_rings, physiological_charge, pka_strongest_acidic
    '                     pka_strongest_basic, polar_surface_area, polarizability, refractivity, rotatable_bond_count
    '                     rule_of_five, state, veber_rule
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: FromMetabolite, ToDescriptor
    ' 
    '         Sub: WriteTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO.Linq

Namespace TMIC.HMDB

    Public Class ChemicalDescriptor

        ''' <summary>
        ''' HMDB main accession
        ''' </summary>
        ''' <returns></returns>
        Public Property accession As String
        Public Property name As String
        Public Property kegg_id As String
        Public Property state As String
        Public Property melting_point As Double
        Public Property logp As Double
        Public Property pka_strongest_acidic As Double
        Public Property pka_strongest_basic As Double
        Public Property polar_surface_area As Double
        Public Property refractivity As Double
        Public Property polarizability As Double
        Public Property rotatable_bond_count As Double
        Public Property acceptor_count As Double
        Public Property donor_count As Double
        Public Property physiological_charge As Double
        Public Property formal_charge As Double
        Public Property number_of_rings As Double
        Public Property bioavailability As Double
        Public Property rule_of_five As String
        Public Property ghose_filter As String
        Public Property veber_rule As String
        Public Property mddr_like_rule As String

        ''' <summary>
        ''' The chemical descriptor names
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property descriptors As Index(Of String)

        Shared ReadOnly readers As PropertyInfo()

        Shared Sub New()
            readers = DataFramework _
                .Schema(GetType(ChemicalDescriptor), PropertyAccess.Readable, nonIndex:=True) _
                .Values _
                .ToArray

            descriptors = readers _
                .Where(Function(p)
                           Return p.PropertyType Is GetType(Double)
                       End Function) _
                .Select(Function(p) p.Name) _
                .AsList + {
                    NameOf(state),
                    NameOf(rule_of_five),
                    NameOf(ghose_filter),
                    NameOf(veber_rule),
                    NameOf(mddr_like_rule)
                }

            readers = readers _
                .Where(Function(p) p.Name Like descriptors) _
                .ToArray
        End Sub

        ''' <summary>
        ''' Create descriptor data set for machine learning 
        ''' </summary>
        ''' <returns></returns>
        Public Function ToDescriptor() As Dictionary(Of String, Double)
            Return readers _
                .ToDictionary(Function(p) p.Name,
                              Function(p)
                                  Dim value As Object = p.GetValue(Me)

                                  If p.PropertyType Is GetType(Double) Then
                                      Return CDbl(value)
                                  ElseIf p.Name = NameOf(state) Then
                                      If CStr(value).TextEquals("Solid") Then
                                          Return 1
                                      Else
                                          Return 0
                                      End If
                                  Else
                                      If CStr(value).ParseBoolean = False Then
                                          Return 0
                                      Else
                                          Return 1
                                      End If
                                  End If
                              End Function)
        End Function

        Public Shared Function FromMetabolite(metabolite As metabolite) As ChemicalDescriptor
            Dim properties = metabolite.experimental_properties.PropertyList.AsList +
                metabolite.predicted_properties.PropertyList
            Dim propertyTable As Dictionary(Of String, String) = properties _
                .GroupBy(Function(p) p.kind) _
                .ToDictionary(Function(p) p.Key,
                              Function(g)
                                  If g.Any(Function(f) IsBooleanFactor(f.value)) Then
                                      Return g.Select(Function(x) x.value).TopMostFrequent
                                  Else
                                      Return g.Select(Function(x) Val(x.value)) _
                                         .Average _
                                         .ToString
                                  End If
                              End Function)
            Dim read = Function(key As String, default$) As String
                           Return propertyTable.TryGetValue(key, [default]:=[default])
                       End Function

            Return New ChemicalDescriptor With {
                .accession = metabolite.accession,
                .name = metabolite.name,
                .kegg_id = metabolite.kegg_id,
                .state = metabolite.state,
                .acceptor_count = read(NameOf(.acceptor_count), 0),
                .bioavailability = read(NameOf(.bioavailability), 0),
                .donor_count = read(NameOf(.donor_count), 0),
                .formal_charge = read(NameOf(.formal_charge), 0),
                .ghose_filter = read(NameOf(.ghose_filter), "no"),
                .logp = read(NameOf(.logp), 0),
                .mddr_like_rule = read(NameOf(.mddr_like_rule), "no"),
                .melting_point = read(NameOf(melting_point), 0),
                .number_of_rings = read(NameOf(.number_of_rings), 0),
                .physiological_charge = read(NameOf(.physiological_charge), 0),
                .pka_strongest_acidic = read(NameOf(.pka_strongest_acidic), 0),
                .pka_strongest_basic = read(NameOf(.pka_strongest_basic), 0),
                .polarizability = read(NameOf(.polarizability), 0),
                .polar_surface_area = read(NameOf(.polar_surface_area), 0),
                .refractivity = read(NameOf(.refractivity), 0),
                .rotatable_bond_count = read(NameOf(.rotatable_bond_count), 0),
                .rule_of_five = read(NameOf(.rule_of_five), "no"),
                .veber_rule = read(NameOf(.veber_rule), "no")
            }
        End Function

        Public Shared Sub WriteTable(metabolites As IEnumerable(Of metabolite), out As Stream)
            Using table As New WriteStream(Of ChemicalDescriptor)(New StreamWriter(out))
                For Each metabolite As metabolite In metabolites
                    Call table.Flush(FromMetabolite(metabolite))
                Next
            End Using
        End Sub
    End Class
End Namespace
