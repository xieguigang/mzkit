#Region "Microsoft.VisualBasic::297f0550d30a86584ecf4debc77c2c3c, src\assembly\assembly\ASCII\MGF\MgfReader.vb"

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

    '     Module MgfReader
    ' 
    '         Function: IonPeaks, ParseIonBlock, parseMetaInfo, ReadIons, StreamParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.MassSpectrum.Math.Spectra

Namespace ASCII.MGF

    Public Module MgfReader

        Const regexp_META$ = "((,\s*)?\S+[:]"".*?"")+"

        <Extension>
        Public Function IonPeaks(ions As IEnumerable(Of Ions)) As IEnumerable(Of PeakMs2)
            Return ions _
                .Select(Function(ion)
                            Dim meta As New MetaData(ion.Meta)
                            Dim spectrum As New LibraryMatrix With {
                                .ms2 = ion.Peaks,
                                .Name = ion.Title
                            }

                            Return New PeakMs2 With {
                                .activation = meta.activation,
                                .collisionEnergy = meta.collisionEnergy,
                                .file = ion.Rawfile,
                                .mz = ion.PepMass.name,
                                .mzInto = spectrum,
                                .rt = ion.RtInSeconds,
                                .scan = meta.scan
                            }
                        End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ReadIons(ParamArray files As String()) As IEnumerable(Of Ions)
            Return files _
                .Select(Function(filepath)
                            Call filepath.__INFO_ECHO
                            Return StreamParser(filepath)
                        End Function) _
                .IteratesALL
        End Function

        Public Iterator Function StreamParser(path$) As IEnumerable(Of Ions)
            Dim lines$() = path.ReadAllLines
            Dim ionBlocks = lines _
                .Split(delimiter:=Function(s)
                                      Return s = "END IONS"
                                  End Function,
                       deliPosition:=DelimiterLocation.NotIncludes
                )

            For Each ion As String() In ionBlocks
                Yield ParseIonBlock(ion)
            Next
        End Function

        Private Function ParseIonBlock(ion As String()) As Ions
            Dim properties = ion _
                 .Where(Function(s) InStr(s, "=") > 1) _
                 .Select(Function(s)
                             Return s.GetTagValue("=", trim:=True)
                         End Function) _
                 .ToDictionary()
            Dim peaks = ion _
                .Skip(properties.Count + 1) _
                .Select(Function(s) s.StringSplit("\s+")) _
                .Select(Function(l)
                            Return New ms2 With {
                                .mz = l(0),
                                .intensity = l(1),
                                .quantity = .intensity
                            }
                        End Function) _
                .ToArray
            Dim getValue = Function(key$)
                               Dim s$ = properties _
                                  .TryGetValue(key, [default]:=Nothing) _
                                  .Value

                               Return s Or EmptyString
                           End Function
            Dim mass As NamedValue
            Dim title$ = getValue("TITLE")
            Dim meta As Dictionary(Of String, String)
            Dim metaStr$ = title.Replace(title.Split.First, "").Trim

            If metaStr.StringEmpty Then
                ' 没有添加其他的注释信息
                meta = New Dictionary(Of String, String)
            Else
                title = title.Split.First
                meta = metaStr.parseMetaInfo
            End If

            With getValue("PEPMASS").StringSplit("\s+")
                mass = New NamedValue(.First, .Last)
            End With

            Return New Ions With {
                .Peaks = peaks,
                .RtInSeconds = Val(getValue("RTINSECONDS")),
                .PepMass = mass,
                .Title = title,
                .Meta = meta,
                .Charge = CInt(Val(getValue("CHARGE"))) Or 1.AsDefault,
                .Accession = getValue("ACCESSION"),
                .Rawfile = getValue("RAWFILE"),
                .Instrument = getValue("INSTRUMENT"),
                .Locus = getValue("LOCUS"),
                .Sequence = getValue("SEQ"),
                .Database = getValue("DB")
            }
        End Function

        <Extension>
        Private Function parseMetaInfo(substr As String) As Dictionary(Of String, String)
            Return substr.StringSplit("[""],\s+") _
                .Select(Function(s) s.GetTagValue(":")) _
                .ToDictionary(Function(key) key.Name,
                              Function(val)
                                  Return val.Value.Trim(""""c)
                              End Function)
        End Function
    End Module
End Namespace
