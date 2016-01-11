Public Class CmdInfo
    Inherits Command

    Sub New()
        Me.CommandAlias = "info"
        Me.Permission = "info"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Me.adminIface.RCClient.SendRaw("say " & Constants.PRODUCT_NAME & " v" & Constants.PRODUCT_VERSION)

        Me.adminIface.RCClient.SendRaw("say " & Constants.PRODUCT_VENDOR)
        Return True
    End Function
End Class