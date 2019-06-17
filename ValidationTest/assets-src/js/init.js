(function () {
    // set js-enabled on body
    document.body.className = ((document.body.className) ? document.body.className + ' js-enabled' : 'js-enabled');

    // back button
    var back = document.querySelector('.govuk-back-link');
    back.addEventListener('click', (event) => {
        history.go(-1);
    });

    // cookie consent
    var consent = document.querySelector('#cookieConsent');
    if (consent) {
        var consentButton = consent.querySelector('button[data-cookie-string]');
        consentButton.addEventListener('click', function (event) {
            document.cookie = consentButton.dataset.cookieString;
            consent.parentNode.removeChild(consent);
        }, false);
    }    
})();