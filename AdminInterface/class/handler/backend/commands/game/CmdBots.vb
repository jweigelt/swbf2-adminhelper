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
Public Class CmdBots
    Inherits Command

    Public Property OnSet As String = "%u set bot count to %c."
    Public Property OnSyntaxError As String = "Syntax: !bots <count>"
    Public Property OnToManyBots As String = "%c exceeds max. count (%max)"
    Public Property MaxCount As Int32 = 16

    Sub New()
        Me.CommandAlias = "bots"
        Me.Permission = "bots"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        If params.Length >= 2 Then
            Dim c As Int32
            If Integer.TryParse(params(1), c) Then
                If c < MaxCount Then
                    Me.adminIface.RCClient.SendRaw("bots " & c.ToString())
                    Me.Say(Me.ParseTemplate(Me.OnSet, {player.UserName, c.ToString()}, {"u", "c"}))
                    Return True
                Else
                    Me.Say(Me.ParseTemplate(Me.OnToManyBots, {params(1), MaxCount.ToString()}, {"c", "max"}))
                End If
            End If
        End If

        Me.Say(Me.OnSyntaxError)
        Return True
    End Function
End Class