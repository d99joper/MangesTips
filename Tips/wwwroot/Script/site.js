(function () {
    var spinner = document.getElementById('page-spinner');
    if (!spinner) return; // Safety check to prevent errors

    document.addEventListener('click', function (e) {
        var a = e.target.closest('a');
        if (!a) return;
        var href = a.getAttribute('href');
        if (!href || href === '#' || href.startsWith('javascript:') || href.startsWith('mailto:')) return;
        if (a.target === '_blank') return;
        if (href.startsWith('http') && href.indexOf(window.location.hostname) === -1) return;
        spinner.classList.add('visible');
    });

    window.addEventListener('pageshow', function (e) {
        if (e.persisted) {
            spinner.classList.remove('visible');
        }
    });
})();

window.addEventListener('pageshow', function (e) {
    document.getElementById('nav-spinner').style.display = 'none';
});