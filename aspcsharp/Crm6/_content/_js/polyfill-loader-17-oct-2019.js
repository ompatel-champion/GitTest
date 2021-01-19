if (!window.Promise) {
	loadScript('/_content/_js/promise-min.js');
}

function loadScript(src) {
    var js = document.createElement('script');
    js.src = src;
    js.onerror = function () {
        throw new Error('Error Loading Script:' + src);
    };
    document.head.appendChild(js);
}