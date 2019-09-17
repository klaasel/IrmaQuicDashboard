
function doVerificationSession() {
  const attr = document.getElementById('attr').value;
  const label = document.getElementById('label').value;
  const message = document.getElementById('message').value;
  const request = !message ? {
    'type': 'disclosing',
    'content': [{
      'label': label, 'attributes': [ attr ]
    }]
  } : {
    'type': 'signing',
    'message': message,
    'content': [{
      'label': label, 'attributes': [ attr ]
    }]
  };
  doSession(request).then(function(result) { showSuccess('Success, attribute value: <strong>' + result.disclosed[0].rawvalue + '</strong>'); });
}
function doIssuanceSession() {
  const attrs = document.getElementById('newattrs').value.split(',');
  doSession({
    'type': 'issuing',
    'credentials': [{
      'credential': 'irma-demo.MijnOverheid.ageLower',
      'attributes': { 'over12': attrs[0], 'over16': attrs[1], 'over18': attrs[2], 'over21': attrs[3] }
    }]
  }).then(function(result) { showSuccess('Success'); });
}
function doSession(request) {
  clearOutput();
  showSuccess('Demo running...');
  const server = document.getElementById('server').value;
  const authmethod = document.getElementById('method').value;
  const key = document.getElementById(authmethod === 'publickey' ? 'key-pem' : 'key').value;
  const requestorname = document.getElementById('requestor').value;
  return irma.startSession(server, request, authmethod, key, requestorname)
    .then(function(pkg) { return irma.handleSession(pkg.sessionPtr, {server: server, token: pkg.token, method: 'popup', language: 'en'}); })
    .then(function(result) {
      console.log('Done', result);
      return result;
    })
    .catch(function(err) { showError(err); });
}
window.onload = function() {
  let u = "https://irmaquic.australiasoutheast.cloudapp.azure.com:8088";
  if (u.endsWith('/'))
    u = u.substring(0, u.length - 1);
  document.getElementById('server').value = u;
  document.getElementById('issuance').addEventListener('click', doIssuanceSession);
  document.getElementById('verification').addEventListener('click', doVerificationSession);
  document.getElementById('method').addEventListener('change', methodChanged);
  methodChanged();
};
// UI handling functions
function clearOutput() {
  const e = document.getElementById('result');
  e.setAttribute('hidden', 'true');
  e.classList.remove('succes', 'warning', 'error');
}
function showError(err) {
  const e = document.getElementById('result');
  e.removeAttribute('hidden');
  e.classList.remove('success');
  if (err === irma.SessionStatus.Cancelled) {
    e.classList.add('warning');
    e.innerText = 'Session was aborted';
  } else {
    e.classList.add('error');
    e.innerText = 'Error occurred: ' + String(err);
  }
  throw err;
}
function showSuccess(text) {
  const e = document.getElementById('result');
  e.innerHTML = text;
  e.removeAttribute('hidden');
  e.classList.add('success');
}
function methodChanged() {
  switch (document.getElementById('method').value) {
    case 'none':
      hideElements('key-container', 'key-pem-container', 'requestor-container');
      break;
    case 'hmac':
      hideElements('key-pem-container');
      showElements('requestor-container', 'key-container');
      break;
    case 'token':
      hideElements('key-pem-container', 'requestor-container');
      showElements('key-container');
      break;
    case 'publickey':
      hideElements('key-container');
      showElements('key-pem-container', 'requestor-container');
      break;
  }
}
function hideElements() {
  for (let i=0; i < arguments.length; i++) document.getElementById(arguments[i]).setAttribute('hidden', 'true');
}
function showElements() {
  for (let i=0; i < arguments.length; i++) document.getElementById(arguments[i]).removeAttribute('hidden');
}

