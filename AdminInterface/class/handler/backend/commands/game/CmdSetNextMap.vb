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
Public Class CmdSetNextMap
    Inherits Command

    Public Property OnSet As String = "Next map was set to %m."
    Public Property OnSyntaxError As String = "Syntax: !setnextmap <mapname>"

    Sub New()
        Me.CommandAlias = "setnextmap"
        Me.Permission = "setnextmap"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")
        Dim map As String = Replace(params(1), "/", String.Empty)
        If params.Length >= 2 Then
            Me.adminIface.RCClient.SendRaw("removemap " & map)
            Me.adminIface.RCClient.SendRaw("addmap " & map)
            'Me.adminIface.RCClient.SendRaw("nextmap " & map)
            Me.Say(Me.ParseTemplate(OnSet, {map}, {"m"}))
            Me.adminIface.Config.ServerInfo.NextMap = map
            Return True
        Else
            Me.Say(Me.OnSyntaxError)
            Return False
        End If
    End Function
End Class