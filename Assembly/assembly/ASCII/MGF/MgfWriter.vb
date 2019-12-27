#Region "Microsoft.VisualBasic::84024a29d1eb820b7e3f678beea19482, Assembly\assembly\ASCII\MGF\MgfWriter.vb"

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

    '     Module MgfWriter
    ' 
    '         Function: ionTitle, (+2 Overloads) MgfIon, (+2 Overloads) SaveTo
    ' 
    '         Sub: WriteAsciiMgf, writeIf
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.MassSpectrum.Math.Spectra

Namespace ASCII.MGF

    Public Module MgfWriter

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MgfIon(matrix As PeakMs2) As Ions
            Return New Ions With {
                .Charge = 1,
                .Peaks = matrix.mzInto.Array,
                .PepMass = New NamedValue With {
                    .name = matrix.mz,
                    .text = matrix.Ms2Intensity
                },
                .RtInSeconds = matrix.rt,
                .Title = $"{matrix.file}#{matrix.scan}",
                .Meta = New MetaData With {
                    .collisionEnergy = matrix.collisionEnergy,
                    .activation = matrix.activation,
                    .scan = matrix.scan
                },
                .Rawfile = matrix.file,
                .Accession = $"{matrix.file}#{matrix.scan}"
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MgfIon(matrix As LibraryMatrix, Optional precursor As ms2 = Nothing) As Ions
            If precursor Is Nothing Then
                precursor = New ms2 With {
                    .mz = matrix.ms2.Max(Function(m) m.mz),
                    .intensity = 1,
                    .quantity = 1
                }
            End If

            Return New Ions With {
                .Peaks = matrix.ms2,
                .Title = matrix.Name,
                .Charge = 1,
                .PepMass = New NamedValue With {
                    .name = precursor.mz,
                    .text = precursor.quantity
                }
            }
        End Function

        <Extension>
        Private Function ionTitle(ion As Ions) As String
            If ion.Meta.IsNullOrEmpty Then
                Return ion.Title
            Else
                Dim metaStr As String = ion.Meta _
                    .Where(Function(t) Not t.Value.StringEmpty) _
                    .Select(Function(m)
                                Return $"{m.Key}:""{m.Value}"""
                            End Function) _
                    .JoinBy(", ")

                Return $"{ion.Title} {metaStr}"
            End If
        End Function

        <Extension>
        Private Sub writeIf(out As TextWriter, key$, value$)
            If Not value.StringEmpty Then
                Call out.WriteLine($"{key}={value}")
            End If
        End Sub

        ''' <summary>
        ''' 将目标<see cref="Ions"/>对象序列化为mgf文件格式的文本段然后写入所给定的文件流数据<paramref name="out"/>之中
        ''' </summary>
        ''' <param name="ion"></param>
        ''' <param name="out"></param>
        ''' <param name="relativeIntensity"></param>
        <Extension>
        Public Sub WriteAsciiMgf(ion As Ions, out As TextWriter, Optional relativeIntensity As Boolean = False)
            Call out.WriteLine("BEGIN IONS")
            Call out.WriteLine("TITLE=" & ion.ionTitle)
            Call out.WriteLine("RTINSECONDS=" & ion.RtInSeconds)
            Call out.WriteLine($"PEPMASS={ion.PepMass.name} {ion.PepMass.text}")
            Call out.WriteLine("CHARGE=" & ion.Charge)

            ' Optional
            Call out.writeIf("ACCESSION", ion.Accession)
            Call out.writeIf("INSTRUMENT", ion.Instrument)
            Call out.writeIf("RAWFILE", ion.Rawfile)
            Call out.writeIf("DB", ion.Database)
            Call out.writeIf("SEQ", ion.Sequence)
            Call out.writeIf("LOCUS", ion.Locus)

            Dim mz, into As Double
            Dim getInto As Func(Of ms2, Double)

            If relativeIntensity Then
                Dim peaks As LibraryMatrix = ion.Peaks

                getInto = Function(m) m.intensity
                peaks = peaks / peaks.Max
                ion = New Ions With {
                    .Peaks = peaks.ToArray
                }
            Else
                getInto = Function(m) m.quantity
            End If

            For Each fragment As ms2 In ion.Peaks
                mz = fragment.mz
                into = getInto(fragment)

                ' write mgf text file data
                Call $"{mz} {into}".DoCall(AddressOf out.WriteLine)
            Next

            Call out.WriteLine("END IONS")
        End Sub

        <Extension>
        Public Function SaveTo(ion As Ions, file$) As Boolean
            Using write As StreamWriter = file.OpenWriter
                Call ion.WriteAsciiMgf(write)
            End Using

            Return True
        End Function

        <Extension>
        Public Function SaveTo(ions As IEnumerable(Of Ions), file$) As Boolean
            Using write As StreamWriter = file.OpenWriter
                For Each ion As Ions In ions
                    Call ion.WriteAsciiMgf(write)
                Next
            End Using

            Return True
        End Function
    End Module
End Namespace
