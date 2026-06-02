window.powerleAnalytics = (function () {
    var measurementId = null;
    var initialized = false;
    var sessionStart = Date.now();

    function getContext() {
        return {
            page_path: window.location.pathname + window.location.search,
            page_title: document.title,
            referrer: document.referrer || null,
            screen_width: window.screen.width,
            screen_height: window.screen.height,
            viewport_width: window.innerWidth,
            viewport_height: window.innerHeight,
            user_agent: navigator.userAgent,
            language: navigator.language,
            timestamp: new Date().toISOString()
        };
    }

    function initGtag(id) {
        if (!id) {
            return;
        }

        measurementId = id;
        window.dataLayer = window.dataLayer || [];
        window.gtag = window.gtag || function () { window.dataLayer.push(arguments); };

        var script = document.createElement('script');
        script.async = true;
        script.src = 'https://www.googletagmanager.com/gtag/js?id=' + encodeURIComponent(id);
        script.onload = function () {
            window.gtag('js', new Date());
            window.gtag('config', measurementId, {
                send_page_view: false,
                anonymize_ip: true
            });
        };
        document.head.appendChild(script);
    }

    function buildEventData(payload) {
        var context = getContext();
        if (payload && typeof payload === 'object') {
            return Object.assign({}, context, payload);
        }
        return context;
    }

    function sendEvent(name, payload) {
        var eventData = buildEventData(payload);

        if (measurementId && typeof window.gtag === 'function') {
            window.gtag('event', name, eventData);
            return;
        }

        sendToServer(name, eventData);
    }

    function sendToServer(name, data) {
        var body = JSON.stringify({ event: name, data: data });
        if (navigator.sendBeacon && typeof navigator.sendBeacon === 'function') {
            var blob = new Blob([body], { type: 'application/json' });
            navigator.sendBeacon('/analytics/collect', blob);
            return;
        }

        if (typeof fetch === 'function') {
            fetch('/analytics/collect', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: body,
                keepalive: true
            }).catch(function () {
                // Non-blocking fallback; ignore failures.
            });
        }
    }

    function trackPageViewInternal(pageName, pagePath) {
        sendEvent('page_view', {
            page_name: pageName || document.title,
            page_path: pagePath || window.location.pathname,
            engagement_ms: Date.now() - sessionStart
        });
    }

    function renderAdSense() {
        try {
            (window.adsbygoogle = window.adsbygoogle || []).push({});
        } catch (e) {
            // Non-blocking: ignore ad rendering failures.
        }
    }

    function trackEventInternal(eventName, data) {
        sendEvent(eventName || 'custom_event', data);
    }

    function reportSessionEnd() {
        sendEvent('session_end', {
            engaged_ms: Date.now() - sessionStart
        });
    }

    document.addEventListener('visibilitychange', function () {
        if (document.visibilityState === 'hidden') {
            reportSessionEnd();
        }
    });

    window.addEventListener('beforeunload', function () {
        reportSessionEnd();
    });

    return {
        initialize: function (id) {
            initialized = true;
            initGtag(id);
        },
        trackPageView: function (pageName, pagePath) {
            if (!initialized) {
                initGtag(null);
                initialized = true;
            }
            trackPageViewInternal(pageName, pagePath);
        },
        trackEvent: function (eventName, data) {
            if (!initialized) {
                initGtag(null);
                initialized = true;
            }
            trackEventInternal(eventName, data);
        },
        renderAdSense: function () {
            renderAdSense();
        }
    };
})();
