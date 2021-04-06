$(function () {
    init();
});

function init() {
    $('.ui.accordion').accordion();
    $('.checkbox').checkbox();
    $('.dropdown').dropdown();

    $('.client-handle').on('click', function () {
        var id = $(this).attr('data-id');
        loadClientDetails(id);
    });

    $('.api-handle').on('click', function () {
        var id = $(this).attr('data-id');
        loadApiDetails(id);
    });

    $('#client_grant').on('change', function () {
        adjustFormWithGrant($(this).dropdown('get value'));
    });

    $('#scope_api_association').on('change', function () {
        $('#scope_name').val($(this).dropdown('get value') + ".");
        $('#scope_desc').val("");
    });

    $('input').bind('input propertychange', function () {
        var str = $(this).val();
        var patt = new RegExp($(this).attr('pattern'));
        var res = patt.test(str);
        if (!res) {
            $(this).parent().addClass('error');
        }
        else {
            $(this).parent().removeClass('error');
        }
    });
}

function openClientAssignScopesModel() {
    $('#client_assign_scopes_model').modal('show');
}

function openApiAssignScopesModel() {
    $('#api_assign_scopes_model').modal('show');
}

function readImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#client_logo').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}


function resetForm() {
    $('#client_secret').parent().show();
    $('#client_redirecturi').parent().show();
    $('#client_postredirecturi').parent().show();
    $('#client_cors_orgins').parent().show();
    $('#client_inactive_message').parent().show();
    $('#client_name').val("");
    $('#client_desc').val("");
    $('#client_id').val("");
    $('#client_secret').val("");
    $('#client_username').val("");
    $('#client_password').val("");
    $('#client_inactive_message').val("");
    $('#client_identitytoken_life').val("");
    $('#client_accesstoken_life').val("");
    $('#client_redirecturi').val("");
    $('#client_postredirecturi').val("");
    $('#client_cors_orgins').val("");
    $('#clients_allowedscopes').html("");
    $('#clients_unappliedscope_list').html("");
    $('#client_beta').prop('checked', false);
}

function authCodeMode() {
    $('#client_cors_orgins').parent().hide();
}

function clientCredMode() {
    $('#client_redirecturi').parent().hide();
    $('#client_postredirecturi').parent().hide();
    $('#client_cors_orgins').parent().hide();
    $('#client_inactive_message').parent().hide();
}

function implicitMode() {
    $('#client_secret').parent().hide();
}

function resOwnerMode() {
    $('#client_redirecturi').parent().hide();
    $('#client_postredirecturi').parent().hide();
    $('#client_cors_orgins').parent().hide();
    $('#client_inactive_message').parent().hide();
}

function adjustFormWithGrant(grant) {
    switch (grant) {
        case "code":
            $('#client_grant').dropdown('set selected', 'code');
            resetForm();
            authCodeMode();
            break;
        case "client_credentials":
            $('#client_grant').dropdown('set selected', 'client_credentials');
            resetForm();
            clientCredMode();
            break;
        case "implicit":
            $('#client_grant').dropdown('set selected', 'implicit');
            resetForm();
            implicitMode();
            break;
        case "resource_owner":
            $('#client_grant').dropdown('set selected', 'resource_owner');
            resetForm();
            resOwnerMode();
            break;
    }
}

function assignNewScopesToClient() {
    var clientId = $('#selected-client').val();
    var newScopes = $('#clients_unappliedscope_list').dropdown('get value');
    $.get("Administration/AssignNewScopesToClient", { clientId: clientId, newScopes: newScopes.toString() }, function (response) {
        if (clientId != "") {
            location.reload();
        }    
    });
}

function assignNewScopesToApi() {
    var apiId = $('#selected-api').val();
    var newScopes = $('#api_unappliedscope_list').dropdown('get value');
    $.get("Administration/AssignNewScopesToApi", { apiId: apiId, newScopes: newScopes.toString() }, function (response) {
        if (apiId != "") {
            location.reload();
        }    
    });
}

