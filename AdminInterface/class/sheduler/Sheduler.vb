'This file is part of SWBF2 SADS-Administation Helper.
'
'SWBF2 SADS-Administation Helper is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'SWBF2 SADS-Administation Helper is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with SWBF2 SADS-Administation Helper.  If not, see <http://www.gnu.org/licenses/>.

'Scheduler-Class
'As several components share the same resources they have to use the same thread so access them
'p.e. we can't send while receiving another packet
Public Class Sheduler

    Private tasks As Queue(Of ShedulerTask)
    Private repeatingTasks As List(Of RepeatingShedulerTask)
    Private ticks As UInt32
    Private workThread As Threading.Thread
    Private _running As Boolean

    Public ReadOnly Property Running As Boolean
        Get
            Return Me._running
        End Get
    End Property
    Public Property TickDelay As Int32 = 10

    Public Function Init() As Boolean
        Me._running = True
        Me.workThread = New Threading.Thread(AddressOf Me.WorkThread_Run)
        Me.workThread.Start()
        Return True
    End Function

    Public Sub Terminate()
        Me.ClearQueue()
        Me._running = False
        Threading.Thread.Sleep(Me.TickDelay)
        If Me.workThread.IsAlive Then
            Me.workThread.Abort()
        End If
        Me.workThread = Nothing
        Me.tasks = Nothing
        Me.repeatingTasks = Nothing
    End Sub

    Private Sub WorkThread_Run()
        While Me._running
            Try
                Me.Tick()
            Catch ex As Exception
                Logger.Log(LogTemplate.SHEDULER_EXCEPTION, LogLevel.warning, ex.ToString)
            End Try
            Threading.Thread.Sleep(Me.TickDelay)
        End While
    End Sub

    Sub New()
        Me.tasks = New Queue(Of ShedulerTask)
        Me.repeatingTasks = New List(Of RepeatingShedulerTask)
    End Sub

    Public Sub PushTask(ByVal tsk As ShedulerTask)
        Me.tasks.Enqueue(tsk)
    End Sub

    Public Sub PushRepeatingTask(ByVal tsk As RepeatingShedulerTask, ByVal interval As Int32)
        tsk.Interval = interval
        Me.repeatingTasks.Add(tsk)
    End Sub

    Public Sub ClearQueue()
        Me.tasks.Clear()
        Me.repeatingTasks.Clear()
    End Sub

    Private Sub Tick()
        If Me.tasks.Count > 0 Then
            Me.tasks.Dequeue.Run()
        End If
        For Each t As RepeatingShedulerTask In Me.repeatingTasks
            t.Tick()
        Next
    End Sub

End Class