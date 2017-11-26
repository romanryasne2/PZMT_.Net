netsh advfirewall firewall add rule name="Roman from Ryasne-2" dir=in action=allow protocol=TCP localport=139
netsh advfirewall firewall add rule name="Roman from Ryasne-2" dir=out action=allow protocol=TCP localport=139
pause