!function(e,n){for(var t in n)e[t]=n[t]}(exports,function(e){var n={};function t(o){if(n[o])return n[o].exports;var r=n[o]={i:o,l:!1,exports:{}};return e[o].call(r.exports,r,r.exports,t),r.l=!0,r.exports}return t.m=e,t.c=n,t.d=function(e,n,o){t.o(e,n)||Object.defineProperty(e,n,{enumerable:!0,get:o})},t.r=function(e){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},t.t=function(e,n){if(1&n&&(e=t(e)),8&n)return e;if(4&n&&"object"==typeof e&&e&&e.__esModule)return e;var o=Object.create(null);if(t.r(o),Object.defineProperty(o,"default",{enumerable:!0,value:e}),2&n&&"string"!=typeof e)for(var r in e)t.d(o,r,function(n){return e[n]}.bind(null,r));return o},t.n=function(e){var n=e&&e.__esModule?function(){return e.default}:function(){return e};return t.d(n,"a",n),n},t.o=function(e,n){return Object.prototype.hasOwnProperty.call(e,n)},t.p="",t.oe=function(e){process.nextTick(function(){throw e})},t(t.s=9)}([function(e,n){e.exports=require("isomorphic-fetch")},function(e,n){e.exports=require("qrcode")},function(e,n){},function(e,n){},function(e,n){e.exports=require("es6-promise")},function(e,n){e.exports=require("es6-object-assign")},function(e,n){e.exports=require("qrcode-terminal")},function(e,n){e.exports=require("eventsource")},function(e,n){},function(e,n,t){"use strict";t.r(n);var o=t(0),r=t.n(o),i=t(1),s=t.n(i),u=(t(8),t(2)),a=t.n(u),c=t(3),d=t.n(c),l={en:{Common:{WaitData:"Waiting for data...",Cancel:"Cancel"},Messages:{FollowInstructions:"Please follow the instructions in your IRMA app"},Sign:{Title:"signing",Body:"A website requests that you sign a message using some IRMA attributes. Please scan the QR code with your IRMA app."},Verify:{Title:"showing attribute(s)",Body:"A website requests that you disclose some IRMA attributes. Please scan the QR code with your IRMA app."},Issue:{Title:"issuing attribute(s)",Body:"A website wants to issue some IRMA attributes to you. Please scan the QR code with your IRMA app."}},nl:{Common:{WaitData:"Wachten op data...",Cancel:"Annuleren"},Messages:{FollowInstructions:"Volg de instructies in uw IRMA app."},Sign:{Title:"ondertekenen",Body:"Een website vraagt u een bericht te ondertekenen met enkele IRMA attributen. Scan de QR code met uw IRMA app."},Verify:{Title:"attributen tonen",Body:"Een website vraagt u enkele IRMA attributen te tonen. Scan de QR code met uw IRMA app."},Issue:{Title:"attributen uitgeven",Body:"Een website wil u enkele IRMA attributen geven. Scan de QR code met uw IRMA app."}}};function f(e){return(f="function"==typeof Symbol&&"symbol"==typeof Symbol.iterator?function(e){return typeof e}:function(e){return e&&"function"==typeof Symbol&&e.constructor===Symbol&&e!==Symbol.prototype?"symbol":typeof e})(e)}t.d(n,"SessionStatus",function(){return g}),t.d(n,"handleSession",function(){return v}),t.d(n,"startSession",function(){return b}),t.d(n,"signSessionRequest",function(){return y}),t.d(n,"waitConnected",function(){return S}),t.d(n,"waitDone",function(){return I}),t(4).polyfill(),t(5).polyfill();var p="undefined"!=typeof window,m=p?void 0:t(6),w=p?void 0:t(7),g={Initialized:"INITIALIZED",Connected:"CONNECTED",Cancelled:"CANCELLED",Done:"DONE",Timeout:"TIMEOUT"},h={method:"popup",element:"irmaqr",language:"en",showConnectedIcon:!0,returnStatus:g.Done,server:"",token:"",resultJwt:!1,disableMobile:!1};function v(e){var n=arguments.length>1&&void 0!==arguments[1]?arguments[1]:{},t={qr:e,done:!1};return Promise.resolve().then(function(){switch(O("Session started",t.qr),t.options=function(e){O("Options:",e);var n=Object.assign({},h,e);n.userAgent=p?window.MSInputMethodContext&&document.documentMode?(O("Detected IE11"),q.Desktop):/Android/i.test(window.navigator.userAgent)?(O("Detected Android"),q.Android):/iPad|iPhone|iPod/.test(navigator.userAgent)&&!window.MSStream?(O("Detected iOS"),q.iOS):(O("Neither Android nor iOS, assuming desktop"),q.Desktop):null,p&&!n.disableMobile&&n.userAgent!==q.Desktop&&("mobile"!==n.method&&O("On mobile; using method mobile instead of "+n.method),n.method="mobile");switch(n.method){case"url":break;case"mobile":if(n.returnStatus!==g.Done)throw new Error("On mobile sessions, returnStatus must be Done");break;case"popup":if(!p)throw new Error("Cannot use method popup in node");if(!(n.language in l))throw new Error("Unsupported language, currently supported: "+Object.keys(l).join(", "));n.element="modal-irmaqr",n.returnStatus=g.Done;break;case"canvas":if(!p)throw new Error("Cannot use method canvas in node");if("string"!=typeof n.element||""===n.element)throw new Error("canvas method requires `element` to be provided in options");break;case"console":if(p)throw new Error("Cannot use console method in browser");break;default:throw new Error("Unsupported method")}if("string"!=typeof n.server)throw new Error("server must be a string (URL)");if(n.server.length>0&&n.returnStatus!==g.Done)throw new Error("If server option is used, returnStatus option must be SessionStatus.Done");if(n.server.length>0&&("string"!=typeof n.token||0===n.token.length))throw new Error("if server option is used, providing token option is required");if(n.resultJwt&&0===n.server.length)throw new Error("resultJwt option was enabled but no server to retrieve result from was provided");return n}(n),t.method=t.options.method,t.method){case"url":return t.done=!0,s.a.toDataURL(JSON.stringify(t.qr));case"mobile":!function(e,n){var t="qr/json/"+encodeURIComponent(JSON.stringify(e));if(n===q.Android){var o="intent://"+t+"#Intent;package=org.irmacard.cardemu;scheme=cardemu;l.timestamp="+Date.now()+";S.browser_fallback_url=https%3A%2F%2Fplay.google.com%2Fstore%2Fapps%2Fdetails%3Fid%3Dorg.irmacard.cardemu;end";O("Navigating:",o),window.location.href=o}else n===q.iOS&&(O("Navigating:","irma://"+t),window.location.href="irma://"+t)}(e,t.options.userAgent);break;case"popup":!function(e,n){(function(){if(!p||window.document.getElementById("irma-modal"))return;var e=window.document.createElement("div");e.id="irma-modal",e.innerHTML=d.a,window.document.body.appendChild(e);var n=window.document.createElement("div");n.classList.add("irma-overlay"),window.document.body.appendChild(n),e.offsetHeight})(),t=e.irmaqr,o=n,D("irma-cancel-button","Common.Cancel",o),D("irma-title",R[t]+".Title",o),D("irma-text",R[t]+".Body",o),window.document.getElementById("irma-modal").classList.add("irma-show");var t,o;var i=window.document.getElementById("irma-cancel-button");i.addEventListener("click",function n(){r()(e.u,{method:"DELETE"}),i.removeEventListener("click",n)})}(e,t.options.language);case"canvas":if(t.canvas=window.document.getElementById(t.options.element),!t.canvas)return Promise.reject("Specified canvas not found in DOM");!function(e,n){s.a.toCanvas(e,JSON.stringify(n),{width:"230",margin:"1"},function(e){if(e)throw e})}(t.canvas,t.qr);break;case"console":m.generate(JSON.stringify(t.qr))}return t.options.returnStatus===g.Initialized?(t.done=!0,g.Initialized):S(t.qr.u)}).then(function(e){if(t.done)return e;switch(O("Session state changed",e,t.qr.u),t.method){case"popup":D("irma-text","Messages.FollowInstructions",t.options.language);case"canvas":!function(e,n){var t=e.getContext("2d");if(t.clearRect(0,0,e.width,e.height),n){var o=window.devicePixelRatio;e.width=230*o,e.height=230*o,t.scale(o,o);var r=new Image;r.onload=function(){return t.drawImage(r,75.5,40,79,150)},r.src=a.a}}(t.canvas,t.options.showConnectedIcon)}return t.options.returnStatus===g.Connected?(t.done=!0,g.Connected):I(t.qr.u)}).then(function(e){return t.done?e:("popup"===t.method&&M(),0===t.options.server.length?(t.done=!0,e):C("".concat(t.options.server,"/session/").concat(t.options.token,"/").concat(t.options.resultJwt?"result-jwt":"result")))}).then(function(e){return t.done?e:t.options.resultJwt?e.text():e.json()}).catch(function(e){throw O("Error or unexpected status",e),"popup"===t.method&&M(),e})}function b(e,n,t,o,r){return Promise.resolve().then(function(){return"object"===f(n)?"publickey"==t||"hmac"==t?y(n,t,o,r):JSON.stringify(n):n}).then(function(n){var r={};switch(t){case void 0:case"none":r["Content-Type"]="application/json";break;case"token":r.Authorization=o,r["Content-Type"]="application/json";break;case"publickey":case"hmac":r["Content-Type"]="text/plain";break;default:throw new Error("Unsupported authentication method")}return C("".concat(e,"/session"),{method:"POST",headers:r,body:n})}).then(function(e){return e.json()})}function y(e,n,o,r){return Promise.resolve().then(t.t.bind(null,10,7)).then(function(t){var i,s;if(e.type?(i=e.type,s={request:e}):e.request&&(i=e.request.type,s=e),"disclosing"!==i&&"issuing"!==i&&"signing"!==i)throw new Error("Not an IRMA session request");if("publickey"!==n&&"hmac"!==n)throw new Error("Unsupported signing method");var u={algorithm:"publickey"===n?"RS256":"HS256",issuer:r,subject:{disclosing:"verification_request",issuing:"issue_request",signing:"signature_request"}[i]};return t.sign(function(e,n,t){return n in e?Object.defineProperty(e,n,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[n]=t,e}({},{disclosing:"sprequest",issuing:"iprequest",signing:"absrequest"}[i],s),o,u)})}function S(e){return k(e,g.Initialized).then(function(e){return e!==g.Connected?Promise.reject(e):e})}function I(e){return k(e,g.Connected).then(function(e){return e!==g.Done?Promise.reject(e):e})}function k(e){var n=arguments.length>1&&void 0!==arguments[1]?arguments[1]:g.Initialized;return new Promise(function(t,o){var r=p?window.EventSource:w;if(!r)return O("No support for EventSource, fallback to polling"),void E("".concat(e,"/status"),n,t,o);var i=new r("".concat(e,"/statusevents")),s=setTimeout(function(){return o("no open message received")},500);i.onopen=function(){clearTimeout(s)},i.onmessage=function(e){clearTimeout(s),i.close(),t(JSON.parse(e.data))},i.onerror=function(e){clearTimeout(s),O("Received server event error",e),i.close(),o(e)}}).catch(function(t){return O("error in server sent event, falling back to polling:",t),function(e){var n=arguments.length>1&&void 0!==arguments[1]?arguments[1]:g.Initialized;return new Promise(function(t,o){return E(e,n,t,o)})}("".concat(e,"/status"),n)})}var E=function e(n,t,o,r){return C(n).then(function(e){return e.json()}).then(function(i){return i!==t?o(i):setTimeout(e,500,n,t,o,r)}).catch(function(e){return r(e)})},q={Desktop:"Desktop",Android:"Android",iOS:"iOS"};function A(e){return e.ok?e:e.text().then(function(n){throw function(){console.warn.apply(console,arguments)}("Server returned error:",n),new Error(e.statusText)})}function C(){return r.a.apply(null,arguments).then(A)}function M(){p&&window.document.getElementById("irma-modal")&&window.document.getElementById("irma-modal").classList.remove("irma-show")}function O(){console.log.apply(console,arguments)}var R={disclosing:"Verify",issuing:"Issue",signing:"Sign"};function D(e,n,t){window.document.getElementById(e).innerText=function(e,n){var t=e.split("."),o=l[n];for(var r in t){if(void 0===o)break;o=o[t[r]]}if(void 0===o)for(r in o=l[h.language],t){if(void 0===o)break;o=o[t[r]]}return void 0===o?"":o}(n,t)}},function(e,n){e.exports=require("jsonwebtoken")}]));