Public Class GenericCommandPacket
        Inherits TcpPacket

    Public Property Response As String = String.Empty

    Public Overrides Sub FetchData(ByVal data As String)
        Me.Response = data
        Me.PacketOK = True
    End Sub

    End Class
