'AdminInterface v3.0
'Star Wars Battlefront II SADS-Administation Helper
'(C) 2012-2015 JW "LeKeks" :: info@janelo.net
'Compile using .NET-FW 4 < / 4 C.P. < (any cpu)
'Requires mysql.data v6.6.5.0, other versions can be used if the reference is updated
'You'll need to use a local copy of the DLL if the GAC's version differs!
Module main

    Sub Main()
        Logger.Log(Constants.PRODUCT_NAME & " " & Constants.PRODUCT_VERSION, LogLevel.always)
        Logger.Log(Constants.PRODUCT_VENDOR, LogLevel.always)
        Dim c As New Core
        c.Run()

        Console.ReadLine()
    End Sub

End Module