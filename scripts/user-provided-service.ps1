$ErrorActionPreference = "Stop"

#assume you have already logged in and targeted the correct Org/Space

$base64String = "MIID........"#<--- replace with your base64 string and remember to remove header/footer

$addressParamJSON = [string]::Format('{{\"base64-certificate\":\"{0}\"}}',$base64String)

#Create a user provided service with the network address of the SMB share
cf create-user-provided-service "root-certificate" -p $addressParamJSON

