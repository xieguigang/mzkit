
''' <summary>
''' the constant value for enumerate the reference library data types
''' </summary>
Public Enum ClusterTypes
    ''' <summary>
    ''' default library data type is the spectrum cluster tree,
    ''' this enumeration value is equaliviant to the 
    ''' <see cref="Tree"/>
    ''' </summary>
    [Default]

    ' 20230406
    ' ArgumentException: An item with the same key has already been added. Key: Tree

    ''' <summary>
    ''' default library data type is the spectrum cluster tree, 
    ''' this enumeration value is equaliviant to the 
    ''' <see cref="[Default]"/>
    ''' </summary>
    Tree ' = [Default]
    ''' <summary>
    ''' spectrum clustering in binary tree format
    ''' </summary>
    Binary
    ''' <summary>
    ''' spectrum packed with the same metabolite id 
    ''' </summary>
    Pack
End Enum
