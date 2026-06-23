Imports System.Text.RegularExpressions

Namespace MetaLib

    ''' <summary>
    ''' 代谢物名称解析器
    ''' <para>从多个公共数据库采集的同义名集合中，通过 NLP 文本挖掘方案生成唯一的俗名。</para>
    ''' <para>算法流程：预处理(Title Case) → 正则清洗 → 停用词过滤 → 最高频最短字符串投票 → 最终 Trim</para>
    ''' </summary>
    Public Class MetaboliteNameResolver

        ' ============================================================
        '  停用词集合（不区分大小写）
        '  涵盖化学/药物数据库中常见的元数据标签、语言标识、药典参考词等
        ' ============================================================
        Private Shared ReadOnly StopWords As HashSet(Of String)

        ' 静态构造函数：初始化停用词表
        Shared Sub New()
            StopWords = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            ' 1. 药典与命名机构标签（INN/BAN/DCF/USAN/JAN 等）
            Dim authorityTags As String() = {
                "INN", "BAN", "DCF", "USAN", "JAN", "USP", "BP", "EP", "NF",
                "WHO-DD", "WHO", "DD", "MART", "MI", "HSDB", "OSHA",
                "VANDA", "VAN", "WLN", "AN", "DCIT", "IS", "OR",
                "Ph", "Eur", "Ph.Eur."
            }
            Call AddStopWords(authorityTags)

            ' 2. 语言标识（INN-Spanish / INN-French 等）
            Dim languageTags As String() = {
                "Spanish", "French", "Latin", "English", "German", "Italian",
                "Japanese", "Chinese", "Russian", "Portuguese", "Dutch",
                "INN-Spanish", "INN-French", "INN-Latin", "INN-English"
            }
            Call AddStopWords(languageTags)

            ' 3. 药典参考标准相关词汇
            Dim referenceTerms As String() = {
                "European", "Pharmacopoeia", "Reference", "Standard", "Monograph",
                "EP-MONOGRAPH", "BP-MONOGRAPH", "USP-MONOGRAPH",
                "ratiopharm"
            }
            Call AddStopWords(referenceTerms)
        End Sub

        Private Shared Sub AddStopWords(words As String())
            For Each w As String In words
                StopWords.Add(w)
            Next
        End Sub

        ''' <summary>
        ''' 从同义名集合中解析出最佳唯一名称
        ''' </summary>
        ''' <param name="synonyms">同义名集合</param>
        ''' <returns>最佳唯一名称；若输入为空则返回空字符串</returns>
        Public Function ResolveBestName(synonyms As IEnumerable(Of String)) As String
            ' ---------- 输入校验 ----------
            If synonyms Is Nothing Then
                Return String.Empty
            End If

            ' ---------- 收集所有清洗后的名称 ----------
            Dim cleanedNames As New List(Of String)()

            For Each synonym As String In synonyms
                If String.IsNullOrWhiteSpace(synonym) Then
                    Continue For
                End If

                ' 步骤1：预处理 - 按空格分割、Title Case、空格拼接
                Dim preprocessed As String = PreprocessToTitleCase(synonym)

                ' 步骤2：正则清洗 + 停用词过滤
                Dim cleaned As String = CleanAndFilter(preprocessed)

                If Not String.IsNullOrWhiteSpace(cleaned) Then
                    cleanedNames.Add(cleaned)
                End If
            Next

            If cleanedNames.Count = 0 Then
                Return String.Empty
            End If

            ' ---------- 步骤3：投票 - 最高频最短字符串 ----------
            Dim bestName As String = VoteByFrequencyAndLength(cleanedNames)

            ' ---------- 步骤4：最终 Trim ----------
            Return FinalTrim(bestName)
        End Function

        ''' <summary>
        ''' 预处理：按空格分割 → 每个单词首字母大写(Title Case) → 空格拼接
        ''' <para>例如 "TROXERUTIN [INN]" → "Troxerutin [Inn]"</para>
        ''' </summary>
        Private Function PreprocessToTitleCase(name As String) As String
            If String.IsNullOrWhiteSpace(name) Then
                Return String.Empty
            End If

            ' 按空格分割
            Dim words As String() = name.Split(" "c)

            ' 逐词转换为 Title Case
            Dim titleWords As New List(Of String)(words.Length)
            Dim i As Integer
            For i = 0 To words.Length - 1
                Dim word As String = words(i)
                If String.IsNullOrEmpty(word) Then
                    titleWords.Add(word)
                Else
                    ' 首字母大写 + 其余字母小写
                    Dim titleWord As String = Char.ToUpper(word.Chars(0)) & word.Substring(1).ToLower()
                    titleWords.Add(titleWord)
                End If
            Next

            ' 用空格拼接
            Return String.Join(" ", titleWords)
        End Function

        ''' <summary>
        ''' 正则清洗 + 停用词过滤
        ''' <para>清洗规则：</para>
        ''' <para>① 移除方括号 [xxx]、圆括号 (xxx)、花括号 {xxx} 内的内容</para>
        ''' <para>② 按连字符/逗号/分号/冒号分割，取第一段（处理 "Troxerutin-ratiopharm" → "Troxerutin"）</para>
        ''' <para>③ 按空格分词，移除停用词</para>
        ''' </summary>
        Private Function CleanAndFilter(name As String) As String
            If String.IsNullOrWhiteSpace(name) Then
                Return String.Empty
            End If

            Dim cleaned As String = name

            ' 正则清洗①：移除方括号内容 [xxx]
            cleaned = Regex.Replace(cleaned, "\[[^\]]*\]", " ")

            ' 正则清洗②：移除圆括号内容 (xxx)
            cleaned = Regex.Replace(cleaned, "\([^)]*\)", " ")

            ' 正则清洗③：移除花括号内容 {xxx}
            cleaned = Regex.Replace(cleaned, "\{[^}]*\}", " ")

            ' 按分隔符（连字符、逗号、分号、冒号）分割，取第一段
            ' 处理 "Troxerutin-ratiopharm" → "Troxerutin"
            ' 处理 "Troxerutin, European..." → "Troxerutin"
            ' 处理 "Troxerutin INN:BAN:DCF" → "Troxerutin INN"
            Dim delimiters As Char() = {"-"c, ","c, ";"c, ":"c}
            Dim parts As String() = cleaned.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)

            If parts.Length = 0 Then
                Return String.Empty
            End If

            cleaned = parts(0).Trim()

            ' 停用词过滤：按空格分词，移除停用词
            Dim words As String() = cleaned.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
            Dim filteredWords As New List(Of String)(words.Length)

            Dim j As Integer
            For j = 0 To words.Length - 1
                Dim word As String = words(j)
                ' 同时检查原词和去句点后的词（如 "MART." → "MART"）
                Dim wordNoPeriod As String = word.Trim("."c)

                If Not StopWords.Contains(word) AndAlso Not StopWords.Contains(wordNoPeriod) Then
                    filteredWords.Add(word)
                End If
            Next

            If filteredWords.Count = 0 Then
                Return String.Empty
            End If

            Return String.Join(" ", filteredWords)
        End Function

        ''' <summary>
        ''' 投票算法：在所有候选名称中，选择出现频率最高的；
        ''' 若频率相同，则选择字符串长度最短的。
        ''' <para>使用 Math.Max / Math.Min 等基础数学函数进行比较</para>
        ''' </summary>
        Private Function VoteByFrequencyAndLength(names As List(Of String)) As String
            ' ---------- 统计频率 ----------
            Dim frequencyMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            Dim i As Integer
            For i = 0 To names.Count - 1
                Dim key As String = names(i).Trim()
                If String.IsNullOrEmpty(key) Then
                    Continue For
                End If

                If frequencyMap.ContainsKey(key) Then
                    frequencyMap(key) = frequencyMap(key) + 1
                Else
                    frequencyMap.Add(key, 1)
                End If
            Next

            If frequencyMap.Count = 0 Then
                Return String.Empty
            End If

            ' ---------- 找出最高频率（使用 Math.Max）----------
            Dim maxFrequency As Integer = 0
            For Each kvp As KeyValuePair(Of String, Integer) In frequencyMap
                maxFrequency = Math.Max(maxFrequency, kvp.Value)
            Next

            ' ---------- 在最高频率候选中，找出最短字符串 ----------
            Dim bestName As String = String.Empty
            Dim minLength As Integer = Integer.MaxValue

            For Each kvp As KeyValuePair(Of String, Integer) In frequencyMap
                If kvp.Value = maxFrequency Then
                    Dim currentLength As Integer = kvp.Key.Length
                    ' 使用基础数学比较：长度更短者胜出
                    If currentLength < minLength Then
                        minLength = currentLength
                        bestName = kvp.Key
                    End If
                End If
            Next

            Return bestName
        End Function

        ''' <summary>
        ''' 最终 Trim：
        ''' <para>1. 移除首尾的空格、逗号、分号、加号、减号</para>
        ''' <para>2. 使用正则表达式将多个连续空格替换为单个空格</para>
        ''' </summary>
        Private Function FinalTrim(name As String) As String
            If String.IsNullOrEmpty(name) Then
                Return String.Empty
            End If

            ' Trim 指定字符：空格、逗号、分号、加号、减号
            Dim trimChars As Char() = {" "c, ","c, ";"c, "+"c, "-"c}
            Dim trimmed As String = name.Trim(trimChars)

            ' 正则替换：多个连续空格 → 单个空格
            trimmed = Regex.Replace(trimmed, " +", " ")

            Return trimmed.Trim()
        End Function

    End Class

End Namespace
