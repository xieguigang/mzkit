#Region "Microsoft.VisualBasic::5ad967cf39a3d69ecf660cde50ea1de6, assembly\assembly\UnifyReader\IDataReader.vb"

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

    '   Total Lines: 28
    '    Code Lines: 22 (78.57%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (21.43%)
    '     File Size: 1.06 KB


    '     Interface IDataReader
    ' 
    '         Function: GetActivationMethod, GetBPC, GetCentroided, GetCharge, GetCollisionEnergy
    '                   GetMsLevel, GetMsMs, GetParentMz, GetPolarity, GetScanId
    '                   GetScanTime, GetTIC, IsEmpty
    ' 
    '     Interface ISpectrumReader
    ' 
    '         Function: GetMsMs
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace DataReader

    Public Interface IDataReader

        Function GetScanTime(scan As Object) As Double
        Function GetScanId(scan As Object) As String
        Function IsEmpty(scan As Object) As Boolean

        Function GetMsMs(scan As Object) As ms2()
        Function GetMsLevel(scan As Object) As Integer
        Function GetBPC(scan As Object) As Double
        Function GetTIC(scan As Object) As Double
        Function GetParentMz(scan As Object) As Double
        Function GetPolarity(scan As Object) As String
        Function GetCharge(scan As Object) As Integer
        Function GetActivationMethod(scan As Object) As ActivationMethods
        Function GetCollisionEnergy(scan As Object) As Double
        Function GetCentroided(scan As Object) As Boolean

    End Interface

    Public Interface ISpectrumReader(Of T)
        Function GetMsMs(scan As T) As ms2()
    End Interface
End Namespace