function deleteApiScope(apiId, scopeName) {
    $.get("Administration/DeleteApiScope", { apiId: apiId, scopeName: scopeName }, function (response) {
        location.reload();
    });
}

function deleteClientScope(clientId, scopeName) {
    $.get("Administration/DeleteClientScope", { clientId: clientId, scopeName: scopeName }, function (response) {
        location.reload();
    });
}

function loadScopeDetails(scopeId) {
    $('#selected-scope').val(scopeId);

    $('#infotab').hide();
    $('#client_details').hide();
    $('#api_details').hide();
    $('#scope_details').show();
    $('#api_scope_section').show();
    $('#api_scope_section_msg').hide();
    $('#scope_api_association_section').hide();


    $.get("Administration/GetScope", { scopeId: scopeId }, function (response) {
        console.log(response);
        $('#scope_name').val(response.scopeName);
        $('#scope_desc').val(response.scopeDescription);
    });

}


function loadApiDetails(apiId) {
    $('#selected-api').val(apiId);

    $('#infotab').hide();
    $('#client_details').hide();
    $('#scope_details').hide();
    $('#api_details').show();


    $.get("Administration/GetApi", { apiId: apiId }, function (response) {
        console.log(response);
        $('#api_name').val(response.name);
        $('#api_display_name').val(response.displayName);
        $('#api_desc').val(response.description);
    });
    $.get("Administration/GetApiScopes", { apiId: apiId }, function (response) {
        console.log(response);
        var html = '';
        for (var i = 0; i < response.length; i++) {
            html += `<tr>
                                <td>`+ response[i].scopeName + `</td>
                                <td>`+ response[i].scopeDescription + `</td>
                                <td>
                                    <a href="#" onclick="deleteApiScope('`+ apiId + `', '` + response[i].scopeName + `')">Delete</a>
                                </td>
                            </tr>`;
        }
        $('#api_allowedscopes').html(html);
    });

    $.get("Administration/GetApiNonAssignedScopes", { apiId: apiId }, function (response) {
        console.log(response);
        var html = '';
        for (var i = 0; i < response.length; i++) {
            html += `<option value="` + response[i].scopeName + `">` + response[i].scopeName + `</option>`;
        }
        $('#api_unappliedscope_list').html(html);
    });
}

