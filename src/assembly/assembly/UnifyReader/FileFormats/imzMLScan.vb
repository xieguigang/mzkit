#Region "Microsoft.VisualBasic::30aa7ed5bd34d667c073152c0555e9e1, src\assembly\assembly\UnifyReader\FileFormats\imzMLScan.vb"

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

    '     Class imzMLScan
    ' 
    '         Function: GetActivationMethod, GetBPC, GetCentroided, GetCharge, GetCollisionEnergy
    '                   GetMsLevel, GetMsMs, GetParentMz, GetPolarity, GetScanId
    '                   GetScanTime, GetTIC, IsEmpty
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace DataReader

    Public Class imzMLScan : Inherits MsDataReader(Of ScanReader)

        Public Overrides Function GetScanTime(scan As ScanReader) As Double
            Return 0
        End Function

        Public Overrides Function GetScanId(scan As ScanReader) As String
            Return $"[{scan.x},{scan.y}]"
        End Function

        Public Overrides Function IsEmpty(scan As ScanReader) As Boolean
            Return scan.MzPtr Is Nothing OrElse scan.IntPtr Is Nothing
        End Function

        Public Overrides Function GetMsMs(scan As ScanReader) As ms2()
            Return scan.LoadMsData
        End Function

        Public Overrides Function GetMsLevel(scan As ScanReader) As Integer
            Return 1
        End Function

        Public Overrides Function GetBPC(scan As ScanReader) As Double
            Return 0
        End Function

        Public Overrides Function GetTIC(scan As ScanReader) As Double
            Return scan.totalIon
        End Function

        Public Overrides Function GetParentMz(scan As ScanReader) As Double
            Return 0
        End Function

        Public Overrides Function GetPolarity(scan As ScanReader) As String
            Return "+"
        End Function

        Public Overrides Function GetCharge(scan As ScanReader) As Integer
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetActivationMethod(scan As ScanReader) As ActivationMethods
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetCollisionEnergy(scan As ScanReader) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetCentroided(scan As ScanReader) As Boolean
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
