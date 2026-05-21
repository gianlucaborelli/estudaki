window.textSelectionTracker = {
    register: function (editorId, dotnetRef) {

        const container = document.getElementById(editorId);

        if (!container)
            return;

        let element =
            container.querySelector('textarea') ||
            container.querySelector('input') ||
            container.querySelector('.mud-input-slot textarea') ||
            container.querySelector('.mud-input-slot input');

        if (!element)
            return;

        const notifySelection = () => {

            const start = element.selectionStart || 0;
            const end = element.selectionEnd || 0;

            dotnetRef.invokeMethodAsync(
                'OnSelectionChanged',
                {
                    text: element.value.substring(start, end),
                    start: start,
                    end: end,
                    cursorPosition: start,
                    hasSelection: start !== end
                });
        };

        element.addEventListener('select', notifySelection);
        element.addEventListener('keyup', notifySelection);
        element.addEventListener('mouseup', notifySelection);
        element.addEventListener('input', notifySelection);

        // salvar para remover depois se quiser
        element._selectionHandler = notifySelection;
    },

    unregister: function (editorId) {

        const container = document.getElementById(editorId);

        if (!container)
            return;

        let element =
            container.querySelector('textarea') ||
            container.querySelector('input') ||
            container.querySelector('.mud-input-slot textarea') ||
            container.querySelector('.mud-input-slot input');

        if (!element || !element._selectionHandler)
            return;

        element.removeEventListener('select', element._selectionHandler);
        element.removeEventListener('keyup', element._selectionHandler);
        element.removeEventListener('mouseup', element._selectionHandler);
        element.removeEventListener('input', element._selectionHandler);
    }
};