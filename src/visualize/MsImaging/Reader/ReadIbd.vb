#Region "Microsoft.VisualBasic::dae1785713f331db4c3f076edeb78fa9, G:/mzkit/src/visualize/MsImaging//Reader/ReadIbd.vb"

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

    '   Total Lines: 98
    '    Code Lines: 73
    ' Comment Lines: 8
    '   Blank Lines: 17
    '     File Size: 3.30 KB


    '     Class ReadIbd
    ' 
    '         Properties: dimension, ibd, resolution, UUID
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: AllPixels, GetPixel, LoadMzArray
    ' 
    '         Sub: release
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Application = Microsoft.VisualBasic.Parallel

Namespace Reader

    ''' <summary>
    ''' handling of the imzml data file
    ''' </summary>
    Public Class ReadIbd : Inherits PixelReader

        Dim pixels As ibdPixel()

        Public ReadOnly Property ibd As ibdReader

        Public ReadOnly Property UUID As String
            Get
                Return ibd.UUID
            End Get
        End Property

        Public Overrides ReadOnly Property dimension As Size
        Public Overrides ReadOnly Property resolution As Double
            Get
                Return 17
            End Get
        End Property

        Sub New(imzML As String, Optional memoryCache As Boolean = False)
            ibd = ibdReader.Open(imzML.ChangeSuffix("ibd"))
            pixels = XML.LoadScans(file:=imzML) _
                .Select(Function(p) New ibdPixel(ibd, p, enableCache:=memoryCache)) _
                .ToArray
            dimension = New Size With {
                .Width = pixels.Select(Function(p) p.X).Max,
                .Height = pixels.Select(Function(p) p.Y).Max
            }

            If memoryCache Then
                Call "loading raw data into memory cache...".__DEBUG_ECHO

                For Each p As ibdPixel In pixels
                    Call p.GetMs()
                Next

                Call ibd.Dispose()
                Call "cache done!".__DEBUG_ECHO
            End If
        End Sub

        Public Overrides Function GetPixel(x As Integer, y As Integer) As PixelScan
            Return pixels _
                .Where(Function(p) p.X = x AndAlso p.Y = y) _
                .FirstOrDefault
        End Function

        ''' <summary>
        ''' load all ions m/z in the raw data file
        ''' </summary>
        ''' <param name="ppm"></param>
        ''' <returns></returns>
        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Dim mzlist = pixels _
                .Select(Function(p)
                            Return Application.DoEvents(Function() p.ReadMz)
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray
            Dim groups = mzlist _
                .GroupBy(Function(mz) mz, Tolerance.PPM(ppm)) _
                .Select(Function(mz) Val(mz.name)) _
                .OrderBy(Function(mzi) mzi) _
                .ToArray

            Return groups
        End Function

        Protected Overrides Sub release()
            Call ibd.Dispose()

            For Each p As ibdPixel In pixels
                Call p.release()
            Next

            Erase pixels
        End Sub

        Public Overrides Function AllPixels() As IEnumerable(Of PixelScan)
            Return pixels.Select(Function(p) DirectCast(p, PixelScan))
        End Function
    End Class
End Namespace
