// paste-handler.js - Handle clipboard paste events for image upload

let pasteHandler = null;

export function initialize(pasteAreaId, fileInputId) {
    const pasteArea = document.getElementById(pasteAreaId);
    const fileInput = document.getElementById(fileInputId);

    if (!pasteArea || !fileInput) {
        return;
    }

    if (pasteHandler) {
        document.removeEventListener('paste', pasteHandler);
        pasteArea.removeEventListener('paste', pasteHandler);
    }

    pasteHandler = async (e) => {
        const target = e.target;
        const isEditableElement = target.tagName === 'INPUT' || 
                                   target.tagName === 'TEXTAREA' || 
                                   target.isContentEditable ||
                                   target.closest('input, textarea, [contenteditable="true"]');

        if (target !== pasteArea && !pasteArea.contains(target) && isEditableElement) {
            return; 
        }

        const items = e.clipboardData?.items;
        if (!items) return;

        let hasImages = false;
        for (let i = 0; i < items.length; i++) {
            if (items[i].type.indexOf('image') !== -1) {
                hasImages = true;
                break;
            }
        }

        if (!hasImages) {
            return;
        }

        e.preventDefault();

        const imageFiles = [];
        let pastedCount = 0;

        for (let i = 0; i < items.length; i++) {
            const item = items[i];

            if (item.type.indexOf('image') !== -1) {
                const blob = item.getAsFile();
                if (blob) {
                    const timestamp = new Date().getTime();
                    const extension = blob.type.split('/')[1] || 'png';
                    const fileName = `pasted-image-${timestamp}-${pastedCount}.${extension}`;

                    const file = new File([blob], fileName, { type: blob.type });
                    imageFiles.push(file);
                    pastedCount++;
                }
            }
        }

        if (imageFiles.length > 0) {
            const dataTransfer = new DataTransfer();

            if (fileInput.files) {
                for (let i = 0; i < fileInput.files.length; i++) {
                    dataTransfer.items.add(fileInput.files[i]);
                }
            }

            imageFiles.forEach(file => {
                dataTransfer.items.add(file);
            });

            fileInput.files = dataTransfer.files;

            const event = new Event('change', { bubbles: true });
            fileInput.dispatchEvent(event);
        }
    };

    pasteArea.addEventListener('paste', pasteHandler);
    document.addEventListener('paste', pasteHandler);    
}

export function dispose() {
    if (pasteHandler) {
        const pasteArea = document.getElementById('pasteArea');
        if (pasteArea) {
            pasteArea.removeEventListener('paste', pasteHandler);
        }
        document.removeEventListener('paste', pasteHandler);
        pasteHandler = null;
    }
}
