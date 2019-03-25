Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace NCBI.PubChem

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationNumber(section As Section, key$) As Double
            Dim info = section.getInformation(key)

            If info Is Nothing OrElse info.Value.Number.StringEmpty Then
                Return 0
            Else
                Return Val(info.Value.Number)
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationString(section As Section, key$) As String
            Dim info = section.getInformation(key)

            If info Is Nothing OrElse info.Value.StringWithMarkup Is Nothing Then
                Return ""
            Else
                Return info.Value.StringWithMarkup.FirstOrDefault?.String
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationStrings(section As Section, key$) As String()
            Dim info = section.getInformation(key)

            If info Is Nothing Then
                Return {}
            Else
                ' 当只有一个字符串的时候,可能会错误的判断为字符串对象
                ' 而非字符串数组
                ' 在这里需要检查一下
                Dim data = info.InfoValue

                If data Is Nothing Then
                    Return {}
                ElseIf TypeOf data Is String Then
                    Return {DirectCast(data, String)}
                ElseIf TypeOf data Is String() Then
                    Return data
                Else
                    Return {Scripting.ToString(data)}
                End If
            End If
        End Function

        <Extension>
        Private Function getInformation(section As Section, key$) As Information
            If section Is Nothing Then
                Return Nothing
            Else
                Return section _
                    .Information _
                    .SafeQuery _
                    .FirstOrDefault(Function(i) i.Name = key)
            End If
        End Function

        <Extension>
        Friend Function GetHMDBId(refs As Reference()) As String
            Return refs.SafeQuery _
                .FirstOrDefault(Function(ref)
                                    Return ref.SourceName = PugViewRecord.HMDB
                                End Function) _
               ?.SourceID
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationTable(section As Section, key$) As Table
            Return section.getInformation(key)?.Table
        End Function
    End Module
End Namespace