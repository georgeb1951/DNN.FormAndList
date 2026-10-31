/*
 * BCC Forms - spam protection client.
 * Renders Google reCAPTCHA v2, runs reCAPTCHA v3, or renders Cloudflare Turnstile for each form
 * module on the page, writes the token into that module's hidden field and keeps the Submit link
 * from posting back until a token exists. Several forms on one page each get their own widget.
 *
 * Forms queue their configuration before or after this file loads:
 *   (window.BCCFormsCaptchaQueue = window.BCCFormsCaptchaQueue || []).push({ ... });
 */
(function (w, d) {
    'use strict';

    if (w.BCCFormsCaptcha) { return; }

    var V3_TOKEN_MAX_AGE = 90 * 1000; // reCAPTCHA v3 tokens expire after two minutes
    var loaders = {};

    function loadScript(url, globalCallbackName, done) {
        var entry = loaders[url];
        if (entry) {
            if (entry.loaded) { done(); } else { entry.waiting.push(done); }
            return;
        }
        entry = loaders[url] = { loaded: false, waiting: [done] };
        w[globalCallbackName] = function () {
            entry.loaded = true;
            var list = entry.waiting.splice(0, entry.waiting.length);
            for (var i = 0; i < list.length; i++) { list[i](); }
        };
        var s = d.createElement('script');
        s.src = url + (url.indexOf('?') < 0 ? '?' : '&') + 'onload=' + globalCallbackName;
        s.async = true;
        s.defer = true;
        s.onerror = function () { entry.failed = true; };
        d.head.appendChild(s);
    }

    function init(cfg) {
        var container = d.getElementById(cfg.container);
        var tokenField = d.getElementById(cfg.tokenField);
        var button = d.getElementById(cfg.button);
        if (!container || !tokenField || !button) { return; }

        var state = { ready: false, tokenTime: 0, passing: false, busy: false };
        var message = d.createElement('div');
        message.className = 'dnnFormMessage dnnFormWarning bccf-captcha-msg';
        message.setAttribute('role', 'alert');
        message.style.display = 'none';
        container.parentNode.insertBefore(message, container.nextSibling);

        function say(text) {
            message.textContent = text || '';
            message.style.display = text ? '' : 'none';
        }
        function setToken(token) {
            tokenField.value = token || '';
            state.tokenTime = token ? new Date().getTime() : 0;
            if (token) { say(''); }
        }
        function clearToken() { setToken(''); }
        function resubmit() {
            state.passing = true;
            button.click(); // runs the LinkButton's javascript: postback
        }

        var lang = cfg.language || '';
        if (cfg.mode === 'ReCaptchaV2') {
            loadScript('https://www.google.com/recaptcha/api.js?render=explicit' +
                       (lang ? '&hl=' + encodeURIComponent(lang) : ''), 'BCCFormsReCaptchaV2Loaded', function () {
                w.grecaptcha.render(container, {
                    sitekey: cfg.siteKey,
                    callback: setToken,
                    'expired-callback': clearToken,
                    'error-callback': clearToken
                });
                state.ready = true;
            });
        } else if (cfg.mode === 'ReCaptchaV3') {
            loadScript('https://www.google.com/recaptcha/api.js?render=' + encodeURIComponent(cfg.siteKey),
                       'BCCFormsReCaptchaV3Loaded', function () {
                w.grecaptcha.ready(function () { state.ready = true; });
            });
        } else if (cfg.mode === 'Turnstile') {
            loadScript('https://challenges.cloudflare.com/turnstile/v0/api.js?render=explicit',
                       'BCCFormsTurnstileLoaded', function () {
                w.turnstile.render(container, {
                    sitekey: cfg.siteKey,
                    action: cfg.action,
                    language: lang || 'auto',
                    callback: setToken,
                    'expired-callback': clearToken,
                    'timeout-callback': clearToken,
                    'error-callback': clearToken
                });
                state.ready = true;
            });
        } else {
            return;
        }

        // Capture phase so this runs before the postback in the link's href.
        button.addEventListener('click', function (e) {
            if (state.passing) { state.passing = false; return; }

            if (cfg.mode === 'ReCaptchaV3') {
                if (tokenField.value && new Date().getTime() - state.tokenTime < V3_TOKEN_MAX_AGE) { return; }
                e.preventDefault();
                e.stopImmediatePropagation();
                if (!state.ready) { say(cfg.text.notReady); return; }
                if (state.busy) { return; }
                state.busy = true;
                w.grecaptcha.execute(cfg.siteKey, { action: cfg.action }).then(function (token) {
                    state.busy = false;
                    setToken(token);
                    resubmit();
                }, function () {
                    state.busy = false;
                    say(cfg.text.failed);
                });
                return;
            }

            if (!tokenField.value) {
                e.preventDefault();
                e.stopImmediatePropagation();
                say(state.ready ? cfg.text.prompt : cfg.text.notReady);
            }
        }, true);
    }

    var queue = w.BCCFormsCaptchaQueue || [];
    w.BCCFormsCaptcha = { init: init };
    w.BCCFormsCaptchaQueue = { push: function (cfg) { init(cfg); } };

    function flush() {
        for (var i = 0; i < queue.length; i++) { init(queue[i]); }
        queue = [];
    }
    if (d.readyState === 'loading') {
        d.addEventListener('DOMContentLoaded', flush);
    } else {
        flush();
    }
}(window, document));
