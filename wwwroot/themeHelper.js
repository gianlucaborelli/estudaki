window.getPreferredColorScheme = () => {
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
}