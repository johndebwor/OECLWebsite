window.registerScrollHandler = function (dotnetRef) {
    window.addEventListener('scroll', function () {
        dotnetRef.invokeMethodAsync('OnScroll', window.scrollY);
    }, { passive: true });
};
