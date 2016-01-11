Public Class ShedulerTask
    Public Delegate Sub funcDelegate(ByVal obj As Object) 'Default Delegate
    Private delegateTask As funcDelegate
    Public Property param As Object

    Public Sub SetTask(ByVal delegateTask As funcDelegate)
        Me.delegateTask = delegateTask
    End Sub
    Public Overridable Sub Run()
        If Not IsNothing(delegateTask) Then
            delegateTask.Invoke(param)
        End If
    End Sub
    Sub New(ByVal delegateTask As funcDelegate, Optional ByVal param As Object = Nothing)
        If Not IsNothing(param) Then Me.param = param
        Me.delegateTask = delegateTask
    End Sub
End Class
