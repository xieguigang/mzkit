Imports System.ComponentModel

''' <summary>
''' Single, double, triple, and aromatic bonds are 
''' represented by the symbols ``-``, ``=``, ``#``, and ``:``, 
''' respectively. Adjacent atoms are assumed to be 
''' connected to each other by a single or aromatic 
''' bond (single and aromatic bonds may always be 
''' omitted).
''' </summary>
Public Enum Bonds As Byte
    <Description("-")> [single] = 1
    <Description("=")> [double] = 2
    <Description("#")> triple = 3
    <Description(":")> aromatic = 6
End Enum
