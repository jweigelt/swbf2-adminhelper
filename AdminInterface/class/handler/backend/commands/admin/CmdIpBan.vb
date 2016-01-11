Public Class CmdIpBan
    Inherits CmdBan

    Sub New()
        Me.CommandAlias = "ipban"
        Me.Permission = "ipban"
        Me.IsPublic = False
    End Sub

    Public Overrides Sub SubmitBan(ByVal affectedUser As User, ByVal player As User)
        Me.adminIface.MySQL.InsertBan(affectedUser, player, True)
    End Sub
End Class