function loadClientDetails(clientId) {
    $('#selected-client').val(clientId);

    $('#infotab').hide();
    $('#api_details').hide();
    $('#scope_details').hide();
    $('#client_details').show();

    $('#client_scope_section').show();
    $('#client_scope_section_grant_msg').show();
    $('#client_scope_section_msg').hide();

    $('#client_grant').addClass('disabled');

    $.get("Administration/GetClient", { clientId:clientId }, function (response) {
        adjustFormWithGrant(response.allowedGrantTypes);        
        $('#client_name').val(response.clientName);
        $('#client_desc').val(response.clientDescription);
        $('#client_id').val(response.clientId);
        $('#client_secret').val(response.clientSecret);
        $('#client_inactive_message').val(response.maintananceMessage);
        $('#client_identitytoken_life').val(response.identityTokenLifetime);
        $('#client_accesstoken_life').val(response.accessTokenLifetime);
        $('#client_redirecturi').val(response.redirectUris);
        $('#client_postredirecturi').val(response.postLogoutRedirectUris);
        $('#client_cors_orgins').val(response.allowedCorsOrigins);
        $('#client_logo').attr('src', response.logo);
        $('#client_beta').prop('checked', response.isBeta);

        $('#code').removeClass();
        $('#code').addClass('csharp');

        switch (response.allowedGrantTypes) {
            case "code":
                var code = `
//Install Packages
//IdentityModel
//Microsoft.AspNetCore.Authentication.OpenIdConnect
//System.IdentityModel.Tokens.Jwt

//Configure Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

    //Add Support for OAuth 2.0 Code-Grant With Identity Server 4
    services.AddAuthentication(opt => 
    {
        opt.DefaultScheme = "Cookies";
        opt.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", opt => 
    {
        opt.SignInScheme = "Cookies";
        opt.Authority = "`+ window.location.origin +`";
        opt.ClientId = "`+ response.clientId +`";
        opt.ResponseType = "code";
        opt.ClientSecret = "`+ response.clientSecret +`";
        opt.UseTokenLifetime = true;
        opt.SaveTokens = true;
    });
}

//Add Authentication and Authorization In Pipeline
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

//To protect an endpoint and access auth info and to read all claims from Http Context
[Authorize]
public async Task<IActionResult> Privacy()
{
    var result = await HttpContext.AuthenticateAsync();
    var accessToken = await HttpContext.GetTokenAsync("access_token");
    var claims = result.Principal.Claims;
}
`;
                $('#code').text(code);
                hljs.highlightAll();
                break;
            case "client_credentials":
                var code = `
//Install Packages
//IdentityModel
//Microsoft.AspNetCore.Authentication.OpenIdConnect
//System.IdentityModel.Tokens.Jwt

//First get an access token
public async Task<string> GetToken()
{
    var client = new HttpClient();
    var disco = await client.GetDiscoveryDocumentAsync("`+ window.location.origin +`");
    if (!disco.IsError)
    {
        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
        Address = disco.TokenEndpoint,
        ClientId = "`+ response.clientId +`",
        ClientSecret = "`+ response.clientSecret +`",
        Scope = "Api1"
        });
        if (tokenResponse.IsError)
        {
            Console.WriteLine(tokenResponse.Error);
        }
        var token = tokenResponse.AccessToken;
        return token;
    }
}

//Using the access token, Call the server
public async Task CallAPI(string token) 
{
    var client = new RestClient("https://APIURL/WeatherForecast");
    client.Timeout = -1;
    var request = new RestRequest(Method.GET);
    request.AddHeader("Authorization", $"Bearer {token}");
    IRestResponse response = client.Execute(request);
    Console.WriteLine(response.Content);
}

//ON API

public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "`+ window.location.origin +`";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
    services.AddAuthorization(options =>
    {
        options.AddPolicy("ApiScope", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", "Api1");
        });
    });
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers().RequireAuthorization("ApiScope");
    });
}
`;
                $('#code').text(code);
                hljs.highlightAll();
                break;
            case "implicit":
                $('#code').removeClass('csharp');
                $('#code').addClass('html');
                var code = `
<!--Main Page-->

<!DOCTYPE html>
<html>
   <head>
      <meta charset="utf-8" />
      <title></title>
   </head>
   <body>
      <button id="login">Login</button>
      <button id="api">Call API</button>
      <button id="logout">Logout</button>
      <pre id="results"></pre>
      <script 
         src="https://cdnjs.cloudflare.com/ajax/libs/oidc-client/1.11.5/oidc-client.min.js"
         integrity="sha512-pGtU1n/6GJ8fu6bjYVGIOT9Dphaw5IWPwVlqkpvVgqBxFkvdNbytUh0H8AP15NYF777P4D3XEeA/uDWFCpSQ1g=="
         crossorigin="anonymous"></script>
      <script>
         function log() {
             document.getElementById('results').innerText = '';
         
             Array.prototype.forEach.call(arguments, function (msg) {
                 if (msg instanceof Error) {
                     msg = "Error: " + msg.message;
                 }
                 else if (typeof msg !== 'string') {
                     msg = JSON.stringify(msg, null, 2);
                 }
                 document.getElementById('results').innerHTML += msg + '\r\n';
             });
         }
         
         document.getElementById("login").addEventListener("click", login, false);
         document.getElementById("api").addEventListener("click", api, false);
         document.getElementById("logout").addEventListener("click", logout, false);
         
         var config = {
             authority: "`+ window.location.origin +`",
             client_id: "`+ response.clientId + `",
             redirect_uri: "`+ response.redirectUris +`",
             response_type: "id_token token",
             scope: "openid profile Api1",
             post_logout_redirect_uri: "`+ response.postLogoutRedirectUris +`",
         };
         var mgr = new Oidc.UserManager(config);
         
         
         mgr.getUser().then(function (user) {
             if (user) {
                 log("User logged in", user.profile);
             }
             else {
                 log("User not logged in");
             }
         });
         
         function login() {
             mgr.signinRedirect();
         }         
         
         function api() {
             mgr.getUser().then(function (user) {
                 var url = "https://localhost:44365/WeatherForecast";
         
                 var xhr = new XMLHttpRequest();
                 xhr.open("GET", url);
                 xhr.onload = function () {
                     log(xhr.status, JSON.parse(xhr.responseText));
                 }
                 xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
                 xhr.send();
             });
         }
         
         function logout() {
             mgr.signoutRedirect();
         }
         
      </script>
   </body>
</html>

<!--Callback Page-->

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title></title>
</head>
<body>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/oidc-client/1.11.5/oidc-client.min.js"
            integrity="sha512-pGtU1n/6GJ8fu6bjYVGIOT9Dphaw5IWPwVlqkpvVgqBxFkvdNbytUh0H8AP15NYF777P4D3XEeA/uDWFCpSQ1g=="
            crossorigin="anonymous">
    </script>
    <script>
        new Oidc.UserManager().signinRedirectCallback().then(function () {
            window.location = "index.html";
        }).catch(function (e) {
            console.error(e);
        });
    </script>
</body>
</html>

//On the API, Set a CORS Policy
//Configure Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();            
    services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "`+ window.location.origin +`";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
    services.AddAuthorization(options =>
    {
        options.AddPolicy("ApiScope", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", "Api1");
        });
    });
    services.AddCors(options =>
    {
        options.AddPolicy("default", policy =>
        {
            policy.WithOrigins("`+ response.allowedCorsOrigins +`")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
    });
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseCors("default");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers().RequireAuthorization("ApiScope");
    });
}
`;
                $('#code').text(code);
                hljs.highlightAll();
                break;
            case "resource_owner":
                var code = `
// Request token
var tokenClient = new TokenClient(disco.TokenEndpoint, "`+ response.clientId + `", "` + response.clientSecret +`");
var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "Api1");
if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}
Console.WriteLine(tokenResponse.Json);
Console.WriteLine("\n\n");`;
                $('#code').text(code);
                hljs.highlightAll();
                break;
        }

    });

    $.get("Administration/GetClientScopes", { clientId: clientId }, function (response) {
        console.log(response);
        var html = '';
        for (var i = 0; i < response.length; i++) {
            html += `<tr>
                                <td>`+ response[i].scopeName +`</td>
                                <td>`+ response[i].scopeDescription +`</td>
                                <td>
                                    <a href="#" onclick="deleteClientScope('`+ clientId + `', '` + response[i].scopeName +`')">Delete</a>
                                </td>
                            </tr>`;
        }
        $('#clients_allowedscopes').html(html);
    });

    $.get("Administration/GetClientNonAssignedScopes", { clientId: clientId }, function (response) {
        console.log(response);
        var html = '';
        for (var i = 0; i < response.length; i++) {
            html += `<option value="` + response[i].scopeName +`">` + response[i].scopeName + `</option>`;
        }
        $('#clients_unappliedscope_list').html(html);
    });



}


