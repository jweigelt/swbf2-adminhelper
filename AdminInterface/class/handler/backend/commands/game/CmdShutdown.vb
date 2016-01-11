Public Class CmdShutdown
    Inherits Command

    Public Property OnShutdown As String = "Server shutting down."

    Sub New()
        Me.CommandAlias = "shutdown"
        Me.Permission = "shutdown"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Me.adminIface.RCClient.SendRaw("shutdown")
        Me.adminIface.Shutdown()
        Return True
    End Function
End Class