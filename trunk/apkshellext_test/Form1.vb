Imports KKHomeProj.Android

Public Class Form1

    Private filename As String

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim apk As AndroidPackage2

        If OpenFileDialog1.ShowDialog() <> Windows.Forms.DialogResult.Cancel Then
            filename = OpenFileDialog1.FileName
        End If

        apk = AndroidPackage2.GetAndroidPackage(filename)
        Dim icon As Icon = apk.icon
        PictureBox1.Image = icon.ToBitmap

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim devices As ArrayList
        devices = AndroidDevice.GetAndroidDevices()
        For Each d As AndroidDevice In devices
            TextBox1.Text = TextBox1.Text & vbCrLf & d.Serialno
        Next
    End Sub
End Class
