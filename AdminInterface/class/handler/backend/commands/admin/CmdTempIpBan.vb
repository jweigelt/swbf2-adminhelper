Public Class CmdTempIpBan
    Inherits CmdTempBan

    Sub New()
        Me.CommandAlias = "tempipban"
        Me.Permission = "tempipban"
        Me.IsPublic = False
    End Sub

    Public Overrides Sub SubmitBan(ByVal affectedUser As User, ByVal player As User, ByVal duration As Int32)
        Me.adminIface.MySQL.InsertBan(affectedUser, player, True, duration)
    End Sub

End Class