// Auto Math Renderer - Observa o DOM e renderiza automaticamente tags math e chemical
(function() {
    'use strict';

    console.log('[AutoMathRenderer] Inicializando...');

    // Set para rastrear elementos já processados
    const processedElements = new Set();

    // Função para renderizar um elemento math
    function renderMathElement(element) {
        if (processedElements.has(element.id)) {
            return; // Já foi processado
        }

        const latex = element.getAttribute('data-latex') || '';

        try {
            if (typeof window.MathQuillHelper !== 'undefined') {
                window.MathQuillHelper.renderStatic(element.id, latex);
                processedElements.add(element.id);
                console.log(`[AutoMathRenderer] ✓ Renderizado: ${element.id}`);
            } else {
                console.warn('[AutoMathRenderer] MathQuillHelper não disponível');
            }
        } catch (error) {
            console.error(`[AutoMathRenderer] Erro ao renderizar ${element.id}:`, error);
        }
    }

    // Função para renderizar um elemento chemical
    function renderChemicalElement(element) {
        if (processedElements.has(element.id)) {
            return; // Já foi processado
        }

        const formula = element.getAttribute('data-formula') || '';

        try {
            if (typeof window.ChemicalHelper !== 'undefined') {
                window.ChemicalHelper.renderStatic(element.id, formula);
                processedElements.add(element.id);
                console.log(`[AutoMathRenderer] ✓ Renderizado: ${element.id}`);
            } else {
                console.warn('[AutoMathRenderer] ChemicalHelper não disponível');
            }
        } catch (error) {
            console.error(`[AutoMathRenderer] Erro ao renderizar ${element.id}:`, error);
        }
    }

    // Processa todos os elementos math e chemical visíveis
    function processAllElements() {
        // Processa elementos math
        const mathElements = document.querySelectorAll('.math-inline-display[id^="math-inline-"]');
        mathElements.forEach(element => {
            if (element.getAttribute('data-latex')) {
                renderMathElement(element);
            }
        });

        // Processa elementos chemical
        const chemicalElements = document.querySelectorAll('.chemical-inline-display[id^="chemical-inline-"]');
        chemicalElements.forEach(element => {
            if (element.getAttribute('data-formula')) {
                renderChemicalElement(element);
            }
        });
    }

    // MutationObserver para detectar novos elementos
    const observer = new MutationObserver((mutations) => {
        let shouldProcess = false;

        for (const mutation of mutations) {
            if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                for (const node of mutation.addedNodes) {
                    if (node.nodeType === Node.ELEMENT_NODE) {
                        // Verifica se é um elemento math ou chemical, ou contém um
                        if (node.classList?.contains('math-inline-display') || 
                            node.classList?.contains('chemical-inline-display') ||
                            node.querySelector?.('.math-inline-display, .chemical-inline-display')) {
                            shouldProcess = true;
                            break;
                        }
                    }
                }
            }
        }

        if (shouldProcess) {
            // Pequeno delay para garantir que o DOM está completamente atualizado
            setTimeout(processAllElements, 50);
        }
    });

    // Inicia observação de todo o body
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });

    // Processa elementos existentes imediatamente
    processAllElements();

    // Processa novamente após DOMContentLoaded
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', processAllElements);
    }

    // Processa novamente após eventos Blazor
    document.addEventListener('enhancedload', processAllElements);

    // Expõe função global para processamento manual
    window.AutoMathRenderer = {
        processAll: processAllElements,
        clearCache: () => {
            processedElements.clear();
            console.log('[AutoMathRenderer] Cache limpo');
        },
        reprocessAll: () => {
            processedElements.clear();
            processAllElements();
            console.log('[AutoMathRenderer] Reprocessando todos os elementos');
        }
    };

    console.log('[AutoMathRenderer] ✓ Inicializado com sucesso');
})();
