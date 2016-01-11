Public Class TcpPacket
    Public Property CommandAlias As String
    Public Property PacketOK As Boolean = False

    Public Overridable Sub FetchData(ByVal data As String)

    End Sub

End Class
