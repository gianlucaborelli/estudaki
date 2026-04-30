// paste-handler.js - Handle clipboard paste events for image upload

let pasteHandler = null;
let fileInputElement = null;

export function initialize(pasteAreaId, fileInputId) {
    const pasteArea = document.getElementById(pasteAreaId);
    fileInputElement = document.getElementById(fileInputId);

    if (!pasteArea) {
        console.error('PasteHandler: pasteArea not found');
        return;
    }

    if (!fileInputElement) {
        console.error('PasteHandler: fileInput not found with id:', fileInputId);
        return;
    }

    if (pasteHandler) {
        pasteArea.removeEventListener('paste', pasteHandler);
    }

    console.log('PasteHandler: Initialized successfully');

    pasteHandler = async (e) => {
        e.preventDefault();

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
            if (!fileInputElement) {
                console.error('PasteHandler: File input element not available');
                return;
            }

            const dataTransfer = new DataTransfer();

            // Adicionar APENAS as novas imagens coladas (não manter as existentes)
            imageFiles.forEach(file => {
                dataTransfer.items.add(file);
            });

            // Atualizar files do input
            fileInputElement.files = dataTransfer.files;

            // Disparar evento change com bubbles para Blazor detectar
            const changeEvent = new Event('change', { bubbles: true, cancelable: true });
            fileInputElement.dispatchEvent(changeEvent);

            console.log(`PasteHandler: ${imageFiles.length} imagem(ns) colada(s) com sucesso`);
        }
    };

    // Adicionar event listener apenas no pasteArea para evitar duplicação
    pasteArea.addEventListener('paste', pasteHandler);
    console.log('PasteHandler: Event listener added to pasteArea');
}

export function dispose() {
    if (pasteHandler) {
        const pasteArea = document.getElementById('pasteArea');
        if (pasteArea) {
            pasteArea.removeEventListener('paste', pasteHandler);
        }
        pasteHandler = null;
    }
    fileInputElement = null;
}
