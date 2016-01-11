Public Class CmdTest
    Inherits Command

    Public Property OnTestRegistered As String = "You are User %u, Slot %s, User-ID: %i, Group: %g"
    Public Property OnTestPlayer As String = "You are Player %u, Slot %s. You are not registered."

    Sub New()
        Me.CommandAlias = "test"
        Me.Permission = "test"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Me.adminIface.MySQL.GetUserDetails(player)
        If player.IsRegistered Then
            Me.Say(Me.ParseTemplate(Me.OnTestRegistered,
           {player.UserName, player.SlotId.ToString(), player.UserId.ToString(), player.GroupName},
           {"u", "s", "i", "g"}))
        Else
            Me.Say(Me.ParseTemplate(Me.OnTestPlayer,
           {player.UserName, player.SlotId.ToString()},
           {"u", "s"}))
        End If
       
        Return True
    End Function
End Class