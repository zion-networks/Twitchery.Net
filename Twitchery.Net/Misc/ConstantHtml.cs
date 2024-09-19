namespace TwitcheryNet.Misc;

public static class ConstantHtml
{
    public const string OAuthHtml =
"""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Authentication Successfull!</title>
</head>
<body>
    <h1>Authentication Successfull!</h1>
    <p>You can now close this window and return to the application.</p>

    <script>
        let hash = window.location.hash;
        let parts = hash.split('&');
        let accessToken = parts[0].split('=')[1];
        let scope = parts[1].split('=')[1];
        let state = parts[2].split('=')[1];
        let tokenType = parts[3].split('=')[1];
        
        let data = {
            accessToken: accessToken,
            scope: scope,
            state: state,
            tokenType: tokenType
        };
        
        // send via post ajax to /login
        let xhr = new XMLHttpRequest();
        xhr.open('POST', '/oauth', true);
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.send(JSON.stringify(data));
    </script>
</body>
</html>
""";
}