Public Class CmdEndgame
    Inherits Command

    Public Property BroadcastMessage As String = "Map-reload forced by %u."
    Public Property DoBroadCast As Boolean = True

    Sub New()
        Me.CommandAlias = "endgame"
        Me.Permission = "endgame"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        If Me.DoBroadCast Then
            Me.Say(Me.ParseTemplate(BroadcastMessage, {player.UserName}, {"u"}))
        End If
        Me.adminIface.RCClient.SendRaw("endgame")
        Return True
    End Function
End Class