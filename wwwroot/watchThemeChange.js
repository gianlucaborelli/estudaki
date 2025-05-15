function watchThemeChange() {
    const observer = new MutationObserver(() => {
        document.body.style.colorScheme =
            document.body.classList.contains('mud-dark-theme') ? 'dark' : 'light';
    });

    observer.observe(document.body, {
        attributes: true,
        attributeFilter: ['class']
    });
}