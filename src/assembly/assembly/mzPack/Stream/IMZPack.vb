#Region "Microsoft.VisualBasic::c4c6b7be4b04b8f1cd9e57540bc5a362, assembly\assembly\mzPack\Stream\IMZPack.vb"

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

    '   Total Lines: 45
    '    Code Lines: 28
    ' Comment Lines: 10
    '   Blank Lines: 7
    '     File Size: 1.56 KB


    '     Interface IMZPack
    ' 
    '         Properties: Application, metadata, MS, source
    ' 
    '     Module ReaderHelper
    ' 
    '         Function: GetXIC
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Linq

Namespace mzData.mzWebCache

    ''' <summary>
    ''' An abstract mzpack data model for the ms spectrum data 
    ''' </summary>
    Public Interface IMZPack

        Property MS As ScanMS1()
        Property Application As FileApplicationClass
        Property source As String
        Property metadata As Dictionary(Of String, String)

    End Interface

    Public Module ReaderHelper

        ''' <summary>
        ''' get XIC data points 
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mz"></param>
        ''' <param name="mzErr"></param>
        ''' <returns>data points has already been re-order by time</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetXIC(raw As IMZPack, mz As Double, mzErr As Tolerance) As ChromatogramTick()
            Return raw.MS _
                .SafeQuery _
                .Select(Function(i)
                            Return New ChromatogramTick With {
                                .Time = i.rt,
                                .Intensity = i.GetIntensity(mz, mzErr)
                            }
                        End Function) _
                .OrderBy(Function(ti) ti.Time) _
                .ToArray
        End Function

    End Module
End Namespace