function getScopesCsv(tableid) {
    var cols = $('#' + tableid + ' tr td:nth-child(1)');
    var csv = "";
    for (var i = 0; i < cols.length; i++) {
        var data = $('#' + tableid + ' tr td:nth-child(1):eq(' + i + ')').text();
        if (i < cols.length - 1) {
            csv += data + ',';
        }
        else {
            csv += data
        }
    }
    return csv;
}

function saveClient() {
    var clientId = $('#selected-client').val();
    var data = {
        "id": clientId,
        "clientName": $('#client_name').val(),
        "clientDescription": $('#client_desc').val(),
        "clientId": $('#client_id').val(),
        "clientSecret": $('#client_secret').val(),
        "allowedGrantTypes": $('#client_grant').dropdown('get value'),
        "redirectUris": $('#client_redirecturi').val(),
        "postLogoutRedirectUris": $('#client_postredirecturi').val(),
        "accessTokenLifetime": $('#client_accesstoken_life').val(),
        "identityTokenLifetime": $('#client_identitytoken_life').val(),
        "allowedCorsOrigins": $('#client_cors_orgins').val(),
        "allowedScopes": getScopesCsv('clients_allowedscopes'),
        "isActive": true,
        "isBeta": $('#client_beta').is(':checked'),
        "maintananceMessage": $('#client_inactive_message').val(),
        "logo": $('#client_logo').attr('src')
    };
    $.post('Administration/SaveClient', { client: data }, function (response) {
        location.reload();
    });
}

