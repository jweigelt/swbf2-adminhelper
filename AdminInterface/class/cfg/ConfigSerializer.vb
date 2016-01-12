'This file is part of SWBF2 SADS-Administation Helper.
'
'SWBF2 SADS-Administation Helper is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'SWBF2 SADS-Administation Helper is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with SWBF2 SADS-Administation Helper.  If not, see <http://www.gnu.org/licenses/>.
'
'XML-Serializer-Wrapper
Imports System.IO
Imports System.Xml.Serialization
Public Class ConfigSerializer

    Private serializer As XmlSerializer
    Public ReadOnly Property cfgType As Type
        Get
            Return _cfgType
        End Get
    End Property
    Private _cfgType As Type

    'asign the type up on object initialisation
    Sub New(ByVal cfgType As Type)
        Me._cfgType = cfgType
        serializer = New XmlSerializer(cfgType)
    End Sub

    Protected Overrides Sub Finalize()
        serializer = Nothing
    End Sub

    Public Sub saveToFile(ByVal fileName As String, ByVal directoryName As String, ByVal config As Object)
        Try
            If Not IO.Directory.Exists(directoryName) Then
                IO.Directory.CreateDirectory(directoryName)
            End If
            fileName = directoryName & fileName
            Dim writer As New StreamWriter(fileName)
            serializer.Serialize(writer, config)
            writer.Close()
        Catch ex As Exception
            Logger.Log(LogTemplate.CFG_WRITE_ERROR, LogLevel.failure, fileName)
        End Try
    End Sub

    Public Function loadFromFile(ByVal fileName As String, ByVal directoryName As String) As Object
        Dim filePath = directoryName & fileName
        Dim config As Object = Nothing
        If IO.File.Exists(filePath) Then
            Try
                Dim reader As New StreamReader(filePath)
                config = serializer.Deserialize(reader)
                reader.Close()
            Catch ex As Exception
                config = Activator.CreateInstance(_cfgType)
                Logger.Log(LogTemplate.CFG_READ_ERROR, LogLevel.failure, fileName)
            End Try
        Else
            Logger.Log(LogTemplate.CFG_WRITE_DEFAULT, LogLevel.info, fileName)
            config = Activator.CreateInstance(_cfgType)
            saveToFile(fileName, directoryName, config)
        End If
        Return config
    End Function

End Class
