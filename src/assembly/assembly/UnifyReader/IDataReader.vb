#Region "Microsoft.VisualBasic::6439e049f15fcd4b329967f42a04c9f5, assembly\UnifyReader\IDataReader.vb"

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

    '     Interface IDataReader
    ' 
    '         Function: GetBPC, GetMsLevel, GetMsMs, GetParentMz, GetPolarity
    '                   GetScanId, GetScanTime, GetTIC, IsEmpty
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
End Namespace
