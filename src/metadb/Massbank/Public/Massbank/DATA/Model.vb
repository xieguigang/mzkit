#Region "Microsoft.VisualBasic::383a616be156354e38d3f46855d127f3, Massbank\Public\Massbank\DATA\Model.vb"

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

    '     Class MASS_SPECTROMETRY
    ' 
    '         Properties: COLLISION_ENERGY, DATAFORMAT, DESOLVATION_GAS_FLOW, DESOLVATION_TEMPERATURE, FRAGMENTATION_METHOD
    '                     ION_MODE, IONIZATION, MS_TYPE, SCANNING, SOURCE_TEMPERATURE
    ' 
    '         Function: ToString
    ' 
    '     Class CHROMATOGRAPHY
    ' 
    '         Properties: COLUMN_NAME, COLUMN_TEMPERATURE, FLOW_GRADIENT, FLOW_RATE, RETENTION_TIME
    '                     SAMPLING_CONE, SOLVENT
    ' 
    '         Function: ToString
    ' 
    '     Class FOCUSED_ION
    ' 
    '         Properties: DERIVATIVE_FORM, DERIVATIVE_MASS, DERIVATIVE_TYPE, PRECURSOR_MZ, PRECURSOR_TYPE
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Massbank.DATA

    Public Class MASS_SPECTROMETRY

        Public Property MS_TYPE As String
        Public Property ION_MODE As String
        Public Property COLLISION_ENERGY As String
        Public Property DATAFORMAT As String
        Public Property DESOLVATION_GAS_FLOW As String
        Public Property DESOLVATION_TEMPERATURE As String
        Public Property FRAGMENTATION_METHOD As String
        Public Property IONIZATION As String
        Public Property SCANNING As String
        Public Property SOURCE_TEMPERATURE As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class CHROMATOGRAPHY

        Public Property COLUMN_NAME As String
        Public Property COLUMN_TEMPERATURE As String
        Public Property FLOW_GRADIENT As String
        Public Property FLOW_RATE As String
        Public Property RETENTION_TIME As String
        Public Property SAMPLING_CONE As String
        Public Property SOLVENT As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    Public Class FOCUSED_ION

        Public Property DERIVATIVE_FORM As String
        Public Property DERIVATIVE_MASS As String
        Public Property DERIVATIVE_TYPE As String
        <Column("PRECURSOR_M/Z")>
        Public Property PRECURSOR_MZ As String
        Public Property PRECURSOR_TYPE As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace
