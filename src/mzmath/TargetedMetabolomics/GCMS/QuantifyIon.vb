#Region "Microsoft.VisualBasic::203b867fa5c1285beb7b938a601f656f, src\mzmath\TargetedMetabolomics\GCMS\QuantifyIon.vb"

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

    '     Class QuantifyIon
    ' 
    '         Properties: id, ms, name, rt
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: FromIons, ToString
    '         Class SchemaProvider
    ' 
    '             Function: FragmentObj, GetObjectSchema, IonObj, RangeObj
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Namespace GCMS

    ''' <summary>
    ''' 定量离子模型
    ''' </summary>
    Public Class QuantifyIon : Implements INamedValue

        Public Property id As String Implements INamedValue.Key
        Public Property rt As DoubleRange
        Public Property name As String
        Public Property ms As ms2()

        Shared Sub New()
            Call MsgPackSerializer.DefaultContext.RegisterSerializer(New SchemaProvider)
        End Sub

        Public Overrides Function ToString() As String
            Return $"Dim {id} As [{rt.Min}, {rt.Max}]"
        End Function

        Public Shared Function FromIons(ions As IEnumerable(Of MSLIon), rtwin As Double) As IEnumerable(Of QuantifyIon)
            Return ions _
                .Select(Function(ion)
                            Return New QuantifyIon With {
                                .id = ion.Name,
                                .ms = ion.Peaks,
                                .rt = New DoubleRange(ion.RT - rtwin, ion.RT + rtwin)
                            }
                        End Function)
        End Function

        Private Class SchemaProvider : Inherits SchemaProvider(Of QuantifyIon)

            Protected Overrides Iterator Function GetObjectSchema() As IEnumerable(Of (obj As Type, schema As Dictionary(Of String, NilImplication)))
                Yield (GetType(QuantifyIon), IonObj)
                Yield (GetType(DoubleRange), RangeObj)
                Yield (GetType(ms2), FragmentObj)
            End Function

            Private Shared Function IonObj() As Dictionary(Of String, NilImplication)
                Return New Dictionary(Of String, NilImplication) From {
                    {NameOf(QuantifyIon.id), NilImplication.MemberDefault},
                    {NameOf(QuantifyIon.name), NilImplication.MemberDefault},
                    {NameOf(QuantifyIon.rt), NilImplication.MemberDefault},
                    {NameOf(QuantifyIon.ms), NilImplication.MemberDefault}
                }
            End Function

            Private Shared Function RangeObj() As Dictionary(Of String, NilImplication)
                Return New Dictionary(Of String, NilImplication) From {
                    {NameOf(DoubleRange.Min), NilImplication.MemberDefault},
                    {NameOf(DoubleRange.Max), NilImplication.MemberDefault}
                }
            End Function

            Private Shared Function FragmentObj() As Dictionary(Of String, NilImplication)
                Return New Dictionary(Of String, NilImplication) From {
                    {NameOf(ms2.mz), NilImplication.MemberDefault},
                    {NameOf(ms2.intensity), NilImplication.MemberDefault},
                    {NameOf(ms2.Annotation), NilImplication.MemberDefault}
                }
            End Function
        End Class
    End Class
End Namespace
