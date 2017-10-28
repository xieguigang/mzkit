Imports System.Reflection

Public Module AppHelper

    Public Function LoadData(Of T As Structure)() As mysql.app()
        Dim values As FieldInfo() = GetType(T) _
            .GetFields _
            .Where(Function(o) o.FieldType Is GetType(T)) _
            .ToArray
        Dim apps As mysql.app() = values _
            .Select(Function(field)
                        Return New mysql.app With {
                            .name = field.Name,
                            .description = field.Description,
                            .catagory = field.Category,
                            .uid = field.GetValue(Nothing)
                        }
                    End Function) _
            .ToArray

        Return apps
    End Function
End Module
