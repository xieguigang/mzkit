#Region "Microsoft.VisualBasic::f00485363afe7882e5c5580f191fa5f3, metadb\Chemoinformatics\MorganFingerprint\MorganFingerprint.vb"

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

    '   Total Lines: 122
    '    Code Lines: 60 (49.18%)
    ' Comment Lines: 44 (36.07%)
    '    - Xml Docs: 77.27%
    ' 
    '   Blank Lines: 18 (14.75%)
    '     File Size: 5.88 KB


    '     Module MorganFingerprint
    ' 
    '         Properties: FingerprintLength
    ' 
    '         Function: CalculateFingerprint, CalculateFingerprintCheckSum
    ' 
    '         Sub: SetLength
    '         Class HashBuilder
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: HashAtom, HashEdge
    ' 
    '         Class StructData
    ' 
    '             Properties: Atoms, Graph
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models
Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.MorganFingerprint
Imports Microsoft.VisualBasic.Math.HashMaps

Namespace MorganFingerprint

    ''' <summary>
    ''' Morgan fingerprints, also known as circular fingerprints, are a type of molecular fingerprint 
    ''' used in cheminformatics to represent the structure of chemical compounds. The algorithm steps 
    ''' for generating Morgan fingerprints are as follows:
    ''' 
    ''' 1. **Initialization**:
    '''  - Start with the initial set of atoms in the molecule.
    '''  - Assign a unique identifier (e.g., integer) to each atom.
    '''  
    ''' 2. **Atom Environment Encoding**:
    '''  - For each atom, encode its immediate environment, which includes the atom type and the types of its directly connected neighbors.
    '''  - This information can be represented as a string or a hash.
    '''  
    ''' 3. **Iterative Expansion**:
    '''  - Expand the environment encoding iteratively to include atoms further away from the starting atom.
    '''  - In each iteration, update the encoding to include the types of atoms that are two, three, etc., bonds away from the starting atom.
    '''  
    ''' 4. **Hashing**:
    '''  - Convert the environment encoding into a fixed-size integer using a hashing function. This integer represents the fingerprint of the atom's environment.
    '''  - Different atoms in the molecule will have different fingerprints based on their environments.
    '''  
    ''' 5. **Circular Fingerprint Generation**:
    '''  - For each atom, generate a series of fingerprints that represent its environment at different radii (number of bonds away).
    '''  - The final fingerprint for an atom is a combination of these series of fingerprints.
    '''  
    ''' 6. **Molecular Fingerprint**:
    '''  - Combine the fingerprints of all atoms in the molecule to create the final molecular fingerprint.
    '''  - This can be done by taking the bitwise OR of all atom fingerprints, resulting in a single fingerprint that represents the entire molecule.
    '''  
    ''' 7. **Optional Folding**:
    '''  - To reduce the size of the fingerprint, an optional folding step can be applied. This involves 
    '''    dividing the fingerprint into chunks and performing a bitwise XOR operation within each chunk.
    '''    
    ''' 8. **Result**:
    '''  - The final result is a binary vector (or a list of integers) that represents the Morgan fingerprint 
    '''    of the molecule. This fingerprint can be used for similarity searching, clustering, and other 
    '''    cheminformatics tasks.
    '''    
    ''' Morgan fingerprints are particularly useful because they capture the circular nature of molecular
    ''' environments, meaning that the path taken to reach an atom is not as important as the environment 
    ''' around it. This makes them effective for comparing the similarity of molecules based on their 
    ''' structural features.
    ''' </summary>
    Public Module MorganFingerprint

        Public ReadOnly Property FingerprintLength As Integer
            Get
                Return m_hashBuilder.FingerprintLength
            End Get
        End Property

        Private Class HashBuilder : Inherits GraphMorganFingerprint(Of MorganAtom, Bound)

            Public Sub New(size As Integer)
                MyBase.New(size)
            End Sub

            Protected Overrides Function HashAtom(v As MorganAtom) As ULong
                ' 20250514 do not use the internal string gethashcode function
                '
                ' 在 .NET 中，字符串的 `GetHashCode()` 方法返回的哈希值**并不能保证在不同时间、操作系统或硬件上保持一致**。以下是具体原因和背景分析：
                '
                ' ---
                ' 
                ' ### 1. **实现依赖性与版本差异**
                ' - **不同 .NET 版本可能生成不同的哈希值**  
                '   微软明确声明，`GetHashCode()` 的实现可能因公共语言运行时（CLR）的版本变化而调整，例如从 .NET Framework 4.x 到 .NET Core 或 .NET 5+ 的升级。这种调整可能是出于性能优化或哈希分布均匀性的考虑。
                '   
                ' - **哈希随机化（.NET Core 2.1+）**  
                '   从 .NET Core 2.1 开始，默认启用了哈希随机化机制，即使同一程序在不同时间运行，同一字符串的哈希值也可能不同。此设计旨在防止哈希碰撞攻击，增强安全性。
                ' 
                ' ---
                ' 
                ' ### 2. **跨操作系统与硬件的差异**
                ' - **操作系统的影响**  
                '   .NET 的不同运行时（如 .NET Framework 仅支持 Windows，而 .NET Core 支持跨平台）在实现哈希算法时可能采用不同策略。例如，Windows 和 Linux 上的哈希计算结果可能不一致。
                ' 
                ' - **硬件架构的差异**  
                '   32 位与 64 位系统的内存寻址方式不同，可能影响 `GetHashCode()` 的默认行为（如对象地址计算）。此外，CPU 架构（x86/x64/ARM）也可能导致哈希值差异。
                ' 
                ' ---
                ' 
                ' ### 3. **设计原则与使用场景限制**
                ' - **仅保证同一进程内的唯一性**  
                '   `GetHashCode()` 的主要设计目标是支持哈希表等数据结构的高效查找，其核心保证是：**在同一进程的同一执行周期内，相同内容的字符串返回相同的哈希值**。但跨进程、跨机器或持久化存储时，这一保证失效。
                ' 
                ' - **哈希冲突的可能性**  
                '   即使在同一环境中，不同字符串可能生成相同的哈希值（哈希碰撞）。例如，字符串 `"FB"` 和 `"Ea"` 在某些情况下哈希值相同。
                ' 
                ' ---
                '
                '
                Return HashLabelKey(v.Atom)
            End Function

            Protected Overrides Function HashEdge(atoms() As MorganAtom, e As Bound, flip As Boolean) As ULong
                Dim hashcode As ULong

                If flip Then
                    hashcode = HashMap.HashCodePair(atoms(e.i).Code, atoms(e.j).Code)
                Else
                    hashcode = HashMap.HashCodePair(atoms(e.j).Code, atoms(e.i).Code)
                End If

                Return hashcode Xor CULng(e.Type) Xor CULng(e.Stereo)
            End Function
        End Class

        Private Class StructData : Implements MorganGraph(Of MorganAtom, Bound)

            Public Property Atoms As MorganAtom() Implements MorganGraph(Of MorganAtom, Bound).Atoms
            Public Property Graph As Bound() Implements MorganGraph(Of MorganAtom, Bound).Graph

        End Class

        Dim m_hashBuilder As New HashBuilder(4096)

        Public Sub SetLength(len As Integer)
            If len Mod 8 <> 0 Then
                Throw New InvalidProgramException("the length of the Morgan Fingerprint vector should be mod by 8!")
            Else
                m_hashBuilder = New HashBuilder(len)
            End If
        End Sub

        <Extension>
        Public Function CalculateFingerprintCheckSum(struct As [Structure], Optional radius As Integer = 3) As Byte()
            Dim bits As BitArray = struct.CalculateFingerprint(radius)
            Dim bytes = New Byte(FingerprintLength / 8 - 1) {}
            bits.CopyTo(bytes, 0)
            Return bytes
        End Function

        <Extension>
        Public Function CalculateFingerprint(struct As [Structure], Optional radius As Integer = 3) As BitArray
            Dim atoms As MorganAtom() = New MorganAtom(struct.Atoms.Length - 1) {}

            ' Initialize atom codes based on atom type
            For i As Integer = 0 To struct.Atoms.Length - 1
                atoms(i) = New MorganAtom(struct.Atoms(i))
            Next

            Return m_hashBuilder.CalculateFingerprint(New StructData With {
                .Atoms = atoms,
                .Graph = struct.Bounds
            }, radius)
        End Function
    End Module
End Namespace
