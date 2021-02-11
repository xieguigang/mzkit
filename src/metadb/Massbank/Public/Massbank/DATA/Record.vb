#Region "Microsoft.VisualBasic::18b6b4b9c911fbe96bbdb839efe5b66b, Massbank\Public\Massbank\DATA\Record.vb"

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

    '     Class Record
    ' 
    '         Properties: [DATE], AC, ACCESSION, AUTHORS, CH
    '                     COMMENT, COPYRIGHT, LICENSE, MS, PK
    '                     PUBLICATION, RECORD_TITLE, SP
    ' 
    '         Function: ToString
    ' 
    '     Class SP
    ' 
    '         Properties: LINEAGE, LINK, NAME, SAMPLE, SCIENTIFIC_NAME
    ' 
    '     Class CH
    ' 
    '         Properties: COMPOUND_CLASS, EXACT_MASS, FORMULA, IUPAC, LINK
    '                     NAME, SMILES
    ' 
    '         Function: ToString
    ' 
    '     Class AC
    ' 
    '         Properties: CHROMATOGRAPHY, INSTRUMENT, INSTRUMENT_TYPE, MASS_SPECTROMETRY
    ' 
    '         Function: ToString
    ' 
    '     Class MS
    ' 
    '         Properties: DATA_PROCESSING, FOCUSED_ION
    ' 
    '         Function: ToString
    ' 
    '     Class PK
    ' 
    '         Properties: ANNOTATION, NUM_PEAK, PEAK, SPLASH
    ' 
    '         Function: ToString
    ' 
    '     Structure PeakData
    ' 
    '         Properties: int, mz, relint
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace Massbank.DATA

    ''' <summary>
    ''' The massbank data records
    ''' </summary>
    Public Class Record : Implements INamedValue

        Public Property ACCESSION As String Implements INamedValue.Key
        Public Property RECORD_TITLE As String
        Public Property [DATE] As String
        Public Property AUTHORS As String
        Public Property LICENSE As String
        Public Property COPYRIGHT As String
        Public Property PUBLICATION As String
        Public Property COMMENT As String()
        Public Property CH As CH
        Public Property AC As AC
        Public Property MS As MS
        Public Property PK As PK
        Public Property SP As SP

        Public Overrides Function ToString() As String
            Return RECORD_TITLE
        End Function

    End Class

    Public Class SP

        Public Property SCIENTIFIC_NAME As String
        Public Property NAME As String
        Public Property LINEAGE As String
        Public Property LINK As String
        Public Property SAMPLE As String

    End Class

    Public Class CH

        Public Property NAME As String()
        Public Property COMPOUND_CLASS As String
        Public Property FORMULA As String
        Public Property EXACT_MASS As String
        Public Property SMILES As String

        ''' <summary>
        ''' ``InChI``
        ''' </summary>
        ''' <returns></returns>
        Public Property IUPAC As String
        Public Property LINK As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    Public Class AC

        Public Property INSTRUMENT As String
        Public Property INSTRUMENT_TYPE As String
        Public Property MASS_SPECTROMETRY As String()
        Public Property CHROMATOGRAPHY As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    Public Class MS

        Public Property FOCUSED_ION As String()
        Public Property DATA_PROCESSING As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    Public Class PK

        Public Property SPLASH As String
        Public Property ANNOTATION As Entity()
        Public Property NUM_PEAK As String
        Public Property PEAK As PeakData()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    Public Structure PeakData

        ''' <summary>
        ''' 碎片的质核比数据
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute, Column("m/z")> Public Property mz As Double
        <XmlAttribute, Column("int.")> Public Property int As Double

        ''' <summary>
        ''' 相对信号强度
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute, Column("rel.int.")>
        Public Property relint As Double

    End Structure
End Namespace
