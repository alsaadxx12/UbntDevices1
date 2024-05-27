Imports System.Web.Mvc
Imports Renci.SshNet
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports Renci.SshNet.Common
Imports System.Net.Sockets
Imports System.Threading

Namespace Controllers
    Public Class DevicesController
        Inherits Controller

        Function Index() As ActionResult
            Return View()
        End Function

        <HttpPost>
        Async Function SaveSettings(ipAddress As String, username As String, password As String) As Task(Of JsonResult)
            Try
                Session("IPAddress") = ipAddress
                Session("Username") = username
                Session("Password") = password

                ' بعد الحفظ، جلب البيانات من الأجهزة باستخدام عنوان IP المخزن
                Dim devices = Await DiscoverDevices(ipAddress, username, password)

                Return Json(New With {.success = True, .devices = devices})
            Catch ex As Exception
                Return Json(New With {.success = False, .message = ex.Message})
            End Try
        End Function

        <HttpPost>
        Async Function ConnectToDevice(ip As String, username As String, password As String) As Task(Of JsonResult)
            Try
                Dim deviceInfo = Await GetDeviceInfoAsync(ip, username, password)
                Return Json(New With {.success = True, .deviceInfo = deviceInfo})
            Catch ex As Exception
                Return Json(New With {.success = False, .message = ex.Message})
            End Try
        End Function

        Private Async Function GetDeviceInfoAsync(ip As String, username As String, password As String) As Task(Of Object)
            Dim deviceInfo As Object = Nothing
            Try
                Dim authMethod = New PasswordAuthenticationMethod(username, password)
                Dim connectionInfo = New ConnectionInfo(ip, 22, username, authMethod)

                Using client As New SshClient(connectionInfo)
                    client.Connect()
                    If client.IsConnected Then
                        ' تنفيذ الأوامر لجلب الإشارة، ESSID، والتردد
                        Dim command = client.RunCommand("iwconfig ath0")
                        Dim commandResult = command.Result.Trim()

                        ' استخدام التعبيرات العادية لاستخراج قيمة الإشارة، ESSID، والتردد
                        Dim signalRegex As New Regex("Signal level=(-?\d+ dBm)")
                        Dim signalMatch = signalRegex.Match(commandResult)
                        Dim signal As String = If(signalMatch.Success, signalMatch.Groups(1).Value, "N/A")

                        Dim ssidRegex As New Regex("ESSID:\x22([^\x22]+)\x22")
                        Dim ssidMatch = ssidRegex.Match(commandResult)
                        Dim ssid As String = If(ssidMatch.Success, ssidMatch.Groups(1).Value, "N/A")

                        Dim frequencyRegex As New Regex("Frequency:(\d+\.\d+ GHz)")
                        Dim frequencyMatch = frequencyRegex.Match(commandResult)
                        Dim frequency As String = If(frequencyMatch.Success, frequencyMatch.Groups(1).Value, "N/A")

                        ' تنفيذ الأوامر لجلب وقت التشغيل
                        Dim uptimeCommand = client.RunCommand("uptime")
                        Dim uptimeInfo = uptimeCommand.Result.Trim()

                        ' استخدام التعبيرات العادية لاستخراج وقت التشغيل
                        Dim uptimeRegex As New Regex("up\s+(?:(\d+\s+days?,\s+\d+:\d+)|(\d+:\d+)|(\d+\s+min))")
                        Dim uptimeMatch = uptimeRegex.Match(uptimeInfo)
                        Dim uptime As String = "N/A"

                        If uptimeMatch.Success Then
                            If Not String.IsNullOrEmpty(uptimeMatch.Groups(1).Value) Then
                                uptime = uptimeMatch.Groups(1).Value
                            ElseIf Not String.IsNullOrEmpty(uptimeMatch.Groups(2).Value) Then
                                uptime = uptimeMatch.Groups(2).Value
                            ElseIf Not String.IsNullOrEmpty(uptimeMatch.Groups(3).Value) Then
                                uptime = uptimeMatch.Groups(3).Value
                            End If
                        Else
                            Debug.WriteLine($"Failed to parse uptime for {ip}: {uptimeInfo}")
                        End If

                        ' تنفيذ الأمر لجلب إصدار البرنامج الثابت
                        Dim firmwareCommand = client.RunCommand("mca-status | grep 'firmwareVersion'")
                        Dim firmwareResult = firmwareCommand.Result.Trim()
                        Dim firmwareRegex As New Regex("firmwareVersion=([\w.-]+)")
                        Dim firmwareMatch = firmwareRegex.Match(firmwareResult)
                        Dim firmwareVersion As String = If(firmwareMatch.Success, firmwareMatch.Groups(1).Value, "N/A")

                        ' تعديل لإستخراج الجزء المطلوب فقط من إصدار البرنامج الثابت
                        Dim shortFirmwareRegex As New Regex("^(XW\.ar934x\.v\d+\.\d+\.\d+)")
                        Dim shortFirmwareMatch = shortFirmwareRegex.Match(firmwareVersion)
                        Dim shortFirmwareVersion As String = If(shortFirmwareMatch.Success, shortFirmwareMatch.Groups(1).Value, firmwareVersion)

                        client.Disconnect()

                        deviceInfo = New With {
                            .signal = signal,
                            .ssid = ssid,
                            .frequency = frequency,
                            .uptime = uptime,
                            .firmwareVersion = shortFirmwareVersion
                        }
                    End If
                End Using
            Catch ex As Exception
                deviceInfo = New With {.success = False, .message = ex.Message}
            End Try
            Return deviceInfo
        End Function

        Private Function IsInSameRange(ip As String, baseIp As String) As Boolean
            Dim ipParts = ip.Split("."c)
            Dim baseIpParts = baseIp.Split("."c)

            If ipParts.Length = 4 AndAlso baseIpParts.Length = 4 Then
                Return ipParts(0) = baseIpParts(0) AndAlso ipParts(1) = baseIpParts(1) AndAlso ipParts(2) = baseIpParts(2)
            End If

            Return False
        End Function

        Private Async Function DiscoverDevices(ipAddress As String, username As String, password As String) As Task(Of List(Of Device))
            Dim devices As New List(Of Device)
            Try
                Dim authMethod = New PasswordAuthenticationMethod(username, password)
                Dim connectionInfo = New ConnectionInfo(ipAddress, 22, username, authMethod)

                Using client As New SshClient(connectionInfo)
                    client.Connect()
                    If client.IsConnected Then
                        Dim command = Await Task.Run(Function() client.RunCommand("discover"))
                        Dim result = command.Result.Trim()

                        If Not String.IsNullOrEmpty(result) Then
                            Dim regex As New Regex("([0-9A-Fa-f:]+)\s+([0-9.]+)\s+(.+)")
                            Dim matches = regex.Matches(result)

                            Dim tasks As New List(Of Task)()
                            For Each match As Match In matches
                                tasks.Add(Task.Run(Sub()
                                                       Dim host = match.Groups(2).Value
                                                       If IsInSameRange(host, ipAddress) Then
                                                           devices.Add(New Device With {
                                                               .MAC = match.Groups(1).Value,
                                                               .Host = host,
                                                               .Name = match.Groups(3).Value
                                                           })
                                                       End If
                                                   End Sub))
                            Next

                            Await Task.WhenAll(tasks)
                        End If

                        client.Disconnect()
                    End If
                End Using
            Catch ex As SshAuthenticationException
                ' معالجة خطأ المصادقة
                devices.Add(New Device With {
                    .Name = "Authentication failed",
                    .ErrorMessage = ex.Message
                })
            Catch ex As Exception
                ' معالجة الأخطاء الأخرى
                devices.Add(New Device With {
                    .Name = "Connection failed",
                    .ErrorMessage = ex.Message
                })
            End Try
            Return devices
        End Function
    End Class

    Public Class Device
        Public Property Name As String
        Public Property Signal As String
        Public Property Uptime As String
        Public Property Host As String
        Public Property Frequency As String
        Public Property MAC As String
        Public Property Firmware As String
        Public Property SSID As String
        Public Property ErrorMessage As String
    End Class
End Namespace
