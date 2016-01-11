Public Class RepeatingShedulerTask
    Inherits ShedulerTask
    Private lastExecution As Int64
    Private startTime As DateTime
    Public Property Interval As Int32 = 1000

    Sub New(ByVal delegateTask As funcDelegate, Optional ByVal param As Object = Nothing)
        MyBase.New(delegateTask, param)
        startTime = New DateTime(1970, 1, 1)
    End Sub

    Public Sub Tick()
        If (Me.lastExecution < Me.GetMillis() - Me.Interval) Then
            Me.lastExecution = Me.GetMillis()
            MyBase.Run()
        End If
    End Sub
    Private Function GetMillis() As Int64

        Return (DateTime.Now - startTime).TotalMilliseconds
    End Function
End Class