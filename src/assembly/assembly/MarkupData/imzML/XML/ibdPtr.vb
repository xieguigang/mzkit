Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.imzML

    ''' <summary>
    ''' ibd pointer data
    ''' </summary>
    Public Class ibdPtr

        Public Property offset As Long
        Public Property arrayLength As Integer
        Public Property encodedLength As Integer

        Public ReadOnly Property UnderlyingType As Type
            Get
                Dim sizeof As Integer = encodedLength / arrayLength

                If sizeof = 4 Then
                    Return GetType(Single)
                Else
                    Return GetType(Double)
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[&{offset.ToHexString}] new {UnderlyingType.Name.ToLower}({arrayLength} - 1){{}}"
        End Function

        Friend Shared Function ParsePtr(bin As binaryDataArray) As ibdPtr
            Dim arrayLength As Integer = Integer.Parse(bin.cvParams.KeyItem("external array length").value)
            Dim offset As Long = Long.Parse(bin.cvParams.KeyItem("external offset").value)
            Dim encodedLength As Integer = Integer.Parse(bin.cvParams.KeyItem("external encoded length").value)

            Return New ibdPtr With {
                .offset = offset,
                .arrayLength = arrayLength,
                .encodedLength = encodedLength
            }
        End Function
    End Class
End Namespace