function activateInactivateClient() {
    var clientId = $('#selected-client').val();
    $.get('Administration/EnableDisableClient', { clientId: clientId }, function (response) {
        location.reload();
    });
}

function deleteClient() {

}

function saveApi() {
    var apiId = $('#selected-api').val();
    var data = {
        "id": apiId,
        "Name": $('#api_name').val(),
        "DisplayName": $('#api_display_name').val(),        
        "Description": $('#api_desc').val(),        
        "IsActive": true,
        "SupportedScopes": getScopesCsv('api_allowedscopes')       
    };
    $.post('Administration/SaveApi', { api: data }, function (response) {
        location.reload();
    });
}

function activateInactivateApi() {
    var apiId = $('#selected-api').val();
    $.get('Administration/EnableDisableApi', { apiId: apiId }, function (response) {
        location.reload();
    });
}

function deleteApi() {

}

function saveScope() {
    var scopeId = $('#selected-scope').val();
    var data = {
        "Id": scopeId,
        "ScopeName": $('#scope_name').val(),
        "ScopeDescription": $('#scope_desc').val()
    };
    $.post('Administration/SaveScope', { scope: data }, function (response) {
        location.reload();
    });
}

function deleteScope() {

}



function newClient() {
    $('#selected-client').val("");
    $('#infotab').hide();
    $('#api_details').hide();
    $('#scope_details').hide();
    $('#client_details').show();
    $('#client_scope_section').hide();
    $('#client_scope_section_msg').show();
    $('#client_scope_section_grant_msg').hide();
    resetForm();
    $('#client_grant').dropdown('set selected', 'code');
    $('#client_grant').removeClass('disabled');
    $.get("Administration/GetClientNonAssignedScopes", { clientId: null }, function (response) {
        var html = '';
        for (var i = 0; i < response.length; i++) {
            html += `<option value="` + response[i].scopeName + `">` + response[i].scopeName + `</option>`;
        }
        $('#clients_unappliedscope_list').html(html);
    });
}

function newApi() {
    $('#selected-api').val("");

    $('#infotab').hide();
    $('#client_details').hide();
    $('#scope_details').hide();
    $('#api_details').show();
    $('#api_scope_section').hide();
    $('#api_scope_section_msg').show();
  

    //Reset API
    $('#api_name').val("");
    $('#api_display_name').val("");
    $('#api_desc').val("");

    $('#api_allowedscopes').html("");

    $.get("Administration/GetApiNonAssignedScopes", { apiId: "" }, function (response) {
        var html = '';
        for (var i = 0; i < response.length; i++) {
            html += `<option value="` + response[i].scopeName + `">` + response[i].scopeName + `</option>`;
        }
        $('#api_unappliedscope_list').html(html);
    });

}

function newScope() {
    $('#selected-scope').val("");

    $('#infotab').hide();
    $('#client_details').hide();
    $('#api_details').hide();
    $('#scope_details').show();
    $('#api_scope_section').show();
    $('#api_scope_section_msg').hide();
    $('#scope_api_association_section').show();  

    //Reset API
    $('#scope_name').val("");
    $('#scope_desc').val("");
}
