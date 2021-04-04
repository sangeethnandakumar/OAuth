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

    $('#client_grant').on('change', function () {
        adjustFormWithGrant($(this).dropdown('get value'));
    });
}

function openClientAssignScopesModel() {
    $('#client_assign_scopes_model').modal('show');
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
    $('#client_identitytoken_life').val("");
    $('#client_accesstoken_life').val("");
    $('#clients_allowedscopes').html("");
    $('#clients_unappliedscope_list').html("");
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
        location.reload();
    });
}

function deleteClientScope(clientId, scopeName) {
    $.get("Administration/DeleteClientScope", { clientId: clientId, scopeName: scopeName }, function (response) {
        location.reload();
    });
}

function loadClientDetails(clientId) {
    $('#selected-client').val(clientId);
    $('#infotab').hide();
    $('#client_details').show();
    $('#client_grant').addClass('disabled');
    $.get("Administration/GetClient", { clientId:clientId }, function (response) {
        console.log(response);
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