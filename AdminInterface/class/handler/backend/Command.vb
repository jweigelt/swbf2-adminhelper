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
'command baseclass
Public Class Command
    Public Property Permission As String
    Public Property IsPublic As Boolean = False
    Public Property CommandAlias As String
    Public Property BySuperAdmin As Boolean = False

    <Xml.Serialization.XmlIgnore>
    Public Property adminIface As Core

    Public Overridable Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Return True
    End Function

    Public Sub Say(ByVal str As String)
        If BySuperAdmin Then
            Logger.Log(str, LogLevel.info)
        Else
            adminIface.RCClient.Say(str)
        End If
    End Sub

    Public Sub Pm(ByVal str As String, ByVal u As User)
        If BySuperAdmin Then
            Logger.Log(str, LogLevel.info)
        Else
            adminIface.RCClient.Warn(str, u)
        End If
    End Sub

    Friend Function ParseTemplate(ByVal template As String, ByVal replacements() As String, Optional ByVal tags() As String = Nothing) As String
        Return IngameTemplate.Parse(template, replacements, tags)
    End Function

End Class
